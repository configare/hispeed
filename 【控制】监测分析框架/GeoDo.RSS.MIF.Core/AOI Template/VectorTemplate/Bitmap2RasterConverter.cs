using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Core
{
    public class Bitmap2RasterConverter : IBitmap2RasterConverter
    {
        public unsafe byte[] ToRaster(Bitmap bitmap, Color[] colors)
        {
            BitmapData pdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                int colorCount = colors.Length;
                byte[] retResult = new byte[bitmap.Width * bitmap.Height];
                byte[] colorParts;
                PixelFormat pixelFrt = pdata.PixelFormat;
                int pixelWidth = GetPixelWidth(pdata.PixelFormat, colors[0], out colorParts);
                int width = pdata.Width;
                int height = pdata.Height;
                int stride = pdata.Stride;
                byte* ptr0 = (byte*)pdata.Scan0;
                byte* ptr = ptr0;
                bool isHited = true;
                int idx = 0;
                for (int r = 0; r < height; r++, ptr = ptr0 + r * stride)
                {
                    for (int c = 0; c < width; c++, idx++, ptr =ptr0 + r * stride + c * pixelWidth)
                    {
                        byte* pixelPtr0 = ptr;
                        byte colorIdx = 0;
                        for (byte iColor = 0; iColor < colorCount; iColor++,ptr = pixelPtr0)
                        {
                            isHited = true;
                            colorIdx = iColor;
                            ColorToBytes(pixelFrt, colors[iColor], colorParts);
                            for (int b = 0; b < pixelWidth; b++, ptr++)
                            {
                                if (colorParts[b] != *ptr)
                                {
                                    isHited = false;
                                    goto nextColorLine;
                                }
                            }
                            if (isHited)
                                break;
                        nextColorLine:
                            ;
                        }
                        if (isHited)
                            retResult[idx] = colorIdx;
                    }
                }
                return retResult;
            }
            finally
            {
                bitmap.UnlockBits(pdata);
            }

        }

        public unsafe int[] ToRaster(Bitmap bitmap, Color interestedColor)
        {
            BitmapData pdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                List<int> idxs = new List<int>();
                byte[] colorParts;
                int pixelWidth = GetPixelWidth(pdata.PixelFormat, interestedColor, out colorParts);
                int width = pdata.Width;
                int height = pdata.Height;
                int stride = pdata.Stride;
                byte* ptr0 = (byte*)pdata.Scan0;
                byte* ptr = ptr0;
                bool isHited = true;
                int idx = 0;
                for (int r = 0; r < height; r++, ptr = ptr0 + r * stride)
                {
                    for (int c = 0; c < width; c++,idx++)
                    {
                        isHited = true;
                        for (int b = 0; b < pixelWidth; b++,ptr ++)
                        {
                            if (colorParts[b] != *ptr)
                                isHited = false;
                        }
                        if (isHited)
                            idxs.Add(idx);
                    }
                }
                return idxs.Count > 0 ? idxs.ToArray() : null;
            }
            finally
            {
                bitmap.UnlockBits(pdata);
            }
        }

        private int GetPixelWidth(PixelFormat pixelFormat, Color color, out byte[] colorParts)
        {
            colorParts = null;
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    colorParts = new byte[1];
                    colorParts[0] = color.R == 0 && color.G == 0 && color.B == 0 ? (byte)0 : (byte)1;
                    return 1;
                case PixelFormat.Format24bppRgb:
                    colorParts = new byte[3];
                    colorParts[0] = color.B;
                    colorParts[1] = color.G;
                    colorParts[2] = color.R;
                    return 3;
                case PixelFormat.Format32bppArgb:
                    colorParts = new byte[4];
                    colorParts[0] = color.B;
                    colorParts[1] = color.G;
                    colorParts[2] = color.R;
                    colorParts[3] = color.A;
                    return 4;
                default:
                    throw new NotSupportedException(pixelFormat.ToString());
            }
        }

        private void ColorToBytes(PixelFormat pixelFormat, Color color, byte[] colorParts)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    colorParts[0] = color.R == 0 && color.G == 0 && color.B == 0 ? (byte)0 : (byte)1;
                    break;
                case PixelFormat.Format24bppRgb:
                    colorParts[0] = color.B;
                    colorParts[1] = color.G;
                    colorParts[2] = color.R;
                    break;
                case PixelFormat.Format32bppArgb:
                    colorParts[0] = color.B;
                    colorParts[1] = color.G;
                    colorParts[2] = color.R;
                    colorParts[3] = color.A;
                    break;
                default:
                    throw new NotSupportedException(pixelFormat.ToString());
            }
        }


        public void Dispose()
        {
        }
    }
}
