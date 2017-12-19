using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class DetailsListViewContainer : BaseListViewContainer
    {
        public DetailsListViewContainer(BaseListViewElement owner)
            : base(owner)
	    {

	    }

        private float columnWidthSum = 0;
         
        protected override bool BeginMeasure(SizeF availableSize)
        {
            columnWidthSum = 0;
            foreach (ListViewDetailColumn column in this.owner.Owner.Columns)
            {
                columnWidthSum += column.Width;
            }


            return base.BeginMeasure(availableSize);
        }

        protected override SizeF MeasureElementCore(RadElement element, SizeF availableSize)
        {
            if (element is BaseListViewGroupVisualItem)
            {
                element.Measure(new SizeF(columnWidthSum, availableSize.Height));

                return element.DesiredSize;
            }

            return base.MeasureElementCore(element, availableSize);
        }

        protected override RectangleF ArrangeElementCore(RadElement element, SizeF finalSize, RectangleF arrangeRect)
        {
            if (element is BaseListViewGroupVisualItem)
            {
                element.PositionOffset = new SizeF(-(this.owner as DetailListViewElement).ColumnScrollBar.Value, 0);
            } 

            return base.ArrangeElementCore(element, finalSize, arrangeRect);
        }
    }
}
