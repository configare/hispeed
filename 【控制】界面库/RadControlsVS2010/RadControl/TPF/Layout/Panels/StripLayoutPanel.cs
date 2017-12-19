using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.Layouts
{
    /// <summary>
    /// StripLayoutPanel is a container for elements. StripLayoutPanel orders the elements
    /// in a single line vertically or horizontally.
    /// <para>
    /// Extends %LayoutPanel:Telerik.WinControls.LayoutPanel%.
    /// </para>
    /// </summary>
    [DefaultProperty("Orientation")]
    public class StripLayoutPanel : LayoutPanel
    {
        #region Nested types
		protected enum StripPosition
        {
            First,
            Middle,
            Last
        }
        #endregion

        private int maxIntegralWidth = 0;
        private int maxIntegralHeight = 0;

        #region Properties
        public static readonly RadProperty OrientationProperty = RadProperty.Register(
			"Orientation", typeof(Orientation), typeof(StripLayoutPanel), new RadElementPropertyMetadata(
                Orientation.Horizontal, ElementPropertyOptions.CanInheritValue | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		/// <summary>
        /// Gets or sets strip orientation - it could be horizontal or vertical.
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory)]
        [RadPropertyDefaultValue("Orientation", typeof(StripLayoutPanel))]
        [Description("Orientation of the strip - could be horizontal or vertical")]
        public Orientation Orientation
		{
			get
			{
				return (Orientation)this.GetValue(OrientationProperty);
			}
			set
			{
				this.SetValue(OrientationProperty, value);
			}
		}

		public static readonly RadProperty ItemsOffsetProperty = RadProperty.Register(
            "ItemsOffset", typeof(Point), typeof(StripLayoutPanel), new RadElementPropertyMetadata(
                new Point(0, 0), ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>
        /// Gets or sets global offset for the containing items
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory)]
        [RadPropertyDefaultValue("ItemsOffset", typeof(StripLayoutPanel))]
        [Description("Offset that is applied to all children")]
        public Point ItemsOffset
        {
            get
            {
                return (Point)this.GetValue(ItemsOffsetProperty);
            }
            set
            {
                this.SetValue(ItemsOffsetProperty, value);
            }
        }

        private bool forceFitToSizeElements = false;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceFitToSizeElements
        {
            get { return forceFitToSizeElements; }
            set { forceFitToSizeElements = value; }
        } 

        // TODO: Remove ForceElementsWidth and ForceElementsHeight properties
        public static readonly RadProperty ForceElementsWidthProperty = RadProperty.Register(
            "ForceElementsWidth",
            typeof(bool),
            typeof(StripLayoutPanel),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceElementsWidth
        {
            get { return (bool)GetValue(ForceElementsWidthProperty); }
            set { SetValue(ForceElementsWidthProperty, value); }
        }

        public static readonly RadProperty ForceElementsHeightProperty = RadProperty.Register(
            "ForceElementsHeight",
            typeof(bool),
            typeof(StripLayoutPanel),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceElementsHeight
        {
            get { return (bool)GetValue(ForceElementsHeightProperty); }
            set { SetValue(ForceElementsHeightProperty, value); }
        }

        protected int MaxIntegralWidth
        {
            get { return this.maxIntegralWidth; }
        }

        protected int MaxIntegralHeight
        {
            get { return this.maxIntegralHeight; }
        }

        #endregion

        #region PerformLayoutCore

        /// <summary>
        /// Performs layout changes based on the element given as a paramater. 
        /// Sizes and places are determined by the concrete layout panel that is used. 
        /// Since all layout panels update their layout automatically through events, 
        /// this function is rarely used directly.
        /// </summary>
        /// <param name="affectedElement"></param>
        public override void PerformLayoutCore(RadElement affectedElement)
		{
			int nextLeftTop = 0, nextRightBottom = 0;
			Rectangle bounds = this.Bounds;
            bounds.Size = TelerikLayoutEngine.CheckSize(this.Size, this.MaxSize, this.MinSize);

			List<PreferredSizeData> prefSizelist = new List<PreferredSizeData>();
			FillPrefSizeList(prefSizelist, bounds);

            if (!this.ShouldIgnoreChildSizes())
			{
                bounds.Size = GetChildrenListSize(prefSizelist);
			}


			foreach (PreferredSizeData data in prefSizelist)
			{
                if (data.Element.Visibility != ElementVisibility.Collapsed)
                {
                    StripPosition pos = GetInvariantPosition(data.Element.Alignment);
                    switch (pos)
                    {
                        case StripPosition.First:
                            SetElementBoundsAuto(bounds, data, nextLeftTop);
                            nextLeftTop += GetInvariantLength(data.Element.BoundingRectangle.Size, data.Element.Margin);
                            break;
                        case StripPosition.Middle:
                            SetElementBoundsAuto(bounds, data, 0);
                            break;
                        case StripPosition.Last:
                            SetElementBoundsAuto(bounds, data, nextRightBottom);
							nextRightBottom -= GetInvariantLength(data.Element.BoundingRectangle.Size, data.Element.Margin);
                            break;
                    }
                }
			}
		}

        protected virtual IEnumerable<RadElement> GetChildrenForLayout()
        {
            return this.GetChildren(ChildrenListOptions.Normal);
        }

		public static readonly RadProperty EqualChildrenWidthProperty = RadProperty.Register(
			"EqualChildrenWidth", typeof(bool), typeof(StripLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public bool EqualChildrenWidth
		{
			get { return (bool)this.GetValue(StripLayoutPanel.EqualChildrenWidthProperty); }
			set { this.SetValue(StripLayoutPanel.EqualChildrenWidthProperty, value); }
		}

		public static readonly RadProperty EqualChildrenHeightProperty = RadProperty.Register(
			"EqualChildrenHeight", typeof(bool), typeof(StripLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public bool EqualChildrenHeight
		{
			get { return (bool)this.GetValue(StripLayoutPanel.EqualChildrenHeightProperty); }
			set { this.SetValue(StripLayoutPanel.EqualChildrenHeightProperty, value); }
		}

		protected virtual Size GetElementPreferredSize(Rectangle bounds, PreferredSizeData data)
		{
			int width = 0;
			int height = 0;
			if (this.EqualChildrenWidth)
			{
				if (this.Orientation == Orientation.Horizontal)
					width = this.maxIntegralWidth;
				else
					width = bounds.Width;
			}
			else
				width = data.PreferredSize.Width;
			if (this.EqualChildrenHeight)
			{
				if (this.Orientation == Orientation.Horizontal)
					height = bounds.Height;
				else
					height = this.maxIntegralHeight;
			}
			else
				height = data.PreferredSize.Height;

			return new Size(width, height);
		}

		protected virtual void SetElementBoundsAuto(Rectangle bounds, PreferredSizeData data, int offset)
		{
            Point location = ItemsOffset;
            if (this.Orientation == Orientation.Horizontal)
			{
				location.X += offset;
			}
			else
			{
				location.Y += offset;
			}

			if (data.Element.AutoSize && data.Element.ElementState == ElementState.Loaded)
			{
				Size prefSize = this.GetElementPreferredSize(bounds, data);
                if (this.IsFitInSize())
                {
                    Size availableSize = Size.Subtract(bounds.Size, this.LayoutEngine.GetBorderSize());
                    availableSize = Size.Subtract(availableSize, data.Element.Margin.Size);

                    prefSize = TelerikLayoutEngine.CheckSize(prefSize, availableSize, Size.Empty);
                }
                Size tempSize = Size.Subtract(this.Size, data.Element.Margin.Size);
                if (this.ForceElementsWidth && prefSize.Width < tempSize.Width)
                    prefSize.Width = tempSize.Width;
                if (this.ForceElementsHeight && prefSize.Height < tempSize.Height)
                    prefSize.Height = tempSize.Height;
                data.Element.SetBounds(new Rectangle(location, prefSize));
			}
			else
			{
				data.Element.Location = location;
			}
		}

		protected virtual StripPosition GetInvariantPosition(ContentAlignment alignment)
		{
			StripPosition res;
            if (this.Orientation == Orientation.Horizontal)
			{
				if (alignment == ContentAlignment.TopLeft ||
					alignment == ContentAlignment.MiddleLeft ||
					alignment == ContentAlignment.BottomLeft)
				{
					res = StripPosition.First;
				}
				else if (alignment == ContentAlignment.TopCenter ||
					alignment == ContentAlignment.MiddleCenter ||
					alignment == ContentAlignment.BottomCenter)
				{
					res = StripPosition.Middle;
				}
				else
				{
					res = StripPosition.Last;
				}
			}
			else
			{
				if (alignment == ContentAlignment.TopLeft ||
					alignment == ContentAlignment.TopCenter ||
					alignment == ContentAlignment.TopRight)
				{
					res = StripPosition.First;
				}
				else if (alignment == ContentAlignment.MiddleLeft ||
					alignment == ContentAlignment.MiddleCenter ||
					alignment == ContentAlignment.MiddleRight)
				{
					res = StripPosition.Middle;
				}
				else
				{
					res = StripPosition.Last;
				}
			}
			return res;
		}

		protected virtual int GetInvariantLength(Size size, Padding margin)
		{
            return this.Orientation == Orientation.Horizontal ?
				size.Width + margin.Horizontal :
				size.Height + margin.Vertical;
		}

		protected virtual void FillPrefSizeList(List<PreferredSizeData> prefSizelist, Rectangle bounds)
		{
			// Split children in 2 lists: with own size and with AutoSizeMode == FitToAvailableSize
			// Only for children with own size will be called GetPreferredSize()
			List<RadElement> fitElementsList = new List<RadElement>();
			int leftTopOffset = 0, rightBottomOffset = 0;
			this.maxIntegralHeight = 0;

			foreach (RadElement child in this.GetChildrenForLayout())
			{
                if (child.IsFitInSize())
				{
					fitElementsList.Add(child);
				}
				else
				{
					// The constructor of PreferredSizeData will call GetPreferredSize()
					PreferredSizeData prefSizedata = new PreferredSizeData(child, bounds);
					StripPosition pos = GetInvariantPosition( child.Alignment);
					if (pos == StripPosition.First)
					{
						leftTopOffset += GetInvariantLength(prefSizedata.PreferredSize, child.Margin);
					}
					else if (pos == StripPosition.Last)
					{
						rightBottomOffset += GetInvariantLength(prefSizedata.PreferredSize, child.Margin);
					}
					prefSizelist.Add(prefSizedata);

                    this.maxIntegralWidth = Math.Max(this.maxIntegralWidth, prefSizedata.PreferredSize.Width);
                    this.maxIntegralHeight = Math.Max(this.maxIntegralHeight, prefSizedata.PreferredSize.Height);
				}
			}

			// Calculate preferred size for "FitToAvailableSize" elements
			Rectangle proposedRect = CalcProposedBounds(bounds, prefSizelist, leftTopOffset, rightBottomOffset);

			// Call GetPreferredSize() for children that are with AutoSizeMode == FitToAvailableSize
			foreach (RadElement child in fitElementsList)
			{
				//prefSizelist.Add(new PreferredSizeData(child, proposedRect));
                prefSizelist.Add(new PreferredSizeData(child, proposedRect.Size, this.forceFitToSizeElements));
			}
		}

		protected virtual Rectangle CalcProposedBounds(Rectangle bounds, List<PreferredSizeData> prefSizeList,
			int leftTopOffset, int rightBottomOffset)
		{
			Rectangle proposedRect;
			bool propagateToChildren = this.ShouldIgnoreChildSizes();

            if (this.Orientation == Orientation.Horizontal)
			{
                if (this.forceFitToSizeElements)
                {
                    proposedRect = new Rectangle(
                    bounds.X + leftTopOffset,
                    bounds.Y,
                    bounds.Width - GetDesiredHorizontalSize(),
                    propagateToChildren ? bounds.Height : GetMaxHeight(prefSizeList));
                }
                else
                {
                    int width = propagateToChildren ? bounds.Width : GetSumWidth(prefSizeList);
                    width -= leftTopOffset + rightBottomOffset;
                    if (width < 0) width = 0;
                    proposedRect = new Rectangle(
                        bounds.X + leftTopOffset,
                        bounds.Y,
                        width,
                        propagateToChildren ? bounds.Height : GetMaxHeight(prefSizeList));
                }
			}
			else
			{
                if (this.forceFitToSizeElements)
                {
                    proposedRect = new Rectangle(
                    bounds.X,
                    bounds.Y + leftTopOffset,
                    propagateToChildren ? bounds.Width : GetMaxWidth(prefSizeList),
                    bounds.Height - GetDesiredVerticalSize());
                }
                else
                {
                    int height = propagateToChildren ? bounds.Height : GetSumHeight(prefSizeList);
                    height -= leftTopOffset + rightBottomOffset;
                    if (height < 0) height = 0;
                    proposedRect = new Rectangle(
                        bounds.X,
                        bounds.Y + leftTopOffset,
                        propagateToChildren ? bounds.Width : GetMaxWidth(prefSizeList),
                        height);
                }
			}

			return proposedRect;
		}

		protected virtual int GetMaxWidth(List<PreferredSizeData> list)
		{
            int maxWidth = 0;
            for (int i = 0; i < list.Count; i++)
            {
                PreferredSizeData data = list[i];
                if (maxWidth < data.PreferredSize.Width)
                {
                    maxWidth = data.PreferredSize.Width;
                }
            }
			return maxWidth;
		}

		protected virtual int GetSumWidth(List<PreferredSizeData> list)
		{
			int sumWidth = 0;
            for (int i = 0; i < list.Count; i++)
            {
                PreferredSizeData data = list[i];
                sumWidth += data.PreferredSize.Width;
            }
			return sumWidth;
		}

		protected virtual int GetMaxHeight(List<PreferredSizeData> list)
		{
			int maxHeight = 0;
            for (int i = 0; i < list.Count; i++)
            {
                PreferredSizeData data = list[i];
                if (maxHeight < data.PreferredSize.Height)
                {
                    maxHeight = data.PreferredSize.Height;
                }
            }
			return maxHeight;
		}

		protected virtual int GetSumHeight(List<PreferredSizeData> list)
		{
			int sumHeight = 0;
            for (int i = 0; i < list.Count; i++)
            {
                PreferredSizeData data = list[i];
                sumHeight += data.PreferredSize.Height;
            }
			return sumHeight;
        }

		protected virtual Size GetChildrenListSize(List<PreferredSizeData> list)
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                return new Size(GetSumWidth(list), GetMaxHeight(list));
            }
            return new Size(GetMaxWidth(list), GetSumHeight(list));
        }

        protected virtual int GetDesiredHorizontalSize()
        {
            int res = 0;
            Orientation orientation = this.Orientation;

            foreach (RadElement child in this.GetChildrenForLayout())
            {
                if (child.AutoSize && child.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize)
                {
                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            res += child.FullBoundingRectangle.Width;
                            break;
                        case Orientation.Vertical:
                            res = Math.Max(res, child.FullBoundingRectangle.Width);
                            break;
                    }
                }
            }

            return res;
        }

        protected virtual int GetDesiredVerticalSize()
        {
            int res = 0;
            Orientation orientation = this.Orientation;

            foreach (RadElement child in this.GetChildrenForLayout())
            {
                if (child.AutoSize && child.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize)
                {
                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            res = Math.Max(res, child.FullBoundingRectangle.Height);
                            break;
                        case Orientation.Vertical:
                            res += child.FullBoundingRectangle.Height;
                            break;
                    }
                }
            }

            return res;
        }

        #endregion

        #region GetPreferredSizeCore
        ///<summary>
        /// Retrieves the preferred size of the layout panel. If the proposed size is 
        /// smaller than the minimal one, the minimal one is retrieved. Since all layout 
        /// panels update their layout automatically through events, this function is 
        /// rarely used directly.
        /// </summary>
        public override Size GetPreferredSizeCore(Size proposedSize)
		{
			if (AutoSize && this.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize)
			{
                if (this.Orientation == Orientation.Horizontal)
					return GetHorizontalSize();
				else
					return GetVerticalSize();
			}
			else return base.GetPreferredSizeCore(proposedSize);
		}

		protected virtual Size GetHorizontalSize()
		{
			int maxHeight = 0, width = 0;

            foreach (RadElement child in this.GetChildrenForLayout())
            {
                Rectangle fullRect = child.FullBoundingRectangle;
                width += fullRect.Width;
                maxHeight = Math.Max(maxHeight, fullRect.Height);
            }
			
			return new Size(width, maxHeight);
		}

		protected virtual Size GetVerticalSize()
		{
			int height = 0, maxWidth = 0;

            foreach (RadElement child in this.GetChildrenForLayout())
            {
                Rectangle fullRect = child.FullBoundingRectangle;
                height += fullRect.Height;
                maxWidth = Math.Max(maxWidth, fullRect.Width);
            }

			return new Size(maxWidth, height);
        }
        #endregion

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
		{
			base.OnChildrenChanged(child, changeOperation);
			if (changeOperation == ItemsChangeOperation.Inserted)
			{
				child.ZIndex = this.ZIndex;
			}
		}
	}
}
