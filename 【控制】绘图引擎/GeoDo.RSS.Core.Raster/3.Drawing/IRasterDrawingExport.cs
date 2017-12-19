using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.CA;
using System.IO;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IRasterDrawingExport
    {
        Bitmap GetBitmapUseOriginResolution();
    }

    public static class RasterDrawingExporter
    {
        public static Bitmap GetBitmapUseOriginResolution(this IRasterDrawing drawing)
        {
            IOverviewGenerator gen = drawing.DataProviderCopy as IOverviewGenerator;
            if (drawing.SelectedBandNos == null)
                return null;
            Bitmap bm = null;
            if (drawing.SelectedBandNos.Length == 1)
            {
                bm = new Bitmap(drawing.DataProviderCopy.Width, drawing.DataProviderCopy.Height, PixelFormat.Format8bppIndexed);
                bm.Palette = BitmapBuilderFactory.GetDefaultGrayColorPalette();
            }
            else
            {
                try
                {
                    bm = new Bitmap(drawing.DataProviderCopy.Width, drawing.DataProviderCopy.Height, PixelFormat.Format24bppRgb);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("无法创建过大的Bitmap[{0}*{1}]", drawing.DataProviderCopy.Width, drawing.DataProviderCopy.Height), ex.InnerException);
                }
            }
            string ext = Path.GetExtension(drawing.FileName).ToLower();
            if (ext == ".bmp" || ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                bm = (Bitmap)Bitmap.FromFile(drawing.FileName);
            else
                gen.Generate(drawing.SelectedBandNos, ref bm);
            IRgbProcessorStack stack = drawing.RgbProcessorStack;
            stack.Apply(null, bm);
            return bm;
        }
    }
}
