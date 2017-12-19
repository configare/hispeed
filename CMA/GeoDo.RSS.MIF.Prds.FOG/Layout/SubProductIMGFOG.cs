using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    public class SubProductIMGFOG : CmaMonitoringSubProduct
    {
        public SubProductIMGFOG(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0IMGAlgorithm")
            {
                string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
                if (string.IsNullOrWhiteSpace(instanceIdentify))
                    return null;
                SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                if (instance == null)
                    return ThemeGraphyResult(null);
                if (instanceIdentify == "0MSI" || instanceIdentify == "MOSI" || instanceIdentify == "TNCI" || instanceIdentify == "ONCI")
                {
                    return ThemeGraphyMCSIDBLV(instance);
                }
                return ThemeGraphyResult(null);
            }
            return null;
        }
    }
}
