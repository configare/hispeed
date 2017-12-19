using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.HDF4;
using GeoDo.FileProject;
using System.Threading;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class frmMod06DataPro : Form
    {
        private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
        private string _path = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD";// + 
        private string _xml = "MOD06DataProcess.xml";
        private List<PrjEnvelopeItem> _envList = new List<PrjEnvelopeItem>();
        //string[] _dataset5kmList = new string[] { "Cloud_Top_Pressure","Cloud_Top_Pressure_Day","Cloud_Top_Pressure_Night", "Cloud_Top_Temperature","Cloud_Top_Temperature_Day","Cloud_Top_Temperature_Night", "Cloud_Fraction","Cloud_Fraction_Day","Cloud_Fraction_Night", "Cloud_Phase_Infrared","Cloud_Phase_Infrared_Day","Cloud_Phase_Infrared_Night"};
        //string[] _dataset1kmList = new string[] { "Cloud_Optical_Thickness", "Cloud_Effective_Radius", "Cloud_Water_Path" };
        //string[] _datasetAIRS = new string[] { "emisIRStd", "CldFrcTot", "H2OMMRStd", "TAirStd" ,"totH2OStd","totCldH2OStd"};
        private string _frmMode = "MOD06";
        private string _fileFilter = "MOD06_L2*.HDF";
        private string[] _selectedNode = new string[] { };
        private Action<int, string> _state = null;
        ConnectMySqlCloud _dbcon;
        Thread runTaskThread;
        private string _outputDir = null;

        public frmMod06DataPro()
        {
            InitializeComponent();
            InitTask();
        }

        public frmMod06DataPro(string frmMode)
        {
            InitializeComponent();
            _frmMode = frmMode;
            InitTask();
        }

        private void InvokeProgress(int p, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string>((arg1, arg2) =>
                {
                    toolStripStatusLabel1.Text = arg2.ToString();
                    LogFactory.WriteLine(string.Format("{0}预处理", _frmMode), arg2.ToString());
                    if (arg1 == -5)
                    {
                        this.Activate();
                        DialogResult it = MessageBox.Show(arg2.ToString() + "\r\n！", "异常提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        if (it != DialogResult.OK)
                        {
                            runTaskThread.Abort();
                            this.Close();
                        }
                    }
                    else if (arg1 == -100)
                    {
                        this.Activate();
                        DialogResult it = MessageBox.Show(arg2.ToString() + "\r\n！", "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (it == DialogResult.OK)
                        {
                            this.progressBar.Value = 0;
                            runTaskThread.Abort();
                            this.btnOk.Enabled = true;
                            toolStripStatusLabel1.Text = "预处理取消！";
                            LogFactory.WriteLine(string.Format("{0}预处理", _frmMode), "预处理取消！");
                        }
                    }
                    else if (arg1 != -1 && arg1 <= 100)
                    {
                        this.progressBar.Value = arg1;
                        if(arg1==100|| arg1 == 0)
                            this.btnOk.Enabled=true;
                    }
                }), p, text);
            }
            else
            {
                toolStripStatusLabel1.Text = text.ToString();
                LogFactory.WriteLine(string.Format("{0}预处理", _frmMode), text.ToString());
                if (p == -5)
                {
                    this.Activate();
                    DialogResult it = MessageBox.Show(text.ToString() + "\r\n请确认网络或磁盘可用！", "异常提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (it != DialogResult.OK)
                    {
                        runTaskThread.Abort();
                        this.Close();
                    }
                }
                else if (p == -100)
                {
                    this.Activate();
                    DialogResult it = MessageBox.Show(text.ToString() + "\r\n！", "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (it == DialogResult.OK)
                    {
                        this.progressBar.Value = 0;
                        runTaskThread.Abort();
                        this.btnOk.Enabled = true;
                        toolStripStatusLabel1.Text = "预处理取消！";
                        LogFactory.WriteLine(string.Format("{0}预处理", _frmMode), "预处理取消！");
                    }
                }
                else if (p != -1 && p <= 100)
                {
                    this.progressBar.Value = p;
                    if (p == 100 || p == 0)
                        this.btnOk.Enabled = true;
                }
            }
        }

        #region 初始化参数及界面
        private void InitTask()
        {
            CheckFrmMode();
            Load += new EventHandler(frmMod06DataPro_Load);
            lstRegions.SelectedIndexChanged += new EventHandler(lstRegionsSelectedIndexChanged);
            if (File.Exists(_dataBaseXml))
            {
                _dbcon = new ConnectMySqlCloud();
                DataBaseArg arg = DataBaseArg.ParseXml(_dataBaseXml);
                if (_frmMode.ToUpper() == "MOD06" || _frmMode.ToUpper() == "MYD06")
                    _outputDir = arg.OutputDir;
                else
                    _outputDir = arg.AIRSRootPath;
                txtOutDir.Text =_outputDir;
                txtHistoryPrj.Text = _outputDir;
            }
            Control.CheckForIllegalCrossThreadCalls = false;//关闭该异常检测的方式来避免异常的出现
            _state = new Action<int, string>(InvokeProgress);
        }
        
        private void CheckFrmMode()
        {
            txtResl.Value = 0.05d;
            if (_frmMode.ToUpper() == "AIRS")
            {
                _fileFilter = "AIRS.*.hdf";
                _xml = "AIRSDataProcess.xml";
                this.Text = "AIRS数据自动处理";
                txtResl.Value = 1d;
            }
            else if (_frmMode.ToUpper() == "MYD06")
            {
                _fileFilter = "MYD06_L2*.HDF";
                _xml = "MYD06DataProcess.xml";
                this.Text = "Aqua MODIS数据自动处理";
                //txtResl.Value = 1d;
            }
            else if (_frmMode.ToUpper() == "MOD06")
            {
                _fileFilter = "MOD06_L2*.HDF";
                _xml = "MOD06DataProcess.xml";
                this.Text = "Terra MODIS数据自动处理";
                //txtResl.Value = 1d;
            }
            _path = Path.Combine(_path, _xml);
        }

        void frmMod06DataPro_Load(object sender, EventArgs e)
        {
            _envList = new List<PrjEnvelopeItem>();
            if (File.Exists(_path))
            {
                InputArg arg = InputArg.ParseXml(_path);
                if (arg != null)
                    InitSetting(arg);
            }
        }

        private void InitSetting(InputArg arg)
        {
            if (arg.Bands !=null)
                _selectedNode = arg.Bands;
            if (arg.InputDir!=null&&Directory.Exists(arg.InputDir))
                txtDirName.Text = arg.InputDir;
            if (arg.ValidEnvelopes != null && arg.ValidEnvelopes.Length>0)
            {
                _envList = arg.ValidEnvelopes.ToList();
                foreach (PrjEnvelopeItem item in _envList)
                {
                    lstRegions.Items.Add(item.Name);
                }
            }
        }

        private void SetInputDir(string inputDir)
        {
            //string[] files = Directory.GetFiles(inputDir, _fileFilter, SearchOption.AllDirectories);
            //if (files == null || files.Length < 1)
            //    return;
            _isvalid = false;
            IsHaveValidFile(inputDir);
            if (!_isvalid)
            {
                FillTreeView(null, inputDir);
                return;
            }
            try
            {
                string[] datasetWithID;
                if (_frmMode == "MOD06" )
                    datasetWithID = _dbcon.QueryDatasetsWithPrdsID("MOD06");
                else if (_frmMode == "MYD06")
                    datasetWithID = _dbcon.QueryDatasetsWithPrdsID("MYD06");
                else
                    datasetWithID = _dbcon.QueryDatasetsWithPrdsID("AIRS");
                FillTreeView(datasetWithID.ToArray(), inputDir);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        string[] validfiles;
        bool _isvalid = false;
        private void IsHaveValidFile(string inputDir)
        {
            try
            {
                if (_isvalid != false)
                    return;
                validfiles = Directory.GetFiles(inputDir, _fileFilter, SearchOption.TopDirectoryOnly);
                if (validfiles != null && validfiles.Length > 0)
                {
                    foreach (string file in validfiles)
                    {
                        if (GeoDo.HDF4.HDF4Helper.IsHdf4(file))
                        {
                            _isvalid = true;
                            break;
                        }
                    }
                }
                //在指定目录及子目录下查找文件
                DirectoryInfo Dir = new DirectoryInfo(inputDir);
                foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录   
                {
                    IsHaveValidFile(Path.Combine(Dir.FullName, d.ToString()));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private void FillTreeView(string[] datasetNames,string inputDir)
        {
            tvdataset.Nodes.Clear();
            TreeNode root = new TreeNode(inputDir);
            if (datasetNames != null)
            {
                foreach (string dsName in datasetNames)
                {
                    root.Nodes.Add(dsName);
                    if (_selectedNode.Contains(dsName))
                        root.LastNode.Checked = true;
                }
                tvdataset.Nodes.Add(root);
                tvdataset.ExpandAll();
            }
            tvdataset.SelectedNode = root;
        }

        #endregion

        #region 按钮事件及响应
        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    txtDirName.Text = dlg.SelectedPath;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                btnOk.Enabled = false;
                if (!CheckArgsIsOk())
                    return;
                progressBar.Visible = true;
                InputArg arg = new InputArg(_path);
                arg.InputDir = txtDirName.Text;
                arg.OutputDir = txtOutDir.Text;
                arg.ValidEnvelopes = _envList.ToArray();
                string[] datasets = GetSelectedDatasets();
                arg.Bands = datasets;
                arg.ToXml(_path);
                runTaskThread = new Thread(new ThreadStart(this.DoProcess));
                runTaskThread.IsBackground = true;
                runTaskThread.Start();
                //文件清理
            }
            catch(SystemException ex)
            {
                MessageBox.Show("处理出错："+ex.Message);
                btnOk.Enabled = true;
            }
            finally
            {
                this.Activate();
                //btnOk.Enabled = true;
                progressBar.Visible = true;
            }
        }

        private void DoProcess()
        {
            try
            {
                if (_frmMode == "MOD06")
                {
                    DataProcesser processer = new DataProcesser(txtDirName.Text, txtOutDir.Text, GetSelectedDatasets(), txtResl.ValueF, _envList.ToArray(), cbxOverlapPrj.Checked, cbxOverlapMosaic.Checked,cbxDirectMosaic.Checked,cbxOnlyPrj.Checked,cbxLostAdded.Checked);
                    processer.IsOriginResl = cbxOriginResl.Checked;
                    processer._historyPrjDir =(cbxDirectMosaic.Checked?txtHistoryPrj.Text:null);
                    if (processer.DoProcess(_state))
                    {
                    }
                }
                else if (_frmMode == "MYD06")
                {
                    MYDDataProcesser processer = new MYDDataProcesser(txtDirName.Text, txtOutDir.Text, GetSelectedDatasets(), txtResl.ValueF, _envList.ToArray(), cbxOverlapPrj.Checked, cbxOverlapMosaic.Checked, cbxDirectMosaic.Checked, cbxOnlyPrj.Checked, cbxLostAdded.Checked);
                    processer.IsOriginResl = cbxOriginResl.Checked;
                    processer._historyPrjDir = (cbxDirectMosaic.Checked ? txtHistoryPrj.Text : null);
                    if (processer.DoProcess(_state))
                    {
                    }
                }
                else
                {
                    AIRSDataProcesser processer = new AIRSDataProcesser(txtDirName.Text, txtOutDir.Text, GetSelectedDatasets(), txtResl.ValueF, _envList.ToArray(), cbxOverlapPrj.Checked, cbxOverlapMosaic.Checked, cbxDirectMosaic.Checked, cbxOnlyPrj.Checked);
                    processer.IsOriginResl = cbxOriginResl.Checked;
                    processer._historyPrjDir = (cbxDirectMosaic.Checked ? txtHistoryPrj.Text : null);
                    if (processer.Process(_state))
                    {
                    }
                }
                runTaskThread = new Thread(new ThreadStart(this.ClearRecycleFiles));
                runTaskThread.IsBackground = true;
                runTaskThread.Start();
                //_state(-1, "开始后台临时文件清理...");
            }
            catch (System.Exception ex)
            {
                LogFactory.WriteLine(_frmMode + "预处理ERROR", ex.Message);
                _state(-100,ex.Message);
                //throw ex;
            }
        }

        private void ClearRecycleFiles()
        {
            try
            {
                sb.Clear();
                string dir = txtOutDir.Text;
                string recycledir = "";
                recycledir = Path.GetPathRoot(dir);
                recycledir = Path.Combine(recycledir, "Recycle");
                if (Directory.Exists(recycledir))
                    TryClearRecycleDir(recycledir);
                else if (Directory.Exists(recycledir.ToUpper()))
                    TryClearRecycleDir(recycledir.ToUpper());
                else if (Directory.Exists(recycledir.ToLower()))
                    TryClearRecycleDir(recycledir.ToLower());
                else
                    sb.Append(recycledir+"不存在！未进行临时文件清理！");
                LogFactory.WriteLine("回收站清理", sb.ToString());
            }
            catch (System.Exception ex)
            {
                return;
            }
        }

        StringBuilder sb = new StringBuilder();
        string fname = "";
        private void TryClearRecycleDir(string recycledir)
        {
            try
            {
                //在指定目录及子目录下查找文件
                DirectoryInfo Dir = new DirectoryInfo(recycledir);
                foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录   
                {
                    TryClearRecycleDir(Path.Combine(Dir.FullName, d.ToString()));
                }
                foreach (FileInfo f in Dir.GetFiles("*.*"))             //查找文件
                {
                    fname = Path.Combine(Dir.FullName, f.ToString());
                    try
                    {
                        if (File.Exists(fname))
                        {
                            File.Delete(fname);
                            sb.Append("删除成功：" + fname + "\r\n");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        sb.Append("删除失败：" + fname + "\r\n");
                    }
                }
                if (Directory.Exists(recycledir))
                {
                    Directory.Delete(recycledir);
                    sb.Append("删除成功：" + recycledir + "\r\n");
                }
            }
            catch (System.Exception ex)
            {
                sb.Append("删除失败：" + recycledir + "\r\n");
            }
        }

        private string[] GetSelectedDatasets()
        {
            List<string> checkNodes = new List<string>();
            foreach (TreeNode node in tvdataset.Nodes)
            {
                if (node.Checked)
                    checkNodes.Add(node.Name);
                if (node.Nodes != null)
                {
                    checkNodes.AddRange(GetSelectedDatasets(node));
                }
            }
            return checkNodes.ToArray();
        }

        private string[] GetSelectedDatasets(TreeNode node)
        {
            List<string> checkNodes = new List<string>();
            if (node.Nodes != null && node.Nodes.Count > 0)
            {
                foreach (TreeNode childnode in node.Nodes)
                {
                    if (childnode.Checked)
                        checkNodes.Add(childnode.Text);
                }
                return checkNodes.ToArray();
            }
            return null;
        }

        private bool CheckArgsIsOk()
        {
            if (string.IsNullOrEmpty(txtDirName.Text) || !Directory.Exists(txtDirName.Text))
            {
                throw new ArgumentException("输入数据的文件夹不存在，请检查！", "提示信息");
            }                
            bool selectedDataSet=false;
            foreach (TreeNode node in tvdataset.Nodes)
            {
                if (HasNodeChecked(node))
                {
                    selectedDataSet = true;
                    break;
                }
            }
            if (!selectedDataSet)
            {
                throw new ArgumentException("请选择待处理数据集！", "提示信息");
            }
            if (string.IsNullOrEmpty(txtOutDir.Text)||!Directory.Exists(txtOutDir.Text))
            {
                throw new ArgumentException("输出数据的文件夹不存在，请检查！", "提示信息");
            }
            if (cbxDirectMosaic.Checked)
            {
                if (string.IsNullOrEmpty(txtHistoryPrj.Text) || !Directory.Exists(txtHistoryPrj.Text))
                    throw new ArgumentException("历史投影文件夹不存在，请检查！", "提示信息");
            }
            if (_envList==null||_envList.Count==0)
            {
                throw new ArgumentException("请设置投影处理的区域！", "提示信息");
            }
            if (CheckReslArgsIsOk(_frmMode))
                return true;
            return false;
        }

        private bool CheckReslArgsIsOk(string mode)
        {
            if (mode == "MOD06" || mode == "MYD06")
            {
                if (txtResl.ValueF < 0.01f)
                {
                    throw new ArgumentException("分辨率不能高于原始分辨率！", "提示信息");
                }
            } 
            else
            {
                if (txtResl.ValueF<1.0f)
                {
                    throw new ArgumentException("分辨率不能高于原始分辨率！", "提示信息");
                }
            }
            return true;
        }

        private bool HasNodeChecked(TreeNode node)
        {
            if (node.Checked)
                return true;
            if (node.GetNodeCount(false) > 0)
            {
                foreach (TreeNode item in node.Nodes)
                {
                    if (HasNodeChecked(item))
                        return true;
                }
            }
            return false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            if (runTaskThread != null)
                runTaskThread.Abort();
            Close();
        }

        private void txtDirName_TextChanged(object sender, EventArgs e)
        {
            string dirName = txtDirName.Text;
            if (Directory.Exists(dirName))
            {
                SetInputDir(dirName);
            }
            else
                FillTreeView(null, dirName);
        }

        private void lstRegionsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRegions.SelectedItem != null)
            {
                string name = lstRegions.SelectedItem.ToString();
                foreach (PrjEnvelopeItem item in _envList)
                {
                    if (name == item.Name)
                    {
                        txtRegionName.Text = name;
                        this.ucGeoRangeControl1.SetGeoEnvelope(item.PrjEnvelope);
                    }
                }
            }
        }

        private void btnAddEvp_Click(object sender, EventArgs e)
        {
            string evpName = txtRegionName.Text.Trim();
            if (string.IsNullOrWhiteSpace(evpName))
            {
                MessageBox.Show("请输入范围标识");
                return;
            }
            foreach (PrjEnvelopeItem item in _envList)
            {
                if (item.Name == evpName)
                {
                    MessageBox.Show("已存在名为" + evpName + "的区域范围名称，请重新输入！");
                    return;
                }
            }
            RasterProject.PrjEnvelope envelope = GetEnvelopeFromUI();
            if (envelope.Width == 0 || envelope.Height == 0
                || !CheckRegion(envelope.MinX, envelope.MaxX, -180, 180)
                || !CheckRegion(envelope.MinY, envelope.MaxY, -90, 90))
            {
                MessageBox.Show("请输入正确的地理坐标范围值！");
                return;
            }
            PrjEnvelopeItem env = new PrjEnvelopeItem(txtRegionName.Text, envelope);
            lstRegions.Items.Add(env.Name);
            _envList.Add(env);
        }

        private RasterProject.PrjEnvelope GetEnvelopeFromUI()
        {
                return new RasterProject.PrjEnvelope(ucGeoRangeControl1.MinX, ucGeoRangeControl1.MaxX,
                        ucGeoRangeControl1.MinY, ucGeoRangeControl1.MaxY);
        }

        private bool CheckRegion(double min, double max, double minLimit, double maxLimit)
        {
            if (min < max)
            {
                if (max <= maxLimit && min >= minLimit)
                {
                    return true;
                }
            }
            return false;
        }

        private void tvdataset_AfterCheck(object sender, TreeViewEventArgs e)
        {
            SetChildChecked(e.Node);  // 判断是否是根节点 
            if (e.Node.Parent != null)
            {
                SetParentChecked(e.Node);
            }
        }

        /// <summary> 
        /// 根据子节点状态设置父节点的状态 
        /// </summary> 
        /// <param name="childNode"></param> 
        private void SetParentChecked(TreeNode childNode)
        {
            TreeNode parentNode = childNode.Parent;
            if (!parentNode.Checked && childNode.Checked)
            {
                int ichecks = 0;
                for (int i = 0; i < parentNode.GetNodeCount(false); i++)
                {
                    TreeNode node = parentNode.Nodes[i];
                    if (node.Checked) { ichecks++; }
                }
                if (ichecks == parentNode.GetNodeCount(false))
                {
                    parentNode.Checked = true;
                    if (parentNode.Parent != null)
                    {
                        SetParentChecked(parentNode);
                    }
                }
            }
            else if (parentNode.Checked && !childNode.Checked)
            {
                parentNode.Checked = false;
            }
        }
        /// <summary> 
        /// 根据父节点状态设置子节点的状态 
        /// </summary> 
        /// <param name="parentNode"></param> 
        private void SetChildChecked(TreeNode parentNode)
        {
            for (int i = 0; i < parentNode.GetNodeCount(false); i++)
            {
                TreeNode node = parentNode.Nodes[i];
                node.Checked = parentNode.Checked;
                if (node.GetNodeCount(false) > 0)
                {
                    SetChildChecked(node);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstRegions.SelectedItem != null)
            {
                foreach (PrjEnvelopeItem item in _envList)
                {
                    if (item.Name == lstRegions.SelectedItem.ToString())
                    {
                        _envList.Remove(item);
                        break;
                    }
                }
                lstRegions.Items.Remove(lstRegions.SelectedItem);
            }
        }

        private void btnOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtOutDir.Text = dlg.SelectedPath;
                }
            }
        }

        private void cbxOriginResl_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxOriginResl.Checked)
            {
                txtResl.Enabled = false;
            } 
            else
            {
                txtResl.Enabled = true;
            }
        }

        private void txtResl_EnabledChanged(object sender, EventArgs e)
        {
            if (_frmMode == "MOD06" || _frmMode == "MYD06")
            {
                txtResl.Value = 0.05f;
            } 
            else
            {
                txtResl.Value = 1f;
            }
        }
        #endregion

        private void cbxDirectMosaic_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxDirectMosaic.Checked)
            {
                cbxOnlyPrj.Checked = false;
                cbxOnlyPrj.Enabled = false;
                cbxOverlapPrj.Checked = false;
                cbxOverlapPrj.Enabled = false;
                lblHistoryPrj.Visible = true;
                btnHistoryPrj.Visible = true;
                txtHistoryPrj.Visible = true;
            }
            else
            {
                cbxOverlapPrj.Enabled = true;
                cbxOnlyPrj.Enabled = true;
                lblHistoryPrj.Visible = false;
                btnHistoryPrj.Visible = false;
                txtHistoryPrj.Visible = false;
            }         
        }

        private void cbxOverlapPrj_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxOverlapPrj.Checked)
            {
                cbxDirectMosaic.Checked = false;
                cbxDirectMosaic.Enabled = false;
            }
            else
            {
                if(!cbxOnlyPrj.Checked)
                    cbxDirectMosaic.Enabled = true;
            }
        }

        private void cbxOnlyPrj_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxOnlyPrj.Checked)
            {
                cbxDirectMosaic.Checked = false;
                cbxDirectMosaic.Enabled = false;
                cbxOverlapMosaic.Checked = false;
                cbxOverlapMosaic.Enabled = false;
                txtOutDir.Enabled = true;
                btnOutDir.Enabled = true;
            }
            else
            {
                cbxDirectMosaic.Enabled = true;
                cbxOverlapMosaic.Enabled = true;
                txtOutDir.Enabled = false;
                txtOutDir.Text=_outputDir;
                btnOutDir.Enabled = false;
            }         

        }

        private void btnHistoryPrj_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtHistoryPrj.Text = dlg.SelectedPath;
                }
            }
        }

        private void cbxLostAdded_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxLostAdded.Checked)
            {
                cbxDirectMosaic.Checked = false;
                cbxDirectMosaic.Enabled = false;
            }
            else
            {
                //cbxDirectMosaic.Checked = true;
                cbxDirectMosaic.Enabled = true;
            }
        }
    }
}
