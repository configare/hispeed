using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using System.IO;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductSIAIICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;
        private double _statArea ;

        public SubProductSIAIICE(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "SIAIAlgorithm")
            {
                return SIAIAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult SIAIAlgorithm(Action<int, string> progressTracker)
        {
            //
            string rasterFile = _argumentProvider.GetArg("SelectedPrimaryFiles") as string;
            if (string.IsNullOrWhiteSpace(rasterFile))
            {
                ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
                IMonitoringSession ms = session.MonitoringSession as IMonitoringSession;
                IWorkspace wks = ms.GetWorkspace();
                if (wks.ActiveCatalog != null)
                {
                    string[] fs = wks.ActiveCatalog.GetSelectedFiles("DBLV");
                    if (fs != null && fs.Length != 0)
                    {
                        rasterFile = fs[0];
                        _argumentProvider.SetArg("SelectedPrimaryFiles", rasterFile);
                    }
                }
            }
            if (string.IsNullOrEmpty(rasterFile) || !File.Exists(rasterFile))
                return null;
            RasterIdentify rid = new RasterIdentify(rasterFile);
            if (rid.SubProductIdentify != "DBLV")
                return null;
            //栅格统计
            string title = "";
            StatResultItem[] areaResult = CommProductStat.AreaStat<Int16>("", rasterFile, ref title, null,
                (v) => { return v == 1; });
            _statArea = areaResult[0].Value;
            return ThemeGraphyResult(null);
        }

        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            
                foreach (IElement item in template.Layout.Elements)
                {
                    if (item is MultlineTextElement)
                    {
                        MultlineTextElement txt = item as MultlineTextElement;
                        if (txt.Text.Contains("{IceArea}"))
                        {
                            Dictionary<string, string> vars = new Dictionary<string, string>();
                            vars.Add("{IceArea}", "海冰面积：" + Math.Round(_statArea, 2) + "平方公里");
                            template.ApplyVars(vars);
                            break;
                        }
                    }
                }
        }
    }
}
