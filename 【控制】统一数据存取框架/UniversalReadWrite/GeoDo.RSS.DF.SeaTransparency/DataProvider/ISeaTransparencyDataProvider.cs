#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/22 10:34:25
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
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.SeaTransparency
{
    /// <summary>
    /// 接口名称：ISeaTransparencyDataProvider
    /// 创建者：DongW
    /// 日期：2013/9/22 10:34:25
    /// 功能：
    /// 修改记录：
    /// </summary>
    public interface ISeaTransparencyDataProvider:IRasterDataProvider
    {
        enumSeaTransparencyType Type { get; }
    }
}
