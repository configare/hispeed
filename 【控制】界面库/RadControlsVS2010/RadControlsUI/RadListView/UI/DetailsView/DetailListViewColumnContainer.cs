using System;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class DetailListViewColumnContainer : VirtualizedStackContainer<ListViewDetailColumn>
    {
        #region Fields

        SizeF availableSize;
        bool scrollColumns = true;
        SizeF desiredSize;
        DetailListViewElement context;

        #endregion

        #region Initialization

        public DetailListViewColumnContainer()
        {

        }

        public DetailListViewColumnContainer(DetailListViewElement context)
        {
            this.context = context;
        }

        public DetailListViewElement Context
        {
            get
            {
                return context;
            }
            set
            {
                context = value;
            }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.NotifyParentOnMouseInput = true;
            this.ItemSpacing = -1;
            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //this.FitElementsToSize = false;          
        }

        #endregion

        #region Properties
        
        protected bool ScrollColumns
        {
            get { return this.scrollColumns; }
            set { this.scrollColumns = value; }
        }

        #endregion

        #region Layout

        protected override bool BeginMeasure(SizeF availableSize)
        {
            this.availableSize = availableSize;
            this.desiredSize = SizeF.Empty;
            return base.BeginMeasure(availableSize);
        }

        protected override SizeF EndMeasure()
        {
            if (this.Children.Count > 0)
            {
                if (Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    this.desiredSize.Width -= ItemSpacing;
                    this.desiredSize.Width = Math.Min(this.desiredSize.Width, this.availableSize.Width);
                }
                else
                {
                    this.desiredSize.Height -= ItemSpacing;
                    this.desiredSize.Height = Math.Min(this.desiredSize.Height, this.availableSize.Height);
                }
            }
             
            return this.desiredSize;
        }

        protected override bool MeasureElement(IVirtualizedElement<ListViewDetailColumn> element)
        {
            DetailListViewCellElement cell = element as DetailListViewCellElement;
            if (cell == null)
            {
                return false;
            }
             
            SizeF desiredCellSize = MeasureElementCore(cell, availableSize);
            desiredSize.Height = Math.Max(desiredSize.Height, desiredCellSize.Height);
            desiredSize.Width += desiredCellSize.Width;

            if (ScrollColumns)
            {
                return desiredSize.Width - this.context.ColumnScrollBar.Value < availableSize.Width;
            }

            return true;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = new RectangleF(PointF.Empty, finalSize);
            
            foreach (RadElement element in this.Children)
            {
                RectangleF arrangeRect = new RectangleF(clientRect.X, clientRect.Y, element.DesiredSize.Width, finalSize.Height);

                if (ScrollColumns)
                {
                    if (RightToLeft)
                    {
                        arrangeRect.X += this.context.ColumnScroller.ScrollOffset;
                    }
                    else
                    {
                        arrangeRect.X -= this.context.ColumnScroller.ScrollOffset;
                    }
                }
                ArrangeElementCore(element, finalSize, arrangeRect);
                clientRect.X += element.DesiredSize.Width + this.ItemSpacing;
            }

            return finalSize;
        }

        protected override bool IsItemVisible(ListViewDetailColumn data)
        {
            return data.Visible;
        }

        #endregion
    }
}
