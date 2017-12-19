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
    public class SubProductCYCIFOG : CmaMonitoringSubProduct
    {

        public SubProductCYCIFOG(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "CYCIAlgorithm")
            {
                return CYCIAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult CYCIAlgorithm(Action<int, string> progressTracker)
        {
            //return CycleTimeStatAnalysisByPixel<Int16>("大雾", "FOG", null, 
            //    (result, dstValue, srcValue) =>
            //    {
            //        return srcValue == 1 ? (Int16)result : dstValue;
            //    },progressTracker
            //);

            return CycleTimeStatAnalysisByRasterMap<Int16>("大雾", "FOG", null, 0.01f, progressTracker);        
        }
    }
}
