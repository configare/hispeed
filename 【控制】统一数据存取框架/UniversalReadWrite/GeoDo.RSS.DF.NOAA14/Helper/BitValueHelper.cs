#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/8/23 18:07:14
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
    /// 类名：BitValueHelper
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/8/23 18:07:14
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class BitValueHelper
    {
        /// <summary>
        /// 获取数据中某一位的值
        /// </summary>
        /// <param name="input">传入的数据类型,可换成其它数据类型,比如Int</param>
        /// <param name="index">要获取的第几位的序号,从0开始</param>
        /// <returns>返回值为-1表示获取值失败</returns>
        public static int GetBitValue(byte input, int index)
        {
            if (index > sizeof(byte))
            {
                return -1;
            }
            //左移到最高位
            int value = input << (sizeof(byte) - 1 - index);
            //右移到最低位
            value = value >> (sizeof(byte) - 1);
            return value;
        }

        //public static int GetBitValueToInt

    }
}
