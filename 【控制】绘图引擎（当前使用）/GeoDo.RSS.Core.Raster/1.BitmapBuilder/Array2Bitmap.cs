using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class Array2Bitmap<T> : IArray2Bitmap<T>, IDisposable
    {
        public unsafe void ToBitmap(int[] indexes, Size size, Dictionary<T, Color> colorMap, ref Bitmap bitmap)
        { 
        }

        public unsafe void ToBitmap(T[] data, Size size, Dictionary<T, Color> colorMap, ref Bitmap bitmap)
        {
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            try
            {
                IntPtr scan0 = bdata.Scan0;
                Color color = Color.Transparent;
                int idx = 0;
                int stride = bdata.Stride;
                byte* ptr0 = (byte*)scan0.ToPointer();
                byte* ptr = ptr0;
                for (int r = 0; r < size.Height; r++, ptr = ptr0 + r * stride)
                {
                    for (int c = 0; c < size.Width; c++, ptr += 4,idx++)
                    {
                        if (colorMap.ContainsKey(data[idx]))
                            color = colorMap[data[idx]];
                        else
                            color = Color.Transparent;
                        *ptr = color.B;
                        *(ptr + 1) = color.G;
                        *(ptr + 2) = color.R;
                        *(ptr + 3) = color.A;
                    }
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
