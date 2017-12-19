using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductRFIMHAZE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductRFIMHAZE(SubProductDef subProductDef)
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
            if (_argumentProvider == null)// || _argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "RFIMAlgorithm")
            {
                return RFIMAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult RFIMAlgorithm()
        {
            string day = DateTime.Now.ToString("yyyyMMdd");
            //影像数据处理逻辑
            string rasterFile = Convert.ToString(_argumentProvider.GetArg("RFIMFile"));
            if (string.IsNullOrWhiteSpace(rasterFile))
                return null;
            string[] files = rasterFile.Split(new char[] { ',' });
            if (files.Length != 2)
                return null;
            string filename = files[0];
            OpenFileFactory.Open(filename);
            return null;
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
