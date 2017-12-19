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

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class monitoringPage : UserControl
    {
        private ISimpleMapControl _simpleMapControl;
        private HdDataMonitorNotify _hdDataMonitor;
        private HdDataFilter _filter = null;
        //private string _monitorRootPath;
        //private string _projectionRootPath;
        //private string _mosaicRootPath;
        //private string _blockRootPath;

        private List<OverViewObject> _lstOverviews = new List<OverViewObject>();
        private List<BlockInfoItem> _recivedBlocks = new List<BlockInfoItem>();
        private List<MosaicInfo> _lstMosaicInfo = new List<MosaicInfo>();

        public monitoringPage()
        {
            InitializeComponent();
            this.Text = "首页";
            lbDate.Text = DateTime.Today.ToString("yyyy年MM月dd日");
            LoadMapViews();
        }

        #region Map
        private ISimpleVectorObjectHost _blockHost;

        private void LoadMapViews()
        {
            try
            {
                UCSimpleMapControl map = new UCSimpleMapControl();
                _simpleMapControl = map as ISimpleMapControl;
                _simpleMapControl.IsAllowAOI = false;
                map.Dock = DockStyle.Fill;
                mapPanel.Visible = true;
                mapPanel.Controls.Add(map);
                map.Load += new EventHandler(map_Load);
                map.MouseDoubleClickMap += new Action<MouseEventArgs, double, double>(map_MouseDoubleClickMap);
            }
            catch
            {
                MsgBox.ShowInfo("缩略图加载失败，暂时不能使用缩略图功能");
            }
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
            _simpleMapControl.ToWorldViewport();
            LoadFilter();
            InitDataMonitor();
        }

        private void AddAoi(ISimpleVectorObjectHost host, string name, Core.DrawEngine.CoordEnvelope env)
        {
            host.Add(new SimpleVectorObject(name, env));
        }

        private void ClearAoi(ISimpleVectorObjectHost host)
        {
            host.Remove(new Func<ISimpleVectorObject, bool>((i) => { return true; }));
        }

        #endregion

        private void LoadFilter()
        {
            dataFileterControl1.CheckedFilterChanged += new Action<HdDataFilter>(dataFileterControl1_CheckedFilterChanged);
            _filter = dataFileterControl1.CheckedFilter;
        }

        void dataFileterControl1_CheckedFilterChanged(HdDataFilter obj)
        {
            FilterBitMap();
            _simpleMapControl.Render();
        }

        private void InitDataMonitor()
        {
            try
            {
                _hdDataMonitor = new HdDataMonitorNotify();
                _hdDataMonitor.DataChanged += new Action<MonitorTask.ChangedType, dynamic[]>(_hdDataMonitor_DataChanged);
                _hdDataMonitor.MessageSend += new Action<string>(_hdDataMonitor_MessageSend);
                _hdDataMonitor.Start();
            }
            catch (Exception ex)
            {
                this.toolStripStatusLabel3.Text = "数据监控启动失败：" + ex.Message;
            }
        }

        void _hdDataMonitor_MessageSend(string obj)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() => { this.toolStripStatusLabel3.Text = obj; }));
            else
                this.toolStripStatusLabel3.Text = obj;
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
                            AddDataOverView(pInfo);
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
                            AddMosaicInfo(pInfo);
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
                            BlockInfo info = arg2[i];
                            if (info.mosaicInfo.dayOrNight != "D")
                                continue;
                            AddBlockInfo(info);
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

        private void AddMosaicInfo(MosaicInfo mosaicInfo)
        {
            _lstMosaicInfo.Add(mosaicInfo);
        }

        private void UpdateProjectionInfo()
        {
            this.lbFiveMini.Text = string.Format(" 截止{0}共收到5分钟段数据 {1}条", DateTime.Now.ToString("HH:mm:ss"), _lstOverviews.Count);
            notifyRadioButtonProjection.NotifyMessage = _lstOverviews.Count;

            FilterBitMap();
            _simpleMapControl.Render();
        }

        private void UpdateMosaicInfo()
        {
            lbMosaic.Text = string.Format(" 截止{0}共收到拼接数据 {1}条", DateTime.Now.ToString("HH:mm:ss"), _lstMosaicInfo.Count);
            notifyRadioButtonMosaic.NotifyMessage = _lstMosaicInfo.Count;
        }

        private void UpdateBlockInfo()
        {
            lbBlock.Text = string.Format(" 截止{0}共收到分幅数据 {1}条", DateTime.Now.ToString("HH:mm:ss"), _recivedBlocks.Count);
            notifyRadioButtonBlock.NotifyMessage = _recivedBlocks.Count;
            FilterBlock();
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
            finally
            {
            }
        }

        private void AddBlockInfo(BlockInfo blockItem)
        {
            if (blockItem == null)
                return;
            string path = DataPathHelper.DataServerBlockPath + blockItem.datapath;
            if (!File.Exists(path))
                return;
            BlockInfoItem item = BlockInfoItem.Create(blockItem);
            if (item == null)
                return;
            _recivedBlocks.Add(item);
        }

        private void FilterBlock()
        {
            try
            {
                ClearAoi(_blockHost);
                string blockType = getBlockType();
                for (int i = 0; i < _recivedBlocks.Count; i++)
                {
                    if (_recivedBlocks[i].BlockInfo.mosaicInfo == null)
                    {
                        toolStripStatusLabel3.Text = string.Format("未获取分幅[{0}]的白天晚上信息,将直接显示", _recivedBlocks[i].Name);
                        continue;
                    }
                    else if (_recivedBlocks[i].BlockInfo.mosaicInfo.dayOrNight != "D")
                        continue;
                    if (_recivedBlocks[i].BlockInfo.blockidentify != blockType)
                        continue;
                    if (this.InvokeRequired)
                        this.Invoke(new Action<BlockInfoItem>(AddBlock), _recivedBlocks[i]);
                    else
                        AddBlock(_recivedBlocks[i]);
                }
                _simpleMapControl.Render();
            }
            finally
            {
            }
        }

        private string getBlockType()
        {
            if (btnBlock005.Checked)
                return "005";
            else if (btnBlock010.Checked)
                return "010";
            else
                return "custom";
        }

        private void AddBlock(BlockInfoItem blockInfoItem)
        {
            string satellite = _filter.Satellite;
            string sensor = _filter.Sensor;
            if (blockInfoItem.BlockInfo.mosaicInfo.satellite != satellite ||
                blockInfoItem.BlockInfo.mosaicInfo.sensor != sensor)
                return;
            string name = blockInfoItem.Name;
            AddAoi(_blockHost, name, blockInfoItem.Envelope);
        }
        
        private void AddDataOverView(ProjectionInfo pInfo)
        {
            string thumbnailFile = Path.ChangeExtension(Path.Combine(DataPathHelper.ProjectionRootPath + pInfo.datapath), ".overview.png");
            PrjEnvelopeItem prjEnv = new PrjEnvelopeItem("", new PrjEnvelope(pInfo.minx, pInfo.maxx, pInfo.miny, pInfo.maxy));
            OverViewObject ov = OverViewObject.CreateOverViewObject(pInfo);
            if (ov == null)
                return;
            _lstOverviews.Add(ov);
        }

        private void AddOverView(OverViewObject ov)
        {
            string satellite = _filter.Satellite;
            string sensor = _filter.Sensor;
            if (ov.ProjectionInfo.orbitInfo.satellite != satellite ||
                ov.ProjectionInfo.orbitInfo.sensor != sensor)
                return;
            string overviewName = ov.OverviewName;
            _simpleMapControl.AddImageLayer(overviewName, ov.Overview, ov.Envelope, true);
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
                    if (block.BlockInfo.mosaicInfo == null)
                        continue;
                    else if (block.BlockInfo.mosaicInfo.dayOrNight != "D")
                        continue;
                    ToolStripMenuItem tool = new ToolStripMenuItem(Path.GetFileName(block.BlockInfo.datapath));
                    tool.Tag = block;
                    tool.Click += new EventHandler(tool_Click);
                    tools.Add(tool);
                    blockCount++;
                }
                blockMenu.DropDownItems.AddRange(tools.ToArray()); 
            }
            menu.Items.Add(new ToolStripLabel("当前位置的数据有："));
            menu.Items.Add(new ToolStripSeparator());
            projectionMenu.Text = "五分钟段投影数据(" + pjCount + ")";
            menu.Items.Add(projectionMenu);
            blockMenu.Text = "分幅数据(" + blockCount + ")";
            menu.Items.Add(blockMenu);
            menu.Items.Add(new ToolStripSeparator());
            menu.Show(_simpleMapControl as UCSimpleMapControl, mousePoint);
        }

        void tool_Click(object sender, EventArgs e)
        {
            ToolStripItem tool = sender as ToolStripItem;
            if(tool.Tag is BlockInfoItem)
            {
                BlockInfoItem item = tool.Tag as BlockInfoItem;
                string filename = Path.Combine(DataPathHelper.DataServerBlockPath, item.BlockInfo.datapath);
                OpenFile(filename);
            }
            else if (tool.Tag is OverViewObject)
            {
                OverViewObject item = tool.Tag as OverViewObject;
                string filename = Path.Combine(DataPathHelper.ProjectionRootPath, item.ProjectionInfo.datapath);
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
            FilterBlock();
        }

        private void btnBlock010_CheckedChanged(object sender, EventArgs e)
        {
            FilterBlock();
        }

        private void btnBlockCustom_CheckedChanged(object sender, EventArgs e)
        {
            FilterBlock();
        }

        public class OverViewObject
        {
            private static int _maxImgSize = 1024;
            
            public static OverViewObject CreateOverViewObject(ProjectionInfo pInfo)
            {
                OverViewObject ov = new monitoringPage.OverViewObject();
                ov.ProjectionInfo = pInfo;
                string pngfilename = Path.ChangeExtension(Path.Combine(DataPathHelper.ProjectionRootPath + pInfo.datapath), ".overview.png");
                PrjEnvelopeItem prjEnv = new PrjEnvelopeItem("", new PrjEnvelope(pInfo.minx, pInfo.maxx, pInfo.miny, pInfo.maxy));
                if (!File.Exists(pngfilename))
                    return null;
                Core.DrawEngine.CoordEnvelope env = CoordEnvelopeFromPrj(prjEnv);
                Bitmap bmp = LoadImage(pngfilename);
                if (bmp == null || bmp.Height * bmp.Width > 1000000)
                    return null;
                if (env.Height <= 0 || env.Width <= 0 || env.Height * env.Width > 1600)
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
                BlockInfoItem item = new BlockInfoItem();
                item.BlockInfo =info;
                item.Name = info.envname;
                item.Envelope = new Core.DrawEngine.CoordEnvelope(info.minx, info.maxx, info.miny, info.maxy);
                return item;
            }

            public Core.DrawEngine.CoordEnvelope Envelope;
            public BlockInfo BlockInfo;
            public string Name;
        }
    }
}
