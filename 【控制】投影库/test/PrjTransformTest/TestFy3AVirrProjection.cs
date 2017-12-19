using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;
using NUnit.Framework;
using System.Diagnostics;
using System.Windows.Forms;

namespace test
{
    [TestFixture]
    public class TestFy3AVirrProjection
    {
        public Action<int, string> ProgressCallback = null;

        [SetUp]
        public void DataReady()
        {
        }

        [Test(Description="测试创建投影提供者")]
        public void TestCreate()
        {
            IFileProjector proj = FileProjector.GetFileProjectByName("FY3_VIRR");
            Assert.IsNotNull(proj, "创建投影提供者失败。");
        }

        [Test]
        public void TestFy3VIRR()
        {
            IRasterDataProvider srcRaster = null;
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_VIRR");
                srcRaster = GetSrcRaster();
                FY3_VIRR_PrjSettings prjSetting = GetPrjSetting(srcRaster);
                ISpatialReference dstSpatialRef = SpatialReference.GetDefault();//默认的WGS84等经纬度投影。
                //dstSpatialRef = SpatialReference.FromPrjFile(@"North Pole Stereographic.prj");
                proj.Project(srcRaster, prjSetting, dstSpatialRef, ProgressCallback);
                Console.WriteLine();
            }
            finally
            {
                if (srcRaster != null)
                {
                    if (srcRaster.BandProvider != null)
                        srcRaster.BandProvider.Dispose();
                    srcRaster.Dispose();
                }
            }
        }

        private IRasterDataProvider GetSrcRaster()
        {
            string srcFilename = @"D:\mas数据\virr\FY3A_VIRRX_GBAL_L1_20090107_0220_1000M_MS.HDF";
            //srcFilename = @"E:\gov\FY3B_VIRRX_GBAL_L1_20120308_0830_1000M_MS.HDF";
            return GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
        }

        private FY3_VIRR_PrjSettings GetPrjSetting(IRasterDataProvider srcRaster)
        {
            string dstFilename = @"D:\fy3VIRR_Block.ldf";
            PrjEnvelope dstEnvelope = PrjEnvelope.CreateByCenter(78.621416, 32.189639, 36.00, 26.00);
            FY3_VIRR_PrjSettings prjSetting = new FY3_VIRR_PrjSettings();
            prjSetting.OutPathAndFileName = dstFilename;    //必填
            //prjSetting.OutResolutionX = 1000F;    //default
            //prjSetting.OutResolutionY = 1000F;    //default
            //prjSetting.OutFormat = "LDF";         //default
            //prjSetting.OutEnvelope = dstEnvelope; //default
            //prjSetting.BandMapTable = bandmapList;//default
            prjSetting.IsRadiation = true;          //default true
            prjSetting.IsSolarZenith = true;       //default true
            return prjSetting;
        }
    }
}
