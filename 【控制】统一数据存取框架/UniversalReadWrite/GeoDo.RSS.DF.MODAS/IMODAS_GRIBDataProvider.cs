#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 8:54:37
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
using GeoDo.RSS.DF.GRIB;

namespace GeoDo.RSS.DF.MODAS
{
    public interface IMODAS_GRIBDataProvider : IGRIBDataProvider
    {
        bool IsReadToRasterDataProvider { get; }
        string[] DeepValues { get; }
        string[] FeatureNames { get; }
        string CurrentDeepValue { get; set; }
        string CurrentFeatureName { get; set; }
    }
}
