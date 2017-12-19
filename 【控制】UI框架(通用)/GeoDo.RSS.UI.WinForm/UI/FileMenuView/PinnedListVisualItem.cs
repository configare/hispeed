using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using System.Drawing;

namespace GeoDo.RSS.UI.WinForm
{
    public class PinnedListVisualItem : RadListVisualItem
    {
        private RadButtonElement pinImage;

        public PinnedListVisualItem()
        {
            AdjustVisibility();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            pinImage = new RadButtonElement();
            pinImage.Alignment = ContentAlignment.MiddleCenter;
            pinImage.ImageAlignment = ContentAlignment.MiddleCenter;
            pinImage.BorderElement.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            pinImage.ButtonFillElement.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            pinImage.RadPropertyChanged += new Telerik.WinControls.RadPropertyChangedEventHandler(pinImage_RadPropertyChanged);
            this.Children.Add(pinImage);
        }

        void pinImage_RadPropertyChanged(object sender, Telerik.WinControls.RadPropertyChangedEventArgs e)
        {
            if (e.Property == ContainsMouseProperty)
            {
                AdjustVisibility();
            }
        }

        private void AdjustVisibility()
        {
            if (pinImage.ContainsMouse)
            {
                pinImage.BorderElement.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                pinImage.ButtonFillElement.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            }
            else
            {
                pinImage.BorderElement.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                pinImage.ButtonFillElement.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            pinImage.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            finalSize.Width -= 36;
            base.ArrangeOverride(finalSize);
            finalSize.Width += 36;
            pinImage.Arrange(new RectangleF(finalSize.Width - 36, 0, 36, 36));

            return finalSize;
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadListVisualItem);
            }
        }
    }
}
