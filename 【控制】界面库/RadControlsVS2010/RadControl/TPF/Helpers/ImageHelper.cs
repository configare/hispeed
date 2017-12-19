using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    /// <summary>
    /// Provides common helper methods related with image manipulation.
    /// TODO: Should be moved to base assembly, making it accessible for all Telerik Assemblies.
    /// </summary>
    public static class ImageHelper
    {
        private const int OleHeaderSize = 78;

        public static void ApplyAlpha(Bitmap bitmap, float fAlpha)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte nScale = (byte)(fAlpha * 255);

                IntPtr SrcScan0 = bitmapData.Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                pSrc += 3;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        *pSrc = (byte)((int)(*pSrc * nScale) / 255);
                        pSrc += 4;
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);
        }

        public static void ApplyMask(Bitmap bitmap, Brush brush)
        {
            Rectangle imageRectangle = new Rectangle(Point.Empty, bitmap.Size);

            using (Bitmap masktImage = new Bitmap(bitmap.Size.Width, bitmap.Size.Height))
            {
                using (Graphics graphics = Graphics.FromImage(masktImage))
                {
                    graphics.FillRectangle(brush, imageRectangle);
                }

                BitmapData imageData = bitmap.LockBits(imageRectangle, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                BitmapData gradientImageData = masktImage.LockBits(imageRectangle, ImageLockMode.ReadOnly, masktImage.PixelFormat);

                for (int y = 0; y < masktImage.Height; y++)
                {
                    for (int x = 0; x < masktImage.Width; x++)
                    {
                        byte imageBit = Marshal.ReadByte(gradientImageData.Scan0, (gradientImageData.Stride * y) + (4 * x));
                        Marshal.WriteByte(imageData.Scan0, (imageData.Stride * y) + (4 * x) + 3, imageBit);
                    }
                }

                bitmap.UnlockBits(imageData);
                masktImage.UnlockBits(gradientImageData);
            }
        }

        /// <summary>
        /// Crops recatnalge from image 
        /// </summary>
        /// <param name="image">An instance of <see cref="Bitmap"/>.</param>
        /// <param name="cropRectangle">An instance of <see cref="Rectangle"/></param>
        /// <returns>Cropped image with the size of cropped rectangle</returns>
        public static Bitmap Crop(Bitmap image, Rectangle cropRectangle)
        {
            int width = cropRectangle.Width;
            int height = cropRectangle.Height;
            int x = cropRectangle.X;
            int y = cropRectangle.Y;
            Bitmap croppedImage = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(image, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
            }

            return croppedImage;
        }

        public static Region RegionFromBitmap(Bitmap bmp, Color transparent)
        {
            GraphicsPath path = GetBitmapRegionPath(bmp, transparent);
            Region reg = new Region(path);
            path.Dispose();

            return reg;
        }

        public static GraphicsPath GetBitmapRegionPath(Bitmap bmp, Color transparent)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int beginX;

            GraphicsPath path = new GraphicsPath();
            int transparentArgb = transparent.ToArgb();
            int pixel;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    pixel = bmp.GetPixel(i, j).ToArgb();
                    if (pixel == transparentArgb)
                        continue;

                    beginX = i;

                    while ((i < width) && (pixel != transparentArgb))
                        i++;

                    path.AddRectangle(new Rectangle(beginX, j, i - beginX, 1));
                }
            }

            return path;
        }

        public static bool PointInRegion(Region region, Point client)
        {
            Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);

            IntPtr hRgn = region.GetHrgn(g);
            bool contains = NativeMethods.PtInRegion(hRgn, client.X, client.Y);
            region.ReleaseHrgn(hRgn);

            g.Dispose();
            bmp.Dispose();

            return contains;
        }
      
        public static Image GetImageFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            Image result = null;
            MemoryStream stream = null;

            try
            {
                int count;
                int start;

                if (IsOleContainer(bytes))
                {
                    count = bytes.Length - OleHeaderSize;
                    start = OleHeaderSize;
                }
                else
                {
                    count = bytes.Length;
                    start = 0;
                }

                stream = new MemoryStream(bytes, start, count);
                result = Image.FromStream(stream);
            }
            catch
            {
                result = null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return result;
        }

        private static bool IsOleContainer(byte[] imageData)
        {
            return (imageData != null
                && imageData[0] == 0x15
                && imageData[1] == 0x1C);
        }
    }
}
