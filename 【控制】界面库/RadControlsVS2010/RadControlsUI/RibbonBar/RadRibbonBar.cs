using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{

    /// <summary>
    /// 	<para>other Telerik RadControls and Windows Represents a RadRibbonBar. The
    ///     RadRibbon bar visual appearance can be customized in numerous ways through themes.
    ///     Also you can nest other telerik controls in the ribbon bar chunks thus creating
    ///     intuitive interface for your applications. All of the application's functionality
    ///     is accessible from a single ribbon. The ribbon is divided into command tabs such as
    ///     Write, Insert, and Page Layout. When the users clicks on a command tab, they see
    ///     chunks such as Clipboard, Font, and Paragraph. Each chunk can hold an unlimited
    ///     number of controls including toolbars, comboboxes, and Forms controls.</para>
    /// 	<para>
    ///         The RadRibbonBar class is a simple wrapper for the
    ///         <see cref="RadRibbonBarElement">RadRibbonBarElement</see> class. All UI and
    ///         logic functionality is implemented in
    ///         <see cref="RadRibbonBarElement">RadRibbonBarElement</see> class. RadRibbonBar
    ///         acts to transfer the events to and from its
    ///         <see cref="RadRibbonBarElement">RadRibbonBarElement</see> class.
    ///     </para>
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.MenuAndToolbarsGroup)]
    [Designer(DesignerConsts.RadRibbonBarDesignerString)]
    [Description("Provides an implementation of the Microsoft Office 2007 ribbon-user interface")]
    [Docking(DockingBehavior.Ask)]
    [ToolboxItem(true)]
    public class RadRibbonBar : RadControl, ITooltipOwner
    {
        private RadRibbonBarElement ribbonBarElement;

        /// <commentsfrom cref="RadRibbonBarElement.CommandTabSelected" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("CommandTabSelected", typeof(RadRibbonBarElement))]
        public event CommandTabEventHandler CommandTabSelected;

        /// <commentsfrom cref="RadRibbonBarElement.CommandTabExpanded" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("CommandTabExpanded", typeof(RadRibbonBarElement))]
        [Obsolete("Please, use the ExpandedStateChanged instead. This event will be removed for Q1 2010.")]
        public event CommandTabEventHandler CommandTabExpanded;

        /// <commentsfrom cref="RadRibbonBarElement.CommandTabCollapsed" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("CommandTabCollapsed", typeof(RadRibbonBarElement))]
        [Obsolete("Please, use the ExpandedStateChanged instead. This event will be removed for Q1 2010.")]
        public event CommandTabEventHandler CommandTabCollapsed;

        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("ExpandedStateChanged", typeof(RadRibbonBarElement))]
        public event EventHandler ExpandedStateChanged;


        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when the RadRibbonBar is painting Key tips")]
        public event CancelEventHandler KeyTipShowing;

        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when the user is press Key tip")]
        public event CancelEventHandler KeyTipActivating;

        public readonly static ExpandCollapseCommand ExpandCollapseCommand;

        /// <summary>Initializes a new instance of the RadRibbonBar control class.</summary>
        static RadRibbonBar()
        {
            ExpandCollapseCommand = new ExpandCollapseCommand();
            ExpandCollapseCommand.Name = "ExpandCollapseCommand";
            ExpandCollapseCommand.Text = "This command expands/collapses the currently selected RibbonBar command tab.";
            ExpandCollapseCommand.OwnerType = typeof(RadRibbonBar);
        }

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchVertically = false;
        }

        private bool compositionEnabled;
        private bool enableTabScrollingOnMouseWheel = false;

        public RadRibbonBar()
        {
            this.AutoSize = true;
            this.UseNewLayoutSystem = true;
            this.CausesValidation = false;
            this.ThemeNameChanged += new ThemeNameChangedEventHandler(RadRibbonBar_ThemeNameChanged);
        }

        internal bool CompositionEnabled
        {
            get
            {
                return compositionEnabled;
            }
            set
            {
                this.compositionEnabled = value;
            }
        }

        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(ApplicationMenuStyle.ApplicationMenu)]
        public ApplicationMenuStyle ApplicationMenuStyle
        {
            get
            {
                return this.ribbonBarElement.ApplicationMenuStyle;
            }
            set
            {
                this.ribbonBarElement.ApplicationMenuStyle = value;
            }
        }

        [Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public RadRibbonBarBackstageView BackstageControl
        {
            get
            {
                return this.ribbonBarElement.BackstageControl;
            }
            set
            {
                this.ribbonBarElement.BackstageControl = value;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.CommandTabs" filter=""/>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RadDescription("CommandTabs", typeof(RadRibbonBarElement))]
        [RadEditItemsAction]
        [RadNewItem("Add New Tab...", true, true, false)]
        [Editor(DesignerConsts.CommandTabsCollectionEditorString, typeof(UITypeEditor))]
        public RadRibbonBarCommandTabCollection CommandTabs
        {
            get
            {
                return this.ribbonBarElement.CommandTabs;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.ContextualTabGroups" filter=""/>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RadDescription("ContextualTabGroups", typeof(RadRibbonBarElement))]
        [RadEditItemsAction]
        [RadNewItem("Add Context...", true, true, false)]
        public RadItemOwnerCollection ContextualTabGroups
        {
            get
            {
                return this.ribbonBarElement.ContextualTabGroups;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.QuickAccessMenuHeight" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("QuickAccessMenuHeight", typeof(RadRibbonBarElement))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(30)]
        public int QuickAccessToolBarHeight
        {
            get
            {
                return this.ribbonBarElement.QuickAccessMenuHeight;
            }
            set
            {
                this.ribbonBarElement.QuickAccessMenuHeight = value;
            }
        }

        /// <summary>
        /// Represent the Ribbon Help button
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Represent the Ribbon Help button!")]
        public RadImageButtonElement HelpButton
        {
            get
            {
                return this.RibbonBarElement.HelpButton;
            }
            set
            {
                this.RibbonBarElement.HelpButton = value;
            }
        }

        /// <summary>
        /// Represent the Ribbon Expand button
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Represent the Ribbon Expand button!")]
        public RadToggleButtonElement ExpandButton
        {
            get
            {
                return this.RibbonBarElement.ExpandButton;
            }
            set
            {
                this.RibbonBarElement.ExpandButton = value;
            }
        }

        /// <summary>
        /// Get or sets value indicating whether RibbonBar Help button is visible or hidden. 
        /// </summary>
        [DefaultValue(false)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Get or sets value indicating whether RibbonBar Help button is visible or hidden.")]
        public bool ShowHelpButton
        {
            get
            {
                return this.RibbonBarElement.ShowHelpButton;
            }
            set
            {
                this.RibbonBarElement.ShowHelpButton = value;
            }
        }

        /// <summary>
        /// Get or sets value indicating whether RibbonBar Help button is visible or hidden. 
        /// </summary>
        [DefaultValue(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Get or sets value indicating whether RibbonBar Help button is visible or hidden.")]
        public bool ShowExpandButton
        {
            get
            {
                return this.RibbonBarElement.ShowExpandButton;
            }
            set
            {
                this.RibbonBarElement.ShowExpandButton = value;
            }
        }

        /// <summary>
        /// Gets or sets whether Key Map (Office 2007 like accelerator keys map)
        /// is used for this speciffic control. Currently this option is implemented for 
        /// the RadRibbonBar control only.
        /// </summary>
        [Browsable(true), DefaultValue(true)]
        public bool EnableKeyMap
        {
            get
            {
                return base.EnableKeyMap;
            }
            set
            {
                base.EnableKeyMap = value;
            }
        }

        ///// <summary>
        ///// Gets or sets a value indicating the type of the fade animation.
        ///// </summary>
        //[DefaultValue(FadeAnimationType.FadeOut)]
        //[Category(RadDesignCategory.AppearanceCategory)]
        //[Description("Gets or sets a value indicating the type of the fade animation.")]
        //private FadeAnimationType ApplicationMenuAnimantionType
        //{
        //    get
        //    {
        //        return this.ribbonBarElement.ApplicationButtonElement.DropDownMenu.FadeAnimationType;
        //    }
        //    set
        //    {
        //        this.ribbonBarElement.ApplicationButtonElement.DropDownMenu.FadeAnimationType = value;
        //    }
        //}
        /// <summary>
        /// Gets or sets a value indicating the type of the fade animation.
        /// </summary>
        [DefaultValue(FadeAnimationType.FadeOut)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets a value indicating the type of the fade animation.")]
        public FadeAnimationType ApplicationMenuAnimantionType
        {
            get
            {
                return this.ribbonBarElement.ApplicationButtonElement.DropDownMenu.FadeAnimationType;
            }
            set
            {
                this.ribbonBarElement.ApplicationButtonElement.DropDownMenu.FadeAnimationType = value;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.QuickAccessToolbarBelowRibbon" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("QuickAccessToolbarBelowRibbon", typeof(RadRibbonBarElement))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool QuickAccessToolbarBelowRibbon
        {
            get
            {
                return this.ribbonBarElement.QuickAccessToolbarBelowRibbon;
            }
            set
            {
                this.ribbonBarElement.QuickAccessToolbarBelowRibbon = value;
            }
        }

        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Enable or Disable Tab Changing on Mouse Wheel")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool EnableTabScrollingOnMouseWheel
        {
            get
            {
                return this.enableTabScrollingOnMouseWheel;
            }
            set
            {
                this.enableTabScrollingOnMouseWheel = value;
            }
        }


        /// <commentsfrom cref="RadRibbonBarElement.QuickAccessMenuItems" filter=""/>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [RadEditItemsAction]

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RadDescription("QuickAccessMenuItems", typeof(RadRibbonBarElement))]
        [RadNewItem("Type here", true, false)]
        public RadItemOwnerCollection QuickAccessToolBarItems
        {
            get
            {
                return this.ribbonBarElement.QuickAccessMenuItems;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.StartButtonImage" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory), Localizable(true)]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [RadDescription("StartButtonImage", typeof(RadRibbonBarElement))]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image StartButtonImage
        {
            get
            {
                return this.ribbonBarElement.StartButtonImage;
            }
            set
            {
                this.ribbonBarElement.StartButtonImage = value;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.StartMenuItems" filter=""/>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [RadEditItemsAction]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RadDescription("StartMenuItems", typeof(RadRibbonBarElement))]
        public RadItemOwnerCollection StartMenuItems
        {
            get
            {
                return this.ribbonBarElement.StartMenuItems;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.StartMenuRightColumnItems" filter=""/>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [RadEditItemsAction]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RadDescription("StartMenuRightColumnItems", typeof(RadRibbonBarElement))]
        public RadItemOwnerCollection StartMenuRightColumnItems
        {
            get
            {
                return this.ribbonBarElement.StartMenuRightColumnItems;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.StartMenuBottomStrip" filter=""/>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [RadEditItemsAction]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RadDescription("StartMenuBottomStrip", typeof(RadRibbonBarElement))]
        public RadItemOwnerCollection StartMenuBottomStrip
        {
            get
            {
                return this.ribbonBarElement.StartMenuBottomStrip;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.StartMenuWidth" filter=""/>
        [Browsable(true), Category(RadDesignCategory.LayoutCategory)]
        [RadDescription("StartMenuWidth", typeof(RadRibbonBarElement))]
        [RadDefaultValue("StartMenuWidth", typeof(RadRibbonBarElement))]
        public int StartMenuWidth
        {
            get
            {
                return this.ribbonBarElement.StartMenuWidth;
            }
            set
            {
                this.ribbonBarElement.StartMenuWidth = value;
            }
        }

        /// <summary>
        /// Gets the options menu button
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.ActionCategory)]
        [Description("Gets the options menu button")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadMenuButtonItem OptionsButton
        {
            get
            {
                return this.ribbonBarElement.OptionsButton;
            }
        }

        /// <summary>
        /// Gets the exit menu button
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.ActionCategory)]
        [Description("Gets the exit menu button")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadMenuButtonItem ExitButton
        {
            get
            {
                return this.ribbonBarElement.ExitButton;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.SelectedCommandTab" filter=""/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonTab SelectedCommandTab
        {
            get
            {
                return (RibbonTab)this.ribbonBarElement.TabStripElement.SelectedItem;
            }
        }

        /// <summary>
        /// Gets the instance of RadRibbonBarElement wrapped by this control. RadRibbonBarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadRibbonBar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRibbonBarElement RibbonBarElement
        {
            get
            {
                return this.ribbonBarElement;
            }
        }

        /// <summary>Gets or sets a value indicating whether the ribbon bar is expanded.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        public bool Expanded
        {
            get
            {
                return this.ribbonBarElement.Expanded;
            }
            set
            {
                this.ribbonBarElement.Expanded = value;
            }
        }


        //FR Ticket#232516
        /// <summary>Gets or sets a value indicating whether the ribbon bar will be collapsed or expanded on ribbon tab double click.</summary>
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the ribbon bar will be collapsed or expanded on ribbon tab double click.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CollapseRibbonOnTabDoubleClick
        {
            get
            {
                return this.RibbonBarElement.CollapseRibbonOnTabDoubleClick;
            }
            set
            {
                this.RibbonBarElement.CollapseRibbonOnTabDoubleClick = value;
            }
        }


        /// <summary>
        /// Gets or sets the small image list
        /// </summary>
        [Browsable(true)]
        public override ImageList SmallImageList
        {
            get
            {
                return base.SmallImageList;
            }
            set
            {
                base.SmallImageList = value;
            }
        }

        /// <summary>
        /// Gets or sets the text of the control
        /// </summary>
        [Localizable(true)]
        public override string Text
        {
            get
            {
                return this.ribbonBarElement.Text;
            }
            set
            {
                Form parentForm = this.Parent as Form;

                if (parentForm != null)
                {
                    parentForm.Text = value;
                }

                this.ribbonBarElement.Text = value;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!this.enableTabScrollingOnMouseWheel || !this.Focused)
            {
                return;
            }

            if (this.ribbonBarElement.TabStripElement.Items.Count == 0)
            {
                return;
            }

            if (this.Expanded || this.ribbonBarElement.Popup.IsPopupShown)
            {
                int tabCount = this.ribbonBarElement.TabStripElement.Items.Count;
                int currentIndex =
                    this.ribbonBarElement.TabStripElement.Items.IndexOf(this.ribbonBarElement.TabStripElement.SelectedItem as RadPageViewItem);
                int scrollToIndex = (tabCount + (Math.Sign(e.Delta) + currentIndex)) % tabCount;
                RibbonTab tab = this.ribbonBarElement.TabStripElement.Items[scrollToIndex] as RibbonTab;

                if (tab.Visibility == ElementVisibility.Visible)
                {
                    this.ribbonBarElement.TabStripElement.SelectedItem = tab;
                }
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            Form parentForm = this.Parent as Form;
            if (parentForm != null)
            {
                parentForm.TextChanged -= new EventHandler(ParentForm_TextChanged);
            }

            base.OnParentChanged(e);

            if (parentForm != null)
            {
                parentForm.TextChanged += new EventHandler(ParentForm_TextChanged);

                this.ribbonBarElement.Text = this.Parent.Text;
                this.ribbonBarElement.SetParentForm();
            }
        }

        private void ParentForm_TextChanged(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                this.ribbonBarElement.Text = this.Parent.Text;
            }
            else
            {
                Form parentForm = this.FindForm();

                if (parentForm != null)
                {
                    this.ribbonBarElement.Text = parentForm.Text;
                }
            }
        }

        /// <summary>
        /// Gets or sets if the ribbon bar has minimize button in its caption
        /// </summary>
        [DefaultValue(true), Category("Behavior")]
        public bool MinimizeButton
        {
            get
            {
                return this.ribbonBarElement.MinimizeButton;
            }
            set
            {
                this.ribbonBarElement.MinimizeButton = value;
            }
        }

        /// <summary>
        /// Gets or sets if the ribbon bar has maximize button in its caption
        /// </summary>
        [DefaultValue(true), Category("Behavior")]
        public bool MaximizeButton
        {
            get
            {
                return this.ribbonBarElement.MaximizeButton;
            }
            set
            {
                this.ribbonBarElement.MaximizeButton = value;
            }
        }

        /// <summary>
        /// Gets or sets if the ribbon bar has close button in its caption
        /// </summary>
        [DefaultValue(true), Category("Behavior")]
        public bool CloseButton
        {
            get
            {
                return this.ribbonBarElement.CloseButton;
            }
            set
            {
                this.ribbonBarElement.CloseButton = value;
            }
        }

        /// <summary>
        /// Gets the localization settings associated with this control
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonBarLocalizationSettings LocalizationSettings
        {
            get
            {
                return this.ribbonBarElement.LocalizationSettings;
            }
        }




        /// <summary>
        /// Gets or sets a flag indicating whether the control causes validation
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        public new bool CausesValidation
        {
            get
            {
                return base.CausesValidation;
            }
            set
            {
                base.CausesValidation = value;
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.OnCommandTabSelected" filter=""/>
        protected virtual void OnCommandTabSelected(CommandTabEventArgs args)
        {
            if (this.CommandTabSelected != null)
            {
                this.CommandTabSelected(this, args);
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.OnCommandTabExpanded" filter=""/>
        protected virtual void OnCommandTabExpanded(CommandTabEventArgs args)
        {
            if (this.CommandTabExpanded != null)
            {
                this.CommandTabExpanded(this, args);
            }
        }

        /// <commentsfrom cref="RadRibbonBarElement.OnCommandTabCollapsed" filter=""/>
        protected virtual void OnCommandTabCollapsed(CommandTabEventArgs args)
        {
            if (this.CommandTabCollapsed != null)
            {
                this.CommandTabCollapsed(this, args);
            }
        }

        protected virtual void OnRibbonBarExpandedStateChanged(EventArgs args)
        {
            if (this.ExpandedStateChanged != null)
            {
                this.ExpandedStateChanged(this, args);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_NCHITTEST)
            {
                if (DWMAPI.IsCompositionEnabled && WmNCHitTest(ref m))
                    return;
            }
            base.WndProc(ref m);
        }

        protected internal bool OnKeyTipItemActivating(RadItem item, CancelEventArgs eventArgs)
        {
            if (this.KeyTipActivating != null)
            {
                this.KeyTipActivating(item, eventArgs);
            }

            return eventArgs.Cancel;
        }

        protected virtual bool WmNCHitTest(ref Message msg)
        {
            RadFormControlBase parentForm = this.FindForm() as RadFormControlBase;

            if (parentForm != null && parentForm.FormBehavior is RadRibbonFormBehavior)
            {
                Point mousePos = new Point((int)msg.LParam);


                RadElement element = this.ElementTree.GetElementAtPoint(this.PointToClient(mousePos));

                //Check whether there are any elements that need to handle to mouse input before
                //redirecting it to the parent form.
                if ((element != null
                    && !(element is RadRibbonBarCaption)
                    && element.Visibility != ElementVisibility.Hidden)
                    || (!this.CompositionEnabled))
                {
                    return false;
                }

                mousePos = this.PointToClient(mousePos);
                int captionAreaHeight = SystemInformation.FrameBorderSize.Height +
                                    SystemInformation.CaptionHeight;
                if (parentForm.WindowState == FormWindowState.Maximized)
                    captionAreaHeight += SystemInformation.FrameBorderSize.Height;

                if (mousePos.Y < captionAreaHeight)
                {
                    msg.Result = new IntPtr(NativeMethods.HTTRANSPARENT);
                    return true;
                }
            }
            return false;
        }

        private void RadRibbonBar_ThemeNameChanged(object source, ThemeNameChangedEventArgs args)
        {
            if (this.ribbonBarElement.Popup != null)
            {
                this.ribbonBarElement.Popup.ThemeName = args.newThemeName;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            this.Behavior.ProccessKeyMap(e.KeyData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.Behavior.IsKeyMapActive)
            {
                this.PaintKeyMap(e.Graphics);
            }
        }

        protected internal virtual void PaintKeyMap(Graphics graphics)
        {
            List<RadItem> keyMap = this.Behavior.GetCurrentKeyMap(this.Behavior.ActiveKeyMapItem);
            int noKeyMap = 1;
            for (int i = 0; i < keyMap.Count; i++)
            {
                RadItem currentKeyMapItem = keyMap[i];

                CancelEventArgs eventArgs = new CancelEventArgs(false);
                if (this.OnKeyTipShowing(currentKeyMapItem, eventArgs))
                {
                    continue;
                }

                if (currentKeyMapItem.Visibility != ElementVisibility.Visible)
                {
                    continue;
                }

                Rectangle bounds = currentKeyMapItem.ControlBoundingRectangle;
                if (currentKeyMapItem.ElementTree == null)
                {
                    continue;
                }

                Control parentControl = currentKeyMapItem.ElementTree.Control;
                Graphics gr = null;
                if (parentControl != this)
                {
                    gr = parentControl.CreateGraphics();
                }

                Point keyTipPaintPoint = Point.Empty;
                int pivotY = bounds.Y + (int)(bounds.Height * 0.66);
                int pivotX = bounds.X + (int)(bounds.Width / 2.0);

                keyTipPaintPoint = new Point(pivotX, pivotY);

                string keyTipString = string.Empty;
                if (!string.IsNullOrEmpty(currentKeyMapItem.KeyTip))
                {
                    keyTipString = currentKeyMapItem.KeyTip;
                }
                else
                {
                    if (noKeyMap < 10)
                    {
                        keyTipString = noKeyMap.ToString();
                    }
                    else
                    {
                        char nextAlpha = (char)((int)'A' + noKeyMap - 10);
                        keyTipString = nextAlpha.ToString();
                    }

                    currentKeyMapItem.KeyTip = keyTipString;
                    noKeyMap++;
                }

                if (gr != null)
                {
                    this.PaintKeyTip(gr, keyTipPaintPoint, keyTipString);
                    gr.Dispose();
                }
                else
                {
                    this.PaintKeyTip(graphics, keyTipPaintPoint, keyTipString);
                }
            }
        }

        protected virtual bool OnKeyTipShowing(RadItem currentKeyMapItem, CancelEventArgs eventArgs)
        {
            if (this.KeyTipShowing != null)
            {
                this.KeyTipShowing(currentKeyMapItem, eventArgs);
            }

            return eventArgs.Cancel;
        }

        protected internal virtual void PaintKeyTip(Graphics graphics, Point pivot, string keyTip)
        {
            int padding = 0;
            using (Font font = new Font("Arial", 10, FontStyle.Regular))
            {
                Size textSize = TextRenderer.MeasureText(keyTip, font);
                Point correctedPivotPoint = new Point((pivot.X - ((textSize.Width / 2) + padding)), pivot.Y);
                Rectangle displayRectangle =
                    new Rectangle(correctedPivotPoint.X,
                    correctedPivotPoint.Y,
                    (textSize.Width + (2 * padding)),
                    (textSize.Height + (2 * padding)));
                graphics.FillRectangle(Brushes.White, displayRectangle);
                graphics.DrawRectangle(Pens.Black, displayRectangle);

                if (this.compositionEnabled)
                {
                    using (GraphicsPath graphicsPath = new GraphicsPath())
                    {
                        SmoothingMode previousSmoothingMode = graphics.SmoothingMode;
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        float sizeInPixels = (float)(font.SizeInPoints / 72.0) * graphics.DpiX;
                        graphicsPath.AddString(keyTip, font.FontFamily, (int)font.Style, sizeInPixels, new Point((correctedPivotPoint.X + padding), (correctedPivotPoint.Y + padding)), StringFormat.GenericDefault);
                        graphics.FillPath(Brushes.Gray, graphicsPath);
                        graphics.SmoothingMode = previousSmoothingMode;
                    }
                }
                else
                {
                    TextRenderer.DrawText(graphics, keyTip, font,
                                   new Point((correctedPivotPoint.X + padding), (correctedPivotPoint.Y + padding)), Color.Gray);
                }
            }
        }

        private RibbonBarInputBehavior ribbonBehavior = null;
        protected override ComponentInputBehavior CreateBehavior()
        {
            this.ribbonBehavior = new RibbonBarInputBehavior(this);
            return this.ribbonBehavior;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.ribbonBarElement = new RadRibbonBarElement();

            this.RootElement.Children.Add(this.ribbonBarElement);

            this.ribbonBarElement.CommandTabSelected +=
                delegate(object sender, CommandTabEventArgs args) { OnCommandTabSelected(args); };
            this.ribbonBarElement.CommandTabExpanded +=
                delegate(object sender, CommandTabEventArgs args) { OnCommandTabExpanded(args); };
            this.ribbonBarElement.CommandTabCollapsed +=
                delegate(object sender, CommandTabEventArgs args) { OnCommandTabCollapsed(args); };

            this.ribbonBarElement.ExpandedStateChanged += delegate(object sender, EventArgs args)
            {
                this.OnRibbonBarExpandedStateChanged(args);
            };

            base.CreateChildItems(ribbonBarElement);
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType == typeof(RadButtonElement) ||
                elementType == typeof(RadRibbonBarButtonGroup) ||
                elementType == typeof(RibbonTabStripElement) ||
                elementType == typeof(RadRibbonBarElement) ||
                element.Class == "ApplicationButton" ||
                elementType == typeof(RadScrollViewer) ||
                elementType == typeof(RadCheckBoxElement) ||
                elementType == typeof(RadToggleButtonElement) ||
                elementType == typeof(RadDropDownButtonElement) ||
                elementType == typeof(RadRepeatButtonElement) ||
                elementType == typeof(RadRibbonBarGroupDropDownButtonElement)
                )
                return true;

            if (elementType.Equals(typeof(RadTextBoxElement)))
            {
                if (element.FindAncestorByThemeEffectiveType(typeof(RadComboBoxElement)) != null)
                {
                    return true;
                }
            }
            else if (elementType.Equals(typeof(RadMaskedEditBoxElement)))
            {
                if (element.FindAncestor<RadDateTimePickerElement>() != null)
                {
                    return true;
                }
            }

            return base.ControlDefinesThemeForElement(element);
        }


        //Nested types
        public class RibbonBarInputBehavior : ComponentInputBehavior
        {
            private RadRibbonBar owner = null;
            public RibbonBarInputBehavior(RadRibbonBar owner)
                : base(owner)
            {
                this.owner = owner;
                this.EnableKeyTips = true;
                this.ShowScreenTipsBellowControl = true;
            }

            protected internal override bool SetInternalKeyMapFocus()
            {
                if (this.owner.ribbonBarElement.TabStripElement.SelectedItem != null)
                {
                    this.owner.ribbonBarElement.TabStripElement.SelectedItem.Focus();
                }
                else if (this.owner.ribbonBarElement.TabStripElement.Items.Count > 0)
                {
                    this.owner.ribbonBarElement.TabStripElement.Items[0].Focus();
                }
                else
                {
                    this.owner.ribbonBarElement.TabStripElement.Focus();
                }

                if (!this.owner.IsDisposed)
                {
                    this.owner.Capture = true;
                }

                return true;
            }

            protected internal override List<RadItem> GetRootItems()
            {
                List<RadItem> rootItems = new List<RadItem>();
                rootItems.Add(this.owner.ribbonBarElement.ApplicationButtonElement);
                foreach (RadItem item in this.owner.QuickAccessToolBarItems)
                {
                    if (item.Enabled)
                    {
                        rootItems.Add(item);
                    }
                }
                foreach (RibbonTab item in this.owner.CommandTabs)
                {
                    if (item.Enabled)
                    {
                        rootItems.Add(item.Tab);//
                    }
                }
                return rootItems;
            }

            protected internal override bool ActivateSelectedItem(RadItem currentKeyMapItem)
            {
                if (currentKeyMapItem == null || currentKeyMapItem.Visibility != ElementVisibility.Visible)
                {
                    return false;
                }

                if (!this.EnableKeyMap)
                {
                    return false;
                }

                CancelEventArgs eventArgs = new CancelEventArgs(false);
                if (this.owner.OnKeyTipItemActivating(currentKeyMapItem, eventArgs))
                {
                    return false;
                }

                if (currentKeyMapItem is RadPageViewItem)
                {
                    foreach (RibbonTab command in this.owner.CommandTabs)
                    {
                        if (command.Tab == currentKeyMapItem)
                        {
                            (currentKeyMapItem as RadPageViewItem).IsSelected = true;
                            break;
                        }
                    }
                    return false;
                }
                else if (currentKeyMapItem == this.owner.RibbonBarElement.ApplicationButtonElement)
                {
                    (this.ActiveKeyMapItem as RadDropDownButtonElement).ShowDropDown();
                    return false;
                }
                else if (currentKeyMapItem is RadQuickAccessToolBar.InnerItem)
                {
                    this.ActiveKeyMapItem.PerformClick();
                    return false;
                }
                else if (currentKeyMapItem is RadRibbonBarGroup)
                {
                    (currentKeyMapItem as RadRibbonBarGroup).DropDownElement.PerformClick();
                    return false;
                }
                else if (currentKeyMapItem is RadDropDownButtonElement)
                {
                    (this.ActiveKeyMapItem as RadDropDownButtonElement).ShowDropDown();
                    return false;
                }
                else if (currentKeyMapItem is RadButtonElement)
                {
                    (currentKeyMapItem as RadButtonElement).PerformClick();
                    return true;
                }
                else if (currentKeyMapItem is RadTextBoxElement)
                {
                    (currentKeyMapItem as RadTextBoxElement).Focus();
                    return false;
                }
                else if (currentKeyMapItem is RadComboBoxElement)
                {
                    (currentKeyMapItem as RadComboBoxElement).TextBoxElement.Focus();
                    return false;
                }
                else if (currentKeyMapItem is RadGalleryElement)
                {
                    (currentKeyMapItem as RadGalleryElement).ShowDropDown();
                    return false;
                }
                this.ActiveKeyMapItem.PerformClick();
                return false;
            }

            protected internal override List<RadItem> GetKeyFocusChildren(RadItem currentKeyMapItem)
            {
                if (currentKeyMapItem == null)
                {
                    return GetRootItems();
                }

                List<RadItem> children = new List<RadItem>();
                if ((currentKeyMapItem is RadPageViewItem) &&
                    (this.owner.SelectedCommandTab.Tab == currentKeyMapItem))
                {
                    for (int i = 0; i < this.owner.SelectedCommandTab.Items.Count; ++i)
                    {
                        RadRibbonBarGroup chunk = this.owner.SelectedCommandTab.Items[i] as RadRibbonBarGroup;
                        if (chunk == null)
                        {
                            continue;
                        }

                        if (chunk.VisibilityState == ChunkVisibilityState.Collapsed)
                        {
                            children.Add(chunk);
                        }
                        else
                        {
                            List<RadItem> subChildren = GetKeyFocusChildren(chunk);
                            children.AddRange(subChildren);
                        }
                    }
                }
                else if (currentKeyMapItem is RadRibbonBarGroup)
                {
                    RadRibbonBarGroup chunk = currentKeyMapItem as RadRibbonBarGroup;
                    if (chunk != null)
                    {
                        if (chunk.VisibilityState == ChunkVisibilityState.Collapsed)
                        {
                            children.AddRange(chunk.DropDownElement.Items);
                        }
                        else
                        {
                            foreach (RadItem item in chunk.Items)
                            {
                                if (!(item is RadRibbonBarButtonGroup))
                                {
                                    if (item.Enabled)
                                    {
                                        children.Add(item);
                                    }
                                }
                                else
                                {
                                    List<RadItem> subChildren = GetKeyFocusChildren(item);
                                    children.AddRange(subChildren);
                                }
                            }
                        }
                    }
                }
                else if (currentKeyMapItem is RadDropDownButtonElement)
                {
                    return children;
                }
                else if (currentKeyMapItem is RadComboBoxElement)
                {
                    children.AddRange((currentKeyMapItem as RadComboBoxElement).Items);
                }
                else if (currentKeyMapItem is RadRibbonBarButtonGroup)
                {
                    foreach (RadItem item in (currentKeyMapItem as RadRibbonBarButtonGroup).Items)
                    {
                        if (!(item is RadRibbonBarButtonGroup) && !(item is RibbonBarGroupSeparator))
                        {
                            children.Add(item);
                        }
                        else
                        {
                            List<RadItem> subChildren = GetKeyFocusChildren(item);
                            children.AddRange(subChildren);
                        }
                    }
                }
                else
                {
                    foreach (RadElement item in currentKeyMapItem.Children)
                    {
                        if (item is RadItem)
                        {
                            if (item.Enabled)
                            {
                                children.Add(item as RadItem);
                            }
                        }
                    }
                }

                return children;
            }

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.ThemeNameChanged -= new ThemeNameChangedEventHandler(RadRibbonBar_ThemeNameChanged);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadRibbonBarAccessibleObject(this);
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }
    }
}
