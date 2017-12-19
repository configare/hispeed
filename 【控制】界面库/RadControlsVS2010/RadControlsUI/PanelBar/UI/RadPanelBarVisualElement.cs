using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using System.Drawing;
using Telerik.WinControls.UI;
using Telerik.WinControls.Design;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using Telerik.WinControls.Paint;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadPanelBarVisualElement : LightVisualElement
    {
        public static RadProperty SelectedProperty = RadProperty.Register(
            "Selected", typeof(bool), typeof(RadPanelBarVisualElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        /// <summary>
        /// Gets or sets whether the group is selected
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets whether the group is selected.")]
        [DefaultValue(false)]
        public bool Selected
        {
            get
            {
                return (bool)this.GetValue(SelectedProperty);
            }
            set
            {
                this.SetValue(SelectedProperty, value);
            }
        }

        public static RadProperty InitialOffsetProperty = RadProperty.Register(
            "InitialOffset", typeof(int), typeof(RadPanelBarVisualElement), new RadElementPropertyMetadata(
                5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public RadPanelBarVisualElement()
        {
            this.ClipDrawing = true;
        }

        static RadPanelBarVisualElement()
        {
            ImageAlignmentProperty.OverrideMetadata(typeof(RadPanelBarVisualElement), new RadElementPropertyMetadata(ContentAlignment.MiddleLeft,
                                                                                                                     ElementPropertyOptions.AffectsLayout |
            ElementPropertyOptions.InvalidatesLayout));
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(RadPanelBarVisualElement));
        }

        internal int captionOffset = 0;

        /// <summary>
        /// Gets or sets the initial offset of caption elements
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the initial offset of caption elements")]
        [DefaultValue(5)]
        public int InitialOffset
        {
            get
            {
                return (int)this.GetValue(InitialOffsetProperty);
            }
            set
            {
                this.SetValue(InitialOffsetProperty, value);
            }
        }

        /// <summary>Gets or sets a value indicating the image alignment.</summary>
        [RadPropertyDefaultValue("ImageAlignment", typeof(RadPanelBarVisualElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public override ContentAlignment ImageAlignment
        {
            get
            {
                return base.ImageAlignment;
            }
            set
            {
                base.ImageAlignment = value;
            }
        }

        private bool allowImageAlignment = true;

        /// <summary>
        /// Gets or sets a value if the image could be aligned.
        /// </summary>
        public bool AllowImageAlignment
        {
            get
            {
                return this.allowImageAlignment;
            }
            set
            {
                this.allowImageAlignment = value;
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF size = base.MeasureOverride(availableSize);
            return new SizeF(Math.Min(size.Width + Padding.Horizontal, availableSize.Width + Padding.Horizontal),
                             Math.Min(size.Height + Padding.Vertical, availableSize.Height + Padding.Vertical));          
        }

        RectangleF textRectangle;

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            this.CalculateTextRectangle(finalSize);
            return finalSize;
        }

        private void CalculateTextRectangle(SizeF finalSize)
        {
            RectangleF rect = new RectangleF(PointF.Empty, finalSize);

            int offset = this.InitialOffset + captionOffset;
            if (this.Image != null)
            {
                offset += this.Image.Width;
            }
             
            rect.X = 0;// this.Size.Width - (int)textSize.Width - this.Padding.Left - offset;
            rect.Width = this.DesiredSize.Width-this.Padding.Horizontal;        
            rect.X += this.Padding.Left + offset;            
            rect.Y += this.Padding.Top;
            rect.Height -= this.Padding.Bottom + this.Padding.Top;
            float widthOffset = rect.Width;
            if (this.Children.Count > 0 && this.Children[0].Visibility == ElementVisibility.Visible)
            {
                if (this.GetGroupElement() != null)
                {
                    if (this.GetGroupElement().GetPanelBarElement().PanelBarStyle != PanelBarStyles.VisualStudio2005ToolBox)
                    {
                        widthOffset = this.Children[0].BoundingRectangle.X;
                    }
                }
            }

            float textWidth = widthOffset < this.DesiredSize.Width ? widthOffset : this.DesiredSize.Width;
            this.textRectangle = new RectangleF(rect.X, rect.Y, textWidth , rect.Height);
        }

        protected override void PaintText(IGraphics graphics)
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                return;
            }

            TextParams textParams = this.CreateTextParams();
            textParams.paintingRectangle = this.textRectangle;
            this.textPrimitiveImpl.PaintPrimitive(graphics, textParams);
        }

        private int imageIndex = -1;
		
        [DefaultValue(-1)]
        [RelatedImageList("ElementTree.Control.ImageList")]
        public new int ImageIndex
        {
            get
            {
                return this.imageIndex;
            }
            set
            {
                if (value != imageIndex)
                {
                    if (imageIndex != -1 && value == -1)
                    {
                        this.Image = null;
                    }

                    imageIndex = value;
                }
            }
        }

        private string imageKey = string.Empty;

        [DefaultValue("")]
        [RelatedImageList("ElementTree.Control.ImageList")]
        public new string ImageKey
        {
            get
            {
                return imageKey;
            }
            set
            {
                if (value != imageKey)
                {
                    if (String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(imageKey))
                    {
                        this.Image = null;
                    }

                    imageKey = value;
                }
            }
        }

        private RadPanelBarGroupElement GetGroupElement()
        {
            RadElement element = this.Parent;

            while (element != null)
            {
                if (element as RadPanelBarGroupElement != null)
                {
                    return element as RadPanelBarGroupElement;
                }
                else
                {
                    element = element.Parent;
                }
            }

            return element as RadPanelBarGroupElement;
        }

        protected override void PaintImage(Telerik.WinControls.Paint.IGraphics graphics)
        {
            if (this.ElementState != ElementState.Loaded || !this.AllowImageAlignment)
            {
                base.PaintImage(graphics);
                return;
            }
            ImageList associatedImageList = this.ElementTree.ComponentTreeHandler.ImageList;

            if (associatedImageList != null && this.ImageIndex > -1 && this.ImageIndex < associatedImageList.Images.Count)
            {
                RadPanelBarGroupElement group = this.GetGroupElement();
                if (group != null)
                {
                    if (group.GetPanelBarElement() != null)
                    {
                        if (group.GetPanelBarElement().ElementTree != null)
                        {
                            if (this.Image == null)
                                this.Image = associatedImageList.Images[this.ImageIndex];
                        }
                    }
                }
            }
            else
            {
                if (associatedImageList != null && !String.IsNullOrEmpty(this.ImageKey))
                {
                    RadPanelBarGroupElement group = this.GetGroupElement();
                    if (group != null)
                    {
                        if (group.GetPanelBarElement() != null)
                        {
                            if (group.GetPanelBarElement().ElementTree != null)
                            {
                                if (this.Image == null)
                                    this.Image = associatedImageList.Images[this.ImageKey];
                            }
                        }
                    }
                }
            }

            int offset = InitialOffset + captionOffset;

            if (this.Image == null)
                return;

            if (this.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
            {
                offset = this.Size.Width - InitialOffset - captionOffset - Image.Size.Width;
            }

            switch (this.ImageAlignment)
            {
                case ContentAlignment.BottomLeft:
                    graphics.DrawBitmap(this.Image, offset, (this.Size.Height - this.Image.Height));
                    break;
                case ContentAlignment.BottomCenter:
                    graphics.DrawBitmap(this.Image, (this.Size.Width - this.Image.Width) / 2, (this.Size.Height - this.Image.Height));
                    break;

                case ContentAlignment.BottomRight:
                    graphics.DrawBitmap(this.Image, (this.Size.Width - this.Image.Width) - offset, (this.Size.Height - this.Image.Height));
                    break;
                case ContentAlignment.MiddleLeft:
                    graphics.DrawBitmap(this.Image, offset, (this.Size.Height - this.Image.Height) / 2);
                    break;
                case ContentAlignment.MiddleRight:
                    graphics.DrawBitmap(this.Image, (this.Size.Width - this.Image.Width) - offset, (this.Size.Height - this.Image.Height) / 2);
                    break;
                case ContentAlignment.MiddleCenter:
                    graphics.DrawBitmap(this.Image, (this.Size.Width - this.Image.Width) / 2, (this.Size.Height - this.Image.Height) / 2);
                    break;

                case ContentAlignment.TopLeft:
                    graphics.DrawBitmap(this.Image, offset, 0);
                    break;
                case ContentAlignment.TopCenter:
                    graphics.DrawBitmap(this.Image, (this.Size.Width - this.Image.Width) / 2, 0);
                    break;

                case ContentAlignment.TopRight:
                    graphics.DrawBitmap(this.Image, (this.Size.Width - this.Image.Width) - offset, 0);
                    break;
            }
        }
    }
}
