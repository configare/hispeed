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
using System.Diagnostics;
using System.Threading;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class DataContinuityCheck : Form
    {
        private Dictionary<string, List<string>> _originFiles2Base = new Dictionary<string, List<string>>();
        private string _inputDir;
        string _lostlog = "{0}数据缺失信息";
        private string _log;
        private Action<string> _state;
        Thread runTaskThread = null;

        public DataContinuityCheck()
        {
            InitializeComponent();
            InitSetting();
            InitTask();
            cbxPrdsLevl.SelectedIndexChanged += new EventHandler(cbxPrdsLevlSelectedIndexChangeed);
        }

        private void InitSetting()
        {
            cbxPrdsLevl.Items.Add("原始产品数据");
            //cbxPrdsLevl.Items.Add("日拼接产品数据");
            //cbxPrdsLevl.Items.Add("周期合成产品数据");
            cbxPrdsLevl.SelectedIndex = 0;
            rdbMODIS.Checked = true;
        }
        private void InitTask()
        {
            Control.CheckForIllegalCrossThreadCalls = false;//关闭该异常检测的方式来避免异常的出现
            _state = new Action<string>(InvokeProgress);
        }

        private void InvokeProgress(string text)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>((arg) =>
                {
                    txtErrorLog.AppendText(arg.ToString() + "\r\n");
                    LogFactory.WriteLine(_log, arg.ToString());
                    if (arg.ToString().Contains("处理完成!") || arg.ToString().Contains("异常提示"))
                        btnOK.Enabled = true;
                }), text);
            }
            else
            {
                txtErrorLog.AppendText(text.ToString() + "\r\n");
                LogFactory.WriteLine(_log, text.ToString());
                if (text.Contains("处理完成!") || text.ToString().Contains("异常提示"))
                    btnOK.Enabled = true;
            }
        }

        private void cbxPrdsLevlSelectedIndexChangeed(object sender, EventArgs e)
        {
            if (cbxPrdsLevl.SelectedIndex == 0)            //raw
            {
                rdbISCCP.Visible = false;
                rdbMODIS.Visible = true;
                rdbAIRS.Visible = true;
            }
            else if (cbxPrdsLevl.SelectedIndex == 1)//日合成
            {
                rdbISCCP.Checked = false;
                rdbISCCP.Visible = false;
                rdbMODIS.Visible = true;
                rdbAIRS.Visible = true;
            }
            else if (cbxPrdsLevl.SelectedIndex == 2)//周期合成
            {
                rdbISCCP.Checked = false;
                rdbISCCP.Visible = false;
                rdbMODIS.Visible = true;
                rdbAIRS.Visible = true;
            }
        }

        private void btnOpenInDir_Click(object sender, EventArgs e)
        {
            try
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
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btnOK.Enabled = false;
                _inputDir = txtInDir.Text;
                //txtErrorLog.Clear();
                if (!CheckArgsIsOk())
                    return;
                runTaskThread = new Thread(new ThreadStart(this.DoProcess));
                runTaskThread.IsBackground = true;
                runTaskThread.Start();
                this.Activate();
            }
            catch (System.Exception ex)
            {
                LogFactory.WriteLine("数据完整性检验", ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnOK.Enabled = true;
            }
        }
        private void DoProcess()
        {
            if (_originFiles2Base.Count!=0)
                _originFiles2Base.Clear();
            if (cbxPrdsLevl.SelectedIndex == 0)//原始数据
            {
                #region 原始数据检验
                if (rdbISCCP.Checked)
                {
                    _log = string.Format(_lostlog, "ISCCP");
                    string[] files = GetISCCPOriginDataFiles(_state);
                    CheckISCCPFileContinuty(files, _state);
                }
                else if (rdbMODIS.Checked)
                {
                    _log = string.Format(_lostlog, "MODIS");
                    CheckMOD06FileContinuty(_inputDir, _state);
                    //LogFactory.WriteLine("MODIS连续性检测", _inputDir + "检测完成！");
                }
                else if (rdbAIRS.Checked)
                {
                    _log = string.Format(_lostlog, "AIRS");
                    string[] files = GetAIRSOriginDataFiles();
                    CheckAIRSFileContinuty(files, _state);
                }
                #endregion
            }
            else if (cbxPrdsLevl.SelectedIndex == 1)//日产品数据
            {
                #region 日产品数据检测
                string fileFilter, sensor;
                if (rdbAIRS.Checked)
                {
                    fileFilter = "*AIRS*day*.ldf";
                    sensor = "AIRS";
                }
                else
                {
                    fileFilter = "*MOD06*day*.ldf";
                    sensor = "MODIS";
                }
                List<string> dayprdsfiles = Directory.GetFiles(_inputDir, fileFilter, SearchOption.AllDirectories).ToList();
                string fname, region;//setname, 
                string[] parts;
                //CloudMask1km_MOD06_china_day_20110101.hdr
                foreach (string file in dayprdsfiles)
                {
                    fname = Path.GetFileNameWithoutExtension(file);
                    parts = fname.Split('_');
                    if (parts.Length != 6)
                    {
                        dayprdsfiles.Remove(file);
                        continue;
                    }
                    region = parts[2];
                    if (region.ToUpper() == "ALL")
                    {
                        dayprdsfiles.Remove(file);
                        continue;
                    }
                }
                #endregion
                if (dayprdsfiles.Count == 0)
                {
                    MessageBox.Show("当前目录不存在可以检测的日拼接产品文！请重试！");
                    return;
                }
                #region 连续性检测
                DayMergePrdsContinutyDetection(dayprdsfiles.ToArray());
                #endregion
            }
            _state("-----------------------------------");
            _state("处理完成！");//启动新线程
        }

        #region 日合成检测
        /// <summary>
        /// 日拼接产品的连续性检验
        /// </summary>
        /// <param name="allmosaicFiles"></param>
        public static void DayMergePrdsContinutyDetection(string[] allfiles)
        {
            Dictionary<string, List<string>> prdsDir = new Dictionary<string, List<string>>();//日产品的目录
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
            Dictionary<string, List<DateTime>> lostMosaicDayPrds = new Dictionary<string, List<DateTime>>();
            //产品分region
            foreach (string dir in prdsDir.Keys)
            {
                //每个文件夹中的月数据按区域分类
                Dictionary<string, List<string>> sortFiles = SortMoasicFiles(prdsDir[dir].ToArray());
                string setName = "";
                foreach (string region in sortFiles.Keys)
                {
                    if (region.ToUpper() == "all")
                        continue;
                    List<DateTime> modate = new List<DateTime>();
                    setName = Path.GetFileNameWithoutExtension(sortFiles[region].ToArray()[0]).Split('-')[0];
                    foreach (string mosaicf in sortFiles[region])
                    {
                        string fileName = Path.GetFileNameWithoutExtension(mosaicf);
                        string[] parts = fileName.Split('-');
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
                                lostMosaicDayPrds.Add(region, new List<DateTime>() { i });
                            }
                            //lostMosaicDayPrds[region].Add(i);
                        }
                    }
                }
                if (lostMosaicDayPrds.Count != 0)
                {
                    DayMergePrdsUnContinuty2Base(setName, lostMosaicDayPrds);
                }
            }
        }

        private static Dictionary<string, List<string>> SortMoasicFiles(string[] files)
        {
            if (files == null || files.Length < 1)
                return null;
            Dictionary<string, List<string>> sortedFiles = new Dictionary<string, List<string>>();
            string region;
            for (int i = 0; i < files.Length; i++)
            {
                ParseFileRegion(Path.GetFileName(files[i]), out region);
                if (!string.IsNullOrEmpty(region))
                {
                    if (sortedFiles.ContainsKey(region))
                        sortedFiles[region].Add(files[i]);
                    else
                        sortedFiles.Add(region, new List<string> { files[i] });
                }
            }
            return sortedFiles;
        }

        //解析文件名称中区域名、时间
        private static void ParseFileRegion(string fileName, out string region)
        {
            //CloudEffectiveRadius-MOD06-china-day-20110101.LDF
            region = string.Empty;
            if (string.IsNullOrEmpty(fileName))
                return;
            string[] fileStrs = fileName.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (fileStrs.Length != 5)
                return;
            region = fileStrs[2];
        }

        /// <summary>
        /// 日拼接连续性检验入库
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="lostMosaicDayPrds"></param>
        private static void DayMergePrdsUnContinuty2Base(string setName, Dictionary<string, List<DateTime>> lostMosaicDayPrds)
        {
            //string sat = "TREEA", sensor = "MODIS", region;
            //ConnectMySqlCloud con = new ConnectMySqlCloud();
            //long prdID;
            //if (_dbConnect.QueryPrdID(sensor, setName, out prdID))
            //{
            //    foreach (string reg in lostMosaicDayPrds.Keys)
            //    {
            //        region = reg;
            //        foreach (DateTime da in lostMosaicDayPrds[reg])
            //        {
            //            _dbConnect.InsertPrdsDataTimeContinutyTable(prdID, setName,da, sat, sensor, region);
            //        }
            //    }
            //}
        }

#endregion

        #region 原始数据检测
        private string[] GetISCCPOriginDataFiles(Action<string> callProBack)
        {
            #region ISCCP原始数据
            if (callProBack!=null)
                callProBack("开始扫描" + _inputDir + "下的ISCCP数据...");
            string[] isccpFiles = Directory.GetFiles(_inputDir, "ISCCP.D2.*.GPC", SearchOption.AllDirectories);
            List<string> validIsccpf = new List<string>();
            if (isccpFiles.Length > 0)
            {
                FileInfo finfo;
                foreach (string file in isccpFiles)
                {
                    finfo = new FileInfo(file);
                    if (finfo.Length == 871000)
                        validIsccpf.Add(file);
                }
                if (callProBack != null) 
                    callProBack("扫描完成！");
                if (validIsccpf.Count > 0)
                {
                    return validIsccpf.ToArray();
                }
            }
            if (callProBack != null)
                callProBack("没有找到待处理的文件！");
            return null;
            #endregion
        }

        private void CheckISCCPFileContinuty(string[] isccpFiles,Action<string> callProBack)
        {
            if (isccpFiles == null || isccpFiles.Length < 1)
            {
                if (callProBack != null)
                    callProBack("当前路径不存在可检测的ISCCP D2数据！");
                return;
            }
            Dictionary<int, Dictionary<int, List<string>>> lostdataTime=null;
            if (!ISCCPProcess.ISCCPContinutyDetec(isccpFiles,callProBack, out lostdataTime))
            {
                if (lostdataTime == null || lostdataTime.Count==0)
                {
                    if (callProBack != null)
                        callProBack("数据时间连续！");
                    return;
                }
                if (callProBack != null)
                    callProBack("-----------------------------------");
                callProBack("ISCCP数据缺失信息");
                string yearmsg = "\t缺少{0}年的数据！";
                string monthmsg = "\t\t缺少{0}年{1}月的数据！";
                string utcTmsg = "\t\t\t缺少{0}年{1}月{2}UTC时的数据！";
                #region 入库
                if (true)
                {
                    Dictionary<string, object> field;                    
                    foreach (int y in lostdataTime.Keys)
                    {
                        if (lostdataTime[y] == null)//全年缺失
                        {
                            for (int i = 1; i < 12; i++)
                            {
                                field = new Dictionary<string, object>();
                                field.Add("Lostyear", y);
                                field.Add("Lostmonth", i);
                                field.Add("AllDayLostTip", true);
                                //if (dbConnect.IshasRecord(tableName, field) == false)
                                //_dbConnect.InsertISCCPDataTimeContinutyTable(y, i, null, true);
                            }
                        }
                        else
                        {
                            foreach (int mon in lostdataTime[y].Keys)
                            {
                                if (lostdataTime[y][mon] == null)//整月缺失
                                {
                                    field = new Dictionary<string, object>();
                                    field.Add("Lostyear", y);
                                    field.Add("Lostmonth", mon);
                                    field.Add("AllDayLostTip", true);
                                    //if (dbConnect.IshasRecord(tableName, field) == false)
                                    //_dbConnect.InsertISCCPDataTimeContinutyTable(y, mon, null, true);
                                }
                                else
                                {
                                    foreach (string utct in lostdataTime[y][mon])
                                    {
                                        field = new Dictionary<string, object>();
                                        field.Add("Lostyear", y);
                                        field.Add("Lostmonth", mon);
                                        field.Add("LostUTCTime", utct);
                                        //if (dbConnect.IshasRecord(tableName, field) == false)
                                        //_dbConnect.InsertISCCPDataTimeContinutyTable(y, mon, utct, false);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 输出提示到log文件
                foreach (int y in lostdataTime.Keys)
                {
                    if (lostdataTime[y] == null)//全年缺失
                    {
                        if (callProBack != null)
                            callProBack(string.Format(yearmsg, y));
                    }
                    else
                    {
                        foreach (int mon in lostdataTime[y].Keys)
                        {
                            if (lostdataTime[y][mon] == null)//整月缺失
                            {
                                if (callProBack != null)
                                    callProBack(string.Format(monthmsg, y, mon));
                            }
                            else
                            {
                                foreach (string utct in lostdataTime[y][mon])
                                {
                                    if (callProBack != null)
                                        callProBack(string.Format(utcTmsg, y, mon, utct));
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                if (callProBack != null)
                    callProBack("数据时间连续！");
                return;
            }
        }

        private string[] GetAIRSOriginDataFiles()
        {
            #region AIRS
            DirectoryInfo fatherPath = new DirectoryInfo(_inputDir);
            string[] AIRSFiles = Directory.GetFiles(_inputDir, "AIRS.*RetStd*.hdf", SearchOption.TopDirectoryOnly);
            if (AIRSFiles.Length > 0)
            {
                _originFiles2Base.Add("AIRS", AIRSFiles.ToList());
            }
            foreach (DirectoryInfo child in fatherPath.GetDirectories("*"))
            {
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
            return _originFiles2Base["AIRS"].ToArray();
            #endregion
        }

        private void CheckAIRSFileContinuty(string[] Files, Action<string> callProBack)
        {
            if (Files == null || Files.Length < 1)
            {
                callProBack("当前路径不存在可检测的AIRS数据！");
                return;
            }
            List<DateTime> lostdataTime;
            Dictionary<DateTime, List<int>> lostdataDayNO;
            DateTime timebegin, timeend;
            bool isContinuty = AIRSContinutyDetec(Files, out lostdataTime, out lostdataDayNO, out timeend, out timebegin);
            if (callProBack != null)
            {
                callProBack("AIRS数据缺失信息");
                callProBack("-----------------------------------");
                callProBack("文件夹：" + txtInDir.Text);
                callProBack("起始时间：" + timebegin.ToString("yyyy-MM-dd"));
                callProBack("结束时间：" + timeend.ToString("yyyy-MM-dd"));
                callProBack("-----------------------------------");
            }
            if (!isContinuty)
            {
                string outformat = "{0}\t{1}";
                //string daymessege = "提示：缺少{0}的AIRS数据文件!\r\n";
                //string NOmessege = "提示：缺少{0}的第{1}个AIRS数据文件!\r\n";
                if (lostdataTime.Count > 0)
                {
                    callProBack(string.Format(outformat, "缺失日期", "全天缺失"));
                    foreach (DateTime time in lostdataTime)
                    {
                        callProBack(string.Format(outformat, time.ToShortDateString(), "是"));
                        //if (cbxContiCheck2DB.Checked)
                        //    _dbConnect.InsertAIRSDataTimeContinutyTable(time, 0, true);
                    }
                }
                else if (callProBack != null)
                {
                    callProBack("数据日期连续！");
                    callProBack("-----------------------------------");
                }
                if (lostdataDayNO.Count > 0)
                {
                    callProBack(string.Format(outformat, "缺失日期", "缺失数据序号"));
                    foreach (DateTime dt in lostdataDayNO.Keys)
                    {
                        foreach (int lostno in lostdataDayNO[dt])
                        {
                            callProBack(string.Format(outformat, dt.ToShortDateString(), lostno));
                            //if (cbxContiCheck2DB.Checked)
                            //    _dbConnect.InsertAIRSDataTimeContinutyTable(dt, lostno, false);
                        }
                    }
                }
                else if (callProBack != null)
                {
                    callProBack("数据每天数据连续！");
                    callProBack("-----------------------------------");
                }
            }
            else if (callProBack != null)
            {
                callProBack("数据日期连续,每天数据连续！");
            }
        }

        private bool AIRSContinutyDetec(string[] AIRSfiles, out List<DateTime> lostdataDay, out Dictionary<DateTime, List<int>> lostdataDayNO, out DateTime maxtime, out DateTime mintime)
        {
            //AIRS.2012.01.01.001.L2.RetStd.v6.0.7.0.G12328075503.hdf
            maxtime = DateTime.MinValue;
            mintime = DateTime.MinValue;
            int year, month, day, dayNO;
            //List<DateTime> dataTime = new List<DateTime>();//记录所有数据的时间
            Dictionary<DateTime, List<int>> dayAndHourMin = new Dictionary<DateTime, List<int>>();
            lostdataDay = new List<DateTime>();//缺失数据的时间
            lostdataDayNO = new Dictionary<DateTime, List<int>>();
            List<int> dayallNO;
            foreach (string file in AIRSfiles)
            {
                string fName = Path.GetFileName(file).ToUpper();
                Match match = Regex.Match(fName, @".(?<year>\d{4}).(?<month>\d{2}).(?<day>\d{2}).(?<NO>\d{3}).");
                if (match.Success)
                {
                    year = Int32.Parse(match.Groups["year"].Value);
                    month = Int32.Parse(match.Groups["month"].Value);
                    day = Int32.Parse(match.Groups["day"].Value);
                    dayNO = Int32.Parse(match.Groups["NO"].Value);
                    DateTime date = new DateTime(year, month, day);
                    if (!dayAndHourMin.Keys.Contains(date))
                    {
                        dayallNO = new List<int>();
                        dayallNO.Add(dayNO);
                        dayAndHourMin.Add(date, dayallNO);
                    }
                    else
                    {
                        if (!dayAndHourMin[date].Contains(dayNO))
                        {
                            dayAndHourMin[date].Add(dayNO);
                        }
                    }
                }
                else
                {
                    LogFactory.WriteLine("AIRS", "文件" + file + "命名不规范，无法解析数据日期！");
                }
            }
            #region 时间连续性检测
            if (dayAndHourMin.Count > 0)
            {
                maxtime = dayAndHourMin.Keys.Max();
                mintime = dayAndHourMin.Keys.Min();
                DateTime midtime;
                #region 日期连续性检测
                if (mintime.Year == maxtime.Year)//同一年
                {
                    for (int i = 1; i < (maxtime.DayOfYear - mintime.DayOfYear); i++)
                    {
                        if (!dayAndHourMin.Keys.Contains(mintime.AddDays(i)))
                        {
                            lostdataDay.Add(mintime.AddDays(i));
                        }
                    }
                }
                else
                {
                    midtime = mintime;
                    for (int y = mintime.Year; y <= maxtime.Year; y++)
                    {
                        int daysofYyear = DateTime.IsLeapYear(y) ? 366 : 365;
                        int daystart;
                        if (y == mintime.Year)
                        {
                            daystart = mintime.DayOfYear;
                            for (int i = 1; i < daysofYyear - daystart; i++)
                            {
                                if (!dayAndHourMin.Keys.Contains(midtime.AddDays(i)))
                                {
                                    lostdataDay.Add(midtime.AddDays(i));
                                }
                            }
                        }
                        else
                        {
                            daystart = 1;
                            midtime = new DateTime(y, 1, 1);
                            for (int i = daystart; i < daysofYyear; i++)
                            {
                                if (!dayAndHourMin.Keys.Contains(midtime.AddDays(i)))
                                {
                                    lostdataDay.Add(midtime.AddDays(i));
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 每天的数据序号连续性检测
                int minNO, maxNO;
                foreach (DateTime dtime in dayAndHourMin.Keys)
                {
                    if (dayAndHourMin[dtime].Count > 1)//数据多于1个
                    {
                        maxNO = dayAndHourMin[dtime].Max();
                        minNO = dayAndHourMin[dtime].Min();
                        for (int i = minNO; i <= maxNO; i++)
                        {
                            if (!dayAndHourMin[dtime].Contains(i))
                            {
                                if (!lostdataDayNO.Keys.Contains(dtime))
                                {
                                    dayallNO = new List<int>();
                                    dayallNO.Add(i);
                                    lostdataDayNO.Add(dtime, dayallNO);
                                }
                                else
                                {
                                    if (!lostdataDayNO[dtime].Contains(i))
                                    {
                                        lostdataDayNO[dtime].Add(i);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
            if (lostdataDay.Count != 0 || lostdataDayNO.Count != 0)
            {
                return false;
            }
            return true;
        }

        private void CheckMOD06FileContinuty(string dir, Action<string> callProBack)
        {
            if (callProBack!=null)
                callProBack("正在扫描"+dir+"下的待检测MOD06文件...");
            string[] m06Files = Directory.GetFiles(dir, "MOD06_L2*.HDF", SearchOption.AllDirectories);
            foreach (string file in m06Files)
            {
                if (Path.GetFileNameWithoutExtension(file).Split('.').Length == 3)
                    continue;
                if (callProBack != null)
                    callProBack("\t标准化" + Path.GetFileName(file) + "文件名...");
                RegularFileNames(file);
            }
            if (callProBack != null)
                callProBack("正在扫描" + dir + "下的待检测MOD03文件...");
            string[] m03Files = Directory.GetFiles(dir, "MOD03*.HDF", SearchOption.AllDirectories);
            foreach (string file in m03Files)
            {
                if (Path.GetFileNameWithoutExtension(file).Split('.').Length == 3)
                    continue;
                if (callProBack != null)
                    callProBack("\t标准化" + Path.GetFileName(file) + "文件名...");
                RegularFileNames(file);
            }
            if (m06Files.Length == 0)
                if (callProBack != null)
                    callProBack(dir + "不存在MOD06_L2文件!");
            if (m03Files.Length == 0)
                if (callProBack != null)
                    callProBack(dir + "不存在MOD03文件!");
            if (m06Files.Length == 0 && m03Files.Length == 0)
                return;
            #region 检测文件是否可以打开为HDF4？
            //foreach (string m06f in m06Files)
            //{
            //    if (!GeoDo.HDF4.HDF4Helper.IsHdf4(m06f))
            //    {
            //        LogFactory.WriteLine("MOD06文件检测", "文件" + m06f + "不是HDF4文件或已损坏，无法进行处理！");
            //        txtErrorLog.Text += "文件" + m06f + "不是HDF4文件或已损坏！\r\n";
            //        lostMOD06.Add(m06f);
            //    }
            //}
            //foreach (string m03f in m03Files)
            //{
            //    if (!GeoDo.HDF4.HDF4Helper.IsHdf4(m03f))
            //    {
            //        LogFactory.WriteLine("MODIS连续性检测", "文件" + m03f + "不是HDF4文件或已损坏，无法进行处理！");
            //        txtErrorLog.Text += "文件" + m03f + "不是HDF4文件或已损坏！\r\n";
            //        lostMOD03.Add(m03f);
            //    }
            //}
            #endregion
            List<string> lostMOD06 = new List<string>();
            List<string> lostMOD03 = new List<string>();
            List<DateTime> datedays = new List<DateTime>();
            foreach (string item in m06Files)
            {
                DateTime dt = DataContinuityDetec.GetMOD06OribitTime(item);
                DateTime datatime = new DateTime(dt.Year, dt.Month, dt.Day);
                if (!datedays.Contains(datatime))
                    datedays.Add(datatime);
                string m03file = Path.Combine(Path.GetDirectoryName(item), Path.GetFileName(item).Replace("MOD06_L2", "MOD03"));
                if (!File.Exists(m03file))
                {
                    lostMOD03.Add(m03file);
                }
            }
            foreach (string item in m03Files)
            {
                DateTime dt = DataContinuityDetec.GetMOD06OribitTime(item);
                DateTime datatime = new DateTime(dt.Year, dt.Month, dt.Day);
                if (!datedays.Contains(datatime))
                    datedays.Add(datatime);
                string m06file = Path.Combine(Path.GetDirectoryName(item), Path.GetFileName(item).Replace("MOD03", "MOD06_L2"));
                if (!File.Exists(m06file))
                {
                    if (callProBack != null)
                        callProBack("\t文件：" + item + "缺少对应MOD06文件！");
                    lostMOD06.Add(m06file);
                }
            }
            #region 日期连续性
            List<DateTime> lostDays = new List<DateTime>();
            DateTime startdate = datedays.Min();
            DateTime enddate = datedays.Max();
            for (DateTime dt = startdate.AddDays(1); dt <= enddate; dt=dt.AddDays(1))
            {
                if (!datedays.Contains(dt))
                    lostDays.Add(dt);
            }
            //foreach (DateTime dataday in datedays)
            //{
            //    if (dataday != enddate && !datedays.Contains(dataday.AddDays(1)))
            //    {
            //        lostDays.Add(dataday.AddDays(1));
            //    }
            //}
            if (callProBack != null)
            {
                callProBack("-----------------------------------");
                callProBack("文件夹：" + dir);
                callProBack("起始日期：" + datedays.Min().ToShortDateString());
                callProBack("结束日期：" + datedays.Max().ToShortDateString());
            }

            if (lostDays.Count >0)
            {
                if (callProBack != null)
                {
                    callProBack("-----------------------------------");
                    callProBack("\tMOD03及MOD06全部缺失：");
                    callProBack("\t缺失日期");
                }
                foreach (DateTime day in lostDays)
                {
                    if (callProBack != null)
                        callProBack("\t" + day.ToShortDateString());
                   //callProBack("缺失" + day.ToShortDateString()+"的数据!");
                }
            }
            else
            {
                if (callProBack != null)
                    callProBack("数据日期连续！");
            }
            #endregion
            if (callProBack != null)
                callProBack("-----------------------------------");
            #region 0603匹配
            if (lostMOD06.Count>0||lostMOD03.Count>0)
            {
                if (callProBack != null)
                {
                    callProBack("MOD06与MOD03匹配缺失：");
                }
                string outformat = "\t{0}\t{1}\t\t{2}";
                if (callProBack != null)
                    callProBack(string.Format(outformat, "缺失类型", "缺失时间", "文件名称"));
                string timeformat = "{0} {1}:{2}";
                string time;
                foreach (string m06 in lostMOD06)
                {
                    DateTime dt = DataContinuityDetec.GetMOD06OribitTime(m06);
                    time = string.Format(timeformat, String.Format("{0, -10}", dt.ToShortDateString()), dt.Hour, dt.Minute);
                    callProBack(string.Format(outformat, "MOD06", time, m06));
                    //callProBack("缺失MOD06文件：" + m06+"!");
                    //if (cbxContiCheck2DB.Checked&&!_dbConnect.IshasRecord("cp_mod06rawdatatimecontinuty_tb", "LostDate", dt, "Keywords", "MOD06"))
                    //{
                    //    _dbConnect.InsertMOD06DataTimeContinutyTable(dt, "MOD06");
                    //}
                }
                foreach (string m03 in lostMOD03)
                {
                    DateTime dt = DataContinuityDetec.GetMOD06OribitTime(m03);
                    time = string.Format(timeformat, String.Format("{0, -10}", dt.ToShortDateString()), dt.Hour, dt.Minute);
                    callProBack(string.Format(outformat, "MOD03", time, m03));
                    //callProBack("缺失MOD03文件：" + m03 + "!");
                    //if (cbxContiCheck2DB.Checked && !_dbConnect.IshasRecord("cp_mod06rawdatatimecontinuty_tb", "LostDate", dt, "Keywords", "MOD03"))
                    //{
                    //    _dbConnect.InsertMOD06DataTimeContinutyTable(dt, "MOD03");
                    //}
                }
            }
            else
            {
                if (callProBack != null)
                    callProBack("数据MOD06与MOD03完全匹配！");
            }
            #endregion
        }

        public static string  RegularFileNames(string file)
        {
            string nfname = Path.GetFileNameWithoutExtension(file);
            string[] parts = nfname.Split('.');
            if (parts.Length == 3)
                return file;
            string nameformat = "{0}.{1}.{2}.hdf";
            string newfname = string.Format(nameformat, parts[0], parts[1], parts[2]);
            String newName = Path.Combine(Path.GetDirectoryName(file), newfname);
            if (File.Exists(newName))
                File.Delete(newName);
            System.IO.File.Move(file, newName);
            return newName;
        }

        #endregion 
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            if (runTaskThread != null)
                runTaskThread.Abort();
            Close();
        }

        private bool CheckArgsIsOk()
        {
            if (string.IsNullOrEmpty(_inputDir) || !Directory.Exists(_inputDir))
            {
                MessageBox.Show("请正确选择文件所在文件夹！");
                return false;
            }
            if (!rdbISCCP.Checked && !rdbMODIS.Checked && !rdbAIRS.Checked)
            {
                MessageBox.Show("请选择数据类型！");
                return false;
            }
            if (cbxPrdsLevl.SelectedIndex != 0 && cbxPrdsLevl.SelectedIndex != 1)
            {
                MessageBox.Show("请选择数据级别！");
                return false;
            }
            return true;
        }

        private void btnOpenOutputLog_Click(object sender, EventArgs e)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\Log\"+ DateTime.Now.ToString("yyyyMMdd")) ;
            string append = string.Format(".DataProcess.{0}.log", DateTime.Now.ToString("yyyyMMdd"));
            if(_log==null)
            {
                string log="";
                if (rdbISCCP.Checked)
                    log = string.Format(_lostlog, "ISCCP") + append;
                else if (rdbMODIS.Checked)
                    log = string.Format(_lostlog, "MODIS") + append;
                else if (rdbAIRS.Checked)
                    log = string.Format(_lostlog, "AIRS") + append;
                if (File.Exists(Path.Combine(dir,log)))
                    Process.Start(Path.Combine(dir,log));
            }
            else
            {
                if (File.Exists(Path.Combine(dir, _log + append)))
                    Process.Start(Path.Combine(dir, _log + append));
            }
        }

    }
}
