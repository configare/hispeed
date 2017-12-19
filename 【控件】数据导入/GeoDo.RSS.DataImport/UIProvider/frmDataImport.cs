using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.DI
{
    public partial class frmDataImport : Form
    {
        private Dictionary<string, string> _products = new Dictionary<string, string>();
        private Dictionary<string, string> _subProducts = new Dictionary<string, string>();
        private ThemeDef _curTheme = null;
        private string[] _eliminateKnownExtArrary = new string[] { ".HDR", ".BPW", ".XML", ".INFO", ".JPG", ".BMP", ".PNG" };
        private List<string> _eliminateKnownExt = new List<string>();
        private int _proIndex = 1;
        private int _subProIndex = 2;
        private ImportFilesObj[] _objs = null;

        public frmDataImport()
        {
            InitializeComponent();
            GetPros();
            InitCmbProInfos();
            InitEliminateExt();
        }

        public ImportFilesObj[] ImportObjs
        {
            get { return _objs; }
        }

        private void InitEliminateExt()
        {
            _eliminateKnownExt = new List<string>();
            _eliminateKnownExt.AddRange(_eliminateKnownExtArrary);
        }

        private void GetPros()
        {
            _products.Clear();
            MonitoringThemeFactory.MergerTheme(AppDomain.CurrentDomain.BaseDirectory + "\\Themes\\CMAThemes.xml");
            _curTheme = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            if (_curTheme == null)
                return;
            ProductDef[] pros = _curTheme.Products;
            if (pros == null || pros.Length == 0)
                return;
            foreach (ProductDef pro in pros)
            {
                _products.Add(pro.Name, pro.Identify);
            }
        }

        private void GetSubPros(string proIdenftiy)
        {
            _subProducts.Clear();
            if (string.IsNullOrEmpty(proIdenftiy) || _curTheme == null)
                return;
            ProductDef pro = _curTheme.GetProductDefByIdentify(proIdenftiy);
            if (pro == null)
                return;
            SubProductDef[] subPros = pro.SubProducts;
            if (subPros == null || subPros.Length == 0)
                return;
            foreach (SubProductDef subPro in subPros)
                _subProducts.Add(subPro.Name, subPro.Identify);
        }

        private void ckFullSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (lvFiles.Items.Count == 0)
                return;
            foreach (ListViewItem item in lvFiles.Items)
            {
                item.Checked = ckFullSelect.Checked;
            }
        }

        private void ckCommSetting_CheckedChanged(object sender, EventArgs e)
        {
            cbCommPro.Enabled = ckCommSetting.Checked;
            cbCommSubpro.Enabled = ckCommSetting.Checked;
        }

        private void InitCmbProInfos()
        {
            cbCommPro.Items.Clear();
            _subProducts.Clear();
            cbCommSubpro.Items.Clear();
            if (_products == null || _products.Count == 0)
                return;
            foreach (string pro in _products.Keys)
                cbCommPro.Items.Add(pro);
            cbCommPro.Items.Add("");
            cbCommPro.SelectedIndex = 0;
        }

        private void InitCmbSubProInfos()
        {
            cbCommSubpro.Items.Clear();
            if (_subProducts == null || _subProducts.Count == 0)
                return;
            foreach (string subPro in _subProducts.Keys)
                cbCommSubpro.Items.Add(subPro);
            cbCommSubpro.Items.Add("");
        }

        private void cbCommPro_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetSubPros(_products.ContainsKey(cbCommPro.Text) ? _products[cbCommPro.Text] : string.Empty);
            InitCmbSubProInfos();
            SetLvFilesProStatus();
        }

        private void cbCommSubpro_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLvFilesSubProStatus();
        }

        private void SetLvFilesProStatus()
        {
            if (lvFiles.Items.Count == 0)
                return;
            foreach (ListViewItem item in lvFiles.Items)
            {
                item.SubItems[_proIndex].Text = cbCommPro.Text;
                UpdateListObj(item);
            }
        }

        private void SetLvFilesSubProStatus()
        {
            if (lvFiles.Items.Count == 0)
                return;
            foreach (ListViewItem item in lvFiles.Items)
            {
                item.SubItems[_subProIndex].Text = cbCommSubpro.Text;
                UpdateListObj(item);
            }
        }

        private void btSelectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                txtDir.Text = dialog.SelectedPath;
            InitLvFiels(txtDir.Text);
        }

        private void InitLvFiels(string fileDir)
        {
            if (string.IsNullOrEmpty(fileDir))
                return;
            lvFiles.Items.Clear();
            string[] files = Directory.GetFiles(fileDir, "*.*", SearchOption.AllDirectories);
            ListViewItem lv = null;
            foreach (string file in files)
            {
                lv = CreateLvFileItem(file);
                if (lv == null)
                    continue;
                lvFiles.Items.Add(lv);
            }
        }

        private ListViewItem CreateLvFileItem(string file)
        {
            if (_eliminateKnownExt.Contains(Path.GetExtension(file).ToUpper()))
                return null;
            RasterIdentify rs = new RasterIdentify(file);
            ImportFilesObj obj = null;
            if (rs == null || string.IsNullOrEmpty(rs.ProductIdentify))
                obj = new ImportFilesObj(null, null, null, null, Path.GetFileName(file), Path.GetDirectoryName(file));
            else
            {
                if (string.IsNullOrEmpty(rs.SubProductIdentify))
                    obj = new ImportFilesObj(rs.ProductName, null, rs.ProductIdentify, null, Path.GetFileName(file), Path.GetDirectoryName(file));
                else
                    obj = new ImportFilesObj(rs.ProductName, rs.SubProductName, rs.ProductIdentify, rs.SubProductIdentify, Path.GetFileName(file), Path.GetDirectoryName(file));
            }
            return CreateLvFileItem(obj);
        }

        private ListViewItem CreateLvFileItem(ImportFilesObj obj)
        {
            if (obj == null)
                return null;
            ListViewItem lvi = new ListViewItem();
            lvi.SubItems.Add(obj.ProName);
            lvi.SubItems.Add(obj.SubProName);
            lvi.SubItems.Add(obj.FileName);
            lvi.SubItems.Add(obj.Dir);
            lvi.Checked = true;
            lvi.Tag = obj;
            return lvi;
        }

        private void txtDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                InitLvFiels(txtDir.Text);
        }

        private void UpdateListObj(ListViewItem lvi)
        {
            if (lvi.Tag == null)
                return;
            ImportFilesObj obj = lvi.Tag as ImportFilesObj;
            if (obj == null)
                return;
            obj.ProName = lvi.SubItems[_proIndex].Text;
            if (_products != null && _products.ContainsKey(obj.ProName))
                obj.ProIdentify = _products[obj.ProName];
            else
                obj.ProIdentify = string.Empty;
            obj.SubProName = lvi.SubItems[_subProIndex].Text;
            if (_subProducts != null && _subProducts.ContainsKey(obj.SubProName))
                obj.SubProIdentify = _subProducts[obj.SubProName];
            else
                obj.SubProIdentify = string.Empty;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            _objs = GetFileListObj();
            DialogResult = DialogResult.OK;
        }

        private ImportFilesObj[] GetFileListObj()
        {
            if (lvFiles.Items.Count == 0)
                return null;
            List<ImportFilesObj> result = new List<ImportFilesObj>();
            foreach (ListViewItem item in lvFiles.Items)
            {
                if (item.Checked)
                    result.Add(item.Tag as ImportFilesObj);
            }
            return result.Count == 0 ? null : result.ToArray();
        }

        private void lvFiles_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            int selectCount = lvFiles.CheckedIndices.Count;
            int total = lvFiles.Items.Count;
            lbFileCount.Text = "共有：" + total + "  选中：" + selectCount;
        }

        public void SetFilesDir(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return;
            txtDir.Text = dir;
            InitLvFiels(txtDir.Text);
        }
    }
}
