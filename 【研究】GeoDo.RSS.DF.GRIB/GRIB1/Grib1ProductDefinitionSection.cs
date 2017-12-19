using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public class Grib1ProductDefinitionSection
    {
        private int _sectionLength;      //PDS段字节长度
        private int _decscale;           //缩放比例
        private int _gridId;             //格网类型ID号
        private int _tableVersion;       //参数集版本号
        /// <summary> 中心的编号</summary>
        private int _centerId;
        /// <summary> Identification of Generating Process.</summary>
        private int _processId;
        private bool _gdsExists;         //若GDS存在，则为真
        private bool _bmsExists;         //若BMS存在，则为真
        private int _parameterNumber;    //参数(要素)索引号
        private GribPDSLevel _level;     //层包含的一些信息
        private int _timeUnit;           //时间单位
        /// <summary> 
        /// Strings used in building a string to represent the time(s) for this PDS
        /// See the decoder for octet 21 to get an understanding.
        /// </summary>
        private string _timeRange = null;
        private int _timeRangeValue;
        private int _forecastTime;      //预报时间
        private DateTime _referenceTime;
        private GribPDSParamTable _parameterTable;
        private Parameter _parameter;//参数集中参数
        /// <summary> Identification of subcenter.</summary>
        private int _subcenterId;

        /// <summary>中心编号</summary>
        public int Center
        {
            get { return _centerId; }
        }

        /// <summary> Process Id </summary>
        public int Process_Id
        {
            get { return _processId; }
        }
        /// <summary> Grid ID as int.</summary>
        public int Grid_Id
        {
            get { return _gridId; }
        }
        /// <summary> SubCenter as int.</summary>
        public int SubCenter
        {
            get { return _subcenterId; }
        }
        /// <summary> gets the Table version as a int.</summary>
        public int TableVersion
        {
            get { return _tableVersion; }
        }
        /// <summary> 
        /// 获取缩放比例
        /// </summary>
        public int DecimalScale
        {
            get { return _decscale; }
        }

        /// <summary> 
        /// 获取参数
        /// </summary>
        public Parameter Parameter
        {
            get { return _parameter; }
        }

        /// <summary>获取预测或分析的层</summary>
        /// <returns> 层（高度或压力）</returns>
        public GribPDSLevel Level
        {
            get { return _level; }
        }

        /// <summary> 
        /// 获取预报的基准时间（起报时间）
        /// </summary>
        public DateTime ReferenceTime
        {
            get { return _referenceTime; }
        }

        /// <summary> 
        /// 获取预报时间
        /// </summary>
        public int ForecastTime
        {
            get { return _forecastTime; }
        }
        /// <summary> 
        /// 获取时间单位
        /// </summary>
        public int TimeUnitValue
        {
            get { return _timeUnit; }
        }
        /// <summary> 
        /// 获取时间单位
        /// </summary>
        public string TimeUnit
        {
            get { return GetTimeUnitString(); }
        }

        /// <summary> ProductDefinition as a int.</summary>
        /// <returns> timeRangeValue </returns>
        public int ProductDefinition
        {
            get { return _timeRangeValue; }
        }
        /// <summary>
        /// 时间范围(整数）
        /// </summary>
        public int TimeRange
        {
            get { return _timeRangeValue; }
        }
        /// <summary>
        /// 时间范围(字符串)
        /// </summary>
        public string TimeRangeString
        {
            get { return _timeRange; }
        }

        public int RefTimeT
        {
            get
            {
                DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan rt = _referenceTime - startTime;
                int t = Convert.ToInt32(rt.TotalSeconds);
                return t;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="raf">with PDS content</param>
        public Grib1ProductDefinitionSection(FileStream fs)
        {
            long position = fs.Position;
            // octets 1-3 PDS length
            _sectionLength = (int)GribNumberHelper.Uint3(fs);
            // Paramter table octet 4
            _tableVersion = fs.ReadByte();
            // Center  octet 5
            _centerId = fs.ReadByte();
            // octet 6 Generating Process - See Table A
            _processId = fs.ReadByte();
            // octet 7 (id of grid type) - not supported yet
            _gridId = fs.ReadByte();
            //octet 8 (flag for presence of GDS and BMS)
            SetGdsBmsExits(fs.ReadByte());
            // octet 9 (parameter and unit)
            _parameterNumber = fs.ReadByte();

            // octets 10-12 (level)
            int levelType = fs.ReadByte();
            int levelValue1 = fs.ReadByte();
            int levelValue2 = fs.ReadByte();
            _level = new GribPDSLevel(levelType, levelValue1, levelValue2);

            // octets 13-17 (base time for reference time)依次为year/month/day/hour/minute
            byte[] timeBytes = new Byte[5];
            fs.Read(timeBytes, 0, 5);

            // octet 18 Forecast time unit
            _timeUnit = fs.ReadByte();

            // octet 19 & 20 used to create Forecast time
            int p1 = fs.ReadByte();
            int p2 = fs.ReadByte();

            // octet 21 (time range indicator)
            _timeRangeValue = fs.ReadByte();
            // forecast time is always at the end of the range
            SetTimeRange(p1, p2);

            // octet 22 & 23
            int avgInclude = GribNumberHelper.Int2(fs);

            // octet 24
            int avgMissing = fs.ReadByte();

            // octet 25
            int century = fs.ReadByte() - 1;

            // octet 26, sub center
            _subcenterId = fs.ReadByte();

            // octets 27-28 (decimal scale factor)
            _decscale = GribNumberHelper.Int2(fs);
            SetReferenceTime(timeBytes, century);
            _parameterTable = GribPDSParamTable.GetParameterTable(_centerId, _subcenterId, _tableVersion);
            if (_parameterTable == null)
                _parameterTable = GribPDSParamTable.GetDefaultParameterTable();
            _parameter = _parameterTable.GetParameter(_parameterNumber);
            fs.Seek(position + _sectionLength, SeekOrigin.Begin);
        }

        private void SetReferenceTime(byte[] timeBytes, int century)
        {
            int year = timeBytes[0];
            int month = timeBytes[1];
            int day = timeBytes[2];
            int hour = timeBytes[3];
            int minute = timeBytes[4];
            _referenceTime = new DateTime(century * 100 + year, month, day, hour, minute, 0, DateTimeKind.Utc);
        }

        private void SetTimeRange(int p1, int p2)
        {
            switch (_timeRangeValue)
            {
                case 0:
                    _timeRange = "product valid at RT + P1";
                    _forecastTime = p1;
                    break;

                case 1:
                    _timeRange = "product valid for RT, P1=0";
                    _forecastTime = 0;
                    break;

                case 2:
                    _timeRange = "product valid from (RT + P1) to (RT + P2)";
                    _forecastTime = p2;
                    break;

                case 3:
                    _timeRange = "product is an average between (RT + P1) to (RT + P2)";
                    _forecastTime = p2;
                    break;

                case 4:
                    _timeRange = "product is an accumulation between (RT + P1) to (RT + P2)";
                    _forecastTime = p2;
                    break;

                case 5:
                    _timeRange = "product is the difference (RT + P2) - (RT + P1)";
                    _forecastTime = p2;
                    break;

                case 6:
                    _timeRange = "product is an average from (RT - P1) to (RT - P2)";
                    _forecastTime = -p2;
                    break;

                case 7:
                    _timeRange = "product is an average from (RT - P1) to (RT + P2)";
                    _forecastTime = p2;
                    break;

                case 10:
                    _timeRange = "product valid at RT + P1";
                    // p1 really consists of 2 bytes p1 and p2
                    _forecastTime = GribNumberHelper.Int2(p1, p2);
                    break;

                case 51:
                    _timeRange = "mean value from RT to (RT + P2)";
                    _forecastTime = p2;
                    break;

                default:
                    break;
            }
        }

        private void SetGdsBmsExits(int exists)
        {
            _gdsExists = (exists & 128) == 128;
            _bmsExists = (exists & 64) == 64;
        }

        private string GetTimeUnitString()
        {
            string timeUnitStr = string.Empty;
            switch (_timeUnit)
            {
                case 0:
                    timeUnitStr = "minute";
                    break;
                case 1:
                    timeUnitStr = "hour";
                    break;

                case 2:
                    timeUnitStr = "day";
                    break;

                case 3:
                    timeUnitStr = "month";
                    break;

                case 4:
                    timeUnitStr = "1year";
                    break;

                case 5:  // 十年
                    timeUnitStr = "decade";
                    break;

                case 6:
                    timeUnitStr = "day";
                    break;

                case 7:
                    timeUnitStr = "century";
                    break;

                case 10:
                    timeUnitStr = "3hours";
                    break;

                case 11:
                    timeUnitStr = "6hours";
                    break;

                case 12:
                    timeUnitStr = "12hours";
                    break;

                case 254:
                    timeUnitStr = "second";
                    break;
            }
            return timeUnitStr;
        }

        /// <summary>
        /// 检查GDS是否存在
        /// </summary>
        public bool GdsExists()
        {
            return _gdsExists;
        }
        /// <summary>
        /// 检查BMS是否存在
        /// </summary>
        public bool BmsExists()
        {
            return _bmsExists;
        }

        private static String GetCenterName(int center)
        {
            switch (center)
            {
                case 0: return "WMO Secretariat";

                case 1:
                case 2: return "Melbourne";

                case 4:
                case 5: return "Moscow";

                case 7: return "US National Weather Service (NCEP)";

                case 8: return "US National Weather Service (NWSTG)";

                case 9: return "US National Weather Service (other)";

                case 10: return "Cairo (RSMC/RAFC)";

                case 12: return "Dakar (RSMC/RAFC)";

                case 14: return "Nairobi (RSMC/RAFC)";

                case 16: return "Atananarivo (RSMC)";

                case 18: return "Tunis Casablanca (RSMC)";

                case 20: return "Las Palmas (RAFC)";

                case 21: return "Algiers (RSMC)";

                case 22: return "Lagos (RSMC)";

                case 24: return "Pretoria (RSMC)";

                case 25: return "La R?union (RSMC)";

                case 26: return "Khabarovsk (RSMC)";

                case 28: return "New Delhi (RSMC/RAFC)";

                case 30: return "Novosibirsk (RSMC)";

                case 32: return "Tashkent (RSMC)";

                case 33: return "Jeddah (RSMC)";

                case 34: return "Tokyo (RSMC), Japan Meteorological Agency";

                case 36: return "Bangkok";

                case 37: return "Ulan Bator";

                case 38: return "Beijing (RSMC)";

                case 40: return "Seoul";

                case 41: return "Buenos Aires (RSMC/RAFC)";

                case 43: return "Brasilia (RSMC/RAFC)";

                case 45: return "Santiago";

                case 46: return "Brazilian Space Agency ? INPE";

                case 51: return "Miami (RSMC/RAFC)";

                case 52: return "Miami RSMC, National Hurricane Center";

                case 53:
                case 54: return "Montreal (RSMC)";

                case 55: return "San Francisco";

                case 57: return "Air Force Weather Agency";

                case 58: return "Fleet Numerical Meteorology and Oceanography Center";

                case 59: return "The NOAA Forecast Systems Laboratory";

                case 60: return "United States National Centre for Atmospheric Research (NCAR)";

                case 64: return "Honolulu";

                case 65: return "Darwin (RSMC)";

                case 67: return "Melbourne (RSMC)";

                case 69: return "Wellington (RSMC/RAFC)";

                case 71: return "Nadi (RSMC)";

                case 74: return "UK Meteorological Office Bracknell (RSMC)";

                case 76: return "Moscow (RSMC/RAFC)";

                case 78: return "Offenbach (RSMC)";

                case 80: return "Rome (RSMC)";

                case 82: return "Norrkoping";

                case 85: return "Toulouse (RSMC)";

                case 86: return "Helsinki";

                case 87: return "Belgrade";

                case 88: return "Oslo";

                case 89: return "Prague";

                case 90: return "Episkopi";

                case 91: return "Ankara";

                case 92: return "Frankfurt/Main (RAFC)";

                case 93: return "London (WAFC)";

                case 94: return "Copenhagen";

                case 95: return "Rota";

                case 96: return "Athens";

                case 97: return "European Space Agency (ESA)";

                case 98: return "ECMWF, RSMC";

                case 99: return "De Bilt";

                case 110: return "Hong-Kong";

                case 210: return "Frascati (ESA/ESRIN)";

                case 211: return "Lanion";

                case 212: return "Lisboa";

                case 213: return "Reykjavik";

                case 254: return "EUMETSAT Operation Centre";


                default: return "Unknown";
            }
        }

        /// <summary> SubCenter as String.</summary>
        /// <param name="center">中心编号</param>
        public string GetSubCenterIdName(int center)
        {
            if (_centerId == 7)
            {
                //NWS
                switch (center)
                {
                    case 0: return "WMO Secretariat";

                    case 1: return "NCEP Re-Analysis Project";

                    case 2: return "NCEP Ensemble Products";

                    case 3: return "NCEP Central Operations";

                    case 4: return "Environmental Modeling Center";

                    case 5: return "Hydrometeorological Prediction Center";

                    case 6: return "Marine Prediction Center";

                    case 7: return "Climate Prediction Center";

                    case 8: return "Aviation Weather Center";

                    case 9: return "Storm Prediction Center";

                    case 10: return "Tropical Prediction Center";

                    case 11: return "NWS Techniques Development Laboratory";

                    case 12: return "NESDIS Office of Research and Applications";

                    case 13: return "FAA";

                    case 14: return "NWS Meteorological Development Laboratory";

                    case 15: return " The North American Regional Reanalysis (NARR) Project";
                }
            }
            return GetCenterName(center);
        }

        /// <summary>定义的结果名称</summary>
        /// <param name="type"></param>
        public static string GetProductDefinitionName(int type)
        {
            switch (type)
            {
                case 0: return "Forecast/Uninitialized Analysis/Image Product";

                case 1: return "Initialized analysis product";

                case 2: return "Product with a valid time between P1 and P2";

                case 3:
                case 6:
                case 7: return "Average";

                case 4: return "Accumulation";

                case 5: return "Difference";

                case 10: return "product valid at reference time P1";

                case 51: return "Climatological Mean Value";

                case 113:
                case 115:
                case 117: return "Average of N forecasts";

                case 114:
                case 116: return "Accumulation of N forecasts";

                case 118: return "Temporal variance";

                case 119:
                case 125: return "Standard deviation of N forecasts";

                case 123: return "Average of N uninitialized analyses";

                case 124: return "Accumulation of N uninitialized analyses";

                case 128: return "Average of daily forecast accumulations";

                case 129: return "Average of successive forecast accumulations";

                case 130: return "Average of daily forecast averages";

                case 131: return "Average of successive forecast averages";

                case 132: return "Climatological Average of N analyses";

                case 133: return "Climatological Average of N forecasts";

                case 134: return "Climatological Root Mean Square difference between N forecasts and their verifying analyses";

                case 135: return "Climatological Standard Deviation of N forecasts from the mean of the same N forecasts";

                case 136: return "Climatological Standard Deviation of N analyses from the mean of the same N analyses";
            }
            return "Unknown";
        }
    }
}
