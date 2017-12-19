using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.VideoMark
{
    public static class ImageProcessClass
    {
        public static bool ImageProcess(Image[] images, Size size, out Size oSize)
        {
            oSize = size;
            if (size == null || size == Size.Empty)
                oSize = new Size(images[0].Width, images[0].Height);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] == null)
                {
                    images[i] = new Bitmap(oSize.Width, oSize.Height);
                    continue;
                }
                if (images[i].Width != oSize.Width || images[i].Height != oSize.Height)
                {
                    images[i] = GetVaildImage(images[i], oSize);
                }
            }
            return true;
        }

        public static bool ImageProcess(ref Image images, Size size, out Size oSize)
        {
            oSize = size;
            if (size == null || size == Size.Empty)
                oSize = new Size(images.Width, images.Height);
            if (images == null)
                images = new Bitmap(oSize.Width, oSize.Height);
            else if (images.Width != oSize.Width || images.Height != oSize.Height)
                images = GetVaildImage(images, oSize);

            return true;
        }

        private static Image GetVaildImage(Image resImage, Size targetSize)
        {
            float scaleX = (float)resImage.Width / targetSize.Width;
            float scaleY = (float)resImage.Height / targetSize.Height;

            float scale = Math.Max(scaleX, scaleY);

            Bitmap newBitmap = new Bitmap(targetSize.Width, targetSize.Height);
            try
            {
                if (scale != 0)
                    using (Graphics g = Graphics.FromImage(newBitmap))
                    {
                        Matrix m = new Matrix();
                        m.Scale(1 / scale, 1 / scale);
                        g.Transform = m;
                        g.DrawImage(resImage, new Point(0, 0));
                    }
            }
            finally
            {
                resImage.Dispose();
            }
            return newBitmap;
        }
    }
}
