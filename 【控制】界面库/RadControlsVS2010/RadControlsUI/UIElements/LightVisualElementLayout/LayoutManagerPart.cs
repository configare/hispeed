using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class LayoutManagerPart : VisualElementLayoutPart
    {
        #region Fields

        //represents the Image part
        private IVisualLayoutPart left;
        //represents the Text part
        private IVisualLayoutPart right;
        private bool isDirty = false;
        private SizeF lastFinalSize = SizeF.Empty;

        #endregion

        #region Constructor

        public LayoutManagerPart(LightVisualElement owner)
            : base(owner)
        {
        }

        #endregion

        #region Properties

        public IVisualLayoutPart LeftPart
        {
            get
            {
                return this.left;
            }
            set
            {
                this.left = value;
            }
        }

        public IVisualLayoutPart RightPart
        {
            get
            {
                return this.right;
            }
            set
            {
                this.right = value;
            }
        }

        /// <summary>
        /// IsDirty Property
        /// </summary>
        public bool IsDirty
        {
            set
            {
                isDirty = value;
            }
        }

        #endregion

        #region Layout

        public override SizeF Measure(SizeF availableSize)
        {
            SizeF result = SizeF.Empty;
            SizeF leftSize = SizeF.Empty;
            SizeF rightSize = SizeF.Empty;
            TextImageRelation textImageRelation = this.owner.TextImageRelation;

            switch (textImageRelation)
            {
                case TextImageRelation.ImageAboveText:
                case TextImageRelation.TextAboveImage:
                    if (this.left != null)
                    {
                        leftSize = this.left.Measure(availableSize);
                    }

                    if (this.right != null)
                    {
                        rightSize = this.right.Measure(new SizeF(availableSize.Width, availableSize.Height - leftSize.Height));
                    }

                    result.Height += leftSize.Height;
                    result.Height += rightSize.Height;
                    result.Width = Math.Max(leftSize.Width, rightSize.Width);
                    break;
                case TextImageRelation.Overlay:
                    if (this.left != null)
                    {
                        leftSize = this.left.Measure(availableSize);
                    }

                    if (this.right != null)
                    {
                        rightSize = this.right.Measure(availableSize);
                    }

                    result.Height = Math.Max(leftSize.Height, rightSize.Height);
                    result.Width = Math.Max(leftSize.Width, rightSize.Width);
                    break;
                case TextImageRelation.ImageBeforeText:
                case TextImageRelation.TextBeforeImage:
                    if (this.left != null)
                    {
                        leftSize = this.left.Measure(availableSize);
                    }

                    if (this.right != null)
                    {
                        rightSize = this.right.Measure(new SizeF(availableSize.Width - leftSize.Width, availableSize.Height));
                    }

                    result.Width += leftSize.Width;
                    result.Width += rightSize.Width;
                    result.Height = Math.Max(leftSize.Height, rightSize.Height);
                    break;
                default:
                    break;
            }

            this.DesiredSize = result;

            return result;
        }
        
        public override SizeF Arrange(RectangleF bounds)
        {
            SizeF finalSize = bounds.Size;
            if (this.lastFinalSize != finalSize || this.isDirty)
            {
                this.lastFinalSize = finalSize;
                this.Measure(finalSize);
            }

            SizeF fieldSize = finalSize;
            RectangleF layoutField = new RectangleF(bounds.Location, finalSize);

            RectangleF imageBounds = RectangleF.Empty;
            RectangleF textBounds = RectangleF.Empty;
            SizeF textSize = SizeF.Empty;
            SizeF imageSize = SizeF.Empty;

            ContentAlignment imageAlign = this.owner.ImageAlignment;
            ContentAlignment textAlign = this.owner.TextAlignment;
            TextImageRelation textImageRelation = this.owner.TextImageRelation;

            if (this.owner.RightToLeft)
            {
                imageAlign = TelerikAlignHelper.RtlTranslateContent(imageAlign);
                textAlign = TelerikAlignHelper.RtlTranslateContent(textAlign);
                textImageRelation = TelerikAlignHelper.RtlTranslateRelation(textImageRelation);
            }

            if ((this.left != null) && (this.owner.Image == null))
            {
                this.left.Arrange(layoutField);
                if (this.right != null)
                {
                    this.right.Arrange(layoutField);
                }
                return finalSize;
            }
            if (this.right != null && (string.IsNullOrEmpty(this.owner.Text) || !this.owner.DrawText))
            {
                this.right.Arrange(layoutField);
                if (this.left != null)
                {
                    this.left.Arrange(layoutField);
                }
                return finalSize;
            }

            imageSize = this.GetInvariantLength(Size.Ceiling(this.left.DesiredSize), this.left.Margin);
            textSize = this.GetInvariantLength(Size.Ceiling(this.right.DesiredSize), this.right.Margin);

            // Subtract the image size from the whole field size
            SizeF textSizeLeft = LayoutUtils.SubAlignedRegion(fieldSize, imageSize, textImageRelation);
            // Create a new size based on the text size and the image size
            SizeF newSize = LayoutUtils.AddAlignedRegion(textSize, imageSize, textImageRelation);
            
            // The new field which is a union of the new sizes
            RectangleF maxFieldRectangle = Rectangle.Empty;            
            maxFieldRectangle.Size = LayoutUtils.UnionSizes(fieldSize, newSize);
            maxFieldRectangle.X += layoutField.X;
            maxFieldRectangle.Y += layoutField.Y;
            // Image doesn't overlay text
            bool imageAlignNoOverlay = (TelerikAlignHelper.ImageAlignToRelation(imageAlign) & textImageRelation) != TextImageRelation.Overlay;
            // Text doesn't overlay image
            bool textAlignNoOverlay = (TelerikAlignHelper.TextAlignToRelation(textAlign) & textImageRelation) != TextImageRelation.Overlay;

            if (imageAlignNoOverlay)
            {
                LayoutUtils.SplitRegion(maxFieldRectangle, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);
            }
            else if (textAlignNoOverlay)
            {
                LayoutUtils.SplitRegion(maxFieldRectangle, textSize, (AnchorStyles)LayoutUtils.GetOppositeTextImageRelation(textImageRelation), out textBounds, out imageBounds);
            }
            else
            {
                // Both image overlays text and text overlays image				
                if (textImageRelation == TextImageRelation.Overlay)
                {
                    LayoutUtils.SplitRegion(maxFieldRectangle, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);
                }
                else
                {
                    RectangleF alignedField = LayoutUtils.Align(newSize, maxFieldRectangle, ContentAlignment.MiddleCenter);
                    LayoutUtils.SplitRegion(alignedField, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);                    
                    LayoutUtils.ExpandRegionsToFillBounds(maxFieldRectangle, (AnchorStyles)textImageRelation, ref imageBounds, ref textBounds);
                }
            }
            ///////////////////////////////////////////////////////////////////////////

            //set image and text bounds according the size of the field
            if ((textImageRelation == TextImageRelation.TextBeforeImage) || (textImageRelation == TextImageRelation.ImageBeforeText))
            {
                float num1 = Math.Min(textBounds.Bottom, layoutField.Bottom);
                textBounds.Y = Math.Max(Math.Min(textBounds.Y, layoutField.Y + ((layoutField.Height - textBounds.Height) / 2)), layoutField.Y);
                textBounds.Height = num1 - textBounds.Y;
            }
            if ((textImageRelation == TextImageRelation.TextAboveImage) || (textImageRelation == TextImageRelation.ImageAboveText))
            {
                float num2 = Math.Min(textBounds.Right, layoutField.Right);
                textBounds.X = Math.Max(Math.Min(textBounds.X, layoutField.X + ((layoutField.Width - textBounds.Width) / 2)), layoutField.X);
                textBounds.Width = num2 - textBounds.X;
            }
            if ((textImageRelation == TextImageRelation.ImageBeforeText) && (imageBounds.Size.Width != 0))
            {
                imageBounds.Width = Math.Max(0, Math.Min(fieldSize.Width - textBounds.Width, imageBounds.Width));
                textBounds.X = imageBounds.X + imageBounds.Width;
            }
            if ((textImageRelation == TextImageRelation.ImageAboveText) && (imageBounds.Size.Height != 0))
            {
                imageBounds.Height = Math.Max(0, Math.Min(fieldSize.Height - textBounds.Height, imageBounds.Height));
                textBounds.Y = imageBounds.Y + imageBounds.Height;
            }
            textBounds = RectangleF.Intersect(textBounds, layoutField);

            //align image and if its size is greater than the field's size it is become offseted as much as the difference
            RectangleF newImageBounds = LayoutUtils.Align(imageSize, imageBounds, imageAlign);
            if (newImageBounds.Width > imageBounds.Width)
            {
                newImageBounds.X = imageBounds.Width - imageSize.Width;
            }
            if (newImageBounds.Height > imageBounds.Height)
            {
                newImageBounds.Y = imageBounds.Height - imageSize.Height;
            }

            textBounds = LayoutUtils.Align(textSize, textBounds, textAlign);
            //textBounds.Offset(layoutField.Location);
            //imageBounds.Offset(layoutField.Location);
            
            /////////////////////////////////////////////////////////////////////////
            textBounds.Size = SizeF.Subtract(textBounds.Size, this.left.Margin.Size);
            imageBounds.Size = SizeF.Subtract(imageBounds.Size, this.right.Margin.Size);

            this.left.Arrange(imageBounds);
            this.right.Arrange(textBounds);

            return finalSize;
        }

        #endregion

        #region HelperFunction

        private SizeF GetMinSizeLen(SizeF size1, SizeF size2)
        {
            if (size1.Width > size2.Width)
            {
                return size2;
            }
            else
            {
                return size1;
            }
        }

        private Size GetInvariantLength(Size size, Padding margin)
        {
            size.Height += margin.Vertical;
            size.Width += margin.Horizontal;
            return size;
        }
        #endregion
    }
}
