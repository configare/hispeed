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
    public class TestFy3AMersiProjection
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
            try
            {
                ///等经纬度投影
                dstFilename = @"D:\mas数据\Mersi\fy3_Block.ldf";
                dstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");//
                srcFilename = @"D:\mas数据\Mersi\FY3A_MERSI_GBAL_L1_20110501_0250_1000M_MS.HDF";
                dstEnvelope = PrjEnvelope.CreateByCenter(116.890377, 27.7621965, 33.49, 21.53);
                
                srcRaster = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
                prjSetting = new FY3_MERSI_PrjSettings();
                //prjSetting.OutResolutionX = 0.01F;
                //prjSetting.OutResolutionY = 0.01F;
                prjSetting.OutPathAndFileName = dstFilename;
                prjSetting.OutFormat = "LDF";
                //prjSetting.OutEnvelope = dstEnvelope;
                //prjSetting.BandMapTable = bandmapList;
                prjSetting.IsRadiation = true;
                _progressCallback = progressCallback;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
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
