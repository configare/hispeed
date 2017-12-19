using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.WinForm;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class ArrayToBmp
    {
        public static Bitmap ToBitmap<T>(T[] buffer, int width, int height, string colorTableName)
        {
            //string colorTableName = string.Format("Cloudsat.{0}", "2B-GEOPROF.Radar_Reflectivity");
            ProductColorTable productColorTable = ProductColorTableFactory.GetColorTable(colorTableName);
            if (productColorTable == null)
                return null;
            RgbStretcherProvider stretcherProvier = new RgbStretcherProvider();
            ColorMapTable<int> colorMapTable = null;
            Func<T, byte> stretcher = null;
            IBitmapBuilder<T> builder = null;
            TypeCode t = Type.GetTypeCode(default(T).GetType());
            if (t == TypeCode.Single)
            {
                stretcher = stretcherProvier.GetStretcher(enumDataType.Float, productColorTable, out colorMapTable) as Func<T, byte>;
                builder = BitmapBuilderFactory.CreateBitmapBuilderFloat() as IBitmapBuilder<T>;
            }
            else if (t == TypeCode.Int16)
            {
                stretcher = stretcherProvier.GetStretcher(enumDataType.Int16, productColorTable, out colorMapTable) as Func<T, byte>;
                builder = BitmapBuilderFactory.CreateBitmapBuilderInt16() as IBitmapBuilder<T>;
            }
            Bitmap bitmap = null;
            bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            bitmap.Palette = BitmapBuilderFactory.GetDefaultGrayColorPalette();
            try
            {
                builder.Build(width, height, buffer, stretcher, ref bitmap);
                if (colorMapTable != null)
                {
                    ColorPalette plt = BitmapBuilderFactory.GetDefaultGrayColorPalette();
                    for (int i = 0; i < 256; i++)
                        plt.Entries[i] = Color.Black;
                    int idx = 1;
                    foreach (ColorMapItem<int> item in colorMapTable.Items)
                    {
                        for (int v = item.MinValue; v < item.MaxValue; v++)
                            plt.Entries[idx] = item.Color;
                        idx++;
                    }
                    bitmap.Palette = plt;
                }
                return bitmap;
            }
            finally
            {
            }
        }

        public static Bitmap RasterToBmp(IRasterDataProvider dataPrd, int bandNum, string colorTableName)
        {
            if (dataPrd.DataType == enumDataType.Float)
            {
                float[] oriData = GetDataValue<float>(dataPrd, bandNum);
                return ArrayToBmp.ToBitmap<float>(oriData, dataPrd.Width, dataPrd.Height, colorTableName);
            }
            else if (dataPrd.DataType == enumDataType.Int16)
            {
                short[] oriData = GetDataValue<short>(dataPrd, bandNum);
                return ArrayToBmp.ToBitmap<short>(oriData, dataPrd.Width, dataPrd.Height, colorTableName);
            }
            return null;
        }

        private unsafe static T[] GetDataValue<T>(IRasterDataProvider dataPrd, int bandNum)
        {
            int width = dataPrd.Width;
            int height = dataPrd.Height;
            int length = width * height;
            enumDataType dataType = dataPrd.DataType;
            IRasterBand band;
            switch (dataType)
            {
                case enumDataType.Float:
                    {
                        float[] buffer = new float[width * height];
                        band = dataPrd.GetRasterBand(bandNum);
                        fixed (float* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Float, dataPrd.Width, dataPrd.Height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] buffer = new short[width * height];
                        band = dataPrd.GetRasterBand(bandNum);
                        fixed (short* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Int16, dataPrd.Width, dataPrd.Height);
                        }
                        return buffer as T[];
                    }
            }
            return null;
        }
    }
}
