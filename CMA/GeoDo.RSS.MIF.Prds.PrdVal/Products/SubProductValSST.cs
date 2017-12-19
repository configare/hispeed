#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-15 20:01:57
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
namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    /// <summary>
    /// 类名：SubProductValSST
    /// 属性描述：
    /// 创建者：Administrator   创建日期：2014-6-15 20:01:57
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductValSST:CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductValSST(SubProductDef subProductDef)
            : base(subProductDef)
        { 

        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "SiteValAlgorithm")
            {
                return SiteValAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult SiteValAlgorithm(Action<int, string> progressTracker)
        {
            object siteArgs = _argumentProvider.GetArg("SiteArgs");
            if (siteArgs == null)
                return null;
            ValArguments args = siteArgs as ValArguments;
            if (args == null)
                return null;
            if (args.FileNamesForVal == null || args.FileNamesForVal.Length < 1)
                return null;
            if (args.FileNamesToVal == null || args.FileNamesToVal.Length < 1)
                return null;
            return DataVal(args);
        }

        private IExtractResult DataVal(ValArguments args)
        {
            //获取待验证数据时间
            //1.若为逐时数据需要合成（仅合成在待验证数据范围内的）
            //2.若当日数据仅存在某一时次，则直接取值
            //foreach (string file in args.FileNamesForVal)
            //{
            //    RasterIdentify rid = new RasterIdentify(file);
            //    string[] file2Vals=
            //}
            return null;
        }
    }
}
