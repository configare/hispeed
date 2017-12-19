using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.Project;
using System.Diagnostics;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.FileProject;
using GeoDo.MEF;
using NUnit.Framework;

namespace test
{
    public partial class frmTestProjection : Form
    {
        public frmTestProjection()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestProj();
        }

        private void TestProj()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Size srSize = Size.Empty;
            Double[] lats = null;
            Double[] longs = null;
            using (IRasterDataProvider srcPrd = GeoDataDriver.Open(@"D:\mas数据\FY3A_VIRRX_GBAL_L1_20110322_0525_1000M_MS.HDF") as IRasterDataProvider)
            {
                IBandProvider srcbandpro = srcPrd.BandProvider as IBandProvider;
                {
                    srSize = new System.Drawing.Size(srcPrd.Width, srcPrd.Height);
                    lats = new Double[srcPrd.Width * srcPrd.Height];
                    longs = new Double[srcPrd.Width * srcPrd.Height];
                    using(IRasterBand latBand = srcbandpro.GetBands("Latitude")[0])
                    {
                        using(IRasterBand lonsBand = srcbandpro.GetBands("Longitude")[0])
                        {
                            unsafe
                            {
                                fixed (Double* ptrLat = lats)
                                {
                                    fixed (Double* ptrLong = longs)
                                    {
                                        IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                        IntPtr bufferPtrLong = new IntPtr(ptrLong);
                                        latBand.Read(0, 0, srcPrd.Width, srcPrd.Height, bufferPtrLat, enumDataType.Double, srcPrd.Width, srcPrd.Height);
                                        lonsBand.Read(0, 0, srcPrd.Width, srcPrd.Height, bufferPtrLong, enumDataType.Double, srcPrd.Width, srcPrd.Height);
                                    }
                                }
                            }
                        }
                    }
                    stopwatch.Stop();
                    WriteLine("读取经纬度{0}ms", stopwatch.ElapsedMilliseconds);
                    stopwatch.Restart();
                    IRasterProjector raster = new RasterProjector();
                    PrjEnvelope destEnvelope;
                    Action<int, string> progressCallback = new Action<int, string>(OutProgress);
                    //progressCallback = null;  //测试不用进度条的情况
                    ISpatialReference srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
                    ISpatialReference dstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("ChinaBoundary.prj");
                    raster.ComputeDstEnvelope(srcSpatialRef, longs, lats, srSize, dstSpatialRef, out destEnvelope, progressCallback);
                    stopwatch.Stop();
                    WriteLine("计算范围{0}ms", stopwatch.ElapsedMilliseconds);
                    WriteLine("范围{0}", destEnvelope.ToString());

                    Size dstSize = new Size((int)(destEnvelope.Width / 0.01), (int)(destEnvelope.Height / 0.01));
                    UInt16[] dstRowLookUpTable = new UInt16[dstSize.Width * dstSize.Height];
                    UInt16[] dstColLookUpTable = new UInt16[dstSize.Width * dstSize.Height];
                    raster.ComputeIndexMapTable(srcSpatialRef, longs, lats, srSize, dstSpatialRef, dstSize, destEnvelope,
                        out dstRowLookUpTable, out dstColLookUpTable, progressCallback);

                    stopwatch.Stop();
                    WriteLine("计算投影查找表{0}ms", stopwatch.ElapsedMilliseconds);
                    stopwatch.Restart();

                    int srcBandCount = srcPrd.BandCount;
                    using (IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
                    {
                        string proj4 = dstSpatialRef.ToProj4String();
                        using (IRasterDataProvider prdWriter = drv.Create(@"d:\Myproj4LutX.ldf", dstSize.Width, dstSize.Height, srcBandCount,
                            enumDataType.UInt16, "INTERLEAVE=BSQ", "VERSION=LDF", "SPATIALREF=" + proj4) as IRasterDataProvider)
                        {
                            UInt16[] dstData = new UInt16[dstSize.Width * dstSize.Height];
                            UInt16[] srcData = new UInt16[srSize.Width * srSize.Height];
                            //int perProgress = 0;
                            //int curProgress = 0;
                            for (int i = 0; i < srcBandCount; i++)
                            {
                                using (IRasterBand latBand = srcPrd.GetRasterBand(i + 1))
                                {
                                    unsafe
                                    {
                                        fixed (UInt16* ptr = srcData)
                                        {
                                            IntPtr bufferptr = new IntPtr(ptr);
                                            latBand.Read(0, 0, srSize.Width, srSize.Height, bufferptr, enumDataType.UInt16, srSize.Width, srSize.Height);
                                        }
                                    }
                                }
                                //stopwatch.Stop();
                                //WriteLine("读取一个通道{0}ms,通道索引{1}", stopwatch.ElapsedMilliseconds, i + 1);
                                //stopwatch.Restart();
                                raster.Project<UInt16>(srcData, srSize, dstRowLookUpTable, dstColLookUpTable, dstSize, dstData, 0, progressCallback);
                                //stopwatch.Stop();
                                //WriteLine("投影一个通道{0}ms,通道索引{1}", stopwatch.ElapsedMilliseconds, i + 1);
                                //stopwatch.Restart();

                                using (IRasterBand band = prdWriter.GetRasterBand(i + 1))
                                {
                                    unsafe
                                    {
                                        fixed (UInt16* ptr = dstData)
                                        {
                                            IntPtr bufferPtr = new IntPtr(ptr);
                                            band.Write(0, 0, band.Width, band.Height, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                                        }
                                    }
                                }
                                //curProgress = (i+1) * 100 / srcBandCount;
                                //if (progressCallback != null && curProgress > perProgress)
                                //{
                                //    progressCallback(curProgress, "");
                                //    perProgress = curProgress;
                                //}
                                //stopwatch.Stop();
                                //WriteLine("写出一个通道{0}ms", stopwatch.ElapsedMilliseconds);
                                //stopwatch.Restart();
                            }
                        }
                    }
                    stopwatch.Stop();
                    WriteLine("投影完所有通道{0}ms", stopwatch.ElapsedMilliseconds);
                    stopwatch.Restart();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Action<int, string> progressCallback = new Action<int, string>(OutProgress);
            TestFy3AVirrProjection fyp = new TestFy3AVirrProjection();
            fyp.ProgressCallback = progressCallback;
            fyp.DataReady();
            fyp.TestFy3VIRR();
            stopwatch.Stop();
            WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
        }

        #region 基础方法
        private void WriteLine(string format, params object[] arg)
        {
            Write(format + "\r\n", arg);
        }

        private void Write(string format, params object[] arg)
        {
            richTextBox1.AppendText(string.Format(format, arg));
        }

        private void OutProgress(int progerss, string text)
        {
            if (InvokeRequired)
                this.Invoke(new Action<int, string>(UpdateProgress), progerss, text);
            else
                UpdateProgress(progerss, text);
        }

        private void UpdateProgress(int progerss, string text)
        {
            progressBar1.Value = progerss;
            this.Text = (progerss + text);
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            TestFy3AMersiProjection fyp = new TestFy3AMersiProjection();
            Action<int, string> progressCallback = new Action<int, string>(OutProgress);
            fyp.DataReady(progressCallback);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            fyp.TestProjection();
            stopwatch.Stop();
            WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TestFy3AMersiProjection frm = new TestFy3AMersiProjection();
            frm.ProgressCallback = new Action<int, string>(OutProgress);
            WriteLine(frm.TestDstExtendWGS84());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TestFy3AMersiProjection frm = new TestFy3AMersiProjection();
            frm.ProgressCallback = new Action<int, string>(OutProgress);
            WriteLine(frm.TestDstExtendPole());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TestFy3AMersi0250Projection fyp = new TestFy3AMersi0250Projection();
            Action<int, string> progressCallback = new Action<int, string>(OutProgress);
            fyp.DataReady(progressCallback);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            fyp.TestProjection();
            stopwatch.Stop();
            WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            TestHdfRead t = new TestHdfRead();
            t.TestReadHDF();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            TestEosProjection t = new TestEosProjection();
            t._progressCallback = new Action<int, string>(OutProgress);
            t.TestProj();
            stopwatch.Stop();
            WriteLine("{0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            TestNoaa1bdProjection t = new TestNoaa1bdProjection();
            t.TestProj();
            stopwatch.Stop();
            WriteLine("{0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //TestRadiationProvider t = new TestRadiationProvider();
            //t.TestCreateRadiationProvider();
            //stopwatch.Stop();
            //WriteLine("{0}ms", stopwatch.ElapsedMilliseconds);
            //stopwatch.Restart();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            TestEosProjection0500 fyp = new TestEosProjection0500();
            Action<int, string> progressCallback = new Action<int, string>(OutProgress);
            fyp.DataReady(progressCallback);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            fyp.TestProjection();
            stopwatch.Stop();
            WriteLine("{0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
        }

        private IRasterDataProvider GetSrcRaster(string filename)
        {
            return GeoDataDriver.Open(filename) as IRasterDataProvider;
        }

        private string SelectFile(string cap)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = cap;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return ofd.FileName;
            }
            return null;
        }

        private void PrjVIRR(string file)
        {
            IRasterDataProvider srcRaster = null;
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_VIRR");
                srcRaster = GetSrcRaster(file);
                FY3_VIRR_PrjSettings prjSetting = new FY3_VIRR_PrjSettings();
                prjSetting.OutPathAndFileName = @"D:\T.ldf";
                ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
                proj.Project(srcRaster, prjSetting, dstSpatialRef, null);
                MessageBox.Show("FY3_VIRR"+"投影输出文件" + @"D:\T.ldf");
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

        private void PrjMERSI1000(string file)
        {
            IRasterDataProvider srcRaster = null;
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_MERSI");
                srcRaster = GetSrcRaster(file);
                FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                prjSetting.OutPathAndFileName = @"D:\T.ldf";
                ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
                proj.Project(srcRaster, prjSetting, dstSpatialRef, null);
                MessageBox.Show("FY3_MERSI" + "投影输出文件" + @"D:\T.ldf");
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

        private void PrjMERSI0250(string file,string secfile)
        {
            IRasterDataProvider srcRaster = null;
            IRasterDataProvider secRaster = null;
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_MERSI");
                srcRaster = GetSrcRaster(file);
                if (!string.IsNullOrWhiteSpace(secfile))
                    secRaster = GetSrcRaster(secfile);
                FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                prjSetting.SecondaryOrbitRaster = secRaster;
                prjSetting.OutPathAndFileName = @"D:\T.ldf";
                ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
                proj.Project(srcRaster, prjSetting, dstSpatialRef, null);
                MessageBox.Show("FY3_MERSI" + "投影输出文件" + @"D:\T.ldf");
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

        private void PrjEos1000(string file, string locfile)
        {
            IRasterDataProvider srcRaster = null;
            IRasterDataProvider locRaster = null;
            try
            {
                IFileProjector proj = FileProjector.GetFileProjectByName("EOS");
                srcRaster = GetSrcRaster(file);
                locRaster = GetSrcRaster(locfile);
                EOS_MODIS_PrjSettings prjSetting = new EOS_MODIS_PrjSettings();
                prjSetting.OutPathAndFileName = @"D:\T.ldf";
                prjSetting.LocationFile = locRaster;
                ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
                proj.Project(srcRaster, prjSetting, dstSpatialRef, null);
                MessageBox.Show("FY3_MERSI" + "投影输出文件" + @"D:\T.ldf");
            }
            finally
            {
                if (srcRaster != null)
                {
                    if (srcRaster.BandProvider != null)
                        srcRaster.BandProvider.Dispose();
                    srcRaster.Dispose();
                    locRaster.Dispose();
                }
            }
        }
        
        private void button13_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择VIRR数据");
            if (file == null)
                return;
            PrjVIRR(file);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择MERSI1000数据");
            if (file == null)
                return;
            PrjMERSI1000(file);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择Eos1000数据");
            string locfile = SelectFile("选择经纬度数据");
            if (file == null || locfile==null)
                return;
            PrjEos1000(file,locfile);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择MERSI0250数据");
            string secfile = SelectFile("选择MERSI1000数据");
            if (file == null)
                return;
            PrjMERSI0250(file, secfile);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择数据");
            if (file == null)
                return;
            ReadFile(file);
        }

        private void ReadFile(string file)
        {
            IRasterDataProvider srcRaster = null;
            try
            {
                srcRaster = GetSrcRaster(file);
                if (srcRaster == null || srcRaster.BandCount==0)
                    return;
                {
                    IRasterBand latBand = srcRaster.GetRasterBand(1);
                    {
                        Size srSize = new Size(latBand.Width, latBand.Height);
                        UInt16[] lats = new UInt16[srSize.Width * srSize.Height];
                        unsafe
                        {
                            fixed (UInt16* ptrLat = lats)
                            {
                                IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                latBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLat, enumDataType.UInt16, srSize.Width, srSize.Height);
                            }
                        }
                    }
                }
                MessageBox.Show("读取一个通道后输出文件" + @"D:\T.ldf");
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

        private void button18_Click(object sender, EventArgs e)
        {
            string wkt = "GEOGCS[\"等经纬度投影\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";
            wkt = "PROJCS[\"Hammer投影\",GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Hammer-Aitoff (world)\"]PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],PARAMETER[\"central_meridian\",105],UNIT[\"Meter\",1]]";
            ISpatialReference srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByWKT(wkt, enumWKTSource.EsriPrjFile);
            string proj4 = srcSpatialRef.ToProj4String();
            ISpatialReference inverSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(proj4);
            inverSpatialRef.IsSame(srcSpatialRef);

            IProjectionTransform tr = ProjectionTransformFactory.GetProjectionTransform(srcSpatialRef, SpatialReference.GetDefault());
            double[] x = new double[] { 1534910 };
            double[] y = new double[] { 4255978 };
            x = new double[] { 121.913 };
            y = new double[] { 38.957 };
            tr.InverTransform(x, y);
            tr.Transform(x, y);
        }

        /// <summary>
        /// 本示例 将一个tiff投影转换为一个等经纬度的tiff
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e)
        {
            string filename = SelectFile("选择数据");
            if (filename == null)
                return;
            ProjectedFileTransform trans = new ProjectedFileTransform();
            IRasterDataProvider srcRaster = GeoDataDriver.Open(filename) as IRasterDataProvider;   //输入数据，必须是已经投影的数据。
            FilePrjSettings prjSettings = new FilePrjSettings();
            prjSettings.OutBandNos = null;                      //从1开始的波段号，为空代表全波段
            prjSettings.OutFormat = "TIFF";                     //输出格式（输入LDF或者TIFF），空值代表LDF
            prjSettings.OutPathAndFileName = @"E:\testG.tiff"; //输出文件名，必须提供，完整的全路径文件名
            //prjSettings.OutEnvelope = new PrjEnvelope(-180,180,-80,80);                    //输出地理范围，空值代表全数据
            //prjSettings.OutResolutionX = 0.01f;              //实际的输出分辨率。不填或0代表与输入数据分辨率一致
            //prjSettings.OutResolutionY = 0.01f;
            ISpatialReference dstSpatialRef = SpatialReference.GetDefault();//目标投影
            Action<int, string> progressCallback = new Action<int, string>(OutProgress);//进度条
            trans.Project(srcRaster, prjSettings, dstSpatialRef, progressCallback);
        }

        private void button20_Click(object sender, EventArgs e)
        {

        }
    }
}
