using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductAvgStatic : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductAvgStatic(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("SelectedPrimaryFiles") == null)
            {
                ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
                IMonitoringSession ms = session.MonitoringSession as IMonitoringSession;
                IWorkspace wks = ms.GetWorkspace();
                if (wks.ActiveCatalog != null)
                {
                    string[] fs = wks.ActiveCatalog.GetSelectedFiles("NDVI");
                    if (fs != null && fs.Length != 0)
                    {
                        _argumentProvider.SetArg("SelectedPrimaryFiles", fs);
                    }
                }
                if (_argumentProvider.GetArg("SelectedPrimaryFiles") == null)
                {
                    PrintInfo("请选择统计文件！");
                    return null;
                }
            }
            string[] fname = GetStringArray("SelectedPrimaryFiles");
            if (fname == null || fname.Length <= 0)
            {
                PrintInfo("请选择统计文件！");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "AVGAlgorithm")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            short validMin = (short)_argumentProvider.GetArg("ValidMin");
            short validMax = (short)_argumentProvider.GetArg("ValidMax");
            if (validMin > validMax)
                return null;
            string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
            SortedDictionary<string, double[]> srcResults = RasterStatFactory.SumAndCountByVector<short>(fname[0], statString,
                (v) => { return (v < validMax && v > validMin); }, progressTracker);
            if (srcResults == null || srcResults.Count < 1)
                return null;
            SortedDictionary<string,double[]> statResults=new SortedDictionary<string,double[]>();
            //去除无效值
            foreach (string key in srcResults.Keys)
            {
                if (srcResults[key] == null)
                    continue;
                statResults.Add(key, srcResults[key]);
            }
            if (statResults.Count <= 0)
                return null;
            string title = _subProductDef.ProductDef.Name+"平均值统计";
            string subTitle = GetSubTitle(fname[0]);
            string outFileIdentify = _argumentProvider.GetArg("OutFileIdentify").ToString();
            IStatResult results = DicToStatResult(statResults, subTitle);
            if (results == null)
                return null;
            string filename = StatResultToFile(fname, results, _subProductDef.ProductDef.Identify, outFileIdentify, title, null, 1, true, 1);
            return new FileExtractResult(outFileIdentify, filename);
        }

        private IStatResult DicToStatResult(SortedDictionary<string, double[]> statResults, string subTitle)
        {
            string[] rowKeys = statResults.Keys.ToArray(); //行
            List<string> cols = new List<string>();          //列
            cols.AddRange(new string[]{"统计分类","平均值"});
            string[] columns = cols.ToArray();
            List<string[]> rows = new List<string[]>();
            for (int i = 0; i < rowKeys.Length; i++)
            {
                string type = rowKeys[i];
                string[] row = new string[2];
                row[0] = type;
                row[1] = (statResults[type][0] / statResults[type][1]).ToString();
                rows.Add(row);
            }
            if (rows == null || rows.Count == 0)
                return null;
            else
                return new StatResult(subTitle, columns, rows.ToArray());
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private string GetSubTitle(string file)
        {
            string subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
            string orbitTimes = string.Empty;
            if (!File.Exists(file))
                return null;
            RasterIdentify rasterId = new RasterIdentify(file);
            orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + " ";
            return subTitle += "\n" + "轨道时间：" + orbitTimes;
        }
    }
}
