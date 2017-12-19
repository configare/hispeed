using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.MVG;
using System.IO;
using GeoDo.Project;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class MvgDriver_MvgToLdfFile
    {
        protected string _fname = @"E:\DST_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131352.MVG";

        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"E:\气象局项目\MAS二期\【配置库】统一数据存取框架\UniversalReadWrite\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public void MvgToLdfFileWithName()
        {
            _fname = @"E:\气象局项目\MAS二期\mvg文件\FOG_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131354.mvg";
            string ldfName = @"e:\mvgToLdf.ldf";
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);

            IMvgDataProvider mvgPrd = prd as IMvgDataProvider;
            MvgHeader mvgHeader = mvgPrd.Header;
            Assert.NotNull(mvgHeader);
            Console.WriteLine("mvgHeader.HeaderSize = " + mvgHeader.HeaderSize);
            Console.WriteLine("mvgHeader.Width = " + mvgHeader.Width);
            Console.WriteLine("mvgHeader.Height = " + mvgHeader.Height);

            mvgPrd.ToLdfFile(ldfName);
            Assert.True(File.Exists(ldfName));
            mvgPrd.Dispose();
            prd.Dispose();
            drv.Dispose();
        }

        [Test]
        public void MvgToLdfFile()
        {
            _fname = @"E:\气象局项目\MAS二期\mvg文件\FOG_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131354.mvg";
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);

            IMvgDataProvider mvgPrd = prd as IMvgDataProvider;
            MvgHeader mvgHeader = mvgPrd.Header;
            Assert.NotNull(mvgHeader);
            Console.WriteLine("mvgHeader.HeaderSize = " + mvgHeader.HeaderSize);
            Console.WriteLine("mvgHeader.Width = " + mvgHeader.Width);
            Console.WriteLine("mvgHeader.Height = " + mvgHeader.Height);

            mvgPrd.ToLdfFile();
            mvgPrd.Dispose();
            prd.Dispose();
            drv.Dispose();
        }

        [Test]
        public void LdfToMvgFileWithName()
        {
            string ldfName = @"E:\气象局项目\MAS二期\mvg文件\FY3B_VIRRX_GBAL_L1_20101128_0500_1000M_MS_PRJ_Whole.LDF";
            string mvgName = @"e:\ldfToMvg.mvg";
            Int16 valueCount = 2;
            string values = "{ 0, 1 }";
            string valueNames = "{120,99}";
            MvgDataProvider.FromLDF(ldfName, mvgName, 1, "VALUECOUNT=" + valueCount, "VALUES=" + values, "VALUENAMES=" + valueNames);

            Assert.True(File.Exists(@"e:\ldfToMvg.hdr"));
            Console.WriteLine("Ldf header informations:");
            using (StreamReader sr = new StreamReader(@"E:\气象局项目\MAS二期\mvg文件\FY3B_VIRRX_GBAL_L1_20101128_0500_1000M_MS_PRJ_Whole.hdr"))
            {
                string ldfHeaderInfo = sr.ReadToEnd();
                Console.Write(ldfHeaderInfo);
            }

            Console.WriteLine("********************************************************");
            Console.WriteLine("Mvg header informations:");
            using (StreamReader sr = new StreamReader(@"e:\ldfToMvg.hdr"))
            {
                string mvgHeaderInfo = sr.ReadToEnd();
                Console.Write(mvgHeaderInfo);
            }
        }

        [Test]
        public void LdfToMvgFile()
        {
            string ldfName = @"E:\气象局项目\MAS二期\mvg文件\FY3A_MERSI_GBAL_L1_20090505_0200_1000M_MS_PRJ_DXX.LDF";
            Int16 valueCount = 2;
            string values = "{ 0, 1 }";
            string valueNames = "{120,99}";
            MvgDataProvider.FromLDF(ldfName, 1, "VALUECOUNT=" + valueCount, "VALUES=" + values, "VALUENAMES=" + valueNames);
        }
    }
}
