using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewVirtualizedContainer : VirtualizedStackContainer<RadTreeNode>
    {       
        public RadTreeViewElement TreeViewElement
        {
            get
            {
                return FindAncestor<RadTreeViewElement>();
            }
        }

        protected override SizeF MeasureElementCore(RadElement element, SizeF availableSize)
        {
            TreeNodeElement virtualizedElement = (TreeNodeElement)element;
            float fixedHeight = ElementProvider.GetElementSize(virtualizedElement.Data).Height;
            float measureWidth = float.PositiveInfinity;

            if (TreeViewElement.AutoSizeItems || virtualizedElement.Editor != null)
            {
                fixedHeight = float.PositiveInfinity;
            }

            if (virtualizedElement.ContentElement.TextWrap)
            {
                measureWidth = availableSize.Width - this.ScrollOffset.Width;
            }
            element.Measure(new SizeF(measureWidth, fixedHeight));
            virtualizedElement.ContentElement.FullDesiredSize = virtualizedElement.ContentElement.DesiredSize;

            element.Measure(new SizeF(availableSize.Width - this.ScrollOffset.Width, fixedHeight));
            
            return element.DesiredSize;
        }

        protected override RectangleF ArrangeElementCore(RadElement element, SizeF finalSize, RectangleF arrangeRect)
        {
            arrangeRect.X = 0;
            arrangeRect.Width = finalSize.Width;
            return base.ArrangeElementCore(element, finalSize, arrangeRect);
        }
    }
}
