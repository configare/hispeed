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
    public class TestEosProjection0500
    {
        protected EOS_MODIS_PrjSettings prjSetting = null;
        protected IRasterDataProvider srcRaster = null;
        protected ISpatialReference dstSpatialRef = null;
        protected string srcFilename = null;
        protected string dstFilename = null;
        protected PrjEnvelope dstEnvelope = null;
        protected Action<int, string> _progressCallback = null;

        [SetUp]
        public void SetUp()
        {
            PrjStdsMapTableParser.CnfgFile = @"D:\工作空间\MAS二期\06.开发代码\【控制】投影库\Output\GeoDo.Project.Cnfg.xml";
        }

        //[SetUp]
        public void DataReady(Action<int, string> progressCallback)
        {
            ///等经纬度投影
            dstFilename = @"D:\mas数据\MODIS\TERRA_2010_03_25_03_09_GZ.MOD02HKM.ldf";
            dstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");//
            srcFilename = @"D:\mas数据\MODIS\TERRA_2010_03_25_03_09_GZ.MOD02HKM.HDF";
            dstEnvelope = PrjEnvelope.CreateByCenter(110, 32, 5, 5);//
            string secondFile = @"D:\mas数据\MODIS\TERRA_2010_03_25_03_09_GZ.MOD03.HDF";
            IRasterDataProvider secondFileRaster = GeoDataDriver.Open(secondFile) as IRasterDataProvider;

            srcRaster = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
            //List<BandMap> bandmapList = new List<BandMap>();
            prjSetting = new EOS_MODIS_PrjSettings();

            prjSetting.OutResolutionX = 0.0025F;
            prjSetting.OutResolutionY = 0.0025F;
            prjSetting.OutPathAndFileName = dstFilename;
            prjSetting.OutFormat = "LDF";
            //prjSetting.OutEnvelope = dstEnvelope;
            //prjSetting.BandMapTable = bandmapList;
            //prjSetting.IsRadiation = false;
            prjSetting.SecondaryOrbitRaster = null;
            prjSetting.LocationFile = secondFileRaster;
            _progressCallback = progressCallback;
        }

        public void TestProjection()
        {
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("EOS_MODIS");
                proj.Project(srcRaster, prjSetting, dstSpatialRef, _progressCallback);
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

        [Test(Description = "测试投影输出范围")]
        public string TestDstExtendWGS84()
        {
            try
            {
                PrjEnvelope dstEnvelope;
                dstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
                srcFilename = @"D:\mas数据\Mersi\FY3A_MERSI_GBAL_L1_20110501_0250_1000M_MS.HDF";
                srcRaster = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_MERSI");
                proj.ComputeDstEnvelope(srcRaster, dstSpatialRef, out dstEnvelope, _progressCallback);
                Console.WriteLine(dstEnvelope.ToString());
                return dstEnvelope.ToString();
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

        [Test(Description = "测试投影输出范围")]
        public string TestDstExtendPole()
        {
            try
            {
                PrjEnvelope dstEnvelope;
                ISpatialReference dstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("North Pole Stereographic.prj");
                string srcFilename = @"D:\mas数据\Mersi\FY3A_MERSI_GBAL_L1_20110501_0250_1000M_MS.HDF";
                IRasterDataProvider srcRaster = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;

                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_MERSI");
                proj.ComputeDstEnvelope(srcRaster, dstSpatialRef, out dstEnvelope, _progressCallback);
                Console.WriteLine(dstEnvelope.ToString());
                return dstEnvelope.ToString();
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

        public Action<int, string> ProgressCallback { get { return _progressCallback; } set { _progressCallback = value; } }
    }
}
