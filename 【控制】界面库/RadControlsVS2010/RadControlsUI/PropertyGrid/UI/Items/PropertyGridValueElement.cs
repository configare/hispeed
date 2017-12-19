using System;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class PropertyGridValueElement : PropertyGridContentElement
    {
        public static RadProperty ContainsErrorProperty = RadProperty.Register(
            "ContainsError", typeof(bool), typeof(PropertyGridValueElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsModifiedProperty = RadProperty.Register(
            "IsModified", typeof(bool), typeof(PropertyGridValueElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        static PropertyGridValueElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PropertyGridValueElementStateManager(), typeof(PropertyGridValueElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = false;
            this.NotifyParentOnMouseInput = true;
            this.ClipDrawing = true;
            this.StretchHorizontally = false;
            this.StretchVertically = true;
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.DrawBorder = false;
            this.DrawFill = false;
        }

        public virtual void Synchronize()
        {
            object value = ((PropertyGridItem)VisualItem.Data).FormattedValue;
            this.Text = value != null ? value.ToString() : "";
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {            
            float desiredWidth = this.PropertyGridTableElement.ValueColumnWidth;
            availableSize.Width = Math.Min(availableSize.Width, desiredWidth);
            SizeF desiredSize = base.MeasureOverride(availableSize);
            desiredSize.Width = Math.Min(desiredWidth, availableSize.Width);
            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
            PropertyGridItemElement itemElement = this.VisualItem as PropertyGridItemElement;
            BaseInputEditor editor = null;
            if (itemElement != null && itemElement.Editor != null)
            {
                editor = itemElement.Editor as BaseInputEditor;     
            }
            this.layoutManagerPart.Arrange(clientRect);

            foreach (RadElement element in this.Children)
            {
                if (editor != null && editor.EditorElement == element)
                {
                    float editorHeight = element.DesiredSize.Height;
                    if (element.StretchVertically)
                    {
                        editorHeight = clientRect.Height;
                    }
                    editorHeight = Math.Min(editorHeight, clientRect.Height);
                    RectangleF editorRect = new RectangleF(clientRect.X, 
                        clientRect.Y + (clientRect.Height - editorHeight) / 2, clientRect.Width, editorHeight);
                    element.Arrange(editorRect);
                    continue;
                }
                element.Arrange(new RectangleF(clientRect.X, clientRect.Y, clientRect.Width, clientRect.Height));
            }

            return finalSize;
        }    
    }
}
