using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class DefaultListControlStackContainer : VirtualizedStackContainer<RadListDataItem>
    {
        public void ForceVisualStateUpdate()
        {
            foreach (RadListVisualItem element in this.Children)
            {
                element.Synchronize();
            }            
        }

        public void ForceVisualStateUpdate(RadListDataItem item)
        {
            foreach (RadListVisualItem element in this.Children)
            {
                if (element.Data == item)
                {
                    element.Synchronize();
                    break;
                }
            }
        }

        protected override RectangleF ArrangeElementCore(RadElement element, SizeF finalSize, RectangleF arrangeRect)
        {
            RadListElement listElement = this.Parent as RadListElement;
            RadListVisualItem item = element as RadListVisualItem;
            if (listElement != null && listElement.ShowGroups && item!=null && !(item is RadListVisualGroupItem) 
                && item.Data.Group != item.Data.Owner.groupFactory.DefaultGroup && item.Data.Group.Collapsible)
            {
                float offset = listElement.CollapsibleGroupItemsOffset;
                arrangeRect = new RectangleF(arrangeRect.Left + offset, arrangeRect.Top, arrangeRect.Width - offset, arrangeRect.Height);
            }
                
            element.Arrange(arrangeRect);
            return arrangeRect;
        }
    }
}
