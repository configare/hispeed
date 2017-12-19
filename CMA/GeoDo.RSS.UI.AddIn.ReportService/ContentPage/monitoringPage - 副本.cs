using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.UI.Bricks;
using GeoDo.RSS.Core.UI;
using GeoDo.RasterProject;
using GeoDo.FileProject;
using System.IO;
using GeoDo.RSS.UI.AddIn.HdService.HdDataServer;
using System.Threading;
using System.Configuration;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.Core.DrawEngine;
using Telerik.WinControls.UI;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class monitoringPage : UserControl
    {
        private ISimpleMapControl _simpleMapControl;
        private HdDataMonitorNotify _hdDataMonitor;
        private HdDataFilter _filter = null;

        private List<OverViewObject> _lstOverviews = new List<OverViewObject>();
        private List<BlockInfoItem> _recivedBlocks = new List<BlockInfoItem>();
        private List<MosaicInfoItem> _lstMosaicInfo = new List<MosaicInfoItem>();
        private const string Block005 = "005";
        private const string Block010 = "010";
        private List<string> _files5 = new List<string>();
        private List<string> _files10 = new List<string>();
        private ISimpleVectorObjectHost _blockHost;
        private Color _blockColor = Color.Red;
        private Color _fontColor = Color.Yellow;

        public monitoringPage()
        {
            InitializeComponent();
            this.Text = "首页";
            lbDate.Text = DateTime.Today.ToString("yyyy年MM月dd日");
            InitBlockGroup();
            LoadMapViews();
            InitTreeView();
        }

        private void InitBlockGroup()
        {
            List<BlockItemGroup> blockDef = new List<BlockItemGroup>();
            cbCustom.Items.Add("无");
            blockDef.Add(new BlockItemGroup("无", "", ""));
            DefinedRegionParse defineRegion = new DefinedRegionParse();
            BlockDefined blockDefined = defineRegion.BlockDefined;
            if (blockDefined == null || blockDefined.BlockItemGroups == null || blockDefined.BlockItemGroups.Length == 0)
                return;
            foreach (BlockItemGroup group in blockDefined.BlockItemGroups)
            {
                if (group.Name == "005" || group.Name == "010")
                    continue;
                cbCustom.Items.Add(string.IsNullOrEmpty(group.Description) ? group.Name : group.Description);
                blockDef.Add(group);
            }
            cbCustom.Tag = blockDef;
            cbCustom.SelectedIndex = 0;
        }

        #region Map

        private void LoadMapViews()
        {
            try
            {
                UCSimpleMapControl map = new UCSimpleMapControl();
                _simpleMapControl = map as ISimpleMapControl;
                _simpleMapControl.IsAllowAOI = false;
                _simpleMapControl.IsAllowSelect = true;
                map.Dock = DockStyle.Fill;
                mapPanel.Visible = true;
                mapPanel.Controls.Add(map);
                map.Load += new EventHandler(map_Load);
                map.MouseDoubleClickMap += new Action<MouseEventArgs, double, double>(map_MouseDoubleClickMap);
                map.MapSelectedIsChanged += new Action<object, CoordEnvelope>(map_MapSelectedIsChanged);
            }
            catch
            {
                MsgBox.ShowInfo("缩略图加载失败，暂时不能使用缩略图功能");
            }
        }

        void map_MapSelectedIsChanged(object sender, CoordEnvelope geoEnvelope)
        {
            if (geoEnvelope == null)
                return;
            pnlSelectedContanier.Visible = true;
            UpdateSelectedFiles(geoEnvelope);
        }

        void map_MouseDoubleClickMap(MouseEventArgs arg1, double arg2, double arg3)
        {
            Point location = arg1.Location;
            ShowLocationMenu(arg2, arg3, location);
        }

        void map_Load(object sender, EventArgs e)
        {
            if (_simpleMapControl == null)
                return;
            _blockHost = _simpleMapControl.CreateObjectHost("Block");
            _blockHost.LabelSetting.ForeColor = _fontColor;
            _blockHost.LabelSetting.MasLabelRuler = CodeCell.AgileMap.Core.masLabelPosition.Center;
            ((CodeCell.AgileMap.Core.FillSymbol)(_blockHost.Symbol)).OutlineSymbol.Color = Color.Red;
            LoadFilter();
            InitDataMonitor(false);
        }

        private void AddAoi(ISimpleVectorObjectHost host, string name, Core.DrawEngine.CoordEnvelope env)
        {
            ISimpleVectorObject sv = new SimpleVectorObject(name, env);

            host.Add(sv);
        }

        private void ClearAoi(ISimpleVectorObjectHost host)
        {
            host.Remove(new Func<ISimpleVectorObject, bool>((i) => { return true; }));
        }

        #endregion

        #region 选中文件批量打开
        private RadTreeView _treeView = null;
        private List<BlockInfoItem> _selectedBlockFiles = new List<BlockInfoItem>();
        private FilterField[] _blockFilter = new FilterField[] { 
            new FilterField() { Name = "005", Description = "5度分幅", FilterType = "" },
            new FilterField() { Name = "010", Description = "10度分幅", FilterType = "" }
        };

        private void InitTreeView()
        {
            _treeView = new RadTreeView();
            _treeView.Dock = DockStyle.Fill;
            pnlSelectedTree.Controls.Add(_treeView);
            pnlSelectedContanier.Visible = false;
        }

        private void btnHideSelectPanel_Click(object sender, EventArgs e)
        {
            pnlSelectedContanier.Visible = false;
        }

        private void btnDownloadSelectedFiles_Click(object sender, EventArgs e)
        {

        }

        private void UpdateSelectedFiles(CoordEnvelope geoEnvelope)
        {
            _selectedBlockFiles.Clear();
            foreach (BlockInfoItem item in _recivedBlocks)
            {
                if (geoEnvelope.Contains(item.Envelope))
                {
                    _selectedBlockFiles.Add(item);
                }
            }
            UpdateTreeView();
        }

        /// <summary>
        /// 节点第一级是分幅
        /// 第二级是卫星传感器
        /// 第三级是数据
        /// </summary>
        private void UpdateTreeView()
        {
            Dictionary<string, List<string>> blockFiles = new Dictionary<string, List<string>>();
            for (int i = 0; i < _selectedBlockFiles.Count; i++)
            {
                BlockInfoItem item = _selectedBlockFiles[i];
                if (item.Satalite != _filter.Satellite || item.Sensor != _filter.Sensor)
                    continue;
                string key = item.BlockInfo.blockidentify;
                string file = DataPathHelper.DataServerBlockPath + item.BlockInfo.datapath;
                if (blockFiles.ContainsKey(key))
                    blockFiles[key].Add(file);
                else
                    blockFiles.Add(key, new List<string>(new string[] { file }));
            }
            _treeView.Nodes.Clear();
            RadTreeNode node = new RadTreeNode(_filter.Text, true);
            List<RadTreeNode> subNodes = new List<RadTreeNode>();
            foreach (FilterField blockFilter in _blockFilter)
            {
                RadTreeNode blockNode = new RadTreeNode(blockFilter.Description);
                blockNode.CheckType = CheckType.CheckBox;
                blockNode.Checked = true;
                if (blockFiles.ContainsKey(blockFilter.Name))
                {
                    blockNode.Tag = blockFiles[blockFilter.Name].ToArray();
                    blockNode.Text += "(选中" + blockFiles[blockFilter.Name].Count + "条)";
                }
                else
                    blockNode.Text += "(选中0条)";
                node.Nodes.Add(blockNode);
            }
            _treeView.Nodes.Add(node);
        }

        private bool Filter(string satellite, string sensor)
        {
            return (satellite != _filter.Satellite || sensor != _filter.Sensor);
        }

        private void btnOpenSelectedFiles_Click(object sender, EventArgs e)
        {
            string[] filesSelected = GetCheckededFiles();
            if (filesSelected.Length == 0)
                return;
            int fileOpenCount = filesSelected.Length;
            if (fileOpenCount > 20)
            {
                if (MsgBox.ShowQuestionYesNo("要打开的文件过多(>20)，点确认打开前20个，点取消重新选择文件") == DialogResult.OK)
                    fileOpenCount = 20;
                else
                    return;
            }
            for (int i = 0; i < fileOpenCount; i++)
            {
                OpenFileFactory.Open(filesSelected[i]);
            }
        }

        private string[] GetCheckededFiles()
        {
            List<string> filesSelected = new List<string>();
            foreach (RadTreeNode node in _treeView.Nodes)
            {
                foreach (RadTreeNode subnode in node.Nodes)
                {
                    if (subnode.Checked)
                    {
                        string[] files = subnode.Tag as string[];
                        if (files != null)
                            filesSelected.AddRange(files);
                    }
                }
            }
            return filesSelected.ToArray();
        }
        #endregion

        private void LoadFilter()
        {
            dataFileterControl1.CheckedFilterChanged += new Action<HdDataFilter>(dataFileterControl1_CheckedFilterChanged);
            _filter = dataFileterControl1.CheckedFilter;
        }

        void dataFileterControl1_CheckedFilterChanged(HdDataFilter obj)
        {
            FilterChanged();
        }

        private void FilterChanged()
        {
            FilterBitMap();
            FilterBlockView();
            _simpleMapControl.Render();
        }

        private void InitDataMonitor(bool immediately)
        {
            try
            {
                _hdDataMonitor = new HdDataMonitorNotify(DateTime.Parse(dtSearchTime.Value.ToShortDateString()).AddHours(-8), DateTime.Parse(dtSearchTime.Value.AddDays(1).ToShortDateString()).AddSeconds(-1).AddHours(-8));
                _hdDataMonitor.DataChanged += new Action<MonitorTask.ChangedType, dynamic[]>(_hdDataMonitor_DataChanged);
                _hdDataMonitor.MessageSend += new Action<string>(_hdDataMonitor_MessageSend);
                if (immediately)
                    _hdDataMonitor.ImmediatelyStart();
                else
                    _hdDataMonitor.Start();
            }
            catch (Exception ex)
            {
                this.toolStripStatusLabel3.Text = "数据监控启动失败：" + ex.Message;
            }
        }

        void _hdDataMonitor_MessageSend(string obj)
        {
            try
            {
                if (obj != null && obj.Length != 0)
                {
                    int ind = obj.IndexOf("\r\n");
                    if (ind > 0)
                        obj = obj.Substring(0, ind);
                }
                if (this.InvokeRequired)
                    this.Invoke(new Action<string>((val) => { this.toolStripStatusLabel3.Text = val; }), obj);
                else
                    this.toolStripStatusLabel3.Text = obj;
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("_hdDataMonitor_MessageSend:" + ex.Message);
            }
        }

        void _hdDataMonitor_DataChanged(MonitorTask.ChangedType arg1, dynamic[] arg2)
        {
            try
            {
                if (arg2 == null || arg2.Length == 0)
                    return;
                if (arg1 == MonitorTask.ChangedType.Add)
                {
                    if (arg2[0] is HdDataServer.ProjectionInfo)
                    {
                        Progress(0);
                        for (int i = 0; i < arg2.Length; i++)
                        {
                            Progress((i + 1) * 100 / arg2.Length);
                            ProjectionInfo pInfo = arg2[i];
                            if (pInfo.orbitInfo.dayOrNight != "D")
                                continue;
                            AddDataOverViewItem(pInfo);
                        }
                        if (this.InvokeRequired)
                            this.Invoke(new Action(UpdateProjectionInfo));
                        else
                            UpdateProjectionInfo();
                    }
                    else if (arg2[0] is HdDataServer.MosaicInfo)
                    {
                        for (int i = 0; i < arg2.Length; i++)
                        {
                            Progress((i + 1) * 100 / arg2.Length);
                            MosaicInfo pInfo = arg2[i];
                            if (pInfo.dayOrNight != "D")
                                continue;
                            AddMosaicInfoItem(pInfo);
                        }
                        if (this.InvokeRequired)
                            this.Invoke(new Action(UpdateMosaicInfo));
                        else
                            UpdateMosaicInfo();
                    }
                    else if (arg2[0] is HdDataServer.BlockInfo)
                    {
                        for (int i = 0; i < arg2.Length; i++)
                        {
                            if (i == 367)
                                i = 367;
                            BlockInfo info = arg2[i];
                            //不显示夜间分块数据
                            if (info.mosaicInfo != null && info.mosaicInfo.dayOrNight != "D")
                                continue;
                            AddBlockInfoItem(info);
                        }
                        if (this.InvokeRequired)
                            this.Invoke(new Action(UpdateBlockInfo));
                        else
                            UpdateBlockInfo();
                    }
                }
            }
            finally
            {
                Progress(0);
            }
        }

        private void AddMosaicInfoItem(MosaicInfo mosaicInfo)
        {
            if (mosaicInfo == null)
                return;
            string path = DataPathHelper.DataServerMosaicPath + mosaicInfo.datapath;
            if (!File.Exists(path))
                return;
            MosaicInfoItem item = MosaicInfoItem.Create(mosaicInfo);
            if (item == null)
                return;
            foreach (MosaicInfoItem match in _lstMosaicInfo)
            {
                if (match.MosaicInfo.datapath == item.MosaicInfo.datapath)
                    return;
            }
            _lstMosaicInfo.Add(item);
        }

        private void UpdateProjectionInfo()
        {
            try
            {
                this.lbFiveMini.Text = string.Format(" 截止{0}共收到轨道数据 {1}条", dtSearchTime.Value.ToShortDateString() == DateTime.Now.ToShortDateString() ? DateTime.Now.ToString("HH:mm:ss") : "23:59:59", _lstOverviews.Count);
                notifyRadioButtonProjection.NotifyMessage = _lstOverviews.Count;
                FilterBitMap();
                _simpleMapControl.Render();
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("UpdateProjectionInfo:" + ex.Message);
            }
        }

        private void UpdateMosaicInfo()
        {
            try
            {
                lbMosaic.Text = string.Format(" 截止{0}共收到拼接数据 {1}条", dtSearchTime.Value.ToShortDateString() == DateTime.Now.ToShortDateString() ? DateTime.Now.ToString("HH:mm:ss") : "23:59:59", _lstMosaicInfo.Count);
                notifyRadioButtonMosaic.NotifyMessage = _lstMosaicInfo.Count;
                FilterMosaic();
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("UpdateMosaicInfo:" + ex.Message);
            }
        }

        private void FilterMosaic()
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("FilterMosaic:" + ex.Message);
            }
            finally
            {
            }
        }

        private void UpdateBlockInfo()
        {
            try
            {
                lbBlock.Text = string.Format(" 截止{0}共收到分幅数据 {1}条", dtSearchTime.Value.ToShortDateString() == DateTime.Now.ToShortDateString() ? DateTime.Now.ToString("HH:mm:ss") : "23:59:59", _recivedBlocks.Count);
                notifyRadioButtonBlock.NotifyMessage = _recivedBlocks.Count;
                FilterBlockView();
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("UpdateBlockInfo:" + ex.Message);
            }
        }

        private void FilterBitMap()
        {
            _filter = dataFileterControl1.CheckedFilter;
            try
            {
                _simpleMapControl.RemoveAllImageLayers();
                for (int i = 0; i < _lstOverviews.Count; i++)
                {
                    ProjectionInfo pInfo = _lstOverviews[i].ProjectionInfo;
                    if (pInfo.orbitInfo.dayOrNight == "D")
                    {
                        if (this.InvokeRequired)
                            this.Invoke(new Action<OverViewObject>(AddOverView), _lstOverviews[i]);
                        else
                            AddOverView(_lstOverviews[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("FilterBitMap:" + ex.Message);
            }
            finally
            {
            }
        }

        private void AddBlockInfoItem(BlockInfo blockItem)
        {
            if (blockItem == null)
                return;
            string path = DataPathHelper.DataServerBlockPath + blockItem.datapath;
            //if (path.Contains("NOAA18") || path.Contains("NA18"))
            //    path = path.Replace("T:", "D:");
            if (!File.Exists(path))
                return;
            BlockInfoItem item = BlockInfoItem.Create(blockItem);
            if (item == null)
                return;
            //查找是否已经加载同文件名的数据
            foreach (BlockInfoItem match in _recivedBlocks)
            {
                if (match.BlockInfo.datapath == item.BlockInfo.datapath)
                    return;
            }
            _recivedBlocks.Add(item);
        }

        private void FilterBlockView()
        {
            try
            {
                ClearAoi(_blockHost);
                string blockType = getBlockType();
                List<string> existsBlockEvname = new List<string>();
                for (int i = 0; i < _recivedBlocks.Count; i++)
                {
                    //if (_recivedBlocks[i].BlockInfo.mosaicInfo == null)
                    //{
                    //    toolStripStatusLabel3.Text = string.Format("未获取分幅[{0}]的白天晚上信息,将直接显示", _recivedBlocks[i].Name);
                    //    continue;
                    //}
                    //不显示夜间分块数据
                    //else if (_recivedBlocks[i].BlockInfo.mosaicInfo != null && _recivedBlocks[i].BlockInfo.mosaicInfo.dayOrNight != "D")
                    if (_recivedBlocks[i].BlockInfo.mosaicInfo != null && _recivedBlocks[i].BlockInfo.mosaicInfo.dayOrNight != "D")
                        continue;
                    if (_recivedBlocks[i].BlockInfo.blockidentify != blockType)
                        continue;
                    if (_recivedBlocks[i].Satalite != _filter.Satellite
                        || _recivedBlocks[i].Sensor != _filter.Sensor)
                        continue;
                    if (existsBlockEvname.IndexOf(_recivedBlocks[i].BlockInfo.envname) != -1)
                        continue;
                    existsBlockEvname.Add(_recivedBlocks[i].BlockInfo.envname);
                    if (this.InvokeRequired)
                        this.Invoke(new Action<BlockInfoItem>(AddBlock), _recivedBlocks[i]);
                    else
                        AddBlock(_recivedBlocks[i]);
                }
                _simpleMapControl.Render();
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("FilterBlock:" + ex.Message);
            }
            finally
            {
            }
            List<BlockInfoItem> itmes = new List<BlockInfoItem>();
            foreach (BlockInfoItem item in _recivedBlocks)
            {
                if (item.BlockInfo.blockidentify == "DBU" || item.BlockInfo.envname == "DBU")
                    item.BlockInfo.envname = "DBU";
                if (item.BlockInfo.envname == "0FEG")
                    itmes.Add(item);
            }
        }

        private string getBlockType()
        {
            if (btnBlock005.Checked)
                return "005";
            else if (btnBlock010.Checked)
                return "010";
            else if (btnBlockCustom.Checked)
            {
                List<BlockItemGroup> groups = cbCustom.Tag as List<BlockItemGroup>;
                if (groups == null)
                    return "custom";
                BlockItemGroup group = groups[cbCustom.SelectedIndex];
                return string.IsNullOrEmpty(group.Identify) ? group.Name : group.Identify;
            }
            return "custom";
        }

        private void AddBlock(BlockInfoItem blockInfoItem)
        {
            string satellite = _filter.Satellite;
            string sensor = _filter.Sensor;
            if (blockInfoItem.BlockInfo.mosaicInfo != null && (blockInfoItem.BlockInfo.mosaicInfo.satellite != satellite ||
                blockInfoItem.BlockInfo.mosaicInfo.sensor != sensor))
                return;
            else if (blockInfoItem.BlockInfo.mosaicInfo == null && (blockInfoItem.Satalite != satellite || blockInfoItem.Sensor != sensor))
                return;
            string name = blockInfoItem.Name;
            AddAoi(_blockHost, name, blockInfoItem.Envelope);
        }

        private void AddDataOverViewItem(ProjectionInfo pInfo)
        {
            string thumbnailFile = Path.ChangeExtension(Path.Combine(DataPathHelper.ProjectionRootPath + pInfo.datapath), ".overview.png");
            PrjEnvelopeItem prjEnv = new PrjEnvelopeItem("",
                new PrjEnvelope(pInfo.minx.GetValueOrDefault(), pInfo.maxx.GetValueOrDefault(), pInfo.miny.GetValueOrDefault(), pInfo.maxy.GetValueOrDefault()));
            OverViewObject item = OverViewObject.CreateOverViewObject(pInfo);
            if (item == null)
                return;
            foreach (OverViewObject match in _lstOverviews)
            {
                if (match.ProjectionInfo.datapath == item.ProjectionInfo.datapath)
                    return;
            }
            _lstOverviews.Add(item);
        }

        private void AddOverView(OverViewObject ov)
        {
            string satellite = _filter.Satellite;
            //if (satellite == "NA18")
            //{
            //    satellite = "NOAA18";
            //}
            string sensor = _filter.Sensor;
            string ovsat = ov.ProjectionInfo.orbitInfo.satellite;
            //if (ovsat == "NA18")
            //{
            //    ovsat = "NOAA18";
            //}
            if (ovsat != satellite ||
                ov.ProjectionInfo.orbitInfo.sensor != sensor)
                return;
            string overviewName = ov.OverviewName;
            _simpleMapControl.AddImageLayer(overviewName, ov.Overview, ov.Envelope, true);
        }

        private MosaicInfoItem[] GetMosaicInfoItem(double lon, double lat)
        {
            if (_lstMosaicInfo.Count == 0)
                return null;
            List<MosaicInfoItem> locationItems = new List<MosaicInfoItem>();
            for (int i = 0; i < _lstMosaicInfo.Count; i++)
            {
                PrjEnvelope env = new PrjEnvelope(_lstMosaicInfo[i].Envelope.MinX, _lstMosaicInfo[i].Envelope.MaxX, _lstMosaicInfo[i].Envelope.MinY, _lstMosaicInfo[i].Envelope.MaxY);
                if (env.Contains(lon, lat))
                    locationItems.Add(_lstMosaicInfo[i]);
            }
            return locationItems.ToArray();
        }

        private BlockInfoItem[] GetLocationBlockItem(double lon, double lat)
        {
            if (_recivedBlocks.Count == 0)
                return null;
            List<BlockInfoItem> locationItems = new List<BlockInfoItem>();
            for (int i = 0; i < _recivedBlocks.Count; i++)
            {
                PrjEnvelope env = new PrjEnvelope(_recivedBlocks[i].Envelope.MinX, _recivedBlocks[i].Envelope.MaxX, _recivedBlocks[i].Envelope.MinY, _recivedBlocks[i].Envelope.MaxY);
                if (env.Contains(lon, lat))
                    locationItems.Add(_recivedBlocks[i]);
            }
            return locationItems.ToArray();
        }

        private OverViewObject[] GetLocationProjectionInfoItem(double lon, double lat)
        {
            if (_lstOverviews.Count == 0)
                return null;
            List<OverViewObject> locationItems = new List<OverViewObject>();
            for (int i = 0; i < _lstOverviews.Count; i++)
            {
                PrjEnvelope env = new PrjEnvelope(_lstOverviews[i].Envelope.MinX, _lstOverviews[i].Envelope.MaxX, _lstOverviews[i].Envelope.MinY, _lstOverviews[i].Envelope.MaxY);
                if (env.Contains(lon, lat))
                    locationItems.Add(_lstOverviews[i]);
            }
            return locationItems.ToArray();
        }

        private void ShowLocationMenu(double lon, double lat, Point mousePoint)
        {
            BlockInfoItem[] blockItems = GetLocationBlockItem(lon, lat);
            OverViewObject[] overviewItems = GetLocationProjectionInfoItem(lon, lat);

            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem projectionMenu = new ToolStripMenuItem();
            ToolStripMenuItem blockMenu = new ToolStripMenuItem();
            int pjCount = 0;
            if (overviewItems != null && overviewItems.Length != 0)
            {
                List<ToolStripItem> tools = new List<ToolStripItem>();
                foreach (OverViewObject item in overviewItems)
                {
                    if (Filter(item.ProjectionInfo.orbitInfo.satellite, item.ProjectionInfo.orbitInfo.sensor))
                        continue;
                    ToolStripMenuItem tool = new ToolStripMenuItem(Path.GetFileName(item.ProjectionInfo.datapath));
                    tool.Tag = item;
                    tool.Click += new EventHandler(tool_Click);
                    tools.Add(tool);
                    pjCount++;
                }
                projectionMenu.DropDownItems.AddRange(tools.ToArray());
            }
            int blockCount = 0;
            if (blockItems != null && blockItems.Length != 0)
            {
                var items = blockItems.OrderBy((ov) => { return ov.BlockInfo.blockidentify; });
                List<ToolStripItem> tools = new List<ToolStripItem>();
                string blockType = getBlockType();
                foreach (BlockInfoItem block in items)
                {
                    if (block == null || block.BlockInfo.blockidentify != blockType)
                        continue;
                    //if (block.BlockInfo.mosaicInfo == null)
                    //    continue;
                    //不显示夜间分块数据
                    //else if (block.BlockInfo.mosaicInfo.dayOrNight != "D")
                    if (block.BlockInfo.mosaicInfo != null && block.BlockInfo.mosaicInfo.dayOrNight != "D")
                        continue;
                    if (Filter(block.Satalite, block.Sensor))
                        continue;
                    ToolStripMenuItem tool = new ToolStripMenuItem(Path.GetFileName(block.BlockInfo.datapath));
                    tool.Tag = block;
                    tool.Click += new EventHandler(tool_Click);
                    tools.Add(tool);
                    blockCount++;
                }
                blockMenu.DropDownItems.AddRange(tools.ToArray());
            }

            MosaicInfoItem[] mosaicItems = GetMosaicInfoItem(lon, lat);
            ToolStripMenuItem mosaicMenu = new ToolStripMenuItem();
            int mosaicCount = 0;
            if (mosaicItems != null && mosaicItems.Length != 0)
            {
                List<ToolStripItem> tools = new List<ToolStripItem>();
                foreach (MosaicInfoItem item in mosaicItems)
                {
                    if (Filter(item.MosaicInfo.satellite, item.MosaicInfo.sensor))
                        continue;
                    ToolStripMenuItem tool = new ToolStripMenuItem(Path.GetFileName(item.MosaicInfo.datapath));
                    tool.Tag = item;
                    tool.Click += new EventHandler(tool_Click);
                    tools.Add(tool);
                    mosaicCount++;
                }
                mosaicMenu.DropDownItems.AddRange(tools.ToArray());
            }

            menu.Items.Add(new ToolStripLabel("当前位置的数据有："));
            menu.Items.Add(new ToolStripSeparator());
            projectionMenu.Text = "轨道投影数据(" + pjCount + ")";
            menu.Items.Add(projectionMenu);
            blockMenu.Text = "分幅数据(" + blockCount + ")";
            menu.Items.Add(blockMenu);
            menu.Items.Add(new ToolStripSeparator());
            mosaicMenu.Text = "拼接数据(" + mosaicCount + ")";
            menu.Items.Add(mosaicMenu);
            menu.Items.Add(new ToolStripSeparator());
            menu.Show(_simpleMapControl as UCSimpleMapControl, mousePoint);
        }

        void tool_Click(object sender, EventArgs e)
        {
            ToolStripItem tool = sender as ToolStripItem;
            if (tool.Tag is BlockInfoItem)
            {
                BlockInfoItem item = tool.Tag as BlockInfoItem;
                string filename = (DataPathHelper.DataServerBlockPath + item.BlockInfo.datapath);
                OpenFile(filename);
            }
            else if (tool.Tag is OverViewObject)
            {
                OverViewObject item = tool.Tag as OverViewObject;
                string filename = DataPathHelper.ProjectionRootPath + item.ProjectionInfo.datapath;
                OpenFile(filename);
            }
            if (tool.Tag is MosaicInfoItem)
            {
                MosaicInfoItem item = tool.Tag as MosaicInfoItem;
                string filename = (DataPathHelper.DataServerMosaicPath + item.MosaicInfo.datapath);
                OpenFile(filename);
            }
        }

        private void OpenFile(string filename)
        {
            if (!File.Exists(filename))
                MsgBox.ShowInfo("文件不存在或无法访问" + filename);
            else
                OpenFileFactory.Open(filename);
        }

        private void Progress(int value)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<int>((val) => { progressBar1.Value = val; }), value);
            else
                progressBar1.Value = value;
        }

        private void RemoveOverView(string pngfilename)
        {
            string overviewName = Path.GetFileNameWithoutExtension(pngfilename);
            _simpleMapControl.RemoveImageLayer(overviewName);
        }

        private void btnBlock005_CheckedChanged(object sender, EventArgs e)
        {
            FilterBlockView();
        }

        private void btnBlock010_CheckedChanged(object sender, EventArgs e)
        {
            FilterBlockView();
        }

        private void btnBlockCustom_CheckedChanged(object sender, EventArgs e)
        {
            cbCustom.Enabled = btnBlockCustom.Checked;
        }

        public class OverViewObject
        {
            private static int _maxImgSize = 1024;

            public static OverViewObject CreateOverViewObject(ProjectionInfo pInfo)
            {
                OverViewObject ov = new monitoringPage.OverViewObject();
                ov.ProjectionInfo = pInfo;
                string pngfilename = Path.ChangeExtension(Path.Combine(DataPathHelper.ProjectionRootPath + pInfo.datapath), ".overview.png");
                PrjEnvelopeItem prjEnv = new PrjEnvelopeItem("",
                    new PrjEnvelope(pInfo.minx.GetValueOrDefault(), pInfo.maxx.GetValueOrDefault(), pInfo.miny.GetValueOrDefault(), pInfo.maxy.GetValueOrDefault()));
                //if (pngfilename.Contains("NOAA18") || pngfilename.Contains("NA18"))
                //    pngfilename = pngfilename.Replace("T:", "D:");
                if (!File.Exists(pngfilename))
                    return null;
                Core.DrawEngine.CoordEnvelope env = CoordEnvelopeFromPrj(prjEnv);
                Bitmap bmp = LoadImage(pngfilename);
                if (bmp == null || bmp.Height * bmp.Width > 1000000)
                    return null;
                if (env.Height <= 0 || env.Width <= 0 || env.Height * env.Width > 16000)
                    return null;
                ov.OverviewName = Path.GetFileNameWithoutExtension(pngfilename);
                ov.ProjectionInfo = pInfo;
                ov.Envelope = env;
                ov.Overview = bmp;
                return ov;
            }

            public string OverviewName;
            public ProjectionInfo ProjectionInfo;
            public Bitmap Overview;
            public Core.DrawEngine.CoordEnvelope Envelope;

            private static Bitmap LoadImage(string filePath)
            {
                try
                {
                    Bitmap bmp;
                    using (Bitmap load = new Bitmap(filePath))
                    {
                        int max = Math.Max(load.Width, load.Height);
                        if (_maxImgSize >= max)
                        {
                            bmp = new Bitmap(load, load.Width, load.Height);
                        }
                        else
                        {
                            double sc = _maxImgSize * 1.0f / max;
                            bmp = new Bitmap(load, (int)(load.Width * sc), (int)(load.Height * sc));
                        }
                    }
                    return bmp;
                }
                catch (Exception ex)
                {
                    LogFactory.WriteLine("LoadImage:" + ex.Message);
                    return null;
                }
            }

            private static Core.DrawEngine.CoordEnvelope CoordEnvelopeFromPrj(PrjEnvelopeItem prjEnv)
            {
                return new Core.DrawEngine.CoordEnvelope(prjEnv.PrjEnvelope.MinX, prjEnv.PrjEnvelope.MaxX, prjEnv.PrjEnvelope.MinY, prjEnv.PrjEnvelope.MaxY);
            }
        }

        public class BlockInfoItem
        {
            public static BlockInfoItem Create(BlockInfo info)
            {
                if (info == null)
                    return null;
                if (string.IsNullOrWhiteSpace(info.datapath))
                    return null;
                string fullfilename = DataPathHelper.DataServerBlockPath + info.datapath;
                if (!File.Exists(fullfilename))
                    return null;
                BlockInfoItem item = new BlockInfoItem();
                item.BlockInfo = info;
                item.Name = info.envname;
                item.Envelope = new CoordEnvelope(info.minx.GetValueOrDefault(), info.maxx.GetValueOrDefault(), info.miny.GetValueOrDefault(), info.maxy.GetValueOrDefault());
                if (info.mosaicInfo != null)
                {
                    item.Satalite = info.mosaicInfo.satellite;
                    item.Sensor = info.mosaicInfo == null ? "" : info.mosaicInfo.sensor;
                }
                else
                {
                    item.Satalite = info.projectionInfo.orbitInfo.satellite;
                    item.Sensor = info.projectionInfo.orbitInfo.sensor;
                }
                return item;
            }

            public Core.DrawEngine.CoordEnvelope Envelope;
            public BlockInfo BlockInfo;
            public string Name = "";
            public string Satalite = "";
            public string Sensor = "";
        }

        public class MosaicInfoItem
        {
            public static MosaicInfoItem Create(MosaicInfo info)
            {
                if (info == null)
                    return null;
                if (string.IsNullOrWhiteSpace(info.datapath))
                    return null;
                string fullfilename = DataPathHelper.DataServerMosaicPath + info.datapath;
                if (!File.Exists(fullfilename))
                    return null;
                MosaicInfoItem item = new MosaicInfoItem();
                item.MosaicInfo = info;
                item.Name = info.envname;
                item.Envelope = new CoordEnvelope(info.minx.GetValueOrDefault(), info.maxx.GetValueOrDefault(), info.miny.GetValueOrDefault(), info.maxy.GetValueOrDefault());
                return item;
            }

            public Core.DrawEngine.CoordEnvelope Envelope;
            public MosaicInfo MosaicInfo;
            public string Name;
        }

        public class FilterField
        {
            public string FilterType;
            public string Name;
            public string Description;
        }

        private void cbCustom_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterBlockView();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_hdDataMonitor != null)
                _hdDataMonitor.Dispose();
            _lstMosaicInfo.Clear();
            _lstOverviews.Clear();
            _recivedBlocks.Clear();
            FilterBitMap();
            _simpleMapControl.Render();
            InitDataMonitor(true);
            FilterBlockView();
        }
    }
}
