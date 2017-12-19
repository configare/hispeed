using System;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using Telerik.WinControls.Layout;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represent a layout for the items contained in a strip
    /// </summary>
    public class RadCommandBarItemsPanel : StackLayoutPanel
    {
        #region Fields

        private RadCommandBarBaseItemCollection items;
        private LayoutPanel overflowPannel;
        private bool allowOverflow = true;

        #endregion

        #region Events

        public event EventHandler ItemOverflowed;
        public event EventHandler ItemOutOfOverflow;
        
        protected virtual void OnItemOverflowed(object sender, EventArgs args)
        {
            if (this.ItemOverflowed != null)
            {
                this.ItemOverflowed(sender, args);
            }
        }

        protected virtual void OnItemOutOfOverflow(object sender, EventArgs args)
        {
            if (this.ItemOutOfOverflow != null)
            {
                this.ItemOutOfOverflow(sender, args);
            }
        }

        #endregion

        #region Properties

        public bool AllowOverflow
        {
            get
            {
                return this.allowOverflow;
            }
            set
            {
                this.allowOverflow = value;
            }
        }

        public RadCommandBarItemsPanel(RadCommandBarBaseItemCollection itemsCollection, LayoutPanel overflowPannel)
        {
            this.items = itemsCollection;
            this.overflowPannel = overflowPannel;
        }

        #endregion

        #region Overrides

        protected override SizeF MeasureOverride(SizeF availableSize)
        { 
            SizeF totalSize = SizeF.Empty;
            bool isHorizontal = this.Orientation == Orientation.Horizontal;
            float sumOfSpace = 0;
            bool overflowed = false;
            int currentIndex;
            int visibleItemsCount = this.items.Count;
            int overflowedItemsCount = this.overflowPannel.Children.Count;

            if (visibleItemsCount == 0 && overflowedItemsCount == 0)//run out of items to arrange - simulate MinSize
            { 
                return isHorizontal ? new SizeF(30, 0) : new SizeF(0, 30);
            }

            CommandBarStripElement parent = (this.Parent as CommandBarStripElement);

            for (currentIndex = 0; currentIndex < visibleItemsCount; ++currentIndex)
            {
                RadCommandBarBaseItem child = this.items[currentIndex] as RadCommandBarBaseItem;
                if (child == null) continue;

                if (!child.VisibleInStrip && parent.Site == null)
                {
                    child.Measure(SizeF.Empty);
                    continue;
                }

                child.Measure(LayoutUtils.InfinitySize);
                float desiredSpace = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                float availableSpace = isHorizontal ? availableSize.Width : availableSize.Height;

                if (parent.Site != null)
                {
                    //do nothing - we are in design time
                }
                else if (sumOfSpace + desiredSpace > availableSpace && this.allowOverflow)
                {
                    overflowed = true;
                    break;
                }
                else
                {
                    if (isHorizontal)
                    {
                        child.Measure(new SizeF(
                            Math.Max(availableSize.Width - sumOfSpace , 0),
                            availableSize.Height));
                    }
                    else
                    {
                        child.Measure(new SizeF(
                            availableSize.Width, 
                            Math.Max(availableSize.Height - sumOfSpace,0)));
                    }
                }

                if (isHorizontal)
                {
                    totalSize.Width += child.DesiredSize.Width;
                    totalSize.Height = Math.Max(totalSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    totalSize.Height += child.DesiredSize.Height;
                    totalSize.Width = Math.Max(totalSize.Width, child.DesiredSize.Width);
                }

                sumOfSpace += desiredSpace;
            }

            if (overflowed && parent.Site == null && this.allowOverflow)
            {
                this.HandleOverflowedItems(visibleItemsCount, currentIndex);
            }
            else if (parent.Site == null)
            {
                this.MeasureOverflowedItems(overflowedItemsCount, availableSize, sumOfSpace, ref totalSize);
            }
             
            if (this.overflowPannel.Children.Count > 0)
            {
                if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    totalSize.Width = availableSize.Width;
                }
                else
                {
                    totalSize.Height = availableSize.Height;
                }
            }

            return totalSize;
        }

        //move items out of overflow panel 
        private void MeasureOverflowedItems(int overflowedItemsCount, SizeF availableSize, float sumOfSpace, ref SizeF totalSize)
        {

            bool isHorizontal = (this.Orientation == Orientation.Horizontal);

            for (int i = 0; i < overflowedItemsCount; ++i)
            {
                RadCommandBarBaseItem child = this.overflowPannel.Children[0] as RadCommandBarBaseItem;
                if (child == null) continue;

                if (!child.VisibleInStrip)
                {
                    child.Measure(SizeF.Empty);
                    this.items.Add(child);
                    this.OnItemOutOfOverflow(child, new EventArgs());
                    continue;
                }

                child.Measure(LayoutUtils.InfinitySize);

                float desiredSpace = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                float availableSpace = isHorizontal ? availableSize.Width : availableSize.Height;

                if (sumOfSpace + desiredSpace > availableSpace)
                {
                    break;
                }

                this.items.Add(child);
                this.OnItemOutOfOverflow(child, new EventArgs());
                if (isHorizontal)
                {
                    child.Measure(new SizeF(availableSize.Width - sumOfSpace, availableSize.Height));
                    totalSize.Width += child.DesiredSize.Width;
                    totalSize.Height = Math.Max(totalSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    child.Measure(new SizeF(availableSize.Width, availableSize.Height - sumOfSpace));
                    totalSize.Height += child.DesiredSize.Height;
                    totalSize.Width = Math.Max(totalSize.Width, child.DesiredSize.Width);
                }

                sumOfSpace += desiredSpace;
            }


        }

        //add items to overflow panel
        private void HandleOverflowedItems(int visibleItemsCount, int currentIndex)
        {
            for (int i = visibleItemsCount - 1; i >= currentIndex; --i)
            {
                RadCommandBarBaseItem child = this.items[i];
                this.items.Remove(child);
                this.overflowPannel.Children.Insert(0, child);
                this.OnItemOverflowed(child, new EventArgs());
            }
        }

        protected override SizeF ArrangeOverride(SizeF arrangeSize)
        {
            RadCommandBarBaseItemCollection children = this.items;
            int count = children.Count;

            // Get desired children size if EqualChildrenHeight or EqualChildrenWidth is used
            // ********************************************************* //
            SizeF maxDesiredChildrenSize = SizeF.Empty;
            bool equalChildrenHeight = false;// this.EqualChildrenHeight;
            bool equalChildrenWidth = false;// this.EqualChildrenWidth;
            if (equalChildrenHeight || equalChildrenWidth)
            {
                for (int i = 0; i < count; i++)
                {
                    RadElement element = children[i];
                    if (equalChildrenHeight)
                    {
                        maxDesiredChildrenSize.Height = Math.Max(element.DesiredSize.Height, maxDesiredChildrenSize.Height);
                    }
                    if (equalChildrenWidth)
                    {
                        maxDesiredChildrenSize.Width = Math.Max(element.DesiredSize.Width, maxDesiredChildrenSize.Width);
                    }
                }
            }

            // Parameters
            // ********************************************************* //
            bool isHorizontal = this.Orientation == Orientation.Horizontal;
            bool isRightToLeft = this.RightToLeft;

            float length = 0;
            RectangleF finalRect = new RectangleF(PointF.Empty, arrangeSize);
            if (isHorizontal && isRightToLeft)
                finalRect.X = arrangeSize.Width;

            float stretchedItemArrangedSpace = ArrangeStretchedItems(arrangeSize);

            // Main loop that does the actual arrangement of the children
            // ********************************************************* //
            for (int i = 0; i < count; i++)
            {
                RadElement element = children[i];

                SizeF childArea = element.DesiredSize;
                if (element.Visibility == ElementVisibility.Collapsed)
                {
                    element.Arrange(new RectangleF(PointF.Empty, childArea));
                    continue;
                }

                // ** 1. Calculate the ChildArea
                if (equalChildrenHeight)
                {
                    if (isHorizontal)
                        childArea.Height = Math.Max(arrangeSize.Height, maxDesiredChildrenSize.Height);
                    else
                        childArea.Height = maxDesiredChildrenSize.Height;
                }
                if (equalChildrenWidth)
                {
                    if (isHorizontal)
                        childArea.Width = maxDesiredChildrenSize.Width;
                    else
                        childArea.Width = Math.Max(arrangeSize.Width, maxDesiredChildrenSize.Width);
                }
                if (element.StretchHorizontally && isHorizontal && !float.IsInfinity(stretchedItemArrangedSpace))
                {
                    childArea.Width = stretchedItemArrangedSpace;
                }
                if (element.StretchVertically && !isHorizontal && !float.IsInfinity(stretchedItemArrangedSpace))
                {
                    childArea.Height = stretchedItemArrangedSpace;
                }
                // ** 2. Calculate the location and size (finalRect) that will be passed to the child's Arrange
                if (isHorizontal)
                {
                    if (isRightToLeft)
                    {
                        length = childArea.Width;
                        finalRect.X -= length;
                    }
                    else
                    {
                        finalRect.X += length;
                        length = childArea.Width;
                    }

                    finalRect.Width = length;

                    if (equalChildrenHeight)
                    {
                        SizeF arrangeArea = finalRect.Size;
                        finalRect.Height = childArea.Height;

                        // Compensate the alignment for EqualChildrenHeight because the basic logic will be bypassed
                        // by the size forcing
                        // Note that the vertical alignment is not affected by RightToLeft...
                        RectangleF alignedRect = LayoutUtils.Align(finalRect.Size, new RectangleF(PointF.Empty, arrangeArea), this.Alignment);
                        finalRect.Y += alignedRect.Y;
                    }
                    else
                    {
                        finalRect.Height = arrangeSize.Height;// Math.Max(arrangeSize.Height, childArea.Height);
                    }
                }
                else
                {
                    finalRect.Y += length;
                    length = childArea.Height;
                    finalRect.Height = length;
                    if (equalChildrenWidth)
                    {
                        SizeF arrangeArea = finalRect.Size;
                        finalRect.Width = childArea.Width;

                        // Compensate the alignment for EqualChildrenHeight because the basic logic will be bypassed
                        // by the size forcing
                        // Note that the horizontal alignment is translated if RightToLeft is true.
                        ContentAlignment alignment = isRightToLeft ? TelerikAlignHelper.RtlTranslateContent(this.Alignment) : this.Alignment;
                        RectangleF alignedRect = LayoutUtils.Align(finalRect.Size, new RectangleF(PointF.Empty, arrangeArea), alignment);
                        finalRect.X += alignedRect.X;
                    }
                    else
                    {
                        finalRect.Width = arrangeSize.Width;// Math.Max(arrangeSize.Width, childArea.Width);
                    }
                }

                // ** 3. Arrange the child
                if (element.StretchVertically && isHorizontal)
                {
                    finalRect.Height = arrangeSize.Height;
                }
                else if (element.StretchHorizontally && !isHorizontal)
                {
                    finalRect.Width = arrangeSize.Width;
                }
                element.Arrange(finalRect);
            }

            return arrangeSize;
        }

        private float ArrangeStretchedItems(SizeF arrangeSize)
        {
            int itemsCount = this.items.Count;
            float totalDesiredSpace = 0;
            float totalSpace = (this.Orientation == System.Windows.Forms.Orientation.Horizontal) ?
                               arrangeSize.Width : arrangeSize.Height;
            int stretchedItemsCount = 0;
            for (int i = 0; i < itemsCount; ++i)
            {
                RadCommandBarBaseItem child = this.items[i] as RadCommandBarBaseItem;
                if (child == null || !child.VisibleInStrip || child.Visibility == ElementVisibility.Collapsed)
                {
                    continue;
                }

                bool isChildStretched = (child.StretchHorizontally && this.Orientation == Orientation.Horizontal) ||
                                        (child.StretchVertically && this.Orientation == Orientation.Vertical);

                if (!isChildStretched)
                {
                    totalDesiredSpace += (this.Orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                         child.DesiredSize.Width : child.DesiredSize.Height;
                }
                else
                {
                    stretchedItemsCount++;
                }
            }

            float spaceForStretching = totalSpace - totalDesiredSpace;
            if (spaceForStretching == 0)
            {
                return 0;
            }

            float stretchedItemArrangedSpace = spaceForStretching / (float)stretchedItemsCount;
            return stretchedItemArrangedSpace;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == StackLayoutPanel.OrientationProperty)
            {
                CommandBarStripElement parent = (this.Parent as CommandBarStripElement);
                foreach (RadCommandBarBaseItem item in parent.Items)
                {
                    if (item.InheritsParentOrientation)
                        item.Orientation = this.Orientation;
                }
                foreach (RadCommandBarBaseItem item in parent.OverflowButton.ItemsLayout.Children)
                {
                    if (item.InheritsParentOrientation)
                        item.Orientation = this.Orientation;
                }
            }
            base.OnPropertyChanged(e);
        }
        #endregion

        internal SizeF GetExpectedSize(SizeF availableSize)
        {
            SizeF result = SizeF.Empty;
            foreach (RadElement child in this.Children)
            {
                RadCommandBarBaseItem item = child as RadCommandBarBaseItem;
                if (item == null || !item.VisibleInStrip)
                {
                    continue;
                }

                item.Measure(availableSize);
                if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    result.Width += item.DesiredSize.Width;
                    result.Height = Math.Max(result.Height, item.DesiredSize.Height);
                }
                else
                {
                    result.Height += item.DesiredSize.Height;
                    result.Width = Math.Max(result.Width, item.DesiredSize.Width);
                }
            }

            foreach (RadElement child in this.overflowPannel.Children)
            {
                RadCommandBarBaseItem item = child as RadCommandBarBaseItem;
                if (item == null || !item.VisibleInStrip)
                {
                    continue;
                }

                child.Measure(availableSize);
                if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    result.Width += child.DesiredSize.Width;
                    result.Height = Math.Max(result.Height, child.DesiredSize.Height);
                }
                else
                {
                    result.Height += child.DesiredSize.Height;
                    result.Width = Math.Max(result.Width, child.DesiredSize.Width);
                }
            }

            return result;
        }
    }
}
