using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.NOAA;
using GeoDo.RSS.DF.NOAA.BandPrd;
using System.IO;
using System.Diagnostics;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class D1BDDriver_Read
    {
        [SetUp]
        public void Init()
        {
        }

        [Test]
        public unsafe void OpenFile()
        {
            string fname = "d:\\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd";
             //GeoDataDriver.Open(fname) as IRasterDataProvider;// ;
             IRasterDataDriver driver =GeoDataDriver.GetDriverByName("NOAA_1BD") as IRasterDataDriver;
            //Assert.NotNull(driver);
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            //GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
            Console.WriteLine(prd.BandCount.ToString());
            IBandProvider bp = prd.BandProvider;
            IRasterBand[] rb = bp.GetBands("SolarZenith");
            Assert.NotNull(rb);
            

            double[] buffer = new double[10004480];
            Stopwatch sw = new Stopwatch();

            fixed (double* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                rb[0].Read(0, 0, 2048, 4885, bufferPtr, enumDataType.Double, 2048, 4885);
                sw.Stop();
                //SecondaryBand_1BD sb = rb[0] as SecondaryBand_1BD;
                //sb.DirectReadGeo(0, 0, 2048, 4885, bufferPtr, enumDataType.Double, 2048, 4885, true);
                //Console.WriteLine(sb.time);
                //Console.WriteLine(sw.ElapsedMilliseconds);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
            //for (int i = 0; i < 5000; i++)
            //{
            //    Console.WriteLine(buffer[i]);
            //}
        }

        private void PrintHeaderInfo(IRasterDataProvider prd)
        {
            Console.WriteLine(prd);
            ID1BDDataProvider noaaPrd = prd as ID1BDDataProvider;
            Assert.NotNull(noaaPrd);
            D1BDHeader header = noaaPrd.Header as D1BDHeader;
            Assert.NotNull(header);
            //Console.WriteLine(noaaPrd.DataType);
            //Console.WriteLine(header.CommonInfoFor1BD.BeginDayFrom1950);
            //Console.WriteLine(header.CommonInfoFor1BD.Current3A);
            //Console.WriteLine(header.CommonInfoFor1BD.CurrentRise);
            //Console.WriteLine(header.CommonInfoFor1BD.DataBeginDayOfYear);
            //Console.WriteLine(header.CommonInfoFor1BD.DataBeginUTC);
            //Console.WriteLine(header.CommonInfoFor1BD.DataBeginYear);
            //Console.WriteLine(header.CommonInfoFor1BD.DataEndDayOfYear);
            //Console.WriteLine(header.CommonInfoFor1BD.DataEndUTC);
            //Console.WriteLine(header.CommonInfoFor1BD.DataEndYear);
            Console.WriteLine(header.CommonInfoFor1BD.FileGenPlace);
            //Console.WriteLine(header.CommonInfoFor1BD.FullRecordCount);
            Console.WriteLine(header.GeographLocationInfoFor1BD.ReferenceEllipse);
        }

        [Test]
        public void ReadGeoInfo()
        {
            string fname = "d:\\0111d2.n16.1bd";
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("NOAA_1BD") as IRasterDataDriver;
            Assert.NotNull(driver);
            IRasterDataProvider prd = driver.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            D1BDDataProvider dp = prd as D1BDDataProvider;
        }

        [Test]
        public void ReadAngleInfo()
        {
            string fname = "d:\\0111d2.n16.1bd";
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("NOAA_1BD") as IRasterDataDriver;
            Assert.NotNull(driver);
            IRasterDataProvider prd = driver.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            D1BDDataProvider dp = prd as D1BDDataProvider;
        }

        [Test]
        public void ReadVisiCoefficient()
        {
            string fname = "d:\\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd";
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("NOAA_1BD") as IRasterDataDriver;
            Assert.NotNull(driver);
            IRasterDataProvider prd = driver.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            D1BDDataProvider dp = prd as D1BDDataProvider;
            double[,] c=null;
            double[,] a = null;
            double[,] b = null;
            dp.ReadVisiCoefficient(ref a,ref b,ref c );
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Console.WriteLine(a[i, j]);
                    //Console.WriteLine(b[i, j]);
                    //Console.WriteLine(c[i, j]);
                }
                Console.WriteLine("\n");
            }
        }

        [Test]
        public void ReadIRCoefficient()
        {
            string fname = "d:\\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd";
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("NOAA_1BD") as IRasterDataDriver;
            Assert.NotNull(driver);
            IRasterDataProvider prd = driver.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            D1BDDataProvider dp = prd as D1BDDataProvider;

            double[,] a = null;
            double[,] b = null;
            dp.ReadIRCoefficient(ref a, ref b);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.WriteLine(a[i, j]);
                }
                Console.WriteLine("\n");
            }
        }

        [Test]
       unsafe public void SetBandProvider()
        {
            string fname = "d:\\0111d2.n16.1bd";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IBandProvider bp = prd.BandProvider;
            IRasterBand[] rb=bp.GetBands("Longitude");
            Console.WriteLine(rb);
            Assert.NotNull(rb);
             UInt16[] buffer = new UInt16[4];
            fixed (UInt16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                rb[0].Read(0, 0, 2, 2, bufferPtr, enumDataType.UInt16, 2, 2);
            }
        }

        [Test]
        public unsafe void TestBandProvider()
        {
            string fname = "d:\\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IBandProvider bp = prd.BandProvider;
            IRasterBand[] rb = bp.GetBands("SolarZenith");
            Assert.NotNull(rb);
            SecondaryBand_1BD sb = rb[0] as SecondaryBand_1BD;

            //double[] buffer = new double[4096];

            //fixed (double* ptr = buffer)
            //{
            //    IntPtr bufferPtr = new IntPtr(ptr);
            //    sb.Read(0, 6000, 2048, 1, bufferPtr, enumDataType.Double, 2048, 1);
            //}
            //for (int i = 0; i < 2048; i++)
            //{
            //    Console.WriteLine(buffer[i]);
            //}

            //Console.WriteLine(rb);
            //List<Int16[]> ta = sb.ReadAngleInfo(200, 1, 0);
            //for (int i = 0; i < 51; i++)
            //{
            //    Console.WriteLine(ta[0][i]);
            //}
            //Int16[] buffer = new Int16[prd.Width * prd.Height];
            //unsafe
            //{
            //    fixed (Int16* ptr = buffer)
            //    {
            //        IntPtr bufferPtr = new IntPtr(ptr);
            //        rb[0].Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.Int16, prd.Width, prd.Height);
            //    }
            //}

        }

        [Test]
        public void GetBandData()
        {

            string fname = "d:\\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.dat";
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            IRasterDataProvider prd = drv.Open(fname,enumDataProviderAccess.ReadOnly) as IRasterDataProvider;// GeoDataDriver.Open(fname) as IRasterDataProvider;
            
            IRasterBand rb = prd.GetRasterBand(3);
            UInt16[] buffer = new UInt16[prd.Width * prd.Height];
            unsafe
            {
                fixed (UInt16* ptr = buffer)
                {
                    IntPtr bufferPtr = new IntPtr(ptr);
                    //prd.Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.UInt16, prd.Width, prd.Height, 1, new int[] { 1 }, enumInterleave.BSQ);
                    rb.Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.UInt16, prd.Width, prd.Height);
                }
            }
            string filename = "d:\\d.dat";
            FileStream fs = new FileStream(filename,FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            for (int i = 0; i < buffer.Length;i++ )
                bw.Write(buffer[i]);
            fs.Close();
        }
    }
}
