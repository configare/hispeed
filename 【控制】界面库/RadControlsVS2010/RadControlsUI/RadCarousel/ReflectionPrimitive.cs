using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.Primitives
{

    /// <summary><para>Represents a RadCarouselReflectionItem primitive that is drawn on the screen.</para></summary>
    public class ReflectionPrimitive : BasePrimitive, IDisposable
    {
        private RadElement ownerElement = null;

        private Bitmap reflectionBitmap;
        private bool reflectionSourceBitmapChanged;
        private Bitmap reflectionSourceBitmap;

        private double itemReflectionPercentage;




        /// <summary>
        /// Default cstor for RadCarouselReflectionPrimitive
        /// </summary>
        /// <param name="ownerElement">which element will be draw</param>
        public ReflectionPrimitive(RadElement ownerElement)
        {
            this.OwnerElement = ownerElement;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

        static public Bitmap CopyBitmap(Bitmap srcBitmap, Rectangle section)
        {
            // Create the new bitmap and associated graphics object
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);
            
            // Draw the specified section of the source bitmap to the new one
            if (srcBitmap == null)
            {
                srcBitmap = new Bitmap(section.Width, section.Height);
            }

            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);

            // Clean up
            g.Dispose();

            // Return the bitmap
            return bmp;
        }

        
        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            base.PaintElement(graphics, angle, scale);

            if (this.ownerElement == null)
            {
                return;
            }

            if (this.reflectionSourceBitmapChanged)
            {
                if (this.reflectionSourceBitmap == null)
                {
                    return;
                }

                int y = (int)Math.Round((reflectionSourceBitmap.Height * this.itemReflectionPercentage));
                if (y == 0)
                {
                    return;
                }


                reflectionSourceBitmap =
                    this.ownerElement.GetAsTransformedBitmap(Brushes.Transparent, this.OwnerElement.AngleTransform,this.OwnerElement.ScaleTransform);

                if (reflectionSourceBitmap == null)
                    return;

                reflectionSourceBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                if (reflectionBitmap != null)
                {
                    reflectionBitmap.Dispose();
                    reflectionBitmap = null; 
                }

                Rectangle boundingSize = this.ownerElement.ControlBoundingRectangle;
                Size destSize = new Size(
                Math.Max(1, boundingSize.Size.Width),
                Math.Max(1, (int)Math.Round(boundingSize.Size.Height * this.ItemReflectionPercentage)));

                if (destSize.Width == 0 || destSize.Height == 0)
                {
                    return;
                }

                Rectangle destRectangle;

                if (this.ownerElement.Shape != null)
                {
                    destRectangle = new Rectangle(1, 1, destSize.Width - 1, destSize.Height - 1);
                }
                else
                {
                    destRectangle = new Rectangle(0, 0, destSize.Width, destSize.Height);
                }

                reflectionBitmap = CopyBitmap(reflectionSourceBitmap, destRectangle);

                ApplyReflectionGradientFade(reflectionBitmap, 1d);
                this.UpdateReflectionImage(reflectionBitmap);
                reflectionSourceBitmapChanged = false;
            }

            if (reflectionBitmap != null)
            {
                graphics.DrawBitmap(reflectionBitmap, 0, 0);
            }
        }


        private static void ApplyReflectionGradientFade(Bitmap bitmap, double initialOpacity)
        {
            Rectangle bitmapBounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(bitmapBounds, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            float maxTransparency = 130f * (float)initialOpacity;

            try
            {
                for (int y = 0; y <= bitmapData.Height - 1; y++)
                {
                    for (int x = 0; x <= bitmapData.Width - 1; x++)
                    {
                        Color sourcePixel =
                            Color.FromArgb(Marshal.ReadInt32(bitmapData.Scan0, (bitmapData.Stride * y) + (4 * x)));

                        int a = (int)(sourcePixel.A / 255f *
                            (maxTransparency - (maxTransparency * (y / ((float)bitmapBounds.Height)))));

                        Marshal.WriteInt32(bitmapData.Scan0, (bitmapData.Stride * y) + (4 * x),
                                           Color.FromArgb(a, sourcePixel).ToArgb());
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }


        /// <summary>
        /// repaint Reflection Image
        /// </summary>
        public void UpdateReflectionImage(Bitmap itemBitmap)
        {
            this.reflectionSourceBitmapChanged = true;

            if (reflectionSourceBitmap != null)
            {
                reflectionSourceBitmap.Dispose();
                reflectionSourceBitmap = null;
            }

            this.reflectionSourceBitmap = itemBitmap;
        }

        protected override void DisposeManagedResources()
        {
            if (reflectionBitmap != null)
                reflectionBitmap.Dispose();

            if (reflectionSourceBitmap != null)
                reflectionSourceBitmap.Dispose();

            if (this.ownerElement != null)
            {
                this.ownerElement.ElementPainted -= HostedItem_ElementPainted;
            }

            base.DisposeManagedResources();
        }

        protected internal override void OnBeginDispose()
        {
            base.OnBeginDispose();

            if (this.ElementTree != null)
            {
                ((RadControl)this.ElementTree.Control).ElementInvalidated -= CarouselContentItem_ElementInvalidated;
            }
        }

        protected override void OnElementTreeChanged(ComponentThemableElementTree previousTree)
        {
            base.OnElementTreeChanged(previousTree);

            if (previousTree != null)
            {
                ((RadControl)previousTree.Control).ElementInvalidated -= CarouselContentItem_ElementInvalidated;
            }

            if (this.ElementTree != null)
            {
                ((RadControl)this.ElementTree.Control).ElementInvalidated += CarouselContentItem_ElementInvalidated;
            }
        }

        /// <summary>
        /// Represent ItemReflectionPercentage
        /// </summary>
        public double ItemReflectionPercentage
        {
            get { return itemReflectionPercentage; }
            set { itemReflectionPercentage = value; }
        }

        /// <summary>
        /// ElementInvalidated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CarouselContentItem_ElementInvalidated(object sender, EventArgs e)
        {
            if (this.OwnerElement != null)
            {
                RadElement invalidated = sender as RadElement;
                for (RadElement currentItem = invalidated; currentItem != null; currentItem = currentItem.Parent)
                {
                    if (currentItem == this.OwnerElement)
                    {
                        this.Invalidate();
                        break;
                    }
                }
            }
        }

        ///// <summary>
        ///// represent the OwnerElement
        ///// </summary>
        public RadElement OwnerElement
        {
            get
            {
                return this.ownerElement;
            }
            set
            {
                if (this.ownerElement != null)
                {
                    this.ownerElement.ElementPainted -= new System.Windows.Forms.PaintEventHandler(HostedItem_ElementPainted);
                }

                if (this.ownerElement != value)
                {
                    this.ownerElement = value;

                    if (this.ownerElement != null)
                    {
                        this.ownerElement.ElementPainted +=
                            new System.Windows.Forms.PaintEventHandler(HostedItem_ElementPainted);
                    }
                }
            }
        }

        private void HostedItem_ElementPainted(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Rectangle rect = this.ownerElement.ControlBoundingRectangle;
           
            Size destSize = new Size(
                Math.Max(1, rect.Size.Width),
                Math.Max(1, (int)Math.Round(rect.Height * this.ItemReflectionPercentage))
            );
            
            Bitmap destBmp = new Bitmap(destSize.Width, destSize.Height);
            this.UpdateReflectionImage(destBmp);
        }
    }
}
