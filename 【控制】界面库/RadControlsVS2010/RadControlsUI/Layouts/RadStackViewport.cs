using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadStackViewport : StackLayoutPanel, IRadScrollViewport
    {
        internal const ulong ExtentSizeInvalidatedStateKey = StackLayoutPanelLastStateKey << 1;
        internal const ulong RadStackViewportLastStateKey = ExtentSizeInvalidatedStateKey;
        private Size extentSize;

        #region Overrides

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            base.OnChildrenChanged(child, changeOperation);

            this.BitState[ExtentSizeInvalidatedStateKey] = true;
        }

        #endregion


        #region IRadScrollViewport Members

        public virtual Size GetExtentSize()
        {
            if (this.GetBitState(ExtentSizeInvalidatedStateKey))
            {
                this.BitState[ExtentSizeInvalidatedStateKey] = false;
                this.extentSize = CalcExtentSize();
            }
            return this.extentSize;
        }

        public virtual void InvalidateViewport()
        {
            this.BitState[ExtentSizeInvalidatedStateKey] = true;
        }

        public virtual Point ResetValue(Point currentValue, Size viewportSize, Size extentSize)
        {
            if (this.Children.Count <= 0)
                return currentValue;

            Point res = currentValue;

            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                res.X = RadCanvasViewport.ValidatePosition(currentValue.X,
                    extentSize.Width - viewportSize.Width);
                int vertPos = GetOffsetFromIndex(currentValue.Y);
                if (vertPos > extentSize.Height - viewportSize.Height)
                    res.Y = this.Children.Count - GetLastFullVisibleItemsNum();
            }
            else
            {
                int horizPos = GetOffsetFromIndex(currentValue.X);
                if (horizPos > extentSize.Width - viewportSize.Width)
                    res.X = this.Children.Count - GetLastFullVisibleItemsNum();
                res.Y = RadCanvasViewport.ValidatePosition(currentValue.Y,
                    extentSize.Height - viewportSize.Height);
            }

            return res;
        }

        public virtual void DoScroll(Point oldValue, Point newValue)
        {
            Point newOffset = newValue;
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                newOffset.Y = GetOffsetFromIndex(newValue.Y);
            }
            else
            {
                newOffset.X = GetOffsetFromIndex(newValue.X);
            }
            this.PositionOffset = new SizeF(-newOffset.X, -newOffset.Y);
        }

        public virtual Size ScrollOffsetForChildVisible(RadElement childElement, Point currentScrollValue)
        {
            int childIndex = this.Children.IndexOf(childElement);
            if (childIndex < 0 || childIndex >= this.Children.Count)
                return Size.Empty;

            Rectangle clientRect = new Rectangle(Point.Empty, this.Size);
            Rectangle childRect = childElement.FullBoundingRectangle;
            childRect.Offset((int)Math.Round(this.PositionOffset.Width), (int)Math.Round(this.PositionOffset.Height));
            Size childOffset = RadCanvasViewport.CalcMinOffset(childRect, clientRect);
            Size viewportOffset = new Size(-childOffset.Width, -childOffset.Height);

            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                viewportOffset.Height = ConvertPixelOffsetToIndexOffset(currentScrollValue.Y, childIndex, viewportOffset.Height);
            }
            else
            {
                viewportOffset.Width = ConvertPixelOffsetToIndexOffset(currentScrollValue.X, childIndex, viewportOffset.Width);
            }

            return viewportOffset;
        }

        public virtual ScrollPanelParameters GetScrollParams(Size viewportSize, Size extentSize)
        {
            int visibleItemsNum = GetNonCollapsedChildrenNum();

            if (visibleItemsNum == 0)
            {
                return new ScrollPanelParameters(
                    0, 0, 0, 0,
                    0, 0, 0, 0
                );
            }

            int horizontalMaxValue = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                Math.Max(1, extentSize.Width) : Math.Max(1, visibleItemsNum - 1);
            int verticalMaxValue = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                Math.Max(1, visibleItemsNum - 1) : Math.Max(1, extentSize.Height);

            // 16 - pixels; 1 - index
            int horizontalSmallChange = this.Orientation == System.Windows.Forms.Orientation.Vertical ? 16 : 1;
            int verticalSmallChange = this.Orientation == System.Windows.Forms.Orientation.Vertical ? 1 : 16;

            int horizontalLargeChange = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                Math.Max(1, viewportSize.Width) : GetLastFullVisibleItemsNum();
            int verticalLargeChange = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                GetLastFullVisibleItemsNum() : Math.Max(1, viewportSize.Height);

            return new ScrollPanelParameters(
                0, horizontalMaxValue, horizontalSmallChange, horizontalLargeChange,
                0, verticalMaxValue, verticalSmallChange, verticalLargeChange
            );
        }

        #endregion


        #region Operations

        protected virtual Size CalcExtentSize()
        {
            return Size.Round(this.DesiredSize);
        }

        private int ConvertPixelOffsetToIndexOffset(int topIndex, int indexToOffset, int pixelOffset)
        {
            int childrenCount = this.Children.Count;
            if (pixelOffset == 0 || topIndex < 0 || topIndex >= childrenCount ||
                indexToOffset < 0 || indexToOffset >= childrenCount)
                return 0;

            int sumSize = 0;

            if (pixelOffset > 0) // Scroll Down
            {
                for (int i = topIndex; i < childrenCount; i++)
                {
                    RadElement child = this.Children[i];
                    if (child.Visibility != ElementVisibility.Collapsed)
                    {
                        sumSize += this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                                child.FullSize.Height : child.FullSize.Width;
                        if (sumSize >= pixelOffset)
                        {
                            return i - topIndex + 1;
                        }
                    }
                }
                return childrenCount - GetLastFullVisibleItemsNum() - topIndex;
            }
            else // Scroll Up
            {
                return -(topIndex - indexToOffset);
            }
        }

        /// <summary>
        /// Returns the number of items that are visible when the viewport is scrolled to its
        /// maximum value (the bottom for vertical stack and the right-most place for left-to-right
        /// horizontal stack). The last item must always be fully visible.
        /// If there are children the result will be at least 1.
        /// </summary>
        /// <returns>Number of full visible items in the viewport. If the items are with different sizes,
        /// the last items are used in the calculations.</returns>
        protected virtual int GetLastFullVisibleItemsNum()
        {
            if (this.Children.Count <= 0)
                return 0;

            int totalSize = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                this.Size.Height : this.Size.Width;
            int sumSize = 0;
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                RadElement child = this.Children[i];
                if (child.Visibility != ElementVisibility.Collapsed)
                {
                    sumSize += this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                        child.FullSize.Height : child.FullSize.Width;
                    if (sumSize > totalSize)
                    {
                        return Math.Max(1, this.Children.Count - i - 1);
                    }
                }
            }
            return this.Children.Count;
        }

        protected virtual int GetNonCollapsedChildrenNum()
        {
            int res = 0;
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                if (child.Visibility != ElementVisibility.Collapsed)
                {
                    res++;
                }
            }
            return res;
        }

        // The topIndex is not included in the calculations - we want the element to be visible
        protected int GetOffsetFromIndex(int topIndex)
        {
            if (this.Children.Count <= 0 || topIndex <= 0)
                return 0;

            int offset = 0;
            int count = Math.Min(topIndex, this.Children.Count);
            for (int i = 0; i < count; i++)
            {
                RadElement child = this.Children[i];
                if (child.Visibility != ElementVisibility.Collapsed)
                {
                    offset += this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                         child.FullSize.Height : child.FullSize.Width;
                }
            }

            return offset;
        }

        #endregion
    }
}
