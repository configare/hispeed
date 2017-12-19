using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Delegate for filtering list of elements.
    /// </summary>
    /// <param name="children">List of elements that should be filtered.</param>
    /// <returns>List of filtered elements taken from the children</returns>
    public delegate RadElementReadonlyList StripViewportFilter(RadElementReadonlyList children);

    public class RadStripViewport : StackLayoutPanel, IRadScrollViewport
    {
        #region BitState Keys

        internal const ulong ItemsInvalidatedStateKey = StackLayoutPanelLastStateKey << 1;
        internal const ulong ExtentSizeInvalidatedStateKey = ItemsInvalidatedStateKey << 1;
        internal const ulong RadStripViewportLastStateKey = ExtentSizeInvalidatedStateKey;

        #endregion

        #region Fields

        private Size extentSize;
        private RadElementReadonlyList items;

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
            this.BitState[ItemsInvalidatedStateKey] = true;
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
            Rectangle childRect = childElement.FullRectangle;
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

        #region Properties
        private StripViewportFilter filter;
        /// <summary>
        /// The filter allows to collapse some of the children in the viewport. Only the elements that are
        /// in the returned list are visible and are used in the layout mechanism. If the property is null
        /// then all children are visible.
        /// </summary>
        /// <value>
        /// A delegate for filtering - see <see cref="StripViewportFilter"/>. By default is null.
        /// </value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StripViewportFilter Filter
        {
            get { return filter; }
            set { filter = value; }
        }
        #endregion

        #region Constructors

        public RadStripViewport()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[ExtentSizeInvalidatedStateKey] = true;
            this.BitState[ItemsInvalidatedStateKey] = true;
            this.ClipDrawing = false;
        }

        public RadStripViewport(StripViewportFilter filter) : this()
        {
            this.filter = filter;
        }

        #endregion

        #region Overrides

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            base.OnChildrenChanged(child, changeOperation);

            this.BitState[ItemsInvalidatedStateKey] = true;
            this.BitState[ExtentSizeInvalidatedStateKey] = true;
        }

        protected IEnumerable<RadElement> GetChildrenForLayout()
        {
            if (this.GetBitState(ItemsInvalidatedStateKey))
            {
                this.BitState[ItemsInvalidatedStateKey] = false;
                if (this.items != null)
                    this.items.Clear();

                RadElementReadonlyList allChildren = new RadElementReadonlyList(this.Children);
                if (this.filter != null)
                {
                    this.items = this.filter(allChildren);
                    CollapseNonFilterred(this.items);
                }
                else
                {
                    this.items = allChildren;
                }
            }
            return this.items;
        }

        #endregion

		#region Operations

        private void CollapseNonFilterred(RadElementReadonlyList filterred)
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                if (!filterred.Contains(child))
                {
                    child.Visibility = ElementVisibility.Collapsed;
                }
            }
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

        private Size GetDesiredItemSize(RadElement item)
        {
            RadListBoxItem listBoxItem = item as RadListBoxItem;
			if (listBoxItem != null && !this.EqualChildrenHeight)
                return listBoxItem.GetDesiredSize();
            return item.FullSize;
        }

        protected virtual Size CalcExtentSize()
        {
            RadElementReadonlyList currentItems = new RadElementReadonlyList(this.GetChildren(ChildrenListOptions.Normal));
            return GetDesiredChildrenSize(currentItems);
        }
        #endregion

        protected int GetDesiredMaxWidth(RadElementReadonlyList children)
        {
            Size childSize = Size.Empty;
            int maxWidth = 0;
            for (int i = 0; i < children.Count; i++)
            {
                childSize = GetDesiredItemSize(children[i]);
                if (maxWidth < childSize.Width)
                {
                    maxWidth = childSize.Width;
                }
            }
            return maxWidth;
        }

        protected int GetDesiredSumWidth(RadElementReadonlyList children)
        {
            Size childSize = Size.Empty;
            int sumWidth = 0;
            for (int i = 0; i < children.Count; i++)
            {
                childSize = GetDesiredItemSize(children[i]);
                sumWidth += childSize.Width;
            }
            return sumWidth;
        }

        protected int GetDesiredMaxHeight(RadElementReadonlyList children)
        {
            Size childSize = Size.Empty;
            int maxHeight = 0;
            for (int i = 0; i < children.Count; i++)
            {
                childSize = GetDesiredItemSize(children[i]);
                if (maxHeight < childSize.Height)
                {
                    maxHeight = childSize.Height;
                }
            }
            return maxHeight;
        }

        protected int GetDesiredSumHeight(RadElementReadonlyList children)
        {
            Size childSize = Size.Empty;
            int sumHeight = 0;
            for (int i = 0; i < children.Count; i++)
            {
                childSize = GetDesiredItemSize(children[i]);
                sumHeight += childSize.Height;
            }
            return sumHeight;
        }

        protected Size GetDesiredChildrenSize(RadElementReadonlyList children)
        {
			if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                return new Size(GetDesiredSumWidth(children), GetDesiredMaxHeight(children));
            }
            return new Size(GetDesiredMaxWidth(children), GetDesiredSumHeight(children));
        }
    }
}
