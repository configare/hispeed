using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.NOAA
{
    public class D1BDHeader
    {
        private bool _isBigEndian = true;
        private CommonInfoFor1BD _commonInfoFor1BD = null;
        private QualityCheckInfoFor1BD _qualityCheckInfoFor1BD = null;
        private ScaleInfoFor1BD _sacleInfoFor1BD = null;
        private RadiantionConvertArgsInfoFor1BD _radiantionConvertArgsInfoFor1BD = null;
        private GeographLocationInfoFor1BD _geographLocationInfoFor1BD = null;
        private SimluateRemoteMeasureInfoFor1BD _simluateRemoteMeasureInfoFor1BD = null;
        private NomalHeaderInfo _nomalHeaderInfo = null;

        public D1BDHeader(object commonInfoFor1BD, object qualityCheckInfoFor1BD,
            object scaleInfoFor1BD, object radiantionConvertArgsInfoFor1BD,
            object geographEnvelopeInfoFor1BD, 
            object simluateRemoteMeasureMeasureInfoFor1BD,
            object nomalHeaderInfo,bool isBigEndian)
        {
            _commonInfoFor1BD = (CommonInfoFor1BD)commonInfoFor1BD;
            _qualityCheckInfoFor1BD = (QualityCheckInfoFor1BD)qualityCheckInfoFor1BD;
            _sacleInfoFor1BD = (ScaleInfoFor1BD)scaleInfoFor1BD;
            _radiantionConvertArgsInfoFor1BD = (RadiantionConvertArgsInfoFor1BD)radiantionConvertArgsInfoFor1BD;
            _geographLocationInfoFor1BD = (GeographLocationInfoFor1BD)geographEnvelopeInfoFor1BD;
            _simluateRemoteMeasureInfoFor1BD = (SimluateRemoteMeasureInfoFor1BD)simluateRemoteMeasureMeasureInfoFor1BD;
            _nomalHeaderInfo = (NomalHeaderInfo)nomalHeaderInfo;
            _isBigEndian = isBigEndian;
        }

        public bool IsBigEndian
        {
            get { return _isBigEndian; }
        }

        public CommonInfoFor1BD CommonInfoFor1BD
        {
            get { return _commonInfoFor1BD; }
        }

        public QualityCheckInfoFor1BD QualityCheckInfoFor1BD
        {
            get { return _qualityCheckInfoFor1BD; }
        }

        public ScaleInfoFor1BD SacleInfoFor1BD
        {
            get { return _sacleInfoFor1BD; }
        }

        public RadiantionConvertArgsInfoFor1BD RadiantionConvertArgsInfoFor1BD
        {
            get { return _radiantionConvertArgsInfoFor1BD; }
        }

        public GeographLocationInfoFor1BD GeographLocationInfoFor1BD
        {
            get { return _geographLocationInfoFor1BD; }
        }

        public SimluateRemoteMeasureInfoFor1BD SimluateRemoteMeasureInfoFor1BD
        {
            get { return _simluateRemoteMeasureInfoFor1BD; }
        }

        public NomalHeaderInfo NomalHeaderInfo
        {
            get { return _nomalHeaderInfo; }
        }

        public static bool Is1BD(byte[] bytes)
        {
            if (ToLocalEndian_Core.ToInt16FromBig(new byte[] { bytes[10], bytes[11] }) != 22016
                && ToLocalEndian_Core.ToInt16FromLittle(new byte[] { bytes[10], bytes[11] }) != 22016)
                return false;
            return true;
        }
    }

    /// <summary>
    /// 一般信息
    /// </summary>
    public class CommonInfoFor1BD
    {
        /// <summary>
        /// 1-3：数据集生成地点的缩写符号
        /// </summary>
        public string FileGenPlace = null;
        /// <summary>
        /// 4：ASCII空格符
        /// </summary>
        public char ASCIIBank = '\0';
        /// <summary>
        /// 5-6：1B格式版本号
        /// </summary>
        public UInt16 Version = UInt16.MaxValue;
        /// <summary>
        /// 7-8：1B格式版本发布的年号（year）
        /// </summary>
        public UInt16 VersionYear = UInt16.MaxValue;
        /// <summary>
        /// 9-10：1B格式版本发布的年日（day of year）
        /// </summary>
        public UInt16 VersionDay = UInt16.MaxValue;
        /// <summary>
        /// 11-12：逻辑记录长度，仅用于数据集生成地点
        /// </summary>
        public UInt16 LogicalRecordLen = UInt16.MaxValue;
        /// <summary>
        /// 13-14：记录块长，仅用于数据集生成地点
        /// </summary>
        public UInt16 RecordBlockLen = UInt16.MaxValue;
        /// <summary>
        /// 15-16：头记录所占的记录个数
        /// </summary>
        public UInt16 HeaderRecordCount = UInt16.MaxValue;
        /// <summary>
        /// 17-22：填充符
        /// </summary>
        public UInt16[] FillFlag17_22 = null;
        /// <summary>
        /// 23-64: 数据集名
        /// </summary>
        public string DatasetName = null;
        /// <summary>
        /// 65-72: 处理块识别标志
        /// </summary>
        public string HandleBlockFlag = null;
        /// <summary>
        /// 73-74: NOAA卫星识别码（4 = NOAA-15）
        /// </summary>
        public UInt16 SatelliteIdentify = UInt16.MaxValue;
        /// <summary>
        /// 75-76：仪器识别符
        /// </summary>
        public UInt16 SensorIdentify = UInt16.MaxValue;// 0:AVHRR
        /// <summary>
        /// 77-78：数据类型（1 = LAC；2 = GAC；3 = HRPT；4 = TIP；5 = HIRS；6 = MSU；7 = SSU；8 = DCS；9 = SEM；10 = AMSU-A；11 = AMSU=B）
        /// </summary>
        public UInt16 DataType = UInt16.MaxValue;
        /// <summary>
        /// 79-80：TIP原码（0 = 无效的；1 = 嵌于GAC中AMSU和TIP；2 = 存储的TIP； 3 = 嵌于HRPT中AMSU 和TIP；4=存储的AIP）
        /// </summary>
        public UInt16 TIPCode = UInt16.MaxValue;
        /// <summary>
        /// 81-84：开始日（从1950年1月1日00时开始）
        /// </summary>
        public UInt32 BeginDayFrom1950 = UInt32.MaxValue;
        /// <summary>
        /// 85-86：开始年（year）
        /// </summary>
        public UInt16 DataBeginYear = UInt16.MaxValue;
        /// <summary>
        /// 87-88：开始日（day of year）
        /// </summary>
        public UInt16 DataBeginDayOfYear = UInt16.MaxValue;
        /// <summary>
        /// 89-92：开始世界时（UTC）（time of day；毫秒）
        /// </summary>
        public UInt32 DataBeginUTC = UInt32.MaxValue;
        /// <summary>
        /// 93-96：结束日（从1950年1月1日00时开始）
        /// </summary>
        public UInt32 EndDayFrom1950 = UInt32.MaxValue;
        /// <summary>
        /// 97-98：结束年（year）
        /// </summary>
        public UInt16 DataEndYear = UInt16.MaxValue;
        /// <summary>
        /// 99-100：结束日（day of year）
        /// </summary>
        public UInt16 DataEndDayOfYear = UInt16.MaxValue;
        /// <summary>
        /// 101-104：结束世界时（UTC）（time of day；毫秒）
        /// </summary>
        public UInt32 DataEndUTC = UInt32.MaxValue;

        public string SatelliteName = string.Empty;

        public string SensorName = string.Empty;

        public bool Current3A = false;

        public bool CurrentRise = false;

        public long FullRecordCount = 0;

        public DateTime OrbitBeginTime = DateTime.MinValue;
    }

    /// <summary>
    /// 数据质量标记信息
    /// </summary>
    public class QualityCheckInfoFor1BD
    {
        /// <summary>
        /// 117-120  1bit    
        /// </summary>
        public bool UsedMotor = false;//0=off，1=on； 0=false,1=true;

        public bool UsedElectron = false;//0=off，1=on； 0=false,1=true;

        public bool EnableChannel1 = false;//0=不可用；1=可用； 0=false,1=true;

        public bool EnableChannel2 = false;//0=不可用；1=可用 ；0=false,1=true;

        public bool EnableChannel3A = false;//0=不可用；1=可用 ；0=false,1=true;

        public bool EnableChannel3B = false;//0=不可用；1=可用 ；0=false,1=true;

        public bool EnableChannel4 = false;//0=不可用；1=可用 ；0=false,1=true;

        public bool EnableChannel5 = false;//0=不可用；1=可用 ；0=false,1=true;

        public bool Used3AOr3B = false;//

        public bool VStandard = false;//0=off，1=on ；0=false,1=true;

        public bool CoolerHot = false;//冷却器热（0=off；1=on）； 0=false,1=true;

        public bool ScanMotor = false;//扫描马达（0=low；1=high） ；0=false,1=true;

        public bool RemotesensingLock = false;//遥测锁定（0=off；1=on）； 0=false,1=true;

        public bool EarthShadow = false;//地球阴影（0=off；1=on）； 0=false,1=true;

        public bool PatchControl = false;//0=off,1=on； 0=false,1=true;
        /// <summary>
        /// 123-124: 状态变化记录数
        /// </summary>
        public UInt16 StateChangeCount = UInt16.MaxValue;// 0 = 没变化
        /// <summary>
        /// 125-128: 第二个仪器的状态 
        /// </summary>
        public UInt32 SecondYQState = UInt32.MaxValue;//如前一个字为0，则没变化
        /// <summary>
        /// 129-130: 本数据集中的扫描线数
        /// </summary> 
        public UInt16 RecordCount = UInt16.MaxValue;
        /// <summary>
        /// 131-132: 本数据集中已定标、定位的扫描线数
        /// </summary>
        public UInt16 MarkRecordCount = UInt16.MaxValue;
        /// <summary>
        /// 133-134: 丢失的扫描线数
        /// </summary>
        public UInt16 LostRecordCount = UInt16.MaxValue;
        /// <summary>
        /// 135-136: 本数据集中的数据空隙记数
        /// </summary>
        public UInt16 DataKXCount = UInt16.MaxValue;
        /// <summary>
        /// 137-138: 没有帧同步码错误的数据帧记数
        /// </summary>
        public UInt16 ErrorFrameCount = UInt16.MaxValue;
        /// <summary>
        /// 139-140: 检测TIP校验错误的PACS记数 
        /// </summary>
        public UInt16 ErrorTIPCount = UInt16.MaxValue;
        /// <summary>
        /// 141-142: 输入数据中检测到的辅助同步错误码总数
        /// </summary>
        public UInt16 ErrorAssistCount = UInt16.MaxValue;
        /// <summary>
        /// 143-144: 时间顺序错误
        /// </summary>
        public UInt16 ErrorTimeOrder = UInt16.MaxValue;//0 = 没有；否则为首次发生的记录数
        /// <summary>
        /// 145-146: 时间顺序错误码
        /// </summary>
        public UInt16 ErrorTimeOrderCode = UInt16.MaxValue;
        /// <summary>
        /// 147-148: SOCC时钟更新标志
        /// </summary>
        public UInt16 UpdateSOCC = UInt16.MaxValue;//0 = 没有；否则为首次发生的记录数
        /// <summary>
        /// 149-150: 地球定位错误标志
        /// </summary>
        public UInt16 ErrorEarthLocation = UInt16.MaxValue;//0 = 没有；否则为首次发生的记录数
        /// <summary>
        /// 151-152: 地球定位错误码
        /// </summary>
        public UInt16 ErrorEarthLocationCode = UInt16.MaxValue;
        /// <summary>
        /// 153-154: PACS状态
        /// </summary>
        public bool DataMode = false;//0 = test data ; 1 = flight data  0=false,1=true;
        public bool TapeDirection = false;//0 = time decrementing   0=false
        public bool PseudoNoise = false;//0 = normal data; 1 = P/N data  0=false,1=true;
        /// <summary>
        /// 155-156: PACS数据源
        /// </summary>
        public UInt16 PACSource = UInt16.MaxValue; //0 = unsed；1 = Gilomore；2 = Wallops；3 = SOCC
    }

    /// <summary>
    /// 定标信息
    /// </summary>
    public class ScaleInfoFor1BD
    {
        /// <summary>
        /// 189-190: 最近的太阳通道定标年
        /// </summary>
        public UInt16 NearSunCHScaleYear = UInt16.MaxValue;
        /// <summary>
        /// 191-192: 最近的太阳通道定标日
        /// </summary>
        public UInt16 NearSunChScaleDay = UInt16.MaxValue;
    }

    /// <summary>
    /// 辐射转换系数
    /// </summary>
    public class RadiantionConvertArgsInfoFor1BD
    {
        /// <summary>
        /// 257-260：CH 1太阳滤波辐照度
        /// </summary>
        public double SunFilterRadiantionCH1 = -1;
        /// <summary>
        /// 261-264：CH 1等效滤波宽度
        /// </summary>
        public double EquivalentFilterWidthCH1 = -1;
        /// <summary>
        /// 265-268：CH 2太阳滤波辐照度
        /// </summary>
        public double SunFilterRadiantionCH2 = -1;
        /// <summary>
        /// 269-272：CH 2等效滤波宽度
        /// </summary>
        public double EquivalentFilterWidthCH2 = -1;
        /// <summary>
        /// 273-276: CH 3a太阳滤波辐照度
        /// </summary>
        public double SunFilterRadiantionCH3A = -1;
        /// <summary>
        /// 277-280: CH 3a等效滤波宽度
        /// </summary>
        public double EquivalentFilterWidthCH3A = -1;
        /// <summary>
        /// 281-284: 通道CH 3b中心波数
        /// </summary>
        public double CenterWaveNumberCH3B = -1;
        /// <summary>
        /// 285-288: 通道CH 3b常数c1
        /// </summary>
        public double C1ConstCH3B = -1;
        /// <summary>
        /// 289-292: 通道CH 3b常数c2
        /// </summary>
        public double C2ConstCH3B = -1;
        /// <summary>
        /// 293-296: 通道CH 4中心波数
        /// </summary>
        public double CenterWaveNumberCH4 = -1;
        /// <summary>
        /// 297-300: 通道CH 4常数c1
        /// </summary>
        public double C1ConstCH4 = -1;
        /// <summary>
        /// 301-304: 通道CH 4常数c2
        /// </summary>
        public double C2ConstCH4 = -1;
        /// <summary>
        /// 305-308: 通道CH 5中心波数
        /// </summary>
        public double CenterWaveNumberCH5 = -1;
        /// <summary>
        /// 309-312: 通道CH 5常数c1
        /// </summary>
        public double C1ConstCH5 = -1;
        /// <summary>
        /// 313-316: 通道CH 5常数c2
        /// </summary>
        public double C2ConstCH5 = -1;
    }

    /// <summary>
    /// 地理定位信息
    /// </summary>
    public class GeographLocationInfoFor1BD
    {
        /// <summary>
        /// 329-336: 参考椭圆模式标识号
        /// </summary>
        public string ReferenceEllipse = string.Empty;
        /// <summary>
        /// 337-338: 星下点地球定位差范围
        /// </summary>
        public double EarthLocationGap = -1;
    }

    /// <summary>
    /// 模拟遥测转换
    /// </summary>
    public class SimluateRemoteMeasureInfoFor1BD
    {
    }
}
