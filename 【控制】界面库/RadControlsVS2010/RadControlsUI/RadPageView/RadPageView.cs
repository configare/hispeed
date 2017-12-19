using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using Telerik.WinControls.Themes.ControlDefault;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a control that has a collection of pages and displays one page at a time.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.ContainersGroup)]
    [Designer(DesignerConsts.RadPageViewDesignerString)]
    [DefaultProperty("Pages")]
    [DefaultEvent("SelectedPageChanged")]
    [ToolboxItem(true)]
    public class RadPageView : RadNCEnabledControl
    {
        #region Event Keys

        private static object PageAddingEventKey;
        private static object PageAddedEventKey;
        private static object PageRemovingEventKey;
        private static object PageRemovedEventKey;
        private static object PageIndexChangingEventKey;
        private static object PageIndexChangedEventKey;
        private static object PagesClearingEventKey;
        private static object PagesClearedEventKey;
        private static object PageCollapsingEventKey;
        private static object PageCollapsedEventKey;
        private static object PageExpandingEventKey;
        private static object PageExpandedEventKey;
        private static object SelectedPageChangingEventKey;
        private static object SelectedPageChangedEventKey;
        private static object ItemListMenuDisplayingEventKey;
        private static object ItemListMenuDisplayedEventKey;
        private static object ViewModeChangingEventKey;
        private static object ViewModeChangedEventKey;
        private static object NewPageRequestedEventKey;

        #endregion

        #region Fields

        private RadPageViewPageCollection pages;
        private RadPageViewElement viewElement;
        private PageViewMode viewMode;
        private bool allowPageIndexChange;
        private byte suspendEvents;
        private Color pageBackColor;

        #endregion

        #region Constructor

        static RadPageView()
        {
            PageAddingEventKey = new object();
            PageAddedEventKey = new object();
            PageRemovingEventKey = new object();
            PageRemovedEventKey = new object();
            PageIndexChangingEventKey = new object();
            PageIndexChangedEventKey = new object();
            PagesClearingEventKey = new object();
            PagesClearedEventKey = new object();
            PageCollapsingEventKey = new object();
            PageCollapsedEventKey = new object();
            PageExpandingEventKey = new object();
            PageExpandedEventKey = new object();
            SelectedPageChangingEventKey = new object();
            SelectedPageChangedEventKey = new object();
            ItemListMenuDisplayingEventKey = new object();
            ItemListMenuDisplayedEventKey = new object();
            ViewModeChangingEventKey = new object();
            ViewModeChangedEventKey = new object();
            NewPageRequestedEventKey = new object();

            new ControlDefault_RadPageView_Telerik_WinControls_RootRadElement().DeserializeTheme();
        }

        protected override void Construct()
        {
            base.Construct();

            this.pages = this.CreatePagesInstance();

            this.viewMode = PageViewMode.Strip;
            this.UpdateUI();
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when page item is about to be created.
        /// </summary>
        public event EventHandler<RadPageViewItemCreatingEventArgs> ItemCreating
        {
            add
            {
                if (this.viewElement != null)
                {
                    this.viewElement.ItemCreating += value;
                }
            }
            remove
            {
                if (this.viewElement != null)
                {
                    this.viewElement.ItemCreating -= value;
                }
            }
        }

        /// <summary>
        /// Raised when the current mode of the view is about to change. Cancelable.
        /// </summary>
        public event EventHandler NewPageRequested
        {
            add
            {
                this.Events.AddHandler(NewPageRequestedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(NewPageRequestedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the current mode of the view is about to change. Cancelable.
        /// </summary>
        public event EventHandler<RadPageViewModeChangingEventArgs> ViewModeChanging
        {
            add
            {
                this.Events.AddHandler(ViewModeChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ViewModeChangingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the current mode of the view has changed.
        /// </summary>
        public event EventHandler<RadPageViewModeEventArgs> ViewModeChanged
        {
            add
            {
                this.Events.AddHandler(ViewModeChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ViewModeChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the built-in ItemsList menu is about to be displayed. Cancelable.
        /// </summary>
        public event EventHandler<RadPageViewMenuDisplayingEventArgs> ItemListMenuDisplaying
        {
            add
            {
                this.Events.AddHandler(ItemListMenuDisplayingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ItemListMenuDisplayingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the built-in ItemsList menu is displayed.
        /// </summary>
        public event EventHandler ItemListMenuDisplayed
        {
            add
            {
                this.Events.AddHandler(ItemListMenuDisplayedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ItemListMenuDisplayedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when a new page is about to be added to the view. Cancelable.
        /// </summary>
        public event EventHandler<RadPageViewCancelEventArgs> PageAdding
        {
            add
            {
                this.Events.AddHandler(PageAddingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageAddingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when a new page has been successfully added to the view.
        /// </summary>
        public event EventHandler<RadPageViewEventArgs> PageAdded
        {
            add
            {
                this.Events.AddHandler(PageAddedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageAddedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when a page is about to be removed from the view. Cancelable.
        /// </summary>
        public event EventHandler<RadPageViewCancelEventArgs> PageRemoving
        {
            add
            {
                this.Events.AddHandler(PageRemovingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageRemovingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when a page has been successfully removed from the view.
        /// </summary>
        public event EventHandler<RadPageViewEventArgs> PageRemoved
        {
            add
            {
                this.Events.AddHandler(PageRemovedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageRemovedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when a page is about to change its index. Cancelable.
        /// </summary>
        public event EventHandler<RadPageViewIndexChangingEventArgs> PageIndexChanging
        {
            add
            {
                this.Events.AddHandler(PageIndexChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageIndexChangingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when a page's index has been successfully changed.
        /// </summary>
        public event EventHandler<RadPageViewIndexChangedEventArgs> PageIndexChanged
        {
            add
            {
                this.Events.AddHandler(PageIndexChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageIndexChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when all pages are about to be removed from the view. Cancelable.
        /// </summary>
        public event CancelEventHandler PagesClearing
        {
            add
            {
                this.Events.AddHandler(PagesClearingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PagesClearingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when all pages have been successfully removed from the view.
        /// </summary>
        public event EventHandler PagesCleared
        {
            add
            {
                this.Events.AddHandler(PagesClearedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PagesClearedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the content of a <see cref="RadPageViewPage"/> is expanding.
        /// This event is only raised when the view mode of the control is set
        /// to ExplorerBar.
        /// </summary>
        public event EventHandler<RadPageViewCancelEventArgs> PageExpanding
        {
            add
            {
                this.Events.AddHandler(PageExpandingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageExpandingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the content of a <see cref="RadPageViewPage"/> is expanded.
        /// This event is only raised when the view mode of the control is set
        /// to ExplorerBar.
        ///</summary>
        public event EventHandler<RadPageViewEventArgs> PageExpanded
        {
            add
            {
                this.Events.AddHandler(PageExpandedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageExpandedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the content of a <see cref="RadPageViewPage"/> is collapsing.
        /// This event is only raised when the view mode of the control is set
        /// to ExplorerBar.
        ///</summary>
        public event EventHandler<RadPageViewCancelEventArgs> PageCollapsing
        {
            add
            {
                this.Events.AddHandler(PageCollapsingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageCollapsingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the content of a <see cref="RadPageViewPage"/> is collapsed.
        /// This event is only raised when the view mode of the control is set
        /// to ExplorerBar.
        ///</summary>
        public event EventHandler<RadPageViewEventArgs> PageCollapsed
        {
            add
            {
                this.Events.AddHandler(PageCollapsedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageCollapsedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when currently selected page has changed.
        /// </summary>
        public event EventHandler<RadPageViewCancelEventArgs> SelectedPageChanging
        {
            add
            {
                this.Events.AddHandler(SelectedPageChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(SelectedPageChangingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when currently selected page has changed.
        /// </summary>
        public event EventHandler SelectedPageChanged
        {
            add
            {
                this.Events.AddHandler(SelectedPageChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(SelectedPageChangedEventKey, value);
            }
        }

        /// <summary>
        /// Temporary suspends event raising.
        /// </summary>
        public void SuspendEvents()
        {
            this.suspendEvents++;
        }

        /// <summary>
        /// Resumes event raising, previously suspended by a SuspendEvents call.
        /// </summary>
        public void ResumeEvents()
        {
            if (this.suspendEvents > 0)
            {
                this.suspendEvents--;
            }
        }

        /// <summary>
        /// Determines whether event raising is currently enabled.
        /// </summary>
        protected override bool CanRaiseEvents
        {
            get
            {
                if (this.suspendEvents > 0)
                {
                    return false;
                }

                return base.CanRaiseEvents;
            }
        }

        #endregion

        #region Properties

        protected override Size DefaultSize
        {
            get
            {
                return new Size(400, 300);
            }
        }

        /// <summary>
        /// Gets or sets the BackColor of all pages.
        /// </summary>
        [Description("Gets or sets the BackColor of all pages.")]
        public Color PageBackColor
        {
            get
            {
                return this.pageBackColor;
            }
            set
            {
                if (this.pageBackColor == value)
                {
                    return;
                }

                this.pageBackColor = value;
                foreach(RadPageViewPage page in this.pages)
                {
                    page.OnPageBackColorChanged(EventArgs.Empty);
                }
            }
        }

        bool ShouldSerializePageBackColor()
        {
            return this.pageBackColor != Color.Empty;
        }

        /// <summary>
        /// Gets or sets the current mode of the view.
        /// </summary>
        [DefaultValue(PageViewMode.Strip)]
        [Description("Gets or sets the current mode of the view.")]
        public PageViewMode ViewMode
        {
            get
            {
                return this.viewMode;
            }
            set
            {
                if (this.viewMode == value)
                {
                    return;
                }
                PageViewMode oldViewMode = this.viewMode;
                RadPageViewModeChangingEventArgs e = new RadPageViewModeChangingEventArgs(value);
                this.OnViewModeChanging(e);
                if (e.Cancel)
                {
                    return;
                }

                this.viewMode = value;
                this.UpdateUI();

                //This is needed since the explorer bar element modifies the non-client area of the control.
                //After changing the view mode we need to reset the nc-modificatinos performed by the explorer bar element.
                if (oldViewMode == PageViewMode.ExplorerBar)
                {
                    NativeMethods.SetWindowPos(
                        new HandleRef(null, this.Handle),
                        new HandleRef(null, IntPtr.Zero),
                        0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOOWNERZORDER | NativeMethods.SWP_DRAWFRAME);
                }

                this.OnViewModeChanged(new RadPageViewModeEventArgs(this.viewMode));
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                if (this.viewElement == null || !this.IsLoaded)
                {
                    return base.DisplayRectangle;
                }

                return this.viewElement.GetContentAreaRectangle();
            }
        }

        /// <summary>
        /// Gets or sets the RadPageViewPage instance that is currently selected within the view.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public RadPageViewPage SelectedPage
        {
            get
            {
                if (this.viewElement == null || this.viewElement.SelectedItem == null)
                {
                    return null;
                }

                return this.viewElement.SelectedItem.Page;
            }
            set
            {
                if (this.SelectedPage == value)
                {
                    return;
                }

                if (value != null && value.Owner != this)
                {
                    throw new ArgumentException("SelectedPage must be owned by the same RadPageView");

                }
                RadPageViewCancelEventArgs cancelArgs = new RadPageViewCancelEventArgs(value);
                this.OnSelectedPageChanging(cancelArgs);
                if (cancelArgs.Cancel)
                {
                    return;
                }

                this.SetSelectedPage(new RadPageViewEventArgs(value));
                this.OnSelectedPageChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the collection of pages for this view.
        /// </summary>
        [Description("Gets the collection of pages for this view.")]
        public RadPageViewPageCollection Pages
        {
            get
            {
                return this.pages;
            }
        }

        /// <summary>
        /// Gets the current RadPageViewElement instance that represents the UI of the view.
        /// </summary>
        [Description("Gets the current RadPageViewElement instance that represents the UI of the view.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPageViewElement ViewElement
        {
            get
            {
                return this.viewElement;
            }
        }

        internal bool AllowPageIndexChange
        {
            get
            {
                return this.allowPageIndexChange;
            }
        }

        #endregion

        #region NC Handling

        protected override void OnNCPaint(Graphics g)
        {
            base.OnNCPaint(g);
            if (this.viewElement != null)
            {
                this.viewElement.OnNCPaint(g);
            }
        }

        protected override Padding GetNCMetrics()
        {
            if (this.viewElement == null)
                return Padding.Empty;
            return this.viewElement.GetNCMetrics();
        }

        protected override bool EnableNCModification
        {
            get
            {
                if (this.viewElement == null)
                    return false;
                return this.viewElement.EnableNCModification;
            }
        }

        protected override bool EnableNCPainting
        {
            get
            {
                if (this.viewElement == null)
                    return false;
                return this.viewElement.EnableNCPainting;
            }
        }

        #endregion

        #region Overrides

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Left ||
                keyData == Keys.Right ||
                keyData == Keys.Up ||
                keyData == Keys.Down)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            this.viewElement.ProcessKeyDown(e);
        }

        protected override bool CanEditElementAtDesignTime(RadElement element)
        {
            //items are dynamically created for pages, do not edit them
            if (element is RadPageViewItem)
            {
                return false;
            }

            return base.CanEditElementAtDesignTime(element);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.viewElement.CallDoMouseWheel(e);
            base.OnMouseWheel(e);
        }

        #endregion

        #region Page Selection

        protected internal virtual void OnPageCollapsed(RadPageViewEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewEventArgs> eh = this.Events[PageCollapsedEventKey] as EventHandler<RadPageViewEventArgs>;

            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageCollapsing(RadPageViewCancelEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewCancelEventArgs> eh = this.Events[PageCollapsingEventKey] as EventHandler<RadPageViewCancelEventArgs>;

            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageExpanding(RadPageViewCancelEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewCancelEventArgs> eh = this.Events[PageExpandingEventKey] as EventHandler<RadPageViewCancelEventArgs>;

            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageExpanded(RadPageViewEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewEventArgs> eh = this.Events[PageExpandedEventKey] as EventHandler<RadPageViewEventArgs>;

            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected virtual void OnSelectedPageChanged(EventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler eh = this.Events[SelectedPageChangedEventKey] as EventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected virtual void OnSelectedPageChanging(RadPageViewCancelEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewCancelEventArgs> eh = this.Events[SelectedPageChangingEventKey] as EventHandler<RadPageViewCancelEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected virtual void SetSelectedPage(RadPageViewEventArgs e)
        {
            this.SuspendLayout();
            this.viewElement.OnSelectedPageChanged(e);
            this.ResumeLayout(false);
        }

        #endregion

        #region Pages Methods

        public void EnsurePageVisible(RadPageViewPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("Page");
            }

            if (page.Owner != this)
            {
                throw new InvalidOperationException("Page is not owned by this view.");
            }

            this.viewElement.EnsureItemVisible(page.Item);
        }

        internal void EnablePageIndexChange()
        {
            this.allowPageIndexChange = true;
        }

        internal void DisablePageIndexChange()
        {
            this.allowPageIndexChange = false;
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new RadPageViewControlCollection(this);
        }

        protected virtual RadPageViewPageCollection CreatePagesInstance()
        {
            return new RadPageViewPageCollection(this);
        }

        protected internal virtual void OnNewPageRequested(EventArgs e)
        {
            EventHandler eh = this.Events[NewPageRequestedEventKey] as EventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageAdding(RadPageViewCancelEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewCancelEventArgs> eh = this.Events[PageAddingEventKey] as EventHandler<RadPageViewCancelEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageAdded(RadPageViewEventArgs e)
        {
            Debug.Assert(this.viewElement != null, "Must have UI instance at this point.");

            e.Page.Attach(this);
            this.viewElement.OnPageAdded(e);

            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewEventArgs> eh = this.Events[PageAddedEventKey] as EventHandler<RadPageViewEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPagesClearing(CancelEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            CancelEventHandler eh = this.Events[PagesClearingEventKey] as CancelEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPagesCleared(EventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler eh = this.Events[PagesClearedEventKey] as EventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageRemoving(RadPageViewCancelEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewCancelEventArgs> eh = this.Events[PageRemovingEventKey] as EventHandler<RadPageViewCancelEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageRemoved(RadPageViewEventArgs e)
        {
            Debug.Assert(this.viewElement != null, "Must have UI instance at this point.");

            e.Page.Detach();
            this.viewElement.OnPageRemoved(e);

            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewEventArgs> eh = this.Events[PageRemovedEventKey] as EventHandler<RadPageViewEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageIndexChanging(RadPageViewIndexChangingEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewIndexChangingEventArgs> eh = this.Events[PageIndexChangingEventKey] as EventHandler<RadPageViewIndexChangingEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnPageIndexChanged(RadPageViewIndexChangedEventArgs e)
        {
            Debug.Assert(this.viewElement != null, "Must have UI instance at this point.");
            this.viewElement.OnPageIndexChanged(e);

            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewIndexChangedEventArgs> eh = this.Events[PageIndexChangedEventKey] as EventHandler<RadPageViewIndexChangedEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnItemListMenuDisplaying(RadPageViewMenuDisplayingEventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler<RadPageViewMenuDisplayingEventArgs> eh = this.Events[ItemListMenuDisplayingEventKey] as EventHandler<RadPageViewMenuDisplayingEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected internal virtual void OnItemListMenuDisplayed(EventArgs e)
        {
            if (!this.CanRaiseEvents)
            {
                return;
            }

            EventHandler eh = this.Events[ItemListMenuDisplayedEventKey] as EventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        #endregion

        #region View Modes

        protected void UpdateUI()
        {
            RadPageViewPage currentSelected = this.SelectedPage;
            if (this.viewElement != null)
            {
                this.viewElement.Dispose();
            }

            this.viewElement = this.CreateUI();
            this.viewElement.Owner = this;

            this.SuspendEvents();
            foreach (RadPageViewPage page in this.pages)
            {
                this.viewElement.OnPageAdded(new RadPageViewEventArgs(page));
            }
            this.RootElement.Children.Add(this.viewElement);

            if (currentSelected != null)
            {
                this.SelectedPage = currentSelected;
            }

            this.ResumeEvents();
        }

        protected virtual void OnViewModeChanging(RadPageViewModeChangingEventArgs e)
        {
            EventHandler<RadPageViewModeChangingEventArgs> eh = this.Events[ViewModeChangingEventKey] as EventHandler<RadPageViewModeChangingEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected virtual void OnViewModeChanged(RadPageViewModeEventArgs e)
        {
            EventHandler<RadPageViewModeEventArgs> eh = this.Events[ViewModeChangedEventKey] as EventHandler<RadPageViewModeEventArgs>;
            if (eh != null)
            {
                eh(this, e);
            }

        }

        protected virtual RadPageViewElement CreateUI()
        {
            switch(this.viewMode)
            {
                case PageViewMode.Stack:
                    return new RadPageViewStackElement();
                case PageViewMode.Outlook:
                    return new RadPageViewOutlookElement();
                case PageViewMode.ExplorerBar:
                    return new RadPageViewExplorerBarElement();
                case PageViewMode.Backstage:
                    return new RadPageViewBackstageElement();
                default:
                    return new RadPageViewStripElement();
            }
        }

        #endregion

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadPageViewAccessibilityObject(this);
        }
    }
}
