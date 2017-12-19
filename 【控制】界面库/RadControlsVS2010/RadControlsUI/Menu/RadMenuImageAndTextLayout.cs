using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class MenuImageAndTextLayout : ImageAndTextLayoutPanel
    {
        private RadElement imageElement;
        private RadElement textElement;

        private RadDropDownMenuLayout MenuLayout
        {
            get
            {
                RadMenuItemLayout menuItemLayout = this.Parent as RadMenuItemLayout;
                if (menuItemLayout != null)
                {
                    RadMenuItem menuItem = menuItemLayout.Parent as RadMenuItem;
                    if (menuItem != null)
                    {
                        return menuItem.MenuLayout;
                    }
                }
                return null;
            }
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);

            if (MenuLayout != null)
            {
                EnsureImageAndTextElements();
                if (LayoutUtils.IsHorizontalRelation(this.TextImageRelation) && this.TextImageRelation == System.Windows.Forms.TextImageRelation.ImageBeforeText)
                {
                    if (this.imageElement != null)
                    {
                        RectangleF rect = new RectangleF(0f, 0f, MenuLayout.LeftColumnWidth, finalSize.Height);
                        if (this.RightToLeft)
                        {
                            rect = LayoutUtils.RTLTranslateNonRelative(rect, new RectangleF(PointF.Empty, finalSize));
                        }

                        this.imageElement.Arrange(rect);
                    }
                    if (this.textElement != null)
                    {
                        RectangleF rect = new RectangleF(MenuLayout.LeftColumnWidth, 0f, this.textElement.DesiredSize.Width, finalSize.Height);
                        if (this.RightToLeft)
                        {
                            rect = LayoutUtils.RTLTranslateNonRelative(rect, new RectangleF(PointF.Empty, finalSize));
                        }

                        this.textElement.Arrange(rect);
                    }
                }
            }
            return finalSize;
        }

        private void EnsureImageAndTextElements()
        {
            if (this.imageElement == null || this.textElement == null)
            {
                foreach (RadElement child in this.Children)
                {
                    if (IsImageElement(child))
                    {
                        this.imageElement = child;
                    }
                    else if (IsTextElement(child))
                    {
                        this.textElement = child;
                    }
                }
            }
        }

        private bool IsImageElement(RadElement element)
        {
            if (element == null)
            {
                return false;
            }
            return (bool)element.GetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty);
        }

        private bool IsTextElement(RadElement element)
        {
            if (element == null)
            {
                return false;
            }
            return (bool)element.GetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty);
        }
    }
}
