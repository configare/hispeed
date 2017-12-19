using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Design;
using Telerik.WinControls.Layouts;
using System.Drawing.Text;
using Telerik.WinControls.TextPrimitiveUtils;
using Telerik.WinControls.Themes;

namespace Telerik.WinControls.UI
{
    public class LightVisualElement : UIItemBase, ITextPrimitive, IImageElement
    {
        #region BitState Keys
        internal const ulong CurrentlyAnimatingStateKey = UIItemBaseLastStateKey << 1;
        internal const ulong CurrentlyAnimatingBackGroundImageStateKey = CurrentlyAnimatingStateKey << 1;
        protected internal const ulong UseHTMLRenderingStateKey = CurrentlyAnimatingBackGroundImageStateKey << 1;
        internal const ulong DisableHTMLRenderingStateKey = UseHTMLRenderingStateKey << 1;
        internal const ulong textWrapStateKey = DisableHTMLRenderingStateKey << 1;
        internal const ulong showKeyboardCuesStateKey = textWrapStateKey << 1;
        internal const ulong measureTrailingSpacesStateKey = showKeyboardCuesStateKey << 1;
        internal const ulong useMnemonicStateKey = measureTrailingSpacesStateKey << 1;
        internal const ulong autoEllipsisStateKey = useMnemonicStateKey << 1;

        internal const ulong LightVisualElementLastStateKey = autoEllipsisStateKey;

        #endregion

        #region Static Members

        private static readonly Dictionary<RadProperty, RadProperty> mappedPrimitiveProperties;

        /// <summary>
        /// Gets the properties, which should mapped if set to a LightVisualElement instance. Used for testing purposes.
        /// </summary>
        internal static Dictionary<RadProperty, RadProperty> PropertiesForMapping
        {
            get
            {
                return mappedPrimitiveProperties;
            }
        }

        #endregion

        #region Fields

        private Image cachedImage = null;
        private Image cachedBackgroundImage = null;
        private StringFormat paintTextFormat;
        private ImagePart imageElement;
        private TextPart textElement;
        protected FormattedTextBlock textBlock = new FormattedTextBlock();
        internal protected LayoutManagerPart layoutManagerPart;
        internal ITextPrimitive textPrimitiveImpl;

        #endregion

        #region Constructor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.CanFocus = true;
            this.ShouldPaint = true;
            this.layoutManagerPart = new LayoutManagerPart(this);
            this.imageElement = new ImagePart(this);
            this.textElement = new TextPart(this);
            this.layoutManagerPart.LeftPart = this.imageElement;
            this.layoutManagerPart.RightPart = this.textElement;
            this.textPrimitiveImpl = new TextPrimitiveImpl();
        }

        static LightVisualElement()
        {
            RadTypeResolver.Instance.RegisterKnownType("Telerik.WinControls.UI.LightVisualElement", typeof(LightVisualElement));
            TextProperty.OverrideMetadata(typeof(LightVisualElement),
                                          new RadElementPropertyMetadata(string.Empty,
                                                                         ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

            //create mapped properties. This allows properties from fill primitives to be ustilized in the element itself
            mappedPrimitiveProperties = new Dictionary<RadProperty, RadProperty>(20);
            AddFillPrimitiveProperties();
            AddBorderPrimitiveProperties();
            AddImagePrimitiveProperties();
        }

        private static void AddImagePrimitiveProperties()
        {
            mappedPrimitiveProperties.Add(ImagePrimitive.ImageProperty, LightVisualElement.ImageProperty);
            mappedPrimitiveProperties.Add(ImagePrimitive.ImageLayoutProperty, LightVisualElement.ImageLayoutProperty);
        }

        private static void AddFillPrimitiveProperties()
        {
            mappedPrimitiveProperties.Add(FillPrimitive.BackColor2Property, LightVisualElement.BackColor2Property);
            mappedPrimitiveProperties.Add(FillPrimitive.BackColor3Property, LightVisualElement.BackColor3Property);
            mappedPrimitiveProperties.Add(FillPrimitive.BackColor4Property, LightVisualElement.BackColor4Property);
            mappedPrimitiveProperties.Add(FillPrimitive.NumberOfColorsProperty, LightVisualElement.NumberOfColorsProperty);
            mappedPrimitiveProperties.Add(FillPrimitive.GradientStyleProperty, LightVisualElement.GradientStyleProperty);
            mappedPrimitiveProperties.Add(FillPrimitive.GradientAngleProperty, LightVisualElement.GradientAngleProperty);
            mappedPrimitiveProperties.Add(FillPrimitive.GradientPercentageProperty, LightVisualElement.GradientPercentageProperty);
            mappedPrimitiveProperties.Add(FillPrimitive.GradientPercentage2Property, LightVisualElement.GradientPercentage2Property);
        }

        private static void AddBorderPrimitiveProperties()
        {
            mappedPrimitiveProperties.Add(BorderPrimitive.BorderBoxStyleProperty, LightVisualElement.BorderBoxStyleProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.BorderDrawModeProperty, LightVisualElement.BorderDrawModeProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.WidthProperty, LightVisualElement.BorderWidthProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.LeftWidthProperty, LightVisualElement.BorderLeftWidthProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.TopWidthProperty, LightVisualElement.BorderTopWidthProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.RightWidthProperty, LightVisualElement.BorderRightWidthProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.BottomWidthProperty, LightVisualElement.BorderBottomWidthProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.LeftColorProperty, LightVisualElement.BorderLeftColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.TopColorProperty, LightVisualElement.BorderTopColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.RightColorProperty, LightVisualElement.BorderRightColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.BottomColorProperty, LightVisualElement.BorderBottomColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.LeftShadowColorProperty, LightVisualElement.BorderLeftShadowColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.TopShadowColorProperty, LightVisualElement.BorderTopShadowColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.RightShadowColorProperty, LightVisualElement.BorderRightShadowColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.BottomShadowColorProperty, LightVisualElement.BorderBottomShadowColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.GradientAngleProperty, LightVisualElement.BorderGradientAngleProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.GradientStyleProperty, LightVisualElement.BorderGradientStyleProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.ForeColor2Property, LightVisualElement.BorderColor2Property);
            mappedPrimitiveProperties.Add(BorderPrimitive.ForeColor3Property, LightVisualElement.BorderColor3Property);
            mappedPrimitiveProperties.Add(BorderPrimitive.ForeColor4Property, LightVisualElement.BorderColor4Property);
            mappedPrimitiveProperties.Add(BorderPrimitive.InnerColorProperty, LightVisualElement.BorderInnerColorProperty);
            mappedPrimitiveProperties.Add(BorderPrimitive.InnerColor2Property, LightVisualElement.BorderInnerColor2Property);
            mappedPrimitiveProperties.Add(BorderPrimitive.InnerColor3Property, LightVisualElement.BorderInnerColor3Property);
            mappedPrimitiveProperties.Add(BorderPrimitive.InnerColor4Property, LightVisualElement.BorderInnerColor4Property);
        }

        #endregion

        #region Overrides

        internal override RadProperty MapStyleProperty(RadProperty propertyToMap, string settingType)
        {
            if (propertyToMap == VisualElement.ForeColorProperty && settingType == RepositoryItemTypes.Border)
            {
                return LightVisualElement.BorderColorProperty;
            }

            RadProperty result;
            LightVisualElement.PropertiesForMapping.TryGetValue(propertyToMap, out result);

            return result;
        }

        public override Filter GetStylablePropertiesFilter()
        {
            PropertyFilter lvElementFilter = new PropertyFilter(Telerik.WinControls.PropertyFilter.GetPropertiesDeclaredByType(typeof(LightVisualElement)));
            return new OrFilter(lvElementFilter, 
                                Telerik.WinControls.PropertyFilter.AppearanceFilter,
                                Telerik.WinControls.PropertyFilter.BehaviorFilter, 
                                Telerik.WinControls.PropertyFilter.LayoutFilter);
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);
           
            if (key == DisableHTMLRenderingStateKey)
            {
                this.BitState[UseHTMLRenderingStateKey] = TinyHTMLParsers.IsHTMLMode(this.Text);
                this.textPrimitiveImpl = TextPrimitiveFactory.CreateTextPrimitiveImp(this.GetBitState(UseHTMLRenderingStateKey));
                if (this.GetBitState(UseHTMLRenderingStateKey))
                {
                    this.layoutManagerPart.Measure(new SizeF(float.MaxValue, float.MaxValue));
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            if (this.paintTextFormat != null)
            {
                this.paintTextFormat.Dispose();
                this.paintTextFormat = null;
            }
            if (this.cachedImage != null)
            {
                if (this.ImageIndex != -1 || this.ImageKey != string.Empty)
                {
                    this.cachedImage.Dispose();
                }
                this.cachedImage = null;
            }

            base.DisposeManagedResources();
        }

        public override float GetPaintingBorderWidth()
        {
            return this.BorderWidth;
        }

        #endregion

        #region Painting

        protected override RectangleF GetClipRect()
        {
            Size size = this.Bounds.Size;

            Padding border = this.GetBorderThickness(true);
            Padding padding = this.GetClipPadding();

            RectangleF client = new RectangleF(
                border.Left + padding.Left,
                border.Top + padding.Top,
                size.Width - border.Horizontal - padding.Horizontal,
                size.Height - border.Vertical - padding.Vertical);

            return client;
        }

        private Padding GetClipPadding()
        {
            Padding padding = this.Padding;
            if (padding.Left > 0)
            {
                padding.Left--;
            }
            if (padding.Top > 0)
            {
                padding.Top--;
            }
            if (padding.Right > 0)
            {
                padding.Right--;
            }
            if (padding.Bottom > 0)
            {
                padding.Bottom--;
            }

            return padding;
        }

        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            if (DrawFill)
            {
                this.PaintFill(graphics, angle, scale);
            }

            this.PaintContent(graphics);

            if (DrawBorder)
            {
                this.PaintBorder(graphics, angle, scale);
            }
        }

        protected virtual void PaintText(IGraphics graphics)
        {
            if (string.IsNullOrEmpty(this.Text) ||!this.DrawText)
            {
                if (ShowHorizontalLine)
                {
                    this.DrawHorizontalLineWithoutText(graphics);
                }

                return;
            }

            if (ShowHorizontalLine)
            {
                //Graphics g = TelerikPaintHelper.CreateMeasurementGraphics();
                this.DrawHorizontalLine(graphics);
            }

            this.PaintTextCore(graphics);
        }

        private void PaintTextCore(IGraphics graphics)
        {
            TextParams textParams = this.CreateTextParams();
            this.textPrimitiveImpl.PaintPrimitive(graphics, this.AngleTransform, this.ScaleTransform, textParams);
        }

        protected virtual void DrawHorizontalLineWithoutText(IGraphics graphics)
        {
            Graphics g = (Graphics)graphics.UnderlayGraphics;
            using (Pen pen = new Pen(this.HorizontalLineColor, this.HorizontalLineWidth))
            {
                g.DrawLine(pen, 0, this.Size.Height / 2, this.Size.Width, this.Size.Height / 2);
            }
        }

        protected virtual void DrawHorizontalLine(IGraphics graphics)
        {
            SizeF size = this.textElement.DesiredSize;
            int x = 0;
            int y = this.Size.Height / 2;
            int x2 = 0;
            int y2 = y;

            ContentAlignment textAlignment = this.GetTextAlignment();

            if (textAlignment == ContentAlignment.MiddleLeft ||
                textAlignment == ContentAlignment.TopLeft ||
                textAlignment == ContentAlignment.BottomLeft)
            {
                x = (int)size.Width + 10;
                x2 = this.Size.Width - 2;
            }
            else if (textAlignment == ContentAlignment.MiddleRight ||
                     textAlignment == ContentAlignment.TopRight ||
                     textAlignment == ContentAlignment.BottomRight)
            {
                x = 1;
                x2 = this.Size.Width - 2 - (int)size.Width - 10;
            }
            else if (textAlignment == ContentAlignment.MiddleCenter ||
                     textAlignment == ContentAlignment.TopCenter ||
                     textAlignment == ContentAlignment.BottomCenter)
            {
                x = 1;
                x2 = this.Size.Width / 2 - (int)size.Width / 2 - 10;
            }

            if (x < x2)
            {
                Graphics g = (Graphics)graphics.UnderlayGraphics;
                using (Pen pen = new Pen(this.HorizontalLineColor, this.HorizontalLineWidth))
                {
                    g.DrawLine(pen, x, y, x2, y2);
                    if (this.TextAlignment == ContentAlignment.MiddleCenter ||
                        this.TextAlignment == ContentAlignment.TopCenter ||
                        this.TextAlignment == ContentAlignment.BottomCenter)
                    {
                        x = (this.Size.Width / 2) + (int)size.Width / 2 + 10;
                        x2 = this.Size.Width - 2;
                        g.DrawLine(pen, x, y, x2, y2);
                    }
                }
            }
        }

        protected virtual void PaintImage(IGraphics graphics)
        {
            if (cachedImage != null)
            {
                Image image = cachedImage;
                this.AnimateImage(image, false);
                ImageAnimator.UpdateFrames();
                SizeF size = this.imageElement.Bounds.Size;
                if (size == SizeF.Empty)
                {
                    return;
                }

                ContentAlignment imageAlignment = this.ImageAlignment;
                if (this.RightToLeft)
                {
                    imageAlignment = TelerikAlignHelper.RtlTranslateContent(imageAlignment);
                }

                switch (this.ImageLayout)
                {
                    case ImageLayout.None:
                        Graphics rawGraphics2 = (Graphics)graphics.UnderlayGraphics;

                        SizeF sz2 = new SizeF(
                            Math.Min(this.imageElement.Bounds.Width, image.Size.Width),
                            Math.Min(this.imageElement.Bounds.Height, image.Size.Height));

                        RectangleF aligmentImageBounds = LayoutUtils.Align(sz2, this.imageElement.Bounds, imageAlignment);

                        rawGraphics2.DrawImageUnscaledAndClipped(image, new Rectangle(new Point((int)aligmentImageBounds.Location.X, (int)aligmentImageBounds.Location.Y), aligmentImageBounds.Size.ToSize()));
                        break;

                    case ImageLayout.Center:

                        float centerX = this.imageElement.Bounds.X + Math.Max(0, (size.Width - image.Width) / 2);
                        float centerY = this.imageElement.Bounds.Y + Math.Max(0, (size.Height - image.Height) / 2);

                        PointF pos = new PointF(Math.Max(0, centerX), Math.Max(0, centerY));
                        pos.X = Math.Min(pos.X, this.imageElement.Bounds.Right);
                        pos.Y = Math.Min(pos.Y, this.imageElement.Bounds.Bottom);
                        SizeF sz = new SizeF(
                            Math.Min(this.imageElement.Bounds.Width, image.Size.Width),
                            Math.Min(this.imageElement.Bounds.Height, image.Size.Height));

                        if (ImageOpacity == 1f)
                        {
                            Graphics rawGraphics = (Graphics)graphics.UnderlayGraphics;

                            rawGraphics.DrawImageUnscaledAndClipped(image, new Rectangle(new Point((int) pos.X, (int) pos.Y), sz.ToSize()));
                            //rawGraphics.DrawImage(image, new Point(centerX, centerY));
                        }
                        else
                        {
                            graphics.DrawBitmap(image, (int)pos.X, (int)pos.Y, ImageOpacity);
                        }
                        break;

                    case ImageLayout.Zoom:

                        RectangleF clientRect = GetClientRectangle(this.Size);

                        #region Keep image aspect ratio in cell
                        if (image.Width == 0 || image.Height == 0)
                            break;

                        float ratioX = (float)size.Width / image.Width;
                        float ratioY = (float)size.Height / image.Height;

                        float factor = Math.Min(ratioX, ratioY);

                        if (factor <= 0f)
                            break;

                        int finalWidth = (int)Math.Round(image.Width * factor);
                        int finalHeight = (int)Math.Round(image.Height * factor);

                        int xStart = (int)clientRect.X + (int)((size.Width - finalWidth) / 2f);
                        int yStart = (int)clientRect.Y + (int)((size.Height - finalHeight) / 2f);
                        #endregion

                        if (ImageOpacity == 1f)
                        //graphics.DrawBitmap(image, 0, 0, size.Width, size.Height);
                            graphics.DrawBitmap(image, xStart, yStart, finalWidth, finalHeight);
                        else
                        //graphics.DrawBitmap(image, 0, 0, size.Width, size.Height, ImageOpacity);
                            graphics.DrawBitmap(image, xStart, yStart, finalWidth, finalHeight, ImageOpacity);
                        break;

                    case ImageLayout.Stretch:
                        if (this.ImageOpacity == 1f)
                        {
                            graphics.DrawBitmap(image, (int)imageElement.Bounds.X, (int)imageElement.Bounds.Y, (int)size.Width, (int)size.Height);
                        }
                        else
                        {
                            graphics.DrawBitmap(image, 0, 0, (int)size.Width, (int)size.Height, this.ImageOpacity);
                        }
                        break;

                    case ImageLayout.Tile:
                        for (float y = imageElement.Bounds.X; y <= imageElement.Bounds.Height - image.Height; y += image.Height)
                            for (float x = imageElement.Bounds.Y; x < imageElement.Bounds.Width - image.Width; x += image.Width)
                            {
                                if (ImageOpacity == 1f)
                                    graphics.DrawBitmap(image, (int)x, (int) y);
                                else
                                    graphics.DrawBitmap(image, (int)x, (int)y, ImageOpacity);
                            }
                        break;
                }
            }
        }

        protected virtual void PaintBackgroundImage(IGraphics graphics)
        {
            if (cachedBackgroundImage == null)
            {
                return;
            }

            Image image = cachedBackgroundImage;
            this.AnimateImage(cachedBackgroundImage, true);
            ImageAnimator.UpdateFrames();
            switch (this.BackgroundImageLayout)
            {
                case ImageLayout.Center:
                    if (ImageOpacity == 1f)
                        graphics.DrawBitmap(image, Math.Max(0, (this.Size.Width - image.Width) / 2), Math.Max(0, (this.Size.Height - image.Height) / 2),
                                            Math.Min(this.Size.Width, image.Size.Width), Math.Min(this.Size.Height, image.Size.Height));
                    else
                        graphics.DrawBitmap(image, Math.Max(0, (this.Size.Width - image.Width) / 2), Math.Max(0, (this.Size.Height - image.Height) / 2),
                                            Math.Min(this.Size.Width, image.Size.Width), Math.Min(this.Size.Height, image.Size.Height), ImageOpacity);
                    break;

                case ImageLayout.Zoom:
                    #region Keep image aspect ratio in cell
                    if (image.Width == 0 || image.Height == 0)
                        break;
                    float ratioX = (float)this.Size.Width / image.Width;
                    float ratioY = (float)this.Size.Height / image.Height;
                    float factor = Math.Min(ratioX, ratioY);
                    if (factor <= 0f)
                        break;

                    int finalWidth = (int)Math.Round(image.Width * factor);
                    int finalHeight = (int)Math.Round(image.Height * factor);

                    int xStart = (this.Size.Width - finalWidth) >> 1;
                    int yStart = (this.Size.Height - finalHeight) >> 1;
                    #endregion

                    if (ImageOpacity == 1f)
                        //graphics.DrawBitmap(image, 0, 0, this.Size.Width, this.Size.Height);
                        graphics.DrawBitmap(image, xStart, yStart, finalWidth, finalHeight);
                    else
                        //graphics.DrawBitmap(image, 0, 0, this.Size.Width, this.Size.Height, ImageOpacity);
                        graphics.DrawBitmap(image, xStart, yStart, finalWidth, finalHeight, ImageOpacity);
                    break;

                case ImageLayout.Stretch:
                    if (this.ImageOpacity == 1f)
                    {
                        graphics.DrawBitmap(image, 0, 0, this.Size.Width, this.Size.Height);
                    }
                    else
                    {
                        graphics.DrawBitmap(image, 0, 0, this.Size.Width, this.Size.Height, this.ImageOpacity);
                    }
                    break;

                case ImageLayout.Tile:
                    for (int y = 0; y < this.Size.Height; y += image.Height)
                        for (int x = 0; x < this.Size.Width; x += image.Width)
                        {
                            if (ImageOpacity == 1f)
                                graphics.DrawBitmap(image, x, y);
                            else
                                graphics.DrawBitmap(image, x, y, ImageOpacity);
                        }
                    break;
            }
        }

        protected virtual void AnimateImage(Image image, bool isBackgroudnImage)
        {
            if (ImageAnimator.CanAnimate(image))
            {
                if (!isBackgroudnImage)
                {
                    if (!this.GetBitState(CurrentlyAnimatingStateKey))
                    {
                        //Begin the animation only once.
                        ImageAnimator.Animate(image, new EventHandler(this.OnFrameChanged));
                        this.BitState[CurrentlyAnimatingStateKey] = true;
                    }
                }
                else
                {
                    if (!this.GetBitState(CurrentlyAnimatingBackGroundImageStateKey))
                    {
                        //Begin the animation only once.
                        ImageAnimator.Animate(image, new EventHandler(this.OnFrameChanged));
                        this.BitState[CurrentlyAnimatingBackGroundImageStateKey] = true;
                    }
                }
            }
        }

        protected virtual void PaintContent(IGraphics graphics)
        {
            PaintBackgroundImage(graphics);
            PaintImage(graphics);
            PaintText(graphics);
        }

        protected virtual Image ClipImage(Image image, Rectangle imageRectange, Size size)
        {
            if (imageRectange.X < 0)
            {
                imageRectange.X = 0;
            }

            if (imageRectange.Y < 0)
            {
                imageRectange.Y = 0;
            }

            int xLen = imageRectange.X + imageRectange.Width;
            int yLen = imageRectange.Y + imageRectange.Height;
            bool shouldResize = false;

            if (xLen > size.Width)
            {
                xLen = size.Width - imageRectange.X;
                shouldResize = true;
            }

            if (yLen > size.Height)
            {
                yLen = size.Height - imageRectange.Y;
                shouldResize = true;
            }
            if (shouldResize)
            {
                Bitmap bitmap = image as Bitmap;
                if (bitmap != null)
                {
                    return ReflectionPrimitive.CopyBitmap(bitmap, new Rectangle(imageRectange.Location, new Size(xLen, yLen)));
                }
            }
            return image;
        }

        private Image GetImage()
        {
            if (this.Image == null && !this.IsImageListSet)
            {
                return null;
            }

            if (this.IsImageListSet)
            {
                if (this.ImageIndex >= 0 &&
                    this.ImageIndex < this.ImageList.Images.Count)
                {
                    return new Bitmap(this.ImageList.Images[this.ImageIndex]);
                }
                else if (!string.IsNullOrEmpty(this.ImageKey) &&
                         this.ImageList.Images.IndexOfKey(this.ImageKey) >= 0)
                {
                    return new Bitmap(this.ImageList.Images[this.ImageKey]);
                }
            }

            return (Image)this.GetValue(ImageProperty);
        }

        private void ToogleRTL(bool rtl)
        {
            if (paintTextFormat == null)
            {
                this.paintTextFormat = this.PaintTextFormat;
            }

            if (rtl)
            {
                this.paintTextFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }
            else
            {
                this.paintTextFormat.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
            }
        }

        private void ToggleHTML(string text)
        {
            this.BitState[UseHTMLRenderingStateKey] = !this.DisableHTMLRendering && TinyHTMLParsers.IsHTMLMode(text);
            
            this.textPrimitiveImpl = TextPrimitiveFactory.CreateTextPrimitiveImp(this.GetBitState(UseHTMLRenderingStateKey));
        }

        internal bool AllowHTMLRendering()
        {
            return !this.GetBitState(DisableHTMLRenderingStateKey) && this.GetBitState(UseHTMLRenderingStateKey);
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF clientSize = GetClientRectangle(availableSize).Size;
            Padding borderThickness = GetBorderThickness(false);
                        
            SizeF desiredSize = this.layoutManagerPart.Measure(clientSize);
            
            desiredSize.Width += Padding.Horizontal + borderThickness.Horizontal;
            desiredSize.Height += Padding.Vertical + borderThickness.Vertical;

            SizeF elementsDesiredSize = MeasureElements(availableSize, clientSize, borderThickness);

            if (elementsDesiredSize.Width > desiredSize.Width)
            {
                desiredSize.Width = elementsDesiredSize.Width;
            }
            if (elementsDesiredSize.Height > desiredSize.Height)
            {
                desiredSize.Height = elementsDesiredSize.Height;
            }        

            desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
            desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);

            return desiredSize;
        }

        protected virtual SizeF MeasureElements(SizeF availableSize, SizeF clientSize, Padding borderThickness)
        {
            SizeF desiredSize = SizeF.Empty;

            if (this.AutoSize)
            {
                foreach (RadElement child in this.Children)
                {
                    SizeF childDesiredSize = SizeF.Empty;

                    if (child.FitToSizeMode == RadFitToSizeMode.FitToParentBounds || BypassLayoutPolicies)
                    {
                        child.Measure(availableSize);
                        childDesiredSize = child.DesiredSize;
                    }
                    else if (child.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                    {
                        child.Measure(new SizeF(clientSize.Width - borderThickness.Horizontal, clientSize.Height - borderThickness.Vertical));
                        childDesiredSize.Width = child.DesiredSize.Width + borderThickness.Horizontal;
                        childDesiredSize.Height = childDesiredSize.Height + borderThickness.Vertical;
                    }
                    else
                    {
                        child.Measure(clientSize);
                        childDesiredSize.Width += child.DesiredSize.Width + Padding.Horizontal + borderThickness.Horizontal;
                        childDesiredSize.Height += child.DesiredSize.Height + Padding.Vertical + borderThickness.Vertical;
                    }

                    desiredSize.Width = Math.Max(desiredSize.Width, childDesiredSize.Width);
                    desiredSize.Height = Math.Max(desiredSize.Height, childDesiredSize.Height);
                }
            }
            else
            {
                foreach (RadElement child in this.Children)
                {
                    child.Measure(availableSize);
                }
                desiredSize = this.Size;
            }

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF fullRect = new RectangleF(PointF.Empty, finalSize);
            RectangleF clientRect = GetClientRectangle(finalSize);
            
            this.layoutManagerPart.Arrange(clientRect);

            foreach (RadElement child in this.Children)
            {
                if (!this.BypassLayoutPolicies)
                {
                    if (child.FitToSizeMode == RadFitToSizeMode.FitToParentContent)
                    {
                        child.Arrange(clientRect);
                    }
                    else if (child.FitToSizeMode == RadFitToSizeMode.FitToParentBounds)
                    {
                        child.Arrange(fullRect);
                    }
                    else if (child.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                    {
                        Padding border = GetBorderThickness(false);
                        child.Arrange(new RectangleF(border.Left, border.Top, fullRect.Width - border.Horizontal, fullRect.Height - border.Vertical));
                    }
                }
                else
                {
                    child.Arrange(fullRect);
                }
            }
            
            return finalSize;
        }

        protected virtual void ArrangeElement(RadElement element, SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);

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
                element.Arrange(new RectangleF(new PointF(clientRect.Left, clientRect.Top), element.DesiredSize));
            }
        }

        protected virtual Padding GetClientOffset(bool includeBorder)
        {
            Padding padding = this.Padding;

            if (includeBorder && this.DrawBorder)
            {
                if (this.BorderBoxStyle == BorderBoxStyle.FourBorders)
                {
                    padding.Left += (int)this.BorderLeftWidth;
                    padding.Top += (int)this.BorderTopWidth;
                    padding.Right += (int)this.BorderRightWidth;
                    padding.Bottom += (int)this.BorderBottomWidth;
                }
                else
                {
                    int borderWidth = (int)this.BorderWidth;
                    if (this.BorderBoxStyle == BorderBoxStyle.OuterInnerBorders)
                    {
                        borderWidth = Math.Max(borderWidth, 2);
                    }

                    padding.Left += borderWidth;
                    padding.Top += borderWidth;
                    padding.Right += borderWidth;
                    padding.Bottom += borderWidth;
                }
            }

            return padding;
        }

        protected virtual RectangleF GetClientRectangle(bool includeBorder, SizeF finalSize)
        {
            Padding padding = this.Padding;
            float left = padding.Left;
            float top = padding.Top;
            float width = finalSize.Width - padding.Horizontal;
            float height = finalSize.Height - padding.Vertical;

            if (includeBorder)
            {
                //TODO: Why include border even if DrawBorder is false?
                Padding thickness = this.GetBorderThickness(false);
                left += thickness.Left;
                top += thickness.Top;
                width -= thickness.Horizontal;
                height -= thickness.Vertical;
            }

            width = Math.Max(0, width);
            height = Math.Max(0, height);

            return new RectangleF(left, top, width, height);
        }

        protected virtual RectangleF GetClientRectangle(SizeF finalSize)
        {
            Padding padding = this.Padding;
            RectangleF clientRect = new RectangleF(padding.Left, padding.Top, 
                finalSize.Width - padding.Horizontal, finalSize.Height - padding.Vertical);

            if (this.DrawBorder)
            {
                Padding thickness = this.GetBorderThickness(false);
                clientRect.X += thickness.Left;
                clientRect.Y += thickness.Top;
                clientRect.Width -= thickness.Horizontal;
                clientRect.Height -= thickness.Vertical;
            }

            clientRect.Width = Math.Max(0, clientRect.Width);
            clientRect.Height = Math.Max(0, clientRect.Height);

            return clientRect;
        }

        protected internal virtual Padding GetBorderThickness(bool checkDrawBorder)
        {
            if (checkDrawBorder && !this.DrawBorder)
            {
                return Padding.Empty;
            }

            Padding thickness = Padding.Empty;

            if (this.BorderBoxStyle == BorderBoxStyle.SingleBorder)
            {
                thickness = new Padding((int)this.BorderWidth);
            }
            else if (this.BorderBoxStyle == BorderBoxStyle.FourBorders)
            {
                thickness = new Padding((int)this.BorderLeftWidth, (int)this.BorderTopWidth, (int)this.BorderRightWidth, (int)this.BorderBottomWidth);
            }
            else if (this.BorderBoxStyle == BorderBoxStyle.OuterInnerBorders)
            {
                int borderWidth = (int)this.BorderWidth;
                if (borderWidth == 1)
                {
                    borderWidth = 2;
                }
                thickness = new Padding(borderWidth);
            }

            return thickness;
        }

        protected ContentAlignment GetTextAlignment(ContentAlignment textAlignment)
        {
            if (this.RightToLeft)
            {
                switch (textAlignment)
                {
                    case ContentAlignment.BottomLeft:
                        return ContentAlignment.BottomRight;
                    case ContentAlignment.BottomRight:
                        return ContentAlignment.BottomLeft;
                    case ContentAlignment.MiddleLeft:
                        return ContentAlignment.MiddleRight;
                    case ContentAlignment.MiddleRight:
                        return ContentAlignment.MiddleLeft;
                    case ContentAlignment.TopLeft:
                        return ContentAlignment.TopRight;
                    case ContentAlignment.TopRight:
                        return ContentAlignment.TopLeft;
                }
            }
            return textAlignment;
        }

        protected ContentAlignment GetTextAlignment()
        {
            return GetTextAlignment(this.TextAlignment);
        }

        #endregion

        #region Event handlers

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == TextAlignmentProperty ||
                e.Property == FontProperty ||
                e.Property == ForeColorProperty ||
                e.Property == TextProperty)
            {               
                this.ToggleHTML(this.Text);
                this.layoutManagerPart.IsDirty = true;
            }
            else if (e.Property == ImageProperty)
            {
                if (this.GetBitState(CurrentlyAnimatingStateKey) && this.cachedImage != null)
                {
                    ImageAnimator.StopAnimate(this.cachedImage, new EventHandler(this.OnFrameChanged));
                }
                this.BitState[CurrentlyAnimatingStateKey] = false;
                cachedImage = (Image)e.NewValue;
                this.layoutManagerPart.IsDirty = true;
            }
            else if (e.Property == BackgroundImageProperty)
            {
                if (this.GetBitState(CurrentlyAnimatingBackGroundImageStateKey) && this.cachedBackgroundImage != null)
                {
                    ImageAnimator.StopAnimate(this.cachedBackgroundImage, new EventHandler(this.OnFrameChanged));
                }
                this.BitState[CurrentlyAnimatingBackGroundImageStateKey] = false;
                cachedBackgroundImage = (Image)e.NewValue;
            }
            else if (e.Property == ImageIndexProperty ||
                     e.Property == ImageKeyProperty)
            {
                cachedImage = GetImage();
            }
            else if (e.Property == RadElement.RightToLeftProperty)
            {
                this.ToogleRTL((bool)e.NewValue);
                this.layoutManagerPart.IsDirty = true;
            }

            base.OnPropertyChanged(e);
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            this.Invalidate();
        }

        #region Link Support

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.DisableHTMLRendering)
            {
                return;
            }

            this.textPrimitiveImpl.OnMouseMove(this, e);            
        }

        #endregion

        #endregion

        #region Rad Properties

        public static RadProperty DrawTextProperty = RadProperty.Register(
            "DrawText", typeof(bool), typeof(LightVisualElement), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty DrawFillProperty = RadProperty.Register(
            "DrawFill", typeof(bool), typeof(LightVisualElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty DrawBorderProperty = RadProperty.Register(
            "DrawBorder", typeof(bool), typeof(LightVisualElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderBoxStyleProperty = RadProperty.Register(
            "BorderBoxStyle", typeof(BorderBoxStyle), typeof(LightVisualElement), new RadElementPropertyMetadata(
                BorderBoxStyle.SingleBorder, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static readonly RadProperty BorderDrawModeProperty = RadProperty.Register(
            "BorderDrawMode", typeof(BorderDrawModes), typeof(LightVisualElement), new RadElementPropertyMetadata(
                BorderDrawModes.RightOverTop, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        public static readonly RadProperty BorderWidthProperty = RadProperty.Register(
            "BorderWidth", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderLeftWidthProperty = RadProperty.Register(
            "BorderLeftWidth", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderRightWidthProperty = RadProperty.Register(
            "BorderRightWidth", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderTopWidthProperty = RadProperty.Register(
            "BorderTopWidth", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderBottomWidthProperty = RadProperty.Register(
            "BorderBottomWidth", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BorderGradientAngleProperty = RadProperty.Register(
            "BorderGradientAngle", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                270f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BorderGradientStyleProperty = RadProperty.Register(
            "BorderGradientStyle", typeof(GradientStyles), typeof(LightVisualElement), new RadElementPropertyMetadata(
                GradientStyles.Linear, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BorderColorProperty = RadProperty.Register(
            "BorderColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDarkDark, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BorderColor2Property = RadProperty.Register(
            "BorderColor2", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty BorderColor3Property = RadProperty.Register(
            "BorderColor3", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty BorderColor4Property = RadProperty.Register(
            "BorderColor4", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty BorderInnerColorProperty = RadProperty.Register(
            "BorderInnerColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlLightLight, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty BorderInnerColor2Property = RadProperty.Register(
            "BorderInnerColor2", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.Control, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty BorderInnerColor3Property = RadProperty.Register(
            "BorderInnerColor3", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty BorderInnerColor4Property = RadProperty.Register(
            "BorderInnerColor4", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDarkDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty BackColor2Property = RadProperty.Register(
            "BackColor2", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.Control, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BackColor3Property = RadProperty.Register(
            "BackColor3", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BackColor4Property = RadProperty.Register(
            "BackColor4", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlLightLight, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty NumberOfColorsProperty = RadProperty.Register(
            "NumberOfColors", typeof(int), typeof(LightVisualElement), new RadElementPropertyMetadata(
                2, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty GradientStyleProperty = RadProperty.Register(
            "GradientStyle", typeof(GradientStyles), typeof(LightVisualElement), new RadElementPropertyMetadata(
                GradientStyles.Linear, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty GradientAngleProperty = RadProperty.Register(
            "GradientAngle", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                90f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty GradientPercentageProperty = RadProperty.Register(
            "GradientPercentage", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                0.5f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty GradientPercentage2Property = RadProperty.Register(
            "GradientPercentage2", typeof(float), typeof(LightVisualElement), new RadElementPropertyMetadata(
                0.666f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BackgroundImageProperty = RadProperty.Register(
            "BackgroundImage", typeof(Image), typeof(LightVisualElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty ImageProperty = RadProperty.Register(
            "Image", typeof(Image), typeof(LightVisualElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty ImageIndexProperty = RadProperty.Register(
            "ImageIndex", typeof(int), typeof(LightVisualElement), new RadElementPropertyMetadata(
                -1, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty ImageKeyProperty = RadProperty.Register(
            "ImageKey", typeof(string), typeof(LightVisualElement), new RadElementPropertyMetadata(
                string.Empty, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty ImageLayoutProperty = RadProperty.Register(
            "ImageLayout", typeof(ImageLayout), typeof(LightVisualElement), new RadElementPropertyMetadata(
                ImageLayout.Center, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BackgroundImageLayoutProperty = RadProperty.Register(
            "BackgroundImageLayout", typeof(ImageLayout), typeof(LightVisualElement), new RadElementPropertyMetadata(
                ImageLayout.Center, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ImageOpacityProperty =
        RadProperty.Register("ImageOpacity", typeof(double), typeof(LightVisualElement),
                             new RadElementPropertyMetadata(1.0d, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty TextAlignmentProperty = RadProperty.Register(
            "TextAlignment", typeof(ContentAlignment), typeof(LightVisualElement), new RadElementPropertyMetadata(
                ContentAlignment.MiddleCenter, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ImageAlignmentProperty = RadProperty.Register(
            "ImageAlignment", typeof(ContentAlignment), typeof(LightVisualElement), new RadElementPropertyMetadata(
                ContentAlignment.MiddleCenter, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty TextImageRelationProperty = RadProperty.Register(
            "TextImageRelation", typeof(TextImageRelation), typeof(LightVisualElement), new RadElementPropertyMetadata(
                TextImageRelation.Overlay, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ShowHorizontalLineProperty = RadProperty.Register(
            "ShowHorizontalLine", typeof(bool), typeof(LightVisualElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty HorizontalLineColorProperty = RadProperty.Register(
            "HorizontalLineColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.Control, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty HorizontalLineWidthProperty = RadProperty.Register(
            "HorizontalLineWidth", typeof(int), typeof(LightVisualElement), new RadElementPropertyMetadata(
                1, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderLeftColorProperty = RadProperty.Register(
            "BorderLeftColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderTopColorProperty = RadProperty.Register(
            "BorderTopColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderRightColorProperty = RadProperty.Register(
            "BorderRightColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderBottomColorProperty = RadProperty.Register(
            "BorderBottomColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderLeftShadowColorProperty = RadProperty.Register(
            "BorderLeftShadowColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderTopShadowColorProperty = RadProperty.Register(
            "BorderTopShadowColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderRightShadowColorProperty = RadProperty.Register(
            "BorderRightShadowColor", typeof(Color), typeof(LightVisualElement), new RadElementPropertyMetadata(
                Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BorderBottomShadowColorProperty =
        RadProperty.Register("BorderBottomShadowColor", typeof(Color), typeof(LightVisualElement),
                             new RadElementPropertyMetadata(Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ClipTextProperty = RadProperty.Register(
            "ClipText", 
            typeof(bool), 
            typeof(LightVisualElement), 
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty TextWrapProperty =  RadProperty.Register(
        "TextWrap", typeof(bool), typeof(LightVisualElement), new RadElementPropertyMetadata(
            false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));


        #endregion

        #region CLR Properties

        private bool drawTextCached = true;

        /// <summary>
        /// Gets or Sets value indicating whether the element should paint its text 
        /// </summary>
        [Description("Indicates whether the element should paint its text")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("DrawText", typeof(LightVisualElement))]
        public bool DrawText
        {
            get
            {
                return this.drawTextCached;
            }
            set
            {
                this.SetValue(DrawTextProperty, value);
                this.drawTextCached = value;
            }
        }


        /// <summary>
        /// Gets or Sets value indicating whether the element should paint its background 
        /// </summary>
        [Description("Indicates whether the element should paint its background")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("DrawFill", typeof(LightVisualElement))]
        public override bool DrawFill
        {
            get
            {
                return (bool)this.GetValue(DrawFillProperty);
            }
            set
            {
                this.SetValue(DrawFillProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets value indicating whether the element should paint its border 
        /// </summary>
        [Description("Indicates whether the element should paint its border")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("DrawBorder", typeof(LightVisualElement))]
        public override bool DrawBorder
        {
            get
            {
                return (bool)this.GetValue(DrawBorderProperty);
            }
            set
            {
                this.SetValue(DrawBorderProperty, value);
            }
        }
        
        /// <summary>
        /// 	<para class="MsoNormal" style="MARGIN: 0in 0in 0pt">
        /// 		<span style="FONT-SIZE: 8pt; COLOR: black; FONT-FAMILY: Verdana">Gets or sets the
        ///     Border style. The two possible values are SingleBorder and FourBorder. In the
        ///     single border case, all four sides share the same appearance although the entire
        ///     border may have gradient. In four border case, each of the four sides may differ in
        ///     appearance. For example, the left border may have different color, shadowcolor, and
        ///     width from the rest. When SingleBorder is chosen, you should use the general
        ///     properties such as width and color, and respectively, when the FourBorder style is
        ///     chosen you should use properties prefixed with the corresponding side, for example,
        ///     LeftColor, LeftWidth for the left side.</span></para>
        /// </summary>
        [RadPropertyDefaultValue("BorderBoxStyle", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        public override BorderBoxStyle BorderBoxStyle
        {
            get
            {
                return (BorderBoxStyle)this.GetValue(BorderBoxStyleProperty);
            }
            set
            {
                this.SetValue(BorderBoxStyleProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [RadPropertyDefaultValue("BorderDrawModes", typeof(BorderPrimitive))]
        [Category(BasePrimitive.BoxCategory)]
        //[Description("Determine the sizing style of the border's sides")]
        public override BorderDrawModes BorderDrawMode
        {
            get
            {
                return (BorderDrawModes)this.GetValue(BorderDrawModeProperty);
            }
            set
            {
                this.SetValue(BorderDrawModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a float value width of the left border. This property
        /// has effect only if <em>FourBorders</em> style is used in <em>BoxStyle</em> property and
        /// affects only the width of the left border.
        /// </summary>
        [RadPropertyDefaultValue("BorderWidth", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        public override float BorderWidth
        {
            get
            {
                return (float)this.GetValue(BorderWidthProperty);
            }
            set
            {
                this.SetValue(BorderWidthProperty, value);
            }
        }        

        /// <summary>
        /// Gets or sets a float value width of the left border. This property
        /// has effect only if <em>FourBorders</em> style is used in <em>BoxStyle</em> property and
        /// affects only the width of the left border.
        /// </summary>
        [RadPropertyDefaultValue("BorderLeftWidth", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        public override float BorderLeftWidth
        {
            get
            {
                return (float)this.GetValue(BorderLeftWidthProperty);
            }
            set
            {
                this.SetValue(BorderLeftWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a float value width of the top border . This property
        /// has effect only if <em>FourBorders</em> style is used in <em>BoxStyle</em> property,
        /// and affects only the top border.
        /// </summary>
        [RadPropertyDefaultValue("BorderTopWidth", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        public override float BorderTopWidth
        {
            get
            {
                return (float)this.GetValue(BorderTopWidthProperty);
            }
            set
            {
                this.SetValue(BorderTopWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a float value width of the right border. This
        /// property has effect only if <em>FourBorders</em> style is used in <em>BoxStyle</em>
        /// property, and affects only the right border.
        /// </summary>
        [RadPropertyDefaultValue("BorderRightWidth", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        public override float BorderRightWidth
        {
            get
            {
                return (float)this.GetValue(BorderRightWidthProperty);
            }
            set
            {
                this.SetValue(BorderRightWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a float value width. This property has effect only if
        /// <em>FourBorders</em> style is used in <em>BoxStyle</em> property, and affects only the
        /// bottom border.
        /// </summary>
        [RadPropertyDefaultValue("BorderBottomWidth", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        public override float BorderBottomWidth
        {
            get
            {
                return (float)this.GetValue(BorderBottomWidthProperty);
            }
            set
            {
                this.SetValue(BorderBottomWidthProperty, value);
            }
        }

        /// <summary>Gets or sets gradient angle for linear gradient measured in degrees.</summary>
        [RadPropertyDefaultValue("BorderGradientAngle", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        public override float BorderGradientAngle
        {
            get
            {
                return (float)this.GetValue(BorderGradientAngleProperty);
            }
            set
            {
                this.SetValue(BorderGradientAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets gradient style. Possible styles are solid, linear, radial, glass,
        /// office glass, gel, and vista.
        /// </summary>
        [RadPropertyDefaultValue("BorderGradientStyle", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        public override GradientStyles BorderGradientStyle
        {
            get
            {
                return (GradientStyles)this.GetValue(BorderGradientStyleProperty);
            }
            set
            {
                this.SetValue(BorderGradientStyleProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderColor
        {
            get
            {
                return (Color)this.GetValue(BorderColorProperty);
            }
            set
            {
                this.SetValue(BorderColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("BorderColor2", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderColor2
        {
            get
            {
                return (Color)this.GetValue(BorderColor2Property);
            }
            set
            {
                this.SetValue(BorderColor2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, and vista gradients. This
        /// is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("BorderColor3", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderColor3
        {
            get
            {
                return (Color)this.GetValue(BorderColor3Property);
            }
            set
            {
                this.SetValue(BorderColor3Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, and vista gradients. This
        /// is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("BorderColor4", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderColor4
        {
            get
            {
                return (Color)this.GetValue(BorderColor4Property);
            }
            set
            {
                this.SetValue(BorderColor4Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("BorderInnerColor", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderInnerColor
        {
            get
            {
                return (Color)this.GetValue(BorderInnerColorProperty);
            }
            set
            {
                this.SetValue(BorderInnerColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("BorderInnerColor2", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderInnerColor2
        {
            get
            {
                return (Color)this.GetValue(BorderInnerColor2Property);
            }
            set
            {
                this.SetValue(BorderInnerColor2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("BorderInnerColor3", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderInnerColor3
        {
            get
            {
                return (Color)this.GetValue(BorderInnerColor3Property);
            }
            set
            {
                this.SetValue(BorderInnerColor3Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("BorderInnerColor4", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderInnerColor4
        {
            get
            {
                return (Color)this.GetValue(BorderInnerColor4Property);
            }
            set
            {
                this.SetValue(BorderInnerColor4Property, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BackColor2
        {
            get
            {
                return (Color)this.GetValue(BackColor2Property);
            }
            set
            {
                this.SetValue(BackColor2Property, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BackColor3
        {
            get
            {
                return (Color)this.GetValue(BackColor3Property);
            }
            set
            {
                this.SetValue(BackColor3Property, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BackColor4
        {
            get
            {
                return (Color)this.GetValue(BackColor4Property);
            }
            set
            {
                this.SetValue(BackColor4Property, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        public override int NumberOfColors
        {
            get
            {
                return (int)this.GetValue(NumberOfColorsProperty);
            }
            set
            {
                this.SetValue(NumberOfColorsProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        public override GradientStyles GradientStyle
        {
            get
            {
                return (GradientStyles)this.GetValue(GradientStyleProperty);
            }
            set
            {
                this.SetValue(GradientStyleProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        public override float GradientAngle
        {
            get
            {
                return (float)this.GetValue(GradientAngleProperty);
            }
            set
            {
                this.SetValue(GradientAngleProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        public override float GradientPercentage
        {
            get
            {
                return (float)this.GetValue(GradientPercentageProperty);
            }
            set
            {
                this.SetValue(GradientPercentageProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        public override float GradientPercentage2
        {
            get
            {
                return (float)this.GetValue(GradientPercentage2Property);
            }
            set
            {
                this.SetValue(GradientPercentage2Property, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image BackgroundImage
        {
            get
            {
                return this.cachedBackgroundImage;
            }
            set
            {
                this.SetValue(BackgroundImageProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image Image
        {
            get
            {
                return this.cachedImage;
            }
            set
            {
                this.SetValue(ImageProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [RelatedImageList("ElementTree.Control.ImageList")]
        [Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor))]
        [TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
        public virtual int ImageIndex
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

        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public virtual string ImageKey
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

        [DefaultValue(ImageLayout.Center)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual ImageLayout ImageLayout
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

        [DefaultValue(ImageLayout.Center)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual ImageLayout BackgroundImageLayout
        {
            get
            {
                return (ImageLayout)this.GetValue(BackgroundImageLayoutProperty);
            }
            set
            {
                this.SetValue(BackgroundImageLayoutProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        public virtual double ImageOpacity
        {
            get
            {
                return (double)this.GetValue(ImageOpacityProperty);
            }
            set
            {
                this.SetValue(ImageOpacityProperty, value);
            }
        }

        [DefaultValue(ContentAlignment.MiddleCenter)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public virtual ContentAlignment TextAlignment
        {
            get
            {
                return (ContentAlignment)this.GetValue(TextAlignmentProperty);
            }
            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }

        [DefaultValue(ContentAlignment.MiddleCenter)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public virtual ContentAlignment ImageAlignment
        {
            get
            {
                return (ContentAlignment)this.GetValue(ImageAlignmentProperty);
            }
            set
            {
                SetValue(ImageAlignmentProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating the TextImageRelation: ImageAboveText,
        ///     ImageBeforeText, Overlay, TextAboveImage, and TextBeforeImage. 
        /// </summary>
        [RadPropertyDefaultValue("TextImageRelation", typeof(LightVisualElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public TextImageRelation TextImageRelation
        {
            get
            {
                return (TextImageRelation)this.GetValue(TextImageRelationProperty);
            }
            set
            {
                this.SetValue(TextImageRelationProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        public virtual bool ShowHorizontalLine
        {
            get
            {
                return (bool)this.GetValue(ShowHorizontalLineProperty);
            }
            set
            {
                this.SetValue(ShowHorizontalLineProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color HorizontalLineColor
        {
            get
            {
                return (Color)this.GetValue(HorizontalLineColorProperty);
            }
            set
            {
                this.SetValue(HorizontalLineColorProperty, value);
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        public virtual int HorizontalLineWidth
        {
            get
            {
                return (int)this.GetValue(HorizontalLineWidthProperty);
            }
            set
            {
                this.SetValue(HorizontalLineWidthProperty, value);
            }
        }

        private StringAlignment CreateStringAlignment(ContentAlignment textAlign)
        {
            switch (textAlign)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                    return StringAlignment.Center;                    
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    return StringAlignment.Near;                    
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    return StringAlignment.Far;                    
            }
            return StringAlignment.Center;
        }

        /// <summary>
        /// 	<para>Gets the StringFormat. The default options for the telerik text are
        ///     the following: StringFormatFlags.NoWrap, StringFormatFlags.FitBlackBox, 
        ///     stringAlignment.Near, StringTrimming.EllipsisCharacter, 
        ///     HotkeyPrefix.Show</para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(true)]
        public virtual StringFormat PaintTextFormat
        {
            get
            {
                if (paintTextFormat == null)
                {
                    paintTextFormat = new StringFormat();
                }

                paintTextFormat.FormatFlags = StringFormatFlags.FitBlackBox;
                if (!this.TextWrap) 
                {
                    paintTextFormat.FormatFlags |= StringFormatFlags.NoWrap;
                }

                paintTextFormat.LineAlignment = StringAlignment.Near;
                paintTextFormat.Trimming = StringTrimming.EllipsisCharacter;
                paintTextFormat.HotkeyPrefix = HotkeyPrefix.Show;
                if (this.MeasureTrailingSpaces)
                {
                    paintTextFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                }

                this.ToogleRTL(this.RightToLeft);
                
                paintTextFormat.Alignment = this.CreateStringAlignment(this.TextAlignment);
				
                return paintTextFormat;
            }
        }

        private ImageList ImageList
        {
            get
            {
                if (this.ElementTree == null)
                    return null;

                return this.ElementTree.ComponentTreeHandler.ImageList;
            }
        }

        private bool IsImageListSet
        {
            get
            {
                bool isIndexSet = (this.ImageIndex >= 0) || (this.ImageKey != string.Empty);
                bool isImageListSet = (this.ImageList != null);
                return isIndexSet && isImageListSet;
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(false)]
        public bool DisableHTMLRendering
        {
            get
            {
                return this.GetBitState(DisableHTMLRenderingStateKey);
            }
            set
            {
                this.SetBitState(DisableHTMLRenderingStateKey, value);
            }
        }

        //[Obsolete("Please, use DisableHTMLRendering property to define whether HTML-like formatting should be applied on the text.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(false)]
        protected bool UseHTMLRendering
        {
            get
            {
                return this.GetBitState(UseHTMLRenderingStateKey);
            }
            set
            {
                this.SetBitState(UseHTMLRenderingStateKey, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public FormattedTextBlock TextBlock
        {
            get
            {
                return this.textBlock;
            }
            set
            {
                this.textBlock = value;
            }
        }

        /// <summary>
        /// Gets and sets the left border color. This applies only if FourBorders is chosen
        /// for BoxStyle property, and affects only the left border.
        /// </summary>
        [RadPropertyDefaultValue("BorderLeftColor", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderLeftColor
        {
            get
            {
                return (Color)this.GetValue(BorderLeftColorProperty);
            }
            set
            {
                this.SetValue(BorderLeftColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the top border color. This applies only if FourBorders is chosen
        /// for BoxStyle property, and affects only the top border.
        /// </summary>
        [RadPropertyDefaultValue("BorderTopColor", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderTopColor
        {
            get
            {
                return (Color)this.GetValue(BorderTopColorProperty);
            }
            set
            {
                this.SetValue(BorderTopColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the right border color. This applies only if FourBorders is chosen
        /// for BoxStyle property, and affects only the right border.
        /// </summary>
        [RadPropertyDefaultValue("BorderRightColor", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderRightColor
        {
            get
            {
                return (Color)this.GetValue(BorderRightColorProperty);
            }
            set
            {
                this.SetValue(BorderRightColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the bottom border color. This applies only if FourBorders is chosen
        /// for BoxStyle property, and affects only the bottom border.
        /// </summary>
        [RadPropertyDefaultValue("BorderBottomColor", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderBottomColor
        {
            get
            {
                return (Color)this.GetValue(BorderBottomColorProperty);
            }
            set
            {
                this.SetValue(BorderBottomColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the left shadow color. This option applies only if
        /// fourBorders is chosen, and affects only the left border.
        /// </summary>
        [RadPropertyDefaultValue("LeftShadowColor", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderLeftShadowColor
        {
            get
            {
                return (Color)this.GetValue(BorderLeftShadowColorProperty);
            }
            set
            {
                this.SetValue(BorderLeftShadowColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the top shadow color. This option applies only if
        /// fourBorders is chosen, and affects only the top border.
        /// </summary>
        [RadPropertyDefaultValue("BorderTopShadowColor", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderTopShadowColor
        {
            get
            {
                return (Color)this.GetValue(BorderTopShadowColorProperty);
            }
            set
            {
                this.SetValue(BorderTopShadowColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the right shadow color. This option applies only if
        /// fourBorders is chosen, and affects only the right border.
        /// </summary>
        [RadPropertyDefaultValue("BorderRightShadowColor", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderRightShadowColor
        {
            get
            {
                return (Color)this.GetValue(BorderRightShadowColorProperty);
            }
            set
            {
                this.SetValue(BorderRightShadowColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the bottom shadow color. This option applies only if
        /// fourBorders is chosen, and affects only the bottom border.
        /// </summary>
        [RadPropertyDefaultValue("BorderBottomShadowColor", typeof(LightVisualElement)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public override Color BorderBottomShadowColor
        {
            get
            {
                return (Color)this.GetValue(BorderBottomShadowColorProperty);
            }
            set
            {
                this.SetValue(BorderBottomShadowColorProperty, value);
            }
        }

        /// <summary>
        /// Determines whether text will be clipped within the calculated text paint rectangle.
        /// </summary>
        [Description("Determines whether text will be clipped within the calculated text paint rectangle.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool ClipText
        {
            get
            {
                return (bool)this.GetValue(ClipTextProperty);
            }
            set
            {
                this.SetValue(ClipTextProperty, value);
            }
        }

        public LayoutManagerPart Layout
        {
            get { return this.layoutManagerPart; }
        }

        #endregion

        #region ITextProvider Members

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShadowSettings Shadow
        {
            get
            {
                return new ShadowSettings(Point.Empty, this.ForeColor);
            }
            set
            {
            }
        }

        /// <summary>
        /// Determines whether character trimming will be automatically applied to the element if text cannot be fitted within the available space.
        /// </summary>
        [DefaultValue(false)]
        [Description("Determines whether character trimming will be automatically applied to the element if text cannot be fitted within the available space.")]
        public bool AutoEllipsis
        {
            get
            {
                return this.GetBitState(autoEllipsisStateKey);
            }
            set
            {
                this.BitState[autoEllipsisStateKey] = value;
            }
        }

        /// <summary>
        /// Determines whether ampersand character will be treated as mnemonic or not.
        /// </summary>
        [DefaultValue(false)]
        [Description("Determines whether ampersand character will be treated as mnemonic or not.")]
        public bool UseMnemonic
        {
            get
            {
                return this.GetBitState(useMnemonicStateKey);
            }
            set
            {
                this.BitState[useMnemonicStateKey] = value;
            }
        }

        public RectangleF GetFaceRectangle()
        {
            RectangleF faceRectangle = new RectangleF(PointF.Empty, this.DesiredSize);
            return faceRectangle;
        }

        //true if the text should wrap to the available layout rectangle
        //otherwise, false. The default is true
        [Description("Determines whether text wrap is enabled.")]
        [RadPropertyDefaultValue("TextWrap", typeof(LightVisualElement)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool TextWrap
        {
            get
            {
                return (bool)this.GetValue(TextWrapProperty);
            }
            set
            {
                this.SetValue(TextWrapProperty, value);
            }
        }


        /// <summary>
        /// Determines whether keyboard focus cues are enabled for this element.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowKeyboardCues
        {
            get
            {
                return this.GetBitState(showKeyboardCuesStateKey);
            }
            set
            {
                this.BitState[showKeyboardCuesStateKey] = value;
            }
        }

        /// <summary>
        /// Determines whether trailing spaces will be included when text size is measured.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MeasureTrailingSpaces
        {
            get
            {
                return this.GetBitState(measureTrailingSpacesStateKey);
            }
            set
            {
                this.BitState[measureTrailingSpacesStateKey] = value;
            }
        }

        internal virtual protected TextParams CreateTextParams()
        {
            TextParams textParams = new TextParams();
            textParams.text = this.Text;
            textParams.alignment = this.TextAlignment;// this.GetTextAlignment();
            textParams.autoEllipsis = this.AutoEllipsis;
            textParams.flipText = this.FlipText;
            textParams.font = this.Font;
            textParams.foreColor = this.ForeColor;
            textParams.measureTrailingSpaces = this.MeasureTrailingSpaces;
            textParams.paintingRectangle = this.layoutManagerPart.RightPart.Bounds;//new RectangleF( PointF.Empty, this.Size );
            textParams.rightToLeft = this.RightToLeft;
            textParams.shadow = this.Shadow;
            textParams.showKeyboardCues = this.ShowKeyboardCues;
            textParams.textOrientation = this.TextOrientation;
            textParams.textRenderingHint = this.TextRenderingHint;
            textParams.textWrap = this.TextWrap;
            textParams.useCompatibleTextRendering = this.UseCompatibleTextRendering;
            textParams.useMnemonic = this.UseMnemonic;
            textParams.stretchHorizontally = this.StretchHorizontally;
            textParams.ClipText = this.ClipText;
            return textParams;
        }

        #endregion

        #region ITextPrimitive Members

        public void PaintPrimitive(IGraphics graphics, float angle, SizeF scale, TextParams textParams)
        {
            this.textPrimitiveImpl.PaintPrimitive(graphics, angle, scale, textParams);
        }

        public void PaintPrimitive(IGraphics graphics, TextParams textParams)
        {
            this.textPrimitiveImpl.PaintPrimitive(graphics, textParams);
        }

        public SizeF MeasureOverride(SizeF availableSize, TextParams textParams)
        {
            return this.textPrimitiveImpl.MeasureOverride(availableSize, textParams);
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.textPrimitiveImpl.OnMouseMove(sender, e);
        }

        public SizeF GetTextSize(SizeF proposedSize, TextParams textParams)
        {
            return this.textPrimitiveImpl.GetTextSize(proposedSize, textParams);
        }

        public SizeF GetTextSize(TextParams textParams)
        {
            return this.textPrimitiveImpl.GetTextSize(textParams);
        }
        #endregion
    }
}
