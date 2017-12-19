using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class SubProductIMGDRT : CmaMonitoringSubProduct
    {
        //private string _errorStr = "";
        private IArgumentProvider _curArguments = null;

        public SubProductIMGDRT(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "0IMGAlgorithm")
            {
                return IMGAlgorithm();
            }
            return null;
        }

        private IExtractResult IMGAlgorithm()
        {
            return ThemeGraphyResult(null);
        }
    }
}
