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
    public partial class frmStatProvinceRegionTemplates : Form
    {
        protected Size _size = Size.Empty;
        protected float _resolution = 0;
        protected readonly string _customVectorRegionFilename = null;
        private List<string> _customRegions = null;
        List<VectorTempFeatureCategory> _categories = new List<VectorTempFeatureCategory>();
        List<Feature> _selectFeature = new List<Feature>();
        private string _selectUrl = null;
        private Envelope _envelope = null;

        public frmStatProvinceRegionTemplates()
        {
            InitializeComponent();
            _customVectorRegionFilename = AppDomain.CurrentDomain.BaseDirectory + "CustomVectorRegion.mainfestfile";
            InitCategories();
        }

        public frmStatProvinceRegionTemplates(Size size, CoordEnvelope coordEenvelope, float resolution)
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
            listView1.Items.Clear();
            listView1.Columns.Clear();
            cbFields.Items.Clear();
            VectorTempFeatureCategory c = new VectorTempFeatureCategory("省级行政区域", VectorAOITemplate.FindVectorFullname("省级行政区域_面.shp"));
                if (c != null)
                {
                    Feature[] fets = c.LoadFeatures();
                    if (fets == null || fets.Length == 0)
                        return;
                    AddVectoreFeaturesToListView(fets);
                    _selectUrl = c.Urls[0];
                }
            LoadCustomRegionFiles();
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
                throw new Exception("frmStatProvinceRegionTemplates ：LoadCustomRegionFiles ", ex);
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
                throw new Exception("frmStatProvinceRegionTemplates:SaveCustomRegionToFile", ex);
            }
        }

        private void AddVectoreFeaturesToListView(Feature[] fets)
        {
            string[] fieldNames = fets[0].FieldNames;
            int idx = 0;
            int ifld = 0;
            foreach (string fld in fieldNames)
            {
                if (fld != "GBCODE" || fld != "ADCODE99")
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
            }
            cbFields.SelectedIndex = idx;
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
