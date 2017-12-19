using System.Drawing;
namespace Telerik.WinControls.UI
{
    public class IconListViewGroupVisualItem : BaseListViewGroupVisualItem
    {
        protected override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF availableSize)
        {
            if (this.Data == null)
            {
                return SizeF.Empty;
            }

            SizeF desiredSize = base.MeasureOverride(availableSize);

            desiredSize.Width = GetClientRectangle(availableSize).Width;

            if (this.Data.Size.Height > 0)
            {
                desiredSize.Height = this.Data.Size.Height;
            }
             
            RadListViewElement listViewElement = this.Data.Owner;

            if (listViewElement != null && !listViewElement.AllowArbitraryItemHeight)
            {
                desiredSize.Height = listViewElement.GroupItemSize.Height;
            }
             
            this.Data.ActualSize = desiredSize.ToSize();

            SizeF clientSize = GetClientRectangle(desiredSize).Size;

            SizeF sizef = new SizeF(clientSize.Width - this.toggleElement.DesiredSize.Width, clientSize.Height);

            this.layoutManagerPart.Measure(sizef);

            return desiredSize;
        }
    }
}
