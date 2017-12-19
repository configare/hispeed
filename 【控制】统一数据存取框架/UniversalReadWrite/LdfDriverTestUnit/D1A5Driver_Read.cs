using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.NOAA;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class D1A5Driver_Read
    {
        [SetUp]
        public void Init()
        {
            byte[] bs = new byte[] { 43,118,133,66};
            float v = BitConverter.ToSingle(bs, 0);
            Console.WriteLine(v.ToString());
        }

        [Test]
        public void OpenFile()
        {

            string fname = "d:\\1110910.1a5";
            fname = @"H:\测试\测试数据\1a5\1110908.1a5";
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("NOAA_1A5") as IRasterDataDriver;
            Assert.NotNull(driver);
            //IRasterDataProvider prd = driver.Open(fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
            Console.WriteLine(prd.BandCount.ToString());
            Console.WriteLine(prd);
            //PrintHeaderInfo(prd);

            ID1A5DataProvider noaaPrd = prd as ID1A5DataProvider;
            Assert.NotNull(noaaPrd);
            D1A5Header header = noaaPrd.Header as D1A5Header;
            Assert.NotNull(header);
            Console.WriteLine(header.SatelliteIdentify);
            Console.WriteLine(header.BitErrorRatio);
            Console.WriteLine(header.DataBeginDayNums);
            Console.WriteLine(header.DataBeginMilliSecond);

        }

        private void PrintHeaderInfo(IRasterDataProvider prd)
        {
            ID1A5DataProvider noaaPrd = prd as ID1A5DataProvider;
            Assert.NotNull(noaaPrd);
            D1A5Header header = noaaPrd.Header as D1A5Header;
            Assert.NotNull(header);

            Console.WriteLine(header.BitErrorRatio);
            Console.WriteLine(header.DataBeginDayNums);
            Console.WriteLine(header.DataBeginMilliSecond);
        }

        [Test]
        public void ReadGeoInfo()
        {
            string fname = "d:\\1110910.1a5";
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("NOAA_1A5") as IRasterDataDriver;
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            D1A5DataProvider dp = prd as D1A5DataProvider;


            //Assert.NotNull(dp.Latitudes);
            //Assert.NotNull(dp.Longitudes);
            //for (int i = 0; i < 3 * 2048; i++)
            //{
            //    Console.WriteLine(dp.Longitudes[i]);
            //    Console.WriteLine(i);
            //}

        }
    }
}
