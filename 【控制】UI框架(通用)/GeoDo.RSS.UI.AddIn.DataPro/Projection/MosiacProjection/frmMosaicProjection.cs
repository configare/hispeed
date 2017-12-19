using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.ProjectDefine;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.UI.Bricks;
using Telerik.WinControls.UI.Docking;
using Telerik.WinControls;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class frmMosaicProjection : UserControl, IDisposable
    {
        private const string OpenFileFilter = "L1级轨道数据(*.hdf)|*.hdf;|1bd文件(*.1bd)|*.1bd;|1b文件(*.1b)|*.1b;|1a5文件(*.1a5)|*.1a5;";
        private ISmartSession _smartSession;
        private ISpatialReference _dstSpatialRef = SpatialReference.GetDefault();
        private IRasterDataProvider[] _retFiles = null;
        private IProgressMonitor _progressMonitor;
        private MosaicProjectionFileProvider _mosaicProjectionFileProvider;
        private ISimpleMapControl _simpleMapControl;
        private ISimpleVectorObjectHost _aoiHost;
        private ISimpleVectorObjectHost _curFileHost;
        private bool _isDrawEnvelope = false;
        private bool _mapAoiChanging = false;
        private MosaicType _mosaicType = MosaicType.Mosaic;
        private Action<int, string> _progress;
        private string _projectConfigDir;

        public event EventHandler ProjectionFinished;

        public frmMosaicProjection(ISmartSession session)
        {
            InitializeComponent();
            this.Text = "L1数据拼接投影";
            if (this.DesignMode)
                return;
            _smartSession = session;
            _progressMonitor = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            _progress = new Action<int, string>(ChangeProgress);
            _mosaicProjectionFileProvider = new MosaicProjectionFileProvider();
            _mosaicProjectionFileProvider.DataIdentifyChanged += new Action<DataIdentify, IRasterDataProvider>(_mosaicProjectionFileProvider_DataIdentifyChanged);
            ucPrjEnvelopes1.OnEnvelopeChanged += new EventHandler(ucPrjEnvelopes1_OnEnvelopeChanged);
            LoadSpatialReference();
            groupBox1.Visible = false;
            LoadMapViews();
            tvInputFiles.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseDoubleClick);
            InitCheckMode();
            //根据配置文件设置投影输出路径
            GetConfigProjectDir();
            //读取本地目录是否存在角度选择文件
            LoadAngleFile();
        }

        private void LoadAngleFile()
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "AngleCheckedFile.txt";
            if (!File.Exists(filename))
                return;
            string[] checkedInfo = File.ReadAllLines(filename);
            if (checkedInfo == null || checkedInfo.Length == 0)
                return;
            string[] splitStr = null;
            foreach (string item in checkedInfo)
            {
                splitStr = item.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (splitStr == null || splitStr.Length != 2)
                    continue;
                switch (splitStr[0].ToLower())
                {
                    case "height":
                        chkHeight.Checked = bool.Parse(splitStr[1]);
                        break;
                    case "angle":
                        btnOutAngle.Checked = bool.Parse(splitStr[1]);
                        break;
                }
            }
        }

        private void GetConfigProjectDir()
        {
            //从投影配置文件读取投影设置位置
            bool isUsed;
            ProjectConfig config = new ProjectConfig();
            string outdir = config.GetConfigValue("ProjectDir");
            string usedValue = config.GetConfigValue("IsUsed");
            if (string.IsNullOrEmpty(outdir) || string.IsNullOrEmpty(usedValue) || bool.TryParse(usedValue, out isUsed) && !isUsed)
                return;
            else
                _projectConfigDir = outdir;
        }

        private void InitCheckMode()
        {
            SetCheckMode(0);
            ucPrjEnvelopes1.SetEnvelopeMode(0);
        }

        #region LoadMap
        private void LoadMapViews()
        {
            try
            {
                UCSimpleMapControl map = new UCSimpleMapControl();
                _simpleMapControl = map as ISimpleMapControl;
                map.AOIIsChanged += new EventHandler(map_AOIIsChanged);
                map.Dock = DockStyle.Fill;
                cvPanel.Visible = true;
                cvPanel.Controls.Add(map);            //
                map.Load += new EventHandler(map_Load);
            }
            catch
            {
                MsgBox.ShowInfo("缩略图加载失败，暂时不能使用缩略图功能");
            }
        }

        void map_AOIIsChanged(object sender, EventArgs e)
        {
            try
            {
                _mapAoiChanging = true;
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope geoAoi = _simpleMapControl.DrawedAOI;
                if (_dstSpatialRef.ProjectionCoordSystem != null)
                {
                    GeoDo.RSS.Core.DrawEngine.CoordEnvelope prjAoi = GeoToPrjEnv(geoAoi, _dstSpatialRef);
                    //ucPrjEnvelopes1.SetSpatialReference(_dstSpatialRef);
                    ucPrjEnvelopes1.SetPrjEnvelope(new PrjEnvelope(prjAoi.MinX, prjAoi.MaxX, prjAoi.MinY, prjAoi.MaxY));
                }
                else
                {
                    ucPrjEnvelopes1.SetPrjEnvelope(new PrjEnvelope(geoAoi.MinX, geoAoi.MaxX, geoAoi.MinY, geoAoi.MaxY));
                }
                SetCheckMode(1);
            }
            finally
            {
                _mapAoiChanging = false;
            }
        }

        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope GeoToPrjEnv(Core.DrawEngine.CoordEnvelope geoAoi, ISpatialReference dstSpatialRef)
        {
            IProjectionTransform transform = ProjectionTransformFactory.GetProjectionTransform(SpatialReference.GetDefault(), dstSpatialRef);
            double[] xs = new double[] { geoAoi.MinX, geoAoi.MaxX };
            double[] ys = new double[] { geoAoi.MinY, geoAoi.MaxY };
            transform.Transform(xs, ys);
            return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(Math.Min(xs[0], xs[1]), Math.Max(xs[0], xs[1]), Math.Min(ys[0], ys[1]), Math.Max(ys[0], ys[1]));
        }

        void map_Load(object sender, EventArgs e)
        {
            if (_simpleMapControl == null)
                return;
            _aoiHost = _simpleMapControl.CreateObjectHost("AOI");
            _curFileHost = _simpleMapControl.CreateObjectHost("CurFile");
        }
        #endregion

        void _mosaicProjectionFileProvider_DataIdentifyChanged(DataIdentify arg1, IRasterDataProvider prd)
        {
            prjArgsSelectBand1.SetArgs(prd);
            if (string.IsNullOrEmpty(_projectConfigDir))
                txtOutDir.Text = Path.GetDirectoryName(prd.fileName);
            else
                txtOutDir.Text = _projectConfigDir;
            float resolution = GetDefaultResolution(arg1);
            UpdatePrjenvelopeSet(resolution);
            UpdateOutputFileName();
        }

        private float GetDefaultResolution(DataIdentify identify)
        {
            float resolution = 0.01f;
            if (_dstSpatialRef == null || _dstSpatialRef.ProjectionCoordSystem == null)
            {
                if (identify.Sensor == "VISSR")
                    resolution = 0.05f;
                else
                    resolution = 0.01f;
            }
            else
            {
                if (identify.Sensor == "VISSR")
                    resolution = 5000f;
                else
                    resolution = 1000f;
            }
            return resolution;
        }

        private void UpdateOutputFileName()
        {
            if (_mosaicProjectionFileProvider.FileItems != null && _mosaicProjectionFileProvider.FileItems.Length != 0)
            {
                string filename = _mosaicProjectionFileProvider.FileItems[0].MainFile.fileName;
                string fileDir;
                if (string.IsNullOrEmpty(_projectConfigDir))
                    fileDir = Path.GetDirectoryName(filename) + @"\Prj\";
                else
                    fileDir = _projectConfigDir;
                //if (!Directory.Exists(fileDir))
                //    Directory.CreateDirectory(fileDir);
                if (rbBlocks.Checked || _mosaicType != MosaicType.Mosaic)
                {
                    txtOutDir.Text = fileDir;
                    return;
                }
                string blockName = "";
                if (rbAllFile.Checked)
                {
                    blockName = "GBAL";
                }
                else if (rbCenter.Checked)
                {
                    blockName = "AOI";
                }
                string satellite;
                string sensor; ;
                DateTime datetime;
                DataIdentify dataIdentify = _mosaicProjectionFileProvider.FileItems[0].MainFile.DataIdentify;
                if (string.IsNullOrWhiteSpace(dataIdentify.Satellite) || string.IsNullOrWhiteSpace(dataIdentify.Sensor) || dataIdentify.OrbitDateTime == DateTime.MinValue)
                {
                    RasterIdentify identify = new RasterIdentify(_mosaicProjectionFileProvider.FileItems[0].MainFile);
                    satellite = string.IsNullOrWhiteSpace(dataIdentify.Satellite) ? identify.Satellite : dataIdentify.Satellite;
                    sensor = string.IsNullOrWhiteSpace(dataIdentify.Sensor) ? identify.Sensor : dataIdentify.Sensor;
                    datetime = identify.OrbitDateTime.TimeOfDay == TimeSpan.Zero ? dataIdentify.OrbitDateTime : identify.OrbitDateTime;
                }
                else
                {
                    satellite = dataIdentify.Satellite;
                    sensor = dataIdentify.Sensor;
                    datetime = dataIdentify.OrbitDateTime;
                }
                float resolution = ucPrjEnvelopes1.ResolutionX;
                string outFilenameWithoutDir = PrjFileName.GetL1PrjFilenameWithOutDir(filename, satellite, sensor, datetime, _dstSpatialRef, blockName, resolution);
                string outfilename = Path.Combine(fileDir, outFilenameWithoutDir);
                txtOutDir.Text = outfilename;
            }
        }

        #region 设置投影方式
        private void LoadSpatialReference()
        {
            toolStripDropDownButton1.DropDownItems.Clear();
            ISpatialReference[] customSpatialReferences = SpatialReferenceSelection.CustomSpatialReferences;
            if (customSpatialReferences != null && customSpatialReferences.Length != 0)
            {
                List<string> otherPrjsBtnList = new List<string>();
                foreach (ISpatialReference prj in customSpatialReferences)
                {
                    ToolStripMenuItem btnPrj = new ToolStripMenuItem();
                    btnPrj.Text = prj.Name;
                    btnPrj.Image = imageList1.Images[2];
                    btnPrj.TextAlign = ContentAlignment.MiddleLeft;
                    btnPrj.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btnPrj.Tag = prj;
                    btnPrj.Click += new EventHandler(btnPrj_Click);
                    toolStripDropDownButton1.DropDownItems.Add(btnPrj);
                }
                SpatialReferenceChanged(customSpatialReferences[0]);
            }
            ToolStripMenuItem morePrj = new ToolStripMenuItem();
            morePrj.Text = "更多投影...";
            morePrj.Image = imageList1.Images[2];
            morePrj.TextAlign = ContentAlignment.MiddleLeft;
            morePrj.TextImageRelation = TextImageRelation.ImageBeforeText;
            morePrj.Click += new EventHandler(morePrj_Click);

            toolStripDropDownButton1.DropDownItems.Add(new ToolStripSeparator());
            toolStripDropDownButton1.DropDownItems.Add(morePrj);
        }

        void morePrj_Click(object sender, EventArgs e)
        {
            try
            {
                using (SpatialReferenceSelection frmSp = new SpatialReferenceSelection())
                {
                    if (frmSp.ShowDialog() == DialogResult.OK)
                    {
                        if (frmSp.SpatialReference != null)
                            SpatialReferenceChanged(frmSp.SpatialReference);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void btnPrj_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tool = sender as ToolStripMenuItem;
            SpatialReferenceChanged(tool.Tag as SpatialReference);
        }

        private void SpatialReferenceChanged(ISpatialReference spatialReference)
        {
            float newResolution = GetResolution(_dstSpatialRef, spatialReference);
            toolStripLabel1.Text = spatialReference.Name;
            _dstSpatialRef = spatialReference;
            ucPrjEnvelopes1.SetSpatialReference(_dstSpatialRef);
            UpdatePrjenvelopeSet(newResolution);
        }

        private float GetResolution(ISpatialReference oldSpatialRef, ISpatialReference newSpatialRef)
        {
            if (oldSpatialRef == null || oldSpatialRef.ProjectionCoordSystem == null)
            {
                if (newSpatialRef == null || newSpatialRef.ProjectionCoordSystem == null)
                    return ucPrjEnvelopes1.ResolutionX;
                else
                    return ucPrjEnvelopes1.ResolutionX * 100000;
            }
            else
            {
                if (newSpatialRef == null || newSpatialRef.ProjectionCoordSystem == null)
                    return ucPrjEnvelopes1.ResolutionX / 100000;
                else
                    return ucPrjEnvelopes1.ResolutionX;
            }
        }

        private void UpdatePrjenvelopeSet(float resolution)
        {
            if (resolution == 0f)
            {
                if (_dstSpatialRef == null || _dstSpatialRef.ProjectionCoordSystem == null)
                    resolution = 0.01f;
                else
                    resolution = 1000f;
            }
            if (_mosaicProjectionFileProvider.FileItems != null && _mosaicProjectionFileProvider.FileItems.Length != 0)
            {
                PrjEnvelope env = _mosaicProjectionFileProvider.FileItems[0].Envelope;
                Size size = env.GetSize(resolution, resolution);
                ucPrjEnvelopes1.SetValue(_dstSpatialRef, new PrjPoint(env.CenterX, env.CenterY), resolution, resolution, size);
            }
            else
            {
                ucPrjEnvelopes1.SetValue(_dstSpatialRef, null, resolution, resolution, new Size((int)resolution, (int)resolution));
            }
        }
        #endregion

        //0:整轨,1:中心点，2:分幅区域
        private void SetCheckMode(int index)
        {
            if (index == 0)
                rbAllFile.Checked = true;
            else if (index == 1)
                rbCenter.Checked = true;
            else if (index == 2)
                rbBlocks.Checked = true;
        }

        void ucPrjEnvelopes1_OnEnvelopeChanged(object sender, EventArgs e)
        {
            if (_mapAoiChanging)
                return;
            if (rbCenter.Checked)
            {
                if (_simpleMapControl != null)
                {
                    PrjEnvelope env = ucPrjEnvelopes1.PrjEnvelopes[0].PrjEnvelope;
                    if (_simpleMapControl.DrawedAOI != null)
                        _simpleMapControl.DrawedAOI = null;
                    if (_dstSpatialRef.ProjectionCoordSystem != null)
                    {
                        Core.DrawEngine.CoordEnvelope geoEnv = new Core.DrawEngine.CoordEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
                        GeoDo.RSS.Core.DrawEngine.CoordEnvelope prjEnv = GeoToPrjEnv(geoEnv, _dstSpatialRef);
                        _simpleMapControl.DrawedAOI = geoEnv;
                    }
                    else
                    {
                        _simpleMapControl.DrawedAOI = new Core.DrawEngine.CoordEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
                    }
                }
            }
            UpdateDrawAOI();
            UpdateOutputFileName();
        }

        private void comBoxEnvelopeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbAllFile.Checked)
            {
                ucPrjEnvelopes1.SetEnvelopeMode(0);
                if (_simpleMapControl != null)
                {
                    _simpleMapControl.DrawedAOI = null;
                }
            }
            else if (rbCenter.Checked)
            {
                ucPrjEnvelopes1.SetEnvelopeMode(1);
                if (_simpleMapControl != null)
                {
                    PrjEnvelope env = ucPrjEnvelopes1.PrjEnvelopes[0].PrjEnvelope;
                    if (_simpleMapControl.DrawedAOI != null)
                        _simpleMapControl.DrawedAOI = null;
                    _simpleMapControl.DrawedAOI = new Core.DrawEngine.CoordEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
                }
            }
            else if (rbBlocks.Checked)
            {
                ucPrjEnvelopes1.SetEnvelopeMode(2);
                if (_simpleMapControl != null)
                {
                    _simpleMapControl.DrawedAOI = null;
                }
            }
            UpdateDrawAOI();
            UpdateOutputFileName();
        }

        private void UpdateDrawAOI()
        {
            if (_aoiHost == null)
                return;
            try
            {
                ClearAoiVectorHost(_aoiHost);
                if (rbAllFile.Checked)
                {
                    PrjEnvelope prj = _mosaicProjectionFileProvider.ExtendEnvelope;
                    if (prj != null)
                        AddAoiToVectorHost(_aoiHost, new PrjEnvelopeItem("AOI", prj));
                }
                else if (rbBlocks.Checked)
                {
                    PrjEnvelopeItem[] prjEnvelopeItem = ucPrjEnvelopes1.PrjEnvelopes;
                    if (prjEnvelopeItem != null)
                    {
                        for (int i = 0; i < prjEnvelopeItem.Length; i++)
                        {
                            AddAoiToVectorHost(_aoiHost, prjEnvelopeItem[i]);
                        }
                    }
                    _simpleMapControl.Render();
                }
                else
                {
                    _simpleMapControl.Render();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddFile();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteFile();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFile();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (tvInputFiles.SelectedNode == null)
                return;
            TreeNode node = tvInputFiles.SelectedNode;
            int index = node.Index;
            if (index == 0)
                return;
            _mosaicProjectionFileProvider.MoveUp(index);
            tvInputFiles.Nodes.Remove(node);
            tvInputFiles.Nodes.Insert(index - 1, node);
            tvInputFiles.SelectedNode = node;
            RefreshOverView();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (tvInputFiles.SelectedNode == null)
                return;
            TreeNode node = tvInputFiles.SelectedNode;
            int index = node.Index;
            if (index == tvInputFiles.Nodes.Count - 1)
                return;
            _mosaicProjectionFileProvider.MoveDown(index);
            tvInputFiles.Nodes.Remove(node);
            tvInputFiles.Nodes.Insert(index + 1, node);
            tvInputFiles.SelectedNode = node;
            RefreshOverView();
        }

        private void btnOutputDir_Click(object sender, EventArgs e)
        {
            SetDir(txtOutDir);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            DoExecte();
        }

        private void RefreshOverView()
        {
            _simpleMapControl.RemoveAllImageLayers();
            for (int i = 0; i < _mosaicProjectionFileProvider.FileItems.Length; i++)
            {
                if (_mosaicProjectionFileProvider.FileItems[i] != null)
                {
                    string filename = _mosaicProjectionFileProvider.FileItems[i].MainFile.fileName;
                    PrjEnvelope prjEnv = _mosaicProjectionFileProvider.FileItems[i].Envelope;
                    Bitmap bmp = _mosaicProjectionFileProvider.FileItems[i].OverViewBmp;
                    if (bmp != null && prjEnv != null)
                    {
                        Core.DrawEngine.CoordEnvelope env = new Core.DrawEngine.CoordEnvelope(prjEnv.MinX, prjEnv.MaxX, prjEnv.MinY, prjEnv.MaxY);
                        _simpleMapControl.AddImageLayer(filename, bmp, env, true);
                    }
                }
            }
            _simpleMapControl.Render();
        }

        private void AddFile()
        {
            StringBuilder msg = new StringBuilder();
            string[] fileNames;
            OpenFileDialog("添加要投影的文件", OpenFileFilter, true, out fileNames);
            if (fileNames != null)
            {
                try
                {
                    StartProgress();
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        ChangeProgress((int)((i + 1.0) / fileNames.Length * 100), "添加文件并生成缩略图");
                        if (fileNames[i] != null)
                        {
                            string errorMsg = "";
                            IRasterDataProvider prd = null;
                            try
                            {
                                prd = _mosaicProjectionFileProvider.Add(fileNames[i], out errorMsg);
                                if (prd != null)
                                {
                                    TreeNode treeNode = new TreeNode(Path.GetFileName(prd.fileName));
                                    treeNode.ToolTipText = prd.fileName;
                                    treeNode.Tag = prd;
                                    tvInputFiles.Nodes.Add(treeNode);
                                    int index = _mosaicProjectionFileProvider.FileItems.Length - 1;
                                    RefreshOverView();
                                }
                                if (!string.IsNullOrWhiteSpace(errorMsg))
                                    msg.AppendLine("添加文件失败：" + fileNames[i] + "[" + errorMsg + "]");
                            }
                            catch (Exception ex)
                            {
                                if (prd != null)
                                {
                                    _mosaicProjectionFileProvider.Remove(prd);
                                    prd.Dispose();
                                }
                                msg.AppendLine("添加文件失败：" + fileNames[i] + "[" + ex.Message + "]");
                            }
                        }
                    }
                    RefreshViewport();
                    RefreshCurFileEnv();
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
            }
        }

        private void DeleteFile()
        {
            if (tvInputFiles.SelectedNode == null)
                return;
            TreeNode node = tvInputFiles.SelectedNode;
            tvInputFiles.Nodes.Remove(node);
            IRasterDataProvider file = node.Tag as IRasterDataProvider;
            if (file != null)
            {
                _mosaicProjectionFileProvider.Remove(file);
            }
            RefreshOverView();
            RefreshCurFileEnv();
        }

        private void ClearFile()
        {
            _mosaicProjectionFileProvider.Clear();
            tvInputFiles.Nodes.Clear();
            RefreshOverView();
            RefreshCurFileEnv();
        }

        private void OpenFileDialog(string title, string filter, bool multiselect, out string[] selectFilenames)
        {
            selectFilenames = null;
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = title;
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.Multiselect = multiselect;
                    if (!String.IsNullOrEmpty(filter))
                        openFileDialog.Filter = filter;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Application.DoEvents();
                        selectFilenames = openFileDialog.FileNames;
                    }
                }
            }
            catch
            {
                MsgBox.ShowInfo("文件不存在,或不可读取！");
            }
        }

        private void SetDir(TextBox txtDir)
        {
            bool isDir = true;
            if (string.IsNullOrWhiteSpace(txtDir.Text))
                isDir = true;
            else
                isDir = PrjFileName.IsDir(txtDir.Text);
            if (isDir)
            {
                using (FolderBrowserDialog dlg = new FolderBrowserDialog())
                {
                    if (txtDir.Text != "")
                        dlg.SelectedPath = txtDir.Text;
                    dlg.ShowNewFolderButton = true;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        txtDir.Text = dlg.SelectedPath;
                    }
                }
            }
            else
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    if (txtDir.Text != "")
                    {
                        dlg.FileName = Path.GetFileName(txtDir.Text);
                        dlg.DefaultExt = ".ldf";
                        dlg.Filter = "局地文件(*.ldf)|*.ldf";
                        dlg.InitialDirectory = Path.GetDirectoryName(txtDir.Text);
                        dlg.RestoreDirectory = true;
                    }
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        txtDir.Text = dlg.FileName;
                    }
                }
            }
        }

        private void DoExecte()
        {
            List<string> retFiles = new List<string>();
            try
            {
                StartProgress();
                string outDir = txtOutDir.Text;
                PrjEnvelopeItem[] prjEnvelopeItem;
                float resolutionX, resolutionY;
                GetMapInfo(out prjEnvelopeItem, out resolutionX, out resolutionY);
                string msg;
                List<object> argExs = new List<object>();
                if (!btnRadiation.Checked)
                    argExs.Add("NotRadiation");
                if (!btnSolarZenith.Checked)
                    argExs.Add("NotSolarZenith");
                if (chkSensorZenith.Checked)
                    argExs.Add("IsSensorZenith");//执行临边变暗订正
                if (btnOutAngle.Checked)
                {
                    argExs.Add("SensorAzimuth");
                    argExs.Add("SensorZenith");
                    argExs.Add("SolarAzimuth");
                    argExs.Add("SolarZenith");
                    //NOAA 特有
                    argExs.Add("SatelliteZenith");
                    argExs.Add("RelativeAzimuth");
                    //FY2  特有
                    argExs.Add("NOMSatelliteZenith");
                    argExs.Add("NOMSunGlintAngle");
                    argExs.Add("NOMSunZenith");
                }
                if (chkHeight.Checked)
                {
                    argExs.Add("ExtBands=Height;DEM");
                }
                PrjOutArg prjarg = new PrjOutArg(_dstSpatialRef, prjEnvelopeItem, resolutionX, resolutionY, outDir);
                prjarg.Args = argExs.ToArray();
                prjarg.SelectedBands = prjArgsSelectBand1.SelectedBandIndexs();
                _retFiles = _mosaicProjectionFileProvider.DoProject(prjarg, _mosaicType, _progress, out msg);
                if (_retFiles == null && !string.IsNullOrWhiteSpace(msg))
                    UpdateMessage(msg);
            }
            catch (Exception ex)
            {
                UpdateMessage(ex.Message);
            }
            finally
            {
                FinishProgerss();
                if (_retFiles != null)
                {
                    for (int i = 0; i < _retFiles.Length; i++)
                    {
                        if (_retFiles[i] != null)
                        {
                            string fileName = _retFiles[i].fileName;
                            retFiles.Add(fileName);
                            _retFiles[i].Dispose();
                            _retFiles[i] = null;
                        }
                    }
                    _retFiles = null;
                }
            }
            OpenFiles(retFiles.ToArray());
        }

        private void OpenFiles(string[] files)
        {
            if (files == null || files.Length == 0)
                return;
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i];
                if (!string.IsNullOrWhiteSpace(fileName))
                    OpenFileFactory.Open(fileName);
            }
            if (ProjectionFinished != null)
                ProjectionFinished(this, null);
        }

        #region 进度条
        private void StartProgress()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { toolStripProgressBar1.Value = 0; toolStripProgressBar1.Visible = true; }));
            }
            else
            {
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Visible = true;
            }
        }

        private void FinishProgerss()
        {
            //if (_progressMonitor != null)
            //    _progressMonitor.Finish();
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    toolStripProgressBar1.Visible = false;
                }));
            else
            {
                toolStripProgressBar1.Visible = false;
            }
        }

        private void ChangeProgress(int progerss, string text)
        {
            progerss = progerss > 100 ? 100 : progerss;
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    toolStripProgressBar1.Value = progerss;
                    UpdateMessage(text);
                    statusStrip1.ResumeLayout();
                }));
            else
            {
                toolStripProgressBar1.Value = progerss;
                UpdateMessage(text);
                statusStrip1.ResumeLayout();
            }
        }

        private void UpdateMessage(string message)
        {
            lbMessage.Tag = message;
            if (message != null && message.Length != 0)
            {
                int ind = message.IndexOf("\r\n");
                if (ind > 0)
                    message = message.Substring(0, ind);
            }
            lbMessage.Text = message;
        }
        #endregion

        private void GetMapInfo(out PrjEnvelopeItem[] prjEnvelopeItem, out float resolutionX, out float resolutionY)
        {
            prjEnvelopeItem = null;
            resolutionX = 0;
            resolutionY = 0;
            if (rbAllFile.Checked)
            {
                resolutionX = ucPrjEnvelopes1.ResolutionX;
                resolutionY = ucPrjEnvelopes1.ResolutionY;
                prjEnvelopeItem = null;
            }
            else if (rbCenter.Checked)
            {
                resolutionX = ucPrjEnvelopes1.ResolutionX;
                resolutionY = ucPrjEnvelopes1.ResolutionY;
                if (_dstSpatialRef == null || _dstSpatialRef.ProjectionCoordSystem == null)
                {
                    prjEnvelopeItem = ucPrjEnvelopes1.PrjEnvelopes;
                }
                else
                {
                    PrjEnvelope env = GetPrjEnvelopeFromCenter(resolutionX, resolutionY);
                    prjEnvelopeItem = new PrjEnvelopeItem[] { new PrjEnvelopeItem("AOI", env) };
                }
            }
            else if (rbBlocks.Checked)//自定义分幅
            {
                resolutionX = ucPrjEnvelopes1.ResolutionX;
                resolutionY = ucPrjEnvelopes1.ResolutionY;
                if (_dstSpatialRef == null || _dstSpatialRef.ProjectionCoordSystem == null)
                {
                    prjEnvelopeItem = ucPrjEnvelopes1.PrjEnvelopes;
                }
                else
                {
                    PrjEnvelopeItem[] geoItems = ucPrjEnvelopes1.PrjEnvelopes;
                    prjEnvelopeItem = new PrjEnvelopeItem[geoItems.Length];
                    IProjectionTransform tran = ProjectionTransformFactory.GetProjectionTransform(SpatialReference.GetDefault(), _dstSpatialRef);
                    for (int i = 0; i < geoItems.Length; i++)
                    {
                        double[] xs = new double[] { geoItems[i].PrjEnvelope.LeftTop.X, geoItems[i].PrjEnvelope.RightBottom.X };
                        double[] ys = new double[] { geoItems[i].PrjEnvelope.LeftTop.Y, geoItems[i].PrjEnvelope.RightBottom.Y };
                        tran.Transform(xs, ys);
                        double minx = Math.Min(xs[0], xs[1]);
                        double maxx = Math.Max(xs[0], xs[1]);
                        double miny = Math.Min(ys[0], ys[1]);
                        double maxy = Math.Max(ys[0], ys[1]);
                        PrjEnvelope env = new PrjEnvelope(minx, maxx, miny, maxy);
                        prjEnvelopeItem[i] = new PrjEnvelopeItem(geoItems[i].Name, env);
                    }
                }
            }
        }

        private PrjEnvelope GetPrjEnvelopeFromCenter(float resolutionX, float resolutionY)
        {
            Size size = ucPrjEnvelopes1.PixelSize;
            PrjPoint center = ucPrjEnvelopes1.CenterLongLat;
            double[] xs = new double[] { center.X };
            double[] ys = new double[] { center.Y };
            using (IProjectionTransform tran = ProjectionTransformFactory.GetProjectionTransform(SpatialReference.GetDefault(), _dstSpatialRef))
            {
                tran.Transform(xs, ys);
            }
            PrjEnvelope env = PrjEnvelope.CreateByCenter(xs[0], ys[0], resolutionX * size.Width, resolutionY * size.Height);
            return env;
        }

        public void Free()
        {
            (this as IDisposable).Dispose();
        }

        void IDisposable.Dispose()
        {
            if (_mosaicProjectionFileProvider != null)
            {
                _mosaicProjectionFileProvider.DataIdentifyChanged -= new Action<DataIdentify, IRasterDataProvider>(_mosaicProjectionFileProvider_DataIdentifyChanged);
                _mosaicProjectionFileProvider.Dispose();
                _mosaicProjectionFileProvider.Clear();
            }
            if (_curFileHost != null)
            {
                _curFileHost.Dispose();
                _curFileHost = null;
            }
            if (_aoiHost != null)
            {
                _aoiHost.Dispose();
                _aoiHost = null;
            }
            if (_simpleMapControl != null)
            {
                (_simpleMapControl as UCSimpleMapControl).AOIIsChanged -= new EventHandler(map_AOIIsChanged);
                (_simpleMapControl as UCSimpleMapControl).Dispose();
                _simpleMapControl = null;
            }
            ucPrjEnvelopes1.OnEnvelopeChanged -= new EventHandler(ucPrjEnvelopes1_OnEnvelopeChanged);
            tvInputFiles.NodeMouseDoubleClick -= new TreeNodeMouseClickEventHandler(treeView1_NodeMouseDoubleClick);
            _progress = null;
        }

        private void btnBandRGBSet_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
        }

        private void btnSmallBmp_Click(object sender, EventArgs e)
        {
            try
            {
            }
            finally
            {
                groupBox1.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
            }
            finally
            {
                groupBox1.Visible = false;
            }
        }

        private void tvInputFiles_MouseUp(object sender, MouseEventArgs e)
        {
            ClearAoiVectorHost(_curFileHost);
        }

        private void tvInputFiles_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = tvInputFiles.GetNodeAt(e.Location);
            RefreshCurFileEnv(node);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //RefreshCurFileEnv();
        }

        private void RefreshCurFileEnv(TreeNode node)
        {
            if (node == null || _mosaicProjectionFileProvider.FileItems == null || _mosaicProjectionFileProvider.FileItems.Length == 0)
            {
                ClearAoiVectorHost(_curFileHost);
                return;
            }
            int index = node.Index;
            PrjEnvelope prjEnv = _mosaicProjectionFileProvider.FileItems[index].Envelope;
            string filename = Path.GetFileName(_mosaicProjectionFileProvider.FileItems[index].MainFile.fileName);
            ClearAoiVectorHost(_curFileHost);
            AddAoiToVectorHost(_curFileHost, new PrjEnvelopeItem(filename, prjEnv));
            _simpleMapControl.Render();
        }

        private void RefreshCurFileEnv()
        {
            TreeNode node = tvInputFiles.SelectedNode;
            RefreshCurFileEnv(node);
        }

        private void RefreshViewport()
        {
            PrjEnvelope prjEnv = _mosaicProjectionFileProvider.ExtendEnvelope;
            if (prjEnv != null)
            {
                _simpleMapControl.ToViewport(new Core.DrawEngine.CoordEnvelope(prjEnv.MinX, prjEnv.MaxX, prjEnv.MinY, prjEnv.MaxY));
            }
            _simpleMapControl.Render();
        }

        void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            if (node == null)
                return;
            int index = node.Index;
            PrjEnvelope prjEnv = _mosaicProjectionFileProvider.FileItems[index].Envelope;
            //comBoxEnvelopeType.SelectedIndex = 1;
        }

        private void AddAoiToVectorHost(ISimpleVectorObjectHost host, PrjEnvelopeItem prjItem)
        {
            PrjEnvelope prjEnv = prjItem.PrjEnvelope;
            Core.DrawEngine.CoordEnvelope env = new Core.DrawEngine.CoordEnvelope(prjEnv.MinX, prjEnv.MaxX, prjEnv.MinY, prjEnv.MaxY);
            host.Add(new SimpleVectorObject(prjItem.Name, env));
        }

        private void ClearAoiVectorHost(ISimpleVectorObjectHost host)
        {
            host.Remove(new Func<ISimpleVectorObject, bool>((i) => { return true; }));
            _simpleMapControl.Render();
        }

        private void btnDrawEnvelope_Click(object sender, EventArgs e)
        {
            _isDrawEnvelope = !_isDrawEnvelope;
            _simpleMapControl.IsAllowPanMap = !_isDrawEnvelope;
        }

        private void btnRadiation_Click(object sender, EventArgs e)
        {
            if (!btnRadiation.Checked)
                btnSolarZenith.Checked = false;
        }

        private void btnSolarZenith_Click(object sender, EventArgs e)
        {
            if (btnSolarZenith.Checked)
                btnRadiation.Checked = true;
        }

        /// <summary>
        /// 设置是否进行结果拼接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripDropDownButton2_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string type = e.ClickedItem.Text;
            toolStripLabel2.Text = type;
            switch (type)
            {
                case "始终拼接":
                    _mosaicType = MosaicType.Mosaic;
                    break;
                case "从不拼接":
                    _mosaicType = MosaicType.NoMosaic;
                    break;
                case "日期拼接":
                    _mosaicType = MosaicType.MosaicByDay;
                    break;
                default:
                    _mosaicType = MosaicType.Mosaic;
                    break;
            }
            UpdateOutputFileName();
        }

        private void lbMessage_DoubleClick(object sender, EventArgs e)
        {
            string message = lbMessage.Tag as string;
            if (string.IsNullOrWhiteSpace(message))
                MsgBox.ShowInfo(message);
        }
    }

    public enum MosaicType
    {
        Mosaic,
        NoMosaic,
        MosaicByDay
    }
}
