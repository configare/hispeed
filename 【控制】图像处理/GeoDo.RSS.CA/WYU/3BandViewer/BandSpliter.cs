using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.CA
{
    public class BandSpliter
    {
        private ColorPalette _palette = null;

        public BandSpliter()
        {
            _palette = GetColorPlatte((new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Palette); 
        }

        public ColorPalette DefaultColorPalette
        {
            get { return _palette; }
        }

        public void SplitBitmaps(Bitmap bm,ref Bitmap redBitmap,ref Bitmap greenBitmap,ref Bitmap blueBitmap)
        {
            CopyBands(bm, redBitmap, greenBitmap, blueBitmap);
        }

        private ColorPalette GetColorPlatte(ColorPalette palette)
        {
            for (int i = 0; i < 256; i++)
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            return palette;
        }

        private unsafe void CopyBands(Bitmap obitmap, Bitmap bmRed, Bitmap bmGreen, Bitmap bmBlue)
        {
            BitmapData bmdata = obitmap.LockBits(new Rectangle(0, 0, obitmap.Width, obitmap.Height), ImageLockMode.ReadOnly, obitmap.PixelFormat);
            //
            BitmapData reddata = bmRed.LockBits(new Rectangle(0, 0, bmRed.Width, bmRed.Height), ImageLockMode.ReadWrite, bmRed.PixelFormat);
            BitmapData greendata = bmGreen.LockBits(new Rectangle(0, 0, bmGreen.Width, bmGreen.Height), ImageLockMode.ReadWrite, bmGreen.PixelFormat);
            BitmapData bluedata = bmBlue.LockBits(new Rectangle(0, 0, bmBlue.Width, bmBlue.Height), ImageLockMode.ReadWrite, bmBlue.PixelFormat);
            byte* targetRedPtr = (byte*)reddata.Scan0;
            byte* targetGreenPtr = (byte*)greendata.Scan0;
            byte* targetBluePtr = (byte*)bluedata.Scan0;
            //
            byte* ptr = (byte*)bmdata.Scan0;
            for (int r = 0; r < bmdata.Height; r++, ptr = (byte*)bmdata.Scan0 + r * bmdata.Stride,
                                                                     targetRedPtr = (byte*)reddata.Scan0 + r * reddata.Stride,
                                                                     targetGreenPtr = (byte*)greendata.Scan0 + r * greendata.Stride,
                                                                     targetBluePtr = (byte*)bluedata.Scan0 + r * bluedata.Stride)
            {
                for (int c = 0; c < bmdata.Width; c++, ptr += 3, targetRedPtr++, targetGreenPtr++, targetBluePtr++)
                {
                    *targetBluePtr = *ptr;
                    *targetGreenPtr = *(ptr + 1);
                    *targetRedPtr = *(ptr + 2);
                }
            }
            //
            bmRed.UnlockBits(reddata);
            bmGreen.UnlockBits(greendata);
            bmBlue.UnlockBits(bluedata);
            //            
            obitmap.UnlockBits(bmdata);
        }
    }
}
