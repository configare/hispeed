using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductIMGHAE : CmaMonitoringSubProduct
    {
        public SubProductIMGHAE(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            object obj = _argumentProvider.GetArg("IsBackGround");
            bool IsBackGround = false;
            if (obj != null&&!string.IsNullOrEmpty(obj.ToString()))
                IsBackGround = bool.Parse(obj.ToString());
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0IMGAlgorithm")
            {
                string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
                if (string.IsNullOrWhiteSpace(instanceIdentify))
                    return null;
                SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                if (instance == null)
                    return ThemeGraphyResult(null);
                if (instanceIdentify == "0MSI" || instanceIdentify == "ONCI"
                    || instanceIdentify == "NCIM" || instanceIdentify == "TNCI" || instanceIdentify == "0SDI"
                    || (IsBackGround && instanceIdentify == "HAEI") || (IsBackGround && instanceIdentify == "OHAI"))
                {
                    return ThemeGraphyMCSIDBLV(instance);
                }
                return ThemeGraphyResult(null);
            }
            return null;
        }
    }
}
