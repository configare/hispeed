using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// A class that represents a border for a Form which is built by images.
    /// </summary>
    public class FormImageBorderPrimitive : BasePrimitive
    {
        #region Fields

        private const int DEFAULT_NON_IMAGE_WIDTH = 3;

        #endregion

        #region RadProperties

        #region Images

        public static RadProperty TopLeftEndProperty = RadProperty.Register(
            "TopLeftEnd",
            typeof(Image),
            typeof(FormImageBorderPrimitive),
            new RadElementPropertyMetadata(null,
                ElementPropertyOptions.AffectsLayout
                | ElementPropertyOptions.AffectsMeasure
                | ElementPropertyOptions.AffectsParentArrange
                | ElementPropertyOptions.AffectsDisplay)
                );

        public static RadProperty TopRightEndProperty = RadProperty.Register(
            "TopRightEnd",
            typeof(Image),
            typeof(FormImageBorderPrimitive),
            new RadElementPropertyMetadata(null,
                ElementPropertyOptions.AffectsLayout
                | ElementPropertyOptions.AffectsMeasure
                | ElementPropertyOptions.AffectsParentArrange
                | ElementPropertyOptions.AffectsDisplay)
                );

        public static RadProperty LeftTextureProperty = RadProperty.Register(
            "LeftTexture",
            typeof(Image),
            typeof(FormImageBorderPrimitive),
            new RadElementPropertyMetadata(null,
                ElementPropertyOptions.AffectsLayout
                | ElementPropertyOptions.AffectsMeasure
                | ElementPropertyOptions.AffectsParentArrange
                | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BottomLeftCornerProperty = RadProperty.Register(
            "BottomLeftCorner",
            typeof(Image),
            typeof(FormImageBorderPrimitive),
            new RadElementPropertyMetadata(null,
                ElementPropertyOptions.AffectsLayout
                | ElementPropertyOptions.AffectsMeasure
                | ElementPropertyOptions.AffectsParentArrange
                | ElementPropertyOptions.AffectsDisplay)
            );

        public static RadProperty BottomTextureProperty = RadProperty.Register(
            "BottomTexture",
            typeof(Image),
            typeof(FormImageBorderPrimitive),
            new RadElementPropertyMetadata(null,
                ElementPropertyOptions.AffectsLayout
                | ElementPropertyOptions.AffectsMeasure
                | ElementPropertyOptions.AffectsParentArrange
                | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BottomRightCornerProperty = RadProperty.Register(
            "BottomRightCorner",
            typeof(Image),
            typeof(FormImageBorderPrimitive),
            new RadElementPropertyMetadata(null,
                ElementPropertyOptions.AffectsLayout
                | ElementPropertyOptions.AffectsMeasure
                | ElementPropertyOptions.AffectsParentArrange
                | ElementPropertyOptions.AffectsDisplay)
            );

        public static RadProperty RightTextureProperty = RadProperty.Register(
           "RightTexture",
           typeof(Image),
           typeof(FormImageBorderPrimitive),
           new RadElementPropertyMetadata(null,
               ElementPropertyOptions.AffectsLayout
               | ElementPropertyOptions.AffectsMeasure
               | ElementPropertyOptions.AffectsParentArrange
               | ElementPropertyOptions.AffectsDisplay)
           );

        #endregion

        #region Colors

        #endregion

        #endregion

        #region Constructor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.FitToSizeMode = RadFitToSizeMode.FitToParentContent;
        }

        #endregion

        #region Properties

        public override Padding BorderThickness
        {
            get
            {
                Padding borderWidth = this.BorderWidth;
                return new Padding(borderWidth.Left, 0, borderWidth.Right, borderWidth.Bottom);
            }
            set
            {
                base.BorderThickness = value;
            }
        }

        /// <summary>
        /// Gets a Padding object that represents
        /// the left, top, right and bottom width of the border.
        /// </summary>
        public Padding BorderWidth
        {
            get
            {
                Padding result = new Padding(
                    this.GetAvailableLeftWidth(),
                    0,
                    this.GetAvailableRightWidth(),
                    this.GetAvailableBottomHeight());

                if (result == Padding.Empty)
                {
                    return new Padding(
                        DEFAULT_NON_IMAGE_WIDTH,
                        0,
                        DEFAULT_NON_IMAGE_WIDTH,
                        DEFAULT_NON_IMAGE_WIDTH);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the left Image which represents the
        /// transition between the image border and the
        /// title bar element.
        /// </summary>
        public Image TopLeftEnd
        {
            get
            {
                return this.GetValue(TopLeftEndProperty) as Image;
            }
            set
            {
                this.SetValue(TopLeftEndProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right Image which represents the
        /// transition between the image border and the
        /// title bar element.
        /// </summary>
        public Image TopRightEnd
        {
            get
            {
                return this.GetValue(TopRightEndProperty) as Image;
            }
            set
            {
                this.SetValue(TopRightEndProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the texture for the left image border.
        /// </summary>
        public Image LeftTexture
        {
            get
            {
                return this.GetValue(LeftTextureProperty) as Image;
            }
            set
            {
                this.SetValue(LeftTextureProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the texture for the bottom image border.
        /// </summary>
        public Image BottomTexture
        {
            get
            {
                return this.GetValue(BottomTextureProperty) as Image;
            }
            set
            {
                this.SetValue(BottomTextureProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the texture for the right image border.
        /// </summary>
        public Image RightTexture
        {
            get
            {
                return this.GetValue(RightTextureProperty) as Image;
            }
            set
            {
                this.SetValue(RightTextureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image for the bottom left border corner.
        /// </summary>
        public Image BottomLeftCorner
        {
            get
            {
                return this.GetValue(BottomLeftCornerProperty) as Image;
            }
            set
            {
                this.SetValue(BottomLeftCornerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image for the bottom right border corner.
        /// </summary>
        public Image BottomRightCorner
        {
            get
            {
                return this.GetValue(BottomRightCornerProperty) as Image;
            }
            set
            {
                this.SetValue(BottomRightCornerProperty, value);
            }
        }

        #endregion

        #region Methods

        #region Overriden

        public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
        {
            base.PaintPrimitive(graphics, angle, scale);

            if (!this.IsAtLeastOneImageLoaded())
            {
                this.PaintBackground(graphics);
            }
            else
            {
                this.PaintLeftTopEnd(graphics);
                this.PaintLeftBorderTexture(graphics);
                this.PaintBottomLeftCorner(graphics);
                this.PaintBottomTexture(graphics);
                this.PaintBottomRightCorner(graphics);
                this.PaintRightBorderTexture(graphics);
                this.PaintRightTopEnd(graphics);
            }
        }

        #endregion

        #region Helper methods

        private bool IsAtLeastOneImageLoaded()
        {
            bool result = false;

            result = this.LeftTexture != null || result;
            result = this.BottomTexture != null  || result;
            result = this.RightTexture != null || result;

            return result;
        }

        private int GetAvailableLeftWidth()
        {
            int result = 0;

            if (this.LeftTexture != null)
            {
                result = this.LeftTexture.Width;

                if (this.TopLeftEnd != null)
                {
                    result = Math.Min(result, this.TopLeftEnd.Width);
                }

                if (this.BottomLeftCorner != null)
                {
                    result = Math.Min(result, this.BottomLeftCorner.Width);
                }
            }

            return result;
        }

        private int GetAvailableBottomHeight()
        {
            int result = 0;

            if (this.BottomTexture != null)
            {
                result = this.BottomTexture.Height;

                if (this.BottomLeftCorner != null)
                {
                    result = Math.Min(result, this.BottomLeftCorner.Height);
                }

                if (this.BottomRightCorner != null)
                {
                    result = Math.Min(result, this.BottomRightCorner.Height);
                }
            }

            return result;
        }

        private int GetAvailableRightWidth()
        {
            int result = 0;

            if (this.RightTexture != null)
            {
                result = this.RightTexture.Width;

                if (this.TopRightEnd != null)
                {
                    result = Math.Min(result, this.TopRightEnd.Width);
                }

                if (this.BottomRightCorner != null)
                {
                    result = Math.Min(result, this.BottomRightCorner.Width);
                }
            }

            return result;
        }

        #endregion

        private void PaintBackground(IGraphics graphics)
        {
            Rectangle leftPartRectangle = new Rectangle(Point.Empty, new Size(DEFAULT_NON_IMAGE_WIDTH, this.Size.Height));

            graphics.FillRectangle(leftPartRectangle, this.BackColor);

            Rectangle bottomPartRectangle = new Rectangle(
                new Point(DEFAULT_NON_IMAGE_WIDTH, this.Size.Height - DEFAULT_NON_IMAGE_WIDTH),
                new Size(this.Size.Width - ( 2 * DEFAULT_NON_IMAGE_WIDTH ) , DEFAULT_NON_IMAGE_WIDTH));

            graphics.FillRectangle(bottomPartRectangle, this.BackColor);

            Rectangle rightPartRectangle = new Rectangle(
                new Point(this.Size.Width - DEFAULT_NON_IMAGE_WIDTH, 0),
                new Size(DEFAULT_NON_IMAGE_WIDTH, this.Size.Height));

            graphics.FillRectangle(rightPartRectangle, this.BackColor);


        }

        private void PaintLeftTopEnd(IGraphics graphics)
        {
            if (this.TopLeftEnd == null)
            {
                return;
            }

            int xCoord = 0;
            int yCoord = 0;

            int availableWidth = this.GetAvailableLeftWidth();

            Rectangle topLeftEndRect = new Rectangle(xCoord, yCoord, availableWidth, this.TopLeftEnd.Height);

            graphics.FillTextureRectangle(topLeftEndRect, this.TopLeftEnd, System.Drawing.Drawing2D.WrapMode.Tile);
        }

        private void PaintRightTopEnd(IGraphics graphics)
        {
            if (this.TopRightEnd == null)
            {
                return;
            }

            int availableRightWidth = this.GetAvailableRightWidth();

            int xCoord = this.Size.Width - availableRightWidth;
            int yCoord = 0;


            Rectangle topLeftEndRect = new Rectangle(xCoord, yCoord, availableRightWidth, this.TopRightEnd.Height);
            graphics.DrawImage(topLeftEndRect, this.TopRightEnd, ContentAlignment.TopLeft, true);
        }

        private void PaintLeftBorderTexture(IGraphics graphics)
        {
            if (this.LeftTexture == null)
            {
                return;
            }

            int xCoord = 0;
            int yCoord = this.TopLeftEnd != null ? this.TopLeftEnd.Height : 0;

            int availableHeight = this.Size.Height;
            int availableWidth = this.GetAvailableLeftWidth();

            if (this.TopLeftEnd != null)
            {
                availableHeight -= this.TopLeftEnd.Height;
            }

            if (this.BottomLeftCorner != null)
            {
                availableHeight -= this.GetAvailableBottomHeight();
            }

            Rectangle leftBorderTextureRect = new Rectangle(xCoord, yCoord, availableWidth, availableHeight);

            graphics.FillTextureRectangle(leftBorderTextureRect, this.LeftTexture, System.Drawing.Drawing2D.WrapMode.Tile);
        }

        private void PaintBottomLeftCorner(IGraphics graphics)
        {
            if (this.BottomLeftCorner == null)
            {
                return;
            }

            int availableBottomHeight = this.GetAvailableBottomHeight();
            int availableLeftWidth = this.GetAvailableLeftWidth();

            int xCoord = 0;
            int yCoord = this.Size.Height - availableBottomHeight;

            Rectangle rectangle = new Rectangle(xCoord, yCoord, availableLeftWidth, availableBottomHeight);

            graphics.DrawImage(rectangle, this.BottomLeftCorner, ContentAlignment.TopLeft, true);
        }

        private void PaintBottomTexture(IGraphics graphics)
        {
            if (this.BottomTexture == null)
            {
                return;
            }

            int availableRightWidth = this.GetAvailableRightWidth();
            int availableLeftWidth = this.GetAvailableLeftWidth();

            int availableWidth = this.Size.Width;
            int availableHeight = this.GetAvailableBottomHeight();

            int xCoord = availableLeftWidth;
            int yCoord = this.Size.Height - availableHeight;


            availableWidth -= availableLeftWidth;
            availableWidth -= availableRightWidth;

            Rectangle bottomBorderTexture = new Rectangle(xCoord, yCoord, availableWidth, availableHeight);

            graphics.FillTextureRectangle(bottomBorderTexture, this.BottomTexture, System.Drawing.Drawing2D.WrapMode.Tile);
        }

        private void PaintBottomRightCorner(IGraphics graphics)
        {
            if (this.BottomRightCorner == null)
            {
                return;
            }

            int availableWidth = this.GetAvailableRightWidth();
            int availableHeight = this.GetAvailableBottomHeight();
            int xCoord = this.Size.Width - availableWidth;
            int yCoord = this.Size.Height - availableHeight;

            Rectangle rectangle = new Rectangle(xCoord, yCoord, availableWidth, availableHeight);

            graphics.DrawImage(rectangle, this.BottomRightCorner, ContentAlignment.TopLeft, true);
        }

        private void PaintRightBorderTexture(IGraphics graphics)
        {
            if (this.RightTexture == null)
            {
                return;
            }

            int availableHeight = this.Size.Height;
            int availableWidth = this.GetAvailableRightWidth();

            int xCoord = this.Size.Width - availableWidth;
            int yCoord = this.TopRightEnd != null ? this.TopRightEnd.Height : 0;

       

            if (this.TopRightEnd != null)
            {
                availableHeight -= this.TopRightEnd.Height;
            }

            if (this.BottomRightCorner != null)
            {
                availableHeight -= this.GetAvailableBottomHeight();
            }

            Rectangle rightBorderTextureRect = new Rectangle(xCoord, yCoord, availableWidth, availableHeight);

            graphics.FillTextureRectangle(rightBorderTextureRect, this.RightTexture, System.Drawing.Drawing2D.WrapMode.Tile);
        }

        #endregion

        #region Disposing

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            if (this.TopRightEnd != null)
            {
                this.TopLeftEnd.Dispose();
            }

            if (this.LeftTexture != null)
            {
                this.LeftTexture.Dispose();
            }

            if (this.BottomLeftCorner != null)
            {
                this.BottomLeftCorner.Dispose();
            }

            if (this.BottomTexture != null)
            {
                this.BottomTexture.Dispose();
            }

            if (this.BottomRightCorner != null)
            {
                this.BottomRightCorner.Dispose();
            }

            if (this.RightTexture != null)
            {
                this.RightTexture.Dispose();
            }

            if (this.TopRightEnd != null)
            {
                this.TopRightEnd.Dispose();
            }
        }

        #endregion
    }
}
