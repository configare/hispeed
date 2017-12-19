#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/8/20 11:00:34
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

namespace GeoDo.RSS.DF.NOAA14
{
    /// <summary>
    /// 类名：NA141BHeader
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/8/20 11:00:34
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class NA141BHeader
    {
        private TBMInfoForNA141B _tbdInfoForNA141B;
        private CommonInfoForNA141B _commonInfoForNA141B;

        public NA141BHeader( object tbdInfoForNA141B, object commonInfoForNA141B)
        {
            _tbdInfoForNA141B = tbdInfoForNA141B as TBMInfoForNA141B;
            _commonInfoForNA141B = commonInfoForNA141B as CommonInfoForNA141B;
        }

        public TBMInfoForNA141B TbdInfoForNA141B
        {
            get { return _tbdInfoForNA141B; }
        }

        public CommonInfoForNA141B CommonInfoForNA141B
        {
            get { return _commonInfoForNA141B; }
        }

        public static bool Is1B(byte[] bytes)
        {
            
            return true;
        }
    }

    /// <summary>
    /// TBM头记录
    /// </summary>
    public class TBMInfoForNA141B
    {
       //76-78	    3	起始纬度
       public double MinLat ; 
       //79-81	    3	结束纬度
       public double MaxLat;
       //82-85	    4	起始经度
       public double MinLon;
       //86-89	    4	结束经度
       public double MaxLon;
       //90-91	    2	开始时间（时）
       public Int16 BeginHour;
       //92-93	    2	开始分
       public Int16 BeginMinite;
       //94-96	    3	数据时段（分）
       public int DataTime;
       //97	    1	增加数据选择（Y／N）
       public bool IsAddData;
       //98-117	    20	通道选择（二进制）

    }

    /// <summary>
    /// 一般信息
    /// </summary>
    public class CommonInfoForNA141B
    {
        //1	    1	     卫星标识
        public Byte SatelliteIdentify;
        //2	    1	     资料类型
        public Byte InformationType;
        //3-8	6	     开始时间
        public DateTime OrbitBeginTime = DateTime.MinValue;
        //9-10	2	     扫描线数
        public UInt16 RecordCount = UInt16.MaxValue;
        //11-16	6	     结束时间
        public DateTime OrbitFinishTime = DateTime.MinValue;
        //18-24	7	     轨道序号ASCII码表示
        public string OrbitOrderCode=null;
        //25	1	     斜坡校正标记
        public byte SlopeCorrection;
        //26	1	扫描脱漏标记（脱漏扫描线数）
        public byte MissCount;
        //27-32	6	质量检验信息
        public QualityCheckInfoForNA141B QualityCheckInfo;
        //33-34	2	定标参数标志
        public UInt16 CalibrationParameters;
        //35	1	数据接收和控制子系统状态
        //36-40	5	充‘0’
        //41-82	42	数据集名（EBCDIC）
        //83-7400	7317	充‘0’

    }

    /// <summary>
    /// 质量检验信息
    /// </summary>
    public class QualityCheckInfoForNA141B
    {
        /// <summary>
        /// 以bit为单位    
        /// </summary>
        // 1  数据无效标记     1：数据无效 0：有效
        public bool IsValid;
        //2 时序错标记       1：时序错   0：正确
        public bool IsErrorTime;
        //3 数据超界标记     1：超界     0：正确
        public bool IsOverBorder;
        //4出现重复同步标记 1：出现     0：无
        public bool IsRepeat;
        //5定标标记         1：定标无效 0：有效
        public bool IsErrorLocation;
        //6地球定位标记     1：定位无效 0：有效
        public bool IsErrorEarthLocation;
        //7升降轨标记       1：升轨     0：降轨
        public bool IsAscendOrbit;
        //8	伪噪声产生标记   1：伪噪声   0：无
        public bool IsNotNoise;
        //1  比特同步状态
        //2  帧同步错误标记
        //3  帧同步锁定标记
        //4
        //5
        //6-8 备份
        //1第一辅帧 TIP奇偶
        //2第二辅帧 TIP奇偶
        //3第三辅帧 TIP奇偶
        //4第四辅帧 TIP奇偶
        //5第五辅帧 TIP奇偶
        //6-8	备份
        //1-6同步错数
        //7-8	备份
    }
}
