using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI.Docking;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    public class DockingGuidesElement : RadItem
    {
        #region Fields

        private DockingGuideImageElement leftImage;
        private DockingGuideImageElement topImage;
        private DockingGuideImageElement rightImage;
        private DockingGuideImageElement bottomImage;
        private DockingGuideImageElement fillImage;
        private DockingGuideImageElement centerBackgroundImage;
        
        #endregion

        #region RadProperties

        public static RadProperty DockingHintBackColorProperty = RadProperty.Register("DockingHintBackColor", typeof(Color),
        typeof(DockingGuidesElement), new RadElementPropertyMetadata(Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty DockingHintBorderColorProperty = RadProperty.Register("DockingHintBorderColor", typeof(Color),
        typeof(DockingGuidesElement), new RadElementPropertyMetadata(Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty DockingHintBorderWidthProperty = RadProperty.Register("DockingHintBorderWidth", typeof(int),
        typeof(DockingGuidesElement), new RadElementPropertyMetadata(3, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Properties

        public DockingGuideImageElement LeftImage
        {
            get
            {
                return this.leftImage;
            }
        }

        public DockingGuideImageElement RightImage
        {
            get
            {
                return this.rightImage;
            }
        }

        public DockingGuideImageElement BottomImage
        {
            get
            {
                return this.bottomImage;
            }
        }

        public DockingGuideImageElement TopImage
        {
            get
            {
                return this.topImage;
            }
        }

        public DockingGuideImageElement FillImage
        {
            get
            {
                return this.fillImage;
            }
        }

        public DockingGuideImageElement CenterBackgroundImage
        {
            get
            {
                return this.centerBackgroundImage;
            }
        }

        public Color DockingHintBackColor
        {
            get
            {
                return (Color)this.GetValue(DockingHintBackColorProperty);
            }
            set
            {
                this.SetValue(DockingHintBackColorProperty, value);
            }
        }

        public Color DockingHintBorderColor
        {
            get
            {
                return (Color)this.GetValue(DockingHintBorderColorProperty);
            }
            set
            {
                this.SetValue(DockingHintBorderColorProperty, value);
            }
        }

        public int DockingHintBorderWidth
        {
            get
            {
                return (int)this.GetValue(DockingHintBorderWidthProperty);
            }
            set
            {
                this.SetValue(DockingHintBorderWidthProperty, value);
            }
        }

        public IDockingGuidesTemplate Template
        {
            get
            {
                return null;
            }
            set
            {
                if (value == null)
                {
                    centerBackgroundImage.ResetValues();
                    leftImage.ResetValues();
                    topImage.ResetValues();
                    rightImage.ResetValues();
                    bottomImage.ResetValues();
                    fillImage.ResetValues();
                }
                else
                {
                    centerBackgroundImage.Initialize(value.CenterBackgroundImage);
                    leftImage.Initialize(value.LeftImage);
                    topImage.Initialize(value.TopImage);
                    rightImage.Initialize(value.RightImage);
                    bottomImage.Initialize(value.BottomImage);
                    fillImage.Initialize(value.FillImage);
                }
            }
        }

        #endregion

        #region Initialization

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            DockingGuidesTemplate template = PredefinedDockingGuidesTemplate.ControlDefault;

            centerBackgroundImage = new DockingGuideImageElement();
            centerBackgroundImage.Class = "CenterBackgroundImage";
            centerBackgroundImage.Initialize(template.CenterBackgroundImage);
            this.Children.Add(centerBackgroundImage);

            leftImage = new DockingGuideImageElement();
            leftImage.Class = "LeftImage";
            leftImage.Initialize(template.LeftImage);
            this.Children.Add(leftImage);
            
            topImage = new DockingGuideImageElement();
            topImage.Class = "TopImage";
            topImage.Initialize(template.TopImage);
            this.Children.Add(topImage);
            
            rightImage = new DockingGuideImageElement();
            rightImage.Class = "RightImage";
            rightImage.Initialize(template.RightImage);
            this.Children.Add(rightImage);
            
            bottomImage = new DockingGuideImageElement();
            bottomImage.Class = "BottomImage";
            bottomImage.Initialize(template.BottomImage);
            this.Children.Add(bottomImage);
            
            fillImage = new DockingGuideImageElement();
            fillImage.Class = "FillImage";
            fillImage.Initialize(template.FillImage);
            this.Children.Add(fillImage);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            DockingGuidesTemplate template = PredefinedDockingGuidesTemplate.ControlDefault;

            DockingHintBackColor = template.DockingHintBackColor;
            DockingHintBorderColor = template.DockingHintBorderColor;
            DockingHintBorderWidth = template.DockingHintBorderWidth;
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);

            foreach (DockingGuideImageElement guideImage in this.Children)
            {
                desiredSize.Width = Math.Max(desiredSize.Width, guideImage.LocationOnCenterGuide.X + guideImage.DesiredSize.Width);
                desiredSize.Height = Math.Max(desiredSize.Height, guideImage.LocationOnCenterGuide.Y + guideImage.DesiredSize.Height);
            }

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            float x = (finalSize.Width - DesiredSize.Width) / 2;
            float y = (finalSize.Height - DesiredSize.Height) / 2;

            CenterBackgroundImage.Arrange(GetImageRect(x, y, CenterBackgroundImage));
            LeftImage.Arrange(GetImageRect(x, y, LeftImage));
            RightImage.Arrange(GetImageRect(x, y, RightImage));
            TopImage.Arrange(GetImageRect(x, y, TopImage));
            BottomImage.Arrange(GetImageRect(x, y, BottomImage));
            FillImage.Arrange(GetImageRect(x, y, FillImage));

            return finalSize;
        }

        private RectangleF GetImageRect(float x, float y, DockingGuideImageElement image)
        {
            return new RectangleF(new PointF(x + image.LocationOnCenterGuide.X, y + image.LocationOnCenterGuide.Y), image.DesiredSize);
        }

        #endregion
    }
}
