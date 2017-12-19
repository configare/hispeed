using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Layouts;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Drawing.Design;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents an image which may be divided in 9 different segments where only the inner one is stretched within the paint rectangle.
    /// </summary>
    [TypeConverter(typeof(RadImageShapeTypeConverter))]
    [Editor(DesignerConsts.RadImageShapeEditorString, typeof(UITypeEditor))]
    public class RadImageShape : ICloneable
    {
        #region Fields

        public const string SerializationSeparator = ";";

        private Image image;
        private InterpolationMode interpolationMode;
        private ImagePaintMode paintMode;
        private RotateFlipType rotateFlip;
        private bool useSegments;
        private ImageSegments visibleSegments;
        private Padding margins;
        private Padding padding;
        private float alpha;
        private RadImageSegment[] segments;

        private Image cachedImage;
        private bool imageDirty;
        private bool segmentsDirty;

        #endregion

        #region Constructor

        public RadImageShape()
        {
            this.interpolationMode = InterpolationMode.NearestNeighbor;
            this.paintMode = ImagePaintMode.Stretch;
            this.rotateFlip = RotateFlipType.RotateNoneFlipNone;
            this.useSegments = true;
            this.visibleSegments = ImageSegments.All;
            this.alpha = 1F;

            this.imageDirty = true;
            this.segmentsDirty = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the RotateFlipType value that defines additional transform on the rendered image.
        /// </summary>
        [DefaultValue(RotateFlipType.RotateNoneFlipNone)]
        [Description("Gets or sets the RotateFlipType value that defines additional transform on the rendered image.")]
        public RotateFlipType RotateFlip
        {
            get
            {
                return this.rotateFlip;
            }
            set
            {
                if (this.rotateFlip == value)
                {
                    return;
                }

                this.rotateFlip = value;
                this.imageDirty = true;
                this.segmentsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the interpolation mode to be applied on the device context when image is rendered.
        /// </summary>
        [DefaultValue(InterpolationMode.NearestNeighbor)]
        [Description("Gets or sets the interpolation mode to be applied on the device context when image is rendered.")]
        public InterpolationMode InterpolationMode
        {
            get
            {
                return this.interpolationMode;
            }
            set
            {
                if (value == InterpolationMode.Default || value == InterpolationMode.Invalid)
                {
                    value = InterpolationMode.NearestNeighbor;
                }

                this.interpolationMode = value;
            }
        }

        /// <summary>
        /// Determines which segments from the image will be painted.
        /// </summary>
        [DefaultValue(ImageSegments.All)]
        [Description("Determines which segments from the image will be painted.")]
        public ImageSegments VisibleSegments
        {
            get
            {
                return this.visibleSegments;
            }
            set
            {
                if (this.visibleSegments == value)
                {
                    return;
                }

                this.visibleSegments = value;
            }
        }

        /// <summary>
        /// Determines whether the image will be rendered using segments.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the image will be rendered using segments.")]
        public bool UseSegments
        {
            get
            {
                return this.useSegments;
            }
            set
            {
                if (this.useSegments == value)
                {
                    return;
                }

                this.useSegments = value;
                this.segmentsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the mode to be used when image is painted.
        /// </summary>
        [DefaultValue(ImagePaintMode.Stretch)]
        [Description("Gets or sets the mode to be used when image is painted.")]
        public ImagePaintMode PaintMode
        {
            get
            {
                return this.paintMode;
            }
            set
            {
                if (this.paintMode == value)
                {
                    return;
                }

                this.paintMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the image to be rendered.
        /// </summary>
        [XmlIgnore]
        [Description("Gets or sets the image to be rendered.")]
        [DefaultValue(null)]
        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (this.image == value)
                {
                    return;
                }

                this.image = value;
                this.imageDirty = true;
                this.segmentsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the string representation of the 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ImageStream
        {
            get
            {
                if (this.image != null)
                {
                    return TelerikHelper.ImageToString(this.image);
                }

                return string.Empty;
            }
            set
            {
                Image newImage = null;
                if (!string.IsNullOrEmpty(value))
                {
                    newImage = TelerikHelper.ImageFromString(value);
                }

                this.Image = newImage;
            }
        }

        /// <summary>
        /// Gets or sets the opacity of the rendered image. Valid values are within the interval [0, 1].
        /// </summary>
        [DefaultValue(1F)]
        [Description("Gets or sets the opacity of the rendered image. Valid values are within the interval [0, 1].")]
        public float Alpha
        {
            get
            {
                return this.alpha;
            }
            set
            {
                value = Math.Max(0, value);
                value = Math.Min(1, value);

                if (this.alpha == value)
                {
                    return;
                }

                this.alpha = value;
                this.imageDirty = true;
                this.segmentsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the Padding structure that defines the margins of the segmented image.
        /// </summary>
        [Description("Gets or sets the Padding structure that defines the margins of the segmented image.")]
        public Padding Margins
        {
            get
            {
                return this.margins;
            }
            set
            {
                if (this.margins == value)
                {
                    return;
                }

                this.margins = value;
                this.segmentsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the Padding structure that defines offset when the image is rendered to the destination rectangle.
        /// </summary>
        [Description("Gets or sets the Padding structure that defines offset when the image is rendered to the destination rectangle.")]
        public Padding Padding
        {
            get
            {
                return this.padding;
            }
            set
            {
                if (this.padding == value)
                {
                    return;
                }

                this.padding = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeImageStream()
        {
            return this.image != null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMargins()
        {
            return this.margins != Padding.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePadding()
        {
            return this.padding != Padding.Empty;
        }

        #endregion

        #region Implementation

        public object Clone()
        {
            RadImageShape cloning = new RadImageShape();
            if (this.image != null)
            {
                cloning.image = this.image.Clone() as Image;
            }

            cloning.alpha = this.alpha;
            cloning.interpolationMode = this.interpolationMode;
            cloning.margins = this.margins;
            cloning.padding = this.padding;
            cloning.paintMode = this.paintMode;
            cloning.rotateFlip = this.rotateFlip;
            cloning.useSegments = this.useSegments;
            cloning.visibleSegments = this.visibleSegments;

            return cloning;
        }

        public RadImageSegment GetSegment(ImageSegments segment)
        {
            if (this.segments != null)
            {
                int length = this.segments.Length;
                for (int i = 0; i < length; i++)
                {
                    if (this.segments[i].Segment == segment)
                    {
                        return this.segments[i];
                    }
                }
            }

            return null;
        }

        public virtual void Paint(Graphics g, RectangleF bounds)
        {
            lock (Locker.SyncObj)
            {
                Rectangle paintRect = LayoutUtils.DeflateRect(Rectangle.Round(bounds), this.padding);
                if (paintRect.Width <= 0 || paintRect.Height <= 0)
                {
                    return;
                }

                InterpolationMode currentMode = g.InterpolationMode;
                g.InterpolationMode = this.interpolationMode;

                if (this.imageDirty)
                {
                    this.ResetImageCache();
                }
                if (this.segmentsDirty)
                {
                    this.ResetSegments();
                }

                if (this.cachedImage != null)
                {
                    this.PaintCore(g, paintRect);
                }

                g.InterpolationMode = currentMode;
            }
        }

        protected virtual void PaintCore(Graphics g, Rectangle paintRect)
        {
            switch (this.paintMode)
            {
                case ImagePaintMode.None:
                    g.DrawImageUnscaledAndClipped(this.cachedImage, new Rectangle(paintRect.Location, this.cachedImage.Size));
                    break;
                case ImagePaintMode.Center:
                    g.DrawImageUnscaledAndClipped(this.cachedImage, this.CenterRect(this.cachedImage.Size, paintRect));
                    break;
                case ImagePaintMode.Tile:
                case ImagePaintMode.TileFlipX:
                case ImagePaintMode.TileFlipXY:
                case ImagePaintMode.TileFlipY:
                    TextureBrush brush = new TextureBrush(this.cachedImage, this.GetWrapMode());
                    brush.TranslateTransform(paintRect.X, paintRect.Y, MatrixOrder.Prepend);
                    g.FillRectangle(brush, paintRect);
                    brush.Dispose();
                    break;

                case ImagePaintMode.CenterXStretchY:
                    this.PaintSegmented(g, this.CenterRectX(this.cachedImage.Size, paintRect));
                    break;
                case ImagePaintMode.CenterYStretchX:
                    this.PaintSegmented(g, this.CenterRectY(this.cachedImage.Size, paintRect));
                    break;
                case ImagePaintMode.CenterXTileY:
                    this.PaintCenterXTileY(g, paintRect);
                    break;
                case ImagePaintMode.CenterYTileX:
                    this.PaintCenterYTileX(g, paintRect);
                    break;

                case ImagePaintMode.Stretch:
                    this.PaintSegmented(g, paintRect);
                    break;
                case ImagePaintMode.StretchXTileY:
                    this.PaintStretchXTileY(g, paintRect);
                    break;
                case ImagePaintMode.StretchYTileX:
                    this.PaintStretchYTileX(g, paintRect);
                    break;
                case ImagePaintMode.StretchXYTileInner:
                    this.PaintStretchXYTileInner(g, paintRect);
                    break;
            }
        }

        private void PaintStretchXYTileInner(Graphics g, Rectangle paintRect)
        {
            //exclude inner segment from painting to stretch only the borders
            ImageSegments visible = this.visibleSegments;
            this.visibleSegments &= ~ImageSegments.Inner;

            this.PaintSegmented(g, paintRect);

            //restore original segments
            this.visibleSegments = visible;

            if ((this.visibleSegments & ImageSegments.Inner) == 0)
            {
                return;
            }

            RadImageSegment segment = this.GetSegment(ImageSegments.Inner);
            if (segment.ImagePart == null)
            {
                return;
            }

            Padding paintMargins = this.GetPaintMargins(paintRect);

            Rectangle destinationRect = segment.GetDestinationRect(paintRect, paintMargins);
            if (destinationRect.Width <= 0 || destinationRect.Height <= 0)
            {
                return;
            }

            if (segment.RenderMargins != paintMargins)
            {
                segment.UpdateSourceRect(this.cachedImage.Size, paintMargins);
            }

            //tile inner segment
            TextureBrush brush = new TextureBrush(segment.ImagePart, WrapMode.Tile);
            brush.TranslateTransform(destinationRect.X, destinationRect.Y, MatrixOrder.Prepend);
            g.FillRectangle(brush, destinationRect);
            brush.Dispose();
        }

        private void PaintStretchYTileX(Graphics g, Rectangle paintRect)
        {
            Image tempImage = new Bitmap(this.cachedImage.Width, paintRect.Height, PixelFormat.Format32bppArgb);
            Graphics tempGraphics = Graphics.FromImage(tempImage);
            tempGraphics.InterpolationMode = this.interpolationMode;

            this.PaintSegmented(tempGraphics, new Rectangle(0, 0, this.cachedImage.Width, paintRect.Height));

            TextureBrush brush = new TextureBrush(tempImage, WrapMode.Tile);
            brush.TranslateTransform(paintRect.X, paintRect.Y, MatrixOrder.Prepend);
            g.FillRectangle(brush, paintRect);

            tempGraphics.Dispose();
            tempImage.Dispose();
            brush.Dispose();
        }

        private void PaintStretchXTileY(Graphics g, Rectangle paintRect)
        {
            Image tempImage = new Bitmap(paintRect.Width, this.cachedImage.Height, PixelFormat.Format32bppArgb);
            Graphics tempGraphics = Graphics.FromImage(tempImage);
            tempGraphics.InterpolationMode = this.interpolationMode;

            this.PaintSegmented(tempGraphics, new Rectangle(0, 0, paintRect.Width, this.cachedImage.Height));

            TextureBrush brush = new TextureBrush(tempImage, WrapMode.Tile);
            brush.TranslateTransform(paintRect.X, paintRect.Y, MatrixOrder.Prepend);
            g.FillRectangle(brush, paintRect);

            tempGraphics.Dispose();
            tempImage.Dispose();
            brush.Dispose();
        }

        private void PaintSegmented(Graphics g, Rectangle paintRect)
        {
            if (!this.useSegments)
            {
                g.DrawImage(this.cachedImage, paintRect);
                return;
            }

            Padding paintMargins = this.GetPaintMargins(paintRect);

            RadImageSegment segment;
            Rectangle destinationRect;

            int length = this.segments.Length;

            for (int i = 0; i < length; i++)
            {
                segment = this.segments[i];
                if ((segment.Segment & this.visibleSegments) == 0)
                {
                    continue;
                }

                destinationRect = segment.GetDestinationRect(paintRect, paintMargins);
                if (destinationRect.Width <= 0 || destinationRect.Height <= 0)
                {
                    continue;
                }

                if (segment.RenderMargins != paintMargins)
                {
                    segment.UpdateSourceRect(this.cachedImage.Size, paintMargins);
                }

                g.DrawImage(this.cachedImage, destinationRect, segment.SourceRect, GraphicsUnit.Pixel);
            }
        }

        private void PaintCenterXTileY(Graphics g, Rectangle paintRect)
        {
            paintRect = this.CenterRectX(this.cachedImage.Size, paintRect);

            Image tempImage = new Bitmap(paintRect.Width, this.cachedImage.Height, PixelFormat.Format32bppArgb);
            Graphics tempGraphics = Graphics.FromImage(tempImage);
            tempGraphics.InterpolationMode = this.interpolationMode;

            this.PaintSegmented(tempGraphics, new Rectangle(0, 0, paintRect.Width, tempImage.Height));

            TextureBrush brush = new TextureBrush(tempImage, WrapMode.Tile);
            brush.TranslateTransform(paintRect.X, paintRect.Y, MatrixOrder.Prepend);
            g.FillRectangle(brush, paintRect);

            tempGraphics.Dispose();
            tempImage.Dispose();
            brush.Dispose();
        }

        private void PaintCenterYTileX(Graphics g, Rectangle paintRect)
        {
            paintRect = this.CenterRectY(this.cachedImage.Size, paintRect);

            Image tempImage = new Bitmap(this.cachedImage.Width, paintRect.Height, PixelFormat.Format32bppArgb);
            Graphics tempGraphics = Graphics.FromImage(tempImage);
            tempGraphics.InterpolationMode = this.interpolationMode;

            this.PaintSegmented(tempGraphics, new Rectangle(0, 0, this.cachedImage.Width, paintRect.Height));

            TextureBrush brush = new TextureBrush(tempImage, WrapMode.Tile);
            brush.TranslateTransform(paintRect.X, paintRect.Y, MatrixOrder.Prepend);
            g.FillRectangle(brush, paintRect);

            tempGraphics.Dispose();
            tempImage.Dispose();
            brush.Dispose();
        }

        #endregion

        #region Helper Methods

        public void Rotate(int degree)
        {
            if (degree < 0)
            {
                degree = 360 + degree;
            }

            if (degree % 90 > 0)
            {
                throw new ArgumentException("Degree should be divided by 90 without remainder");
            }

            RotateFlipType rotate = this.rotateFlip;

            switch(degree)
            {
                case 90:
                    rotate = RotateFlipType.Rotate90FlipNone;
                    break;
                case 180:
                    rotate = RotateFlipType.Rotate180FlipNone;
                    break;
                case 270:
                    rotate = RotateFlipType.Rotate270FlipNone;
                    break;
            }

            this.Margins = LayoutUtils.RotateMargin(this.margins, degree);
            this.padding = LayoutUtils.RotateMargin(this.padding, degree);
            this.RotateFlip = rotate;
        }

        public static string Serialize(RadImageShape shape)
        {
            if (shape == null)
            {
                return string.Empty;
            }

            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            XmlSerializer serializer = new XmlSerializer(typeof(RadImageShape));
            serializer.Serialize(writer, shape);
            writer.Flush();

            string state = Convert.ToBase64String(stream.ToArray());

            writer.Close();
            stream.Close();

            return state;
        }

        public static RadImageShape Deserialize(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                return null;
            }

            byte[] bytes = Convert.FromBase64String(state);
            MemoryStream stream = new MemoryStream(bytes);
            XmlTextReader reader = new XmlTextReader(stream);
            XmlSerializer serializer = new XmlSerializer(typeof(RadImageShape));

            RadImageShape shape = serializer.Deserialize(reader) as RadImageShape;

            reader.Close();
            stream.Close();

            return shape;
        }

        private Rectangle CenterRectX(Size size, Rectangle rect)
        {
            rect.X += (rect.Width - size.Width) / 2;
            rect.Width = size.Width;

            return rect;
        }

        private Rectangle CenterRectY(Size size, Rectangle rect)
        {
            rect.Y += (rect.Height - size.Height) / 2;
            rect.Height = size.Height;

            return rect;
        }

        private Padding GetPaintMargins(Rectangle paintRect)
        {
            Padding paintMargins = this.margins;

            //clamp by the X axis
            int hExceed = this.margins.Horizontal - paintRect.Width + 1;
            if (hExceed > 0)
            {
                float leftRatio = (float)this.margins.Left / this.margins.Horizontal;
                float rightRatio = (float)this.margins.Right / this.margins.Horizontal;

                int decreaseLeft = (int)(leftRatio * hExceed);
                paintMargins.Left -= decreaseLeft;

                int decreaseRight = (int)(rightRatio * hExceed);
                paintMargins.Right -= decreaseLeft;

                hExceed -= (decreaseLeft + decreaseRight);
                if (hExceed > 0)
                {
                    if (this.margins.Left >= this.margins.Right)
                    {
                        paintMargins.Left -= hExceed;
                    }
                    else
                    {
                        paintMargins.Right -= hExceed;
                    }
                }
            }

            //clamp by the Y axis
            int vExceed = this.margins.Vertical - paintRect.Height;
            if (vExceed > 0)
            {
                float topRatio = (float)this.margins.Top / this.margins.Vertical;
                float bottomRatio = (float)this.margins.Bottom / this.margins.Vertical;

                int decreaseTop = (int)(topRatio * vExceed);
                paintMargins.Top -= decreaseTop;

                int decreaseBottom = (int)(bottomRatio * vExceed);
                paintMargins.Bottom -= decreaseBottom;

                vExceed -= (decreaseTop + decreaseBottom);
                if (vExceed > 0)
                {
                    if (this.margins.Top >= this.margins.Bottom)
                    {
                        paintMargins.Top -= vExceed;
                    }
                    else
                    {
                        paintMargins.Bottom -= vExceed;
                    }
                }
            }

            return paintMargins;
        }

        private Rectangle CenterRect(Size size, Rectangle rect)
        {
            int offsetX = (rect.Width - size.Width) / 2;
            int offsetY = (rect.Height - size.Height) / 2;

            return new Rectangle(rect.X + offsetX, rect.Y + offsetY, size.Width, size.Height);
        }

        private WrapMode GetWrapMode()
        {
            WrapMode mode = WrapMode.Clamp;
            switch(this.paintMode)
            {
                case ImagePaintMode.Tile:
                    mode = WrapMode.Tile;
                    break;
                case ImagePaintMode.TileFlipX:
                    mode = WrapMode.TileFlipX;
                    break;
                case ImagePaintMode.TileFlipXY:
                    mode = WrapMode.TileFlipXY;
                    break;
                case ImagePaintMode.TileFlipY:
                    mode = WrapMode.TileFlipY;
                    break;
            }

            return mode;
        }

        private void ResetImageCache()
        {
            if (this.cachedImage != null)
            {
                this.cachedImage.Dispose();
                this.cachedImage = null;
            }

            this.CacheImage();
            this.imageDirty = false;
            this.segmentsDirty = true;
        }

        private void ResetSegments()
        {
            this.DisposeSegments();
            this.BuildSegments();

            this.segmentsDirty = false;
        }

        private void CacheImage()
        {
            if (this.image == null)
            {
                return;
            }

            this.cachedImage = this.image.Clone() as Image;
            Bitmap bitmap = this.cachedImage as Bitmap;

            if (bitmap != null)
            {
                if (this.alpha < 1F)
                {
                    ImageHelper.ApplyAlpha(bitmap, this.alpha);
                }

                if (this.rotateFlip != RotateFlipType.RotateNoneFlipNone)
                {
                    bitmap.RotateFlip(this.rotateFlip);
                }
            }
        }

        private void BuildSegments()
        {
            if (this.cachedImage == null || !this.useSegments)
            {
                return;
            }

            this.segments = new RadImageSegment[9];
            for (int i = 0; i < 9; i++)
            {
                this.segments[i] = new RadImageSegment((ImageSegments)Math.Pow(2, i));
            }

            this.UpdateSegments();
        }

        private void UpdateSegments()
        {
            int length = this.segments.Length;
            for (int i = 0; i < length; i++)
            {
                this.segments[i].UpdateFromImage(this.cachedImage, this.margins);
            }
        }

        private void DisposeSegments()
        {
            if (this.segments == null)
            {
                return;
            }

            int length = this.segments.Length;
            for (int i = 0; i < length; i++)
            {
                this.segments[i].Dispose();
            }

            this.segments = null;
        }

        #endregion
    }
}
