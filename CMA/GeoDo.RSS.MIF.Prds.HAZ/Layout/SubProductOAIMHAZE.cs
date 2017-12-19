using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductOAIMHAZE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductOAIMHAZE(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NCIMAlgorithm")
            {
                return NCIMAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult NCIMAlgorithm()
        {
            //真彩图处理逻辑
            string natrueColorFile = Convert.ToString(_argumentProvider.GetArg("NatrueColorFile"));
           
            OpenFileFactory.Open(natrueColorFile);

            IMonitoringSession ms = _argumentProvider.EnvironmentVarProvider as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "NCIM");
            ms.DoAutoExtract(false);
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "ONCI");
            ms.DoAutoExtract(false);
            ms.ChangeActiveSubProduct("OAIM");

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
