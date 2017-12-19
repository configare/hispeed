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
    public class EosModisIO
    {
        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"D:\工作空间\MAS二期\06.开发代码2\【控制】统一数据存取框架\UniversalReadWrite\Output\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public void GetDriver()
        {
            string fnam = @"D:\masData\MODIS\TERRA_2010_03_25_03_09_GZ.MOD021KM.hdf";
            IRasterDataProvider prd = GeoDataDriver.Open(fnam) as IRasterDataProvider;
            Assert.NotNull(prd, "建立EOS——MODIS Driver失败");
        }

        [Test]
        public void ReadBand()
        {
            string fnam = @"D:\masData\MODIS\TERRA_2010_03_25_03_09_GZ.MOD021KM.hdf";
            using (IRasterDataProvider prd = GeoDataDriver.Open(fnam) as IRasterDataProvider)
            {
                Assert.NotNull(prd, "建立EOS——MODIS Driver失败");
                UInt16[] buffer = new UInt16[prd.Width * prd.Height];
                unsafe
                {
                    fixed (UInt16* pointer = buffer)
                    {
                        IntPtr ptr = new IntPtr(pointer);
                        for (int i = 0; i < 10; i++)
                        {
                            prd.GetRasterBand(1).Read(0, 0, prd.Width, prd.Height, ptr, prd.DataType, prd.Width, prd.Height);
                        }
                    }
                }
            }
        }
    }
}
