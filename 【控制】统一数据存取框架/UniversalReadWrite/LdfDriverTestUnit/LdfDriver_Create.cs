using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using NUnit.Framework;
using GeoDo.RSS.DF.LDF;
using System.IO;
using GeoDo.Project;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class LdfDriver_Create
    {
        protected string _newFileName = null;

        [SetUp]
        public void LdfFileName()
        {
            _newFileName = "d:\\LDF_2012_2_27.ldf";
            PrjStdsMapTableParser.CnfgFile = @"F:\技术研究\MAS_II\home\源代码\【控制】统一数据存取框架\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }
        [Test]
        public void DeleteFile()
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            string dstfname = "d:\\CreateCopy.ldf";
            drv.Delete(dstfname);
            Assert.True(!File.Exists(dstfname));
        }

        [Test]
        public void CreateCopy()
        { 
            string fname = @"D:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            string dstfname = "d:\\CreateCopy.ldf";
            IRasterDataProvider srcprd = drv.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            IRasterDataProvider dstprd = drv.CreateCopy(dstfname, srcprd);
            Assert.True(File.Exists(dstfname));
            dstprd.Dispose();
            srcprd.Dispose();
        }


        [Test]
        public void CreateCopy_AddBand()
        {
            string fname = @"D:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            string dstfname = "d:\\CreateCopy.ldf";
            IRasterDataProvider srcprd = drv.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            IRasterDataProvider dstprd = drv.CreateCopy(dstfname, srcprd);
            Assert.True(File.Exists(dstfname));
            dstprd.AddBand(enumDataType.UInt16);
            Assert.AreEqual(srcprd.BandCount + 1, dstprd.BandCount);
            dstprd.Dispose();
            srcprd.Dispose();
        }

        [Test]
        public void CreateCopyWithFillZero()
        {
            string fname = @"D:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            string dstfname = "d:\\CreateCopy_Zero.ldf";
            IRasterDataProvider srcprd = drv.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            IRasterDataProvider dstprd = drv.CreateCopy(dstfname, srcprd);
            for (int b = 1; b <= dstprd.BandCount; b++)
                dstprd.GetRasterBand(b).Fill(0);
            Assert.True(File.Exists(dstfname));
            dstprd.Dispose();
            srcprd.Dispose();
        }

        [Test]
        public void CreateEmptyLdfFile()
        {
            Assert.That(_newFileName != null);
            IGeoDataDriver driver = GeoDataDriver.GetDriverByName("LDF");
            Assert.NotNull(driver);
            ILdfDriver drv = driver as ILdfDriver;
            Assert.NotNull(drv);
            int width = 2048;
            int height = 3390;
            int bandCount = 10;
            enumDataType dataType = enumDataType.UInt16;
            enumInterleave interleave = enumInterleave.BSQ;
            string version = "LDF";
            //string proj4 = "+proj=merc +lon_0=0 +k=1 +x_0=0 +y_0=0 +a=6378137 +b=6378137";
            string mapInfo = "{1,1}:{110,35}:{0.01,0.01}";
            bool withHdr = true;
            ILdfDataProvider prd = drv.Create(_newFileName, width, height, bandCount, dataType,
                "INTERLEAVE=" + interleave.ToString(),
                "VERSION=" + version,
                //"SPATIALREF=" + proj4,
                "MAPINFO=" + mapInfo,"WITHHDR=" + withHdr.ToString()) as ILdfDataProvider;
            Assert.NotNull(prd);
            Assert.That(prd.Width == width && prd.Height == height);
            Assert.That(prd.BandCount == bandCount);
            Assert.That(prd.DataType == dataType);
            ILdfHeader header = prd.Header;
            Assert.NotNull(header);
            HdrFile hdr = header.ToHdrFile();
            PrintHdrInfo(hdr);
            Ldf1Header ldfheader = header as Ldf1Header;
            Assert.NotNull(ldfheader);
            Assert.That(ldfheader._width == width && ldfheader._height == height);
            Assert.That(ldfheader.DataType == dataType);
            Assert.NotNull(prd.SpatialRef);
            for (int i = 0; i < bandCount; i++)
            {
                IRasterBand band = prd.GetRasterBand(i + 1);
                Assert.NotNull(band);
                Assert.That(band.Width == width && band.Height == height);
                Assert.That(band.DataType == dataType);
            }
            prd.Dispose();
            drv.Dispose();
            Assert.True(File.Exists(_newFileName));
            FileInfo fInfo = new FileInfo(_newFileName);
            Assert.True(width * height * bandCount * 2 + header.HeaderSize == fInfo.Length);
            ISpatialReference spatialRef = prd.SpatialRef;
            Assert.NotNull(spatialRef);
            Console.Out.WriteLine(spatialRef.ToString());
        }

        private void PrintHdrInfo(HdrFile hdr)
        {
            Console.WriteLine("Hdr File:");
            Console.WriteLine(hdr.Samples.ToString());
            Console.WriteLine(hdr.Lines.ToString());
            Console.WriteLine(hdr.Bands.ToString());
            Console.WriteLine(hdr.Intertleave.ToString());
            Console.WriteLine(hdr.DataType.ToString());
            Console.WriteLine(hdr.ByteOrder.ToString());
            Console.WriteLine(hdr.HeaderOffset.ToString());
            Console.WriteLine("////");
            hdr.SaveTo(HdrFile.GetHdrFileName(_newFileName));
            Console.WriteLine("LOAD HDR FILE:");
            Console.Write(File.ReadAllText(HdrFile.GetHdrFileName(_newFileName)));
            Console.WriteLine("////END LOAD HDR FILE");
        }
    }
}
