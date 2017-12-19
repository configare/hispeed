using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class SubProductSTATCLD : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductSTATCLD(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
            AOITemplateStat<float> aoiTempStat = new AOITemplateStat<float>();
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
            string[] fname = GetStringArray("SelectedPrimaryFiles");
            if (fname == null || fname.Length <= 0)
            {
                PrintInfo("请选择统计文件！");
                return null;
            }
            foreach (string name in fname)
            {
                if (!File.Exists(name))
                {
                    PrintInfo("需要统计的文件不存在！");
                    return null;
                }
            }
            if (_argumentProvider.GetArg("VTIRegion") == null)
            {
                PrintInfo("请设置需要统计的指数分段值！");
                return null;
            }
            SortedDictionary<float, float>  TauRegions = _argumentProvider.GetArg("VTIRegion") as SortedDictionary<float, float>;
            if (TauRegions == null || TauRegions.Count == 0)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "STATAlgorithm")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            string zoom = _argumentProvider.GetArg("resultZoom").ToString();
            float resultZoom = 1;
            if (!string.IsNullOrEmpty(zoom))
                resultZoom = float.Parse(zoom);
            string outId = _argumentProvider.GetArg("OutFileIdentify") as string;
            //string outFileId = CreatTitleByFileName(fname[0], outId);
            if (outId != null)
            {
                //自定义
                if (outId == "CCCA")
                {
                    Dictionary<string, int[]> aoi = _argumentProvider.GetArg("AOI") as Dictionary<string, int[]>;
                    if (aoi != null && aoi.Count > 0)
                    {
                        return CreatCustomStatResult(aoi, TauRegions, resultZoom, fname, outId);
                    }
                }
                SubProductInstanceDef instance = FindSubProductInstanceDefs(outId);
                if (instance == null)
                {
                    return STATAlgorithm();
                }
                else
                {
                    _argumentProvider.SetArg("OutFileIdentify", outId);
                    Dictionary<string, Func<float, bool>> filters = new Dictionary<string, Func<float, bool>>();
                    foreach (float key in TauRegions.Keys)
                    {
                        float min = key;
                        float max = TauRegions[key];
                        string filterKey = min + "-" + max;
                        filters.Add(filterKey, (v) =>
                        {
                            float value = v / resultZoom;
                            return (value >= min && value < max);
                        });
                    }
                    return StatRaster<float>(instance, filters, progressTracker);
                }
            }
            return null;
        }

        private IExtractResult STATAlgorithm()
        {
            return AreaStatResult<Int16>("云","CLD", (v) => { return v == 1; });
        }

        private string CreatTitleByFileName(string fname,string statType)
        {
            RasterIdentify id = new RasterIdentify(fname);
            string identify = id.SubProductIdentify;//U5TI
            if (string.IsNullOrEmpty(identify))
                return null;
            switch (identify)
            {
                case "0VTI":
                    statType = statType.Remove(0, 1);
                    return "V" + statType;
                case "0SWI":
                    statType = statType.Remove(0, 1);
                    return "S" + statType;
                case "TVDI":
                    statType = statType.Remove(0, 1);
                    return "T" + statType;
                case "0DNT":
                    statType = statType.Remove(0, 1);
                    return "D" + statType;
                default:
                    statType = statType.Remove(0, 1);
                    return "V" + statType;
            }
        }

        private IStatResult CreatStatResultItem(SortedDictionary<float, float> ndviRegions, Dictionary<string, int[]> aoiShengJie,
                                                                      IRasterDataProvider prd, float resultZoom, string productTitle)
        {
            StatAnalysisEngine<int> statEngine = new StatAnalysisEngine<int>();
            string[] columns = new string[ndviRegions.Count() + 1];
            columns[0] = "行政分区";
            List<string[]> rows = new List<string[]>();
            foreach (KeyValuePair<string, int[]> keyValue in aoiShengJie)
            {
                string[] s = new string[ndviRegions.Count() + 1];
                s[0] = keyValue.Key;
                int i = 1;
                foreach (KeyValuePair<float, float> region in ndviRegions)
                {
                    columns[i] = "干旱指数[" + region.Key.ToString() + "到" + region.Value.ToString() + "]";
                    s[i] = (CalcTotalArea(prd, keyValue.Value, region.Key * resultZoom, region.Value * resultZoom).ToString());
                    i++;
                }
                rows.Add(s);
            }
            string[][] ros = rows.ToArray();
            return new StatResult(productTitle, columns, ros);
        }

        private double CalcTotalArea(IRasterDataProvider dataProvider, int[] aoi, float minValue, float maxValue)
        {
            double convertArea = 0;
            IRasterOperator<Int16> rasterOper = new RasterOperator<Int16>();
            int count = rasterOper.Count(dataProvider, aoi, (value) =>
            {
                if (minValue < value && value <= maxValue)
                    return true;
                else return false;
            });
            convertArea = count * AreaCountHelper.CalcArea(dataProvider.ResolutionX, dataProvider.ResolutionY);
            return convertArea / 1000000;
        }

        private IExtractResult CreatCustomStatResult(Dictionary<string, int[]> aoi, SortedDictionary<float, float> regions, float resultZoom, string[] fname, string outFileId)
        {
            string productTitle = _argumentProvider.GetArg("ProductTitle").ToString();
            string extInfos = GetStringArgument("extinfo");
            string title = "统计日期：" + DateTime.Now.ToShortDateString();
            RasterIdentify id = new RasterIdentify(fname[0]);
            if (id.OrbitDateTime != DateTime.MinValue)
                title += "  轨道日期：" + id.OrbitDateTime;
            IRasterDataProvider prd = GeoDataDriver.Open(fname[0]) as IRasterDataProvider;
            IStatResult statResult = CreatStatResultItem(regions, aoi, prd, resultZoom, title);
            string filename = StatResultToFile(fname, statResult, "DRT", outFileId,"干旱指数自定义区域统计",extInfos,1,true,1);
            return new FileExtractResult(outFileId, filename);
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
