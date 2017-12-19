using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layout;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a menu. It may be nested in other telerik controls. RadMenu can be
    /// horizontal or vertical. You can add, remove, and disable menu items at run-time. It 
    /// offers full support for the Telerik RadControls 
    /// for WinForm theming engine, allowing you to easily construct a variety of stunning 
    /// visual effects. You can nest any other RadControl within a RadMenu
    /// . For example, you can create a menu with an embedded 
    /// textbox or combobox.
    /// </summary>
	public class RadMenuElement : RadItem, ITooltipOwner
	{
        // Fields
        private WrapLayoutPanel layoutPanel;
        private RadItemOwnerCollection items;
        private RadItem contextItem;
        private bool allowMerge = true;
        private StackLayoutPanel systemButtons;
        private RadImageButtonElement minimizeButton;
        private RadImageButtonElement maximizeButton;
        private RadImageButtonElement closeButton;
        private FillPrimitive fill;
        private BorderPrimitive border;

        static RadMenuElement()
        {
            new Themes.ControlDefault.Menu().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadMenuItemBase) };
            this.items.DefaultType = typeof(RadMenuItem);
            this.items.ItemsChanged += new ItemChangedDelegate(this.ItemsChanged);
        }

        protected override void CreateChildElements()
        {
            // if there is an old reference we clear it.
            this.contextItem = null;

            this.layoutPanel = new WrapLayoutPanel();
            this.layoutPanel.ZIndex = 10;
            this.layoutPanel.BindProperty(WrapLayoutPanel.OrientationProperty, this, RadMenuElement.OrientationProperty, PropertyBindingOptions.OneWay);
            //todo:
            //this.layoutPanel.BindProperty(WrapLayoutPanel.Eql, this, RadMenuElement.AllItemsEqualHeightProperty, PropertyBindingOptions.OneWay);

            this.items.Owner = this.layoutPanel;
            this.Children.Add(this.layoutPanel);

            fill = new FillPrimitive();
            fill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            fill.Visibility = ElementVisibility.Collapsed;
            fill.Class = "MenuFill";
            fill.ZIndex = 5;
            this.Children.Add(fill);

                        
            this.systemButtons = new StackLayoutPanel();
            this.systemButtons.Alignment = ContentAlignment.MiddleRight;
            this.systemButtons.ZIndex = 0;
            this.systemButtons.Class = "SystemButtonsStackLayoutPanel";
            this.minimizeButton = new RadImageButtonElement();
            this.minimizeButton.StretchHorizontally = false;
            this.minimizeButton.StretchVertically = false;
            this.minimizeButton.Class = "MinimizeButton";
            this.minimizeButton.ButtonFillElement.Name = "MinimizeButtonFill";
            this.systemButtons.Children.Add(minimizeButton);

            this.maximizeButton = new RadImageButtonElement();
            this.maximizeButton.StretchHorizontally = false;
            this.maximizeButton.StretchVertically = false;
            this.maximizeButton.Class = "MaximizeButton";
            this.maximizeButton.ButtonFillElement.Name = "MaximizeButtonFill";
            this.systemButtons.Children.Add(maximizeButton);

            this.closeButton = new RadImageButtonElement();
            this.closeButton.StretchHorizontally = false;
            this.closeButton.StretchVertically = false;
            this.closeButton.Class = "CloseButton";
            this.closeButton.ButtonFillElement.Name = "CloseButtonFill";
            this.systemButtons.Children.Add(closeButton);
            this.systemButtons.ZIndex = 6;
            this.Children.Add(systemButtons);


            this.border = new BorderPrimitive();
            border.Class = "MenuBorder";
            border.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.Children.Add(border);

            systemButtons.Visibility = ElementVisibility.Collapsed;
        }
        
        #region Events

        /// <summary>
		///		Occurs when the menu Orientation property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the menu Orientation property value changes.")]
		public event EventHandler OrientationChanged;

		/// <summary>
		///		Occurs when the menu AllItemsEqualHeight property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the menu AllItemsEqualHeight property value changes.")]
		public event EventHandler AllItemsEqualHeightChanged;

        #endregion

        #region Properties

        public RadImageButtonElement MinimizeButton
        {
            get { return minimizeButton; }
            set { minimizeButton = value; }
        }
        public RadImageButtonElement MaximizeButton
        {
            get { return maximizeButton; }
            set { maximizeButton = value; }
        }
        public RadImageButtonElement CloseButton
        {
            get { return closeButton; }
            set { closeButton = value; }
        }

        public StackLayoutPanel SystemButtons
        {
            get { return systemButtons; }
            set { systemButtons = value; }
        }

        /// <summary>Gets or sets the context items.</summary>
        public RadMenuItemBase ContextItem
        {
            get
            {
                return this.contextItem as RadMenuItemBase;
            }
            set
            {                
                this.contextItem = value as RadMenuItemBase;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="WrapLayoutPanel"/>class
        /// that represents the layout panel in which the menu items reside.
        /// </summary>
        [Browsable(false)]
        public WrapLayoutPanel ItemsLayout
        {
            get
            {
                return this.layoutPanel;
            }
        }

        /// <summary>
        /// 	<para>
        ///         Gets all root menu items (see <see cref="RadMenuItemBase"/> for more
        ///         information about menu items).
        ///     </para>
        /// </summary>
        /// <remarks>
        /// 	<para>Root menu items are these menu items that can be displayed in the menu when
        ///     nothing is dropped down.</para>
        /// 	<para>
        ///         Menu items are hierarchical items - they have a parent item and a list of children
        ///         items. Children items are menu items that can be dropped down as submenu of
        ///         their parent. The difference between the root and the non-root menu items is that
        ///         root items have no parent item (the property
        ///         <see cref="RadMenuItemBase.IsRootItem"/> can be used to check if an item is a
        ///         root one).
        ///     </para>
        /// 	<para>Note that <strong>Items</strong> contains all root menu items, not just the
        ///     items that are displayed. An item remains in the Items collection even if it is an
        ///     overflow item and is therefore not currently visible.</para>
        /// </remarks>
        /// <seealso cref="RadMenuItemBase">RadMenuItemBase Class</seealso>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [RadNewItem("Type here", true)]
        public RadItemOwnerCollection Items
        {
            get
            {
                //this.EnsureChildElements();
                return this.items;
            }
        }

        [Category(RadDesignCategory.BehaviorCategory),
        DefaultValue(true),
        Description("Allow Merge")]
        public bool AllowMerge
        {
            get
            {
                return this.allowMerge;
            }
            set
            {
                if (this.allowMerge != value)
                {
                    this.allowMerge = value;
                }
            }
        }

        public static RadProperty OrientationProperty = RadProperty.Register(
            "Orientation", typeof(Orientation), typeof(RadMenuElement), new RadElementPropertyMetadata(
                Orientation.Horizontal));

        /// <summary>
        ///     Gets or sets the
        ///     <see cref="System.Windows.Forms.Orientation">orientation</see> of menu
        ///     items - Horizontal or Vertical.
        /// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets the orientation of menu items. Horizontal or Vertical.")]
		[RadPropertyDefaultValue("Orientation", typeof(RadMenuElement))]
		public Orientation Orientation
		{
			get
			{
				return (Orientation) this.GetValue(OrientationProperty);
			}
			set
			{
				this.SetValue(OrientationProperty, value);
			}
		}

        public static RadProperty AllItemsEqualHeightProperty = RadProperty.Register(
            "AllItemsEqualHeight", typeof(bool), typeof(RadMenuElement), new RadElementPropertyMetadata(
                false));
		
		/// <summary>
		///		Gets or sets whether all items will appear with the same size (the size of the highest item in the collection).
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets whether all items will appear with the same size (the size of the highest item in the collection).")]
		[RadPropertyDefaultValue("AllItemsEqualHeight", typeof(RadMenuElement))]
		public bool AllItemsEqualHeight
		{
			get
			{
				return (bool) this.GetValue(AllItemsEqualHeightProperty);
			}
			set
			{
				this.SetValue(AllItemsEqualHeightProperty, value);
			}
		}

        public static RadProperty DropDownAnimationEnabledProperty = RadProperty.Register(
            "DropDownAnimationEnabled", typeof(bool), typeof(RadMenuElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

		/// <summary>
		///		Gets or sets a value indicating whether the DropDown animation will be enabled when it shows.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadPropertyDefaultValue("DropDownAnimationEnabled", typeof(RadMenuElement))]
		[Description("Gets or sets a value indicating whether the DropDown animation will be enabled when it shows.")]
		public bool DropDownAnimationEnabled
		{
			get
			{
				return (bool)this.GetValue(DropDownAnimationEnabledProperty);
			}
			set
			{
				this.SetValue(DropDownAnimationEnabledProperty, value);
			}
		}

        public static RadProperty DropDownAnimationEasingProperty = RadProperty.Register(
            "DropDownAnimationEasing", typeof(RadEasingType), typeof(RadMenuElement), new RadElementPropertyMetadata(
                RadEasingType.Linear, ElementPropertyOptions.None));

		/// <summary>
		///		Gets or sets the type of the DropDown animation.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadPropertyDefaultValue("DropDownAnimationEasing", typeof(RadMenuElement))]
		[Description("Gets or sets the type of the DropDown animation.")]
		public RadEasingType DropDownAnimationEasing
		{
			get
			{
				return (RadEasingType)this.GetValue(DropDownAnimationEasingProperty);
			}
			set
			{
				this.SetValue(DropDownAnimationEasingProperty, value);
			}
		}

        public static RadProperty DropDownAnimationFramesProperty = RadProperty.Register(
            "DropDownAnimationFrames", typeof(int), typeof(RadMenuElement), new RadElementPropertyMetadata(
                4, ElementPropertyOptions.None));

		/// <summary>
		///		Gets or sets the number of frames that will be used when the DropDown is being animated.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadPropertyDefaultValue("DropDownAnimationFrames", typeof(RadMenuElement))]
		[Description("Gets or sets the number of frames that will be used when the DropDown is being animated.")]
		public int DropDownAnimationFrames
		{
			get
			{
				return (int)this.GetValue(DropDownAnimationFramesProperty);
			}
			set
			{
				this.SetValue(DropDownAnimationFramesProperty, value);
			}
		}

        /// <summary>
        /// Gets an instance of the <see cref="FillPrimitive"/>class
        /// that represents the menu background fill.
        /// </summary>
        [Browsable(false)]
        public FillPrimitive MenuFill
        {
            get
            {
                return this.fill;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="BorderPrimitive"/>class
        /// that represents the border of the menu.
        /// </summary>
        [Browsable(false)]
        public BorderPrimitive MenuBorder
        {
            get
            {
                return this.border;
            }
        }

        #endregion

        #region Event handlers

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadMenuElement.OrientationProperty)
            {
                SetItemsRotationDegree((Orientation)e.NewValue);
                OnOrientationChanged(EventArgs.Empty);
            }
            if (e.Property == RadMenuElement.AllItemsEqualHeightProperty)
            {
                OnAllItemsEqualHeightChanged(EventArgs.Empty);
            }
            base.OnPropertyChanged(e);
        }

        protected override void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnTunnelEvent(sender, args);

            if (args.RoutedEvent == RootRadElement.AutoSizeChangedEvent)
            {
                if (this.ElementTree == null || typeof(RadComboBox).IsAssignableFrom(this.ElementTree.Control.GetType()))
                {
                    AutoSizeEventArgs eventArgs = (AutoSizeEventArgs)args.OriginalEventArgs;
                    if (eventArgs.AutoSize)
                    {
                        this.layoutPanel.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
                        this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
                    }
                    else
                    {
                        this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
                        this.layoutPanel.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
                    }
                }
            }
        }

        /// <summary>
		///		Raises the <see cref="OrientationChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnOrientationChanged(EventArgs args)
		{
			if (this.OrientationChanged != null)
			{
				this.OrientationChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="AllItemsEqualHeightChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnAllItemsEqualHeightChanged(EventArgs args)
		{
			if (this.AllItemsEqualHeightChanged != null)
			{
				this.AllItemsEqualHeightChanged(this, args);
			}
		}

        private void ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted ||
                operation == ItemsChangeOperation.Set)
            {
                target.AngleTransform = GetRotationDegree(this.Orientation);
            }

            RadMenuItemBase menuItem = target as RadMenuItemBase;
            if (menuItem != null)
            {
                if (operation == ItemsChangeOperation.Inserted || operation == ItemsChangeOperation.Set)
                {
                    menuItem.Owner = this;
                    if (!this.IsDesignMode)
                    {
                        menuItem.ClickMode = ClickMode.Press;
                    }
                    menuItem.IsMainMenuItem = true;
                }
                else if (operation == ItemsChangeOperation.Removed)
                {
                    menuItem.Deselect();
                    menuItem.Owner = null;
                    if (menuItem.IsPopupShown)
                    {
                        menuItem.HideChildItems();
                    }
                }
            }
        }

        #endregion

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            //fill.Arrange(new RectangleF(0, 0, finalSize.Width, finalSize.Height));
            //layoutPanel.Arrange(new RectangleF( 0, 0, finalSize.Width, finalSize.Height));
            base.ArrangeOverride(finalSize);
            RectangleF systemButtonsRectangle = new RectangleF(finalSize.Width - systemButtons.DesiredSize.Width - 3, 
                                                               0, 
                                                               systemButtons.DesiredSize.Width, 
                                                               systemButtons.DesiredSize.Height);
            if (this.RightToLeft)
            {
                systemButtonsRectangle.X = 3;
            }
            systemButtons.Arrange(systemButtonsRectangle);
            return finalSize; 
        } 
        

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF size = base.MeasureOverride(availableSize);
            size.Width += this.Padding.Horizontal;
            size.Height += this.Padding.Vertical;
            return size;
        }

		private void SetItemsRotationDegree(Orientation orientation)
		{
			float degree = GetRotationDegree(orientation);

			this.SuspendLayout();
			foreach (RadItem item in this.Items)
			{
				item.AngleTransform = degree;
			}
			this.ResumeLayout(true);
		}

		private float GetRotationDegree(Orientation orientation)
		{
			float degree = 0f;

			if (orientation == Orientation.Horizontal)
				degree = 0f;
			else
				degree = 90f;

			return degree;
		}

        #region ITooltipOwner
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
        #endregion
    }
}
