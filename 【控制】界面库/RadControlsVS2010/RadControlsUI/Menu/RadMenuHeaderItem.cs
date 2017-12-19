using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadMenuHeaderItem : RadMenuItemBase
    {
        private ImagePrimitive imagePrimitive;
        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive;
        private TextPrimitive textPrimitive;

        #region Constructors

        public RadMenuHeaderItem()
            : this("")
        {
        }

        public RadMenuHeaderItem(string text)
        {
            this.Text = text;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "RadMenuHeaderItem";
        }

        #endregion

        #region Properties

        public override bool Selectable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///    Gets or sets the index value of the image that is displayed on the item. 
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the index value of the image that is displayed on the item.")]
        [RelatedImageList("MenuElement.ElementTree.Control.ImageList")]
        public override int ImageIndex
        {
            get
            {
                return base.ImageIndex;
            }
            set
            {
                base.ImageIndex = value;
            }
        }

        /// <summary>
        ///    Gets or sets the key accessor for the image in the ImageList.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the key accessor for the image in the ImageList.")]
        [RelatedImageList("MenuElement.ElementTree.Control.ImageList")]
        public override string ImageKey
        {
            get
            {
                return base.ImageKey;
            }
            set
            {
                base.ImageKey = value;
            }
        }

        #endregion

        protected override void CreateChildElements()
        {
            // fill
            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "RadMenuHeaderItemFillPrimitive";
            this.fillPrimitive.BackColor = Color.Empty;
            this.fillPrimitive.GradientStyle = GradientStyles.Solid;
            this.fillPrimitive.Name = "MenuHeaderItemFill";
            this.Children.Add(this.fillPrimitive);

            // border
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "RadMenuHeaderItemBorderPrimitive";
            this.borderPrimitive.Name = "MenuHeaderItemBorder";
            this.Children.Add(this.borderPrimitive);

            // image
            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadButtonItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadButtonItem.ImageProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadButtonItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.Class = "RadMenuHeaderItemImagePrimitive";
            this.Children.Add(this.imagePrimitive);

            this.textPrimitive = new TextPrimitive();
            this.textPrimitive.Class = "RadMenuHeaderItemText";
            this.Children.Add(this.textPrimitive);

            this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadButtonItem.TextProperty, PropertyBindingOptions.TwoWay);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
            RadDropDownMenuLayout menuLayout = this.Parent as RadDropDownMenuLayout;
            float leftColumn = 0;
            float rightColumn = 0;
            if (menuLayout != null)
            {
                leftColumn = menuLayout.LeftColumnMaxPadding + menuLayout.LeftColumnWidth;
                rightColumn = menuLayout.LeftColumnMaxPadding + menuLayout.RightColumnWidth;
            }

            foreach (RadElement element in this.Children)
            { 
                if (element == this.imagePrimitive)
                {
                    float leftColumnWidth = menuLayout != null ? menuLayout.LeftColumnWidth : 0;
                    RectangleF finalRect = new RectangleF(clientRect.Left, clientRect.Top, leftColumnWidth, clientRect.Height);
                    if (this.RightToLeft)
                    {
                        finalRect = LayoutUtils.RTLTranslateNonRelative(finalRect, clientRect);
                    }

                    element.Arrange(finalRect);
                }
                else if (element == this.textPrimitive)
                {
                    RectangleF cr = new RectangleF(clientRect.Left + leftColumn, clientRect.Top, clientRect.Width - rightColumn, clientRect.Height);
                    RectangleF r = LayoutUtils.Align(this.textPrimitive.DesiredSize, cr, this.textPrimitive.Alignment);
                    if (this.RightToLeft)
                    {
                        r = LayoutUtils.RTLTranslateNonRelative(r, clientRect);
                    }

                    element.Arrange(r);
                }
                else if (element.FitToSizeMode == RadFitToSizeMode.FitToParentBounds)
                {
                    element.Arrange(new RectangleF(Point.Empty, finalSize));
                }
                else if (element.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                {
                    element.Arrange(new RectangleF(this.BorderThickness.Left, this.BorderThickness.Top, finalSize.Width - this.BorderThickness.Horizontal, finalSize.Height - this.BorderThickness.Vertical));
                }
                else
                {
                    element.Arrange(clientRect);
                } 
            }

            return finalSize;
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "Class")
            {
                return this.Class != "RadMenuHeaderItem";
            }

            return base.ShouldSerializeProperty(property);
        }
    }
}
