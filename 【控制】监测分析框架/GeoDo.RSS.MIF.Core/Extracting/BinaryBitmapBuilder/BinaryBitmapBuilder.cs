using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Core
{
    public class BinaryBitmapBuilder : IBinaryBitmapBuilder,IDisposable
    {
        public Bitmap CreateBinaryBitmap(System.Drawing.Size size, Color trueColor, Color falseColor)
        {
            try
            {
                Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format8bppIndexed);
                ColorPalette cp = bitmap.Palette;
                cp.Entries[1] = trueColor;
                cp.Entries[0] = falseColor;
                for (int i = 2; i < 256; i++)
                    cp.Entries[i] = falseColor;
                bitmap.Palette = cp;
                return bitmap;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message + ",不支持创建过大的影像" + size.Width + "x" + size.Height, ex);
            }
        }

        public unsafe void Reset(System.Drawing.Size size, ref System.Drawing.Bitmap bitmap)
        {
            float scaleX = bitmap.Width / (float)size.Width;
            float scaleY = bitmap.Height / (float)size.Height;
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            try
            {
                IntPtr scan0 = bdata.Scan0;
                int stride = bdata.Stride;
                byte* ptr0 = (byte*)scan0.ToPointer();
                int count = stride * bitmap.Height;
                for (int i = 0; i < count; i++,ptr0++)
                    *ptr0 = 0;
            }
            finally
            {
                bitmap.UnlockBits(bdata);
            }
        }

        public unsafe void Fill(int[] indexes, System.Drawing.Size size, ref System.Drawing.Bitmap bitmap)
        {
            float scaleX = bitmap.Width / (float)size.Width;
            float scaleY = bitmap.Height / (float)size.Height;
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            try
            {
                IntPtr scan0 = bdata.Scan0;
                int stride = bdata.Stride;
                byte* ptr0 = (byte*)scan0.ToPointer();
                byte* ptr = ptr0;
                int count = indexes.Length;
                int r = 0, c = 0;
                int w = size.Width;
                int h = size.Height;
                for (int i = 0; i < count; i++)
                {
                    r = indexes[i] / w;
                    c = indexes[i] - r * w;
                    r = (int)(r * scaleY);
                    c = (int)(c * scaleX);
                    ptr = ptr0 + r * stride + c;
                    *ptr = 1;//true value         
                }
            }
            finally
            {
                bitmap.UnlockBits(bdata);
            }
        }

        public void Dispose()
        {
        }
    }
}
