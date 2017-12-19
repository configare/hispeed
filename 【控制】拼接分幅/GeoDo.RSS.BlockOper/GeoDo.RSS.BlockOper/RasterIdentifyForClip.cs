using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.BlockOper
{
    /// <summary>
    /// 监测分析栅格数据集标识
    /// </summary>
    public class RasterIdentifyForClip
    {
        private string[] SENSORS = new string[] { "VIRR", "MERSI", "AVHRR", "MODIS", "VISSR", "AMSUB", "MWRID", "MWRIA", "TOUXX", "SBUSX", "IRASX", "ERMXX", "MWTSX", "MWHSX" };
        private string[] SATELLITES = new string[] { "FY1D", "FY2C", "FY2D", "FY2E", "FY2F", "FY3A", "FY3B", "FY3C", "NOAA12", "NOAA14", "NOAA16", "NOAA17", "NOAA18", "NOAA19", "EOSA", "EOST", "MTSAT" };
        private string[] RESOLUTIONS = new string[] { "0250M", "0500M", "1000M", "5000M", "010KM", "015KM" };
        private string[] EXTENSIONS = new string[] { ".GXD", ".GXT", ".XLS", ".XLSX" };
        private string[] CYCFLAGS = new string[] { "POAD", "POSD", "POTD", "POAM", "POAQ", "POAY", "AOAD", "AOSD", "AOTD", "AOAM", "AOAQ", "AOAY", "MAAD", "MASD", "MATD", "MAAM", "MAAQ", "MAAY", "MIAD", "MISD", "MITD", "MIAM", "MIAQ", "MIAY" };
        private string[] REGIONREGEXSTR = new string[] { @"(?<region>洞庭湖流域)",
            @"(?<region>鄱阳湖流域)",
            @"(?<region>0S\d{2}|S\d{2})", 
            @"(?<region>0F\S{2}|F\S{2})_", 
            @"(?<region>EA)_",
            @"(?<region>EB)_",
            @"\S*_(?<region>\S*[\u4E00-\u9FA5])_"};//匹配中文
        private string[] LEVELSTR = new string[] { @"_(?<Level>L\d{1})_" };
        private string[] INVAILDFORMATS = new string[] { ".TXT", ".XLS", ".XLSX" };
        /// <summary>
        /// 专题标识(气象、...)
        /// </summary>
        public string ThemeIdentify;
        /// <summary>
        /// 产品标识(火情、水情、...)
        /// </summary>
        public string ProductIdentify;
        /// <summary>
        /// 产品名（中文）
        /// </summary>
        public string ProductName;
        /// <summary>
        /// 子产品标识(二值图、强度图)
        /// </summary>
        public string SubProductIdentify;
        /// <summary>
        /// 子产名（中文）
        /// </summary>
        public string SubProductName;
        /// <summary>
        /// 卫星标识
        /// </summary>
        public string Satellite;
        /// <summary>
        /// 传感器标识
        /// </summary>
        public string Sensor;
        /// <summary>
        /// 分辨率(米)
        /// eg:500M,1KM,50CM
        /// </summary>
        public string Resolution;
        /// <summary>
        /// 轨道时间
        /// </summary>
        public DateTime OrbitDateTime = DateTime.MinValue;
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime GenerateDateTime = DateTime.Now;

        /// <summary>
        /// FileExtName
        /// eg:  .dat
        /// </summary>
        public string Format = string.Empty;

        /// <summary>
        /// 文件名扩展信息
        /// eg: _minValue_maxValue
        /// </summary>
        public string ExtInfos = string.Empty;

        private string[] _oriFileName;
        /// <summary>
        /// 原始文件名
        /// </summary>
        public string[] OriFileName
        {
            get
            {
                return _oriFileName;
            }
        }

        private List<DateTime> _obritTimes = null;
        /// <summary>
        /// 轨道时间：按传入文件的顺序生成
        /// </summary>
        public DateTime[] ObritTiems
        {
            get
            {
                return _obritTimes == null || _obritTimes.Count == 0 ? null : _obritTimes.ToArray();
            }
        }

        private DateTime _minDate = DateTime.MinValue;
        private DateTime _maxDate = DateTime.MaxValue;
        /// <summary>
        /// 轨道时间：最小时间
        /// </summary>
        public DateTime MinOrbitDate
        {
            get
            {
                return _minDate;
            }
        }
        /// <summary>
        /// 轨道时间：最大时间
        /// </summary>
        public DateTime MaxOrbitDate
        {
            get
            {
                return _maxDate;
            }
        }

        private string _regionIdentify;
        /// <summary>
        /// 区域标识
        /// </summary>
        public string RegionIdentify
        {
            get
            {
                return _regionIdentify;
            }
            set
            {
                _regionIdentify = value;
            }
        }

        /// <summary>
        /// 轨道时间，默认：yyyy年MM月dd日 HH:mm  或 yyyy年MM月dd日 HH:mm ~ yyyy年MM月dd日 HH:mm
        /// </summary>
        public string ObritTimeRegion = string.Empty;
        private string _timeFormat = "yyyy年MM月dd日 HH:mm";

        private string _projectName = "GLL";
        private string _level = "";
        /// <summary>
        /// 产品周期表示
        /// 日：POAD 周：POSD 旬：POTD 月：POAM 季：POAQ 年：POAY 
        /// </summary>
        private string _cycFlag = string.Empty;
        public string CYCFlag
        {
            get { return _cycFlag; }
            set { _cycFlag = value; }
        }

        public bool IsOutput2WorkspaceDir = false;

        public RasterIdentifyForClip()
        {
        }

        public RasterIdentifyForClip(string fname)
        {
            CreateRasterIdentify(fname);
        }

        public RasterIdentifyForClip(IRasterDataProvider raster)
        {
            CreateRasterIdentify(raster);
        }

        private void CreateRasterIdentify(IRasterDataProvider raster)
        {
            CreateRasterIdentifyOnlyFilename(raster.fileName);
            if ((!IsInVaildFormats(Format)) &&
                (string.IsNullOrEmpty(Satellite) || Satellite.ToUpper() == "NUL" || string.IsNullOrEmpty(Sensor) || Sensor.ToUpper() == "NUL"
                || OrbitDateTime == DateTime.MinValue || string.IsNullOrEmpty(Resolution)))
                TryGetInfosFromRasterProvider(raster);
        }

        private void TryGetInfosFromRasterProvider(IRasterDataProvider raster)
        {
            try
            {
                if (raster != null)
                {
                    _projectName = GetProjectionIdentify(raster.SpatialRef.GeographicsCoordSystem.Name);
                    DataIdentify df = raster.DataIdentify;
                    if (string.IsNullOrEmpty(Satellite) || Satellite.ToUpper() == "NUL")
                        Satellite = df.Satellite;
                    if (string.IsNullOrEmpty(Sensor) || Sensor.ToUpper() == "NUL")
                        Sensor = df.Sensor;
                    if (OrbitDateTime == DateTime.MinValue)
                        OrbitDateTime = df.OrbitDateTime;
                    if (string.IsNullOrWhiteSpace(Resolution) || Resolution == "NULL")
                    {
                        Resolution = TryGetResolution(raster.ResolutionX);
                    }
                }
            }
            catch
            {
            }
        }

        private void CreateRasterIdentifyOnlyFilename(string fname)
        {
            if (string.IsNullOrWhiteSpace(fname))
                return;
            _oriFileName = new string[] { fname };
            string filenameNoPath = Path.GetFileName(fname);
            Format = Path.GetExtension(fname);
            TryGetProject(fname);
            TryGetLevel(fname);
            TryGetProductIdentify(fname);
            TryGetRegionIdentify(fname);
            //周期信息
            foreach (string cyc in CYCFLAGS)
            {
                if (filenameNoPath.Contains(cyc))
                {
                    _cycFlag = cyc;
                    break;
                }
            }
            //分辨率
            foreach (string resolution in RESOLUTIONS)
            {
                if (filenameNoPath.Contains(resolution))
                {
                    Resolution = resolution;
                    break;
                }
            }
            filenameNoPath = filenameNoPath.ToUpper();
            //卫星、传感器
            if (string.IsNullOrEmpty(Sensor) || Sensor == "NUL")
                foreach (string sensor in SENSORS)
                {
                    if (filenameNoPath.Contains(sensor))
                    {
                        Sensor = sensor;
                        break;
                    }
                }
            if (string.IsNullOrEmpty(Satellite) || Satellite == "NUL")
                foreach (string satellite in SATELLITES)
                {
                    if (filenameNoPath.Contains(satellite))
                    {
                        Satellite = satellite;
                        break;
                    }
                }
            if (filenameNoPath.Contains("AQUA"))
            {
                Satellite = "EOSA";
                Sensor = "MODIS";
            }
            else if (filenameNoPath.Contains("TERRA"))
            {
                Satellite = "EOST";
                Sensor = "MODIS";
            }
            else if (filenameNoPath.Contains("NA18"))
            {
                Satellite = "NOAA18";
                Sensor = "AVHRR";
            }
            else if (Regex.IsMatch(filenameNoPath, @"^MST\d*_\S*"))
            {
                Satellite = "MTSAT";
            }
            //日期
            if (OrbitDateTime == DateTime.MinValue)
            {
                string exp = @"(?<date>\d{8})_";
                Match m = Regex.Match(filenameNoPath, exp);
                if (m.Success)
                {
                    string strDate = m.Groups["date"].Value;
                    string strTime = null;
                    exp = @"_(?<time>\d{4})_";
                    m = Regex.Match(filenameNoPath, exp);
                    if (m.Success)
                    {
                        strTime = m.Groups["time"].Value;
                    }
                    else
                    {
                        exp = @"_(?<date>\d{8})_(?<time>\d{4})";
                        m = Regex.Match(filenameNoPath, exp);
                        if (m.Success)
                            strTime = m.Groups["time"].Value;
                    }
                    if (strTime == null)
                        strTime = "0000";
                    try
                    {
                        OrbitDateTime = DateTime.Parse(
                            strDate.Substring(0, 4) + "-" +
                            strDate.Substring(4, 2) + "-" +
                            strDate.Substring(6, 2) + " " +
                            strTime.Substring(0, 2) + ":" +
                            strTime.Substring(2, 2) + ":00");
                    }
                    catch
                    {
                        OrbitDateTime = GetProductOrbitDateTime(filenameNoPath);
                        if (OrbitDateTime == DateTime.MinValue)
                            OrbitDateTime = GetOrbitDateTime(filenameNoPath);
                    }
                }
                else
                {
                    OrbitDateTime = GetProductOrbitDateTime(filenameNoPath);
                    if (OrbitDateTime == DateTime.MinValue)
                        OrbitDateTime = GetOrbitDateTime(filenameNoPath);
                }
            }
            //设置时间区间
            if (OrbitDateTime != DateTime.MinValue)
            {
                //日期
                string exp = @"_(?<datetime>\d{14})";
                Match m = Regex.Match(filenameNoPath, exp);
                if (m.Success)
                {
                    Match n = m.NextMatch();
                    if (n.Success)
                    {
                        string strDateTime = n.Groups["datetime"].Value;
                        DateTime maxDate = DateTime.MinValue;
                        if (DateTime.TryParse(
                            strDateTime.Substring(0, 4) + "-" +
                            strDateTime.Substring(4, 2) + "-" +
                            strDateTime.Substring(6, 2) + " " +
                            strDateTime.Substring(8, 2) + ":" +
                            strDateTime.Substring(10, 2) + ":" +
                            strDateTime.Substring(12, 2), out maxDate))
                        {
                            _maxDate = maxDate;
                            _minDate = OrbitDateTime;
                            _obritTimes = new List<DateTime>();
                            _obritTimes.Add(MinOrbitDate);
                            _obritTimes.Add(MaxOrbitDate);
                        }
                    }
                }
            }
        }

        private void TryGetLevel(string fname)
        {
            int length = LEVELSTR.Length;
            string exp;
            for (int i = 0; i < length; i++)
            {
                exp = LEVELSTR[i];
                Match m = Regex.Match(Path.GetFileName(fname), exp);
                if (m.Success)
                {
                    _level = m.Groups["Level"].Value;
                    return;
                }
            }
            _level = string.Empty;
        }

        private void TryGetProject(string fname)
        {
            IRasterDataProvider prd = RasterDataDriver.Open(fname) as IRasterDataProvider;
            try
            {
                if (prd != null)
                    _projectName = GetProjectionIdentify(prd.SpatialRef.GeographicsCoordSystem.Name);
            }
            finally
            {
                if (prd != null)
                    prd.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prjName">
        /// projRef.GeographicsCoordSystem.Name 
        /// projRef.ProjectionCoordSystem.Name.Name</param>
        /// <returns></returns>
        public string GetProjectionIdentify(string prjName)
        {
            switch (prjName)
            {
                case "GCS_WGS_1984":
                case "等经纬度投影":
                case "GLL":
                    return "GLL";
                case "Polar Stereographic":
                    return "PSG";
                case "Albers Conical Equal Area":
                    return "AEA";
                case "Lambert Conformal Conic":
                    return "LBT";
                case "Mercator":
                    return "MCT";
                case "Hammer":
                    return "HAM";
                default:
                    return "PRJ";
            }
        }

        private void CreateRasterIdentify(string fname)
        {
            CreateRasterIdentifyOnlyFilename(fname);
            if ((!IsInVaildFormats(Format)) &&
                (string.IsNullOrEmpty(Satellite) || Satellite.ToUpper() == "NUL" || string.IsNullOrEmpty(Sensor) || Sensor.ToUpper() == "NUL"
                || OrbitDateTime == DateTime.MinValue || string.IsNullOrEmpty(Resolution)))
                TryGetInfosFromRasterProvider(fname);
        }

        private bool IsInVaildFormats(string Format)
        {
            if (INVAILDFORMATS.Contains(Format.ToUpper()))
                return true;
            return false;
        }

        private void TryGetRegionIdentify(string fname)
        {
            int length = REGIONREGEXSTR.Length;
            string exp;
            for (int i = 0; i < length; i++)
            {
                exp = REGIONREGEXSTR[i];
                Match m = Regex.Match(Path.GetFileName(fname), exp);
                if (m.Success)
                {
                    _regionIdentify = m.Groups["region"].Value.Replace("\\", "");
                    if (_regionIdentify.ToUpper() == ProductIdentify)
                        TryGetRegionIdentify(fname.Replace(_regionIdentify, ""));
                    return;
                }
            }
            _regionIdentify = string.Empty;
        }

        private void TryGetInfosFromRasterProvider(string fname)
        {
            string extension = Path.GetExtension(fname).ToUpper();
            foreach (string ext in EXTENSIONS)
                if (extension == ext)
                    return;
            if (!File.Exists(fname))
                return;
            IRasterDataProvider prd = null;
            IGeoDataDriver id = null;
            try
            {
                prd = RasterDataDriver.Open(fname) as IRasterDataProvider;
                if (prd != null)
                {
                    _projectName = GetProjectionIdentify(prd.SpatialRef.GeographicsCoordSystem.Name);
                    DataIdentify df = prd.DataIdentify;
                    if (string.IsNullOrEmpty(Satellite) || Satellite.ToUpper() == "NUL")
                        Satellite = df.Satellite;
                    if (string.IsNullOrEmpty(Sensor) || Sensor.ToUpper() == "NUL")
                        Sensor = df.Sensor;
                    if (OrbitDateTime == DateTime.MinValue)
                        OrbitDateTime = df.OrbitDateTime;
                    if (string.IsNullOrWhiteSpace(Resolution) || Resolution == "NULL")
                    {
                        Resolution = TryGetResolution(prd.ResolutionX);
                    }
                }
            }
            catch
            { }
            finally
            {
                if (prd != null)
                    prd.Dispose();
            }
        }
        //"", "", ""
        private string TryGetResolution(float resolution)
        {
            if (resolution == 0.01f || resolution == 1000f)
                return "1000M";
            else if (resolution == 0.0025f || resolution == 250f)
                return "0250M";
            else if (resolution == 0.005f || resolution == 500f)
                return "0500M";
            else if (resolution == 0.05f || resolution == 5000f)
                return "5000M";
            else if (resolution == 0.1f || resolution == 10000f)
                return "010KM";
            else if (resolution == 0.15f || resolution == 15000f)
                return "015KM";
            else
                return null;
        }

        private static DateTime GetProductOrbitDateTime(string fname)
        {
            //日期
            string exp = @"_(?<datetime>\d{14})";
            Match m = Regex.Match(fname, exp);
            string dateTime = m.NextMatch().Groups["datetime"].Value;
            if (m.Success)
            {
                string strDateTime = m.Groups["datetime"].Value;
                try
                {
                    return DateTime.Parse(
                        strDateTime.Substring(0, 4) + "-" +
                        strDateTime.Substring(4, 2) + "-" +
                        strDateTime.Substring(6, 2) + " " +
                        strDateTime.Substring(8, 2) + ":" +
                        strDateTime.Substring(10, 2) + ":" +
                        strDateTime.Substring(12, 2));
                }
                catch
                {
                }
            }
            return DateTime.MinValue;
        }

        private DateTime GetOrbitDateTime(string fname)
        {
            //日期
            string exp = @"_(?<year>\d{4})_(?<month>\d{2})_(?<day>\d{2})_(?<hour>\d{2})_(?<monutes>\d{2})";
            Match m = Regex.Match(fname, exp);
            if (m.Success)
            {
                try
                {
                    return DateTime.Parse(
                        m.Groups["year"].Value + "-" +
                        m.Groups["month"].Value + "-" +
                        m.Groups["day"].Value + " " +
                        m.Groups["hour"].Value + ":" +
                        m.Groups["monutes"].Value + ":" +
                        "00");
                }
                catch
                {
                }
            }
            return DateTime.MinValue;
        }

        private void TryGetProductIdentify(string fname)
        {
            string filename = Path.GetFileName(fname);
            string extName = Path.GetExtension(filename).ToLower();
            if (extName == ".dat" || extName == ".gxd" || extName == ".xlsx" ||
                extName == ".xls" || extName == ".bmp" || extName == ".gxt" ||
                extName == ".txt" || extName == ".mvg" || extName == ".ldf")
            {
                string[] parts = filename.Split('_');
                if (parts.Length > 2)
                {
                    ProductIdentify = parts[0];
                    SubProductIdentify = parts[1];
                }
            }
        }

        public override string ToString()
        {
            return (ProductIdentify ?? "NULL") +
                "_" + (SubProductIdentify ?? "NULL") +
                (string.IsNullOrEmpty(_regionIdentify) ? "" : ("_" + _regionIdentify)) +
                "_" + (Satellite ?? "NULL") +
                "_" + (Sensor ?? "NULL") +
                "_" + (Resolution ?? "NULL") +
                "_" + GetDateTimeString(OrbitDateTime) +
                "_" + GetDateTimeString(GenerateDateTime) +
                (string.IsNullOrEmpty(CYCFlag) ? "" : "_" + CYCFlag) +
                (string.IsNullOrEmpty(ExtInfos) ? "" : ExtInfos);
        }

        /// <summary>
        /// <产品>_<子产品>[_区域]_<卫星>_<传感器>_<分辨率>_<轨道时间><扩展名>
        /// </summary>
        /// <param name="extName"></param>
        /// <returns></returns>
        public string ToWksFileName(string extName)
        {
            string orbitDataTime;
            string prdIdentify = (ProductIdentify ?? "NULL");
            if (ObritTiems != null && ObritTiems.Length > 1)
            {
                string minDateTime = GetDateTimeString(MinOrbitDate);
                string maxDateTime = GetDateTimeString(MaxOrbitDate);
                if (minDateTime != maxDateTime)
                {
                    orbitDataTime = minDateTime + "_" + maxDateTime;
                }
                else
                    orbitDataTime = minDateTime;
            }
            else
                orbitDataTime = GetDateTimeString(OrbitDateTime);
            //by chennan 按日旬月季年修改产品时间
            UpdateDateByCYC(OrbitDateTime, ref orbitDataTime);
            string outFilename;
            if (prdIdentify == Satellite && SubProductIdentify == Sensor)
            {
                return ((Satellite ?? "NULL") +
                            "_" + (Sensor ?? "NULL") +
                            (string.IsNullOrEmpty(_regionIdentify) ? "" : ("_" + _regionIdentify)) +
                            "_" + _projectName +
                             (string.IsNullOrEmpty(_level) ? "" : "_" + _level) +
                            "_" + orbitDataTime.Substring(0, 8) + "_" + orbitDataTime.Substring(8, 4) +
                            "_" + (Resolution ?? "NULL") +
                           (string.IsNullOrEmpty(CYCFlag) ? "" : "_" + CYCFlag) +
                           (string.IsNullOrEmpty(ExtInfos) ? "" : ExtInfos) +
                           extName);
            }
            return prdIdentify +
                "_" + (SubProductIdentify ?? "NULL") +
                (string.IsNullOrEmpty(_regionIdentify) ? "" : ("_" + _regionIdentify)) +
                "_" + (Satellite ?? "NULL") +
                "_" + (Sensor ?? "NULL") +
                "_" + (Resolution ?? "NULL") +
                "_" + orbitDataTime +
               (string.IsNullOrEmpty(CYCFlag) ? "" : "_" + CYCFlag) +
               (string.IsNullOrEmpty(ExtInfos) ? "" : ExtInfos) +
               extName;
        }

        private void UpdateDateByCYC(DateTime OrbitDateTime, ref string orbitDataTime)
        {
            // 日：POAD 周：POSD 旬：POTD 月：POAM 季：POAQ 年：POAY 
            if (string.IsNullOrEmpty(_cycFlag) || !CYCFLAGS.Contains(_cycFlag)
                || _oriFileName == null || _oriFileName.Length > 1)
                return;
            DateTime dt = OrbitDateTime;
            switch (_cycFlag.ToUpper())
            {
                case "POAD":
                case "AOAD":
                case "MAAD":
                case "MIAD":
                    dt = new DateTime(dt.Year, dt.Month, dt.Day);
                    orbitDataTime = GetDateTimeString(dt);
                    break;
                case "POSD":
                case "AOSD":
                case "MASD":
                case "MISD":
                    dt = GetServenDays(OrbitDateTime);
                    orbitDataTime = GetDateTimeString(dt);
                    break;
                case "POTD":
                case "AOTD":
                case "MATD":
                case "MITD":
                    dt = new DateTime(dt.Year, dt.Month, GetDays010(OrbitDateTime));
                    orbitDataTime = GetDateTimeString(dt);
                    break;
                case "POAM":
                case "AOAM":
                case "MAAM":
                case "MIAM":
                    dt = GetMonth(OrbitDateTime);
                    orbitDataTime = GetDateTimeString(dt);
                    break;
                case "POAQ":
                case "AOAQ":
                case "MAAQ":
                case "MIAQ":
                    dt = GetSeason(OrbitDateTime);
                    orbitDataTime = GetDateTimeString(dt);
                    break;
                case "POAY":
                case "AOAY":
                case "MAAY":
                case "MIAY":
                    dt = GetYear(OrbitDateTime);
                    orbitDataTime = GetDateTimeString(dt);
                    break;
            }
        }

        private int GetDays010(DateTime OrbitDateTime)
        {
            int day = OrbitDateTime.Day;
            if (day > 20)
                return OrbitDateTime.AddDays(1 - day).AddMonths(1).AddDays(-1).Day;
            else if (day > 10)
                return 20;
            else
                return 10;
        }

        private DateTime GetSeason(DateTime OrbitDateTime)
        {
            int month = OrbitDateTime.Month;
            if (month >= 11 || month == 1)
                return new DateTime(OrbitDateTime.AddYears(1).Year, 1, 31);
            else if (month >= 8)
                return new DateTime(OrbitDateTime.Year, 10, 31);
            else if (month >= 5)
                return new DateTime(OrbitDateTime.Year, 7, 31);
            else
                return new DateTime(OrbitDateTime.Year, 4, 30);
        }

        private DateTime GetQuarter(DateTime OrbitDateTime)
        {
            DateTime startQuarter = OrbitDateTime.AddMonths(0 - (OrbitDateTime.Month - 1) % 3).AddDays(1 - OrbitDateTime.Day);  //本季度初
            startQuarter = new DateTime(startQuarter.Year, startQuarter.Month, startQuarter.Day);
            DateTime endQuarter = startQuarter.AddMonths(3).AddDays(-1);  //本季度末
            endQuarter = new DateTime(endQuarter.Year, endQuarter.Month, endQuarter.Day);
            return endQuarter;
        }

        private DateTime GetMonth(DateTime OrbitDateTime)
        {
            DateTime startMonth = OrbitDateTime.AddDays(1 - OrbitDateTime.Day);  //本月月初
            startMonth = new DateTime(startMonth.Year, startMonth.Month, startMonth.Day);
            DateTime endMonth = startMonth.AddMonths(1).AddDays(-1);  //本月月末
            endMonth = new DateTime(endMonth.Year, endMonth.Month, endMonth.Day);
            return endMonth;
        }

        private DateTime GetServenDays(DateTime OrbitDateTime)
        {
            DateTime startWeek = OrbitDateTime.AddDays(1 - Convert.ToInt32(OrbitDateTime.DayOfWeek.ToString("d")));  //本周周一
            startWeek = new DateTime(startWeek.Year, startWeek.Month, startWeek.Day);
            DateTime endWeek = startWeek.AddDays(6);  //本周周日
            endWeek = new DateTime(endWeek.Year, endWeek.Month, endWeek.Day);
            return endWeek;
        }

        private DateTime GetYear(DateTime OrbitDateTime)
        {
            DateTime startYear = new DateTime(OrbitDateTime.Year, 1, 1);  //本年年初
            DateTime endYear = new DateTime(OrbitDateTime.Year, 12, 31);  //本年年末
            return endYear;
        }

        private string GetDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        public RasterIdentifyForClip(string[] files)
        {
            if (files == null || files.Length == 0)
                return;
            if (files.Length == 1)
                CreateRasterIdentify(files[0]);
            else
            {
                RasterIdentifyForClip rid = null;
                CreateRasterIdentify(files[0]);
                foreach (string file in files)
                {
                    rid = new RasterIdentifyForClip(file);
                    GetOrbitDateTime(rid);
                    GetSatellite(rid);
                    GetSensor(rid);
                    TryGetRegionIdentify(rid);
                }
                GetMinMaxTime();
            }
        }

        private void GetMinMaxTime()
        {
            if (_obritTimes == null || _obritTimes.Count == 0)
                return;
            List<DateTime> tempDate = new List<DateTime>();
            tempDate.AddRange(_obritTimes);
            tempDate.Sort();
            _maxDate = tempDate[tempDate.Count - 1];
            _minDate = tempDate[0];
            if (_maxDate.ToString() == _minDate.ToString())
                ObritTimeRegion = _maxDate.ToString(_timeFormat);
            else
                ObritTimeRegion = _minDate.ToString(_timeFormat) + " ~ " + _maxDate.ToString(_timeFormat);
        }

        public Dictionary<DateTime, string> SortByOrbitDate(string[] files)
        {
            if (files == null || files.Length == 0)
                return null;
            Dictionary<DateTime, string> result = new Dictionary<DateTime, string>();
            if (files.Length == 1)
            {
                CreateRasterIdentify(files[0]);
                result.Add(OrbitDateTime, files[0]);
            }
            else
            {
                RasterIdentifyForClip rid = null;
                CreateRasterIdentify(files[0]);
                foreach (string file in files)
                {
                    rid = new RasterIdentifyForClip(file);
                    GetOrbitDateTime(rid);
                    result.Add(rid.OrbitDateTime, file);
                }
            }
            return result.Count == 0 ? null : result.OrderBy(o => o.Key).ToDictionary(o => o.Key, v => v.Value);
        }

        private void GetOrbitDateTime(RasterIdentifyForClip rid)
        {
            if (_obritTimes == null)
                _obritTimes = new List<DateTime>();
            _obritTimes.Add(rid.OrbitDateTime);
            if (rid.OrbitDateTime.ToString("yyyMMdd") == OrbitDateTime.ToString("yyyyMMdd"))
                return;
            else if (rid.OrbitDateTime.ToString("yyyyMM") == OrbitDateTime.ToString("yyyyMM"))
                OrbitDateTime = DateTime.Parse(rid.OrbitDateTime.ToString("yyyy-MM") + "-01");
            else if (rid.OrbitDateTime.ToString("yyyy") == OrbitDateTime.ToString("yyyy"))
                OrbitDateTime = DateTime.Parse(rid.OrbitDateTime.ToString("yyyy") + "-01-01");
        }

        private void GetSatellite(RasterIdentifyForClip rid)
        {
            if (string.IsNullOrEmpty(rid.Satellite) || string.IsNullOrEmpty(Satellite) || rid.Satellite.ToLower() != Satellite.ToLower())
                Satellite = "MULT";
        }

        private void GetSensor(RasterIdentifyForClip rid)
        {
            if (string.IsNullOrEmpty(rid.Sensor) || string.IsNullOrEmpty(Sensor) || rid.Sensor.ToLower() != Sensor.ToLower())
                Sensor = "MULT";
        }

        private void TryGetRegionIdentify(RasterIdentifyForClip rid)
        {
            if (string.IsNullOrEmpty(rid.RegionIdentify) && string.IsNullOrEmpty(RegionIdentify))
                _regionIdentify = "";
            else if ((string.IsNullOrEmpty(rid.RegionIdentify) || string.IsNullOrEmpty(RegionIdentify)) || (rid.RegionIdentify.ToUpper() != RegionIdentify.ToUpper()))
                _regionIdentify = "MULT";
        }
    }
}
