using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using ContentAlignment = System.Drawing.ContentAlignment;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Elements;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///     Represents a rad dropdown button element. The
	///     <see cref="RadDropDownButton">RadDropDownButton</see> control is a simple wrapper
	///     for the RadDropDownButtonElement. All UI and logic functionality is implemented in
	///     RadDropDownButtonElement class. The
	///     <see cref="RadDropDownButton">RadDropDownButton</see> acts to transfer events to
	///     and from its RadDropDownButtonElement instance. RadDropDownButtonElement can be
	///     nested in other telerik controls.
	/// </summary>
	[ToolboxItem(false), ComVisible(false)]
	[Designer(DesignerConsts.RadDropDownButtonElementDesignerString)]
	public class RadDropDownButtonElement : RadItem, IDropDownMenuOwner, ISiteProvider, IItemsOwner, IImageElement, ITooltipOwner
    {
        #region Fields

        #region BitState Keys

        internal const ulong PreparedForDesignTimeStateKey = RadItemLastStateKey << 1;
        internal const ulong DropDownInheritThemeClassNameStateKey = PreparedForDesignTimeStateKey << 1;
        internal const ulong RadDropDownButtonElementLastStateKey = DropDownInheritThemeClassNameStateKey;

        #endregion

        // Drop down fileds
        protected RadDropDownButtonPopup menu;

		// Children        
		protected RadButtonElement actionButton;
		protected DropDownEditorLayoutPanel layoutPanel;
		protected RadArrowButtonElement arrowButton;
		private static Size ArrowButtonDefaultSize = new Size(12, 12);
		private BorderPrimitive borderPrimitive;
        private object owner = null;

		private RadDirection dropDownDirection = RadDirection.Down;

        private static readonly object DropDownOpeningEventKey;
		private static readonly object DropDownOpenedEventKey;
		private static readonly object DropDownClosedEventKey;
		private static readonly object DropDownItemClickedEventKey;
        private static readonly object DropDownClosingEventKey;

        #endregion

        #region Dependency properties

        public static RadProperty IsPressedProperty = RadProperty.Register(
			"IsPressed",
			typeof(bool),
			typeof(RadDropDownButtonElement),
			new RadElementPropertyMetadata(
				false,
				ElementPropertyOptions.None));

		public static RadProperty IsDropDownShownProperty = RadProperty.Register(
			"IsDropDownShown", typeof(bool), typeof(RadDropDownButtonElement), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.None));

		public static RadProperty MouseOverStateProperty = RadProperty.Register(
			"MouseOverState",
			typeof(DropDownButtonMouseOverState),
			typeof(RadDropDownButtonElement),
			new RadElementPropertyMetadata(
                DropDownButtonMouseOverState.None,
				ElementPropertyOptions.None));

		public static RadProperty ShowArrowProperty = RadProperty.Register(
			"ShowArrow",
			typeof(bool),
			typeof(RadDropDownButtonElement),
			new RadElementPropertyMetadata(
				true,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		#endregion

        #region Property mappings

        public static Dictionary<RadProperty, RadProperty> mappedStyleProperties;

        private static void AddMappedPropertyMappings()
        {
            mappedStyleProperties.Add(ImagePrimitive.ImageProperty, RadButtonItem.ImageProperty);
            mappedStyleProperties.Add(ImagePrimitive.ImageKeyProperty, RadButtonItem.ImageKeyProperty);
        }

        internal override RadProperty MapStyleProperty(RadProperty propertyToMap, string settingType)
        {
            RadProperty mappedProperty = null;
            if (mappedStyleProperties.TryGetValue(propertyToMap, out mappedProperty))
            {
                return mappedProperty;
            }
            return base.MapStyleProperty(propertyToMap, settingType);
        }

        #endregion

        #region Initialization

        static RadDropDownButtonElement()
		{
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new DropDownButtonStateManagerFatory(), typeof(RadDropDownButtonElement));
            mappedStyleProperties = new Dictionary<RadProperty, RadProperty>();

            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadDropDownButton().DeserializeTheme();
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadDropDownMenu().DeserializeTheme();

            DropDownOpeningEventKey = new object();
			DropDownOpenedEventKey = new object();
			DropDownClosedEventKey = new object();
            DropDownClosingEventKey = new object();
			DropDownItemClickedEventKey = new object();

            AddMappedPropertyMappings();
		}

        /// <summary>Initializes a new instance of the DropDownButtonElement class.</summary>
        public RadDropDownButtonElement()
        {
            this.menu = CreateDropDown();
            this.menu.DropDownOpening += new CancelEventHandler(menu_DropDownOpening);
            this.menu.DropDownOpened += new EventHandler(menu_DropDownOpened);
            this.menu.DropDownClosed += new RadPopupClosedEventHandler(menu_DropDownClosed);
            this.menu.DropDownClosing += new RadPopupClosingEventHandler(menu_DropDownClosing);
        }

        protected override void DisposeManagedResources()
        {
            menu.Dispose();

            base.DisposeManagedResources();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.ThemeRole = "DropDownButton";
        }

        protected override void CreateChildElements()
        {
            this.arrowButton = new RadArrowButtonElement();
            this.arrowButton.MinSize = RadDropDownButtonElement.ArrowButtonDefaultSize;
            this.arrowButton.SetValue(DropDownEditorLayoutPanel.IsArrowButtonProperty, true);
            this.arrowButton.Class = "DropDownButtonArrowButton";
            this.arrowButton.MouseEnter += new EventHandler(arrowButton_MouseEnter);
            this.arrowButton.MouseLeave += new EventHandler(arrowButton_MouseLeave);
            this.arrowButton.Click += new EventHandler(arrowButton_Click);

            this.actionButton = new ActionButtonElement();
            this.actionButton.BindProperty(RadButtonElement.DisplayStyleProperty, this, RadButtonItem.DisplayStyleProperty, PropertyBindingOptions.OneWay);
            this.actionButton.BindProperty(RadButtonElement.ImageAlignmentProperty, this, RadButtonItem.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
            this.actionButton.BindProperty(RadButtonElement.TextAlignmentProperty, this, RadButtonItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);
            this.actionButton.BindProperty(RadButtonElement.TextImageRelationProperty, this, RadButtonItem.TextImageRelationProperty, PropertyBindingOptions.OneWay);
            this.actionButton.BindProperty(RadButtonElement.TextProperty, this, RadButtonItem.TextProperty, PropertyBindingOptions.TwoWay);
            this.actionButton.BindProperty(RadButtonElement.ImageIndexProperty, this, RadButtonItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.actionButton.BindProperty(RadButtonElement.ImageProperty, this, RadButtonItem.ImageProperty, PropertyBindingOptions.TwoWay);
            this.actionButton.BindProperty(RadButtonElement.ImageKeyProperty, this, RadButtonItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);
            this.actionButton.SetValue(DropDownEditorLayoutPanel.IsContentProperty, true);
            this.actionButton.MouseLeave += new EventHandler(actionButton_MouseLeave);
            this.actionButton.MouseEnter += new EventHandler(actionButton_MouseEnter);
            this.actionButton.Click += new EventHandler(actionButton_Click);
            this.actionButton.NotifyParentOnMouseInput = true;

            this.layoutPanel = new DropDownEditorLayoutPanel();
            this.layoutPanel.BindProperty(DropDownEditorLayoutPanel.AutoSizeModeProperty, this, RadDropDownButtonElement.AutoSizeModeProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.Children.Add(this.arrowButton);
            this.layoutPanel.Children.Add(this.actionButton);
            DockLayoutPanel.SetDock(arrowButton, Dock.Right);
            DockLayoutPanel.SetDock(actionButton, Dock.Left);
            layoutPanel.LastChildFill = true;

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.borderPrimitive.Class = "DropDownButtonBorder";
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.layoutPanel);
        }

        protected virtual RadDropDownButtonPopup CreateDropDown()
        {
            RadDropDownButtonPopup dropDownMenu = new RadDropDownButtonPopup(this);

            if (this.IsInValidState(true))
            {
                dropDownMenu.ImageList = this.ElementTree.ComponentTreeHandler.ImageList;
            }

            return dropDownMenu;
        }

        #endregion

		#region Properties

		/// <summary>
		/// Gets the drop down menu
		/// </summary>
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadDropDownMenu DropDownMenu
		{
			get { return this.menu; }
		}

		/// <summary>
		/// Gets the arrow button
		/// </summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadArrowButtonElement ArrowButton
		{
			get
			{
				return this.arrowButton;
			}
		}

		/// <summary>
		/// Gets the action button
		/// </summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadButtonElement ActionButton
		{
			get
			{
				return this.actionButton;
			}
		}

		/// <summary>
		/// Gets or sets the minimum size of the arrow button
		/// </summary>
		public Size ArrowButtonMinSize
		{
			get
			{
				if (this.arrowButton != null)
					return this.arrowButton.MinSize;
				return Size.Empty;
			}
			set
			{
				if (this.arrowButton != null)
					this.arrowButton.MinSize = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating the position where the arrow button appears in drop-down button.
		/// </summary>
		[Browsable(true),		
		Category(RadDesignCategory.AppearanceCategory),
	    RadPropertyDefaultValue("ArrowPosition", typeof(DropDownEditorLayoutPanel)),
		Description("Gets or sets a value indicating the position where the arrow button appears in drop-down button.")]
		public virtual DropDownButtonArrowPosition ArrowPosition
		{
			get	{ return this.layoutPanel.ArrowPosition; }
			set	{ this.layoutPanel.ArrowPosition = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating the direction in which the dropdown item emerges from its parent container.
		/// </summary>
		[Browsable(true),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),		
	    Description("Gets or sets a value indicating the direction in which the dropdown item emerges from its parent container.")]
		public RadDirection DropDownDirection
		{
			get { return this.dropDownDirection; }
			set { this.dropDownDirection = value; }
		}

		/// <summary>
		/// Gets or sets the expand arrow button
		/// </summary>
		[Browsable(false),
		RadDefaultValue("ExpandArrow", typeof(DropDownEditorLayoutPanel))]
		public bool ExpandArrowButton
		{
			get
			{
				return this.layoutPanel.ExpandArrow;
			}
			set
			{
				this.layoutPanel.ExpandArrow = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the drop down is shown
		/// </summary>
		[Browsable(false)]
		public bool IsDropDownShown
		{
			get { return (bool)this.GetValue(RadDropDownButtonElement.IsDropDownShownProperty); }
		}

		/// <summary>
		/// Gets the Items collection where you can add and remove items from the
		/// DropDownButton.
		/// </summary>
		[RadEditItemsAction]
		[Category(RadDesignCategory.DataCategory),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.menu.Items;
			}
		}

		/// <summary>
		///		Indicates whether the DropDown of the button should have two columns or one column.
		/// </summary>
		[DefaultValue(false)]
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Indicates whether the DropDown of the button should have two columns or one column.")]
		public bool HasTwoColumnDropDown
		{
			get
			{
				return this.menu.IsTwoColumnMenu;
			}
			set
			{
				this.menu.IsTwoColumnMenu = value;
			}
		}

        /// <summary>
		/// Gets or sets a value indicating whether an arrow button is displayed on the drop-down buuton.
		/// </summary>
		[Browsable(true),
	    RadPropertyDefaultValue("ShowArrow", typeof(RadDropDownButtonElement)),		
		Category(RadDesignCategory.AppearanceCategory),		
		Description("Gets or sets a value indicating whether an arrow button is displayed on the drop-down button.")]		
		public virtual bool ShowArrow
		{
			get
			{				
				return (bool)this.GetValue(ShowArrowProperty);				
			}
			set
			{				
				this.SetValue(ShowArrowProperty, value);
			}
		}


		///<summary>
		/// Gets or sets the image that is displayed on a button element.
		/// </summary>		
		[RadPropertyDefaultValue("Image", typeof(RadButtonItem)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the image that is displayed on a button element."),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
		public virtual Image Image
		{
			get
			{
				return (Image)this.GetValue(RadButtonItem.ImageProperty);
			}
			set
			{
				this.SetValue(RadButtonItem.ImageProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the image list index value of the image displayed on the button control.
		/// </summary>
        [RadPropertyDefaultValue("ImageIndex", typeof(RadButtonItem)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the image list index value of the image displayed on the button control."),
		RefreshProperties(RefreshProperties.All),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
		public virtual int ImageIndex
		{
			get
			{
				return (int)this.GetValue(RadButtonItem.ImageIndexProperty);
			}
			set
			{
				this.SetValue(RadButtonItem.ImageIndexProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the key accessor for the image in the ImageList.
		/// </summary>
        [RadPropertyDefaultValue("ImageKey", typeof(RadButtonItem)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the key accessor for the image in the ImageList."),
		RefreshProperties(RefreshProperties.All),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.RadImageKeyConverterString)]
		public virtual string ImageKey
		{
			get
			{
				return (string)this.GetValue(RadButtonItem.ImageKeyProperty);
			}
			set
			{
				this.SetValue(RadButtonItem.ImageKeyProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the position of text and image relative to each other. 
		/// </summary>
        [RadPropertyDefaultValue("ImageTextRelation", typeof(RadButtonItem)),
        RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the position of text and image relative to each other.")]
		public virtual TextImageRelation TextImageRelation
		{
			get
			{
				return (TextImageRelation)this.GetValue(RadButtonItem.TextImageRelationProperty);
			}
			set
			{
				this.SetValue(RadButtonItem.TextImageRelationProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the alignment of image content on the drawing surface. 
		/// </summary>
        [RadPropertyDefaultValue("ImageAlignment", typeof(RadButtonItem)),
        RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the alignment of image content on the drawing surface.")]        
		public virtual System.Drawing.ContentAlignment ImageAlignment
		{
			get
			{
				return (ContentAlignment)this.GetValue(RadButtonItem.ImageAlignmentProperty);
			}
			set
			{
				this.SetValue(RadButtonItem.ImageAlignmentProperty, value);
			}
		}
		/// <summary>
		/// Gets or sets the alignment of text content on the drawing surface. 
		/// </summary>
        [RadPropertyDefaultValue("TextAlignment", typeof(RadButtonItem)),
        RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the alignment of text content on the drawing surface.")]
		public virtual System.Drawing.ContentAlignment TextAlignment
		{
			get
			{
				return (ContentAlignment)this.GetValue(RadButtonItem.TextAlignmentProperty);
			}
			set
			{
				this.SetValue(RadButtonItem.TextAlignmentProperty, value);
			}
		}

		/// <summary>
		/// Specifies the logical combination of image and text primitives in the element.
		/// </summary>
        [RadPropertyDefaultValue("DisplayStyle", typeof(RadButtonItem)),
        Browsable(true),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Specifies the logical combination of image and text primitives in the element.")]
		public DisplayStyle DisplayStyle
		{
			get
			{
				return (DisplayStyle)this.GetValue(RadButtonItem.DisplayStyleProperty);
			}
			set
			{
				this.SetValue(RadButtonItem.DisplayStyleProperty, value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the button item is in the pressed state.
		/// </summary>
		[ReadOnly(true),
		Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),		
		Description("Gets a value indicating whether the button item is in the pressed state.")]
		public bool IsPressed
		{
			get
			{
				return (bool)this.GetValue(IsPressedProperty);
			}
		}

		/// <summary>
		/// Gets the border element
		/// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadElement BorderElement
		{
			get
			{
				return this.borderPrimitive;
			}
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        #endregion

        #region Event handlers

        private void menu_DropDownOpening(object sender, CancelEventArgs e)
        {
            if (this.IsInValidState(true))
            {
                this.DropDownMenu.ImageList = this.ElementTree.ComponentTreeHandler.ImageList;
            }

            this.OnDropDownOpening(e);
        }

        private void menu_DropDownOpened(object sender, EventArgs e)
        {
            this.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, true);
            this.OnDropDownOpened(e);
        }

        private void menu_DropDownClosed(object sender, RadPopupClosedEventArgs e)
        {
            this.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, false);
            this.OnDropDownClosed(e);
        }

        private void menu_DropDownClosing(object sender, RadPopupClosingEventArgs args)
        {
            this.OnDropDownClosing(args);
        }

		private void arrowButton_MouseEnter(object sender, EventArgs e)
		{
			this.SetMouseOverState(DropDownButtonMouseOverState.OverArrowButton);			
		}
		
        private void arrowButton_MouseLeave(object sender, EventArgs e)
		{
			this.SetMouseOverState(DropDownButtonMouseOverState.None);			
		}

		private void arrowButton_Click(object sender, EventArgs e)
		{
			this.OnClick(EventArgs.Empty);
		}

		private void actionButton_MouseEnter(object sender, EventArgs e)
		{
			this.SetMouseOverState(DropDownButtonMouseOverState.OverActionButton);
		}

		private void actionButton_MouseLeave(object sender, EventArgs e)
		{
			this.SetMouseOverState(DropDownButtonMouseOverState.None);
		}

		private void actionButton_Click(object sender, EventArgs e)
		{
			this.OnClick(EventArgs.Empty);
		}

        protected override void DoClick(EventArgs e)
        {
            base.DoClick(e);

            this.ShowDropDown();
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == RadDropDownButtonElement.IsDropDownShownProperty)
			{
				foreach (RadElement child in this.ChildrenHierarchy)
				{
					child.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, e.NewValue);
				}
			}
			if (e.Property == RadDropDownButtonElement.ShowArrowProperty)
			{
				this.arrowButton.Visibility = (bool)e.NewValue ? ElementVisibility.Visible : ElementVisibility.Collapsed;
				this.InvalidateMeasure();
                this.InvalidateArrange();
			}
		}

        protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			IItemsElement buttonGroup = null;
			for (RadElement parent = this.Parent; parent != null && buttonGroup == null; parent = parent.Parent)
			{
                buttonGroup = parent as IItemsElement;
			}
			if (buttonGroup != null)
			{
				buttonGroup.ItemClicked(this);
			}
		}

        /// <summary>Determines whether the event is passed up in the control hierarchy.</summary>
        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);
            this.DoOnBubbleEvent(sender, args);
        }

		internal virtual void DoOnBubbleEvent(RadElement sender, RoutedEventArgs args)
		{
			if (args.RoutedEvent == RadElement.MouseDownEvent &&
				(sender == this.arrowButton || sender == this.actionButton))
			{
                if (this.menu == null)
                    return;

                if ( !this.menu.IsVisible && this.Items.Count > 0)
                {
                    this.ShowDropDown();
                }
                else if (this.menu.IsVisible)
                {
                    this.menu.ClosePopup(RadPopupCloseReason.Mouse);
                }
			}
		}

        #endregion

        #region Methods
        
        /// <summary>
		/// Shows the drop down menu at given location
		/// </summary>
		/// <param name="location">The upper left corner of the drop down in screen coordinates</param>
		public virtual void ShowDropDown(Point location)
		{
			Rectangle screenRect = RadPopupHelper.GetScreenRect(this);
			Rectangle popupRect = new Rectangle(location, this.menu.Size);
			Rectangle bounds = RadPopupHelper.EnsureBoundsInScreen(popupRect, screenRect);

            this.menu.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
			this.menu.Show(bounds.Location);
		}

		/// <summary>Shows the drop down menu.</summary>
		public virtual void ShowDropDown()
		{
            this.menu.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;

            if (this.IsDesignMode)
            {
                this.menu.Show(this, 0, RadDirection.Down);
            }
            else
            {
                this.menu.Show(this, 0, this.dropDownDirection);
            }
		}

		/// <summary>Hides the drop down menu.</summary>
		public virtual void HideDropDown()
		{
			this.menu.ClosePopup(RadPopupCloseReason.CloseCalled);
		}		
		
        #endregion

		#region Events
				
		/// <summary>
		/// Occurs when the drop-down is opening.
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory),
		Description("Occurs before the drop-down window appears.")]
		public event CancelEventHandler DropDownOpening
		{
			add
			{
				this.Events.AddHandler(RadDropDownButtonElement.DropDownOpeningEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadDropDownButtonElement.DropDownOpeningEventKey, value);
			}
		}

		/// <summary>
		/// Raises the DropDownOpening event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownOpening(CancelEventArgs e)
		{
            CancelEventHandler handler1 = (CancelEventHandler)this.Events[RadDropDownButtonElement.DropDownOpeningEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the drop-down has opened.
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory),
		Description("Occurs before the drop-down window appears.")]
		public event EventHandler DropDownOpened
		{
			add
			{
				this.Events.AddHandler(RadDropDownButtonElement.DropDownOpenedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadDropDownButtonElement.DropDownOpenedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the DropDownOpened event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownOpened(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadDropDownButtonElement.DropDownOpenedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the drop-down window has closed.
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory),
		Description("Occurs when the drop-down window has closed.")]
		public event EventHandler DropDownClosed
		{
			add
			{
				this.Events.AddHandler(RadDropDownButtonElement.DropDownClosedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadDropDownButtonElement.DropDownClosedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the DropDownClosed event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownClosed(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadDropDownButtonElement.DropDownClosedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        /// <summary>
        /// Occurs when the drop-down window is about to close.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the drop-down window is about to close.")]
        public event RadPopupClosingEventHandler DropDownClosing
        {
            add
            {
                this.Events.AddHandler(RadDropDownButtonElement.DropDownClosingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadDropDownButtonElement.DropDownClosingEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the drop-down of the button is about to close.
        /// </summary>
        /// <param name="args">An instance of the <see cref="Telerik.WinControls.UI.RadPopupClosingEventArgs"/>
        /// class that contains information about the event.</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownClosing(RadPopupClosingEventArgs args)
        {
            RadPopupClosingEventHandler handler1 = (RadPopupClosingEventHandler)this.Events[RadDropDownButtonElement.DropDownClosingEventKey];
            if (handler1 != null)
            {
                handler1(this, args);
            }
        }

		#endregion

		#region ISiteProvider

		public ISite GetSite()
		{
			ISite site = null;
			if (this.ElementTree != null && this.ElementTree.Control.Site != null)
			{
				site = this.ElementTree.Control.Site;
			}
			return site;
		}

		#endregion ISiteProvider

		#region IDropDownMenuOwner Members

		/// <summary>
		/// Gets or sets value indicating whether DropDownMenu will have the same class name as the owner control or its own.
		/// True means that the same class name will be used as the control that opened the dropdown.
		/// </summary>
		[DefaultValue(false)]
		public bool DropDownInheritsThemeClassName
		{
			get
			{
				return this.GetBitState(DropDownInheritThemeClassNameStateKey);
			}
			set
			{
                this.SetBitState(DropDownInheritThemeClassNameStateKey, value);
			}
		}

	    bool IDropDownMenuOwner.ControlDefinesThemeForElement(RadElement element)
	    {
            return false;
	    }

	    #endregion

        private void SetMouseOverState(DropDownButtonMouseOverState state)
        {
            this.SetValue(RadDropDownButtonElement.MouseOverStateProperty, state);

            this.arrowButton.SetValue(MouseOverStateProperty, state);
            foreach (RadElement child in this.arrowButton.ChildrenHierarchy)
            {
                child.SetValue(RadDropDownButtonElement.MouseOverStateProperty, state);
            }
            this.actionButton.SetValue(MouseOverStateProperty, state);
            foreach (RadElement child in this.actionButton.ChildrenHierarchy)
            {
                child.SetValue(RadDropDownButtonElement.MouseOverStateProperty, state);
            }
            this.borderPrimitive.SetValue(RadDropDownButtonElement.MouseOverStateProperty, state);
        }
    }
}

