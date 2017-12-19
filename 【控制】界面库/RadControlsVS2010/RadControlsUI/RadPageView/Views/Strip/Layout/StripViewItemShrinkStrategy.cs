using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    internal class StripViewItemShrinkStrategy
    {
        #region Nested Types

        private class ShrinkInfoComparer : IComparer<PageViewItemSizeInfo>
        {
            #region Fields

            private StripViewItemShrinkStrategy strategy;

            #endregion

            #region Constructor

            public ShrinkInfoComparer(StripViewItemShrinkStrategy strategy)
            {
                this.strategy = strategy;
            }

            #endregion

            #region IComparer Members

            public int Compare(PageViewItemSizeInfo x, PageViewItemSizeInfo y)
            {
                float length1 = x.layoutLength;
                float length2 = y.layoutLength;

                if (length1 == length2 && this.strategy.shrinkInfo.collapse)
                {
                    length1 = x.currentLength;
                    length2 = y.currentLength;
                }

                return length1.CompareTo(length2);
            }

            #endregion
        }

        private class StripViewShrinkInfo
        {
            public float layoutLength;
            public float shrinkAmount;
            public float currentLength;
            public float minLength;
            public bool collapse;
            public bool execute;
        }

        #endregion

        #region Fields

        private StripViewLayoutInfo layoutInfo;
        private StripViewShrinkInfo shrinkInfo;

        #endregion

        #region Constructor

        public StripViewItemShrinkStrategy(StripViewLayoutInfo sizeInfo)
        {
            this.layoutInfo = sizeInfo;
            this.shrinkInfo = new StripViewShrinkInfo();
        }

        #endregion

        #region Public Methods

        public void Execute()
        {
            if (!this.CanShrink())
            {
                return;
            }

            this.UpdateShrinkInfo();
            if (!this.shrinkInfo.execute)
            {
                return;
            }

            this.layoutInfo.items.Sort(new ShrinkInfoComparer(this));

            if (this.shrinkInfo.collapse)
            {
                this.CollapseItems();
            }
            else
            {
                this.ExpandItems();
            }
        }

        #endregion

        #region Collapse

        private void CollapseItems()
        {
            int startIndex = this.layoutInfo.itemCount - 1;
            int compareIndex = startIndex - 1;
            int singleCollapse;

            while (this.shrinkInfo.shrinkAmount > 0 && compareIndex >= 0)
            {
                float differ = this.GetCollapseDifference(startIndex, compareIndex);

                if (differ > 0)
                {
                    int endIndex = compareIndex + 1;
                    singleCollapse = this.GetSingleCollapse(differ, this.layoutInfo.itemCount - endIndex);
                    for (int i = startIndex; i >= endIndex; i--)
                    {
                        this.CollapseItem(this.layoutInfo.items[i], singleCollapse);
                    }
                }

                compareIndex--;
            }

            if (this.shrinkInfo.shrinkAmount > 0)
            {
                singleCollapse = this.GetEvenCollapse((int)this.shrinkInfo.shrinkAmount, this.layoutInfo.itemCount);
                for (int i = this.layoutInfo.itemCount - 1; i >= 0; i--)
                {
                    this.CollapseItem(this.layoutInfo.items[i], singleCollapse);
                }
            }
        }

        private int GetEvenCollapse(int amount, int itemCount)
        {
            int modulus = amount % itemCount;
            return (amount + (itemCount - modulus)) / itemCount;
        }

        private float GetCollapseDifference(int index1, int index2)
        {
            PageViewItemSizeInfo item1 = this.layoutInfo.items[index1];
            PageViewItemSizeInfo item2 = this.layoutInfo.items[index2];

            float differ = item1.layoutLength - item2.layoutLength;
            Debug.Assert(differ >= 0, "Invalid collapse algorithm");

            return differ;
        }

        private int GetSingleCollapse(float difference, int itemCount)
        {
            float currentShrinkAmount = Math.Min(this.shrinkInfo.shrinkAmount, difference);
            int singleCollapse;
            if (currentShrinkAmount * itemCount <= this.shrinkInfo.shrinkAmount)
            {
                singleCollapse = (int)currentShrinkAmount;
            }
            else
            {
                int modulus = (int)this.shrinkInfo.shrinkAmount % itemCount;
                singleCollapse = (int)((this.shrinkInfo.shrinkAmount + itemCount - modulus) / itemCount);
            }

            return singleCollapse;
        }

        private void CollapseItem(PageViewItemSizeInfo item, int collapse)
        {
            float layoutLength = item.layoutLength;
            float minLength = item.minLength;

            int maxCollapse = (int)Math.Max(0, layoutLength - minLength);
            maxCollapse = Math.Min(maxCollapse, collapse);

            SizeF forcedSize;
            if (this.layoutInfo.vertical)
            {
                forcedSize = new SizeF(item.layoutSize.Width, item.layoutSize.Height - maxCollapse);
            }
            else
            {
                forcedSize = new SizeF(item.layoutSize.Width - maxCollapse, item.layoutSize.Height);
            }

            item.SetLayoutSize(forcedSize);
            this.shrinkInfo.shrinkAmount -= collapse;
        }

        #endregion

        #region Expand

        private void ExpandItems()
        {
            int startIndex = 0;
            int expandItemCount = this.layoutInfo.itemCount;

            while (this.shrinkInfo.shrinkAmount > 0 && startIndex < this.layoutInfo.itemCount)
            {
                float differ = this.GetExpandDifference(startIndex);

                if (differ <= 0)
                {
                    expandItemCount--;
                }
                else
                {
                    int singleExpand = this.GetSingleExpand(differ, expandItemCount);
                    if (singleExpand > 0)
                    {
                        for (int i = startIndex; i < this.layoutInfo.itemCount; i++)
                        {
                            this.ExpandItem(this.layoutInfo.items[i], singleExpand);
                        }
                    }
                }

                startIndex++;
            }
        }

        private void ExpandItem(PageViewItemSizeInfo item, int expand)
        {
            SizeF expandSize;
            if (this.layoutInfo.vertical)
            {
                expandSize = new SizeF(item.layoutSize.Width, item.currentSize.Height + expand);
            }
            else
            {
                expandSize = new SizeF(item.currentSize.Width + expand, item.layoutSize.Height);
            }

            item.SetCurrentSize(expandSize);
            item.SetLayoutSize(expandSize);

            this.shrinkInfo.shrinkAmount -= expand;
        }

        private float GetExpandDifference(int index)
        {
            PageViewItemSizeInfo item = this.layoutInfo.items[index];

            float differ = item.layoutLength - item.currentLength;
            Debug.Assert(differ >= 0, "Invalid expand algorithm");

            item.SetLayoutSize(item.currentSize);

            return differ;
        }

        private int GetSingleExpand(float difference, int itemCount)
        {
            if (difference <= 0 || this.shrinkInfo.shrinkAmount < itemCount)
            {
                return 0;
            }

            if (difference > this.shrinkInfo.shrinkAmount)
            {
                difference = this.shrinkInfo.shrinkAmount;
            }

            while (difference > 1)
            {
                if (difference * itemCount <= this.shrinkInfo.shrinkAmount)
                {
                    break;
                }
                difference--;
            }

            if (difference * itemCount > this.shrinkInfo.shrinkAmount)
            {
                int modulus = (int)difference % itemCount;
                difference = Math.Max(1, difference - modulus);
            }

            return (int)difference;
        }

        #endregion

        #region Helper Mehtods

        private float GetSizeLength(SizeF size)
        {
            return this.layoutInfo.vertical ? size.Height : size.Width;
        }

        private float GetPaddingLength(Padding padding)
        {
            return this.layoutInfo.vertical ? padding.Vertical : padding.Horizontal;
        }

        private void UpdateShrinkInfo()
        {
            foreach (PageViewItemSizeInfo itemInfo in this.layoutInfo.items)
            {
                this.shrinkInfo.layoutLength += itemInfo.layoutLength + itemInfo.marginLength;
                this.shrinkInfo.currentLength += itemInfo.currentLength + itemInfo.marginLength;
                this.shrinkInfo.minLength += itemInfo.minLength + itemInfo.marginLength;
            }

            //add padding and item spacing
            int itemSpacing = (this.layoutInfo.itemCount - 1) * this.layoutInfo.itemSpacing;
            this.shrinkInfo.layoutLength += this.layoutInfo.paddingLength + this.layoutInfo.borderLength + itemSpacing;
            this.shrinkInfo.currentLength += this.layoutInfo.paddingLength + this.layoutInfo.borderLength + itemSpacing;
            this.shrinkInfo.minLength += this.layoutInfo.paddingLength + this.layoutInfo.borderLength + itemSpacing;

            float prevLength = this.GetSizeLength(this.layoutInfo.itemLayout.PreviousConstraint);
            if (prevLength == 0)
            {
                prevLength = this.shrinkInfo.layoutLength;
            }

            this.shrinkInfo.collapse = this.shrinkInfo.currentLength >= this.layoutInfo.availableLength ||
                this.shrinkInfo.currentLength == this.shrinkInfo.minLength ||
                this.layoutInfo.availableSize == this.layoutInfo.previousSize;

            if (this.shrinkInfo.collapse)
            {
                this.shrinkInfo.shrinkAmount = this.shrinkInfo.layoutLength - this.layoutInfo.availableLength;
            }
            else
            {
                this.shrinkInfo.shrinkAmount = this.layoutInfo.availableLength - this.shrinkInfo.currentLength;
            }

            this.shrinkInfo.execute = this.shrinkInfo.layoutLength > this.layoutInfo.availableLength;
        }

        private bool CanShrink()
        {
            if (this.layoutInfo.contentOrientation == PageViewContentOrientation.Auto)
            {
                return true;
            }

            if (this.layoutInfo.vertical)
            {
                return this.layoutInfo.contentOrientation == PageViewContentOrientation.Vertical270 ||
                    this.layoutInfo.contentOrientation == PageViewContentOrientation.Vertical90;
            }

            return this.layoutInfo.contentOrientation == PageViewContentOrientation.Horizontal ||
                    this.layoutInfo.contentOrientation == PageViewContentOrientation.Horizontal180;
        }

        #endregion
    }
}
