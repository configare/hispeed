using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductLayoutVgtImg : CmaMonitoringSubProduct
    {

        public SubProductLayoutVgtImg(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() != "0IMGAlgorithm")
                return null;
            return IMGAlgorithm();
        }

        private IExtractResult IMGAlgorithm()
        {
            return ThemeGraphyResult(null);
        }
    }
}
