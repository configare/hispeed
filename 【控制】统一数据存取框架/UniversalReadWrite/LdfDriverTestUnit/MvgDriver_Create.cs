using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.MVG;
using System.IO;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class MvgDriver_Create
    {
        protected string _newFileName = null;
        protected string _existFileName = null;

        [SetUp]
        public void MvgFileName()
        {
            _newFileName = "e:\\MVG_2012_3_12.MVG";
            _fileName = "E:\\DST_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131352.MVG";
            //  _newFileName = @"E:\DST_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131352.MVG";
            PrjStdsMapTableParser.CnfgFile = @"E:\气象局项目\MAS二期\【配置库】统一数据存取框架\统一数据存取框架\UniversalReadWrite\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public void CreatEmptyMvgFile()
        {
            Assert.That(_newFileName != null);
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("MVG") as IRasterDataDriver;
            Assert.NotNull(driver);
            IMvgDriver drv = driver as IMvgDriver;
            Assert.NotNull(drv);
            int width = 2048;
            int height = 3390;
            string mapInfo = "{1,1}:{110,35}:{0.01,0.01}";
            Int16 valueCount = 2;
            bool withHdr = true;
            string values = "{ 0, 1 }";
            string valueNames = "{120,99}";

            IMvgDataProvider prd = drv.Create(_newFileName, width, height, 1, enumDataType.Int16, "VALUECOUNT=" + valueCount, "VALUES=" + values,
                                            "VALUENAMES=" + valueNames, "MAPINFO=" + mapInfo, "WITHHDR=" + withHdr.ToString()) as IMvgDataProvider;
            Assert.NotNull(prd);
            Assert.That(prd.BandCount == 1);
            Assert.That(prd.DataType == enumDataType.Int16);


            MvgHeader header = prd.Header;
            Assert.True(File.Exists(_newFileName));
            FileInfo fInfo = new FileInfo(_newFileName);
            HdrFile hdr = (header as MvgHeader).ToHdrFile();
            PrintHdrInfo(hdr);

            prd.Dispose();
            drv.Dispose();
            Assert.True(File.Exists(_newFileName));
        }

        [Test]
        public void OpenFile()
        {
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fileName, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.IsNotNull(prd);
            MvgHeader header = (prd as IMvgDataProvider).Header as MvgHeader;
            Assert.IsNotNull(header);
            Console.WriteLine("header.Width=" + header.Width);
            Console.WriteLine("header.Height=" + header.Height);
            HdrFile hdr = header.ToHdrFile();
            PrintHdrInfo(hdr);
        }

        [Test]
        public void CreateCopy()
        {
            string fname = "E:\\DST_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131352.MVG";
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("MVG") as IRasterDataDriver;
            string dstName = "E:\\Copy.MVG";
            IRasterDataProvider srcprd = drv.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            IRasterDataProvider dstprd = drv.CreateCopy(dstName, srcprd);
            Assert.True(File.Exists(dstName));
            dstprd.Dispose();
            srcprd.Dispose();
        }

        [Test]
        public void CreateCopyWithFillZero()
        {
            string fname = @"E:\\DST_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131352.MVG";
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("MVG") as IRasterDataDriver;
            string dstfname = "e:\\CreateCopy_Zero.mvg";
            IRasterDataProvider srcprd = drv.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            IRasterDataProvider dstprd = drv.CreateCopy(dstfname, srcprd);
            for (int b = 1; b <= dstprd.BandCount; b++)
                dstprd.GetRasterBand(b).Fill(0);
            Assert.True(File.Exists(dstfname));
            dstprd.Dispose();
            srcprd.Dispose();
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


        public string _fileName { get; set; }
    }
}
