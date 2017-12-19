using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.Drawing.Imaging;

namespace Telerik.WinControls
{
    public class RadImageSegment
    {
        #region Fields

        private ImageSegments segment;
        private Image imagePart;
        private Rectangle sourceRect;
        private Padding renderMargins;

        #endregion

        #region Constructor

        public RadImageSegment(ImageSegments segment)
        {
            this.segment = segment;
        }

        #endregion

        #region Properties

        public Padding RenderMargins
        {
            get
            {
                return this.renderMargins;
            }
        }

        public Rectangle SourceRect
        {
            get
            {
                return this.sourceRect;
            }
        }

        /// <summary>
        /// Gets the segment associated with this object.
        /// </summary>
        public ImageSegments Segment
        {
            get
            {
                return this.segment;
            }
        }

        /// <summary>
        /// Gets or sets the image part associated with this object.
        /// </summary>
        public Image ImagePart
        {
            get
            {
                return this.imagePart;
            }
            set
            {
                this.imagePart = value;
            }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (this.imagePart != null)
            {
                this.imagePart.Dispose();
                this.imagePart = null;
            }
        }

        public Rectangle GetDestinationRect(Rectangle paintRect, Padding margins)
        {
            Rectangle destinationRect = Rectangle.Empty;

            switch (this.segment)
            {
                case ImageSegments.TopLeft:
                    destinationRect = new Rectangle(paintRect.X, paintRect.Y, margins.Left, margins.Top);
                    break;
                case ImageSegments.Top:
                    destinationRect = new Rectangle(paintRect.X + margins.Left, paintRect.Y, paintRect.Width - margins.Horizontal, margins.Top);
                    break;
                case ImageSegments.TopRight:
                    destinationRect = new Rectangle(paintRect.Right - margins.Right, paintRect.Y, margins.Right, margins.Top);
                    break;
                case ImageSegments.Right:
                    destinationRect = new Rectangle(paintRect.Right - margins.Right, paintRect.Y + margins.Top, margins.Right, paintRect.Height - margins.Vertical);
                    break;
                case ImageSegments.BottomRight:
                    destinationRect = new Rectangle(paintRect.Right - margins.Right, paintRect.Bottom - margins.Bottom, margins.Right, margins.Bottom);
                    break;
                case ImageSegments.Bottom:
                    destinationRect = new Rectangle(paintRect.X + margins.Left, paintRect.Bottom - margins.Bottom, paintRect.Width - margins.Horizontal, margins.Bottom);
                    break;
                case ImageSegments.BottomLeft:
                    destinationRect = new Rectangle(paintRect.X, paintRect.Bottom - margins.Bottom, margins.Left, margins.Bottom);
                    break;
                case ImageSegments.Left:
                    destinationRect = new Rectangle(paintRect.X, paintRect.Y + margins.Top, margins.Left, paintRect.Height - margins.Vertical);
                    break;
                case ImageSegments.Inner:
                    destinationRect = new Rectangle(paintRect.X + margins.Left, paintRect.Y + margins.Top, paintRect.Width - margins.Horizontal, paintRect.Height - margins.Vertical);
                    break;
            }

            return destinationRect;
        }

        public void UpdateSourceRect(Size imageSize, Padding margins)
        {
            this.renderMargins = margins;

            switch (this.segment)
            {
                case ImageSegments.Left:
                    this.sourceRect = new Rectangle(0, margins.Top, margins.Left, imageSize.Height - margins.Vertical);
                    break;
                case ImageSegments.TopLeft:
                    this.sourceRect = new Rectangle(0, 0, margins.Left, margins.Top);
                    break;
                case ImageSegments.Top:
                    this.sourceRect = new Rectangle(margins.Left, 0, imageSize.Width - margins.Horizontal, margins.Top);
                    break;
                case ImageSegments.TopRight:
                    this.sourceRect = new Rectangle(imageSize.Width - margins.Right, 0, margins.Right, margins.Top);
                    break;
                case ImageSegments.Right:
                    this.sourceRect = new Rectangle(imageSize.Width - margins.Right, margins.Top, margins.Right, imageSize.Height - margins.Vertical);
                    break;
                case ImageSegments.BottomRight:
                    this.sourceRect = new Rectangle(imageSize.Width - margins.Right, imageSize.Height - margins.Bottom, margins.Right, margins.Bottom);
                    break;
                case ImageSegments.Bottom:
                    this.sourceRect = new Rectangle(margins.Left, imageSize.Height - margins.Bottom, imageSize.Width - margins.Horizontal, margins.Bottom);
                    break;
                case ImageSegments.BottomLeft:
                    this.sourceRect = new Rectangle(0, imageSize.Height - margins.Bottom, margins.Left, margins.Bottom);
                    break;
                case ImageSegments.Inner:
                    this.sourceRect = new Rectangle(margins.Left, margins.Top, imageSize.Width - margins.Horizontal, imageSize.Height - margins.Vertical);
                    break;
            }
        }

        public void UpdateFromImage(Image image, Padding margins)
        {
            this.UpdateSourceRect(image.Size, margins);
            this.imagePart = this.GetImagePart(image);
        }

        private Image GetImagePart(Image image)
        {
            int width = this.sourceRect.Width;
            int height = this.sourceRect.Height;

            if (width <= 0 || height <= 0)
                return null;

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            Rectangle destinationRect = new Rectangle(0, 0, width, height);

            g.DrawImage(image, destinationRect, this.sourceRect.X, this.sourceRect.Y, width, height, GraphicsUnit.Pixel);
            g.Dispose();

            return bmp;
        }

        #endregion
    }
}
