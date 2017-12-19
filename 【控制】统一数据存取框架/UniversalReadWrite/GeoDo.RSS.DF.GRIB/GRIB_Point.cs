#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 8:50:24
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

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 格点场格点
    /// 用于记录数值模式预报及资料同化后的格点数据
    /// </summary>
    public struct GRIB_Point : IComparable<GRIB_Point>
    {
        public int Index;
        public float Value;

        public override string ToString()
        {
            return "Index : " + Index.ToString() + " , Value : " + Value.ToString();
        }

        public int CompareTo(GRIB_Point other)
        {
            if (this.Value > other.Value)
                return 1;
            else if (Value < other.Value)
                return -1;
            else
                return 0;
        }
    }
}
