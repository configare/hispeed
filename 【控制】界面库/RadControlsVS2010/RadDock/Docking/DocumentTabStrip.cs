using System;
using System.Drawing;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using Telerik.WinControls.UI.Localization;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A predefined <see cref="DockTabStrip">DockTabStrip</see> instance that resides within a <see cref="DocumentContainer">DocumentContainer</see> and hosts documents.
    /// For a document is considered a DocumentWindow instance or a ToolWindow, which has a TabbedDocument DockState.
    /// </summary>
    [ToolboxItem(false)]
    public class DocumentTabStrip : DockTabStrip
    {
        #region Fields

        //private RadElement window;
        internal DockLayoutPanel documentButtonsLayout;
        private RadButtonElement closeButton;
        private OverflowDropDownButtonElement overflowMenuButton;
        private TabStripItem boldedItem;

        #endregion

        #region Constructors/Initializers & dispose

        static DocumentTabStrip()
        {
            //ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Docking.Resources.ControlDefault.DocumentTabStrip.xml");
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_Docking_DocumentTabStrip().DeserializeTheme();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DocumentTabStrip()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="DocumentTabStrip">DocumentTabStrip</see> instance and associates it with the provided RadDock instance.
        /// </summary>
        /// <param name="dockManager"></param>
        public DocumentTabStrip(RadDock dockManager)
            : base(dockManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.documentButtonsLayout = new DockLayoutPanel();
            this.documentButtonsLayout.Class = "documentButtonsLayout";
            this.documentButtonsLayout.StretchHorizontally = false;
            this.documentButtonsLayout.StretchVertically = false;
            this.documentButtonsLayout.Alignment = ContentAlignment.TopRight;
            this.documentButtonsLayout.ZIndex = 10;
            this.documentButtonsLayout.Margin = new Padding(0, 2, 0, 0);

            //window.Children.Add(documentButtonsLayout);
            this.splitPanelElement.Children.Add(documentButtonsLayout);
            this.tabStripElement.ContentArea.Visibility = ElementVisibility.Visible;

            this.closeButton = new RadButtonElement();
            this.closeButton.Class = "CloseButton";
            this.closeButton.ButtonFillElement.Class = "CloseButtonFill";
            this.closeButton.SetDefaultValueOverride(RadElement.MarginProperty, new System.Windows.Forms.Padding(0, 0, 2, 0));
            this.closeButton.ClickMode = ClickMode.Release;
            this.closeButton.Click += new EventHandler(closeButton_Click);
            this.closeButton.ZIndex = 5;
            this.closeButton.StretchHorizontally = false;
            this.closeButton.StretchVertically = false;

            DockLayoutPanel.SetDock(closeButton, Telerik.WinControls.Layouts.Dock.Right);
            this.documentButtonsLayout.Children.Add(closeButton);

            this.overflowMenuButton = new OverflowDropDownButtonElement();
            this.overflowMenuButton.SetDefaultValueOverride(RadElement.MarginProperty, new System.Windows.Forms.Padding(0, 0, 2, 0));
            this.overflowMenuButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.overflowMenuButton.ShowArrow = false;
            this.overflowMenuButton.Class = "OverflowDropDownButton";
            this.overflowMenuButton.DisplayStyle = DisplayStyle.Image;
            this.overflowMenuButton.BorderElement.Visibility = ElementVisibility.Collapsed;
            this.overflowMenuButton.ActionButton.Class = "OverflowDropDownActionButton";
            this.overflowMenuButton.ActionButton.ButtonFillElement.Class = "OverflowDropDownButtonFill";
            this.overflowMenuButton.StretchHorizontally = false;
            this.overflowMenuButton.StretchVertically = false;
            this.overflowMenuButton.MouseDown += new System.Windows.Forms.MouseEventHandler(activeDocumentsMenuDropDownButton_MouseDown);
            this.overflowMenuButton.ZIndex = 10;
            DockLayoutPanel.SetDock(overflowMenuButton, Telerik.WinControls.Layouts.Dock.Right);
            this.documentButtonsLayout.Children.Add(this.overflowMenuButton);

            this.tabStripElement.Alignment = ContentAlignment.TopLeft;
            this.tabStripElement.StripAlignment = StripViewAlignment.Top;
            this.tabStripElement.AllowDrag = true;
            this.splitPanelElement.Visibility = ElementVisibility.Hidden;

            this.tabStripElement.ItemContainer.BringToFront();

            this.SetToolTips();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (closeButton != null)
            {
                closeButton.Click -= new EventHandler(closeButton_Click);
            }

            if (overflowMenuButton != null)
            {
                overflowMenuButton.MouseDown -= new System.Windows.Forms.MouseEventHandler(activeDocumentsMenuDropDownButton_MouseDown);
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OverflowDropDownButtonElement OverflowMenuButton
        {
            get { return overflowMenuButton; }
        }

        /// <summary>
        /// Returns <see cref="Telerik.WinControls.UI.Docking.DockType.Document">Document</see> dock type.
        /// </summary>
        public override DockType DockType
        {
            get
            {
                return DockType.Document;
            }
        }

        /// <summary>
        /// The default tabstrip alignment is Top.
        /// </summary>
        protected override TabStripAlignment DefaultTabStripAlignment
        {
            get
            {
                return TabStripAlignment.Top;
            }
        }

        /// <summary>
        /// This predefined instance is never a redock target.
        /// </summary>
        internal override bool IsRedockTarget
        {
            get
            {
                //document strips are not stored as redock target.
                return false;
            }
            set
            {
            }
        }

        #endregion

        #region Event handlers

        protected override void OnSelectedIndexChanging(TabStripPanelSelectedIndexChangingEventArgs e)
        {
            base.OnSelectedIndexChanging(e);
            if (e.NewTabPanel is DockWindow && this.DockManager != null)
            {
                this.DockManager.ActiveWindow = e.NewTabPanel as DockWindow;
                if (this.DockManager.ActiveWindow != e.NewTabPanel)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (this.tabStripElement == null || this.overflowMenuButton == null)
            {
                return;
            }

            this.UpdateOverflowButton();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            RadElement element = this.ElementTree.GetElementAtPoint(e.Location);
            if (!(element is TabStripItem))
            {
                return;
            }

            if (this.DockManager != null && this.ActiveWindow != null)
            {
                ContextMenuService service = this.DockManager.GetService<ContextMenuService>(ServiceConstants.ContextMenu);
                if (service != null)
                {
                    service.DisplayContextMenu(this.ActiveWindow, Control.MousePosition);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            RadElement element = this.ElementTree.GetElementAtPoint(e.Location);
            bool show = false;
            if (element != null)
            {
                show = (element is TabStripItem) || (element is RadTabStripElement);
            }

            DockWindow activeWindow = this.ActiveWindow;
            RadDock docking = this.DockManager;
            if (!show || activeWindow == null || docking == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Middle && (element is TabStripItem))
            {
                docking.CloseWindow(activeWindow);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(System.Windows.Forms.ControlEventArgs e)
        {
            base.OnControlAdded(e);

            UpdateView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlRemoved(System.Windows.Forms.ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            DockWindow window = e.Control as DockWindow;
            if (window.TabStripItem == this.boldedItem)
            {
                this.boldedItem = null;
            }

            UpdateView();
        }

        protected override void OnTabStripAlignmentChanged(EventArgs e)
        {
            switch (this.TabStripAlignment)
            {
                case TabStripAlignment.Top:
                    this.documentButtonsLayout.Alignment = ContentAlignment.TopRight;
                    this.documentButtonsLayout.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, 2, 0, 0));
                    DockLayoutPanel.SetDock(this.closeButton, Telerik.WinControls.Layouts.Dock.Right);
                    DockLayoutPanel.SetDock(this.overflowMenuButton, Telerik.WinControls.Layouts.Dock.Right);
                    break;
                case TabStripAlignment.Bottom:
                    this.documentButtonsLayout.Alignment = ContentAlignment.BottomRight;
                    this.documentButtonsLayout.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, 0, 0, 2));
                    DockLayoutPanel.SetDock(this.closeButton, Telerik.WinControls.Layouts.Dock.Right);
                    DockLayoutPanel.SetDock(this.overflowMenuButton, Telerik.WinControls.Layouts.Dock.Right);
                    break;
                case TabStripAlignment.Left:
                    this.documentButtonsLayout.Alignment = ContentAlignment.BottomLeft;
                    DockLayoutPanel.SetDock(this.closeButton, Telerik.WinControls.Layouts.Dock.Top);
                    DockLayoutPanel.SetDock(this.overflowMenuButton, Telerik.WinControls.Layouts.Dock.Top);
                    break;
                case TabStripAlignment.Right:
                    this.documentButtonsLayout.Alignment = ContentAlignment.BottomRight;
                    DockLayoutPanel.SetDock(this.closeButton, Telerik.WinControls.Layouts.Dock.Top);
                    DockLayoutPanel.SetDock(this.overflowMenuButton, Telerik.WinControls.Layouts.Dock.Top);
                    break;
            }

            this.tabStripElement.ItemContainer.BringToFront();

            base.OnTabStripAlignmentChanged(e);
        }

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();

            TabStripItem currentBoldItem = this.boldedItem;
            if (currentBoldItem != null)
            {
                //re-apply the bold style as it is set locally and the theme will not change it
                this.boldedItem = null;
                currentBoldItem.ResetValue(VisualElement.FontProperty, ValueResetFlags.Local);
                this.BoldSelectedDocument(currentBoldItem);
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            if (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
            {
                DockLayoutPanel.SetDock(closeButton, Telerik.WinControls.Layouts.Dock.Left);
                this.closeButton.SetDefaultValueOverride(RadElement.MarginProperty, new System.Windows.Forms.Padding(2, 0, 0, 0));
                DockLayoutPanel.SetDock(overflowMenuButton, Telerik.WinControls.Layouts.Dock.Left);
                this.overflowMenuButton.SetDefaultValueOverride(RadElement.MarginProperty, new System.Windows.Forms.Padding(2, 0, 0, 0));
            }
            else
            {
                DockLayoutPanel.SetDock(closeButton, Telerik.WinControls.Layouts.Dock.Right);
                this.closeButton.SetDefaultValueOverride(RadElement.MarginProperty, new System.Windows.Forms.Padding(0, 0, 2, 0));
                DockLayoutPanel.SetDock(overflowMenuButton, Telerik.WinControls.Layouts.Dock.Right);
                this.overflowMenuButton.SetDefaultValueOverride(RadElement.MarginProperty, new System.Windows.Forms.Padding(0, 0, 2, 0));
            }
        }

        protected internal override void OnLocalizationProviderChanged()
        {
            base.OnLocalizationProviderChanged();

            this.SetToolTips();
        }

        private void activeDocumentsMenuDropDownButton_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.DockManager == null)
            {
                return;
            }

            ContextMenuService service = this.DockManager.GetService<ContextMenuService>(ServiceConstants.ContextMenu);
            if (service != null)
            {
                service.DisplayActiveWindowList(this, Control.MousePosition);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (this.ActiveWindow != null)
            {
                this.ActiveWindow.Close();
            }
        }
        
        #endregion

        #region Implementation

        #region Internal methods
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateFocus"></param>
        protected internal override void UpdateTabSelection(bool updateFocus)
        {
            base.UpdateTabSelection(updateFocus);

            UpdateView();
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal override void UpdateButtons()
        {
            base.UpdateButtons();

            if (this.DockManager == null)
            {
                return;
            }

            DockWindow active = this.ActiveWindow;
            if (active == null)
            {
                return;
            }

            DocumentStripButtons buttons = active.DocumentButtons;

            //update close button
            if ((buttons & DocumentStripButtons.Close) == 0)
            {
                this.closeButton.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                if (this.DockManager.CanChangeWindowState(active, DockState.Hidden, false))
                {
                    this.closeButton.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.closeButton.Visibility = ElementVisibility.Collapsed;
                }
            }

            //update active window list button
            if ((buttons & DocumentStripButtons.ActiveWindowList) == 0)
            {
                this.overflowMenuButton.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                this.overflowMenuButton.Visibility = ElementVisibility.Visible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal override void UpdateAfterTransaction()
        {
            base.UpdateAfterTransaction();

            if (this.DockManager == null)
            {
                return;
            }

            DockWindow activeDocument = this.DockManager.DocumentManager.ActiveDocument;
            DockWindow active = this.ActiveWindow;
            TabStripItem item = active == activeDocument ? active.TabStripItem : null;
            this.BoldSelectedDocument(item);
        }

        /// <summary>
        /// Updates the currently active window by setting bold if the specified window instance is the currently active document within the owning RadDock instance.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="active"></param>
        protected internal override void UpdateActiveWindow(DockWindow window, bool active)
        {
            base.UpdateActiveWindow(window, active);

            if (window == null || this.DockManager == null || ((IComponentTreeHandler)this.DockManager).Initializing)
            {
                return;
            }

            TabStripItem item = null;
            if (active && window == this.DockManager.DocumentManager.ActiveDocument)
            {
                item = window.TabStripItem;
            }
            this.BoldSelectedDocument(item);
        }
        
        #endregion

        protected override void UpdateTabStripVisibility(bool visible)
        {
            base.UpdateTabStripVisibility(visible);

            if (this.documentButtonsLayout == null)
            {
                return;
            }

            if (visible)
            {
                this.documentButtonsLayout.Visibility = ElementVisibility.Visible;
            }
            else
            {
                this.documentButtonsLayout.Visibility = ElementVisibility.Collapsed;
            }
        }

        private void SetToolTips()
        {
            RadDockLocalizationProvider provider = RadDockLocalizationProvider.CurrentProvider;

            this.closeButton.ToolTipText = provider.GetLocalizedString(RadDockStringId.DocumentTabStripCloseButton);
            this.overflowMenuButton.ActionButton.ToolTipText = provider.GetLocalizedString(RadDockStringId.DocumentTabStripListButton);
        }

        private void UpdateView()
        {
            if (this.HasVisibleTabPanels)
            {
                if (this.splitPanelElement.Visibility != ElementVisibility.Visible)
                {
                    this.splitPanelElement.Visibility = ElementVisibility.Visible;
                }
            }
            else
            {
                if (this.splitPanelElement.Visibility != ElementVisibility.Hidden)
                {
                    this.splitPanelElement.Visibility = ElementVisibility.Hidden;
                }
            }
        }

        private void UpdateOverflowButton()
        {
            bool overflow = false;
            for (int i = 1; i < this.tabStripElement.Items.Count; i++)
            {
                if (this.tabStripElement.Items[i].Visibility != ElementVisibility.Visible)
                {
                    overflow = true;
                    break;
                }
            }

            overflowMenuButton.OverflowMode = overflow;
        }

        private void BoldSelectedDocument(TabStripItem tabItem)
        {
            if (this.boldedItem == tabItem)
            {
                return;
            }

            RadDock dockManager = this.DockManager;
            if (dockManager == null)
            {
                return;
            }

            //remove the Bold Font from the item.
            if (this.boldedItem != null)
            {
                this.boldedItem.ResetValue(VisualElement.FontProperty, ValueResetFlags.Local);
                this.boldedItem = null;
            }

            if (dockManager.DocumentManager.BoldActiveDocument && tabItem != null)
            {
                this.boldedItem = tabItem;
                Font currFont = tabItem.Font;
                if (!currFont.Bold)
                {
                    tabItem.Font = new Font(currFont, currFont.Style | FontStyle.Bold);
                }
            }

            this.UpdateOverflowButton();
            this.UpdateLayout();
        }

        #endregion

        #region Theming

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element == this.closeButton ||
                element == this.overflowMenuButton)
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        #endregion
    }
}
