using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.BlockOper;

namespace UnitTest
{
    [TestFixture]
    public class ClipTest
    {
        [Test]
        public void ClipDataProvider()
        {
            string fname = "d:\\ce110429n18.ldf";
            //IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            Assert.NotNull(driver);
            IRasterDataProvider prd = driver.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Console.WriteLine(prd.CoordEnvelope.Height);
            Console.WriteLine(prd.CoordEnvelope.Width);
            Console.WriteLine(prd.Height);
            Console.WriteLine(prd.Width);

            RasterClipProcesser cp = new RasterClipProcesser();
            //BlockDef block=new BlockDef("a",

        }
    }
}
