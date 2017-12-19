#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-15 20:03:30
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
using System.Windows.Forms;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.MEM;
using System.Text.RegularExpressions;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.FileProject;
using GeoDo.RSS.DF.GDAL.HDF5Universal;
using GeoDo.HDF5;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    /// <summary>
    /// 类名：SubProductCLD
    /// 属性描述：
    /// 创建者：lxj   创建日期：2014-6-15 20:03:30
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductValCLD:CmaMonitoringSubProduct
    {
        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;
        private static Regex[] DataReg = new Regex[]
        {
            new Regex (@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled),
            new Regex (@"A(?<year>\d{4})(?<day>\d{3})", RegexOptions.Compiled),
        };
        public SubProductValCLD(SubProductDef subProductDef)
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
            if (_curArguments.GetArg("AlgorithmName").ToString() == "SateValAlgorithm")
            {
                return SiteValAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult SiteValAlgorithm(Action<int, string> progressTracker)
        {
            ValArguments args = _argumentProvider.GetArg("SateArgs") as ValArguments;
            
           return null;//new FileExtractResult("HIST", histfilename);
        }
    }
}