using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Text.RegularExpressions;
using GeoDo.RSS.BlockOper;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class AIRSDataProcesser
    {
        private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
        public string _historyPrjDir = null;
        private List<string> _allMosaicPaths = new List<string>();   //所有日拼接文件所在文件夹全路径
        private string _inputDir;
        private string _outputDir;        //界面设置的输出目录
        private List<string> _allPaths=new List<string>();   //所有输出文件所在文件夹全路径
        private string[] _dataSets;//选取的需要投影的数据集
        private PrjEnvelopeItem[] _prjEnvelopes;
        private float _resl;
        private bool _overlapPrj = false;
        private bool _overlapMosaic = false;
        static ConnectMySqlCloud _dbCon =null;
        string _projectLog = "AIRSProjectError";
        DataProcesser _pro = new DataProcesser();
        private bool _isOriginResl = true;
        private bool _isDirectMosaic = false;
        private bool _isOnlyPrj = false;

        public AIRSDataProcesser(string inputDir, string outputDir, string[] dataSets, float resl, PrjEnvelopeItem[] prjEnvelopes, bool overlapPrj, bool overlapMosaic, bool isdirectmosaic = false, bool isonlyprj = false)
        {
            _inputDir = inputDir;
            _outputDir = outputDir;
            _pro._outputDir = _outputDir;
            _dataSets = dataSets;//选择的数据集
            _resl = resl;
            _prjEnvelopes = prjEnvelopes;
            _overlapPrj = overlapPrj;
            _overlapMosaic = overlapMosaic;
            _isDirectMosaic = isdirectmosaic;
            _isOnlyPrj = isonlyprj;
            if (File.Exists(_dataBaseXml))
            {
                _dbCon = new ConnectMySqlCloud();
            }
            else
            {
                MessageBox.Show("无法读取数据库配置文件,请先设置数据库连接配置文件！");
            }
        }

        public bool IsOriginResl
        {
            get
            {
                return _isOriginResl;
            }
            set
            {
                _isOriginResl = value;
            }
        }

        public bool Process(Action<int, string> progressCallback)
        {
            if (progressCallback != null)
                progressCallback(-1, "输出路径：" + _outputDir);
            if (progressCallback != null)
                progressCallback(1, "开始检查待处理文件匹配...");
            string[] files = CheckFiles(progressCallback);
            if (files == null || files.Length < 1)
            {
                if (progressCallback != null)
                    progressCallback(0, "当前路径下不存在可处理的AIRS文件，请重新选择！");
                return false;
            }
            if (progressCallback != null)
                progressCallback(-1, "当前路径下共"+files.Length+"个待处理AIRS文件。");
            if (_isOnlyPrj)//只投影；
            {
                if (progressCallback != null)
                    progressCallback(5, "完成待处理匹配检查,开始投影处理...");
                DoProject(files, progressCallback);
                if (progressCallback != null)
                    progressCallback(100, "文件投影结束！");
                return true;
            }
            if (!_isDirectMosaic)//先投影后拼接；
            {
                if (progressCallback != null)
                    progressCallback(5, "完成待处理匹配检查,开始投影处理...");
                DoProject(files, progressCallback);
                if (progressCallback != null)
                    progressCallback(75, "文件投影结束，开始拼接生成日产品...");
            }
            else//历史投影文件直接拼接
            {
                if (progressCallback != null)
                    progressCallback(5, "检查待处理生成的历史投影文件开始...");
                AddGranuleDir2Mosaic(files, progressCallback);
                if (progressCallback != null)
                    progressCallback(75, "历史投影文件开始拼接生成日产品...");
            }
            Moasic(progressCallback);
            if (progressCallback != null)
                progressCallback(100, "文件预处理结束！");
            return true;
        }

        #region 检索待处理文件
        //检索待处理文件,输出缺失信息
        private string[] CheckFiles(Action<int, string> progressCallback)
        {
            if (progressCallback != null)
                progressCallback(-1, "检索AIRS文件...");
            string[] AIRSFiles = Directory.GetFiles(_inputDir, "AIRS*L2.RetStd*.HDF", SearchOption.AllDirectories);
            List<string> airsf = AIRSFiles.ToList();
            //for (int i = 0; i < AIRSFiles.Length; i++)
            //{
            //    if (!GeoDo.HDF4.HDF4Helper.IsHdf4(AIRSFiles[i]))
            //    {
            //        airsf.Remove(AIRSFiles[i]);
            //        progressCallback(-1, "文件" + AIRSFiles[i] + "不是HDF4文件，无法进行处理！");
            //    }
            //}
            return airsf.ToArray();
        }

        #endregion

        #region 投影处理

        private void QueryIDs(string[] dataSets)
        {
            long setID, prdsID;
            double selfFillValue, dayFillValue, dayInvalidValue;
            string set;
            foreach (string setName in dataSets)
            {
                set = setName.Replace("_", "");
                if (_dbCon.QueryDatasetsID("AIRS", set, out prdsID, out setID))
                {
                    _pro._datasetsIDs.Add(set, setID);
                    _pro._datasetsPrdsIDs.Add(set, prdsID);
                }
                if (_dbCon.QueryDatasetsInvalidValue("AIRS", set, out selfFillValue, out dayFillValue, out dayInvalidValue))
                {
                    _pro._selfFillValue.Add(set, selfFillValue);
                    _pro._dayFillValue.Add(set, dayFillValue);
                    _pro._dayInvalidValue.Add(set, dayInvalidValue);
                }
            }
            long regionID;
            foreach (PrjEnvelopeItem prjItem in _prjEnvelopes)
            {
                if (!_dbCon.IshasDataRegionRecord(prjItem, out  regionID))//!_dbCon.IshasRecord(tableName, "regionName", prjItem.Name.ToLower()))
                {
                    regionID = _dbCon.InsertRegionTable(prjItem.Name.ToLower(), prjItem.PrjEnvelope.MinX, prjItem.PrjEnvelope.MaxX, prjItem.PrjEnvelope.MinY, prjItem.PrjEnvelope.MaxY);
                }
                _pro._regionIDs.Add(prjItem.Name, regionID);
            }
        }

        private void DoProject(string[] files, Action<int, string> progressCallback)
        {
            List<string> sets = new List<string>();
            foreach (string set in _dataSets)
            {
                if (!string.IsNullOrEmpty(set))
                    sets.Add(set);
            }
            if (sets.Count==0)
            {
                progressCallback(-5,"请选择有效的数据集！");
                return;
            }
            string[] SelectedSets = sets.ToArray();//获取所选数据集中的可处理数据集
            if (progressCallback != null)
                progressCallback(-1, "链接数据库，查询数据集ID开始...");
            QueryIDs(SelectedSets);
            if (progressCallback != null)
                progressCallback(-1, "查询数据集ID结束！");
            if (_prjEnvelopes != null && _prjEnvelopes.Count() > 0)
            {
                StringBuilder outDirBuilder = new StringBuilder(200);
                StringBuilder outFilenameBuilder = new StringBuilder(400);
                string outDir = string.Empty;
                string dataStr = string.Empty;
                string dnLabel,setName,outFilename;
                //string subDir = "5分钟段产品\\{0}\\AQUA\\AIRS\\{1}\\{2}\\{3}\\{4}\\{5}";
                float resl = _isOriginResl ? 1.0f : _resl;
                foreach (PrjEnvelopeItem prjItem in _prjEnvelopes)
                {
                    if (progressCallback != null) 
                        progressCallback(-1, prjItem.Name + "投影区域投影开始...");
                    int count = files.Length;
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (progressCallback != null)
                        {
                            int pct = 6 + (int)(i / (count * 1.0f) * (75 - 6));
                            progressCallback(pct, "共" + files.Length + "个待投影文件，开始投影第" + (i + 1) + "个...");
                        }
                        DateTime dt = GetOribitTime(files[i], out dataStr);
                        if ( dt == DateTime.MinValue||dataStr == "" )
                        {
                            if (progressCallback != null)
                                progressCallback(-1, "\t获取文件名中的时间信息失败！" + files[i]);
                            continue;
                        }
                        //根据文件名中的时间信息确定其归属于白天或夜间文件夹
                        DateTime timeparts = dt.Date.AddHours(9);
                        dnLabel = (dt.CompareTo(timeparts) <= 0) ? "day" : "night";
                        foreach (string item in SelectedSets)
                        {
                            //if (progressCallback != null) 
                            //    progressCallback(-1, "数据集" + item + "投影开始...");
                            setName = item.Replace("_", "");
                            //outDir = Path.Combine(_outputDir, string.Format(subDir, setName, dt.Year, dt.Month, dt.Day, dnLabel, resl));//GetDatasetOutDir(outDirs, item);
                            //outFilename = Path.Combine(outDir, string.Format("{0}_{1}_{2}_granule_{3}_{4}{5}", setName, "AIRS", prjItem.Name, dataStr, resl.ToString("f2"), ".LDF"));
                            outDirBuilder.Clear();
                            outDir = outDirBuilder.Append(Path.Combine(_outputDir, "5分钟段产品\\")).Append(setName).Append("\\AQUA\\AIRS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl).ToString();
                            outFilenameBuilder.Clear();
                            outFilename = outFilenameBuilder.Append(outDir + "\\").Append(setName).Append("_AIRS_").Append(prjItem.Name).Append("_granule_").Append(dataStr + "_").Append(resl.ToString("f2") + ".LDF").ToString();
                            if (progressCallback != null)
                                progressCallback(-1, "\t\t正在投影" + item + "数据集,文件：" + outFilename);
                            if (File.Exists(outFilename))
                            {
                                if (_overlapPrj == false)
                                {
                                    if (progressCallback != null)
                                        progressCallback(-1, "\t\t\t投影文件已存在,跳过！");
                                    if (!_allPaths.Contains(outDir))
                                        _allPaths.Add(outDir);
                                    continue;
                                }
                                else
                                {
                                    File.Delete(outFilename);
                                    if (progressCallback != null)
                                        progressCallback(-1, "\t\t\t删除已存在的投影文件成功！");
                                }
                            }
                            if (!Directory.Exists(outDir))
                                DataProcesser.TryCreateDstDir(outDir, progressCallback);
                            if (!_allPaths.Contains(outDir))
                                _allPaths.Add(outDir);
                            if (_pro.Project(prjItem, item, files[i], files[i], outFilename, resl, dataStr, _projectLog, _overlapPrj, "AIRS"))
                            {
                                if (progressCallback != null)
                                    progressCallback(-1, "\t\t\t\t成功！投影处理成功！");
                            }
                            else
                                if (progressCallback != null)
                                    progressCallback(-1, "\t\t\t\t失败!投影处理出错！");
                        }
                        if (progressCallback != null)
                            progressCallback(-1, files[i] + "投影结束！");
                        if (progressCallback != null)
                            progressCallback(-1, "-----------------------------------------------");
                    }
                    if (progressCallback != null) 
                        progressCallback(-1, prjItem.Name + "区域投影结束！");
                }
                return;
            }
            if (progressCallback != null)
                progressCallback(-1, "没有可用的投影区域！");
        }
        #endregion

        #region 历史投影文件直接拼接
        private void AddGranuleDir2Mosaic(string[] files, Action<int, string> progressCallback)
        {
            string[] SelectedSets = _dataSets;
            if (progressCallback != null)
                progressCallback(6, "链接数据库，查询数据产品类别...");
            QueryIDs(SelectedSets);
            if (_prjEnvelopes != null && _prjEnvelopes.Count() > 0)
            {
                //string subDir = "5分钟段产品\\{0}\\AQUA\\AIRS\\{1}\\{2}\\{3}\\{4}\\{5}";
                float resl = _isOriginResl ? 1.0f : _resl;
                #region
                string infilename, infiledir, outDir, dataStr, dnLabel;
                StringBuilder outDirbuilder = new StringBuilder(200);
                for (int i = 0; i < files.Length; i++)
                {
                    infilename = Path.GetFileName(files[i]);
                    infiledir = Path.GetDirectoryName(files[i]);
                    DateTime dt = GetOribitTime(files[i], out dataStr);
                    if (dt == DateTime.MinValue|| dataStr == "" )
                    {
                        if (progressCallback != null)
                            progressCallback(-1, "获取文件名中的时间信息失败！");
                        continue;
                    }
                    DateTime timeparts = dt.Date.AddHours(9);
                    dnLabel = (dt.CompareTo(timeparts) <= 0) ? "day" : "night";//根据文件名中的时间信息确定其归属于白天或夜间文件夹
                    foreach (string item in SelectedSets)
                    {
                        //outDir = Path.Combine(_historyPrjDir, string.Format(subDir, item.Replace("_", ""), dt.Year, dt.Month, dt.Day, dnLabel, resl));//GetDatasetOutDir(outDirs, item);
                        outDirbuilder.Clear();
                        outDir = outDirbuilder.Append(Path.Combine(_historyPrjDir,"5分钟段产品\\")).Append(item.Replace("_", "")).Append("\\AQUA\\AIRS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl).ToString();
                        if (Directory.Exists(outDir) && !_allPaths.Contains(outDir))
                        {
                            _allPaths.Add(outDir);
                            if (progressCallback != null)
                                progressCallback(-1, outDir + "加至拼接路径成功！");
                        }
                    }
                }
                #endregion
            }
        }
        #endregion

        #region 拼接处理
        private void Moasic(Action<int, string> progressCallback)
        {
            //搜索每个生成目录，目录下有相同时间的文件进行拼接
            int count = 0;
            foreach (string dir in _allPaths)
            {
                if (progressCallback != null)
                {
                    int pct = 75 + (int)(count * 1.0f / _allPaths.Count * (99 - 75));
                    progressCallback(pct, "共" + _allPaths.Count + "个待拼接文件，开始拼接第" + (++count) + "个...");
                }
                if (!Directory.Exists(dir))
                {
                    if (progressCallback != null)
                        progressCallback(-1, dir + "不存在！");
                    continue;
                }
                string[] files = Directory.GetFiles(dir, "*AIRS*granule*.ldf");
                if (files.Length == 0)
                {
                    if (progressCallback != null)
                        progressCallback(-1, dir + "不存在可处理的AIRS投影文件！");
                    continue;
                }
                Dictionary<string, List<string>> sortFiles = _pro.SortFilesByRegion(files);
                foreach (string key in sortFiles.Keys)
                {
                    if (progressCallback != null)
                        progressCallback(-1, key + "区域投影文件拼接开始...");
                    foreach (PrjEnvelopeItem prj in _prjEnvelopes)
                    {
                        if (prj.Name == key)
                        {
                            _pro.DoMoasic(prj, sortFiles[key].ToArray(), progressCallback, _outputDir, _overlapMosaic, "AIRS");
                        }
                    }
                }
            }
            if (progressCallback != null)
                progressCallback(99, "文件拼接结束，开始生成拼接日志...");
            _pro.MosaicCheckAndOutPutLog(progressCallback,"AIRS", "AIRSMosaicLog.txt");
        }

        public static RasterProject.PrjEnvelope GetAIRSFileEnv(string fileName)
        {
            RasterProject.PrjEnvelope env = new RasterProject.PrjEnvelope(-180,180,-90,90);
            try
            {
                string xmlf = fileName + ".xml";
                if (File.Exists(xmlf))
                {
                    float WestBoundingCoordinate, NorthBoundingCoordinate, EastBoundingCoordinate, SouthBoundingCoordinate;
                    XElement xml = XElement.Load(xmlf);
                    if (xml == null)
                        return env;
                    IEnumerable<XElement> elements = xml.Elements();
                    foreach (XElement ele in elements)
                    {
                        if (ele == null || ele.IsEmpty || ele.Name != "SpatialDomainContainer")
                            continue;
                        IEnumerable<XElement> subelements = ele.Elements();
                        foreach (XElement xel in subelements)
                        {
                            if (xel == null || xel.IsEmpty || xel.Name != "HorizontalSpatialDomainContainer")
                                continue;
                            {
                                IEnumerable<XElement> subsubelements = xel.Elements();
                                foreach (XElement subxel in subsubelements)
                                {
                                    if (subxel == null || subxel.IsEmpty || subxel.Name != "BoundingRectangle")
                                        continue;
                                    XElement BoundingRectangleX = subxel;
                                    if (!BoundingRectangleX.IsEmpty)
                                    {
                                        XElement WestBoundingCoordinateX = BoundingRectangleX.Element("WestBoundingCoordinate");
                                        WestBoundingCoordinate = float.Parse(WestBoundingCoordinateX.Value);
                                        XElement NorthBoundingCoordinateX = BoundingRectangleX.Element("NorthBoundingCoordinate");
                                        NorthBoundingCoordinate = float.Parse(NorthBoundingCoordinateX.Value);
                                        XElement EastBoundingCoordinateX = BoundingRectangleX.Element("EastBoundingCoordinate");
                                        EastBoundingCoordinate = float.Parse(EastBoundingCoordinateX.Value);
                                        XElement SouthBoundingCoordinateX = BoundingRectangleX.Element("SouthBoundingCoordinate");
                                        SouthBoundingCoordinate = float.Parse(SouthBoundingCoordinateX.Value);
                                        return new RasterProject.PrjEnvelope(WestBoundingCoordinate, EastBoundingCoordinate, SouthBoundingCoordinate, NorthBoundingCoordinate);
                                    }
                                }
                            }
                        }
                    }
                }
                return env;
            }
            catch (System.Exception ex)
            {
                return env;
            }
        }

        public static DateTime GetOribitTime(string fileName, out string orbitTime, Action<string> ProBack=null)
        {
            orbitTime = "";
            string xmlf = fileName + ".xml";
            string fname = Path.GetFileNameWithoutExtension(fileName);
            try
            {
                if (File.Exists(xmlf))
                {
                    string rangeBeginningDate = "", rangeBeginningTime = "";
                    DateTime orbitdate = DateTime.MaxValue;
                    XElement xml = XElement.Load(xmlf);
                    XElement rangeDateTimeX = xml.Element("RangeDateTime");
                    if (!rangeDateTimeX.IsEmpty)
                    {
                        XElement rangeBeginningDateX = rangeDateTimeX.Element("RangeBeginningDate");
                        rangeBeginningDate = rangeBeginningDateX.Value;
                        XElement rangeBeginningTimeX = rangeDateTimeX.Element("RangeBeginningTime");
                        rangeBeginningTime = rangeBeginningTimeX.Value.Remove(8);
                        orbitdate = Convert.ToDateTime(rangeBeginningDate +" "+ rangeBeginningTime);
                        orbitTime = orbitdate.ToString("yyyyMMddHHmm"); //+datematch.Groups["num"].Value;
                        return orbitdate;
                    }
                    throw new ArgumentException("XML文件存在但没有可用的时间信息！");
                }
                else
                {
                    return GetAIRSDateTimeFromStr(fname, out orbitTime);
                }
            }
            catch (System.Exception ex)
            {
                if (ProBack != null)
                    ProBack("通过XML文件解析数据时间信息失败:"+ex.Message);
                try
                {
                    return GetAIRSDateTimeFromStr(fname, out orbitTime);
                }
                catch (System.Exception exx)
                {
                    if (ProBack != null)
                        ProBack("通过文件名解析数据时间信息失败:" + ex.Message);
                    return DateTime.MinValue;
                }
            }
        }

        public static DateTime GetAIRSDateTimeFromStr(string fname,out string orbitTime)
        {
            orbitTime = "";
            Match datematch = Regex.Match(fname, @".(?<year>\d{4}).(?<month>\d{2}).(?<day>\d{2}).(?<num>\d{3})");
            if (datematch.Success)
            {
                int year = Int32.Parse(datematch.Groups["year"].Value);
                int month = Int32.Parse(datematch.Groups["month"].Value);
                int day = Int32.Parse(datematch.Groups["day"].Value);
                DateTime date = new DateTime(year, month, day);
                int min = Int32.Parse(datematch.Groups["num"].Value) * 6;
                date = date.AddMinutes(min);
                orbitTime = date.ToString("yyyyMMddHHmm"); //+datematch.Groups["num"].Value;
                return date;
            }
            return DateTime.MinValue;
        }

        #endregion

        public static bool AIRSContinutyDetec(string[] AIRSfiles, out List<DateTime> lostdataDay,out Dictionary<DateTime,List<int>> lostdataDayNO)
        {
            //AIRS.2012.01.01.001.L2.RetStd.v6.0.7.0.G12328075503.hdf
            int year, month, day,dayNO;
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
                    DateTime date = new DateTime(year,month,day);
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
                DateTime maxtime = dayAndHourMin.Keys.Max();
                DateTime mintime = dayAndHourMin.Keys.Min();
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
                        for (int i = minNO; i <= maxNO;i++ )
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
            if (lostdataDay.Count != 0 || lostdataDayNO.Count!=0)
            {
                return false;
            }
            return true;
        }

        public static void DayMergePrdsUnContinuty2Base(string setName, Dictionary<string, List<DateTime>> lostMosaicDayPrds)
        {
            string sat = "AQUA", sensor = "AIRS",region;
            ConnectMySqlCloud con = new ConnectMySqlCloud();
            long prdID;
            if (con.QueryPrdID(sensor,setName, out prdID))
            {
                foreach (string reg in lostMosaicDayPrds.Keys)
                {
                    region = reg;
                    foreach (DateTime da in lostMosaicDayPrds[reg])
                    {
                        con.InsertPrdsDataTimeContinutyTable(prdID, setName,da, sat, sensor, region);
                    }
                }
            }
        }
    }
}
