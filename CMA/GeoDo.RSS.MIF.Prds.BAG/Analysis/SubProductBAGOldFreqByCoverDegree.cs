using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductBAGOldFreqByCoverDegree : CmaMonitoringSubProduct
    {

        public SubProductBAGOldFreqByCoverDegree(SubProductDef productDef)
            : base(productDef)
        {
            _identify = productDef.Identify;
            _isBinary = false;
            _algorithmDefs = new List<AlgorithmDef>(productDef.Algorithms);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            List<string> covertDegreeRegions = _argumentProvider.GetArg("CovertDegreeRegion") as List<string>;
            if (_argumentProvider.GetArg("AlgorithmName") == null || covertDegreeRegions == null || covertDegreeRegions.Count == 0)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "BCDF")
            {
                int count = covertDegreeRegions.Count;
                ExtractResultArray results = new ExtractResultArray("BAG_BCDF");
                for (int i = 0; i < count; i++)
                {
                    float[] minmax = BAGStatisticHelper.GetCovertDegreeValue(covertDegreeRegions[i]);
                    FileExtractResult result = FREQAlgorithm(minmax[0], minmax[1]) as FileExtractResult;
                    results.Add(result);
                }
                return results;
            }
            return null;
        }

        private IExtractResult FREQAlgorithm(float minValue, float maxValue)
        {
            return TimesStatAnalysisByPixel<float>("蓝藻", "BAG", "_" + minValue + "_" + maxValue , (dstValue, srcValue) =>
            {
                if (srcValue > minValue && srcValue <= maxValue)
                    return ++dstValue;
                else
                    return dstValue;
            },null);
        }
    }
}
