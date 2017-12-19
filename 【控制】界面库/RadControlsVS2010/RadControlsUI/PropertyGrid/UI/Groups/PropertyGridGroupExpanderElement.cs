
using System.Drawing;
namespace Telerik.WinControls.UI
{
    public class PropertyGridGroupExpanderElement : PropertyGridExpanderElement
    {
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.NotifyParentOnMouseInput = true;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = Size.Empty;
            PropertyGridGroupElement visualItem = this.VisualItem as PropertyGridGroupElement;
            PropertyGridTableElement propertyGridTableElement = this.PropertyGridTableElement;
            if (propertyGridTableElement != null && visualItem != null)
            {
                PropertyGridGroupItem groupDataItem = visualItem.Data as PropertyGridGroupItem;
                if (groupDataItem != null)
                {
                    desiredSize.Width = propertyGridTableElement.ItemIndent;
                    if (float.IsPositiveInfinity(availableSize.Height))
                    {
                        desiredSize.Height = propertyGridTableElement.ItemHeight;
                    }
                    else
                    {
                        desiredSize.Height = availableSize.Height;
                    }
                }
            }

            return desiredSize;
        }
    }
}
