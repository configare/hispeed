using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using Telerik.WinControls.UI;
using System.IO;

namespace GeoDo.RSS.MIF.UI
{
    public partial class UCWorkspace : UserControl, IWorkspace
    {
        private WorkspaceDef _definition;
        private Dictionary<string, ICatalog> _catalogs = new Dictionary<string, ICatalog>();
        private RadPageView _radPageView;
        private Action<object> _itemDoubleClickedHandler;
        private ListViewItemEventHandler _listViewItemDoubleClicked;
        private RadTreeView.TreeViewEventHandler _treeViewNodeDoubleClicked;
        public delegate void ListViewGroupHandler(string columnName);
        public ListViewGroupHandler ListViewGroup;
        public delegate void ListViewGroupInitHandler(ToolStripComboBox tsCB);
        public ListViewGroupInitHandler ListViewGroupInit;
        private string _preProductIdentify;

        private RadPageView _histroyPageView = null;

        public UCWorkspace()
        {
            InitializeComponent();
            _listViewItemDoubleClicked = new ListViewItemEventHandler(this.lv_ItemMouseDoubleClick);
            _treeViewNodeDoubleClicked = new RadTreeView.TreeViewEventHandler(this.tv_NodeMouseDoubleClick);
        }

        public ICatalog ActiveCatalog
        {
            get
            {
                RadPageViewItemPage page = _radPageView.SelectedPage as RadPageViewItemPage;
                //by chennan 20120814 工作空间划分为当前和历史
                if (page != null && page.Name == "histroy")
                    page = _histroyPageView.SelectedPage as RadPageViewItemPage;
                //
                if (page == null)
                    return null;
                CatalogDef df = page.Tag as CatalogDef;
                if (df == null)
                    return null;
                if (_catalogs.ContainsKey(df.ClassString))
                    return _catalogs[df.ClassString];
                return null;
            }
        }

        public object GetEnvironmentVar(string identify)
        {
            if (string.IsNullOrEmpty(identify))
                return null;
            return null;
        }

        public void Apply(string productIdentify)
        {
            WorkspaceDef wks = WorkspaceDefinitionFactory.GetWorkspaceDef(productIdentify);
            Apply(wks);
        }

        public void Apply(WorkspaceDef wksdef)
        {
            if (wksdef != null && wksdef.Identify == _preProductIdentify)
                return;
            if (_radPageView != null)
            {
                UnattachEvents();
                _catalogs.Clear();
                this.Controls.Remove(_radPageView);
            }
            _definition = wksdef;
            GetStrategyFilter(wksdef);
            _radPageView = new RadPageView();
            _radPageView.Font = this.Font;
            _radPageView.Dock = DockStyle.Fill;

            //by chennan 20120814 工作空间划分为当前和历史
            _histroyPageView = new RadPageView();
            _histroyPageView.Font = this.Font;
            _histroyPageView.Dock = DockStyle.Fill;
            //

            if (_definition != null)
            {
                foreach (CatalogDef catalog in _definition.CatalogDefs)
                {
                    RadPageViewItemPage tabPage = GetRadPage(catalog);
                    tabPage.Dock = DockStyle.Fill;
                    tabPage.Tag = catalog;
                    //by chennan 20120814 工作空间划分为当前和历史

                    if (catalog is ExtractingCatalogDef)
                    {
                        _radPageView.Pages.Add(tabPage);
                    }
                    else
                    {
                        _histroyPageView.Pages.Add(tabPage);
                    }
                }
            }

            //by chennan 20120814 工作空间划分为当前和历史
            RadPageViewItemPage page = new RadPageViewItemPage();
            page.Text = "历史监测信息";
            page.Name = "histroy";
            page.Controls.Add(_histroyPageView);
            _radPageView.Pages.Add(page);
            //
            this.Controls.Add(_radPageView);
            //
            if (wksdef != null)
                _preProductIdentify = wksdef.Identify;
            else
                _preProductIdentify = null;
        }

        private void GetStrategyFilter(WorkspaceDef wksdef)
        {
            StrategyFilter = UWorkspace.StrategyFilter(wksdef);
        }

        private void UnattachEvents()
        {
            foreach (ICatalog c in _catalogs.Values)
            {
                if (c.UI is RadListView)
                    (c.UI as RadListView).ItemMouseDoubleClick -= this._listViewItemDoubleClicked;
                else if (c.UI is RadTreeView)
                    (c.UI as RadTreeView).NodeMouseDoubleClick -= this._treeViewNodeDoubleClicked;
            }
        }

        public WorkspaceDef Definition
        {
            get { return _definition; }
        }

        public StrategyFilter StrategyFilter { get; private set; }

        public string GetNewestFile(string subProductIdentify)
        {
            ICatalog c = ActiveCatalog;//GetCatalog("CurrentExtracting");
            if (c == null)
                return null;
            string[] fs = c.GetSelectedFiles(subProductIdentify);
            return fs != null && fs.Length > 0 ? fs[0] : null;
        }

        public ICatalog GetCatalog(string classIdentify)
        {
            if (_catalogs == null || _catalogs.Count == 0 || string.IsNullOrEmpty(classIdentify))
                return null;
            if (_catalogs.ContainsKey(classIdentify))
                return _catalogs[classIdentify];
            return null;
        }

        public ICatalog GetCatalogByIdentify(string identify)
        {
            if (_catalogs == null || _catalogs.Count == 0 || string.IsNullOrEmpty(identify))
                return null;
            foreach (ICatalog cit in _catalogs.Values)
            {
                if (cit.Definition == null)
                    continue;
                SubProductCatalogDef c = cit.Definition as SubProductCatalogDef;
                if (c == null)
                    continue;
                if (c.Identify.Contains(identify))
                    return cit;
            }
            return null;
        }

        public void ChangeTo(ICatalog catalog)
        {
            if (catalog == null || catalog.UI == null)
                return;
            if (_radPageView.SelectedPage != null)
                _radPageView.SelectedPage = catalog.UI as RadPageViewPage;
            //by chennan 20120814 工作空间划分为当前和历史
            else if (_histroyPageView.SelectedPage != null)
                _histroyPageView.SelectedPage = catalog.UI as RadPageViewPage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="catalog">目录定义</param>
        /// <returns></returns>
        private RadPageViewItemPage GetRadPage(CatalogDef catalog)
        {
            RadPageViewItemPage page = new RadPageViewItemPage();
            page.Text = catalog.Text;
            Control ui = GetUserControl(catalog);
            if (ui != null)
            {
                ui.Dock = DockStyle.Fill;
                page.Controls.Add(ui);
            }
            if (catalog is ExtractingCatalogDef)
            {
                _catalogs.Add(catalog.ClassString, new CatalogTreeView(this, catalog, page, ui as RadTreeView, this.Font));
            }
            else if (catalog is SubProductCatalogDef)
            {
                UCOperationBar oprBar = new UCOperationBar();
                oprBar.btnOpenFiles.Tag = catalog.ClassString;
                oprBar.Dock = DockStyle.Top;
                oprBar.tsBtStrategy.Click += new EventHandler(tsBtStrategy_Click);
                oprBar.btnOpenFiles.Click += new EventHandler(oprBar_Click);
                oprBar.tsCBGroup.Tag = catalog.ClassString;
                page.Controls.Add(oprBar);
                _catalogs.Add(catalog.ClassString, new CatalogListView(this, catalog, page, ui as RadListView));
                InitOprBarGroupItem(oprBar.tsCBGroup);
                oprBar.tsCBGroup.SelectedIndexChanged += new EventHandler(tsCBGroup_SelectedIndexChanged);
            }
            return page;
        }

        // 策略按钮
        void tsBtStrategy_Click(object sender, EventArgs e)
        {
            FormStrategy formStrategy = new FormStrategy();
            formStrategy.Init(_definition);
            if (formStrategy.ShowDialog() != DialogResult.OK)
                return;
            RefreshData();
        }
        
        // 更新数据
        private void RefreshData()
        {
        }

        void tsCBGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox tsCB = sender as ToolStripComboBox;
            if (ListViewGroup != null)
                ListViewGroup(tsCB.Text);
        }

        private void InitOprBarGroupItem(ToolStripComboBox tsCBGroup)
        {
            if (ListViewGroupInit != null)
                ListViewGroupInit(tsCBGroup);
        }

        void oprBar_Click(object sender, EventArgs e)
        {
            ICatalog c = _catalogs[(sender as ToolStripButton).Tag.ToString()];
            SubProductCatalogDef catalogDef = c.Definition as SubProductCatalogDef;
            string filter = catalogDef.Filter;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = filter;
                dlg.Multiselect = true;
                dlg.InitialDirectory = MifEnvironment.GetWorkspaceDir();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string[] fnames = dlg.FileNames;
                    if (fnames == null || fnames.Length == 0)
                        return;
                    foreach (string f in fnames)
                        c.AddItem(new CatalogItem(f, catalogDef, null));
                }
            }
        }

        private Control GetUserControl(CatalogDef catalog)
        {
            if (catalog is ExtractingCatalogDef)
            {
                return GetExtractingCatalogUI(catalog as ExtractingCatalogDef);
            }
            else if (catalog is SubProductCatalogDef)
            {
                return GetSubProductCatalogUI(catalog as SubProductCatalogDef);
            }
            return null;
        }

        private Control GetSubProductCatalogUI(SubProductCatalogDef catalog)
        {
            RadListView lv = new RadListView();
            lv.Tag = catalog;
            lv.AllowEdit = false;
            lv.Font = this.Font;
            lv.ViewType = ListViewType.DetailsView;
            lv.FullRowSelect = true;
            lv.ShowGroups = true;
            lv.EnableSorting = true;
            lv.EnableColumnSort = true;
            lv.ItemMouseDoubleClick += _listViewItemDoubleClicked;
            lv.MultiSelect = true;
            foreach (CatalogAttriteDef att in catalog.AttributeDefs)
            {
                lv.Columns.Add(att.Text);
                lv.Columns[lv.Columns.Count - 1].Visible = att.Visible;
            }
            return lv;
        }

        void lv_ItemMouseDoubleClick(object sender, ListViewItemEventArgs e)
        {
            if (_itemDoubleClickedHandler != null)
            {
                ItemInfoListViewDataItem item = e.Item as ItemInfoListViewDataItem;
                _itemDoubleClickedHandler(item != null ? item.Tag : null);
            }
        }

        private Control GetExtractingCatalogUI(ExtractingCatalogDef catalog)
        {
            RadTreeView tv = new RadTreeView();
            tv.Tag = catalog;
            tv.ShowLines = true;
            tv.HideSelection = false;
            tv.Font = this.Font;
            tv.NodeMouseDoubleClick += _treeViewNodeDoubleClicked;
            return tv;
        }

        void tv_NodeMouseDoubleClick(object sender, RadTreeViewEventArgs e)
        {
            if (_itemDoubleClickedHandler != null)
                _itemDoubleClickedHandler(e.Node.Tag);
        }

        public IWorkspace Workspace
        {
            get { return this; }
        }

        public void SetDoubleClickHandler(Action<object> itemDoubleClickedHandler)
        {
            _itemDoubleClickedHandler = itemDoubleClickedHandler;
        }        
    }

}
