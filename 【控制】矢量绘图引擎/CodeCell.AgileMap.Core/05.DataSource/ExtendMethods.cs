using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CodeCell.AgileMap.Core
{
    public static class ExtendMethods
    {
        public unsafe static Bitmap BytesToBitmap(this IRasterReader reader, byte[] buffer,int width,int height, int byteCountPerPixel)
        {
            if (buffer.Length != (width * height * byteCountPerPixel))
                throw new ArgumentOutOfRangeException("Buffer的大小与指定的Widht、Height不匹配!");
            if (byteCountPerPixel != 3 && byteCountPerPixel != 4)
                throw new NotSupportedException("不支持的像素格式\""+byteCountPerPixel.ToString()+"\"!");
            PixelFormat pf = GetPixelFormat(byteCountPerPixel);
            Bitmap bitmap = new Bitmap(width, height, pf);
            BytesToBitmap(reader, buffer, width, height, byteCountPerPixel, ref bitmap);
            return bitmap;
        }

        public unsafe static bool BytesToBitmap(this IRasterReader reader, byte[] buffer, int width, int height, int byteCountPerPixel, ref Bitmap bitmap)
        {
            if (buffer.Length != (width * height * byteCountPerPixel))
                throw new ArgumentOutOfRangeException("Buffer的大小与指定的Widht、Height不匹配!");
            if (byteCountPerPixel != 3 && byteCountPerPixel != 4)
                throw new NotSupportedException("不支持的像素格式\"" + byteCountPerPixel.ToString() + "\"!");
            PixelFormat pf = GetPixelFormat(byteCountPerPixel);
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pf);
            IntPtr ptr = bdata.Scan0;
            byte* beginPt = (byte*)ptr;
            byte* pt = (byte*)ptr;
            for (int row = 0; row < bitmap.Height; row++)
            {
                pt = beginPt + row * bdata.Stride;
                int rowAddress = row * bitmap.Width * byteCountPerPixel;
                for (int col = 0; col < bitmap.Width; col++, pt++)
                {
                    int colAddress = rowAddress + col * byteCountPerPixel;
                    if (byteCountPerPixel == 3)
                    {
                        *pt = buffer[colAddress + 2];
                        *(++pt) = buffer[colAddress + 1];
                        *(++pt) = buffer[colAddress];
                    }
                    else if (byteCountPerPixel == 4)
                    {
                        *pt = buffer[colAddress + 3];
                        *(++pt) = buffer[colAddress + 2];
                        *(++pt) = buffer[colAddress + 1];
                        *(++pt) = buffer[colAddress];
                    }
                }
            }
            bitmap.UnlockBits(bdata);
            return true;
        }

        private static PixelFormat GetPixelFormat(int bandCount)
        {
            switch (bandCount)
            { 
                case 1:
                    return PixelFormat.Format8bppIndexed;
                case 2:
                    return PixelFormat.Format16bppGrayScale;
                case 3:
                    return PixelFormat.Format24bppRgb;
                case 4:
                    return PixelFormat.Format32bppArgb;
                default:
                    throw new ArgumentOutOfRangeException("bandCount");
            }
        }
    }
}
