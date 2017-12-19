using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Security;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.Design;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false)]
    public class TabStripPanel : SplitPanel
    {
        #region Fields

        internal static bool DisableSelection;

        private static readonly object EVENT_DESELECTED;
        private static readonly object EVENT_DESELECTING;
        private static readonly object EVENT_RIGHTTOLEFTLAYOUTCHANGED;
        private static readonly object EVENT_SELECTED;
        private static readonly object EVENT_SELECTING;

        protected RadPageViewTabStripElement tabStripElement;
        private TabPanelCollection tabPanels;
        private int selectedIndex = -1;
        private ImageList tabImageList;
        private Point dragStart;
        private bool dragging;
        private bool lockChildIndexSet;
        private int suspendStripItemsChanged;
        private int suspendStripSelecting;
        private bool showTabStrip;
        private bool showItemCloseButton;
        private bool pageViewUpdate = false;
        private TabStripAlignment tabStripAlign;
        private TabStripTextOrientation tabStripTextOrientation;

        #endregion

        #region Initialization & Dispose

        static TabStripPanel()
        {
            //DEFAULT_ITEMSIZE = Size.Empty;
            //DEFAULT_PADDING = new Point(6, 3);
            EVENT_DESELECTING = new object();
            EVENT_DESELECTED = new object();
            EVENT_SELECTING = new object();
            EVENT_SELECTED = new object();
            EVENT_RIGHTTOLEFTLAYOUTCHANGED = new object();
        }

        public TabStripPanel()
        {
            this.tabPanels = new TabPanelCollection(this);
            this.showTabStrip = true;
            this.tabStripAlign = this.DefaultTabStripAlignment;
            this.tabStripTextOrientation = TabStripTextOrientation.Default;
            this.Behavior.BitmapRepository.DisableBitmapCache = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.tabImageList != null)
                {
                    this.tabImageList.Disposed -= new EventHandler(this.DetachImageList);
                }

                if (this.tabStripElement != null)
                {
                    this.tabStripElement.ItemSelecting -= new EventHandler<RadPageViewItemSelectingEventArgs>(tabStripElement_ItemSelecting);
                    this.tabStripElement.ItemsChanged -= new EventHandler<RadPageViewItemsChangedEventArgs>(tabStripElement_ItemsChanged);
                }
            }

            base.Dispose(disposing);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.tabStripElement = this.CreateTabStripElementInstance();

            this.tabStripElement.StretchHorizontally = true;
            this.tabStripElement.StretchVertically = true;
            this.tabStripElement.StripButtons = StripViewButtons.None;
            this.tabStripElement.ContentArea.Visibility = ElementVisibility.Collapsed;
            this.tabStripElement.StripAlignment = StripViewAlignment.Bottom;
            this.tabStripElement.ItemSelecting += new EventHandler<RadPageViewItemSelectingEventArgs>(tabStripElement_ItemSelecting);
            this.tabStripElement.ItemsChanged += new EventHandler<RadPageViewItemsChangedEventArgs>(tabStripElement_ItemsChanged);
            this.splitPanelElement.Children.Add(this.tabStripElement);

            if (this.tabPanels != null)
            {
                SyncTabElements();
            }
        }

        protected virtual RadPageViewTabStripElement CreateTabStripElementInstance()
        {
            return new RadPageViewTabStripElement();
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }

        #endregion

        #region Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public TabPanel SelectedTab
        {
            get
            {
                int selectedIndex = this.SelectedIndex;
                if (this.tabPanels != null && this.tabPanels.Count > 0)
                {
                    if (selectedIndex < 0)
                    {
                        this.selectedIndex = 0;
                    }
                    if (selectedIndex >= 0 && selectedIndex < this.tabPanels.Count)
                    {
                        return this.tabPanels[selectedIndex];
                    }
                }

                return null;
            }
            set
            {
                int num = this.GetTabPanelIndex(value);
                this.SelectedIndex = num;
            }

        }

        [DefaultValue(-1), Browsable(false), Category("Behavior")]
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }
            set
            {
                if (this.selectedIndex != value)
                {
                    TabPanel oldPanel = GetTabPanel(selectedIndex, false);
                    TabPanel newPanel = GetTabPanel(value, false);
                    TabStripPanelSelectedIndexChangingEventArgs args = new TabStripPanelSelectedIndexChangingEventArgs(this.selectedIndex, value, oldPanel, newPanel);
                    OnSelectedIndexChanging(args);
                    if (args.Cancel)
                    {
                        return;
                    }
                }

                if (this.IsHandleCreated)
                {
                    if (value < -1)
                    {
                        throw new ArgumentOutOfRangeException("SelectedIndex");
                    }

                    if (this.selectedIndex != value)
                    {
                        this.selectedIndex = value;
                        this.UpdateTabSelection(true);
                        OnSelectedIndexChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    this.selectedIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets the default alignment of the TabStripElement.
        /// </summary>
        protected virtual TabStripAlignment DefaultTabStripAlignment
        {
            get
            {
                return TabStripAlignment.Bottom;
            }
        }

        /// <summary>
        /// Gets the default text orientation.
        /// </summary>
        protected virtual TabStripTextOrientation DefaultTabStripTextOrientation
        {
            get
            {
                if (this.tabStripAlign == TabStripAlignment.Top || this.tabStripAlign == TabStripAlignment.Bottom)
                {
                    return TabStripTextOrientation.Horizontal;
                }

                return TabStripTextOrientation.Vertical;
            }
        }

        /// <summary>
        /// Gets or sets the text orientation of the <see cref="RadTabStripElement">TabStripElement</see> used to switch among child panels.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the text orientation of the TabStripElement used to switch among child panels.")]
        public TabStripTextOrientation TabStripTextOrientation
        {
            get
            {
                return this.tabStripTextOrientation;
            }
            set
            {
                //check for actual value change
                if (value == this.tabStripTextOrientation)
                {
                    return;
                }

                this.tabStripTextOrientation = value;
                this.OnTabStripTextOrientationChanged(EventArgs.Empty);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabPanelCollection TabPanels
        {
            get { return tabPanels; }
        }

        [Browsable(false)]
        public bool HasVisibleTabPanels
        {
            get
            {
                return this.tabPanels.Count > 0;
            }
        }

        /// <summary>
        /// Determines whether each TabStripItem will display a CloseButton, which allows for explicit close of its corresponding panel.
        /// </summary>
        [DefaultValue(false)]
        [Description("Determines whether each TabStripItem will display a CloseButton, which allows for explicit close of its corresponding panel.")]
        public bool ShowItemCloseButton
        {
            get
            {
                return this.showItemCloseButton;
            }
            set
            {
                if (this.showItemCloseButton == value)
                {
                    return;
                }

                this.showItemCloseButton = value;
                foreach (TabStripItem item in this.tabStripElement.Items)
                {
                    item.ShowCloseButton = this.showItemCloseButton;
                }

                this.UpdateLayout();
            }
        }

        /// <summary>
        /// Gets the point where the mouse was pressed and a drag operation has been instanciated.
        /// </summary>
        [Browsable(false)]
        public Point DragStart
        {
            get
            {
                return this.dragStart;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="RadTabStripElement">TabStripElement</see> used to navigate among child panels is displayed.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Determines whether the TabStripElement used to navigate among child panels is displayed.")]
        public bool TabStripVisible
        {
            get
            {
                return this.showTabStrip;
            }
            set
            {
                if (this.showTabStrip == value)
                {
                    return;
                }

                this.showTabStrip = value;

                this.UpdateTabStripVisibility(this.GetTabStripVisible());
                this.UpdateLayout();
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the <see cref="RadTabStripElement">TabStripElement</see> used to switch among child panels.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the alignment of the TabStripElement used to switch among child panels.")]
        public TabStripAlignment TabStripAlignment
        {
            get
            {
                return this.tabStripAlign;
            }
            set
            {
                //check for default value
                if (value == TabStripAlignment.Default)
                {
                    value = this.DefaultTabStripAlignment;
                }

                if (this.tabStripAlign == value)
                {
                    return;
                }

                this.tabStripAlign = value;
                this.OnTabStripAlignmentChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public RadPageViewStripElement TabStripElement
        {
            get
            {
                return this.tabStripElement;
            }
        }

        [Browsable(false)]
        public virtual Rectangle TabPanelBounds
        {
            get
            {
                bool tabStripVisible = this.GetTabStripVisible();
                this.UpdateTabStripVisibility(tabStripVisible);

                Rectangle bounds = this.DisplayRectangle;
                Padding padding = this.tabStripElement.ContentArea.Padding + this.tabStripElement.ContentArea.GetBorderThickness(true);
                Size tabsSize = tabStripVisible ? this.tabStripElement.ItemContainer.Size : Size.Empty;

                Rectangle panelBounds = Rectangle.Empty;
                switch (this.tabStripAlign)
                {
                    case TabStripAlignment.Left:
                        panelBounds = new Rectangle(
                                bounds.Left + padding.Left + tabsSize.Width,
                                bounds.Top + padding.Top,
                                bounds.Width - (padding.Horizontal + tabsSize.Width),
                                bounds.Height - padding.Vertical);
                        break;
                    case TabStripAlignment.Top:
                        panelBounds = new Rectangle(
                                bounds.Left + padding.Left,
                                bounds.Top + padding.Top + tabsSize.Height,
                                bounds.Width - padding.Horizontal,
                                bounds.Height - (padding.Vertical + tabsSize.Height));
                        break;
                    case TabStripAlignment.Right:
                        panelBounds = new Rectangle(
                                bounds.Left + padding.Left,
                                bounds.Top + padding.Top,
                                bounds.Width - (padding.Horizontal + tabsSize.Width),
                                bounds.Height - padding.Vertical);
                        break;
                    case TabStripAlignment.Bottom:
                        panelBounds = new Rectangle(
                                bounds.Left + padding.Left,
                                bounds.Top + padding.Top,
                                bounds.Width - padding.Horizontal,
                                bounds.Height - (padding.Vertical + tabsSize.Height));
                        break;
                }

                return panelBounds;
            }
        }

        protected Padding TabPanelPaddings
        {
            get
            {
                Rectangle bounds = this.DisplayRectangle;
                Rectangle panelBounds = this.TabPanelBounds;

                Padding padding = new Padding();
                padding.Left = panelBounds.Left - bounds.Left;
                padding.Right = bounds.Right - panelBounds.Right;
                padding.Top = panelBounds.Top - bounds.Top;
                padding.Bottom = bounds.Bottom - panelBounds.Bottom;

                return padding;
            }
        }

        /// <summary>
        /// Determines whether the child panels' Index update is currently locked.
        /// </summary>
        protected internal bool CanUpdateChildIndex
        {
            get
            {
                return !this.lockChildIndexSet;
            }
            set
            {
                this.lockChildIndexSet = !value;
            }
        }

        [DefaultValue((string)null), RefreshProperties(RefreshProperties.Repaint), Description("TabBaseImageListDescr"), Category("CatAppearance")]
        public new ImageList ImageList
        {
            get
            {
                return this.tabImageList;
            }
            set
            {
                if (this.tabImageList != value)
                {
                    if (this.tabImageList != null)
                    {
                        this.tabImageList.Disposed -= DetachImageList;
                    }

                    this.tabImageList = value;

                    if (value != null)
                    {
                        value.Disposed += DetachImageList;
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Point Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        #endregion

        #region Events

        public event TabStripPanelSelectedIndexChangingEventHandler SelectedIndexChanging;

        public event EventHandler SelectedIndexChanged;

        #endregion

        #region Event handlers

        /// <summary>
        /// Handles the click of a CloseButton on a child TabStripItem.
        /// Closes the corresponding TabPanel by default.
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void OnTabCloseButtonClicked(TabStripItem item)
        {
            this.tabPanels.Remove(item.TabPanel);
        }

        protected virtual void OnDragInitialized(Point mouse)
        {
        }

        protected virtual void OnTabStripAlignmentChanged(EventArgs e)
        {
            switch (this.tabStripAlign)
            {
                case TabStripAlignment.Top:
                    this.tabStripElement.StripAlignment = StripViewAlignment.Top;
                    break;
                case TabStripAlignment.Bottom:
                    this.tabStripElement.StripAlignment = StripViewAlignment.Bottom;
                    break;
                case TabStripAlignment.Left:
                    this.tabStripElement.StripAlignment = StripViewAlignment.Left;
                    break;
                case TabStripAlignment.Right:
                    this.tabStripElement.StripAlignment = StripViewAlignment.Right;
                    break;
            }

            this.UpdateLayout();
        }

        protected virtual void OnTabStripTextOrientationChanged(EventArgs e)
        {
            this.UpdateTextOrientation();
            this.UpdateLayout();
        }

        protected virtual void OnSelectedIndexChanging(TabStripPanelSelectedIndexChangingEventArgs e)
        {
            TabStripPanelSelectedIndexChangingEventHandler handler = SelectedIndexChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            EventHandler handler = SelectedIndexChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.dragStart = Point.Empty;
            if (e.Button == MouseButtons.Left)
            {
                this.dragStart = new Point(e.X, e.Y);
            }
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);

            this.dragging = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button != MouseButtons.Left || this.dragging)
            {
                return;
            }

            Point mouse = new Point(e.X, e.Y);
            if (SplitPanelHelper.ShouldBeginDrag(mouse, this.dragStart))
            {
                this.dragging = true;
                this.OnDragInitialized(mouse);
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            this.UpdateActivePanelBounds();

            base.OnLayout(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.tabPanels.Count > 0)
            {
                if (this.selectedIndex < 0 || this.selectedIndex > this.tabPanels.Count - 1)
                {
                    this.selectedIndex = 0;
                }
            }

            this.UpdateTabSelection(true);
        }

        protected override bool ProcessFocusRequested(RadElement element)
        {
            return false;
        }

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();

            this.UpdateLayout();
        }

       
        private void tabStripElement_ItemSelecting(object sender, RadPageViewItemSelectingEventArgs e)
        {
            if (this.suspendStripSelecting > 0)
            {
                return;
            }

            TabStripItem nextItem = e.NextItem as TabStripItem;
            if (nextItem != null)
            {
                this.pageViewUpdate = true;
                if (!this.SelectTab(nextItem.TabPanel))
                {
                    e.Cancel = true;
                }
                this.pageViewUpdate = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void tabStripElement_ItemsChanged(object sender, RadPageViewItemsChangedEventArgs e)
        {
            if (this.Disposing || this.IsDisposed || !this.IsHandleCreated)
            {
                return;
            }

            if (this.suspendStripItemsChanged > 0)
            {
                return;
            }

            this.SuspendStripNotifications(true, true);

            //we need to sync item and panel indexes
            switch (e.Operation)
            {
                case ItemsChangeOperation.Inserted:
                case ItemsChangeOperation.Set:
                    TabStripItem item = e.ChangedItem as TabStripItem;
                    if (item != null)
                    {
                        int index = this.tabStripElement.Items.IndexOf(item);
                        this.Controls.SetChildIndex(item.TabPanel, index);
                        this.selectedIndex = index;
                        this.UpdateTabSelection(true);
                    }
                    break;
            }

            this.ResumeStripNotifications(true, true);
        }

        #endregion

        #region Public methods

        public bool SelectTab(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            SelectedIndex = index;
            return SelectedIndex == index;
        }

        public bool SelectTab(string tabPanelName)
        {
            if (tabPanelName == null)
            {
                throw new ArgumentNullException("tabPanelName");
            }
            TabPanel tabPanel = this.TabPanels[tabPanelName];
            return SelectTab(tabPanel);
        }

        public bool SelectTab(TabPanel tabPanel)
        {
            if (tabPanel == null)
            {
                throw new ArgumentNullException("tabPanel");
            }
            int index = this.GetTabPanelIndex(tabPanel);
            return SelectTab(index);
        }

        public void DeselectTab(int index)
        {
            TabPanel tabPanel = this.GetTabPanel(index, true);
            if (this.SelectedTab == tabPanel)
            {
                if ((0 <= index) && (index < (this.TabPanels.Count - 1)))
                {
                    this.SelectedTab = this.GetTabPanel(++index, true);
                }
                else
                {
                    this.SelectedTab = this.GetTabPanel(0, true);
                }
            }
        }

        public void DeselectTab(string tabPanelName)
        {
            if (tabPanelName == null)
            {
                throw new ArgumentNullException("tabPanelName");
            }

            TabPanel tabPanel = this.TabPanels[tabPanelName];
            this.DeselectTab(tabPanel);
        }

        public void DeselectTab(TabPanel tabPanel)
        {
            if (tabPanel == null)
            {
                throw new ArgumentNullException("tabPanel");
            }

            int index = this.GetTabPanelIndex(tabPanel);
            this.DeselectTab(index);
        }

        public override string ToString()
        {
            string str = base.ToString();
            if (this.TabPanels != null)
            {
                str = str + ", TabPanels.Count: " + this.TabPanels.Count.ToString(CultureInfo.CurrentCulture);
                if (this.TabPanels.Count > 0)
                {
                    str = str + ", TabPanels[0]: " + this.TabPanels[0].ToString();
                }
            }

            return str;
        }

        #endregion

        #region Implementation

        #region Internal methods

        protected internal virtual void UpdateTabSelection(bool updateFocus)
        {
            if (!this.IsHandleCreated || this.tabPanels == null)
            {
                return;
            }

            this.SuspendLayout();

            this.lockChildIndexSet = true;
            this.SuspendStripNotifications(false, true);

            if (this.selectedIndex >= 0 && this.selectedIndex < this.tabPanels.Count)
            {
                TabPanel panel = this.tabPanels[this.selectedIndex];
                this.SetSelected(panel);

                if (updateFocus && !panel.ContainsFocus)
                {
                    panel.SelectNextControl(null, true, true, false, false);
                }
            }
            else
            {
                SelectTabItem(null);
            }

            for (int i = 0; i < this.tabPanels.Count; i++)
            {
                if (i != this.selectedIndex)
                {
                    this.tabPanels[i].Visible = false;
                }
            }

            this.ResumeStripNotifications(false, true);
            this.lockChildIndexSet = false;

            this.ResumeLayout();
        }

        /// <summary>
        /// Temporary suspends notifications like TabSelecting and TabSelected from the parented RadTabStripElement.
        /// </summary>
        protected internal void SuspendStripNotifications(bool suspendItemsChanged, bool suspendSelectionChanged)
        {
            if (suspendItemsChanged)
            {
                this.suspendStripItemsChanged++;
            }
            if (suspendSelectionChanged)
            {
                this.suspendStripSelecting++;
            }
        }

        /// <summary>
        /// Callback to notify the panel that a control has been successfully removed, tab strip has been updated and any additional update is allowed.
        /// </summary>
        /// <param name="value"></param>
        protected internal virtual void UpdateAfterControlRemoved(Control value)
        {
        }

        /// <summary>
        /// Resumes previously suspended notifications like TabSelecting and TabSelected from the parented RadTabStripElement.
        /// </summary>
        protected internal void ResumeStripNotifications(bool itemsChanged, bool selection)
        {
            if (itemsChanged && this.suspendStripItemsChanged > 0)
            {
                this.suspendStripItemsChanged--;
            }
            if (selection && this.suspendStripSelecting > 0)
            {
                this.suspendStripSelecting--;
            }
        }

        #endregion

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return false;
            }

            switch (keyData & Keys.KeyCode)
            {
                case Keys.Prior:
                case Keys.Next:
                case Keys.End:
                case Keys.Home:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Determines whether the tabstrip element is visible.
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetTabStripVisible()
        {
            if (!this.showTabStrip)
            {
                return false;
            }

            return this.tabPanels != null && this.tabPanels.Count > 0;
        }

        /// <summary>
        /// Forces layout update by explicitly re-setting the current bounds and performing a layout pass.
        /// </summary>
        protected virtual void UpdateLayout()
        {
            this.RootElement.InvalidateMeasure();
            this.RootElement.UpdateLayout();
            this.PerformLayout();
        }

        protected virtual void UpdateTabStripVisibility(bool visible)
        {
            if (this.tabStripElement != null)
            {
                if (visible)
                {
                    tabStripElement.ItemContainer.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    tabStripElement.ItemContainer.Visibility = ElementVisibility.Collapsed;
                }
            }
        }

        private void UpdateTextOrientation()
        {
            switch (this.tabStripTextOrientation)
            {
                case TabStripTextOrientation.Vertical:
                    this.tabStripElement.TextOrientation = Orientation.Vertical;
                    break;
                default:
                    this.tabStripElement.TextOrientation = Orientation.Horizontal;
                    break;
            }
        }

        private void UpdateActivePanelBounds()
        {
            TabPanel activePanel = this.SelectedTab;
            if (activePanel != null)
            {
                activePanel.Bounds = this.TabPanelBounds;
            }
            else
            {
                this.UpdateTabStripVisibility(this.GetTabStripVisible());
            }
        }

        private void RemoveTabElement(TabPanel tabPanel)
        {
            for (int i = 0; i < this.tabStripElement.Items.Count; i++)
            {
                if (((TabStripItem)this.tabStripElement.Items[i]).TabPanel.Equals(tabPanel))
                {
                    this.SuspendStripNotifications(true, true);
                    this.tabStripElement.RemoveItem(this.tabStripElement.Items[i]);
                    this.ResumeStripNotifications(true, true);
                    return;
                }
            }
        }

        private void SyncTabElements()
        {
            if (this.tabPanels.Count != this.tabStripElement.Items.Count)
            {
                this.SuspendStripNotifications(true, true);

                while (this.tabStripElement.Items.Count > 0)
                {
                    this.tabStripElement.RemoveItem(this.tabStripElement.Items[0]);
                }
                for (int i = 0; i < this.tabPanels.Count; i++)
                {
                    TabStripItem item = new TabStripItem(this.tabPanels[i]);
                    item.ShowCloseButton = this.showItemCloseButton;
                    this.tabStripElement.AddItem(item);

                }

                this.ResumeStripNotifications(true, true);
            }
        }
       
        private void SetSelected(TabPanel tabPanel)
        {
            tabPanel.SuspendLayout();

            bool selectionChange = !tabPanel.Visible;
            bool focused = this.ContainsFocus;

            tabPanel.Visible = true;
            if (selectionChange && focused)
            {
                tabPanel.Focus();
            }

            tabPanel.Bounds = this.TabPanelBounds;
            SelectTabItem(tabPanel.TabStripItem);

            tabPanel.ResumeLayout();

            if (!this.DesignMode || !selectionChange)
            {
                return;
            }

            ISelectionService selService = this.GetService(typeof(ISelectionService)) as ISelectionService;
            if (selService != null)
            {
                selService.SetSelectedComponents(new Component[] { tabPanel }, SelectionTypes.Replace);
            }
            IComponentChangeService changeService = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (changeService != null)
            {
                changeService.OnComponentChanged(this, null, null, null);
            }
        }

        private void DetachImageList(object sender, EventArgs e)
        {
            this.ImageList = null;
        }

        private TabPanel GetTabPanel(int index, bool throwException)
        {
            if ((index < 0) || (index >= this.tabPanels.Count))
            {
                if (throwException)
                {
                    throw new ArgumentOutOfRangeException("index", "InvalidArgument");
                }
                else
                {
                    return null;
                }
            }

            return this.tabPanels[index];
        }

        private int GetTabPanelIndex(TabPanel tabPanel)
        {
            if (this.tabPanels != null)
            {
                for (int i = 0; i < this.tabPanels.Count; i++)
                {
                    if (this.tabPanels[i].Equals(tabPanel))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private bool ShouldSerializeTabStripAlignment()
        {
            return this.tabStripAlign != DefaultTabStripAlignment;
        }

        private bool ShouldSerializeTabStripTextOrientation()
        {
            return this.tabStripTextOrientation != TabStripTextOrientation.Default;
        }

        private void SelectTabItem(TabStripItem tabStripItem)
        {
            if (pageViewUpdate)
            {
                return;
            }

            this.TabStripElement.SelectedItem = tabStripItem;
            if (this.TabStripElement.SelectedItem != null)
            {
                this.TabStripElement.EnsureItemVisible(this.TabStripElement.SelectedItem);
            }
        }

        #endregion

        public new class ControlCollection : Control.ControlCollection
        {
            private TabStripPanel owner;

            public ControlCollection(TabStripPanel owner)
                : base(owner)
            {
                this.owner = owner;
            }

            public override void Add(Control value)
            {
                TabPanel tabPanel = value as TabPanel;
                if (tabPanel == null)
                {
                    throw new ArgumentException("Collection may contain only TabPanel instances.");
                }

                //lock subsequent updates
                this.owner.SuspendStripNotifications(true, true);

                if (!this.owner.tabPanels.Contains(tabPanel) && this.owner.tabStripElement != null)
                {
                    tabPanel.TabStripItem = new TabStripItem(tabPanel);
                    tabPanel.TabStripItem.ShowCloseButton = this.owner.showItemCloseButton;
                    this.owner.tabStripElement.AddItem(tabPanel.TabStripItem);
                    tabPanel.TabStripItem.UpdateCloseButton(this.owner);
                }

                base.Add(tabPanel);

                tabPanel.Visible = false;

                ISite site = this.owner.Site;
                if ((site != null) && (tabPanel.Site == null))
                {
                    IContainer container = site.Container;
                    if (container != null)
                    {
                        container.Add(tabPanel);
                    }
                }

                if (this.owner.IsHandleCreated && !TabStripPanel.DisableSelection)
                {
                    this.owner.selectedIndex = this.IndexOf(tabPanel);
                    this.owner.UpdateTabSelection(true);
                }
                this.owner.ResumeStripNotifications(true, true);
            }

            public override void SetChildIndex(Control child, int newIndex)
            {
                if (this.owner.lockChildIndexSet)
                {
                    return;
                }

                base.SetChildIndex(child, newIndex);

                if (this.owner.suspendStripItemsChanged > 0)
                {
                    return;
                }

                this.owner.SuspendStripNotifications(true, true);

                for (int i = 0; i < this.owner.TabStripElement.Items.Count; i++)
                {
                    TabStripItem tabItem = this.owner.TabStripElement.Items[i] as TabStripItem;
                    if (tabItem != null)
                    {
                        if (tabItem.TabPanel == child)
                        {
                            this.owner.TabStripElement.RemoveItem(tabItem);
                            if (newIndex >= 0)
                            {
                                this.owner.TabStripElement.InsertItem(newIndex, tabItem);
                            }
                            else
                            {
                                this.owner.TabStripElement.AddItem(tabItem);
                            }

                            this.owner.selectedIndex = newIndex;
                            this.owner.UpdateTabSelection(true);
                            break;
                        }
                    }
                }

                this.owner.ResumeStripNotifications(true, true);
            }

            public override void Remove(Control value)
            {
                int index = this.IndexOf(value);
                if (index == -1)
                {
                    return;
                }

                this.owner.SuspendStripNotifications(true, true);

                TabPanel panel = (TabPanel)value;
                int selectedIndex = this.owner.SelectedIndex;

                base.Remove(value);
                this.owner.RemoveTabElement((TabPanel)value);

                if (this.owner.IsHandleCreated)
                {
                    if (this.Count > 0)
                    {
                        if (index > this.Count - 1)
                        {
                            index = this.Count - 1;
                        }
                        this.owner.selectedIndex = index;
                    }
                    else
                    {
                        this.owner.selectedIndex = -1;
                    }
                    this.owner.UpdateTabSelection(true);
                }

                this.owner.ResumeStripNotifications(true, true);
                this.owner.UpdateAfterControlRemoved(value);
            }
        }
    }
}
