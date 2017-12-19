#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-18 17:17:37
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

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// 接口名称：IBandNameRaster
    /// 创建者：罗战克
    /// 日期：2013-09-18 17:17:37
    /// 功能：
    /// 修改记录：
    /// </summary>
    public interface IBandNameRaster
    {
        bool TryGetBandNameFromBandNo(int bandNo, out int bandName);
        bool TryGetBandNoFromBandName(int bandName, out int bandNo);
        bool TryGetBandNameFromBandNos(int[] basebands, out int[] bandNames);
        bool TryGetBandNoFromBandNames(int[] basebands, out int[] bandNos);
    }
}
