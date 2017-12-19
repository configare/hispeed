using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductFSTAFIR : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductFSTAFIR(SubProductDef subProductDef)
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
            string[] argFileArg = _argumentProvider.GetArg("RegionFileName") as string[];
            string argFileName = argFileArg[0];
            bool argFileIsCY = bool.Parse(argFileArg[1]);
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
            if (algorith != "FSTAAlgorithm")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            string zoom = _argumentProvider.GetArg("resultZoom").ToString();
            float resultZoom = 100;
            if (!string.IsNullOrEmpty(zoom))
                resultZoom = float.Parse(zoom);
            string outId = _argumentProvider.GetArg("OutFileIdentify") as string;
            Int16[] nanValues = GetNanValues("CloudyValue");
            Int16[] waterValues = GetNanValues("WaterValue");
            bool isWeight = bool.Parse(_argumentProvider.GetArg("isWeight").ToString());
            if (outId != null)
            {
                SubProductInstanceDef instance = FindSubProductInstanceDefs(outId);
                if (instance == null)
                {
                    return STATAlgorithm();
                }
                else
                {
                    float maxmin_max = float.MinValue;
                    float maxmin_min = float.MaxValue;

                    Dictionary<string, Func<short, bool>> filters = new Dictionary<string, Func<short, bool>>();
                    foreach (float key in lstRegions.Keys)
                    {
                        float min = key == float.MinValue ? float.MinValue : (key * resultZoom);
                        float max = lstRegions[key] == float.MaxValue ? float.MaxValue : (lstRegions[key] * resultZoom);
                        string filterKey = min == float.MinValue ? "<" + max / resultZoom :
                                           max == float.MaxValue ? ">=" + min / resultZoom :
                                           min / resultZoom + "~" + max / resultZoom;
                        filters.Add(filterKey, (v) =>
                        {
                            if (IsNanValue(v, nanValues) || IsNanValue(v, waterValues))
                                return false;
                            return (v >= min && v < max);
                        });
                        if (maxmin_max < max)
                            maxmin_max = max;
                        if (maxmin_min > min)
                            maxmin_min = min;
                    }
                    string sumKey=maxmin_min/resultZoom+"~"+maxmin_max/resultZoom;
                    if (filters.ContainsKey(sumKey))
                    {
                        filters.Add("合计", filters[sumKey]);
                        filters.Remove(sumKey);
                    }

                    if (instance.AOIProvider == "省级行政区划")
                        return StatProcentRaster<short>(fname[0], "行政区划", filters, progressTracker, isWeight, resultZoom);
                    if (instance.AOIProvider == "土地利用类型")
                        return StatProcentRaster<short>(fname[0], "土地利用类型", filters, progressTracker, isWeight, resultZoom);
                    return StatRaster<short>(instance, filters, progressTracker, isWeight, resultZoom);
                }
            }
            return null;
        }

        private IExtractResult StatProcentRaster<T>(string fname, string statVector, Dictionary<string, Func<short, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom)
        {
            string title;
            string[] colDescs;
            IStatResult[] results = StatProcentRasterToStatResult(fname, statVector, filters, progressTracker, out title, out colDescs, false, weight, weightZoom);

            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string[] outFileIdentifys = new string[] { outFileIdentify, outFileIdentify.Substring(1) + "P" };
            string productIdentify = _subProductDef.ProductDef.Identify;
            IExtractResultArray array = new ExtractResultArray("FIR");
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

        private IStatResult[] StatProcentRasterToStatResult(string fname, string statVector, Dictionary<string, Func<short, bool>> filters, Action<int, string> progressTracker, out string title, out string[] colDescs, bool fullZeroAdd, bool weight, float weightZoom)
        {
            IStatResult[] results = null;
            title = "";
            colDescs = null;
            string statString = AreaStatProvider.GetAreaStatItemFileName(statVector);
            IRasterDataProvider rdp = GeoDataDriver.Open(fname) as IRasterDataProvider;
            SortedDictionary<string, double[][]> srcResult = RasterStatFactory.StatPercent<Int16>(fname, statString, filters, progressTracker, weight, weightZoom);
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
                    //如果全为0
                    if (fullZeroAdd || (!fullZeroAdd && zeroCount < regionCount))
                        statResult.Add(key, value);
                }
            }
            if (statResult.Count == 0)
                return null;
            string productName = _subProductDef.ProductDef.Name;
            title = productName + statVector;
            string subTitle = GetSubTitle(fname);
            colDescs = new string[] { "面积", "百分比" };
            results = DicToStatResult(statResult, filters.Keys.ToArray(), subTitle, colDescs);
            if (results == null)
                return null;
            return results;
        }

        private SortedDictionary<float, float> GetArgFileRegion(string argFileName)
        {
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
                regionValues.Add(MathCompare.FloatCompare(minmax[0], float.MinValue) ? float.MinValue : minmax[0],
                                 MathCompare.FloatCompare(minmax[1], float.MaxValue) ? float.MaxValue : minmax[1]);
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
            return AreaStatResult<Int16>("火情", "FIR", (v) => { return v == 1; });
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
                if (colDesc == "百分比")
                    continue;
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

        private bool IsNanValue(Int16 pixelValue, Int16[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
            {
                return false;
            }
            else
            {
                foreach (Int16 value in nanValues)
                {
                    if (pixelValue == value)
                        return true;
                }
            }
            return false;
        }

        private Int16[] GetNanValues(string argumentName)
        {
            string nanValuestring = _argumentProvider.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<Int16> values = new List<Int16>();
                    Int16 value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (Int16.TryParse(valueStrings[i], out value))
                            values.Add(value);
                    }
                    if (values.Count > 0)
                    {
                        return values.ToArray();
                    }
                }
            }
            return null;
        }

        public class StringArrayComparer : IComparer<string[]>
        {
            private bool bASC;
            public StringArrayComparer(bool asceding)
            {
                bASC = asceding;
            }

            #region IComparer<Student>
            public int Compare(string[] bef, string[] last)
            {
                if (!bASC)
                {
                    //降序  
                    string[] temp;
                    temp = last;
                    last = bef;
                    bef = temp;
                }

                int flg = -1;
                flg = Comparer<string>.Default.Compare(bef[0], last[0]);
                if (flg == 0)
                {
                    flg = Comparer<string>.Default.Compare(bef[1], last[1]);
                }
                return flg;
            }

            #endregion
        }

    }
}
