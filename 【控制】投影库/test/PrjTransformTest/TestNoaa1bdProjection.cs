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
    public class TestNoaa1bdProjection
    {
        protected Action<int, string> _progressCallback = null;

        [SetUp]
        public void DataReady()
        {
        }

        [Test]
        public void TestProj()
        {
            IRasterDataProvider srcRaster = null;
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("NOAA_1BD");
                srcRaster = GetSrcRaster();
                FilePrjSettings prjSetting = GetPrjSetting(srcRaster);
                ISpatialReference dstSpatialRef = SpatialReference.GetDefault();        //默认的WGS84等经纬度投影。
                proj.Project(srcRaster, prjSetting, dstSpatialRef, _progressCallback);
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
            string srcFilename = @"D:\masData\noaa_1bd\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd";
            //srcFilename = @"D:\mas数据\NOAA18_1BD\NA18_AVHRR_HRPT_L1_ORB_MLT_NUL_20120319_0618_1100M_PJ.L1B";
            return GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
        }

        private FilePrjSettings GetPrjSetting(IRasterDataProvider srcRaster)
        {
            string dstFilename = @"D:\masData\noaa_1bd\testtt.ldf";
            PrjEnvelope dstEnvelope = PrjEnvelope.CreateByCenter(78.621416, 32.189639, 36.00, 26.00);
            NOAA_PrjSettings prjSetting = new NOAA_PrjSettings();
            prjSetting.OutPathAndFileName = dstFilename;    //必填
            //prjSetting.OutResolutionX = 1000F;     //default
            //prjSetting.OutResolutionY = 1000F;     //default
            //prjSetting.OutFormat = "LDF";          //default
            //prjSetting.OutEnvelope = dstEnvelope;  //default
            prjSetting.IsRadiation = true;          //default true
            prjSetting.IsSolarZenith = true;
            return prjSetting;
        }
    }
}
