using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///		Defines the way the tab base allocates the necessary space.
	/// </summary>
	public enum TabBaseStretchMode
	{
		/// <summary>
		///		The base area takes all the available width when TabPosition is Top or Bottom
		///		and all the available height when TabPosition is Left or Right.
		/// </summary>
		Default,
		/// <summary>
		///		The base area takes all the available width and height after the size of the tab items
		///		have been calculated.
		/// </summary>
		StretchToRemainingSpace,
		/// <summary>
		///		The base area takes no more size than the tab items when it doesn't have child elements
		///		and takes the necessary space when it has child elements.
		/// </summary>
		NoStretch
	}

    /// <summary>
    ///     Defines the possible positions of the tab items
    ///     relatively to the base area.
    /// </summary>
	public enum TabPositions
	{
		/// <summary>
		///		The tab items will appear on the left of the base area.
		/// </summary>
		Left,
		/// <summary>
		///		The tab items will appear on the right of the base area.
		/// </summary>
		Right,
		/// <summary>
		///		The tab items will appear on the top of the base area.
		/// </summary>
		Top,
		/// <summary>
		///		The tab items will appear on the bottom of the base area.
		/// </summary>
		Bottom
	}

    /// <summary>
    ///     The LayoutPanel is responsible for the layout of the
    ///     <see cref="RadTabStrip">TabStrip</see> control. It arranges the tab items, the base
    ///     area (that could contain other controls when the control is in TabControl mode),
    ///     the the scroll buttons, and the overflow button.
    /// </summary>
	public class TabLayoutPanel : LayoutPanel
    {
        #region Fields
        private PreferredSizeData leftScrollButtonData = null;
        private PreferredSizeData rightScrollButtonData = null;
        private PreferredSizeData upScrollButtonData = null;
        private PreferredSizeData downScrollButtonData = null;
        private PreferredSizeData overflowButtonData = null;
		private PreferredSizeData tabStripHead = null;

		private int maxHeight;
        private int maxWidth;
        private int nextLeft;
        private int itemsSize;
		private int nextDown;
        private bool useVerticalRTL = true;

        private RadTabStripElement parentTabStripElement;

        #endregion

        #region DependencyProperties
        public static RadProperty IsTabStripBaseProperty = RadProperty.Register(
			"IsTabStripBase", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty IsTabStripHeadProperty = RadProperty.Register(
			"IsTabStripHead", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsLeftScrollButtonProperty = RadProperty.Register(
			"IsLeftScrollButtonProperty", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsRightScrollButtonProperty = RadProperty.Register(
			"IsRightScrollButton", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsUpScrollButtonProperty = RadProperty.Register(
			"IsUpScrollButton", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsDownScrollButtonProperty = RadProperty.Register(
			"IsDownScrollButton", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsDropDownButtonProperty = RadProperty.Register(
			"IsDropDownButton", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty ItemsOverlapFactorProperty = RadProperty.Register(
			"ItemsOverlapFactor", typeof(int), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				-2, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ItemsOffsetProperty = RadProperty.Register(
			"ItemsOffset", typeof(int), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				30, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty ScrollItemsOffsetProperty = RadProperty.Register(
			"ScroollItemsOffset", typeof(int), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));
		
		public static RadProperty TextOrientationProperty = RadProperty.Register(
		"TextOrientation", typeof(Orientation), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
			Orientation.Horizontal, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.CanInheritValue | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty TabsPositionProperty = RadProperty.Register(
			"TabsPosition", typeof(TabPositions), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				TabPositions.Top, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty TabScrollButtonsPositionProperty = RadProperty.Register(
			"TabScrollButtonsPosition", typeof(TabScrollButtonsPosition), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				TabScrollButtonsPosition.RightBottom, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty TabStripScrollStyleProperty = RadProperty.Register(
		"TabScrollStyle", typeof(TabStripScrollStyle), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
			TabStripScrollStyle.ScrollByItem));

        public static RadProperty TabItemsDropDownButtonPositionProperty = RadProperty.Register(
			"TabDropDownButtonPosition", typeof(TabItemsDropDownButtonPosition), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				TabItemsDropDownButtonPosition.RightBottom, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty AllTabsEqualHeightProperty = RadProperty.Register(
			"AllTabsEqualHeight", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty StretchBaseModeProperty = RadProperty.Register(
			"StretchBaseMode", typeof(TabBaseStretchMode), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				TabBaseStretchMode.Default, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ShrinkModeProperty = RadProperty.Register(
		"ShrinkMode", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
			false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));
    
		
		public static RadProperty FitToAvailableWidthProperty = RadProperty.Register(
			"FitToAvailableWidth", typeof(bool), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        #endregion

        #region Loading/Initialization

        protected override void DisposeManagedResources()
        {
            this.parentTabStripElement.Items.ItemsChanged -= Items_ItemsChanged;

            base.DisposeManagedResources();
        }

        protected override void LoadCore()
        {
            base.LoadCore();

            if (!this.UseNewLayoutSystem)
            {
                return;
            }

            this.parentTabStripElement.Items.ItemsChanged += Items_ItemsChanged;
            this.UpdateTabsPosition(this.TabsPosition);
        }

        private Orientation Orientation
        {
            get
            {
                TabPositions position = this.TabsPosition;
                if (position == TabPositions.Top || position == TabPositions.Bottom)
                {
                    return Orientation.Horizontal;
                }

                return Orientation.Vertical;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the layout should be reversed when in vertical right-to-left mode.
        /// </summary>
        public bool UseVerticalRTL
        {
            get
            {
                return useVerticalRTL;
            }
            set
            {
                useVerticalRTL = value;
            }
        }


        /// <summary>
        /// Defines the number of pixels with which two tab items overlap. When the number is
        /// negative, the items will stray from each other.
        /// </summary>
		[Description("Defines the number of pixels with which two tab items will overlap. When the number is negative the items will stray from each other.")]
        public int ItemsOverlapFactor
		{
			get
			{
				return (int)this.GetValue(ItemsOverlapFactorProperty);
			}
			set
			{
				this.SetValue(ItemsOverlapFactorProperty, value);
			}
		}

		/// <summary>
		/// Defines a shrink behavior of tabItems
		/// </summary>
		[Description("Defines a shrink behavior of tabs in RadTabStrip")]
		public bool ShrinkMode
		{
			get
			{
				return (bool)this.GetValue(ShrinkModeProperty);
			}
			set
			{
				this.SetValue(ShrinkModeProperty, value);
			}
		}

		/// <summary>
		///		Defines the number of pixels at which the first tab item will be offset.
		/// </summary>
		[Description("Defines the number of pixels at which the first tab item will be offset.")]
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

		/// <summary>
		///		Defines the number of pixels at which the scroll buttons will be offset.
		/// </summary>
		[Description("Defines the number of pixels at which the scroll buttons will be offset.")]
        public int ScrollItemsOffset
        {
            get
            {
                return (int)this.GetValue(ScrollItemsOffsetProperty);
            }
            set
            {
				if (value == 0)
				{

                    //if (this.ParentTabStripElement.scrollingManager.leftButton != null &&
                    //    this.ParentTabStripElement.scrollingManager.rightButton != null &&
                    //    this.ParentTabStripElement.scrollingManager.upButton != null &&
                    //this.ParentTabStripElement.scrollingManager.downButton != null)
                    //{
                    //    this.ParentTabStripElement.scrollingManager.leftButton.Enabled = false;
                    //    this.ParentTabStripElement.scrollingManager.rightButton.Enabled = true;
                    //    this.ParentTabStripElement.scrollingManager.upButton.Enabled = false;
                    //    this.ParentTabStripElement.scrollingManager.downButton.Enabled = true;
                    //}
				}

                this.SetValue(ScrollItemsOffsetProperty, value);
            }
        }

        /// <summary>
        ///     Defines the <see cref="TabPositions">position</see> of the tab items relatively to
        ///     the base area.
        /// </summary>
		[Description("Defines the position of the tab items relatively to the base area.")]
		public TabPositions TabsPosition
		{
            get
            {
				return (TabPositions)this.GetValue(TabLayoutPanel.TabsPositionProperty);
            }
            set
            {
				this.SetValue(TabLayoutPanel.TabsPositionProperty, value);
            }
		}

		/// <summary>
		///		Defines the position of the drop down button responsible for the overflow functionality.
		/// </summary>
		[Description("Defines the position of the drop down button responsible for the overflow functionality.")]
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

        /// <summary>
        ///     Defines the <see cref="TabScrollButtonsPosition">position</see> of the scroll
        ///     buttons.
        /// </summary>
		[Description("Defines the position of the scroll buttons.")]
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
		///		Defines whether all tab items will appear with an equal height (with the height of the highest element) or each element will appear with its own height.
		/// </summary>
		[Description("Defines whether all tab items will appear with an equal height (with the height of the highest element) or each element will appear with its own height.")]
		public bool AllTabsEqualHeight
		{
			get
			{
				return (bool) this.GetValue(AllTabsEqualHeightProperty);
			}
			set
			{
				this.SetValue(AllTabsEqualHeightProperty, value);
			}
		}

		/// <summary>
		///		Defines the way the tab base allocates the necessary space.
		/// </summary>
		[Description("Defines the way the tab base allocates the necessary space.")]
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

		/// <summary>
		///		Defines whether the tabstrip width should be with higher priority than the available width.
		/// </summary>
		[Description("Defines whether the tabstrip width should be with higher priority than the available width.")]
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
        #endregion
        /// <commentsfrom cref="Telerik.WinControls.Layouts.IRadLayoutElement.GetPreferredSize" filter=""/>
        public override Size GetPreferredSizeCore(Size proposedSize)
		{
			Size desiredSize = Size.Empty;
			Size baseSize = Size.Empty;
			foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
			{
                if ((bool)child.GetValue(IsTabStripBaseProperty))
				{
					baseSize = child.FullSize;
				}
				else if (!(bool) child.GetValue(IsTabStripHeadProperty))
				{
                    if (this.TabsPosition == TabPositions.Left || this.TabsPosition == TabPositions.Right)
					{
						desiredSize.Height += child.FullBoundingRectangle.Height;
						desiredSize.Height -= this.ItemsOverlapFactor;
						desiredSize.Width = Math.Max(desiredSize.Width, child.FullBoundingRectangle.Width);
                    }
					else
					{
						desiredSize.Width += child.FullBoundingRectangle.Width;
						desiredSize.Width -= this.ItemsOverlapFactor;
						desiredSize.Height = Math.Max(desiredSize.Height, child.FullBoundingRectangle.Height);
					}
				}
            }

			if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
				desiredSize.Width += this.ItemsOffset;
			else
				desiredSize.Height += this.ItemsOffset;

			if (this.TabsPosition == TabPositions.Left || this.TabsPosition == TabPositions.Right)
			{
				desiredSize.Width += baseSize.Width;
				desiredSize.Height = Math.Max(desiredSize.Height, baseSize.Height);
			}
			else
			{
				desiredSize.Height += baseSize.Height;
				desiredSize.Width = Math.Max(desiredSize.Width, baseSize.Width);
			}

			if (this.FitToAvailableWidth && (desiredSize.Width > this.AvailableSize.Width))
			{
				desiredSize.Width = this.AvailableSize.Width;
			}

			return desiredSize;
		}

		internal RadTabStripElement ParentTabStripElement
		{
			get
			{
				return parentTabStripElement;
			}
			set
			{
				this.parentTabStripElement = value;
			}
		}

        private void InitializePreferredSizeDataList(List<PreferredSizeData> list)
        {
            leftScrollButtonData = null;
            rightScrollButtonData = null;
            upScrollButtonData = null;
            downScrollButtonData = null;
            overflowButtonData = null;
			tabStripHead = null;

            maxHeight = 0;
            maxWidth = 0;

            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                PreferredSizeData data = new PreferredSizeData(child, this.AvailableSize);
                if (!(bool)data.Element.GetValue(IsTabStripBaseProperty) &&
                    !(bool)data.Element.GetValue(IsLeftScrollButtonProperty) &&
                    !(bool)data.Element.GetValue(IsRightScrollButtonProperty) &&
                    !(bool)data.Element.GetValue(IsUpScrollButtonProperty) &&
                    !(bool)data.Element.GetValue(IsDownScrollButtonProperty) &&
                    !(bool)data.Element.GetValue(IsDropDownButtonProperty) &&
					!(bool) data.Element.GetValue(IsTabStripHeadProperty))
                {
                    Rectangle rotatedBounds = data.Element.GetBoundingRectangle(data.PreferredSize);
                    maxHeight = Math.Max(maxHeight, rotatedBounds.Height);
                    maxWidth = Math.Max(maxWidth, rotatedBounds.Width);
                }

				if ((bool)data.Element.GetValue(IsTabStripHeadProperty))
					tabStripHead = data;
               
                if ((bool)data.Element.GetValue(IsLeftScrollButtonProperty))
                    leftScrollButtonData = data;
                if ((bool)data.Element.GetValue(IsRightScrollButtonProperty))
                    rightScrollButtonData = data;

                if ((bool)data.Element.GetValue(IsUpScrollButtonProperty))
                    upScrollButtonData = data;
               
                if ((bool)data.Element.GetValue(IsDownScrollButtonProperty))
                    downScrollButtonData = data;

                if ((bool)data.Element.GetValue(IsDropDownButtonProperty))
                    overflowButtonData = data;

                list.Add(data);
            }
        }

        private void InitializeBaseRectangle(ref Rectangle baseRect, List<PreferredSizeData> list)
        {
            foreach (PreferredSizeData data in list)
            {
                if ((bool)data.Element.GetValue(IsTabStripBaseProperty))
                {
                    Size baseSize = data.PreferredSize;

                    if (this.TabsPosition == TabPositions.Left || TabsPosition == TabPositions.Right)
                    {
                        int stripWidth = maxWidth + this.Padding.Horizontal;
                        baseSize.Height = this.AvailableSize.Height;
						if (this.StretchBaseMode == TabBaseStretchMode.StretchToRemainingSpace)
                            baseSize.Width = Math.Max(this.AvailableSize.Width - stripWidth, 7);
						else
                            baseSize.Width = Math.Max(baseSize.Width, 7); // RIBBONBAR FIX: this.AvailableSize.Width - stripWidth);

                        if (this.TabsPosition == TabPositions.Left)
                            baseRect = new Rectangle(this.Bounds.X + maxWidth + this.Padding.Left - 2, 0, baseSize.Width, baseSize.Height);
                        else
                            baseRect = new Rectangle(2, 0, baseSize.Width, baseSize.Height);
                    }
                    else
                    {
                        int stripHeight = maxHeight + this.Padding.Vertical;
                        baseSize.Width = this.AvailableSize.Width;
						if (this.StretchBaseMode == TabBaseStretchMode.StretchToRemainingSpace)
                            baseSize.Height = Math.Max(this.AvailableSize.Height - stripHeight, 7);
						else
                            baseSize.Height = Math.Max(baseSize.Height, 7); // RIBBONBAR FIX: this.AvailableSize.Height - stripHeight);

                        if (this.TabsPosition == TabPositions.Top)
                            baseRect = new Rectangle(this.Bounds.X, maxHeight + this.Padding.Top - 2, baseSize.Width, baseSize.Height);
                        else
                            baseRect = new Rectangle(this.Bounds.X, 2, baseSize.Width, baseSize.Height);
                    }
					if (this.StretchBaseMode == TabBaseStretchMode.NoStretch)
					{
                        if (this.TabsPosition == TabPositions.Left || TabsPosition == TabPositions.Right)
                            baseRect.Height = this.Size.Height;
                        else
						    baseRect.Width = this.Size.Width;
					}
                    data.Element.Bounds = baseRect;
                    break;
                }
            }
         
        
        }

		private Size GetMaxSize(List<PreferredSizeData> list)
		{
			Size maxSize = Size.Empty;

			foreach (PreferredSizeData data in list)
			{
				if ((bool) data.Element.GetValue(IsTabStripBaseProperty) ||
					(bool) data.Element.GetValue(IsTabStripHeadProperty) ||
					(bool) data.Element.GetValue(IsLeftScrollButtonProperty) ||
					(bool) data.Element.GetValue(IsRightScrollButtonProperty) ||
					(bool) data.Element.GetValue(IsUpScrollButtonProperty) ||
					(bool) data.Element.GetValue(IsDownScrollButtonProperty) ||
					(bool) data.Element.GetValue(IsDropDownButtonProperty))
					continue;

				Size realSize = data.PreferredSize;

				maxSize.Width = Math.Max(maxSize.Width, realSize.Width);
				maxSize.Height = Math.Max(maxSize.Height, realSize.Height);
			}

			return maxSize;
		}

		int nextRight = 0;

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
            base.OnPropertyChanged(e);

            if (this.ElementState != ElementState.Loaded || !this.UseNewLayoutSystem)
            {
                return;
            }

			if (e.Property == ShrinkModeProperty)
			{
                this.ResetProportions();
				this.ScrollItemsOffset = 0;
			}
            else if (e.Property == LayoutOverlapProperty)
            {
                this.UpdateLayoutOverlap((TabPositions)this.GetValue(TabsPositionProperty));
            }
            else if (e.Property == AllTabsEqualHeightProperty)
            {
                this.StretchTabItems(this.Orientation);
                this.SetTabAlignment();
                this.ScrollItemsOffset = 0;
            }
            else if (e.Property == TextOrientationProperty)
            {
                this.SetTabAlignment();
                this.ScrollItemsOffset = 0;
            }
            else if (e.Property == TabsPositionProperty)
            {
                this.UpdateTabsPosition((TabPositions)e.NewValue);
            }
            else if (e.Property == ItemsOverlapFactorProperty)
            {
                this.ScrollItemsOffset = 0;
                this.SetTabAlignment();
            }
		}

        /// <commentsfrom cref="Telerik.WinControls.Layouts.IRadLayoutElement.PerformLayout" filter=""/>
        public override void PerformLayoutCore(RadElement affectedElement)
        {
            List<PreferredSizeData> list = new List<PreferredSizeData>();

            this.InitializePreferredSizeDataList(list);

            Rectangle baseRect = Rectangle.Empty;
            this.InitializeBaseRectangle(ref baseRect, list);

            if (!this.ShrinkMode)
            {
                this.CalculateTabsPositions(list, baseRect);

                this.SetScrollingBehaviour(baseRect);

                this.SetDropDownButtonPosition(baseRect);
            }
            else
            {
                this.CalculateShrinkTabsPositions(list, baseRect);

            }
        }

        private void UpdateLayoutOverlap(TabPositions position)
        {
            this.parentTabStripElement.UpdateTabStripLayoutOverlap(position, LayoutOverlap);
        }

        private void UpdateTabsPosition(TabPositions position)
        {
            if (this.ParentTabStripElement == null)
            {
                return;
            }

            if (this.boxLayout== null)
            {
                return;
            }

            this.ScrollItemsOffset = 0;
            this.UpdateLayoutOverlap(position);

            switch (position)
            {
                case TabPositions.Top:
                    this.boxLayout.Orientation = Orientation.Horizontal;
                    DockLayoutPanel.SetDock(this, Telerik.WinControls.Layouts.Dock.Top);
                    StretchTabItems(Orientation.Horizontal);
                    this.boxLayout.StretchHorizontally = true;
                    this.boxLayout.StretchVertically = false;
                    break;
                case TabPositions.Bottom:
                    this.boxLayout.Orientation = Orientation.Horizontal;
                    DockLayoutPanel.SetDock(this, Telerik.WinControls.Layouts.Dock.Bottom);
                    StretchTabItems(Orientation.Horizontal);
                    this.boxLayout.StretchHorizontally = true;
                    this.boxLayout.StretchVertically = false;
                    break;
                case TabPositions.Left:
                    this.boxLayout.Orientation = Orientation.Vertical;
                    DockLayoutPanel.SetDock(this, Telerik.WinControls.Layouts.Dock.Left);
                    StretchTabItems(Orientation.Vertical);
                    this.boxLayout.StretchHorizontally = false;
                    this.boxLayout.StretchVertically = true;
                    break;
                case TabPositions.Right:
                    this.boxLayout.Orientation = Orientation.Vertical;
                    DockLayoutPanel.SetDock(this, Telerik.WinControls.Layouts.Dock.Right);
                    StretchTabItems(Orientation.Vertical);
                    this.boxLayout.StretchHorizontally = false;
                    this.boxLayout.StretchVertically = true;
                    break;
            }

            this.SetTabAlignment();
        }

        private void ResetProportions()
        {
            if (this.parentTabStripElement == null || this.parentTabStripElement.IsDisposing || this.parentTabStripElement.IsDisposed)
            {
                return;
            }

            foreach (TabItem item in this.parentTabStripElement.Items)
            {
                BoxLayout.SetProportion(item, 0);
            }
        }

		private void CalculateScrollButtonsSize(List<PreferredSizeData> list, Size maxItemSize )
		{
			foreach (PreferredSizeData data in list)
			{
				if ((data.Element is RadRepeatScrollButtonElement)
					|| ((bool)data.Element.GetValue(IsDropDownButtonProperty)))
				{
					Size size = maxItemSize;
					int sizeToSet = 0;
					if (maxItemSize.Width == 0)
						maxItemSize = new Size(20, 20);
					sizeToSet = Math.Min(size.Width, size.Height);
					sizeToSet = Math.Max(sizeToSet, 30);
					sizeToSet -= 10;

					if (data.Element.Visibility == ElementVisibility.Visible)
					{
						if ((bool)(data.Element.GetValue(IsDropDownButtonProperty)))
							if ((data.Element as RadDropDownButtonElement) != null)
							{
								(data.Element as RadDropDownButtonElement).ArrowButtonMinSize =
									new Size(sizeToSet, sizeToSet);
							}

						data.Element.Size = new Size(sizeToSet, sizeToSet);
						data.Element.MinSize = new Size(sizeToSet, sizeToSet);
						data.Element.MaxSize = new Size(sizeToSet, sizeToSet);

						if (this.ParentTabStripElement != null)
							if (this.ParentTabStripElement.AutoScroll)
							{
								data.Element.Visibility = ElementVisibility.Visible;
							}
							else
							{
								if (!((bool)data.Element.GetValue(IsDropDownButtonProperty)))
									data.Element.Visibility = ElementVisibility.Hidden;
							}
					}
				}
			}
		}

		private int GetHorizontalOffset(PreferredSizeData data)
		{
			int x = 0;
			if (TelerikHelper.IsRightAligned(data.Element.Alignment))
			{
				nextRight -= (data.PreferredSize.Width + data.Element.Margin.Horizontal);
				x = nextRight + data.Element.Margin.Left + ScrollItemsOffset;
				x -= (leftScrollButtonData.PreferredSize.Width
				+ rightScrollButtonData.PreferredSize.Width );

				nextRight += this.ItemsOverlapFactor;
			}
			else
				x = nextLeft + ItemsOffset + ScrollItemsOffset;
			return x;
		}

		private int GetVerticalOffset(PreferredSizeData data)
		{
			int y = 0;
			if (TelerikHelper.IsBottomAligned(data.Element.Alignment))
			{
				nextDown -= (data.PreferredSize.Width + data.Element.Margin.Horizontal);
				y = nextDown + data.Element.Margin.Left + ScrollItemsOffset;
				if ( this.parentTabStripElement.AutoScroll )
				y -= ( upScrollButtonData.PreferredSize.Height
					+ downScrollButtonData.PreferredSize.Height );
				nextDown += this.ItemsOverlapFactor;
			}
			else
				y = nextLeft + ItemsOffset + ScrollItemsOffset;
			return y;
		}

		private bool IsTabItemSizeData(PreferredSizeData data)
		{
			return (!(bool)data.Element.GetValue(IsTabStripBaseProperty) &&
					data != leftScrollButtonData && data != rightScrollButtonData &&
					data != upScrollButtonData && data != downScrollButtonData &&
					data != overflowButtonData);
		}

		private void CalculateShrinkTabsPositions(List<PreferredSizeData> list, Rectangle baseRect)
		{
            if (!this.ParentTabStripElement.IsInValidState(true))
            {
                return;
            }

			nextLeft = 0;
			nextRight = this.Size.Width;
			this.nextDown = this.Size.Height;
			itemsSize = ItemsOffset;
			Size maxItemSize = GetMaxSize(list);

			this.leftScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
			this.rightScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
			this.upScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
			this.downScrollButtonData.Element.Visibility = ElementVisibility.Hidden;

			foreach (PreferredSizeData data in list)
			{
				if (IsTabItemSizeData(data))
				{
					itemsSize += data.PreferredSize.Width + data.Element.Margin.Horizontal - this.ItemsOverlapFactor;
				}
			}

			int parentSz = 0;
			if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
			{
				parentSz = this.ParentTabStripElement.ElementTree.Control.Bounds.Width;
			}
			else
			{
				parentSz = this.ParentTabStripElement.ElementTree.Control.Bounds.Height;
			}

			if (itemsSize > parentSz)
			{
				SetShrinkTabsPositions(list, baseRect, maxItemSize, parentSz);
			}
			else
			{
				this.CalculateTabsPositions(list, baseRect);
			}
		}

		private void SetShrinkTabsPositions(List<PreferredSizeData> list, Rectangle baseRect, Size maxItemSize, int parentSz)
		{
			int divideValue = this.ParentTabStripElement.Items.Count;

			if (divideValue <= 0)
			{
				divideValue = 1;
			}
			int preferredWidth = (divideValue * this.ItemsOverlapFactor + parentSz - this.ItemsOffset) / divideValue;
			if (preferredWidth < 23)
			{
				preferredWidth = 23;
			}
			
			foreach (PreferredSizeData data in list)
			{
				if (IsTabItemSizeData(data))
				{
					SetShrinkTabBounds(baseRect, maxItemSize, preferredWidth, data);
				}
			}
		}

		private void SetShrinkTabBounds(Rectangle baseRect, Size maxItemSize, int preferredWidth, PreferredSizeData data)
		{
			bool isTabStripHead = (bool)data.Element.GetValue(IsTabStripHeadProperty);

			int preferredHeight = this.AllTabsEqualHeight ? maxItemSize.Height : data.PreferredSize.Height;

			if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
			{
				int x = GetHorizontalOffset(data);

				if (this.TabsPosition == TabPositions.Top)
				{
					if (isTabStripHead)
					{
						data.Element.Bounds = new Rectangle(0, 0, this.Size.Width, maxItemSize.Height);
					}
					else
					{
						data.Element.Bounds = new Rectangle(x, maxItemSize.Height - preferredHeight,
							Math.Min(preferredWidth, data.PreferredSize.Width), preferredHeight);
					}
				}
				else
				{
					if (isTabStripHead)
					{
						data.Element.Bounds = new Rectangle(0, baseRect.Height, this.Size.Width, maxItemSize.Height);
					}
					else
					{
						data.Element.Bounds = new Rectangle(x, baseRect.Height,
							Math.Min(preferredWidth, data.PreferredSize.Width), preferredHeight);
					}
				}
			}
			else
			{
				int y = GetVerticalOffset(data);
				int x = this.AllTabsEqualHeight ? 0 : maxItemSize.Height - preferredHeight;

				if (this.TabsPosition == TabPositions.Left)
				{
					if (isTabStripHead)
					{
						data.Element.Bounds = new Rectangle(0, 0, maxItemSize.Height, this.Size.Height);
					}
					else
					{
						data.Element.Bounds = new Rectangle(x, y,
							Math.Min(preferredWidth, data.PreferredSize.Width), preferredHeight);
					}
				}
				else
				{
					if (isTabStripHead)
					{
						data.Element.Bounds = new Rectangle(baseRect.Width, 0, maxItemSize.Height, this.Size.Height);
					}
					else
					{
						data.Element.Bounds = new Rectangle(baseRect.Width, y,
							Math.Min(preferredWidth, data.PreferredSize.Width), preferredHeight);
					}
				}
			}

			if (this.TabsPosition == TabPositions.Left || TabsPosition == TabPositions.Right)
			{
				if (!TelerikHelper.IsBottomAligned(data.Element.Alignment))
				{
					nextLeft += Math.Min(preferredWidth, data.Element.BoundingRectangle.Height) + data.Element.Margin.Vertical - this.ItemsOverlapFactor;
				}
			}
			else
			{
				if (!TelerikHelper.IsRightAligned(data.Element.Alignment))
				{
					nextLeft += Math.Min(preferredWidth, data.PreferredSize.Width) + data.Element.Margin.Horizontal - this.ItemsOverlapFactor;
				}
			}
		}

        private void CalculateTabsPositions(List<PreferredSizeData> list, Rectangle baseRect)
        {
			nextLeft = 0;
			nextRight = this.Size.Width;
			this.nextDown = this.Size.Height;
            itemsSize = ItemsOffset;
			Size maxItemSize = GetMaxSize(list);

			CalculateScrollButtonsSize(list, maxItemSize);
	        foreach (PreferredSizeData data in list)
            {
				if (IsTabItemSizeData(data))
                {
					bool isTabStripHead = (bool) data.Element.GetValue(IsTabStripHeadProperty);

					int preferredHeight = this.AllTabsEqualHeight ? maxItemSize.Height : data.PreferredSize.Height;

					if (this.TabsPosition == TabPositions.Bottom)
					{
						int x = GetHorizontalOffset(data);

						if (isTabStripHead)
							data.Element.Bounds = new Rectangle(0, baseRect.Height,
                                this.Size.Width, this.Size.Height - baseRect.Height);
						else
							data.Element.Bounds = new Rectangle(x, baseRect.Height,
								data.PreferredSize.Width, preferredHeight);
					}
					else if (this.TabsPosition == TabPositions.Left)
					{
						int y = GetVerticalOffset(data);
						int x = this.AllTabsEqualHeight ? 0 : maxItemSize.Height - preferredHeight;

						if (isTabStripHead)
							data.Element.Bounds = new Rectangle(0, 0,
                                this.Size.Width - baseRect.Width, this.Size.Height);
						else
							data.Element.Bounds = new Rectangle(x, y,
								data.PreferredSize.Width, preferredHeight);
					}
					else if (this.TabsPosition == TabPositions.Right)
					{
						int y = GetVerticalOffset(data);

						if (isTabStripHead)
							data.Element.Bounds = new Rectangle(baseRect.Width, 0,
                                this.Size.Width - baseRect.Width, this.Size.Height);
						else
							data.Element.Bounds = new Rectangle(baseRect.Width, y,
								data.PreferredSize.Width, preferredHeight);
					}
					else
					{
						int x = GetHorizontalOffset(data);

						if (isTabStripHead)
							data.Element.Bounds = new Rectangle(0, 0,
                                this.Size.Width, this.Size.Height - baseRect.Height);
						else
							data.Element.Bounds = new Rectangle(x, maxItemSize.Height - preferredHeight,
								data.PreferredSize.Width, preferredHeight);
					}

					if (isTabStripHead)
						continue;

                    itemsSize += data.PreferredSize.Width;

					if (this.TabsPosition == TabPositions.Left || TabsPosition == TabPositions.Right)
					{
						if (!TelerikHelper.IsBottomAligned(data.Element.Alignment))
							nextLeft += data.Element.BoundingRectangle.Height + data.Element.Margin.Vertical - this.ItemsOverlapFactor;
					}
					else
					{
						if (!TelerikHelper.IsRightAligned(data.Element.Alignment))
							nextLeft += data.Element.BoundingRectangle.Width + data.Element.Margin.Horizontal - this.ItemsOverlapFactor;
					}
                }

			}

	    }


        private void SetLeftRightScrollButtonsBehaviour(Rectangle baseRect)
        {
            if ((leftScrollButtonData != null) && ( rightScrollButtonData != null ))
            {
                if ((this.TabsPosition == TabPositions.Top) || (this.TabsPosition == TabPositions.Bottom))
                {
                    upScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
                    downScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
					if (baseRect.Right >= this.Size.Width - this.ItemsOffset - leftScrollButtonData.Element.Size.Width)
                    {
                        leftScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
                        rightScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
                    }
                    else
                    {
						if (this.ParentTabStripElement.AutoScroll)
						{
							leftScrollButtonData.Element.Visibility = ElementVisibility.Visible;
							rightScrollButtonData.Element.Visibility = ElementVisibility.Visible;
						}
						
					}
           
                    switch (this.TabScrollButtonsPosition)
                    {
                        case TabScrollButtonsPosition.RightBottom:
                            {
								if (this.TabsPosition == TabPositions.Top)
								{
									leftScrollButtonData.Element.Location = new Point(
								 baseRect.Right - leftScrollButtonData.PreferredSize.Width - rightScrollButtonData.PreferredSize.Width,

								 maxHeight - leftScrollButtonData.PreferredSize.Height);


									rightScrollButtonData.Element.Location = new Point(
											baseRect.Right - rightScrollButtonData.PreferredSize.Width,
											maxHeight - rightScrollButtonData.PreferredSize.Height);

								}
								else
								{
									leftScrollButtonData.Element.Location = new Point(
								 baseRect.Right - leftScrollButtonData.PreferredSize.Width - rightScrollButtonData.PreferredSize.Width,
								 
								 baseRect.Height);


									rightScrollButtonData.Element.Location = new Point(
											baseRect.Right - rightScrollButtonData.PreferredSize.Width,
											baseRect.Height);

								}

								break;
                            }
                        case TabScrollButtonsPosition.BothSides:
                            {
								if (this.TabsPosition == TabPositions.Top)
								{
									leftScrollButtonData.Element.Location = new Point(
								 0,
								 maxHeight - leftScrollButtonData.PreferredSize.Height);

									rightScrollButtonData.Element.Location = new Point(
											Math.Min(baseRect.Right - rightScrollButtonData.PreferredSize.Width,
													nextLeft + ItemsOffset),
											maxHeight - rightScrollButtonData.PreferredSize.Height);
								}
								else
								{
									leftScrollButtonData.Element.Location = new Point(
							 0,
								 baseRect.Height );

									rightScrollButtonData.Element.Location = new Point(
											Math.Min(baseRect.Right - rightScrollButtonData.PreferredSize.Width,
													nextLeft + ItemsOffset),
									 baseRect.Height );
							
								}
                                break;
                           
                            }
                        case TabScrollButtonsPosition.LeftTop:
                            {
								if (this.TabsPosition == TabPositions.Top)
								{
									leftScrollButtonData.Element.Location = new Point(
								 0,
								 maxHeight - leftScrollButtonData.PreferredSize.Height);

									rightScrollButtonData.Element.Location = new Point(
											leftScrollButtonData.PreferredSize.Width,
											maxHeight - rightScrollButtonData.PreferredSize.Height);
								}
								else
								{
									leftScrollButtonData.Element.Location = new Point(
								 0,
								 baseRect.Height );

									rightScrollButtonData.Element.Location = new Point(
											leftScrollButtonData.PreferredSize.Width,
											baseRect.Height );
								}
                                break;                        
                            }
                    }
              }
            }
        }

        private void SetUpDownScrollButtonsBehaviour(Rectangle baseRect)
        {
            if (upScrollButtonData != null && rightScrollButtonData != null)
            {

                if ((this.TabsPosition == TabPositions.Left) || (this.TabsPosition == TabPositions.Right))
                {
                    leftScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
                    rightScrollButtonData.Element.Visibility = ElementVisibility.Hidden;

					if (baseRect.Bottom >= this.Size.Height - this.ItemsOffset - this.upScrollButtonData.Element.Size.Height)
					{
						upScrollButtonData.Element.Visibility = ElementVisibility.Hidden;
						downScrollButtonData.Element.Visibility = ElementVisibility.Hidden;

					}
					else
					{
						if (this.ParentTabStripElement.AutoScroll)
						{

							upScrollButtonData.Element.Visibility = ElementVisibility.Visible;
							downScrollButtonData.Element.Visibility = ElementVisibility.Visible;
						}
					}

                    int x = baseRect.Right - upScrollButtonData.PreferredSize.Width;

                    switch (this.TabScrollButtonsPosition)
                    {
                        case TabScrollButtonsPosition.RightBottom:
                            {
								if (this.TabsPosition == TabPositions.Left)
								{
									
									upScrollButtonData.Element.Location = new Point(
												   tabStripHead.Element.Size.Width - upScrollButtonData.PreferredSize.Width,
													baseRect.Height - downScrollButtonData.Element.Size.Width
												   - upScrollButtonData.PreferredSize.Width);

									downScrollButtonData.Element.Location = new Point(
										  tabStripHead.Element.Size.Width - downScrollButtonData.Element.Size.Width,
										baseRect.Height - downScrollButtonData.PreferredSize.Width);
								}
								else
								{
									upScrollButtonData.Element.Location = new Point(
												   x + upScrollButtonData.PreferredSize.Width,
													baseRect.Height - downScrollButtonData.Element.Size.Width
												   - upScrollButtonData.PreferredSize.Width);

									downScrollButtonData.Element.Location = new Point(
										x + downScrollButtonData.PreferredSize.Width,
										baseRect.Height - downScrollButtonData.PreferredSize.Width);
								
								}
                                break;
                            }
                        case TabScrollButtonsPosition.LeftTop:
                            {
								if (this.TabsPosition == TabPositions.Left)
								{
									upScrollButtonData.Element.Location = new Point(
												   tabStripHead.Element.Size.Width - upScrollButtonData.PreferredSize.Width,
												  baseRect.Top);

									downScrollButtonData.Element.Location = new Point(
										   tabStripHead.Element.Size.Width - upScrollButtonData.PreferredSize.Width
										  ,baseRect.Top + upScrollButtonData.PreferredSize.Width);
								}
								else
								{
									upScrollButtonData.Element.Location = new Point(
												  x + upScrollButtonData.PreferredSize.Width,
												  baseRect.Top);

									downScrollButtonData.Element.Location = new Point(
											x + downScrollButtonData.PreferredSize.Width,
											 baseRect.Top + upScrollButtonData.PreferredSize.Width);
								
								}
                                break;
                            }

                        case TabScrollButtonsPosition.BothSides:
                            {
								if (this.TabsPosition == TabPositions.Left)
								{
									upScrollButtonData.Element.Location = new Point(
										   tabStripHead.Element.Size.Width - upScrollButtonData.PreferredSize.Width,
										baseRect.Top);

									downScrollButtonData.Element.Location = new Point(
										tabStripHead.Element.Size.Width - upScrollButtonData.PreferredSize.Width,
										baseRect.Height - downScrollButtonData.PreferredSize.Width);
								}
								else
								{
									upScrollButtonData.Element.Location = new Point(
												  x + upScrollButtonData.PreferredSize.Width,
												  baseRect.Top);

									downScrollButtonData.Element.Location = new Point(
									  x + downScrollButtonData.PreferredSize.Width,
									   baseRect.Height - downScrollButtonData.PreferredSize.Width);
							
								}
                                break;
                            }
                    }
                }
            }
        }

        private void SetScrollingBehaviour(Rectangle baseRect)
        {
			if ((this.TabsPosition == TabPositions.Top) || (this.TabsPosition == TabPositions.Bottom))
			{
				SetLeftRightScrollButtonsBehaviour(baseRect);
			}
			else
				SetUpDownScrollButtonsBehaviour(baseRect);
        }

		private void SetHorizontalDropDownButton(Rectangle baseRect)
		{
			if (overflowButtonData == null)
				return;

			if (this.TabDropDownButtonPosition == TabItemsDropDownButtonPosition.RightBottom)
			{
				if (( leftScrollButtonData != null && rightScrollButtonData != null ) && (leftScrollButtonData.Element.Visibility == ElementVisibility.Visible)
					&& (this.TabScrollButtonsPosition == TabScrollButtonsPosition.RightBottom))
				{
					if (this.TabsPosition == TabPositions.Top)
					{
						overflowButtonData.Element.Location = new Point(
							Math.Min(baseRect.Right - overflowButtonData.PreferredSize.Width - leftScrollButtonData.PreferredSize.Width - rightScrollButtonData.PreferredSize.Width,
								nextLeft + ItemsOffset),
							maxHeight - overflowButtonData.PreferredSize.Height);
					}
					else
					{
						overflowButtonData.Element.Location = new Point(
							Math.Min(baseRect.Right - overflowButtonData.PreferredSize.Width - leftScrollButtonData.PreferredSize.Width - rightScrollButtonData.PreferredSize.Width,
								nextLeft + ItemsOffset),
							 baseRect.Height);
					
					}
				}
				else if ((leftScrollButtonData != null && rightScrollButtonData != null) && (leftScrollButtonData.Element.Visibility == ElementVisibility.Visible) &&
					(this.TabScrollButtonsPosition == TabScrollButtonsPosition.BothSides))
				{
					if (this.TabsPosition == TabPositions.Top)
					{

						overflowButtonData.Element.Location = new Point(
							Math.Min(baseRect.Right - overflowButtonData.PreferredSize.Width - rightScrollButtonData.PreferredSize.Width,
								nextLeft + ItemsOffset),
							maxHeight - overflowButtonData.PreferredSize.Height);
					}
					else
					{
						overflowButtonData.Element.Location = new Point(
							Math.Min(baseRect.Right - overflowButtonData.PreferredSize.Width - rightScrollButtonData.PreferredSize.Width,
								nextLeft + ItemsOffset),
							 baseRect.Height);
			
					}
				}
				else 
				{
					if (this.TabsPosition == TabPositions.Top)
					{

						overflowButtonData.Element.Location = new Point(
							Math.Min(baseRect.Right - this.overflowButtonData.PreferredSize.Width,
								nextLeft + ItemsOffset),
							maxHeight - overflowButtonData.PreferredSize.Height);
					}
					else
					{
						overflowButtonData.Element.Location = new Point(
							Math.Min(baseRect.Right - this.overflowButtonData.PreferredSize.Width,
								nextLeft + ItemsOffset),
							 baseRect.Height);
				
					}
				}
			}

			else
			{
				if ((leftScrollButtonData != null && rightScrollButtonData != null) && (leftScrollButtonData.Element.Visibility == ElementVisibility.Visible)
					&& (this.TabScrollButtonsPosition == TabScrollButtonsPosition.LeftTop))
				{
					if (this.TabsPosition == TabPositions.Top)
					{
						overflowButtonData.Element.Location = new Point(
							  leftScrollButtonData.PreferredSize.Width + rightScrollButtonData.PreferredSize.Width,
							  maxHeight - this.overflowButtonData.PreferredSize.Height);
					}
					else
					{
						overflowButtonData.Element.Location = new Point(
					  leftScrollButtonData.PreferredSize.Width + rightScrollButtonData.PreferredSize.Width,
					  baseRect.Height);
				
					}
				}
				else if ((leftScrollButtonData != null && rightScrollButtonData != null) && (leftScrollButtonData.Element.Visibility == ElementVisibility.Visible) &&
					(this.TabScrollButtonsPosition == TabScrollButtonsPosition.BothSides))
				{
					if (this.TabsPosition == TabPositions.Top)
					{

						overflowButtonData.Element.Location = new Point(
							  leftScrollButtonData.PreferredSize.Width,
						maxHeight - this.overflowButtonData.PreferredSize.Height);
					}
					else
					{
						overflowButtonData.Element.Location = new Point(
							  leftScrollButtonData.PreferredSize.Width,
						 baseRect.Height);
					
					}
				}
				else
				{
					if (this.TabsPosition == TabPositions.Top)
					{

						overflowButtonData.Element.Location = new Point(
							  0,
							  maxHeight - this.overflowButtonData.PreferredSize.Height);

					}
					else
					{
						overflowButtonData.Element.Location = new Point(
							  0,
							  baseRect.Height);

					}
				}
			}
		}

        private void SetDropDownButtonPosition(Rectangle baseRect)
        {
            if ((this.TabsPosition == TabPositions.Top) || (this.TabsPosition == TabPositions.Bottom))
            {
				SetHorizontalDropDownButton(baseRect);
            }
            else
            {
				int x = baseRect.Right + overflowButtonData.PreferredSize.Width;
                if (this.TabsPosition == TabPositions.Left)
                {
					x = baseRect.Left;
                }

                switch (this.TabDropDownButtonPosition)
                {

                    case TabItemsDropDownButtonPosition.RightBottom:
                        {
                            if ((downScrollButtonData.Element.Visibility == ElementVisibility.Visible)
                                && (this.TabScrollButtonsPosition == TabScrollButtonsPosition.RightBottom))
                            {
                              
                                this.overflowButtonData.Element.Location = new Point(
												 x - overflowButtonData.PreferredSize.Width,
                                                baseRect.Height - downScrollButtonData.Element.Size.Width
                                               - upScrollButtonData.PreferredSize.Width
                                               - overflowButtonData.PreferredSize.Width);


                            }
                            else
                                if ((downScrollButtonData.Element.Visibility == ElementVisibility.Visible)
                                      && (this.TabScrollButtonsPosition == TabScrollButtonsPosition.BothSides))
                                {
                                    this.overflowButtonData.Element.Location = new Point(
													  x - overflowButtonData.PreferredSize.Width,
                                                      baseRect.Height - downScrollButtonData.Element.Size.Width
                                                     - overflowButtonData.PreferredSize.Width);


                                }
                                else
                                {
                                    this.overflowButtonData.Element.Location = new Point(
                                                         x - overflowButtonData.PreferredSize.Width,
                                                           baseRect.Height
                                                          - overflowButtonData.PreferredSize.Width);
                                }
                            break;
                        }

                    case TabItemsDropDownButtonPosition.LeftTop:
                        {
                            if ((downScrollButtonData.Element.Visibility == ElementVisibility.Visible)
                               && (this.TabScrollButtonsPosition == TabScrollButtonsPosition.RightBottom))
                            {

                                this.overflowButtonData.Element.Location = new Point(
                                                             x - overflowButtonData.PreferredSize.Width,
                                                               baseRect.Top
                                                              );

                            }
                            else
                                if ((downScrollButtonData.Element.Visibility == ElementVisibility.Visible)
                                      && (this.TabScrollButtonsPosition == TabScrollButtonsPosition.BothSides))
                                {
                                    this.overflowButtonData.Element.Location = new Point(
													 x - overflowButtonData.PreferredSize.Width,
                                                      baseRect.Top + upScrollButtonData.Element.Size.Width
                                                     );


                                }
                                else
                                {
									if (this.ParentTabStripElement != null)
									{
										if (this.upScrollButtonData.Element.Visibility == ElementVisibility.Visible)
										{
											this.overflowButtonData.Element.Location = new Point(
															 x - overflowButtonData.PreferredSize.Width,
															   baseRect.Top
										+ upScrollButtonData.Element.Size.Width
															   + downScrollButtonData.Element.Size.Width);

										}
										else
										{
											this.overflowButtonData.Element.Location = new Point(
									   x - overflowButtonData.PreferredSize.Width,
													 baseRect.Top);

										}
									}
                                }
                            break;
                        }
                }
            }
        }

        ////KIRO

		#region Fields

		private BoxLayout boxLayout;
		private RadElement topScrollButton;
		private RadElement bottomScrollButton;
		private RadElement leftScrollButton;
		private RadElement rightScrollButton;
		private RadElement dropDownButton;

		#endregion

		public static RadProperty LayoutOverlapProperty = RadProperty.Register(
			"LayoutOverlap", typeof(int), typeof(TabLayoutPanel), new RadElementPropertyMetadata(
				2, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		/// <summary>
		/// Gets the type of layout panel to be used for matching with the theme
		/// </summary>
		protected override Type ThemeEffectiveType
		{
			get
			{
				return typeof(TabLayoutPanel);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int LayoutOverlap
		{
			get
			{
				return (int)this.GetValue(LayoutOverlapProperty);
			}
			set
			{
				this.SetValue(LayoutOverlapProperty, value);
			}
		}

		private void Items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }

            this.StretchTargetItem(this.Orientation, (TabItem)target);
            this.ScrollItemsOffset = 0;
		}

		internal BoxLayout BoxLayout
		{
			get
			{
				return this.boxLayout;
			}
			set
			{
				this.boxLayout = value;
            }
		}

		internal RadElement BottomScrollButton
		{
			get
			{
				return this.bottomScrollButton;
			}
			set
			{
				this.bottomScrollButton = value;

			}
		}

		internal RadElement DropDownButton
		{
			get
			{
				return this.dropDownButton;
			}

			set
			{
				this.dropDownButton = value;
			}
		}

		internal RadElement TopScrollButton
		{
			get
			{
				return this.topScrollButton;
			}
			set
			{
				this.topScrollButton = value;

			}
		}

		internal RadElement LeftScrollButton
		{
			get
			{
				return this.leftScrollButton;
			}
			set
			{
				this.leftScrollButton = value;

			}
		}

		internal RadElement RightScrollButton
		{
			get
			{
				return this.rightScrollButton;
			}
			set
			{
				this.rightScrollButton = value;

			}
		}

		protected override SizeF MeasureOverride(SizeF availableSize)
		{
			SizeF sz = SizeF.Empty;
			foreach (RadElement element in this.Children)
			{
				if (element is BoxLayout)
                {
                    SizeF sizeForBoxLayout = new SizeF(float.PositiveInfinity, float.PositiveInfinity);
                    if (this.ShrinkMode)
                    {
                        if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
                        {
                            sizeForBoxLayout.Width = availableSize.Width - this.ItemsOffset;
                            sizeForBoxLayout.Height = availableSize.Height;
                        }
                        else
                        {
                            sizeForBoxLayout.Height = availableSize.Height - this.ItemsOffset;
                            sizeForBoxLayout.Width = availableSize.Width;
                        }

                        this.SetShrinkMode(true, availableSize);
                    }
                    element.Measure(sizeForBoxLayout);
                }
                else
                {
                    element.Measure(availableSize);
                }

                if (!float.IsInfinity(availableSize.Width) && !float.IsInfinity(availableSize.Height))
                {
                    if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
                    {
                        sz.Width = availableSize.Width;
                        sz.Height = Math.Max(element.DesiredSize.Height, sz.Height);
                    }
                    else
                    {
                        sz.Height = availableSize.Height;
                        sz.Width = Math.Max(element.DesiredSize.Width, sz.Width);
                    }
                }
                else
                {
                    if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
                    {
                        sz.Width += element.DesiredSize.Width - this.ItemsOverlapFactor;
                        sz.Height = Math.Max(element.DesiredSize.Height, sz.Height);
                    }
                    else
                    {
                        sz.Height += element.DesiredSize.Height - this.ItemsOverlapFactor;
                        sz.Width = Math.Max(element.DesiredSize.Width, sz.Width);
                    }
                }
			}

            if (float.IsInfinity(availableSize.Width) && float.IsInfinity(availableSize.Height))
            {
                if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
                {
                    sz.Width += this.ItemsOffset;
                }
                else
                {
                    sz.Height += this.ItemsOffset;
                }
            }

            //TODO: Temporary hack to force the tabstrip to perform additional arrange pass upon measure
            if (this.parentTabStripElement != null && this.parentTabStripElement.OverflowMode == TabStripItemOverflowMode.Hide)
            {
                this.parentTabStripElement.InvalidateArrange();
            }
			return sz;
		}

		private void SetScrollButtonsBouunds(bool rightLeft, bool topBottom)
		{
			if (rightLeft)
			{
				ArrangeRightLeftScrollButtons();
				return;
			}
			else
			{
				ArrangeTopBottomtScrollButtons();
			}
		}


		private void ArrangeTopBottomtScrollButtons()
		{
			float xOffset = 0;

			if (this.TabsPosition == TabPositions.Left)
				xOffset = this.boxLayout.DesiredSize.Width - this.TopScrollButton.DesiredSize.Width;

			switch (this.TabScrollButtonsPosition)
			{
				case TabScrollButtonsPosition.RightBottom:

					this.TopScrollButton.Arrange(new RectangleF(
						xOffset,
						this.DesiredSize.Height -
						this.BottomScrollButton.DesiredSize.Height -
						this.TopScrollButton.DesiredSize.Height,
						this.TopScrollButton.DesiredSize.Width,
						this.TopScrollButton.DesiredSize.Height));

					this.BottomScrollButton.Arrange(new RectangleF(
						xOffset,
						this.DesiredSize.Height - this.BottomScrollButton.DesiredSize.Height,
						this.BottomScrollButton.DesiredSize.Width,
						this.BottomScrollButton.DesiredSize.Height));
					break;

				case TabScrollButtonsPosition.LeftTop:
					this.TopScrollButton.Arrange(new RectangleF(
						xOffset,
						0,
						this.TopScrollButton.DesiredSize.Width,
						this.TopScrollButton.DesiredSize.Height));

					this.BottomScrollButton.Arrange(new RectangleF(
						xOffset,
						this.BottomScrollButton.DesiredSize.Height,
						this.BottomScrollButton.DesiredSize.Width,
						this.BottomScrollButton.DesiredSize.Height));

					break;

				case TabScrollButtonsPosition.BothSides:
					this.TopScrollButton.Arrange(new RectangleF(
						xOffset,
						0,
						this.TopScrollButton.DesiredSize.Width,
						this.TopScrollButton.DesiredSize.Height));

					this.BottomScrollButton.Arrange(new RectangleF(
						xOffset,
						this.DesiredSize.Height - this.BottomScrollButton.DesiredSize.Height,
						this.BottomScrollButton.DesiredSize.Width,
						this.BottomScrollButton.DesiredSize.Height));

					break;
			}
		}

		private void SetDropDownButtonBounds(bool rightLeft, bool topBottom)
		{

			bool hasVisibleScrollButton = false;

			if ((this.leftScrollButton.Visibility == ElementVisibility.Visible)
				|| (this.rightScrollButton.Visibility == ElementVisibility.Visible)
				|| (this.topScrollButton.Visibility == ElementVisibility.Visible)
				|| (this.bottomScrollButton.Visibility == ElementVisibility.Visible))
			{
				hasVisibleScrollButton = true;
			}

			if (rightLeft)
			{
				ArrangeRightLeftDropDownButton(hasVisibleScrollButton);
			}
			else
			{
				ArrangeTopBottomDropDownButton(hasVisibleScrollButton);
			}
		}

		private void ArrangeTopBottomDropDownButton(bool hasVisibleScrollButton)
		{
			float xOffset = 0;

			if (this.TabsPosition == TabPositions.Left)
				xOffset = this.boxLayout.DesiredSize.Width - this.TopScrollButton.DesiredSize.Width;


			if (this.ParentTabStripElement.TabDropDownButtonPosition == TabItemsDropDownButtonPosition.RightBottom)
			{
				float yOffset = this.DesiredSize.Height - this.DropDownButton.DesiredSize.Height - 1;

				if (hasVisibleScrollButton)
				{

					switch (this.ParentTabStripElement.TabScrollButtonsPosition)
					{
						case TabScrollButtonsPosition.RightBottom:
							yOffset = this.DesiredSize.Height - this.DropDownButton.DesiredSize.Width - 1
							- this.RightScrollButton.DesiredSize.Width - this.LeftScrollButton.DesiredSize.Width;
							break;
						case TabScrollButtonsPosition.LeftTop:
							yOffset = this.DesiredSize.Height - this.DropDownButton.DesiredSize.Width - 1;
							break;
						case TabScrollButtonsPosition.BothSides:
							yOffset = this.DesiredSize.Height - this.DropDownButton.DesiredSize.Width - 1
							- this.RightScrollButton.DesiredSize.Width;
							break;
					}
				}

				Console.WriteLine("DropDown Button:" + yOffset);		
				this.DropDownButton.Arrange(new RectangleF(xOffset,
					yOffset,
					this.DropDownButton.DesiredSize.Width,
					this.DropDownButton.DesiredSize.Height));
			}
			else
			{
				float yOffset = 0;

				if (hasVisibleScrollButton)
				{
					switch (this.ParentTabStripElement.TabScrollButtonsPosition)
					{
						case TabScrollButtonsPosition.RightBottom:
							yOffset = 0;
							break;
						case TabScrollButtonsPosition.LeftTop:
							yOffset = this.RightScrollButton.DesiredSize.Height + this.LeftScrollButton.DesiredSize.Height;
							break;
						case TabScrollButtonsPosition.BothSides:
							yOffset = this.LeftScrollButton.DesiredSize.Height;
							break;
					}
				}

				this.DropDownButton.Arrange(new RectangleF(xOffset,
					yOffset,
					this.DropDownButton.DesiredSize.Width,
					this.DropDownButton.DesiredSize.Height));

			}
		}

		private void ArrangeRightLeftDropDownButton(bool hasVisibleScrollButton)
		{
			float yOffset = this.boxLayout.DesiredSize.Height - this.DropDownButton.DesiredSize.Height;

			if (this.TabsPosition == TabPositions.Bottom)
				yOffset = 0;

			if (this.ParentTabStripElement.TabDropDownButtonPosition == TabItemsDropDownButtonPosition.RightBottom)
			{
				float xOffset = this.DesiredSize.Width - this.DropDownButton.DesiredSize.Width - 1;

				if (hasVisibleScrollButton)
				{
					switch (this.ParentTabStripElement.TabScrollButtonsPosition)
					{
						case TabScrollButtonsPosition.RightBottom:
							xOffset = this.DesiredSize.Width - this.DropDownButton.DesiredSize.Width - 1
							- this.RightScrollButton.DesiredSize.Width - this.LeftScrollButton.DesiredSize.Width;
							break;
						case TabScrollButtonsPosition.LeftTop:
							xOffset = this.DesiredSize.Width - this.DropDownButton.DesiredSize.Width - 1;
							break;
						case TabScrollButtonsPosition.BothSides:
							xOffset = this.DesiredSize.Width - this.DropDownButton.DesiredSize.Width - 1
							- this.RightScrollButton.DesiredSize.Width;
							break;
					}
				}

				this.DropDownButton.Arrange(new RectangleF(xOffset,
					yOffset,
					this.DropDownButton.DesiredSize.Width,
					this.DropDownButton.DesiredSize.Height));
			}
			else
			{
				float xOffset = 0;

				if (hasVisibleScrollButton)
				{
					switch (this.ParentTabStripElement.TabScrollButtonsPosition)
					{
						case TabScrollButtonsPosition.RightBottom:
							xOffset = 0;
							break;
						case TabScrollButtonsPosition.LeftTop:
							xOffset = this.RightScrollButton.DesiredSize.Width + this.LeftScrollButton.DesiredSize.Width;
							break;
						case TabScrollButtonsPosition.BothSides:
							xOffset = this.LeftScrollButton.DesiredSize.Width;
							break;
					}
				}

				this.DropDownButton.Arrange(new RectangleF(xOffset,
					yOffset,
					this.DropDownButton.DesiredSize.Width,
					this.DropDownButton.DesiredSize.Height));

			}
		}

		private void ArrangeRightLeftScrollButtons()
		{
			float yOffset = this.boxLayout.BoundingRectangle.Bottom - this.RightScrollButton.DesiredSize.Height;

			if (this.TabsPosition == TabPositions.Bottom)
				yOffset = 0;

			switch (this.TabScrollButtonsPosition)
			{
				case TabScrollButtonsPosition.RightBottom:


					this.RightScrollButton.Arrange(new RectangleF(
						this.DesiredSize.Width - this.RightScrollButton.DesiredSize.Width - 1,
						yOffset,
						this.RightScrollButton.DesiredSize.Width,
						this.RightScrollButton.DesiredSize.Height));

					this.LeftScrollButton.Arrange(new RectangleF(
						this.DesiredSize.Width - 2 * this.LeftScrollButton.DesiredSize.Width - 2,
						yOffset,
						this.LeftScrollButton.DesiredSize.Width,
						this.LeftScrollButton.DesiredSize.Height));

					break;

				case TabScrollButtonsPosition.LeftTop:
					this.RightScrollButton.Arrange(new RectangleF(
					this.LeftScrollButton.DesiredSize.Width + 2,
						yOffset,
					this.RightScrollButton.DesiredSize.Width,
					this.RightScrollButton.DesiredSize.Height));

					this.LeftScrollButton.Arrange(new RectangleF(
						1,
				yOffset, this.LeftScrollButton.DesiredSize.Width,
						this.LeftScrollButton.DesiredSize.Height));
					break;

				case TabScrollButtonsPosition.BothSides:
					this.RightScrollButton.Arrange(new RectangleF(
						this.DesiredSize.Width - this.RightScrollButton.DesiredSize.Width - 1,
				yOffset, this.RightScrollButton.DesiredSize.Width,
						this.RightScrollButton.DesiredSize.Height));

					this.LeftScrollButton.Arrange(new RectangleF(
							1,
				yOffset,
				this.LeftScrollButton.DesiredSize.Width,
							this.LeftScrollButton.DesiredSize.Height));

					break;
			}
		}

		private void ShowScrollButtons(bool rightLeft, bool topBottom)
		{
			if (rightLeft)
			{
				this.rightScrollButton.Visibility = ElementVisibility.Visible;
				this.leftScrollButton.Visibility = ElementVisibility.Visible;

				this.topScrollButton.Visibility = ElementVisibility.Hidden;
				this.bottomScrollButton.Visibility = ElementVisibility.Hidden;

				return;
			}

			this.rightScrollButton.Visibility = ElementVisibility.Hidden;
			this.leftScrollButton.Visibility = ElementVisibility.Hidden;

			this.topScrollButton.Visibility = ElementVisibility.Visible;
			this.bottomScrollButton.Visibility = ElementVisibility.Visible;
		}

		protected override SizeF ArrangeOverride(SizeF finalSize)
		{
		
			this.RightScrollButton.Arrange(new RectangleF(
				PointF.Empty,
				this.RightScrollButton.DesiredSize));
			this.LeftScrollButton.Arrange(new RectangleF(
				PointF.Empty,
				this.RightScrollButton.DesiredSize));
			this.TopScrollButton.Arrange(new RectangleF(
				PointF.Empty,
				this.RightScrollButton.DesiredSize));
			this.BottomScrollButton.Arrange(new RectangleF(
				PointF.Empty,
				this.RightScrollButton.DesiredSize));
			this.DropDownButton.Arrange(new RectangleF(
				PointF.Empty,
				this.RightScrollButton.DesiredSize));

            RectangleF arrangeRect = new RectangleF(PointF.Empty, finalSize);
            this.parentTabStripElement.TabStripHeadFill.Arrange(arrangeRect);
            this.parentTabStripElement.TabStripHeadBorder.Arrange(arrangeRect);

            if (this.ParentTabStripElement.ElementTree != null && 
                (this.ParentTabStripElement.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes))
            {
                this.boxLayout.RightToLeft = true;
            }
            else
            {
                if (this.ParentTabStripElement.ElementTree != null)
                {
                    this.boxLayout.RightToLeft = false;
                }
            }

			if (this.TabsPosition == TabPositions.Top || this.TabsPosition == TabPositions.Bottom)
			{
				finalSize = ArrangeRightLeftChildren(finalSize);
			}
			else
			{
				finalSize = ArrangeTopBottomChildren(finalSize);
			}

			return finalSize;
		}


		private SizeF ArrangeTopBottomChildren(SizeF finalSize)
		{
			int yoffset = this.ScrollItemsOffset;
			if (this.ScrollItemsOffset == 0)
				yoffset += this.ItemsOffset;

            if (this.ShrinkMode)
            {
                this.boxLayout.Arrange(new RectangleF(new PointF(0, this.ItemsOffset),
                    new SizeF(finalSize.Width, finalSize.Height - this.ItemsOffset)));
            }
            else
            {
				if (this.ParentTabStripElement.ElementTree != null 
                    && (this.ParentTabStripElement.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                    && this.UseVerticalRTL)
				{
                    this.boxLayout.Arrange(new RectangleF(new PointF(0, -yoffset + finalSize.Height - this.boxLayout.DesiredSize.Height - (this.ItemsOverlapFactor * this.boxLayout.Children.Count)), new SizeF(
						this.boxLayout.DesiredSize.Width ,
						this.boxLayout.DesiredSize.Height - (this.ItemsOverlapFactor * this.boxLayout.Children.Count ))));
                }
				else
				{
					this.boxLayout.Arrange(new RectangleF(new PointF(0, yoffset), new SizeF(
						this.boxLayout.DesiredSize.Width ,
						this.boxLayout.DesiredSize.Height - (this.ItemsOverlapFactor * this.boxLayout.Children.Count))));  
				}
            }

			if ((this.ParentTabStripElement.AutoScroll) && (this.boxLayout.DesiredSize.Height >= this.DesiredSize.Height - this.ItemsOffset))
			{
				if (!this.ShrinkMode)
				{
					SetScrollButtonsBouunds(false, true);
					ShowScrollButtons(false, true);
				}
			}
			else
			{
				HideScrollButtons();
			}

			if (this.ParentTabStripElement.ShowOverFlowButton)
			{
				this.DropDownButton.Visibility = ElementVisibility.Visible;
				SetDropDownButtonBounds(false, true);
			}
			else
			{
				this.DropDownButton.Visibility = ElementVisibility.Collapsed;
			}

			return finalSize;
		}

		private SizeF ArrangeRightLeftChildren(SizeF finalSize)
		{
			int offset = 0;
			int scrolledItems = 0;
			int itemsOverLapFactor = GetScrolledItemsCount(ref offset, ref scrolledItems);

			int xoffset = this.ScrollItemsOffset;

			if (this.ScrollItemsOffset == 0)
				xoffset += this.ItemsOffset;

            if (this.ShrinkMode)
            {
              
                if ((this.ParentTabStripElement.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes))
                {
                    this.boxLayout.Arrange(new RectangleF(new PointF(0, 0),
        new SizeF(finalSize.Width - this.ItemsOffset, finalSize.Height)));
                }
                else
                {
                    this.boxLayout.Arrange(new RectangleF(new PointF(this.ItemsOffset, 0),
                       new SizeF(finalSize.Width - this.ItemsOffset, finalSize.Height)));
                }

              
            }
            else
            {
				if (this.ParentTabStripElement.ElementTree != null && 
				  (this.ParentTabStripElement.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes)) 
				{

					this.boxLayout.Arrange(new RectangleF(new PointF(-itemsOverLapFactor - xoffset +
						finalSize.Width - (this.boxLayout.DesiredSize.Width - this.ItemsOverlapFactor * this.boxLayout.Children.Count), 0),
						new SizeF(this.boxLayout.DesiredSize.Width - (this.ItemsOverlapFactor * this.boxLayout.Children.Count), this.boxLayout.DesiredSize.Height)));
				}
				else
				{
					this.boxLayout.Arrange(new RectangleF(new PointF(-itemsOverLapFactor + xoffset, 0),
						new SizeF(this.boxLayout.DesiredSize.Width - (this.ItemsOverlapFactor*this.boxLayout.Children.Count),
						this.boxLayout.DesiredSize.Height)));
				}
			}

			if  ((this.ParentTabStripElement.AutoScroll) && (this.boxLayout.DesiredSize.Width >= this.DesiredSize.Width - this.ItemsOffset))
			{
				if (!this.ShrinkMode)
				{
					SetScrollButtonsBouunds(true, false);
					ShowScrollButtons(true, false);
				}
			}
			else
			{
				HideScrollButtons();
			}

			if (this.ParentTabStripElement.ShowOverFlowButton)
			{
				this.DropDownButton.Visibility = ElementVisibility.Visible;
				SetDropDownButtonBounds(true, false);
			}
			else
			{
				this.DropDownButton.Visibility = ElementVisibility.Collapsed;
			}

			return finalSize;
		}

		private void HideScrollButtons()
        {
            if (this.LeftScrollButton.Visibility == ElementVisibility.Visible ||
                this.RightScrollButton.Visibility == ElementVisibility.Visible ||
                this.TopScrollButton.Visibility == ElementVisibility.Visible ||
                this.BottomScrollButton.Visibility == ElementVisibility.Visible)
            {
                this.LeftScrollButton.Visibility = ElementVisibility.Hidden;
                this.RightScrollButton.Visibility = ElementVisibility.Hidden;
                this.TopScrollButton.Visibility = ElementVisibility.Hidden;
                this.BottomScrollButton.Visibility = ElementVisibility.Hidden;
                this.ScrollItemsOffset = 0;
            }
        }

		private int GetScrolledItemsCount(ref int offset, ref int scrolledItems)
		{
			for (int i = 0; i < this.boxLayout.Children.Count; i++)
			{
				if (offset >= Math.Abs(this.ScrollItemsOffset))
					break;

				scrolledItems = i;
				offset += this.boxLayout.Children[i].Bounds.Width;
			}

			int itemsOverLapFactor = scrolledItems * this.ItemsOverlapFactor;
			return itemsOverLapFactor;
		}

		/// <summary>
		/// Gets or sets the docking of the tabs
		/// </summary>
		public TabPositions Dock
		{
			get
			{
				return (TabPositions)this.GetValue(TabsPositionProperty);
			}
			set
			{
				this.ParentTabStripElement.SetValue(TabsPositionProperty, value);
				this.SetValue(TabsPositionProperty, value);
				
			}
		}

		private void SetTabAlignment()
		{
			if (this.boxLayout != null)
				foreach (TabItem item in this.boxLayout.Children)
				{
					switch (TabsPosition)
					{
						case TabPositions.Top:
							{
								switch (item.Alignment)
								{
									case ContentAlignment.TopCenter:
									case ContentAlignment.MiddleCenter:
										item.Alignment = ContentAlignment.BottomCenter;
										break;
									case ContentAlignment.TopRight:
									case ContentAlignment.MiddleRight:
										item.Alignment = ContentAlignment.BottomRight;
										break;
									default:
										item.Alignment = ContentAlignment.BottomLeft;
										break;
								}
							}
							break;
						case TabPositions.Bottom:
							{
								switch (item.Alignment)
								{
									case ContentAlignment.BottomCenter:
									case ContentAlignment.MiddleCenter:
										item.Alignment = ContentAlignment.TopCenter;
										break;
									case ContentAlignment.BottomRight:
									case ContentAlignment.MiddleRight:
										item.Alignment = ContentAlignment.TopRight;
										break;
									default:
										item.Alignment = ContentAlignment.TopLeft;
										break;

								}
								break;
							}
						case TabPositions.Right:
							{
								switch (item.Alignment)
								{
									case ContentAlignment.BottomRight:
									case ContentAlignment.BottomCenter:
										item.Alignment = ContentAlignment.BottomLeft;
										break;
									case ContentAlignment.MiddleRight:
									case ContentAlignment.MiddleCenter:
										item.Alignment = ContentAlignment.MiddleLeft;
										break;
									default:
										item.Alignment = ContentAlignment.TopLeft;
										break;

								}
								break;
							}
						case TabPositions.Left:
							{
								switch (item.Alignment)
								{
									case ContentAlignment.BottomLeft:
									case ContentAlignment.BottomCenter:
										item.Alignment = ContentAlignment.BottomRight;
										break;
									case ContentAlignment.MiddleLeft:
									case ContentAlignment.MiddleCenter:
										item.Alignment = ContentAlignment.MiddleRight;
										break;
									default:
										item.Alignment = ContentAlignment.TopRight;
										break;
								}
							}

							break;
					}
				}
		}

        private int GetItemWeight(TabItem measuredItem)
        {
            int currentItemWeight = this.ParentTabStripElement.Items.Count;

            lock (MeasurementGraphics.SyncObject)
            {
                MeasurementGraphics g = MeasurementGraphics.CreateMeasurementGraphics();

                int measuredItemWidth = Size.Ceiling(g.Graphics.MeasureString(measuredItem.Text, measuredItem.Font)).Width + measuredItem.Padding.Horizontal;
                if (measuredItem.Image != null)
                {
                    measuredItemWidth += measuredItem.Image.Width;
                }

                foreach (TabItem item in this.ParentTabStripElement.Items)
                {
                    int itemWidth = Size.Ceiling(g.Graphics.MeasureString(item.Text, item.Font)).Width + item.Padding.Horizontal;
                    if (item.Image != null)
                    {
                        itemWidth += item.Image.Width;
                    }
                    if (measuredItemWidth > itemWidth)
                    {
                        currentItemWeight++;
                    }
                }

                g.Dispose();
            }
                      
            return currentItemWeight;
        }



        private int GetWidth()
        {
            int width = this.ItemsOffset;

            lock (MeasurementGraphics.SyncObject)
            {
                MeasurementGraphics g = MeasurementGraphics.CreateMeasurementGraphics();

                foreach (TabItem item in this.ParentTabStripElement.Items)
                {
                    SizeF sz = g.Graphics.MeasureString(item.Text, item.Font);
                    int itemImageOffset = 0;

                    if (item.Image != null)
                    {
                        itemImageOffset = item.Image.Width;
                    }

                    width += (int)sz.Width + item.Padding.Horizontal + itemImageOffset - ItemsOverlapFactor;
                }

                g.Dispose();
            }

            return width;
        }

        private void SetShrinkMode(bool shrinkMode, SizeF availableSize)
		{
            if (this.boxLayout == null)
            {
                return;
            }

            float itemsWidth = GetWidth();
            if (!shrinkMode)
            {
                this.HideScrollButtons();
            }

            Orientation orientation = this.Orientation;
            this.StretchTabItems(orientation);

            if (orientation == Orientation.Horizontal)
            {
                if (itemsWidth < availableSize.Width && shrinkMode)
                {
                    for (int i = 0; i < this.boxLayout.Children.Count; i++)
                    {
                        BoxLayout.SetProportion(this.boxLayout.Children[i], 0);
                    }
                    return;
                }

                this.boxLayout.StretchHorizontally = shrinkMode;
            }
            else
            {
                if (itemsWidth < availableSize.Height && shrinkMode)
                {
                    for (int i = 0; i < this.boxLayout.Children.Count; i++)
                    {
                        BoxLayout.SetProportion(this.boxLayout.Children[i], 0);
                    }
                    return;
                }

                this.boxLayout.StretchVertically = shrinkMode;
            }

            for (int i = 0; i < this.boxLayout.Children.Count; i++)
			{
                int itemWeight = GetItemWeight(this.boxLayout.Children[i] as TabItem);
                BoxLayout.SetProportion(this.boxLayout.Children[i], shrinkMode ? itemWeight : 0);
			}
		}

		private void StretchTargetItem(Orientation orientation, TabItem item)
		{
			if (item == null)
				return;

			if (this.boxLayout == null)
				return;

			if (orientation == Orientation.Horizontal)
			{
				StretchTopBottomItem(item);	
			}
			else
			{
				StretchRightLeftItem(item);				
			}

			SetTabAlignment();
		}

        private void StretchRightLeftItem(TabItem item)
        {

            if (this.AllTabsEqualHeight)
            {
                item.StretchHorizontally = true;
            }
            else
            {
                item.StretchHorizontally = false;

            }

            item.StretchVertically = false;

            //item.StretchVertically = this.ShrinkMode;
        }

        private void StretchTopBottomItem(TabItem item)
        {
            item.StretchHorizontally = false;
            if (this.AllTabsEqualHeight)
            {
                item.StretchVertically = true;
            }
            else
                item.StretchVertically = false;

            //item.layout.StretchHorizontally = true;
        }

		internal void StretchTabItems(Orientation orientation)
		{
			if (this.boxLayout == null)
				return;

			if (orientation == Orientation.Horizontal)
			{
				StretchTopBottomTabItems();
			}
			else
			{
				StretchRightLeftTabItems();
			}
		}

		private void StretchTopBottomTabItems()
		{
			foreach (RadItem item in this.boxLayout.Children)
			{
				StretchTopBottomItem((TabItem)item);
			}
		}

		private void StretchRightLeftTabItems()
		{
			foreach (RadItem item in this.boxLayout.Children)
			{
				StretchRightLeftItem((TabItem)item);
			}
		}
	}
}
