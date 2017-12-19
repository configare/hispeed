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
    public class TestFy3AMersi0250Projection
    {
        protected FY3_MERSI_PrjSettings prjSetting = null;
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
            IRasterDataProvider secondFileRaster = null;
            try
            {
                ///等经纬度投影
                dstFilename = @"D:\mas数据\Mersi\fy3Mersi0250_Block.ldf";
                dstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");//
                srcFilename = @"D:\mas数据\Mersi\FY3A_MERSI_GBAL_L1_20110501_0250_0250M_MS.HDF";
                dstEnvelope = PrjEnvelope.CreateByCenter(110, 32, 5, 5);//
                string secondFile = @"D:\mas数据\Mersi\FY3A_MERSI_GBAL_L1_20110501_0250_1000M_MS.HDF";
                secondFileRaster = GeoDataDriver.Open(secondFile) as IRasterDataProvider;
                srcRaster = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
                //List<BandMap> bandmapList = new List<BandMap>();
                prjSetting = new FY3_MERSI_PrjSettings();
                prjSetting.OutResolutionX = 0.0025F;
                prjSetting.OutResolutionY = 0.0025F;
                prjSetting.OutPathAndFileName = dstFilename;
                prjSetting.OutFormat = "LDF";
                //prjSetting.OutEnvelope = dstEnvelope;
                //prjSetting.BandMapTable = bandmapList;
                //prjSetting.IsRadiation = false;
                prjSetting.SecondaryOrbitRaster = secondFileRaster;
                _progressCallback = progressCallback;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (secondFileRaster != null)
                    secondFileRaster.Dispose();
            }
        }

        //[Test(Description = "测试投影")]
        public void TestProjection()
        {
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_MERSI");
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
