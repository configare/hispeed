using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.HDF;
using GeoDo.HDF4;
using OSGeo.GDAL;
using System.IO;
using GeoDo.HDF5;
using GeoDo.Project;

namespace GeoDo.RSS.DF.GDAL.HDF5Universal
{
    /// <summary>
    /// 选择hdf5数据集，用于打开hdf5使用。
    /// </summary>
    public partial class Hdf5DatasetSelection : Form
    {
        private string _filename;
        private string[] _dsNames = null;
        private Dictionary<string, Dictionary<string, string>> _dsAttributes = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> _fileAttributes = new Dictionary<string, string>();
        private string _selectedDsNames;
        private string _geoInfo;
        private string _prjInfo;
        private bool _isHdf4;
        private bool _isHdf5;

        public Hdf5DatasetSelection()
        {
            InitializeComponent();
            Gdal.AllRegister();
            groupBox4.Visible = false;
            tvDatasets.CheckBoxes = true;
            _isHdf4 = false;
            _isHdf5 = false;
            tvDatasets.AfterCheck += new TreeViewEventHandler(tvDatasets_AfterCheck);
            tvDatasets.AfterSelect += new TreeViewEventHandler(tvDatasets_AfterSelect);
        }

        public string SelectedDatasets
        {
            get { return _selectedDsNames; }
        }

        public string GeoInfo
        {
            get
            {
                return _geoInfo;
            }
        }

        public string PrjInfo
        {
            get
            {
                return _prjInfo;
            }
        }

        public string FileName
        {
            get { return _filename; }
        }

        public string[] GetOpenOptions()
        {
            List<string> hdfOptions = new List<string>();
            if (!string.IsNullOrWhiteSpace(_selectedDsNames))
                hdfOptions.Add("datasets=" + _selectedDsNames);
            if (!string.IsNullOrWhiteSpace(_geoInfo))
                hdfOptions.Add("geoinfo=" + _geoInfo);
            if (!string.IsNullOrWhiteSpace(_prjInfo))
                hdfOptions.Add("proj4=" + _prjInfo);
            return hdfOptions.ToArray();
        }

        void tvDatasets_AfterSelect(object sender, TreeViewEventArgs e)
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

        public void LoadFile(string filename)
        {
            _filename = filename;
            textBox1.Text = _filename;
            SetInputFile(_filename);
        }

        private void SetInputFile(string filename)
        {
            if (GeoDo.HDF5.HDF5Helper.IsHdf5(filename))
                _isHdf5 = true;
            else if (GeoDo.HDF4.HDF4Helper.IsHdf4(filename))
                _isHdf4 = true;
            else
            {
                ShowMsgBox("即不是HDF5数据，也不是HDF4数据，无法打开！");
                return;
            }

            textBox1.Text = filename;
            SetInputFileDataSet(filename);
            //UpdateOutPutFilename(filename);
        }

        //private void UpdateOutPutFilename(string inFilename)
        //{
        //    string filename = Path.GetFileNameWithoutExtension(inFilename);
        //    //DataIdentify df = DataIdentifyMatcher.Match(inFilename);

        //    string[] names = filename.Split('_');
        //    if (names.Length > 8)
        //    {
        //        names[2] = "GLL";
        //    }
        //    string newFilename = names[0];
        //    for (int i = 1; i < names.Length; i++)
        //    {
        //        string name = names[i];
        //        newFilename = newFilename + "_" + name;
        //    }
        //    string filenameExt = ".LDF";
        //    textBox3.Text = Path.Combine(Path.GetDirectoryName(inFilename), newFilename + filenameExt);
        //}

        private void ShowMsgBox(string msg)
        {
            MessageBox.Show(msg, "信息提示");
        }

        private void SetInputFileDataSet(string filename)
        {
            using (IHdfOperator oper = HdfOperatorFactory(filename))
            {
                if (oper == null)
                    return;
                GetAttrs(oper);
            }

            FillTreeView(filename);
            TrySetGeoInfo();
        }

        private IHdfOperator HdfOperatorFactory(string filename)
        {
            if (_isHdf5) return new Hdf5Operator(filename);
            if (_isHdf4) return new Hdf4Operator(filename);
            return null;
        }

        private void GetAttrs(IHdfOperator oper)
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

        private void TrySetGeoInfo()
        {
            if (_fileAttributes == null)
                return;
            double minX = 0d, maxX = 0d, minY = 0d, maxY = 0d, resolutionX = 0d, resolutionY = 0d;
            if (_fileAttributes.ContainsKey("Bottom-Right Latitude"))
            {
                string minLat = _fileAttributes["Bottom-Right Latitude"];
                if (double.TryParse(minLat, out minY))
                {
                    geoRange1.MinY = minY;
                }
            }
            if (_fileAttributes.ContainsKey("Left-Top Longtitude"))
            {
                string minlon = _fileAttributes["Left-Top Longtitude"];
                if (double.TryParse(minlon, out minX))
                    geoRange1.MinX = minX;
            }
            if (_fileAttributes.ContainsKey("Left-Top Latitude"))
            {
                string maxLat = _fileAttributes["Left-Top Latitude"];
                if (double.TryParse(maxLat, out maxY))
                {
                    geoRange1.MaxY = maxY;
                }
            }
            if (_fileAttributes.ContainsKey("Bottom-Right Longtitude"))
            {
                string maxLon = _fileAttributes["Bottom-Right Longtitude"];
                if (double.TryParse(maxLon, out maxX))
                    geoRange1.MaxX = maxX;
            }
            if (_fileAttributes.ContainsKey("Latitude Resolution"))
            {
                string latRegolution = _fileAttributes["Latitude Resolution"];
                if (double.TryParse(latRegolution, out resolutionY))
                    txtResolutionY.Text = latRegolution;
            }
            if (_fileAttributes.ContainsKey("Longtitude Resolution"))
            {
                string lonRegolution = _fileAttributes["Longtitude Resolution"];
                if (double.TryParse(lonRegolution, out resolutionX))
                    txtResolutionX.Text = lonRegolution;
            }
            //以下代码用于修正其部分数据在等经纬度时候，单位没有按照度写的问题，以及以150度为中心点时候的坐标范围问题。
            if (_fileAttributes.ContainsKey("Projection Type"))
            {
                string projectionType = _fileAttributes["Projection Type"];
                if (projectionType == "Geographic Longitude/Latitude")
                {
                    if (_fileAttributes.ContainsKey("Unit Of Resolution"))
                    {
                        string unit = _fileAttributes["Unit Of Resolution"];
                        if (unit == "Meter")
                        {
                            if (resolutionX != 0d)
                            {
                                txtResolutionX.Text = (resolutionX / 100000d).ToString();
                            }
                            if (resolutionY != 0d)
                            {
                                txtResolutionY.Text = (resolutionY / 100000d).ToString();
                            }
                        }
                    }
                    if (geoRange1.MinX == -30d && geoRange1.MaxX == -30d)
                    {
                        geoRange1.MaxX = 330d;
                    }
                }
            }
        }

        private void chkGeoInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGeoInfo.Checked)
                this.Width = 745;
            else
                this.Width = 580;
        }

        private void bntOk_Click(object sender, EventArgs e)
        {
            _selectedDsNames = GetSelectedDataSets();
            if (_selectedDsNames == null || _selectedDsNames.Length == 0)
            {
                ShowMsgBox("没有选择任何数据集");
                return;
            }
            if (chkGeoInfo.Checked)
            {
                _geoInfo = GetGeoInfo();
                if (string.IsNullOrWhiteSpace(_geoInfo))
                {
                    ShowMsgBox("坐标信息不正确");
                    return;
                }
                _prjInfo = GetPrjInfo();
                if (string.IsNullOrWhiteSpace(_prjInfo))
                {
                    ShowMsgBox("投影信息不正确");
                    return;
                }

            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private string GetPrjInfo()
        {
            try
            {
                ISpatialReference prjInfo = SpatialReference.GetDefault();
                PrimeMeridian centerLongitude = prjInfo.GeographicsCoordSystem.PrimeMeridian;
                centerLongitude.Value = 150d;
                return prjInfo.ToProj4String();
            }
            catch
            {
                return null;
            }
        }

        private string GetGeoInfo()
        {
            string geoInfo = null;
            double minx = geoRange1.MinX;
            double maxX = geoRange1.MaxX;
            double minY = geoRange1.MinY;
            double maxY = geoRange1.MaxY;
            if (string.IsNullOrWhiteSpace(txtResolutionX.Text))
                return null;
            if (string.IsNullOrWhiteSpace(txtResolutionY.Text))
                return null;
            geoInfo = string.Format("{0},{1},{2},{3},{4},{5}", minx, maxX, minY, maxY, txtResolutionX.Text, txtResolutionY.Text);
            return geoInfo;
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
            return string.IsNullOrWhiteSpace(dss) ? null : dss.TrimEnd(',');
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.Filter = "HDF数据(*.HDF,*.H5)|*.HDF;*.H5";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadFile(diag.FileName);
                }
            }
        }
    }
}
