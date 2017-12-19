using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public class SubProductCompStatSNW : CmaMonitoringSubProduct
    {
        //省级行政区划、自定义、土地类型
        public SubProductCompStatSNW(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "COMA")
            {

                return STATAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult STATAlgorithm(Action<int, string> progressTracker)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            SubProductInstanceDef instance = FindSubProductInstanceDefs(outFileIdentify);
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            if (files == null || files.Length == 0)
                return null;
            _argumentProvider.SetArg("statname", "变化积雪");
            if (outFileIdentify == "COCC")
            {
                Dictionary<string,Func<short,bool>> dic=new Dictionary<string,Func<short,bool>>();
                dic.Add("新增雪面积（平方公里）", (v) => { return v == 4; });
                dic.Add("持续雪面积（平方公里）", (v) => { return v == 1; });
                dic.Add("融化雪面积（平方公里）", (v) => { return v == 5; });
                return StatRaster<short>(instance, dic, progressTracker);
            }
            string title = string.Empty;
            StatResultItem[] floodResult = CommProductStat.AreaStat<Int16>("变化积雪", files[0], ref title, aioObj, (v) => { return v == 4; });
            StatResultItem[] sameResult = CommProductStat.AreaStat<Int16>("变化积雪", files[0], ref title, aioObj, (v) => { return v == 1; });
            StatResultItem[] reduceResult = CommProductStat.AreaStat<Int16>("变化积雪", files[0], ref title, aioObj, (v) => { return v == 5; });
            if (floodResult == null && sameResult == null && reduceResult == null)
                return null;
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();
            StatResultItem[][] resultArray = new StatResultItem[][] { floodResult, sameResult, reduceResult };
            for (int i = 0; i < 3;i++ )
            {
                if (resultArray[i] != null && resultArray[i].Length > 0)
                {
                    foreach (StatResultItem item in resultArray[i])
                    {
                        if (result.ContainsKey(item.Name))
                        {
                            result[item.Name][i] = item.Value.ToString();
                        }
                        else
                        {
                            result.Add(item.Name, new string[3] { "0", "0","0"});
                            result[item.Name][i] = item.Value.ToString();
                        }
                    }
                }
            }
            if (result.Count == 0)
                return null;
            List<string[]> resultList = new List<string[]>();
            foreach (string key in result.Keys)
            {
                resultList.Add(new string[] { key, result[key][0], result[key][1], result[key][2] });
            }
            string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
            RasterIdentify id = new RasterIdentify(files[0]);
            if (id.OrbitDateTime != null)
                sentitle += "    轨道日期：" + id.OrbitDateTime.ToShortDateString();
            string[] columns = new string[] { "矢量分区", "新增雪面积（平方公里）", "持续雪面积（平方公里）", "融化雪面积（平方公里）" };
            IStatResult fresult = new StatResult(sentitle, columns, resultList.ToArray());
            string outputIdentify = _argumentProvider.GetArg("OutFileIdentify").ToString();
            //string filename = StatResultToFile(files, fresult, "SNW", outputIdentify, title, null, 1, true, 1);
            string filename = StatResultToFile(files, fresult, "SNW", outputIdentify, title, null, 1, true, 1);
            return new FileExtractResult(outputIdentify, filename);
        }
    }
}
