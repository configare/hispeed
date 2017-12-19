using System;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Themes.ControlDefault;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    public class RadPageViewStackElement : RadPageViewElement
    {
        #region Fields

        internal StackViewLayoutInfo layoutInfo;

        #endregion

        #region Ctor

        static RadPageViewStackElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new StackViewElementStateManager(), typeof(RadPageViewStackElement));
            new ControlDefault_RadPageView_Telerik_WinControls_UI_RadPageViewStackElement().DeserializeTheme();
        }

        public RadPageViewStackElement()
        {
        }

        #endregion

        #region RadProperties

        public static readonly RadProperty StackPositionProperty = RadProperty.Register(
            "StackPosition",
            typeof(StackViewPosition),
            typeof(RadPageViewStackElement),
            new RadElementPropertyMetadata(
                StackViewPosition.Bottom,
                ElementPropertyOptions.AffectsMeasure |
                ElementPropertyOptions.AffectsArrange |
                ElementPropertyOptions.CanInheritValue |
                ElementPropertyOptions.AffectsDisplay)
            );

        public static readonly RadProperty ItemSelectionModeProperty = RadProperty.Register(
            "ItemSelectionMode",
            typeof(StackViewItemSelectionMode),
            typeof(RadPageViewStackElement),
            new RadElementPropertyMetadata(
                StackViewItemSelectionMode.Standard,
                ElementPropertyOptions.AffectsMeasure |
                ElementPropertyOptions.AffectsArrange |
                ElementPropertyOptions.CanInheritValue |
                ElementPropertyOptions.AffectsDisplay)
            );

        #endregion

        #region CLR Properties

        internal override PageViewLayoutInfo ItemLayoutInfo
        {
            get
            {
                return this.layoutInfo;
            }
        }

        protected override RadElement ItemsParent
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="StackViewPosition"/> enum
        /// which determines the location of the items in relation to the content area.
        /// </summary>
        [Description("Gets or sets a value that determines the location of the items in relation to the content area.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("StackPosition", typeof(RadPageViewStackElement))]
        public StackViewPosition StackPosition
        {
            get
            {
                return (StackViewPosition)this.GetValue(StackPositionProperty);
            }
            set
            {
                this.SetValue(StackPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="StackViewItemSelectionMode"/> enum
        /// that determines how items in the stack view are selected and positioned.
        /// </summary>
        [Description("Gets or sets a value that determines how items in the stack view are selected and positioned.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadPropertyDefaultValue("ItemSelectionMode", typeof(RadPageViewStackElement))]
        public StackViewItemSelectionMode ItemSelectionMode
        {
            get
            {
                return (StackViewItemSelectionMode)this.GetValue(ItemSelectionModeProperty);
            }
            set
            {
                this.SetValue(ItemSelectionModeProperty, value);
            }
        }

        #endregion

        #region Methods

        #region Navigation

        protected internal override bool IsNextKey(System.Windows.Forms.Keys key)
        {
            StackViewPosition position = this.layoutInfo.position;
            switch(position)
            {
                case StackViewPosition.Bottom:
                case StackViewPosition.Top:
                    return key == System.Windows.Forms.Keys.Down;
                case StackViewPosition.Left:
                    return key == System.Windows.Forms.Keys.Left;
                case StackViewPosition.Right:
                    return key == System.Windows.Forms.Keys.Right;
                    
            }
            return base.IsNextKey(key);
        }

        protected internal override bool IsPreviousKey(System.Windows.Forms.Keys key)
        {
            StackViewPosition position = this.layoutInfo.position;
            switch (position)
            {
                case StackViewPosition.Bottom:
                case StackViewPosition.Top:
                    return key == System.Windows.Forms.Keys.Up;
                case StackViewPosition.Left:
                    return key == System.Windows.Forms.Keys.Right;
                case StackViewPosition.Right:
                    return key == System.Windows.Forms.Keys.Left;

            }
            return base.IsPreviousKey(key);
        }

        #endregion

        protected internal override void SetSelectedItem(RadPageViewItem item)
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
            base.SetSelectedItem(item);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.ItemSizeMode = PageViewItemSizeMode.EqualSize;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.ContentArea.ThemeRole = "StackViewContentArea";
            this.Header.Visibility = ElementVisibility.Visible;
        }

        protected override RadPageViewItem CreateItem()
        {
            return new RadPageViewStackItem();
        }

        protected internal override PageViewContentOrientation GetAutomaticItemOrientation(bool content)
        {
            StackViewPosition stackPosition = this.StackPosition;
            switch (stackPosition)
            {
                case StackViewPosition.Left:
                    return PageViewContentOrientation.Vertical270;
                case StackViewPosition.Right:
                    return PageViewContentOrientation.Vertical90;
                default:
                    return PageViewContentOrientation.Horizontal;
            }
        }

        #endregion

        #region Layout

        public override RectangleF GetItemsRect()
        {
            RectangleF clientRect = this.GetClientRectangle(true, this.Size);
            clientRect.Y += this.Header.DesiredSize.Height + this.Header.Margin.Vertical;
            clientRect.Height -= (this.Header.DesiredSize.Height + this.Footer.DesiredSize.Height
                + this.Header.Margin.Vertical + this.Footer.Margin.Vertical);
            return clientRect;
        }

        #region Helper methods

        private SizeF GetAvailableSizeForContent(RectangleF clientRect)
        {
            SizeF result = SizeF.Empty;
            float itemLength = this.layoutInfo.layoutLength;

            itemLength = this.CorrectOffsetBasedOnSelectionContext(itemLength);

            switch (this.StackPosition)
            {
                case StackViewPosition.Bottom:
                case StackViewPosition.Top:
                    result = new SizeF(clientRect.Width, clientRect.Height - itemLength);
                    break;
                case StackViewPosition.Left:
                case StackViewPosition.Right:
                    result = new SizeF(clientRect.Width - itemLength, clientRect.Height);
                    break;
            }

            return result;
        }

        private RectangleF GetStandardContentRectangle(PageViewItemSizeInfo item, RectangleF clientRect)
        {
            RectangleF contentRectangle = Rectangle.Empty;

            SizeF contentAreaSize = this.GetContentSizeForItem(item, clientRect);
            float itemsLength = this.layoutInfo.layoutLength;
            StackViewPosition position = this.layoutInfo.position;
            switch (position)
            {
                case StackViewPosition.Bottom:
                case StackViewPosition.Right:
                    contentRectangle = new RectangleF(
                        clientRect.X,
                        clientRect.Y,
                        contentAreaSize.Width,
                        contentAreaSize.Height);
                    break;
                case StackViewPosition.Top:
                    contentRectangle = new RectangleF(
                        clientRect.X,
                        clientRect.Y + itemsLength,
                       contentAreaSize.Width,
                        contentAreaSize.Height);
                    break;
                case StackViewPosition.Left:
                    contentRectangle = new RectangleF(
                        clientRect.X + itemsLength,
                        clientRect.Y,
                        contentAreaSize.Width,
                        contentAreaSize.Height);
                    break;
            }
            return contentRectangle;
        }

        private RectangleF GetTopBottomContentWithSelectedRectangle(bool isTop, PageViewItemSizeInfo selectedItem, SizeF contentAreaSize, RectangleF clientRect)
        {
            RectangleF selectedItemRect = selectedItem.itemRectangle;

            if (!isTop && this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentWithSelected || 
                (isTop && this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentAfterSelected))
            {
                return new RectangleF(
                    clientRect.Left,
                    selectedItemRect.Bottom + selectedItem.item.Margin.Bottom,
                    contentAreaSize.Width,
                    contentAreaSize.Height);
            }
            else if ((!isTop && this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentAfterSelected)
                || (isTop && this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentWithSelected))
            {
                return  new RectangleF(
                                    clientRect.Left,
                                    selectedItemRect.Top - selectedItem.item.Margin.Top - contentAreaSize.Height,
                                    contentAreaSize.Width,
                                    contentAreaSize.Height);
            }

            return Rectangle.Empty;
        }

        private RectangleF GetLeftRightContentWithSelectedRectangle(bool isLeft, PageViewItemSizeInfo selectedItem, SizeF contentAreaSize, RectangleF clientRect)
        {
            RectangleF selectedItemRect = selectedItem.itemRectangle;
            if ((!isLeft && this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentWithSelected)
                || (isLeft && this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentAfterSelected))
            {
                return new RectangleF(
                    selectedItemRect.Right + selectedItem.item.Margin.Right,
                    clientRect.Y,
                    contentAreaSize.Width,
                    contentAreaSize.Height);
            }
            else if ((!isLeft && this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentAfterSelected) ||
                (isLeft && this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentWithSelected))
            {
                return new RectangleF(
                    (selectedItemRect.Left - selectedItem.item.Margin.Left) - contentAreaSize.Width,
                    clientRect.Y,
                    contentAreaSize.Width,
                    contentAreaSize.Height);
            }

            return Rectangle.Empty;

        }

        internal RectangleF GetContentWithSelectedContentRectangle(PageViewItemSizeInfo item, RectangleF clientRect)
        {
            PageViewItemSizeInfo selectedItem = item;
            if (selectedItem == null)
            {
                return clientRect;
            }
            RectangleF contentRectangle = Rectangle.Empty;
            SizeF contentAreaSize = this.GetContentSizeForItem(item, clientRect);//this.GetAvailableSizeForContent(clientRect);

            StackViewPosition position = this.layoutInfo.position;
            switch (position)
            {
                case StackViewPosition.Bottom:
                    contentRectangle = this.GetTopBottomContentWithSelectedRectangle(false, selectedItem, contentAreaSize, clientRect);
                    break;
                case StackViewPosition.Top:
                    contentRectangle = this.GetTopBottomContentWithSelectedRectangle(true, selectedItem, contentAreaSize, clientRect);
                    break;
                case StackViewPosition.Right:
                    contentRectangle = this.GetLeftRightContentWithSelectedRectangle(false, selectedItem, contentAreaSize, clientRect);
                    break;
                case StackViewPosition.Left:
                    contentRectangle = this.GetLeftRightContentWithSelectedRectangle(true, selectedItem, contentAreaSize, clientRect);
                    break;
            }
            return contentRectangle;
        }

        protected virtual RectangleF GetContentAreaRectangle(RectangleF clientRect)
        {
            RectangleF contentRectangle = Rectangle.Empty;
            PageViewItemSizeInfo selectedItem = this.layoutInfo.selectedItem;
            if (this.layoutInfo.selectionMode != StackViewItemSelectionMode.Standard)
            {
                contentRectangle = this.GetContentWithSelectedContentRectangle(selectedItem, clientRect);
            }
            else
            {
                contentRectangle = this.GetStandardContentRectangle(selectedItem, clientRect);
            }

            return contentRectangle;
        }

        private float GetItemWidth(PageViewItemSizeInfo item)
        {
            float result = item.desiredSize.Width;
            SizeF availableSize = this.layoutInfo.availableSize;
            float availableWidth = availableSize.Width - item.marginLength;
            if (result > availableWidth)
            {
                result = availableWidth;
            }

            return result;
        }

        private float GetItemHeight(PageViewItemSizeInfo item)
        {
            float result = item.desiredSize.Height;
            SizeF availableSize = this.layoutInfo.availableSize;
            float availableHeight = availableSize.Height - item.marginLength;
            if (result > availableHeight)
            {
                result = availableHeight;
            }

            return result;
        }

        #endregion

        #region Measure

        internal virtual StackViewLayoutInfo CreateLayoutInfo(SizeF availableSize)
        {
            return new StackViewLayoutInfo(this, availableSize);
        }

        protected virtual SizeF MeasureContentArea(ref SizeF availableSize)
        {
            this.ContentArea.Measure(availableSize);

            if (StackPosition == StackViewPosition.Top || StackPosition == StackViewPosition.Bottom)
            {
                if (!StretchHorizontally)
                {
                    availableSize.Width = this.ContentArea.DesiredSize.Width;
                }
                availableSize.Height -= this.ContentArea.DesiredSize.Height;
            }
            else
            {
                availableSize.Width -= this.ContentArea.DesiredSize.Width;
                if (!StretchVertically)
                {
                    availableSize.Height = this.ContentArea.DesiredSize.Height;
                }
            }

            return this.ContentArea.DesiredSize;
        }

        protected override SizeF MeasureItems(SizeF availableSize)
        {
            SizeF desiredSize = SizeF.Empty;

            this.layoutInfo = this.CreateLayoutInfo(availableSize);
            StackViewPosition stackPosition = this.layoutInfo.position;

            switch (stackPosition)
            {
                case StackViewPosition.Bottom:
                case StackViewPosition.Top:
                    this.ApplyItemMetricsHorizontal();
                    break;
                case StackViewPosition.Left:
                case StackViewPosition.Right:
                    this.ApplyItemMetricsVertical();
                    break;
            }

            this.MeasureItemsCore();

            if (stackPosition == StackViewPosition.Top || stackPosition == StackViewPosition.Bottom)
            {
                availableSize.Height -= layoutInfo.layoutLength;
                desiredSize.Height = layoutInfo.layoutLength;
                desiredSize.Width = layoutInfo.maxWidth;
            }
            else
            {
                availableSize.Width -= layoutInfo.layoutLength;
                desiredSize.Width = layoutInfo.layoutLength;
                desiredSize.Height = layoutInfo.maxHeight;
            }
            
            SizeF contentAreaDesiredSize = MeasureContentArea(ref availableSize);

            if (stackPosition == StackViewPosition.Top || stackPosition == StackViewPosition.Bottom)
            {
                desiredSize.Height += contentAreaDesiredSize.Height;
                desiredSize.Width = Math.Max(desiredSize.Width, contentAreaDesiredSize.Width);
            }
            else
            {
                desiredSize.Width += contentAreaDesiredSize.Width;
                desiredSize.Height = Math.Max(desiredSize.Height, contentAreaDesiredSize.Height);
            }

            return desiredSize;
        }

        private void MeasureItemsCore()
        {
            StackViewPosition position = this.layoutInfo.position;
            int integralItemSpacing = this.layoutInfo.itemSpacing * (this.layoutInfo.itemCount - 1);
            this.layoutInfo.layoutLength += integralItemSpacing;
            foreach (PageViewItemSizeInfo info in this.layoutInfo.items)
            {
                System.Windows.Forms.Padding itemMargin = this.GetItemMargin(info);
                if (info.layoutSize != info.desiredSize)
                {
                    SizeF availableSize = info.layoutSize;
                    info.item.Measure(availableSize);
                    if (this.Owner == null)
                    {
                        SizeF desiredSize = info.item.DesiredSize;
                        if ((position == StackViewPosition.Top || position == StackViewPosition.Bottom) &&
                            info.item.StretchHorizontally && !float.IsInfinity(info.layoutSize.Width))
                        {
                            desiredSize.Width = info.layoutSize.Width;
                        }
                        if ((position == StackViewPosition.Left || position == StackViewPosition.Right) &&
                            info.item.StretchVertically && !float.IsInfinity(info.layoutSize.Height))
                        {
                            desiredSize.Height = info.layoutSize.Height;
                        }
                        info.desiredSize = desiredSize;
                    }
                    else
                    {
                        info.desiredSize = info.layoutSize;
                    }
                }

                switch (position)
                {
                    case StackViewPosition.Top:
                    case StackViewPosition.Bottom:
                        this.layoutInfo.layoutLength += (info.desiredSize.Height + itemMargin.Vertical);
                        break;
                    case StackViewPosition.Left:
                    case StackViewPosition.Right:
                        this.layoutInfo.layoutLength += (info.desiredSize.Width + itemMargin.Horizontal);
                        break;
                }
            }
        }

        #region Vertical mode measure methods
       
        private void ApplyItemMetricsVertical()
        {
            PageViewItemSizeMode sizeMode = this.layoutInfo.sizeMode;
            switch (sizeMode)
            {
                case PageViewItemSizeMode.EqualHeight:
                    this.MeasureItemsVerticalEqualHeight();
                    break;
                case PageViewItemSizeMode.EqualWidth:
                    this.MeasureItemsVerticalEqualWidth();
                    break;
                case PageViewItemSizeMode.EqualSize:
                    this.MeasureItemsEqualSizeVertical();
                    break;
                case PageViewItemSizeMode.Individual:
                    this.MeasureItemsVerticalIndividual();
                    break;
            }
        }

        private void MeasureItemsEqualSizeVertical()
        {
            SizeF availableSize = this.layoutInfo.availableSize;
            float maxItemWidth = this.layoutInfo.maxWidth;
            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                item.SetLayoutSize(new SizeF(maxItemWidth, availableSize.Height - (this.Owner != null ? (float)item.marginLength : 0f)));
            }
        }

        private void MeasureItemsVerticalEqualHeight()
        {
            SizeF availableSize = this.layoutInfo.availableSize;

            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                item.SetLayoutSize(new SizeF(item.desiredSize.Width, availableSize.Height - item.marginLength));
            }
        }

        private void MeasureItemsVerticalEqualWidth()
        {
            float maxItemWidth = this.layoutInfo.maxWidth;

            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                float itemHeight = this.GetItemHeight(item);
                item.SetLayoutSize(new SizeF(maxItemWidth, itemHeight));
            }
        }

        private void MeasureItemsVerticalIndividual()
        {
            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                float itemHeight = this.GetItemHeight(item);
                item.SetLayoutSize(new SizeF(item.desiredSize.Width, itemHeight));
            }
        }

        #endregion

        #region Horizontal mode measure methods

        private void ApplyItemMetricsHorizontal()
        {
            PageViewItemSizeMode sizeMode = this.layoutInfo.sizeMode;
            switch (sizeMode)
            {
                case PageViewItemSizeMode.EqualHeight:
                    this.MeasureItemsHorizontalEqualHeight();
                    break;
                case PageViewItemSizeMode.EqualWidth:
                    this.MeasureItemsHorizontalEqualWidth();
                    break;
                case PageViewItemSizeMode.EqualSize:
                    this.MeasureItemsEqualSizeHorizontal();
                    break;
                case PageViewItemSizeMode.Individual:
                    this.MeasureItemsHorizontalIndividualSize();
                    break;
            }
        }

        private void MeasureItemsEqualSizeHorizontal()
        {
            SizeF availableSize = this.layoutInfo.availableSize;
            float maxItemHeight = this.layoutInfo.maxHeight;
            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                item.SetLayoutSize(new SizeF(availableSize.Width - item.marginLength, maxItemHeight));
            }
        }

        private void MeasureItemsHorizontalEqualWidth()
        {
            float availableWidth = this.layoutInfo.availableSize.Width;
            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                item.SetLayoutSize(new SizeF(availableWidth - item.marginLength, item.desiredSize.Height));
            }
        }

        private void MeasureItemsHorizontalEqualHeight()
        {
            float maxItemHeight = this.layoutInfo.maxHeight;
            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                float width = this.GetItemWidth(item);
                item.SetLayoutSize(new SizeF(width, maxItemHeight));
            }
        }

        private void MeasureItemsHorizontalIndividualSize()
        {
            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                float width = this.GetItemWidth(item);
                item.SetLayoutSize(new SizeF(width, item.desiredSize.Height));
            }
        }

        #endregion

        #endregion

        #region Arrange
        internal System.Windows.Forms.Padding GetItemMargin(RadPageViewStackItem item)
        {
            StackViewPosition position = this.layoutInfo.position;
            System.Windows.Forms.Padding itemMargin = item.Margin;

            if (!item.AutoFlipMargin)
                return itemMargin;

            switch (position)
            {
                case StackViewPosition.Left:
                    return new System.Windows.Forms.Padding(itemMargin.Top, itemMargin.Right, itemMargin.Bottom, itemMargin.Left);
                case StackViewPosition.Right:
                    return new System.Windows.Forms.Padding(itemMargin.Bottom, itemMargin.Left, itemMargin.Top, itemMargin.Right);
                default:
                    return itemMargin;
            }
        }
        internal System.Windows.Forms.Padding GetItemMargin(PageViewItemSizeInfo item)
        {
            return this.GetItemMargin(item.item as RadPageViewStackItem);
        }

        private PointF GetItemLocation(ref float currentItemOffset, PageViewItemSizeInfo item, RectangleF clientRect)
        {
            PointF itemLocation = PointF.Empty;
            System.Windows.Forms.Padding itemMargin = this.GetItemMargin(item);
            SizeF itemSize = item.desiredSize;
            StackViewPosition position = this.layoutInfo.position;
            switch (position)
            {
                case StackViewPosition.Bottom:
                    currentItemOffset += itemSize.Height + itemMargin.Bottom;
                    itemLocation = new PointF(
                        clientRect.X + itemMargin.Left,
                        clientRect.Bottom - currentItemOffset);
                    currentItemOffset += itemMargin.Top;
                    break;
                case StackViewPosition.Left:
                    currentItemOffset += itemMargin.Left;
                    itemLocation = new PointF(
                        clientRect.X + currentItemOffset,
                        clientRect.Y + (this.Owner == null ? 0 : itemMargin.Top));
                    currentItemOffset += itemMargin.Right + itemSize.Width;
                    break;
                case StackViewPosition.Top:
                    currentItemOffset += itemMargin.Top;
                    itemLocation = new PointF(
                        clientRect.X + itemMargin.Left,
                        clientRect.Y + currentItemOffset);
                    currentItemOffset += itemSize.Height + itemMargin.Bottom;
                    break;
                case StackViewPosition.Right:
                    currentItemOffset += itemSize.Width + itemMargin.Right;
                    itemLocation = new PointF(
                        clientRect.Right - currentItemOffset,
                        clientRect.Y + (this.Owner == null ? 0 : itemMargin.Top));
                    currentItemOffset += itemMargin.Left;
                    break;
            }

            return itemLocation;
        }

        internal virtual SizeF GetContentSizeForItem(PageViewItemSizeInfo sizeInfo, RectangleF clientRect)
        {            
            return this.GetAvailableSizeForContent(clientRect);
        }


        internal virtual float GetItemOffset(RectangleF clientRect, PageViewItemSizeInfo sizeInfo, float proposedOffset)
        {
            if (object.ReferenceEquals(sizeInfo, this.layoutInfo.selectedItem) &&
                    this.layoutInfo.selectionMode != StackViewItemSelectionMode.Standard)
            {

                SizeF contentSize = this.GetContentSizeForItem(sizeInfo, clientRect);

                switch(this.layoutInfo.position)
                {
                    case StackViewPosition.Bottom:
                    case StackViewPosition.Top:
                        proposedOffset += contentSize.Height;
                        break;
                    default:
                        proposedOffset += contentSize.Width;
                        break;
                }

                proposedOffset = this.CorrectOffsetBasedOnSelectionContext(proposedOffset);
            }

            return proposedOffset;
        }

        internal virtual float CorrectOffsetBasedOnSelectionContext(float proposedOffset)
        {
            if (this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentWithSelected)
            {
                if (this.layoutInfo.selectedItem.itemIndex < this.Items.Count - 1)
                {
                    proposedOffset -= this.layoutInfo.itemSpacing;
                }
            }
            else if (this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentAfterSelected)
            {
                if (this.layoutInfo.selectedItem.itemIndex > 0)
                {
                    proposedOffset -= this.layoutInfo.itemSpacing;
                }
            }
            return proposedOffset;
        }

        protected virtual float GetInitialItemsOffset(RectangleF clientRect)
        {
            return 0;
        }

        protected override RectangleF ArrangeItems(RectangleF clientRect)
        {
            float itemOffset = this.GetInitialItemsOffset(clientRect);
            PointF itemLocation = PointF.Empty;
            int itemSpacing = this.layoutInfo.itemSpacing;

            for (int i = this.layoutInfo.items.Count - 1; i > -1; i--)
            {
                PageViewItemSizeInfo sizeInfo = this.layoutInfo.items[i];
                if (this.layoutInfo.selectionMode == StackViewItemSelectionMode.ContentAfterSelected)
                {
                    itemLocation = this.GetItemLocation(ref itemOffset, sizeInfo, clientRect);
                    itemOffset = this.GetItemOffset(clientRect, sizeInfo, itemOffset);
                }
                else
                {
                    itemOffset = this.GetItemOffset(clientRect, sizeInfo, itemOffset);
                    itemLocation = this.GetItemLocation(ref itemOffset, sizeInfo, clientRect);
                }

                itemOffset += itemSpacing;
                sizeInfo.itemRectangle = new RectangleF(itemLocation, sizeInfo.desiredSize);

                if (Owner == null)
                {
                    if (StackPosition == StackViewPosition.Left)
                    {
                        sizeInfo.itemRectangle.Height = clientRect.Height;
                    }
                    if (StackPosition == StackViewPosition.Left || StackPosition == StackViewPosition.Right)
                    {
                        sizeInfo.itemRectangle.Height -= this.layoutInfo.items[i].marginLength * 2;
                    }
                }

                sizeInfo.item.Arrange(sizeInfo.itemRectangle);
            }

            return this.GetContentAreaRectangle(clientRect);
        }

        #endregion

        #endregion

        #region Event handling

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadPageViewStackElement.StackPositionProperty)
            {
                if (this.ItemContentOrientation == PageViewContentOrientation.Auto)
                {
                    this.UpdateItemOrientation(this.Items);
                }
            }

            base.OnPropertyChanged(e);
        }

        #endregion
    }
}
