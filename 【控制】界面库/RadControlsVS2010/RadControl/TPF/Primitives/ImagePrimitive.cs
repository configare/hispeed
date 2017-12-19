using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using Telerik.WinControls.Enumerations;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Telerik.WinControls.Primitives
{
	/// <summary>Represents an image that is drawn on the screen.</summary>
	public class ImagePrimitive : BasePrimitive, IImageElement
    {
        #region BitState Keys

        internal const ulong ImageInvalidatedStateKey = BasePrimitiveLastStateKey << 1;
        internal const ulong IsImageUpdatingStateKey = ImageInvalidatedStateKey << 1;
        internal const ulong IsImageListUpdatingStateKey = IsImageUpdatingStateKey << 1;
        internal const ulong IsSetFromImageListStateKey = IsImageListUpdatingStateKey << 1;
        internal const ulong CurrentlyAnimatingStateKey = IsSetFromImageListStateKey << 1;
        internal const ulong ImageClonedStateKey = CurrentlyAnimatingStateKey << 1;

        internal const ulong ImagePrimitiveLastStateKey = ImageClonedStateKey;

        #endregion

        #region Fields

        private RotateFlipType rotateFlip = RotateFlipType.RotateNoneFlipNone;
        private Image cachedImage = null;

        #endregion

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.Auto;
            this.StretchHorizontally = false;
            this.StretchVertically = false;
            this.BitState[ImageInvalidatedStateKey] = true;
        }

        public override Filter GetStylablePropertiesFilter()
        {
            return Telerik.WinControls.PropertyFilter.ImagePrimitiveFilter;
        }

		#region Properties

		#region Dependency Properties
		public static RadProperty ImageProperty = RadProperty.Register(
			"Image", typeof(Image), typeof(ImagePrimitive), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

		public static RadProperty ImageIndexProperty = RadProperty.Register(
			"ImageIndex", typeof(int), typeof(ImagePrimitive), new RadElementPropertyMetadata(
				-1, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageKeyProperty = RadProperty.Register(
			"ImageKey", typeof(string), typeof(ImagePrimitive), new RadElementPropertyMetadata(
				string.Empty, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageScalingProperty = RadProperty.Register(
			"ImageScaling", typeof(ImageScaling), typeof(ImagePrimitive), new RadElementPropertyMetadata(
				ImageScaling.None, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

		public static RadProperty TransparentColorProperty = RadProperty.Register(
			"TransparentColor", typeof(Color), typeof(ImagePrimitive), new RadElementPropertyMetadata(
				Color.FromArgb(255, 0, 255), ElementPropertyOptions.AffectsDisplay));

		public static RadProperty ScaleSizeProperty = RadProperty.Register(
            "ScaleSize", typeof(Size), typeof(ImagePrimitive), 
            new RadElementPropertyMetadata(new Size(16, 16), ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty UseSmallImageListProperty = RadProperty.Register(
			"UseSmallImageList", typeof(bool), typeof(ImagePrimitive), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.None));
		#endregion

		/// <summary>
		/// Gets or sets the flag controlling whether the image primitive fills up the available area horizontally
		/// </summary>
		[DefaultValue(false)]
		public override bool StretchHorizontally
		{
			get { return base.StretchHorizontally; }
			set { base.StretchHorizontally = value; }
		}

		/// <summary>
		/// Gets or sets the flag controlling whether the image primitive fills up the available area vertically
		/// </summary>
		[DefaultValue(false)]
		public override bool StretchVertically
		{
			get { return base.StretchVertically; }
			set { base.StretchVertically = value; }
		}

		public static RadProperty ImageLayoutProperty = RadProperty.Register(
            "ImageLayout", typeof(ImageLayout), typeof(ImagePrimitive), 
            new RadElementPropertyMetadata(ImageLayout.Center, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets the way ImagePrimitive will layout its image on the screen.
        /// </summary>
		public ImageLayout ImageLayout
		{
			get
			{
				return (ImageLayout)this.GetValue(ImageLayoutProperty);
			}
			set
			{
				this.SetValue(ImageLayoutProperty, value);
			}
		}

		/// <summary>
        /// Gets or sets the desired size to be used when displaying the image. Works when ImageScalingMode is set to FitToSize.
		/// </summary>
        [Description("Gets or sets the desired size to be used when displaying the image. Works when ImageScalingMode.FitToSize is applied.")]
		public Size ScaleSize
		{
			get
			{
				return (Size)this.GetValue(ScaleSizeProperty);
			}
			set
			{
                this.SetValue(ScaleSizeProperty, value);
			}
		}

		/// <summary>Gets or sets the image that is displayed.</summary>
		[RadPropertyDefaultValue("Image", typeof(ImagePrimitive)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the image that is displayed."),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
		public Image Image
		{
			get
			{
                return (Image)this.GetValue(ImageProperty);
			}
			set
			{
				this.SetValue(ImageProperty, value);
			}
		}

        /// <summary>
        /// Gets the Image that will be painted on the screen, with settings such as Opacity and Flip applied.
        /// </summary>
        [Browsable(false)]
        public Image RenderImage
        {
            get
            {
                return this.GetPaintImage();
            }
        }

        private void BuildImageCache(Image image)
        {
            if (this.cachedImage != null && this.BitState[ImageClonedStateKey])
            {
                this.cachedImage.Dispose();
                this.cachedImage = null;
            }

            this.BitState[ImageClonedStateKey] = false;
            this.cachedImage = image;
            if (this.cachedImage == null || ImageAnimator.CanAnimate(this.cachedImage))
            {
                return;
            }

            Bitmap cached = this.cachedImage.Clone() as Bitmap;
            Debug.Assert(cached != null, "Failed to clone image.");
            if (cached != null)
            {
                cached.MakeTransparent(this.TransparentColor);
                if (this.rotateFlip != RotateFlipType.RotateNoneFlipNone)
                {
                    cached.RotateFlip(this.rotateFlip);
                }
                this.cachedImage = cached;
                this.BitState[ImageClonedStateKey] = true;
            }
        }

		/// <summary>Gets or sets the image list index value of the displayed image.</summary>
		[RadPropertyDefaultValue("ImageIndex", typeof(ImagePrimitive)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the image list index value of the displayed image."),
		RefreshProperties(RefreshProperties.All),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		Editor(DesignerConsts.ThemeNameEditorString, typeof(UITypeEditor))]
		public int ImageIndex
		{
			get
			{
				return (int)this.GetValue(ImageIndexProperty);
			}
			set
			{
				this.SetValue(ImageIndexProperty, value);
			}
		}

		/// <summary>Gets or sets the key accessor for the image in the ImageList.</summary>
		[RadPropertyDefaultValue("ImageKey", typeof(ImagePrimitive))]
		[Description("Gets or sets the key accessor for the image in the ImageList."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RelatedImageList("ElementTree.Control.ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
		public string ImageKey
		{
			get
			{
				return (string)this.GetValue(ImageKeyProperty);
			}
			set
			{
				this.SetValue(ImageKeyProperty, value);
			}
		}

        /// <summary>Specifies whether the image should be taken from the SmallImageList or from the ImageList when the ImageIndex/ImageKey property is set.</summary>
        [RadPropertyDefaultValue("UseSmallImageList", typeof(ImagePrimitive)),
        Category("Image")]
        public bool UseSmallImageList
        {
            get
            {
                return (bool)this.GetValue(UseSmallImageListProperty);
            }
            set
            {
                this.SetValue(UseSmallImageListProperty, value);
            }
        }

		/// <summary>
		/// Gets or sets image scaling. Possible values are members of ImageScaling
		/// enumeration: None and SizeToFit.
		/// </summary>
		[RadPropertyDefaultValue("ImageScaling", typeof(ImagePrimitive)),
		Category("Image"), //Appearance
		Description("ToolStripItemImageScalingDescr")]
		public ImageScaling ImageScaling
		{
			get
			{
				return (ImageScaling)GetValue(ImageScalingProperty);
			}
			set
			{
				this.SetValue(ImageScalingProperty, value);
			}
		}

		/// <summary>Gets actual index.</summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int ActualIndex
		{
			get
			{
				if (this.ImageIndex >= 0)
				{
					return this.ImageIndex;
				}
                if ((this.ElementTree != null) && (this.ElementTree.ComponentTreeHandler.ImageList != null))
				{
                    return this.ElementTree.ComponentTreeHandler.ImageList.Images.IndexOfKey(this.ImageKey);
				}
				return -1;
			}
		}

		private ImageList ImageList
		{
			get
			{
				if (this.ElementTree == null)
					return null;

				if (this.UseSmallImageList)
					return this.ElementTree.ComponentTreeHandler.SmallImageList;
				else
                    return this.ElementTree.ComponentTreeHandler.ImageList;
			}
		}

        private Size GetImageSize()
        {
            if (this.ImageScaling == ImageScaling.SizeToFit)
            {
                return this.ScaleSize;
            }

            Image image = this.Image;
            return image == null ? Size.Empty : image.Size;
        }

		/// <summary><para>Gets a value indicating whether the primitive has content.</para></summary>
        [Browsable(false)]
		public override bool IsEmpty
		{
			get
			{
				return this.Image == null;
			}
		}

		private bool IsImageListSet
		{
			get
			{
				bool isIndexSet = (this.ImageIndex >= 0) || (this.ImageKey != string.Empty);
				bool isImageListSet = (this.ImageList != null);
				return (isIndexSet && isImageListSet);
			}
		}

		#endregion

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.UpdateImage();
        }

        protected override void OnUnloaded(ComponentThemableElementTree oldTree)
        {
            base.OnUnloaded(oldTree);
            this.UpdateImage();
        }

        private void UpdateImage()
        {
            if (this.GetBitState(IsImageUpdatingStateKey))
            {
                return;
            }

            this.BitState[IsImageListUpdatingStateKey] = true;
            //if the image list is set to null (from a valid value), set the image to null
            if (!this.IsImageListSet)
            {
                if (this.GetBitState(IsSetFromImageListStateKey))
                {
                    this.SetValue(ImageProperty, null);
                }
            }
            //set the image property to the referenced image from the image list
            else
            {
                if (this.ImageIndex >= 0 && this.ImageIndex < this.ImageList.Images.Count)
                {
                    this.SetValue(ImageProperty, new Bitmap(this.ImageList.Images[this.ImageIndex]));
                    this.BitState[IsSetFromImageListStateKey] = true;
                }
                else if (!string.IsNullOrEmpty(this.ImageKey) && this.ImageList.Images.IndexOfKey(this.ImageKey) >= 0)
                {
                    this.SetValue(ImageProperty, new Bitmap(this.ImageList.Images[this.ImageKey]));
                    this.BitState[IsSetFromImageListStateKey] = true;
                }
            }

            this.BitState[IsImageListUpdatingStateKey] = false;

            //make the image as invalid
            this.BitState[ImageInvalidatedStateKey] = true;
        }

        protected override void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnTunnelEvent(sender, args);

            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }

            if (args.RoutedEvent == RootRadElement.OnRoutedImageListChanged ||
                    args.RoutedEvent == RadElement.ParentChangedEvent)
            {
                this.UpdateImage();

                if (this.UseNewLayoutSystem)
                {
                    this.InvalidateMeasure();
                    this.InvalidateArrange();
                    if (this.Parent != null)
                    {
                        this.Parent.InvalidateMeasure();
                    }
                }
                else
                {
                    this.LayoutEngine.InvalidateLayout();
                }
            }
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
            if (e.Property == ImagePrimitive.ImageIndexProperty && !this.GetBitState(IsImageUpdatingStateKey))
			{
                this.BitState[ImageInvalidatedStateKey] = true;
				this.BitState[IsImageUpdatingStateKey] = true;
                int imageIndex = (int)e.NewValue;
				//
                if (imageIndex >= 0 && this.ImageList != null &&
                    imageIndex < this.ImageList.Images.Count)
                {
                    this.SetValue(ImageProperty, new Bitmap(this.ImageList.Images[imageIndex]));
                    this.BitState[IsSetFromImageListStateKey] = true;
                }
                else
                {
                    this.ResetValue(ImageProperty, ValueResetFlags.Local);
                }

                this.ResetValue(ImageKeyProperty, ValueResetFlags.Local);
                this.BitState[IsImageUpdatingStateKey] = false;
			}
			if (e.Property == ImagePrimitive.ImageKeyProperty && !this.GetBitState(IsImageUpdatingStateKey))
			{
                this.BitState[ImageInvalidatedStateKey] = true;
                this.BitState[IsImageUpdatingStateKey] = true;
                string imageKey = (string)e.NewValue;

                if (!string.IsNullOrEmpty(imageKey) && this.ImageList != null &&
                    this.ImageList.Images.IndexOfKey(imageKey) >= 0)
                {
                    this.SetValue(ImageProperty, new Bitmap(this.ImageList.Images[imageKey]));
                    this.BitState[IsSetFromImageListStateKey] = true;
                }
                else
                {
                    this.ResetValue(ImageProperty, ValueResetFlags.Local);
                }

                this.ResetValue(ImageIndexProperty, ValueResetFlags.Local);
                this.BitState[IsImageUpdatingStateKey] = false;
			}
			if (e.Property == ImageProperty && !this.GetBitState(IsImageUpdatingStateKey) && !this.GetBitState(IsImageListUpdatingStateKey))
			{
                this.BitState[ImageInvalidatedStateKey] = true;
                this.BitState[IsImageUpdatingStateKey] = true;
                this.BitState[IsSetFromImageListStateKey] = false;

                this.ResetValue(ImageIndexProperty, ValueResetFlags.Local);
                this.ResetValue(ImageKeyProperty, ValueResetFlags.Local);
			    this.BitState[CurrentlyAnimatingStateKey] = false;
                ImageAnimator.StopAnimate(Image, this.OnFrameChanged);
                this.BitState[IsImageUpdatingStateKey] = false;
			}

			if (e.Property == TransparentColorProperty || e.Property == OpacityProperty)
			{
                this.BitState[ImageInvalidatedStateKey] = true;
			}

			base.OnPropertyChanged(e);
		}

		/// <summary>
		/// Gets or sets the transparent color for the image
		/// </summary>
		[RadPropertyDefaultValue("TransparentColor", typeof(ImagePrimitive)), Category("Image")]
		[Description("Transparent color to be used on the image")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(RadColorEditorConverter))]
		public Color TransparentColor
		{
			get
			{
				return (Color)this.GetValue(TransparentColorProperty);
			}
			set
			{
				this.SetValue(TransparentColorProperty, value);
			}
		}

		/// <summary>
		/// 	<para>Retrieves the size of a rectangular area into which a primitive can be
		///     fitted. If the proposed size is too small, the minimal size is retrieved.</para>
		/// </summary>
		public override Size GetPreferredSizeCore(Size proposedSize)
		{
            Size sz = this.GetImageSize();
			return sz;
		}

		protected override SizeF MeasureOverride(SizeF availableSize)
		{
			base.MeasureOverride(availableSize);
            Size desired = this.GetImageSize();
            Padding padding = this.Padding;

            desired.Width += padding.Horizontal;
            desired.Height += padding.Vertical;

            return desired;
		}

        protected override void DisposeManagedResources()
        {
            if (this.cachedImage != null)
            {
                if (ImageAnimator.CanAnimate(this.cachedImage))
                {
                    ImageAnimator.StopAnimate(this.cachedImage, OnFrameChanged);
                }
                else if (this.BitState[ImageClonedStateKey])
                {
                    this.cachedImage.Dispose();
                }
                this.cachedImage = null;
            }

            base.DisposeManagedResources();
        }
		
		/// <summary>
		/// Gets or sets the type of rotate/flip to be applied.
		/// </summary>
        [DefaultValue(RotateFlipType.RotateNoneFlipNone)]
        [Description("Gets or sets the type of rotate/flip to be applied.")]
		public RotateFlipType RotateFlipType
		{
			get
			{
				return this.rotateFlip;
			}
			set
			{
				if (this.rotateFlip != value)
				{
					this.rotateFlip = value;
                    this.BitState[ImageInvalidatedStateKey] = true;
                    this.Invalidate();
				}
			}
		}

        //This method begins the animation.
        private void AnimateImage( Image image )
        {
            if (ImageAnimator.CanAnimate(image))
            {
                if (!this.GetBitState(CurrentlyAnimatingStateKey))
                {
                    //Begin the animation only once.
                    ImageAnimator.Animate(image, this.OnFrameChanged);
                    this.BitState[CurrentlyAnimatingStateKey] = true;
                }
            }
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            //Force a call to the Paint event handler.
            this.Invalidate();
        }

		/// <summary>Draws the primitive on the screen.</summary>
		public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
		{
            Image image = this.GetPaintImage();
			if (image == null)
			{
				return;
			}

            Rectangle imageBounds = this.GetImageBounds(image);
            if (imageBounds.Width <= 0 || imageBounds.Height <= 0)
            {
                return;
            }

            //TODO: do not obtain raw graphics here, refactor RadGdiGraphics image methods instead.
            Graphics rawGraphics = graphics.UnderlayGraphics as Graphics;

            if (graphics.Opacity < 1)
            {
                this.PaintWithOpacity(rawGraphics, image, imageBounds, (float)graphics.Opacity);
            }
            else
            {
                switch (this.ImageLayout)
                {
                    case ImageLayout.None:
                        rawGraphics.DrawImageUnscaledAndClipped(image, imageBounds);
                        break;
                    case ImageLayout.Tile:
                        TextureBrush brush = new TextureBrush(image, WrapMode.Tile);
                        rawGraphics.FillRectangle(brush, imageBounds);
                        brush.Dispose();
                        break;
                    default:
                        rawGraphics.DrawImage(image, imageBounds);
                        break;
                }
            }
		}

        private void PaintWithOpacity(Graphics rawGraphics, Image image, Rectangle bounds, float opacity)
        {
            ImageAttributes attributes = RadGdiGraphics.GetOpacityAttributes(opacity);
            int srcWidth = image.Width;
            int srcHeight = image.Height;

            switch (this.ImageLayout)
            {
                case ImageLayout.None:
                    srcWidth = bounds.Width;
                    srcHeight = bounds.Height;
                    rawGraphics.DrawImage(image, bounds, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel, attributes);
                    break;
                case ImageLayout.Tile:
                    for (int y = bounds.X; y <= bounds.Height; y += image.Height)
                    {
                        for (int x = bounds.Y; x <= bounds.Width; x += image.Width)
                        {
                            Rectangle destRect = new Rectangle(x, y, srcWidth, srcHeight);
                            rawGraphics.DrawImage(image, destRect, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel, attributes);
                        }
                    }
                    break;
                default:
                    rawGraphics.DrawImage(image, bounds, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel, attributes);
                    break;
            }

            attributes.Dispose();
        }

        private Image GetPaintImage()
        {
            Image image = this.Image;
            if (image == null)
            {
                return null;
            }

            Image paintImage;

            if (ImageAnimator.CanAnimate(image))
            {
                this.AnimateImage(image);
                ImageAnimator.UpdateFrames();
                paintImage = image;
            }
            else
            {
                if (this.BitState[ImageInvalidatedStateKey])
                {
                    this.BuildImageCache(image);
                    this.BitState[ImageInvalidatedStateKey] = false;
                }
                paintImage = this.cachedImage;
            }

            return paintImage;
        }

        private Rectangle GetImageBounds(Image image)
        {
            Size imageSize = this.GetImageSize();
            Rectangle bounds = this.Bounds;

            Padding padding = this.Padding;
            int left = padding.Left;
            int top = padding.Top;
            int width = bounds.Width - padding.Horizontal;
            int height = bounds.Height - padding.Vertical;

            switch(this.ImageLayout)
            {
                case ImageLayout.None:
                    width = Math.Min(width, imageSize.Width);
                    height = Math.Min(height, imageSize.Height);
                    break;
                case ImageLayout.Center:
                    left += (width - imageSize.Width) / 2;
                    top += (height - imageSize.Height) / 2;
                    break;
                case ImageLayout.Zoom:
                    float scaleX = width / (float)imageSize.Width;
                    float scaleY = height / (float)imageSize.Height;
                    float ratio = Math.Min(scaleX, scaleY);

                    int scaledWidth = (int)(imageSize.Width * ratio + .5F);
                    int scaledHeight = (int)(imageSize.Height * ratio + .5F);
                    left += (width - scaledWidth) / 2;
                    top += (height - scaledHeight) / 2;

                    width = scaledWidth;
                    height = scaledHeight;
                    break;
            }

            return new Rectangle(left, top, width, height);
        }
    }
}
