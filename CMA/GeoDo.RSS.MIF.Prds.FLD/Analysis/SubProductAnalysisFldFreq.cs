using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductAnalysisFldFreq : CmaMonitoringSubProduct
    {

        public SubProductAnalysisFldFreq(SubProductDef subProductDef)
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
                return FREQAlgorithm();
            }
            return null;
        }

        private IExtractResult FREQAlgorithm()
        {
            return TimesStatAnalysisByPixel<Int16>("水情", "FLD",null, (dstValue, srcValue) =>
            {
                if (srcValue == 0)
                    return dstValue;
                else
                    return ++dstValue;
            },null);
        }
    }
}
