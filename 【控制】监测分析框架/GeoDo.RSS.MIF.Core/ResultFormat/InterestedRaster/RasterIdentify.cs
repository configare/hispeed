using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 监测分析栅格数据集标识
    /// </summary>
    public class RasterIdentify
    {
        private string[] SENSORS = new string[] { "VIRR", "MERSI", "AVHRR", "MODIS", "VISSR", "AMSUB", "MWRID", "MWRIA", "TOUXX", "SBUSX", "IRASX", "ERMXX", "MWTSX", "MWHSX" };
        private string[] SATELLITES = new string[] { "FY1D", "FY2C", "FY2D", "FY2E", "FY2F", "FY3A", "FY3B", "FY3C", "NOAA12", "NOAA14", "NOAA16", "NOAA17", "NOAA18", "NOAA19", "EOSA", "EOST", "MTSAT" };
        private string[] RESOLUTIONS = new string[] { "0250M", "0500M", "1000M", "5000M", "010KM", "015KM", "1250M" };
        private string[] EXTENSIONS = new string[] { ".GXD", ".GXT", ".XLS", ".XLSX" };
        private string[] CYCFLAGS = new string[] { "POAD", "POSD", "POTD", "POAM", "POAQ", "POAY", "AOAD", "AOSD", "AOTD", "AOAM", "AOAQ", "AOAY", "MAAD", "MASD", "MATD", "MAAM", "MAAQ", "MAAY", "MIAD", "MISD", "MITD", "MIAM", "MIAQ", "MIAY" };
        private string[] REGIONREGEXSTR = new string[] { @"_(?<region>洞庭湖流域)",
            @"_(?<region>鄱阳湖流域)",
            @"_(?<region>0S\d{2}|S\d{2})_", 
            @"_(?<region>0F\S{2}|F\S{2})_", 
            @"_(?<region>EA)_",
            @"_(?<region>EB)_",
            @"\S*_(?<region>\S*[\u4E00-\u9FA5]\d*)_", //匹配中文
            @"_(?<region>DXX\d*)_"};
        private string[] INVAILDFORMATS = new string[] { ".TXT", ".XLS", ".XLSX" };
        private string[] TIMEZONES = new string[] { @"_TZ\(UTC(?<TimeSpan>\S*)\)(?<TimeZone>\S*)\.\S+" };
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
            set { _oriFileName = value; }
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
        }

        /// <summary>
        /// 轨道时间，默认：yyyy年MM月dd日 HH:mm  或 yyyy年MM月dd日 HH:mm ~ yyyy年MM月dd日 HH:mm
        /// </summary>
        public string ObritTimeRegion = string.Empty;
        private string _timeFormat = "yyyy年MM月dd日 HH:mm";

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

        private string _timeZone;
        public string TimeZone
        {
            set { _timeZone = value; }
            get { return _timeZone; }
        }

        private string _timeSpan;
        public string TimeSpan
        {
            set { _timeSpan = value; }
            get { return _timeSpan; }
        }

        private string _prjName;
        /// <summary>
        /// 投影名称
        /// </summary>
        public string PrjName
        {
            get { return _prjName; }
            set { _prjName = value; }
        }

        private string[] _sorFiles;
        /// <summary>
        /// 多文件按轨道时间正序排序后结果
        /// </summary>
        public string[] SortFiles
        {
            get { return _sorFiles; }
            set { _sorFiles = value; }
        }

        public RasterIdentify()
        {
        }

        public RasterIdentify(string fname)
        {
            CreateRasterIdentify(fname);
        }

        public RasterIdentify(IRasterDataProvider raster)
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
            TryGetProductIdentify(fname);
            TryGetRegionIdentify(fname);
            TryGetTimeZoneIdentify(fname);
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
            GetMinMaxTimeByOnlyFile(fname);
            bool formInfoFile = TryGetInfosFromInfoFile(fname);
            GetMinMaxTimeByOnlyFile(fname);
            if (formInfoFile)
                return;
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

        private void GetMinMaxTimeByOnlyFile(string fname)
        {
            if (_minDate != DateTime.MinValue || _maxDate != DateTime.MaxValue)
                return;
            string regStr = @"_(?<date1>(?<year1>\d{4})(?<month1>\d{2})(?<day1>\d{2})(?<hour1>\d{2})(?<minute1>\d{2})(?<second1>\d{2}))_(?<date2>(?<year2>\d{4})(?<month2>\d{2})(?<day2>\d{2})(?<hour2>\d{2})(?<minute2>\d{2})(?<second2>\d{2}))";
            Regex reg = new Regex(regStr);
            if (reg.IsMatch(Path.GetFileName(fname).ToUpper()))
            {
                Match m = reg.Match(Path.GetFileName(fname).ToUpper());
                DateTime datetime1 = DateTime.MinValue, datetime2 = DateTime.MaxValue;
                if (DateTime.TryParse(m.Groups["year1"].Value + "-" + m.Groups["month1"].Value + "-" + m.Groups["day1"].Value + " " + m.Groups["hour1"].Value + ":" + m.Groups["minute1"].Value + ":" + m.Groups["second1"].Value, out datetime1)
                 && DateTime.TryParse(m.Groups["year2"].Value + "-" + m.Groups["month2"].Value + "-" + m.Groups["day2"].Value + " " + m.Groups["hour2"].Value + ":" + m.Groups["minute2"].Value + ":" + m.Groups["second2"].Value, out datetime2))
                {
                    _minDate = datetime1 >= datetime2 ? datetime2 : datetime1;
                    _maxDate = datetime1 >= datetime2 ? datetime1 : datetime2;
                }
            }
        }

        private void TryGetTimeZoneIdentify(string fname)
        {
            if (TIMEZONES.Length == 0)
                return;
            Regex regex = null;
            string filenameUpper = Path.GetFileName(fname).ToUpper();
            foreach (string timezoneStr in TIMEZONES)
            {
                regex = new Regex(timezoneStr);
                if (regex.IsMatch(filenameUpper))
                {
                    Match m = regex.Match(filenameUpper);
                    _timeSpan = m.Groups["TimeSpan"].Value;
                    _timeZone = m.Groups["TimeZone"].Value;
                }
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
                    if (_regionIdentify.ToUpper() == ProductIdentify || _regionIdentify.ToUpper() == SubProductIdentify)
                        TryGetRegionIdentify(fname.Replace(_regionIdentify, ""));
                    return;
                }
            }
            ////火情历史数据处理
            //Match mTemp = Regex.Match(Path.GetFileName(fname), @"^(?<region>\S{2})_");
            //if (mTemp.Success)
            //{
            //    _regionIdentify = "0F" + mTemp.Groups["region"].Value.Replace("\\", "");
            //    return;
            //}
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
            else if (resolution == 0.0125f || resolution == 1250f)
                return "1250M";
            else
                return null;
        }

        /*
       <Attribute text="产品类别" identify="SubProductIdentify" format="" visible="true"/>
       <Attribute text="卫星" identify="Satellite" format="" visible="true"/>
       <Attribute text="传感器" identify="Sensor" format="" visible="true"/>
       <Attribute text="轨道时间" identify="OrbitDateTime" format="yyyy-MM-dd HH:mm:ss" visible="true"/>
       <Attribute text="原始文件" identify="SourceFile" format="" visible="true"/>
       <Attribute text="监测区域" identify="RegionIdentify" format="" visible="true"/>
       <Attribute text="描述" identify="Description" format="" visible="true"/>
       <Attribute text="轨道时间分组" identify="OrbitTimeGroup" format="yyyy-MM-dd" visible="true"/>
       <Attribute text="文件名" identify="FileName" format="" visible="true"/>
       <Attribute text="路径" identify="FileDir" format="" visible="true"/>
       <Attribute text="类别中文" identify="CatalogItemCN" format="" visible="false"/>
       <Attribute text="数据集定义" identify="CatalogDef" format="" visible="false"/>
        */
        private bool TryGetInfosFromInfoFile(string fname)
        {
            string infoFile = Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname) + ".INFO");
            if (!File.Exists(infoFile))
                return false;
            CatalogItemInfo info = new CatalogItemInfo(infoFile);
            int flag = 0;
            if (info.Properties.ContainsKey("Satellite"))
            {
                Satellite = info.Properties["Satellite"] == null ? null : info.Properties["Satellite"].ToString();
                flag++;
            }
            if (info.Properties.ContainsKey("Sensor"))
            {
                Sensor = info.Properties["Sensor"] == null ? null : info.Properties["Sensor"].ToString();
                flag++;
            }
            if (info.Properties.ContainsKey("OrbitDateTime"))
            {
                OrbitDateTime = info.Properties["OrbitDateTime"] == null ? DateTime.MinValue : DateTime.Parse(info.Properties["OrbitDateTime"].ToString());
                flag++;
            }
            if (info.Properties.ContainsKey("OrbitTimes"))
            {
                string times = info.Properties["OrbitTimes"].ToString();
                if (!string.IsNullOrEmpty(times))
                {
                    string[] timeArray = times.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (timeArray != null && timeArray.Length == 2)
                    {
                        _minDate = DateTime.Parse(timeArray[0]);
                        _maxDate = DateTime.Parse(timeArray[1]);
                    }
                }
                flag++;
            }
            return flag >= 3 ? true : false;
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
                    GetProductNameByIdentify();
                }
            }
        }

        private void GetProductNameByIdentify()
        {
            if (string.IsNullOrEmpty(ProductIdentify))
                return;
            ThemeDef theme = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            if (theme == null)
                return;
            ProductDef pro = theme.GetProductDefByIdentify(ProductIdentify);
            if (pro == null)
                return;
            ProductName = pro.Name;
            if (string.IsNullOrEmpty(SubProductIdentify))
                return;
            SubProductDef subPro = pro.GetSubProductDefByIdentify(SubProductIdentify);
            if (subPro == null)
                return;
            SubProductName = subPro.Name;
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
            string prdIdentify;
            GetWksFilenameInfo(out orbitDataTime, out prdIdentify);
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

        /// <summary>
        /// <产品>_<子产品>[_区域]_<卫星>_<传感器>[_投影]_<分辨率>_<轨道时间><扩展名>
        /// </summary>
        /// <param name="extName"></param>
        /// <returns></returns>
        public string ToPrjWksFileName(string extName)
        {
            string orbitDataTime;
            string prdIdentify;
            GetWksFilenameInfo(out orbitDataTime, out prdIdentify);
            return prdIdentify +
               "_" + (SubProductIdentify ?? "NULL") +
               (string.IsNullOrEmpty(_regionIdentify) ? "" : ("_" + _regionIdentify)) +
               "_" + (Satellite ?? "NULL") +
               "_" + (Sensor ?? "NULL") +
               "_" + (PrjName ?? "NULL") +
               "_" + (Resolution ?? "NULL") +
               "_" + orbitDataTime +
              (string.IsNullOrEmpty(CYCFlag) ? "" : "_" + CYCFlag) +
              (string.IsNullOrEmpty(ExtInfos) ? "" : ExtInfos) +
              extName;
        }

        private void GetWksFilenameInfo(out string orbitDataTime, out string prdIdentify)
        {

            prdIdentify = (ProductIdentify ?? "NULL");
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

        public string ToWksFullFileName(string extName)
        {
            string dir = GetWksFileDir(extName);
            string fname = ToWksFileName(extName);
            return Path.Combine(dir, fname);
        }

        public string ToPrjWksFullFileName(string extName)
        {
            string dir = GetWksFileDir(extName);
            string fname = ToPrjWksFileName(extName);
            return Path.Combine(dir, fname);
        }

        private string GetWksFileDir(string extName)
        {
            string prdIdentify = (ProductIdentify ?? "NULL");
            string productTime = DateTime.Now.ToString("yyyy-MM-dd");
            string dir = Path.Combine(MifEnvironment.GetWorkspaceDir(), prdIdentify);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            dir = Path.Combine(dir, productTime);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            dir = Path.Combine(dir, GetProductClass(extName));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        private string GetProductClass(string extName)
        {
            extName = extName.ToLower();
            if (extName == ".dat" || extName == ".ldf")
            {
                if (!string.IsNullOrEmpty(_cycFlag) && CYCFLAGS.Contains(_cycFlag.ToUpper()))
                    return "周期产品" + "\\" + _cycFlag.ToUpper();
                return "栅格产品";
            }
            else if (extName == ".xlsx")
                return "统计产品";
            else if (extName == ".gxd" || extName == ".gxt")
                return "专题产品";
            else if (extName == ".bmp" || extName == ".gif" || extName == ".avi")
                return "媒体产品";
            else if (extName == ".txt")
                return "信息列表";
            else if (extName == ".shp")
                return "矢量产品";
            return string.Empty;
        }

        public string ToLongString()
        {
            return ToString();
        }

        public string ToShortString()
        {
            return ToString();
        }

        private string GetDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        public RasterIdentify(string[] files)
        {
            if (files == null || files.Length == 0)
                return;
            SortedDictionary<DateTime, string> dateFileDic = new SortedDictionary<DateTime, string>();
            if (files.Length == 1)
            {
                CreateRasterIdentify(files[0]);
                dateFileDic.Add(OrbitDateTime, files[0]);
            }
            else
            {
                RasterIdentify rid = null;
                CreateRasterIdentify(files[0]);
                foreach (string file in files)
                {
                    rid = new RasterIdentify(file);
                    GetOrbitDateTime(rid);
                    GetSatellite(rid);
                    GetSensor(rid);
                    TryGetRegionIdentify(rid);
                    if (!dateFileDic.ContainsKey(rid.OrbitDateTime))
                        dateFileDic.Add(rid.OrbitDateTime, file);
                }
                GetMinMaxTime();
                //by chennan 修改积雪天数统计图无法生成问题
                //dateFileDic.Clear();
            }
            GetSortFiles(dateFileDic);
        }

        private void GetSortFiles(SortedDictionary<DateTime, string> dateFileDic)
        {
            if (dateFileDic.Count == 0)
                return;
            List<string> result = new List<string>();
            foreach (DateTime item in dateFileDic.Keys)
                result.Add(dateFileDic[item]);
            _sorFiles = result.ToArray();
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
                RasterIdentify rid = null;
                CreateRasterIdentify(files[0]);
                foreach (string file in files)
                {
                    rid = new RasterIdentify(file);
                    GetOrbitDateTime(rid);
                    result.Add(rid.OrbitDateTime, file);
                }
            }
            return result.Count == 0 ? null : result.OrderBy(o => o.Key).ToDictionary(o => o.Key, v => v.Value);
        }

        private void GetOrbitDateTime(RasterIdentify rid)
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

        private void GetSatellite(RasterIdentify rid)
        {
            if (string.IsNullOrEmpty(rid.Satellite) || string.IsNullOrEmpty(Satellite) || rid.Satellite.ToLower() != Satellite.ToLower())
                Satellite = "MULT";
        }

        private void GetSensor(RasterIdentify rid)
        {
            if (string.IsNullOrEmpty(rid.Sensor) || string.IsNullOrEmpty(Sensor) || rid.Sensor.ToLower() != Sensor.ToLower())
                Sensor = "MULT";
        }

        private void TryGetRegionIdentify(RasterIdentify rid)
        {
            if (string.IsNullOrEmpty(rid.RegionIdentify) && string.IsNullOrEmpty(RegionIdentify))
                _regionIdentify = "";
            else if ((string.IsNullOrEmpty(rid.RegionIdentify) || string.IsNullOrEmpty(RegionIdentify)) || (rid.RegionIdentify.ToUpper() != RegionIdentify.ToUpper()))
                _regionIdentify = "MULT";
        }

        public void SetRegionIdentify(string regionIdentify)
        {
            _regionIdentify = regionIdentify;
        }
    }
}
