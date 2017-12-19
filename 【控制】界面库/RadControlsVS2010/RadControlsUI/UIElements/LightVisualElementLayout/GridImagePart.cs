using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class ImagePart : VisualElementLayoutPart
    {
        public ImagePart(LightVisualElement owner) : base(owner) { }

        public override SizeF Measure(SizeF availableSize)
        {
            if (this.Owner.Image != null)
            {
                if (this.Owner.ImageLayout == System.Windows.Forms.ImageLayout.Zoom)
                {
                    float ratioX = (float)availableSize.Width / this.Owner.Image.Width;
                    float ratioY = (float)availableSize.Height / this.Owner.Image.Height;
                    float factor = Math.Min(ratioX, ratioY);
                    int finalWidth = (int)Math.Round(this.Owner.Image.Width * factor);
                    int finalHeight = (int)Math.Round(this.Owner.Image.Height * factor);
                    this.desiredSize = new SizeF(finalWidth, finalHeight);
                }
                else
                {
                    this.desiredSize = new SizeF(this.Owner.Image.Size.Width, this.Owner.Image.Size.Height);
                }
            }
            else
            {
                this.desiredSize = SizeF.Empty;
            }

            this.desiredSize.Width = Math.Min(availableSize.Width, this.desiredSize.Width);
            this.desiredSize.Height = Math.Min(availableSize.Height, this.desiredSize.Height);
            return this.desiredSize;
        }

        
    }
}
