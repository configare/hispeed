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
using GeoDo.RSS.Config;
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
            using (IRasterDataProvider srcPrd = GeoDataDriver.Open(@"D:\masData\FY3A_VIRRX_GBAL_L1_20110322_0525_1000M_MS.HDF") as IRasterDataProvider)
            {
                using (IBandProvider srcbandpro = srcPrd.BandProvider as IBandProvider)
                {
                    srSize = new System.Drawing.Size(srcPrd.Width, srcPrd.Height);
                    lats = new Double[srcPrd.Width * srcPrd.Height];
                    longs = new Double[srcPrd.Width * srcPrd.Height];
                    using (IRasterBand latBand = srcbandpro.GetBands("Latitude")[0])
                    {
                        using (IRasterBand lonsBand = srcbandpro.GetBands("Longitude")[0])
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
            Action<int, string> progressCallback = new Action<int, string>(OutProgress);
            TestFy3AVirrProjection fyp = new TestFy3AVirrProjection();
            fyp.DataReady();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    Test_Project p = new Test_Project();
        //    p.Porject_VIRR_FULL();
        //    MessageBox.Show("ok.");
        //}
    }
}
