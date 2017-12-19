using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.DF.LDF;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.IO;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class LdfDriver_ReadLdf
    {
        protected string _fnameBSQ = @"D:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
        protected string _fnameBIP = null;
        protected string _fnameBIL = null;

        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"F:\技术研究\MAS_II\home\源代码\【控制】统一数据存取框架\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        private IRasterDataProvider OpenLdfFile(string fname)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
            return prd;
        }

        [Test]
        [Category("Block")]
        public void OpenBSQFile_GeoDo_Block()
        {
            IRasterDataProvider prd = OpenLdfFile(_fnameBSQ);
            Assert.Greater(prd.BandCount, 0);
            ReadOneBandByBlock(prd,1,"D:\\GeoDo_LDF");
            prd.Dispose();
        }

        [Test]
        [Category("Block")]
        public void OpenBSQFile_GDAL_Block()
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            IRasterDataProvider prd = drv.Open(_fnameBSQ,enumDataProviderAccess.ReadOnly) as IRasterDataProvider;// OpenLdfFile(_fnameBSQ);
            Assert.Greater(prd.BandCount, 0);
            ReadOneBandByBlock(prd, 1, "D:\\GDAL_LDF");
            prd.Dispose();
        }

        [Test]
        [Category("BlockWithSample")]
        public void OpenBSQFile_GeoDo_Block_Sample()
        {
            IRasterDataProvider prd = OpenLdfFile(_fnameBSQ);
            Assert.Greater(prd.BandCount, 0);
            ReadOneBandByBlockBySample(prd, 1, "D:\\GeoDo_LDF");
            prd.Dispose();
        }

        [Test]
        [Category("BlockWithSample")]
        public void OpenBSQFile_GDAL_Block_Sample()
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            IRasterDataProvider prd = drv.Open(_fnameBSQ, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;// OpenLdfFile(_fnameBSQ);
            Assert.Greater(prd.BandCount, 0);
            ReadOneBandByBlockBySample(prd, 1, "D:\\GDAL_LDF");
            prd.Dispose();
        }

        [Test]
        [Category("FullSize")]
        public void OpenBSQFile_GeoDo_FullSize()
        {
            IRasterDataProvider prd = OpenLdfFile(_fnameBSQ);
            Assert.Greater(prd.BandCount, 0);
            ReadOneBandByFullSize(prd, 1, "D:\\GeoDo_LDF");
            prd.Dispose();
        }

        [Test]
        [Category("FullSize")]
        public void OpenBSQFile_GDAL_FullSize()
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            IRasterDataProvider prd = drv.Open(_fnameBSQ, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;// OpenLdfFile(_fnameBSQ);
            Assert.Greater(prd.BandCount, 0);
            ReadOneBandByFullSize(prd, 1, "D:\\GDAL_LDF");
            prd.Dispose();
        }

        private unsafe void ReadOneBand(IRasterDataProvider prd, int bandNo, string fname, int xSize, int ySize, int xOffset, int yOffset, int xBufferSize, int yBufferSize)
        {
            IRasterBand band = prd.GetRasterBand(bandNo);
            UInt16[] bandValues = new UInt16[xBufferSize * yBufferSize];
            fixed (UInt16* ptr = bandValues)
            {
                IntPtr butrer = new IntPtr(ptr);
                band.Read(xOffset, yOffset, xSize, ySize, butrer, enumDataType.UInt16, xBufferSize, yBufferSize);
            }
            WriteBandToFile(bandValues, fname + "_" + bandNo + ".dat");
        }

        private unsafe void ReadOneBandByFullSize(IRasterDataProvider prd, int bandNo, string fname)
        {
            ReadOneBand(prd, bandNo, fname, prd.Width, prd.Height, 0, 0, prd.Width, prd.Height);
        }

        private unsafe void ReadOneBandByBlock(IRasterDataProvider prd, int bandNo,string fname)
        {
            ReadOneBand(prd, bandNo, fname, 500, 500, 0, 0, 500, 500);
        }

        private unsafe void ReadOneBandByBlockBySample(IRasterDataProvider prd, int bandNo, string fname)
        {
            //int times = 100;
            //for(int i=0;i<times;i++)
                ReadOneBand(prd, bandNo, fname, 500, 500, 0, 0, 499, 499);
        }

        private void WriteBandToFile(UInt16[] bandValues, string fname)
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

        [Test]
        public void OpenBIPFile()
        {
            IRasterDataProvider prd = OpenLdfFile(_fnameBIP);
            Assert.NotNull(prd);
            prd.Dispose();

        }

        [Test]
        public void OpenBILFile()
        {
            IRasterDataProvider prd = OpenLdfFile(_fnameBIL);
            Assert.NotNull(prd);
            prd.Dispose();
        }
    }
}
