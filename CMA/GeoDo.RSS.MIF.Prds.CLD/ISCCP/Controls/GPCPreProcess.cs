#region Version Info
/*========================================================================
* 功能概述：根据设置的自定义区域提取GPC数据中的单波段数据，并存储为LDF文件
* 
* 创建者：$zhangyanbing$     时间：2014-2-19 09:10:15
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RasterProject;
using System.Collections;
using System.Collections.Generic;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Runtime.InteropServices;
using GeoDo.RSS.DF.GPC;
using GeoDo.FileProject;
using System.Text.RegularExpressions;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// 类名：GPCPreProcess
    /// 属性描述：搜寻指定文件夹下的GPC格式的D2数据，并根据设置的自定义区域提取GPC数据中的单波段数据，存储为LDF文件
    /// 创建者：zhangyanbing   创建日期：2014-2-19 09:10:15
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class GPCPreProcess : Form
    {
        CoordEnvelope _env = null;
        private Dictionary<String, String> _allFiles = new Dictionary<String, String>();
        private Dictionary<string, Dictionary<String, Int16>> _allDatasets = new Dictionary<string, Dictionary<String, Int16>>();
        Dictionary<string, string> _group2prd = new Dictionary<string, string>();
        private Dictionary<String, String> _sets2subPrds = new Dictionary<String, String>();
        private bool _fillcolor =true;
        private  Dictionary<string, Int16> _sets2bandNO = new Dictionary<string, Int16>();
        private List<PrjEnvelopeItem> _envList = new List<PrjEnvelopeItem>();
        private string _path = AppDomain.CurrentDomain.BaseDirectory+ "ISCCPDataProcess.xml";
        private string[] _xmlSelectedNode = new string[] { };

        public GPCPreProcess()
        {
            InitializeComponent();
            this.treeviewdataset.Dock = DockStyle.Fill;
            treeviewdataset.CheckBoxes = true;
            treeviewdataset.AfterCheck += new TreeViewEventHandler(treeviewdataset_AfterCheck);
            DataSets2bandNO();
            Datasets2Prds.AddAllD2DataSets(out _allDatasets);
            Datasets2Prds.Group2Prds(out _group2prd);
            Datasets2Prds.DataSets2subPrds(out _sets2subPrds);
            AddDefaultRegions();
            cbxRegionlist.SelectedIndexChanged += new EventHandler(ChangeRegion);
        }

        #region 文件选择及树填充
        private void btnOpen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtInDir.Text = dialog.SelectedPath;
                if (_allFiles != null)
                {
                    _allFiles.Clear();
                }
                if (TryGetGPCfile(dialog.SelectedPath))
                {
                    //ISCCPContinutyDetec(_allFiles.Values.ToArray(),"ISCCP");
                    FillAllTreeView();
                }              
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtOutDir.Text = dialog.SelectedPath;
            }
        }

        private bool TryGetGPCfile(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*.GPC", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
            {
                MessageBox.Show("当前路径不存在可处理的GPC数据!");
                return false;
            }
            FileInfo finfo;
            long size;
            foreach (string file in files)
            {
                finfo = new FileInfo(file);
                size = finfo.Length;
                if (size == 871000)
                    _allFiles.Add(Path.GetFileName(file), file);
            }
            if (_allFiles == null)
            {
                MessageBox.Show("当前路径不存在可处理的GPC数据!");
                return false;
            }
            return true;
        }


        private void FillAllTreeView()
        {
            this.treeviewdataset.Nodes.Clear();
            this.treeviewdataset.BeginUpdate();
            //foreach (string  file in _allFiles.Keys)
            //{
            //    this.treeviewdataset.Nodes.Add(new TreeNode(Path.GetFileName(file)));
            //}
            this.treeviewdataset.Nodes.Add(new TreeNode("所有数据集"));
            Dictionary<string,Int16> DatasetsG=new Dictionary<string,Int16>();
            foreach (TreeNode node in treeviewdataset.Nodes)
            {
                foreach(var group  in _allDatasets.Keys)
                {
                    node.Nodes.Add(group);
                }
                foreach(TreeNode groupnode  in node.Nodes)
                {
                    foreach (var  Dataset in _allDatasets[groupnode.Text].Keys)
                    {
                        groupnode.Nodes.Add(Dataset);
                        if (_xmlSelectedNode.Contains(Dataset))
                            groupnode.LastNode.Checked = true;
                    }
                }
            }
            this.treeviewdataset.EndUpdate();
            this.treeviewdataset.CollapseAll();
        }

        /// <summary>
        /// ISCCP D2数据的数据集与波段号之间的对应关系
        /// </summary>
        private void DataSets2bandNO()
        {
            //_sets2bandNO.Add("Latitude", 1);
            //_sets2bandNO.Add("Longitude index (equal-area)", 2);
            //_sets2bandNO.Add("Western-most longitude index (equal-angle)", 3);
            //_sets2bandNO.Add("Eastern-most longitude index (equal-angle)", 4);
            _sets2bandNO.Add("Land water coast code", 5);//5
            _sets2bandNO.Add("Number of observations", 6);//6
            _sets2bandNO.Add("Number of daytime observations", 7);
            _sets2bandNO.Add("Mean cloud amount", 8);//8
            _sets2bandNO.Add("Mean IR-marginal cloud amount", 9);
            _sets2bandNO.Add("Frequency of mean cloud amount = 0-10%", 10);
            _sets2bandNO.Add("Frequency of mean cloud amount = 10-20%", 11);
            _sets2bandNO.Add("Frequency of mean cloud amount = 20-30%", 12);
            _sets2bandNO.Add("Frequency of mean cloud amount = 30-40%", 13);
            _sets2bandNO.Add("Frequency of mean cloud amount = 40-50%", 14);
            _sets2bandNO.Add("Frequency of mean cloud amount = 50-60%", 15);
            _sets2bandNO.Add("Frequency of mean cloud amount = 60-70%", 16);
            _sets2bandNO.Add("Frequency of mean cloud amount = 70-80%", 17);
            _sets2bandNO.Add("Frequency of mean cloud amount = 80-90%", 18);
            _sets2bandNO.Add("Frequency of mean cloud amount = 90-100%", 19);
            _sets2bandNO.Add("Mean cloud top pressure(PC)", 20);
            _sets2bandNO.Add("Standard deviation of spatial mean over time(PC)", 21);
            _sets2bandNO.Add("Time mean of standard deviation over space(PC)", 22);//23
            _sets2bandNO.Add("Cloud temperature(TC)", 23);
            _sets2bandNO.Add("Standard deviation of spatial mean over time(TC)", 24);
            _sets2bandNO.Add("Time mean of standard deviation over space(TC)", 25);
            _sets2bandNO.Add("Mean cloud optical thickness(TAU)", 26);//26
            _sets2bandNO.Add("Standard deviation of spatial mean over time(TAU)", 27);
            _sets2bandNO.Add("Time mean of standard deviation over space(TAU)", 28);
            _sets2bandNO.Add("Mean cloud water path(WP)", 29);//29
            _sets2bandNO.Add("Standard deviation of spatial mean over time(WP)", 30);//20
            _sets2bandNO.Add("Time mean of standard deviation over space(WP)", 31);
            _sets2bandNO.Add("Mean CA for low-level clouds", 32);
            _sets2bandNO.Add("Mean PC for low-level clouds", 33);
            _sets2bandNO.Add("Mean TC for low-level clouds", 34);
            _sets2bandNO.Add("Mean CA for middle-level clouds", 35);
            _sets2bandNO.Add("Mean PC for middle-level clouds", 36);
            _sets2bandNO.Add("Mean TC for middle-level clouds", 37);
            _sets2bandNO.Add("Mean CA for high-level clouds", 38);
            _sets2bandNO.Add("Mean PC for high-level clouds", 39);
            _sets2bandNO.Add("Mean TC for high-level clouds", 40);
            _sets2bandNO.Add("Mean CA for cloud type 1 = Cumulus, liquid", 41);
            _sets2bandNO.Add("Mean PC for cloud type 1 = Cumulus, liquid", 42);
            _sets2bandNO.Add("Mean TC for cloud type 1 = Cumulus, liquid", 43);
            _sets2bandNO.Add("Mean TAU for cloud type 1 = Cumulus, liquid", 44);
            _sets2bandNO.Add("Mean WP for cloud type 1 = Cumulus, liquid", 45);
            _sets2bandNO.Add("Mean CA for cloud type 2 = Stratocumulus, liquid", 46);
            _sets2bandNO.Add("Mean PC for cloud type 2 = Stratocumulus, liquid", 47);
            _sets2bandNO.Add("Mean TC for cloud type 2 = Stratocumulus, liquid", 48);
            _sets2bandNO.Add("Mean TAU for cloud type 2 = Stratocumulus, liquid", 49);
            _sets2bandNO.Add("Mean WP for cloud type 2 = Stratocumulus, liquid", 50);
            _sets2bandNO.Add("Mean CA for cloud type 3 = Stratus, liquid", 51);
            _sets2bandNO.Add("Mean PC for cloud type 3 = Stratus, liquid", 52);
            _sets2bandNO.Add("Mean TC for cloud type 3 = Stratus, liquid", 53);
            _sets2bandNO.Add("Mean TAU for cloud type 3 = Stratus, liquid", 54);
            _sets2bandNO.Add("Mean WP for cloud type 3 = Stratus, liquid", 55);
            _sets2bandNO.Add("Mean CA for cloud type 4 = Cumulus, ice", 56);
            _sets2bandNO.Add("Mean PC for cloud type 4 = Cumulus, ice", 57);
            _sets2bandNO.Add("Mean TC for cloud type 4 = Cumulus, ice", 58);
            _sets2bandNO.Add("Mean TAU for cloud type 4 = Cumulus, ice", 59);
            _sets2bandNO.Add("Mean WP for cloud type 4 = Cumulus, ice", 60);
            _sets2bandNO.Add("Mean CA for cloud type 5 = Stratocumulus, ice", 61);
            _sets2bandNO.Add("Mean PC for cloud type 5 = Stratocumulus, ice", 62);
            _sets2bandNO.Add("Mean TC for cloud type 5 = Stratocumulus, ice", 63);
            _sets2bandNO.Add("Mean TAU for cloud type 5 = Stratocumulus, ice", 64);
            _sets2bandNO.Add("Mean WP for cloud type 5 = Stratocumulus, ice", 65);
            _sets2bandNO.Add("Mean CA for cloud type 6 = Stratus, ice", 66);
            _sets2bandNO.Add("Mean PC for cloud type 6 = Stratus, ice", 67);
            _sets2bandNO.Add("Mean TC for cloud type 6 = Stratus, ice", 68);
            _sets2bandNO.Add("Mean TAU for cloud type 6 = Stratus, ice", 69);
            _sets2bandNO.Add("Mean WP for cloud type 6 = Stratus, ice", 70);
            _sets2bandNO.Add("Mean CA for cloud type 7 = Altocumulus, liquid", 71);
            _sets2bandNO.Add("Mean PC for cloud type 7 = Altocumulus, liquid", 72);
            _sets2bandNO.Add("Mean TC for cloud type 7 = Altocumulus, liquid", 73);
            _sets2bandNO.Add("Mean TAU for cloud type 7 = Altocumulus, liquid", 74);
            _sets2bandNO.Add("Mean WP for cloud type 7 = Altocumulus, liquid", 75);
            _sets2bandNO.Add("Mean CA for cloud type 8 = Altostratus, liquid", 76);
            _sets2bandNO.Add("Mean PC for cloud type 8 = Altostratus, liquid", 77);
            _sets2bandNO.Add("Mean TC for cloud type 8 = Altostratus, liquid", 78);
            _sets2bandNO.Add("Mean TAU for cloud type 8 = Altostratus, liquid", 79);
            _sets2bandNO.Add("Mean WP for cloud type 8 = Altostratus, liquid", 80);
            _sets2bandNO.Add("Mean CA for cloud type 9 = Nimbostratus, liquid", 81);
            _sets2bandNO.Add("Mean PC for cloud type 9 = Nimbostratus, liquid", 82);
            _sets2bandNO.Add("Mean TC for cloud type 9 = Nimbostratus, liquid", 83);
            _sets2bandNO.Add("Mean TAU for cloud type 9 = Nimbostratus, liquid", 84);
            _sets2bandNO.Add("Mean WP for cloud type 9 = Nimbostratus, liquid", 85);
            _sets2bandNO.Add("Mean CA for cloud type 10 = Altocumulus, ice", 86);
            _sets2bandNO.Add("Mean PC for cloud type 10 = Altocumulus, ice", 87);
            _sets2bandNO.Add("Mean TC for cloud type 10 = Altocumulus, ice", 88);
            _sets2bandNO.Add("Mean TAU for cloud type 10 = Altocumulus, ice", 89);
            _sets2bandNO.Add("Mean WP for cloud type 10 = Altocumulus, ice", 90);
            _sets2bandNO.Add("Mean CA for cloud type 11 = Altostratus, ice", 91);
            _sets2bandNO.Add("Mean PC for cloud type 11 = Altostratus, ice", 92);
            _sets2bandNO.Add("Mean TC for cloud type 11 = Altostratus, ice", 93);
            _sets2bandNO.Add("Mean TAU for cloud type 11 = Altostratus, ice", 94);
            _sets2bandNO.Add("Mean WP for cloud type 11 = Altostratus, ice", 95);
            _sets2bandNO.Add("Mean CA for cloud type 12 = Nimbostratus, ice", 96);
            _sets2bandNO.Add("Mean PC for cloud type 12 = Nimbostratus, ice", 97);
            _sets2bandNO.Add("Mean TC for cloud type 12 = Nimbostratus, ice", 98);
            _sets2bandNO.Add("Mean TAU for cloud type 12 = Nimbostratus, ice", 99);
            _sets2bandNO.Add("Mean WP for cloud type 12 = Nimbostratus, ice", 100);
            _sets2bandNO.Add("Mean CA for cloud type 13 = Cirrus", 101);
            _sets2bandNO.Add("Mean PC for cloud type 13 = Cirrus", 102);
            _sets2bandNO.Add("Mean TC for cloud type 13 = Cirrus", 103);
            _sets2bandNO.Add("Mean TAU for cloud type 13 = Cirrus", 104);
            _sets2bandNO.Add("Mean WP for cloud type 13 = Cirrus", 105);
            _sets2bandNO.Add("Mean CA for cloud type 14 = Cirrostratus", 106);
            _sets2bandNO.Add("Mean PC for cloud type 14 = Cirrostratus", 107);
            _sets2bandNO.Add("Mean TC for cloud type 14 = Cirrostratus", 108);
            _sets2bandNO.Add("Mean TAU for cloud type 14 = Cirrostratus", 109);
            _sets2bandNO.Add("Mean WP for cloud type 14 = Cirrostratus", 110);
            _sets2bandNO.Add("Mean CA for cloud type 15 = Deep convective", 111);
            _sets2bandNO.Add("Mean PC for cloud type 15 = Deep convective", 112);
            _sets2bandNO.Add("Mean TC for cloud type 15 = Deep convective", 113);
            _sets2bandNO.Add("Mean TAU for cloud type 15 = Deep convective", 114);
            _sets2bandNO.Add("Mean WP for cloud type 15 = Deep convective", 115);
            _sets2bandNO.Add("Mean TS from clear sky composite(TS)", 116);//116
            _sets2bandNO.Add("Time mean of standard deviation over space(TS)", 117);
            _sets2bandNO.Add("Mean RS from clear sky composite(RS)", 118);//118
            _sets2bandNO.Add("Mean ice snow cover", 119);//119
            _sets2bandNO.Add("Mean Surface pressure (PS)", 120);
            _sets2bandNO.Add("Mean Near-surface air temperature (TSA)", 121);
            _sets2bandNO.Add("Mean Temperature at 740 mb (T)", 122);
            _sets2bandNO.Add("Mean Temperature at 500 mb (T)", 123);
            _sets2bandNO.Add("Mean Temperature at 375 mb (T)", 124);
            _sets2bandNO.Add("Mean Tropopause pressure (PT)", 125);
            _sets2bandNO.Add("Mean Tropopause temperature (TT)", 126);
            _sets2bandNO.Add("Mean Stratosphere temperature at 50 mb (T)", 127);
            _sets2bandNO.Add("Mean Precipitable water for 1000-680 mb (PW)", 128);
            _sets2bandNO.Add("Mean Precipitable water for 680-310 mb (PW)", 129);
            _sets2bandNO.Add("Mean Ozone column abundance (O3)", 130);
        }
        #endregion

        #region 数据集选择响应

        void treeviewdataset_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            if (e.Node.Level== 0)
            {
                _fillcolor = false;
                CheckChangeALL(e.Node);
                return;
            }else if (e.Node.Level ==1)
            {
                _fillcolor = false;
                CheckChangeALL(e.Node);
                return;
            }
            else if (e.Node.Level == 2 )
            {
                if (e.Node.Checked && _fillcolor)
                    e.Node.Parent.BackColor = Color.Green;
                else
                    e.Node.Parent.BackColor = Color.White;
            }            
        }

        private void CheckChangeALL(TreeNode root)
        {
            bool check = root.Checked;
            foreach (TreeNode node in root.Nodes)
            {
                if (node.Level == 1)
                {
                    node.Checked = check;
                    foreach (TreeNode subnode in node.Nodes)
                    {
                        subnode.Checked = check;
                    }
                }
                if (node.Level==2)
                {
                    node.Checked = check;
                }                    
            }
            treeviewdataset.Refresh();
        }

        #endregion

        #region 范围设置
        private void AddDefaultRegions()
        {
            if (File.Exists(_path))
            {
                InputArg arg = InputArg.ParseXml(_path);
                if (arg != null)
                {
                    _xmlSelectedNode = arg.Bands;
                    if (Directory.Exists(arg.InputDir))
                    {
                        txtInDir.Text = arg.InputDir;
                    }
                    txtOutDir.Text = arg.OutputDir;                    
                    if (arg.ValidEnvelopes != null && arg.ValidEnvelopes.Length > 0)
                    {
                        _envList = arg.ValidEnvelopes.ToList();
                        foreach (PrjEnvelopeItem item in _envList)
                        {
                            cbxRegionlist.Items.Add(item.Name);
                        }
                    }
                }
            }
            else
            {
                RasterProject.PrjEnvelope globalEnv = new RasterProject.PrjEnvelope(-180, 180, -90, 90);
                PrjEnvelopeItem globalPrjEnv = new PrjEnvelopeItem("Global", globalEnv);
                _envList.Add(globalPrjEnv);
                RasterProject.PrjEnvelope chinaEnv = new RasterProject.PrjEnvelope(65, 145, 10, 60);
                PrjEnvelopeItem chinaPrjEnv = new PrjEnvelopeItem("China", chinaEnv);
                _envList.Add(chinaPrjEnv);
                foreach (PrjEnvelopeItem env in _envList)
                {
                    cbxRegionlist.Items.Add(env.Name);
                }
                cbxRegionlist.SelectedIndex = 1;
            }
        }

        private void btnSaveNewRegion_Click(object sender, EventArgs e)
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
            RasterProject.PrjEnvelope env = new RasterProject.PrjEnvelope(dtbxMinX.Value, dtbxMaxX.Value, dtbxMinY.Value, dtbxMaxY.Value);
            if (env.Width <= 0 || env.Height <= 0
                || env.MinX < -180 || env.MaxX > 180
                || env.MinY < -90 || env.MaxY > 90)
            {
                MessageBox.Show("经纬度值超出有效值");
                return;
            }
            if (env.MinX == env.MaxX || env.MinY == env.MaxY)
            {
                MessageBox.Show("范围最大最小值不能相等！");
                return;
            }
            PrjEnvelopeItem newenv = new PrjEnvelopeItem(txtRegionName.Text, env);
            _envList.Add(newenv);
            cbxRegionlist.Items.Add(newenv.Name);
            cbxRegionlist.SelectedItem = newenv.Name;
            cbxRegionlist.Visible = true;
            txtRegionName.Visible = false;
            btnSaveNewRegion.Enabled = false;
            btnAddRegion.Enabled = true;
            btnCancelNewReg.Enabled = false;
        }

        private void btnCancelNewReg_Click(object sender, EventArgs e)
        {
            cbxRegionlist.Visible = true;
            txtRegionName.Visible = false;
            btnSaveNewRegion.Enabled = false;
            btnAddRegion.Enabled = true;
            btnCancelNewReg.Enabled = false;
            cbxRegionlist.SelectedIndex = 1;
        }

        private void btnAddRegion_Click(object sender, EventArgs e)
        {
            txtRegionName.Clear();
            cbxRegionlist.Visible = false;
            txtRegionName.Visible = true;
            btnSaveNewRegion.Enabled = true;
            btnAddRegion.Enabled = false;
            btnCancelNewReg.Enabled = true;
        }

        /// <summary>
        /// 改变选择区域时响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeRegion(object sender, EventArgs e)
        {
            foreach (PrjEnvelopeItem evp in _envList)
            {
                if (cbxRegionlist.SelectedItem.ToString() == evp.Name)
                {
                    dtbxMinX.Value = evp.PrjEnvelope.MinX;
                    dtbxMaxX.Value = evp.PrjEnvelope.MaxX;
                    dtbxMinY.Value = evp.PrjEnvelope.MinY;
                    dtbxMaxY.Value = evp.PrjEnvelope.MaxY;
                }
            }
        }

        private bool AddRegion()
        {
            double minlon = dtbxMinX.Value;
            double maxlon = dtbxMaxX.Value;
            double minlat = dtbxMinY.Value;
            double maxlat = dtbxMaxY.Value;
            CoordEnvelope env = new CoordEnvelope(minlon, maxlon, minlat, maxlat);
            if (env.Width <= 0 || env.Height <= 0
                || env.MinX < -180 || env.MaxX > 180
                || env.MinY < -90 || env.MaxY > 90)
            {
                errorProvider1.SetError(panel2, "经纬度值超出有效值");
                return false;
            }
            if (env.MinX == env.MaxX || env.MinY == env.MaxY)
            {
                errorProvider1.SetError(panel2, "经纬度范围不合法");
                return false;
            }
            _env = env;
            return true;
        }

        #endregion

        #region 数据集输出LDF
        private void btnOK_Click(object sender, EventArgs e)
        {
            btnOK.Enabled = false;
            try
            {
                if (!CheckArgsIsOk())
                    return;
                if (TryProcessfiles())//选择数据集判断
                {
                    this.Activate();
                    MessageBox.Show("数据处理完成！");
                    Close();
                }
                else
                {                    
                    MessageBox.Show("执行出错，请重试！");
                    return;
                }
            }
            catch (System.Exception ex)
            {

            }
            finally { btnOK.Enabled = true; }
        }

        private bool CheckArgsIsOk()
        {
            if (string.IsNullOrEmpty(txtInDir.Text) || !Directory.Exists(txtInDir.Text))
            {
                MessageBox.Show("请选择输入文件");
                return false;
            }
            if (string.IsNullOrEmpty(txtOutDir.Text))
            {
                MessageBox.Show("请设置输出目录");
                return false;
            }
            if (!AddRegion() || _env == null)
            {
                MessageBox.Show("请设置经纬度范围");
                return false;
            }
            return true;
        }

        private bool TryProcessfiles()
        {
                bool processd = false;
                Dictionary<string, string> checkband1 = new Dictionary<string, string>();
                foreach (TreeNode node in treeviewdataset.Nodes)
                {
                    foreach (TreeNode groupnode in node.Nodes)
                    {
                        foreach (TreeNode subnode in groupnode.Nodes)
                        {
                            if (subnode.Checked)//选中
                            {
                                checkband1.Add(subnode.Text, groupnode.Text);
                            }
                        }
                    }
                    if (checkband1.Count != 0)
                    {
                        processd = true;
                        InputArg arg = new InputArg();
                        arg.ValidEnvelopes = _envList.ToArray();
                        string[] datasets = checkband1.Keys.ToArray();
                        arg.Bands = datasets;
                        arg.InputDir = txtInDir.Text;
                        arg.OutputDir = txtOutDir.Text;
                        arg.ToXml(_path);
                        foreach (string filename in _allFiles.Keys)
                        {
                            ProcessGPC2ldf(filename, checkband1);
                        }
                        checkband1.Clear();
                    }
                }
                return processd;
        }


        private unsafe void ProcessGPC2ldf(string filename,Dictionary <string,string> checkbands)
        {
            string orgName = "xxxx", prdslevel = "xx",regionCode = cbxRegionlist.SelectedItem.ToString(), year="0000", month="00", UTCtime="0000";
            string[] args = filename.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length==9)
            {
                orgName = args[0];
                prdslevel = args[1];
                //halfYearCode = args[2];
                //regionCode = args[3];
                year = args[4];
                month = args[5];
                //day = args[6];
                UTCtime = args[7]; 
            }
            float[] buffer = null;
            int width, height, xOffset, yOffset;
            using (IRasterDataProvider dataprd = GeoDataDriver.Open(_allFiles[filename]) as IRasterDataProvider)
            {
                width = (int)(_env.Width / dataprd.ResolutionX);
                height = (int)(_env.Height / dataprd.ResolutionY);
                xOffset = (int)((_env.MinX + 180) / dataprd.ResolutionX);
                yOffset = (int)((90 - _env.MaxY) / dataprd.ResolutionY);
                enumDataType dataType = dataprd.DataType;
                string[] options = CreateLDFOptions(dataprd.ResolutionX, dataprd.ResolutionY);
                string prdsname="", subprdsname="";
                string outdir,bandfname; 
                foreach (string bname in checkbands.Keys)//datasets names
                {
                    prdsname = _group2prd[checkbands[bname]];
                    subprdsname = _sets2subPrds[bname];
                    outdir = Path.Combine(txtOutDir.Text, prdsname + "\\" + orgName + "_" + prdslevel + "\\" + year + "\\" + month);//_allFiles[filename].Substring(0, _allFiles[filename].LastIndexOf("."));
                    bandfname = prdsname + "_" + subprdsname + "_" + orgName + "_" + prdslevel + "_" + regionCode + "_" + year + "_" + month + "_" + UTCtime + ".LDF";
                    bandfname = Path.Combine(outdir, bandfname);
                    IRasterBand band = dataprd.GetRasterBand(_sets2bandNO[bname]);
                    buffer = new float[width * height];
                    fixed (float* ptr = buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(xOffset, yOffset, width, height, bufferPtr, dataType, width, height);
                    }
                    TryCreateLDFFile(bandfname, 1, width, height, buffer, options, dataType);
                }
            }
        }

        private string[] CreateLDFOptions(float resX,float resY)
        {
            string bandNames = "";
            ISpatialReference spatialRef = SpatialReference.GetDefault();
            string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + spatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + _env.MinX + "," + _env.MaxY + "}:{" + resX + "," + resY + "}",
                            "BANDNAMES="+ bandNames
                        };
            return options;
        }

        private void TryCreateLDFFile(string bandfname, int bandNO, int xSize, int ySize, float[] buffer, string[] options, enumDataType dataType)
        {
            string dstDir  =Path.GetDirectoryName(bandfname);
            if (!Directory.Exists(dstDir))
            {
                Directory.CreateDirectory(dstDir);
            }
            if (File.Exists(bandfname))
            {
                File.Delete(bandfname);
            }
            IRasterDataDriver driver = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            using (IRasterDataProvider bandRaster = driver.Create(bandfname, xSize, ySize, 1, dataType, options) as IRasterDataProvider)
            {
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    bandRaster.GetRasterBand(1).Write(0, 0, xSize, ySize, handle.AddrOfPinnedObject(), enumDataType.Float, xSize, ySize);
                }
                finally
                {
                    handle.Free();
                }
            }
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }




    }
}
