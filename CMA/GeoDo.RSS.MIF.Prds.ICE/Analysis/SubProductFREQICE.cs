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
    public class SubProductFREQICE : CmaMonitoringSubProduct
    {

        public SubProductFREQICE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FREQAlgorithm")
            {
                return FREQAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult FREQAlgorithm(Action<int, string> progressTracker)
        {
            return TimesStatAnalysisByPixel<Int16>("海冰", "ICE", null, (dstValue, srcValue) =>
            {
                if (srcValue == 0)
                    return dstValue;
                else
                    return ++dstValue;
            },progressTracker);
        }
    }
}
