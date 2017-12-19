using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Base element for all visual elements across RadPageView.
    /// </summary>
    public class RadPageViewElementBase : LightVisualElement
    {
        #region Fields

        private PageViewContentOrientation contentOrientation;
        private PageViewContentOrientation borderAndFillOrientation;

        #endregion

        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.CaptureOnMouseDown = true;
            this.borderAndFillOrientation = PageViewContentOrientation.Horizontal;
            this.contentOrientation = PageViewContentOrientation.Horizontal;

            this.ImageLayout = ImageLayout.None;
            this.ClipDrawing = true;
            this.BypassLayoutPolicies = true;
        }

        #endregion

        #region RadProperties

        public static RadProperty BorderPaddingProperty = RadProperty.Register(
           "BorderPadding",
           typeof(Padding),
           typeof(RadPageViewElementBase),
           new RadElementPropertyMetadata(Padding.Empty, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty FillPaddingProperty = RadProperty.Register(
           "FillPadding",
           typeof(Padding),
           typeof(RadPageViewElementBase),
           new RadElementPropertyMetadata(Padding.Empty, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region CLR Properties

        /// <summary>
        /// Gets the content orientation for this item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PageViewContentOrientation ContentOrientation
        {
            get
            {
                return this.contentOrientation;
            }
        }

        /// <summary>
        /// Gets the content orientation for this item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PageViewContentOrientation BorderAndFillOrientation
        {
            get
            {
                return this.borderAndFillOrientation;
            }
        }

        /// <summary>
        /// Gets or sets the padding that defines the offset of element's fill.
        /// This does not affect element's layout logic such as size and location but has only appearance impact.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the padding that defines the offset of element's fill.. This does not affect element's layout logic such as size and location but has only appearance impact.")]
        public Padding FillPadding
        {
            get
            {
                return (Padding)this.GetValue(FillPaddingProperty);
            }
            set
            {
                this.SetValue(FillPaddingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the padding that defines the offset of the border.
        /// This does not affect element's layout logic such as size and location but has only appearance impact.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the padding that defines the offset of the border. This does not affect element's layout logic such as size and location but has only appearance impact.")]
        public Padding BorderPadding
        {
            get
            {
                return (Padding)this.GetValue(BorderPaddingProperty);
            }
            set
            {
                this.SetValue(BorderPaddingProperty, value);
            }
        }

        #endregion

        #region Layout

        protected override PointF CalcLayoutOffset(PointF startPoint)
        {
            Point location = this.Location;

            //Margins are explicitly excluded since they are added to the arrange rect.
            //TODO: Test carefully for glitches
            startPoint.X += location.X;
            startPoint.Y += location.Y;

            return startPoint;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF clientSize = this.GetClientRectangle(availableSize).Size;

            SizeF childSize = this.MeasureChildren(clientSize);
            SizeF contentSize = this.MeasureContent(clientSize);

            SizeF measured = this.CalculateMeasuredSize(contentSize, childSize);
            measured = this.ApplyClientOffset(measured);
            measured = this.ApplyMinMaxSize(measured);

            switch (this.contentOrientation)
            {
                case PageViewContentOrientation.Vertical270:
                case PageViewContentOrientation.Vertical90:
                    measured = new SizeF(measured.Height, measured.Width);
                    break;
            }

            return measured;
        }

        protected virtual SizeF CalculateMeasuredSize(SizeF contentSize, SizeF childSize)
        {
            SizeF measured = contentSize;
            if (childSize.Width > measured.Width)
            {
                measured.Width = childSize.Width;
            }
            if (childSize.Height > measured.Height)
            {
                measured.Height = childSize.Height;
            }

            return measured;
        }

        protected virtual SizeF MeasureContent(SizeF availableSize)
        {
            if (this.contentOrientation == PageViewContentOrientation.Vertical270 ||
                this.contentOrientation == PageViewContentOrientation.Vertical90)
            {
                availableSize = new SizeF(availableSize.Height, availableSize.Width);
            }

            return this.layoutManagerPart.Measure(availableSize);
        }

        protected SizeF GetLightVisualElementSize(SizeF avaliable)
        {
            return base.MeasureOverride(avaliable);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            this.ArrangeChildren(finalSize);
            this.ArrangeContent(finalSize);

            return finalSize;
        }

        protected virtual void ArrangeContent(SizeF finalSize)
        {
            if (this.contentOrientation == PageViewContentOrientation.Vertical270 ||
                this.contentOrientation == PageViewContentOrientation.Vertical90)
            {
                finalSize = new SizeF(finalSize.Height, finalSize.Width);
            }

            this.layoutManagerPart.Arrange(this.GetClientRectangle(finalSize));
        }

        protected virtual void ArrangeChildren(SizeF finalSize)
        {
            int count = this.Children.Count;
            RectangleF client = this.GetClientRectangle(finalSize);

            for (int i = 0; i < count; i++)
            {
                this.Children[i].Arrange(client);
            }
        }

        /// <summary>
        /// Adds padding and border size to the provided measured size.
        /// </summary>
        /// <param name="measured"></param>
        /// <returns></returns>
        protected SizeF ApplyClientOffset(SizeF measured)
        {
            //add border and padding to the desired size
            Padding padding = this.Padding;
            Padding border = this.GetBorderThickness(true);

            measured.Width += padding.Horizontal + border.Horizontal;
            measured.Height += padding.Vertical + border.Vertical;

            return measured;
        }

        /// <summary>
        /// Applies the Min/Max size constraints to the already measured size.
        /// </summary>
        /// <param name="measured"></param>
        /// <returns></returns>
        protected SizeF ApplyMinMaxSize(SizeF measured)
        {
            SizeF minSize = this.MinSize;
            SizeF maxSize = this.MaxSize;

            measured.Width = Math.Max(minSize.Width, measured.Width);
            measured.Height = Math.Max(minSize.Height, measured.Height);

            if (maxSize.Width > 0)
            {
                measured.Width = Math.Min(maxSize.Width, measured.Width);
            }
            if (maxSize.Height > 0)
            {
                measured.Height = Math.Min(maxSize.Height, measured.Height);
            }

            return measured;
        }

        #endregion

        #region Paint

        protected internal virtual void SetContentOrientation(PageViewContentOrientation orientation, bool recursive)
        {
            if (this.contentOrientation != orientation)
            {
                this.contentOrientation = orientation;
                this.FillPrimitiveImpl.InvalidateFillCache(RadPageViewElement.ItemContentOrientationProperty);
                this.InvalidateMeasure();
            }

            if (recursive)
            {
                foreach (RadElement child in this.Children)
                {
                    RadPageViewElementBase pageElement = child as RadPageViewElementBase;
                    if (pageElement != null)
                    {
                        pageElement.SetContentOrientation(orientation, recursive);
                    }
                }
            }
        }

        protected internal virtual void SetBorderAndFillOrientation(PageViewContentOrientation orientation, bool recursive)
        {
            if (this.borderAndFillOrientation == orientation)
            {
                return;
            }

            this.borderAndFillOrientation = orientation;
            this.FillPrimitiveImpl.InvalidateFillCache(RadPageViewElement.ItemContentOrientationProperty);

            if (recursive)
            {
                foreach (RadElement child in this.Children)
                {
                    RadPageViewElementBase pageElement = child as RadPageViewElementBase;
                    if (pageElement != null)
                    {
                        pageElement.SetBorderAndFillOrientation(orientation, recursive);
                    }
                }
            }
        }

        protected object ApplyOrientationTransform(IGraphics graphics, PageViewContentOrientation orientation)
        {
            if (orientation == PageViewContentOrientation.Auto ||
                orientation == PageViewContentOrientation.Horizontal)
            {
                return null;
            }

            RectangleF bounds = this.Bounds;
            object state = graphics.SaveState();

            //the enginner constant 1 is used to fix floating-point error when transforming the device
            switch (orientation)
            {
                case PageViewContentOrientation.Vertical270:
                    graphics.TranslateTransform(bounds.X, bounds.Bottom - 1);
                    graphics.RotateTransform(270);
                    break;
                case PageViewContentOrientation.Vertical90:
                    graphics.TranslateTransform(bounds.Right - 1, bounds.Y);
                    graphics.RotateTransform(90);
                    break;
                case PageViewContentOrientation.Horizontal180:
                    graphics.TranslateTransform(bounds.Right - 1, bounds.Bottom - 1);
                    graphics.RotateTransform(180);
                    break;
            }

            return state;
        }

        protected override void PrePaintElement(IGraphics graphics)
        {
            //update the RotateFlip property of the image shape (if any)
            RadImageShape background = this.BackgroundShape;
            RotateFlipType rotateFlip = RotateFlipType.RotateNoneFlipNone;
            Padding padding = Padding.Empty;
 
            if (background != null)
            {
                rotateFlip = background.RotateFlip;
                padding = background.Padding;

                switch(this.borderAndFillOrientation)
                {
                    case PageViewContentOrientation.Horizontal180:
                        background.RotateFlip = RotateFlipType.Rotate180FlipNone;
                        background.Padding = new Padding(padding.Right, padding.Bottom, padding.Left, padding.Top);
                        break;
                    case PageViewContentOrientation.Vertical90:
                        background.RotateFlip = RotateFlipType.Rotate90FlipNone;
                        background.Padding = new Padding(padding.Bottom, padding.Left, padding.Top, padding.Right);
                        break;
                    case PageViewContentOrientation.Vertical270:
                        background.RotateFlip = RotateFlipType.Rotate270FlipNone;
                        background.Padding = new Padding(padding.Top, padding.Right, padding.Bottom, padding.Left);
                        break;
                }
            }

            base.PrePaintElement(graphics);

            if (background != null)
            {
                background.RotateFlip = rotateFlip;
                background.Padding = padding;
            }
        }

        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            object state = null;

            if (this.DrawFill)
            {
                state = CorrectFillAndBorderOrientation(graphics);
                this.PaintFill(graphics, angle, scale);

                if (state != null)
                {
                    graphics.RestoreState(state);
                }
            }

            state = this.ApplyOrientationTransform(graphics, this.contentOrientation);
            this.PaintContent(graphics);

            if (this.DrawBorder)
            {
                if (state != null)
                {
                    graphics.RestoreState(state);
                }
                state = CorrectFillAndBorderOrientation(graphics);
                this.PaintBorder(graphics, angle, scale);
            }

            if (state != null)
            {
                graphics.RestoreState(state);
            }
        }

        protected virtual object CorrectFillAndBorderOrientation(IGraphics g)
        {
            return this.ApplyOrientationTransform(g, this.borderAndFillOrientation);
        }

        protected override void PaintFill(IGraphics graphics, float angle, SizeF scale, RectangleF rect)
        {
            rect = this.ModifyBorderAndFillPaintRect(rect, this.FillPadding);
            base.PaintFill(graphics, angle, scale, rect);
        }

        protected override void PaintBorder(IGraphics graphics, float angle, SizeF scale, RectangleF rect)
        {
            rect = this.ModifyBorderAndFillPaintRect(rect, this.BorderPadding);
            base.PaintBorder(graphics, angle, scale, rect);
        }

        protected virtual RectangleF ModifyBorderAndFillPaintRect(RectangleF preferred, Padding padding)
        {
            if (this.borderAndFillOrientation == PageViewContentOrientation.Vertical90 ||
                this.borderAndFillOrientation == PageViewContentOrientation.Vertical270)
            {
                preferred.Size = new SizeF(preferred.Height, preferred.Width);
            }

            return LayoutUtils.DeflateRect(preferred, padding);
        }

        #endregion
    }
}
