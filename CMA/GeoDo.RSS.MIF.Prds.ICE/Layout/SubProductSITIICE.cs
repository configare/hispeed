using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Layout.Elements;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductSITIICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;
        private List<double> _statAreas = null;

        public SubProductSITIICE(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "SITIAlgorithm")
            {
                return SITIAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult SITIAlgorithm(Action<int, string> progressTracker)
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
                    string[] fs = wks.ActiveCatalog.GetSelectedFiles("0SIT");
                    if (fs != null && fs.Length != 0)
                    {
                        rasterFile = fs[0];
                        _argumentProvider.SetArg("SelectedPrimaryFiles", rasterFile);
                    }
                }
            }
            if (string.IsNullOrEmpty(rasterFile) || !File.Exists(rasterFile))
                return null;
            //栅格统计
            Dictionary<string, Func<short, bool>> filters = new Dictionary<string, Func<short, bool>>();
            //海冰厚度中四个标记值
            float[] rasterValues = new float[] { -1, 1, 5, 10 };
            for (int i = 0; i < rasterValues.Length; i++)
            {
                float min = rasterValues[i];
                string filterKey = min.ToString();
                filters.Add(filterKey, (v) =>
                {
                    return (v == min);
                });
            }
            Dictionary<string, SortedDictionary<string, double>> dic = RasterStatFactory.Stat(rasterFile, null, filters, progressTracker);
            if (dic == null||dic.Count<1)
                return null;
            SortedDictionary<string, double> statAreas = dic[dic.Keys.First()];
            _statAreas = new List<double>();
            double total = 0;
            foreach (string item in statAreas.Keys)
            {
                _statAreas.Add(statAreas[item]);
                total += statAreas[item];
            }
            _statAreas.Add(total);
            return ThemeGraphyResult(null);
        }

        protected override void ApplyAttributesOfElement(string name, GeoDo.RSS.Layout.IElement ele)
        {
            if (ele is GeoDo.RSS.Layout.Elements.ILegendElement)
            {
                ILegendElement legendElement = ele as ILegendElement;
                //修改图例名称
                legendElement.Text = "海冰面积：" + Math.Round(_statAreas[_statAreas.Count - 1], 2) + "平方公里";
                Layout.LegendItem[] legends = legendElement.LegendItems;
                if (legends != null && legends.Length > 0)
                {
                    int i = 0;
                    foreach (Layout.LegendItem item in legends)
                    {
                        item.Text = item.Text + ":" + Math.Round(_statAreas[i], 2);
                        i++;
                    }
                }
            }
            base.ApplyAttributesOfElement(name, ele);
        }
    }
}
