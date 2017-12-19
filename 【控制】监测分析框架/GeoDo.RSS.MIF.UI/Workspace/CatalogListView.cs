using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using Telerik.WinControls.UI;
using System.IO;
using System.Windows.Forms;
using Telerik.WinControls.Data;
using System.ComponentModel;

namespace GeoDo.RSS.MIF.UI
{
    public class CatalogListView : Catalog
    {
        private RadListView _listView;

        public CatalogListView(IWorkspace wks, CatalogDef catalogDef, RadPageViewItemPage uiPage, RadListView listView)
            : base(wks, catalogDef, uiPage)
        {
            _listView = listView;
            LoadCatalogItems();
            _listView.MouseUp += new System.Windows.Forms.MouseEventHandler(_listView_MouseUp);
            InitDelegate();
        }

        private void InitDelegate()
        {
            if (_wks is UCWorkspace)
            {
                UCWorkspace ucw = _wks as UCWorkspace;
                ucw.ListViewGroup += new UCWorkspace.ListViewGroupHandler(ListView_Group);
                ucw.ListViewGroupInit += new UCWorkspace.ListViewGroupInitHandler(ListView_GroupInit);
            }
        }

        void _listView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewDataItem it = _listView.SelectedItem;
                if (it != null)
                {
                    _currentCatalogItem = it.Tag as CatalogItem;
                    ContentMenuStrip.Show(_listView, e.Location);
                }
            }
        }

        public override string[] GetSelectedFiles()
        {
            if (_listView.SelectedItems == null || _listView.SelectedItems.Count == 0)
                return null;
            List<string> items = new List<string>(_listView.SelectedItems.Count);
            foreach (var lv in _listView.SelectedItems)
            {
                ItemInfoListViewDataItem tag = lv as ItemInfoListViewDataItem;
                if (tag == null)
                    continue;
                items.Add(tag.Tag.ToString());
            }
            return items.Count > 0 ? items.ToArray() : null;
        }

        public override ICatalogItem[] GetSelectedItems()
        {
            if (_listView.SelectedItems == null || _listView.SelectedItems.Count == 0)
                return null;
            List<ICatalogItem> items = new List<ICatalogItem>(_listView.SelectedItems.Count);
            foreach (var lv in _listView.SelectedItems)
            {
                ItemInfoListViewDataItem tag = lv as ItemInfoListViewDataItem;
                if (tag == null)
                    continue;
                items.Add(new CatalogItem(tag.Tag.ToString(), _definition as SubProductCatalogDef, null));
            }
            return items.Count > 0 ? items.ToArray() : null;
        }

        public override void AddItem(ICatalogItem item)
        {
            if (item.Info.Properties.ContainsKey("SubProductIdentify"))
            {
                object v = item.Info.Properties["SubProductIdentify"];
                string identify = v != null ? v.ToString() : null;
                if (_definition.Identify.Contains(identify))
                {
                    if (IsExist(item))
                        return;
                    //还需要判断数据格式(eg:*.dat,*.bmp)
                    AddFileToUI(string.Empty, item.FileName);
                }
            }
        }

        public override bool IsExist(ICatalogItem item)
        {
            //if (_listView.Items == null || _listView.Items.Count == 0 || item == null || string.IsNullOrEmpty(item.FileName))
            //    return false;
            //foreach (var v in _listView.Items)
            //{
            //    ItemInfoListViewDataItem data = v as ItemInfoListViewDataItem;
            //    if (data == null || data.Tag == null)
            //        continue;
            //    if (data.Tag.ToString() == item.FileName)
            //    {
            //        //选中条目
            //        _listView.SelectedItem = data;
            //        return true;
            //    }
            //}
            //return false;
            ItemInfoListViewDataItem data = IsExist(item, true);
            if (data == null)
                return false;
            else
                return true;
        }

        private ItemInfoListViewDataItem IsExist(ICatalogItem item, bool isSelected)
        {
            if (_listView.Items == null || _listView.Items.Count == 0 || item == null || string.IsNullOrEmpty(item.FileName))
                return null;
            foreach (var v in _listView.Items)
            {
                ItemInfoListViewDataItem data = v as ItemInfoListViewDataItem;
                if (data == null || data.Tag == null)
                    continue;
                if (data.Tag.ToString() == item.FileName)
                {
                    _listView.SelectedItem = data;
                    return data;
                }
            }
            return null;
        }

        public override void Update(ICatalogItem item)
        {
        }

        public override void Clear()
        {
            _listView.Items.Clear();
        }

        protected override void AddFileToUI(string dateDir, string fname)
        {
            SubProductCatalogDef def = _definition as SubProductCatalogDef;
            CatalogItem it = new CatalogItem(fname, def);

            string sensor = it.Info.GetPropertyValue("Sensor");
            if (!string.IsNullOrEmpty(sensor) && !_wks.StrategyFilter.Sensors.Contains(sensor))
                return;

            //it.Info.Properties.
            ListViewDataItem lvItem = new ListViewDataItem();
            lvItem.Text = fname;

            Dictionary<string, object> colValues = new Dictionary<string, object>();
            foreach (CatalogAttriteDef att in def.AttributeDefs)
            {
                string v = it.Info.GetPropertyValue(att.Identify);
                if (!colValues.ContainsKey(att.Identify))
                    colValues.Add(att.Identify, v);
                else
                    colValues[att.Identify] = v;
            }
            if (colValues.ContainsKey("FileName"))
                colValues["FileName"] = Path.GetFileName(fname);
            if (colValues.ContainsKey("OrbitDateTime") && (colValues["OrbitDateTime"] == null || colValues["OrbitDateTime"].ToString() == string.Empty))
                colValues["OrbitDateTime"] = dateDir;
            if (colValues.ContainsKey("CatalogItemCN") && it.Info.Properties["CycFlagCN"].ToString() != "\\")
                colValues["CatalogItemCN"] = it.Info.Properties["CycFlagCN"];
            _listView.Items.Add(new ItemInfoListViewDataItem(colValues.Values.ToArray(), fname));
        }

        protected void AddFileToUI_tmp(string dateDir, string fname)
        {
            SubProductCatalogDef def = _definition as SubProductCatalogDef;
            CatalogItem it = new CatalogItem(fname, def);
            ListViewDataItem lvItem = new ListViewDataItem();
            lvItem.Text = fname;
            Dictionary<string, object> colValues = new Dictionary<string, object>();
            foreach (CatalogAttriteDef att in def.AttributeDefs)
            {
                string v = it.Info.GetPropertyValue(att.Identify);
                if (!colValues.ContainsKey(att.Identify))
                    colValues.Add(att.Identify, v);
                else
                    colValues[att.Identify] = v;
            }
            if (colValues.ContainsKey("FileName"))
                colValues["FileName"] = Path.GetFileName(fname);
            if (colValues.ContainsKey("OrbitDateTime") && (colValues["OrbitDateTime"] == null || colValues["OrbitDateTime"].ToString() == string.Empty))
                colValues["OrbitDateTime"] = dateDir;
            if (colValues.ContainsKey("CatalogItemCN") && it.Info.Properties["CycFlagCN"].ToString() != "\\")
                colValues["CatalogItemCN"] = it.Info.Properties["CycFlagCN"];
            _listView.Items.Add(new ItemInfoListViewDataItem(colValues.Values.ToArray(), fname));
        }

        public override bool RemoveItem(ICatalogItem item, bool removeOther)
        {
            if (item == null || _listView.Items.Count == 0)
                return true;
            ItemInfoListViewDataItem tag = null;
            List<ListViewDataItem> temp = new List<ListViewDataItem>();
            temp.AddRange(_listView.Items);
            foreach (ListViewDataItem lvi in temp)
            {
                tag = lvi as ItemInfoListViewDataItem;
                if (tag == null)
                    continue;
                if (tag.Tag.ToString().ToUpper() == item.FileName.ToUpper())
                    _listView.Items.Remove(lvi);
            }
            if (removeOther)
                RemoveOtherCatalog(item, false);
            return true;
        }


        private void RemoveOtherCatalog(ICatalogItem item, bool removeOther)
        {
            if (item == null || string.IsNullOrEmpty(item.FileName))
                return;
            ICatalog catalog = _wks.GetCatalog("CurrentExtracting");
            if (catalog != null)
                catalog.RemoveItem(item, removeOther);
        }

        private void ListView_Group(string columnName)
        {
            _listView.GroupDescriptors.Clear();
            if (columnName == "无")
            {
                _listView.EnableGrouping = false;
                _listView.ShowGroups = false;
            }
            else
            {
                if (columnName.IndexOf("时间") != -1)
                {
                    if (columnName.IndexOf("轨道时间") != -1)
                        _listView.GroupDescriptors.Add(new GroupDescriptor(
                                new SortDescriptor[] { new SortDescriptor(columnName + "分组", ListSortDirection.Descending) }));
                    else
                        _listView.GroupDescriptors.Add(new GroupDescriptor(
                                   new SortDescriptor[] { new SortDescriptor(columnName, ListSortDirection.Descending) }));
                }
                else
                    _listView.GroupDescriptors.Add(new GroupDescriptor(
                         new SortDescriptor[] { new SortDescriptor(columnName, ListSortDirection.Ascending) }));
                _listView.EnableGrouping = true;
                _listView.ShowGroups = true;
            }
        }

        private void ListView_GroupInit(ToolStripComboBox tsCB)
        {
            tsCB.Items.Clear();
            tsCB.Items.Add("无");
            tsCB.SelectedIndex = 0;
            if (_listView.Columns == null || _listView.Columns.Count == 0)
                return;
            foreach (ListViewDetailColumn column in _listView.Columns)
            {
                if (column.Visible)
                    tsCB.Items.Add(column.Name);
            }
        }

        protected override bool RemoveFromWks(ICatalogItem item)
        {
            ItemInfoListViewDataItem data = IsExist(item, false);
            if (data == null)
                return false;
            else
            {
                _listView.Items.Remove(data);
                return true;
            }
        }
    }

    public class ItemInfoListViewDataItem : ListViewDataItem
    {
        protected object _tag = null;

        public ItemInfoListViewDataItem(object[] colValues, object tag)
        {
            _tag = tag;
            if (colValues != null)
                foreach (object v in colValues)
                    SubItems.Add(v);
        }

        public object Tag
        {
            get { return _tag; }
        }

        public override ListViewSubDataItemCollection SubItems
        {
            get
            {
                return base.SubItems;
            }
        }
    }
}
