using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.UI.Bricks;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RasterProject;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class frmDataMoasic :
        //Form
        ToolWindow, ISmartToolWindow
    {
        private int _id;
        private ISmartSession _session = null;
        private EventHandler _onWindowClosed;
        private MoasicFileProvider _mosaicFileProvider;
        private string[] _invaildValues;
        private List<string> _invailFileInfo = new List<string>();
        private bool _processInvaild = false;
        private IProgressMonitor _progressMonitor;
        private CoordEnvelope _outCoordEnvelope = null;
        private string _outDir = string.Empty;
        private bool _isUpdateArgs = false;
        private int _reduceRes = 0;
        private ISimpleMapControl _simpleMapControl;
        private ISimpleVectorObjectHost _aoiHost;
        private ISimpleVectorObjectHost _curFileHost;

        public frmDataMoasic(ISmartSession session)
            : base()
        {
            InitializeComponent();
            _id = 9019;
            this.panelLeft.Width = 350;
            _progressMonitor = session.ProgressMonitorManager.DefaultProgressMonitor;
            _mosaicFileProvider = new MoasicFileProvider(new Action<int, string>(ChangeProgress));
            Init(session);
            CreateMapControl();
        }

        public int Id
        {
            get { return _id; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;        
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        private void CreateMapControl()
        {
            UCSimpleMapControl map = new UCSimpleMapControl();
            _simpleMapControl = map as ISimpleMapControl;
            map.Dock = DockStyle.Fill;
            this.panelRight.Controls.Add(map);
            map.AOIIsChanged += new EventHandler(map_AOIIsChanged);
            map.Load += new EventHandler(map_Load);
        }

        void map_Load(object sender, EventArgs e)
        {
            if (_simpleMapControl == null)
                return;
            _aoiHost = _simpleMapControl.CreateObjectHost("AOI");
            _curFileHost = _simpleMapControl.CreateObjectHost("CurFile");
        }

        void map_AOIIsChanged(object sender, EventArgs e)
        {
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope geoAoi = _simpleMapControl.DrawedAOI;
            if (OutGeoRangeControl.Enabled)
            {
                OutGeoRangeControl.MaxY = Math.Round(geoAoi.MaxY, 2);
                OutGeoRangeControl.MinY = Math.Round(geoAoi.MinY, 2);
                OutGeoRangeControl.MaxX = Math.Round(geoAoi.MaxX, 2);
                OutGeoRangeControl.MinX = Math.Round(geoAoi.MinX, 2);
                _outCoordEnvelope = new CoordEnvelope(geoAoi.MinX, geoAoi.MaxX, geoAoi.MinY, geoAoi.MaxY);
            }
            else
            {
                MsgBox.ShowInfo("已设定中心经度，不可更改输出范围！");
            }
        }

        void btnSaveFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = ".ldf";
            saveFileDialog1.Filter = "局地投影文件(*.ldf)|*.ldf";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                txtOutFileName.Text = saveFileDialog1.FileName;
        }

        private bool CheckFile(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return false;
            if (_mosaicFileProvider != null && _mosaicFileProvider.FileItems.Length != 0)
            {
                using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filename) as IRasterDataProvider)
                {
                    if (dataPrd == null)
                        return false;
                    if (dataPrd.BandCount != _mosaicFileProvider.FileItems[0].MainFile.BandCount)
                        return false;
                }
            }
            return true;
        }

        private void CheckDstFilename(string dir, string tempFilename)
        {
            if (string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(txtOutFileName.Text)))
                txtOutFileName.Text = Path.Combine(dir, tempFilename);
            else
                txtOutFileName.Text = Path.Combine(dir, Path.GetFileNameWithoutExtension(txtOutFileName.Text) + ".ldf");
        }
 
        private void ClearOutGeoInfos()
        {
            OutGeoRangeControl.MinX = 0;
            OutGeoRangeControl.MaxX = 0;
            OutGeoRangeControl.MinY = 0;
            OutGeoRangeControl.MaxY = 0;
            _outCoordEnvelope = null;
        }

        private void UpdateOutGeoRange()
        {
            if (_mosaicFileProvider != null && _mosaicFileProvider.FileItems.Length > 0)
            {
                OutGeoRangeControl.MaxX = Math.Round(_mosaicFileProvider.Envelope.MaxX, 2);
                OutGeoRangeControl.MinX = Math.Round(_mosaicFileProvider.Envelope.MinX, 2);
                OutGeoRangeControl.MaxY = Math.Round(_mosaicFileProvider.Envelope.MaxY, 2);
                OutGeoRangeControl.MinY = Math.Round(_mosaicFileProvider.Envelope.MinY, 2);
                _outCoordEnvelope = _mosaicFileProvider.Envelope;
            }
            else
            {
                ClearOutGeoInfos();
                _outCoordEnvelope = null;
            }
        }

        private void btnFullSelect_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in txtFileList.Items)
                li.Selected = true;
        }

        private void txtCenterLon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
                return;
            if (e.KeyChar == '.' && txtCenterLon.Text.IndexOf('.') == -1)
                return;
            if (!(e.KeyChar == '-') && !(e.KeyChar == '+') && !Char.IsDigit(e.KeyChar))
                e.Handled = true;
            float centerLon = 0;
            string strCenter = string.Empty;
            if (string.IsNullOrEmpty(txtCenterLon.SelectedText))
                strCenter = txtCenterLon.Text + e.KeyChar;
            else
                strCenter = txtCenterLon.Text.Replace(txtCenterLon.SelectedText, "") + e.KeyChar;
            float.TryParse(strCenter, out centerLon);
            if (centerLon > 180 || centerLon < -180)
            {
                MsgBox.ShowInfo("中心经度值应位于[-180~180]之间!");
                e.Handled = true;
            }
        }

        private void ckProcessInvaild_CheckedChanged(object sender, EventArgs e)
        {
            txtVaild.ReadOnly = !ckProcessInvaild.Checked;
            if (ckProcessInvaild.Checked)
            {
                labTip.Visible = true;
                txtVaild.Focus();
            }
            else
                labTip.Visible = false;
        }

        private void txtVaild_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
                return;
            if (e.KeyChar == '.' && txtCenterLon.Text.IndexOf('.') == -1)
                e.Handled = true;
            if (!(e.KeyChar == '-') && !(e.KeyChar == '+') && !(e.KeyChar == '，')
                && !(e.KeyChar == ',') && !Char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void ckResolution_CheckedChanged(object sender, EventArgs e)
        {
            txtResolution.ReadOnly = !ckResolution.Checked;
        }

        private void txtResolution_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
                return;
            if ((e.KeyChar == '.' && txtCenterLon.Text.IndexOf('.') == -1) || e.KeyChar == '-' || e.KeyChar == '+')
                e.Handled = true;
            if (!Char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOpenFiles_Click(object sender, EventArgs e)
        {
            StringBuilder msg = new StringBuilder();
            if (txtFileList.Items.Count == 0)
            {
                _isUpdateArgs = false;
            }
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                string filter = SupportedFileFilters.LdfFilter + "|"
                    + SupportedFileFilters.NoaaFilter + "|" + SupportedFileFilters.SrfFilterString + "|"
                    + SupportedFileFilters.ImageFilterString;
                dlg.Filter = filter;
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StartProgress();
                        IRasterDataProvider prd = null;
                        int i=0;
                        foreach (string filename in dlg.FileNames)
                        {
                            ChangeProgress((int)((i++ + 1.0) / dlg.FileNames.Length * 100), "添加文件并生成缩略图");
                            try
                            {
                                if (!CheckFile(filename))
                                {
                                    msg.AppendLine("添加文件失败：" + filename + "为空或波段数与已添加文件不相同！");
                                    continue;
                                }
                                prd = _mosaicFileProvider.Add(filename);
                                if (prd == null)
                                {
                                    msg.AppendLine("添加文件失败：" + "已添加文件：" + filename + "！");
                                    continue;
                                }
                                ListViewItem li = new ListViewItem(filename);
                                li.ToolTipText = filename;
                                li.Selected = true;
                                li.ImageIndex = 0;
                                li.Tag = prd;
                                txtFileList.Items.Add(li);
                                RefreshOverView();
                            }
                            catch (Exception ex)
                            {
                                if (prd != null)
                                {
                                    _mosaicFileProvider.Remove(prd);
                                    prd.Dispose();
                                }
                                msg.AppendLine("添加文件失败：" + filename + "[" + ex.Message + "]");
                            }
                        }
                        if (txtFileList.Items.Count != 0)
                            tabControl1.SelectedIndex = 1;
                        RefreshViewport();
                        RefreshCurFileEnv();
                        UpdateOutGeoRange();
                        SettTVInInfos();
                    }
                    catch (Exception ex)
                    {
                        msg.AppendLine(ex.Message);
                    }
                    finally
                    {
                        if (msg.Length != 0)
                            MsgBox.ShowInfo(msg.ToString());
                        FinishProgerss();
                    }
                    if (txtFileList.Items.Count > 0)
                    {
                        if (txtOutFileName.Text == "")
                        {
                            string oFileName = txtFileList.Items[0].Text;
                            string dirName = Path.GetDirectoryName(oFileName);
                            string fileName = Path.GetFileNameWithoutExtension(oFileName) + "_Moasic.ldf";
                            txtOutFileName.Text = Path.Combine(dirName, fileName);
                        }
                    }
                }  
            }   
        }

        private void SettTVInInfos()
        {
            tvInfo.Nodes.Clear();
            tvBands.Nodes.Clear();
            if (_mosaicFileProvider != null && _mosaicFileProvider.FileItems.Length > 0)
            {
                InitTVInInfos();
                tvInfo.Nodes[0].ExpandAll();
                tvBands.Nodes[0].ExpandAll();
            }
        }

        private void InitTVInInfos()
        {
            TreeNode tn = new TreeNode("拼接后文件属性");
            tvInfo.Nodes.Add(tn);
            using (IRasterDataProvider dataPrd = _mosaicFileProvider.FileItems[0].MainFile)
            {
                if (dataPrd != null)
                    tn = new TreeNode("投影类型");
                string projectType = string.Empty;
                GetProjectType(dataPrd.SpatialRef, out projectType);
                tn.Nodes.Add("投影方式: " + projectType);
                tvInfo.Nodes[0].Nodes.Add(tn);

                tn = new TreeNode("图像分辨率");
                if (dataPrd.ResolutionX != double.MinValue)
                    tn.Nodes.Add("经度方向: " + dataPrd.ResolutionX.ToString());
                if (dataPrd.ResolutionY != double.MinValue)
                    tn.Nodes.Add("纬度方向: " + dataPrd.ResolutionY.ToString());
                tvInfo.Nodes[0].Nodes.Add(tn);

                tn = new TreeNode("原始文件属性");
                tvInfo.Nodes.Add(tn);

                foreach (ListViewItem li in txtFileList.Items)
                {
                    if (li.Tag == null)
                        continue;
                    IRasterDataProvider args = li.Tag as IRasterDataProvider;
                    tn = new TreeNode(Path.GetFileName(args.fileName));
                    tn.Tag = args;
                    tvInfo.Nodes[1].Nodes.Add(tn);
                    TreeNode ctn = new TreeNode("图像行列");
                    if (args.Width != 0 && args.Height != 0)
                    {
                        ctn.Nodes.Add("宽度: " + args.Width);
                        ctn.Nodes.Add("高度: " + args.Height);
                    }
                    tn.Nodes.Add(ctn);
                    TreeNode rtn = new TreeNode("经纬度范围");
                    if (args.CoordEnvelope != null)
                    {
                        rtn.Nodes.Add("纬度：" + args.CoordEnvelope.MinX + "-" + args.CoordEnvelope.MaxX);
                        rtn.Nodes.Add("经度：" + args.CoordEnvelope.MinY + "-" + args.CoordEnvelope.MaxY);
                    }
                    tn.Nodes.Add(rtn);
                }

                TreeNode tnband = new TreeNode("选择的数据集或波段值");
                tnband.Checked = true;
                tvBands.Nodes.Add(tnband);
                if (dataPrd.BandCount != 0)
                {
                    for (int i = 1; i <= dataPrd.BandCount; i++)
                    {
                        TreeNode t = tnband.Nodes.Add("波段" + i);
                        t.Checked = true;
                    }
                }
            }
        }

        private void GetProjectType(Project.ISpatialReference spatialReference, out string projectType)
        {
            if (spatialReference == null)
                projectType = "等经纬度";
            else if (spatialReference.GeographicsCoordSystem == null)
                projectType = "";
            else if (spatialReference.ProjectionCoordSystem == null)
                projectType = "等经纬度";
            else
            {
                string projectName = spatialReference.ProjectionCoordSystem.Name.Name;
                switch (projectName)
                {
                    case "Polar Stereographic":
                        projectType = "极射赤面投影";
                        break;
                    case "Albers Conical Equal Area":
                        projectType = "阿尔伯斯等面积投影";
                        break;
                    case "Lambert Conformal Conic":
                        projectType = "兰伯托";
                        break;
                    case "Mercator":
                        projectType = "墨卡托";
                        break;
                    case "Hammer":
                        projectType = "Hammer";
                        break;
                    default:
                        projectType = "";
                        break;
                }
            }
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtFileList.Items == null || txtFileList.Items.Count == 0)
                {
                    MsgBox.ShowInfo("请至少选择一个待拼接文件!");
                    return;
                }
                int[] selectedBands = GetSelectedBands();
                if (selectedBands == null)
                {
                    MsgBox.ShowInfo("请至少选择一个拼接波段!");
                    return;
                }
                if (ckResolution.Checked)
                {
                    int reduceRes;
                    if (Int32.TryParse(txtResolution.Text, out reduceRes))
                        _reduceRes = reduceRes;
                }
                _processInvaild = ckProcessInvaild.Checked;
                _invaildValues = GetInvaildString(txtVaild.Text);
                string[] srcFileNames = GetArgsItems();
                _outDir = txtOutFileName.Text;
                string resultFileName = MoasicFiles(srcFileNames, _processInvaild, _invaildValues, _outDir, _outCoordEnvelope, selectedBands, _session.ProgressMonitorManager.DefaultProgressMonitor);
                OpenFileFactory.Open(resultFileName);
            }
            catch (Exception exception)
            {
                MsgBox.ShowError(exception.Message);
            }
        }

        private int[] GetSelectedBands()
        {
            List<int> bands=new List<int>();
            for(int i=1;i<= tvBands.Nodes[0].Nodes.Count;i++)
            {
                if (tvBands.Nodes[0].Nodes[i - 1].Checked)
                    bands.Add(i);
            }
            return bands.Count==0?null:bands.ToArray();
        }

        private string[] GetInvaildString(string validValues)
        {
            string[] invailds = validValues.Split(new char[] { ',','，' });
            return invailds;
        }

        private void ckCenterLon_CheckedChanged(object sender, EventArgs e)
        {
            OutGeoRangeControl.Enabled = !ckCenterLon.Checked;
            txtCenterLon.ReadOnly = !ckCenterLon.Checked;
            if (ckCenterLon.Checked)
                UpdateOutGeoRangeByCenterLon();
            else
            {
                ClearOutGeoInfos();
                GetArgsItems();
                if (_mosaicFileProvider.FileItems.Length >= 1)
                    InitOutGeoRangeControl(_mosaicFileProvider.Envelope);
            }
        }

        private string[] GetArgsItems()
        {
            List<string> fileNames = new List<string>();
            foreach (ListViewItem it in txtFileList.Items)
            {
                if (it.Text != null)
                    fileNames.Add(it.Text);
            }
            return fileNames.ToArray();
        }

        private void InitOutGeoRangeControl(CoordEnvelope outCorEnvelope)
        {
            if (outCorEnvelope != null)
            {
                CoordEnvelope envelope = outCorEnvelope;
                OutGeoRangeControl.MinX = Math.Round(envelope.MinX, 2);
                if (_isUpdateArgs)
                    OutGeoRangeControl.MaxX = Math.Round(envelope.MaxX - 360, 2);
                else
                    OutGeoRangeControl.MaxX = Math.Round(envelope.MaxX, 2);
                OutGeoRangeControl.MinY = Math.Round(envelope.MinY, 2);
                OutGeoRangeControl.MaxY = Math.Round(envelope.MaxY, 2);
                _outCoordEnvelope = outCorEnvelope;
            }
            else
            {
                ClearOutGeoInfos();
                _outCoordEnvelope = null;
            }
        }

        private void UpdateOutGeoRangeByCenterLon()
        {
            if (!string.IsNullOrEmpty(txtCenterLon.Text))
            {
                float lonMin = Convert.ToSingle(txtCenterLon.Text) - 180;
                OutGeoRangeControl.MinX = lonMin;
                OutGeoRangeControl.MaxX = lonMin;
                if (_outCoordEnvelope != null)
                {
                    double maxY = _outCoordEnvelope.MaxY;
                    double minY = _outCoordEnvelope.MinY;
                    _outCoordEnvelope = new CoordEnvelope(lonMin, lonMin, minY, maxY);
                }
            }
            else
            {
                ClearOutGeoInfos();
            }
        }

        private CoordEnvelope CalcOutRegion(List<string> moasicFileNames)
        {
            CoordEnvelope dstCoordEnvelope = null;
            try
            {
                foreach (string fileName in moasicFileNames)
                {
                    CheckFile(fileName);
                    using (IRasterDataProvider rasterprd = GeoDataDriver.Open(fileName) as IRasterDataProvider)
                    {
                        if (dstCoordEnvelope != null)
                            dstCoordEnvelope = dstCoordEnvelope.Union(rasterprd.CoordEnvelope);
                        else
                            dstCoordEnvelope = rasterprd.CoordEnvelope;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return dstCoordEnvelope;
        }

        private void txtCenterLon_TextChanged(object sender, EventArgs e)
        {
            if (ckCenterLon.Checked)
            {
                UpdateOutGeoRangeByCenterLon();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearOutGeoInfos();
            GetArgsItems();
            if (_mosaicFileProvider.FileItems.Length >= 1)
                InitOutGeoRangeControl(_mosaicFileProvider.Envelope);
        }

        private void RefreshViewport()
        {
            CoordEnvelope env = _mosaicFileProvider.Envelope;
            _simpleMapControl.ToViewport(new Core.DrawEngine.CoordEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY));
            _simpleMapControl.Render();
        }

        private void RefreshOverView()
        {
            _simpleMapControl.RemoveAllImageLayers();
            for (int i = 0; i < _mosaicFileProvider.FileItems.Length; i++)
            {
                if (_mosaicFileProvider.FileItems[i] != null)
                {
                    string filename = _mosaicFileProvider.FileItems[i].MainFile.fileName;
                    CoordEnvelope env = _mosaicFileProvider.FileItems[i].Envelope;
                    Bitmap bmp = _mosaicFileProvider.FileItems[i].OverViewBmp;
                    if (bmp != null && env != null)
                    {
                        Core.DrawEngine.CoordEnvelope envelope = new Core.DrawEngine.CoordEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
                        _simpleMapControl.AddImageLayer(filename, bmp, envelope, true);
                    }
                }
            }
            _simpleMapControl.Render();
        }

        private void RefreshCurFileEnv(int[] indexs)
        {   
            if (indexs == null || indexs.Length==0)
                return;
            ClearAoiVectorHost(_curFileHost);
            if(_mosaicFileProvider.FileItems == null || _mosaicFileProvider.FileItems.Length == 0)
                return;
            foreach(int index in indexs)
            {
               CoordEnvelope env = _mosaicFileProvider.FileItems[index].Envelope;
               string filename = Path.GetFileName(_mosaicFileProvider.FileItems[index].MainFile.fileName);
               AddAoiToVectorHost(_curFileHost, filename, env);
            }
            _simpleMapControl.Render();
        }

        private void RefreshCurFileEnv()
        {
            if (_mosaicFileProvider == null || _mosaicFileProvider.FileItems.Length == 0)
            {
                ClearAoiVectorHost(_curFileHost);
                _simpleMapControl.ToChinaViewport();
            }
            ListView.SelectedIndexCollection indexs = txtFileList.SelectedIndices;
            if (indexs != null && indexs.Count > 0)
            {
                int[] indexArray = new int[indexs.Count];
                int i = 0;
                foreach (int item in indexs)
                {
                    indexArray[i++] = item;
                }
                RefreshCurFileEnv(indexArray);
            }
            else
            {
                ClearAoiVectorHost(_curFileHost);
                foreach (MosaicItem item in _mosaicFileProvider.FileItems)
                {
                    CoordEnvelope env = item.Envelope;
                    string filename = Path.GetFileName(item.MainFile.fileName);
                    AddAoiToVectorHost(_curFileHost, filename, env);
                }
                _simpleMapControl.Render();
            }
        }

        private void ClearAoiVectorHost(ISimpleVectorObjectHost host)
        {
            host.Remove(new Func<ISimpleVectorObject, bool>((i) => { return true; }));
            _simpleMapControl.Render();
        }

        private void AddAoiToVectorHost(ISimpleVectorObjectHost host, string name,CoordEnvelope envelope)
        {
            Core.DrawEngine.CoordEnvelope env = new Core.DrawEngine.CoordEnvelope(envelope.MinX, envelope.MaxX, envelope.MinY, envelope.MaxY);
            host.Add(new SimpleVectorObject(name, env));
        }

        private string MoasicFiles(string[] srcFiles, bool processInvaild, string[] invaildValues, string outDir, CoordEnvelope outCoordEnvelope, int[] selectedBands, IProgressMonitor progress)
        {
            try
            {
                if (File.Exists(outDir))
                {
                    GetFilename(outDir);
                }
                int count = srcFiles.Length;
                IRasterDataProvider[] srcRaster = new IRasterDataProvider[count];
                for (int i = 0; i < count; i++)
                {
                    IRasterDataProvider src = GeoDataDriver.Open(srcFiles[i]) as IRasterDataProvider;
                    srcRaster[i] = src;
                }
                if (progress != null)
                {
                    progress.Reset("", 100);
                    progress.Start(false);
                }
                RasterMoasicProcesser processer = new RasterMoasicProcesser();
                IRasterDataProvider dstRaster = processer.Moasic(srcRaster, selectedBands, "LDF", outDir, outCoordEnvelope, srcRaster[0].ResolutionX, srcRaster[0].ResolutionY, processInvaild, invaildValues,
                    new Action<int, string>((int progerss, string text) =>
                    {
                        if (progress != null)
                            progress.Boost(progerss, text);
                    }));
                string dstFileName = dstRaster.fileName;
                for (int i = 0; i < count; i++)
                {
                    if(srcRaster[i]!=null)
                       srcRaster[i].Dispose();
                }
                dstRaster.Dispose();
                return dstFileName;
            }
            finally
            {
                if (progress != null)
                    progress.Finish();
            }
        }

        private void StartProgress()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() => { progressBar1.Value = 0; progressBar1.Visible = true; }));
            else
            {
                progressBar1.Value = 0;
                progressBar1.Visible = true;
            }
        }

        private void FinishProgerss()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    progressBar1.Visible = false;
                }));
            else
            {
                progressBar1.Visible = false;
            }
        }

        private void ChangeProgress(int progerss, string text)
        {
            progerss = progerss > 100 ? 100 : progerss;
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    progressBar1.Value = progerss;
                    progressBar1.Text = text;
                }));
            else
            {
                progressBar1.Value = progerss;
                progressBar1.Text = text;
            }
        }

        private void btnRemoveFiles_Click(object sender, EventArgs e)
        {
            if (txtFileList.SelectedItems == null || txtFileList.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem view in txtFileList.SelectedItems)
            {
                txtFileList.Items.Remove(view);
                IRasterDataProvider file = view.Tag as IRasterDataProvider;
                if (file != null)
                {
                    _mosaicFileProvider.Remove(file);
                    file.Dispose();
                }
            }
            RefreshOverView();
            RefreshCurFileEnv();
            _isUpdateArgs = true;
            UpdateOutGeoRange();
            ChangeOutFileName();
            SettTVInInfos();
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            if (_mosaicFileProvider != null && _mosaicFileProvider.FileItems.Length > 0)
            {
                OutGeoRangeControl.MaxX = Math.Round(_mosaicFileProvider.Envelope.MaxX, 2);
                OutGeoRangeControl.MinX = Math.Round(_mosaicFileProvider.Envelope.MinX, 2);
                OutGeoRangeControl.MaxY = Math.Round(_mosaicFileProvider.Envelope.MaxY, 2);
                OutGeoRangeControl.MinY = Math.Round(_mosaicFileProvider.Envelope.MinY, 2);
                _outCoordEnvelope = _mosaicFileProvider.Envelope;
            }
            else
                ClearOutGeoInfos();
        }

        private void tvBands_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                foreach (TreeNode node in e.Node.Nodes)
                {
                    node.Checked = e.Node.Checked;
                }
            }
        }

        private void ChangeOutFileName()
        {
            if (txtFileList.Items.Count == 0)
            {
                txtOutFileName.Text = "";
            }
            else
            {
                string oFileName = txtFileList.Items[0].Text;
                string dirName = Path.GetDirectoryName(oFileName);
                string fileName = Path.GetFileNameWithoutExtension(oFileName) + "_MOASIC.ldf";
                fileName = GetFilename(fileName);
                txtOutFileName.Text = Path.Combine(dirName, fileName);
            }
        }

        private string GetFilename(string filename)
        {
            if (File.Exists(filename))
            {
                string dir = Path.GetDirectoryName(filename);
                string filenameWithExt = Path.GetFileNameWithoutExtension(filename);
                string fileExt = Path.GetExtension(filename);
                int i = 1;
                string outFileNmae = Path.Combine(dir, filenameWithExt + "(" + i + ")" + fileExt);
                while (File.Exists(outFileNmae))
                    outFileNmae = Path.Combine(dir, filenameWithExt + "(" + i++ + ")" + fileExt);
                return outFileNmae;
            }
            else
                return filename;
        }
    }
}
