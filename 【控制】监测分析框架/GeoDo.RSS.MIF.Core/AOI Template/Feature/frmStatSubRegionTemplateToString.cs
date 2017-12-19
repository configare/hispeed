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

namespace GeoDo.RSS.MIF.Core
{
    public partial class frmStatSubRegionTemplateToString : Form
    {
        protected Size _size = Size.Empty;
        protected float _resolution = 0;
        protected readonly string _customVectorRegionFilename = null;
        private List<string> _customRegions = null;
        List<VectorTempFeatureCategory> _categories = new List<VectorTempFeatureCategory>();
        List<Feature> _selectFeature = new List<Feature>();
        private string _selectUrl = null;
        private Envelope _envelope = null;

        public frmStatSubRegionTemplateToString()
        {
            InitializeComponent();
            _customVectorRegionFilename = AppDomain.CurrentDomain.BaseDirectory + "CustomVectorRegion.mainfestfile";
            InitCategories();
        }

        public frmStatSubRegionTemplateToString(Size size, CoordEnvelope coordEenvelope, float resolution)
        {
            _size = size;
            _envelope = new Envelope(coordEenvelope.MinX, coordEenvelope.MinY, coordEenvelope.MaxX, coordEenvelope.MaxY);
            _resolution = resolution;
            InitializeComponent();
            _customVectorRegionFilename = AppDomain.CurrentDomain.BaseDirectory + "CustomVectorRegion.mainfestfile";
            InitCategories();
        }

        public SelectedFeaturesInfo GetSelectedFeatureInfo()
        {
            if(_selectFeature==null||_selectFeature.Count<1)
                return null;
            string fieldName = cbFields.Text;
            string url = _selectUrl;
            List<string> fieldValues=new List<string>();
            string value;
            foreach (Feature item in _selectFeature)
            {
                value=item.GetFieldValue(fieldName);
                if (!string.IsNullOrEmpty(value))
                    fieldValues.Add(value);
            }
            SelectedFeaturesInfo featureInfo = new SelectedFeaturesInfo(url, fieldName, fieldValues.ToArray());
            return featureInfo;
        }

        public Feature[] GetSelectedFeatures()
        {
            return _selectFeature.Count > 0 ? _selectFeature.ToArray() : null;
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
            VectorTempFeatureCategory wholeCategory = new VectorTempFeatureCategory("当前区域");
            _categories.Add(wholeCategory);
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

            VectorTempFeatureCategory waterBackCategory = new VectorTempFeatureCategory("湖泊", VectorAOITemplate.FindVectorFullname("中国湖泊.shp"));
            _categories.Add(waterBackCategory);
            LoadCustomRegionFiles();
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
            //_selectFeature.Clear();
            //using (frmMaskTemplate frm = new frmMaskTemplate(_size, _leftUpCoord, _resolution))
            //{
            //    frm.SelectModel = masSelectModel.Multi;
            //    if (frm.ShowDialog() == DialogResult.OK)
            //    {
            //        MaskTemplateObj[] objs = frm.MaskObject;
            //        if (objs != null && objs.Length > 0)
            //        {
            //            _maskObjs.AddRange(objs);
            //        }
            //        if (frm.Filename != null)
            //        {
            //            PersistAddFileToList(frm.Filename);
            //        }
            //        DialogResult = DialogResult.OK;
            //        Close();
            //    }
            //}
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
    }
}
