#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-13 16:21:58
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
using System.IO;
using System.Drawing;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.Windows.Forms;
namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    /// <summary>
    /// 类名：SubProductSiteVal
    /// 属性描述：用站点数据做评估
    /// 创建者：lixj   创建日期：2014-6-13 16:21:58
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductSiteVal : CmaMonitoringSubProduct
    {
        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;
        public SubProductSiteVal(SubProductDef subProductDef)
            : base(subProductDef)
        { 

        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "SiteValAlgorithm")
            {
                return SiteValAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult SiteValAlgorithm(Action<int, string> progressTracker)
        {
            return null;
        }
    }
}
