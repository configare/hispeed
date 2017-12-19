using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class PropertyGridIndentElement : PropertyGridContentElement
    {
        #region Initialization

        static PropertyGridIndentElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PropertyGridItemElementStateManager(), typeof(PropertyGridIndentElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.Visibility = ElementVisibility.Visible;
            this.NotifyParentOnMouseInput = true;
            this.ClipDrawing = true;
            this.StretchVertically = true;
            this.StretchHorizontally = false;
            this.DrawBorder = false;
            this.DrawFill = false;
        }

        #endregion

        #region Methods

        public virtual void Synchronize()
        {
            PropertyGridItemElement visualItem = VisualItem as PropertyGridItemElement;
            if (visualItem != null)
            {
                PropertyGridItem dataItem = visualItem.Data as PropertyGridItem;
                bool indented = dataItem.Level > 0;
                if (indented)
                {
                    this.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.Visibility = ElementVisibility.Hidden;
                }
            }
        }

        #endregion

        #region Layout

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
                    desiredSize.Width = propertyGridTableElement.ItemIndent * dataItem.Level;
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

        #endregion           
    }
}
