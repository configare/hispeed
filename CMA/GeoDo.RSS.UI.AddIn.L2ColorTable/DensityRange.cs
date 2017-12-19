#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-08 9:19:52
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

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    /// <summary>
    /// 类名：DensityRange
    /// 属性描述：
    /// 创建者：罗战克   创建日期：2013-09-08 9:19:52
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public struct DensityRange
    {
        public DensityRange(int value_l, int value_r, int r, int g, int b)
        {
            minValue = value_l;
            maxValue = value_r;
            RGB_r = r;
            RGB_g = g;
            RGB_b = b;
        }

        public int minValue;

        public int maxValue;

        public int RGB_r;

        public int RGB_g;

        public int RGB_b;

        public string ToString()
        {
            return string.Format("{0} to {1} [{2},{3},{4}]", minValue, maxValue, RGB_r, RGB_g, RGB_b);
        }
    }
}
