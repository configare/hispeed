using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.FileProject;
using GeoDo.Project;
using System.IO;
using System.Diagnostics;
using GeoDo.RasterProject;
using GeoDo.ProjectDefine;

namespace test
{
    public partial class frmProjectionALL : Form
    {
        private Action<int, string> _progressCallback;
        public frmProjectionALL()
        {
            InitializeComponent();
            this.Load += new EventHandler(frmProjectionALL_Load);
        }

        void frmProjectionALL_Load(object sender, EventArgs e)
        {
            _progressCallback = new Action<int, string>(OutProgress);
            FileProjector.LoadAllFileProjectors();
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
            toolStripProgressBar1.Value = progerss;
            toolStripStatusLabel1.Text = text;
            Application.DoEvents();
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

        private IRasterDataProvider GetSrcRaster(string filename)
        {
            return GeoDataDriver.Open(filename) as IRasterDataProvider;
        }

        private string GetOutPutFile(string file)
        {
            return Path.Combine(textBox2.Text, Path.GetFileNameWithoutExtension(file) + "_prj.ldf");
        }

        private string GetOutPutFile(string file,int index)
        {
            return Path.Combine(Path.GetDirectoryName(file)+"\\Prj", Path.GetFileNameWithoutExtension(file) + index + "_prj.ldf");
        }

        private void WriteLine(string format, params object[] arg)
        {
            Write(format + "\r\n", arg);
        }

        private void Write(string format, params object[] arg)
        {
            toolStripStatusLabel1.Text = string.Format(format, arg);
        }

        private ISpatialReference GetOutSpatialRef()
        {
            try
            {
                string prjFile = textBox1.Text;
                if (string.IsNullOrWhiteSpace(prjFile))
                    return SpatialReference.GetDefault();
                else
                    return SpatialReference.FromPrjFile(prjFile);
            }
            catch
            {
                return SpatialReference.GetDefault();
            }
        }

        private string SetPrjFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.prj|*.prj";
                ofd.Title = "选择输出prj文件";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return ofd.FileName;
            }
            return null;
        }

        private string SetOutputDir()
        {
            using (FolderBrowserDialog ofd = new FolderBrowserDialog())
            {
                ofd.Description = "设置输出路径";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return ofd.SelectedPath;
            }
            return null;
        }

        private void PrjVIRR(string file)
        {
            IRasterDataProvider srcRaster = null;
            try
            {
                PrjEnvelope[] prjEnvelopes = new PrjEnvelope[] 
                {
                    PrjEnvelope.CreateByCenter(100, 30, 10, 10),
                    PrjEnvelope.CreateByCenter(110, 29, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 28, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 27, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 26, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 25, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 24, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 23, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 22, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 21, 10, 10),
                    PrjEnvelope.CreateByCenter(105, 20, 10, 10)
                };
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                srcRaster = GetSrcRaster(file);
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_VIRR");
                proj.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    try
                    {
                        GeoDo.RasterProject.PrjEnvelope prjEnvelope = prjEnvelopes[i];
                        FY3_VIRR_PrjSettings prjSetting = new FY3_VIRR_PrjSettings();
                        prjSetting.OutPathAndFileName = GetOutPutFile(file,i);
                        prjSetting.OutEnvelope = prjEnvelope;
                        prjSetting.OutResolutionX = 0.0025F;
                        prjSetting.OutResolutionY = 0.0025F;
                        ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
                        proj.Project(srcRaster, prjSetting, dstSpatialRef, _progressCallback);
                        stopwatch.Stop();
                        WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
                        this.Text = prjSetting.OutPathAndFileName;
                    }
                    catch (Exception ex)
                    {
                        this.Text = ex.Message;
                    }
                }
                proj.EndSession();
                stopwatch.Stop();
                WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                if (srcRaster != null)
                {
                    if (srcRaster.BandProvider != null)
                        srcRaster.BandProvider.Dispose();
                    srcRaster.Dispose();
                }
                GC.Collect();
            }
        }

        private void PrjMERSI1000(string file)
        {
            IRasterDataProvider srcRaster = null;
            try
            {
                GeoDo.RasterProject.PrjEnvelope[] prjEnvelopes = new GeoDo.RasterProject.PrjEnvelope[] 
                {
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(100, 30, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 29, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 28, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 27, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 26, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 25, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 24, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 23, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 22, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 21, 10, 10),
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(105, 20, 10, 10)
                };
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                srcRaster = GetSrcRaster(file);
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_MERSI");
                proj.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    try
                    {
                        GeoDo.RasterProject.PrjEnvelope prjEnvelope = prjEnvelopes[i];
                        FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                        prjSetting.OutPathAndFileName = GetOutPutFile(file,i);
                        prjSetting.OutEnvelope = prjEnvelope;// GeoDo.RasterProject.PrjEnvelope.CreateByCenter(90, 26, 10, 10);
                        prjSetting.OutResolutionX = 0.0025F;
                        prjSetting.OutResolutionY = 0.0025F;
                        ISpatialReference dstSpatialRef = GetOutSpatialRef();
                        proj.Project(srcRaster, prjSetting, dstSpatialRef, _progressCallback);
                        stopwatch.Stop();
                        WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
                        //MessageBox.Show("投影输出文件" + prjSetting.OutPathAndFileName);
                    }
                    catch (Exception ex)
                    {
                        this.Text = ex.Message;
                    }
                }
                proj.EndSession();
            }
            catch (Exception ex)
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
                GC.Collect();
            }
        }

        private void PrjMERSI0250(string file, string secfile)
        {
            IRasterDataProvider srcRaster = null;
            IRasterDataProvider secondFileRaster = null;
            try
            {
                GeoDo.RasterProject.PrjEnvelope[] prjEnvelopes = new GeoDo.RasterProject.PrjEnvelope[] 
                {
                    GeoDo.RasterProject.PrjEnvelope.CreateByCenter(100, 42, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(100, 36, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(100, 30, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 29, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 28, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 27, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 26, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 25, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 24, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 23, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 22, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 21, 10, 10),
                    //GeoDo.RasterProject.PrjEnvelope.CreateByCenter(110, 20, 10, 10)
                };
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                srcRaster = GetSrcRaster(file);
                if (!string.IsNullOrWhiteSpace(secfile))
                    secondFileRaster = GetSrcRaster(secfile);
                IFileProjector proj = FileProjector.GetFileProjectByName("FY3_MERSI");
                proj.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    try
                    {
                        GeoDo.RasterProject.PrjEnvelope prjEnvelope = prjEnvelopes[i];
                        FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                        prjSetting.SecondaryOrbitRaster = secondFileRaster;
                        prjSetting.OutPathAndFileName = GetOutPutFile(file, i);
                        //prjSetting.OutEnvelope = prjEnvelope;
                        ISpatialReference dstSpatialRef = GetOutSpatialRef();
                        proj.Project(srcRaster, prjSetting, dstSpatialRef, _progressCallback);
                        this.Text = prjSetting.OutPathAndFileName;
                    }
                    catch (Exception ex)
                    {
                        this.Text = ex.Message;
                    }
                }
                proj.EndSession();
                stopwatch.Stop();
                WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                if (secondFileRaster != null)
                    secondFileRaster.Dispose();
                if (srcRaster != null)
                {
                    if (srcRaster.BandProvider != null)
                        srcRaster.BandProvider.Dispose();
                    srcRaster.Dispose();
                }
                GC.Collect();
            }
        }

        private void PrjEos1000(string file, string locfile)
        {
            IRasterDataProvider srcRaster = null;
            IRasterDataProvider locRaster = null;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                IFileProjector proj = FileProjector.GetFileProjectByName("EOS");
                srcRaster = GetSrcRaster(file);
                locRaster = GetSrcRaster(locfile);
                EOS_MODIS_PrjSettings prjSetting = new EOS_MODIS_PrjSettings();
                prjSetting.OutPathAndFileName = GetOutPutFile(file);
                prjSetting.LocationFile = locRaster;
                ISpatialReference dstSpatialRef = GetOutSpatialRef();
                proj.Project(srcRaster, prjSetting, dstSpatialRef, _progressCallback);
                stopwatch.Stop();
                WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
                MessageBox.Show("投影输出文件" + prjSetting.OutPathAndFileName);
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
                GC.Collect();
            }
        }

        private void PrjNOAA(string file)
        {
            IRasterDataProvider srcRaster = null;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                IFileProjector proj = FileProjector.GetFileProjectByName("NOAA_1BD");
                srcRaster = GetSrcRaster(file);
                NOAA_PrjSettings prjSetting = new NOAA_PrjSettings();
                prjSetting.OutPathAndFileName = GetOutPutFile(file);
                ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
                proj.Project(srcRaster, prjSetting, dstSpatialRef, _progressCallback);
                stopwatch.Stop();
                WriteLine("数据投影{0}ms", stopwatch.ElapsedMilliseconds);
                //MessageBox.Show("投影输出文件" + prjSetting.OutPathAndFileName);
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            finally
            {
                if (srcRaster != null)
                {
                    if (srcRaster.BandProvider != null)
                        srcRaster.BandProvider.Dispose();
                    srcRaster.Dispose();
                }
                GC.Collect();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string prjFile = SetPrjFile();
            if (!string.IsNullOrWhiteSpace(prjFile))
                textBox1.Text = prjFile;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string prjFile = SetOutputDir();
            if (!string.IsNullOrWhiteSpace(prjFile))
                textBox2.Text = prjFile;
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

        private void button16_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择MERSI0250数据");
            string secfile = SelectFile("选择MERSI1000数据");
            if (file == null)
                return;
            PrjMERSI0250(file, secfile);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择Eos1000数据");
            string locfile = SelectFile("选择经纬度数据");
            if (file == null || locfile == null)
                return;
            PrjEos1000(file, locfile);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择NOAA数据");
            if (file == null)
                return;
            PrjNOAA(file);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmProjectionSetting frm = new frmProjectionSetting();
            frm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string fileName = SelectFile("选择轨道数据");
            ISpatialReference prj = SelectPrj("选择目标投影定义");
            ProjectionFactory quick = new ProjectionFactory();
            string[] outFiles = null;
            string errorMessage;
            try
            {
                PrjOutArg prjOutArg = new PrjOutArg(prj, null, 0, 0, Path.GetDirectoryName(fileName));
                outFiles = quick.Project(fileName, prjOutArg,_progressCallback, out errorMessage);
            }
            finally
            {
            }
        }

        private ISpatialReference SelectPrj(string p)
        {
            SpatialReferenceSelection frm = new SpatialReferenceSelection();
            frm.ShowDialog();
            return frm.SpatialReference;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SpatialReferenceSelection frm = new SpatialReferenceSelection();
            frm.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string file = SelectFile("选择数据");
            FileChecker c = new FileChecker();
            this.Text = c.GetFileType(file);
        }
    }
}
