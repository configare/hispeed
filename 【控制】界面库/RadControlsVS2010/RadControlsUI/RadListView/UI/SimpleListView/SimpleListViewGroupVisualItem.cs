using System;
using System.Drawing;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public class SimpleListViewGroupVisualItem : BaseListViewGroupVisualItem
    {
        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Data == null)
            {
                return SizeF.Empty;
            }

            SizeF desiredSize = base.MeasureOverride(LayoutUtils.InfinitySize);

            desiredSize.Width += this.toggleElement.DesiredSize.Width;

            if (this.Data.Size.Height > 0)
            {
                desiredSize.Height = this.Data.Size.Height;
            }

            if (this.Data.Size.Width > 0)
            {
                desiredSize.Width = this.Data.Size.Width;
            }

            RadListViewElement listViewElement = this.Data.Owner;

            if (listViewElement != null && !listViewElement.AllowArbitraryItemHeight)
            {
                desiredSize.Height = listViewElement.GroupItemSize.Height;
            }

            if (listViewElement != null && !listViewElement.AllowArbitraryItemWidth && !this.Data.Owner.FullRowSelect)
            {
                desiredSize.Width = listViewElement.GroupItemSize.Width;
            }

            if (listViewElement != null && listViewElement.FullRowSelect)
            {
                desiredSize.Width = Math.Max(GetClientRectangle(availableSize).Width, desiredSize.Width);
            }

            this.Data.ActualSize = desiredSize.ToSize();

            SizeF clientSize = GetClientRectangle(desiredSize).Size;

            this.toggleElement.Measure(availableSize);

            SizeF sizef = new SizeF(clientSize.Width - this.toggleElement.DesiredSize.Width, clientSize.Height);

            this.layoutManagerPart.Measure(sizef);

            return desiredSize;
        }
    }
}
