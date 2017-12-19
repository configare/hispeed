using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public partial class frmStatSubRegionTemplatesMWS : Form
    {
        protected Size _size = Size.Empty;
        protected float _resolution = 0;
        protected readonly string _customVectorRegionFilename = null;
        private List<string> _customRegions = null;
        List<VectorTempFeatureCategory> _categories = new List<VectorTempFeatureCategory>();
        List<Feature> _selectFeature = new List<Feature>();
        private string _selectUrl = null;
        private Envelope _envelope = null;

        public frmStatSubRegionTemplatesMWS()
        {
            InitializeComponent();
            _customVectorRegionFilename = AppDomain.CurrentDomain.BaseDirectory + "CustomVectorRegion.mainfestfile";
            InitCategories();
        }

        public frmStatSubRegionTemplatesMWS(Size size, CoordEnvelope coordEenvelope, float resolution)
        {
            _size = size;
            _envelope = new Envelope(coordEenvelope.MinX, coordEenvelope.MinY, coordEenvelope.MaxX, coordEenvelope.MaxY);
            _resolution = resolution;
            InitializeComponent();
            _customVectorRegionFilename = AppDomain.CurrentDomain.BaseDirectory + "CustomVectorRegion.mainfestfile";
            InitCategories();
        }

        public Feature[] GetStatFeatures(out string fieldName, out string shapeFilename, out int filedIndex)
        {
            fieldName = cbFields.Text;
            shapeFilename = _selectUrl;
            GetFiledIndex(out filedIndex, fieldName);
            return _selectFeature == null || _selectFeature.Count == 0 ? null : _selectFeature.ToArray();
        }

        private GeoDo.Project.ISpatialReference _spatialReference = null;

        /// <summary>
        /// 支持定义投影坐标
        /// </summary>
        public GeoDo.Project.ISpatialReference SpatialRef
        {
            get 
            { 
                return _spatialReference;
            }
            set 
            {
                _spatialReference = value; 
            }
        }

        public Feature[] GetSelectedFeatures()
        {
            return _selectFeature.Count > 0 ? _selectFeature.ToArray() : null;
        }

        public Envelope GetSelectedEnvelope()
        {
            if (_selectFeature.Count == 0)
                return null;
            Envelope envelope = _selectFeature[0].Geometry.Envelope;
            for (int i = 1; i < _selectFeature.Count; i++)
            {
                envelope.UnionWith(_selectFeature[i].Geometry.Envelope);
            }
            return envelope;
        }

        public Dictionary<string, int[]> GetFeatureAOIIndex()
        {
            int filedIndex = -1;
            GetFiledIndex(out filedIndex, cbFields.Text);
            if (_selectFeature == null || _selectFeature.Count == 0 || filedIndex == -1)
                return null;
            Dictionary<string, int[]> result = new Dictionary<string, int[]>();
            int featuresLength = _selectFeature.Count;
            List<int> tempInt = new List<int>();

            VectorAOIGenerator vg = new VectorAOIGenerator();
            int[] aoi = null;
            string currentFiledValue = string.Empty;
            for (int i = 0; i < featuresLength; i++)
            {
                Feature feature = _selectFeature[i];
                if (_spatialReference != null && _spatialReference.ProjectionCoordSystem != null && !feature.Projected)
                {
                    using (GeoDo.Project.IProjectionTransform transform = GeoDo.Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), _spatialReference))
                    {
                        using (ShapePolygon spolygon = ShapePolygonPrjToGeo(transform, feature.Geometry as ShapePolygon))
                        {
                            aoi = vg.GetAOI(new ShapePolygon[] { spolygon }, _envelope, _size);
                        }
                    }
                }
                else
                {
                    ShapePolygon spolygon = _selectFeature[i].Geometry as ShapePolygon;
                    aoi = vg.GetAOI(new ShapePolygon[] { spolygon }, _envelope, _size);
                }
                if (aoi == null || aoi.Length == 0)
                    continue;
                currentFiledValue = _selectFeature[i].FieldValues[filedIndex];
                if (result.ContainsKey(currentFiledValue))
                {
                    tempInt.AddRange(result[currentFiledValue]);
                    tempInt.AddRange(aoi);
                    result[currentFiledValue] = tempInt.ToArray();
                }
                else
                    result.Add(currentFiledValue, aoi);
                tempInt.Clear();
            }
            return result.Count == 0 ? null : result;
        }

        private ShapePolygon ShapePolygonPrjToGeo(GeoDo.Project.IProjectionTransform transform, ShapePolygon shapePolygon)
        {
            if (shapePolygon == null)
                return null;
            double prjX;
            double prjY;
            int ringLength = shapePolygon.Rings.Length;
            ShapeRing[] ring = new ShapeRing[ringLength];
            int potsLength = 0;
            for (int i = 0; i < ringLength; i++)
            {
                if (shapePolygon.Rings[i].Points == null)
                    continue;
                potsLength = shapePolygon.Rings[i].Points.Length;
                ShapePoint[] shpPoint = new ShapePoint[potsLength];
                for (int j = 0; j < shapePolygon.Rings[i].Points.Length; j++)
                {
                    //ShapePoint point = new ShapePoint(shapePolygon.Rings[i].Points[j].X, shapePolygon.Rings[i].Points[j].Y);
                    double[] x = new double[] { shapePolygon.Rings[i].Points[j].X };
                    double[] y = new double[] { shapePolygon.Rings[i].Points[j].Y };
                    transform.Transform(x, y);
                    shpPoint[j] = new ShapePoint(x[0], y[0]); ;
                }
                ring[i] = new ShapeRing(shpPoint);
            }
            ShapePolygon prjSp = new ShapePolygon(ring);
            return prjSp;
        }
            
        private ShapePolygon ShapePolygonGeoToPrj(IProjectionTransform transform, ShapePolygon shapePolygon)
        {
            if (shapePolygon == null || shapePolygon.IsProjected)
                return null;
            double prjX;
            double prjY;
            int ringLength = shapePolygon.Rings.Length;
            ShapeRing[] ring = new ShapeRing[ringLength];
            int potsLength = 0;
            for (int i = 0; i < ringLength; i++)
            {
                if (shapePolygon.Rings[i].Points == null)
                    continue;
                potsLength = shapePolygon.Rings[i].Points.Length;
                ShapePoint[] shpPoint = new ShapePoint[potsLength];
                for (int j = 0; j < shapePolygon.Rings[i].Points.Length; j++)
                {
                    //tran.Geo2Prj(shapePolygon.Rings[i].Points[j].X, shapePolygon.Rings[i].Points[j].Y, out prjX, out prjY);transform
                    //ShapePoint point = new ShapePoint(prjX, prjY);
                    //shpPoint[j] = point;
                    ShapePoint point = shapePolygon.Rings[i].Points[j];
                    transform.Transform(point);
                    shpPoint[j] = point;
                }
                ring[i] = new ShapeRing(shpPoint);
            }
            ShapePolygon prjSp = new ShapePolygon(ring);
            return prjSp;
        }

        private void GetFiledIndex(out int filedIndex, string fieldName)
        {
            filedIndex = -1;
            if (_selectFeature == null || _selectFeature.Count == 0)
                return;
            Feature firstFeature = _selectFeature[0];
            for (int i = 0; i < firstFeature.FieldNames.Length; i++)
            {
                if (firstFeature.FieldNames[i] == fieldName)
                    filedIndex = i;
            }
        }

        private void InitCategories()
        {
            treeView1.Nodes.Clear();
            //
            //VectorTempFeatureCategory wholeCategory = new VectorTempFeatureCategory("当前区域");
            //_categories.Add(wholeCategory);
            //
            VectorTempFeatureCategory chinaCategory = new VectorTempFeatureCategory("中国边界", VectorAOITemplate.FindVectorFullname("中国边界.shp"));
            _categories.Add(chinaCategory);
            //
            VectorTempFeatureCategory chinaAdminsCategory = new VectorTempFeatureCategory("省级行政区域", VectorAOITemplate.FindVectorFullname("省级行政区域_面.shp"));
            _categories.Add(chinaAdminsCategory);
            //
            VectorTempFeatureCategory cityCategory = new VectorTempFeatureCategory("地区行政区域", VectorAOITemplate.FindVectorFullname("市级行政区域_面.shp"));
            _categories.Add(cityCategory);
            //
            VectorTempFeatureCategory countryCategory = new VectorTempFeatureCategory("县市行政区域", VectorAOITemplate.FindVectorFullname("县级行政区域_面.shp"));
            _categories.Add(countryCategory);
            ////
            //VectorTempFeatureCategory landUseTypeCategory = new VectorTempFeatureCategory("土地利用类型", VectorAOITemplate.FindVectorFullname("土地利用类型_合并.shp"));
            //_categories.Add(landUseTypeCategory);
            ////
            //VectorTempFeatureCategory waterBackCategory = new VectorTempFeatureCategory("湖泊", VectorAOITemplate.FindVectorFullname("中国湖泊.shp"));
            //_categories.Add(waterBackCategory);
            //LoadCustomRegionFiles();
            //
            foreach (VectorTempFeatureCategory c in _categories)
            {
                TreeNode tn = new TreeNode(c.Name);
                tn.Tag = c;
                treeView1.Nodes.Add(tn);
            }
            treeView1.ExpandAll();
            //
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        private void LoadCustomRegionFiles()
        {
            try
            {
                if (_customVectorRegionFilename == null || !File.Exists(_customVectorRegionFilename))
                    return;
                string[] files = File.ReadAllLines(_customVectorRegionFilename, Encoding.Default);
                if (files == null || files.Length == 0)
                    return;
                foreach (string f in files)
                {
                    string[] parts = f.Split('$');
                    if (parts.Length != 2)
                        continue;
                    VectorTempFeatureCategory c = new VectorTempFeatureCategory(parts[0].ToUpper(), parts[1]);
                    if (_customRegions == null)
                        _customRegions = new List<string>();
                    _customRegions.Add(parts[0].ToUpper());
                    _categories.Add(c);
                    _customRegions.Add(parts[0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("frmStatSubRegionTemplate ：LoadCustomRegionFiles ", ex);
            }
        }

        public VectorTempFeatureCategory FeatureCategory
        {
            get
            {
                if (treeView1.SelectedNode != null)
                    return treeView1.SelectedNode.Tag as VectorTempFeatureCategory;
                return null;
            }
        }

        public void SetMultitSelect(bool isCan)
        {
            listView1.MultiSelect = isCan;
            btnSelectAll.Visible = btnUnSelectAll.Visible = isCan;
        }

        public void SetbtnSelectAllEnable(bool isCan)
        {
            btnSelectAll.Enabled = isCan;
        }

        public void SetbtnUnSelectAllEnable(bool isCan)
        {
            btnUnSelectAll.Enabled = isCan;
        }

        private void SetFeatures(int selectedNum)
        {
            if (treeView1.Nodes.Count < selectedNum)
                return;
            TreeNode landNode = treeView1.Nodes[selectedNum];
            VectorTempFeatureCategory c = landNode.Tag as VectorTempFeatureCategory;
            if (c != null)
            {
                Feature[] fets = c.LoadFeatures();
                if (fets == null || fets.Length == 0)
                    return;
                AddVectoreFeaturesToListView(fets);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            CollectFeatures();
            Close();
        }

        private void CollectFeatures()
        {
            if (listView1.SelectedIndices.Count == 0)
                return;
            _selectFeature.Clear();
            foreach (ListViewItem it in listView1.SelectedItems)
            {
                Feature fet = it.Tag as Feature;
                _selectFeature.Add(fet);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnMoreSource_Click(object sender, EventArgs e)
        {
            _selectFeature.Clear();
            using (frmMaskTemplate frm = new frmMaskTemplate(_size,
                //_leftUpCoord, 
                _resolution))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    //MaskTemplateObj[] objs = frm.MaskObject;
                    //if (objs != null && objs.Length > 0)
                    //{
                    //    _maskObjs.AddRange(objs);
                    //}
                    if (frm.Filename != null)
                    {
                        PersistAddFileToList(frm.Filename);
                    }
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void PersistAddFileToList(string filname)
        {
            DialogResult isYes = MsgBox.ShowQuestionYesNo("要将矢量文件添加的\"已定义的矢量区域列表\"中吗?\n" +
                                                                                       "(下次不需要再选择文件)\n\n" +
                "按[是(Y)]添加到矢量区域列表,按[否(N)]不添加。");
            if (isYes == DialogResult.Yes)
            {
                string name = frmRename.ReName(Path.GetFileName(filname), new frmRename.IsRepeatNameChecker(NameIsRepeat));
                SaveCustomRegionToFile(name, filname);
                InitCategories();
            }
        }

        private void SaveCustomRegionToFile(string name, string filname)
        {
            if (_customVectorRegionFilename == null || name == null || filname == null)
                return;
            try
            {
                using (StreamWriter sw = new StreamWriter(_customVectorRegionFilename, true, Encoding.Default))
                {
                    sw.WriteLine(name + "$" + filname);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("frmStatSubRegionTemplate:SaveCustomRegionToFile", ex);
            }
        }

        private bool NameIsRepeat(string name)
        {
            if (treeView1.Nodes.Count == 0)
                return false;
            name = name.ToUpper();
            foreach (TreeNode tn in treeView1.Nodes)
                if (tn.Text.ToUpper() == name)
                    return true;
            return false;
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (notLoadFeatures || e.Button == MouseButtons.Right)
                return;
            TreeNode tn = treeView1.GetNodeAt(e.Location);
            if (tn == null)
                return;
            AddTreeListViewItems(tn);
        }

        private void AddTreeListViewItems(TreeNode tn)
        {
            listView1.Items.Clear();
            listView1.Columns.Clear();
            cbFields.Items.Clear();
            if (tn.Tag != null)
            {
                VectorTempFeatureCategory c = tn.Tag as VectorTempFeatureCategory;
                if (c != null)
                {
                    Feature[] fets = c.LoadFeatures();
                    if (fets == null || fets.Length == 0)
                        return;
                    AddVectoreFeaturesToListView(fets);
                    _selectUrl = c.Urls[0];
                }
            }
        }

        private void AddVectoreFeaturesToListView(Feature[] fets)
        {
            string[] fieldNames = fets[0].FieldNames;
            int idx = 0;
            int ifld = 0;
            foreach (string fld in fieldNames)
            {
                cbFields.Items.Add(fld);
                ColumnHeader header = new ColumnHeader();
                header.Text = fld;
                if (fld == "土地利用类型" || fld.ToUpper().Contains("NAME") ||
                    fld.Contains("名称") || fld.ToUpper().Contains("CHINESE"))
                {
                    header.Width = 130;
                    if (!fld.ToUpper().Contains("ENAME"))
                        idx = ifld;
                }
                else
                {
                    header.Width = 100;
                }
                ifld++;
                listView1.Columns.Add(header);
            }
            cbFields.SelectedIndex = idx;
            //
            foreach (Feature fet in fets)
            {
                ListViewItem it = new ListViewItem();
                string[] fieldValues = fet.FieldValues;
                if (fieldValues == null || fieldValues == null)
                {
                    it.Text = fet.ToString();
                    continue;
                }
                it.Text = fieldValues[0];
                for (int i = 1; i < fieldValues.Length; i++)
                {
                    it.SubItems.Add(fieldValues[i]);
                }
                it.Tag = fet;
                listView1.Items.Add(it);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem it in listView1.Items)
            {
                it.Selected = true;
                it.Checked = listView1.CheckBoxes;
            }
            listView1.Focus();
        }

        private void btnUnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem it in listView1.Items)
            {
                it.Selected = false;
                it.Checked = listView1.CheckBoxes;
            }
            listView1.Focus();
        }

        private void frmStatSubRegionTemplates_Load(object sender, EventArgs e)
        {

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            if (_customRegions == null)
                return;
            if (_customRegions.Contains(treeView1.SelectedNode.Text))
            {
                //
                string[] lines = File.ReadAllLines(_customVectorRegionFilename, Encoding.Default);
                List<string> newLines = new List<string>();
                foreach (string l in lines)
                    if (!l.ToUpper().Contains(treeView1.SelectedNode.Text + "$"))
                        newLines.Add(l);
                File.WriteAllLines(_customVectorRegionFilename, newLines.ToArray(), Encoding.Default);
                //
                treeView1.Nodes.Remove(treeView1.SelectedNode);
            }
        }

        private bool notLoadFeatures = false;
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode tn = treeView1.GetNodeAt(e.Location);
            if (tn != null)
            {
                if (e.Button == MouseButtons.Right)
                    notLoadFeatures = true;
                treeView1.SelectedNode = tn;
                notLoadFeatures = false;
            }
        }

        public TreeNode SelectTreeNode
        {
            get { return treeView1.SelectedNode; }
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _customRegions != null && _customRegions.Contains(treeView1.SelectedNode.Text))
            {
                contextMenuStrip1.Show(treeView1, e.Location);
            }
        }

        private int _firstItem = 0;
        private void btSearch_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0 || string.IsNullOrEmpty(txtSearch.Text))
                return;
            _firstItem = 0;
            GetListViewItemNext();
        }

        private void GetListViewItemNext()
        {
            if (listView1.Items.Count <= _firstItem)
            {
                MsgBox.ShowInfo("已搜索到最后一行!");
                return;
            }
            int length = listView1.Items.Count;
            for (int i = _firstItem + 1; i < length; i++)
            {
                if (IncludeString(listView1.Items[i]))
                {
                    listView1.Items[i].Selected = true;
                    listView1.Items[i].EnsureVisible();
                    _firstItem = i;
                    return;
                }
            }
            MsgBox.ShowInfo("已搜索到最后一行!");
        }

        private bool IncludeString(ListViewItem listViewItem)
        {
            listView1.Focus();
            int length = listViewItem.SubItems.Count;
            for (int i = 0; i < length; i++)
            {
                if (listViewItem.SubItems[i].Text.IndexOf(txtSearch.Text) != -1)
                {
                    listView1.Items[_firstItem].Selected = false;
                    return true;
                }
            }
            return false;
        }

        private void GetListViewItemUp()
        {
            if (listView1.Items.Count <= _firstItem)
                _firstItem = listView1.Items.Count - 1;
            if (_firstItem <= 0)
            {
                MsgBox.ShowInfo("已搜索到第一行!");
                return;
            }
            for (int i = _firstItem - 1; i >= 0; i--)
            {
                if (IncludeString(listView1.Items[i]))
                {
                    listView1.Items[i].Selected = true;
                    listView1.Items[i].EnsureVisible();
                    _firstItem = i;
                    return;
                }
            }
            MsgBox.ShowInfo("已搜索到第一行!");
        }

        private void btUp_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0 || string.IsNullOrEmpty(txtSearch.Text))
                return;
            GetListViewItemUp();
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0 || string.IsNullOrEmpty(txtSearch.Text))
                return;
            GetListViewItemNext();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btSearch_Click(sender, e);
        }

        #region 新增“统计区域另存为方案”功能，zyb,2014.06.05
        private void btnUseCustomBlock_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count < 1)
            {
                MessageBox.Show("不存在可选择的矢量区域！");
                return;
            }
            try
            {
                CustomAOISolutionArgs arg = new CustomAOISolutionArgs();
                string[] slsNames = arg.SolutionNames;
                String solutionName=null;
                string vecName, kryfName;
                string[] aoiNames;
                frmApplyCustomAOISolution frm = new frmApplyCustomAOISolution(slsNames);
                frm.Left = this.Right + 5;
                frm.Top = this.Top;
                if (frm.ShowDialog() == DialogResult.OK)
                    solutionName = frm.SelectedSolutionName;
                else
                    return;
                if (solutionName!=null)
                {
                    arg.GetUniqueSolution(solutionName,out vecName,out kryfName,out aoiNames);
                    foreach (TreeNode node in treeView1.Nodes)
                    {
                        if (node.Text.ToUpper() == vecName.ToUpper())
                        {
                            node.Checked=true;
                            //AddTreeListViewItems(node);
                            break;
                        }
                    }
                    int idx=-1;
                    foreach (ColumnHeader col in listView1.Columns)
                    {
                        if (col.Text == kryfName)
                        {
                            idx = col.Index;
                            break;
                        }
                    }
                    foreach (ListViewItem it in this.listView1.Items)
                    {
                        foreach (string aoiname in aoiNames)
                        {
                            if (it.SubItems[idx].Text == aoiname)
                            {
                                it.Selected = true;
                                it.Checked = listView1.CheckBoxes;
                                break;
                            }
                        }
                    }
                    listView1.Focus();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddCustomProject_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeView1.SelectedNode == null)
                    throw new ArgumentException("请选择矢量区域！");
                if (listView1.Items.Count == 0 || listView1.SelectedItems.Count < 1)
                    throw new ArgumentException("请选择统计区域！");
                string vectorName = treeView1.SelectedNode.Text;
                string[] fieldNames = (listView1.Items[0].Tag as Feature).FieldNames;
                int idx = 0;
                string keyfieldName = null;
                int ifld = 0;
                foreach (string fld in fieldNames)
                {
                    if (fld.ToUpper().Contains("NAME") ||
                        fld.Contains("名称") || fld.ToUpper().Contains("CHINESE"))
                    {
                        if (!fld.ToUpper().Contains("ENAME"))
                        {
                            idx = ifld;
                            keyfieldName = fld;
                            break;
                        }
                    }
                    ifld++;
                }
                List<string> selectedKeyValues = new List<string>();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    Feature fea = item.Tag as Feature;
                    if (keyfieldName == null)
                        keyfieldName = fea.FieldNames[idx];
                    selectedKeyValues.Add(fea.FieldValues[idx]);
                }
                CustomAOISolutionArgs arg = new CustomAOISolutionArgs();
                frmAddNewCustomAOISolution frm = new frmAddNewCustomAOISolution(arg.SolutionNames);
                frm.Left = this.Right + 5;
                frm.Top = this.Top;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm.IsDeleteMode)
                    {
                        arg._tryDeletedSolutions = frm.DeletedSolutions;
                        arg.UpdateDeleteSolutions();
                    }
                    if (frm.IsAddMode)
                    {
                        arg._newSolutionName = frm.NewCustomAOISolutionName;
                        arg._vectorName = vectorName;
                        arg._keyFieldName = keyfieldName;
                        arg._statRegionNames = selectedKeyValues.ToArray();
                        arg.ToXml();
                    }
                    MessageBox.Show("自定义方案编辑成功！");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node!=null)
                AddTreeListViewItems(e.Node);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
