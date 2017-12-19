#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/27 11:20:13
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.FY1D
{
    /// <summary>
    /// 类名：D1A5Header
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/27 11:20:13
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class D1A5Header
    {
        /// <summary>
        /// 1-2:卫星识别码
        /// </summary>
        public UInt16 SatelliteIdentify = UInt16.MaxValue;

        /// <summary>
        /// 3-4:观测开始时间年
        /// </summary>
        public UInt16 DataBeginYear = UInt16.MaxValue;

        /// <summary>
        /// 5-8：观测开始时间毫秒
        /// </summary>
        public UInt32 DataBeginMilliSecond = UInt32.MaxValue;

        /// <summary>
        /// 9-10：观测开始时间日记数
        /// </summary>
        public UInt16 DataBeginDayNums = UInt16.MaxValue;

        /// <summary>
        /// 11-12：观测结束时间年
        /// </summary>
        public UInt16 DataEndYear = UInt16.MaxValue;

        /// <summary>
        /// 13-16：观测结束时间毫秒
        /// </summary>
        public UInt32 DataEndMilliSecond = UInt32.MaxValue;

        /// <summary>
        ///17-18：观测结束时间日记数
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
        /// 37-100:定标系数和标准差
        ///</summary>


        /// <summary>
        /// 199-200:轨道序号
        /// </summary>
        public UInt16 TrackNumber = UInt16.MaxValue;

        ///<summary>
        ///201-208:历元时间
        ///</summary>
        public DateTime Time = DateTime.MaxValue;

        public Double EpochTrackTime = Double.MaxValue;

        ///<summary>
        ///209-216:轨道半长轴
        /// </summary>
        public Double OrbitSemiMajorAxis = Double.MaxValue;

        ///<summary>
        ///217-224：轨道偏心率
        /// </summary>
        public Double OrbitEccentricity = Double.MaxValue;

        ///<summary>
        ///225-232：轨道倾角
        /// </summary>
        public Double OrbitInclination = Double.MaxValue;

        ///<summary>
        ///233-240：轨道升交点赤经
        /// </summary>
        public Double LongitudeAscendingNode = Double.MaxValue;

        ///<summary>
        ///241-248:轨道近地点幅角
        /// </summary>
        public Double PerigeeAngle = Double.MaxValue;
        ///<summary>
        ///249-256:轨道平近点角
        /// </summary>
        public Double MeanAnomaly = Double.MaxValue;

        ///<summary>
        ///257-258:升降轨标记：1升轨0降轨
        /// </summary>
        public UInt16 AscDescendTag = UInt16.MaxValue;

        ///<summary>
        ///259-260：定位所用资料类型
        ///</summary>
        public UInt16 ResurceType = UInt16.MaxValue;

        ///<summary>
        ///261 -262:历元轨道序号
        /// </summary>
        public UInt16 OrbitNumber = UInt16.MaxValue;

        ///<summary>
        ///265 -272:卫星轨道周期
        /// </summary>
        public Double OrbitCycle = Double.MaxValue;

        ///<summary>
        ///273-296:三个角度信息：姿态角/滚动角；俯仰角；偏航角
        /// </summary>
        public double[] Angles = new double[3];

        ///<summary>
        ///317-348:四个角点的地理位置(左下 右下  左上  右上)
        /// </summary>
        public float[] Lats = new float[4];

        public float[] Lons = new float[4];

        /// <summary>
        /// 353-360：开始时间（从80年起以秒为单位）
        /// </summary>
        public Double DataBeginSecond = Double.MaxValue;

        /// <summary>
        /// 361-368：结束时间（从80年起以秒为单位）
        /// </summary>
        public Double DataEndSecond = Double.MaxValue;

        public string SatelliteName = string.Empty;

        public DateTime OrbitBeginTime = DateTime.MinValue;


        public static bool Is1A5(byte[] bytes, string fileExtension)
        {
            UInt16 satelliteId = ToLocalEndian.ToUInt16FromBig(new byte[] { bytes[0], bytes[1] });
            if (satelliteId != 113&&satelliteId!=114)
                return false;
            return true;
        }
    }
}
