using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Prds.CLD;
using System.IO;
using GeoDo.RSS.RasterTools;
using CodeCell.AgileMap.Core;
using GeoDo.RasterProject;
using System.Threading;
using System.Diagnostics;
using GeoDo.RSS.Core.DF;
using GeoDo.MathAlg;
using GeoDo.RSS.MIF.Core;
using System.Xml.Linq;
using OSGeo.GDAL;
using GeoDo.RSS.DF.GDAL;
using GeoDo.HDF5;
using GeoDo.RSS.MIF.Prds.VGT;
using GeoDo.FileProject;
using GeoDo.RSS.DF.AWX;
using GeoDo.RSS.MIF.Prds.MWS;
using GeoDo.Tools;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmMod06DataPro frm = new frmMod06DataPro();
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataContinuityCheck chk = new DataContinuityCheck();
            chk.Show();
        }

        //数据库检索
        private void button3_Click(object sender, EventArgs e)
        {
            //GeoDo.RSS.MIF.Prds.CLD.CLDDataRetrieval re = new CLDDataRetrieval();
            //FormRetrieval re = new FormRetrieval();
            //re.Show();
            //CLDDataRetrieval frm = new CLDDataRetrieval();
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm._openEventhandler += new CLDDataRetrieval.DoubleClickRowHdrEventHandler(OpenRetrievalFile);
            //frm.Show();
            try
            {
                CLDPrdsDataRetrieval nfrm = new CLDPrdsDataRetrieval();
                nfrm.StartPosition = FormStartPosition.CenterScreen;
                nfrm._openEventhandler += new CLDPrdsDataRetrieval.DoubleClickRowHdrEventHandler(OpenRetrievalFile);
                nfrm.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void OpenRetrievalFile(object source, DoubleClickRowHdrEventArgs args)
        {
            //ICommand cmd = _session.CommandEnvironment.Get(2000);
            //if (cmd != null)
            //    cmd.Execute(args.FullName);
        }


        //数据入库
        private void button4_Click(object sender, EventArgs e)
        {
            FileToDatabase f2db = new FileToDatabase();
            f2db.Show();
        }

        //周期合成
        private void button5_Click(object sender, EventArgs e)
        {
            string sensor = textBox4.Text.ToUpper().Contains("MOD") ? "MODIS" : "AIRS";
            //PeriodMergeTips pm = new PeriodMergeTips(sensor);
            //if (pm.ParseArgsXml())
            //    pm.Show();
            //else
            //    MessageBox.Show("文件配置出错，请确认配置文件存在！");
            try
            {
                PeriodComp pm = new PeriodComp(sensor);
                if (pm.ParseArgsXml())
                    pm.Show();
                else
                    MessageBox.Show("文件配置出错，请确认配置文件存在！");                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //MODIS文件名规范化，去除，版本号及文件生成信息
            string inputdir = textBox1.Text;

        }

        public void RegularMODFileNames(string file)
        {
            string nfname = Path.GetFileNameWithoutExtension(file);
            string[] parts = nfname.Split('.');
            string nameformat = "{0}.{1}.{2}.hdf";
            string newfname = string.Format(nameformat, parts[0], parts[1], parts[2]);
            String newName = Path.Combine(Path.GetDirectoryName(file), newfname);
            if (File.Exists(newName))
            {
                File.Delete(newName);
            }
            System.IO.File.Move(file, newName);
        }

        public void RegularProFileNames(string file)
        {
            string fname = Path.GetFileName(file);
            string nfname = fname.Replace("-", "_");
            String newName = Path.Combine(Path.GetDirectoryName(file), nfname);
            if (File.Exists(newName))
            {
                File.Delete(newName);
            }
            System.IO.File.Move(file, newName);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //投影，日拼接文件名规范化，-替换为_
            string inputdir = textBox2.Text;
            string[] files = Directory.GetFiles(inputdir, "*day*.ldf", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileNameWithoutExtension(file).Contains('-'))
                {
                    RegularProFileNames(file);
                }
            }
            string[] hdrfiles = Directory.GetFiles(inputdir, "*day*.hdr", SearchOption.AllDirectories);
            foreach (string file in hdrfiles)
            {
                if (Path.GetFileNameWithoutExtension(file).Contains('-'))
                {
                    RegularProFileNames(file);
                }
            }


        }

        private void button8_Click(object sender, EventArgs e)
        {
            //投影文件规范化
            string inputdir = textBox3.Text;
            string[] files = Directory.GetFiles(inputdir, "*granule*.ldf", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileNameWithoutExtension(file).Contains('-'))
                {
                    RegularProFileNames(file);
                }
            }
            string[] hdrfiles = Directory.GetFiles(inputdir, "*granule*.hdr", SearchOption.AllDirectories);
            foreach (string file in hdrfiles)
            {
                if (Path.GetFileNameWithoutExtension(file).Contains('-'))
                {
                    RegularProFileNames(file);
                }
            }


        }

        //测试直方图统计
        private void button9_Click(object sender, EventArgs e)
        {
            //string dir = @"D:\Geodo\Prj_云参数\项目文档\文档-统计分析\SVD分解测试\测试数据\isccp\12";
            //string[] files = Directory.GetFiles(dir, "*.gpc");
            //int[] bands = new int[] { 34, 35, 46 };
            string file = @"D:\Geodo\试验数据\FY3A数据驱动\00VGT_NDVI_FY3A_VIRR_1000M_20130426024500.dat";
            string []files=null;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "场数据(*.dat;*.ldf;*.gpc;*.000)|*.dat;*.ldf;*.gpc;*.000|所有文件(*.*)|*.*";
                dlg.InitialDirectory = @"E:\周期合成产品\CloudFractionDay\TERRA\MODIS\day\0.05\Month\Avg";
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    files = dlg.FileNames;
                }
            }

            CloudParaFileStatics aaa=new CloudParaFileStatics();
            string[] fnam = aaa.FilesSeriesMeanStat(files, 1, null, Path.GetDirectoryName(files[0]), null);
            //using (IRasterDataProvider datap = GeoDataDriver.Open(file) as RasterDataProvider)
            //{                
            //    double[] values;
            //    if (CloudParaFileStatics.ComputeMinMaxAvg(datap, datap.DataType, new int[] { 1 }, null, out values, null))
            //    {
            //        if (values != null)
            //            foreach (int value in values)
            //           {
            //               Console.WriteLine(value.ToString());
            //           }
            //    }

            //}

            //CloudParaFileStatics st = new CloudParaFileStatics();
            //Dictionary<int, RasterQuickStatResult> results = null;// st.FilesHistoStat(files, bands, new string[] { "-1000" }, null);
            //results = st.FilesHistoStat(new string[] { file }, new int[] { 1 }, null,  null);
            //frmRasterQuickStat frm = new frmRasterQuickStat();//)
            //{
            //    //frm.Owner = _smartSession.SmartWindowManager.MainForm as Form;
            //    frm.StartPosition = FormStartPosition.CenterScreen;
            //    frm.Apply(file, results);
            //    frm.Show();
            //}
            //string name = Guid.NewGuid().ToString() + ".ldf";
            //string tempf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TEMP", name);


        }

        private void button10_Click(object sender, EventArgs e)
        {
            DataBaseArgsSet frm = new DataBaseArgsSet();
            frm.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            frmMod06DataPro frm = new frmMod06DataPro("AIRS");
            frm.Show();

        }

        //测试相关系数计算
        private void button12_Click(object sender, EventArgs e)
        {
            string dirL = @"D:\Geodo\Prj_云参数\数据\DATA_MOD06\日拼接产品\CloudTopTemperature\AQUA\MODIS\2011\1\day";
            string[] lfiles = Directory.GetFiles(dirL, "*.LDF");
            string dirR = @"D:\Geodo\Prj_云参数\数据\DATA_MOD06\周期合成产品\CloudTopTemperature\AQUA\MODIS\day\Ten\Avg";
            string[] rfiles = Directory.GetFiles(dirR, "*.LDF");
            int leftBandNum = 1;
            int rightBandNum = 1;
            string[] fillvalueR = new string[2] { "-32767", "-32768" };
            string[] fillvalueL = new string[2] { "-32767", "-32768" };
            CloudParaFileStatics st = new CloudParaFileStatics();
            long scL, scR;
            double correla = st.FilesCorrelateStat(lfiles, leftBandNum, fillvalueL,false, rfiles, rightBandNum, fillvalueR,false, null, out scL, out scR);


        }

        private void btnstatregionset_Click(object sender, EventArgs e)
        {
            StatRegionSet frm = new StatRegionSet();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (StatRegionSet.UseRegion)
            {
                MessageBox.Show("使用自定义区域！");
            }
            else
                MessageBox.Show("不使用自定义区域！");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //MicapsDataProcess mp = new MicapsDataProcess();
            //string[] displayNames = mp.DisplayNames;
            //List <int> indexs =new List<int>();
            //foreach (string name in displayNames)
            //{
            //        indexs.Add(MicapsDataProcess.Index(name));
            //}
            //return;
            string dirL = @"D:\Geodo\Micaps\140507";
            string [] filesL =Directory.GetFiles(dirL,"*.000",SearchOption.TopDirectoryOnly);
            string dirR = @"D:\Geodo\Micaps";
            string [] filesR =Directory.GetFiles(dirR,"*.000",SearchOption.TopDirectoryOnly);

            //float[] data =MicapsDataProcess.GetMicapsData(files,2,null);
            int leftBandNum = 6;
            string[] fillvalue =new string[1]{"9999"};
            string rdir = @"D:\Geodo\Micaps\140507\ldf";
            string[] rasterfiles = Directory.GetFiles(rdir, "*.ldf", SearchOption.TopDirectoryOnly);
            string[] fillValueRst = new string[2] { "-32767", "-32768" };
            long  cr,cl;
            double[][] rasterdata;
            double[][] micapsdata;
            //MicapsDataProcess.GetMicapsDataMatrixs(filesL, 6, fillvalue, filesR, 6, fillvalue, null, out rasterdata, out micapsdata);
            //MicapsDataProcess.GetMatchedMatrixs(rasterfiles, 1, fillValueRst, files, 6, fillvalue, null, out rasterdata, out micapsdata);
            //CloudParaFileStatics st = new CloudParaFileStatics();

            //double cor = st.MicapsCorrelateStat(files, leftBandNum, fillvalue, true, files, 9, fillvalue, true, null, out cl, out cr);
            //double cor = st.MicapsCorrelateStat(files, leftBandNum, fillvalue, true, rasterfiles, 1, fillValueRst, false, null, out cl, out cr);
            //if (cor != 0)
            {
                //frmRasterQuickStat frm = new frmRasterQuickStat();//)
                //{
                //    frm.StartPosition = FormStartPosition.CenterScreen;
                //    frm.Apply(files[0], results);
                //    frm.Show();
                //}
            }

        }

        //测试micaps对应栅格抽取点
        private void button15_Click(object sender, EventArgs e)
        {
            //string rdir = @"D:\Geodo\Micaps\140507\ldf";
            //string[] rasterfiles = Directory.GetFiles(rdir, "*.ldf", SearchOption.TopDirectoryOnly);
            //int bandNum=1;
            //string[] fillValueRst = new string[2] { "-32767","-32768" };
            //string dir = @"D:\Geodo\Micaps\140507";
            //string[] micapsFiles = Directory.GetFiles(dir, "*.000", SearchOption.TopDirectoryOnly);
            //int mbandNo=6;
            //string [] fillValuestr=new string [1] {"9999"};
            //Envelope envelope=null;
            //PrjEnvelope prjenvelope = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
            //if (prjenvelope != null)
            //    envelope = new Envelope(prjenvelope.MinX, prjenvelope.MinY, prjenvelope.MaxX, prjenvelope.MaxY);
            //double[] rasterdata;
            //double[] micapsdata;
            //MicapsDataProcess.GetMatchedValues(rasterfiles, bandNum, fillValueRst, micapsFiles, mbandNo, fillValuestr, envelope, out rasterdata, out micapsdata);
            //CloudParaStat st = new CloudParaStat();
            //double results;
            //    results = st.CalculateCorrelationCoefficient(rasterdata, micapsdata);
            //string list = "CloudFraction_MOD06_china_granule_201102130820.LDF";
            //string monthdir = @"D:\5分钟段产品\CloudFraction\AQUA\MODIS\2011\1\1\day\0.05";
            //string tempName = Path.GetFileNameWithoutExtension(list);
            //string blockNum = tempName.Split('_')[4].Substring(8, 4);
            //int start = monthdir.LastIndexOf("\\") + 1;
            //string resl = monthdir.Substring(start, monthdir.Length - start);

            //if (blockNum != "" || resl != "")
            //{
            //}
            string dir = @"D:\5分钟段产品\CloudWaterPath\AQUA\MODIS\2011";
            string[] files = Directory.GetFiles(dir, "*.ldf", SearchOption.AllDirectories);
            foreach (string file in files )
            {
                string fname = Path.GetFileNameWithoutExtension(file);
                if (fname.EndsWith("_0.01"))
                {
                    continue;
                }
                string fpath = Path.GetDirectoryName(file);
                string ext = Path.GetExtension(file);
                fname += "_0.01";
                string newname = Path.Combine(fpath, fname + ext);
                System.IO.File.Move(file, newname);
            }
        }

        private void btnMRTS_Click(object sender, EventArgs e)
        {
                Thread runTaskThread = new Thread(new ThreadStart(this.DoProcess));
                runTaskThread.Start();
        }

        string _sds = "",_of="";
        private void DoProcess()
        {
            string exedir = @"D:\MRTS\bin";
            string exeName = Path.Combine(exedir, "swath2grid.exe");
            //string exeName = AppDomain.CurrentDomain.BaseDirectory + "swath2grid.exe";
            //string prmfname = Path.Combine(exedir,"TmpParam.prm");
            //string prmfname = AppDomain.CurrentDomain.BaseDirectory + "TmpParam.prm";
            string cmd = GenerateCMDLineParas();
            try
            {
                Process convertProcess = new Process();
                ProcessStartInfo startinfo = new ProcessStartInfo(exeName, cmd);
                startinfo.UseShellExecute = false;          //不使用系统外壳程序启动 
                startinfo.RedirectStandardInput = false;    //不重定向输入 
                startinfo.RedirectStandardOutput = true;    //重定向输出，而不是默认的显示在dos控制台上 
                //startinfo.CreateNoWindow = true;            //不创建窗口                 
                convertProcess.StartInfo = startinfo;
                //convertProcess.StartInfo.UseShellExecute = false;////不使用系统外壳程序启动 
                convertProcess.Start();
                convertProcess.WaitForExit();
                return;
            }
            catch (Exception ex)
            {
                return ;// WriteLog("转换过程出错！原因：" + ex.Message);
            }            


        }

        private string GenerateCMDLineParas()
        {
            //13个参数
            string inf = @"D:\test\MOD06_L2.A2011001.0205.hdf";//-if,INPUT_FILENAME
            string gf = @"D:\test\MOD03.A2011001.0205.hdf";//-gf
            string outf = @"D:\test\20110010205";//-of
            string sds = "Cloud_Effective_Radius";//-sds
            string kk= "NN";//-kk
            string oproj = "GEO";//-oproj
            string oprm = "0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0";//-oprm,15个0
            string osp = "8";//-osp=output sphere number,8=GRS 1980/WGS 84
            string oul = "117.893129098,54.972938015";//-oul=output upper left corner 
            string olr = "153.358067142,33.321731011";//-olr=output lower right corner 
            string opsz = "0.05";//-opsz
            string off = "GEOTIFF_FMT"; //-off
            string osst = "LAT_LONG";//-osst=output spatial subset type 
            string cmdformat = "-if={0} -gf={1} -of={2} -sds=\"{3}\" -kk={4} -oproj={5}  -osp={6} -opsz={7} -off={8} -osst={9} -oul={10} -olr={11}";// ";
            return string.Format(cmdformat, inf, gf, outf, sds, kk, oproj, osp, opsz, off, osst, oul, olr);//, oprmt, oul, olr);
        }

        private void btnTwoFilesScatter_Click(object sender, EventArgs e)
        {
            IRasterDataProvider XdataProvider = null, YdataProvider = null;
            bool isNewX = false, isNewY = false;
            int[] bandNos = null;
            int[] aoi = null;// GetAOI();
            using (frmScatterTwoVarSelector frm = new frmScatterTwoVarSelector())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                //frm.Apply(GetCurrentDataProvider(), aoi);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    XdataProvider = frm.XDataProvider;
                    isNewX = frm.IsNewXDataProvider;
                    YdataProvider = frm.YDataProvider;
                    isNewY = frm.IsNewYDataProvider;
                    bandNos = new int[] { frm.XBandNo, frm.YBandNo };
                    aoi = frm.AOI;
                    if (bandNos == null || XdataProvider == null || YdataProvider == null)
                        return;
                    if (XdataProvider.Width != YdataProvider.Width || XdataProvider.Height != YdataProvider.Height)
                        return;
                    if (aoi != null)
                        return;
                    //构建虚拟的dataProvider
                    IRasterBand xband = XdataProvider.GetRasterBand(bandNos[0]);
                    IRasterBand yband = YdataProvider.GetRasterBand(bandNos[1]);
                    IRasterDataProvider localprd = new LogicalRasterDataProvider("temp", new IRasterBand[2] { xband ,yband}, null);
                    //IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                    try
                    {
                        //progress.Reset("正在准备生成散点图...", 100);
                        //progress.Start(false);
                        frmScatterGraph frm1 = new frmScatterGraph();
                        //frm1.Owner = _smartSession.SmartWindowManager.MainForm as Form;
                        frm1.StartPosition = FormStartPosition.CenterScreen;
                        LinearFitObject fitObj = new LinearFitObject();
                        frm1.Reset(localprd, 1, 2, aoi,
                                   fitObj, null
                            /*(idx, tip) => { progress.Boost(idx, "正在准备生成散点图..."); }*/
                                   );
                        //progress.Finish();
                        frm1.Show();
                        frm1.Rerender();
                        frm1.FormClosed += new FormClosedEventHandler((obj, es) =>
                        {
                            if (isNewX)
                                XdataProvider.Dispose();
                            if (isNewY)
                                YdataProvider.Dispose();
                        });
                    }
                    finally
                    {
                        //progress.Finish();
                    }

                }
            }
            //构建虚拟的dataProvider
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // 6 samples of 3D data (x, y, z)
            //double [,] values = new double [6,3]{
            //{1.0d, 2.0d, 3.0d},
            //{1.0d, 2.0d, 3.0d},
            //{1.0d, 2.0d, 3.0d},
            //{1.0d, 2.0d, 3.0d},
            //{1.0d, 2.0d, 3.0d},
            //{1.0d, 2.0d, 3.0}};
 
            // add this data to ALGLIB's format
 
            // this guy gets passed in and is filled in with an integer status code
            int info, infoT;
 
            // scalar values that describe variances along each eigenvector
            //double[] eigValues, eigValuesT;
 
            //// unit eigenvectors which are the orthogonal basis that we want
            //double[,] eigVectors, eigVectorsT;
 
            // perform the analysis
            //alglib.pcabuildbasis(values, 6, 3, out info, out eigValues, out eigVectors);
 
            //// now the vectors can be accessed as follows:
            //double basis0_x = eigVectors[0,0];
            //double basis0_y = eigVectors[1,0];
            //double basis0_z = eigVectors[2,0];
 
            //double basis1_x = eigVectors[0,1];
            //double basis1_y = eigVectors[1,1];
            //double basis1_z = eigVectors[2,1];
 
            //double basis2_x = eigVectors[0,2];
            //double basis2_y = eigVectors[1,2];
            //double basis2_z = eigVectors[2,2];

            //double[,] values = new double[10, 2]{
            //    {2.5,2.4},
            //    {0.5,0.7},
            //    {2.2,2.9},
            //    {1.9,2.2},
            //    {3.1,3},
            //    {2.3,2.7},
            //    {2,1.6},
            //    {1,1.1},
            //    {1.5,1.6},
            //    {1.1,0.9}
            //};

            //int  ntimes= values.GetLength(0);//时间序列个数
            //int npoints = values.GetLength(1);//点数
            //alglib.pcabuildbasis(values,ntimes, npoints, out info, out eigValues, out eigVectors);
            //double[,] meanvalues = new double[10, 2]{
            //    {0.69 , 0.49},
            //    {-1.31 , -1.21},
            //    {0.39 , 0.99},
            //    {0.0899999999999999 , 0.29},
            //    {1.29 , 1.09},
            //    {0.49 , 0.79},
            //    {0.19 , -0.31},
            //    {-0.81 , -0.81},
            //    {-0.31 , -0.31},
            //    {-0.71 , -1.01}  
            //};
            //string outDir = @"e:\eof";
            //CloudParaStat stat = new CloudParaStat();
            //string[] resoult = stat.AlglibEOF(meanvalues, outDir,null, true, null);

           
            //eigVectors的每一列为特征向量
            //double[,] timeEfficients = new double[ntimes, npoints];
            //for (int n = 0; n < ntimes; n++)//1~n
            //{
            //    for (int p = 0; p < npoints; p++)
            //    {
            //        for (int k = 0; k < npoints; k++)//1~m
            //        {
            //            timeEfficients[n, p] += eigVectors[k, p] * meanvalues[n, k];
            //        }
            //    }
            //}
            string fileName = @"D:\MOD06_L2.A2011002.0115.hdf";
            IRasterDataProvider lsrRaster = null;
            lsrRaster = RasterDataDriver.Open(fileName) as IRasterDataProvider;
            Dictionary<string, string> attr = lsrRaster.Attributes.GetAttributeDomain("SUBDATASETS");
            List<string> dss = new List<string>();
            int idx = 0;
            foreach (string key in attr.Keys)
                if (idx++ % 2 == 0)
                {
                    string datasetName = attr[key];
                    int groupIndex = datasetName.LastIndexOf(":") - 5;
                    if (groupIndex == -1)
                        continue;
                    else
                        if (datasetName.Substring(groupIndex).Contains("mod06"))
                            dss.Add(datasetName.Substring(groupIndex+6));                  
                }
            ConnectMySqlCloud con = new ConnectMySqlCloud();
            con.UpdateMOD06setstable(dss.ToArray());

        }
        private string GetDatasetShortName(string datasetName)
        {
            string shortDatasetName = null;
            int groupIndex = datasetName.LastIndexOf(":") - 5;
            if (groupIndex == -1)
                shortDatasetName = datasetName;
            else
                shortDatasetName = datasetName.Substring(groupIndex + 1);
            return shortDatasetName;
        }

        private void Updatetable()
        {
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates())
            //{
            //    if (frm.ShowDialog() == DialogResult.OK)
            //    {
            //    }
            //}
            //return;
            //frmUniformDataManage frm = new frmUniformDataManage();
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.Show();
            string L2LSRfname = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_ORBT_L2_LSR_MLT_NUL_20140611_0220_1000M_MS.HDF";
            string l1hdffname = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_GBAL_L1_20140611_0220_1000M_MS.HDF";
            string landcover = @"D:\Geodo\LAI\Landcover\glc2000_v1_1.img";
            //string[] fileNames = new string[] { L2LSRfname, l1hdffname,landcover};
            //string _satellite=null, _sensor=null;
            //DateTime _datatime=DateTime.MinValue;
            //bool _isFirst = true;
            //bool issame=true;
            //foreach (string fileName in fileNames)
            //{
            //    //using (IRasterDataProvider prd = GeoDataDriver.Open(fileName) as IRasterDataProvider)
            //    RasterIdentify l2datid = new RasterIdentify(Path.GetFileName(fileName));
            //    if (_isFirst)
            //    {
            //        _satellite = l2datid.Satellite;
            //        _sensor = l2datid.Sensor;
            //        _datatime = l2datid.OrbitDateTime;
            //        _isFirst = false;
            //    }
            //    else
            //    {
            //        if (l2datid.Satellite != _satellite || l2datid.Sensor != _sensor
            //            || l2datid.OrbitDateTime != _datatime)
            //            issame= false;
            //    }
            //}       
            //if (issame)
            //{
            //   //MessageBox.Show("相同！");
            //}
            DateTime dtstart = DateTime.Now;
            string _L2LSRFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_ORBT_L2_LSR_MLT_NUL_20140611_0220_1000M_MS.HDF";
            string _L1GranulesFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_GBAL_L1_20140611_0220_1000M_MS.HDF";
            string _landCoverFile = @"D:\Geodo\LAI\Landcover\glc2000_v1_1.img";


            UInt16[][] output = new UInt16[2][];
            #region
            IRasterDataProvider lsrRaster = null;
            IRasterDataProvider angleRaster = null;
            IRasterDataProvider landcoverRaster = null;
            float[] bandslope = new float[] { 1.0E-4f, 1.0E-4f, -2.2051458E38f };
            float[] bandInt = new float[] { 0.0f, 0.0f, 1.0E-4f };
            float angleslope = 0.01f;
            int bandfillValue = 65535;
            int angleFillValue = 32767;
            float latlonFillValue = 999.9f;
            int outwidth, outheight;
            try
            {
                string lsrdataset = "MERSI_LSR_1KMSDS";
                string[] openArgs = new string[] { "datasets=" + lsrdataset };
                lsrRaster = RasterDataDriver.Open(_L2LSRFile, openArgs) as IRasterDataProvider;
                if (lsrRaster == null || lsrRaster.BandCount != 7)
                    return;
                outwidth=lsrRaster.Width;
                outheight = lsrRaster.Height; 
                UInt16[] reddata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(3), 0, 0, outwidth, outheight);
                UInt16[] IRdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(4), 0, 0, outwidth, outheight);
                UInt16[] swirdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(6), 0, 0, outwidth, outheight);
                string[] locationArgs = new string[] { "datasets = SolarZenith,SolarAzimuth,SensorZenith,SensorAzimuth,Longitude,Latitude" };
                angleRaster = RasterDataDriver.Open(_L1GranulesFile, locationArgs) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount != 6)
                    return;
                landcoverRaster = GeoDataDriver.Open(_landCoverFile) as IRasterDataProvider;
                if (landcoverRaster == null || landcoverRaster.BandCount < 1)
                    return;
                //int lctWidth = landcoverRaster.Width;
                //float lctReslX = landcoverRaster.ResolutionX;
                //float lctReslY = landcoverRaster.ResolutionY;
                //double lctMinLon = landcoverRaster.CoordEnvelope.MinX;
                //double lctMaxLat = landcoverRaster.CoordEnvelope.MaxY;
                //output[0] = new ushort[reddata.Length];
                //output[1] = new ushort[reddata.Length];
                //System.Threading.Tasks.Parallel.For(0, reddata.Length, (i) => { SubProductVgtMersiLAI.CalLAISinglePixel(i, reddata, IRdata, swirdata, angleRaster, landcoverRaster,ref output); });
                int iend1 = reddata.Length / 2-1;
                int istart2 = iend1+1;
                int iend2 = reddata.Length-1;
                SubProductVgtMersiLAI.output=new ushort[2][];
                SubProductVgtMersiLAI.output[0] = new ushort[reddata.Length];
                SubProductVgtMersiLAI.output[1] = new ushort[reddata.Length];
                txtLAItime.Text += "文件读取等准备时间"+(DateTime.Now - dtstart).TotalMilliseconds.ToString()+"\r\n";
                dtstart = DateTime.Now;
                //var tasks = new Action[] { () => SubProductVgtMersiLAI.CalLAIParts(0, iend1, reddata, IRdata, swirdata, angleRaster, landcoverRaster) ,
                //() => SubProductVgtMersiLAI.CalLAIParts(istart2, iend2, reddata, IRdata, swirdata, angleRaster, landcoverRaster) };
                //System.Threading.Tasks.Parallel.Invoke(tasks);
                txtLAItime.Text += "LAI计算时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                dtstart = DateTime.Now;

                //UInt16[] lelai;
                //for (int i = 0; i < ; i++)
                //{
                //    if (reddata[i] == bandfillValue || IRdata[i] == bandfillValue || swirdata[i] == bandfillValue)
                //    {
                //        output[0][i]=32767;
                //        output[1][i] = 32767;
                //        continue;
                //    }
                //    float[] b346 = new float[] { reddata[i] * bandslope[0], IRdata[i] * bandslope[1], swirdata[i] * bandslope[2] + bandInt[2] };
                //    int y = i / angleRaster.Width;
                //    int x = i % angleRaster.Width;
                //    Int16[] sza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), x, y, 1, 1);
                //    Int16[] saa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(2), x, y, 1, 1);
                //    Int16[] vza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(3), x, y, 1, 1);
                //    Int16[] vaa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(4), x, y, 1, 1);
                //    if (sza[0] == angleFillValue || saa[0] == angleFillValue || vza[0] == angleFillValue || vaa[0] == angleFillValue)
                //    {
                //        output[0][i] = 32767;
                //        output[1][i] = 32767;
                //        continue;
                //    }
                //    float[] angles = new float[] { sza[0] * angleslope, saa[0] * angleslope, vza[0] * angleslope, vaa[0] * angleslope };
                //    float[] lonM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(5), x, y, 1, 1);
                //    float[] latM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(6), x, y, 1, 1);
                //    if (latM[0] == latlonFillValue || lonM[0] == latlonFillValue)
                //    {
                //        output[0][i] = 32767;
                //        output[1][i] = 32767;
                //        continue;
                //    }
                //    float lat = latM[0], lon = lonM[0];
                //    //根据像元的经纬度去LandCoverType文件中找相应的偏移量xoffset和yoffset，offset =xoffset+yoffset*landcoverRaster.Width;
                //    int xoffset = (int)Math.Floor((lon - lctMinLon) / lctReslX);
                //    int yoffset = (int)Math.Floor((lctMaxLat - lat) / lctReslY);
                //    if (xoffset < 0 || yoffset < 0)
                //    {
                //        output[0][i] = 32767;
                //        output[1][i] = 32767;
                //        continue;
                //    }
                //    byte[] lct = SubProductVgtMersiLAI.GetDataValue<byte>(landcoverRaster.GetRasterBand(1), xoffset, yoffset, 1, 1);
                //    //int offset = xoffset + yoffset * lctWidth;
                //    SubProductVgtMersiLAI.CalLAI(b346, angles, lct[0], out lelai);
                //    output[0][i] = lelai[0];
                //    output[1][i] = lelai[1];
                //}
            #endregion
                //#region 建立输出文件
                //IRasterDataProvider mainRaster = null;
                //mainRaster = new ArrayRasterDataProvider<UInt16>("Array", output, outwidth, outheight);
                //RasterIdentify datid = new RasterIdentify(Path.GetFileName(_L2LSRFile));
                //datid.ProductIdentify = "VGT";
                //datid.SubProductIdentify = "0LAI";
                //float outResolution = 0.01f;
                //if (datid.Resolution == "1000M")
                //{
                //    outResolution = 0.01f;
                //}
                //string _outpath = Path.GetDirectoryName(_L2LSRFile);
                //string laiFileName = Path.Combine(_outpath, datid.ToWksFileName(".ldf"));

                //HDF4FilePrjSettings setting = new HDF4FilePrjSettings();
                //setting.LocationFile = angleRaster;
                //setting.OutFormat = "LDF";
                //setting.OutResolutionX = setting.OutResolutionY = outResolution;
                //Dictionary<string, double> exargs = new Dictionary<string, double>();
                //if (true)//double.TryParse(GetDefaultNullValue(mainRaster.DataType), out invalidValue))
                //{
                //    exargs.Add("FillValue", 32767);
                //    setting.ExtArgs = new object[] { exargs };
                //}
                ////CoordEnvelope envelope;
                ////HDF5Filter.GetDataCoordEnvelope(_L2LSRFile, out envelope);

                //HDF4FileProjector projector = new HDF4FileProjector();
                //GeoDo.RasterProject.PrjEnvelope mainPrj = null;//new GeoDo.RasterProject.PrjEnvelope(envelope.MinX,envelope.MaxX,envelope.MinY,envelope.MaxY);
                //projector.ComputeDstEnvelope(angleRaster, GeoDo.Project.SpatialReference.GetDefault(), out mainPrj, null);
                //if (mainPrj != null)
                //{
                //    setting.OutEnvelope = mainPrj;
                //    setting.OutPathAndFileName = laiFileName;
                //    projector.Project(mainRaster, setting, GeoDo.Project.SpatialReference.GetDefault(), null);
                //    
                //}
                //else
                //{
                //    return;
                //}
                //#endregion
                //SubProductVgtMersiLAI.OutputLAI(angleRaster,_L2LSRFile,Path.GetDirectoryName(_L2LSRFile));
                txtLAItime.Text += "投影输出时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
            }
            catch (System.Exception ex)
            {
                return ;
            }
            finally
            {
                if (lsrRaster != null)
                    lsrRaster.Dispose();
                if (angleRaster != null)
                    angleRaster.Dispose();
                if (landcoverRaster != null)
                    landcoverRaster.Dispose();
            }
        }

        private void FlowProcess()
        {
            DateTime dtstart = DateTime.Now;
            string _L2LSRFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_ORBT_L2_LSR_MLT_NUL_20140611_0220_1000M_MS.HDF";
            string _L1GranulesFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_GBAL_L1_20140611_0220_1000M_MS.HDF";
            string _landCoverFile = @"D:\Geodo\LAI\Landcover\glc2000_v1_1.img";
            UInt16[][] output = new UInt16[2][];
            #region
            IRasterDataProvider lsrRaster = null;
            IRasterDataProvider angleRaster = null;
            IRasterDataProvider landcoverRaster = null;
            float[] bandslope = new float[] { 1.0E-4f, 1.0E-4f, -2.2051458E38f };
            float[] bandInt = new float[] { 0.0f, 0.0f, 1.0E-4f };
            float angleslope = 0.01f;
            int bandfillValue = 65535;
            int angleFillValue = 32767;
            float latlonFillValue = 999.9f;
            int outwidth, outheight;
            try
            {
                string lsrdataset = "MERSI_LSR_1KMSDS";
                string[] openArgs = new string[] { "datasets=" + lsrdataset };
                lsrRaster = RasterDataDriver.Open(_L2LSRFile, openArgs) as IRasterDataProvider;
                if (lsrRaster == null || lsrRaster.BandCount != 7)
                    return;
                outwidth = lsrRaster.Width;
                outheight = lsrRaster.Height;
                UInt16[] reddata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(3), 0, 0, outwidth, outheight);
                UInt16[] IRdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(4), 0, 0, outwidth, outheight);
                UInt16[] swirdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(6), 0, 0, outwidth, outheight);
                string[] locationArgs = new string[] { "datasets = SolarZenith,SolarAzimuth,SensorZenith,SensorAzimuth,Longitude,Latitude" };
                angleRaster = RasterDataDriver.Open(_L1GranulesFile, locationArgs) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount != 6)
                    return;
                landcoverRaster = GeoDataDriver.Open(_landCoverFile) as IRasterDataProvider;
                if (landcoverRaster == null || landcoverRaster.BandCount < 1)
                    return;
                int lctWidth = landcoverRaster.Width;
                float lctReslX = landcoverRaster.ResolutionX;
                float lctReslY = landcoverRaster.ResolutionY;
                double lctMinLon = landcoverRaster.CoordEnvelope.MinX;
                double lctMaxLat = landcoverRaster.CoordEnvelope.MaxY;
                output[0] = new ushort[reddata.Length];
                output[1] = new ushort[reddata.Length];
                txtLAItime.Text += "文件读取等准备时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                dtstart = DateTime.Now;
                UInt16[] lelai=null;
                int tt=0;
                for (int i = 0; i < reddata.Length; i++)
                {
                    if (reddata[i] == bandfillValue || IRdata[i] == bandfillValue || swirdata[i] == bandfillValue)
                    {
                        output[0][i]=32767;
                        output[1][i] = 32767;
                        continue;
                    }
                    float[] b346 = new float[] { reddata[i] * bandslope[0], IRdata[i] * bandslope[1], swirdata[i] * bandslope[2] + bandInt[2] };
                    int y = i / angleRaster.Width;
                    int x = i % angleRaster.Width;
                    Int16[] sza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), x, y, 1, 1);
                    Int16[] saa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(2), x, y, 1, 1);
                    Int16[] vza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(3), x, y, 1, 1);
                    Int16[] vaa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(4), x, y, 1, 1);
                    if (sza[0] == angleFillValue || saa[0] == angleFillValue || vza[0] == angleFillValue || vaa[0] == angleFillValue)
                    {
                        output[0][i] = 32767;
                        output[1][i] = 32767;
                        continue;
                    }
                    float[] angles = new float[] { sza[0] * angleslope, saa[0] * angleslope, vza[0] * angleslope, vaa[0] * angleslope };
                    float[] lonM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(5), x, y, 1, 1);
                    float[] latM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(6), x, y, 1, 1);
                    if (latM[0] == latlonFillValue || lonM[0] == latlonFillValue)
                    {
                        output[0][i] = 32767;
                        output[1][i] = 32767;
                        continue;
                    }
                    float lat = latM[0], lon = lonM[0];
                    //根据像元的经纬度去LandCoverType文件中找相应的偏移量xoffset和yoffset，offset =xoffset+yoffset*landcoverRaster.Width;
                    int xoffset = (int)Math.Floor((lon - lctMinLon) / lctReslX);
                    int yoffset = (int)Math.Floor((lctMaxLat - lat) / lctReslY);
                    if (xoffset < 0 || yoffset < 0)
                    {
                        output[0][i] = 32767;
                        output[1][i] = 32767;
                        continue;
                    }
                    byte[] lct = SubProductVgtMersiLAI.GetDataValue<byte>(landcoverRaster.GetRasterBand(1), xoffset, yoffset, 1, 1);
                    //int offset = xoffset + yoffset * lctWidth;
                    if (tt==0)
                    {
                    txtLAItime.Text += "LAI单次计算开始" + DateTime.Now.ToString() + "\r\n";
                    dtstart = DateTime.Now;
                    }
                    //lelai=SubProductVgtMersiLAI.CalLAI(b346, angles, lct[0]);
                    if (tt==0)
                    {
                    txtLAItime.Text += "LAI单词计算时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                        tt++;
                    }

                    output[0][i] = lelai[0];
                    output[1][i] = lelai[1];
                }

                txtLAItime.Text += "LAI计算总时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                dtstart = DateTime.Now;
            #endregion
                #region 建立输出文件
                IRasterDataProvider mainRaster = null;
                mainRaster = new ArrayRasterDataProvider<UInt16>("Array", output, outwidth, outheight);
                RasterIdentify datid = new RasterIdentify(Path.GetFileName(_L2LSRFile));
                datid.ProductIdentify = "VGT";
                datid.SubProductIdentify = "0LAI";
                float outResolution = 0.01f;
                if (datid.Resolution == "1000M")
                {
                    outResolution = 0.01f;
                }
                string _outpath = Path.GetDirectoryName(_L2LSRFile);
                string laiFileName = Path.Combine(_outpath, datid.ToWksFileName(".ldf"));

                HDF4FilePrjSettings setting = new HDF4FilePrjSettings();
                setting.LocationFile = angleRaster;
                setting.OutFormat = "LDF";
                setting.OutResolutionX = setting.OutResolutionY = outResolution;
                Dictionary<string, double> exargs = new Dictionary<string, double>();
                if (true)//double.TryParse(GetDefaultNullValue(mainRaster.DataType), out invalidValue))
                {
                    exargs.Add("FillValue", 32767);
                    setting.ExtArgs = new object[] { exargs };
                }
                //CoordEnvelope envelope;
                //HDF5Filter.GetDataCoordEnvelope(_L2LSRFile, out envelope);

                HDF4FileProjector projector = new HDF4FileProjector();
                GeoDo.RasterProject.PrjEnvelope mainPrj = null;//new GeoDo.RasterProject.PrjEnvelope(envelope.MinX,envelope.MaxX,envelope.MinY,envelope.MaxY);
                projector.ComputeDstEnvelope(angleRaster, GeoDo.Project.SpatialReference.GetDefault(), out mainPrj, null);
                if (mainPrj != null)
                {
                    setting.OutEnvelope = mainPrj;
                    setting.OutPathAndFileName = laiFileName;
                    projector.Project(mainRaster, setting, GeoDo.Project.SpatialReference.GetDefault(), null);
                    
                }
                else
                {
                    return;
                }
                #endregion
                //SubProductVgtMersiLAI.OutputLAI(angleRaster, _L2LSRFile, Path.GetDirectoryName(_L2LSRFile));
                txtLAItime.Text += "投影输出时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
            }
            catch (System.Exception ex)
            {
                return;
            }
            finally
            {
                if (lsrRaster != null)
                    lsrRaster.Dispose();
                if (angleRaster != null)
                    angleRaster.Dispose();
                if (landcoverRaster != null)
                    landcoverRaster.Dispose();
            }
        }

        private void FlowProcessBand()
        {
            DateTime dtstart = DateTime.Now;
            string _L2LSRFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_ORBT_L2_LSR_MLT_NUL_20140611_0220_1000M_MS.HDF";
            string _L1GranulesFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_GBAL_L1_20140611_0220_1000M_MS.HDF";
            string _landCoverFile = @"D:\Geodo\LAI\Landcover\glc2000_v1_1.img";
            UInt16[][] output = new UInt16[2][];
            #region
            IRasterDataProvider lsrRaster = null;
            IRasterDataProvider angleRaster = null;
            IRasterDataProvider landcoverRaster = null;
            float[] bandslope = new float[] { 1.0E-4f, 1.0E-4f, -2.2051458E38f };
            float[] bandInt = new float[] { 0.0f, 0.0f, 1.0E-4f };
            float angleslope = 0.01f;
            int bandfillValue = 65535;
            int angleFillValue = 32767;
            float latlonFillValue = 999.9f;
            int outwidth, outheight;
            try
            {
                string lsrdataset = "MERSI_LSR_1KMSDS";
                string[] openArgs = new string[] { "datasets=" + lsrdataset };
                lsrRaster = RasterDataDriver.Open(_L2LSRFile, openArgs) as IRasterDataProvider;
                if (lsrRaster == null || lsrRaster.BandCount != 7)
                    return;
                outwidth = lsrRaster.Width;
                outheight = lsrRaster.Height;
                UInt16[] reddata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(3), 0, 0, outwidth, outheight);
                UInt16[] IRdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(4), 0, 0, outwidth, outheight);
                UInt16[] swirdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(6), 0, 0, outwidth, outheight);
                string[] locationArgs = new string[] { "datasets = SolarZenith,SolarAzimuth,SensorZenith,SensorAzimuth,Longitude,Latitude" };
                angleRaster = RasterDataDriver.Open(_L1GranulesFile, locationArgs) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount != 6)
                    return;
                Int16[] sza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                Int16[] saa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(2), 0, 0, outwidth, outheight);
                Int16[] vza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(3), 0, 0, outwidth, outheight);
                Int16[] vaa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(4), 0, 0, outwidth, outheight);
                float[] lonM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(5), 0, 0, outwidth, outheight);
                float[] latM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(6), 0, 0, outwidth, outheight);
                landcoverRaster = GeoDataDriver.Open(_landCoverFile) as IRasterDataProvider;
                if (landcoverRaster == null || landcoverRaster.BandCount < 1)
                    return;
                int lctWidth = landcoverRaster.Width;
                float lctReslX = landcoverRaster.ResolutionX;
                float lctReslY = landcoverRaster.ResolutionY;
                double lctMinLon = landcoverRaster.CoordEnvelope.MinX;
                double lctMaxLat = landcoverRaster.CoordEnvelope.MaxY;
                output[0] = new ushort[reddata.Length];
                output[1] = new ushort[reddata.Length];
                txtLAItime.Text += "文件读取等准备时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                dtstart = DateTime.Now;
                UInt16[] lelai=null;
                int tt = 0;
                for (int i = 0; i < reddata.Length; i++)
                {
                    if (reddata[i] == bandfillValue || IRdata[i] == bandfillValue || swirdata[i] == bandfillValue)
                    {
                        output[0][i] = 32767;
                        output[1][i] = 32767;
                        continue;
                    }
                    float[] b346 = new float[] { reddata[i] * bandslope[0], IRdata[i] * bandslope[1], swirdata[i] * bandslope[2] + bandInt[2] };
                    int y = i / angleRaster.Width;
                    int x = i % angleRaster.Width;
                    //Int16[] sza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), x, y, 1, 1);
                    //Int16[] saa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(2), x, y, 1, 1);
                    //Int16[] vza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(3), x, y, 1, 1);
                    //Int16[] vaa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(4), x, y, 1, 1);
                    if (sza[i] == angleFillValue || saa[i] == angleFillValue || vza[i] == angleFillValue || vaa[i] == angleFillValue)
                    {
                        output[0][i] = 32767;
                        output[1][i] = 32767;
                        continue;
                    }
                    float[] angles = new float[] { sza[i] * angleslope, saa[i] * angleslope, vza[i] * angleslope, vaa[i] * angleslope };
                    //float[] lonM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(5), 0, 0, outwidth, outheight);
                    //float[] latM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(6), 0, 0, outwidth, outheight);
                    if (latM[i] == latlonFillValue || lonM[i] == latlonFillValue)
                    {
                        output[0][i] = 32767;
                        output[1][i] = 32767;
                        continue;
                    }
                    float lat = latM[i], lon = lonM[i];
                    //根据像元的经纬度去LandCoverType文件中找相应的偏移量xoffset和yoffset，offset =xoffset+yoffset*landcoverRaster.Width;
                    int xoffset = (int)Math.Floor((lon - lctMinLon) / lctReslX);
                    int yoffset = (int)Math.Floor((lctMaxLat - lat) / lctReslY);
                    if (xoffset < 0 || yoffset < 0)
                    {
                        output[0][i] = 32767;
                        output[1][i] = 32767;
                        continue;
                    }
                    byte[] lct = SubProductVgtMersiLAI.GetDataValue<byte>(landcoverRaster.GetRasterBand(1), xoffset, yoffset, 1, 1);
                    //int offset = xoffset + yoffset * lctWidth;
                    if (tt == 0)
                    {
                        txtLAItime.Text += "LAI单次计算开始" + DateTime.Now.ToString() + "\r\n";
                        dtstart = DateTime.Now;
                    }
                    //lelai = SubProductVgtMersiLAI.CalLAI(b346, angles, lct[0]);
                    if (tt == 0)
                    {
                        txtLAItime.Text += "LAI单词计算时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                        tt++;
                    }
                    output[0][i] = lelai[0];
                    output[1][i] = lelai[1];
                }
                txtLAItime.Text += "LAI计算总时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                dtstart = DateTime.Now;
            #endregion
                #region 建立输出文件
                IRasterDataProvider mainRaster = null;
                mainRaster = new ArrayRasterDataProvider<UInt16>("Array", output, outwidth, outheight);
                RasterIdentify datid = new RasterIdentify(Path.GetFileName(_L2LSRFile));
                datid.ProductIdentify = "VGT";
                datid.SubProductIdentify = "0LAI";
                float outResolution = 0.01f;
                if (datid.Resolution == "1000M")
                {
                    outResolution = 0.01f;
                }
                string _outpath = Path.GetDirectoryName(_L2LSRFile);
                string laiFileName = Path.Combine(_outpath, datid.ToWksFileName(".ldf"));
                HDF4FilePrjSettings setting = new HDF4FilePrjSettings();
                setting.LocationFile = angleRaster;
                setting.OutFormat = "LDF";
                setting.OutResolutionX = setting.OutResolutionY = outResolution;
                Dictionary<string, double> exargs = new Dictionary<string, double>();
                if (true)//double.TryParse(GetDefaultNullValue(mainRaster.DataType), out invalidValue))
                {
                    exargs.Add("FillValue", 32767);
                    setting.ExtArgs = new object[] { exargs };
                }
                //CoordEnvelope envelope;
                //HDF5Filter.GetDataCoordEnvelope(_L2LSRFile, out envelope);

                HDF4FileProjector projector = new HDF4FileProjector();
                GeoDo.RasterProject.PrjEnvelope mainPrj = null;//new GeoDo.RasterProject.PrjEnvelope(envelope.MinX,envelope.MaxX,envelope.MinY,envelope.MaxY);
                projector.ComputeDstEnvelope(angleRaster, GeoDo.Project.SpatialReference.GetDefault(), out mainPrj, null);
                if (mainPrj != null)
                {
                    setting.OutEnvelope = mainPrj;
                    setting.OutPathAndFileName = laiFileName;
                    projector.Project(mainRaster, setting, GeoDo.Project.SpatialReference.GetDefault(), null);
                }
                else
                {
                    return;
                }
                #endregion
                //SubProductVgtMersiLAI.OutputLAI(angleRaster, _L2LSRFile, Path.GetDirectoryName(_L2LSRFile));
                txtLAItime.Text += "投影输出时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
            }
            catch (System.Exception ex)
            {
                return;
            }
            finally
            {
                if (lsrRaster != null)
                    lsrRaster.Dispose();
                if (angleRaster != null)
                    angleRaster.Dispose();
                if (landcoverRaster != null)
                    landcoverRaster.Dispose();
            }
        }

        private void PartsParallProcess()
        {
            DateTime dtstart = DateTime.Now;
            string _L2LSRFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_ORBT_L2_LSR_MLT_NUL_20140611_0220_1000M_MS.HDF";
            string _L1GranulesFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_GBAL_L1_20140611_0220_1000M_MS.HDF";
            string _landCoverFile = @"D:\Geodo\LAI\Landcover\glc2000_v1_1.img";
            #region
            IRasterDataProvider lsrRaster = null;
            IRasterDataProvider angleRaster = null;
            IRasterDataProvider landcoverRaster = null;
            int outwidth, outheight;
            try
            {
                string lsrdataset = "MERSI_LSR_1KMSDS";
                string[] openArgs = new string[] { "datasets=" + lsrdataset };
                lsrRaster = RasterDataDriver.Open(_L2LSRFile, openArgs) as IRasterDataProvider;
                if (lsrRaster == null || lsrRaster.BandCount != 7)
                    return;
                outwidth = lsrRaster.Width;
                outheight = lsrRaster.Height;
                UInt16[] reddata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(3), 0, 0, outwidth, outheight);
                UInt16[] IRdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(4), 0, 0, outwidth, outheight);
                UInt16[] swirdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(6), 0, 0, outwidth, outheight);
                string[] locationArgs = new string[] { "datasets = SolarZenith,SolarAzimuth,SensorZenith,SensorAzimuth,Longitude,Latitude" };
                angleRaster = RasterDataDriver.Open(_L1GranulesFile, locationArgs) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount != 6)
                    return;
                Int16[] sza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                Int16[] saa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(2), 0, 0, outwidth, outheight);
                Int16[] vza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(3), 0, 0, outwidth, outheight);
                Int16[] vaa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(4), 0, 0, outwidth, outheight);
                float[] lonM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(5), 0, 0, outwidth, outheight);
                float[] latM = SubProductVgtMersiLAI.GetDataValue<float>(angleRaster.GetRasterBand(6), 0, 0, outwidth, outheight);
                landcoverRaster = GeoDataDriver.Open(_landCoverFile) as IRasterDataProvider;
                if (landcoverRaster == null || landcoverRaster.BandCount < 1)
                    return;
                //int lctWidth = landcoverRaster.Width;
                //float lctReslX = landcoverRaster.ResolutionX;
                //float lctReslY = landcoverRaster.ResolutionY;
                //double lctMinLon = landcoverRaster.CoordEnvelope.MinX;
                //double lctMaxLat = landcoverRaster.CoordEnvelope.MaxY;
                //output[0] = new ushort[reddata.Length];
                //output[1] = new ushort[reddata.Length];
                //System.Threading.Tasks.Parallel.For(0, reddata.Length, (i) => { SubProductVgtMersiLAI.CalLAISinglePixel(i, reddata, IRdata, swirdata, angleRaster, landcoverRaster,ref output); });
                                SubProductVgtMersiLAI.output = new ushort[2][];
                SubProductVgtMersiLAI.output[0] = new ushort[reddata.Length];
                SubProductVgtMersiLAI.output[1] = new ushort[reddata.Length];
                txtLAItime.Text += "文件读取等准备时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                dtstart = DateTime.Now;
                int parts = 7;
                int step = reddata.Length / parts;
                int[] istart = new int[parts];
                int[] iend = new int[parts];
                istart[0] = 0;
                iend[0] = 0 + step;
                for (int p = 1; p < parts; p++)
                {
                    istart[p] = istart[p - 1] + step;
                    iend[p] = iend[p - 1] + step;
                }
                iend[parts - 1] = reddata.Length;
                var tasks =new Action[] { 
                    () => SubProductVgtMersiLAI.CalLAIParts(istart[0], iend[0], outwidth,reddata, IRdata, swirdata,  landcoverRaster,sza,saa,vza,vza,latM,lonM) ,
                () => SubProductVgtMersiLAI.CalLAIParts(istart[1], iend[1],  outwidth,reddata, IRdata, swirdata,landcoverRaster,sza,saa,vza,vza,latM,lonM),
            () => SubProductVgtMersiLAI.CalLAIParts(istart[2], iend[2],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            () => SubProductVgtMersiLAI.CalLAIParts(istart[3], iend[3],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            () => SubProductVgtMersiLAI.CalLAIParts(istart[4], iend[4],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            () => SubProductVgtMersiLAI.CalLAIParts(istart[5], iend[5],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            () => SubProductVgtMersiLAI.CalLAIParts(istart[6], iend[6],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[7], iend[7],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[8], iend[8],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[9], iend[9],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //        () => SubProductVgtMersiLAI.CalLAIParts(istart[10], iend[10], outwidth,reddata, IRdata, swirdata,  landcoverRaster,sza,saa,vza,vza,latM,lonM) ,
            //    () => SubProductVgtMersiLAI.CalLAIParts(istart[11], iend[11],  outwidth,reddata, IRdata, swirdata,landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[12], iend[12],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[13], iend[13],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[14], iend[14],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[15], iend[15],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[16], iend[16],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[17], iend[17],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[18], iend[18],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
            //() => SubProductVgtMersiLAI.CalLAIParts(istart[19], iend[19],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM)
            };
                System.Threading.Tasks.Parallel.Invoke(tasks);
                txtLAItime.Text += "LAI计算时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                dtstart = DateTime.Now;
            #endregion
                #region 建立输出文件
                //SubProductVgtMersiLAI.OutputLAI(angleRaster, _L2LSRFile, Path.GetDirectoryName(_L2LSRFile));
                //txtLAItime.Text += "投影输出时间" + (DateTime.Now - dtstart).TotalMilliseconds.ToString() + "\r\n";
                #endregion
            }
            catch (System.Exception ex)
            {
                return;
            }
            finally
            {
                if (lsrRaster != null)
                    lsrRaster.Dispose();
                if (angleRaster != null)
                    angleRaster.Dispose();
                if (landcoverRaster != null)
                    landcoverRaster.Dispose();
            }
        }

        static float[] bandslope = new float[3] { 1, 1, 1 };
        static float[] bandInt = new float[3] { 0.0f, 0.0f, 0 };
        private string[] test()
        {
            //string L2LSRFile = @"D:\Geodo\LAI\FY-3A_MERSI\FY3A_MERSI_ORBT_L2_LSR_MLT_NUL_20140611_0220_1000M_MS.HDF";
            string L2LSRFile = @"D:\Geodo\LAI\FY3C\FY3C_MERSI_ORBT_L2_LSR_MLT_NUL_20140615_0210_1000M_MS.HDF";
            string L1GranulesFile = @"D:\Geodo\LAI\FY3C\FY3C_MERSI_GBAL_L1_20140615_0210_GEO1K_MS.HDF";
            //Hdf5Operator hdficgeo = new Hdf5Operator(l1geo);
            //string[] HDFdatasetsgeo = hdficgeo.GetDatasetNames;
            RasterIdentify datid = new RasterIdentify(Path.GetFileName(L2LSRFile));
            if (!ParseLAIConfigXml.ParseXML(datid.Satellite, datid.Sensor,true))
                return null;
            string lsrdataset = ParseLAIConfigXml.DataSet;
            int rno = ParseLAIConfigXml.RedNO;
            int irno = ParseLAIConfigXml.IRNO;
            int swirno = ParseLAIConfigXml.SWIRNO;
            string geodatastes = ParseLAIConfigXml.GeoDataSets;

            if (!ParseLAIConfigXml.ParseXML(datid.Satellite, datid.Sensor, false))
                return null;
            rno = ParseLAIConfigXml.RedNO;
            irno = ParseLAIConfigXml.IRNO;
            swirno = ParseLAIConfigXml.SWIRNO;
            int szano = ParseLAIConfigXml.SZANO;
            int saano = ParseLAIConfigXml.SAANO;
            int vzano = ParseLAIConfigXml.VZANO;
            int vaano = ParseLAIConfigXml.VAANO;

            Hdf5Operator hdfic = new Hdf5Operator(L2LSRFile);
            string[] HDFdatasets = hdfic.GetDatasetNames;
            bool matched = false;
            for (int i = 0; i < HDFdatasets.Length; i++)
            {
                if (HDFdatasets[i].ToUpper() == lsrdataset.ToUpper())
                {
                    matched = true;
                    break;
                }
            }
            if (!matched)
                return null;
            Dictionary<string, string> dsAttrs = hdfic.GetAttributes(lsrdataset);
            if (!dsAttrs.ContainsKey("band_name"))
                return null;
            string[] bandNO = dsAttrs["band_name"].Split(',');
            if (!bandNO.Contains(rno.ToString()) || !bandNO.Contains(irno.ToString()) || !bandNO.Contains(swirno.ToString()))
                return null;
            int ridx = 0, iridx = 0, swiridx = 0;
            for (int id = 0; id < bandNO.Length; id++)
            {
                if (bandNO[id] == rno.ToString())
                {
                    ridx = id + 1;
                }
                else if (bandNO[id] == irno.ToString())
                {
                    iridx = id + 1;
                }
                else if (bandNO[id] == swirno.ToString())
                {
                    swiridx = id + 1;
                }
            }
            if (ridx == 0 || iridx == 0 || swiridx == 0)
                return null;
            if (dsAttrs.ContainsKey("Intercept"))
            {
                string[] bandIntstr = dsAttrs["Intercept"].Split(',');
                if (bandIntstr.Length == 1)
                {
                    float bandintval = float.Parse(bandIntstr[0]);
                    bandInt = new float[] { bandintval, bandintval, bandintval };
                }
                else
                {
                    bandInt[0] = float.Parse(bandIntstr[ridx - 1]);
                    bandInt[1] = float.Parse(bandIntstr[iridx - 1]);
                    bandInt[2] = float.Parse(bandIntstr[swiridx - 1]);
                }
            }
            if (dsAttrs.ContainsKey("Slope"))
            {
                string[] bandSlope = dsAttrs["Slope"].Split(',');
                if (bandSlope.Length == 1)
                {
                    float bandslpval = float.Parse(bandSlope[0]);
                    bandslope = new float[] { bandslpval, bandslpval, bandslpval };
                }
                else
                {
                    bandslope[0] = float.Parse(bandSlope[ridx - 1]);
                    bandslope[1] = float.Parse(bandSlope[iridx - 1]);
                    bandslope[2] = float.Parse(bandSlope[swiridx - 1]);
                }
            }
            Hdf5Operator hdficgeo = new Hdf5Operator(L1GranulesFile);
            string[] HDFdatasetsgeo = hdficgeo.GetDatasetNames;
            int matchedcount = 0;
            string[] geosets = geodatastes.Split(',');
            for (int i = 0; i < HDFdatasets.Length; i++)
            {
                foreach (string set in geosets)
                {
                    if (HDFdatasets[i].ToUpper() == lsrdataset.ToUpper())
                    {
                        matchedcount++;
                        break;
                    }
                }
                if (matchedcount == 6)
                    break;
            }

            string[] locationArgs = new string[] { "datasets = " + geodatastes };//SolarZenith,SolarAzimuth,SensorZenith,SensorAzimuth,Longitude,Latitude
            IRasterDataProvider angleRaster = RasterDataDriver.Open(L1GranulesFile, locationArgs) as IRasterDataProvider;
            if (angleRaster == null || angleRaster.BandCount != 6)
                return null;

            return null;
        }
        private static string _isccprootpath = null, _airsrootpath = null, _cloudsatrootpath = null;
        private static Dictionary<int, string> _modisrootpath = null;
        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                using (frmStatSubRegionTemplatesMWS frm = new frmStatSubRegionTemplatesMWS())
                {
                    frm.listView1.MultiSelect = true;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {

                    }
                }
                using (frmStatSXRegionTemplates frm = new frmStatSXRegionTemplates())
                {
                    frm.listView1.MultiSelect = true;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {

                    }
                }
                using (frmStatProvinceRegionTemplates frm = new frmStatProvinceRegionTemplates())
                {
                    frm.listView1.MultiSelect = true;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {

                    }
                }
                //string xml = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\PreProcessRootPathsArgs.xml";
                //DataBaseArg arg = new DataBaseArg();
                //if (arg.ParseRootPathXml(xml))
                //{
                //    _isccprootpath = arg.ISCCPRootPath;
                //    _airsrootpath = arg.AIRSRootPath;
                //    _cloudsatrootpath = arg.CloudSATRootPath;
                //    _modisrootpath = arg.MODISRootPath;
                //}
                //if (_isccprootpath != null || _airsrootpath != null || _cloudsatrootpath != null || _modisrootpath != null)
                //    arg.RootPathToXml(xml);


            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //List<string> _allMosaicPaths=new List<string>();
            ////_allMosaicPaths.Add(@"D:\test\日拼接产品\CloudPhaseOpticalProperties\AQUA\MODIS\2011\2\day\0.01");
            //_allMosaicPaths.Add(@"D:\日拼接产品\CloudTopTemperature\AQUA\MODIS\2011\1\day\0.05");
            //string _outputDir = @"D:\";
            //#region 将每个数据集的每个月的day/night日拼接数据进行有效率统计
            //string relaDir = null;
            //ConnectMySqlCloud con =new ConnectMySqlCloud();
            //List<ConnectMySqlCloud.DayMergeLine> lineValues =null;
            //string logformat = "MosaicLog.{0}.txt";
            //string log;
            //foreach (string daydir in _allMosaicPaths)
            //{
            //    relaDir = daydir.Replace(_outputDir, "") ;
            //    lineValues = con.QureyDayMergeMonthly(relaDir);
            //    if (lineValues.Count<1)
            //        continue;
            //    log = string.Format(logformat, relaDir.Replace('\\', '_'));
            //    string logname =Path.Combine(daydir + "\\", log);
            //    if (File.Exists(logname))
            //    {
            //        File.Delete(logname);
            //    }
            //    string lineformat = "\t{0}\t{1}\t{2}\t{3}\t{4}";
            //    using (StreamWriter sw = new StreamWriter(logname, true, Encoding.Default))
            //    {
            //        sw.WriteLine("日期\t\t有效率(%)\t\t5分钟段个数\t\t日产品文件\t\t5分钟段时分");
            //        for (int i = 0; i < lineValues.Count; i++)
            //        {
            //            sw.WriteLine(string.Format(lineformat, String.Format("{0, -12}", lineValues[i].Datatime.ToShortDateString()), String.Format("{0, 3}",lineValues[i].ValidPct), String.Format("{0,5}",lineValues[i].GranuleCounts), String.Format("{0, -60}",lineValues[i].ImageName), lineValues[i].GranuleTimes));
            //        }
            //    }
            //}
            //#endregion
            
            //string[] set = test();
            //int[] resl = ParseLAIConfigXml.ParseXML("FY3A","MERSI",out set);
            //string fname = @"D:\Geodo\试验数据\SMART产品数据\SIC\FY3A_VIRRX_GBAL_L2_SIC_MLT_PSG_20130721_POAD_1000M_MS.HDF";
            //if (!IsCompatible(fname,null,null))
            //{
            //    return;
            //}
            //IRasterDataProvider lsrRaster = GeoDataDriver.Open(fname) as IRasterDataProvider;
            //int bc = lsrRaster.BandCount;
            //FlowProcess();
            //FlowProcessBand();
            //PartsParallProcess();
        //    string regionNames = "测试";
        //    GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
        //    string fieldName;
        //    string shapeFilename;
        //    int fieldIndex = -1;
        //    List<string> fieldValues = new List<string>();
        //    string regionsname = "";
        //    bool useRegionProj = false;//是否用解决方案命名了自定义矢量AOI区域
        //    using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates())
        //    {
        //        frm.listView1.MultiSelect = true;
        //        if (frm.ShowDialog() == DialogResult.OK)
        //        {
        //            if (useRegionProj)
        //                regionsname = regionNames.Trim();
        //             Feature[]  fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
        //            if (fets != null)
        //            {
        //                string fetname;
        //                foreach (Feature fet in fets)
        //                {
        //                    fetname = fet.GetFieldValue(fieldIndex);
        //                    fieldValues.Add(fetname); //获得选择区域名称
        //                    aoiContainer.AddAOI(fet);
        //                    if (!useRegionProj)
        //                        regionsname += fetname;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            aoiContainer = null;
        //            regionsname = "全国";
        //        }
        //    }

        }

        protected  bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            if (header1024 == null)
            {
                header1024 = new byte[1024];
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(header1024, 0, 1024);
                    fs.Close();
                }
            }
            if (!HDF5Helper.IsHdf5(header1024))
                return false;
            string geoprjstr = "Projection Type".ToUpper();
            string geoprj1 = "Geographic Longitude/Latitude".ToUpper();
            string geoprj2 = "Geographic Longitude/Latitute".ToUpper();
            string prjType = null;
            Hdf5Operator hdfic = new Hdf5Operator(fileName);
            Dictionary<string, string> fileAttributes = hdfic.GetAttributes();
            foreach (KeyValuePair<string, string> fileAttribute in fileAttributes)
            {
                if (fileAttribute.Key.ToUpper() == geoprjstr)
                {
                    prjType = fileAttribute.Value.ToUpper();
                    break;
                }
            }
            if (prjType == null)
            {
                return false;
            }
            if (prjType != null && (prjType != geoprj1 && prjType != geoprj2))
            {
                return false;
            }
            string[] HDFdatasets = hdfic.GetDatasetNames;
            bool matched = false;
            //foreach (String[] sets in FY3HDF5GEOProvider._datasets)
            //{
            //    foreach (string set in sets)
            //    {
            //        matched = false;
            //        for (int i = 0; i < HDFdatasets.Length; i++)
            //        {
            //            if (HDFdatasets[i] == set)
            //            {
            //                matched = true;
            //            }
            //        }
            //        if (!matched)
            //        {
            //            break;
            //        }
            //    }
            //    if (matched)
            //        return true;
            //}
            //return false;
            return true;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            string awxfile = @"D:\Geodo\陈超\TERRA_2013_07_27_02_45_GZ_1KM_FOG.AWX";
            IRasterDataProvider awxdprd = GeoDataDriver.Open(awxfile) as IRasterDataProvider;
            if (awxdprd!=null)
            {
                int bandx = awxdprd.BandCount;
            }
            string datf = @"D:\Geodo\陈超\FOG_Sea_FY3B_VIRR_1000M_20140713100000.dat";
            string hdrf = @"D:\Geodo\陈超\FOG_Sea_FY3B_VIRR_1000M_20140713100000.hdr";
            //AWXFile awx = new AWXFile();
            //awx.Write(hdrf, hdrf);

        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                CLDPrdsDataRetrieval nfrm = new CLDPrdsDataRetrieval();
                nfrm.StartPosition = FormStartPosition.CenterScreen;
                nfrm._openEventhandler += new CLDPrdsDataRetrieval.DoubleClickRowHdrEventHandler(OpenRetrievalFile);
                nfrm.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //string dir = @"D:\5分钟段产品\CloudFraction\AQUA\MODIS\2011\1\1\day\0.05";
            //string[] files = Directory.GetFiles(dir, "*MOD06*granule*.ldf");
            //Dictionary<string, List<string>> sortFiles = null;
            //DataProcesser dpro = new DataProcesser();
            ////将每天文件按照区域进行区分
            //sortFiles = dpro.SortFilesByRegion(files);//区域标识，文件列表
            //foreach (string key in sortFiles.Keys)
            //{
            //    //
            //    dpro.SortFilesByTime(sortFiles[key]);
            //}
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                //vgtForm frm = new vgtForm();
                //frm.Show();
                string path =@"E:\TEST";
                string[] lfiles = Directory.GetFiles(path, "*.ldf", SearchOption.TopDirectoryOnly);
                CloudParaFileStatics st = new CloudParaFileStatics();
                int bandno = 1;
                string [] fillvalue = null;
                string[] files = st.FilesSeriesMeanStat(lfiles, bandno, fillvalue, path, null);
                foreach (string file in files)
                    if (File.Exists(file))
                    {
                        Process.Start(file);
                    }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        StringBuilder sb = new StringBuilder();
        string fname = "";
        private void TryClearRecycleDir(string recycledir)
        {
            //应为递归处理，减少同时执行的任务量；
            try
            {
                //在指定目录及子目录下查找文件
                DirectoryInfo Dir = new DirectoryInfo(recycledir);
                foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录   
                {
                    //sb.Append(Dir + d.ToString() + "\\" + "\r\n");
                    TryClearRecycleDir(Path.Combine(Dir.FullName, d.ToString()));
                }
                foreach (FileInfo f in Dir.GetFiles("*.*"))             //查找文件
                {
                    fname = Path.Combine(Dir.FullName, f.ToString());
                    try
                    {
                        if (File.Exists(fname))
                        {
                            File.Delete(fname);
                            sb.Append("删除成功：" + fname + "\r\n");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        sb.Append("删除失败：" + fname + "\r\n");
                    }
                }
                if (Directory.Exists(recycledir))
                {
                    Directory.Delete(recycledir);
                    sb.Append("删除成功：" + recycledir + "\r\n");
                }
            }
            catch (System.Exception ex)
            {
                sb.Append("删除失败：" + recycledir  + "\r\n");
            }
        }


        private void button22_Click(object sender, EventArgs e)
        {
            //if (_argumentProvider.GetArg("SolarZenith") == null)
            //{
            //    PrintInfo("参数\"SolarZenith\"为空。");
            //    return null;
            //}
            //int szano = (int)(_argumentProvider.GetArg("SolarZenith"));
            //if (_argumentProvider.GetArg("SolarAzimuth") == null)
            //{
            //    PrintInfo("参数\"SolarAzimuth\"为空。");
            //    return null;
            //}
            //int saano = (int)(_argumentProvider.GetArg("SolarAzimuth"));
            //if (_argumentProvider.GetArg("SensorZenith") == null)
            //{
            //    PrintInfo("参数\"SensorZenith\"为空。");
            //    return null;
            //}
            //int vzano = (int)(_argumentProvider.GetArg("SensorZenith"));
            //if (_argumentProvider.GetArg("SensorAzimuth") == null)
            //{
            //    PrintInfo("参数\"SensorAzimuth\"为空。");
            //    return null;
            //}
            //int vaano = (int)(_argumentProvider.GetArg("SensorAzimuth"));

            string L2LSRFile = @"D:\Geodo\LAI\prj_file\Prj\FY3A_MERSI_GBAL_GLL_L1_20140611_0220_1000M.ldf";
            string l2lsrFname = L2LSRFile;
            int rnoCH = -1, irnoCH = -1, swirnoCH = -1;
            IBandNameRaster bandNameRaster = null;
            //bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            using (IRasterDataProvider dr = GeoDataDriver.Open(L2LSRFile) as IRasterDataProvider)
            {
                if (dr != null)
                    bandNameRaster = dr as IBandNameRaster;
            }
            if (bandNameRaster == null)
            {
                //PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return ;
            }
            int rno = 3, irno = 4, swirno = 6;
            int newbandNo = -1;
            if (bandNameRaster.TryGetBandNoFromBandName(rno, out newbandNo))
                rnoCH = newbandNo;
            if (bandNameRaster.TryGetBandNoFromBandName(irno, out newbandNo))
                irnoCH = newbandNo;
            if (bandNameRaster.TryGetBandNoFromBandName(swirno, out newbandNo))
                swirnoCH = newbandNo;

            string L2CLDMaskFile = @"D:\Geodo\LAI\prj_file\Prj\FY3A_MERSI_GLL_L2_CLM_MLT_NUL_20140611_0220_1000M_MS.LDF";
            string _saafile, _szafile, _vaafile, _vzafile;
            txtAngles.Text = Path.GetDirectoryName(l2lsrFname);
            _szafile = Path.Combine(txtAngles.Text, Path.GetFileNameWithoutExtension(l2lsrFname) + ".SolarZenith" + Path.GetExtension(l2lsrFname));
            _saafile = Path.Combine(txtAngles.Text, Path.GetFileNameWithoutExtension(l2lsrFname) + ".SolarAzimuth" + Path.GetExtension(l2lsrFname));
            _vzafile = Path.Combine(txtAngles.Text, Path.GetFileNameWithoutExtension(l2lsrFname) + ".SensorZenith" + Path.GetExtension(l2lsrFname));
            _vaafile = Path.Combine(txtAngles.Text, Path.GetFileNameWithoutExtension(l2lsrFname) + ".SensorAzimuth" + Path.GetExtension(l2lsrFname));
            string landCoverFile = @"D:\Geodo\LAI\Landcover\glc2000_v1_1.img";
                        IRasterDataProvider lsrRaster = null;
            IRasterDataProvider angleRaster = null;
            IRasterDataProvider L2CLDMaskRaster = null;
            IRasterDataProvider landcoverRaster = null;
            int outwidth, outheight;
            //int rno = 1, irno = 2, swirno = 3;
            try
            {
                #region 波段数据读取
                lsrRaster = GeoDataDriver.Open(L2LSRFile) as IRasterDataProvider;
                if (lsrRaster == null || lsrRaster.BandCount <3)
                    return;
                float prjReslX = lsrRaster.ResolutionX;
                double prjMinLon = lsrRaster.CoordEnvelope.MinX;
                double prjMaxLat = lsrRaster.CoordEnvelope.MaxY;
                outwidth = lsrRaster.Width;
                outheight = lsrRaster.Height;
                UInt16[] reddata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(rno), 0, 0, outwidth, outheight);
                UInt16[] IRdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(irno), 0, 0, outwidth, outheight);
                UInt16[] swirdata = SubProductVgtMersiLAI.GetDataValue<UInt16>(lsrRaster.GetRasterBand(swirno), 0, 0, outwidth, outheight);
                Int16[] sza, saa, vza, vaa;
                angleRaster = GeoDataDriver.Open(_szafile) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount <1)
                    return;
                sza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(_saafile) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return;
                saa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(_vzafile) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return;
                vza = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(_vaafile) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return ;
                vaa = SubProductVgtMersiLAI.GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                L2CLDMaskRaster = GeoDataDriver.Open(L2CLDMaskFile) as IRasterDataProvider;
                if (L2CLDMaskRaster == null || L2CLDMaskRaster.BandCount < 1)
                    return ;
                #region 计算云检测文件的xy偏移量；
                double l1minx = lsrRaster.CoordEnvelope.MinX, l1maxy = lsrRaster.CoordEnvelope.MaxY;
                float reslx = lsrRaster.ResolutionX, resly = lsrRaster.ResolutionY;
                double CLDMaskminx = L2CLDMaskRaster.CoordEnvelope.MinX, CLDMaskmaxy = L2CLDMaskRaster.CoordEnvelope.MaxY;
                int _prjCLDMSKxOffset = 0, _prjCLDMSKyOffset = 0;
                _prjCLDMSKxOffset = (int)Math.Ceiling((l1minx - CLDMaskminx) / reslx);
                _prjCLDMSKyOffset = (int)Math.Ceiling((CLDMaskmaxy - l1maxy) / reslx);
                #endregion
                byte[] cldmask = SubProductVgtMersiLAI.GetDataValue<byte>(L2CLDMaskRaster.GetRasterBand(1), _prjCLDMSKxOffset, _prjCLDMSKyOffset, outwidth, outheight);
                landcoverRaster = GeoDataDriver.Open(landCoverFile) as IRasterDataProvider;
                if (landcoverRaster == null || landcoverRaster.BandCount < 1)
                    return ;
                #endregion
                UInt16[][] output=new UInt16[1][];
                output[0] = new ushort[reddata.Length];
                #region 分块并行处理
                int parts = 7;
                int step = reddata.Length / parts;
                int[] istart = new int[parts];
                int[] iend = new int[parts];
                istart[0] = 0;
                iend[0] = 0 + step;
                for (int p = 1; p < parts; p++)
                {
                    istart[p] = istart[p - 1] + step;
                    iend[p] = iend[p - 1] + step;
                }
                iend[parts - 1] = reddata.Length;
            }
                #endregion
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button23_Click(object sender, EventArgs e)
        {
            try
            {
                //DataMove frm = new DataMove();
                //frm.Show();
                string dir = txtdir.Text;
                if (Directory.Exists(dir))
                {
                    string replacestr = txtreplacestr.Text;
                    string filter = txtfilter.Text;
                    string[] files = Directory.GetFiles(dir, filter);
                    string fname,nfname;
                    foreach(string file in files)
                    {
                        fname = Path.GetFileName(file);
                        if (fname.Contains(replacestr))
                        {
                            nfname = Path.Combine(dir, fname.Replace(replacestr, ""));
                            File.Move(file, nfname);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = txtdir.Text;
                if (Directory.Exists(dir))
                {
                    string[] dataset1kmList = new string[] { "CloudOpticalThickness", "CloudEffectiveRadius", "CloudWaterPath" };
                    string replacestr = txtreplacestr.Text;
                    string append = txtfilter.Text,filter ="*.ldf";
                    string[] files = Directory.GetFiles(dir, filter,SearchOption.AllDirectories);
                    string fname, nfname,olddir,ndir,nhdr;
                    bool is1km = false;
                    foreach (string file in files)
                    {
                        olddir = Path.GetDirectoryName(file);
                        foreach (string set in dataset1kmList)
                        {
                            if (olddir.Contains(set))
                            {
                                is1km = true;
                                break;
                            }
                        }
                        if (is1km)
                        {
                            append = "0.01";
                        }
                        else
                            append = "0.05";
                        ndir = olddir.Replace("AQUA", "TERRA");
                        if (!ndir.Contains(append))
                            ndir = Path.Combine(ndir, "day", append);
                        if (!Directory.Exists(ndir))
                        {
                            Directory.CreateDirectory(ndir);
                        }
                        fname = Path.GetFileNameWithoutExtension(file);
                        if (!fname.Contains("_"))
                        {
                            nfname = Path.Combine(ndir, fname.Replace("-", "_")+"_"+append+".ldf");
                            nhdr = Path.Combine(ndir, fname.Replace("-", "_") + "_" + append + ".hdr");
                            File.Move(file, nfname);
                            File.Move(Path.Combine(olddir, fname + ".hdr"), nhdr);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOverVIEW_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = txtdir.Text;
                TryOverViewFiles(dir, "*.ldf", null);
            }
            catch (System.Exception ex)
            {
            	
            }
        }

        private void TryOverViewFiles(string dir, string dayfileformat, Action<int, string> progressCallback = null)
        {
            try
            {
                //在指定目录及子目录下查找文件
                DirectoryInfo Dir = new DirectoryInfo(dir);
                foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录   
                {
                    //isFinaldir = false;
                    TryOverViewFiles(Path.Combine(Dir.FullName, d.ToString()), dayfileformat);
                }
                string[] parafiles = Directory.GetFiles(dir, dayfileformat, SearchOption.TopDirectoryOnly);
                if (parafiles.Length != 0)
                {
                    foreach (string file in parafiles)
                    {
                        if (!File.Exists(Path.Combine(Path.GetDirectoryName(file),Path.ChangeExtension(file,".overview.png"))))
                            OverViewHelper.OverView(file, 800);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            string bytefiledir =@"E:\周期合成产品\CloudFractionDay\TERRA\MODIS\day\0.05\Year\Avg";
            string bytefile = "CloudFractionDay_MOD06_china_Year_2011_AVG_day_0.05.LDF";
            string int16fdir = @"E:\周期合成产品\CloudTopTemperatureDay\TERRA\MODIS\day\0.05\Year\Avg";
            string int16file = "CloudTopTemperatureDay_MOD06_china_Year_2011_AVG_day_0.05.LDF";
            CloudParaFileStaticsAOI st = new CloudParaFileStaticsAOI();
            Dictionary<int, RasterQuickStatResult> results = null;// st.FilesHistoStat(files, bands, new string[] { "-1000" }, null);
            results = st.FilesHistoStatAOI(new string[] { Path.Combine(bytefiledir, bytefile)}, new int[] { 1 }, null, null, null, null);
            frmRasterQuickStat frm = new frmRasterQuickStat();//)
            {
                //frm.Owner = _smartSession.SmartWindowManager.MainForm as Form;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Apply(bytefile, results);
                frm.Show();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button28_Click(object sender, EventArgs e)
        {
            string bytefiledir = @"E:\周期合成产品\CloudFractionDay\TERRA\MODIS\day\0.05\Year\Avg\CloudFractionDay_MOD06_china_Year_2011_AVG_day_0.05.LDF";
            string bytefile = "CloudFractionDay_MOD06_china_Year_2011_AVG_day_0.05.LDF";
            string int16fdir = @"E:\周期合成产品\CloudTopTemperatureDay\TERRA\MODIS\day\0.05\Year\Avg\CloudTopTemperatureDay_MOD06_china_Year_2011_AVG_day_0.05.LDF";
            string int16file = "CloudTopTemperatureDay_MOD06_china_Year_2011_AVG_day_0.05.LDF";
            //string[] _lfiles = new string[] { bytefiledir };
            string[] _lfiles = Directory.GetFiles(@"E:\周期合成产品\CloudFractionDay\TERRA\MODIS\day\0.05\Month\Avg", "*.LDF", SearchOption.TopDirectoryOnly);
            int _leftBandNum =  1;
            string[] _fillvalueL = null;
            string _leftBandName ="band 1";
            string _outdir = @"e:\eof\temp";
            CloudParaFileStatics st = new CloudParaFileStatics();
            string[] result;
            try
            {
                result = st.FilesEOFStat(_lfiles, _leftBandNum, _leftBandName, _fillvalueL, false, _outdir, null,null,null,double.Parse("1"));
                if (result != null)
                {
                    foreach (string file in result)
                    {
                        if (Path.GetFileName(file).Contains("累计方差贡献"))
                        {
                            Process.Start(file);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            string fname=null;
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.InitialDirectory = @"D:\Geodo\PRJ_KQS";
                diag.Filter = "HDF5数据(*.HDF,*.H5)|*.HDF;*.H5";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fname=(diag.FileName);
                }
            }
            //fname = @"D:\Geodo\PRJ_KQS\FY2E_2013_10_12_19_00_turbulence.h5";
            //fname = @"D:\Geodo\PRJ_KQS\MOD06_L2.A2001051.1515.hdf";
            //fname = @"D:\Geodo\试验数据\FY3 L3产品数据\FY3-AB_大气产品\MERSI海上气溶胶日-旬-月产品\FY3A_MERSI_30B0_L2_ASO_MLT_GLL_20140227_POAD_1000M_MS.HDF";
            IRasterDataProvider dataPrd = GeoDataDriver.Open(fname) as IRasterDataProvider;

        }

        private void button30_Click(object sender, EventArgs e)
        {
            try
            {
                using (CLDDataRetrieval form = new CLDDataRetrieval())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string dbfile in DataReader.FileSelectedList.ToArray())
                        {
                            if (!File.Exists(dbfile))
                                throw new FileLoadException(dbfile + "文件不存在，请确认归档路径设置正确!");
                        }
                        string[] result = DataReader.FileSelectedList.ToArray();
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show((ex.Message)) ;
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            string dir = @"D:\Geodo\ZYB\项目资料\Prj_长时间序列云参数\数据\DATA_AIRS_L2_2012";
            string[] files = Directory.GetFiles(dir, "AIRS.*RetStd*.hdf", SearchOption.TopDirectoryOnly); ;
            GeoDo.RasterProject.PrjEnvelope _chinaENV = new GeoDo.RasterProject.PrjEnvelope(65, 145, 10, 60);
            foreach (string file in files)
            {
                #region 筛选中国区的AIRS文件
                GeoDo.RasterProject.PrjEnvelope env = AIRSDataProcesser.GetAIRSFileEnv(file);
                GeoDo.RasterProject.PrjEnvelope dstmainPrj = GeoDo.RasterProject.PrjEnvelope.Intersect(_chinaENV, env);
                if (dstmainPrj == null || dstmainPrj.Width <= 0 || dstmainPrj.Height <= 0)
                    continue;
                MessageBox.Show(Path.GetFileName(file) + "与中国区相交！", "提示", MessageBoxButtons.OKCancel);
                #endregion
            }
        }
        frmPlot frm = new frmPlot();
        private void button32_Click(object sender, EventArgs e)
        {
            string[] dtimes = new string[5] { "1", "2", "3", "4", "5" };
            ucCloudsatPlot plots = frm.plots;
            frm.plots.AddYAxis(dtimes);
            string fullfilename ="trst";
            Bitmap bmp = null;
            int  x1=1, x2=5, h1=1, h2=5;
            //plots.Reset(fullfilename, bmp, x1, x2, h1, h2, null);
            //plots.Rerender();
            //frm.Reset(fullfilename, bmp, x1, x2, h1, h2, dtimes, null);
            frm.Show();
        }


    }
}
