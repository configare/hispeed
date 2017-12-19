using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    /// <summary>
    /// 大雾面积统计
    /// </summary>
    public class SubProductSTATFOG : CmaMonitoringSubProduct
    {
        public SubProductSTATFOG(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() != "STATAlgorithm")
                return null;
            //按照Instance执行统计操作
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (instanceIdentify != null)
            {
                SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                if (instance == null || instanceIdentify == "CCCA" || instanceIdentify == "CCAR")
                    return STATAlgorithm();
                else
                    return StatRaster<short>(instance, (v) => { return v == 1; }, true, progressTracker,true);
            }
            return null;
        }

        private IExtractResult STATAlgorithm()
        {
            return AreaStatResult<Int16>("大雾", "FOG", (v) => { return v == 1; });
        }
    }
}
