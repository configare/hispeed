
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class VirtualizedStackContainer<T> : BaseVirtualizedContainer<T>
    {
        #region Fields

        Orientation orientation = Orientation.Vertical;
        SizeF scrollOffset;
        SizeF desiredSize;
        SizeF availableSize;
        SizeF originalAvailableSize;
        int itemSpacing;
        bool fitElementsToSize = true;
        float offset;

        #endregion

        #region Properties

        public int ItemSpacing
        {
            get
            {
                return itemSpacing;
            }
            set
            {
                if (itemSpacing != value)
                {
                    itemSpacing = value;
                    InvalidateMeasure();
                }
            }
        }

        public Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                if (this.orientation != value)
                {
                    this.orientation = value;
                    InvalidateMeasure();
                }
            }
        }

        public bool FitElementsToSize
        {
            get
            {
                return this.fitElementsToSize;
            }
            set
            {
                if (this.fitElementsToSize != value)
                {
                    this.fitElementsToSize = value;
                    InvalidateArrange();
                }
            }
        }

        public SizeF ScrollOffset
        {
            get
            {
                return this.scrollOffset;
            }
            set
            {
                if (this.scrollOffset != value)
                {
                    this.scrollOffset = value;
                    InvalidateMeasure();
                }
            }
        }

        #endregion

        #region Layout

        protected override bool BeginMeasure(SizeF availableSize)
        {
            if (!base.BeginMeasure(availableSize))
            {
                return false;
            }

            this.originalAvailableSize = availableSize;
            this.desiredSize = SizeF.Empty;
            this.availableSize = availableSize;
            
            if (this.availableSize.Width == 0 || this.availableSize.Height == 0)
            {
                while (this.Children.Count > 0)
                {
                    RemoveElement(0);
                }
                return false;
            }

            InitializeOffset();

            return true;
        }

        protected override bool MeasureElement(IVirtualizedElement<T> element)
        {
            RadElement radElement = element as RadElement;
            if (radElement == null)
            {
                return false;
            }

            SizeF elementDesiredSize = MeasureElementCore(radElement, availableSize);
            bool continueMeasure = true;

            if (this.orientation == Orientation.Vertical)
            {
                continueMeasure = availableSize.Height > elementDesiredSize.Height;
                float height = elementDesiredSize.Height + ItemSpacing;
                availableSize.Height -= height;
                desiredSize.Height += height;
                desiredSize.Width = Math.Max(desiredSize.Width, elementDesiredSize.Width);
                return continueMeasure;
            }

            continueMeasure = availableSize.Width > elementDesiredSize.Width;
            float width = elementDesiredSize.Width + ItemSpacing;
            
            availableSize.Width -= width;
            desiredSize.Width += width;
            desiredSize.Height = Math.Max(desiredSize.Height, elementDesiredSize.Height);
            return continueMeasure;
        }

        protected virtual SizeF MeasureElementCore(RadElement element, SizeF availableSize)
        {      
            IVirtualizedElement<T> virtualizedElement = (IVirtualizedElement<T>)element;
            SizeF fixedSize = ElementProvider.GetElementSize(virtualizedElement.Data);
            if (Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                if (fixedSize.Height > 0)
                {
                    availableSize.Height = fixedSize.Height;
                }
                if (!FitElementsToSize)
                {
                    availableSize.Width = float.PositiveInfinity;
                }
            }
            else
            {
                availableSize.Width = fixedSize.Width;
                if (!FitElementsToSize)
                {
                    availableSize.Height = float.PositiveInfinity;
                }
            }

            element.Measure(availableSize);
                        
            return element.DesiredSize;
        }

        protected override SizeF EndMeasure()
        {
            if (this.Children.Count > 0)
            {
                if (Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    this.desiredSize.Width -= ItemSpacing;
                }
                else
                {
                    this.desiredSize.Height -= ItemSpacing;
                }
            }

            desiredSize.Width = Math.Min(originalAvailableSize.Width, desiredSize.Width);
            desiredSize.Height = Math.Min(originalAvailableSize.Height, desiredSize.Height);

            return this.desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            InitializeOffset();

            foreach (RadElement element in this.Children)
            {
                if (this.orientation == Orientation.Vertical)
                {
                    IVirtualizedElement<T> virtualizedElement = (IVirtualizedElement<T>)element;
                    float width = FitElementsToSize /*|| element.StretchHorizontally*/ ? finalSize.Width : element.DesiredSize.Width;
                    float height = ElementProvider.GetElementSize(virtualizedElement.Data).Height;
                    RectangleF arrangeRect = new RectangleF(ScrollOffset.Width, offset, width, height);
                    if (RightToLeft)
                    {
                        arrangeRect.X = finalSize.Width - width;
                    }
                    arrangeRect = ArrangeElementCore(element, finalSize, arrangeRect);
                    offset += arrangeRect.Height + ItemSpacing;
                }
                else
                {
                    float height = FitElementsToSize || element.StretchVertically ? finalSize.Height : element.DesiredSize.Height;
                    RectangleF arrangeRect = ArrangeElementCore(element, finalSize, new RectangleF(offset, 0, element.DesiredSize.Width, height));
                    offset += arrangeRect.Width + ItemSpacing;
                }                
            }            

            return finalSize;
        }

        protected virtual RectangleF ArrangeElementCore(RadElement element, SizeF finalSize, RectangleF arrangeRect)
        {
            element.Arrange(arrangeRect);
            return arrangeRect;
        }

        private void InitializeOffset()
        {
            if (this.orientation == Orientation.Vertical)
            {
                this.offset = scrollOffset.Height;
                this.availableSize.Height -= this.offset;
            }
            else
            {
                if (RightToLeft)
                {
                    this.offset = availableSize.Width - scrollOffset.Width;
                }
                else
                {
                    this.offset = scrollOffset.Width;
                }
                this.availableSize.Width -= this.offset;
            }
        }

        #endregion
    }
}
