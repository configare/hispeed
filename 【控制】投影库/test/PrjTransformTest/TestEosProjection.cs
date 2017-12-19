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
    public class TestEosProjection
    {
        public Action<int, string> _progressCallback = null;

        [Test]
        public void TestProj()
        {
            IRasterDataProvider srcRaster = null;
            IRasterDataProvider location = null;
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("EOS");
                srcRaster = GetSrcRaster();
                location = GetLocationRaster();
                EOS_MODIS_PrjSettings prjSetting = GetPrjSetting(srcRaster, location);
                ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
                //dstSpatialRef = SpatialReference.FromPrjFile(@"North Pole Lambert Azimuthal Equal Area.prj");
                proj.Project(srcRaster, prjSetting, dstSpatialRef, _progressCallback);

                Console.WriteLine();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            string srcFilename = @"D:\mas数据\EOS\TERRA_2012_03_16_02_56_GZ.MOD021KM.hdf";
            return GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
        }

        private IRasterDataProvider GetLocationRaster()
        {
            string srcFilename = @"D:\mas数据\EOS\TERRA_2012_03_16_02_56_GZ.MOD03.hdf";
            return GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
        }

        private EOS_MODIS_PrjSettings GetPrjSetting(IRasterDataProvider srcRaster,IRasterDataProvider location)
        {
            string dstFilename = @"D:\TERRA_2012_03_16_02_56_GZ.MOD021KM.ldf";
            PrjEnvelope dstEnvelope = PrjEnvelope.CreateByCenter(78.621416, 32.189639, 36.00, 26.00);
            //List<BandMap> bandmapList = new List<BandMap>();
            EOS_MODIS_PrjSettings prjSetting = new EOS_MODIS_PrjSettings();
            prjSetting.OutPathAndFileName = dstFilename;    //必填
            prjSetting.LocationFile = location;             //经纬度坐标文件，必须指定
            //prjSetting.OutResolutionX = 1000F;    //default
            //prjSetting.OutResolutionY = 1000F;    //default
            //prjSetting.OutFormat = "LDF";         //default
            //prjSetting.OutEnvelope = dstEnvelope; //default
            //prjSetting.BandMapTable = bandmapList;//default
            prjSetting.IsRadiation = true;          //default true
            prjSetting.IsSolarZenith = true;
            return prjSetting;
        }
    }
}
