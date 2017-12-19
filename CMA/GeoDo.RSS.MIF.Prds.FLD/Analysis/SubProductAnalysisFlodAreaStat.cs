using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using GeoDo.RSS.MIF.Core;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.DF.MEM;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    //泛滥水体面积统计
    public class SubProductAnalysisFlodAreaStat : CmaMonitoringSubProduct
    {
        //行政区+地类、省级行政区划、自定义、土地类型
        public SubProductAnalysisFlodAreaStat(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FWAS")
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
            _argumentProvider.SetArg("statname", "变化水体");
            if (outFileIdentify == "COCC")
            {
                Dictionary<string, Func<short, bool>> dic = new Dictionary<string, Func<short, bool>>();
                dic.Add("扩大水体面积（平方公里）", (v) => { return v == 4; });
                dic.Add("未变水体面积（平方公里）", (v) => { return v == 1; });
                dic.Add("缩小水体面积（平方公里）", (v) => { return v == 5; });
                return StatRaster<short>(instance, dic, progressTracker);
            }
            if (outFileIdentify == "COCU")
            {
                _argumentProvider.SetArg("statname", "洪涝水体");
                Dictionary<string, Func<short, bool>> dic = new Dictionary<string, Func<short, bool>>();
                dic.Add("洪涝水体面积", (v) => { return v == 4; });
                return StatRaster<short>(instance, dic, progressTracker);
            }
            if (outFileIdentify == "CODU")
            {
                _argumentProvider.SetArg("statname", "洪涝水体");
                using (IRasterDataProvider rdp = GeoDataDriver.Open(files[0]) as IRasterDataProvider)
                {
                    LastDaysSetValue outLastDays = (rdp as MemoryRasterDataProvider).GetExtHeader<LastDaysSetValue>();
                    Dictionary<string, int[]> coduDic = new Dictionary<string, int[]>();
                    coduDic.Add("<" + outLastDays.LastDaysColor[0].ToString() + "日", new int[] { outLastDays.LastDaysColor[0], 0 });
                    for (int i = 1; i < outLastDays.LastDaysColor.Length; i++)
                    {
                        if (outLastDays.LastDaysColor[i] == 0)
                            break;
                        coduDic.Add(outLastDays.LastDaysColor[i - 1].ToString() + "日~" +
                                    outLastDays.LastDaysColor[i].ToString() + "日", new int[] { outLastDays.LastDaysColor[i - 1], outLastDays.LastDaysColor[i] });
                    }
                    Dictionary<string, Func<short, bool>> dic = new Dictionary<string, Func<short, bool>>();
                    int index = -1;
                    foreach (string key in coduDic.Keys)
                    {
                        index++;
                        string funKey = key;
                        if (index == 0)
                            dic.Add("洪涝" + key, (v) => { return v > 0 && v <= coduDic[funKey][0]; });
                        else
                            dic.Add("洪涝" + key, (v) => { return v > coduDic[funKey][0] && v <= coduDic[funKey][1]; });
                    }
                    return StatRaster<short>(instance, dic, progressTracker);
                }
            }

            string title = string.Empty;
            StatResultItem[] floodResult = CommProductStat.AreaStat<Int16>("变化水体", files[0], ref title, aioObj, (v) => { return v == 4; });
            StatResultItem[] sameResult = CommProductStat.AreaStat<Int16>("变化水体", files[0], ref title, aioObj, (v) => { return v == 1; });
            StatResultItem[] reduceResult = CommProductStat.AreaStat<Int16>("变化水体", files[0], ref title, aioObj, (v) => { return v == 5; });
            if (floodResult == null && sameResult == null && reduceResult == null)
                return null;
            //增加单次面积统计百分比计算

            bool isTotal = true;
            double floodPercent = 0, samePercent = 0, reducePercent = 0;
            if (floodResult.Length == 1 && sameResult.Length == 1 && reduceResult.Length == 1)
            {
                double histroyArea = sameResult[0].Value + reduceResult[0].Value;
                floodPercent = Math.Round(floodResult[0].Value / histroyArea * 100, 2);
                samePercent = Math.Round(sameResult[0].Value / histroyArea * 100, 2);
                reducePercent = Math.Round(reduceResult[0].Value / histroyArea * 100, 2);

                floodResult = AddPercent(floodResult, floodPercent);
                sameResult = AddPercent(sameResult, samePercent);
                reduceResult = AddPercent(reduceResult, reducePercent);
                isTotal = false;
            }

            //
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();
            StatResultItem[][] resultArray = new StatResultItem[][] { floodResult, sameResult, reduceResult };
            for (int i = 0; i < 3; i++)
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
                            result.Add(item.Name, new string[3]);
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
            if (id.MinOrbitDate != DateTime.MinValue && id.MaxOrbitDate != DateTime.MaxValue && id.MaxOrbitDate != id.MinOrbitDate)
            {
                sentitle += "    背景水体日期：" + id.MinOrbitDate.ToShortDateString();
                sentitle += "    轨道日期：" + id.MaxOrbitDate.ToShortDateString();
            }
            else
            {
                if (id.OrbitDateTime != null)
                    sentitle += "    轨道日期：" + id.OrbitDateTime.ToShortDateString();
            }
            string[] columns = new string[] { "矢量分区", "扩大水体面积（平方公里）", "未变水体面积（平方公里）", "缩小水体面积（平方公里）" };
            IStatResult fresult = new StatResult(sentitle, columns, resultList.ToArray());
            string outputIdentify = _argumentProvider.GetArg("OutFileIdentify").ToString();
            string filename = StatResultToFile(files, fresult, "FLD", outputIdentify, title, null, 1, isTotal, 1);


            return new FileExtractResult(outputIdentify, filename);
        }

        private StatResultItem[] AddPercent(StatResultItem[] areaResult, double percent)
        {
            List<StatResultItem> items = new List<StatResultItem>();
            items.AddRange(areaResult);
            StatResultItem sri = new StatResultItem();
            sri.Name = "百分比(%)";
            sri.Value = percent;
            items.Add(sri);
            return items.ToArray();
        }
    }
}
