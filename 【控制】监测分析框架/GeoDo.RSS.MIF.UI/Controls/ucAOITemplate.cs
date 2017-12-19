using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.UI
{
    public partial class ucAOITemplate : UserControl
    {
        RadGroupBox aoiGroupBox = new RadGroupBox();
        RadSplitButton btnAddAoi = new RadSplitButton();
        RadButton btnRemoveAoi = new RadButton();
        RadListView listbox = new RadListView();
        private List<string> _aois = new List<string>();

        public event OnAOITempleteChangedHandler AOIChangedHandler;

        public ucAOITemplate()
        {
            InitializeComponent();
            InitControl();
        }

        public string[] Aois
        {
            get { return _aois.ToArray(); }
        }

        private void InitControl()
        {
            this.Controls.Clear();

            aoiGroupBox.Text = "感兴趣区域模板";
            aoiGroupBox.Dock = DockStyle.Fill;

            listbox.AllowEdit = false;
            listbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            listbox.Location = new Point(aoiGroupBox.Left + 2, aoiGroupBox.Top + 20);
            listbox.Size = new Size(aoiGroupBox.Width - 48, aoiGroupBox.Height - 24);
            listbox.ShowCheckBoxes = true;
            listbox.ItemCheckedChanged += new ListViewItemEventHandler(listbox_ItemCheckedChanged);

            string[] aoiTemps = AOITemplateFactory.TemplateNames;
            if (aoiTemps != null && aoiTemps.Length != 0)
            {
                for (int i = 0; i < aoiTemps.Length; i++)
                {
                    RadMenuItem item1 = new RadMenuItem(aoiTemps[i]);
                    item1.Tag = listbox;
                    item1.Click += new EventHandler(aoiItemMenu_Click);
                    btnAddAoi.Items.Add(item1);
                }
            }

            btnAddAoi.Text = "+";
            btnAddAoi.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnAddAoi.Size = new Size(40, 24);
            btnAddAoi.Location = new Point(aoiGroupBox.Width - 44, aoiGroupBox.Top + 20);
            btnAddAoi.Tag = listbox;
            //btnAddAoi.Click += new EventHandler(DropDownButtonElement_Click);

            btnRemoveAoi.Text = "—";
            btnRemoveAoi.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnRemoveAoi.Size = new Size(40, 24);
            btnRemoveAoi.Location = new Point(aoiGroupBox.Width - 44, aoiGroupBox.Top + 48);
            btnRemoveAoi.Tag = listbox;
            btnRemoveAoi.Click += new EventHandler(btnRemoveAoi_Click);

            aoiGroupBox.Controls.Add(btnRemoveAoi);
            aoiGroupBox.Controls.Add(btnAddAoi);
            aoiGroupBox.Controls.Add(listbox);
            this.Controls.Add(aoiGroupBox);
        }

        public void BuildControl(AOITemplate[] productAois,string subproductAois)
        {
            _aois = new List<string>();
            if (productAois != null && productAois.Length != 0)
            {
                foreach (AOITemplate aoi in productAois)
                {
                    ListViewDataItem item = new ListViewDataItem(aoi.Template);
                    item.Tag = aoi.IsReverse;
                    item.CheckState = aoi.IsChecked ? Telerik.WinControls.Enumerations.ToggleState.On : Telerik.WinControls.Enumerations.ToggleState.Off;
                    listbox.Items.Add(item);
                    if (aoi.IsChecked && !_aois.Contains(aoi.Template))
                        _aois.Add(aoi.Template);
                }
                if (!string.IsNullOrEmpty(subproductAois) && listbox.Items.Count > 0)
                {
                    foreach (ListViewDataItem item in listbox.Items)
                    {
                        item.CheckState = Telerik.WinControls.Enumerations.ToggleState.Off;
                    }
                    //解析subproductAois
                    string[] aois = subproductAois.Split(',');
                    if (aois != null && aois.Length > 0)
                    {
                        foreach (string aoi in aois)
                        {
                            foreach (ListViewDataItem item in listbox.Items)
                            {
                                if (item.Text == aoi)
                                {
                                    item.CheckState = Telerik.WinControls.Enumerations.ToggleState.On;
                                    continue;
                                }
                            }
                        }
                    }
                }
                if (AOIChangedHandler != null)
                    AOIChangedHandler(this, _aois.ToArray());
            }
        }

        void aoiItemMenu_Click(object sender, EventArgs e)
        {
            RadMenuItem item = (sender as RadMenuItem);
            RadListView lv = item.Tag as RadListView;
            bool isExsit = false;
            foreach (ListViewDataItem it in lv.Items)
            {
                if (it.Text == item.Text)
                {
                    isExsit = true;
                    break;
                }
            }
            if (!isExsit)
            {
                ListViewDataItem lvitem = new ListViewDataItem(item.Text);
                lvitem.CheckState = Telerik.WinControls.Enumerations.ToggleState.On;
                lvitem.Tag = true;
                lv.Items.Add(lvitem);
                if (!_aois.Contains(item.Text))
                {
                    _aois.Add(item.Text);
                    if (AOIChangedHandler != null)
                        AOIChangedHandler(this, _aois.ToArray());
                }
            }
        }

        void btnRemoveAoi_Click(object sender, EventArgs e)
        {
            RadListView lv = (sender as RadButton).Tag as RadListView;
            ListViewDataItem item = lv.SelectedItem;
            if (item == null)
                return;
            lv.Items.Remove(item);
            if (_aois.Contains(item.Text))
            {
                _aois.Remove(item.Text);
                if (AOIChangedHandler != null)
                    AOIChangedHandler(this, _aois.ToArray());
            }
        }

        void listbox_ItemCheckedChanged(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.CheckState == Telerik.WinControls.Enumerations.ToggleState.On)
                _aois.Add(e.Item.Text);
            else
                _aois.Remove(e.Item.Text);
            if (AOIChangedHandler != null)
                AOIChangedHandler(this, _aois.ToArray());
        }

        void DropDownButtonElement_Click(object sender, EventArgs e)
        {
            return;
            CodeCell.AgileMap.Core.Feature[] features = null;
            string fieldName;
            string shapeFilename;
            int fieldIndex = -1;
            using (frmStatSubRegionTemplates frmTemplates = new frmStatSubRegionTemplates())
            {
                if (frmTemplates.ShowDialog() == DialogResult.OK)
                {
                    features = frmTemplates.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                    //...待实现
                }
            }
        }

    }
}
