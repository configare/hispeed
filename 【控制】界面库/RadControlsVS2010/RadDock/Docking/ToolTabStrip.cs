using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.TabStrip;
using System.Diagnostics;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI.Localization;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A predefined <see cref="DockTabStrip">DockTabStrip</see> instance that is used to store ToolWindow instances.
    /// </summary>
    [ToolboxItem(false)]
    public class ToolTabStrip : DockTabStrip
    {
        #region Fields

        /// <summary>
        /// The root element which describes the layout of this instance.
        /// </summary>
        protected DockLayoutPanel window;

        private ToolWindowCaptionElement caption;
        private DockLayoutPanel captionLayout;
        private FillPrimitive captionFill;
        private BorderPrimitive captionBorder;
        private TextPrimitive captionText;
        private RadButtonElement closeButton;
        private RadToggleButtonElement autoHideButton;
        private OverflowDropDownButtonElement actionMenuButton;
        private bool captionVisible;
        //an internal flag with higher priority, used for example by a floating window to prevent the strip from having caption
        private bool canDisplayCaption;

        private AutoHidePosition autoHidePosition = AutoHidePosition.Auto;
        private AutoHidePosition currentAutoHidePosition;

        #endregion

        #region Initialization & Dispose

        static ToolTabStrip()
        {
            //ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Docking.Resources.ControlDefault.ToolTabStrip.xml");
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_Docking_ToolTabStrip().DeserializeTheme();
        }

        /// <summary>
        /// Initializes a default instance of the <see cref="ToolTabStrip">ToolTabStrip</see> class.
        /// </summary>
        public ToolTabStrip()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a default instance of the <see cref="ToolTabStrip">ToolTabStrip</see> class and associates it with the provided RadDock instance.
        /// </summary>
        /// <param name="dockManager"></param>
        public ToolTabStrip(RadDock dockManager)
            : base(dockManager)
        {
            this.captionVisible = true;
            this.canDisplayCaption = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.CreateCaption();
            this.window = new DockLayoutPanel();
            this.window.Margin = this.GetDefaultWindowMargin();
            this.window.FitToSizeMode = RadFitToSizeMode.FitToParentBounds;
            this.splitPanelElement.Children.Add(this.window);
            DockLayoutPanel.SetDock(this.caption, Telerik.WinControls.Layouts.Dock.Top);
            this.window.Children.Add(this.caption);
            this.window.Children.Add(this.tabStripElement);

            this.tabStripElement.ItemFitMode = StripViewItemFitMode.Shrink;

            this.SetToolTips();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.caption.RadPropertyChanged -= new RadPropertyChangedEventHandler(caption_RadPropertyChanged);
                this.closeButton.Click -= new EventHandler(captionCloseButton_Click);
                this.autoHideButton.Click -= new EventHandler(captionAutoHideButton_Click);
                this.actionMenuButton.MouseDown -= new MouseEventHandler(captionSystemDownButton_MouseDown);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element == this.autoHideButton ||
                element == this.actionMenuButton ||
                element == this.closeButton)
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        private void CreateCaption()
        {
            this.caption = new ToolWindowCaptionElement();
            this.caption.Class = "ToolWindowCaption";
            this.caption.RadPropertyChanged += new RadPropertyChangedEventHandler(caption_RadPropertyChanged);

            this.captionFill = new FillPrimitive();
            this.captionFill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.captionFill.Class = "TitleFill";
            this.caption.Children.Add(this.captionFill);

            this.captionBorder = new BorderPrimitive();
            this.captionBorder.Class = "TitleBorder";
            this.caption.Children.Add(this.captionBorder);

            this.captionLayout = new DockLayoutPanel();
            this.captionLayout.Class = "CaptionButtonsLayout";
            this.captionLayout.Margin = new Padding(0, 2, 0, 2);
            this.caption.Children.Add(this.captionLayout);

            this.closeButton = new RadButtonElement();
            this.closeButton.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(1, 0, 1, 0));
            this.closeButton.Class = "CloseButton";
            this.closeButton.ButtonFillElement.Class = "CloseButtonFill";
            this.closeButton.MinSize = new Size(7, 7);
            this.closeButton.ClickMode = ClickMode.Release;
            this.closeButton.Click += new EventHandler(captionCloseButton_Click);
            DockLayoutPanel.SetDock(this.closeButton, Telerik.WinControls.Layouts.Dock.Right);
            this.captionLayout.Children.Add(closeButton);

            this.autoHideButton = new RadToggleButtonElement();
            this.autoHideButton.Class = "AutoHideButton";
            this.autoHideButton.ButtonFillElement.Class = "AutoHideButtonFill";
            this.autoHideButton.MinSize = new Size(7, 7);
            this.autoHideButton.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(1, 0, 1, 0));
            this.autoHideButton.ClickMode = ClickMode.Release;
            this.autoHideButton.ToggleState = ToggleState.On;
            this.autoHideButton.Click += new EventHandler(captionAutoHideButton_Click);
            DockLayoutPanel.SetDock(this.autoHideButton, Telerik.WinControls.Layouts.Dock.Right);
            this.captionLayout.Children.Add(autoHideButton);

            this.actionMenuButton = new OverflowDropDownButtonElement();
            this.actionMenuButton.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(1, 0, 1, 0));
            this.actionMenuButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.actionMenuButton.ShowArrow = false;
            this.actionMenuButton.Class = "SystemDropDownButton";
            this.actionMenuButton.ActionButton.ButtonFillElement.Class = "SystemDropDownButtonFill";
            this.actionMenuButton.ActionButton.Class = "SystemDropDownActionButton";
            this.actionMenuButton.MinSize = new Size(7, 7);
            this.actionMenuButton.DisplayStyle = DisplayStyle.Image;
            this.actionMenuButton.Text = "";
            this.actionMenuButton.BorderElement.Visibility = ElementVisibility.Collapsed;
            this.actionMenuButton.MouseDown += new MouseEventHandler(captionSystemDownButton_MouseDown);
            DockLayoutPanel.SetDock(this.actionMenuButton, Telerik.WinControls.Layouts.Dock.Right);
            this.captionLayout.Children.Add(this.actionMenuButton);

            this.captionText = new TextPrimitive();
            this.captionText.Text = "ToolTabStrip";
            this.captionText.Class = "TitleCaption";
            this.captionText.StretchHorizontally = true;
            this.captionText.StretchVertically = true;
            this.captionText.TextAlignment = ContentAlignment.MiddleLeft;
            DockLayoutPanel.SetDock(this.captionText, Telerik.WinControls.Layouts.Dock.Left);
            this.captionLayout.Children.Add(captionText);
        }

        #endregion

        #region Event Handlers

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
            bool show = false;
            if (element != null)
            {
                show = (element is TabStripItem) || (element is SplitPanelElement);
            }
            else
            {
                show = (this.CaptionElement.ControlBoundingRectangle.Contains(e.Location));
            }

            if (!show)
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
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (!this.ShouldHandleDoubleClick())
            {
                return;
            }

            this.Capture = false;
            this.DockManager.OnToolTabStripDoubleClick(this, e.Location);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            DockLayoutPanel.SetDock(this.captionText,
                (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) ?
                Telerik.WinControls.Layouts.Dock.Right :
                Telerik.WinControls.Layouts.Dock.Left);

            DockLayoutPanel.SetDock(this.actionMenuButton,
                (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) ?
                Telerik.WinControls.Layouts.Dock.Left :
                Telerik.WinControls.Layouts.Dock.Right);

            DockLayoutPanel.SetDock(this.autoHideButton,
                (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) ?
                Telerik.WinControls.Layouts.Dock.Left :
                Telerik.WinControls.Layouts.Dock.Right);

            DockLayoutPanel.SetDock(this.closeButton,
                (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) ?
                Telerik.WinControls.Layouts.Dock.Left :
                Telerik.WinControls.Layouts.Dock.Right);

            this.captionText.TextAlignment = (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
        }

        protected override void OnTabStripAlignmentChanged(EventArgs e)
        {
            base.OnTabStripAlignmentChanged(e);

            this.window.SetDefaultValueOverride(RadElement.MarginProperty, this.GetDefaultWindowMargin());
        }

        protected internal override void OnLocalizationProviderChanged()
        {
            base.OnLocalizationProviderChanged();

            this.SetToolTips();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            this.UpdateCaptionText();
            if (this.autoHideButton != null)
            {
                this.autoHideButton.ToggleState = this.GetAutoHideButtonChecked() ? ToggleState.On : ToggleState.Off;
            }

            base.OnLayout(e);
        }

        private void captionSystemDownButton_MouseDown(object sender, MouseEventArgs e)
        {
            DockWindow activeWindow = this.ActiveWindow;
            if (activeWindow == null)
            {
                return;
            }

            RadDock dock = this.DockManager;
            if (dock == null)
            {
                return;
            }

            ContextMenuService service = dock.GetService<ContextMenuService>(ServiceConstants.ContextMenu);
            if (service != null)
            {
                service.DisplayContextMenu(activeWindow, Control.MousePosition);
            }
        }

        private void captionAutoHideButton_Click(object sender, EventArgs e)
        {
            Debug.Assert(this.ActiveWindow != null, "There should be a valid ActiveWindow at this point.");
            Debug.Assert(this.DockManager != null, "There should be a valid DockManager at this point.");

            this.DockManager.OnAutoHideButtonClicked(this);
        }

        private void captionCloseButton_Click(object sender, EventArgs e)
        {
            if (this.ActiveWindow != null)
            {
                this.ActiveWindow.Close();
            }
        }

        private void caption_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadElement.BoundsProperty && this.ActiveWindow != null)
            {
                this.ActiveWindow.Bounds = this.TabPanelBounds;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override string ThemeClassName
        {
            get
            {
                return typeof(ToolTabStrip).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        internal string CaptionText
        {
            get
            {
                if (this.captionText == null)
                {
                    return null;
                }

                return this.captionText.Text;
            }
            set
            {
                if (this.captionText != null)
                {
                    this.captionText.Text = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Rectangle TabPanelBounds
        {
            get
            {
                Rectangle bounds = base.TabPanelBounds;
                if (this.CaptionElement.Visibility == ElementVisibility.Visible)
                {
                    int captionHeight = this.CaptionElement.Size.Height;
                    bounds.Y += captionHeight;
                    bounds.Height -= captionHeight;
                }

                return bounds;
            }
        }

        /// <summary>
        /// Gets the ToolWindowCaptionElement that appears as titlebar of ToolTabStrip
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ToolWindowCaptionElement CaptionElement
        {
            get
            {
                return this.caption;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="DockLayoutPanel"/>class
        /// that represents the layout panel that holds the caption.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DockLayoutPanel CaptionLayoutPanel
        {
            get
            {
                return this.window;
            }
        }

        /// <summary>
        /// Gets the close RadButtonElement that appears as titlebar of ToolTabStrip
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RadButtonElement CloseButton
        {
            get { return this.closeButton; }
        }

        /// <summary>
        /// Gets the system menu RadDropDownButtonElement that appears as titlebar of ToolTabStrip
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RadDropDownButtonElement ActionMenuButton
        {
            get { return this.actionMenuButton; }
        }

        /// <summary>
        /// Gets the auto-hide RadButtonElement that appears as titlebar of ToolTabStrip
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RadToggleButtonElement AutoHideButton
        {
            get { return this.autoHideButton; }
        }

        /// <summary>
        /// Gets or sets the auto-hide position for the strip.
        /// </summary>
        [Description("Gets or sets the auto-hide position for the strip.")]
        [DefaultValue(AutoHidePosition.Auto)]
        public AutoHidePosition AutoHidePosition
        {
            get { return autoHidePosition; }
            set
            {
                autoHidePosition = value;
            }
        }

        internal AutoHidePosition CurrentAutoHidePosition
        {
            get
            {
                if (AutoHidePosition == AutoHidePosition.Auto)
                {
                    return currentAutoHidePosition;
                }

                return this.AutoHidePosition;
            }
            set { currentAutoHidePosition = value; }
        }

        internal bool CanDisplayCaption
        {
            get
            {
                return this.canDisplayCaption;
            }
            set
            {
                this.canDisplayCaption = value;
                this.UpdateCaptionVisibility(true);
            }
        }

        /// <summary>
        /// Returns <see cref="Telerik.WinControls.UI.Docking.DockType.ToolWindow">ToolWindow</see> dock type.
        /// </summary>
        public override DockType DockType
        {
            get
            {
                return DockType.ToolWindow;
            }
        }

        /// <summary>
        /// Determines whether the Caption element of the strip is visible.
        /// </summary>
        [Description("Determines whether the Caption element of the strip is visible.")]
        public bool CaptionVisible
        {
            get
            {
                if (!this.canDisplayCaption)
                {
                    return false;
                }

                return this.captionVisible;
            }
            set
            {
                if (this.captionVisible == value)
                {
                    return;
                }

                this.captionVisible = value;
                this.UpdateCaptionVisibility(true);
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

            this.UpdateCaptionText();
            RadDock dock = this.DockManager;
            if (dock == null)
            {
                return;
            }

            DockWindow window = this.ActiveWindow;
            if (window == null)
            {
                return;
            }

            FloatingWindow floatingWindow = window.FloatingParent;
            if (floatingWindow != null)
            {
                floatingWindow.OnSelectedTabChanged(this);
            }
            else
            {
                this.UpdateCaptionVisibility(true);
            }
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

            ToolStripCaptionButtons buttons = active.ToolCaptionButtons;

            //update close button visibility
            if ((buttons & ToolStripCaptionButtons.Close) == 0)
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

            //update auto-hide button visibility
            if ((buttons & ToolStripCaptionButtons.AutoHide) == 0)
            {
                this.autoHideButton.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                if (this.DockManager.CanChangeWindowState(active, DockState.AutoHide, false))
                {
                    this.autoHideButton.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.autoHideButton.Visibility = ElementVisibility.Collapsed;
                }
            }

            //update system menu visiblity
            if ((buttons & ToolStripCaptionButtons.SystemMenu) == 0)
            {
                this.actionMenuButton.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                this.actionMenuButton.Visibility = ElementVisibility.Visible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="active"></param>
        protected internal override void UpdateActiveWindow(DockWindow window, bool active)
        {
            base.UpdateActiveWindow(window, active);

            if (this.caption != null)
            {
                caption.SetValue(ToolWindowCaptionElement.IsActiveProperty, active);
            }
        }

        internal override void CopyTo(DockTabStrip clone)
        {
            base.CopyTo(clone);

            ToolTabStrip toolClone = clone as ToolTabStrip;
            if (toolClone != null)
            {
                toolClone.autoHidePosition = this.autoHidePosition;
            }
        }

        #endregion

        /// <summary>
        /// Determines whether a mouse double-click event should be handled.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldHandleDoubleClick()
        {
            return this.DockManager != null && this.ActiveWindow != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected override bool IsDragAllowed(System.Drawing.Point location)
        {
            return this.captionText.ControlBoundingRectangle.Contains(location);
        }

        /// <summary>
        /// Determines whether the tabstrip element is visible.
        /// </summary>
        /// <returns></returns>
        protected override bool GetTabStripVisible()
        {
            if (!base.GetTabStripVisible())
            {
                return false;
            }

            return this.TabPanels.Count > 1;
        }

        /// <summary>
        /// Determines whether the auto-hide button is checked.
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetAutoHideButtonChecked()
        {
            return true;
        }

        /// <summary>
        /// Updates the caption text, depending on the currently selected dock window.
        /// </summary>
        protected void UpdateCaptionText()
        {
            TabPanel selected = this.SelectedTab;
            if (selected != null && this.captionText != null)
            {
                ToolWindow window = selected as ToolWindow;
                if (window != null && !string.IsNullOrEmpty(window.Caption))
                {
                    this.captionText.Text = window.Caption;
                }
                else
                {
                    this.captionText.Text = selected.Text;
                }
            }
        }

        private void UpdateCaptionVisibility(bool updateActiveWindowBounds)
        {
            if (this.caption == null)
            {
                return;
            }

            if (!this.canDisplayCaption)
            {
                this.caption.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                if (this.captionVisible)
                {
                    this.caption.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.caption.Visibility = ElementVisibility.Collapsed;
                }
            }

            if (updateActiveWindowBounds && this.ActiveWindow != null)
            {
                this.ActiveWindow.Bounds = this.TabPanelBounds;
            }
        }

        private void SetToolTips()
        {
            RadDockLocalizationProvider provider = RadDockLocalizationProvider.CurrentProvider;

            this.closeButton.ToolTipText = provider.GetLocalizedString(RadDockStringId.ToolTabStripCloseButton);
            this.autoHideButton.ToolTipText = provider.GetLocalizedString(RadDockStringId.ToolTabStripUnpinButton);
            this.actionMenuButton.ActionButton.ToolTipText = provider.GetLocalizedString(RadDockStringId.ToolTabStripDockStateButton);
        }

        private Padding GetDefaultWindowMargin()
        {
            Padding defaultMargin = Padding.Empty;

            switch (this.tabStripElement.StripAlignment)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Left:
                    defaultMargin = new Padding(1, 2, 1, 1);
                    break;
                case StripViewAlignment.Bottom:
                case StripViewAlignment.Right:
                    defaultMargin = new Padding(1, 1, 1, 2);
                    break;
            }

            return defaultMargin;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeCaptionVisible()
        {
            return !this.captionVisible;
        }

        #endregion
    }
}
