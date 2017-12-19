using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class ColorMapTableConverter<T>
    {
        public ColorPalette GetColorPalette(IRgbStretcher<T> stretcher, ColorMapTable<T> colorMapTable)
        {
            ColorPalette palette = (new Bitmap(1, 1, PixelFormat.Format8bppIndexed)).Palette;
            byte minByte = 0, maxByte = 0;
            Func<T, byte> tobyte = stretcher.Stretcher;
            foreach (ColorMapItem<T> it in colorMapTable.Items)
            {
                minByte = tobyte(it.MinValue);
                maxByte = tobyte(it.MaxValue);
                for (int i = minByte; i < maxByte; i++)
                    palette.Entries[i] = it.Color;
            }
            return palette;
        }
    }
}
