using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Reflection;
using System.IO;
using GeoDo.MEF;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.Layout.Elements
{
    public partial class UCElementsListView : UserControl
    {
        private RadListView _listView = null;
        private ListViewDataItem _dragItem = null;
        private Point _lastMouseDownLocation = Point.Empty;
        private IElement[] _customElements = null;

        public UCElementsListView()
        {
            InitializeComponent();
            _listView = new RadListView();
            _listView.Dock = DockStyle.Fill;
            _listView.AllowEdit = false;
            _listView.ShowGroups = true;
            _listView.ShowCheckBoxes = false;
            _listView.ViewType = ListViewType.ListView;
            _listView.GroupIndent = 25;
            _listView.EnableCustomGrouping = true;
            _listView.ItemSpacing = 8;
            _listView.AllowDrop = true;
            _listView.ItemMouseDown += new ListViewItemMouseEventHandler(_listView_ItemMouseDown);
            _listView.ItemMouseMove += new ListViewItemMouseEventHandler(_listView_ItemMouseMove);
            _listView.ItemMouseUp += new ListViewItemMouseEventHandler(_listView_ItemMouseUp);
            this.Controls.Add(_listView);
            Load += new EventHandler(UCElementsListView_Load);
        }

        public IElement[] CustomElements
        {
            set { _customElements = value; }
        }

        void _listView_ItemMouseUp(object sender, ListViewItemMouseEventArgs e)
        {
            _dragItem = null;
        }

        void _listView_ItemMouseMove(object sender, ListViewItemMouseEventArgs e)
        {
            if (e.OriginalEventArgs.Button == MouseButtons.Left)
            {
                if (_dragItem != null)
                    _listView.DoDragDrop(new DataObject(LayoutHost.cstDragDropDataFormat, _dragItem.Tag as Type), DragDropEffects.Copy);
            }
            _dragItem = null;
        }

        void _listView_ItemMouseDown(object sender, ListViewItemMouseEventArgs e)
        {
            if (e.OriginalEventArgs.Button == MouseButtons.Left)
            {
                _listView.SelectedItem = e.Item;
                _dragItem = _listView.SelectedItem;
            }
        }

        void UCElementsListView_Load(object sender, EventArgs e)
        {
            FindAllElements();
        }

        public void FindAllElements()
        {
            object[] objs;
            string[] dlls = MefConfigParser.GetAssemblysByCatalog("专题制图");
            using (IComponentLoader<IElement> loader = new ComponentLoader<IElement>())
            {
                objs = loader.LoadComponents(dlls);
            }
            Dictionary<string, List<IElement>> categorys = ClassifyElements(objs);
            CreatGroupsFromClassify(categorys);
            AddCustomElementsToTree();
        }

        //将Elements按照标记的属性分类
        private Dictionary<string, List<IElement>> ClassifyElements(object[] objs)
        {
            if (objs == null || objs.Length == 0)
                return null;
            object[] atts;
            Dictionary<string, List<IElement>> categorys = new Dictionary<string, List<IElement>>();
            foreach (object element in objs)
            {
                atts = element.GetType().GetCustomAttributes(true);
                foreach (object att in atts)
                {
                    if (att is CategoryAttribute)
                    {
                        if (categorys.Keys.Contains((att as CategoryAttribute).Category))
                            categorys[(att as CategoryAttribute).Category].Add(element as IElement);
                        else
                        {
                            List<IElement> eles = new List<IElement>();
                            eles.Add(element as IElement);
                            categorys.Add((att as CategoryAttribute).Category, eles);
                        }
                        break;
                    }
                }
            }
            return categorys;
        }

        //创建ListView中的groups&items
        private void CreatGroupsFromClassify(Dictionary<string, List<IElement>> categorys)
        {
            ListViewDataItemGroup group;
            ListViewDataItem item;
            List<IElement> elements;
            List<ListViewDataItemGroup> groups = new List<ListViewDataItemGroup>();
            List<ListViewDataItem> items = new List<ListViewDataItem>();
            foreach (string key in categorys.Keys)
            {
                group = new ListViewDataItemGroup();
                group.Text = key;
                group.Font = new System.Drawing.Font("微软雅黑", 11);
                group.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
                elements = categorys[key];
                _listView.Groups.Add(group);
                foreach (IElement ele in elements)
                {
                    item = new ListViewDataItem();
                    item.Text = ele.Name;
                    item.Font = new Font("微软雅黑", 10);
                    item.ImageAlignment = ContentAlignment.MiddleLeft;
                    item.TextAlignment = ContentAlignment.MiddleLeft;
                    item.Tag = ele.GetType();
                    item.TextImageRelation = TextImageRelation.ImageBeforeText;
                    if (ele.Icon != null)
                        item.Image = ele.Icon;
                    item.Group = group;
                    _listView.Items.Add(item);
                }
            }
        }

        private void AddCustomElementsToTree()
        {
            if (_customElements == null || _customElements.Length == 0)
                return;
            ListViewDataItemGroup group = new ListViewDataItemGroup();
            group.Text = "自定义";
            group.Font = new System.Drawing.Font("微软雅黑", 11);
            group.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            _listView.Groups.Add(group);
            ListViewDataItem item;
            List<ListViewDataItemGroup> groups = new List<ListViewDataItemGroup>();
            List<ListViewDataItem> items = new List<ListViewDataItem>();
            foreach (IElement ele in _customElements)
            {
                item = new ListViewDataItem();
                item.Text = ele.Name;
                item.Font = new Font("微软雅黑", 10);
                item.ImageAlignment = ContentAlignment.MiddleLeft;
                item.TextAlignment = ContentAlignment.MiddleLeft;
                item.Tag = ele.GetType();
                item.TextImageRelation = TextImageRelation.ImageBeforeText;
                if (ele.Icon != null)
                    item.Image = ele.Icon;
                item.Group = group;
                _listView.Items.Add(item);
            }
        }
    }
}
