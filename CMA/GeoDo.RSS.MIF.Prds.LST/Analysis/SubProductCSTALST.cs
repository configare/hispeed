using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class SubProductCSTALST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductCSTALST(SubProductDef subProductDef)
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
            string argFileName = _argumentProvider.GetArg("RegionFileName").ToString();
            if (string.IsNullOrEmpty(argFileName))
            {
                PrintInfo("请设置需要统计的指数分段值参数文件！");
                return null;
            }
            SortedDictionary<float, float> lstRegions = GetArgFileRegion(argFileName);
            if (lstRegions == null || lstRegions.Count == 0)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "CSTAAlgorithm")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            string zoom = _argumentProvider.GetArg("resultZoom").ToString();
            float resultZoom = 100;
            if (!string.IsNullOrEmpty(zoom))
                resultZoom = float.Parse(zoom);
            string outId = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (outId != null)
            {
                SubProductInstanceDef instance = FindSubProductInstanceDefs(outId);
                if (instance == null)
                {
                    return STATAlgorithm();
                }
                else
                {
                    float K2T = -273;
                    Dictionary<string, Func<short, bool>> filters = new Dictionary<string, Func<short, bool>>();
                    foreach (float key in lstRegions.Keys)
                    {
                        float min = key;
                        float max = lstRegions[key];
                        string filterKey = (min + K2T) + "~" + (max + K2T)+"℃";
                        filters.Add(filterKey, (v) =>
                        {
                            float value = v / resultZoom;
                            return (value >= min && value < max);
                        });
                    }
                    if (instance.AOIProvider == "省级行政区划")
                        return StatProcentRaster<short>(fname[0], filters, progressTracker);
                    return StatRaster<short>(instance, filters, progressTracker);
                }
            }
            return null;
        }

        private IExtractResult StatProcentRaster<T>(string fname, Dictionary<string, Func<short, bool>> filters, Action<int, string> progressTracker)
        {
            string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
            SortedDictionary<string, double[][]> srcResult = RasterStatFactory.StatPercent<short>(fname, statString, filters, progressTracker);
            //删除为空的AOI区域以及计算百分比
            SortedDictionary<string, double[][]> statResult = new SortedDictionary<string, double[][]>();
            //区间个数
            int regionCount = filters.Keys.Count;
            foreach (string key in srcResult.Keys)
            {
                if (srcResult[key] == null)
                    continue;
                else
                {
                    int zeroCount = 0;
                    double[][] value = new double[regionCount][];
                    for (int i = 0; i < regionCount; i++)
                    {
                        value[i] = new double[] { srcResult[key][i][0], srcResult[key][i][0] / srcResult[key][i][1] * 100 };
                        if (value[i][0] == 0)
                            zeroCount++;
                    }
                    //如果全为0，也不添加
                    if (zeroCount < regionCount)
                        statResult.Add(key, value);
                }
            }
            if (statResult.Count == 0)
                return null;
            string productName = _subProductDef.ProductDef.Name;
            string title = productName + "行政区划";
            string subTitle = GetSubTitle(fname);
            string[] colDescs = new string[] { "面积", "百分比" };
            IStatResult[] results = DicToStatResult(statResult, filters.Keys.ToArray(), subTitle, colDescs);
            if (results == null)
                return null;
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string[] outFileIdentifys = new string[] { outFileIdentify, outFileIdentify.Substring(1) + "P" };
            string productIdentify = _subProductDef.ProductDef.Identify;
            IExtractResultArray array = new ExtractResultArray("LST");
            string filename = null;
            for (int i = 0; i < results.Length; i++)
            {
                if (i >= outFileIdentifys.Length)
                    break;
                filename = StatResultToFile(new string[] { fname }, results[i], productIdentify, outFileIdentifys[i], title + colDescs[i] + "统计", null, 1, false, 1);
                array.Add(new FileExtractResult(outFileIdentify, filename));
            }

            return array;
        }

        private SortedDictionary<float, float> GetArgFileRegion(string argFileName)
        {
            float T2K = 273;
            if (string.IsNullOrEmpty(argFileName) || !File.Exists(argFileName))
                return null;
            SortedDictionary<float, float> regionValues = new SortedDictionary<float, float>();
            string[] lines = File.ReadAllLines(argFileName, Encoding.Default);
            if (lines == null || lines.Length < 1)
                return null;
            for (int i = 0; i < lines.Length; i++)
            {
                float[] minmax = ParseRegionToFloat(lines[i]);
                if (minmax == null || minmax.Length == 0)
                    continue;
                regionValues.Add(minmax[0] + T2K, minmax[1] + T2K);
            }
            return regionValues;
        }

        private float[] ParseRegionToFloat(string re)
        {
            if (string.IsNullOrEmpty(re))
                return null;
            string[] parts = re.Split('~');
            if (parts == null || parts.Length == 0)
                return null;
            float min, max;
            if (float.TryParse(parts[0], out min) && float.TryParse(parts[1], out max))
                return new float[] { min, max };
            return null;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private IExtractResult STATAlgorithm()
        {
            return AreaStatResult<Int16>("陆表高温", "LST", (v) => { return v == 1; });
        }

        private static string GetSubTitle(string file)
        {
            string subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
            string orbitTimes = string.Empty;
            if (!File.Exists(file))
                return null;
            RasterIdentify rasterId = new RasterIdentify(file);
            orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + " ";
            return subTitle += "\n" + "轨道时间：" + orbitTimes;
        }

        private IStatResult[] DicToStatResult(SortedDictionary<string, double[][]> areaResultDic, string[] filterKeys, string subTitle, string[] colDescs)
        {
            List<IStatResult> statResultLst = new List<IStatResult>();
            string[] rowKeys = areaResultDic.Keys.ToArray(); //行
            List<string> cols = new List<string>();          //列
            cols.Add("统计分类");
            int index = 0;
            foreach (string colDesc in colDescs)
            {
                try
                {
                    foreach (string item in filterKeys)
                    {
                        cols.AddRange(new string[] { item });
                    }
                    string[] columns = cols.ToArray();
                    List<string[]> rows = new List<string[]>();
                    for (int i = 0; i < rowKeys.Length; i++)
                    {
                        string type = rowKeys[i];
                        string[] row = new string[1 + filterKeys.Length];
                        row[0] = type;
                        for (int j = 0; j < filterKeys.Length; j++)
                        {
                            row[j + 1] = areaResultDic[type][j][index].ToString();
                        }
                        rows.Add(row);
                    }
                    if (rows == null || rows.Count == 0)
                        continue;
                    else
                        statResultLst.Add(new StatResult(subTitle, columns, rows.ToArray()));
                }
                finally
                {
                    cols.Clear();
                    cols.Add("统计分类");
                    index++;
                }
            }
            return statResultLst.Count == 0 ? null : statResultLst.ToArray();
        }
    }
}
