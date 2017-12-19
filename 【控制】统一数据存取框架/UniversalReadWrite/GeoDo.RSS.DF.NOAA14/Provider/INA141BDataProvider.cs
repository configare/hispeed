#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/8/20 11:02:06
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
using GeoDo.RSS.DF.GDAL;

namespace GeoDo.RSS.DF.NOAA14
{
    /// <summary>
    /// 接口名称：INA141BDataProvider
    /// 创建者：DongW
    /// 日期：2013/8/20 11:02:06
    /// 功能：
    /// 修改记录：
    /// </summary>
    interface INA141BDataProvider : IGDALRasterDataProvider
    {
        NA141BHeader Header { get; }
        void ReadVisiCoefficient(ref double[,] operCoef, ref double[,] testCoef, ref double[,] beforeSendCoef);
        void ReadIRCoefficient(ref double[,] operCoef, ref double[,] beforeSendCoef);
    }
}
