using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class SimpleListViewContainer : BaseListViewContainer
    {
        private int maxDesiredWidth = 0;

        public SimpleListViewContainer(BaseListViewElement owner)
            : base(owner)
        {
        }

        protected override RectangleF ArrangeElementCore(RadElement element, SizeF finalSize, RectangleF arrangeRect)
        {
            if (element is BaseListViewGroupVisualItem)
            {
                return base.ArrangeElementCore(element, finalSize, arrangeRect);
            }

            if (this.owner.FullRowSelect)
            {
                arrangeRect.Width = ((IVirtualizedElement<ListViewDataItem>)element).Data.ActualSize.Width;
            }

            if (this.owner.Owner.ShowGroups &&
                (this.owner.Owner.EnableCustomGrouping || this.owner.Owner.EnableGrouping) &&
                (this.owner.Owner.Groups.Count > 0) &&
                !this.owner.Owner.FullRowSelect)
            {
                arrangeRect.X += this.owner.Owner.GroupIndent;
                arrangeRect.Width -= this.owner.Owner.GroupIndent;
            }

            element.Arrange(arrangeRect);
            return arrangeRect;
        }

        protected override bool BeginMeasure(SizeF availableSize)
        {
            maxDesiredWidth = 0;
            return base.BeginMeasure(availableSize);
        }

        protected override bool MeasureElement(IVirtualizedElement<ListViewDataItem> element)
        {
            bool result = base.MeasureElement(element);

            maxDesiredWidth = Math.Max(maxDesiredWidth, element.Data.ActualSize.Width);

            return result;
        }

        protected override SizeF EndMeasure()
        {
            SizeF result = base.EndMeasure();
            if (this.owner.FullRowSelect)
            {
                foreach (ListViewDataItem item in this.DataProvider)
                {
                    item.ActualSize = new Size(this.maxDesiredWidth, item.ActualSize.Height);
                }
            }

            return result;
        }

    }
}
