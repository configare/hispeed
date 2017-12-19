using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.DF.GPC;
using System.Text.RegularExpressions;
using GeoDo.RSS.Core.DF;
using SharpCompress.Reader;
using SharpCompress.Common;
using System.Threading;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class FileToDatabase : Form
    {
        private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
        private Dictionary<string, List<string>> _originFiles2Base = new Dictionary<string, List<string>>();
        private string _inputDir;
        private string _dataDocDir;
        private Action<string> _state;
        private string _log = "原始产品数据入库";
        private DataBaseArg arg;
        Thread runTaskThread = null, runTaskThread1 = null;

        public FileToDatabase()
        {
            InitializeComponent();
            InitSetting();
            InitTask();
            cbxPrdsLevl.SelectedIndexChanged +=new EventHandler(cbxPrdsLevlSelectedIndexChangeed);
            radiMODIS.Checked = true;
        }
      
        private void InitSetting()
        {
            cbxPrdsLevl.Items.Add("原始产品数据");
            //cbxPrdsLevl.Items.Add("历史日拼接数据");
            //cbxPrdsLevl.Items.Add("周期合成产品数据");
            //checkbxMODIS.Checked = true;
            cbxPrdsLevl.SelectedIndex = 0;
            if (!File.Exists(_dataBaseXml))
            {
                MessageBox.Show("数据库配置文件不存在，请先配置数据库！");
                return;
            }
            arg = DataBaseArg.ParseXml(_dataBaseXml);
            //txtDocDir.Text = arg.OutputDir;
        }

        private void InitTask()
        {
            Control.CheckForIllegalCrossThreadCalls = false;//关闭该异常检测的方式来避免异常的出现
            _state = new Action<string>(InvokeProgress);
        }

        private void InvokeProgress(string text)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<string>((arg) =>
                    {
                        txtErrorLog.AppendText(arg.ToString() + "\r\n");
                        LogFactory.WriteLine(_log, arg.ToString());
                        if (arg.ToString().Contains("处理结束"))
                            btnOK.Enabled = true;
                    }), text);
                }
                else
                {
                    txtErrorLog.AppendText(text.ToString() + "\r\n");
                    LogFactory.WriteLine(_log, text.ToString());
                    if (text.Contains("处理结束"))
                        btnOK.Enabled = true;
                }
            }
            catch (System.Exception ex)
            {
                LogFactory.WriteLine(_log, ex.Message);
                return;
            }
           
        }

        private void cbxPrdsLevlSelectedIndexChangeed(object sender, EventArgs e)
        {
            if (cbxPrdsLevl.SelectedIndex==1)//日合成
            {
                checkbxCloudSAT.Visible = false;
                checkbxISCCP.Visible = false;
                checkbxMODIS.Visible = true;
                checkbxAIRS.Visible = false;
            }
            else if (cbxPrdsLevl.SelectedIndex==2)//周期合成
            {
                checkbxCloudSAT.Visible = false;
                checkbxISCCP.Visible = false;
                checkbxMODIS.Visible = true;
                checkbxAIRS.Visible = true;
            }
            else//raw
            {
                //checkbxCloudSAT.Visible = true;
                //checkbxISCCP.Visible = true;
                //checkbxMODIS.Visible = true;
                //checkbxAIRS.Visible = true;
            }
        }

        private void btnOpenInDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtInDir.Text = dlg.SelectedPath;
                    _inputDir = dlg.SelectedPath;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btnOK.Enabled = false;
                _inputDir = txtInDir.Text;
                _dataDocDir = txtDocDir.Text;
                if (!CheckArgsIsOk())
                    return;
                if (!Directory.Exists(_inputDir))
                    return;
                if (!Directory.Exists(_dataDocDir))
                    Directory.CreateDirectory(_dataDocDir);
                runTaskThread = new Thread(new ThreadStart(DoProcess));
                runTaskThread.IsBackground = true;
                if (_state != null)
                    _state("开始将数据归档并入库，请稍候...");
                runTaskThread.Start();
            }
            catch (System.Exception ex)
            {
                txtErrorLog.Text += "错误信息:"+ex.Message+"\r\n";
                MessageBox.Show("错误信息:"+ex.Message);
                btnOK.Enabled = true;
            }
            finally
            {
                this.Activate();
            }
        }

        private void DoProcess()
        {
            if (_originFiles2Base.Count != 0)
            {
                _originFiles2Base.Clear();
            }
            if (cbxPrdsLevl.SelectedIndex == 0)//原始数据
            {
                try
                {
                    string subdataDocDir = Path.Combine(_dataDocDir, "原始数据");
                    #region 原始数据入库
                    //_logfName = "RawData2Database";
                    if (radiMODIS.Checked)
                    {
                        GetMODOriginDataFiles(_state);
                        if (!_originFiles2Base.Keys.Contains("MOD06") || _originFiles2Base["MOD06"].Count < 1)
                        {
                            txtErrorLog.Text += "当前路径不存在可入库的MODIS数据！\r\n";
                        }
                    }
                    else if (radiAIRS.Checked)
                    {
                        GetAIRSOriginDataFiles(_state);
                        if (!_originFiles2Base.Keys.Contains("AIRS") || _originFiles2Base["AIRS"].Count < 1)
                        {
                            txtErrorLog.Text += "当前路径不存在可入库的AIRS数据！\r\n";
                        }
                    }
                    else if (radiISCCP.Checked)
                    {
                        GetISCCPOriginDataFiles(_state);
                        if (_originFiles2Base.Keys.Contains("ISCCP"))
                        {
                            if (_state != null)
                                _state("共计" + _originFiles2Base["ISCCP"].Count + "个待入库ISCCP文件！");
                            else
                                txtErrorLog.Text += "共计" + _originFiles2Base["ISCCP"].Count + "个待入库ISCCP文件！\r\n";
                        }
                    }
                    else if (radiCloudSAT.Checked)
                    {
                        GetCloudSATOriginDataFiles(_state);
                        if (!_originFiles2Base.Keys.Contains("CloudSAT") || _originFiles2Base["CloudSAT"].Count < 1)
                        {
                            txtErrorLog.Text += "当前路径不存在可入库的CloudSAT数据！\r\n";
                        }
                    }
                    if (_originFiles2Base.Count == 0)
                    {
                        MessageBox.Show("当前目录不存在可入库的文件！请重新选择");
                        return;
                    }
                    #region 数据归档
                    //Dictionary<string, List<string>> uniformOriginFiles = new Dictionary<string, List<string>>();
                    //foreach (string mode in _originFiles2Base.Keys)
                    //{
                    //    if (_state != null)
                    //        _state(string.Format("正在归档{0}数据，请稍候...", mode));
                    //    List<string> unifiles = FileToDatabase.Files2UniformDir(_originFiles2Base[mode], _dataDocDir, mode, _state);
                    //    uniformOriginFiles.Add(mode, unifiles);
                    //}
                    #endregion
                    try
                    {
                        OriginData2Database OriDbase = new OriginData2Database(subdataDocDir, _originFiles2Base, cbxOverrideRecord.Checked, _state);
                        OriDbase.IsFiles2UniformDir = cbxData2DocDir.Checked;
                        OriDbase._DocDir = _dataDocDir;
                        runTaskThread1 = new Thread(new ThreadStart(OriDbase.CheckFile2Table));
                        runTaskThread1.IsBackground = true;
                        if (_state != null)
                            _state(string.Format("开始将数据归档并入库，请稍候..."));
                        runTaskThread1.Start();
                    }
                    catch (System.Exception ex)
                    {
                        _state(ex.Message);
                        return;
                    }
                    #endregion
                }
                catch (System.Exception ex)
                {
                    _state(ex.Message);
                    return;
                }

            }
            else if (cbxPrdsLevl.SelectedIndex == 1)//历史日产品数据
            {
                #region 日产品数据入库
                string fileFilter,sensor;
                if (radiAIRS.Checked)
                {
                    fileFilter = "*AIRS*day*.ldf";//文件名的格式有待确认
                    sensor = "AIRS";
                }
                else
                {
                    fileFilter = "mod06_*_*.dat";//文件名的格式有待确认
                    sensor = "MODIS";
                }
                List<string> dayprdsfiles = Directory.GetFiles(_inputDir, fileFilter, SearchOption.AllDirectories).ToList();
                //归档；
                    //从文件名中解析数据的时间信息、数据集信息、区域信息、分辨率信息、日夜标识信息
                    //拼接生成数据的归档路径，对数据按照现有的规则进行重命名；//日拼接产品\CloudTopTemperature\TERRA\MODIS\2011\1\day\0.05
                List<string> unidayprdsfiles = new List<string>();
                //string newfnameformat = "{0}_MOD06_china_day_{1}_0.01.dat";
                string newfname = "";
                foreach (string oldfile in dayprdsfiles)
                {
                    //mod06_20131201_CMLF.dat
                    //CloudOpticalThickness_MOD06_china_day_20110101_0.01.LDF
                    System.IO.File.Move(oldfile, newfname);
                }
                //入库---注意数据中的无效值问题
                    //需要查询数据集对应的产品ID及数据集ID，regionID，
                //double selfFillValue = _selfFillValue[setName];
                //double dayFillValue = _dayFillValue[setName];
                //double dayInvalidValue = _dayInvalidValue[setName];
                //重新计算有效百分比，granulesCount/granulesTimes、日夜标识DataSource
                #region 入库
                //归档路径
                string tableName = "CP_DayMergeProducts_TB";
                ConnectMySqlCloud dbcon = new ConnectMySqlCloud(_dataBaseXml);
                string setName, imagedata, regionName, fname;
                float resl;
                double invalidValue=32767;
                int validPercent,regionID=1;;
                long prdID, datasetID=0; 
                string datasource = "";
                string[] parts;
                DateTime fdate;
                DataProcesser datapro =new DataProcesser();
                foreach (string newFileName in unidayprdsfiles)
                {
                    imagedata = newFileName.Replace(_dataDocDir, "");
                    fname = Path.GetFileNameWithoutExtension(newFileName);
                    parts = fname.Split('_');
                    if (parts.Length != 6)
                        continue;
                    setName = parts[0];
                    if (!dbcon.QueryDatasetsID(sensor, setName, out prdID,out datasetID))
                        continue;
                    regionName=parts[2];
                    if (!dbcon.QueryRegionID(regionName,out regionID))
                    {

                    }
                    if (parts[3].ToLower() == "day")
                        datasource = "D";
                    else
                        datasource = "N";
                    //fdate =DataProcesser.getDayTime(newFileName);
                    fdate = DateTime.TryParseExact(Path.GetFileNameWithoutExtension(newFileName).Split('_')[4], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out fdate) ? fdate : DateTime.MinValue;
                    validPercent =ComputeValidPercent(datapro,newFileName,invalidValue,out resl);
                    if (!dbcon.IshasRecord(tableName, "ImageName", Path.GetFileName(newFileName)))
                    {
                        dbcon.InsertNDayMergeProductsTable(fdate, prdID, datasetID, imagedata, regionID, regionName, sensor, resl, validPercent, 0, "", datasource);
                        _state(newFileName+"入库完成！");
                    }
                    else
                    {
                        dbcon.DeleteCLDParatableRecord(tableName, "ImageName", Path.GetFileName(newFileName));
                        dbcon.InsertNDayMergeProductsTable(fdate, prdID, datasetID, imagedata, regionID, regionName, sensor, resl, validPercent, 0, "", datasource);
                        _state(newFileName + "入库更新完成！");
                    }
                }
                #endregion
                #endregion
            }
            else if (cbxPrdsLevl.SelectedIndex == 2)//周期合成产品
            {
                //PeriodicSynPrds2Base psp = new PeriodicSynPrds2Base(_inputDir, _logfName, _dataBaseXml);
                //psp.StartComp();
            }

        }
        /// <summary>
        /// 日拼接产品的连续性检验
        /// </summary>
        /// <param name="allmosaicFiles"></param>
        public static bool DayMergePrdsContinutyDetection(string [] allfiles, out Dictionary<string, List<DateTime>> lostMosaicDayPrds)
        {
            Dictionary<string,List<string>> prdsDir = new Dictionary<string ,List<string>>();//日产品的目录
            foreach (string file in allfiles)
            {
                string dir = Path.GetDirectoryName(file);
                if (!prdsDir.ContainsKey(dir))
                {
                    prdsDir.Add(dir, new List<string>() { file });
                }
                else
                {
                    prdsDir[dir].Add(file);
                }
            }
            //产品分region

            //sort各region日产品文件
            Dictionary<string, List<string>> allmosaicFiles =new Dictionary<string,List<string>>();
            lostMosaicDayPrds = new Dictionary<string, List<DateTime>>();
            foreach (string region in allmosaicFiles.Keys)
            {
                List<DateTime> modate = new List<DateTime>();
                foreach (string mosaicf in allmosaicFiles[region])
                {
                    string fileName = Path.GetFileNameWithoutExtension(mosaicf);
                    string[] parts = fileName.Split('_');
                    if (parts.Length == 5)
                    {
                        string da = parts[4];
                        int year = Int32.Parse(da.Substring(0, 4));
                        int month = Int32.Parse(da.Substring(4, 2));
                        int day = Int32.Parse(da.Substring(6, 2));
                        modate.Add(new DateTime(year, month, day));
                    }
                }
                DateTime mindate = modate.Min();
                DateTime maxdate = modate.Max();
                for (DateTime i = mindate; i < maxdate; i.AddDays(1))
                {
                    if (!modate.Contains(i))
                    {
                        if (!lostMosaicDayPrds.Keys.Contains(region))
                        {
                            lostMosaicDayPrds.Add(region, new List<DateTime>());
                        }
                        lostMosaicDayPrds[region].Add(i);
                    }
                }
            }
            if (lostMosaicDayPrds.Count != 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 日拼接连续性检验入库
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="lostMosaicDayPrds"></param>
        private void DayMergePrdsUnContinuty2Base(string setName, Dictionary<string, List<DateTime>> lostMosaicDayPrds)
        {
            string sat = "AQUA", sensor = "MODIS", region;
            ConnectMySqlCloud con = new ConnectMySqlCloud();
            long prdID;
            if (con.QueryPrdID(sensor, setName, out prdID))
            {
                foreach (string reg in lostMosaicDayPrds.Keys)
                {
                    region = reg;
                    foreach (DateTime da in lostMosaicDayPrds[reg])
                    {
                        con.InsertPrdsDataTimeContinutyTable(prdID,setName, da, sat, sensor, region);
                    }
                }
            }
        }

        public int ComputeValidPercent(DataProcesser datapro,string fname,double invalidValue,out float resl)
        {
            resl = 0;
            using (IRasterDataProvider dataprd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                resl = dataprd.ResolutionX;
                return datapro.ComputeVaildPct(dataprd, invalidValue);
            }
        }

        public static void GetRslandEnvolop(string fname,string invalidValue, out float resl, out double lulon, out double lulat, out double rdlon, out double rdlat, out double validPercent)
        {
            resl = 0; lulon = 0; lulat = 0; rdlon = 0; rdlat = 0; validPercent = 0;
            string region = Path.GetFileNameWithoutExtension(fname).Split('_')[2];
            using (IRasterDataProvider dataprd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                resl = dataprd.ResolutionX;
                lulon = dataprd.CoordEnvelope.MinX;
                lulat = dataprd.CoordEnvelope.MaxY;
                rdlon = dataprd.CoordEnvelope.MaxX;
                rdlat = dataprd.CoordEnvelope.MinY;
                IRasterBand band = dataprd.GetRasterBand(1);
                enumDataType dataType = dataprd.DataType;
                int width = band.Width;
                int height = band.Height;
                int validCount = 0;
                switch (dataType)
                {
                    case enumDataType.Float:
                        unsafe
                        {
                            float[] currentDatabuffer = new float[width * height];
                            fixed (float* cptr = currentDatabuffer)
                            {
                                IntPtr currentBuffer = new IntPtr(cptr);
                                band.Read(0, 0, width, height, currentBuffer, dataType, width, height);                                    
                                for (int i = 0; i < currentDatabuffer.Length; i++)
                                {
                                    if (string.IsNullOrWhiteSpace(invalidValue)||currentDatabuffer[i] != float.Parse(invalidValue))
                                    {
                                        validCount++;
                                    }
                                }
                            }
                            break;
                        }
                    case enumDataType.Int16:
                        unsafe
                        {
                            Int16[] currentDatabuffer = new Int16[width * height];
                            fixed (Int16* cptr = currentDatabuffer)
                            {
                                IntPtr currentBuffer = new IntPtr(cptr);
                                band.Read(0, 0, width, height, currentBuffer, dataType, width, height);
                                for (int i = 0; i < currentDatabuffer.Length; i++)
                                {
                                    if (string.IsNullOrWhiteSpace(invalidValue) || currentDatabuffer[i] != short.Parse(invalidValue))
                                    {
                                        validCount++;
                                    }
                                }
                            }
                        }
                        break;
                }
                validPercent = validCount / (width*height*1.0)*100.0;

            }
        }

        private void GetISCCPOriginDataFiles(Action<string> state)
        {
            #region ISCCP原始数据
            if (state!=null)
                state("扫描"+_inputDir+"的ISCCP.D2文件！");
            string[] isccpFiles = Directory.GetFiles(_inputDir, "ISCCP.D2.*.GPC", SearchOption.TopDirectoryOnly);
            ScanISCCPD2files(isccpFiles);
            DirectoryInfo path = new DirectoryInfo(_inputDir);
            foreach (DirectoryInfo subdir in path.GetDirectories())
            {
                if (state != null)
                    state(string.Format("扫描{0}的ISCCP.D2文件！", subdir.FullName));
                isccpFiles = Directory.GetFiles(subdir.FullName, "ISCCP.D2.*.GPC", SearchOption.AllDirectories);
                ScanISCCPD2files(isccpFiles);
            }
            #endregion;
        }

        private void ScanISCCPD2files(string[] isccpFiles)
        {
            List<string> validIsccpf = new List<string>();
            if (isccpFiles.Length > 0)
            {
                FileInfo finfo;
                foreach (string file in isccpFiles)
                {
                    finfo = new FileInfo(file);
                    if (finfo.Length == 871000)
                    {
                        if (_originFiles2Base.ContainsKey("ISCCP"))
                            _originFiles2Base["ISCCP"].Add(file);
                        else
                            _originFiles2Base.Add("ISCCP", new List<string>(){file});
                    }
                }
            }
        }
        private void GetMODOriginDataFiles(Action<string> state)
        {
            #region MOD06
            if (state != null)
                state("扫描MOD03文件！");//" + _inputDir + "的
            string[] m03Files = Directory.GetFiles(_inputDir, "MOD03*.HDF", SearchOption.AllDirectories);
            foreach (string file in m03Files)
            {
                DataContinuityCheck.RegularFileNames(file);
            }
            if (state != null)
                state("扫描MOD06文件！");//" + _inputDir + "的
            string[] m06Files = Directory.GetFiles(_inputDir, "MOD06_L2*.HDF", SearchOption.AllDirectories);
            foreach (string file in m06Files)
            {
                DataContinuityCheck.RegularFileNames(file);
            }
            List<string> validMod06f = new List<string>();
            List<string> validMod03f = new List<string>();
            //m03Files = Directory.GetFiles(_inputDir, "MOD03*.HDF", SearchOption.AllDirectories);
            if (m06Files.Length > 0)//m06Files.Length==m03Files.Length
            {
                if (state != null)
                    state("正在匹配MOD06与MOD03文件！");
                foreach (string file in m06Files)
                {
                    string newfile=DataContinuityCheck.RegularFileNames(file);
                    string path = Path.GetDirectoryName(newfile);
                    string m06fname = Path.GetFileName(newfile);
                    string m03fname = Path.Combine(path, m06fname.Replace("MOD06_L2", "MOD03"));
                    if (File.Exists(m03fname))
                    {
                        validMod06f.Add(newfile);
                        validMod03f.Add(m03fname);
                    }
                }
                if (validMod06f.Count > 0)
                    _originFiles2Base.Add("MOD06", validMod06f);
                if (validMod06f.Count > 0)
                    _originFiles2Base.Add("MOD03", validMod03f);
                if (state != null)
                    state("共计" + _originFiles2Base["MOD06"].Count + "个待入库MOD06文件！");
            }
            #endregion

        }
        private void GetCloudSATOriginDataFiles(Action<string> state)
        {
            #region CloudSAT
            string fileformat ="*_CS_*.hdf";
            string zipformat = fileformat+".zip";
            if (state != null)
                state("开始扫描" + _inputDir + "下的cloudSAT文件...");
            //UnzipCloudSATFiles(_inputDir, zipformat,state);
            string[] CloudSATFiles = Directory.GetFiles(_inputDir, zipformat, SearchOption.AllDirectories);
            if (CloudSATFiles.Length > 0)
            {
                _originFiles2Base.Add("CloudSAT", CloudSATFiles.ToList());
                if (state != null)
                    state("共计" + CloudSATFiles.Length + "个待入库cloudSAT文件！");
            }
            #endregion
        }

        private void GetAIRSOriginDataFiles(Action<string> state)
        {
            #region AIRS
                DirectoryInfo fatherPath = new DirectoryInfo(_inputDir);
                if (state != null)
                    state("开始扫描" + _inputDir + "下的AIRS文件！");
                string[] AIRSFiles = Directory.GetFiles(_inputDir, "AIRS.*RetStd*.hdf", SearchOption.TopDirectoryOnly);
                if (AIRSFiles.Length > 0)
                {
                    _originFiles2Base.Add("AIRS", AIRSFiles.ToList());
                }
                foreach (DirectoryInfo child in fatherPath.GetDirectories("*"))
                {
                    if (state != null)
                        state("开始扫描" + child.FullName + "下的AIRS文件！");
                    AIRSFiles = Directory.GetFiles(child.FullName, "AIRS.*RetStd*.hdf", SearchOption.AllDirectories);
                    if (AIRSFiles.Length > 0)
                    {
                        if (!_originFiles2Base.ContainsKey("AIRS"))
                        {
                            _originFiles2Base.Add("AIRS", AIRSFiles.ToList());
                        }
                        else
                        {
                            foreach (string file in AIRSFiles)
                            {
                                _originFiles2Base["AIRS"].Add(file);
                            }
                        }                    
                    }
                }
                if (state != null)
                    state("共计" + _originFiles2Base["AIRS"].Count + "个待入库AIRS文件！");

            #endregion
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult= DialogResult.Cancel;
            //System.Environment.Exit(System.Environment.ExitCode);
            if (runTaskThread1 != null)
                runTaskThread1.Abort();
            if (runTaskThread != null)
                runTaskThread.Abort();
            Close();
        }

        private bool CheckArgsIsOk()
        {
            if (string.IsNullOrEmpty(_inputDir) || !Directory.Exists(_inputDir))
                throw new ArgumentException("请选择待入库文件所在文件夹！");
            if (cbxData2DocDir.Checked )
            {
                if (string.IsNullOrEmpty(_dataDocDir))
                    throw new ArgumentException("请选择归档目录！");
            }
            if (!radiISCCP.Checked && !radiMODIS.Checked && !radiAIRS.Checked && !radiCloudSAT.Checked)
                throw new ArgumentException("请选择数据类型！");
            if (cbxPrdsLevl.SelectedItem==null)
                throw new ArgumentException("请选择数据级别！");
            return true;
        }

        private void btnOpenOutputLog_Click(object sender, EventArgs e)
        {
            string log = string.Format("原始产品数据入库.DataProcess.{0}.log", DateTime.Now.ToString("yyyyMMdd"));
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", log);
            if (File.Exists(dir))
            {
                System.Diagnostics.Process.Start(dir);
            }
            else if (Directory.Exists(Path.GetDirectoryName(dir)))
            {
                System.Diagnostics.Process.Start("Explorer.exe", dir);
            }
            else
            {
                MessageBox.Show("无法开文件打位置，请确定目录" + Path.GetDirectoryName(dir)+"存在！");
            }
        }

        private void btnBrowseDocDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtDocDir.Text = dlg.SelectedPath;
                    _dataDocDir = dlg.SelectedPath;
                }
            }
        }

        private void txtErrorLog_TextChanged(object sender, EventArgs e)
        {
            txtErrorLog.Focus();
            txtErrorLog.Select(txtErrorLog.Text.Length,0);
            txtErrorLog.ScrollToCaret();
        }

        private void checkbxMODIS_CheckedChanged(object sender, EventArgs e)
        {
            
            if (checkbxMODIS.Checked)
            {
                txtDocDir.Text = arg.OutputDir;
                checkbxAIRS.Checked = false;
                checkbxCloudSAT.Checked = false;
                checkbxISCCP.Checked = false;
                checkbxAIRS.Enabled = false;
                checkbxCloudSAT.Enabled = false;
                checkbxISCCP.Enabled = false;
            }
            else if (checkbxISCCP.Checked)
            {
                txtDocDir.Text = arg.ISCCPRootPath;
                checkbxAIRS.Checked = false;
                checkbxCloudSAT.Checked = false;
                checkbxMODIS.Checked = false;
                checkbxAIRS.Enabled = false;
                checkbxCloudSAT.Enabled = false;
                checkbxMODIS.Enabled = false;
            }
            else if (checkbxCloudSAT.Checked)
            {
                txtDocDir.Text = arg.CloudSATRootPath;
                checkbxAIRS.Checked = false;
                checkbxMODIS.Checked = false;
                checkbxISCCP.Checked = false;
                checkbxAIRS.Enabled = false;
                checkbxMODIS.Enabled = false;
                checkbxISCCP.Enabled = false;
            }
            else if (checkbxAIRS.Checked)
            {
                txtDocDir.Text = arg.AIRSRootPath;
                checkbxMODIS.Checked = false;
                checkbxCloudSAT.Checked = false;
                checkbxISCCP.Checked = false;
                checkbxMODIS.Enabled = false;
                checkbxCloudSAT.Enabled = false;
                checkbxISCCP.Enabled = false;
            }
            else
            {
                txtDocDir.Text = "";
                checkbxMODIS.Enabled = true;
                checkbxAIRS.Enabled = true;
                checkbxCloudSAT.Enabled = true;
                checkbxISCCP.Enabled = true;
            }
        }

        private void radiMODIS_CheckedChanged(object sender, EventArgs e)
        {
            if (radiMODIS.Checked)
            {
                txtDocDir.Text = arg.OutputDir;
                //radiAIRS.Checked = false;
                //radiCloudSAT.Checked = false;
                //radiISCCP.Checked = false;
                //radiAIRS.Enabled = false;
                //radiCloudSAT.Enabled = false;
                //radiISCCP.Enabled = false;
            }
            else if (radiISCCP.Checked)
            {
                txtDocDir.Text = arg.ISCCPRootPath;
                //checkbxAIRS.Checked = false;
                //checkbxCloudSAT.Checked = false;
                //checkbxMODIS.Checked = false;
                //checkbxAIRS.Enabled = false;
                //checkbxCloudSAT.Enabled = false;
                //checkbxMODIS.Enabled = false;
            }
            else if (radiCloudSAT.Checked)
            {
                txtDocDir.Text = arg.CloudSATRootPath;
                //checkbxAIRS.Checked = false;
                //checkbxMODIS.Checked = false;
                //checkbxISCCP.Checked = false;
                //checkbxAIRS.Enabled = false;
                //checkbxMODIS.Enabled = false;
                //checkbxISCCP.Enabled = false;
            }
            else if (radiAIRS.Checked)
            {
                txtDocDir.Text = arg.AIRSRootPath;
                //checkbxMODIS.Checked = false;
                //checkbxCloudSAT.Checked = false;
                //checkbxISCCP.Checked = false;
                //checkbxMODIS.Enabled = false;
                //checkbxCloudSAT.Enabled = false;
                //checkbxISCCP.Enabled = false;
            }
            else
            {
                txtDocDir.Text = "";
                //checkbxMODIS.Enabled = true;
                //checkbxAIRS.Enabled = true;
                //checkbxCloudSAT.Enabled = true;
                //checkbxISCCP.Enabled = true;
            }

        }
    }
}
