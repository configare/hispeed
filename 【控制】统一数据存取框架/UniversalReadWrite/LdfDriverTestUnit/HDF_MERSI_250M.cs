using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class HDF_MERSI_250M
    {
        [Test]
        public void Read()
        {
            string fnam = "f:\\FY3A_MERSI_GBAL_L1_20100429_0300_0250M_MS.HDF";
            fnam = "f:\\FY3A_VIRR_2010_06_24_11_39_1000M_L1B.HDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fnam) as IRasterDataProvider;
            Assert.NotNull(prd);
        }
    }
}
