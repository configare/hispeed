using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using System.Diagnostics;

namespace LdfDriverTestUnit
{
    [TestFixture]
    class LdfDriver_WriteLdf
    {
        protected string _originalFname = @"D:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";

        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"F:\技术研究\MAS_II\home\源代码\【控制】统一数据存取框架\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public unsafe void CopyBlockToEmptyFile()
        {
            int xSize = 512;
            int ySize = 512;
            int bandCount = 20;
            string fname = "d:\\Ldf_Write_GeoDo.ldf";
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            IRasterDataProvider dstPrd = CreateEmptyFile(fname, drv, xSize, ySize, bandCount);
            IRasterDataProvider srcPrd = OpenLdfFile(_originalFname);
            UInt16[] buffer = new UInt16[xSize * ySize];
            long lostTime = 0;
            fixed (UInt16* ptr = buffer)
            {
                IntPtr butterPtr = new IntPtr(ptr);
                for (int b = 1; b <= bandCount; b++)
                {
                    srcPrd.GetRasterBand(b).Read(200, 200, xSize, ySize, butterPtr, enumDataType.UInt16, xSize, ySize);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    dstPrd.GetRasterBand(b).Write(0, 0, xSize, ySize, butterPtr, enumDataType.UInt16, xSize, ySize);
                    sw.Stop();
                    lostTime += sw.ElapsedMilliseconds;
                }
            }
            srcPrd.Dispose();
            dstPrd.Dispose();
            Console.WriteLine("Time Only Write:" + lostTime.ToString());
        }

        [Test]
        public unsafe void Moasic()
        {
            int xSize = 500;
            int ySize = 500;
            int xOffset = 500;
            int yOffset = 500;
            string fname = "d:\\Moasic.LDF";
            IRasterDataProvider dstPrd = OpenLdfFile(fname);
            UInt16[] buffer = new UInt16[xSize * ySize];
            fixed (UInt16* ptr = buffer)
            {
                IntPtr butterPtr = new IntPtr(ptr);
                dstPrd.GetRasterBand(1).Write(xOffset, yOffset, xSize, ySize, butterPtr, enumDataType.UInt16, xSize, ySize);
            }
            dstPrd.Dispose();
        }

        [Test]
        public unsafe void WriteWithSample()
        {
            int xSize = 512;
            int ySize = 512;
            int bandCount = 20;
            int dstWith = 2000;
            int dstHeight = 2000;
            string fname = "d:\\Ldf_Write_GeoDo_Sample.ldf";
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            IRasterDataProvider dstPrd = CreateEmptyFile(fname, drv, dstWith, dstHeight, bandCount);
            IRasterDataProvider srcPrd = OpenLdfFile(_originalFname);
            UInt16[] buffer = new UInt16[xSize * ySize];
            fixed (UInt16* ptr = buffer)
            {
                IntPtr butterPtr = new IntPtr(ptr);
                for (int b = 1; b <= bandCount; b++)
                {
                    srcPrd.GetRasterBand(b).Read(200, 200, xSize, ySize, butterPtr, enumDataType.UInt16, xSize, ySize);
                    dstPrd.GetRasterBand(b).Write(0, 0, dstWith, dstHeight, butterPtr, enumDataType.UInt16, xSize, ySize);
                }
            }
            srcPrd.Dispose();
            dstPrd.Dispose();
        }


        private IRasterDataProvider CreateEmptyFile(string fname, IRasterDataDriver driver, int xSize, int ySize, int bandCount)
        {
            return driver.Create(fname, xSize, ySize, bandCount, enumDataType.UInt16);
        }

        private IRasterDataProvider OpenLdfFile(string fname)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
            return prd;
        }
    }
}
