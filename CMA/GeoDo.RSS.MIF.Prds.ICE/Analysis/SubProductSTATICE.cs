using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductSTATICE : CmaMonitoringSubProduct
    {

        public SubProductSTATICE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "STATAlgorithm")
            {
                string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
                if (instanceIdentify != null)
                {
                    SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                    if (instance == null || instance.OutFileIdentify == "CCCA"||instance.OutFileIdentify=="CCAR")
                    {
                        return STATAlgorithm();
                    }
                    else
                        return StatRaster<short>(instance, (v) => { return v == 1; }, progressTracker);
                }    
            }
            return null;
        }

        private IExtractResult STATAlgorithm()
        {
            return AreaStatResult<Int16>("海冰","ICE", (v) => { return v == 1; });
        }
    }
}
