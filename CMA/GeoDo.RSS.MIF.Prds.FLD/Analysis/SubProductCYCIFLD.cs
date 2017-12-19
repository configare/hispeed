using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductCYCIFLD : CmaMonitoringSubProduct
    {
         private IContextMessage _contextMessage = null;

         public SubProductCYCIFLD(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "CYCIAlgorithm")
                return CYCIAlgorithm(progressTracker);
            PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
            return null;
        }

        private IExtractResult CYCIAlgorithm(Action<int, string> progressTracker)
        {
            return CycleTimeStatAnalysisByPixel<Int16>("水情", "FLD", null, (result, dstValue, srcValue) =>
            {
                return srcValue == 1 ? (Int16)result : dstValue;
            }, progressTracker);
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
