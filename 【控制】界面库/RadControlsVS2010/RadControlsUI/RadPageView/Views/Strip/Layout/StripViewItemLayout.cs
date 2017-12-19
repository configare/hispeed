using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class StripViewItemLayout : RadPageViewElementBase
    {
        #region Fields

        private const int PartialItemOffset = 15;

        private StripViewLayoutInfo layoutInfo;
        private AnimatedPropertySetting scrollAnimation;
        private bool enableScrolling;
        private RadPageViewItem itemToEnsureVisible;

        #endregion

        #region Constructor/Initializers

        static StripViewItemLayout()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new StripViewElementStateManager(), typeof(StripViewItemLayout));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.enableScrolling = true;
            this.scrollAnimation = new AnimatedPropertySetting();
            this.scrollAnimation.NumFrames = 5;
            this.scrollAnimation.ApplyEasingType = RadEasingType.InOutQuad;
            this.scrollAnimation.Property = ScrollOffsetProperty;
        }

        #endregion

        #region Properties

        internal static RadProperty ScrollOffsetProperty = RadProperty.Register(
            "StripButtons",
            typeof(int),
            typeof(StripViewItemLayout),
            new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets the scroll offset applied to the strip.
        /// </summary>
        [Browsable(false)]
        public int ScrollOffset
        {
            get
            {
                return (int)this.GetValue(ScrollOffsetProperty);
            }
            private set
            {
                this.SetValue(ScrollOffsetProperty, value);
            }
        }

        internal StripViewLayoutInfo LayoutInfo
        {
            get
            {
                return this.layoutInfo;
            }
        }

        #endregion

        #region Overrides

        protected override void OnUnloaded(ComponentThemableElementTree oldTree)
        {
            base.OnUnloaded(oldTree);

            this.itemToEnsureVisible = null;
            this.layoutInfo = null;
            this.scrollAnimation.Stop(this);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }

            if (RadPageViewStripElement.PropertyInvalidatesScrollOffset(e.Property))
            {
                this.ResetScrollOffset();
            }
            else if (e.Property == BoundsProperty)
            {
                RadPageViewElement parent = this.FindAncestor<RadPageViewElement>();
                if (parent != null)
                {
                    parent.OnContentBoundsChanged();
                }
            }
        }

        #endregion

        #region Measure

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            float prevItemLength = -1;
            if (this.layoutInfo != null)
            {
                prevItemLength = this.layoutInfo.layoutLength;
            }

            this.layoutInfo = null;
            if (availableSize.Width <= 0 || availableSize.Height <= 0)
            {
                this.ResetScrollOffset();
                return SizeF.Empty;
            }

            this.layoutInfo = new StripViewLayoutInfo(this, availableSize);
            if (this.layoutInfo.itemCount == 0)
            {
                this.ResetScrollOffset();
                return SizeF.Empty;
            }

            this.ApplyItemSizeMode();
            this.ApplyItemFitMode();

            if (this.layoutInfo == null)
            {
                return SizeF.Empty;
            }

            this.MeasureItems();

            this.UpdateScrollOffset(prevItemLength, availableSize);

            SizeF measured = this.layoutInfo.measuredSize;
            //apply padding and border
            measured = this.ApplyClientOffset(measured);
            //consider min/max size
            return this.ApplyMinMaxSize(measured);
        }

        private void MeasureItems()
        {
            SizeF measured = SizeF.Empty;

            foreach (PageViewItemSizeInfo itemInfo in this.layoutInfo.items)
            {
                //item size mode and item fit mode may have altered the item's desired size and if so re-measure is needed
                if (itemInfo.layoutSize != itemInfo.desiredSize)
                {
                    itemInfo.item.Measure(itemInfo.layoutSize);
                }

                Padding margin = itemInfo.item.Margin;
                if (this.layoutInfo.vertical)
                {
                    measured.Width = Math.Max(measured.Width, itemInfo.layoutSize.Width + margin.Horizontal);
                    measured.Height += itemInfo.layoutSize.Height + margin.Vertical;
                }
                else
                {
                    measured.Width += itemInfo.layoutSize.Width + margin.Horizontal;
                    measured.Height = Math.Max(measured.Height, itemInfo.layoutSize.Height + margin.Vertical);
                }

                this.layoutInfo.layoutLength += itemInfo.layoutLength + itemInfo.marginLength;
            }

            int spacing = (this.layoutInfo.itemCount - 1) * this.layoutInfo.itemSpacing;
            if (this.layoutInfo.vertical)
            {
                measured.Height += spacing;
            }
            else
            {
                measured.Width += spacing;
            }

            this.layoutInfo.layoutLength += spacing;

            this.layoutInfo.measuredSize = measured;
        }

        private void ApplyItemSizeMode()
        {
            if (this.layoutInfo.sizeMode == PageViewItemSizeMode.Individual)
            {
                return;
            }

            foreach (PageViewItemSizeInfo itemInfo in this.layoutInfo.items)
            {
                SizeF layoutSize = itemInfo.layoutSize;

                switch(this.layoutInfo.align)
                {
                    case StripViewAlignment.Top:
                    case StripViewAlignment.Bottom:
                        if ((this.layoutInfo.sizeMode & PageViewItemSizeMode.EqualWidth) == PageViewItemSizeMode.EqualWidth)
                        {
                            layoutSize.Width = this.layoutInfo.maxWidth;
                        }
                        if ((this.layoutInfo.sizeMode & PageViewItemSizeMode.EqualHeight) == PageViewItemSizeMode.EqualHeight)
                        {
                            layoutSize.Height = this.layoutInfo.maxHeight;
                        }
                        break;
                    case StripViewAlignment.Left:
                    case StripViewAlignment.Right:
                        //width and height are swapped for left and right alignment
                        if ((this.layoutInfo.sizeMode & PageViewItemSizeMode.EqualWidth) == PageViewItemSizeMode.EqualWidth)
                        {
                            layoutSize.Height = this.layoutInfo.maxHeight;
                        }
                        if ((this.layoutInfo.sizeMode & PageViewItemSizeMode.EqualHeight) == PageViewItemSizeMode.EqualHeight)
                        {
                            layoutSize.Width = this.layoutInfo.maxWidth;
                        }
                        break;
                }

                itemInfo.SetLayoutSize(layoutSize);
            }
        }

        private void ApplyItemFitMode()
        {
            Debug.Assert(this.layoutInfo != null, "Invalid measure pass.");

            if ((this.layoutInfo.fitMode & StripViewItemFitMode.Fill) == StripViewItemFitMode.Fill)
            {
                new StripViewItemExpandStrategy(this.layoutInfo).Execute();
            }
            if ((this.layoutInfo.fitMode & StripViewItemFitMode.Shrink) == StripViewItemFitMode.Shrink)
            {
                new StripViewItemShrinkStrategy(this.layoutInfo).Execute();
            }
        }

        #endregion

        #region Arrange

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            //no need to arrange if no valid layout info is accumulated via a measure pass.
            if (this.layoutInfo == null)
            {
                return finalSize;
            }

            RectangleF client = this.GetAlignedClientRectangle(finalSize);
            if (this.RightToLeft && 
                this.layoutInfo.align != StripViewAlignment.Left &&
                this.layoutInfo.align != StripViewAlignment.Right)
            {
                client = LayoutUtils.RTLTranslateNonRelative(client, new RectangleF(PointF.Empty, finalSize));
            }
            if (client.Width > 0 && client.Height > 0)
            {
                this.ArrangeItems(client);
            }

            StripViewItemContainer parent = this.FindAncestor<StripViewItemContainer>();
            if (parent != null)
            {
                parent.UpdateButtonsEnabledState();
            }

            if (this.itemToEnsureVisible != null)
            {
                this.ScrollToItem(itemToEnsureVisible);
                this.itemToEnsureVisible = null;
            }

            return finalSize;
        }

        private void ArrangeItems(RectangleF client)
        {
            float left = client.X;
            float top = client.Y;
            PointF location = Point.Empty;

            foreach (RadPageViewItem item in this.Children)
            {

                SizeF desired = item.ForcedLayoutSize;
                if ((this.layoutInfo.fitMode & StripViewItemFitMode.FillHeight) != 0)
                {
                    switch (this.layoutInfo.align)
                    {
                        case StripViewAlignment.Top:
                        case StripViewAlignment.Bottom:
                            desired = new SizeF(desired.Width, client.Height);
                            break;
                        case StripViewAlignment.Left:
                        case StripViewAlignment.Right:
                            desired = new SizeF(client.Width, desired.Height);
                            break;
                    }
                }

                Padding margin = item.Margin;

                switch (this.layoutInfo.align)
                {
                    case StripViewAlignment.Top:
                        location = new PointF(left + margin.Left, client.Bottom - desired.Height - margin.Vertical);
                        left += desired.Width + margin.Horizontal + this.layoutInfo.itemSpacing;
                        break;
                    case StripViewAlignment.Bottom:
                        location = new PointF(left + margin.Left, client.Y + margin.Top);
                        left += desired.Width + margin.Horizontal + this.layoutInfo.itemSpacing;
                        break;
                    case StripViewAlignment.Left:
                        location = new PointF(client.Right - desired.Width - margin.Horizontal, top + margin.Top);
                        top += desired.Height + margin.Vertical + this.layoutInfo.itemSpacing;
                        break;
                    case StripViewAlignment.Right:
                        location = new PointF(client.X + margin.Left, top + margin.Top);
                        top += desired.Height + margin.Vertical + this.layoutInfo.itemSpacing;
                        break;
                }

                RectangleF arrangeRectangle = new RectangleF(location, desired);
                if (this.RightToLeft &&
                    (this.layoutInfo.align == StripViewAlignment.Top || this.layoutInfo.align == StripViewAlignment.Bottom))
                {
                    arrangeRectangle = LayoutUtils.RTLTranslateNonRelative(arrangeRectangle, client);
                }

                item.Arrange(arrangeRectangle);
            }
        }

        private RectangleF GetAlignedClientRectangle(SizeF finalSize)
        {
            RectangleF client = this.GetClientRectangle(finalSize);
            if (client.Width <= 0 || client.Height <= 0)
            {
                return RectangleF.Empty;
            }

            switch(this.layoutInfo.itemAlign)
            {
                case StripViewItemAlignment.Center:
                    client = this.GetCenterClientRect(client);
                    break;
                case StripViewItemAlignment.Far:
                    client = this.GetFarClientRect(client);
                    break;
            }

            return this.AddStripAndScrollOffset(client);
        }

        private RectangleF AddStripAndScrollOffset(RectangleF client)
        {
            switch(this.layoutInfo.align)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    switch(this.layoutInfo.itemAlign)
                    {
                        case StripViewItemAlignment.Near:
                        case StripViewItemAlignment.Center:
                            client.X -= this.ScrollOffset;
                            break;
                        case StripViewItemAlignment.Far:
                            client.X += this.ScrollOffset;
                            break;
                    }
                    break;
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    switch (this.layoutInfo.itemAlign)
                    {
                        case StripViewItemAlignment.Near:
                        case StripViewItemAlignment.Center:
                            client.Y -= this.ScrollOffset;
                            break;
                        case StripViewItemAlignment.Far:
                            client.Y += this.ScrollOffset;
                            break;
                    }
                    break;
            }

            return client;
        }

        private RectangleF GetCenterClientRect(RectangleF client)
        {
            SizeF measured = this.layoutInfo.measuredSize;
            switch(this.layoutInfo.align)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    if (measured.Width < client.Width)
                    {
                        client.X += (int)((client.Width - measured.Width) / 2F + .5F);
                    }
                    break;
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    if (measured.Height < client.Height)
                    {
                        client.Y += (int)((client.Height - measured.Height) / 2F + .5F);
                    }
                    break;
            }

            return client;
        }

        private RectangleF GetFarClientRect(RectangleF client)
        {
            SizeF measured = this.layoutInfo.measuredSize;
            switch (this.layoutInfo.align)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    client.X += (client.Width - measured.Width);
                    break;
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    client.Y += (client.Height - measured.Height);
                    break;
            }

            return client;
        }

        #endregion

        #region Scrolling

        internal void EnableScrolling(bool enable)
        {
            this.enableScrolling = enable;
            if (this.layoutInfo == null)
            {
                return;
            }

            int offset = this.ScrollOffset;
            if (!this.enableScrolling)
            {
                this.scrollAnimation.UnapplyValue(this);
                this.scrollAnimation.EndValue = 0;
                this.ScrollOffset = offset;
            }
            else
            {
                this.scrollAnimation.EndValue = offset;
            }
        }

        internal void SetScrollAnimation(RadEasingType type)
        {
            this.scrollAnimation.ApplyEasingType = type;
        }

        internal bool EnsureVisible(RadPageViewItem item)
        {
            if (item.Visibility == ElementVisibility.Collapsed)
            {
                return false;
            }

            if (this.layoutInfo == null || !this.IsMeasureValid || !this.IsArrangeValid)
            {
                this.itemToEnsureVisible = item;
                return false;
            }

            this.ScrollToItem(item);
            return true;
        }

        private void ScrollToItem(RadPageViewItem item)
        {
            RectangleF client = this.GetClientRectangle(this.layoutInfo.availableSize);
            RectangleF itemBounds = item.BoundingRectangle;

            this.ScrollToBounds(client, itemBounds);
            this.InvalidateArrange();
        }

        private bool ShouldUseRightToLeft()
        {
            return this.RightToLeft && this.layoutInfo.align != StripViewAlignment.Left && this.layoutInfo.align != StripViewAlignment.Right;
        }

        private void ScrollToBounds(RectangleF client, RectangleF itemBounds)
        {
            //scroll in such way that next item is partially visible
            int nextItemPart = this.layoutInfo.itemSpacing + PartialItemOffset;

            float farOffset;
            float nearOffset;
            if (this.layoutInfo.vertical)
            {
                farOffset = itemBounds.Bottom - client.Bottom + nextItemPart;
                nearOffset = client.Y - itemBounds.Y + nextItemPart;
            }
            else
            {
                farOffset = itemBounds.Right - client.Right + nextItemPart;
                nearOffset = client.X - itemBounds.X + nextItemPart;
            }

            //swap near and far offset when item alignment is far

            if (this.layoutInfo.itemAlign == StripViewItemAlignment.Far ^ ShouldUseRightToLeft())
            {
                float temp = nearOffset;
                nearOffset = farOffset;
                farOffset = temp;
            }

            //near offset is with higher priority
            if (nearOffset > 0)
            {
                this.SetScrollOffset((int)(this.ScrollOffset - nearOffset), true);
            }
            else if (farOffset > 0)
            {
                this.SetScrollOffset((int)(this.ScrollOffset + farOffset), true);
            }
        }

        internal bool CanScroll(StripViewButtons button)
        {
            if(this.layoutInfo == null || this.layoutInfo.itemCount <= 1)
            {
                return false;
            }

            if (this.layoutInfo.itemAlign == StripViewItemAlignment.Far ^ ShouldUseRightToLeft())
            {
                button = this.FlipScrollButtons(button);
            }

            return this.GetScrollStep(this.PreviousConstraint, button) > 0;
        }

        private StripViewButtons FlipScrollButtons(StripViewButtons buttons)
        {
            if (buttons == StripViewButtons.LeftScroll)
            {
                buttons = StripViewButtons.RightScroll;
            }
            else
            {
                buttons = StripViewButtons.LeftScroll;
            }

            return buttons;
        }

        internal void Scroll(StripViewButtons button)
        {
            if (this.layoutInfo == null)
            {
                return;
            }

            if (this.layoutInfo.itemAlign == StripViewItemAlignment.Far ^ ShouldUseRightToLeft())
            {
                button = this.FlipScrollButtons(button);
            } 

            switch (button)
            {
                case StripViewButtons.LeftScroll:
                    this.SetScrollOffset(this.ScrollOffset - this.GetScrollStep(this.PreviousConstraint, StripViewButtons.LeftScroll), true);
                    Debug.Assert(this.ScrollOffset >= 0, "Invalid scroll state");
                    break;
                case StripViewButtons.RightScroll:
                    this.SetScrollOffset(this.ScrollOffset + this.GetScrollStep(this.PreviousConstraint, StripViewButtons.RightScroll), true);
                    break;
            }
        }

        private void UpdateScrollOffset(float prevItemLength, SizeF availableSize)
        {
            if (this.ScrollOffset == 0)
            {
                return;
            }

            SizeF prevSize = this.PreviousConstraint;
            if (prevSize.Width <= 0 ||
                prevSize.Height <= 0 ||
                prevSize.Width == float.PositiveInfinity ||
                prevSize.Height == float.PositiveInfinity)
            {
                this.ResetScrollOffset();
                return;
            }

            float differ = 0;

            switch (this.layoutInfo.align)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    differ = availableSize.Width - prevSize.Width;
                    break;
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    differ = availableSize.Height - prevSize.Height;
                    break;
            }

            int newScrollOffset = this.ScrollOffset;

            if (differ > 0)
            {
                newScrollOffset -= (int)differ;
            }

            //if (this.layoutInfo.layoutLength < prevItemLength)
            //{
            //    differ = prevItemLength - this.layoutInfo.layoutLength;
            //    newScrollOffset -= (int)differ;
            //}

            this.SetScrollOffset(Math.Max(0, newScrollOffset), false);
        }

        private void ResetScrollOffset()
        {
            bool scroll = this.enableScrolling;

            this.EnableScrolling(false);
            this.SetScrollOffset(0, false);
            this.EnableScrolling(scroll);
        }

        private void SetScrollOffset(int offset, bool arrange)
        {
            if (this.layoutInfo != null)
            {
                float clientLength = this.layoutInfo.availableLength - this.layoutInfo.borderLength - this.layoutInfo.paddingLength;
                if (offset + clientLength > this.layoutInfo.layoutLength)
                {
                    offset = (int)(this.layoutInfo.layoutLength - clientLength);
                }
                offset = Math.Max(0, offset);
            }

            if (this.ScrollOffset == offset)
            {
                return;
            }

            if (this.enableScrolling)
            {
                if (this.scrollAnimation.IsAnimating(this))
                {
                    this.scrollAnimation.Stop(this);
                }

                this.scrollAnimation.StartValue = this.scrollAnimation.EndValue;
                this.scrollAnimation.EndValue = offset;

                this.scrollAnimation.ApplyValue(this);
            }
            else
            {
                this.ScrollOffset = offset;
            }

            if (arrange)
            {
                this.InvalidateArrange();
            }
        }

        private int GetScrollStep(SizeF available, StripViewButtons scrollButtons)
        {
            if (this.layoutInfo == null)
            {
                return 0;
            }

            //apply border and padding
            RectangleF client = this.GetClientRectangle(available);

            float hiddenLength = 0;
            float maxLength = 0;

            float itemLength = this.layoutInfo.layoutLength;

            switch (this.layoutInfo.align)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    if (itemLength > client.Width)
                    {
                        hiddenLength = itemLength - client.Width;
                    }
                    maxLength = client.Width;
                    break;
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    if (itemLength > client.Height)
                    {
                        hiddenLength = itemLength - client.Height;
                    }
                    maxLength = client.Height;
                    break;
            }

            float scrollStep;
            if (scrollButtons == StripViewButtons.LeftScroll)
            {
                scrollStep = this.ScrollOffset;
            }
            else
            {
                scrollStep = hiddenLength - this.ScrollOffset;
            }

            return (int)Math.Min(maxLength, scrollStep);
        }

        #endregion
    }
}
