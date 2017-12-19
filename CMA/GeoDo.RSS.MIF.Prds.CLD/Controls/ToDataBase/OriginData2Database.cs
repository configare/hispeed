using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using GeoDo.FileProject;
using SharpCompress.Reader;
using SharpCompress.Common;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class OriginData2Database
    {
        private Dictionary<string, string> _originFile2BaseTable = new Dictionary<string, string>();
        private Dictionary<string, List<string>> _originFiles2Base = new Dictionary<string, List<string>>();
        private string _inputDir;
        private bool _isOverrideRecord;
        private Dictionary<string, String> _CloudSATRawPrdsComments = new Dictionary<string, string>();
        private static ConnectMySqlCloud _dbConnect;
        public Action<string> _calProBack=null;
        Dictionary<string, List<string>> _uniformOriginFiles = new Dictionary<string, List<string>>();
        private string _path = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\Mod06DataProcess.xml";// + 
        public static RasterProject.PrjEnvelope _chinaENV = null;
        public bool IsFiles2UniformDir = true;
        public string _DocDir = "";

        public OriginData2Database()
        {
            InitializeComponent();
        }

        public OriginData2Database(string inputDir, Dictionary<string, List<string>> originFiles2Base, bool isOverrideRecord,Action<string> calProBack)
        {
            InitializeComponent();
            _uniformOriginFiles = originFiles2Base;
            _inputDir = inputDir;
            _isOverrideRecord = isOverrideRecord;
            _dbConnect = new ConnectMySqlCloud();
            _calProBack = calProBack;
            if (File.Exists(_path))
            {
                InputArg arg = InputArg.ParseXml(_path);
                if (arg != null && arg.ValidEnvelopes != null && arg.ValidEnvelopes.Length > 0)
                    _chinaENV = arg.ValidEnvelopes[0].PrjEnvelope;
            }
            if (_chinaENV == null)
                _chinaENV = new RasterProject.PrjEnvelope(65, 145, 10, 60);
        }

        private void InitializeComponent()
        {
            _originFile2BaseTable.Add("ISCCP", "CP_ISCCPD2_TB".ToLower());
            _originFile2BaseTable.Add("MOD06", "CP_MODISMOD06_TB".ToLower());
            _originFile2BaseTable.Add("AIRS", "CP_AIRS_TB".ToLower());
            _originFile2BaseTable.Add("CloudSAT", "CP_CloudSat_TB".ToLower());
            InitializeCloudSATrawPrdsComments();
        }

        private void InitializeCloudSATrawPrdsComments()
        {
            _CloudSATRawPrdsComments.Add("1B-CPR", "Level 1B Received echo powers");
            _CloudSATRawPrdsComments.Add("2B-GEOPROF", "Cloud mask and radar reflectivities");
            _CloudSATRawPrdsComments.Add("2B-CLDCLASS", "Cloud Classification");
            _CloudSATRawPrdsComments.Add("2B-CWC-RO", "Radar-only liquid/ice water content");
            _CloudSATRawPrdsComments.Add("2B-TAU", "Cloud optical depth");
            _CloudSATRawPrdsComments.Add("2B-CWC-RVOD", "Radar + visible optical depth liquid/ice water content");
            _CloudSATRawPrdsComments.Add("2B-FLXHR", "Radiative fluxes and heating rates");
            _CloudSATRawPrdsComments.Add("2B-GEOPROF-Lidar".ToUpper(), "CloudSat CPR + CALIPSO Lidar Cloud mask");
            _CloudSATRawPrdsComments.Add("MODIS-AUX", "MODIS Auxiliary Data(Data from 22 MODIS channels)");
            _CloudSATRawPrdsComments.Add("ECMWF-AUX", "ECMWF Auxiliary Data(State Variables)");
            _CloudSATRawPrdsComments.Add("2C-PRECIP-COLUMN", "Precipitation Column precipitation");
        }

        public void CheckFile2Table()
        {
            try
            {
                #region 数据归档
                List<string> unifiles = new List<string>();
                if (IsFiles2UniformDir)
                {
                    foreach (string mode in _uniformOriginFiles.Keys)
                    {
                        if (_calProBack != null)
                            _calProBack(string.Format("正在归档{0}数据，请稍候...", mode));
                        unifiles.Clear();
                        unifiles = Files2UniformDir(_uniformOriginFiles[mode], _inputDir, mode, _calProBack);
                        _originFiles2Base.Add(mode, unifiles.ToList());
                    }
                }
                else
                {
                    string imagedata = null;
                    foreach (string mode in _uniformOriginFiles.Keys)
                    {
                        unifiles.Clear();
                        foreach (string file in _uniformOriginFiles[mode])
                        {
                            imagedata = file.Replace(_DocDir, "");
                            if (!unifiles.Contains(imagedata))
                                unifiles.Add(imagedata);
                        }
                        _originFiles2Base.Add(mode, unifiles.ToList());
                    }
                }
                #endregion

                foreach (string label in _originFiles2Base.Keys)
                {
                    if (label.Contains("ISCCP"))
                    {
                        ISCCPRawDataF2Database(_originFile2BaseTable[label], _originFiles2Base[label], _calProBack);
                    }
                    else if (label.Contains("MOD06"))
                    {
                        MODRawDataF2Database(_originFile2BaseTable[label], _originFiles2Base[label], _calProBack);
                    }
                    else if (label.Contains("AIRS"))
                    {
                        AIRSRawDataF2Database(_originFile2BaseTable[label], _originFiles2Base[label], _calProBack);
                    }
                    else if (label.Contains("CloudSAT"))
                    {
                        CloudSATRawDataF2Database(_originFile2BaseTable[label], _originFiles2Base[label], _calProBack);
                    }
                }
                _calProBack("处理结束！");
            }
            catch (System.Exception ex)
            {
                _calProBack(ex.Message);
            }
        }

        public static List<string> Files2UniformDir(List<string> files, string dir, string mode, Action<string> state)
        {
            string sensorDir = Path.Combine(dir, mode.ToUpper().Contains("MOD0") ? "MODIS" : mode.ToUpper());
            switch (mode.ToUpper())
            {
                case "MOD06":
                case "MOD03":
                case "AIRS":
                case "ISCCP":
                case "CLOUDSAT":
                    return Uniformfiles2Dir(files, sensorDir, mode, state);
                default:
                    return Uniformfiles2Dir(files, dir, mode, state);
            }
        }

        private static void UnzipCloudSATFiles(string zipfile, string unzipdir, Action<string> state)
        {
            string hdfFile =Path.GetFileName(zipfile.Substring(0, zipfile.Length - 4));
            if (!hdfFile.ToLower().EndsWith(".hdf"))
            {
                if (state != null)
                    state("\t文件格式有误！");
                return;
            }
            string dsthdfFile = Path.Combine(unzipdir, hdfFile);
            if (File.Exists(dsthdfFile))
            {
                //if (state != null)
                //    state("\thdf文件已存在，跳过解压！");
                return;
            }
            //if (state != null)
            //    state("\t开始解压hdf文件...");
            try
            {
                using (Stream stream = File.OpenRead(zipfile))
                {
                    if (stream == null)
                        return ;
                    using (var reader = ReaderFactory.Open(stream))
                    {
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                //Console.WriteLine(reader.Entry.FilePath);
                                reader.WriteEntryToDirectory(unzipdir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (state != null)
                    state(zipfile + "解压失败," + ex.Message);
                else
                    throw new FileLoadException(zipfile + "解压失败," + ex.Message);
            }
        }

        /// <summary>
        /// 将扫描得到的原始数据文件归档
        /// </summary>
        /// <param name="files"></param>
        /// <param name="dir"></param>
        /// <param name="mode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static List<string> Uniformfiles2Dir(List<string> files, string dir, string mode, Action<string> state)
        {
            List<string> filesDir = new List<string>();
            string dstDir = "",fname,dstfname,tempf,tempdstf;
            DateTime fdate = DateTime.MinValue;
            StringBuilder fn, dstfn, dstDirB;            
            foreach (string file in files)
            {
                fname = Path.GetFileName(file);
                if (mode.ToUpper() == "AIRS")
                {
                    #region 筛选中国区的AIRS文件
                    RasterProject.PrjEnvelope env = AIRSDataProcesser.GetAIRSFileEnv(file);
                    GeoDo.RasterProject.PrjEnvelope dstmainPrj = GeoDo.RasterProject.PrjEnvelope.Intersect(_chinaENV, env);
                    if (dstmainPrj == null || dstmainPrj.Width <= 0 || dstmainPrj.Height <= 0)
                    {
                        if (state != null)
                            state("文件不在中国区！" + fname);
                        continue;
                    }
                    #endregion
                }
                if (state != null)
                    state("开始归档" + fname + "...");
                fdate = GetFileDateTime(fname, mode);
                dstDirB = new StringBuilder(dir);
                dstDirB.Append("\\").Append(fdate.Year.ToString()).Append("\\").Append(fdate.Month.ToString());
                dstDir = dstDirB.ToString();
                if (!Directory.Exists(dstDir))
                    Directory.CreateDirectory(dstDir);
                dstfname = dstDirB.Append("\\").Append(fname).ToString(); //Path.Combine(dstDir, fname);
                //文件复制
                if (mode.ToUpper() != "CLOUDSAT")
                {
                    if (dstfname != file&&!File.Exists(dstfname))
                        System.IO.File.Copy(file, dstfname);
                    else
                    {
                        if (state != null)
                            state("文件已存在或源数据位置与目标数据位置相同，跳过归档！");
                    }
                    if (!filesDir.Contains(dstfname))
                        filesDir.Add(dstfname);
                    if (mode == "AIRS")
                    {
                        fn = new StringBuilder(file);
                        dstfn = new StringBuilder(dstfname);
                        tempf = fn.Append(".xml").ToString();
                        tempdstf = dstfn.Append(".xml").ToString();
                        if (File.Exists(tempf) && !File.Exists(tempdstf))
                            System.IO.File.Copy(tempf, tempdstf, true);
                        tempf = fn.Append(".jpg").ToString();
                        tempdstf = dstfn.Append(".jpg").ToString();
                        if (File.Exists(tempf) && !File.Exists(tempdstf))
                            System.IO.File.Copy(tempf, tempdstf, true);
                        tempf = fn.Append(".map.gz").ToString();
                        tempdstf = dstfn.Append(".map.gz").ToString();
                        if (File.Exists(tempf) && !File.Exists(tempdstf))
                            System.IO.File.Copy(tempf, tempdstf, true);
                        //if (File.Exists(file + ".jpg") && !File.Exists(dstfname + ".jpg"))
                        //    System.IO.File.Copy(file + ".jpg", dstfname + ".jpg", true);
                        //if (File.Exists(file + ".map.gz") && !File.Exists(dstfname + ".map.gz"))
                        //    System.IO.File.Copy(file + ".map.gz", dstfname + ".map.gz", true);
                    }
                } 
                else
                {
                    UnzipCloudSATFiles(file, dstDir, state);
                    filesDir.Add(dstfname.Substring(0, dstfname.Length - 4));
                }
                if (state != null)
                    state("归档完成！");
            }
            if (state != null)
                state(mode + "数据归档完成！");
            return filesDir;
        }

        private static DateTime GetFileDateTime(string fname, string mode)
        {
            switch (mode.ToUpper())
            {
                case "MOD06":
                    string dt;
                    return DataProcesser.GetOribitTime(fname, out dt);
                case "MOD03":
                    string dt3;
                    return DataProcesser.GetOribitTime(fname, out dt3);
                case "AIRS":
                    string outstr;
                    return AIRSDataProcesser.GetAIRSDateTimeFromStr(fname, out outstr);
                case "ISCCP":
                    string isccpregex = @".(?<year>\d{4}).(?<month>\d{2}).(?<day>\d{2}).(?<utc>\d{4})";
                    Match match = Regex.Match(fname, isccpregex);
                    if (match.Success)
                    {
                        int year = int.Parse(match.Groups["year"].Value);
                        int month = int.Parse(match.Groups["month"].Value);
                        return new DateTime(year, month, 1);
                    }
                    return DateTime.MinValue;
                case "CLOUDSAT":
                    string[] parts = fname.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    return OriginData2Database.GetInfoFromCloudSATName(parts[0]);
                default:
                    return DateTime.MinValue;
            }
        }

        private void ISCCPRawDataF2Database(string tableName, List<string> filelist,Action<string> ProBack)
        {
            ProBack("-------------------------------------------------");
            ProBack("ISCCP原始数据入库开始：");
            //ISCCP.D2.0.GLOBAL.2008.07.99.0000.GPC
            string fullDir,  fName, localfName;
            int year, month, utcTime;
            DateTime fDateTime = DateTime.MinValue;
            string pattern =@".(?<year>\d{4}).(?<month>\d{2}).(?<day>\d{2}).(?<utc>\d{4})";
            foreach (string file in filelist)
            {
                fName = Path.GetFileName(file);
                ProBack("\t" + fName + "入库开始：");
                fullDir = Path.GetDirectoryName(file);
                if (Directory.Exists(new DirectoryInfo(_inputDir).Parent.FullName))
                    localfName = file.Replace(new DirectoryInfo(_inputDir).Parent.FullName, "");
                else
                    localfName = file.Replace(_inputDir, "");
                Match match = Regex.Match(fName, pattern);
                if (match.Success)
                {
                    //LogFactory.WriteLine(_log, "\t\t解析文件名时间信息成功！");
                    //ProBack("\t\t解析文件名时间信息成功！");
                    year = int.Parse(match.Groups["year"].Value);
                    month = int.Parse(match.Groups["month"].Value);
                    fDateTime = new DateTime(year, month, 1);
                    utcTime = int.Parse(match.Groups["utc"].Value);
                    try
                    {
                        //if (_dbConnect.IshasRecord(tableName, "ImageName", fName) == false)
                        if (_dbConnect.IshasRecord(tableName, "ImageData", localfName) == false)
                        {
                            _dbConnect.InsertISCCP2Table(tableName, fDateTime, utcTime, fName, localfName);  //插入,insert
                            ProBack("\t\t\t文件入库成功！");
                            continue;
                        }
                        else
                        {
                            //if(_dbConnect.UpdateRawCLDParatable(tableName, fName, localfName))
                            //    ProBack("\t\t\t文件记录更新成功！");
                            //else
                                ProBack("\t\t\t文件记录已存在，跳过！");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ProBack("入库错误" + ex.Message);
                    }
                }
                else
                    ProBack("\t\t解析文件名时间失败，文件入库失败！");
            }
            ProBack("ISCCP原始数据入库结束！");
            ProBack("--------------------------------------------------------------------------------------");
        }

        private void MODRawDataF2Database(string tableName, List<string> filelist, Action<string> ProBack)
        {
            //MOD06_L2.A2011001.0350.hdf
            //MOD03.A2011001.0350.hdf
            ProBack("--------------------------------------------------------------------------------------");
            ProBack("MOD06数据入库开始：");
            string  fName, localfName;
            DateTime fDateTime = DateTime.MinValue;
            string sensor = "MODIS";
            string dt;
            foreach (string file in filelist)
            {
                fName = Path.GetFileName(file);
                ProBack("\t" + fName + "入库开始：");
                fDateTime = DataProcesser.GetOribitTime(file, out dt);
                if (fDateTime == DateTime.MinValue||string.IsNullOrEmpty(dt))
                {
                    ProBack("\t\t解析文件名时间信息失败，入库失败！");
                    continue;
                }
                if (Directory.Exists(new DirectoryInfo(_inputDir).Parent.FullName))
                    localfName = file.Replace(new DirectoryInfo(_inputDir).Parent.FullName, "");//Path.Combine(localDir, fName);
                else
                    localfName = file.Replace(_inputDir, "");
                try
                {
                    //if (_dbConnect.IshasRecord(tableName, "ImageName", fName) == false)
                    if (_dbConnect.IshasRecord(tableName, "ImageData", localfName) == false)
                    {
                        _dbConnect.InsertRawMOD06Table(tableName, fDateTime, fName, localfName, sensor);  //插入,insert
                        ProBack("\t\t\t文件入库成功！");
                    }
                    else
                    {
                        //if (_dbConnect.UpdateRawCLDParatable(tableName, fName, localfName))
                        //    ProBack("\t\t\t文件记录更新成功！");
                        //else
                            ProBack("\t\t\t文件记录已存在，跳过！");
                    }
                }
                catch (System.Exception ex)
                {
                    ProBack("入库错误" + ex.Message);
                }
            }
            ProBack("MOD06数据入库结束！");
            ProBack("--------------------------------------------------------------------------------------");
        }

        private void AIRSRawDataF2Database(string tableName, List<string> filelist, Action<string> ProBack)
        {
            ProBack("-------------------------------------------------");
            ProBack("AIRS原始数据入库开始：");
            string fName, localfName;
            DateTime fDateTime = DateTime.MinValue;
            string sensor = "AIRS", dirname, dataStr;
            foreach (string file in filelist)
            {
                try
                {
                    fName = Path.GetFileName(file);
                    ProBack("\t" + fName + "入库开始：");
                    dirname =new DirectoryInfo(_inputDir).Parent.FullName;
                    if (Directory.Exists(dirname))
                        localfName = file.Replace(dirname, "");//Path.Combine(localDir, fName);
                    else
                        localfName = file.Replace(_inputDir, "");//Path.Combine(localDir, fName);
                    fDateTime = AIRSDataProcesser.GetOribitTime(file, out dataStr);
                    if (fDateTime == DateTime.MinValue)
                    {
                        ProBack("\t\t解析文件名时间信息失败，入库失败！");
                        continue;
                    }
                    //ProBack("查询记录存在否开始...");
                    //if (_dbConnect.IshasRecord(tableName, "ImageName", fName) == false)
                    if (_dbConnect.IshasRecord(tableName, "ImageData", localfName) == false)
                    {
                        //ProBack("查询完毕，开始插入新纪录...");
                        _dbConnect.InsertRawMOD06Table(tableName, fDateTime, fName, localfName, sensor);  //插入,insert
                        ProBack("\t\t\t文件入库成功！");
                    }
                    else
                    {
                        //ProBack("查询完毕，纪录已存在，开始尝试更新记录...");
                        //if (_dbConnect.UpdateRawCLDParatable(tableName, fName, localfName))
                        //    ProBack("\t\t\t文件记录更新成功！");
                        //else
                        ProBack("\t\t\t文件记录已存在，跳过！");
                    }
                }
                catch (System.Exception ex)
                {
                    ProBack("入库错误"+ex.Message);
                }
            }
            ProBack("AIRS L2数据入库结束！");
            ProBack("--------------------------------------------------------------------------------------");
        }

        private void CloudSATRawDataF2Database(string tableName, List<string> filelist, Action<string> ProBack)
        {
            ProBack("--------------------------------------------------------------------------------------");
            ProBack("CloudSAT原始数据入库开始：");
            string fName, localfName, prdsIndicate, datatype;
            int granuleNO;
            DateTime fDateTime = DateTime.MinValue;
            string[] fNameParts;
            long clousatID=0;
            Dictionary<string, long> file2ID = new Dictionary<string, long>();
            foreach (string file in filelist)
            {
                #region 文件入库
                try
                {
                    fName = Path.GetFileName(file);
                    ProBack("\t" + fName + "入库开始：");
                    //2007101052404_05066_CS_MODIS-AUX_GRANULE_P_R04_E02.hdf
                    fNameParts = Path.GetFileNameWithoutExtension(file).Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fNameParts.Length != 8 || (fNameParts.Length>=2&&fNameParts[2] != "CS"))
                    {
                        ProBack("\t文件名不规范，入库失败！");
                        continue;
                    }
                    if (Directory.Exists(new DirectoryInfo(_inputDir).Parent.FullName))
                        localfName = file.Replace(new DirectoryInfo(_inputDir).Parent.FullName, "");//Path.Combine(localDir, fName);
                    else
                        localfName = file.Replace(_inputDir, "");//Path.Combine(localDir, fName);
                    fDateTime = GetInfoFromCloudSATName(fNameParts[0]);
                    if (fDateTime == DateTime.MinValue)
                    {
                        ProBack("\t\t解析文件名时间信息失败，入库失败！");
                        continue;
                    }
                    granuleNO = int.Parse(fNameParts[1]);
                    prdsIndicate = fNameParts[3].ToUpper();
                    datatype = fNameParts[4];
                    //if (_dbConnect.IshasRecord(tableName, "ImageName", fName) == false)//新增
                    if (_dbConnect.IshasRecord(tableName, "ImageData", localfName) == false)//新增
                    {
                        clousatID = _dbConnect.InsertCloudSAT2Table(tableName, fDateTime, fName, localfName, granuleNO, prdsIndicate, datatype);  //插入,insert
                        ProBack("\t\t\t文件记录入库成功！");
                        if (clousatID != 0)
                        {
                            if (!file2ID.ContainsKey(file))
                            {
                                file2ID.Add(file, clousatID);
                            }
                        }
                        clousatID = 0;
                    }
                    else
                    {
                        //_dbConnect.DeleteCLDParatableRecord(tableName, "ImageName", fName);
                        //clousatID = _dbConnect.InsertCloudSAT2Table(tableName, fDateTime, fName, localfName, granuleNO, prdsIndicate, datatype);  //插入,insert
                        //ProBack("\t\t\t文件记录更新成功！");
                        //if (clousatID != 0)
                        //{
                        //    if (!file2ID.ContainsKey(file))
                        //    {
                        //        file2ID.Add(file, clousatID);
                        //    }
                        //}
                        //clousatID = 0;
                        //if (_dbConnect.UpdateRawCLDParatable(tableName, fName, localfName))
                        //    ProBack("\t\t\t文件记录更新成功！");
                        //else
                        ProBack("\t\t\t文件记录已存在，跳过！");
                    }
                }
                catch (System.Exception ex)
                {
                    ProBack(file+"入库失败！"+ex.Message);
                }
                #endregion
            }
            try
            {
                _dbConnect.UpdateLink();
                //ProBack("CloudSAT文件与区域关联记录更新开始：");
                #region CloudSAT与region关联
                if (file2ID.Count != 0)
                {
                    Dictionary<long, PrjEnvelopeItem> allRegions = _dbConnect.GetAllRegion();
                    ConnectCloudSAT2Region c2r = new ConnectCloudSAT2Region(file2ID, allRegions);
                    c2r.Compute(ProBack);
                }
                ProBack("CloudSAT文件与区域关联更新完成！");
                #endregion
            }
            catch (System.Exception ex)
            {
                ProBack("错误！" +ex.Message);
            }
            ProBack("CloudSAT原始数据入库结束！");
            ProBack("--------------------------------------------------------------------------------------");
        }

        public static DateTime GetInfoFromCloudSATName(string fNameDatestr)
        {
            //2007101052404
            try
            {
                int year, dayofyear, hour, min, sec;
                DateTime fDate;
                year = int.Parse(fNameDatestr.Substring(0, 4));
                dayofyear = int.Parse(fNameDatestr.Substring(4, 3));
                hour = int.Parse(fNameDatestr.Substring(7, 2));
                min = int.Parse(fNameDatestr.Substring(9, 2));
                sec = int.Parse(fNameDatestr.Substring(11, 2));
                fDate = new DateTime(year, 1, 1).AddDays(dayofyear - 1).AddHours(hour).AddMinutes(min).AddSeconds(sec);
                return fDate;
            }
            catch (System.Exception ex)
            {
                return DateTime.MinValue;
            }
        }
    }
}
