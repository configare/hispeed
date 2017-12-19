using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CodeCell.Bricks.AppFramework
{
    public abstract class UIBuilder : IUIBuilder, ICommandHelper
    {
        protected Form _hostForm = null;
        protected IApplication _application = null;
        /// <summary>
        /// 工具条
        /// </summary>
        protected List<ToolStrip> _toolstrips = new List<ToolStrip>(2);
        /// <summary>
        /// 一级菜单
        /// </summary>
        protected List<ToolStripMenuItem> _toolStripMenuItems = new List<ToolStripMenuItem>(5);
        /// <summary>
        /// 按钮、工具对象与界面元素
        /// </summary>
        protected Dictionary<IItem, ToolStripItem> _uiItems = new Dictionary<IItem, ToolStripItem>();
        /// <summary>
        /// 实时更新状态的计时器
        /// </summary>
        protected Timer _uiTimer = null;
         protected ICommand _currentTool = null;
        private bool _needRefresh = true;
        protected IHook _defaultHook = null;
        private Container _uiTimerContainer = new Container();
        private Container _publicTimerContainer = new Container();
        private object uiLockObj = new object();
        private object publicLockObject = new object();
        private ICommandProvider _provider = null;

        public UIBuilder(Form hostForm, IApplication application)
        {
            _hostForm = hostForm;
            _application = application;
            CreateUITimer();
            CreateHook(_application,this as ICommandHelper);
            hostForm.FormClosed += new FormClosedEventHandler(hostForm_FormClosed);
            (application as ApplicationDefault).SetHook(_defaultHook);
        }

        protected abstract void CreateHook(IApplication application, ICommandHelper iCommandHelper);

        private void CreateUITimer()
        {
            _uiTimer = new Timer(_uiTimerContainer);
            _uiTimer.Interval = 200;
            _uiTimer.Tick += new EventHandler(UI_Timer_Tick);
        }

        void hostForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _uiTimerContainer.Dispose();
            _publicTimerContainer.Dispose();
        }

        void UI_Timer_Tick(object sender, EventArgs e)
        {
            lock (uiLockObj)
            {
                Refresh();
            }
        }

        #region IUIItemsContainer 成员

        public void Refresh()
        {
            if (!_needRefresh || _hostForm.WindowState == FormWindowState.Minimized)
                return;
            DirectRefresh();
        }

        public bool IntimeRefresh
        {
            get { return _uiTimer.Enabled; }
            set { _uiTimer.Enabled = value; }
        }

        public void DirectRefresh()
        {
            foreach (ToolStrip bar in _toolstrips)
            {
                bar.Visible = (bar.Tag as IToolbar).Visible;
                foreach (ToolStripItem btn in bar.Items)
                    if (btn is ToolStripSeparator)
                    {
                        if ((btn as ToolStripSeparator).Tag != null)
                            btn.Visible = ((btn as ToolStripSeparator).Tag as ICommand).Visible;
                    }
            }
            ApplyCursorOfTool(null);
            foreach (IItem it in _uiItems.Keys)
            {
                try
                {
                    if (_uiItems[it] == null)
                        continue;
                    _uiItems[it].Visible = it.Visible;
                    if (!_uiItems[it].Visible)
                        continue;
                    if (it is ICommand)
                    {
                        _uiItems[it].Enabled = (it as ICommand).Enabled;
                        //if(!(it is IControlItem))
                        //    _uiItems[it].DisplayStyle = it.DisplayStyle;
                        if (_uiItems[it] is ToolStripMenuItem)
                            (_uiItems[it] as ToolStripMenuItem).Checked = (it as ICommand).Checked;
                        if (it is ITool)
                        {
                            if (it.Equals(_currentTool))
                            {
                                //应用当前工具的鼠标符号
                                ApplyCursorOfTool(it as ITool);
                                //刷新菜单的Checked属性
                                if (_uiItems[it] is ToolStripMenuItem)
                                    (_uiItems[it] as ToolStripMenuItem).Checked = true;
                                else if (_uiItems[it] is ToolStripButton)
                                    (_uiItems[it] as ToolStripButton).Checked = true;
                            }
                            else
                            {
                                if (_uiItems[it] is ToolStripMenuItem)
                                    (_uiItems[it] as ToolStripMenuItem).Checked = false;
                                else if (_uiItems[it] is ToolStripButton)
                                    (_uiItems[it] as ToolStripButton).Checked = false;
                            }
                        }
                    }
                    else if (it is IControlItem)
                    {
                        _uiItems[it].Enabled = (it as ICommand).Enabled;
                    }
                }
                catch 
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 应用当前工具的绘图鼠标符号到绘图控件
        /// </summary>
        /// <param name="tool"></param>
        private void ApplyCursorOfTool(ITool tool)
        {
            if (_defaultHook.ContainerControl != null)
            {
                if (tool == null)
                    _defaultHook.ContainerControl.Cursor = Cursors.Default;
                else
                    _defaultHook.ContainerControl.Cursor = tool.Cursor;
            }
        }

        public void Building(ICommandProvider commandProvider)
        {
            _provider = commandProvider;
            if (commandProvider == null)
                return;
            if (commandProvider.Menus != null)
            {
                foreach (IItem it in commandProvider.Menus)
                {
                    AddUIItem(it);
                }
            }
            if (commandProvider.Toolbars != null)
            {
                foreach (IItem it in commandProvider.Toolbars)
                    AddUIItem(it);
            }
            //
            if (_toolstrips != null)
            {
                foreach (ToolStrip ts in _toolstrips)
                {
                    SetToolStripStyle(ts);
                    _hostForm.Controls.Add(ts);
                }
            }
            if (_toolStripMenuItems != null && _toolStripMenuItems.Count > 0)
            {
                MenuStrip ms = new MenuStrip();
                foreach (ToolStripMenuItem it in _toolStripMenuItems)
                {
                    ms.Items.Add(it);
                }
                SetMenuStripStyle(ms);
                _hostForm.Controls.Add(ms);
                _hostForm.MainMenuStrip = ms;
            }
            //先将最近使用过的文件初始化
            InitRecentFiles();
            //
            _uiTimer.Start();
        }

        private void InitRecentFiles()
        {
            ICommand[] cmds = FindCommand(typeof(cmdRecentFiles));
            if (cmds != null && cmds.Length > 0)
            {
                bool ok = cmds[0].Enabled;
                _application.RecentUsedFilesMgr.LoadRecentUsedFiles();
            }
        }

        #endregion

        private void SetToolStripStyle(ToolStrip toolstrip)
        {
            toolstrip.RenderMode = ToolStripRenderMode.System;
        }

        private void SetMenuStripStyle(MenuStrip menustrip)
        {
            menustrip.RenderMode = ToolStripRenderMode.System;
        }

        private void AddUIItem(IItem item)
        {
            if (item is IToolbar)
            {
                AddToolbar(item as IToolbar);
            }
            else if (item is IMenu)
            {
                AddMenu(item as IMenu);
            }
            else
            {
                throw new Exception("只能注册IToolbar与IMenu类型的根UI元素！");
            }
        }

        private void SetHookForItem(IItem it)
        {
            (it as ICommand).Init(_defaultHook);
            if (it.Children != null)
                foreach (IItem c in it.Children)
                    SetHookForItem(c);
        }

        private void AddToolbar(IToolbar toolbar)
        {
            if (toolbar == null)
                return;
            ToolStrip _toolstrip = new ToolStrip();
            _toolstrip.GripStyle = ToolStripGripStyle.Hidden;
            switch (toolbar.DockStyle)
            {
                case DockStyle.Left:
                case DockStyle.Right:
                    _toolstrip.BackColor = Color.Gray;
                    break;
                default:
                    break;
            }
            _toolstrip.Name = (toolbar as IItem).Name;
            _toolstrip.Tag = toolbar;
            _toolstrip.Dock = toolbar.DockStyle;
            _toolstrip.MouseEnter += new EventHandler(_toolstrip_MouseEnter);
            _toolstrip.Visible = toolbar.Visible;
            (toolbar as BaseToolbar).SetHook(_defaultHook);
            foreach (IItem it in toolbar.Items)
            {
                if (it is IShortcutFilter)
                    _application.ShortcutProcessor.AddShortcutFilter(it as IShortcutFilter);
                if (!(it is ICommand || it is ITool || it is IControlItem))
                    continue;
                if (it is IControlItem)
                {
                    AddControlItem(_toolstrip, it as IControlItem);
                }

                else if (it is ITool)
                {
                    AddTool(_toolstrip, it as ITool);
                }
                else if (it is ICommand)
                {
                    AddCommand(_toolstrip, it as ICommand);
                }
            }
            _toolstrips.Add(_toolstrip);
        }

        void _toolstrip_MouseEnter(object sender, EventArgs e)
        {
            (sender as ToolStrip).FindForm().Activate();
        }

        private void AddMenu(IMenu menu)
        {
            if (menu == null)
                return;

            ToolStripMenuItem ms = new ToolStripMenuItem();
            ms.Text = menu.Text;
            ms.Name = menu.Name;
            ms.Tag = menu;
            if (menu.Items == null)
                goto endLine;
            
            foreach (ICommand command in menu.Items)
            {
                AddCommandToMenuItem(menu, ms, command);
            }
        endLine:
            _toolStripMenuItems.Add(ms);
        }

        private void AddCommandToMenuItem(IMenu menu, ToolStripMenuItem ms, ICommand command)
        {
            if (command is IShortcutFilter)
                _application.ShortcutProcessor.AddShortcutFilter(command as IShortcutFilter);
            if (command.BeginGroup)
                ms.DropDownItems.Add(new ToolStripSeparator());
            ToolStripMenuItem it = new ToolStripMenuItem(command.Text);
            it.Image = command.Image;
            it.Name = it.Name;
            it.Tag = command;
            it.Click += new EventHandler(IItem_Click);
            command.Init(_defaultHook);
            ms.DropDownItems.Add(it);
            if (command.Children != null && command.Children.Length > 0)
                foreach (ICommand cmd in command.Children)
                    AddCommandToMenuItem(menu, it, cmd);
        }

        private void AddCommand(ToolStrip toolstrip, ICommand command)
        {
            if (command == null || toolstrip == null)
                return;
            if (command.BeginGroup && toolstrip.Items.Count > 0)
            {
                ToolStripSeparator sp = new ToolStripSeparator();
                sp.Tag = command;
                toolstrip.Items.Add(sp);
            }
            //
            ToolStripButton tsb = null;
            if (command.Children != null && command.Children.Length > 0)
            {
                ToolStripDropDownButton down = new ToolStripDropDownButton();
                down.Text = command.Text;
                down.DisplayStyle = ToolStripItemDisplayStyle.Image;
                down.Image = command.Image;
                foreach (ICommand child in command.Children)
                {
                    ToolStripMenuItem mn = new ToolStripMenuItem(child.Text);
                    mn.DisplayStyle = child.DisplayStyle;
                    //tsb.Image = child.Image;
                    mn.Click += new EventHandler(IItem_Click);
                    mn.Name = child.Name;
                    mn.Tag = child;
                    mn.ToolTipText = child.ToolTips;
                    mn.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    down.DropDownItems.Add(mn);
                    child.Init(_defaultHook);
                    _uiItems.Add(child, mn);
                }
                _uiItems.Add(command, down);
                command.Init(_defaultHook);
                toolstrip.Items.Add(down);
            }
            else
            {
                tsb = new ToolStripButton(command.Text);
                tsb.DisplayStyle = command.DisplayStyle;
                tsb.Image = command.Image;
                tsb.Click += new EventHandler(IItem_Click);
                tsb.Name = command.Name;
                tsb.Tag = command;
                tsb.ToolTipText = command.ToolTips;
                //tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
                toolstrip.Items.Add(tsb);
                command.Init(_defaultHook);
                _uiItems.Add(command, tsb);
            }
        }

        private void AddTool(ToolStrip toolstrip, ITool tool)
        {
            if (toolstrip == null || tool == null)
                return;
            if (tool.BeginGroup && toolstrip.Items.Count > 0)
            {
                ToolStripSeparator sp = new ToolStripSeparator();
                sp.Tag = tool;
                toolstrip.Items.Add(sp);
            }
            //
            ToolStripButton tsb = new ToolStripButton(tool.Text);
            tsb.DisplayStyle = tool.DisplayStyle;
            tsb.Image = tool.Image;
            tsb.Click += new EventHandler(IItem_Click);
            tsb.Name = tool.Name;
            tsb.Tag = tool;
            tsb.ToolTipText = tool.ToolTips;
            //tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolstrip.Items.Add(tsb);
            //
            tool.Init(_defaultHook);
            //
            _uiItems.Add(tool, tsb);
        }

        private void AddControlItem(ToolStrip toolstrip, IControlItem controlItem)
        {
            if (toolstrip == null || controlItem == null)
                return;
            if (!(controlItem.Control is ToolStripItem))
                throw new Exception("控件类型错误，可以加载工具栏上的控件必须是ToolStripItem的派生组件！");
            if (controlItem.BeginGroup && toolstrip.Items.Count > 0)
            {
                ToolStripSeparator sp = new ToolStripSeparator();
                sp.Tag = controlItem;
                toolstrip.Items.Add(sp);
            }
            ToolStripItem _toolStripItem = controlItem.Control as ToolStripItem;
            _toolStripItem.Width = controlItem.Size.Width;
            _toolStripItem.Height = controlItem.Size.Height;
            _toolStripItem.Name = controlItem.Name;
            _toolStripItem.Text = controlItem.Text;
            _toolStripItem.Image = controlItem.Image;
            _toolStripItem.Tag = controlItem;
            if (!(_toolStripItem is ToolStripLabel))
                _toolStripItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolstrip.Items.Add(_toolStripItem);
            //
            if (controlItem.Items != null && _toolStripItem is ToolStripDropDownButton)
            {
                ToolStripDropDownButton dr = controlItem.Control as ToolStripDropDownButton;
                ToolStripMenuItem tb = null;
                foreach (IItem it in controlItem.Items)
                {
                    if ((it as ICommand).BeginGroup)
                    {
                        ToolStripSeparator sp = new ToolStripSeparator();
                        sp.Tag = it;
                        dr.DropDownItems.Add(sp);
                    }
                    tb = new ToolStripMenuItem(it.Text);
                    tb.Name = it.Name;
                    tb.Image = (it as ICommand).Image;
                    tb.Click += new EventHandler(IItem_Click);
                    tb.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    (it as ICommand).Init(_defaultHook);
                    dr.DropDownItems.Add(tb);
                    _uiItems.Add(it, tb);
                }
            }
            //
            controlItem.Init(_defaultHook);
            //
            _uiItems.Add(controlItem, _toolStripItem);
        }

        void IItem_Click(object sender, EventArgs e)
        {
            _needRefresh = false;
            try
            {
                IItem item = (sender as ToolStripItem).Tag as IItem;
                if (item == null)
                    return;
                if (item is ICommand) //ICommand,ITool
                {
                    if (item is ITool)
                    {
                        _currentTool = item as ICommand;
                        ToolStrip c = (sender as ToolStripItem).Owner;
                        IToolbar toolbar = c.Tag as IToolbar;
                        //ClearCurrentToolFromAllToolbars();
                        //toolbar.CurrentTool = _currentTool;
                    }
                    (item as ICommand).Click();
                }
            }
            finally
            {
                _needRefresh = true;
            }
        }

        void ClearCurrentToolFromAllToolbars()
        {
            foreach (ToolStrip ts in _toolstrips)
            {
                (ts.Tag as IToolbar).CurrentTool = null;
            }
        }

        #region ICommandHelper 成员

        public ICommand CurrentCommand
        {
            get
            {
                return _currentTool;
            }
        }

        public bool EablededRefreshUI
        {
            get { return _uiTimer.Enabled; }
            set { _uiTimer.Enabled = value; }
        }

        public ICommand FindCommand(string name)
        {
            foreach (IItem it in _uiItems.Keys)
            {
                if ((it is ICommand) && (it as ICommand).Name == name)
                    return it as ICommand;

            }
            return null;
        }

        public ICommand[] FindCommand(Type type)
        {
            List<ICommand> _cmds = new List<ICommand>();
            foreach (IItem it in _uiItems.Keys)
            {
                if (it is ICommand)
                    if (it.GetType().Equals(type))
                        _cmds.Add(it as ICommand);
            }
            return _cmds.Count > 0 ? _cmds.ToArray() : null;
        }

        public ICommand FindCommand(Guid id)
        {
            foreach (IItem it in _uiItems.Keys)
            {
                if ((it is ICommand) && (it as ICommand).Id == id)
                    return it as ICommand;

            }
            return null;
        }

        public IToolbar FindToolbar(Type type)
        {
            if (_provider == null || _provider.Toolbars == null || _provider.Toolbars.Length == 0)
                return null;
            foreach (IToolbar bar in _provider.Toolbars)
                if (bar.GetType().Equals(type))
                    return bar;
            return null;
        }

        public void SetCurrentTool(ITool tool)
        {
            IItem_Click(_uiItems[tool] as ToolStripItem, null);
        }

        public object GetControlByCommandType(Type commandType)
        {
            if (_uiItems == null || _uiItems.Count == 0)
                return null;
            ICommand[] cmds = FindCommand(commandType);
            if (cmds == null || cmds.Length == 0)
                return null;
            if (_uiItems.ContainsKey(cmds[0]))
                return _uiItems[cmds[0]];
            return null;
        }

        #endregion
    }
}
