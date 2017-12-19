using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI.TabStrip;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines the possible modes of tab items that overflow the available layout ares in a RadTabStrip.
    /// </summary>
    public enum TabStripItemOverflowMode
    {
        /// <summary>
        /// No action is performed.
        /// </summary>
        None,
        /// <summary>
        /// Each item that exceeds layout area is hidden.
        /// </summary>
        Hide,
        /// <summary>
        /// Each item proportionally decreases its size to accomodate the less space.
        /// </summary>
        Shrink,
    }

	/// <summary>
	/// Defines the possible positions of the drop down button in the RadTabStrip.
	/// </summary>
	public enum TabItemsDropDownButtonPosition
	{
		/// <summary>
		/// Button will be located on the left in the case of horizontal tabs flow or at the top in the case of vertical tabs mode.
		/// </summary>
		LeftTop,
		/// <summary>
		/// Button will be located on the right in the case of horizontal tabs flow or at the bottom in the case of vertical tabs mode.
		/// </summary>
		RightBottom
	}

	/// <summary>
	/// Defines the possible positions of the scroll buttons in the RadTabStrip.
	/// </summary>
	public enum TabScrollButtonsPosition
	{
		/// <summary>
		/// The scroll buttons will be located on the left in the case of horizontal tabs flow and at the top in the case of vertical tabs mode.
		/// </summary>
		LeftTop,
		/// <summary>
		/// Scroll buttons will be located on the right in the case of horizontal tabs flow and at the bottom in the case of vertical tabs mode.
		/// </summary>
		RightBottom,
		/// <summary>
		/// Buttons will be located at each side of the strip.
		/// </summary>
		BothSides
	}

	/// <summary>
	///		Defines the possible ways for scrolling the TabStrip items using the scroll buttons.
	/// </summary>
	public enum TabStripScrollStyle
    {
		/// <summary>
		/// Each time a scroll button is pressed the tab items are scrolled by one item.
		/// </summary>
		ScrollByItem,
		/// <summary>
		/// Each time a scroll button is pressed the tab items are scrolled by a certain step.
		/// </summary>
		ScrollByStep,
	}

    /// <summary>
    /// Represents a tab strip element. The RadTabStrip class is a simple wrapper for 
    /// the RadTabStripElement class. The RadTabStrip acts to transfer events to and from 
    /// its corresponding RadTabStripElement instance. The RadTabStripElement which is 
    /// essentially the RadTabStrip control may be nested in other telerik controls.
    /// </summary>
	[ToolboxItem(false), ComVisible(false)]
	public class RadTabStripElement : RadItem
    {
        #region BitState Keys

        internal const ulong OffsetScrollingStateKey = RadItemLastStateKey << 1;
        internal const ulong AutoScrollStateKey = OffsetScrollingStateKey << 1;
        internal const ulong DraggingStateKey = AutoScrollStateKey << 1;
        internal const ulong ShowOverFlowButtonStateKey = DraggingStateKey << 1;
        internal const ulong IsRealDragStateKey = ShowOverFlowButtonStateKey << 1;
        internal const ulong AllowSelectionStateKey = IsRealDragStateKey << 1;
        internal const ulong AllowEditStateKey = AllowSelectionStateKey << 1;
        internal const ulong IsUsedInDockingStateKey = AllowEditStateKey << 1;
        internal const ulong AllowDragingInDockingStateKey = IsUsedInDockingStateKey << 1;

        #endregion

        private RadItemOwnerCollection items;
		private FillPrimitive tabContentFill;
		private TabLayoutPanel tabLayout;
        private TabStripItemOverflowMode overflowMode;

		private TabItem draggedItem;
		private TabItem replacedItem;

		internal TabStripElementScrolling scrollingManager;
		private ItemsOverflowDropDown overflowManager = new ItemsOverflowDropDown();

		private Point initialMousePosition = Point.Empty;

		private Point currentPosition;
		private int scrollOffsetStep = 5;
        private int overflowOffset;
		
		private PositionPointer upPointer1;
		private PositionPointer downPointer1;
		private System.Windows.Forms.Form outlineForm;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[AllowSelectionStateKey] = true;

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;

            this.scrollingManager = new TabStripElementScrolling(this);
            this.overflowMode = TabStripItemOverflowMode.None;

            this.items = this.CreateItemsCollection();
            this.items.ItemsChanged += new ItemChangedDelegate(this.ItemsChanged);
        }

        virtual protected RadItemOwnerCollection CreateItemsCollection()
        {
            RadItemOwnerCollection collection = new RadItemOwnerCollection();
            collection = new RadItemOwnerCollection();
            collection.ItemTypes = new Type[] { typeof(TabItem) };
#pragma warning disable 0618
			collection.ExcludedTypes = new Type[] { typeof(RibbonTab),  typeof(RadRibbonBarCommandTab) };
#pragma warning restore 0618
			return collection;
        }
     

		static RadTabStripElement()
		{
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new TabStripElementStateManager(), typeof(RadTabStripElement));
            new Themes.ControlDefault.TabStrip().DeserializeTheme();
        }

		public static RoutedEvent OnRoutedTabHovered = RadTabStripElement.RegisterRoutedEvent("OnRoutedTabHovered", typeof(RadTabStripElement));
		public static RoutedEvent OnRoutedTabSelected = RadTabStripElement.RegisterRoutedEvent("OnRoutedTabSelected", typeof(RadTabStripElement));
		public static RoutedEvent OnTabDeselected = RadTabStripElement.RegisterRoutedEvent("OnTabDeselected", typeof(RadTabStripElement));
		public static RoutedEvent OnTabUnHovered = RadTabStripElement.RegisterRoutedEvent("OnTabUnHovered", typeof(RadTabStripElement));

		/// <summary>
		///		Occurs when the user clicks a tab item which is not selected.
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
		[Description("Occurs when the user clicks a tab item which is not selected.")]
		public event TabEventHandler TabSelected;
		/// <summary>
		///		Occurs when the mouse pointer is over a tab item.
		/// </summary>
		[Category(RadDesignCategory.MouseCategory)]
		[Description("Occurs when the mouse pointer is over a tab item.")]
		public event TabEventHandler TabHovered;
		/// <summary>
		///		Occurs when a tab item is clicked.
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
		[Description("Occurs when a tab item is clicked.")]
		public event TabEventHandler TabClicked;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsDragging
        {
            get
            {
                return this.GetBitState(DraggingStateKey);
            }
        }

		public event TabEventHandler TabDragIsOutsideElement;

		/// <summary>
		///		Occurs when the user starts dragging a tab item. The event occurs just before the appearance of the dragged item as a floating element.
		/// </summary>
		[Category(RadDesignCategory.DragDropCategory)]
		[Description("Occurs when the user starts dragging a tab item. The event occurs just before the appearance of the dragged item as a floating element.")]
		public event TabDragCancelEventHandler TabDragStarting;
		/// <summary>
		///		Occurs when the user starts dragging a tab item. The event occurs just after the appearance of the dragged item as a floating element.
		/// </summary>
		[Category(RadDesignCategory.DragDropCategory)]
		[Description("Occurs when the user starts dragging a tab item. The event occurs just after the appearance of the dragged item as a floating element.")]
		public event TabDragEventHandler TabDragStarted;

		/// <summary>
		///		Occurs when the user starts dragging a tab item. The event occurs just after the appearance of the dragged item as a floating element.
		/// </summary>
		[Category(RadDesignCategory.ActionCategory)]
		[Description("Occurs when the user starts selecting a tab item. The event occurs just after the mouse button is down on the tab item")]
		public event TabCancelEventHandler TabSelecting;

		/// <summary>
		///		Occurs when the user ends dragging a tab item. The event occurs just before the tab item is positioned on its new place.
		/// </summary>
		[Category(RadDesignCategory.DragDropCategory)]
		[Description("Occurs when the user ends dragging a tab item. The event occurs just before the tab item is positioned on its new place.")]
		public event TabDragCancelEventHandler TabDragEnding;
		/// <summary>
		///		Occurs when the user ends dragging a tab item. The event occurs just after the tab item is positioned on its new place.
		/// </summary>
		[Category(RadDesignCategory.DragDropCategory)]
		[Description("Occurs when the user ends dragging a tab item. The event occurs just after the tab item is positioned on its new place.")]
		public event TabDragEventHandler TabDragEnded;

		/// <summary>
		///		Occurs when the TabsPosition property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the TabsPosition property value changes.")]
		public event EventHandler TabsPositionChanged;

		/// <summary>
		///		Occurs when the AllTabsEqualHeight property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the AllTabsEqualHeight property value changes.")]
		public event EventHandler AllTabsEqualHeightChanged;

        public static RadProperty SelectedItemMarginsProperty = RadProperty.Register(
            "SelectedItemMargins",
            typeof(Padding),
            typeof(RadTabStripElement),
            new RadElementPropertyMetadata(Padding.Empty, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ItemMarginsProperty = RadProperty.Register(
            "ItemMargins", 
            typeof(Padding),
            typeof(RadTabStripElement),
            new RadElementPropertyMetadata(Padding.Empty, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

		public static RadProperty IsHoveredProperty = RadProperty.Register(
			"IsHovered", typeof(bool), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.CanInheritValue));


        public static RadProperty SeparatorColorProperty = RadProperty.Register(
            "SeparatorColor", typeof(Color), typeof(RadTabStripElement), new RadElementPropertyMetadata(
                Color.FromArgb(255,118, 154, 204), ElementPropertyOptions.None));


		public static RadProperty AllowDragDropProperty = RadProperty.Register(
			"AllowDragDrop", typeof(bool), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.None));

		
		public static RadProperty IsSelectedProperty = RadProperty.Register(
			"IsSelected", typeof(bool), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.CanInheritValue));

		public static RadProperty TabStripScrollStyleProperty = RadProperty.Register(
			"TabScrollStyle", typeof(TabStripScrollStyle), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				TabStripScrollStyle.ScrollByItem));

		public static RadProperty ShrinkModeProperty = RadProperty.Register(
			"ShrinkMode", typeof(bool), typeof(RadTabStripElement), new RadElementPropertyMetadata(
			false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));
    
	

		public static RadProperty ItemsOffsetProperty = RadProperty.Register(
			"ItemsOffset", typeof(int), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				30));

		public static RadProperty ScrollItemsOffsetProperty = RadProperty.Register(
			"ScroollItemsOffset", typeof(int), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				0));

		public static RadProperty TabsPositionProperty = RadProperty.Register(
			"TabsPosition", typeof(TabPositions), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				TabPositions.Top, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

		public static RadProperty TabScrollButtonsPositionProperty = RadProperty.Register(
			"TabScrollButtonsPosition", typeof(TabScrollButtonsPosition), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				TabScrollButtonsPosition.RightBottom, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty TabItemsDropDownButtonPositionProperty = RadProperty.Register(
			"TabDropDownButtonPosition", typeof(TabItemsDropDownButtonPosition), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				TabItemsDropDownButtonPosition.RightBottom, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty AllTabsEqualHeightProperty = RadProperty.Register(
			"AllTabsEqualHeight", typeof(bool), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				true));

		public static RadProperty StretchBaseModeProperty = RadProperty.Register(
			"StretchBaseMode", typeof(TabBaseStretchMode), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				TabBaseStretchMode.Default));

		public static RadProperty FitToAvailableWidthProperty = RadProperty.Register(
			"FitToAvailableWidth", typeof(bool), typeof(RadTabStripElement), new RadElementPropertyMetadata(
				false));

        /// <summary>Gets a value indicating whether the inner layout is affected.</summary>
		public override bool AffectsInnerLayout
		{
			get
			{
				return true;
			}
		}

        public TabStripElementScrolling ScrollingManager
        {
            get
            {
                return this.scrollingManager;
            }
        }

        /// <summary>
        /// Gets or sets the margins to be applied to each item.
        /// Margins are rotated accordingly when TabsPosition property changes.
        /// It is assumed that the supplied value is valid for TabPositions.Top.
        /// </summary>
        [VsbBrowsable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding ItemMargins
        {
            get
            {
                return (Padding)this.GetValue(ItemMarginsProperty);
            }
            set
            {
                this.SetValue(ItemMarginsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the margins to be applied to the currently selected item.
        /// Margins are rotated accordingly when TabsPosition property changes.
        /// It is assumed that the supplied value is valid for TabPositions.Top.
        /// </summary>
        [VsbBrowsable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding SelectedItemMargins
        {
            get
            {
                return (Padding)this.GetValue(SelectedItemMarginsProperty);
            }
            set
            {
                this.SetValue(SelectedItemMarginsProperty, value);
            }
        }

		/// <summary>
		///Gets or sets a value indicating whether the user can edit the text of  tab items.
		/// </summary>        
		[Category("Behavior"), DefaultValue(false)]
		[Description("Gets or sets a value indicating whether the user can edit the text of tab items.")]
		public bool AllowEdit
		{
			get
			{
				return this.GetBitState(AllowEditStateKey);
			}
			set
			{
                this.SetBitState(AllowEditStateKey, value);
			}
		}
		/// <summary>
		/// Defines a shrink behavior of tabItems
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Defines a shrink behavior of tabs in RadTabStrip")]
		[DefaultValue(false)]
		public bool ShrinkMode
		{
			get
			{
				return (bool)this.GetValue(ShrinkModeProperty);
			}
			set
			{
				this.SetValue(ShrinkModeProperty, value);
                if (!this.UseNewLayoutSystem)
                {
                    if (!value)
                    {
                        foreach (TabItem item in this.Items)
                        {
                            item.TextPrimitive.AutoSize = true;
                        }
                    }
                }
			}
		}

        /// <summary>
        /// Gets or sets the color of the separators
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the color of the separators")]
        public Color SeparatorColor
        {
            get
            {
                return (Color)this.GetValue(SeparatorColorProperty);
            }
            set
            {
                this.SetValue(SeparatorColorProperty, value);
            }
        }

		/// <summary>
		///		Gets or sets whether the AutoScroll functionality of the TabStrip element will be allowed.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets whether the AutoScroll functionality of the TabStrip element will be allowed.")]
		[DefaultValue(false)]
		public bool AutoScroll
		{
			get
			{
				return this.GetBitState(AutoScrollStateKey);
			}
			set
			{
                this.SetBitState(AutoScrollStateKey, value);
			}
		}

		/// <summary>
		///		Gets or sets a value indicating whether the drag and drop functionality 
        ///     of the TabStrip element is allowed.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets whether the drag and drop functionality of the TabStrip element will be allowed.")]
		[DefaultValue(false)]
		public bool AllowDragDrop
		{
			get
			{
				return (bool)this.GetValue(AllowDragDropProperty);
			}
			set
			{
				this.SetValue(AllowDragDropProperty, value);
			
			}
		}

		/// <summary>
		///	Retrieves the currently selected tab element. Could be TabItem or any 
        /// other RadElement which has been placed directly in the Items collection 
        /// (for example <see cref="Telerik.WinControls.UI.RadButtonElement">
        /// RadButtonElement</see>).
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadElement SelectedTab
		{
			get
			{
				foreach (RadElement child in this.Items)
				{
					if (this.GetIsSelected(child))
					{
						return child;
					}
				}

				return null;
			}
			set
			{
                if (SelectedTab == value)
                {
                    return;
                }

				SetSelectedTab(value);
			}
		}

		/// <summary>
		///		Gets a collection of items which are children of the TabStrip element.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[RadNewItem("Type here", true)]
		[Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
		[Description("Gets the collection of items which are children of the TabStrip element.")]
		public virtual RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

		/// <summary>
		///		Retrieves the element which represents the base area of the TabStrip. 
        ///     By default this is a FillPrimitive which either wraps around the elements 
        ///     placed in it or utilizes the available space left after the tab items' 
        ///     sizes have been calculated.
		/// </summary>
		[Browsable(false)]
		public virtual RadElement TabContent
		{
			get
			{
				return this.tabContentFill;
			}
		}



		/// <commentsfrom cref="TabLayoutPanel.TabDropDownButtonPosition" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TabDropDownButtonPosition", typeof(TabLayoutPanel))]
		[RadPropertyDefaultValue("TabDropDownButtonPosition", typeof(RadTabStripElement))]
		public TabItemsDropDownButtonPosition TabDropDownButtonPosition
		{
			get
			{
				return (TabItemsDropDownButtonPosition)this.GetValue(TabItemsDropDownButtonPositionProperty);
			}
			set
			{
				this.SetValue(TabItemsDropDownButtonPositionProperty, value);
			}
		}

		/// <commentsfrom cref="TabLayoutPanel.TabDropDownButtonPosition" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TabScrollButtonsPosition", typeof(TabLayoutPanel))]
		[RadPropertyDefaultValue("TabDropDownButtonPosition", typeof(RadTabStripElement))]
		public TabScrollButtonsPosition TabScrollButtonsPosition
		{
			get
			{
				return (TabScrollButtonsPosition)this.GetValue(TabScrollButtonsPositionProperty);
			}
			set
			{
				this.SetValue(TabScrollButtonsPositionProperty, value);
			}
		}

		/// <summary>
		///		Gets or sets whether the overflow button should be visible.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets whether the overflow button should be visible or not.")]
		[DefaultValue(false)]
		public bool ShowOverFlowButton
		{
			get
			{
				return this.GetBitState(ShowOverFlowButtonStateKey);
			}
			set
			{
                this.SetBitState(ShowOverFlowButtonStateKey, value);
			}
		}

		/// <commentsfrom cref="TabLayoutPanel.TabsPosition" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TabsPosition", typeof(TabLayoutPanel))]
		[RadPropertyDefaultValue("TabsPosition", typeof(RadTabStripElement))]
		public TabPositions TabsPosition
		{
			get
			{
				return (TabPositions)this.GetValue(TabsPositionProperty);
			}
			set
			{
				this.SetValue(TabsPositionProperty, value);
			}
		}

		/// <summary>
		///		Defines the way the scrolling of the tab items is done.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Defines the way the scrolling of the tab items is done.")]
		[RadPropertyDefaultValue("TabScrollStyle", typeof(RadTabStripElement))]
		public TabStripScrollStyle TabScrollStyle
		{
			get
			{
				return (TabStripScrollStyle)this.GetValue(TabStripScrollStyleProperty);
			}
			set
			{
				this.SetValue(TabStripScrollStyleProperty, value);
			}
		}

		/// <summary>
		///		Defines the number of pixels with which the scrolling advances each time a scroll button is pressed.
		/// </summary>
		/// <remarks>Setting this property has effect only when the <see cref="TabScrollStyle"/> property is set to ScrollByStep.</remarks>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Defines the number of pixels with which the scrolling advances each time a scroll button is pressed.")]
		[DefaultValue(8)]
		public int ScrollOffsetStep
		{
			get
			{
				return scrollOffsetStep;
			}
			set
			{
				scrollOffsetStep = value;
			}
		}

		/// <commentsfrom cref="TabLayoutPanel.AllTabsEqualHeight" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("AllTabsEqualHeight", typeof(TabLayoutPanel))]
		[RadPropertyDefaultValue("AllTabsEqualHeight", typeof(RadTabStripElement))]
		public bool AllTabsEqualHeight
		{
			get
			{
				return (bool)this.GetValue(AllTabsEqualHeightProperty);
			}
			set
			{
				this.SetValue(AllTabsEqualHeightProperty, value);
			}
		}

		

		/// <commentsfrom cref="TabLayoutPanel.ItemsOffset" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("ItemsOffset", typeof(TabLayoutPanel))]
		[RadPropertyDefaultValue("ItemsOffset", typeof(RadTabStripElement))]
		public int ItemsOffset
		{
			get
			{
				return (int)this.GetValue(ItemsOffsetProperty);
			}
			set
			{
				this.SetValue(ItemsOffsetProperty, value);
			}
		}
		
		[Browsable(false)]
		public bool IsUsedInDocking
		{
			get
			{
				return this.GetBitState(IsUsedInDockingStateKey);
			}
			set
			{
                this.SetBitState(IsUsedInDockingStateKey, value);
			}
		}

		[Browsable(false)]
		public bool AllowDragingInDocking
		{
			get
			{
				return this.GetBitState(AllowDragingInDockingStateKey);
			}
			set
			{
                this.SetBitState(AllowDragingInDockingStateKey, value);
			}
		}

        //TODO: This behavior should be revisited. It is a temp workaround for the RadDock Q2 2009 document strip
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int OverflowOffset
        {
            get
            {
                return this.overflowOffset;
            }
            set
            {
                if (this.overflowOffset == value)
                {
                    return;
                }

                this.overflowOffset = value;
                this.InvalidateMeasure();
                this.InvalidateArrange();
            }
        }

        /// <summary>
        /// Gets or sets the overflow behavior of rad tab strip
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the overflow behavior of rad tab strip")]
        [DefaultValue(OverFlowBehavior.BringIntoView)]
        public OverFlowBehavior TabStripOverFlowBehavior
        {
            get
            {
                return this.overflowManager.overFlowBehavior;
            }
            set
            {
                this.overflowManager.overFlowBehavior = value;
            }
        }

		/// <commentsfrom cref="TabLayoutPanel.StretchBaseMode" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("StretchBaseMode", typeof(TabLayoutPanel))]
		[RadPropertyDefaultValue("StretchBaseModeProperty", typeof(RadTabStripElement))]
		public TabBaseStretchMode StretchBaseMode
		{
			get
			{
				return (TabBaseStretchMode) this.GetValue(StretchBaseModeProperty);
			}
			set
			{
				
				this.SetValue(StretchBaseModeProperty, value);
			}
		}

		/// <commentsfrom cref="TabLayoutPanel.FitToAvailableWidth" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("FitToAvailableWidth", typeof(TabLayoutPanel))]
		[RadPropertyDefaultValue("FitToAvailableWidthProperty", typeof(RadTabStripElement))]
		public bool FitToAvailableWidth
		{
			get
			{
				return (bool) this.GetValue(FitToAvailableWidthProperty);
			}
			set
			{
				this.SetValue(FitToAvailableWidthProperty, value);
			}
		}

        //TODO: Check whether this works as expected and replace the old Shrink logic if appropriate
        /// <summary>
        /// Gets or sets the action to be taken when tab items exceed available size.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabStripItemOverflowMode OverflowMode
        {
            get
            {
                return this.overflowMode;
            }
            set
            {
                if (this.overflowMode == value)
                {
                    return;
                }

                this.overflowMode = value;
            }
        }

		public ItemsOverflowDropDown OverflowManager
		{
			get { return overflowManager; }
		}

        public RadRepeatScrollButtonElement LeftScrollButton
        {
            get
            {
                return this.scrollingManager.leftButton;
            }
        }

        public RadRepeatScrollButtonElement RightScrollButton
        {
            get
            {
                return this.scrollingManager.rightButton;
            }
        }

        public RadRepeatScrollButtonElement UpScrollButton
        {
            get
            {
                return this.scrollingManager.upButton;
            }
        }

        public RadRepeatScrollButtonElement DownScrollButton
        {
            get
            {
                return this.scrollingManager.downButton;
            }
        }

		internal void ItemClicked(RadElement tabElement)
		{
			if (this.GetIsSelected(tabElement))
			{
				return;
			}

			if (!this.CanSelectTab(tabElement))
			{
				return;
			}

			this.SetSelectedTab(tabElement);
		}

		internal void ItemHovered(RadElement tabElement)
		{
			if (this.GetIsHovered(tabElement))
			{
				return;
			}

			if (!this.CanHoverTab(tabElement))
			{
				return;
			}

			this.SetHoveredTab(tabElement);
		}

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF sz = base.ArrangeOverride(finalSize);

            if (this.ShrinkMode)
            {
                if (this.Items.Count > 0)
                {
                    float proportion = (float)this.Items[0].GetValue(BoxLayout.ProportionProperty);
                    if (proportion > 0)
                    {
                        for (int i = 0; i < this.Children.Count; i++)
                        {
                            RadElement element = this.Children[i];
                          
                        
                           
                            if (element.Class == "Separator")
                            {
                              
                                element.Visibility = ElementVisibility.Hidden;
                                int separatorIndex = this.separators.IndexOf((FillPrimitive)element);

                                if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
                                {
                                    int xOffset = this.ItemsOffset + 1;
                                    if (this.RightToLeft)
                                    {
                                        xOffset = 0;
                                    }

                                    for (int j = 0; j <= separatorIndex; j++)
                                    {
                                        if (j < this.Items.Count)
                                        {
                                            TabItem item = (TabItem)this.Items[j];
                                            if (this.RightToLeft)
                                            {
                                                item = (TabItem)this.Items[this.Items.Count - j - 1];
                                            }

                                            xOffset += (int)item.BoundingRectangle.Width - this.TabLayout.ItemsOverlapFactor;
                                        }
                                    }

                                    element.Visibility = ElementVisibility.Visible; 
                                    element.Arrange(new RectangleF(xOffset, 0, 1, this.BoxLayout.BoundingRectangle.Height - 1));
                                }
                                else
                                {
                                    int yOffset = this.ItemsOffset + 1;
                                    if (this.RightToLeft)
                                    {
                                        yOffset = 0;
                                    }

                                    for (int j = 0; j <= separatorIndex; j++)
                                    {
                                        if (j < this.Items.Count)
                                        {
                                            TabItem item = (TabItem)this.Items[j];
                                            if (this.RightToLeft)
                                            {
                                                item = (TabItem)this.Items[this.Items.Count - j - 1];
                                            }
                                            yOffset += (int)item.BoundingRectangle.Height - this.TabLayout.ItemsOverlapFactor;
                                        }
                                    }

                                    element.Visibility = ElementVisibility.Visible;
                                    element.Arrange(new RectangleF(0, yOffset, this.BoxLayout.BoundingRectangle.Width - 1, 1 ));
                                }

                                
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.Children.Count; i++)
                        {
                            RadElement element = this.Children[i];
                            if (element.Class == "Separator")
                            {
                                element.Visibility = ElementVisibility.Collapsed;
                                element.Arrange(RectangleF.Empty);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.Children.Count; i++)
                {
                    RadElement element = this.Children[i];
                    if (element.Class == "Separator")
                    {
                        element.Visibility = ElementVisibility.Collapsed;
                    }
                }
            }

            switch (this.overflowMode)
            {
                case TabStripItemOverflowMode.Hide:
                    PerformOverflowHide(finalSize);
                    break;
                case TabStripItemOverflowMode.Shrink:
                    PerformOverflowShrink(finalSize);
                    break;
            }

            return sz;
        }

        private void PerformOverflowShrink(SizeF arrangeSize)
        {
            TabStripShrinkInfo info = this.PrepareShrinkInfo(arrangeSize);
            //check whether we need to perform some shrinking
            TabPositions position = this.TabsPosition;
            switch (position)
            {
                case TabPositions.Top:
                case TabPositions.Bottom:
                    if (info.itemSize.Width <= info.availableSize.Width)
                    {
                        this.ResetItemsMaxSize();
                        return;
                    }
                    break;
                case TabPositions.Left:
                case TabPositions.Right:
                    if (info.itemSize.Height <= info.availableSize.Height)
                    {
                        this.ResetItemsMaxSize();
                        return;
                    }
                    break;
            }

            int count = this.Items.Count;
            float ratio = 0F;
            Size maxSize = Size.Empty;
            float sizeToReduce = info.itemSize.Width - info.availableSize.Width;
            float totalReduced = 0F;

            for (int i = 0; i < count; i++)
            {
                TabItem item = this.Items[i] as TabItem;
                if (item.Visibility != ElementVisibility.Visible)
                {
                    continue;
                }

                float correction = 0F;

                switch (position)
                {
                    case TabPositions.Top:
                    case TabPositions.Bottom:
                        ratio = item.TextPreferredSize.Width / info.textSize.Width;
                        correction = ratio * sizeToReduce;
                        maxSize = new Size((int)Math.Round(item.TextPreferredSize.Width - correction), 0);
                        maxSize.Width = Math.Max(1, maxSize.Width);
                        break;
                    case TabPositions.Left:
                    case TabPositions.Right:
                        ratio = item.TextPreferredSize.Height / info.textSize.Height;
                        correction = ratio * sizeToReduce;
                        maxSize = new Size(0, (int)Math.Ceiling(item.TextPreferredSize.Height - correction));
                        maxSize.Height = Math.Max(1, maxSize.Height);
                        break;
                }

                item.TextPrimitive.MaxSize = maxSize;

                totalReduced += correction;
                if (totalReduced >= sizeToReduce)
                {
                    break;
                }
            }
        }

        private void ResetItemsMaxSize()
        {
            foreach (TabItem item in this.Items)
            {
                item.TextPrimitive.ResetValue(RadElement.MaxSizeProperty, ValueResetFlags.Local);
            }
        }

        private TabStripShrinkInfo PrepareShrinkInfo(SizeF arrangeSize)
        {
            TabStripShrinkInfo info = new TabStripShrinkInfo();
            int count = this.Items.Count;
            TabItem item;
            TabPositions position = this.TabsPosition;

            for (int i = 0; i < count; i++)
            {
                item = this.Items[i] as TabItem;
                if (item.Visibility != ElementVisibility.Visible)
                {
                    continue;
                }

                float textShrink;

                switch (position)
                {
                    case TabPositions.Top:
                    case TabPositions.Bottom:
                        textShrink = item.TextPreferredSize.Width - item.TextPrimitive.DesiredSize.Width;
                        info.itemSize.Width += item.DesiredSize.Width + item.Margin.Horizontal + textShrink;
                        info.textSize.Width += item.TextPreferredSize.Width;
                        break;
                    case TabPositions.Left:
                    case TabPositions.Right:
                        textShrink = item.TextPreferredSize.Height - item.TextPrimitive.DesiredSize.Height;
                        info.itemSize.Height += item.DesiredSize.Height + item.Margin.Vertical + textShrink;
                        info.textSize.Height += item.TextPreferredSize.Height;
                        break;
                }
            }

            int length = this.ItemsOffset + (count - 1) * this.tabLayout.ItemsOverlapFactor;

            switch (position)
            {
                case TabPositions.Top:
                case TabPositions.Bottom:
                    info.itemSize.Width += length;
                    break;
                case TabPositions.Left:
                case TabPositions.Right:
                    info.itemSize.Height += length;
                    break;
            }

            info.availableSize = arrangeSize;
            return info;
        }

        internal virtual void UpdateTabStripLayoutOverlap(TabPositions position, int overlap)
        {
            switch (position)
            {
                case TabPositions.Top:
                    this.TabContent.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, -overlap, 0, 0));
                    this.tabLayout.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, overlap, 0, 0));
                    break;
                case TabPositions.Bottom:
                    this.TabContent.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, 0, 0, 0));
                    this.tabLayout.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, -overlap, 0, overlap));
                    break;
                case TabPositions.Left:
                    this.TabContent.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(-overlap, 0, 0, 0));
                    this.tabLayout.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(overlap, 0, 0, 0));
                    break;
                case TabPositions.Right:
                    this.TabContent.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, 0, 0, 0));
                    this.tabLayout.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(-overlap, 0, overlap, 0));
                    break;
            }
        }

        private void PerformOverflowHide(SizeF arrangeSize)
        {
            int count = this.Items.Count;
            bool hasVisibleItems = false;
            TabPositions position = this.TabsPosition;
            float arranged = 0;
            bool visible = false;

            float maxWidth = arrangeSize.Width - this.overflowOffset;
            float maxHeight = arrangeSize.Height - this.overflowOffset;

            for (int i = 0; i < count; i++)
            {
                RadItem item = this.Items[i];
                if (item.Visibility == ElementVisibility.Collapsed)
                {
                    continue;
                }

                switch (position)
                {
                    case TabPositions.Top:
                    case TabPositions.Bottom:
                        arranged += item.DesiredSize.Width;
                        visible = arranged <= maxWidth;
                        break;
                    case TabPositions.Left:
                    case TabPositions.Right:
                        arranged += item.DesiredSize.Height;
                        visible = arranged <= maxHeight;
                        break;
                }

                if (visible)
                {
                    item.Visibility = ElementVisibility.Visible;
                    hasVisibleItems = true;
                }
                else
                {
                    item.Visibility = ElementVisibility.Hidden;
                }
            }

            if (!hasVisibleItems && count > 0)
            {
                this.Items[0].Visibility = ElementVisibility.Visible;
            }
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == AutoScrollStateKey)
            {
                if (this.UseNewLayoutSystem)
                {
                    this.tabLayout.InvalidateMeasure();
                    this.tabLayout.InvalidateArrange();
                }
                else
                {
                    this.tabLayout.PerformLayoutCore(this);
                }
            }
            else if (key == ShowOverFlowButtonStateKey)
            {
                if (newValue)
                {
                    overflowManager.DropDownButton.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    overflowManager.DropDownButton.Visibility = ElementVisibility.Hidden;
                }
            }
        }

        private List<FillPrimitive> separators = new List<FillPrimitive>();

        private void ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
            if (separators.Count < this.Items.Count)
            {
                FillPrimitive primitive = new FillPrimitive();
                primitive.GradientStyle = GradientStyles.Linear;
                primitive.NumberOfColors = 2;
                primitive.BackColor = Color.FromArgb(40, SeparatorColor);
                primitive.BackColor2 = Color.FromArgb(100, SeparatorColor);
                primitive.BackColor3 = SeparatorColor;
                primitive.Class = "Separator";
                separators.Add(primitive);
                primitive.Visibility = ElementVisibility.Hidden;
                this.Children.Add(primitive);
            }
            else
            {
                RadElement separator = null;
                foreach (RadElement element in this.Children)
                {
                    if (element.Class == "Separator")
                    {
                        separator = element;
                        break;
                    }
                }

                if (separator != null)
                {
                    separator.ZIndex = 100000000;
                    this.Children.Remove(separator);
                    separators.RemoveAt(0);
                }
            }

			if (target != null)
			{
				if (this.UseNewLayoutSystem && this.tabLayout != null)
				{
					this.tabLayout.ScrollItemsOffset = 0;
				}
			}

			if (operation == ItemsChangeOperation.Inserted ||
				operation == ItemsChangeOperation.Set)
			{
				this.RotateItems(this.TabsPosition);

    		}

			if (operation == ItemsChangeOperation.Removed)
			{
				if (this.outlineForm != null)
				{
					this.outlineForm.Hide();
					this.outlineForm.Dispose();
					this.outlineForm = null;
				}
				if (this.items.Count > 0)
					this.SelectedTab = this.items[this.items.Count - 1];
			}

			if (operation == ItemsChangeOperation.Inserted && this.SelectedTab == null)
			{
				SetIsSelected(target, true);
				this.RefreshTabs(target);
			}
			else
				if (operation == ItemsChangeOperation.Inserted || operation == ItemsChangeOperation.Set)
				{

                    if (GetIsSelected(target))
					{
						this.RefreshTabs(target);
						this.RaiseTabSelected(target);
					}
					else
					{
						this.RefreshTabs();
					}
				}

            if (!this.UseNewLayoutSystem)
			    this.tabLayout.PerformLayout();
		}

		private void RefreshTabs()
		{
			this.RefreshTabs(null);
		}

		public void ScrollIntoView(TabItem item)
		{
			if (this.TabsPosition == TabPositions.Top ||
				this.TabsPosition == TabPositions.Bottom)
			{
                this.TabLayout.ScrollItemsOffset = 0;
                if (item.ControlBoundingRectangle.Right > this.ControlBoundingRectangle.Right)
                {
                    int offset = item.ControlBoundingRectangle.Right - this.ControlBoundingRectangle.Right + 40;
                    this.TabLayout.ScrollItemsOffset -= offset;
                }
			}
			else
			{
                this.TabLayout.ScrollItemsOffset = 0;
                if (item.ControlBoundingRectangle.Bottom >= this.ControlBoundingRectangle.Bottom)
                {
                    int offset = item.ControlBoundingRectangle.Bottom - this.ControlBoundingRectangle.Bottom + 40;
                    this.TabLayout.ScrollItemsOffset -= offset;
                }			
			}

            this.InvalidateArrange();
		}

		private void RaiseTabHovered(RadElement tab)
		{
			bool value = this.GetIsHovered(tab);
			if (value)
			{
				RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, OnRoutedTabHovered);
				tab.RaiseTunnelEvent(tab, args);
				OnTabHovered(new TabEventArgs(tab));
				if (!args.Canceled)
				{
					this.RaiseBubbleEvent(this, args);
				}
			}
			else
			{
				RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, OnTabUnHovered);
				tab.RaiseTunnelEvent(tab, args);
				if (!args.Canceled)
				{
					this.RaiseBubbleEvent(this, args);
				}
			}
		}

		private void RaiseTabSelected(RadElement tab)
		{
			bool value = this.GetIsSelected(tab);
			if (value)
			{
				RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, OnRoutedTabSelected);
				tab.RaiseTunnelEvent(tab, args);
				OnTabSelected(new TabEventArgs(tab));
				if (!args.Canceled)
				{
					this.RaiseBubbleEvent(this, args);
				}
			}
			else
			{
				RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, OnTabDeselected);
				tab.RaiseTunnelEvent(tab, args);
				if (!args.Canceled)
				{
					this.RaiseBubbleEvent(this, args);
				}
			}
		}

        public TabLayoutPanel TabLayout
        {
            get
            {
                return this.tabLayout;
            }
        }

        [Browsable(false)]
        public BorderPrimitive BaseFillBorder
        {
            get
            {
                return this.baseFillBorder;
            }
        }

        private FillPrimitive tabStripHeadFill;
        private BorderPrimitive tabStripHeadBorder;
        private BorderPrimitive baseFillBorder;
        private DockLayoutPanel dockLayout;
        private BoxLayout boxLayout;

        /// <summary>
        /// Gets the fill primitive used to define background behind tab items.
        /// </summary>
        [Browsable(false)]
        public FillPrimitive TabStripHeadFill
        {
            get
            {
                return this.tabStripHeadFill;
            }
        }

        /// <summary>
        /// Gets the border primitive used to define border around tab items.
        /// </summary>
        [Browsable(false)]
        public BorderPrimitive TabStripHeadBorder
        {
            get
            {
                return this.tabStripHeadBorder;
            }
        }

		// Public for test purposes, should be internal
        [Browsable(false)]
		public BoxLayout BoxLayout
		{
			get
			{
				return this.boxLayout;
			}
		}

        [Browsable(false)]
        public TabLayoutPanel TabsLayoutPanel
        {
            get
            {
                return this.tabLayout;
            }
        }

        [Browsable(false)]
        public DockLayoutPanel TabStripMainLayoutPanel
        {
            get
            {
                return this.dockLayout;
            }
        }


		protected override void CreateChildElements()
		{
            this.tabLayout = new TabLayoutPanel();
            this.tabLayout.ParentTabStripElement = this;

			this.tabLayout.BindProperty(TabLayoutPanel.ItemsOffsetProperty, this, RadTabStripElement.ItemsOffsetProperty, PropertyBindingOptions.TwoWay);
			this.tabLayout.BindProperty(TabLayoutPanel.ScrollItemsOffsetProperty, this, RadTabStripElement.ScrollItemsOffsetProperty, PropertyBindingOptions.TwoWay);
			this.tabLayout.BindProperty(TabLayoutPanel.TabsPositionProperty, this, RadTabStripElement.TabsPositionProperty, PropertyBindingOptions.OneWay);
			this.tabLayout.BindProperty(TabLayoutPanel.TabScrollButtonsPositionProperty, this, RadTabStripElement.TabScrollButtonsPositionProperty, PropertyBindingOptions.OneWay);
			this.tabLayout.BindProperty(TabLayoutPanel.TabItemsDropDownButtonPositionProperty, this, RadTabStripElement.TabItemsDropDownButtonPositionProperty, PropertyBindingOptions.OneWay);
			this.tabLayout.BindProperty(TabLayoutPanel.AllTabsEqualHeightProperty, this, RadTabStripElement.AllTabsEqualHeightProperty, PropertyBindingOptions.OneWay);
			this.tabLayout.BindProperty(TabLayoutPanel.TextOrientationProperty, this, RadTabStripElement.TextOrientationProperty, PropertyBindingOptions.OneWay);
			this.tabLayout.BindProperty(TabLayoutPanel.TabStripScrollStyleProperty, this, RadTabStripElement.TabStripScrollStyleProperty, PropertyBindingOptions.OneWay);
			this.tabLayout.BindProperty(TabLayoutPanel.ShrinkModeProperty, this, RadTabStripElement.ShrinkModeProperty, PropertyBindingOptions.OneWay);
			this.tabLayout.BindProperty(TabLayoutPanel.StretchBaseModeProperty, this, RadTabStripElement.StretchBaseModeProperty, PropertyBindingOptions.OneWay);
			this.tabLayout.BindProperty(TabLayoutPanel.FitToAvailableWidthProperty, this, RadTabStripElement.FitToAvailableWidthProperty, PropertyBindingOptions.OneWay);

			this.tabContentFill = new FillPrimitive();
			this.tabContentFill.AutoSizeMode = RadAutoSizeMode.Auto;
			this.tabContentFill.Class = "TabBaseFill";
			this.tabContentFill.SetValue(TabLayoutPanel.IsTabStripBaseProperty, true);
			this.tabContentFill.ShouldHandleMouseInput = true;

            this.tabStripHeadFill = new FillPrimitive();
            this.tabStripHeadFill.AutoSizeMode = RadAutoSizeMode.Auto;
            this.tabStripHeadFill.Class = "TabHeadFill";
            this.tabStripHeadFill.SetValue(TabLayoutPanel.IsTabStripHeadProperty, true);

            this.tabStripHeadBorder = new BorderPrimitive();
            this.tabStripHeadBorder.AutoSizeMode = RadAutoSizeMode.Auto;
            this.tabStripHeadBorder.Class = "TabHeadBorder";
            this.InitTabLayout();

			this.baseFillBorder = new BorderPrimitive();
			baseFillBorder.Class = "TabBaseBorder";
			baseFillBorder.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.tabContentFill.Children.Add(baseFillBorder);

			this.scrollingManager.InitializeScrollElements(this.tabLayout);
			this.OverflowManager.InitializeOverflowDropDown(this, this.tabLayout);

            this.tabLayout.BringToFront();
		}

        internal virtual void InitTabLayout()
        {
            this.dockLayout = new DockLayoutPanel();

            this.tabLayout.StretchVertically = false;

            this.boxLayout = new BoxLayout();
            this.boxLayout.StretchHorizontally = true;
            this.boxLayout.StretchVertically = false;
            this.boxLayout.Class = "TabItemLayout";
            this.items.Owner = this.boxLayout;

            DockLayoutPanel.SetDock(this.tabLayout, Dock.Top);

            this.TabContent.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, -this.tabLayout.LayoutOverlap, 0, 0));
            this.tabLayout.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, 0, 0, 0));

            this.tabLayout.BoxLayout = this.boxLayout;
            this.tabLayout.StretchTabItems(Orientation.Horizontal);
            this.tabLayout.Children.Add(this.boxLayout);

            // The last child is Dock.Fill as tabContent should be.
            //DockLayoutPanel.SetDock(this.tabContent, Dock.Bottom);


            // should have a default size as it has to be always visible
            // To be consistened with the old layout that value should be 7 
            this.tabContentFill.MinSize = new Size(0, 7);

            this.dockLayout.Children.Add(this.tabLayout);

            this.tabStripHeadBorder.StretchHorizontally = true;
            this.tabStripHeadBorder.StretchVertically = true;
            this.tabStripHeadBorder.SetDefaultValueOverride( RadElement.VisibilityProperty, ElementVisibility.Hidden);
            this.tabLayout.Children.Add(this.tabStripHeadBorder);
            this.tabStripHeadBorder.SendToBack();

            this.tabStripHeadFill.StretchHorizontally = true;
            this.tabStripHeadFill.StretchVertically = true;
            this.tabStripHeadFill.Visibility = ElementVisibility.Hidden;
            this.tabLayout.Children.Add(this.tabStripHeadFill);
            this.tabStripHeadFill.SendToBack();

            this.dockLayout.Children.Add(this.tabContentFill);

            this.Children.Add(dockLayout);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.UnassignMouseHandlers();
            this.AssignMouseHandlers();
        }

        protected override void DisposeManagedResources()
        {
            this.UnassignMouseHandlers();
            base.DisposeManagedResources();
        }

		public int GetIndex(TabItem item)
		{
			return this.Items.IndexOf(item);
		}

		private void AssignMouseHandlers()
		{
            if (this.ElementTree == null)
            {
                return;
            }

            Control host = this.ElementTree.Control;
            host.MouseMove += Control_MouseMove;
            host.MouseUp += Control_MouseUp;
            host.SizeChanged += Control_SizeChanged;
            host.MouseDown += Control_MouseDown;
		}

        private void UnassignMouseHandlers()
        {
            if (this.ElementTree == null)
            {
                return;
            }

            Control host = this.ElementTree.Control;
            host.MouseMove -= Control_MouseMove;
            host.MouseUp -= Control_MouseUp;
            host.SizeChanged -= Control_SizeChanged;
            host.MouseDown -= Control_MouseDown;
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (this.ElementTree != null && this.AllowDragDrop)
			{
                RadElement elementAtPoint = this.ElementTree.ComponentTreeHandler.ElementTree.GetElementAtPoint(e.Location);

				if (elementAtPoint is TabItem)
				{
					if (e.Button == MouseButtons.Left)
					{
						this.ElementTree.Control.Capture = true;
						if ((this.items.Count > 1) || AllowDragingInDocking)
							BeginDrag(e);
					}
				}
			}

		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.ElementTree != null && this.AllowDragDrop)
			{
				this.ElementTree.Control.Capture = false;
				if (e.Button == MouseButtons.Left)
				{
					if (this.outlineForm != null)
						this.outlineForm.Hide();

					SetGhostFormVisibility(false);
					EndDrag(e.Location);
				}

				if (this.outlineForm != null)
				{
					this.outlineForm.Hide();
					this.outlineForm.Dispose();
					this.outlineForm = null;
					this.draggedItem = null;
					this.replacedItem = null;
				}
			}
		}

		private void Control_SizeChanged(object sender, EventArgs e)
		{
			if ((this.LeftScrollButton.Visibility == ElementVisibility.Hidden)
				|| (this.RightScrollButton.Visibility == ElementVisibility.Hidden))
			{
				tabLayout.ScrollItemsOffset = 0;
				//tabLayout.PerformLayoutCore(this);
				this.LeftScrollButton.Enabled = false;
				this.RightScrollButton.Enabled = true;
				this.UpScrollButton.Enabled = false;
				this.DownScrollButton.Enabled = true;
			}
		}

		public void SelectTabByText(string tabItemText)
		{
			foreach (TabItem item in this.Items)
			{
				if (String.Equals(item.Text, tabItemText))
				{
					this.SelectedTab = item;
					break;
				}
			}
		}

		private void SetDragDropPointersLocation( TabItem item )
		{
			if (!(this.ElementTree.Control is RadTabStrip))
			{
				upPointer1.Visible = false;
				downPointer1.Visible = false;
				return;
			}

			if ((this.TabsPosition == TabPositions.Top) || (this.TabsPosition == TabPositions.Bottom))
			{
				if (!this.UseNewLayoutSystem)
				{
					this.upPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.Location.X,
					   item.Bounds.Bottom));
			
					this.downPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.Location.X,
						item.Bounds.Top - downPointer1.Size.Height));
				}
				else
				{
		//			this.upPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(boxLayout.Margin.Left
		//				 + boxLayout.BoundingRectangle.X + item.BoundingRectangle.X,
		//			   this.tabLayout.BoundingRectangle.Y + item.BoundingRectangle.Bottom));

		//			this.downPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.BoundingRectangle.X
		//				+ boxLayout.Margin.Left + boxLayout.BoundingRectangle.X,
		//				this.tabLayout.BoundingRectangle.Y + item.BoundingRectangle.Top - downPointer1.Size.Height));

					this.upPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.ControlBoundingRectangle.Location.X,
						item.ControlBoundingRectangle.Y + item.BoundingRectangle.Height));
					this.downPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.ControlBoundingRectangle.Location.X,
						item.ControlBoundingRectangle.Y - downPointer1.Size.Height));

		//			Console.WriteLine(item.Text + " " + this.upPointer1.Location); 
				}
			}
			else
			{

				if (!this.UseNewLayoutSystem)
				{
					if (!(item.Bounds.Top + item.Size.Width >= this.ElementTree.Control.Bounds.Height))

						this.upPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.Location.X,
						   item.Bounds.Top + item.Size.Width));
					else
					{
						this.upPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.Location.X,
						   this.ElementTree.Control.Bounds.Height));

					}
					this.downPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.Location.X,
						item.Bounds.Top - downPointer1.Size.Height));
				}
				else
				{
					this.upPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.ControlBoundingRectangle.Location.X,
						item.ControlBoundingRectangle.Y + item.BoundingRectangle.Height));

					this.downPointer1.Location = this.ElementTree.Control.PointToScreen(new Point(item.ControlBoundingRectangle.Location.X,
						item.ControlBoundingRectangle.Y - downPointer1.Size.Height));
	
				}

			}
		}

		private void InitializeDragDropPointers()
		{
			this.upPointer1 = new PositionPointer(Direction.Up);
			this.downPointer1 = new PositionPointer(Direction.Down);
			
			this.upPointer1.Size = new Size(6, 6);
			this.downPointer1.Size = new Size(5, 5);

			this.upPointer1.Opacity = 0.5;
			this.downPointer1.Opacity = 0.5;
		
	//		this.upPointer1.ArrowColor = Color.FromArgb(208, 212, 221);
	//		this.downPointer1.ArrowColor = Color.FromArgb(208, 212, 221);		
		}

		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			if (!this.AllowDragDrop) return;

			if ((this.items.Count > 1) || ( this.AllowDragingInDocking ))
			{
				if (e.Button != MouseButtons.Left)
				{
					if (this.outlineForm != null)
						this.outlineForm.Hide();

					this.SetGhostFormVisibility(false);
				}
	
				if (this.draggedItem == null)
				{
					if (this.outlineForm != null)
						this.outlineForm.Hide();

					SetGhostFormVisibility(false);
					
					return;
				}

                RadElement elementAtPoint = this.ElementTree.ComponentTreeHandler.ElementTree.GetElementAtPoint(new Point(e.X, e.Y));
				
				this.currentPosition = new Point(e.X, e.Y);
				
				this.IsRealDrag(e.Location);

				if (this.GetBitState(IsRealDragStateKey) && (this.AllowDragDrop) && (e.Button == MouseButtons.Left))
						if (elementAtPoint == null)
						{
							OnTabDragIsOutsideElement(new TabEventArgs(this.draggedItem));
						}

				if (elementAtPoint != null)
				{
					if (elementAtPoint.Visibility != ElementVisibility.Visible)
					{
						SetGhostFormVisibility(false);
						return;
					}

					if ((this.outlineForm != null) && (elementAtPoint is RadRepeatScrollButtonElement))
					{
						SetGhostFormVisibility(false);
						return;
					}

					this.PerformDragging(e, elementAtPoint);			
				}
				else SetGhostFormVisibility(false);

				this.SetOutLineForm(e);	
			}
		}

		private void PerformDragging(MouseEventArgs e, RadElement elementAtPoint)
		{
			if (this.GetBitState(IsRealDragStateKey) && (e.Button == MouseButtons.Left))
			{
				if ((this.outlineForm != null) || this.GetBitState(IsUsedInDockingStateKey))
				{
					if (this.outlineForm != null)
					{
						if (!this.outlineForm.Visible)
						{
							Telerik.WinControls.NativeMethods.ShowWindow(this.outlineForm.Handle, 8);
						}
						this.outlineForm.Location = Cursor.Position;
					}

					TabItem item = elementAtPoint as TabItem;	

					if (this.outlineForm != null)
					{
						this.outlineForm.AddOwnedForm(upPointer1);
						this.outlineForm.AddOwnedForm(downPointer1);
					}

					if (item != null)
					{
						if (outlineForm == null)
							InitializeDragDropPointers();

						SetDragDropPointersLocation(item);

						if (this.ElementTree.Control.ClientRectangle.Contains(new Point(e.X, e.Y)))
						{
							SetGhostFormVisibility(true);
						}
						else
						{
							SetGhostFormVisibility(false);
						}
					}
					else SetGhostFormVisibility(false);

				}
			}
			
		}

		private void SetOutLineForm(MouseEventArgs e)
		{
			if ( this.draggedItem != null )
			if (this.AllowDragDrop && this.GetBitState(IsRealDragStateKey) && (e.Button == MouseButtons.Left))
				if (this.outlineForm != null)
				{
					if (!this.outlineForm.Visible)
					{
						Telerik.WinControls.NativeMethods.ShowWindow(this.outlineForm.Handle, 8);
					}
					foreach (TabItem item in this.Items)
					{
						SetIsHovered(item, false);
					}
					this.outlineForm.Location = Cursor.Position;

				}
		}

		private void SetGhostFormVisibility(bool shouldShow)
		{
			if ((outlineForm == null )|| ( upPointer1 == null ) || (downPointer1 == null))
				return;

			if (shouldShow)
			{	
				if (this.ElementTree.Control is RadTabStrip)
				{
					Telerik.WinControls.NativeMethods.ShowWindow(this.upPointer1.Handle, 4);
					Telerik.WinControls.NativeMethods.ShowWindow(this.downPointer1.Handle, 4);					
				}	
			}
			else
			{
				this.upPointer1.Visible = false;
				this.downPointer1.Visible = false;
			}
		}

		private void RefreshTabsAfterDragDrop()
		{
			if (this.replacedItem == null) return;
			if (this.draggedItem == this.replacedItem) return;

			TabDragCancelEventArgs args = new TabDragCancelEventArgs(this.draggedItem, this.replacedItem, false);

			if (this.draggedItem != null)
				OnTabDragEnding(args);

			if (args.Cancel)
			{
				this.draggedItem = null;
				this.replacedItem = null;
                this.BitState[AllowSelectionStateKey] = true;
				return;
			}

			int index2 = this.Items.IndexOf(this.replacedItem);

			if (this.UseNewLayoutSystem)
				index2 = this.boxLayout.Children.IndexOf(this.replacedItem);


			if (index2 != -1)
			{	
			    if (this.UseNewLayoutSystem)
			    {
                    int idx = this.items.IndexOf(this.draggedItem);
                    if (idx != -1)  //fix for one tab Julian[04/06/2009]
                    {
                        this.Items.RemoveAt(idx);
                        this.Items.Insert(index2, this.draggedItem);
                    }
			    }
			    else
			    {
					this.Items.RemoveAt(this.items.IndexOf(this.draggedItem));
					this.Items.Insert(index2, this.draggedItem);
				}
			}

			if ((this.draggedItem != null) && this.GetBitState(IsRealDragStateKey))
				OnTabDragEnded(new TabDragEventArgs(this.draggedItem, this.replacedItem));

			this.draggedItem = null;
			this.replacedItem = null;
            this.BitState[AllowSelectionStateKey] = true;
        }


		protected virtual bool IsRealDrag(Point mousePosition)
		{
			if (!this.GetBitState(IsRealDragStateKey))
			{
				this.BitState[IsRealDragStateKey] = (Math.Abs((int)(mousePosition.X - this.initialMousePosition.X)) >= SystemInformation.DragSize.Width) || (Math.Abs((int)(mousePosition.Y - this.initialMousePosition.Y)) >= SystemInformation.DragSize.Height);
			}
			return this.GetBitState(IsRealDragStateKey);
		}

		private void BeginDrag(MouseEventArgs e)
		{
            this.BitState[AllowSelectionStateKey] = false;
			this.initialMousePosition = new Point(e.X, e.Y); ;
            RadElement elementAtPoint = this.ElementTree.ComponentTreeHandler.ElementTree.GetElementAtPoint(this.initialMousePosition);

			if (elementAtPoint == null)
			{
				return;
			}
			else
			{
				if ((elementAtPoint is RadRepeatScrollButtonElement)
					|| (elementAtPoint.Visibility != ElementVisibility.Visible))
					return;
			}

			this.draggedItem = elementAtPoint as TabItem;

			TabDragCancelEventArgs args = new TabDragCancelEventArgs(this.draggedItem, null, false);
			this.OnTabDragStarting(args);

			if (args.Cancel)
			{
				this.draggedItem = null;
                this.BitState[AllowSelectionStateKey] = true;
				return;
			}

			TabDragEventArgs dragArgs = new TabDragEventArgs(this.draggedItem, null);
			this.OnTabDragStarted(dragArgs);


			this.BitState[IsRealDragStateKey] = false;
			this.BitState[DraggingStateKey] = true;

			if (this.draggedItem != null)
				PrepareDragging(e);
		}

        public void CancelDrag()
        {
            this.EndDrag(new Point(Int32.MinValue, Int32.MinValue));
        }

		internal void EndDrag(Point location)
		{
            this.BitState[DraggingStateKey] = false;

			if (this.draggedItem != null)
			{

                RadElement elementAtPoint = this.ElementTree.ComponentTreeHandler.ElementTree.GetElementAtPoint(location);

				if (elementAtPoint != null)
				{
					if ((elementAtPoint is RadRepeatScrollButtonElement) || (elementAtPoint.Visibility != ElementVisibility.Visible))
					{
						return;
					}

					this.replacedItem = elementAtPoint as TabItem;
					if (this.replacedItem != null)
					{

						RefreshTabsAfterDragDrop();

						if ((this.outlineForm != null) && (this.upPointer1 != null) && (this.downPointer1 != null))
						{
							SetGhostFormVisibility(false);
						}
					}

				}
				if (this.outlineForm != null)
				{
					this.outlineForm.Dispose();
					this.outlineForm = null;
				}

                this.BitState[IsRealDragStateKey] = false;
                this.BitState[AllowSelectionStateKey] = true;
			}
		}

		private void PrepareDragging(MouseEventArgs e)
		{
			if (this.draggedItem.Visibility == ElementVisibility.Visible)
			{
				if (!this.IsUsedInDocking)
				{
					this.outlineForm = TelerikHelper.CreateOutlineForm();
					this.outlineForm.ShowInTaskbar = false;
					this.outlineForm.ShowIcon = false;

					this.InitializeDragDropPointers();

					this.outlineForm.BackgroundImage = this.draggedItem.GetItemBitmap();
					this.outlineForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
					this.outlineForm.Size = this.draggedItem.GetItemBitmap().Size;
					this.outlineForm.MinimumSize = this.draggedItem.GetItemBitmap().Size;
					this.outlineForm.Location = Cursor.Position;
				}
			}
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == TabsPositionProperty)
			{
				this.OnTabsPositionChanged(EventArgs.Empty);
				this.RotateItems((TabPositions)e.NewValue);
				this.EndEdit(true);
			}
			else if (e.Property == RightToLeftProperty)
			{
				this.TabLayout.ScrollItemsOffset = 0;	
				this.TabLayout.InvalidateArrange();

				this.EndEdit(true);
			}
			else if (e.Property == TextOrientationProperty)
			{
                this.RotateItems(this.TabsPosition);
				this.EndEdit(true);
			}
	        else if (e.Property == AllTabsEqualHeightProperty)
			{
				OnAllTabsEqualHeightChanged(EventArgs.Empty);
				this.EndEdit(true);
			}
            else if (e.Property == ItemMarginsProperty || e.Property == SelectedItemMarginsProperty)
            {
                this.RotateItems(this.TabsPosition);
            }

			base.OnPropertyChanged(e);
		}

        protected virtual void RotateItems(TabPositions tabPosition)
		{
			foreach (TabItem item in this.Items)
			{
                this.RotateItem(item, tabPosition);
                switch(tabPosition)
                {
                    case TabPositions.Left:
                    case TabPositions.Top:
                        this.FlipText = false;
                        break;
                    case TabPositions.Right:
                        this.FlipText = this.TextOrientation == Orientation.Vertical;
                        break;
                    case TabPositions.Bottom:
                        this.FlipText = this.TextOrientation == Orientation.Horizontal;
                        break;
                }
			}
		}

        protected virtual void RotateItem(TabItem item, TabPositions tabPosition)
        {
            switch (tabPosition)
            {
                case TabPositions.Top:
                    item.SetDefaultValueOverride(RadItem.AngleTransformProperty, 0F);
                    item.ImagePrimitive.SetDefaultValueOverride(RadItem.AngleTransformProperty, 0F);

                    if (item.TextImageRelation == TextImageRelation.TextBeforeImage)
                    {
                        item.SetDefaultValueOverride(ImageAndTextLayoutPanel.TextImageRelationProperty, TextImageRelation.ImageBeforeText);
                    }
                    break;
                case TabPositions.Left:
                    item.SetDefaultValueOverride(RadItem.AngleTransformProperty, 270F);
                    item.ImagePrimitive.SetDefaultValueOverride(RadItem.AngleTransformProperty, 0F);
                    if (item.TextImageRelation == TextImageRelation.ImageBeforeText)
                    {
                        item.SetDefaultValueOverride(ImageAndTextLayoutPanel.TextImageRelationProperty, TextImageRelation.TextBeforeImage);
                    }
                    break;
                case TabPositions.Right:
                    item.SetDefaultValueOverride(RadItem.AngleTransformProperty, 90F);
                    item.ImagePrimitive.SetDefaultValueOverride(RadItem.AngleTransformProperty, 0F);
                    if (item.TextImageRelation == TextImageRelation.TextBeforeImage)
                    {
                        item.SetDefaultValueOverride(ImageAndTextLayoutPanel.TextImageRelationProperty, TextImageRelation.ImageBeforeText);
                    }
                    break;
                case TabPositions.Bottom:
                    item.SetDefaultValueOverride(RadItem.AngleTransformProperty, 180F);
                    item.ImagePrimitive.SetDefaultValueOverride(RadItem.AngleTransformProperty, 180F);
                    if (item.TextImageRelation == TextImageRelation.ImageBeforeText)
                    {
                        item.SetDefaultValueOverride(ImageAndTextLayoutPanel.TextImageRelationProperty, TextImageRelation.TextBeforeImage);
                    }
                    break;
            }

            item.UpdateMargins();
        }

		protected virtual bool CanSelectTab(RadElement tabElement)
		{
			return true;
		}

		protected virtual bool CanHoverTab(RadElement tabElement)
		{
			return true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
		}

		/// <summary>
		///		Raises the <see cref="TabHovered"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabEventArgs"/> that contains the event data.</param>
		protected virtual void OnTabHovered(TabEventArgs args)
		{
			
			if (this.TabHovered != null)
			{
				this.TabHovered(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="TabSelecting"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabCancelEventArgs"/> that contains the event data.</param>
		protected virtual void OnTabSelecting(TabCancelEventArgs args)
		{
			if (this.TabSelecting != null)
			{
				this.TabSelecting(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="TabSelected"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabEventArgs"/> that contains the event data.</param>
		protected virtual void OnTabSelected(TabEventArgs args)
		{
			this.EndEdit(false);

			if (this.TabSelected != null)
			{
				this.TabSelected(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="TabDragStarting"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabEventArgs"/> that contains the event data.</param>
		protected virtual void OnTabDragStarting(TabDragCancelEventArgs args)
		{			
			if (this.TabDragStarting != null)
			{
				this.TabDragStarting(this, args);
			}
		}

		protected virtual void OnTabDragIsOutsideElement(TabEventArgs args)
		{
			if (this.TabDragIsOutsideElement != null)
			{
				if (this.TabDragIsOutsideElement != null)
				{
					this.TabDragIsOutsideElement(this, args);
				}
			}
		}

		/// <summary>
		///		Raises the <see cref="TabDragStarted"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabEventArgs"/> that contains the event data.</param>
		protected virtual void OnTabDragStarted(TabDragEventArgs args)
		{
			if (this.TabDragStarted != null)
			{
				this.TabDragStarted(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="TabDragEnding"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabEventArgs"/> that contains the event data.</param>
		protected virtual void OnTabDragEnding(TabDragCancelEventArgs args)
		{
			if (this.TabDragEnding != null)
			{
				this.TabDragEnding(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="TabDragEnded"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabEventArgs"/> that contains the event data.</param>
		protected virtual void OnTabDragEnded(TabDragEventArgs args)
		{
			if (this.TabDragEnded != null)
			{
				this.TabDragEnded(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="TabsPositionChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabEventArgs"/> that contains the event data.</param>
		protected virtual void OnTabsPositionChanged(EventArgs args)
		{
			if (this.TabsPositionChanged != null)
			{
				this.TabsPositionChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="AllTabsEqualHeightChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="TabEventArgs"/> that contains the event data.</param>
		protected virtual void OnAllTabsEqualHeightChanged(EventArgs args)
		{
			if (this.AllTabsEqualHeightChanged != null)
			{
				this.AllTabsEqualHeightChanged(this, args);
			}
		}

		protected internal override void OnStyleBuilt()
		{
			foreach (RadElement tab in this.Items)
			{
				RaiseTabSelected(tab);
			}
		}

		/// <summary>
		///		Retrieves the position of the item (passed as an argument) in the collection of items.
		/// </summary>
		/// <param name="Item">The <see cref="Telerik.WinControls.UI.TabItem"/> for which index it is searched for.</param>
		/// <returns>The index of the item.</returns>
		public int FindIndex(TabItem Item)
		{
			return Items.IndexOf(Item);
		}

        /// <summary>Retrieves the tab item at the specified point.</summary>
        /// <returns>The found <see cref="Telerik.WinControls.UI.TabItem"/>.</returns>
		public TabItem GetItemByPoint(Point point)
		{
			foreach (TabItem item in Items)
			{
				if (item.GetItemRectangleToTabStrip().Contains(point))
					return item;
			}
			return null;
		}

		/// <summary>
		///		Retrieves whether a given child element is hovered.
		/// </summary>
		/// <param name="childElement">The child element.</param>
		/// <returns>Whether the element is hovered or not.</returns>
		public bool GetIsHovered(RadElement childElement)
		{
			return (bool)childElement.GetValue(RadTabStripElement.IsHoveredProperty);
		}

		/// <summary>
		///		Retrieves whether a given child element is selected.
		/// </summary>
		/// <param name="childElement">The child element.</param>
		/// <returns>Whether the element is selected or not.</returns>
		public bool GetIsSelected(RadElement childElement)
		{
			return (bool)childElement.GetValue(RadTabStripElement.IsSelectedProperty);
		}

		/// <summary>
		///		Selects a given tab element.
		/// </summary>
		/// <param name="tabElement">The element which is about to be selected.</param>
		public void SetSelectedTab(RadElement tabElement)
		{
            if ((this.Parent != null) && (this.items.Count > 0))
			{
                if (!this.GetBitState(AllowSelectionStateKey))
                {
                    return;
                }

				TabCancelEventArgs args = new TabCancelEventArgs(tabElement);

				OnTabSelecting(args);

				bool cancel = args.Cancel;

				if (cancel)
				{
					return;
				}

				foreach (RadElement child in this.Items)
				{
					if (this.GetIsSelected(child))
					{
						this.SetIsSelected(child, false);
					}
				}

				bool found = false;
				foreach (RadElement child in this.Items)
				{
					if (child == tabElement)
					{

						this.SetIsSelected(child, true);                                       
						found = true;
						break;
					}
				}

				if (tabElement == null)
				{
					return;
				}

				if (!found)
				{
                    if (this.Site == null || !this.Site.DesignMode)
					    throw new Exception("Tab to select does not belong to tabs collection");
				} 
			}

			this.RefreshTabs(tabElement);
		}

        /// <summary>
		///		Sets a child element as selected or deselected and raises the respective routed event.
		/// </summary>
		/// <param name="childElement">The child element.</param>
		/// <param name="value">Whether the element should be selected (true) or deselected (false).</param>
		public void SetIsSelected(RadElement childElement, bool value)
		{
			if (GetIsSelected(childElement) != value)
			{
			

				childElement.SetValue(RadTabStripElement.IsSelectedProperty, value);
				this.RaiseTabSelected(childElement);
			}
		}

		/// <summary>
		///		Sets a child element as hovered or not hovered and raises the respective routed event.
		/// </summary>
		/// <param name="childElement">The child element.</param>
		/// <param name="value">Whether the element should be hovered (true) or not hovered (false).</param>
		public void SetIsHovered(RadElement childElement, bool value)
		{
			if (this.GetIsHovered(childElement) != value)
			{
				childElement.SetValue(RadTabStripElement.IsHoveredProperty, value);
				this.RaiseTabHovered(childElement);
			}
		}

		/// <summary>
		///		Hovers a given tab element.
		/// </summary>
		/// <param name="tabElement">The element which is about to be hovered.</param>
		public void SetHoveredTab(RadElement tabElement)
		{
			this.SetIsHovered(tabElement, true);
			this.RefreshHoveredTabs(tabElement);
		}

        /// <summary>
        /// Refreshesh all tabs selected/deselected state and Z order conforming to
        /// newSelectedTab. The NewSelectedTab element should already have its property
        /// RadTabStripElement.IsSeelctedProperty set to true;
        /// </summary>
		public void RefreshTabs(RadElement newSelectedTab)
		{
			RadElement selectedTab = newSelectedTab;

			int minZIndex = 5; //engineer constant
			int i = 1;
			//changing Zedindex, when tabs is added
			foreach (RadElement tab in this.Items)
			{

				if (newSelectedTab != null && tab != newSelectedTab)
				{
					SetIsSelected(tab, false);
				}

				if (selectedTab == null && GetIsSelected(tab))
				{
					selectedTab = tab;
				}
				else
				{
					tab.ZIndex = this.Items.Count - i + minZIndex;
				}
				i++;
			}

			if (selectedTab != null)
			{
				selectedTab.ZIndex = this.Items.Count + 1 + minZIndex;
			}
		}

	
		public void RefreshHoveredTabs(RadElement newHoveredTab)
		{
            RadElement hoveredTab = newHoveredTab;
			int i = 1;
			//changing Zedindex, when tabs is added
			foreach (RadElement tab in this.Items)
			{
				if (newHoveredTab != null && tab != newHoveredTab)
				{
					if (newHoveredTab != SelectedTab)
						SetIsHovered(tab, false);
				}

				if (hoveredTab == null && GetIsHovered(tab))
				{
					hoveredTab = tab;
				}

				else
				{
					tab.ZIndex = this.Items.Count - i;
				}

				i++;
			}

			if (hoveredTab != null)
			{
				hoveredTab.ZIndex = this.items.Count + 1;
			}
		}

        /// <summary>Initiates TabHovered event.</summary>
		public void CallOnTabHovered(TabEventArgs e)
		{
			OnTabHovered(e);
		}

        /// <summary>Fires MouseDown event.</summary>
		public void CallOnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			OnMouseDown(e);
		}

        /// <summary>Fires MouseUp event.</summary>
		public void CallOnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			OnMouseUp(e);
		}

        /// <summary>Fires MouseMove event.</summary>
		public void CallOnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			OnMouseMove(e);
		}

#region EDITORS
		/// <summary>
		/// Occurs when editor validating fails.
		/// </summary>
		[Browsable(true),
	   Category(RadDesignCategory.ActionCategory),
	   Description("Occurs when editor validating fails.")]
		public event EventHandler ValidationError;

		/// <summary>
		/// Occurs when the editor changed the value edting.
		/// </summary>
		[Browsable(true),
	   Category(RadDesignCategory.ActionCategory),
	   Description("Occurs when the editor changed the value edting.")]
		public event CancelEventHandler ValueValidating;

		/// <summary>
		/// Occurs when the editor finished the value editing.
		/// </summary>
		[Browsable(true),
	   Category(RadDesignCategory.ActionCategory),
	   Description("Occurs when the editor finished the value editing.")]
		public event EventHandler ValueChanged;

		/// <summary>
		/// Occurs when the editor is changing the value during the editing proccess.
		/// </summary>
		[Category(RadDesignCategory.ActionCategory),
		 Description("Occurs when the editor is changing the value during the editing proccess.")]
		public event ValueChangingEventHandler ValueChanging;

		/// <summary>
		/// Occurs before the tab item label text is edited.
		/// </summary>
		[Category("Behavior")]
		[Description("Occurs before the tab item label text is edited.")]
		public event CancelEventHandler Editing;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void EditorRequiredHandler(object sender, EditorRequiredEventArgs e);

		/// <summary>
		/// Occurs before the tab item label text is edited.
		/// </summary>
		[Category("Behavior")]
		[Description("Occurs before the tab item label text is edited.")]
		public event EventHandler Edited;
		/// <summary>
		/// Occurs when TabStrip required editor.
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.ActionCategory),
	   Description("Occurs when TabStrip required editor.")]
		public event EditorRequiredHandler EditorRequired;

		internal void CallValidatingError(object sender, EventArgs e)
		{
			if (this.ValidationError != null)
				this.ValidationError(sender, e);
		}

		internal void CallValidation(object sender, CancelEventArgs e)
		{
			this.OnValidating(sender, e);
		}


		internal void CallValueChanged(object sender, EventArgs e)
		{
			this.EndEdit(false);
		}


		protected void OnValueChanged(object sender, EventArgs e)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(sender, e);
			}
		}

		internal void CallValidating(object sender, CancelEventArgs e)
		{
			this.OnValidating(sender, e);
		}

		protected void OnValidating(object sender, CancelEventArgs e)
		{
			if (this.ValueValidating != null)
				this.ValueValidating(this.SelectedTab, e);
		}

		internal void CallValueChanging(object sender, ValueChangingEventArgs e)
		{
			this.OnValueChanging(sender, e);
		}

		protected void OnValueChanging(object sender, ValueChangingEventArgs e)
		{
			if (this.ValueChanging != null)
				this.ValueChanging(this, e);
		}

		internal void WireEvents(IValueEditor editor)
		{
			if (editor != null)
			{
				editor.ValueChanging += new ValueChangingEventHandler(CallValueChanging);
				editor.Validating += new CancelEventHandler(CallValidating);
				editor.ValueChanged += new EventHandler(CallValueChanged);
				editor.ValidationError += new ValidationErrorEventHandler(CallValidatingError);
			}
		}

		internal void UnwireEvents(IValueEditor editor)
		{
			if (editor != null)
			{
				editor.ValueChanging -= new ValueChangingEventHandler(CallValueChanging);
				editor.Validating -= new CancelEventHandler(CallValidating);
				editor.ValueChanged -= new EventHandler(CallValueChanged);
				editor.ValidationError -= new ValidationErrorEventHandler(CallValidatingError);
			}
		}

		/// <summary>
		/// Retrive edited value from selectedTab
		/// </summary>
		/// <param name="selectedTab">current selectedTab</param>
		/// <returns></returns>
		internal object GetEditedValue(TabItem  selectedTab)
		{
			object editedValue = null;
			switch (selectedTab.editMode)
			{
				case EditMode.Name:
					editedValue = selectedTab.Text;
					break;
				case EditMode.Tag:
				case EditMode.Value:
					editedValue = selectedTab.Tag;
					break;
			}
			return editedValue;
		}

		private void SetEditedValue(TabItem selectedTab, object newValue)
		{
			switch (selectedTab.editMode)
			{
				case EditMode.Name:
					selectedTab.Text = newValue.ToString();
					break;
				case EditMode.Tag:
				case EditMode.Value:
					selectedTab.Tag = newValue;
					break;
			}
		}

		private TabItem oldItem = null;
		private IValueEditor editor = null;
		/// <summary>
		/// get or set active editor in the tree
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IValueEditor ActiveEditor
		{
			get
			{
				return editor;
			}
			set
			{
				if (editor != value)
				{
					editor = value;
				}
			}
		}


	
		/// <summary>
		/// Entry point for editing         
		/// </summary>
		/// <returns>Returns true if editing can begin.</returns>
		public bool BeginEdit()
		{
			TabItem selectedTab = (TabItem)this.SelectedTab;

			if (!this.GetBitState(AllowEditStateKey) ||
				SelectedTab == null || this.ActiveEditor != null)// !selectedNode.IsLabelEditable ||
			{
				return false;
			}

		if (this.SelectedTab.ControlBoundingRectangle.Right > this.ControlBoundingRectangle.Right)
			return false;

		if (this.SelectedTab.ControlBoundingRectangle.Bottom > this.ControlBoundingRectangle.Bottom)
			return false;

			object editedValue = this.GetEditedValue(selectedTab);
			this.oldItem = selectedTab;

			CancelEventArgs args = new CancelEventArgs();
			this.CallEditing(selectedTab, args);
			if (args.Cancel)
			{
				return false;
			}
			this.BeginEditCore(selectedTab, editedValue);

			return true;
		}


		private void BeginEditCore(TabItem selectedTab, object editedValue)
		{
			TabStripEditManager editManager = new TabStripEditManager((RadTabStrip)this.ElementTree.Control, selectedTab);

			
			this.ActiveEditor = editManager.DefaultEditor;
			
			this.WireEvents(this.ActiveEditor);
			//this.ActiveEditor.Initialize(selectedTab, editedValue);


			RadItem editor = this.ActiveEditor as RadItem;
			editor.AutoSize = false;

			Size editorSize = Size.Empty;
		
			editor.MaxSize = Size.Empty;
			editor.MinSize = Size.Empty;
			if (editor as RadHostItem != null)
			{
				((RadHostItem)editor).HostedControl.Size = Size.Empty;
				((RadHostItem)editor).HostedControl.MinimumSize = Size.Empty;
				((RadHostItem)editor).HostedControl.MaximumSize = Size.Empty;

			}

			this.TabLayout.Children.Add(this.ActiveEditor as RadItem);

			editorSize = this.SelectedTab.Size;

			if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
			{
				editor.Size = editorSize;
			}
			else
			{
				editor.Size = new Size(editorSize.Height, editorSize.Width);			
			}

			if (editor as RadHostItem != null)
			{
				desiredLocation = this.SelectedTab.ControlBoundingRectangle.Location;

				((RadHostItem)editor).HostedControl.LocationChanged += new EventHandler(HostedControl_LocationChanged);
	
				editor.MaxSize = editor.Size;
				editor.MinSize = editor.Size;
				editor.Location = this.SelectedTab.ControlBoundingRectangle.Location;
				
				((RadHostItem)editor).HostedControl.Size = editor.Size;
				((RadHostItem)editor).HostedControl.MinimumSize = editor.Size;
				((RadHostItem)editor).HostedControl.MaximumSize = editor.Size;
				((RadHostItem)editor).HostedControl.Location = this.SelectedTab.ControlBoundingRectangle.Location;
				
			}


			this.ActiveEditor.BeginEdit();
		
		}

        private void HostedControl_LocationChanged(object sender, EventArgs e)
		{

			if ( (sender as Control).Location  != this.desiredLocation )
			(sender as Control).Location = this.desiredLocation;
		}

		

	

		private Point desiredLocation;

		/// <summary>
		/// Allows a pending edit operation to be ended.
		/// </summary>
		/// <param name="cancelEdit">Boolean value indicating whether the edit operation should be canceled.</param>
		/// <returns></returns>
		public bool EndEdit(bool cancelEdit)
		{
			if (this.ActiveEditor == null)
				return false;

			this.TabLayout.Children.Remove(this.ActiveEditor as RadItem);
		
			CancelEventArgs cancelArgs = new CancelEventArgs();
			this.CallValidating(this, cancelArgs);

			if (cancelArgs.Cancel)
			{
				this.CallValidatingError(this, new EventArgs());
				return false;
			}

			this.UnwireEvents(this.ActiveEditor);

			object newValue = this.ActiveEditor.Value;
			(this.ActiveEditor as RadItem).Dispose();
			this.ActiveEditor = null;

			if (!cancelEdit && this.oldItem != null)
			{
				this.SetEditedValue(this.oldItem, newValue);

				this.OnValueChanged(this.SelectedTab, EventArgs.Empty);
				this.CallEdited(this.SelectedTab, EventArgs.Empty);
			}

		
			return true;
		}

		private void CallEditing(object sender, CancelEventArgs args)
		{
			this.OnEditing(sender, args);
		}

		private void OnEditing(object sender, CancelEventArgs args)
		{		
			if (this.Editing != null)
			{
				this.Editing(sender, args);
			}
		}

		private void CallEdited(object sender, EventArgs args)
		{
			this.OnEdited(sender, args);
		}

		private void OnEdited(object sender, EventArgs args)
		{			
			if (this.Edited != null)
			{
				this.Edited(sender, args);
			}
		}

		internal void CallEditorRequired(object sender, EditorRequiredEventArgs e)
		{
			this.OnEditorRequired(sender, e);
		}

		protected void OnEditorRequired(object sender, EditorRequiredEventArgs e)
		{
			if (this.EditorRequired != null)
				this.EditorRequired(sender, e);
		}
	
#endregion

		/// <summary>Fires a bubble event.</summary>
        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnBubbleEvent(sender, args);
		
			if (args.RoutedEvent == RadElement.MouseClickedEvent)
			{
				if (this.Items.Contains(sender as RadItem))
				{
					if (TabClicked != null)
					{
						this.TabClicked(this, new TabEventArgs(sender as RadElement));
					}
				}
			}
			else if (args.RoutedEvent == RadElement.MouseDownEvent)
			{
				if (this.Items.Contains(sender as RadItem))
				{
					this.ItemClicked(sender);
				}

				if (sender == this.LeftScrollButton)
				{                      
                  this.scrollingManager.ScrollLeft();
				}
				else if (sender == this.RightScrollButton)
				{                    
                    this.scrollingManager.ScrollRight();                     
				}
				else if (sender == this.UpScrollButton)
				{                   
					this.scrollingManager.ScrollUp();
				}
				else if (sender == this.DownScrollButton)
				{
                   
                    this.scrollingManager.ScrollDown();
				}
			}
		}
	}
}
