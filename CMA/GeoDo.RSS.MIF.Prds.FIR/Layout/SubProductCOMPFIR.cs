using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductCOMPFIR : CmaMonitoringSubProduct
    {

        public SubProductCOMPFIR(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "COMP")
            {
                return CompareAlgorithm();
            }
            return null;
        }

        private IExtractResult CompareAlgorithm()
        {
            return CompareAnalysisByPixel<float,Int16>("火情", "FIR", "", (fstFileValue, sedFileValue) =>
            {
                if (fstFileValue == 1f && sedFileValue == 1f)
                    return 2;
                else if (fstFileValue == 0f && sedFileValue == 1f)
                    return 3;
                else if (fstFileValue == 1f && sedFileValue == 0f)
                    return 1;
                else return 0;
            });
        }
    }
}
