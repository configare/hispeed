using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class SubProduct0MDSDST : CmaMonitoringSubProduct
    {
        public SubProduct0MDSDST(SubProductDef subDef)
            : base(subDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() != "0MDSAlgorithm")
                return null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            return MDSAlgorithm(progressTracker);
        }

        private IExtractResult MDSAlgorithm(Action<int, string> progressTracker)
        {
            //return TimesStatAnalysisByPixel<Int16>("沙尘", "DST", null, (dstValue, srcValue) =>
            //{
            //    if (srcValue == 0)
            //        return dstValue;
            //    else
            //        return ++dstValue;
            //}, progressTracker);

            return TimesStatAnalysisByRasterMap<Int16>("沙尘", "DST", null, 0.01f, progressTracker);
        }
    }
}
