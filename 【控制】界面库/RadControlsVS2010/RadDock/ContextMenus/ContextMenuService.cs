using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using Telerik.WinControls.UI.Localization;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Provides methods for displaying a context menu for a document or tool window.
    /// </summary>
    public class ContextMenuService : RadDockService
    {
        #region Keys/Constants

        public const string ActivateWindow = "ActivateWindow";
        public const string CloseWindow = "CloseWindow";
        public const string CloseAllButThis = "CloseAllButThis";
        public const string CloseAll = "CloseAll";
        public const string NewHTabGroup = "NewHTabGroup";
        public const string NewVTabGroup = "NewVTabGroup";
        public const string MoveToPrevTabGroup = "MoveToPrevTabGroup";
        public const string MoveToNextTabGroup = "MoveToNextTabGroup";
        public const string DockStateFloating = "Floating";
        public const string DockStateDocked = "Docked";
        public const string DockStateTabbedDocument = "TabbedDocument";
        public const string DockStateAutoHide = "AutoHide";
        public const string DockStateHidden = "Hidden";

        private static object ContextMenuDisplayingEventKey;
        private static object ContextMenuItemClickedEventKey;

        #endregion

        #region Fields

        private bool allowDocumentContextMenu;
        private bool allowToolContextMenu;
        private bool allowActiveWindowListContextMenu;
        private bool hookItemClick;
        private RadContextMenu currentMenu;

        #endregion

        #region Constructors

        public ContextMenuService()
        {
            this.allowDocumentContextMenu = true;
            this.allowToolContextMenu = true;
            this.allowActiveWindowListContextMenu = true;
            this.hookItemClick = true;
        }

        static ContextMenuService()
        {
            ContextMenuDisplayingEventKey = new object();
            ContextMenuItemClickedEventKey = new object();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether context menus, associated with a tool window may be displayed.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether context menus, associated with a tool window may be displayed.")]
        public bool AllowToolContextMenu
        {
            get
            {
                return this.allowToolContextMenu;
            }
            set
            {
                this.allowToolContextMenu = value;
            }
        }

        /// <summary>
        /// Determines whether context menus, associated with a document window may be displayed.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether context menus, associated with a document window may be displayed.")]
        public bool AllowDocumentContextMenu
        {
            get
            {
                return this.allowDocumentContextMenu;
            }
            set
            {
                this.allowDocumentContextMenu = value;
            }
        }

        /// <summary>
        /// Determines whether a context menu, listing all opened documents within a document strip, may be displayed.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether a context menu, listing all opened documents within a document strip, may be displayed.")]
        public bool AllowActiveWindowListContextMenu
        {
            get
            {
                return this.allowActiveWindowListContextMenu;
            }
            set
            {
                this.allowActiveWindowListContextMenu = value;
            }
        }

        /// <summary>
        /// Determines whether a context menu is currently displayed.
        /// </summary>
        [Browsable(false)]
        public bool IsMenuDisplayed
        {
            get
            {
                if (this.currentMenu == null)
                {
                    return false;
                }

                return this.currentMenu.DropDown.Visible;
            }
        }

        /// <summary>
        /// Gets the currently displayed menu.
        /// </summary>
        [Browsable(false)]
        public RadContextMenu DisplayedMenu
        {
            get
            {
                if (this.currentMenu == null || !this.currentMenu.DropDown.Visible)
                {
                    return null;
                }

                return this.currentMenu;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Notifies that a context menu is about to be displayed.
        /// </summary>
        public event ContextMenuDisplayingEventHandler ContextMenuDisplaying
        {
            add
            {
                this.Events.AddHandler(ContextMenuDisplayingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ContextMenuDisplayingEventKey, value);
            }
        }

        /// <summary>
        /// Notifies that a context menu item has been clicked.
        /// </summary>
        public event ContextMenuItemClickEventHandler ContextMenuItemClicked
        {
            add
            {
                this.Events.AddHandler(ContextMenuItemClickedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ContextMenuItemClickedEventKey, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays a context menu at the specified screen position, associated with the provided <see cref="DockWindow">DockWindow</see> instance.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="screenPos"></param>
        public void DisplayContextMenu(DockWindow window, Point screenPos)
        {
            if (!this.CanOperate() || !this.CanDisplayMenu(window))
            {
                return;
            }

            List<RadMenuItemBase> items = this.BuildContextMenuItems(window);
            ContextMenuDisplayingEventHandler eh = this.Events[ContextMenuDisplayingEventKey] as ContextMenuDisplayingEventHandler;
            if (eh != null)
            {
                ContextMenuDisplayingEventArgs e = new ContextMenuDisplayingEventArgs(window, items, screenPos);
                eh(this, e);
                if (e.Cancel)
                {
                    return;
                }

                //get the display position from the event arguments since the user may have it changed
                screenPos = e.DisplayPosition;
            }

            this.DisplayMenuCore(items, screenPos);
        }

        /// <summary>
        /// Displays a context menu, listing all currently active documents within the specified document strip.
        /// </summary>
        /// <param name="strip"></param>
        /// <param name="screenPos"></param>
        public void DisplayActiveWindowList(DocumentTabStrip strip, Point screenPos)
        {
            if (!this.allowActiveWindowListContextMenu || !this.CanOperate())
            {
                return;
            }

            List<RadMenuItemBase> items = this.BuildContextMenuItems(strip);
            ContextMenuDisplayingEventHandler eh = this.Events[ContextMenuDisplayingEventKey] as ContextMenuDisplayingEventHandler;
            if (eh != null)
            {
                ContextMenuDisplayingEventArgs e = new ContextMenuDisplayingEventArgs(strip, items, screenPos);
                eh(this, e);
                if (e.Cancel)
                {
                    return;
                }

                //get the display position from the event arguments since the user may have it changed
                screenPos = e.DisplayPosition;
            }

            this.DisplayMenuCore(items, screenPos);
        }

        /// <summary>
        /// Gets the menu items, associated with the specified DockWindow.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public List<RadMenuItemBase> GetContextMenuItems(DockWindow window)
        {
            return this.GetContextMenuItems(window, false);
        }

        /// <summary>
        /// Gets the menu items, associated with the specified DockWindow.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="defaultAction">True to execute the default action, associated with each item, when an item is clicked.</param>
        /// <returns></returns>
        public List<RadMenuItemBase> GetContextMenuItems(DockWindow window, bool defaultAction)
        {
            if (window == null)
            {
                throw new ArgumentNullException("Window");
            }
            if (window.DockManager != this.DockManager)
            {
                throw new ArgumentException("Specified DockWindow is not owned by the DockManager this service is registered with.");
            }

            this.hookItemClick = defaultAction;
            List<RadMenuItemBase> items = this.BuildContextMenuItems(window);
            this.hookItemClick = true;

            return items;
        }

        #endregion

        #region Overrides

        protected override void DisposeManagedResources()
        {
            this.DisposeCurrentMenu();
            base.DisposeManagedResources();
        }

        #endregion

        #region Overridables

        /// <summary>
        /// Determines whether a context menu can be displayed for the specified window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        protected virtual bool CanDisplayMenu(DockWindow window)
        {
            //check whether we can display a menu for the specified window
            switch (window.DockState)
            {
                case DockState.TabbedDocument:
                    return this.allowDocumentContextMenu;
                default:
                    return this.allowToolContextMenu;
            }
        }

        /// <summary>
        /// Displays the context menu at the specified screen position, using the provided list of items.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="screenPos"></param>
        protected virtual void DisplayMenuCore(List<RadMenuItemBase> items, Point screenPos)
        {
            this.DisposeCurrentMenu();

            this.currentMenu = new RadContextMenu();
            this.currentMenu.ThemeName = this.DockManager.ThemeName;
            this.currentMenu.DropDown.RightToLeft = this.DockManager.RightToLeft;
            this.currentMenu.DropDown.HorizontalPopupAlignment = (this.DockManager.RightToLeft == System.Windows.Forms.RightToLeft.Yes)?
                HorizontalPopupAlignment.RightToRight : HorizontalPopupAlignment.LeftToLeft;
            foreach (RadItem item in items)
            {
                this.currentMenu.Items.Add(item);
            }

            this.currentMenu.Show(screenPos);
        }

        /// <summary>
        /// The entry point used to handle menu item clicks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnContextMenuItemClick(object sender, EventArgs e)
        {
            RadMenuItemBase menuItem = sender as RadMenuItemBase;
            DockWindow window = menuItem.Tag as DockWindow;
            Debug.Assert(window != null, "No DockWindow associated with a context menu item.");

            bool performAction = true;

            ContextMenuItemClickEventHandler eh = this.Events[ContextMenuItemClickedEventKey] as ContextMenuItemClickEventHandler;
            if (eh != null)
            {
                ContextMenuItemClickEventArgs args = new ContextMenuItemClickEventArgs(window, menuItem);
                eh(this, args);
                performAction = !args.Handled;
            }

            if (performAction)
            {
                this.PerformMenuItemAction(window, menuItem);
            }

            this.DisposeCurrentMenu();
        }

        /// <summary>
        /// Performs the core action, depending on the clicked menu item.
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="window"></param>
        protected virtual void PerformMenuItemAction(DockWindow window, RadMenuItemBase menuItem)
        {
            switch(menuItem.Name)
            {
                case ActivateWindow:
                    this.DockManager.ActiveWindow = window;
                    window.EnsureVisible();
                    break;
                case DockStateAutoHide:
                case DockStateDocked:
                case DockStateFloating:
                case DockStateHidden:
                case DockStateTabbedDocument:
                    this.DockManager.SetWindowState(window, (DockState)Enum.Parse(typeof(DockState), menuItem.Name));
                    break;
                case CloseAll:
                    this.CloseAllDocuments(null);
                    break;
                case CloseAllButThis:
                    this.CloseAllDocuments(window);
                    break;
                case CloseWindow:
                    window.Close();
                    break;
                case NewHTabGroup:
                    this.DockManager.AddDocument(window, window.TabStrip as DocumentTabStrip, DockPosition.Bottom);
                    break;
                case NewVTabGroup:
                    this.DockManager.AddDocument(window, window.TabStrip as DocumentTabStrip, DockPosition.Right);
                    break;
                case MoveToPrevTabGroup:
                    this.DockManager.MoveToPreviousDocumentTabStrip(window);
                    break;
                case MoveToNextTabGroup:
                    this.DockManager.MoveToNextDocumentTabStrip(window);
                    break;
            }
        }

        #endregion

        #region Private Implementation

        private void CloseAllDocuments(DockWindow toSkip)
        {
            this.DockManager.BeginTransactionBlock();

            foreach (DockWindow docWindow in DockHelper.GetDockWindows(this.DockManager.MainDocumentContainer, true, this.DockManager))
            {
                if (docWindow == toSkip)
                {
                    continue;
                }

                docWindow.Close();
            }

            this.DockManager.EndTransactionBlock();
        }

        /// <summary>
        /// Prepares the list of menu items, available for the specified dock window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private List<RadMenuItemBase> BuildContextMenuItems(DockWindow window)
        {
            List<RadMenuItemBase> items = new List<RadMenuItemBase>();

            switch(window.DockState)
            {
                case DockState.TabbedDocument:
                    this.AddCloseDocumentMenuItems(window, items);
                    if (window.DockType == DockType.ToolWindow)
                    {
                        items.Add(new RadMenuSeparatorItem());
                        this.AddDockStateMenuItems(window, items);
                    }
                    this.AddTabbedGroupMenuItems(window, items);
                    break;
                default:
                    this.AddDockStateMenuItems(window, items);
                    break;
            }

            return items;
        }

        /// <summary>
        /// Prepares the list of opened documents within the specified DocumentTabStrip instance.
        /// </summary>
        /// <param name="strip"></param>
        /// <returns></returns>
        private List<RadMenuItemBase> BuildContextMenuItems(DocumentTabStrip strip)
        {
            List<RadMenuItemBase> items = new List<RadMenuItemBase>();
            foreach (DockWindow window in this.DockManager.DocumentManager.GetActiveWindowList(strip))
            {
                items.Add(this.CreateMenuItem(window, ActivateWindow, null, true));
            }

            return items;
        }

        /// <summary>
        /// Adds the menu items, which alter the DockState member of the specified window.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="items"></param>
        private void AddDockStateMenuItems(DockWindow window, List<RadMenuItemBase> items)
        {
            RadDockLocalizationProvider localizationProvider = RadDockLocalizationProvider.CurrentProvider;
            RadMenuItem item;

            //floating state
            item = this.CreateMenuItem(window, DockStateFloating, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuFloating));
            if (window.DockState == DockState.Floating)
            {
                item.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            }
            else if(!this.DockManager.CanChangeWindowState(window, DockState.Floating))
            {
                item.Enabled = false;
            }
            items.Add(item);

            //docked state
            item = this.CreateMenuItem(window, DockStateDocked, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuDockable));
            if (window.DockState == DockState.Docked)
            {
                item.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            }
            else if(!this.DockManager.CanChangeWindowState(window, DockState.Docked))
            {
                item.Enabled = false;
            }
            items.Add(item);

            if (window.DockState != DockState.TabbedDocument)
            {
                //tabbed document state
                item = this.CreateMenuItem(window, DockStateTabbedDocument, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuTabbedDocument));
                if (!this.DockManager.CanChangeWindowState(window, DockState.TabbedDocument))
                {
                    item.Enabled = false;
                }
                items.Add(item);

                //auto-hide state
                item = this.CreateMenuItem(window, DockStateAutoHide, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuAutoHide));
                if (window.DockState == DockState.AutoHide)
                {
                    item.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
                }
                else if (!this.DockManager.CanChangeWindowState(window, DockState.AutoHide))
                {
                    item.Enabled = false;
                }
                items.Add(item);
            }

            //hidden state
            item = this.CreateMenuItem(window, DockStateHidden, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuHide));
            if (window.DockState == DockState.Hidden)
            {
                item.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            }
            else if (!this.DockManager.CanChangeWindowState(window, DockState.Hidden))
            {
                item.Enabled = false;
            }
            items.Add(item);
        }

        /// <summary>
        /// Adds the menu items, associated with close action upon the specified window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="items"></param>
        private void AddCloseDocumentMenuItems(DockWindow window, List<RadMenuItemBase> items)
        {
            RadDockLocalizationProvider localizationProvider = RadDockLocalizationProvider.CurrentProvider;

            items.Add(this.CreateMenuItem(window, CloseWindow, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuClose)));
            items.Add(this.CreateMenuItem(window, CloseAllButThis, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuCloseAllButThis)));
            items.Add(this.CreateMenuItem(window, CloseAll, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuCloseAll)));
        }

        /// <summary>
        /// Adds the menu items, related to the tabbed groups in the DocumentContainer
        /// </summary>
        /// <param name="window"></param>
        /// <param name="items"></param>
        private void AddTabbedGroupMenuItems(DockWindow window, List<RadMenuItemBase> items)
        {
            DockTabStrip strip = window.DockTabStrip;
            if (strip == null)
            {
                return;
            }

            RadDockLocalizationProvider localizationProvider = RadDockLocalizationProvider.CurrentProvider;

            if (strip.TabPanels.Count > 1)
            {
                items.Add(new RadMenuSeparatorItem());
                items.Add(this.CreateMenuItem(window, NewHTabGroup, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuNewHorizontalTabGroup)));
                items.Add(this.CreateMenuItem(window, NewVTabGroup, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuNewVerticalTabGroup)));
            }

            bool canMoveBack = this.DockManager.GetPreviousDocumentStrip(window) != null;
            bool canMoveForward = this.DockManager.GetNextDocumentStrip(window) != null;

            if (canMoveBack || canMoveForward)
            {
                items.Add(new RadMenuSeparatorItem());
            }

            if (canMoveBack)
            {
                items.Add(this.CreateMenuItem(window, MoveToPrevTabGroup, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuMoveToPreviousTabGroup)));
            }

            if (canMoveForward)
            {
                items.Add(this.CreateMenuItem(window, MoveToNextTabGroup, localizationProvider.GetLocalizedString(RadDockStringId.ContextMenuMoveToNextTabGroup)));
            }
        }

        private void DisposeCurrentMenu()
        {
            if (this.currentMenu == null)
            {
                return;
            }

            foreach (RadMenuItemBase item in this.currentMenu.Items)
            {
                item.Click -= OnContextMenuItemClick;
            }

            this.currentMenu.Dispose();
            this.currentMenu = null;

            GC.Collect();
        }

        private RadMenuItem CreateMenuItem(DockWindow window, string name)
        {
            return this.CreateMenuItem(window, name, null);
        }

        private RadMenuItem CreateMenuItem(DockWindow window, string name, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = window.Text;
            }

            return this.CreateMenuItem(window, name, text, false);
        }

        private RadMenuItem CreateMenuItem(DockWindow window, string name, string text, bool useImage)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = window.Text;
            }

            RadMenuItem item = new RadMenuItem(text);
            if (useImage)
            {
                item.Image = window.Image;
            }
            if (hookItemClick)
            {
                item.Click += OnContextMenuItemClick;
            }
            item.Name = name;
            item.Tag = window;

            return item;
        }

        #endregion
    }
}
