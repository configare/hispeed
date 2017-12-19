using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public class SubProduct0PSISNW: CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;

        public SubProduct0PSISNW(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _progressTracker = progressTracker;
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "SDSWEAlgorithm")
            {
                return SDSWEAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult SDSWEAlgorithm(Action<int, string> progressTracker)
        {
            string[] arguments = _argumentProvider.GetArg("SDSWEArguments") as string[];
            if (arguments==null||arguments.Length<2)
                return null;
            if (!File.Exists(arguments[0]) || string.IsNullOrEmpty(arguments[1]))
                return null;
            if (arguments[1].Contains("SD"))
            {
                if(arguments[1].Contains("North"))
                    _argumentProvider.SetArg("OutFileIdentify", "NSDI");
                else
                    _argumentProvider.SetArg("OutFileIdentify", "PSDI");
            }
            else
            {
                if (arguments[1].Contains("North"))
                    _argumentProvider.SetArg("OutFileIdentify", "NWEI");
                else
                    _argumentProvider.SetArg("OutFileIdentify", "SWEI");
            }
            _argumentProvider.SetArg("SelectedPrimaryFiles",arguments[0]);
            _argumentProvider.SetArg("fileOpenArgs",arguments[1]);
            return ThemeGraphyResult(null);
        }

    }
}
