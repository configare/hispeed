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
using GeoDo.ProjectDefine;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void button1_Click(object sender, EventArgs e)
        {
            ISpatialReference srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
            string prj4 = srcSpatialRef.ToProj4String();
            ISpatialReference dstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
            using (IProjectionTransform tran = ProjectionTransformFactory.GetProjectionTransform(srcSpatialRef, dstSpatialRef))
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (frmTestProjection frm = new frmTestProjection())
            {
                frm.ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IFileProjector[] prjs = FileProjector.LoadAllFileProjectors();
            Console.WriteLine("...");

            IFileProjector prj = FileProjector.GetFileProjectByName("SINGLEFILE");
            FilePrjSettings settings = prj.CreateDefaultPrjSettings();
            IPrjSettingsEditor editor = GetSettingEditor(settings);
            if (editor != null)
                editor.Apply(settings);
            //
        }

        private IPrjSettingsEditor GetSettingEditor(FilePrjSettings settings)
        {
            using (IComponentLoader<IPrjSettingsEditor> loader = new ComponentLoader<IPrjSettingsEditor>())
            {
                IPrjSettingsEditor[] editors = null;// loader.LoadComponents(Configer.GEODO_RSS_DIR_FILEPRJECTORS);
                foreach (IPrjSettingsEditor ed in editors)
                {
                    if (ed.IsSupport(settings.GetType()))
                        return ed;
                }
            }
            return null;
        }

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    ISpatialReference srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
        //    srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("Mercator (sphere).prj");
        //    string proj4 = srcSpatialRef.ToProj4String();
        //    srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(proj4);
        //}

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    TestFy3AVirrProjection fyp = new TestFy3AVirrProjection();
        //    Action<int, string> progressCallback = new Action<int, string>(OutProgress);
        //    fyp.DataReady(progressCallback);
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    fyp.TestFy3VIRR();
        //    stopwatch.Stop();
        //    WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
        //    stopwatch.Restart();
        //}

        private void TestPrj4Parser_Click(object sender, EventArgs e)
        {
            ISpatialReference srcSpatialRef = null;// SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
            string prjFile = "f:\\32600.prj";
            srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("Mercator (sphere).prj");
            string proj4 = srcSpatialRef.ToProj4String();
            ISpatialReference inverSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(proj4);

            bool isSame = srcSpatialRef.IsSame(inverSpatialRef);
        }

        private void TestSpatialRefUI_Click(object sender, EventArgs e)
        {
            //SpatialReferenceSelection spatialReferenceSelUI = new SpatialReferenceSelection();
            //spatialReferenceSelUI.Visible = true;
            //(spatialReferenceSelUI as Form).Text = spatialReferenceSelUI.Name;
            using (SpatialReferenceSelection spatialReferenceSelUI = new SpatialReferenceSelection())
            {
                if (spatialReferenceSelUI.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    MessageBox.Show(spatialReferenceSelUI.SpatialReference != null ?
                        spatialReferenceSelUI.SpatialReference.ToString() : string.Empty);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string filename = @"D:\masData\virr\FY3A_VIRRX_GBAL_L1_20100801_0825_1000M_MS.HDF";
            IRasterDataProvider srcRaster = GetSrcRaster(filename);
            Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
            double[] longs;
            double[] lats;
            double[] needFindLats = new double[] { 70.24819, 71.141205, 72.315506 };
            double[] needFindLongs = new double[] { 46.420624, 46.38007, 54.442673 };
            int[] outCols = new int[3];
            int[] outRows = new int[3];
            ReadLatLong(srcRaster, out longs, out lats);
            RasterForShape r = new RasterForShape();
            r.Find(longs, lats, srcSize, needFindLongs, needFindLats, out outCols, out outRows);
        }

        private IRasterDataProvider GetSrcRaster(string filename)
        {
            return GeoDataDriver.Open(filename) as IRasterDataProvider;
        }

        private void ReadLatLong(IRasterDataProvider srcRaster, out double[] longitudes, out double[] latitudes)
        {
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                {
                    Size srSize = new System.Drawing.Size(srcRaster.Width, srcRaster.Height);
                    longitudes = new Double[srcRaster.Width * srcRaster.Height];
                    latitudes = new Double[srcRaster.Width * srcRaster.Height];
                    using (IRasterBand lonBand = srcbandpro.GetBands("Longitude")[0])
                    {
                        using (IRasterBand latBand = srcbandpro.GetBands("Latitude")[0])
                        {
                            unsafe
                            {
                                fixed (Double* ptrLong = longitudes)
                                fixed (Double* ptrLat = latitudes)
                                {
                                    {
                                        IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                        IntPtr bufferPtrLong = new IntPtr(ptrLong);
                                        latBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLat, enumDataType.Double, srSize.Width, srSize.Height);
                                        lonBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLong, enumDataType.Double, srSize.Width, srSize.Height);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取经纬度数据集失败:" + ex.Message, ex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            long mem = MemoryHelper.GetMaxArray();
        }

        /// <summary>
        /// H2A_SM2B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            Action<int, string> progress = new Action<int, string>(OutProgress);
            string dir = textBox1.Text;
            string outDir = textBox2.Text;
            string[] files = Directory.GetFiles(dir, "*.h5");
            StringBuilder msgInfo = new StringBuilder();
            for (int i = 0; i < files.Length; i++)
            {
                IRasterDataProvider mainRaster = null;
                IRasterDataProvider locationRaster = null;
                string file = files[i];
                try
                {
                    progress((int)((i + 1f) / files.Length * 100), null);
                    string[] openArgs = new string[] { "datasets=" + "wind_dir_selection,wind_speed_selection" };
                    mainRaster = RasterDataDriver.Open(file, openArgs) as IRasterDataProvider;
                    string[] locationArgs = new string[] { "datasets=" + "wvc_lon,wvc_lat", "geodatasets=" + "wvc_lon,wvc_lat" };
                    locationRaster = RasterDataDriver.Open(file, locationArgs) as IRasterDataProvider;
                    if (locationRaster == null || locationRaster.BandCount == 0)
                    {
                        msgInfo.AppendLine("经纬度HDF数据文件，不存在经纬度数据集[Longitude,Latitude]" + file);
                        return;
                    }
                    string outFilename = Path.Combine(outDir, Path.GetFileNameWithoutExtension(file) + ".LDF");
                    FY3L2L3FilePrjSettings setting = new FY3L2L3FilePrjSettings();
                    setting.ExtArgs = new object[] { "360" }; //360代表，大于180度的经纬度值，实际需要转换
                    setting.LocationFile = locationRaster;
                    setting.OutFormat = "LDF";
                    setting.OutPathAndFileName = outFilename;
                    setting.OutResolutionX = 0.25f;
                    setting.OutResolutionY = 0.25f;
                    FY3L2L3FileProjector projector = new FY3L2L3FileProjector();
                    projector.Project(mainRaster, setting, SpatialReference.GetDefault(), null);
                }
                finally
                {
                    if (mainRaster != null)
                    {
                        mainRaster.Dispose();
                        mainRaster = null;
                    }
                    if (locationRaster != null)
                    {
                        locationRaster.Dispose();
                        locationRaster = null;
                    }
                }
            }
        }

        /// <summary>
        /// H2A_RM2B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            Action<int, string> progress = new Action<int, string>(OutProgress);
            string dir = textBox1.Text;
            string outDir = textBox2.Text;
            string[] files = Directory.GetFiles(dir, "*.h5");
            StringBuilder msgInfo = new StringBuilder();

            for (int i = 0; i < files.Length; i++)
            {
                FY3L2L3FilePrjSettings setting = new FY3L2L3FilePrjSettings();
                setting.OutFormat = "LDF";
                setting.OutResolutionX = 0.25f;
                setting.OutResolutionY = 0.25f;
                //扩展：经纬度数据集的放大倍数
                Dictionary<string, double> args = new Dictionary<string, double>();
                args.Add("xzoom", 0.000001d);
                args.Add("yzoom", 0.000001d);
                setting.ExtArgs = new object[] { args };

                IRasterDataProvider mainRaster = null;
                IRasterDataProvider locationRaster = null;
                string file = files[i];
                try
                {
                    progress((int)((i + 1f) / files.Length * 100), null);
                    string[] openArgs = new string[] { "datasets=" + "Res0_ap,Res0_cl,Res0_ic,Res0_sst,Res0_ssw,Res0_wv,Res10_sst,Res10_ssw,Res18_ap,Res18_cl,Res18_ic,Res18_ssw,Res18_wv,Res6_sst" };
                    mainRaster = RasterDataDriver.Open(file, openArgs) as IRasterDataProvider;
                    string[] locationArgs = new string[] { "datasets=" + "Long_of_Observation_Point,Lat_of_Observation_Point", "geodatasets=" + "Long_of_Observation_Point,Lat_of_Observation_Point" };
                    locationRaster = RasterDataDriver.Open(file, locationArgs) as IRasterDataProvider;
                    if (locationRaster == null || locationRaster.BandCount == 0)
                    {
                        msgInfo.AppendLine("经纬度HDF数据文件，不存在经纬度数据集[Longitude,Latitude]" + file);
                        return;
                    }
                    string outFilename = Path.Combine(outDir, Path.GetFileNameWithoutExtension(file) + ".LDF");
                    setting.OutPathAndFileName = outFilename;
                    setting.LocationFile = locationRaster;
                    FY3L2L3FileProjector projector = new FY3L2L3FileProjector();
                    projector.Project(mainRaster, setting, SpatialReference.GetDefault(), null);
                }
                finally
                {
                    if (mainRaster != null)
                    {
                        mainRaster.Dispose();
                        mainRaster = null;
                    }
                    if (locationRaster != null)
                    {
                        locationRaster.Dispose();
                        locationRaster = null;
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            frmFY3OrbitProductProjection frm = new frmFY3OrbitProductProjection();
            frm.ShowDialog();
        }
    }
}
