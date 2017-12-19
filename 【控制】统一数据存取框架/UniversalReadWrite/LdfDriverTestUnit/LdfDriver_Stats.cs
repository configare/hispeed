using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class LdfDriver_Stats
    {
        protected string _originalFname = @"D:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";

        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"F:\技术研究\MAS_II\home\源代码\【控制】统一数据存取框架\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public void TestHistogram()
        {
            IRasterDataProvider prd = GeoDataDriver.Open(_originalFname) as IRasterDataProvider;
            Assert.NotNull(prd);
            int max = 1000;
            int[] histogram = new int[max];
            IRasterBand band = prd.GetRasterBand(20);
            band.ComputeHistogram(0, max, 0, histogram, true, false, null);
            for (int i = 0; i < histogram.Length; i++)
            {
                Console.WriteLine(i.ToString().PadLeft(5, ' ') + '=' + histogram[i].ToString());
            }
            prd.Dispose();
        }

        [Test]
        public void TestHistogram_From_Negative()
        {
            IRasterDataProvider prd = GeoDataDriver.Open(_originalFname) as IRasterDataProvider;
            Assert.NotNull(prd);
            int begin = -100;
            int[] histogram = new int[UInt16.MaxValue + Math.Abs(begin)];
            IRasterBand band = prd.GetRasterBand(1);
            band.ComputeHistogram(begin, UInt16.MaxValue, 0, histogram, true, false, null);
            for (int i = begin; i < histogram.Length; i++)
            {
                Console.WriteLine(i.ToString().PadLeft(5, ' ') + '=' + histogram[i- begin].ToString());
            }
            prd.Dispose();
        }
    }
}
