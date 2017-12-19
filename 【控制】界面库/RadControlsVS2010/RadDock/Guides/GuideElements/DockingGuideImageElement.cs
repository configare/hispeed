using System.Drawing;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Paint;
using System;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    public class DockingGuideImageElement : RadItem, IDockingGuideImage
    {
        #region RadProperties
                
        public static RadProperty ImageProperty = RadProperty.Register("Image", typeof(Image),
            typeof(DockingGuideImageElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty HotImageProperty = RadProperty.Register("HotImage", typeof(Image),
            typeof(DockingGuideImageElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty LocationOnCenterGuideProperty = RadProperty.Register("LocationOnCenterGuide", typeof(Point),
            typeof(DockingGuideImageElement), new RadElementPropertyMetadata(
                Point.Empty, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsHotProperty = RadProperty.Register("IsHot", typeof(bool),
            typeof(DockingGuideImageElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout));
        
        #endregion

        #region Properties

        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
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

        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public Image HotImage
        {
            get
            {
                return (Image)this.GetValue(HotImageProperty);
            }
            set
            {
                this.SetValue(HotImageProperty, value);
            }
        }

        public Point LocationOnCenterGuide
        {
            get
            {
                return (Point)this.GetValue(LocationOnCenterGuideProperty);
            }
            set
            {
                this.SetValue(LocationOnCenterGuideProperty, value);
            }
        } 

        public Size PreferredSize
        {
            get 
            {
                if ((bool)GetValue(IsHotProperty) && this.HotImage != null)
                {
                    return this.HotImage.Size;
                }
                else if (this.Image != null)
                {
                    return this.Image.Size;
                }
                return Size.Empty;
            }
        }

        #endregion

        #region Initialization

        public virtual void Initialize(IDockingGuideImage image)
        {
            this.Image = image.Image;
            this.HotImage = image.HotImage;
            this.LocationOnCenterGuide = image.LocationOnCenterGuide;
        }
            
        public virtual void ResetValues()
        {
            this.ResetValue(ImageProperty, ValueResetFlags.Local);
            this.ResetValue(HotImageProperty, ValueResetFlags.Local);
            this.ResetValue(LocationOnCenterGuideProperty, ValueResetFlags.Local);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.ShouldPaint = true;
        }

        #endregion

        #region Paint

        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            if ((bool)GetValue(IsHotProperty) && HotImage != null)
            {
                graphics.DrawBitmap(HotImage, 0, 0);
            }
            else if (Image != null)
            {
                graphics.DrawBitmap(Image, 0, 0);
            }

        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF defaultDesiredSize = base.MeasureOverride(availableSize);
            if ((bool)GetValue(IsHotProperty) && HotImage != null)
            {
                return new SizeF(HotImage.Size.Width, HotImage.Size.Height);
            }
            else if (Image != null)
            {
                return new SizeF(Image.Size.Width, Image.Size.Height);
            }
            return defaultDesiredSize;
        }

        #endregion

        #region Event handlers

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            SetValue(IsHotProperty, true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            SetValue(IsHotProperty, false);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (this.Parent != null && e.Property == LocationOnCenterGuideProperty)
            {
                this.Parent.InvalidateMeasure(true);
            }
        }

        #endregion
    }
}
