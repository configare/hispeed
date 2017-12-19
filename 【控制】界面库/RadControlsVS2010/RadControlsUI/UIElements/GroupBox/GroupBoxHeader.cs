using System;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    public class GroupBoxHeader : GroupBoxVisualElement
    {
        private TextPrimitive textPrimitive;
        private ImagePrimitive imagePrimitive;
        private ImageAndTextLayoutPanel imageAndTextLayout;     

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

        public static RadProperty HeaderPositionProperty = RadProperty.Register("HeaderPosition",
            typeof(HeaderPosition),
            typeof(GroupBoxHeader),
        new RadElementPropertyMetadata(
                        HeaderPosition.Top,
                      ElementPropertyOptions.AffectsLayout |
                      ElementPropertyOptions.InvalidatesLayout |
                      ElementPropertyOptions.AffectsMeasure |
                      ElementPropertyOptions.AffectsDisplay));

        public HeaderPosition HeaderPosition
        {
            get
            {
                return (HeaderPosition)this.GetValue(HeaderPositionProperty);
            }

            set
            {
                if ((HeaderPosition)this.GetValue(HeaderPositionProperty) != value)
                {
                    switch (value)
                    {
                        case HeaderPosition.Top: this.AngleTransform = 0; break;
                        case HeaderPosition.Left: this.AngleTransform = -90; break;
                        case HeaderPosition.Right: this.AngleTransform = 90; break;
                        case HeaderPosition.Bottom: this.AngleTransform = 0; break;
                    }

                    this.SetValue(HeaderPositionProperty, value);
                    ((RadGroupBoxElement)this.Parent).InvalidateMeasureInMainLayout++;
                }
            }
        }

        public static RadProperty GroupBoxStyleProperty = RadProperty.Register("GroupBoxStyle", typeof(RadGroupBoxStyle), typeof(GroupBoxHeader),
        new RadElementPropertyMetadata(RadGroupBoxStyle.Standard, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsDisplay |ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange));

        public RadGroupBoxStyle GroupBoxStyle
        {
            get
            {
                return (RadGroupBoxStyle)this.GetValue(GroupBoxStyleProperty);
            }

            set
            {
                this.SetValue(GroupBoxStyleProperty, value);
            }
        }

        public TextPrimitive TextPrimitive
        {
            get { return textPrimitive; }
            set { textPrimitive = value; }
        }

        public ImagePrimitive ImagePrimitive
        {
            get { return imagePrimitive; }
            set { imagePrimitive = value; }
        }

        public ImageAndTextLayoutPanel ImageAndTextLayout
        {
            get { return imageAndTextLayout; }
            set { imageAndTextLayout = value; }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.textPrimitive = new TextPrimitive();
            this.textPrimitive.UseMnemonic = false;

            this.imagePrimitive = new ImagePrimitive();


            this.imageAndTextLayout = new ImageAndTextLayoutPanel();
            this.BindProperty(RadItem.TextProperty, this.textPrimitive, RadItem.TextProperty, PropertyBindingOptions.TwoWay);
            this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);

            imageAndTextLayout.Children.Add(imagePrimitive);
            imageAndTextLayout.Children.Add(textPrimitive);

            this.Children.Add(imageAndTextLayout);

            this.Class = "GroupBoxHeader";
        }

        public override string Text
        {
            get
            {
                return this.textPrimitive.Text;
            }
            set
            {
                if (this.textPrimitive.Text != value)
                {
                    this.textPrimitive.Text = value;                    
                }
            }
        }

        public override string ToString()
        {   
            return "GroupBoxHeader";
        }

        public override bool RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
                this.imageAndTextLayout.RightToLeft = value;
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);

            SizeF desiredSize = SizeF.Empty;
            //p.p. 29.07.09 added 2*
            desiredSize.Width = imageAndTextLayout.DesiredSize.Width + ( 2 * this.BorderThickness.Vertical );
            desiredSize.Height = imageAndTextLayout.DesiredSize.Height +( 2 * this.BorderThickness.Horizontal );

            if (this.GroupBoxStyle == RadGroupBoxStyle.Office)
            {     
               desiredSize.Width = availableSize.Width;
            }   
            
            return desiredSize;
        }  
    }
}