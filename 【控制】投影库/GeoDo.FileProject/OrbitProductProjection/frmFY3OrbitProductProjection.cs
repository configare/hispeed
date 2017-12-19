#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-04 10:55:06
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.HDF5;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL.HDF5Universal;
using GeoDo.Project;
using System.Text.RegularExpressions;

namespace GeoDo.FileProject
{
    /// <summary>
    /// 类名：frmFY3L2L3OrbitProjection
    /// 属性描述：风三2、3级轨道产品投影
    /// 创建者：罗战克   创建日期：2013-09-04 10:55:06
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class frmFY3OrbitProductProjection : Form
    {
        private string[] _dsNames = null;
        private Dictionary<string, Dictionary<string, string>> _dsAttributes = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> _fileAttributes = new Dictionary<string, string>();

        public frmFY3OrbitProductProjection()
        {
            InitializeComponent();
            tvDatasets.AfterCheck += new TreeViewEventHandler(tvDatasets_AfterCheck);
        }

        void tvDatasets_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            if (e.Node.Level == 0)
            {
                CheckChangeALL(e.Node);
            }
        }

        private void CheckChangeALL(TreeNode root)
        {
            bool check = root.Checked;
            foreach (TreeNode node in root.Nodes)
            {
                if (node.Level == 1)
                    node.Checked = check;
            }
        }

        private void InputFile()
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.Title = "获取风三L2、L3级产品数据";
                diag.Filter = "*.HDF(*.HDF)|*.HDF";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetInputFile(diag.FileName);
                }
            }
        }

        private void SetInputFile(string filename)
        {
            if (!GeoDo.HDF5.HDF5Helper.IsHdf5(filename))
            {
                ShowMsgBox("非HDF5数据，不支持");
                return;
            }
            textBox1.Text = filename;
            SetInputFileDataSet(filename);
            UpdateOutPutFilename(filename);
            UpdateResolution(filename);
            IsL1OrbitFile(filename);
        }

        private Regex _fy3L1Regex = new Regex(@"^(?<satellite>\S*)_(?<sensor>\S*)_(?<bound>\S+)_(?<level>L1)_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})_(?<hour>\d{2})(?<minutes>\d{2})_(?<resolution>\d+)(?<units>\S+)_MS.HDF$");

        /// <summary>
        /// 针对L1级数据，直接选择自己为经纬度数据文件
        /// </summary>
        /// <param name="filename"></param>
        private void IsL1OrbitFile(string filename)
        {
            Match match = _fy3L1Regex.Match(filename);
            if (match.Success)
            {
                SetInputLocationFile(filename);
            }
        }

        private void UpdateOutPutFilename(string inFilename)
        {
            string filename = Path.GetFileNameWithoutExtension(inFilename);
            DataIdentify df = DataIdentifyMatcher.Match(inFilename);
            string[] names = filename.Split('_');
            if (names.Length > 8)
            {
                names[2] = "GLL";
            }
            string newFilename = names[0];
            for (int i = 1; i < names.Length; i++)
            {
                string name = names[i];
                newFilename = newFilename + "_" + name;
            }
            string filenameExt = ".LDF";
            textBox3.Text = Path.Combine(Path.GetDirectoryName(inFilename), newFilename + filenameExt);
        }

        private void ShowMsgBox(string msg)
        {
            MessageBox.Show(msg, "信息提示");
        }

        private void SetInputFileDataSet(string filename)
        {
            try
            {
                using (Hdf5Operator oper = new Hdf5Operator(filename))
                {
                    _dsNames = oper.GetDatasetNames;
                    _dsAttributes.Clear();
                    _fileAttributes.Clear();
                    foreach (string dsName in _dsNames)
                    {
                        Dictionary<string, string> dsAttrs = oper.GetAttributes(dsName);
                        _dsAttributes.Add(dsName, dsAttrs);
                    }
                    _fileAttributes = oper.GetAttributes();
                }
                FillTreeView(filename);
            }
            catch(Exception ex)
            {
                ShowMsgBox(ex.Message);
            }
        }

        private void FillTreeView(string filename)
        {
            tvDatasets.Nodes.Clear();
            TreeNode root = new TreeNode(Path.GetFileName(filename));
            foreach (string dsName in _dsNames)
            {
                root.Nodes.Add(dsName);
            }
            tvDatasets.Nodes.Add(root);
            tvDatasets.ExpandAll();
            tvDatasets.SelectedNode = root;
        }

        private void btnSrc_Click(object sender, EventArgs e)
        {
            InputFile();
        }

        private void tvDatasets_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                ShowFileAttrInfo();
            }
            else if (e.Node.Level == 1)
            {
                ShowDsAttrInfo(e.Node.Text);
            }
        }

        private void ShowFileAttrInfo()
        {
            ShowDics(_fileAttributes);
        }

        private void ShowDsAttrInfo(string dsName)
        {
            if (!_dsAttributes.ContainsKey(dsName))
                return;
            Dictionary<string, string> dic = _dsAttributes[dsName];
            ShowDics(dic);
        }

        private void ShowDics(Dictionary<string, string> dic)
        {
            txtAttrs.Clear();
            StringBuilder str = new StringBuilder();
            foreach (string attr in dic.Keys)
            {
                str.AppendLine(attr + "=" + dic[attr]);
            }
            txtAttrs.Text = str.ToString();
        }

        private void btnLocation_Click(object sender, EventArgs e)
        {
            InputGeoFile();
        }

        #region GeoFile
        private void InputGeoFile()
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.Title = "L1轨道数据";
                diag.Filter = "*.HDF(*.HDF)|*.HDF";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetInputLocationFile(diag.FileName);
                }
            }
        }

        private void SetInputLocationFile(string filename)
        {
            if (!GeoDo.HDF5.HDF5Helper.IsHdf5(filename))
            {
                ShowMsgBox("非HDF5数据，不支持");
                return;
            }
            using (Hdf5Operator oper = new Hdf5Operator(filename))
            {
                if (oper != null)
                {
                    textBox2.Text = filename;
                    FillGeoDatasets(oper.GetDatasetNames);
                }
            }
        }

        private void FillGeoDatasets(string[] geoDatasetNames)
        {
            string defaultLat = null;
            string defaultLon = null;
            cmbLat.Items.Clear();
            cmbLon.Items.Clear();
            foreach (string ds in geoDatasetNames)
            {
                cmbLat.Items.Add(ds);
                cmbLon.Items.Add(ds);
                if (ds.ToLower() == "latitude")
                    defaultLat = ds;
                else if (ds.ToLower() == "longitude")
                    defaultLon = ds;
            }
            if (!string.IsNullOrWhiteSpace(defaultLat))
                cmbLat.Text = defaultLat;
            if (!string.IsNullOrWhiteSpace(defaultLon))
                cmbLon.Text = defaultLon;
        }
        #endregion

        private void btnOutput_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog diag = new SaveFileDialog())
            {
                if (!string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    diag.InitialDirectory = Path.GetDirectoryName(textBox3.Text);
                    diag.FileName = Path.GetFileName(textBox3.Text);
                }
                diag.Title = "输出文件";
                diag.Filter = "*.LDF(*.LDF)|*.LDF";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBox3.Text = diag.FileName;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
                return;
            string dss = GetSelectedDataSets();
            if (dss == null || dss.Length == 0)
            {
                ShowMsgBox("需要选择要投影的数据集");
                return;
            }
            float resolutionX;
            float resolutionY;
            if (!float.TryParse(cmbResolutionX.Text, out resolutionX) || resolutionX == 0f)
            {
                ShowMsgBox("经度分辨率没有正确设置");
                return;
            }
            if (!float.TryParse(cmbResolutionY.Text, out resolutionY) || resolutionY == 0f)
            {
                ShowMsgBox("经度分辨率没有正确设置");
                return;
            }

            string mainfilename = textBox1.Text;
            string locationFilename = textBox2.Text;
            string outFilename = textBox3.Text;
            string lonlatDs = GetLongLatDatasetNames();

            IRasterDataProvider mainRaster = null;
            IRasterDataProvider locationRaster = null;
            Action<int, string> progress = new Action<int, string>(OnProgress);
            try
            {
                string[] openArgs = new string[] { "datasets=" + dss };
                mainRaster = RasterDataDriver.Open(mainfilename, openArgs) as IRasterDataProvider;
                if (mainRaster == null)
                {
                    ShowMsgBox("无法读取数据");
                    return;
                }
                if (mainRaster.DataType == enumDataType.Atypism)
                {
                    ShowMsgBox("不支持混合类型的数据");
                    return;
                }
                string[] locationArgs = new string[] { "datasets=" + lonlatDs, "geodatasets=" + lonlatDs };
                locationRaster = RasterDataDriver.Open(locationFilename, locationArgs) as IRasterDataProvider;
                if (locationRaster == null || locationRaster.BandCount == 0)
                {
                    ShowMsgBox("经纬度HDF数据文件，不存在经纬度数据集[Longitude,Latitude]");
                    return;
                }
                FY3L2L3FilePrjSettings setting = new FY3L2L3FilePrjSettings();
                setting.LocationFile = locationRaster;
                setting.OutFormat = "LDF";
                setting.OutPathAndFileName = outFilename;
                setting.OutResolutionX = resolutionX;
                setting.OutResolutionY = resolutionY;
                //Dictionary<string, double> args = new Dictionary<string, double>();
                //args.Add("xzoom", 0.000001d);
                //args.Add("yzoom", 0.000001d);
                //setting.ExtArgs = new object[] { args };
                FY3L2L3FileProjector projector = new FY3L2L3FileProjector();
                projector.Project(mainRaster, setting, SpatialReference.GetDefault(), progress);
                ShowMsgBox("投影结束");
            }
            catch (Exception ex)
            {
                ShowMsgBox(ex.Message);
            }
            finally
            {
                if (mainRaster != null)
                {
                    mainRaster.Dispose();
                    mainRaster = null;
                }
                if (locationRaster != null)
                {
                    locationRaster.Dispose();
                    locationRaster = null;
                }
            }
        }

        private string GetLongLatDatasetNames()
        {
            if (string.IsNullOrWhiteSpace(cmbLon.Text) || string.IsNullOrWhiteSpace(cmbLat.Text))
                return null;
            return cmbLon.Text + "," + cmbLat.Text;
        }

        private string GetSelectedDataSets()
        {
            if (tvDatasets.Nodes == null || tvDatasets.Nodes.Count == 0)
                return null;
            string dss = null;
            TreeNode root = tvDatasets.Nodes[0];
            foreach (TreeNode node in root.Nodes)
            {
                if (node.Checked)
                {
                    dss += node.Text + ",";
                }
            }
            if (dss.TrimEnd(',').IndexOf(',') == -1&&dss.ToUpper().Contains("LANDSEA"))
            {
                string outFile=textBox3.Text;
                textBox3.Text = Path.Combine(Path.GetDirectoryName(outFile), Path.GetFileNameWithoutExtension(outFile) + "_LandSeaMask" + Path.GetExtension(outFile));
            }
            return string.IsNullOrWhiteSpace(dss) ? null : dss.TrimEnd(',');;
        }

        private void btnCalcel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 输出分辨率
        //L1:FY3B_MWHSX_GBAL_L1_20130904_0123_015KM_MS.HDF
        //L2:FY3B_VIRRX_ORBT_L2_CLM_MLT_NUL_20130722_0455_1000M_MS.HDF
        //L3:FY3A_VIRRX_H010_L3_NVI_MLT_HAM_20130720_AOTD_1000M_MS.HDF
        //0250M\1000M\5000M\010KM\015KM\200KM\050KM\075KM\028KM\100KM
        private Regex _regexFY3Resolution = new Regex(@"_(((?<resolutionM>\d{4})M)|((?<resolutionKM>\d{3})KM))_", RegexOptions.Compiled);//L1

        private void UpdateResolution(string filename)
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            Match match = _regexFY3Resolution.Match(filename);
            if (match.Success)
            {
                Group group = match.Groups["resolutionM"];
                if (group.Success)
                {
                    string resolutionStr = group.Value;
                    float resolution;
                    if (TryGetHDFResolution(resolutionStr, "M", out resolution))
                    {
                        cmbResolutionX.Text = resolution.ToString();
                        cmbResolutionY.Text = resolution.ToString();
                    }
                }
                else
                {
                    group = match.Groups["resolutionKM"];
                    if (group.Success)
                    {
                        string resolutionStr = group.Value;
                        float resolution;
                        if (TryGetHDFResolution(resolutionStr, "KM", out resolution))
                        {
                            cmbResolutionX.Text = resolution.ToString();
                            cmbResolutionY.Text = resolution.ToString();
                        }
                    }
                }
            }
        }

        private bool TryGetHDFResolution(string resolutionStr, string unit, out float resolution)
        {
            resolution = 0;
            unit = unit.ToUpper();
            float ut = 0;
            if (unit == "M")
            {
                ut = 100000f;
            }
            else if (unit == "KM")
            {
                ut = 100f;
            }
            else
            {
                return false;
            }
            int resolutionM = 0;
            if (int.TryParse(resolutionStr, out resolutionM))
            {
                resolution = resolutionM / ut;
                return true;
            }
            return false;
        }

        private void lbLinkResolution_Click(object sender, EventArgs e)
        {
            if (lbLinkResolution.Text == "+")
            {
                lbLinkResolution.Text = "-";
                cmbResolutionY.Enabled = true;
            }
            else
            {
                lbLinkResolution.Text = "+";
                cmbResolutionY.Enabled = false;
                if (cmbResolutionY.Text != cmbResolutionX.Text)
                {
                    try
                    {
                        //_isBegingChangedValue = true;
                        cmbResolutionY.Text = cmbResolutionX.Text;
                    }
                    finally
                    {
                        //_isBegingChangedValue = false;
                    }
                }
            }
        }

        private void cmbResolutionX_TextChanged(object sender, EventArgs e)
        {
            if (lbLinkResolution.Text == "+")
            {
                try
                {
                    //_isBegingChangedValue = true;
                    cmbResolutionY.Text = cmbResolutionX.Text;
                }
                finally
                {
                    //_isBegingChangedValue = false;
                }
            }
            DoValueChanged();
        }

        private void cmbResolutionY_TextChanged(object sender, EventArgs e)
        {
            DoValueChanged();
        }

        private void DoValueChanged()
        {
            //if (_isBegingChangedValue)
            return;
        }
        #endregion

        #region 进度条
        private void OnProgress(int p, int msg)
        {
            progressBar1.Value = p;
        }

        private void OnProgress(int progerss, string text)
        {
            if (InvokeRequired)
                this.Invoke(new Action<int, string>(UpdateProgress), progerss, text);
            else
                UpdateProgress(progerss, text);
        }

        private void UpdateProgress(int progerss, string text)
        {
            if (progerss > progressBar1.Maximum)
                return;
            progressBar1.Value = progerss;
            progressBar1.Text = text;
            Application.DoEvents();
        }
        #endregion
    }
}
