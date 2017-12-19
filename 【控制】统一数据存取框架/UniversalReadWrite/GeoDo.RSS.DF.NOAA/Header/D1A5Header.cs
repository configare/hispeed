using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.NOAA
{
    public class D1A5Header
    {
        /// <summary>
        /// 1-2:卫星识别码
        /// </summary>
        public UInt16 SatelliteIdentify = UInt16.MaxValue;
        
        /// <summary>
        /// 3-4:开始时间年
        /// </summary>
        public UInt16 DataBeginYear = UInt16.MaxValue;

        /// <summary>
        /// 5-8：开始时间毫秒
        /// </summary>
        public UInt32 DataBeginMilliSecond = UInt32.MaxValue;

        /// <summary>
        /// 9-10：开始时间日记数
        /// </summary>
        public UInt16 DataBeginDayNums = UInt16.MaxValue;

        /// <summary>
        /// 11-12：结束时间年
        /// </summary>
        public UInt16 DataEndYear = UInt16.MaxValue;

        /// <summary>
        /// 13-16：结束时间毫秒
        /// </summary>
        public UInt32 DataEndMilliSecond = UInt32.MaxValue;

        /// <summary>
        ///17-18：结束时间日记数
        /// </summary>
        public UInt16 DataEndDayNums = UInt16.MaxValue;

        /// <summary>
        /// 19-20：好扫描线数
        /// </summary>
        public UInt16 RecordCount = UInt16.MaxValue;

        /// <summary>
        /// 21-22：末扫描线序号
        /// </summary>
        public UInt16 LastRecord = UInt16.MaxValue;

        ///<summary>
        /// 23-24:同步码错数
        ///</summary>
        public UInt16 ErrorFrameCount = UInt16.MaxValue;
        /// <summary>
        /// 25-26：误码率数
        /// </summary>
        public UInt16 BitErrorRatio = UInt16.MaxValue;

        /// <summary>
        /// 29-30：时序错数
        ///</summary>
        public UInt16 ErrorTimeOrder = UInt16.MaxValue;

        /// <summary>
        /// 31-32：扫描线丢失次数
        /// </summary>
        public UInt16 LostRecordCount = UInt16.MaxValue;

        ///<summary>
        /// 33-34：斜坡分析结果
        ///</summary>
        public UInt16 SlopeAnalyseResult = UInt16.MaxValue;

        ///<summary>
        /// 37-116:定标系数和标准差
        ///</summary>
       

        /// <summary>
        /// 119-120:轨道序号
        /// </summary>
        public UInt16 TrackNumber = UInt16.MaxValue;

        ///<summary>
        ///121-128:历元时间
        ///</summary>
        public UInt64 Time = UInt64.MaxValue;

        ///<summary>
        ///129-136:轨道半长轴
        /// </summary>

        ///<summary>
        ///137-144：轨道偏心率
        /// </summary>

        ///<summary>
        ///145-152：轨道斜角
        /// </summary>

        ///<summary>
        ///153-160：轨道升焦点赤经
        /// </summary>
        
        ///<summary>
        ///161-168:轨道近地点幅角
        /// </summary>

        ///<summary>
        ///169-176:轨道平近点角
        /// </summary>

        ///<summary>
        ///177-178:升降轨标记：1升轨0降轨
        /// </summary>
        public UInt16 AscDescendTag = UInt16.MaxValue;
        ///<summary>
        ///179-180：定位所用资料类型
        ///</summary>

        ///<summary>
        ///181 -182:历元轨道序号
        /// </summary>

        ///<summary>
        ///183 -190:卫星轨道周期
        /// </summary>
        
        ///<summary>
        ///193-204:四个角度信息：姿态角；滚动角；俯仰角；偏航角
        /// </summary>

        ///<summary>
        ///224-256:四个角点的地理位置
        /// </summary>

        /// <summary>
        /// 257-264：开始时间（从80年起以秒为单位）
        /// </summary>
        public UInt64 DataBeginSecond = UInt64.MaxValue;

        /// <summary>
        /// 265-272：结束时间（从80年起以秒为单位）
        /// </summary>
        public UInt64 DataEndSecond = UInt64.MaxValue;

        public string SatelliteName = string.Empty;

        //public string SensorName = string.Empty;

        public static bool Is1A5(byte[] bytes, string fileExtension)
        {
            if (fileExtension != ".1A5")
                return false;
            ushort satelliteIdentify=ToLocalEndian_Core.ToUInt16FromBig(new byte[] { bytes[0], bytes[1] });
            if ((satelliteIdentify != 3) && (satelliteIdentify != 5) && (satelliteIdentify != 7) && (satelliteIdentify != 9))
                return false;
            return true;
        }
    }
}
