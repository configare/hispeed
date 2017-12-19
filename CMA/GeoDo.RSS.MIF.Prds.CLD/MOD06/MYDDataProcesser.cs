using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Drawing;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Text.RegularExpressions;
using GeoDo.RSS.BlockOper;
using System.Windows.Forms;
using System.Diagnostics;
using GeoDo.Tools;
using System.Threading;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class MYDDataProcesser
    {
        private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
        private string _inputDir;
        public string _historyPrjDir = null;
        public string _outputDir;        //设置的输出目录
        private List<string> _allPaths=new List<string>();   //所有输出文件所在文件夹全路径
        private List<string> _allMosaicPaths = new List<string>();   //所有日拼接文件所在文件夹全路径
        private string[] _dataSets;
        private PrjEnvelopeItem[] _prjEnvelopes;
        private string[] _dataset5kmList = new string[] { "Cloud_Top_Pressure", "Cloud_Top_Pressure_Day", "Cloud_Top_Pressure_Night", "Cloud_Top_Temperature", "Cloud_Top_Temperature_Day", "Cloud_Top_Temperature_Night", "Cloud_Fraction", "Cloud_Fraction_Day", "Cloud_Fraction_Night", "Cloud_Phase_Infrared", "Cloud_Phase_Infrared_Day", "Cloud_Phase_Infrared_Night", "Cloud_Mask_5km" };
        private string[] _dataset1kmList = new string[] { "Cloud_Effective_Radius","Cloud_Optical_Thickness","Cloud_Water_Path_1621","Cloud_Effective_Radius_1621",
            "Effective_Radius_Difference","Cloud_Water_Path","Cloud_Optical_Thickness_1621", "Cloud_Phase_Optical_Properties","Cloud_Multi_Layer_Flag", 
            "Cirrus_Reflectance_Flag", "Cirrus_Reflectance", "Cloud_Mask_1km",   "Quality_Assurance_1km", 
            "Cloud_Water_Path_Uncertainty_1621","Cloud_Optical_Thickness_Uncertainty_1621","Cloud_Effective_Radius_Uncertainty_1621",
            "Cloud_Water_Path_Uncertainty","Cloud_Optical_Thickness_Uncertainty","Cloud_Effective_Radius_Uncertainty"};
        private bool _overlapPrj =false;
        private bool _overlapMosaic = false;
        static ConnectMySqlCloud _dbCon;
        string _projectLog = "MYD06ProjectError";
        public Dictionary<string, long> _datasetsIDs = new Dictionary<string, long>();
        public Dictionary<string, long> _datasetsPrdsIDs = new Dictionary<string, long>();
        public Dictionary<string, long> _regionIDs = new Dictionary<string, long>();
        public Dictionary<string, double> _selfFillValue = new Dictionary<string, double>();
        public Dictionary<string, double> _dayFillValue = new Dictionary<string, double>();
        public Dictionary<string, double> _dayInvalidValue = new Dictionary<string, double>();
        private float _resl;
        private bool _isOriginResl = true;
        private bool _isDirectMosaic = false;
        private bool _isOnlyPrj = false;
        private bool _isLostAdded = false;

        public MYDDataProcesser()
        {
            if (File.Exists(_dataBaseXml))
                _dbCon = new ConnectMySqlCloud();
            else
                MessageBox.Show("无法读取数据库配置文件,请先设置数据库连接配置文件！");
        }

        public MYDDataProcesser(string inputDir, string outputDir, string[] dataSets, float resl, PrjEnvelopeItem[] prjEnvelopes, bool overlapPrj, bool overlapMosaic, bool isdirectmosaic = false, bool isonlyprj = false, bool isLostAdded = false)
        {
            _inputDir = inputDir;
            _outputDir = outputDir;
            _dataSets = dataSets;
            _resl = resl;
            _prjEnvelopes = prjEnvelopes;
            _overlapPrj = overlapPrj;
            _overlapMosaic = overlapMosaic;
            _isDirectMosaic = isdirectmosaic;
            _isOnlyPrj = isonlyprj;
            _isLostAdded = isLostAdded;
            if (File.Exists(_dataBaseXml))
                _dbCon = new ConnectMySqlCloud();
            else
                MessageBox.Show("无法读取数据库配置文件,请先设置数据库连接配置文件！");
        }

        public bool IsOriginResl
        {
            get{return _isOriginResl;}
            set
            {
                _isOriginResl = value;
            }
        }

        public bool DoProcess(Action<int, string> progressCallback)
        {
            if (progressCallback != null)
                progressCallback(-1, "输出路径：" + _outputDir);
            if (progressCallback != null)
                progressCallback(1, "开始检查待处理文件匹配...");
            string[] files = CheckFiles(progressCallback);
            if (files == null || files.Length < 1)
            {
                if (progressCallback != null)
                    progressCallback(0, "当前路径下不存在可处理的MYD06_L2文件，请重新选择！");
                return false;
            }
            if (_isOnlyPrj)//只投影；
            {
                if (progressCallback != null)
                    progressCallback(5, "完成待处理匹配检查,开始投影处理计算...");
                DoProject(files, progressCallback);
                //MRTSDoProject(files, progressCallback);
                if (progressCallback != null)
                    progressCallback(100, "文件投影结束！");
                return true;
            }
            if (!_isDirectMosaic)//先投影后拼接；
            {
                if (progressCallback != null)
                    progressCallback(5, "完成待处理匹配检查,开始投影处理计算...");
                DoProject(files, progressCallback);
                //TryReProject(progressCallback);
                //MRTSDoProject(files, progressCallback);
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
            TryDeleteProjectErrorFiles(progressCallback);
            Moasic(progressCallback);
            if (progressCallback != null)
                progressCallback(100, "文件预处理结束！");
            return true;
        }

        #region 检索待处理文件
        private string[] CheckFiles(Action<int, string> progressCallback)
        {
            string file;
            if (progressCallback != null)
                progressCallback(-1, "检索MYD06_L2文件...");
            string[] rawm06Files = Directory.GetFiles(_inputDir, "MYD06_L2*.HDF", SearchOption.AllDirectories);
            if (rawm06Files == null || rawm06Files.Length < 1)
                return null;
            List<string> newm06FilesList = new List<string>();
            string newName;
            foreach (string originfile in rawm06Files)
            {
                if (Path.GetFileNameWithoutExtension(originfile).Split('.').Length == 3)
                {
                    if (!newm06FilesList.Contains(originfile))
                        newm06FilesList.Add(originfile);
                    continue;
                }
                newName = DataContinuityCheck.RegularFileNames(originfile);
                if (!newm06FilesList.Contains(newName))
                    newm06FilesList.Add(newName);
            }
            string[] m06Files = newm06FilesList.ToArray();
            List<string> mod06f = m06Files.ToList();
            if (progressCallback != null)
                progressCallback(-1, "检索MYD03文件...");
            string[] rawm03Files = Directory.GetFiles(_inputDir, "MYD03*.HDF", SearchOption.AllDirectories);
            foreach (string originfile in rawm03Files)
            {
                if (Path.GetFileNameWithoutExtension(originfile).Split('.').Length == 3)
                    continue;
                DataContinuityCheck.RegularFileNames(originfile);
            }
            List<string> m06list = new List<string>();
            //List<string> m03list = new List<string>();
            progressCallback(-1, "MYD06文件匹配MYD03开始...");
            if (mod06f != null && mod06f.Count > 0)
            {
                foreach (string item in mod06f)
                {
                    file = Path.Combine(Path.GetDirectoryName(item), Path.GetFileName(item).Replace("MYD06_L2", "MYD03"));
                    if (!File.Exists(file))
                    {
                        progressCallback(-1, "文件" + item + "缺少对应的MYD03数据文件，无法进行1km分辨率数据集处理！");
                        continue;
                    }
                    else
                    {
                        m06list.Add(item);
                        //m03list.Add(file);
                    }
                }
            }
            return mod06f.ToArray();
        }
        #endregion

        #region 投影
        private void QueryIDs(string[] dataSets1km, string[] dataSets5km, Action<int, string> progressCallback=null)
        {
            long setID, prdsID;
            double selfFillValue, dayFillValue, dayInvalidValue;
            string set;
            foreach (string setName in dataSets1km)
            {
                set = setName.Replace("_", "");
                if (_dbCon.QueryDatasetsID("MYD06", set, out prdsID, out setID))
                {
                    _datasetsIDs.Add(set, setID);
                    _datasetsPrdsIDs.Add(set, prdsID);
                }
                if (_dbCon.QueryDatasetsInvalidValue("MYD06", set, out selfFillValue, out dayFillValue, out dayInvalidValue))
                {
                    _selfFillValue.Add(set, selfFillValue);
                    _dayFillValue.Add(set, dayFillValue);
                    _dayInvalidValue.Add(set, dayInvalidValue);
                }

            }
            foreach (string setName in dataSets5km)
            {
                set = setName.Replace("_", "");
                if (_dbCon.QueryDatasetsID("MYD06", set, out prdsID, out setID))
                {
                    _datasetsIDs.Add(set, setID);
                    _datasetsPrdsIDs.Add(set, prdsID);
                }
                if (_dbCon.QueryDatasetsInvalidValue("MYD06", set, out selfFillValue, out dayFillValue, out dayInvalidValue))
                {
                    _selfFillValue.Add(set, selfFillValue);
                    _dayFillValue.Add(set, dayFillValue);
                    _dayInvalidValue.Add(set, dayInvalidValue);
                }
            }
            long regionID;
            foreach (PrjEnvelopeItem prjItem in _prjEnvelopes)
            {
                if (!_dbCon.IshasDataRegionRecord(prjItem, out  regionID))//!_dbCon.IshasRecord(tableName, "regionName", prjItem.Name.ToLower()))
                {
                    regionID = _dbCon.InsertRegionTable(prjItem.Name.ToLower(), prjItem.PrjEnvelope.MinX, prjItem.PrjEnvelope.MaxX, prjItem.PrjEnvelope.MinY, prjItem.PrjEnvelope.MaxY);
                    if (progressCallback != null)
                        progressCallback(-1, prjItem.Name + "投影区域入库成功！");
                }
                _regionIDs.Add(prjItem.Name, regionID);
            }
        }

        private void TryReProject(Action<int,string > progressCallback)
        {
            string reprjlog = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + "{0}.DataProcess.{1}.log";
            reprjlog = string.Format(reprjlog, "MYD06ReProject", DateTime.Now.ToString("yyyyMMdd"));
            if (File.Exists(reprjlog))
            {
                StreamReader sr = null;
                try
                {
                    //FileStream fs = new FileStream(reprjlog, FileMode.Open, FileAccess.Read);
                    sr = new StreamReader(reprjlog, Encoding.Default);
                    PrjEnvelopeItem prjItem=null;
                    string prjRegionName =null,dataFile,dataSet,locationFile,outFilename;
                    float outResolution;
                    while (true)
                    {
                        string da = sr.ReadLine();
                        if (da == null||string.IsNullOrWhiteSpace(da))
                            break;
                        try
                        {
                            string[] parts = da.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            //"投影失败;" + dataFile + ";" + dataSet + ";" + locationFile + ";" +prjItem.Name+";" + outResolution.ToString("f2") + ";" + outFilename+ ";" + ex.Message
                            if (parts == null || parts.Length < 7)
                                continue;
                            dataFile = parts[1];
                            dataSet = parts[2];
                            locationFile = parts[3];
                            prjRegionName = parts[4];
                            if (!float.TryParse(parts[5], out outResolution))
                                continue;
                            outFilename = parts[6];
                            foreach (PrjEnvelopeItem item in _prjEnvelopes)
                            {
                                if (item.Name == prjRegionName)
                                {
                                    prjItem = item;
                                    break;
                                }
                            }
                            if (prjItem == null)
                                continue;
                            if (!File.Exists(locationFile) || !File.Exists(dataFile))
                                continue;
                            if (File.Exists(outFilename))
                                File.Delete(outFilename);
                            else if (!Directory.Exists(Path.GetDirectoryName(outFilename)))
                                TryCreateDstDir(Path.GetDirectoryName(outFilename), progressCallback);
                            Project(prjItem, dataSet, locationFile, dataFile, outFilename, outResolution, null, _projectLog);
                        }
                        catch (System.Exception ex)
                        {
                            progressCallback(-1, ex.Message);
                            continue;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    progressCallback(-1, ex.Message);
                    return;
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                        sr.Dispose();
                    }
                    string donefile = Path.ChangeExtension(reprjlog, ".done." + DateTime.Now.ToString("HHmmss") + ".log");
                    if (!File.Exists(donefile))
                        File.Move(reprjlog, donefile);
                    else
                    {
                        int cc=1;
                        string newdonefile;
                        while (true)
                        {
                            newdonefile = Path.Combine(Path.GetFileNameWithoutExtension(donefile) + cc + ".log");
                            if (!File.Exists(newdonefile))
                            {
                                File.Move(reprjlog, Path.ChangeExtension(reprjlog, ".done.log"));
                                break;
                            }
                            cc++;
                        }
                    }
                }
            }
        }

        private void TryDeleteProjectErrorFiles(Action<int, string> progressCallback)
        {
            string reprjlog = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + "{0}.DataProcess.{1}.log";
            reprjlog = string.Format(reprjlog, "MYD06ProjectError", DateTime.Now.ToString("yyyyMMdd"));
            if (File.Exists(reprjlog))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(reprjlog, Encoding.Default);
                    string outFilename="";
                    while (true)
                    {
                        string da = sr.ReadLine();
                        if (da == null || string.IsNullOrWhiteSpace(da))
                            break;
                        try
                        {
                            string[] parts = da.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            //"投影失败!" + ";" + dataFile + ";" + dataSet + ";" + outResolution.ToString("f2") + ";" + outFilename + ";" + ex.Message 
                            if (parts == null || parts.Length <6)
                                continue;
                            outFilename = parts[4];
                            if (File.Exists(outFilename))
                                File.Delete(outFilename);
                        }
                        catch (System.Exception ex)
                        {
                            progressCallback(-1, ex.Message+"；删除错误的投影文件：" + outFilename + "失败！");
                            continue;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    progressCallback(-1, ex.Message);
                    return;
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                        sr.Dispose();
                    }
                    try
                    {
                        string donefile = Path.ChangeExtension(reprjlog, ".done." + DateTime.Now.ToString("HHmmss") + ".log");
                        if (!File.Exists(donefile))
                            File.Move(reprjlog, donefile);
                        else
                        {
                            int cc = 1;
                            string newdonefile;
                            while (true)
                            {
                                newdonefile = Path.Combine(Path.GetFileNameWithoutExtension(donefile) + cc + ".log");
                                if (!File.Exists(newdonefile))
                                {
                                    File.Move(reprjlog, Path.ChangeExtension(reprjlog, ".done.log"));
                                    break;
                                }
                                cc++;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        progressCallback(-1, ex.Message);
                    }
                }
            }
        }

        private void DoProject(string[] files, Action<int, string> progressCallback)
        {
            string[] dataSets1km = Get1KMDataSets();
            string[] dataSets5km = Get5KMDataSets();
            if (progressCallback != null)
                progressCallback(6, "链接数据库，查询数据产品类别...");
            QueryIDs(dataSets1km, dataSets5km, progressCallback);
            if (_prjEnvelopes != null && _prjEnvelopes.Count() > 0)
            {
                string infilename, infiledir, outDir, dataStr, dnLabel;
                string locationFile;
                //string subDir = "5分钟段产品\\{0}\\AQUA\\MODIS\\{1}\\{2}\\{3}\\{4}\\{5}";
                string setName, outFilename;
                StringBuilder outDirBuilder = new StringBuilder(200);
                StringBuilder outFilenameBuilder = new StringBuilder(400);
                float resl05 = _isOriginResl ? 0.05f : _resl;
                float resl01 = _isOriginResl ? 0.01f : _resl;
                if (_isLostAdded)
                {
                    Dictionary<float, List<string>> items = new Dictionary<float, List<string>>();
                    if (dataSets5km.Length != 0)
                    {
                        if (!items.ContainsKey(resl05))
                            items.Add(resl05, dataSets5km.ToList());
                    }
                    if (dataSets1km.Length != 0)
                    {
                        if (!items.ContainsKey(resl01))
                            items.Add(resl01, dataSets1km.ToList());
                        else
                            items[resl01].AddRange(dataSets1km);
                    }
                    files = GetReProjectFiles(files, items, progressCallback);
                    if (files.Length==0)
                    {
                        if (progressCallback != null)
                            progressCallback(-1, "从拼接日志中解析的待重新投影文件个数为0！");
                    }
                }
                foreach (PrjEnvelopeItem prjItem in _prjEnvelopes)
                {
                    if (progressCallback != null)
                        progressCallback(-1,prjItem.Name + "投影区域分文件投影开始...");
                    int count = files.Length;
                    for (int i = 0; i < files.Length; i++)
                    {
                        if ((_allPaths.Count+1)%300==0)
                        {
                            TryDeleteProjectErrorFiles(progressCallback);
                        }
                        if (progressCallback != null)
                        {
                            int pct = 6 + (int)(i / (count * 1.0f) * (75 - 6));
                            progressCallback(pct, "共" + files.Length + "个待投影文件，开始投影第" + (i + 1) + "个...");
                        }
                        infilename = Path.GetFileName(files[i]);
                        infiledir = Path.GetDirectoryName(files[i]);
                        if (progressCallback != null) 
                            progressCallback(-1, infilename + "投影开始...");
                        DateTime dt = GetOribitTime(files[i], out dataStr);
                        if (dataStr == "" || dt == DateTime.MinValue)
                        {
                            if (progressCallback != null)
                                progressCallback(-1, "\t获取文件名中的时间信息失败！" + files[i]);
                            continue;
                        }
                        DateTime timeparts = dt.Date.AddHours(9);
                        dnLabel = (dt.CompareTo(timeparts) <= 0) ? "day" : "night";//根据文件名中的时间信息确定其归属于白天或夜间文件夹
                        #region 5km投影
                        if (dataSets5km.Length != 0&&progressCallback != null)
                            progressCallback(-1, "\t5km数据集投影开始...");
                        foreach (string item in dataSets5km)
                        {
                            if (item.ToUpper().Contains("DAY") && dnLabel == "night")
                            {
                                if (progressCallback != null)
                                    progressCallback(-1, "\t\t不投影" + item + "数据集的" + dnLabel + "数据！");
                                continue;
                            }
                            else if (item.ToUpper().Contains("NIGHT") && dnLabel == "day")
                            {
                                if (progressCallback != null)
                                    progressCallback(-1, "\t\t不投影" + item + "数据集的" + dnLabel + "数据！");
                                continue;
                            }
                            //if (progressCallback != null)
                            //    progressCallback(-1, "\t正在投影" + item + "数据集...");
                            setName = item.Replace("_", "");
                            //outDir = Path.Combine(_outputDir, string.Format(subDir, setName, dt.Year, dt.Month, dt.Day, dnLabel, resl05));//GetDatasetOutDir(outDirs, item);
                            //outFilename = Path.Combine(outDir, string.Format("{0}_{1}_{2}_granule_{3}_{4}{5}", setName, "MYD06", prjItem.Name, dataStr, resl05.ToString("f2"), ".LDF"));
                            outDirBuilder.Clear();
                            //outDir = outDirBuilder.Append(Path.Combine(_outputDir, "5分钟段产品\\")).Append(setName + "\\").Append("AQUA\\MODIS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl05.ToString("f2")).ToString();
                            outDir = outDirBuilder.Append(Path.Combine(_outputDir, "5分钟段产品\\")).Append(setName + "\\").Append("AQUA\\MODIS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl05.ToString("f2")).ToString();
                            outFilenameBuilder.Clear();
                            outFilename = outFilenameBuilder.Append(setName).Append("_MYD06_").Append(prjItem.Name).Append("_granule_").Append(dataStr + "_").Append(resl05.ToString("f2") + ".LDF").ToString();
                            outFilename = Path.Combine(outDir, outFilename);
                            if (progressCallback != null)
                                progressCallback(-1, "\t\t正在投影"+item + "数据集,文件：" + outFilename);
                            if (File.Exists(outFilename))
                            {
                                if (_overlapPrj == false)
                                {
                                    if (progressCallback != null)
                                        progressCallback(-1, "\t\t\t5km投影文件已存在,跳过！");
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
                                TryCreateDstDir(outDir, progressCallback);
                            if (!_allPaths.Contains(outDir))
                                _allPaths.Add(outDir);
                            if (Project(prjItem, item, files[i], files[i], outFilename, resl05, dataStr, _projectLog, _overlapPrj))
                            {
                                if (progressCallback != null)
                                    progressCallback(-1, "\t\t\t\t成功！5km投影处理成功！");
                            }
                            else
                            {
                                if (progressCallback != null)
                                    progressCallback(-1, "\t\t\t\t失败!5km投影处理出错！");
                            }
                        }
                        if (dataSets5km.Length != 0 && progressCallback != null)
                            progressCallback(-1, "\t5km数据集投影结束！");
                        #endregion
                        #region 1km投影
                        if (dataSets1km.Length < 1)
                            continue;
                        locationFile = Path.Combine(infiledir, infilename.Replace("MYD06_L2", "MYD03"));
                        if (File.Exists(locationFile))
                        {
                            //if (!GeoDo.HDF4.HDF4Helper.IsHdf4(locationFile))
                            //{
                            //    if (progressCallback != null) 
                            //        progressCallback(-1, "文件" + locationFile + "不是HDF4文件或已损坏，无法进行1km处理！");
                            //    continue;
                            //}
                            if (dataSets1km.Length != 0 && progressCallback != null)
                                progressCallback(-1, "\t1km分辨率数据集投影开始...");
                            foreach (string item in dataSets1km)
                            {
                                if (item.ToUpper().Contains("DAY") && dnLabel == "night")
                                {
                                    if (progressCallback != null)
                                        progressCallback(-1, "\t\t不投影" + item + "数据集的" + dnLabel + "数据！");
                                    continue;
                                }
                                else if (item.ToUpper().Contains("NIGHT") && dnLabel == "day")
                                {
                                    if (progressCallback != null)
                                        progressCallback(-1, "\t\t不投影" + item + "数据集的" + dnLabel + "数据！");
                                    continue;
                                }
                                //if (progressCallback != null)
                                //    progressCallback(-1, "\t正在投影" + item + "数据集...");
                                setName = item.Replace("_", "");
                                //outDir = Path.Combine(_outputDir, string.Format(subDir, setName, dt.Year, dt.Month, dt.Day, dnLabel, resl01)); //GetDatasetOutDir(outDirs, item);
                                //outFilename = Path.Combine(outDir, string.Format("{0}_{1}_{2}_granule_{3}_{4}{5}", setName, "MYD06", prjItem.Name, dataStr, resl01.ToString("f2"), ".LDF"));
                                outDirBuilder.Clear();
                                //outDir = outDirBuilder.Append(Path.Combine(_outputDir, "5分钟段产品\\")).Append(setName + "\\").Append("AQUA\\MODIS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl01.ToString("f2")).ToString();
                                outDir = outDirBuilder.Append(Path.Combine(_outputDir, "5分钟段产品\\")).Append(setName + "\\").Append("AQUA\\MODIS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl01.ToString("f2")).ToString();
                                outFilenameBuilder.Clear();
                                outFilename = outFilenameBuilder.Append(setName).Append("_MYD06_").Append(prjItem.Name).Append("_granule_").Append(dataStr + "_").Append(resl01.ToString("f2") + ".LDF").ToString();
                                outFilename = Path.Combine(outDir, outFilename);
                                if (progressCallback != null)
                                    progressCallback(-1, "\t\t正在投影" + item + "数据集,文件：" + outFilename);
                                if (File.Exists(outFilename))
                                {
                                    if (_overlapPrj == false)
                                    {
                                        if (progressCallback != null)
                                            progressCallback(-1, "\t\t\t1km投影文件已存在,跳过！");
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
                                    TryCreateDstDir(outDir, progressCallback);
                                if (!_allPaths.Contains(outDir))
                                    _allPaths.Add(outDir);
                                if (Project(prjItem, item, locationFile, files[i], outFilename, resl01, dataStr, _projectLog, _overlapPrj))
                                {
                                    if (progressCallback != null)
                                        progressCallback(-1, "\t\t\t\t成功!1km投影处理完成！");
                                }
                                else
                                {
                                    if (progressCallback != null)
                                        progressCallback(-1, "\t\t\t\t失败!1km投影处理出错！");
                                }
                            }
                            if (dataSets1km.Length != 0 && progressCallback != null)
                                progressCallback(-1, "\t1km数据集投影结束！");
                        }
                        #endregion
                        if (progressCallback != null) 
                            progressCallback(-1, infilename + "投影结束！");
                        if (progressCallback != null) 
                            progressCallback(-1, "-----------------------------------------------");
                    }
                    if (progressCallback != null) 
                        progressCallback(-1, prjItem.Name + "投影区域分文件投影结束！");
                }
            }
        }

        public static void TryCreateDstDir(string outDir, Action<int, string> progressCallback)
        {
            int times = 0;
            while (true)
            {
                try
                {
                    if (!Directory.Exists(outDir))
                        Directory.CreateDirectory(outDir);
                    return ;
                }
                catch (System.Exception ex)
                {
                    if ((times++) % 4 == 1)
                        if (progressCallback != null) 
                            progressCallback(-5, ex.Message);
                    else
                    {
                        if (progressCallback != null) 
                            progressCallback(-1, ex.Message);
                        Thread.Sleep(100);
                    }
                }
            }
        }

        public static DateTime GetOribitTime(string fileName, out string orbitTime)
        {
            int year;
            orbitTime = "";
            string fName = Path.GetFileName(fileName).ToUpper();
            Match match = Regex.Match(fName, @".(?<year>\d{4})(?<days>\d{3}).(?<hour>\d{2})(?<minutes>\d{2}).");
            if (match.Success)
            {
                year = Int32.Parse(match.Groups["year"].Value);
                int daysofyear = Int32.Parse(match.Groups["days"].Value);
                DateTime date = DataContinuityDetec.GetDateFormDaysOfYear(year, daysofyear);
                date = date.AddHours(double.Parse(match.Groups["hour"].Value)).AddMinutes(double.Parse(match.Groups["minutes"].Value));
                orbitTime = date.ToString("yyyyMMddHHmm");
                return date;
            }
            return DateTime.MinValue;
        }

        private string[] Get5KMDataSets()
        {
            List<string> dataSets5km = new List<string>();
            foreach (string dataset in _dataSets)
            {
                if (!string.IsNullOrEmpty(dataset) && !_dataset1kmList.Contains(dataset))
                    dataSets5km.Add(dataset);
            }
            return dataSets5km.ToArray();
        }

        private string[] Get1KMDataSets()
        {
            List<string> dataSets1km = new List<string>();
            foreach (string dataset in _dataSets)
            {
                if (_dataset1kmList.Contains(dataset))
                    dataSets1km.Add(dataset);
            }
            return dataSets1km.ToArray();
        }

        public bool Project(PrjEnvelopeItem prjItem, string dataSet, string locationFile, string dataFile, string outFilename, float outResolution, string dataStr, string log, bool overlapprj = false, string proMode = "MYD06")
        {
            //return ProjectError(prjItem, dataSet, locationFile, dataFile, outFilename, outResolution, dataStr,log);
            string minX, maxX, minY, maxY, outresl;
            minX = prjItem.PrjEnvelope.MinX.ToString();
            maxX = prjItem.PrjEnvelope.MaxX.ToString();
            minY = prjItem.PrjEnvelope.MinY.ToString();
            maxY = prjItem.PrjEnvelope.MaxY.ToString();
            outresl = outResolution.ToString();
            string cmd = "\"" + prjItem.Name + "\"" + " " + "\"" + minX + "\"" + " " + "\"" + maxX + "\"" + " " + "\"" + minY + "\"" + " " + "\"" + maxY
                    + "\"" + " " + "\"" + dataSet + "\"" + " " + "\"" + locationFile + "\"" + " " + "\"" + dataFile
                 + "\"" + " " + "\"" + outFilename + "\"" + " " + "\"" + outresl
                 + "\"";
            string exeName = AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.MIF.Prds.CLD.Projection.exe";
            try
            {
                Process convertProcess = new Process();
                ProcessStartInfo startinfo = new ProcessStartInfo(exeName, cmd);
                startinfo.UseShellExecute = false;          //不使用系统外壳程序启动 
                startinfo.RedirectStandardInput = false;    //不重定向输入 
                startinfo.RedirectStandardOutput = true;    //重定向输出，而不是默认的显示在dos控制台上 
                startinfo.CreateNoWindow = true;            //不创建窗口                 
                convertProcess.StartInfo = startinfo;
                //convertProcess.StartInfo.UseShellExecute = false;////不使用系统外壳程序启动 
                convertProcess.Start();
                convertProcess.WaitForExit();
                return true;
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine(log, "投影失败!" + ";" + dataFile + ";" + dataSet + ";" + outResolution.ToString("f2") + ";" + outFilename + ";" + ex.Message);
                return false;// WriteLog("转换过程出错！原因：" + ex.Message);
            }
        }

        #endregion

        #region 历史投影文件直接拼接
        private void AddGranuleDir2Mosaic(string[] files, Action<int, string> progressCallback)
        {
            string[] dataSets1km = Get1KMDataSets();
            string[] dataSets5km = Get5KMDataSets();
            if (progressCallback != null)
                progressCallback(6, "链接数据库，查询数据产品类别...");
            QueryIDs(dataSets1km, dataSets5km);
            if (_prjEnvelopes != null && _prjEnvelopes.Count() > 0)
            {
                //string subDir = "5分钟段产品\\{0}\\AQUA\\MODIS\\{1}\\{2}\\{3}\\{4}\\{5}";
                float resl05 = _isOriginResl ? 0.05f : _resl;
                float resl01 = _isOriginResl ? 0.01f : _resl;
                #region
                string infilename, infiledir, outDir, dataStr, dnLabel;
                StringBuilder outDirbuilder = new StringBuilder(200);
                for (int i = 0; i < files.Length; i++)
                {
                    infilename = Path.GetFileName(files[i]);
                    infiledir = Path.GetDirectoryName(files[i]);
                    //if (progressCallback != null)
                    //    progressCallback(-1, "正在获取"+infilename+"文件的投影数据...");
                    DateTime dt = GetOribitTime(files[i], out dataStr);
                    if (dataStr == "" || dt == DateTime.MinValue)
                    {
                        if (progressCallback != null) 
                            progressCallback(-1, "获取文件名中的时间信息失败！");
                        continue;
                    }
                    DateTime timeparts = dt.Date.AddHours(9);
                    dnLabel = (dt.CompareTo(timeparts) <= 0) ? "day" : "night";//根据文件名中的时间信息确定其归属于白天或夜间文件夹
                    foreach (string item in dataSets5km)
                    {
                        if (item.ToUpper().Contains("DAY") && dnLabel == "night")
                        {
                            if (progressCallback != null)
                                progressCallback(-1, "\t\t"+item + "数据集的" + dnLabel + "数据不进行拼接！");
                            continue;
                        }
                        else if (item.ToUpper().Contains("NIGHT") && dnLabel == "day")
                        {
                            if (progressCallback != null)
                                progressCallback(-1, "\t\t" + item + "数据集的" + dnLabel + "数据不进行拼接！");
                            continue;
                        }
                        //outDir = Path.Combine(_historyPrjDir, string.Format(subDir, item.Replace("_", ""), dt.Year, dt.Month, dt.Day, dnLabel, resl05));//GetDatasetOutDir(outDirs, item);
                        outDirbuilder.Clear();
                        //outDir = outDirbuilder.Append(Path.Combine(_historyPrjDir, "5分钟段产品\\")).Append(item.Replace("_", "")).Append("\\AQUA\\MODIS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl05.ToString("f2")).ToString();
                        outDir = outDirbuilder.Append(Path.Combine(_historyPrjDir, "5分钟段产品\\")).Append(item.Replace("_", "")).Append("\\AQUA\\MODIS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl05.ToString("f2")).ToString();
                        if (Directory.Exists(outDir))
                        {
                            if (!_allPaths.Contains(outDir))
                            {
                                _allPaths.Add(outDir);
                                if (progressCallback != null)
                                    progressCallback(-1, outDir + "加至拼接路径成功！");
                            }
                        }
                        else
                        {
                            if (progressCallback != null)
                                progressCallback(-1, outDir + "不存在，无法进行拼接！");
                        }
                    }
                    foreach (string item in dataSets1km)
                    {
                        if (item.ToUpper().Contains("DAY") && dnLabel == "night")
                        {
                            if (progressCallback != null)
                                progressCallback(-1, "\t\t" + item + "数据集的" + dnLabel + "数据不进行拼接！");
                            continue;
                        }
                        else if (item.ToUpper().Contains("NIGHT") && dnLabel == "day")
                        {
                            if (progressCallback != null)
                                progressCallback(-1, "\t\t" + item + "数据集的" + dnLabel + "数据不进行拼接！");
                            continue;
                        }
                        //outDir = Path.Combine(_historyPrjDir, string.Format(subDir, item.Replace("_", ""), dt.Year, dt.Month, dt.Day, dnLabel, resl01)); //GetDatasetOutDir(outDirs, item);
                        outDirbuilder.Clear();
                        //outDir = outDirbuilder.Append(Path.Combine(_historyPrjDir, "5分钟段产品\\")).Append(item.Replace("_", "")).Append("\\AQUA\\MODIS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl01.ToString("f2")).ToString();
                        outDir = outDirbuilder.Append(Path.Combine(_historyPrjDir, "5分钟段产品\\")).Append(item.Replace("_", "")).Append("\\AQUA\\MODIS\\").Append(dt.Year + "\\").Append(dt.Month + "\\").Append(dt.Day + "\\").Append(dnLabel + "\\").Append(resl01.ToString("f2")).ToString();
                        if (Directory.Exists(outDir))
                        {
                            if (!_allPaths.Contains(outDir))
                            {
                                _allPaths.Add(outDir);
                                if (progressCallback != null)
                                    progressCallback(-1, outDir + "加至拼接路径成功！");
                            }
                        }
                        else
                        {
                            if (progressCallback != null)
                                progressCallback(-1, outDir + "不存在，无法进行拼接！");
                        }
                    }
                }
                #endregion
            }
        }
        #endregion

        #region 拼接
        private void Moasic(Action<int, string> progressCallback)
        {
            //搜索每个生成目录，目录下有相同时间的文件进行拼接
            int count = 0;
            string[] files = null;
            Dictionary<string, List<string>> sortFiles = null;
            foreach (string dir in _allPaths)
            {
                if (progressCallback != null)
                {
                    int pct = 75 +( int)(count*1.0f / _allPaths.Count * (99 - 75));
                    progressCallback(pct, "共" + _allPaths.Count + "个待拼接文件，开始拼接第" + (++count) + "个...");
                    progressCallback(-1, "\t"+dir + "投影文件日拼接开始...");
                }                
                if (!Directory.Exists(dir))
                {
                    if (progressCallback != null) 
                        progressCallback(-1, dir + "不存在！");
                    continue;
                }
                //扫描每天文件夹下的5分钟段投影文件
                files = Directory.GetFiles(dir, "*MYD06*granule*.ldf");
                if (files.Length == 0)
                {
                    if (progressCallback != null) 
                        progressCallback(-1, dir + "不存在可处理的MODIS投影文件！");
                    continue;
                }
                //将每天文件按照区域进行区分
                sortFiles = SortFilesByRegion(files);//区域标识，文件列表
                foreach (string key in sortFiles.Keys)
                {
                    //将5分钟段文件按时间进行升序排列
                    SortFilesByTime(sortFiles[key]);
                    //if (progressCallback != null) 
                    //    progressCallback(-1, key + "区域投影文件拼接开始...");
                    foreach (PrjEnvelopeItem prj in _prjEnvelopes)
                    {
                        if (prj.Name == key)
                        {
                            DoMoasic(prj, sortFiles[key].ToArray(),/*fillvalue,ids,_regionIDs[key],*/ progressCallback, _outputDir,_overlapMosaic);
                        }
                    }
                }
            }
            if (progressCallback != null)
                progressCallback(99, "文件拼接结束，开始生成拼接日志...");
            //数据的连续性检验通过查询数据库，指定时间范围，检索日拼接数据日期文件，判断哪些日拼接文件缺失
            MosaicCheckAndOutPutLog(progressCallback);
        }

        public void MosaicCheckAndOutPutLog(Action<int, string> progressCallback,string sensor = "MODIS", string defaultlog = "MYD06MosaicLog.txt")
        {
            #region 将每个数据集的每个月的day/night日拼接数据进行有效率统计
            string relaDir = null;
            List<ConnectMySqlCloud.DayMergeLine> lineValues = null;
            string logformat = "MosaicLog.{0}.txt";
            string log;
            foreach (string daydir in _allMosaicPaths)
            {
                try
                {
                    relaDir = daydir.Replace(_outputDir, "");
                    lineValues = _dbCon.QureyDayMergeMonthly(relaDir);
                    if (lineValues == null || lineValues.Count < 1)
                        continue;
                    log = string.Format(logformat, relaDir.Replace('\\', '_'));
                    string logname = Path.Combine(daydir + "\\", log);
                    if (File.Exists(logname))
                    {
                        File.Delete(logname);
                    }
                    #region 日拼接连续性检测
                    List<DateTime> modate = new List<DateTime>();
                    Dictionary<DateTime, string[]> moInvalidPct = new Dictionary<DateTime, string[]>();
                    int cc = 0;
                    foreach (ConnectMySqlCloud.DayMergeLine dayline in lineValues)
                    {
                        if (!modate.Contains(dayline.Datatime))
                            modate.Add(dayline.Datatime);
                        if (dayline.ValidPct <= 70)
                        {
                            cc++;
                            if (!moInvalidPct.ContainsKey(dayline.Datatime))
                                moInvalidPct.Add(dayline.Datatime, new string[] { dayline.ValidPct.ToString(), dayline.ImageName });
                            else
                                moInvalidPct.Add(dayline.Datatime.AddSeconds(cc), new string[] { dayline.ValidPct.ToString(), dayline.ImageName });
                        }
                    }
                    DateTime mindate = new DateTime(modate.Min().Year, modate.Min().Month, 1);
                    DateTime maxdate = new DateTime(modate.Min().Year, modate.Min().Month+1, 1);
                    if (modate.Count != DateTime.DaysInMonth(modate.Min().Year, modate.Min().Month))
                    {
                        using (StreamWriter sw = new StreamWriter(logname, true, Encoding.Default))
                        {
                            sw.WriteLine("------------------------------------------------");
                            sw.WriteLine("\t缺失日拼接日期");
                            for (DateTime i = mindate; i < maxdate; i = i.AddDays(1))
                            {
                                if (!modate.Contains(i))
                                {
                                    sw.WriteLine("\t" + i.ToString("yyyy-MM-dd"));
                                }
                            }
                        }
                    }
                    if (moInvalidPct.Count > 0)
                    {
                        using (StreamWriter sw = new StreamWriter(logname, true, Encoding.Default))
                        {
                            sw.WriteLine("------------------------------------------------");
                            sw.WriteLine("拼接产品有效率过低(<=70%)");
                            sw.WriteLine("\t\t日期\t\t\t有效率(%)\t\t\t文件");
                            foreach (DateTime day in moInvalidPct.Keys)
                            {
                                sw.WriteLine("\t" + String.Format("{0, -12}", day.ToString("yyyy-MM-dd")) + "\t" + String.Format("{0, 3}", moInvalidPct[day][0]) + "\t" + moInvalidPct[day][1]);
                            }
                        }
                    }
                    #endregion
                    string lineformat = "\t{0}\t{1}\t{2}\t{3}\t{4}";
                    using (StreamWriter sw = new StreamWriter(logname, true, Encoding.Default))
                    {
                        sw.WriteLine("------------------------------------------------");
                        sw.WriteLine("\t\t日期\t\t\t有效率(%)\t5分钟段个数\t\t日产品文件\t\t\t\t5分钟段时分");
                        for (int i = 0; i < lineValues.Count; i++)
                        {
                            sw.WriteLine(string.Format(lineformat, String.Format("{0, -12}", lineValues[i].Datatime.ToString("yyyy-MM-dd")), String.Format("{0, 3}", lineValues[i].ValidPct), String.Format("{0,5}", lineValues[i].GranuleCounts), String.Format("{0, -60}", lineValues[i].ImageName), lineValues[i].GranuleTimes));
                        }
                    }
                    Process.Start(logname);
                }
                catch (System.Exception ex)
                {
                    progressCallback(-1, daydir+"的拼接日志生成失败！");
                }                
            }
            #endregion
            progressCallback(-1, "输出拼接结果日志结束！");
        }

        private void MosaicOutPutLogEveryFile(string mosaicFname,DateTime time,int validPct,int granuleCounts,string granuletimes)
        {
            string relaDir,daydir;
            string logformat = "MosaicLogEveryFile.{0}.txt";
            string lineformat = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}";
            string log;
            bool isneedhead =true;
            daydir =Path.GetDirectoryName(mosaicFname);
            relaDir = daydir.Replace(_outputDir, "");
            log = string.Format(logformat, relaDir.Replace('\\', '_'));
            string logname = Path.Combine(daydir + "\\", log);
            if (File.Exists(logname))
                isneedhead=false;
            using (StreamWriter sw = new StreamWriter(logname, true, Encoding.Default))
            {
                if (isneedhead)
                    sw.WriteLine("处理时间\t\t\t\t\t\t\t日期\t\t\t有效率(%)\t5分钟段个数\t\t日产品文件\t\t\t\t5分钟段时分");
                sw.WriteLine(string.Format(lineformat, String.Format("{0, -20}", DateTime.Now.ToString()), String.Format("{0, -12}", time.ToString("yyyy-MM-dd")), String.Format("{0, 3}", validPct), String.Format("{0,5}", granuleCounts), String.Format("{0, -60}", Path.GetFileName(mosaicFname)), granuletimes));
            }
        }            

        public void DoMoasic(PrjEnvelopeItem prjEvlop, string[] list,Action<int, string> progressCallback, string outPath,bool overlapMosaic = false, string sensor = "MODIS", string log = "MosaicError")
        {
            if (list.Length < 1)
                return;
            string granuleTime;
            DateTime dayFileDate;
            string newFileName = GetMoasicFileName(list.ToArray(), out granuleTime, out dayFileDate);
            string dir = Path.GetDirectoryName(newFileName);
            if (!Directory.Exists(dir))
                TryCreateDstDir(dir, progressCallback);
            if (!_allMosaicPaths.Contains(dir))
                _allMosaicPaths.Add(dir);
            if (progressCallback != null)
                progressCallback(-1, "\t输出文件:"+newFileName);
            if (File.Exists(newFileName))
            {
                if (overlapMosaic == false && _dbCon.IshasRecord("CP_DayMergeProducts_TB", "ImageName", Path.GetFileName(newFileName))
                    && _dbCon.QueryDayMergeGranuleCounts(Path.GetFileName(newFileName)) >= list.Count())
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t"+Path.GetFileName(newFileName) + "文件已存在，拼接跳过！");
                    string overviewpng = Path.Combine(Path.GetDirectoryName(newFileName), Path.ChangeExtension(Path.GetFileName(newFileName), ".png"));
                    if (!File.Exists(newFileName))
                    {
                        GenerateOverview(newFileName, null, null);
                        if (progressCallback != null)
                            progressCallback(-1, "\t" + Path.GetFileName(newFileName) + "重新生成快视图成功！");
                    }
                    return;
                }
                else
                {
                    File.Delete(newFileName);
                    if (progressCallback != null)
                        progressCallback(-1, "\t重新拼接开始...");
                }
            }
            CoordEnvelope env = new CoordEnvelope(prjEvlop.PrjEnvelope.MinX, prjEvlop.PrjEnvelope.MaxX, prjEvlop.PrjEnvelope.MinY, prjEvlop.PrjEnvelope.MaxY);
            string setName = Path.GetFileNameWithoutExtension(newFileName).Split('_')[0];
            double selfFillValue = _selfFillValue[setName];
            double dayFillValue = _dayFillValue[setName];
            double dayInvalidValue = _dayInvalidValue[setName];
            long prdID = _datasetsPrdsIDs[setName];
            long datasetID = _datasetsIDs[setName];
            long regionID = _regionIDs[prjEvlop.Name];
            try
            {
                float resolution=float.NaN;
                float resolutionX = 0.05f, resolutionY = 0.05f;
                if (float.TryParse(Path.GetFileNameWithoutExtension(newFileName).Split('_')[5],out resolution) )
                {
                    resolutionX = resolution;
                    resolutionY = resolution;
                }
                enumDataType datatype;
                int bandcount=1;
                IRasterDataProvider outRaster = null;
                int validPercent=0;
                for (int k = 0; k < list.Length; k++)
                {
                    try
                    {
                        using (IRasterDataProvider inrst = GeoDataDriver.Open(list[k]) as IRasterDataProvider)
                        {
                            if (inrst == null)
                                continue;
                            datatype = inrst.DataType;
                            bandcount = inrst.BandCount;
                            if (inrst.Height == 0 || inrst.Width == 0 || bandcount == 0 || inrst.ResolutionX == float.NaN || inrst.ResolutionY == float.NaN)
                            {
                                inrst.Dispose();
                                continue;
                            }
                            if (resolution == float.NaN || resolution == 0)
                            {
                                resolutionX = inrst.ResolutionX;
                                resolutionY = inrst.ResolutionY;
                            }
                            if (outRaster == null)
                            {
                                outRaster = RasterMosaic.CreateRaster(newFileName, env, resolutionX, resolutionY, bandcount, inrst);
                                break;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        if (progressCallback != null)
                            progressCallback(-1, "投影文件" + Path.GetFileName(list[k]) + "打开出错！" + ex.Message);
                        continue;
                    }
                }
                if (bandcount>0)
                {
                    for (int i = 1; i <= bandcount; i++)
                    {
                        outRaster.GetRasterBand(i).Fill(dayFillValue);
                    }
                    if (progressCallback != null) 
                        progressCallback(-1, "\t开始执行拼接...");
                    RasterMosaic mosic = new RasterMosaic();
                    mosic.Sensor = sensor;
                    mosic.SetDstFillValues(dayFillValue.ToString());
                     Dictionary<string, string> sv = new Dictionary<string, string>();
                     sv.Add(selfFillValue.ToString(), dayInvalidValue.ToString());
                    mosic.SetSpatialValues(sv);
                    List<string> validFileList = new List<string>();
                    List<CoordEnvelope> rasterEnv = new List<CoordEnvelope>();
                    try
                    {
                        int i = 1;
                        foreach (string file in list)
                        {
                            try
                            {
                                using (IRasterDataProvider inrst = GeoDataDriver.Open(file) as IRasterDataProvider)
                                {
                                    if (inrst == null)
                                    {
                                        LogFactory.WriteLine(sensor + log, inrst.fileName + ":打开异常，未执行拼接！");
                                        File.Delete(file);
                                        continue;
                                    }
                                    if (inrst.Height == 0 || inrst.Width == 0 || inrst.BandCount == 0 || inrst.ResolutionX == float.NaN || inrst.ResolutionY == float.NaN)
                                    {
                                        LogFactory.WriteLine(sensor + log, inrst.fileName + ":格式错误，未执行拼接！");
                                        inrst.Dispose();
                                        File.Delete(file);
                                        continue;
                                    }
                                    validFileList.Add(file);
                                    rasterEnv.Add(inrst.CoordEnvelope);
                                    if (progressCallback != null) 
                                        progressCallback(-1, "\t\t正在拼接第" + i + "个：" + Path.GetFileName(file));
                                    mosic.Mosaic(new IRasterDataProvider[1] { inrst }, outRaster);
                                    i++;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                if (progressCallback != null)
                                    progressCallback(-1, "\t\t投影文件" + file + "加至拼接列表出错！" + ex.Message);
                                continue;
                            }
                        }
                        if (progressCallback != null) 
                            progressCallback(-1, "\t执行拼接结束！");
                        validPercent = ComputeVaildPct(outRaster, dayFillValue);
                    }
                    finally
                    {
                        if (outRaster!=null)
                            outRaster.Dispose();
                    }
                    try
                    {
                        MosaicOutPutLogEveryFile(outRaster.fileName, dayFileDate, validPercent, list.Length, granuleTime);
                        if (progressCallback != null)
                            progressCallback(-1, "\t输出拼接日志完成！");
                        GenerateOverview(newFileName, validFileList.ToArray(), rasterEnv);
                        if (progressCallback != null)
                            progressCallback(-1, "\t输出拼接快视图完成！");
                    }
                    catch (System.Exception ex)
                    {
                        progressCallback(-1, "\t异常！" + ex.Message + newFileName);
                    }
                    string imagedata = newFileName.Replace(outPath, "");
                    string datasource = "";
                    if (Path.GetFileNameWithoutExtension(newFileName).Split('_')[3].ToLower() == "day")
                        datasource = "D";
                    else
                        datasource = "N";
                    if (!_dbCon.IshasRecord("CP_DayMergeProducts_TB", "ImageName", Path.GetFileName(newFileName)))
                    {
                        _dbCon.InsertNDayMergeProductsTable(dayFileDate/*getDayTime(newFileName)*/, prdID, datasetID, imagedata, regionID, prjEvlop.Name, sensor, resolution, validPercent, list.Length, granuleTime, datasource);
                        if (progressCallback != null)
                            progressCallback(-1, "\t拼接结果入库完成！"); 
                    }
                    else
                    {
                        _dbCon.DeleteCLDParatableRecord("CP_DayMergeProducts_TB", "ImageName", Path.GetFileName(newFileName));
                        _dbCon.InsertNDayMergeProductsTable(dayFileDate, prdID, datasetID, imagedata, regionID, prjEvlop.Name, sensor, resolution, validPercent, list.Length, granuleTime, datasource);
                        if (progressCallback != null)
                            progressCallback(-1, "\t拼接结果入库更新完成！");
                    }
                    if (progressCallback != null)
                        progressCallback(-1, "拼接完成！");
                }
            }
            catch (System.Exception ex)
            {
                LogFactory.WriteLine(sensor+log, string.Format(newFileName + ":拼接文件出错，异常提示【{0}】", ex.Message));
                if (progressCallback != null) 
                    progressCallback(-1, newFileName + "拼接文件出错！");
                if (File.Exists(newFileName))
                    File.Delete(newFileName);
            }
        }

        private string GetMoasicFileName(string[] fileArray, out string granuleTime, out DateTime mosaicDate)
        {
            granuleTime = "";
            mosaicDate = DateTime.MinValue;
            if (fileArray == null || fileArray.Length < 1)
                return null;
            string filen;
            string[] fileStrs;
            foreach (string file in fileArray)
            {
                filen = Path.GetFileName(file);
                fileStrs = Path.GetFileNameWithoutExtension(filen).Split('_');
                if (fileStrs.Length < 5)
                    continue;
                granuleTime += fileStrs[4].Substring(8, 4) + ",";
            }
            granuleTime = granuleTime.TrimEnd(',');
            filen = Path.GetFileName(fileArray[0]);
            fileStrs = Path.GetFileNameWithoutExtension(filen).Split('_');
            if (fileStrs.Length < 5)
                return null;
            DirectoryInfo granuledir = new DirectoryInfo(Path.GetDirectoryName(fileArray[0]));
            string daynightLabel = granuledir.Parent.FullName.EndsWith("day") ? "day" : "night";
            string newFileName = filen.Replace(fileStrs[4], fileStrs[4].Remove(8)).Replace("_granule_", string.Format("_{0}_", daynightLabel));//将"年月日时分"改为"年月日"
            mosaicDate = DateTime.ParseExact(fileStrs[4].Remove(8), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            string monthdir = Path.Combine(granuledir.Parent.Parent.Parent.FullName, daynightLabel, fileStrs[fileStrs.Length - 1]);
            newFileName = Path.Combine(monthdir, newFileName);
            newFileName = newFileName.Replace("5分钟段产品", "日拼接产品");
            if (_isDirectMosaic)
                return newFileName.Replace(_historyPrjDir, _outputDir);
            else
            return newFileName;
        }

        /// <summary>
        /// 不同区域生成一组，每组按照时间顺序排序
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> SortFilesByRegion(string[] files)
        {
            if (files == null || files.Length < 1)
                return null;
            Dictionary<string, List<string>> sortedFiles = new Dictionary<string, List<string>>();
            string region;
            for (int i = 0; i < files.Length; i++)
            {
                ParseFileRegion(Path.GetFileName(files[i]), out region);
                if (string.IsNullOrEmpty(region))
                    continue;
                if (sortedFiles.ContainsKey(region))
                    sortedFiles[region].Add(files[i]);
                else
                    sortedFiles.Add(region, new List<string> { files[i] });
            }
            return sortedFiles;
        }

        public  void SortFilesByTime( List<string> files)
        {
            //将5分钟段文件按照时间先后顺序排序；
            Dictionary<int, string> timeSortDayfiles = new Dictionary<int, string>();
            string[] fileStrs;
            int granuleTime;
            foreach (string file in files)
            {
                if (string.IsNullOrEmpty(file))
                    continue;
                fileStrs = file.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (fileStrs.Length != 6)
                    continue;
                ////CloudFraction_MYD06_china_granule_201101010705_0.05.LDF
                if (int.TryParse(fileStrs[4].Substring(8, 4), out granuleTime))
                {
                    if (!timeSortDayfiles.ContainsKey(granuleTime))
                        timeSortDayfiles.Add(granuleTime, file);
                }
            }
            if (timeSortDayfiles.Count == 0)
                return ;
            //冒泡法升序排序
            int[] granuleTimeSorted = BubbleSort(timeSortDayfiles.Keys.ToArray());
            List<string> granuleFilesSorted = new List<string>();
            files.Clear();
            for (int k = 0; k < granuleTimeSorted.Length; k++)
            {
                granuleFilesSorted.Add(timeSortDayfiles[granuleTimeSorted[k]]);
                files.Add(timeSortDayfiles[granuleTimeSorted[k]]);
            }
        }

        public static int [] BubbleSort(int [] a) 
        {
            int j, k;
            int flag;
            int n = a.Length;
            flag = n;
            while (flag > 0)
            {
                k = flag;
                flag = 0;
                for (j = 1; j < k; j++)
                    if (a[j - 1] >a[j])
                    {
                        Swap(ref a[j - 1], ref a[j]);
                        flag = j;
                    }
            }
            return a;
        }

        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        private void GenerateOverview(string prdfname, string[] list, List<CoordEnvelope> rasterEnv)
        {
            IRasterDataProvider outRaster = null;
            Bitmap bmpOriginal = null;
            string filename = null;
            try
            {
                filename = OverViewHelper.OverView(prdfname, 800);
                outRaster = GeoDataDriver.Open(prdfname) as IRasterDataProvider;//
                bmpOriginal = new Bitmap(filename);
                string blockNum = string.Empty;
                string tempName = string.Empty;
                float x, y, ratio;
                ratio = (float)outRaster.Width / 800f;
                using (Graphics g = Graphics.FromImage(bmpOriginal))
                {
                    if (list != null && rasterEnv != null && list.Length == rasterEnv.Count)
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            tempName = Path.GetFileNameWithoutExtension(list[i]);
                            blockNum = tempName.Split('_')[4].Substring(8, 4);
                            x = Convert.ToSingle(rasterEnv[i].Center.X - outRaster.CoordEnvelope.MinX) / outRaster.ResolutionX / ratio;
                            y = Convert.ToSingle(outRaster.CoordEnvelope.MaxY - rasterEnv[i].Center.Y) / outRaster.ResolutionY / ratio;
                            g.DrawString(blockNum, new Font("Times New Roman", 8, FontStyle.Bold), new SolidBrush(Color.Red), new PointF(x, y));
                        }
                    }
                }
                string filenameWithoutExt = Path.GetFileNameWithoutExtension(filename).Replace(".overview", "");
                bmpOriginal.Save(Path.Combine(Path.GetDirectoryName(filename), filenameWithoutExt + ".png"));
            }
            finally
            {
                if (outRaster!= null)
                    outRaster.Dispose();
                if (outRaster != null)
                    bmpOriginal.Dispose();
                if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
                    File.Delete(filename);
            }
        }

        public int ComputeVaildPct(IRasterDataProvider outRaster,double invalidValue)
        {
            switch (outRaster.DataType)
            {
                case enumDataType.Byte:
                    return ComputeVaildPctBYTE(outRaster, invalidValue);
                case enumDataType.Float:
                    return ComputeVaildPctFloat(outRaster, invalidValue);
                case enumDataType.Int16:
                    return ComputeVaildPctINt16(outRaster, invalidValue);
                default:
                    return 100;
            }

        }

        private int ComputeVaildPctINt16(IRasterDataProvider outRaster, double invalidValue)
        {
            using (IRasterBand band = outRaster.GetRasterBand(1))
            {
                int width = band.Width;
                int height = band.Height;
                int validCount = 0;
                Int16[] currentDatabuffer = new Int16[width * height];
                unsafe
                {
                    fixed (Int16* cptr = currentDatabuffer)
                    {
                        IntPtr currentBuffer = new IntPtr(cptr);
                        band.Read(0, 0, width, height, currentBuffer, band.DataType, width, height);
                    }
                }
                for (int i = 0; i < currentDatabuffer.Length; i++)
                {
                    if (currentDatabuffer[i] != invalidValue)
                    {
                        validCount++;
                    }
                }
                //LogFactory.WriteLine(_mosaicLog, "计算INt16数据有效率结束！");
                return (int)(validCount / (width * height * 1.0) * 100);
            }
        }

        private int ComputeVaildPctFloat(IRasterDataProvider outRaster, double invalidValue)
        {
            //LogFactory.WriteLine(_mosaicLog, "计算Float数据有效率开始：");
            using (IRasterBand band = outRaster.GetRasterBand(1))
            {
                int width = band.Width;
                int height = band.Height;
                int validCount = 0;
                unsafe
                {
                    float[] currentDatabuffer = new float[width * height];
                    fixed (float* cptr = currentDatabuffer)
                    {
                        IntPtr currentBuffer = new IntPtr(cptr);
                        band.Read(0, 0, width, height, currentBuffer, band.DataType, width, height);
                        for (int i = 0; i < currentDatabuffer.Length; i++)
                        {
                            if (currentDatabuffer[i] != invalidValue)
                            {
                                validCount++;
                            }
                        }
                    }
                }
                //LogFactory.WriteLine(_mosaicLog, "计算Float数据有效率结束！");
                return (int)(validCount / (width * height * 1.0) * 100);
            }

        }

        private int ComputeVaildPctBYTE(IRasterDataProvider outRaster, double invalidValue)
        {
            //LogFactory.WriteLine(_mosaicLog, "计算BYTE数据有效率开始：");
            using (IRasterBand band = outRaster.GetRasterBand(1))
            {
                int width = band.Width;
                int height = band.Height;
                int validCount = 0;
                unsafe
                {
                    Byte[] currentDatabuffer = new Byte[width * height];
                    fixed (Byte* cptr = currentDatabuffer)
                    {
                        IntPtr currentBuffer = new IntPtr(cptr);
                        band.Read(0, 0, width, height, currentBuffer, band.DataType, width, height);
                        for (int i = 0; i < currentDatabuffer.Length; i++)
                        {
                            if (currentDatabuffer[i] != invalidValue)
                            {
                                validCount++;
                            }
                        }
                    }
                }
                //LogFactory.WriteLine(_mosaicLog, "计算BYTE数据有效率结束！");
                return (int)(validCount / (width * height * 1.0) * 100);
            }

        }

        /// <summary>
        /// 解析文件名称中区域名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="region"></param>
        private void ParseFileRegion(string fileName, out string region)
        {
            //CloudFraction_MYD06_china_granule_201101010705_0.05.LDF
            region = "china";
            if (string.IsNullOrEmpty(fileName))
                return;
            string[] fileStrs = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (fileStrs.Length != 6)
            {
                return;
            }
            region = fileStrs[2];
        }

        #endregion

        private static string GetDefaultNullValue(enumDataType datatype)
        {
            switch (datatype)
            {
                case enumDataType.Byte:
                    return "126";
                case enumDataType.Double:
                    return "-32767";
                case enumDataType.Float:
                    return "-32767";
                case enumDataType.Int16:
                    return "-32767";
                case enumDataType.Int32:
                    return "-32767";
                case enumDataType.Int64:
                    return "-32767";
                case enumDataType.UInt16:
                    return UInt16.MaxValue.ToString();
                case enumDataType.UInt32:
                    return UInt16.MaxValue.ToString();
                case enumDataType.UInt64:
                    return UInt16.MaxValue.ToString();
                case enumDataType.Unknow:
                case enumDataType.Atypism:
                case enumDataType.Bits:
                default:
                    break;
            }
            return null;
        }

        #region 补充数据投影拼接
        private string[] GetReProjectFiles(string[] allfiles, Dictionary<float,List<string>> items, Action<int, string> progressCallback)
        {
            string[] singleSetRPfiles=null;
            List<string> allSetRPfiles =new List<string>();
            foreach (float resl  in items.Keys)
            {
                foreach (string key in items[resl])
                {
                    singleSetRPfiles = GetReProjectFiles(allfiles, key, resl, progressCallback);
                    foreach (string file in singleSetRPfiles)
                    {
                        if (!allSetRPfiles.Contains(file))
                            allSetRPfiles.Add(file);
                    }
                }
            }
            return allSetRPfiles.ToArray();
        }

        private string [] GetReProjectFiles(string [] allfiles,string item,float resl,Action<int, string> progressCallback)
        {
            //从filedays的keys中获取月份，对选择的多个数据集中的某一个或者全部，拼接路径得到其月拼接日志所在地；
            //如果拼接日志存在且存在有效百分比<90%的某天invalidday，且filedays.ContainsKey(invalidday),重新投影filedays[invalidday]的文件；
            //如果拼接文件不存在,重新投影该月的数据；
            //对原始HDF文件进行日期提取，并生成filedays=dictionary<DAY,list<timefile>>;
            Dictionary<DateTime, List<string>> filedays = new Dictionary<DateTime, List<string>>();
            Dictionary<DateTime, string> monthdirlog = new Dictionary<DateTime, string>();
            DateTime orbitfileDt,orbitfileDay,mosaicMonth;
            string orbitfileDtstr, dnLabel, monthdir,monthlog, logformat = "MosaicLog.{0}.txt";
            //string item = "Cloud_Fraction";
            string setName = item.Replace("_", "");
            //float resl05 = _isOriginResl ? 0.05f : _resl;;
            StringBuilder outDirBuilder = new StringBuilder();
            List<string> rePrjFiles=new List<string>();
            if (progressCallback != null)
                progressCallback(-1,"根据输入文件，查找对应拼接日志文件...");
            foreach (string file in allfiles)
            {
                orbitfileDt = GetOribitTime(file, out orbitfileDtstr);
                dnLabel = (orbitfileDt.CompareTo(orbitfileDt.Date.AddHours(9)) <= 0) ? "day" : "night";//根据文件名中的时间信息确定其归属于白天或夜间文件夹
                if (item.ToLower().Contains("day") && dnLabel == "night")
                    continue;
                if (item.ToLower().Contains("night") && dnLabel =="day" )
                    continue;
                orbitfileDay = (dnLabel=="day") ? orbitfileDt.Date : orbitfileDt.Date.AddHours(9);
                if (filedays.ContainsKey(orbitfileDay))
                {
                    filedays[orbitfileDay].Add(file);
                }
                else
                {
                    filedays.Add(orbitfileDay, new List<string> { file });
                }
                mosaicMonth = new DateTime(orbitfileDay.Year, orbitfileDay.Month, 1, orbitfileDay.Hour, orbitfileDay.Minute, orbitfileDay.Second);
                if (monthdirlog.ContainsKey(mosaicMonth))
                    continue;
                outDirBuilder.Clear();
                monthdir = outDirBuilder.Append(Path.Combine(_outputDir, "日拼接产品\\")).Append(setName + "\\").Append("AQUA\\MODIS\\").Append(orbitfileDay.Year + "\\").Append(orbitfileDay.Month + "\\").Append(dnLabel + "\\").Append(resl).ToString();
                monthlog = Path.Combine(monthdir, string.Format(logformat, monthdir.Replace(_outputDir, "").Replace('\\', '_')));
                if (!monthdirlog.ContainsKey(mosaicMonth))
                {
                    monthdirlog.Add(mosaicMonth, monthlog);
                }
            }
            if (monthdirlog.Count==0)
            {
                if (progressCallback != null)
                    progressCallback(-1, "输入文件没有对应的拼接日志文件，全部进行重新投影！");
                return allfiles;
            }
            if (progressCallback != null)
                progressCallback(-1, "解析对应拼接日志文件，获取重新投影数据信息...");
             DateTime[] prjedDays;
             int[] prjedDaysValidPct;
            foreach (DateTime filemonth in monthdirlog.Keys)
            {
                monthlog = monthdirlog[filemonth];
                if (File.Exists(monthlog))
                {
                    prjedDays = TryGetInvalidDayFromMonthlog(monthlog, out prjedDaysValidPct,progressCallback);
                    if (prjedDays != null && prjedDaysValidPct != null && prjedDaysValidPct.Length !=0&&prjedDaysValidPct.Length == prjedDays.Length)
                    {
                        foreach (DateTime dt in filedays.Keys)
                        {
                            if (dt.Year == filemonth.Year && dt.Month == filemonth.Month && dt.Hour == filemonth.Hour&&!prjedDays.Contains(dt))
                                rePrjFiles.AddRange(filedays[dt]);
                        }
                        for (int i = 0; i < prjedDays.Length; i++)
                        {
                            if (prjedDaysValidPct[i] < 90)
                            {
                                rePrjFiles.AddRange(filedays[prjedDays[i]]);
                            }
                        }
                        continue;
                    }
                }
                {//将整个月的数据重投影
                    foreach (DateTime dt in filedays.Keys )
                    {
                        if (dt.Year == filemonth.Year && dt.Month == filemonth.Month && dt.Hour == filemonth.Hour)
                        {
                            rePrjFiles.AddRange(filedays[dt]);
                        }
                    }
                }
            }
            if (progressCallback != null)
                progressCallback(-1, "解析对应拼接日志文件，获取重新投影数据信息完成！共" + rePrjFiles.Count+"个待重新投影文件！");
            return rePrjFiles.ToArray();
        }

        public static DateTime[] TryGetInvalidDayFromMonthlog(string monthlog,out int [] pcts,Action<int, string> progressCallback)
        {
            pcts = null;
            StreamReader sr = null;
            int pct,hourAdded=0;
            DateTime dt;
            string[] parts ;
            List<DateTime> dtlist = new List<DateTime>();
            List<int> pctlist = new List<int>();
            try
            {
                if (Path.GetFileNameWithoutExtension(monthlog).ToLower().Contains("night"))
                    hourAdded=9;
                sr = new StreamReader(monthlog, Encoding.Default);
                while (true)
                {
                    string da = sr.ReadLine();
                    if (da == null || string.IsNullOrWhiteSpace(da))
                        break;
                    try
                    {
                        parts = da.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        //		日期			有效率(%)	5分钟段个数		日产品文件				5分钟段时分
                        //	2011-1-1    	 90	   15	CloudFraction_MYD06_china_day_20110101_0.05.LDF             	0025,0030,0035,0205,0210,0215,0345,0350,0355,0520,0525,0530,0535,0700,0705
                        if (parts == null || parts.Length !=5)
                            continue;
                            if (int.TryParse(parts[1].Replace(" ", ""), out pct))
                        {
                            //timeparts =parts[0].Replace(" ", "").Split('-');
                            //if (timeparts.Length == 3 && int.TryParse(timeparts[0],out year )&&int.TryParse(timeparts[1],out month )&&int.TryParse(timeparts[2],out day ))
                            //{
                            //    pctlist.Add(pct);
                            //    dtlist.Add(new DateTime(year,month,day).AddHours(hourAdded));
                            //}
                            if (DateTime.TryParseExact(parts[0].Replace(" ", ""), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dt))
                            {
                                pctlist.Add(pct);
                                dtlist.Add(dt.Date.AddHours(hourAdded));
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        if (progressCallback != null)
                            progressCallback(-1, ex.Message);
                        continue;
                    }
                }
                pcts = pctlist.ToArray();
                return dtlist.ToArray();
            }
            catch (System.Exception ex)
            {
                if (progressCallback != null)
                    progressCallback(-1, monthlog+"提取拼接日志信息失败！"+ex.Message);
                return null;
            }
        }
        #endregion
    }
}
