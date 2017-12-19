using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class ListElementProvider : VirtualizedPanelElementProvider<RadListDataItem, RadListVisualItem>
    {
        private RadListElement listElement;

        public ListElementProvider(RadListElement listElement)
        {
            if (listElement == null)
            {
                throw new ArgumentException("Owner element can not be null.");
            }

            this.listElement = listElement;
        }

        public override IVirtualizedElement<RadListDataItem> CreateElement(RadListDataItem data, object context)
        {
            if (data is RadListDataGroupItem)
            {
                RadListVisualGroupItem result = (this.listElement.OnCreatingVisualListItem() as RadListVisualGroupItem);

                if (result == null)
                {
                    result = new RadListVisualGroupItem();
                }

                return result;
            }
            else
            {
                RadListVisualItem result = this.listElement.OnCreatingVisualListItem();

                if (result == null)
                {
                    result = new RadListVisualItem();
                }

                return result;
            }
        }        

        public override SizeF GetElementSize(RadListDataItem item)
        {
            if (item.Owner == null)
            {
                return SizeF.Empty;
            }

            if (listElement.AutoSizeItems)
            {
                if (item.MeasuredSize.Width<2 || item.MeasuredSize.Height<2)
                {
                    SizeF availableSize = new SizeF(float.PositiveInfinity, float.PositiveInfinity);
                    if (this.listElement.FitItemsToSize)
                    {
                        availableSize = this.listElement.ViewElement.Size;
                    }
                    item.MeasuredSize = listElement.MeasureItem(item, availableSize);
                }
                return item.MeasuredSize;
            }

            if (item.Height != -1)
            {
                return new SizeF(0, item.Height);
            }

            return new SizeF(0, listElement.ItemHeight);
        }
    }
}
