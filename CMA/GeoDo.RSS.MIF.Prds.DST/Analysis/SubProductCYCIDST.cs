using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class SubProductCYCIDST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductCYCIDST(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() != "CYCIAlgorithm")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
            {
                PrintInfo("请选择需要进行统计的文件！");
                return null;
            }
            return CYCIAlgorithm(progressTracker);
        }

        private IExtractResult CYCIAlgorithm(Action<int, string> progressTracker)
        {
            //return CycleTimeStatAnalysisByPixel<Int16>("沙尘", "DST", null, (result, dstValue, srcValue) =>
            //{
            //    return srcValue == 1 ? (Int16)result : dstValue;
            //}, progressTracker);

            return CycleTimeStatAnalysisByRasterMap<Int16>("沙尘", "DST", null, 0.01f, progressTracker);
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
