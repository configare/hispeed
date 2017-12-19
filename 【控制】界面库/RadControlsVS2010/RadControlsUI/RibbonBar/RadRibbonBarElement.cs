using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Collections;
using Telerik.WinControls.Layouts;
using System.Runtime.InteropServices;
using Telerik.WinControls.Design;
using System.IO;
using Telerik.WinControls.Primitives;
using System.Diagnostics;
using Telerik.WinControls.UI.RibbonBar;
using Telerik.WinControls.Styles;
using Telerik.WinControls.Interfaces;

namespace Telerik.WinControls.UI
{

    public enum ApplicationMenuStyle
    {
        ApplicationMenu,
        BackstageView
    }

    /// <summary>
    /// Represents a ribbon bar element. The RadRibbonBarElement can be nested in other 
    /// telerik controls. Essentially RadRibbonBar class is a simple wrapper for 
    /// RadRibbonBarElement class. RadRibbonBar acts to transfer events to and from the its
    /// corresponding instance of the RadRibbonBarElement.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadRibbonBarElement : RadItem, ITooltipOwner
    {
        static RadRibbonBarElement()
        {
            new Themes.ControlDefault.RibbonBar().DeserializeTheme();
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadApplicationMenuDropDown().DeserializeTheme();

            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadRibbonBarElementStateManager(), typeof(RadRibbonBarElement));
        }

        private Control containerControl = null;
        private RibbonBarPopup ribbonPopup;
        private bool expanded = true;
        private RibbonBarLocalizationSettings localizationSettings;
        private bool collapseRibbonOnTabDoubleClick = true;
        private float? highestTabContentPanelHeight = null;
        private RibbonTab removedTab;

        public static RadProperty IsRibbonFormActiveProperty = RadProperty.Register(
            "IsRibbonFormActive",
            typeof(bool),
            typeof(RadRibbonBarElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue)
            );

        public static RadProperty IsBackstageModeProperty = RadProperty.Register(
            "IsBackstageMode",
            typeof(bool),
            typeof(RadRibbonBarElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay)
            );

        public static RadProperty RibbonFormWindowStateProperty = RadProperty.Register(
           "RibbonFormWindowState",
           typeof(FormWindowState),
           typeof(RadRibbonBarElement),
           new RadElementPropertyMetadata(FormWindowState.Normal, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue)
           );

        public static RadProperty QuickAccessMenuHeightProperty = RadProperty.Register(
            "QuickAccessMenuHeight", typeof(int), typeof(RadRibbonBarElement), new RadElementPropertyMetadata(
                30, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty QuickAccessToolbarBelowRibbonProperty = RadProperty.Register(
            "QuickAccessToolbarBelowRibbonProperty", typeof(bool), typeof(RadRibbonBarElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.CanInheritValue));

        /// <summary>
        ///		Occurs when a command tab is selected.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when a command tab is selected.")]
        public event CommandTabEventHandler CommandTabSelected;

        /// <summary>
        ///		Occurs when a command tab is expanded by double clicking a collapsed command tab item.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when a command tab is expanded by double clicking a collapsed command tab item.")]
        public event CommandTabEventHandler CommandTabExpanded;

        /// <summary>
        ///		Occurs when a command tab is collapsed by double clicking an expanded command tab item.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when a command tab is collapsed by double clicking an expanded command tab item.")]
        public event CommandTabEventHandler CommandTabCollapsed;

        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when the RadRibbonBar is either expanded or collapsed. The state of the control can be acquired from the Expanded property")]
        public event EventHandler ExpandedStateChanged;

        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when the RadRibbonBar's  ApplicationMenuStyle is changed")]
        public event EventHandler ApplicationMenuStyleChanged;

        private RibbonTabStripElement tabStrip;
        internal RadApplicationMenuButtonElement dropDownButton;
        private RadMenuButtonItem optionsButton;
        private RadMenuButtonItem exitButton;
        private RadImageButtonElement helpButton;
        private RadToggleButtonElement expandButton;
        private int suspendExpandButtonNotifications = 0;
        private RadRibbonBarBackstageView backstageControl;
        private ApplicationMenuStyle applicationMenuStyle = ApplicationMenuStyle.ApplicationMenu;

        ///////////////////////////
        //MDI support 
        //13.03.07
        //Peter 
        public RadMDIControlsItem MDIbutton;
        private bool collapsingEnabled = true;
        //////////////////////////////////

        public RadRibbonBarBackstageView BackstageControl
        {
            get
            {
                return this.backstageControl;
            }
            set
            {
                this.backstageControl = value;
                this.backstageControl.Owner = this;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the groups are collapsed according to the ribbon's size.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a boolean value determining whether the groups are collapsed according to the ribbon's size.")]
        [DefaultValue(true)]
        public bool CollapsingEnabled
        {
            get
            {
                return this.collapsingEnabled;
            }
            set
            {
                this.collapsingEnabled = value;
                ExpandableStackLayout.SetCollapsingEnabled(this, value);
            }
        }

        /// <summary>
        /// Gets or sets the Minimize button
        /// </summary>
        [DefaultValue(true), Category("Behavior")]
        public bool MinimizeButton
        {
            get
            {
                return this.ribbonCaption.MinimizeButton.Visibility == ElementVisibility.Visible;
            }
            set
            {
                this.ribbonCaption.MinimizeButton.Visibility =
                    value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets the Maximize button
        /// </summary>
        [DefaultValue(true), Category("Behavior")]
        public bool MaximizeButton
        {
            get
            {
                return this.ribbonCaption.MaximizeButton.Visibility == ElementVisibility.Visible;
            }
            set
            {
                this.ribbonCaption.MaximizeButton.Visibility =
                    value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }
        }


        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the ribbon bar will be collapsed or expanded on ribbon tab double click.")]
        public bool CollapseRibbonOnTabDoubleClick
        {
            get
            {
                return this.collapseRibbonOnTabDoubleClick;
            }
            set
            {
                if (this.collapseRibbonOnTabDoubleClick != value)
                {
                    if (!value)
                    {
                        this.Expanded = true;
                    }

                    this.collapseRibbonOnTabDoubleClick = value;
                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("CollapseRibbonOnTabDoubleClick"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the Close button
        /// </summary>
        [DefaultValue(true), Category("Behavior")]
        public bool CloseButton
        {
            get
            {
                return this.ribbonCaption.CloseButton.Visibility == ElementVisibility.Visible;
            }
            set
            {
                this.ribbonCaption.CloseButton.Visibility =
                    value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }
        }

        /// <summary>Gets a collection of the command tabs.</summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection of the command tabs.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RadEditItemsAction]
        [Editor(typeof(CommandTabsEditor), typeof(UITypeEditor))]
        public RadRibbonBarCommandTabCollection CommandTabs
        {
            get
            {
                return (RadRibbonBarCommandTabCollection)this.tabStrip.TabItems;
            }
        }

        /// <summary>
        /// Gets or the localization settings for this element
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonBarLocalizationSettings LocalizationSettings
        {
            get
            {
                if (this.localizationSettings == null)
                    this.localizationSettings = new RibbonBarLocalizationSettings(this);

                return localizationSettings;
            }
        }

        private RadItemOwnerCollection contextualTabGroups;

        /// <summary>
        /// Gets a collection of contextual tab groups.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection of the contextual tab groups.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //[RadEditItemsAction]
        [RadNewItem("Add New Group...", true, true, false)]
        public RadItemOwnerCollection ContextualTabGroups
        {
            get
            {
                return this.contextualTabGroups;
            }
        }

        /// <summary>
        /// Get or sets value indicating whether RibbonBar Help button is visible or hidden. 
        /// </summary>
        [DefaultValue(false)]
        [Category(RadDesignCategory.AppearanceCategory), Description("Get or sets value indicating whether RibbonBar Help button is visible or hidden.")]
        public bool ShowHelpButton
        {
            get
            {
                return this.HelpButton.Visibility == ElementVisibility.Visible;
            }
            set
            {
                this.HelpButton.Visibility = value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }
        }

        /// <summary>
        /// Get or sets value indicating whether RibbonBar Expand button is visible or hidden. 
        /// </summary>
        [DefaultValue(true)]
        [Category(RadDesignCategory.AppearanceCategory), Description("Get or sets value indicating whether RibbonBar Help button is visible or hidden.")]
        public bool ShowExpandButton
        {
            get
            {
                return this.ExpandButton.Visibility == ElementVisibility.Visible;
            }
            set
            {
                this.ExpandButton.Visibility = value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }
        }

        //[RadNewItem("Type here", true)]
        /// <summary>
        /// Gets the collection of quick access menu items.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets the collection of quick access toolbar items.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection QuickAccessMenuItems
        {
            get
            {
                return this.quickAccessToolBar.Items;
            }
        }

        /// <summary>Gets or sets the height of the quick access.</summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the height of the quick access.")]
        [RadPropertyDefaultValue("QuickAccessMenuHeight", typeof(RadRibbonBarElement))]
        public int QuickAccessMenuHeight
        {
            get
            {
                return (int)this.GetValue(QuickAccessMenuHeightProperty);
            }
            set
            {
                this.SetValue(QuickAccessMenuHeightProperty, value);
            }
        }

        /// <summary>Gets or sets if the quick access toolbar is below the ribbon.</summary>
        [Description("Gets or sets if the quick access toolbar is below the ribbon.")]
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadPropertyDefaultValue("QuickAccessToolbarBelowRibbon", typeof(RadRibbonBarElement))]
        public bool QuickAccessToolbarBelowRibbon
        {
            get
            {
                return (bool)this.GetValue(QuickAccessToolbarBelowRibbonProperty);
            }
            set
            {
                this.SetValue(QuickAccessToolbarBelowRibbonProperty, value);
            }
        }

        /// <summary>
        ///		Gets or sets the image of the start button placed in the top left corner.
        /// </summary>
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory), Localizable(true)]
        [Description("Gets or sets the image of the start button placed in the top left corner.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image StartButtonImage
        {
            get
            {
                return this.dropDownButton.Image;
            }
            set
            {
                this.dropDownButton.Image = value;
            }
        }

        /// <summary>
        /// Gets the application menu element
        /// </summary>
        [Browsable(false), Category(RadDesignCategory.ActionCategory)]
        [Description("Gets the application menu element")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadApplicationMenuButtonElement ApplicationButtonElement
        {
            get
            {
                return this.dropDownButton;
            }
        }

        /// <summary>
        /// Gets the options menu button
        /// </summary>
        [Browsable(false), Category(RadDesignCategory.ActionCategory)]
        [Description("Gets the options menu button")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadMenuButtonItem OptionsButton
        {
            get
            {
                return this.optionsButton;
            }
        }

        /// <summary>
        /// Gets the exit menu button
        /// </summary>
        [Browsable(false), Category(RadDesignCategory.ActionCategory)]
        [Description("Gets the exit menu button")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadMenuButtonItem ExitButton
        {
            get
            {
                return this.exitButton;
            }
        }

        //[RadNewItem("Type here", true)]
        /// <summary>
        /// Gets the collection of the start button menu item.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets the collection of the start button menu item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection StartMenuItems
        {
            get
            {
                return this.dropDownButton.Items;
            }
        }

        //[RadNewItem("Type here", true)]
        /// <summary>
        /// Gets the collection of the start button menu items which appear on the right.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets the collection of the start button menu items which appear on the right.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection StartMenuRightColumnItems
        {
            get
            {
                RadApplicationMenuDropDown dropDown = this.dropDownButton.DropDownMenu as RadApplicationMenuDropDown;
                if (dropDown != null)
                {
                    return dropDown.RightColumnItems;
                }
                return null;
            }
        }

        /// <summary>
        ///	Gets the collection of the start button menu DropDown which is displayed when the button has two columns.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets the collection of the start button menu items which appear on the right.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadItemOwnerCollection StartMenuBottomStrip
        {
            get
            {
                RadApplicationMenuDropDown dropDown = this.dropDownButton.DropDownMenu as RadApplicationMenuDropDown;
                if (dropDown != null)
                {
                    return dropDown.ButtonItems;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the width of the start menu
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.LayoutCategory)]
        [DefaultValue(300)]
        [Description("Gets or sets the width of the start menu")]
        public int StartMenuWidth
        {
            get
            {
                RadApplicationMenuDropDown dropDown = this.dropDownButton.DropDownMenu as RadApplicationMenuDropDown;
                if (dropDown != null)
                {
                    return dropDown.RightColumnWidth;
                }
                return 0;
            }
            set
            {
                RadApplicationMenuDropDown dropDown = this.dropDownButton.DropDownMenu as RadApplicationMenuDropDown;
                if (dropDown != null)
                {
                    dropDown.RightColumnWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets the RadItem that holds
        /// the RibbonTabStripElement. For internal use.
        /// </summary>
        public RadItem RibbonTabStripHolder
        {
            get
            {
                return this.tabStripHolder;
            }
        }

        /// <summary>
        ///		Gets an instance of the TabStripElement which is used to display the tab items in the RibbonBarElement.
        /// </summary>
        [Browsable(false)]
        public RibbonTabStripElement TabStripElement
        {
            get
            {
                return this.tabStrip;
            }
        }

        /// <summary>
        ///		Gets the instance of the currently selected command tab.
        /// </summary>
        [Browsable(false)]
        public RibbonTab SelectedCommandTab
        {
            get
            {
                return this.tabStrip.SelectedItem as RibbonTab;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the 
        /// RadRibbonBarElement is expanded or not.
        /// </summary>
        public bool Expanded
        {
            get
            {
                return this.expanded;
            }
            set
            {
                if (this.CommandTabs.Count == 0)
                {
                    throw new InvalidOperationException(
                        "Expand operation could not complete. There are no command tabs or there is not a command tab selected.");
                }
                if (value != this.expanded)
                {
                    this.expanded = value;

                    if (!this.expanded)
                    {
                        this.PrepareRibbonPopup();
                    }
                    else
                    {
                        this.RevertPopupSettings();
                    }

                    this.DoExpandCollapse();

                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("Expanded"));
                    this.OnRibbonBarExpandedStateChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual float GetMaximumRibbonGroupMargin(ExpandableStackLayout groupsHolder)
        {
            int currentMargin = 0;
            foreach (RadRibbonBarGroup group in groupsHolder.Children)
            {
                if (group.Margin.Vertical > currentMargin)
                {
                    currentMargin = group.Margin.Vertical;
                }
            }

            return currentMargin;
        }

        protected virtual float GetMaximumTabContentHeight()
        {
            float result = 90;

            foreach (RibbonTab tab in this.CommandTabs)
            {
                if (tab.ContentLayout != null)
                {
                    SizeF desiredSize = MeasurementControl.ThreadInstance.GetDesiredSize(tab.ContentLayout, new SizeF(float.MaxValue, float.MaxValue));
                    float currentHeight = this.tabStrip.ContentArea.GetBorderThickness(true).Vertical + this.tabStrip.ContentArea.Padding.Vertical + this.GetMaximumRibbonGroupMargin(tab.ContentLayout);
                    currentHeight += desiredSize.Height;
                    if (currentHeight > result)
                    {
                        result = currentHeight;
                    }
                }
            }

            return result;
        }

        protected virtual float GetMaximumTabStripHeight()
        {
            float tabLayoutHeight = this.tabStrip.ItemContainer.DesiredSize.Height + this.tabStrip.ItemContainer.Margin.Vertical;

            if (!this.expanded)
            {
                return tabLayoutHeight;
            }

            if (this.highestTabContentPanelHeight.HasValue)
            {
                float tabStripHeight = this.highestTabContentPanelHeight.Value + tabLayoutHeight;
                if (this.tabStrip.DesiredSize.Height > tabStripHeight)
                {
                    this.highestTabContentPanelHeight = this.tabStrip.DesiredSize.Height - tabLayoutHeight;
                }
                return this.highestTabContentPanelHeight.Value + tabLayoutHeight;
            }

            this.highestTabContentPanelHeight = this.GetMaximumTabContentHeight();

            return highestTabContentPanelHeight.Value + tabLayoutHeight;
        }

        protected virtual void DoExpandCollapse()
        {
            this.suspendExpandButtonNotifications++;

            if (this.expanded)
            {
                this.tabStrip.SelectedItem = this.tabOldSelected;
                (this.tabStrip.SelectedItem as RibbonTab).Items.Owner.Visibility = ElementVisibility.Visible;
                this.expandButton.ToggleState = Enumerations.ToggleState.On;
            }
            else
            {
                (this.tabStrip.SelectedItem as RibbonTab).Items.Owner.Visibility = ElementVisibility.Collapsed;
                this.expandButton.ToggleState = Enumerations.ToggleState.Off;
                this.tabStrip.SelectedItem = null;
            }

            this.suspendExpandButtonNotifications--;
        }

        /// <summary>
        ///		Raises the <see cref="CommandTabSelected"/> event.
        /// </summary>
        /// <param name="args">An <see cref="CommandTabEventArgs"/> that contains the event data.</param>
        protected virtual void OnCommandTabSelected(CommandTabEventArgs args)
        {
            if (this.CommandTabSelected != null)
            {
                this.CommandTabSelected(this, args);
            }
        }

        /// <summary>
        ///		Raises the <see cref="CommandTabExpanded"/> event.
        /// </summary>
        /// <param name="args">An <see cref="CommandTabEventArgs"/> that contains the event data.</param>
        protected virtual void OnCommandTabExpanded(CommandTabEventArgs args)
        {
            if (this.CommandTabExpanded != null)
            {
                this.CommandTabExpanded(this, args);
            }
        }

        /// <summary>
        ///		Raises the <see cref="CommandTabCollapsed"/> event.
        /// </summary>
        /// <param name="args">An <see cref="CommandTabEventArgs"/> that contains the event data.</param>
        protected virtual void OnCommandTabCollapsed(CommandTabEventArgs args)
        {
            if (this.CommandTabCollapsed != null)
            {
                this.CommandTabCollapsed(this, args);
            }
        }

        /// <summary>
        ///		Raises the <see cref="ApplicationMenuStyleChanged"/> event.
        /// </summary>
        /// <param name="args">An <see cref="CommandTabEventArgs"/> that contains the event data.</param>
        protected virtual void OnApplicationMenuStyleChanged(EventArgs args)
        {
            bool isBackstage = (this.ApplicationMenuStyle == ApplicationMenuStyle.BackstageView);
            this.SetValue(IsBackstageModeProperty, isBackstage);
            this.dropDownButton.SetValue(IsBackstageModeProperty, isBackstage);
            this.quickAccessToolBar.SetValue(IsBackstageModeProperty, isBackstage);
            Telerik.WinControls.UI.RadQuickAccessToolBar.InnerItem item = this.quickAccessToolBar.GetInnerItem();

            if (item != null)
            {
                item.SetValue(IsBackstageModeProperty, isBackstage);
            }

            this.dropDownButton.SetDefaultValueOverride(RadButtonItem.ImageProperty,
                isBackstage ? Properties.Resources.telerikLogoSmall : Properties.Resources.telerikLogo);

            if (this.ApplicationMenuStyleChanged != null)
            {
                this.ApplicationMenuStyleChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="ExpandedStateChanged"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> instance that contains the event data.</param>
        protected virtual void OnRibbonBarExpandedStateChanged(EventArgs args)
        {
            if (this.ExpandedStateChanged != null)
            {
                this.ExpandedStateChanged(this, args);
            }
        }

        /// <summary>
        /// Calls the OnCommandTabCollapsed event.
        /// For internal use only.
        /// </summary>
        /// <param name="args">The event args associated with this event</param>
        internal void CallOnCommandTabCollapsed(CommandTabEventArgs args)
        {
            this.OnCommandTabCollapsed(args);
        }

        /// <summary>
        /// Calls the OnCommandTabExpanded event.
        /// For internal use only.
        /// </summary>
        /// <param name="args">The event args associated with this event</param>
        internal void CallOnCommandTabExpanded(CommandTabEventArgs args)
        {
            this.OnCommandTabExpanded(args);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.contextualTabGroups = new RadItemOwnerCollection();
            this.contextualTabGroups.DefaultType = typeof(ContextualTabGroup);
            this.contextualTabGroups.ItemTypes = new Type[] { typeof(ContextualTabGroup) };
            this.contextualTabGroups.ItemsChanged += new ItemChangedDelegate(ContextualTabGroups_ItemsChanged);
        }

        private Color[] contextualColors = new Color[]
		{
			Color.FromArgb(0xede278),
			Color.FromArgb(0xa5e28e),
			Color.FromArgb(0xebdb82),
			Color.FromArgb(0xd9b2d8),
			Color.FromArgb(0xf69f8d),
			Color.FromArgb(0x78a1d8)
		};

        private void ReplaceTabItemWithRibbonTab(ContextualTabGroup target)
        {
            int count = this.TabStripElement.Items.Count;
            int contextGroupCount = target.TabItems.Count;


            for (int i = 0; i < count; ++i)
            {
                RibbonTab ribbonTab = (RibbonTab)this.TabStripElement.Items[i];

                for (int j = 0; j < contextGroupCount; ++j)
                {
                    RadPageViewItem contextTabItem = (RadPageViewItem)target.TabItems[j];

                    if (ribbonTab.obsoleteTab == contextTabItem)
                    {
                        contextTabItem = ribbonTab;
                    }
                }
            }
        }


        /// <summary>Gets a boolean value indicating whether the inner layout is affected.</summary>
        public override bool AffectsInnerLayout
        {
            get
            {
                return true;
            }
        }

        private RadRibbonBarCaption ribbonCaption;

        public RadRibbonBarCaption RibbonCaption
        {
            get { return ribbonCaption; }
            set { ribbonCaption = value; }
        }

        /// <summary>
        /// Gets an instance of the <see cref="FillPrimitive"/>
        /// class that represents the fill behind the Ribbon's tab strip.
        /// </summary>
        public FillPrimitive TabStripPanelFill
        {
            get
            {
                return this.tabStripBackground;
            }
        }

        public ApplicationMenuStyle ApplicationMenuStyle
        {
            get { return applicationMenuStyle; }
            set
            {
                if (value == applicationMenuStyle)
                {
                    return;
                }

                applicationMenuStyle = value;

                this.OnApplicationMenuStyleChanged(EventArgs.Empty);
            }
        }

        private RadQuickAccessToolBar quickAccessToolBar;

        private FillPrimitive quickAccessToolbarFill;
        private RadItem quickAccessToolbarFillPanel;
        private FillPrimitive captionFill;
        private BorderPrimitive captionBorder;
        private RadItem tabStripHolder;
        private FillPrimitive tabStripBackground;
        protected override void CreateChildElements()
        {
            this.backstageControl = new RadRibbonBarBackstageView();

            this.dropDownButton = new RadApplicationMenuButtonElement();
            this.dropDownButton.Owner = this;
            this.dropDownButton.ThemeRole = "RibbonBarApplicationButton";
            this.dropDownButton.DisplayStyle = DisplayStyle.Image;
            this.dropDownButton.StretchHorizontally = false;
            this.dropDownButton.StretchVertically = false;
            this.dropDownButton.Margin = new Padding(0, 4, 0, 0);
            this.dropDownButton.ImageAlignment = ContentAlignment.MiddleCenter;
            this.dropDownButton.DropDownOpening += new CancelEventHandler(dropDownButton_DropDownOpening);
            this.dropDownButton.Click += new EventHandler(dropDownButton_Click);
            this.dropDownButton.ShowArrow = false;
            this.dropDownButton.ZIndex = 3;

            this.dropDownButton.Class = "ApplicationButton";

            if (this.dropDownButton.BorderElement != null)
            {
                this.dropDownButton.BorderElement.Class = "OfficeButtonBorder";
                this.dropDownButton.BorderElement.Visibility = ElementVisibility.Collapsed;
            }

            this.dropDownButton.ActionButton.Class = "OfficeButton";

            if (this.dropDownButton.ActionButton.BorderElement != null)
            {
                this.dropDownButton.ActionButton.BorderElement.Class = "OfficeButtonInnerBorder";
                this.dropDownButton.ActionButton.BorderElement.Visibility = ElementVisibility.Collapsed;
            }

            if (this.dropDownButton.ActionButton.ButtonFillElement != null)
            {
                this.dropDownButton.ActionButton.ButtonFillElement.Class = "OfficeButtonFill";
            }

            this.optionsButton = new RadMenuButtonItem();
            this.optionsButton.Click += new EventHandler(OfficeMenuButton_Click);
            this.optionsButton.Text = "Options";
            ((RadApplicationMenuDropDown)this.dropDownButton.DropDownMenu).ButtonItems.Add(this.optionsButton);

            this.exitButton = new RadMenuButtonItem();
            this.exitButton.Click += new EventHandler(OfficeMenuButton_Click);
            this.exitButton.Text = "Exit";
            ((RadApplicationMenuDropDown)this.dropDownButton.DropDownMenu).ButtonItems.Add(this.exitButton);

            // *** Tab strip
            this.tabStripHolder = new RadItem();
            this.tabStripHolder.Class = "RibbonBarTabStripHolder";
            //this.tabStripHolder.Margin = new Padding(0, 1, 0, 0);
            this.tabStripBackground = new FillPrimitive();
            this.tabStripBackground.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            this.tabStripBackground.Class = "RibbonBarTabStripBackground";
            this.tabStripBackground.GradientStyle = GradientStyles.Solid;

            tabStripHolder.Children.Add(this.tabStripBackground);
            this.tabStrip = new RibbonTabStripElement();
            this.tabStrip.StateManager = new RadRibbonBarElementStateManager().StateManagerInstance;
            this.tabStrip.ItemFitMode = StripViewItemFitMode.Shrink;
            this.tabStrip.ItemSelected += new EventHandler<RadPageViewItemSelectedEventArgs>(tabStrip_ItemSelected);
            this.tabStrip.ItemSelecting += new EventHandler<RadPageViewItemSelectingEventArgs>(tabStrip_ItemSelecting);
            this.tabStrip.ItemClicked += new EventHandler(tabStrip_ItemClicked);

            this.tabStripBackground.BindProperty(
                FillPrimitive.BackColorProperty, this.tabStrip.ContentArea,
                FillPrimitive.BackColorProperty, PropertyBindingOptions.OneWay);

            tabStripHolder.Children.Add(this.tabStrip);

            this.quickAccessToolBar = new RadQuickAccessToolBar(this);
            this.quickAccessToolBar.Margin = new Padding(-3, 0, 0, 0);
            this.quickAccessToolBar.BackColor = Color.Transparent;
            this.quickAccessToolBar.Items.ItemsChanged += new ItemChangedDelegate(OnQuickAccessToolbar_ItemsChanged);
            this.quickAccessToolBar.MinSize = new Size(0, this.QuickAccessMenuHeight);
            this.quickAccessToolBar.ZIndex = 1;
            this.Children.Add(this.quickAccessToolBar);

            // *** Caption
            ribbonCaption = new RadRibbonBarCaption(this);
            ribbonCaption.BackColor = Color.Transparent;
            ribbonCaption.Caption = "RadRibbonBar";
            ribbonCaption.BindProperty(RadItem.TextProperty, this, RadItem.TextProperty, PropertyBindingOptions.OneWay);
            this.contextualTabGroups.Owner = ribbonCaption.CaptionLayout;

            this.Children.Add(ribbonCaption);

            this.CommandTabs.ItemsChanged += new ItemChangedDelegate(CommandTabs_ItemsChanged);

            this.captionFill = new RibbonBarCaptionFillPrimitive();
            this.captionFill.ZIndex = -1;
            this.captionFill.Class = "RibbonCaptionFill";

            this.captionBorder = new BorderPrimitive();
            this.captionBorder.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.captionBorder.Class = "RibbonCaptionBorder";

            this.captionFill.Children.Add(captionBorder);

            //this.Children.Add(this.tabStrip);
            this.Children.Add(tabStripHolder);
            this.Children.Add(this.dropDownButton);


            ///////////////////////////
            //MDI functionality 
            //13.03.07
            //Peter            
            MDIbutton = new RadMDIControlsItem();
            MDIbutton.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            MDIbutton.ZIndex = 1000;
            MDIbutton.Class = "MDI control box";
            MDIbutton.ThemeRole = "MdiButtonsItem";
            MDIbutton.Alignment = ContentAlignment.MiddleRight;
            this.Children.Add(MDIbutton);
            ///////////////////////////

            this.Children.Add(this.captionFill);

            this.dropDownButton.SetDefaultValueOverride(RadButtonItem.ImageProperty, Properties.Resources.telerikLogo);

            this.quickAccessToolbarFill = new FillPrimitive();
            this.quickAccessToolbarFill.Class = "QuickAccessToolbarWhenBelowRibbonFill";
            this.quickAccessToolbarFill.StretchHorizontally = true;
            this.quickAccessToolbarFill.StretchVertically = true;

            this.quickAccessToolbarFill.BindProperty(FillPrimitive.BackColorProperty, this.tabStrip.ItemContainer, FillPrimitive.BackColorProperty, PropertyBindingOptions.OneWay);
            this.quickAccessToolbarFill.GradientStyle = GradientStyles.Solid;
            this.quickAccessToolbarFillPanel = new RadItem();
            this.quickAccessToolbarFillPanel.Class = "QuickAccessToolbarBelowRibbonPanel";
            this.quickAccessToolbarFillPanel.ZIndex = -1;
            this.quickAccessToolbarFillPanel.Children.Add(this.quickAccessToolbarFill);
            this.quickAccessToolbarFillPanel.StretchVertically = true;
            this.quickAccessToolbarFillPanel.StretchHorizontally = true;
            this.Children.Add(this.quickAccessToolbarFillPanel);


            //help button
            this.helpButton = new RadImageButtonElement();
            this.helpButton.ButtonFillElement.Visibility = ElementVisibility.Hidden;
            this.helpButton.DisplayStyle = DisplayStyle.Image;
            this.helpButton.ShowBorder = false;
            this.helpButton.StretchHorizontally = false;
            this.helpButton.StretchVertically = false;
            this.helpButton.ZIndex = 1000;
            this.helpButton.Image = Properties.Resources.ribbonBarHelpButton;
            //this.helpButton.Alignment = ContentAlignment.TopRight;
            this.helpButton.Padding = new Padding(0, 4, 2, 0);
            this.helpButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.helpButton.Visibility = ElementVisibility.Collapsed;
            this.Children.Add(this.helpButton);

            //expand button
            this.expandButton = new RadToggleButtonElement();
            this.expandButton.ToggleState = Enumerations.ToggleState.On;
            this.expandButton.ToggleStateChanged += new StateChangedEventHandler(expandButton_ToggleStateChanged);
            this.expandButton.ToggleStateChanging += new StateChangingEventHandler(expandButton_ToggleStateChanging);
            this.expandButton.DisplayStyle = DisplayStyle.Image;
            this.expandButton.ShowBorder = false;
            this.expandButton.StretchHorizontally = false;
            this.expandButton.StretchVertically = false;
            this.expandButton.ZIndex = 1000;
            this.expandButton.Image = Properties.Resources.DropDown2;
            this.expandButton.Padding = new Padding(6);
            this.expandButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.expandButton.Visibility = ElementVisibility.Visible;
            this.expandButton.Class = "RibbonBarExpandButton";
            this.Children.Add(this.expandButton);
        }

        private void tabStrip_ItemSelected(object sender, RadPageViewItemSelectedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                this.SelectTab((RibbonTab)e.SelectedItem);
            }
        }

        private void tabStrip_ItemSelecting(object sender, RadPageViewItemSelectingEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                this.UnselectTab((RibbonTab)this.tabStrip.SelectedItem);
            }
        }

        private void tabStrip_ItemClicked(object sender, EventArgs e)
        {
            if (this.BackstageControl.IsShown)
            {
                this.backstageControl.HidePopup();
            }
        }

        void expandButton_ToggleStateChanging(object sender, StateChangingEventArgs args)
        {
            if (this.CommandTabs.Count == 0)
            {
                args.Cancel = true;
            }
        }

        void expandButton_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            if (this.suspendExpandButtonNotifications == 0)
            {
                this.Expanded = (args.ToggleState == Enumerations.ToggleState.On);
            }
        }

        void dropDownButton_Click(object sender, EventArgs e)
        {
            if (this.applicationMenuStyle == ApplicationMenuStyle.BackstageView && this.ElementTree.Control.Site == null)
            {
                if (this.backstageControl.IsShown)
                {
                    this.backstageControl.HidePopup();
                }
                else
                {
                    Point backstageLocation = Point.Empty;
                    backstageLocation.X = this.ElementTree.Control.Location.X;
                    backstageLocation.Y = this.ElementTree.Control.Location.Y + this.dropDownButton.ControlBoundingRectangle.Bottom - 1;
                    this.backstageControl.ThemeName = (this.ElementTree.Control as RadControl).ThemeName;
                    this.backstageControl.RightToLeft = (this.RightToLeft) ?
                        System.Windows.Forms.RightToLeft.Yes :
                        System.Windows.Forms.RightToLeft.No;

                    this.backstageControl.ShowPopup(backstageLocation, this);
                }
            }
        }

        void dropDownButton_DropDownOpening(object sender, CancelEventArgs e)
        {

            if (this.applicationMenuStyle == ApplicationMenuStyle.BackstageView && this.ElementTree.Control.Site == null)
            {
                e.Cancel = true;
            }
        }

        public int ApplicationMenuRightColumnWidth
        {
            get
            {
                return ((RadApplicationMenuDropDown)this.dropDownButton.DropDownMenu).RightColumnWidth;
            }
            set
            {
                ((RadApplicationMenuDropDown)this.dropDownButton.DropDownMenu).RightColumnWidth = value;
            }
        }

        #region New layouts

        private SizeF GetSizeForQAToolbar(SizeF availableSize)
        {
            SizeF measureSize;

            if (!this.QuickAccessToolbarBelowRibbon)
            {
                float width = availableSize.Width
                    - (ribbonCaption.SystemButtons.DesiredSize.Width +
                    this.ApplicationButtonElement.DesiredSize.Width
                    + this.quickAccessToolBar.Margin.Horizontal);

                if (!this.IsDesignMode)
                {
                    ContextualTabGroup tabGroup = this.ribbonCaption.CaptionLayout.GetLeftMostGroup(false);

                    if (tabGroup != null)
                    {
                        if (!this.RightToLeft)
                        {
                            width = tabGroup.TabItems[0].ControlBoundingRectangle.X - this.dropDownButton.DesiredSize.Width;
                        }
                        else
                        {

                            width = availableSize.Width -
                            (tabGroup.TabItems[0].ControlBoundingRectangle.Right + this.dropDownButton.DesiredSize.Width);
                        }
                    }
                }

                measureSize = new SizeF(width, availableSize.Height);

            }
            else
            {
                measureSize = new SizeF(availableSize);
            }

            return measureSize;
        }

        private void MeasureQAToolbar(SizeF availableSize)
        {
            this.quickAccessToolBar.Measure(this.GetSizeForQAToolbar(availableSize));
        }

        private void ArrangeQAToolbar(SizeF finalSize)
        {
            RectangleF quickAccessToolBarRectangle;
            float yCoord = Math.Abs((this.ribbonCaption.DesiredSize.Height - this.quickAccessToolBar.DesiredSize.Height) / 2);
            float quickAccessToolbarYCoord = (this.tabStrip.Visibility == ElementVisibility.Collapsed) ? this.dropDownButton.ControlBoundingRectangle.Bottom : this.tabStrip.ControlBoundingRectangle.Bottom;
            if (!this.tabStrip.Children.Contains(this.tabStrip.ContentArea))
            {
                quickAccessToolbarYCoord -= this.tabStrip.ContentArea.DesiredSize.Height;
            }

            if (!this.RightToLeft)
            {
                if (this.QuickAccessToolbarBelowRibbon)
                {
                    quickAccessToolBarRectangle = new RectangleF(
                        0f,
                        quickAccessToolbarYCoord,
                        this.quickAccessToolBar.DesiredSize.Width,
                        this.quickAccessToolBar.DesiredSize.Height);
                }
                else
                {
                    quickAccessToolBarRectangle = new RectangleF(dropDownButton.DesiredSize.Width, yCoord, quickAccessToolBar.DesiredSize.Width, quickAccessToolBar.DesiredSize.Height);
                }
            }
            else
            {
                if (this.QuickAccessToolbarBelowRibbon)
                {
                    quickAccessToolBarRectangle = new RectangleF(
                        finalSize.Width - this.quickAccessToolBar.DesiredSize.Width,
                        quickAccessToolbarYCoord,
                        this.quickAccessToolBar.DesiredSize.Width,
                        this.quickAccessToolBar.DesiredSize.Height);
                }
                else
                {
                    quickAccessToolBarRectangle = new RectangleF(
                        finalSize.Width - this.dropDownButton.DesiredSize.Width - this.quickAccessToolBar.DesiredSize.Width,
                        yCoord,
                        this.quickAccessToolBar.DesiredSize.Width,
                        this.quickAccessToolBar.DesiredSize.Height);
                }
            }
            this.quickAccessToolBar.InvalidateArrange();
            this.quickAccessToolBar.Arrange(quickAccessToolBarRectangle);
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF res = SizeF.Empty;
            this.MDIbutton.Measure(availableSize);
            this.HelpButton.Measure(availableSize);
            this.ExpandButton.Measure(availableSize);

            this.captionFill.Measure(availableSize);
            this.tabStripHolder.Measure(availableSize);
            this.dropDownButton.Measure(availableSize);

            this.MeasureQAToolbar(availableSize);

            float captionHeight = this.QuickAccessMenuHeight + this.QuickAccessToolBar.Margin.Vertical;
            float highestExpandableLayoutHeight = this.tabStrip.Visibility != ElementVisibility.Collapsed
                ? this.GetMaximumTabStripHeight() : 0;

            if (this.QuickAccessToolbarBelowRibbon)
            {
                this.ribbonCaption.Measure(new SizeF(availableSize.Width - dropDownButton.DesiredSize.Width, captionHeight));
                float fillHeight = this.Size.Height - this.quickAccessToolBar.ControlBoundingRectangle.Top;
                this.quickAccessToolbarFillPanel.Measure(new SizeF(availableSize.Width, fillHeight));
            }
            else
            {
                this.ribbonCaption.Measure(new SizeF(availableSize.Width - (dropDownButton.DesiredSize.Width + (this.quickAccessToolBar.DesiredSize.Width)), captionHeight));
                this.quickAccessToolbarFillPanel.Measure(Size.Empty);
            }

            float captionMaxHeight = this.QuickAccessMenuHeight;
            if (this.tabStrip.Visibility == ElementVisibility.Collapsed)
            {
                captionMaxHeight = Math.Max(this.dropDownButton.ControlBoundingRectangle.Bottom, this.QuickAccessMenuHeight);
            }
            res.Height = highestExpandableLayoutHeight
                + captionMaxHeight + this.Padding.Vertical;

            if (this.QuickAccessToolbarBelowRibbon)
            {
                res.Height += this.QuickAccessMenuHeight;
            }

            res.Width = availableSize.Width;
            return res;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF dropDownButtonRectangle;
            RectangleF tabStripRectangle = RectangleF.Empty;
            if (this.tabStrip.Visibility == ElementVisibility.Collapsed)
            {
                tabStripRectangle = new RectangleF(0f,
                   this.quickAccessToolBar.DesiredSize.Height,
                   finalSize.Width, this.tabStrip.ItemContainer.DesiredSize.Height);
                this.tabStripHolder.Arrange(tabStripRectangle);
            }
            else
            {
                float highestExpandableLayoutHeight = this.GetMaximumTabStripHeight();
                tabStripRectangle = new RectangleF(0f,
                    this.quickAccessToolBar.DesiredSize.Height,
                    finalSize.Width, highestExpandableLayoutHeight);
                this.tabStripHolder.Arrange(tabStripRectangle);
            }

            float mdiButtonOffset = this.MDIbutton.DesiredSize.Width;
            if (this.MDIbutton.Visibility != ElementVisibility.Visible)
            {
                mdiButtonOffset = 0;
            }
            if (!this.RightToLeft)
            {
                this.ExpandButton.Arrange(new RectangleF(tabStripRectangle.Width - mdiButtonOffset - this.ExpandButton.DesiredSize.Width - this.HelpButton.DesiredSize.Width, tabStripRectangle.Y, this.ExpandButton.DesiredSize.Width, this.ExpandButton.DesiredSize.Height));
                this.HelpButton.Arrange(new RectangleF(tabStripRectangle.Width - mdiButtonOffset - this.HelpButton.DesiredSize.Width, tabStripRectangle.Y, this.HelpButton.DesiredSize.Width, this.HelpButton.DesiredSize.Height));
                this.MDIbutton.Arrange(new RectangleF(tabStripRectangle.Width - this.MDIbutton.DesiredSize.Width,
                    this.ribbonCaption.ControlBoundingRectangle.Bottom,
                    this.MDIbutton.DesiredSize.Width, this.MDIbutton.DesiredSize.Height));
            }
            else
            {
                this.ExpandButton.Arrange(new RectangleF(mdiButtonOffset + this.helpButton.DesiredSize.Width, tabStripRectangle.Y, this.ExpandButton.DesiredSize.Width, this.ExpandButton.DesiredSize.Height));
                this.HelpButton.Arrange(new RectangleF(mdiButtonOffset, tabStripRectangle.Y, this.HelpButton.DesiredSize.Width, this.HelpButton.DesiredSize.Height));
                this.MDIbutton.Arrange(new RectangleF(0, this.ribbonCaption.ControlBoundingRectangle.Bottom,
                    this.MDIbutton.DesiredSize.Width, this.MDIbutton.DesiredSize.Height));
            }

            this.captionFill.Arrange(new RectangleF(0f, 0f, finalSize.Width, this.quickAccessToolBar.DesiredSize.Height));//            

            //Application menu position
            if (!this.RightToLeft)
            {
                dropDownButtonRectangle = new RectangleF(PointF.Empty, dropDownButton.DesiredSize);
            }
            else
            {
                dropDownButtonRectangle = new RectangleF(new PointF(finalSize.Width - dropDownButton.DesiredSize.Width, 0f), dropDownButton.DesiredSize);
            }
            this.dropDownButton.Arrange(dropDownButtonRectangle);

            //end Application menu

            this.ArrangeQAToolbar(finalSize);

            RectangleF ribbonCaptionRectangle;

            if (!this.QuickAccessToolbarBelowRibbon)
            {
                if (!this.RightToLeft)
                {
                    ribbonCaptionRectangle = new RectangleF(new PointF(this.dropDownButton.DesiredSize.Width + this.quickAccessToolBar.DesiredSize.Width, 0), this.ribbonCaption.DesiredSize);
                }
                else
                {
                    ribbonCaptionRectangle = new RectangleF(Point.Empty, this.ribbonCaption.DesiredSize);
                }

                this.quickAccessToolbarFillPanel.Arrange(new RectangleF(0, 0, 0, 0));

            }
            else
            {
                if (!this.RightToLeft)
                {
                    ribbonCaptionRectangle = new RectangleF(new PointF(this.dropDownButton.DesiredSize.Width, 0), this.ribbonCaption.DesiredSize);
                }
                else
                {
                    ribbonCaptionRectangle = new RectangleF(Point.Empty, this.ribbonCaption.DesiredSize);
                }
                float fillHeight = this.quickAccessToolBar.DesiredSize.Height + this.quickAccessToolBar.Padding.Vertical;
                this.quickAccessToolbarFillPanel.Arrange(new RectangleF(0, this.quickAccessToolBar.ControlBoundingRectangle.Location.Y, this.Size.Width, fillHeight));
            }

            this.ribbonCaption.Arrange(ribbonCaptionRectangle);


            return finalSize;
        }

        #endregion

        protected override void PaintOverride(Telerik.WinControls.Paint.IGraphics screenRadGraphics, Rectangle clipRectangle, float angle, SizeF scale, bool useRelativeTransformation)
        {
            Form parentForm = this.ElementTree.Control.FindForm();

            base.PaintOverride(screenRadGraphics, clipRectangle, angle, scale, useRelativeTransformation);


            if (((this.ElementTree.Control is RadRibbonBar)) && (this.ElementTree.Control as RadRibbonBar).CompositionEnabled && !this.IsDesignMode)
            {
                if (parentForm is RadFormControlBase &&
                    (parentForm as RadFormControlBase).FormBehavior is RadRibbonFormBehavior)
                {
                    this.ribbonCaption.CaptionLayout.CaptionTextElement.ControlBoundingRectangle.Inflate(10, 0);
                    bool isWindowsSeven = Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
                    Color textColor = parentForm.WindowState == FormWindowState.Maximized && !isWindowsSeven ? Color.White : Color.Black;
                    Rectangle drawRect = this.ribbonCaption.CaptionLayout.CaptionTextElement.ControlBoundingRectangle;
                    drawRect.Inflate(8, 5);
                    TelerikPaintHelper.DrawGlowingText(
                        (Graphics)screenRadGraphics.UnderlayGraphics,
                        this.Text,
                        (this.ribbonCaption.CaptionLayout.CaptionTextElement as TextPrimitive).Font,
                        drawRect,
                        textColor, TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }



        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadElement.RightToLeftProperty)
            {
                bool isRTL = (bool)e.NewValue;
                this.TabStripElement.RightToLeft = isRTL;

                //this.quickAccessToolBar.Children[1].Children[0].Shape = isRTL ? null : new Telerik.WinControls.Tests.QAShape();

                if (isRTL)
                {
                    this.dropDownButton.DropDownMenu.HorizontalPopupAlignment = HorizontalPopupAlignment.RightToRight;
                }
                else
                {
                    this.dropDownButton.DropDownMenu.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToLeft;
                }
            }
            else if (e.Property == QuickAccessToolbarBelowRibbonProperty)
            {
                this.quickAccessToolBar.GetInnerItem().ShowFillAndBorder(!this.QuickAccessToolbarBelowRibbon);

                this.InvalidateMeasure();
                this.InvalidateArrange();
            }
            else if (e.Property == RadRibbonBarElement.QuickAccessMenuHeightProperty)
            {
                this.quickAccessToolBar.MinSize = new Size(0, (int)e.NewValue);
                this.quickAccessToolBar.MaxSize = new Size(0, (int)e.NewValue);
            }
            else if (e.Property == RadItem.BoundsProperty)
            {
                if (this.ribbonPopup != null && this.lastClickedTab != null)
                {
                    this.ribbonPopup.Size = new Size(
                        (int)(this.tabStrip.DesiredSize.Width),
                        (int)Math.Ceiling(this.GetMaximumTabStripHeight()));
                    Point popupLocation = this.ElementTree.Control.PointToScreen(this.ControlBoundingRectangle.Location);
                    popupLocation.Offset(0, this.lastClickedTab.ControlBoundingRectangle.Bottom - this.lastClickedTab.BorderThickness.Vertical);
                    this.ribbonPopup.Location = popupLocation;
                }
            }
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);
            RadPageViewItem item = sender as RadPageViewItem;
            if (item != null)
            {
                RadRibbonBar ribbon = (RadRibbonBar)this.ElementTree.Control;

                if (args.RoutedEvent == RadElement.MouseClickedEvent)
                {
                    if (!this.Expanded)
                    {
                        if (object.ReferenceEquals(sender, lastClickedTab))
                        {
                            this.ribbonPopup.ClosePopup(RadPopupCloseReason.Mouse);
                            this.lastClickedTab = null;
                        }
                        else
                        {
                            this.ribbonPopup.Size = new Size(
                                (this.Size.Width),
                                (int)Math.Ceiling(this.GetMaximumTabContentHeight()));
                            Point popupLocation = this.ElementTree.Control.PointToScreen(this.ControlBoundingRectangle.Location);
                            popupLocation.Offset(0, item.ControlBoundingRectangle.Bottom - item.BorderThickness.Vertical);
                            this.ribbonPopup.ThemeName = this.ElementTree.ThemeName;

                            ExpandableStackLayout.InvalidateAll(this.ribbonPopup.RootElement);
                            this.ribbonPopup.RootElement.UpdateLayout();
                            this.ribbonPopup.ShowPopup(new Rectangle(popupLocation, new Size(1, 1)));

                            this.lastClickedTab = item;
                            if (this.lastPopupTab == item)
                            {
                                this.tabStrip.InvalidateMeasure(true);
                            }
                            this.lastPopupTab = item;
                        }
                    }
                }

                if (args.RoutedEvent == RadElement.MouseDoubleClickedEvent)
                {
                    if (!this.collapseRibbonOnTabDoubleClick)
                    {
                        return;
                    }

                    //if the currently double clicked element is the selected tab
                    if (item.IsSelected && ribbon.Expanded)
                    {
                        this.Expanded = false;
                        this.tabStrip.SelectedItem = null;
                    }
                    else if (item.IsSelected && !ribbon.Expanded)
                    {
                        this.Expanded = true;
                        foreach (RadPageViewItem tabItem in this.tabStrip.Items)
                        {
                            if (tabItem.Text == item.Text)
                            {
                                this.tabStrip.SelectItem(tabItem);
                                break;
                            }
                        }
                    }

                    return;
                }
            }
        }

        #region Update references

        protected internal override void UpdateReferences(ComponentThemableElementTree tree, bool updateInheritance, bool recursive)
        {
            base.UpdateReferences(tree, updateInheritance, recursive);

            if (this.containerControl == null)
            {
                this.containerControl = this.ElementTree.Control;
            }
            else
            {
                this.containerControl.ClientSizeChanged -= new EventHandler(Control_SizeChanged);
                this.containerControl.VisibleChanged -= new EventHandler(Control_VisibleChanged);
            }

            this.ElementTree.Control.ClientSizeChanged += new EventHandler(Control_SizeChanged);
            this.ElementTree.Control.VisibleChanged += new EventHandler(Control_VisibleChanged);
            this.containerControl = this.ElementTree.Control;
        }

        //The VisibleChanged event is used because the layout is
        //not triggered on time in certain cases.
        private void Control_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.IsDisposed)
            {
                this.tabStrip.InvalidateMeasure();
                this.tabStrip.InvalidateArrange();
                this.tabStrip.UpdateLayout();
                this.ribbonCaption.CaptionLayout.InvalidateMeasure();
                this.ribbonCaption.CaptionLayout.InvalidateArrange();
                this.InvalidateMeasure();
                this.InvalidateArrange();
                this.ribbonCaption.CaptionLayout.UpdateLayout();
                this.UpdateLayout();
            }
        }

        //The SizeChanged event is used because the layout is
        //not triggered on time in certain cases.
        private void Control_SizeChanged(object sender, EventArgs e)
        {
            if (!this.IsDisposed)
            {
                this.ribbonCaption.CaptionLayout.InvalidateMeasure();
                this.ribbonCaption.CaptionLayout.InvalidateArrange();
                this.InvalidateMeasure();
                this.InvalidateArrange();
                this.ribbonCaption.CaptionLayout.UpdateLayout();
                this.UpdateLayout();
            }
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="FillPrimitive"/>instance
        /// that represents the fill of the ribbon's caption.
        /// </summary>
        [Browsable(false)]
        public FillPrimitive CaptionFill
        {
            get
            {
                return this.captionFill;
            }
        }

        /// <summary>
        /// Gets the <see cref="BorderPrimitive"/>instance
        /// that represents the border of the ribbon's caption.
        /// </summary>
        [Browsable(false)]
        public BorderPrimitive CaptionBorder
        {
            get
            {
                return this.captionBorder;
            }
        }

        /// <summary>
        /// Gets the QuickAccessToolBar
        /// </summary>
        [Browsable(false)]
        public RadQuickAccessToolBar QuickAccessToolBar
        {
            get
            {
                return quickAccessToolBar;
            }
        }

        ///////////////////////////
        //RibbonBar help button
        //07.07.09
        //Peter 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Represent the Ribbon Help button!")]
        public RadImageButtonElement HelpButton
        {
            get
            {
                return this.helpButton;
            }
            set
            {
                this.helpButton = value;
            }
        }

        ///////////////////////////
        //RibbonBar expand/collapse button
        //03.03.11
        //IT 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Represent the Ribbon Expand/Collapse button!")]
        public RadToggleButtonElement ExpandButton
        {
            get
            {
                return expandButton;
            }
            set
            {
                expandButton = value;
            }
        }

        #region Popup support

        private RadElement chunksHolder;
        private RadPageViewItem lastClickedTab;
        private RadPageViewItem lastPopupTab;

        private void PrepareRibbonPopup()
        {
            if (this.ribbonPopup == null)
            {
                this.ribbonPopup = new RibbonBarPopup(this);
                this.ribbonPopup.RootElement.RightToLeft = this.ElementTree.RootElement.RightToLeft;
                this.chunksHolder = this.tabStrip.ContentArea;
                this.ribbonPopup.PopupClosed += this.RadRibbonBar_PopupClosed;
            }

            if (this.tabStrip.Children.Contains(this.chunksHolder))
            {
                this.tabStrip.Children.Remove(this.chunksHolder);
                this.ribbonPopup.RootElement.Children.Add(this.chunksHolder);
                if (this.ElementTree != null)
                {
                    this.ribbonPopup.ImageList = this.ElementTree.ComponentTreeHandler.ImageList;
                }

                this.InvalidateMeasure();
            }
        }

        private void RadRibbonBar_PopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            if (this.IsInValidState(true))
            {
                RadRibbonBar ribbon = this.ElementTree.Control as RadRibbonBar;

                if (!ribbon.Expanded)
                {
                    this.tabStrip.SelectedItem = null;
                    this.lastClickedTab = null;
                }

                this.TabStripElement.InvalidateMeasure(true);
            }
        }

        private void RevertPopupSettings()
        {
            this.ribbonPopup.ClosePopup(RadPopupCloseReason.CloseCalled);
            this.ribbonPopup.RootElement.Children.Remove(this.chunksHolder);
            this.tabStrip.Children.Add(this.chunksHolder);

            this.InvalidateMeasure();
        }

        /// <summary>
        /// Gets an instance of the RibbonBarPopup class which represents the
        /// RadRibbonBar popup.
        /// </summary>
        [Browsable(false)]
        public RibbonBarPopup Popup
        {
            get
            {
                return this.ribbonPopup;
            }
        }

        #endregion

        private bool contentAreaColorsAltered = false;
        private RadPageViewItem previousItem = null;

        private void UnselectTab(RibbonTab commandTab)
        {
            if (commandTab == null)
            {
                return;
            }
            commandTab.Items.Owner.Visibility = ElementVisibility.Collapsed;
        }

        private RibbonTab tabOldSelected = null;

        private void SelectTab(RibbonTab commandTab)
        {
            UnselectTab(this.tabOldSelected);
            if (this.BackstageControl.IsShown)
            {
                this.backstageControl.HidePopup();
            }

            this.tabOldSelected = (RibbonTab)this.TabStripElement.SelectedItem;

            commandTab.InvalidateMeasure();
            commandTab.Items.Owner.Visibility = ElementVisibility.Visible;
            commandTab.InvalidateMeasure();

            ContextualTabGroup group = this.FindContextualTabGroup(commandTab);
            this.ChangeTabBaseFillStyle(group, this.tabStrip.ContentArea, commandTab);
            this.previousItem = commandTab;

            this.OnCommandTabSelected(new CommandTabEventArgs(commandTab));
        }

        protected virtual void ChangeTabBaseFillStyle(ContextualTabGroup group, LightVisualElement contentArea, LightVisualElement tabFill)
        {
            if (group != null)
            {
                contentAreaColorsAltered = true;
                contentArea.DrawFill = true;
                contentArea.BackColor = Color.FromArgb(200, group.BaseColor);

                tabFill.DrawFill = true;
                tabFill.BackColor = Color.FromArgb(200, group.BaseColor);
                tabFill.BackColor2 = Color.FromArgb(100, group.BaseColor);
                tabFill.BackColor3 = Color.FromArgb(50, group.BaseColor);
                tabFill.BackColor4 = Color.FromArgb(100, group.BaseColor);
            }
            else
            {
                if (contentAreaColorsAltered)
                {
                    contentArea.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                    contentArea.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                    contentAreaColorsAltered = false;
                }
            }

            if (this.previousItem != null)
            {
                this.previousItem.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                this.previousItem.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                this.previousItem.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
                this.previousItem.ResetValue(LightVisualElement.BackColor3Property, ValueResetFlags.Local);
                this.previousItem.ResetValue(LightVisualElement.BackColor4Property, ValueResetFlags.Local);
            }
        }

        private ContextualTabGroup FindContextualTabGroup(RibbonTab commandTab)
        {
            ContextualTabGroup result = null;

            foreach (ContextualTabGroup group in this.contextualTabGroups)
            {
                if (group.TabItems.Contains(commandTab.Tab))
                {
                    result = group;
                    break;
                }
            }

            return result;
        }

        internal void SetParentForm()
        {
            this.MDIbutton.LayoutPropertyChanged();
        }

        #region Event handling

        private void ContextualTabGroups_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted)
            {
                ContextualTabGroup newGroup = (ContextualTabGroup)target;

                this.ReplaceTabItemWithRibbonTab(newGroup);

                if (newGroup.BaseColor == Color.Empty)
                {
                    int newColorIndex = this.contextualTabGroups.Count % this.contextualColors.Length;
                    newGroup.BaseColor = this.contextualColors[newColorIndex];
                }

                if (this.IsDesignMode)
                {
                    this.ribbonCaption.CaptionLayout.InvalidateMeasure();
                    this.ribbonCaption.CaptionLayout.InvalidateArrange();
                    this.ribbonCaption.CaptionLayout.UpdateLayout();
                }
            }
            else if (operation == ItemsChangeOperation.Removed)
            {
                if (this.IsDesignMode)
                {
                    this.ribbonCaption.CaptionLayout.InvalidateMeasure();
                    this.ribbonCaption.CaptionLayout.InvalidateArrange();
                    this.ribbonCaption.CaptionLayout.UpdateLayout();
                }
            }
        }

        private void OfficeMenuButton_Click(object sender, EventArgs e)
        {
            this.dropDownButton.HideDropDown();
        }

        private void OnQuickAccessToolbar_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted ||
                operation == ItemsChangeOperation.Set)
            {
                if (target is RadButtonElement)
                {
                    target.AddBehavior(new RibbonButtonBorderBehavior());
                }
                target.NotifyParentOnMouseInput = true;
            }
        }

        private void CommandTabs_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            this.highestTabContentPanelHeight = null;

            RibbonTab commandTab = (RibbonTab)target;
            if (operation == ItemsChangeOperation.Removed)
            {
                this.tabStrip.RemoveItem(commandTab.Tab);
                this.removedTab = commandTab;
                if (this.tabStrip.SelectedItem == commandTab.Tab)
                {
                    if (this.tabStrip.Items.Count > 0)
                    {
                        this.tabStrip.SelectedItem = this.tabStrip.Items[0];
                    }
                    else
                    {
                        this.tabStrip.SelectedItem = null;
                    }
                }
            }
            else if (operation == ItemsChangeOperation.Inserted)
            {
                if (commandTab.Tab != null && !this.tabStrip.Items.Contains(commandTab.Tab))
                {
                    //  this.tabStrip.Items.Insert(target.Index, commandTab.Tab);
                    if (this.tabStrip.SelectedItem == commandTab)
                    {
                        this.SelectTab(commandTab);
                    }
                    if (this.removedTab == commandTab)
                    {
                        this.tabStrip.SelectedItem = commandTab;
                    }
                }
            }

            if (this.ribbonCaption != null)
            {
                if (this.ribbonCaption.CaptionLayout != null)
                {

                    this.tabStrip.InvalidateMeasure();
                    this.tabStrip.InvalidateArrange();
                    this.tabStrip.UpdateLayout();
                    this.ribbonCaption.InvalidateMeasure();
                    this.ribbonCaption.InvalidateArrange();
                    this.ribbonCaption.CaptionLayout.InvalidateMeasure();
                    this.ribbonCaption.CaptionLayout.InvalidateArrange();
                }
            }
        }

        #endregion

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            if (this.containerControl != null)
            {
                this.containerControl.SizeChanged -= new EventHandler(Control_SizeChanged);
                this.containerControl.VisibleChanged -= new EventHandler(Control_VisibleChanged);
            }

            if (this.ribbonPopup != null && this.ribbonPopup.IsPopupShown)
            {
                this.ribbonPopup.ClosePopup(RadPopupCloseReason.CloseCalled);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return this.ElementTree.Control;
            }
            set
            {

            }
        }
    }
}
