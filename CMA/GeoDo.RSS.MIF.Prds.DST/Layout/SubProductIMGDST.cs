using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class SubProductIMGDST : CmaMonitoringSubProduct
    {

        public SubProductIMGDST(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _name = subProductDef.Name;
            _identify = subProductDef.Identify;
            
            _algorithmDefs = new List<AlgorithmDef>();
            _algorithmDefs.AddRange(subProductDef.Algorithms);

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0IMGAlgorithm")
            {
                return IMGAlgorithm(); ;
            }
            return null;
        }

        private IExtractResult IMGAlgorithm()
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrWhiteSpace(instanceIdentify))
                return null;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
            if (instance == null)
                return ThemeGraphyResult(null);
            if (instanceIdentify == "0MSI" || instanceIdentify == "MOSI" || instanceIdentify == "TNCI" || instanceIdentify == "ONCI"
                ||instanceIdentify == "0SSI" )
            {
                return ThemeGraphyMCSIDBLV(instance);
            }
            return ThemeGraphyResult(null);
        }
    }
}
