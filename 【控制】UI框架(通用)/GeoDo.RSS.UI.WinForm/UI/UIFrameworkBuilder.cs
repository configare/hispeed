using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI;
using System.Drawing;
using Telerik.WinControls;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.WinForm
{
    internal class UIFrameworkBuilder : IUIFrameworkBuilder, IUIFrameworkHelper
    {
        private RadRibbonBar _radRibbonBar;
        private RadRibbonBarBackstageView _fileMenuView;
        private UIFrameworkDefinition _uiDef;
        private ISmartSession _session = null;
        private EventHandler _buttonClickHandler;
        private Dictionary<string, List<RadItem>> _tabs = new Dictionary<string, List<RadItem>>();
        private RibbonTab _currentTab = null;
        //private RadDropDownList _queryCommandList;
        private RadDropDownListElement _queryCommandList;
        private Action<object> _tabChangedHandler;

        public UIFrameworkBuilder(ISmartSession session,UIFrameworkDefinition uiDef)
        {
            _uiDef = uiDef;
            _session = session;
            _buttonClickHandler = new EventHandler(button_Click);
        }

        void button_Click(object sender, EventArgs e)
        {
            if (sender is RadButtonElement)
            {
                UIButton btn = (sender as RadItem).Tag as UIButton;
                if (string.IsNullOrEmpty(btn.Argument))
                    Execute(btn.Identify);
                else
                    Execute(btn.Identify, btn.Argument);
            }
            else if (sender is RadMenuItem)
                Execute(((sender as RadItem).Tag as UIMenuItem).Identify, ((sender as RadItem).Tag as UIMenuItem).Argument);
        }

        private void Execute(int id)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute();
        }

        private void Execute(int id, string argument)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute(argument);
        }

        public void Build(object host)
        {
            _radRibbonBar = host as RadRibbonBar;
            BuildQuickAccessBar();
            BuildTab(_uiDef);
            BuildFileMenuView(_uiDef);
            _radRibbonBar.CommandTabSelected += new CommandTabEventHandler(_radRibbonBar_CommandTabSelected);
        }

        void _radRibbonBar_CommandTabSelected(object sender, CommandTabEventArgs args)
        {
            if (_tabChangedHandler != null)
                _tabChangedHandler(_radRibbonBar.SelectedCommandTab.Tag);
        }

        private IRecentFileContainer _recentFileContainer;

        private void BuildFileMenuView(UIFrameworkDefinition uiDef)
        {
            UIFileMenuView mv = uiDef.UIFileMenu;
            if (mv.MenuItems == null || mv.MenuItems.Length == 0)
                return;
            _fileMenuView = new RadRibbonBarBackstageView();
            _fileMenuView.Text = "开始";
            _fileMenuView.ItemClicked += new EventHandler<BackstageItemEventArgs>(_fileMenuView_ItemClicked);
            _fileMenuView.BackstageViewOpened += new EventHandler(_fileMenuView_BackstageViewOpened);
            _fileMenuView.BackstageViewClosed += new EventHandler(_fileMenuView_BackstageViewClosed);
            (_session.SmartWindowManager.MainForm as Form).Controls.Add(_fileMenuView);
            _radRibbonBar.RibbonBarElement.Text = "文件";
            _radRibbonBar.BackstageControl = _fileMenuView;
            

            Font btnItemFont = new Font("微软雅黑", 10.5f);
            foreach (UIMenuItem item in mv.MenuItems)
            {
                if (!string.IsNullOrEmpty(item.Provider))
                {
                    BackstageViewPage page = new BackstageViewPage();
                    IUIProvider uiprd = item.ContentOfUIProvider.Control as IUIProvider;
                    if (uiprd != null)
                    {
                        page.Controls.Add(uiprd.Content as Control);
                        (uiprd as IUIProvider).Init(_session);
                        if (uiprd is IRecentFileContainer)
                        {
                            _recentFileContainer = uiprd as IRecentFileContainer;
                            _recentFileContainer.SetFileMenView(_fileMenuView);
                        }
                    }
                    BackstageTabItem tabPage = new BackstageTabItem(item.Text);
                    tabPage.Margin = new Padding(3, 3, 3, 3);
                    tabPage.Font = btnItemFont;
                    tabPage.Page = page;
                    _fileMenuView.Controls.Add(page);
                    _fileMenuView.Items.Add(tabPage);
                }
                else
                {
                    BackstageButtonItem fileMenuItem = new BackstageButtonItem(item.Text);
                    fileMenuItem.Margin = new Padding(3, 3, 3, 3);
                    fileMenuItem.Font = btnItemFont;
                    fileMenuItem.Tag = item.Identify;
                    fileMenuItem.Click += new EventHandler(fileMenuItem_Click);
                    Image img = GetImage(item.Image);
                    if (img != null)
                        fileMenuItem.Image = img;
                    _fileMenuView.Items.Add(fileMenuItem);
                }
            }
        }

        private void EnableQuickAccessToolBarItems(bool enable)
        {
            foreach (RadItem item in _radRibbonBar.QuickAccessToolBarItems)
            {
                item.Enabled = enable;
            }
        }

        void _fileMenuView_BackstageViewOpened(object sender, EventArgs e)
        {
            EnableQuickAccessToolBarItems(false);
            if (_recentFileContainer != null)
                _recentFileContainer.LoadItemsByRecentUsedFiles();
        }

        void _fileMenuView_BackstageViewClosed(object sender, EventArgs e)
        {
            EnableQuickAccessToolBarItems(true);
        }

        void fileMenuItem_Click(object sender, EventArgs e)
        {
            HideFileMenuView();
            int id = 0;
            if (int.TryParse((sender as BackstageButtonItem).Tag.ToString(), out id))
            {
                ICommand cmd = _session.CommandEnvironment.Get(id);
                if (cmd != null)
                    cmd.Execute();
            }
        }

        void _fileMenuView_ItemClicked(object sender, BackstageItemEventArgs e)
        {

        }

        private void HideFileMenuView()
        {
            _fileMenuView.HidePopup();
        }

        private void BuildQuickAccessBar()
        {
            if (_uiDef == null || _uiDef.QuickAccessBar == null || _uiDef.QuickAccessBar.Buttons == null || _uiDef.QuickAccessBar.Buttons.Length == 0)
                return;
            RadItem[] items = GetRadItem(_uiDef.QuickAccessBar.Buttons.Cast<UICommand>().ToArray());
            foreach (RadItem it in items)
            {
                RadButtonElement btn = it as RadButtonElement;
                if (btn != null)
                    btn.DisplayStyle = DisplayStyle.Image;
            }
            _radRibbonBar.QuickAccessToolBarItems.AddRange(items);
            //AddFileOpenAs();
            AddCommandQueryTextBox();
        }

        RadDropDownButtonElement _fileOpenAs;
        private void AddFileOpenAs()
        {
            _fileOpenAs = new RadDropDownButtonElement();
            _fileOpenAs.MinSize = new Size(100, 22);
            _fileOpenAs.Text = "文件打开为";
            _fileOpenAs.ArrowPosition = DropDownButtonArrowPosition.Right;
            FileDrivers driver = new FileDrivers(_session);
            RadMenuItem[] items = driver.LoadMenuItems();
            _fileOpenAs.Items.AddRange(items);
            _radRibbonBar.QuickAccessToolBarItems.Add(_fileOpenAs);
        }

        private void AddCommandQueryTextBox()
        {
            _queryCommandList = new RadDropDownListElement();
            _queryCommandList.MinSize = new Size(140, 22);
            _queryCommandList.BackColor = Color.FromArgb(209, 221, 231);
            _queryCommandList.Text = "查找命令";
            _queryCommandList.NullText = "查找命令";
            _queryCommandList.AutoCompleteDataSource = null;
            _queryCommandList.AutoCompleteValueMember = null;
            _queryCommandList.AutoCompleteDisplayMember = null;
            _queryCommandList.KeyUp += new KeyEventHandler(QueryCommand_KeyUp);
            _queryCommandList.TextBox.KeyUp += new KeyEventHandler(TextBox_KeyUp);
            _queryCommandList.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(QueryCommand_SelectedIndexChanged);
            _radRibbonBar.QuickAccessToolBarItems.Add(_queryCommandList);
        }

        void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            QueryCommand_KeyUp(sender, e);
        }

        void QueryCommand_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                RadDropDownTextBoxElement item = sender as RadDropDownTextBoxElement;
                if (item == null || string.IsNullOrEmpty(item.Text))
                    return;
                ICommand[] cmds = _session.CommandEnvironment.Search(item.Text);
                if (cmds == null)
                    return;
                _queryCommandList.Items.Clear();
                foreach (ICommand command in cmds)
                    _queryCommandList.Items.Add(new RadListDataItem(command.Text, command));
                _queryCommandList.Focus();
                //_queryCommandList.ShowDropDown();
                _queryCommandList.ShowPopup();
            }
        }

        void QueryCommand_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RadListDataItem obj = _queryCommandList.SelectedItem;
            if (obj == null)
                return;
            (obj.Value as ICommand).Execute();
        }

        private void BuildTab(UIFrameworkDefinition uidef)
        {
            _uiDef = uidef;
            foreach (UITab uitab in uidef.UITabs)
            {
                RibbonTab tab = null;
                //load ribbontab from single assembly
                if (uitab.ContentOfUIProvider != null && uitab.ContentOfUIProvider.Control != null)
                {
                    tab = (RibbonTab)(uitab.ContentOfUIProvider.Control as IUIProvider).Content;
                    tab.Tag = uitab.ContentOfUIProvider.Control as IUIProvider;
                    (tab.Tag as IUIProvider).Init(_session);
                    if (!string.IsNullOrEmpty(uitab.Text))
                        tab.Text = uitab.Text;
                    TryLoadItemsOfTab(uidef, uitab, tab);//支持在UIProvider的Tab下的子节点
                }
                else//create ribbontab by xml
                {
                    tab = new RibbonTab();
                    tab.Text = uitab.Text;
                    tab.Name = uitab.Name;
                    tab.Tag = uitab;
                    TryLoadItemsOfTab(uidef, uitab, tab);
                }
                //
                Font font = uidef.GetFont("tabfont");
                if (font != null)
                    tab.Font = font;
                tab.Click += new EventHandler(tab_Click);
                //
                _tabs.Add(tab.Text, new List<RadItem>(tab.Items.ToArray()));
                if (tab.Text != "开始")
                    tab.Items.Clear();
                else
                    _currentTab = tab;
                _radRibbonBar.CommandTabs.Add(tab);
            }
        }

        void tab_Click(object sender, EventArgs e)
        {
            if (_currentTab != null)
                _currentTab.Items.Clear();
            RibbonTab tab = sender as RibbonTab;
            tab.Items.AddRange(_tabs[tab.Text].ToArray());
            _currentTab = tab;
        }

        public void InsertTab(int idx, object tab)
        {
            InsertTab(idx, tab as RibbonTab);
        }

        public void InsertTab(int idx, RibbonTab tab)
        {
            if (_tabs.ContainsKey(tab.Text))
            {
                ActiveTab(tab.Text);
                return;
            }
            Font font = _uiDef.GetFont("tabfont");
            if (font != null)
                tab.Font = font;
            _tabs.Add(tab.Text, new List<RadItem>(tab.Items.ToArray()));
            tab.Items.Clear();
            if (idx < 0 && idx >= _tabs.Count)
                _radRibbonBar.CommandTabs.Add(tab);
            else
                _radRibbonBar.CommandTabs.Insert(idx, tab);            
            tab.Click += new EventHandler(tab_Click);
            ActiveTab(tab.Text);
        }

        public int IndexOf(string tabText)
        {
            int idx = -1;
            foreach (RibbonTab key in _radRibbonBar.CommandTabs)
            {
                idx++;
                if (tabText == key.Text)
                    return idx;
            }
            return idx;
        }

        public bool IsExist(string tabText)
        {
            if (string.IsNullOrEmpty(tabText))
                return false;
            return _tabs.ContainsKey(tabText);
        }

        public void ActiveTab(string tabText)
        {
            if (!IsExist(tabText))
                return;
            RibbonTab tab = _radRibbonBar.CommandTabs[IndexOf(tabText)] as RibbonTab;
            tab.PerformClick();
            tab.IsSelected = true;
        }

        public object GetActiveTab()
        {
            return (GeoDo.RSS.UI.AddIn.Theme.UITabProcuct.Instance as GeoDo.RSS.UI.AddIn.Theme.IThemeProductsTab);
        }

        public void ActiveDefaultTab()
        {
            ActiveTab("开始");
        }

        public void Remove(string tabText)
        {
            _radRibbonBar.CommandTabs.RemoveAt(IndexOf(tabText));
            if (_tabs.ContainsKey(tabText))
            {
                List<RadItem> tabs = _tabs[tabText];
                for (int i = 0; i < tabs.Count; i++)
                {
                    RadItem item = tabs[i];
                    item.Dispose();
                    item = null;
                }
                tabs.Clear();
                _tabs.Remove(tabText);
            }
        }

        public Image GetImage(string resName)
        {
            return _uiDef.GetImage(resName);
        }

        private void TryLoadItemsOfTab(UIFrameworkDefinition uidef, UITab uitab, RibbonTab tab)
        {
            if (uitab.Children == null)
                return;
            foreach (UIItem it in uitab.Children)
            {
                if (it is UICommandGroup)
                {
                    RadRibbonBarGroup group = GetRadRibbonBarGroup(it as UICommandGroup);
                    if (group != null)
                    {
                        tab.Items.Add(group);
                        if (!it.Visible)
                            group.Visibility = ElementVisibility.Collapsed;
                    }
                }
            }
        }

        private RadRibbonBarGroup GetRadRibbonBarGroup(UICommandGroup uiCommandGroup)
        {
            RadRibbonBarGroup g = null;
            if (uiCommandGroup.ContentOfUIProvider != null)
            {
                if (uiCommandGroup.ContentOfUIProvider.Control != null)
                {
                    g = (RadRibbonBarGroup)(uiCommandGroup.ContentOfUIProvider.Control as IUIProvider).Content;
                    g.Tag = uiCommandGroup.ContentOfUIProvider.Control as IUIProvider;
                    (g.Tag as IUIProvider).Init(_session);
                    if (uiCommandGroup.UICommands != null && uiCommandGroup.UICommands.Length != 0)
                    {
                        RadItem[] items = GetRadItem(uiCommandGroup.UICommands);
                        if (items != null)
                            g.Items.AddRange(items);
                    }
                }
            }
            else
            {
                g = new RadRibbonBarGroup();
                g.Text = uiCommandGroup.Text;
                g.Name = uiCommandGroup.Name;
                if (!uiCommandGroup.Visible)
                    g.Visibility = ElementVisibility.Collapsed;
                if (!uiCommandGroup.AllowCollapsed)
                    g.AllowCollapsed = false;
                RadItem[] items = GetRadItem(uiCommandGroup.UICommands);
                if (items != null)
                    g.Items.AddRange(items);
            }
            return g;
        }

        private RadItem[] GetRadItem(UICommand[] uiCommand)
        {
            if (uiCommand == null)
                return null;
            List<RadItem> items = new List<RadItem>();
            foreach (UICommand cmd in uiCommand)
            {
                if (cmd is UIButton)
                {
                    RadButtonElement btn = new RadButtonElement(cmd.Text);
                    btn.Name = cmd.Name;
                    Image img = GetImage((cmd as UIButton).Image);
                    if (img != null)
                        btn.Image = img;
                    btn.Tag = cmd;
                    btn.Click += _buttonClickHandler;
                    btn.ImageAlignment = GetImageAligment((cmd as UIButton).ImageAligment);
                    btn.TextAlignment = GetTextAligment((cmd as UIButton).TextAligment);
                    btn.TextImageRelation = GetTextImageRelation((cmd as UIButton).TextImageRelation);
                    items.Add(btn);
                }
                else if (cmd is UIDropDownButton)
                {
                    RadDropDownButtonElement dbtn = new RadDropDownButtonElement();
                    dbtn.Text = cmd.Text;
                    dbtn.Name = dbtn.Text;
                    Image img = GetImage((cmd as UIDropDownButton).Image);
                    if (img != null)
                        dbtn.Image = img;
                    dbtn.ImageAlignment = GetImageAligment((cmd as UIDropDownButton).ImageAligment);
                    dbtn.Tag = cmd;
                    dbtn.TextAlignment = GetTextAligment((cmd as UIDropDownButton).TextAligment);
                    dbtn.DropDownDirection = GetRadDirection((cmd as UIDropDownButton).DropDownDirection);
                    dbtn.ArrowPosition = GetArrowPosition((cmd as UIDropDownButton).ArrowPosition);
                    Font font = _uiDef.GetFont((cmd as UIDropDownButton).Font);
                    if (font != null)
                        dbtn.Font = font;
                    //
                    UIItem[] menuItems = (cmd as UIDropDownButton).MenuItems;
                    if (menuItems != null)
                    {
                        foreach (UIItem it in menuItems)
                        {
                            if (it is UIMenuHeader)
                            {
                                dbtn.Items.Add(new RadMenuHeaderItem(it.Text));
                            }
                            else if (it is UIMenuItem)
                            {
                                RadMenuItem rit = new RadMenuItem(it.Text);
                                dbtn.Items.Add(rit);
                                rit.Tag = it;
                                rit.Click += _buttonClickHandler;
                            }
                        }
                    }
                    //
                    items.Add(dbtn);
                }
            }
            return items.Count > 0 ? items.ToArray() : null;
        }

        private TextImageRelation GetTextImageRelation(string textImageRelation)
        {
            foreach (TextImageRelation alg in Enum.GetValues(typeof(TextImageRelation)))
            {
                if (alg.ToString() == textImageRelation)
                    return alg;
            }
            return TextImageRelation.ImageAboveText;
        }

        private DropDownButtonArrowPosition GetArrowPosition(string p)
        {
            foreach (DropDownButtonArrowPosition e in Enum.GetValues(typeof(DropDownButtonArrowPosition)))
            {
                if (e.ToString() == p)
                    return e;
            }
            return DropDownButtonArrowPosition.Bottom;
        }

        private RadDirection GetRadDirection(string direction)
        {
            foreach (RadDirection e in Enum.GetValues(typeof(RadDirection)))
            {
                if (e.ToString() == direction)
                    return e;
            }
            return RadDirection.Down;
        }

        private ContentAlignment GetTextAligment(string aligment)
        {
            foreach (ContentAlignment alg in Enum.GetValues(typeof(ContentAlignment)))
            {
                if (alg.ToString() == aligment)
                    return alg;
            }
            return ContentAlignment.BottomCenter;
        }

        private ContentAlignment GetImageAligment(string aligment)
        {
            foreach (ContentAlignment alg in Enum.GetValues(typeof(ContentAlignment)))
            {
                if (alg.ToString() == aligment)
                    return alg;
            }
            return ContentAlignment.TopCenter;
        }

        public void SetTabChangedEvent(Action<object> tabChanged)
        {
            _tabChangedHandler = tabChanged;
        }

        public void SetVisible(string groupName, bool visible)
        {
            if (_currentTab == null)
                return;
            foreach (RadItem g in _currentTab.Items)
            {
                if (g.Name != null && g.Name == groupName)
                {
                    g.Visibility = visible ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                    return;
                }
            }
        }

        public void Insert(string groupName, int index)
        {
            if (_currentTab == null)
                return;
            if (index >= _currentTab.Items.Count)
                return;
            RadItem item=null;
            foreach (RadItem g in _currentTab.Items)
            {
                if (g.Name != null && g.Name == groupName)
                {
                    item = g;
                    break;
                }
            }
            if (item != null)
            {
                _currentTab.Items.Remove(item);
                _currentTab.Items.Insert(index, item);
            }
        }

        public void SetLockBesideX(string groupName, bool locked)
        {
            if (_currentTab == null)
                return;
            foreach (RadItem g in _currentTab.Items)
            {
                if (g.Name != null && g.Name == groupName)
                    continue;
                g.Enabled = !locked;
            }
            foreach (RadItem tab in _radRibbonBar.CommandTabs)
            {
                if (tab == _currentTab)
                    continue;
                tab.Enabled = !locked;
            }
            foreach(RadItem tab in _radRibbonBar.QuickAccessToolBarItems)
                tab.Enabled  = !locked;
            _radRibbonBar.BackstageControl.Enabled = !locked;
        }
    }
}
