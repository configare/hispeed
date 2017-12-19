using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;

namespace UnitTest
{
    [TestFixture]
    public unsafe class UnitTest_BitmapBuilder
    {

        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"F:\技术研究\MAS_II\源代码\【控制】绘图引擎\DefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        public void ReadRasterFile(string fname, out int width, out int height, out UInt16[] redBuffer, out UInt16[] greenBuffer, out UInt16[] blueBuffer)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
            width = prd.Width;
            height = prd.Height;
            redBuffer = new UInt16[prd.Width * prd.Height];
            greenBuffer = new UInt16[prd.Width * prd.Height];
            blueBuffer = new UInt16[prd.Width * prd.Height];
            fixed (UInt16* redPtr = redBuffer, greenPtr = greenBuffer, bluePtr = blueBuffer)
            {
                prd.GetRasterBand(1).Read(0, 0, prd.Width, prd.Height, new IntPtr(redPtr), enumDataType.UInt16, prd.Width, prd.Height);
                prd.GetRasterBand(2).Read(0, 0, prd.Width, prd.Height, new IntPtr(greenPtr), enumDataType.UInt16, prd.Width, prd.Height);
                prd.GetRasterBand(3).Read(0, 0, prd.Width, prd.Height, new IntPtr(bluePtr), enumDataType.UInt16, prd.Width, prd.Height);
            }
            prd.Dispose();
        }

        public void ReadRasterFile(string fname, int red, int green, int blue, out int width, out int height, out UInt16[] redBuffer, out UInt16[] greenBuffer, out UInt16[] blueBuffer)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
            width = prd.Width;
            height = prd.Height;
            redBuffer = new UInt16[prd.Width * prd.Height];
            greenBuffer = new UInt16[prd.Width * prd.Height];
            blueBuffer = new UInt16[prd.Width * prd.Height];
            fixed (UInt16* redPtr = redBuffer, greenPtr = greenBuffer, bluePtr = blueBuffer)
            {
                prd.GetRasterBand(red).Read(0, 0, prd.Width, prd.Height, new IntPtr(redPtr), enumDataType.UInt16, prd.Width, prd.Height);
                prd.GetRasterBand(green).Read(0, 0, prd.Width, prd.Height, new IntPtr(greenPtr), enumDataType.UInt16, prd.Width, prd.Height);
                prd.GetRasterBand(blue).Read(0, 0, prd.Width, prd.Height, new IntPtr(bluePtr), enumDataType.UInt16, prd.Width, prd.Height);
            }
            prd.Dispose();
        }

        public void BitmapToArray(string fname, out int width, out int height, out byte[] redBuffer, out byte[] greenBuffer, out byte[] blueBuffer)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
            width = prd.Width;
            height = prd.Height;
            redBuffer = new byte[prd.Width * prd.Height];
            greenBuffer = new byte[prd.Width * prd.Height];
            blueBuffer = new byte[prd.Width * prd.Height];
            fixed (byte* redPtr = redBuffer, greenPtr = greenBuffer, bluePtr = blueBuffer)
            {
                prd.GetRasterBand(1).Read(0, 0, prd.Width, prd.Height, new IntPtr(redPtr), enumDataType.Byte, prd.Width, prd.Height);
                prd.GetRasterBand(2).Read(0, 0, prd.Width, prd.Height, new IntPtr(greenPtr), enumDataType.Byte, prd.Width, prd.Height);
                prd.GetRasterBand(3).Read(0, 0, prd.Width, prd.Height, new IntPtr(bluePtr), enumDataType.Byte, prd.Width, prd.Height);
            }
            prd.Dispose();
        }

        [Test]
        public void DefaultRGB()
        {
            string fname = "f:\\Penguins.jpg";
            //fname = "f:\\4_大昭寺_IMG_GE.tif";
            UInt16[] rBuffer = null, gBuffer = null, bBuffer = null;
            int width = 0, height = 0;
            ReadRasterFile(fname, out width, out height, out rBuffer, out gBuffer, out bBuffer);
            Console.WriteLine("Width:" + width.ToString() + ",Height:" + height.ToString());
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (IBitmapBuilder<UInt16> builder = BitmapBuilderFactory.CreateBitmapBuilderUInt16())
            {
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                //bitmap.Palette = _colorPalette;
                builder.Build(width, height, rBuffer, gBuffer, bBuffer, ref bitmap);
                //builder.Build(width, height, gBuffer, ref bitmap);
                bitmap.Save(fname + ".bmp", ImageFormat.Bmp);
                bitmap.Dispose();
            }
            sw.Stop();
            Console.WriteLine("Lost Time:" + sw.ElapsedMilliseconds.ToString());
            //MessageBox.Show(sw.ElapsedMilliseconds.ToString());
        }

        [Test]
        public void StretchRGB()
        {
            string fname = "f:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            UInt16[] rBuffer = null, gBuffer = null, bBuffer = null;
            int width = 0, height = 0;
            //BitmapToArray(fname, 3, 4, 2, out width, out height, out rBuffer, out gBuffer, out bBuffer);
            ReadRasterFile(fname, 6, 4, 3, out width, out height, out rBuffer, out gBuffer, out bBuffer);
            Console.WriteLine("Width:" + width.ToString() + ",Height:" + height.ToString());
            IRgbStretcher<UInt16> rgbs = new LinearRgbStretcherUInt16(0, 1000,true);
            Func<UInt16, byte> stretcher = (rgbs).Stretcher;
            using (IBitmapBuilder<UInt16> builder = BitmapBuilderFactory.CreateBitmapBuilderUInt16())
            {
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                //
                Stopwatch sw = new Stopwatch();
                sw.Start();
                //
                builder.Build(width, height, rBuffer, gBuffer, bBuffer, stretcher, stretcher, stretcher, ref bitmap);
                sw.Stop();
                Console.WriteLine("Lost Time:" + sw.ElapsedMilliseconds.ToString());
                MessageBox.Show(sw.ElapsedMilliseconds.ToString());
                bitmap.Save(fname + ".bmp", ImageFormat.Bmp);
                bitmap.Dispose();
            }
        }
    }
}
