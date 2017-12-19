using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadMenuItemLayout: LayoutPanel
    {
        private ImagePrimitive imagePrimitive;
        private ArrowPrimitive arrowPrimitive;
        private RadMenuCheckmark checkmark;
        private StackLayoutPanel textPanel;
        private TextPrimitive textPrimitive;
        private TextPrimitive descriptionTextPrimitive;
        private TextPrimitive shortcutTextPrimitive;
        private LinePrimitive textSeparator;
        private ImageAndTextLayoutPanel internalLayoutPanel;

        private float maxHeight;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

        #region Properties 

        public ImagePrimitive ImagePrimitive
        {
            get 
            {
                return this.imagePrimitive; 
            }
        }

        public ArrowPrimitive ArrowPrimitive
        {
            get
            {
                return this.arrowPrimitive;
            }
        }

        public RadMenuCheckmark Checkmark
        {
            get
            {
                return this.checkmark;
            }
        }

        public StackLayoutPanel TextPanel
        {
            get
            {
                return this.textPanel;
            }
        }

        public ImageAndTextLayoutPanel InternalLayoutPanel
        {
            get
            {
                return this.internalLayoutPanel;
            }
        }

        public TextPrimitive Text
        {
            get
            {
                return this.textPrimitive;
            }
        }

        public TextPrimitive Description
        {
            get
            {
                return this.descriptionTextPrimitive;
            }
        }

        public TextPrimitive Shortcut
        {
            get
            {
                return this.shortcutTextPrimitive;
            }
        }

        public LinePrimitive TextSeparator
        {
            get
            {
                return this.textSeparator;
            }
        }

        #endregion

        protected override void CreateChildElements()
        {
            // text image relation 
            this.internalLayoutPanel = new MenuImageAndTextLayout();
            this.internalLayoutPanel.Class = "RadMenuItemInternalLayoutPanel";
            this.Children.Add(this.internalLayoutPanel);

            // checkbox
            this.checkmark = new RadMenuCheckmark();
            this.checkmark.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            this.checkmark.Alignment = ContentAlignment.MiddleCenter;
            this.checkmark.CheckElement.Alignment = ContentAlignment.MiddleCenter;
            this.checkmark.CheckElement.Class = "RadMenuItemCheckPrimitive";
            this.internalLayoutPanel.Children.Add(this.checkmark);

            // image
            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.SetValue(RadCheckmark.IsImageProperty, true);
            this.imagePrimitive.Class = "RadMenuItemImagePrimitive";
            this.imagePrimitive.Alignment = ContentAlignment.MiddleCenter;
            this.imagePrimitive.ZIndex = this.checkmark.ZIndex + 1;
            this.checkmark.Children.Add(this.imagePrimitive);

            // menu text + separator + description in a vertical stack panel
            textPanel = new StackLayoutPanel();
            textPanel.StretchHorizontally = false;
            textPanel.StretchVertically = false;
            textPanel.Class = "RadMenuItemTextPanel";
            textPanel.Orientation = Orientation.Vertical;
            textPanel.EqualChildrenWidth = true;
            textPanel.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            this.internalLayoutPanel.Children.Add(textPanel);

            // text
            this.textPrimitive = new TextPrimitive();
            this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            this.textPrimitive.Class = "RadMenuItemTextPrimitive";
            textPanel.Children.Add(textPrimitive);

            // separator
            textSeparator = new LinePrimitive();
            textSeparator.Class = "RadMenuItemTextSeparator";
            textSeparator.Visibility = ElementVisibility.Collapsed;
            textPanel.Children.Add(textSeparator);

            // description
            this.descriptionTextPrimitive = new TextPrimitive();
            this.descriptionTextPrimitive.Class = "RadMenuItemDescriptionText";
            textPanel.Children.Add(this.descriptionTextPrimitive);

            // shortcuttext            
            this.shortcutTextPrimitive = new TextPrimitive();
            this.shortcutTextPrimitive.Class = "RadMenuItemShortcutPrimitive";
            this.shortcutTextPrimitive.Visibility = ElementVisibility.Collapsed;
            this.Children.Add(this.shortcutTextPrimitive);

            // arrow
            this.arrowPrimitive = new ArrowPrimitive();
            this.arrowPrimitive.Visibility = ElementVisibility.Hidden;
            this.arrowPrimitive.Direction = (this.RightToLeft) ? ArrowDirection.Left : ArrowDirection.Right;
            this.arrowPrimitive.Alignment = ContentAlignment.MiddleLeft;
            this.arrowPrimitive.Class = "RadMenuItemArrowPrimitive";
            this.arrowPrimitive.SmoothingMode = SmoothingMode.Default;
            this.arrowPrimitive.MinSize = Size.Empty;
            this.arrowPrimitive.MaxSize = Size.Empty;
            this.Children.Add(this.arrowPrimitive);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadElement.RightToLeftProperty)
            {
                this.arrowPrimitive.Direction = (this.RightToLeft) ? ArrowDirection.Left : ArrowDirection.Right;

            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF availableSizeForChildren = new SizeF(float.PositiveInfinity, availableSize.Height);
            SizeF resultSize = SizeF.Empty;
            maxHeight = 0;
            foreach (RadElement element in this.Children)
            {
                element.Measure(availableSizeForChildren);
                maxHeight = Math.Max(element.DesiredSize.Height, maxHeight);                
                resultSize.Width += element.DesiredSize.Width;
            }
            resultSize.Height = maxHeight;
            resultSize.Width += this.Padding.Horizontal;
            resultSize.Height += this.Padding.Vertical;
            return resultSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            float leftColumnWidth = 0;
            float rightColumnWidth = 0;
            RectangleF clientRect = GetClientRectangle(finalSize);
            RadDropDownMenuLayout menuLayout = ((RadMenuItem)this.Parent).MenuLayout;
            if (menuLayout != null)
            {
                leftColumnWidth = menuLayout.LeftColumnWidth;
                rightColumnWidth = menuLayout.RightColumnWidth;
            }
            else 
            {
                if (this.checkmark != null && this.imagePrimitive != null)
                {
                    leftColumnWidth = Math.Max(checkmark.DesiredSize.Width, imagePrimitive.DesiredSize.Width);
                }
                if (this.arrowPrimitive != null)
                {
                    rightColumnWidth = this.arrowPrimitive.DesiredSize.Width;
                }
            }

            foreach (RadElement element in this.Children)
            {
                if (element.FitToSizeMode == RadFitToSizeMode.FitToParentBounds)
                {
                    element.Arrange(new RectangleF(Point.Empty, finalSize));
                }
                else if (element.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                {
                    element.Arrange(new RectangleF(this.BorderThickness.Left, this.BorderThickness.Top, finalSize.Width - this.BorderThickness.Horizontal, finalSize.Height - this.BorderThickness.Vertical));
                }
                else
                {
                    RectangleF finalRect = new RectangleF(new PointF(clientRect.Left, clientRect.Top), element.DesiredSize);

                    if (IsRightColumnElement(element))
                    {
                        finalRect = new RectangleF(clientRect.Right - rightColumnWidth, clientRect.Top, rightColumnWidth, clientRect.Bottom);
                    }
                    else if (IsLeftContent(element))
                    {
                        finalRect = new RectangleF(clientRect.Left, clientRect.Top, element.DesiredSize.Width, clientRect.Bottom);
                    }
                    else if (IsRightContent(element))
                    {
                        finalRect = new RectangleF(clientRect.Right - element.DesiredSize.Width - rightColumnWidth, clientRect.Top, element.DesiredSize.Width, clientRect.Bottom);
                    }

                    if (this.RightToLeft)
                    {
                        finalRect = LayoutUtils.RTLTranslateNonRelative(finalRect, clientRect);
                    }

                    element.Arrange(finalRect);
                }
            }

            return finalSize;
        }

        protected virtual bool IsLeftColumnElement(RadElement element)
        {
            return element == checkmark || element == imagePrimitive;
        }

        protected virtual bool IsRightColumnElement(RadElement element)
        {
            return element == arrowPrimitive;
        }

        protected virtual bool IsLeftContent(RadElement element)
        {
            return element == internalLayoutPanel;
        }

        protected virtual bool IsRightContent(RadElement element)
        {
            return element == shortcutTextPrimitive;
        }

        protected virtual RectangleF GetClientRectangle(SizeF finalSize)
        {
            float left = this.Padding.Left + this.BorderThickness.Left;
            float top = this.Padding.Top + this.BorderThickness.Top;
            float width = Math.Max(0, finalSize.Width - (this.Padding.Horizontal + this.BorderThickness.Horizontal));
            float height = Math.Max(0, finalSize.Height - (this.Padding.Vertical + this.BorderThickness.Vertical));
            return new RectangleF(left, top, width, height);
        }
    }
}
