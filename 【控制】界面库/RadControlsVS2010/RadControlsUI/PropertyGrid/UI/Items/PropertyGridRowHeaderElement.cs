using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridRowHeaderElement : PropertyGridContentElement
    {
        public static RadProperty IsRootItemWithChildrenProperty = RadProperty.Register(
            "IsRootItemWithChildren", typeof(bool), typeof(PropertyGridRowHeaderElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsInEditModeProperty = RadProperty.Register(
            "IsInEditMode", typeof(bool), typeof(PropertyGridRowHeaderElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        static PropertyGridRowHeaderElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PropertyGridHeaderElementStateManager(), typeof(PropertyGridRowHeaderElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
            this.DrawBorder = false;
            this.DrawFill = false;
            this.NotifyParentOnMouseInput = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.ElementTree.Control.Cursor = Cursors.Default;
        }

        public virtual void Synchronize()
        {
            PropertyGridItem item = VisualItem.Data as PropertyGridItem;
            if (item != null)
            {
                if (item.Level == 0)
                {
                    this.Visibility = ElementVisibility.Collapsed;
                }
                else
                {
                    this.Visibility = ElementVisibility.Visible;
                }
            }
            PropertyGridItemElement visualItem = VisualItem as PropertyGridItemElement;
            if (visualItem != null)
            {
                this.SetValue(IsRootItemWithChildrenProperty, visualItem.Data.GridItems.Count > 0 && visualItem.Data.Level == 0);
                this.SetValue(IsInEditModeProperty, visualItem.GetValue(PropertyGridItemElement.IsInEditModeProperty));
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = Size.Empty;
            PropertyGridItemElement visualItem = this.VisualItem as PropertyGridItemElement;
            PropertyGridTableElement propertyGridTableElement = this.PropertyGridTableElement;
            if (propertyGridTableElement != null && visualItem != null)
            {
                PropertyGridItem dataItem = visualItem.Data as PropertyGridItem;
                if (dataItem != null)
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
