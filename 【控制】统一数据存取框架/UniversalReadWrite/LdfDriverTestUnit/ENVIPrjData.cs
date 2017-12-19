using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class ENVIPrjData
    {
        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"F:\技术研究\MAS_II\源代码\【控制】绘图引擎\DefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public void HammerPrjFile()
        {
            string fname = "f:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
        }
    }
}
