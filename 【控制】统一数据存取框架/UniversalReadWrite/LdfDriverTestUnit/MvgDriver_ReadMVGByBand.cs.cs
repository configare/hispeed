using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class MvgDriver_ReadMVGByBand
    {
        protected string _fname = @"E:\气象局项目\MAS二期\mvg文件\FOG_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131354.mvg";

        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"E:\气象局项目\MAS二期\【配置库】统一数据存取框架\UniversalReadWrite\RefDLL\Project\GeoDo.Project.Cnfg.xml";
            // PrjStdsMapTableParser.CnfgFile = @"C:\工作\3.13\【配置库】统一数据存取框架\统一数据存取框架\UniversalReadWrite\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public unsafe void ReadMvgFileNormal()
        {
            // _fname = @"C:\工作\3.13\SNW_DBLV_FY3A_VIRR_1000M_NULL_P001_200911131356.mvg";
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            IRasterBand band = prd.GetRasterBand(1);
            Assert.NotNull(band);
            Int16[] data = new Int16[prd.Width * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            Console.WriteLine("band.Width = " + band.Width);
            Console.WriteLine("band.Height= " + band.Height);
            fixed (Int16* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                band.Read(0, 0, prd.Width, prd.Height, buffer, enumDataType.Int16, prd.Width, prd.Height);
            }
            string dstFname = @"e:\mvg";//@"C:\工作\3.13\read";
            WriteBandToFile(data, dstFname + "_normal" + "_read" + ".dat");
            prd.Dispose();
            drv.Dispose();
        }

        [Test]
        public unsafe void ReadMvgFileWithOffset()
        {
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            IRasterBand band = prd.GetRasterBand(1);
            Assert.NotNull(band);
            Int16[] data = new Int16[prd.Width * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            Console.WriteLine("band.Width = " + band.Width);
            Console.WriteLine("band.Height= " + band.Height);
            fixed (Int16* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                band.Read(1, 1, prd.Width, prd.Height, buffer, enumDataType.Int16, prd.Width, prd.Height);
            }
            string dstFname = @"e:\mvg";//@"C:\工作\3.13\read";
            WriteBandToFile(data, dstFname + "_offset" + "_read" + ".dat");
            prd.Dispose();
            drv.Dispose();
        }

        [Test]
        public unsafe void ReadMvgFile_noOffset_Bigger()
        {
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            IRasterBand band = prd.GetRasterBand(1);
            Assert.NotNull(band);
            Int16[] data = new Int16[prd.Width * 2 * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            Console.WriteLine("band.Width = " + band.Width);
            Console.WriteLine("band.Height= " + band.Height);
            fixed (Int16* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                band.Read(0, 0, prd.Width, prd.Height, buffer, enumDataType.Int16, prd.Width * 2, prd.Height);
            }
            string dstFname = @"e:\mvg";//@"C:\工作\3.13\read";
            WriteBandToFile(data, dstFname + "_bigger" + "_read" + ".dat");
            prd.Dispose();
            drv.Dispose();
        }

        [Test]
        public unsafe void ReadMvgFile_noOffset_smaller()
        {
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            IRasterBand band = prd.GetRasterBand(1);
            Assert.NotNull(band);
            Int16[] data = new Int16[prd.Width / 2 * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            Console.WriteLine("band.Width = " + band.Width);
            Console.WriteLine("band.Height= " + band.Height);
            fixed (Int16* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                band.Read(0, 0, prd.Width, prd.Height, buffer, enumDataType.Int16, prd.Width / 2, prd.Height);
            }
            string dstFname = @"e:\mvg";//@"C:\工作\3.13\read";
            WriteBandToFile(data, dstFname + "_smaller" + "_read" + ".dat");
            prd.Dispose();
            drv.Dispose();
        }

        [Test]
        public unsafe void ReadMvgFile_Offset_smaller()
        {
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            IRasterBand band = prd.GetRasterBand(1);
            Assert.NotNull(band);
            Int16[] data = new Int16[prd.Width / 2 * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            Console.WriteLine("band.Width = " + band.Width);
            Console.WriteLine("band.Height= " + band.Height);
            fixed (Int16* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                band.Read(1, 1, prd.Width, prd.Height, buffer, enumDataType.Int16, prd.Width / 2, prd.Height);
            }
            string dstFname = @"e:\mvg";//@"C:\工作\3.13\read";
            WriteBandToFile(data, dstFname + "_offset" + "_smaller" + "_read" + ".dat");
            prd.Dispose();
            drv.Dispose();
        }

        private void WriteBandToFile(Int16[] bandValues, string fname)
        {
            using (FileStream fs = new FileStream(fname, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    int len = bandValues.Length;
                    for (int i = 0; i < len; i++)
                    {
                        bw.Write(bandValues[i]);
                    }
                }
            }
        }
    }
}
