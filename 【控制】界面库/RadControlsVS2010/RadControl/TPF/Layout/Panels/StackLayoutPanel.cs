using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using System.ComponentModel;

namespace Telerik.WinControls.Layouts
{
    /// <summary>
    /// 	<para>Layout panel is a container for other elements. It orders the contained
    ///     elements as a stack vertically or horizontally. When the elements pass through the
    ///     left end of the stacklayout, the last one is put on a new line. If horizontal is
    ///     chosen the width of all elements is the width of the largest element in the
    ///     column.</para>
    /// </summary>
    public class StackLayoutPanel : LayoutPanel
    {
        public static RadProperty RowProperty = RadProperty.Register(
            "Row", typeof(int), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty OrientationProperty = RadProperty.Register(
            "Orientation", typeof(Orientation), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                Orientation.Horizontal, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout
                | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty CollapseElementsOnResizeProperty = RadProperty.Register(
            "CollapseElementsOnResize", typeof(bool), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        public static RadProperty AllElementsEqualSizeProperty = RadProperty.Register(
            "AllElementsEqualSize", typeof(bool), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty ChildrenForcedSizeProperty = RadProperty.Register(
            "ChildrenForcedSize", typeof(Size), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                Size.Empty, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty FlipMaxSizeDimensionsProperty = RadProperty.Register(
            "FlipMaxSizeDimensions", typeof(bool), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty UseParentSizeAsAvailableSizeProperty = RadProperty.Register(
            "UseParentSizeAsAvailableSize", typeof(bool), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsInStripModeProperty = RadProperty.Register(
            "IsInStripMode", typeof(bool), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        internal const ulong StackLayoutPanelLastStateKey = LayoutPanelLastStateKey;


        /// <summary>
        /// Gets or sets the elements orientation inside the stacklayout. 
        /// Possible values are horizontal and vertical.
        /// </summary>
        [RadPropertyDefaultValue("Orientation", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
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

        /// <summary>
        /// Gets or sets a value indicating whether the elements have equal size.
        /// </summary>
        [RadPropertyDefaultValue("AllElementsEqualSize", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool AllElementsEqualSize
        {
            get
            {
                return (bool)this.GetValue(AllElementsEqualSizeProperty);
            }
            set
            {
                this.SetValue(AllElementsEqualSizeProperty, value);
            }
        }
        public static readonly RadProperty EqualChildrenWidthProperty = RadProperty.Register(
            "EqualChildrenWidth", typeof(bool), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets a value indicating whether the elements have equal width.
        /// </summary>
        [RadPropertyDefaultValue("EqualChildrenWidth", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool EqualChildrenWidth
        {
            get
            {
                return (bool)this.GetValue(EqualChildrenWidthProperty);
            }
            set
            {
                this.SetValue(EqualChildrenWidthProperty, value);
            }
        }

        public static readonly RadProperty EqualChildrenHeightProperty = RadProperty.Register(
            "EqualChildrenHeight", typeof(bool), typeof(StackLayoutPanel), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets a value indicating whether the elements have equal height.
        /// </summary>
        [RadPropertyDefaultValue("EqualChildrenHeight", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool EqualChildrenHeight
        {
            get
            {
                return (bool)this.GetValue(EqualChildrenHeightProperty);
            }
            set
            {
                this.SetValue(EqualChildrenHeightProperty, value);
            }
        }

        /// <summary>
        /// ChildrenForcedSize
        /// </summary>
        public Size ChildrenForcedSize
        {
            get
            {
                return (Size)this.GetValue(ChildrenForcedSizeProperty);
            }
            set
            {
                this.SetValue(ChildrenForcedSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether maximum size dimensions are
        /// flipped.
        /// </summary>
        [RadPropertyDefaultValue("FlipMaxSizeDimensions", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool FlipMaxSizeDimensions
        {
            get
            {
                return (bool)this.GetValue(FlipMaxSizeDimensionsProperty);
            }
            set
            {
                this.SetValue(FlipMaxSizeDimensionsProperty, value);
            }
        }

        /// <summary>Gets or sets a value indicating whether elements are collapsed on resize.</summary>
        [RadPropertyDefaultValue("CollapseElementsOnResize", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool CollapseElementsOnResize
        {
            get
            {
                return (bool)this.GetValue(CollapseElementsOnResizeProperty);
            }
            set
            {
                this.SetValue(CollapseElementsOnResizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the panel will use its direct parent size to arrange the child elements or
        /// whether it will use the first ancestor which is a layout panel or an element with AutoSizeMode = FitToAvailableSize.
        /// </summary>
        [RadPropertyDefaultValue("UseParentSizeAsAvailableSize", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool UseParentSizeAsAvailableSize
        {
            get
            {
                return (bool)this.GetValue(UseParentSizeAsAvailableSizeProperty);
            }
            set
            {
                this.SetValue(UseParentSizeAsAvailableSizeProperty, value);
            }
        }

        /// <summary>
        ///	Gets or sets a value indicating whether the panel is in Strip mode or not. When in Strip mode the panel doesn't
        /// move the child elements to a new row when there's not enough space but rather arranges all elements on a single row.
        /// </summary>
        [RadPropertyDefaultValue("IsInStripMode", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool IsInStripMode
        {
            get
            {
                return (bool)this.GetValue(IsInStripModeProperty);
            }
            set
            {
                this.SetValue(IsInStripModeProperty, value);
            }
        }

        /// <summary>
        /// Notifies all children when same child changes. Effectively redraws all
        /// children in the panel.
        /// </summary>
        public override bool InvalidateChildrenOnChildChanged
        {
            get
            {
                return (this.Orientation == Orientation.Vertical);
            }
        }

        /// <summary>Gets a value indicating whether children are suppressed.</summary>
        [Obsolete("Alignment cannot be disabled anymore (use TopLeft to avoid alignment offset)")]
        public override bool SuppressChildrenAlignment
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Performs layout changes based on the element given as a paramater. 
        /// Sizes and places are determined by the concrete layout panel that is used. 
        /// Since all layout panels update their layout automatically through events, 
        /// this function is rarely used directly.
        /// </summary>
        /// <param name="affectedElement"></param>
        public override void PerformLayoutCore(RadElement affectedElement)
        {
            Size availableSize = this.UseParentSizeAsAvailableSize ? this.Parent.Size : this.AvailableSize;
            Size maxPanelSize;
            if (this.Orientation == Orientation.Horizontal)
                maxPanelSize = LayoutHorizontal(availableSize);
            else
                maxPanelSize = LayoutVertical(availableSize);

            /*
            if ((RightToLeft) && (this.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize))
            {
                this.Location = new Point(availableSize.Width - maxPanelSize.Width, this.Location.Y);
            }
            else
            {
                this.Location = new Point(0, this.Location.Y);
            }
             */
        }

        protected bool ShouldCollapseElements(List<PreferredSizeData> list, Size availableSize)
        {
            int sumWidth = 0;
            bool containsExpandedElements = false;
            bool shouldCollapseElements = false;

            //Woraraound: the first time ribon appears available Size is calculated incorrectly - no need to collapse 
            if (availableSize.Width == 0 || availableSize.Height == 0)
                return false;

            foreach (PreferredSizeData data in list)
            {
                sumWidth += GetDataSize(data, false).Width + data.Element.Margin.Horizontal;

                if (sumWidth > availableSize.Width)
                    shouldCollapseElements = true;

                ICollapsibleElement collapsibleElement = data.Element as ICollapsibleElement;

                if ((collapsibleElement != null) && (collapsibleElement.CollapseStep < collapsibleElement.CollapseMaxSteps))
                    containsExpandedElements = true;

                if (shouldCollapseElements && containsExpandedElements)
                    return true;
            }

            return false;
        }

        protected bool ShouldExpandElements(List<PreferredSizeData> list, Size availableSize)
        {
            int sumWidth = 0;
            bool containsCollapsedElements = false;

            foreach (PreferredSizeData data in list)
            {
                sumWidth += GetDataSize(data, false).Width + data.Element.Margin.Horizontal;

                ICollapsibleElement collapsibleElement = data.Element as ICollapsibleElement;

                if ((collapsibleElement != null) && (collapsibleElement.CollapseStep > 1))
                    containsCollapsedElements = true;
            }

            return (containsCollapsedElements && (sumWidth < availableSize.Width));
        }

        protected PreferredSizeData GetElementToCollapse(List<PreferredSizeData> list)
        {
            for (int i = 1; ; i++)
            {
                PreferredSizeData lastElementToCollapse = null;
                bool advanceToNextPriority = false;

                foreach (PreferredSizeData data in list)
                {
                    ICollapsibleElement collapsibleElement = data.Element as ICollapsibleElement;

                    if (collapsibleElement == null)
                        continue;

                    if ((collapsibleElement.CollapseStep == i) && (collapsibleElement.CollapseStep < collapsibleElement.CollapseMaxSteps))
                        lastElementToCollapse = data;

                    if (i < collapsibleElement.CollapseMaxSteps)
                        advanceToNextPriority = true;
                }

                if (lastElementToCollapse != null)
                    return lastElementToCollapse;

                if (!advanceToNextPriority)
                    break;
            }
            return null;
        }

        protected PreferredSizeData GetElementToExpand(List<PreferredSizeData> list, int sumWidth, int availableWidth)
        {
            PreferredSizeData elementToExpand = null;

            for (int i = 2; ; i++)
            {
                PreferredSizeData lastElementToExpand = null;
                bool advanceToNextPriority = false;
                int maxSumWidth = 0;

                foreach (PreferredSizeData data in list)
                {
                    ICollapsibleElement collapsibleElement = data.Element as ICollapsibleElement;

                    if (collapsibleElement == null)
                        continue;

                    if (i < collapsibleElement.CollapseMaxSteps)
                        advanceToNextPriority = true;

                    if (collapsibleElement.CollapseStep != i)
                        continue;

                    //if (data.Element.Size.Width == collapsibleElement.ExpandedSize.Width)
                    //	continue;

                    int newSumWidth = sumWidth - data.Element.Size.Width + collapsibleElement.ExpandedSize.Width;
                    if ((newSumWidth < availableWidth) && (newSumWidth > maxSumWidth))
                    {
                        lastElementToExpand = data;
                        maxSumWidth = newSumWidth;
                    }
                }

                if (lastElementToExpand != null)
                    elementToExpand = lastElementToExpand;

                if (!advanceToNextPriority)
                    break;
            }
            return elementToExpand;
        }

        protected bool CollapseElements(List<PreferredSizeData> list, Size availableSize)
        {
            if (!ShouldCollapseElements(list, availableSize))
                return false;

            bool collapsedSuccessfully = false;

            while (!collapsedSuccessfully)
            {
                PreferredSizeData elementToCollapse = GetElementToCollapse(list);

                if (elementToCollapse == null)
                    return false;

                collapsedSuccessfully = ((ICollapsibleElement)elementToCollapse.Element).CollapseElement(elementToCollapse.PreferredSize);
                elementToCollapse.PreferredSize = elementToCollapse.Element.GetPreferredSize(availableSize);
            }

            return collapsedSuccessfully;
        }

        protected bool ExpandElements(List<PreferredSizeData> list, Size availableSize)
        {
            if (!ShouldExpandElements(list, availableSize))
                return false;

            int sumWidth = GetSumWidth(list);

            PreferredSizeData elementToExpand = GetElementToExpand(list, sumWidth, availableSize.Width);

            if (elementToExpand == null)
                return false;

            bool expandedSuccessfully = ((ICollapsibleElement)elementToExpand.Element).ExpandElement();

            return expandedSuccessfully;
        }

        protected int GetSumWidth(List<PreferredSizeData> list)
        {
            int sumWidth = 0;

            foreach (PreferredSizeData data in list)
            {
                sumWidth += GetDataSize(data, false).Width + data.Element.Margin.Horizontal;
            }
            return sumWidth;
        }

        protected Size GetDataSize(PreferredSizeData data, bool getRealSize)
        {
            Size size = data.PreferredSize;

            if (getRealSize && (data.Element.AngleTransform != 0f || data.Element.ScaleTransform.Width != 1f || data.Element.ScaleTransform.Height != 1f))
                size = data.Element.GetBoundingRectangle(size).Size;

            return size;
        }

        private Size LayoutHorizontal(Size availableSize)
        {
            int nextTop = 0, nextLeft;
            int maxRowHeight = 0, row = 0;
            int rowElements = 0;
            int width = 0;
            int maxWidth = 0;
            Size maxSize = Size.Empty;

            List<PreferredSizeData> list = new List<PreferredSizeData>();
            FillList(list, availableSize);

            maxSize = GetMaxSize(list);

            nextLeft = !this.RightToLeft ? 0 : availableSize.Width;

            if (this.CollapseElementsOnResize && !this.IsDesignMode)
            {
                bool hasCollapsedElements = CollapseElements(list, availableSize);

                if (!hasCollapsedElements)
                    ExpandElements(list, availableSize);
            }

            foreach (PreferredSizeData data in list)
            {
                if (this.EqualChildrenWidth)
                {
                    if (this.ChildrenForcedSize != Size.Empty)
                        data.PreferredSize = new Size(this.ChildrenForcedSize.Width, data.PreferredSize.Height);
                    else
                        data.PreferredSize = new Size(maxSize.Width, data.PreferredSize.Height);
                }
                if (this.EqualChildrenHeight)
                {
                    if (this.ChildrenForcedSize != Size.Empty)
                        data.PreferredSize = new Size(data.PreferredSize.Width, this.ChildrenForcedSize.Height);
                    else
                        data.PreferredSize = new Size(data.PreferredSize.Width, maxSize.Height);
                }

                //if (this.AllElementsEqualSize)
                //{
                //    if (this.ChildrenForcedSize != Size.Empty)
                //        data.PreferredSize = new Size(this.ChildrenForcedSize.Width, this.ChildrenForcedSize.Height);
                //    else
                //    {
                //        if (this.FlipMaxSizeDimensions)
                //            data.PreferredSize = new Size(maxSize.Width, data.PreferredSize.Height);					
                //        else					
                //            data.PreferredSize = new Size(data.PreferredSize.Width, maxSize.Height);					
                //    }
                //}				

                int step = GetDataSize(data, true).Width + data.Element.Margin.Horizontal;

                if (!this.RightToLeft)
                {
                    if ((!this.IsInStripMode) && (nextLeft + step > availableSize.Width) &&
                        (rowElements > 0))
                    {
                        nextTop += maxRowHeight;
                        nextLeft = 0;
                        maxRowHeight = 0;
                        rowElements = 0;
                        row++;
                    }
                    data.Element.Bounds = new Rectangle(new Point(nextLeft, nextTop), GetDataSize(data, false));
                }
                else
                {
                    if ((!this.IsInStripMode) && (nextLeft - step < 0) &&
                        (rowElements > 0))
                    {
                        nextTop += maxRowHeight;
                        nextLeft = availableSize.Width;
                        maxRowHeight = 0;
                        rowElements = 0;
                        row++;
                        width = 0;
                    }
                    data.Element.Bounds = new Rectangle(new Point(nextLeft - step, nextTop), GetDataSize(data, false));
                }

                if (!this.RightToLeft)
                {
                    nextLeft += step;
                }
                else
                {
                    nextLeft -= step;
                }
                data.Element.SetValue(RowProperty, row);
                rowElements++;
                width += GetDataSize(data, true).Width;
                maxWidth = Math.Max(maxWidth, width);
                maxRowHeight = Math.Max(maxRowHeight, GetDataSize(data, true).Height + data.Element.Margin.Vertical);
            }

            if ((RightToLeft) && (this.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize))
            {
                int difference = availableSize.Width - maxWidth;
                foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
                {
                    child.Location = new Point(child.Location.X - difference, child.Location.Y);
                }
            }

            return new Size(maxWidth, Math.Max(maxRowHeight, nextTop));
        }

        private Size LayoutVertical(Size availableSize)
        {
            int row = 0;
            int nextTop = 0;
            Size maxSize = Size.Empty;

            List<PreferredSizeData> list = new List<PreferredSizeData>();
            FillList(list, availableSize);

            maxSize = GetMaxSize(list);

            foreach (PreferredSizeData data in list)
            {
                data.Element.SetValue(RowProperty, row);

                if (this.EqualChildrenWidth)
                {
                    if (this.ChildrenForcedSize != Size.Empty)
                        data.PreferredSize = new Size(this.ChildrenForcedSize.Width - data.Element.Margin.Horizontal, data.PreferredSize.Height);
                    else
                        data.PreferredSize = new Size(maxSize.Width, data.PreferredSize.Height);
                }
                if (this.EqualChildrenHeight)
                {
                    if (this.ChildrenForcedSize != Size.Empty)
                        data.PreferredSize = new Size(data.PreferredSize.Width, this.ChildrenForcedSize.Height - data.Element.Margin.Vertical);
                    else
                        data.PreferredSize = new Size(data.PreferredSize.Width, maxSize.Height);
                }

                //if (this.AllElementsEqualSize)
                //{
                //    if (this.FlipMaxSizeDimensions)
                //        data.PreferredSize = new Size(data.PreferredSize.Width, maxSize.Height);
                //    else
                //        data.PreferredSize = new Size(maxSize.Width, data.PreferredSize.Height);
                //}

                if (this.RightToLeft)
                {
                    data.Element.Location = new Point((this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize ? availableSize.Width : maxSize.Width) - data.PreferredSize.Width, nextTop);
                }
                else
                {
                    data.Element.Location = new Point(0, nextTop);
                }

                if (data.Element.AutoSize)
                {
                    //if (this.AllElementsEqualSize)
                    //    data.Element.CoercedSize = data.PreferredSize;
                    //else
                    //    data.Element.Size = data.PreferredSize;
                    if (this.EqualChildrenWidth || this.EqualChildrenHeight)
                        data.Element.CoercedSize = data.PreferredSize;
                    else
                        data.Element.Size = data.PreferredSize;
                }

                nextTop += data.Element.BoundingRectangle.Height + data.Element.Margin.Vertical;
                row++;
            }

            return new Size(maxSize.Width, nextTop);
        }

        protected void FillList(List<PreferredSizeData> list, Size availableSize)
        {
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                list.Add(new PreferredSizeData(child, availableSize));
            }
        }

        public Size GetMaxSize()
        {
            List<PreferredSizeData> list = new List<PreferredSizeData>();
            FillList(list, this.UseParentSizeAsAvailableSize ? this.Parent.Size : this.AvailableSize);
            return GetMaxSize(list);
        }

        protected Size GetMaxSize(List<PreferredSizeData> list)
        {
            Size maxSize = Size.Empty;
            foreach (PreferredSizeData data in list)
            {
                maxSize.Width = Math.Max(maxSize.Width, data.PreferredSize.Width + data.Element.Margin.Horizontal);
                maxSize.Height = Math.Max(maxSize.Height, data.PreferredSize.Height + data.Element.Margin.Vertical);
            }
            return maxSize;
        }

        /// <summary>
        /// Retrieves the preferred size of the layout panel. If the proposed size is 
        /// smaller than the minimal one, the minimal one is retrieved. Since all layout 
        /// panels update their layout automatically through events, this function is 
        /// rarely used directly.
        /// </summary>
        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            RadElement child = null;
            if (this.Children.Count > 0)
                child = this.Children[0];

            if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
            {
                return this.Parent.Size;
            }

            if (this.Orientation == Orientation.Horizontal)
                return GetHorizontalSize();
            else
                return GetVerticalSize();
        }

        private Size GetHorizontalSize()
        {
            int row = 0;
            int maxRowHeight = 0, maxWidth = 0;
            int width = 0, height = 0;

            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                int newRow = (int)child.GetValue(RowProperty);

                if (newRow != row)
                {
                    height += maxRowHeight;
                    maxWidth = Math.Max(maxWidth, width);

                    maxRowHeight = 0;
                    width = 0;
                    row = newRow;
                }

                Rectangle fullRect = child.FullBoundingRectangle;
                width += fullRect.Width;
                maxRowHeight = Math.Max(maxRowHeight, fullRect.Height);
            }
            if (this.LayoutableChildrenCount > 0)
            {
                height += maxRowHeight;
                maxWidth = Math.Max(maxWidth, width);
            }

            return new Size(maxWidth, height);
        }

        private Size GetVerticalSize()
        {
            int height = 0, maxWidth = 0;

            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                height += child.FullBoundingRectangle.Height;
                maxWidth = Math.Max(maxWidth, child.FullBoundingRectangle.Width);
            }

            return new Size(maxWidth, height);
        }

        /// <summary>
        /// ArrangeOverride
        /// </summary>
        /// <param name="arrangeSize"></param>
        /// <returns></returns>
        protected override SizeF ArrangeOverride(SizeF arrangeSize)
        {
            RadElementCollection children = this.Children;
            int count = children.Count;

            // Get desired children size if EqualChildrenHeight or EqualChildrenWidth is used
            // ********************************************************* //
            SizeF maxDesiredChildrenSize = SizeF.Empty;
            bool equalChildrenHeight = this.EqualChildrenHeight;
            bool equalChildrenWidth = this.EqualChildrenWidth;
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
                element.Arrange(finalRect);
            }

            return arrangeSize;
        }

        protected override SizeF MeasureOverride(SizeF constraint)
        {
            RadElementCollection internalChildren = this.Children;
            int count = internalChildren.Count;
            bool isHorizontal = this.Orientation == Orientation.Horizontal;

            // Get the available size that will be given to children's Measure()
            //
            SizeF availableSize = constraint;
            if (isHorizontal)
            {
                availableSize.Width = float.PositiveInfinity;
            }
            else
            {
                availableSize.Height = float.PositiveInfinity;
            }

            // Call Measure() for each child
            //
            for (int i = 0; i < count; i++)
            {
                RadElement element = internalChildren[i];
                element.Measure(availableSize);
            }

            // Get desired children size if EqualChildrenHeight or EqualChildrenWidth is used
            //
            SizeF maxDesiredChildrenSize = SizeF.Empty;
            bool equalChildrenHeight = this.EqualChildrenHeight;
            bool equalChildrenWidth = this.EqualChildrenWidth;
            if (equalChildrenHeight || equalChildrenWidth)
            {
                for (int i = 0; i < count; i++)
                {
                    RadElement element = internalChildren[i];
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

            // Calculate the own desired size based on children's DesiredSize
            //
            SizeF res = SizeF.Empty;
            for (int i = 0; i < count; i++)
            {
                RadElement element = internalChildren[i];
                SizeF desiredSize = element.DesiredSize;
                if (equalChildrenHeight)
                {
                    desiredSize.Height = maxDesiredChildrenSize.Height;
                }
                if (equalChildrenWidth)
                {
                    desiredSize.Width = maxDesiredChildrenSize.Width;
                }

                if (isHorizontal)
                {
                    res.Width += desiredSize.Width;
                    res.Height = Math.Max(res.Height, desiredSize.Height);
                }
                else
                {
                    res.Width = Math.Max(res.Width, desiredSize.Width);
                    res.Height += desiredSize.Height;
                }
            }
            return res;
        }


        ////////////////////////////////////////////////

        /*
		protected override SizeF ArrangeOverride(SizeF finalSize)
		{
			float width = 0;
			float height = 0;
			foreach (RadElement child in this.Children)
			{
				if (this.Orientation == Orientation.Horizontal)
				{
					if (this.RightToLeft)
					{
						width += child.DesiredSize.Width;
						child.Arrange(new RectangleF(finalSize.Width - width, 0, child.DesiredSize.Width, child.DesiredSize.Height));
					}
					else
					{
						child.Arrange(new RectangleF(width, 0, child.DesiredSize.Width, child.DesiredSize.Height));
						width += child.DesiredSize.Width;
					}

				}
				else
				{
					child.Arrange(new RectangleF(0, height, child.DesiredSize.Width, child.DesiredSize.Height));
					height += child.DesiredSize.Height;
				}
			}
			return finalSize;
		}

		protected override SizeF MeasureOverride(SizeF availableSize)
		{
			float width = 0;
			float height = 0;
			foreach (RadElement child in this.Children)
			{
				child.Measure(availableSize);
				if (this.Orientation == Orientation.Horizontal)
				{
					width += child.DesiredSize.Width;
					height = Math.Max(height, child.DesiredSize.Height);
				}
				else
				{
					width = Math.Max(width, child.DesiredSize.Width);
					height += child.DesiredSize.Height;
				}
			}
			return new SizeF(width, height);
		}*/
    }

    
}