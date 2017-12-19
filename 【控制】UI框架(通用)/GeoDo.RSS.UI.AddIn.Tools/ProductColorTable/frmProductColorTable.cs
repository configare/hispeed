using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class frmProductColorTable : Form
    {
        private bool _hasChanged = false;
        //ProductColorTable[] _colors = null;
        private ProductDic[] _productDics = null;
        private ProductDic _selectedProductDic = null;
        private ProductColorTable[] _selectedProductColorTables = null;
        private ProductColorTable _selectedProductColorTable = null;

        public frmProductColorTable()
        {
            InitializeComponent();
            this.Load += new EventHandler(frmProductColorTable_Load);
        }

        void frmProductColorTable_Load(object sender, EventArgs e)
        {
            lvColors.MouseDoubleClick += new MouseEventHandler(lvColors_MouseDoubleClick);
            InitLoadColorTables("");
        }

        private void InitLoadColorTables(string selectedFilename)
        {
            ProductColorTableParser parser = new ProductColorTableParser();
            string[] files = parser.LoadColorTables();
            ProductDic[] pdcs = new ProductDic[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                ProductDic pdc = new ProductDic();
                pdc.FileName = files[i];
                pdc.ProductIdentify = Path.GetFileNameWithoutExtension(files[i]);
                pdcs[i] = pdc;
            }
            TrySetProductDicName(pdcs);
            _productDics = pdcs;
            InitLoadProductDicList(selectedFilename);
        }

        private void TrySetProductDicName(ProductDic[] productDics)
        {
            ThemeDef def = MonitoringThemeFactory.GetAllThemes()[0];//"CMA"
            foreach (ProductDic prd in productDics)
            {
                MIF.Core.ProductDef prdDef = def.GetProductDefByIdentify(prd.ProductIdentify);
                if (prdDef != null)
                    prd.ProductName = prdDef.Name.Replace(" ", "");
                else
                    prd.ProductName = prd.ProductIdentify;
            }
        }

        private void InitLoadProductDicList(string selectedFilename)
        {
            listBox1.Items.Clear();
            foreach (ProductDic product in _productDics)
            {
                listBox1.Items.Add(product);
            }
            listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);
            if (listBox1.Items.Count != 0)
            {
                ProductDic selectedPdc = null;
                if (!string.IsNullOrWhiteSpace(selectedFilename))
                {
                    foreach (ProductDic product in _productDics)
                    {
                        if (selectedFilename == product.FileName)
                        {
                            selectedPdc = product;
                            break;
                        }
                    }
                }
                if (selectedPdc == null)
                    selectedPdc = _productDics[0];
                listBox1.SelectedItem = selectedPdc;
            }
        }

        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TryAskSave();
            ProductDic product = listBox1.SelectedItem as ProductDic;
            LoadCurProductColorTable(product);
        }

        private void LoadCurProductColorTable(ProductDic product)
        {
            _selectedProductDic = product;
            _selectedProductColorTables = null;
            _selectedProductColorTable = null;
            if (product == null || string.IsNullOrWhiteSpace(product.FileName))
            {
                ClearPcts();
                return;
            }
            ProductColorTableParser parser = new ProductColorTableParser();
            _selectedProductColorTables = ProductColorTableParser.Parse(product.FileName);
            if (_selectedProductColorTables == null)
                ClearPcts();
            else
                LoadColorTable(_selectedProductColorTables);
        }

        private void ClearPcts()
        {
            lvwColorTableList.Items.Clear();
            UpdateColorTableInfo();
            lvColors.Items.Clear();
        }

        private void LoadColorTable(ProductColorTable[] colors)
        {
            lvwColorTableList.Items.Clear();
            for (int i = 0; i < colors.Length; i++)
            {
                ListViewItem it = new ListViewItem(colors[i].Description);
                it.SubItems.Add(colors[i].Identify);
                it.SubItems.Add(colors[i].SubIdentify);
                it.SubItems.Add(colors[i].ColorTableName);
                it.SubItems.Add(colors[i].LabelText);
                it.Tag = colors[i];
                if (i == 0)
                    it.Selected = true;
                lvwColorTableList.Items.Add(it);
            }
        }

        void lvColors_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem lstItem = lvColors.GetItemAt(e.X, e.Y);
            if (lstItem == null)
                return;
            EditProductColor(lstItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwColorTableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwColorTableList.SelectedItems == null || lvwColorTableList.SelectedItems.Count == 0)
                return;
            int selectedIndex = lvwColorTableList.SelectedIndices[0];//选择的索引
            ProductColorTable colorTable = lvwColorTableList.SelectedItems[0].Tag as ProductColorTable;
            if (colorTable == null)
                return;
            _selectedProductColorTable = colorTable;
            UpdateColorTableInfo();
            LoadColorList(_selectedProductColorTable);
        }

        private void LoadColorList(ProductColorTable colorTable)
        {
            lvColors.Items.Clear();
            ProductColor[] colors = colorTable.ProductColors;
            for (int i = 0; i < colors.Length; i++)
            {
                LoadListViewItem(colors[i]);
            }
        }

        private void LoadListViewItem(ProductColor productColor)
        {
            ListViewItem it = new ListViewItem("");
            it.SubItems.Add(productColor.LableText);
            it.SubItems.Add(productColor.MinValue.ToString());
            it.SubItems.Add(productColor.MaxValue.ToString());
            it.SubItems.Add(productColor.DisplayLengend.ToString());
            it.UseItemStyleForSubItems = false;
            it.SubItems[0].BackColor = productColor.Color;
            it.Tag = productColor;
            lvColors.Items.Add(it);
        }

        private void UpdateColorTableInfo()
        {
            if (_selectedProductColorTable == null)
            {
                txtColorDes.Text = "";
                //label2.Text = "";
                txtColorName.Text = "";
                txtPrdIdentify.Text = "";
                txtSubPrdIdentify.Text = "";
            }
            else
            {
                txtColorDes.Text = _selectedProductColorTable.Description;
                //label2.Text = _curProductColorTable.LabelText;
                txtColorName.Text = _selectedProductColorTable.ColorTableName;
                txtPrdIdentify.Text = _selectedProductColorTable.Identify;
                txtSubPrdIdentify.Text = _selectedProductColorTable.SubIdentify;
            }
        }

        private void lvColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvColors.SelectedItems == null || lvColors.SelectedItems.Count == 0)
                return;
        }

        private void EditProductColor(ListViewItem lstItem)
        {
            ProductColor productColor = lstItem.Tag as ProductColor;
            using (frmColorItem frmcolor = new frmColorItem())
            {
                frmcolor.StartPosition = FormStartPosition.Manual;
                frmcolor.Location = new Point(this.Right - frmcolor.Width, this.Top + btnEdit.Top);
                frmcolor.SetProductColor(productColor);
                if (frmcolor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    _hasChanged = true;
                    productColor = frmcolor.ProductColor;
                    UpdateListViewItem(lstItem, productColor);
                    UpdateProductColor();
                }
            }
        }

        private void GetCurProductColorTable()
        {
            List<ProductColor> lstProductColor = new List<ProductColor>();
            foreach (ListViewItem lstItem in lvColors.Items)
            {
                ProductColor pc = lstItem.Tag as ProductColor;
                lstProductColor.Add(pc);
            }
            _selectedProductColorTable.ProductColors = lstProductColor.ToArray();
        }

        private void AddProductColor()
        {
            if (_selectedProductColorTable == null)
                return;
            ProductColor productColor = null;
            using (frmColorItem frmcolor = new frmColorItem())
            {
                frmcolor.StartPosition = FormStartPosition.Manual;
                frmcolor.Location = new Point(this.Right - frmcolor.Width, this.Top + btnAdd.Top);
                frmcolor.SetProductColor(null);
                if (frmcolor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    _hasChanged = true;
                    productColor = frmcolor.ProductColor;
                    LoadListViewItem(productColor);
                    UpdateProductColor();
                }
            }
        }

        private void UpdateProductColor()
        {
            GetCurProductColorTable();
        }

        private void RemoveProductColor()
        {
            if (lvColors.SelectedItems == null || lvColors.SelectedItems.Count == 0)
                return;
            ListViewItem lstItem = lvColors.SelectedItems[0];
            lvColors.Items.Remove(lstItem);
            _hasChanged = true;
            UpdateProductColor();
        }

        private void UpdateListViewItem(ListViewItem lstItem, ProductColor productColor)
        {
            lstItem.Tag = productColor;
            lstItem.SubItems[0].BackColor = productColor.Color;
            lstItem.SubItems[1].Text = productColor.LableText.ToString();
            lstItem.SubItems[2].Text = productColor.MinValue.ToString();
            lstItem.SubItems[3].Text = productColor.MaxValue.ToString();
            lstItem.SubItems[4].Text = productColor.DisplayLengend.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveColorTable();
        }

        private void SaveColorTable()
        {
            try
            {
                if (!_hasChanged || _selectedProductDic == null || _selectedProductColorTables == null)
                    return;
                ProductColorTableParser.WriteToXml(_selectedProductColorTables, _selectedProductDic.FileName);
                ProductColorTableFactory.ReLoadAllColorTables();
                if (chkUpdateTheme.Checked)
                {
                    LayoutTemplateHelper.UpdateLegend(_selectedProductColorTables, null);
                }
                _hasChanged = false;
                MsgBox.ShowInfo("更新完成");
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(@"更新完成：" + ex.Message);
            }
        }

        private void TryAskSave()
        {
            if (_hasChanged)
            {
                if (MessageBox.Show("是否保存当前更新的颜色表", "信息提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveColorTable();
                }
                else
                {
                    _hasChanged = false;
                }
            }
        }

        private void CloseForm()
        {
            TryAskSave();
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvColors.SelectedItems == null || lvColors.SelectedItems.Count == 0)
                return;
            ListViewItem lstItem = lvColors.SelectedItems[0];
            EditProductColor(lstItem);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddProductColor();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveProductColor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Import();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void Import()
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.Title = "导入产品颜色表";
                diag.Filter = "产品颜色表(*.pct)|*.pct|所有文件(*.*)|*.*";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filename = diag.FileName;
                    Import(filename);
                }
            }
        }

        private void Import(string filename)
        {
            ProductColorTable[] colortables = ProductColorTableParser.Parse(filename);
            if (colortables == null)
            {
                MsgBox.ShowInfo("导入产品颜色表失败：解析出来的颜色表数据为空");
                return;
            }
            bool isCopy = true;
            if (_productDics != null)
            {
                string name = Path.GetFileNameWithoutExtension(filename).ToLower();
                for (int i = 0; i < _productDics.Length; i++)
                {
                    if (name == _productDics[i].ProductIdentify.ToLower())
                    {
                        if ((MessageBox.Show("已经存在名称为" + name + "的颜色表，是否替换？", "信息提示", MessageBoxButtons.YesNo) == DialogResult.Yes))
                            isCopy = true;
                        else
                            isCopy = false;
                        break;
                    }
                }
            }
            if (isCopy)
            {
                string selectedFilename = null;
                selectedFilename = ProductColorTableParser.Import(filename, true);
                ReloadColorTable(selectedFilename);
            }
        }

        private void ReloadColorTable(string selectedFilename)
        {
            InitLoadColorTables(selectedFilename);
        }

        private void Export()
        {
            using (SaveFileDialog diag = new SaveFileDialog())
            {
                diag.Title = "导出产品颜色表";
                diag.Filter = "产品颜色表(*.pct)|*.pct|所有文件(*.*)|*.*";
                ProductDic product = listBox1.SelectedItem as ProductDic;
                diag.FileName = Path.GetFileNameWithoutExtension(product.FileName);
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filename = diag.FileName;
                    ProductColorTableParser.WriteToXml(_selectedProductColorTables, filename);
                }
            }
        }
    }

    internal class ProductDic
    {
        public string ProductIdentify;
        public string ProductName;
        public string FileName;

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(ProductName) || ProductIdentify == ProductName)
                return ProductIdentify;
            else
                return string.Format("{0}[{1}]", ProductIdentify, ProductName);
        }
    }
}
