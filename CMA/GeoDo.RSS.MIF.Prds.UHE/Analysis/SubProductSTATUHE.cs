using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.UHE
{
    public class SubProductSTATUHE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductSTATUHE(SubProductDef subProductDef)
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
            if (algorith == "STATAlgorithm" && algorith != "CHAZSTATAlgorithm")
                return AreasSTAT(lstRegions,progressTracker);
            else if (algorith == "CHAZSTATAlgorithm")
                return AreasCHAZSTAT(lstRegions, progressTracker);
            else
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
        }

        private IExtractResult AreasSTAT(SortedDictionary<float, float> lstRegions,Action<int, string> progressTracker)
        {
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
                    Dictionary<string, Func<short, bool>> filters = new Dictionary<string, Func<short, bool>>();
                    foreach (float key in lstRegions.Keys)
                    {
                        float min = key;
                        float max = lstRegions[key];
                        string filterKey = min + "—" + max;
                        filters.Add(filterKey, (v) =>
                        {
                            float value = v / resultZoom;
                            return (value >= min && value < max);
                        });
                    }
                    return StatRaster<short>(instance, filters, progressTracker);
                }
            }
            return null;
        }

        private SortedDictionary<float, float> GetArgFileRegion(string argFileName)
        {
            if (string.IsNullOrEmpty(argFileName) || !File.Exists(argFileName))
                return null;
            SortedDictionary<float, float> regionValues = new SortedDictionary<float, float>();
            string[] lines = File.ReadAllLines(argFileName,Encoding.Default);
            if (lines == null || lines.Length < 1)
                return null;
            for (int i = 0; i < lines.Length;i++ )
            {
                float[] minmax = ParseRegionToFloat(lines[i]);
                if (minmax == null || minmax.Length == 0)
                    continue;
                regionValues.Add(minmax[0], minmax[1]);
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
            return AreaStatResult<Int16>("城市热岛","UHE", (v) => { return v == 1; });
        }

        #region 新增两个城市热岛文件分级统计结果面积求差
        private IExtractResult AreasCHAZSTAT(SortedDictionary<float, float> lstRegions, Action<int, string> progressTracker)
        {
            if (_argumentProvider.GetArg("mainfiles") == null)
            {
                PrintInfo("请选择城市热岛指数(被减数)数据。");
                return null;
            }
            string bjianshu = _argumentProvider.GetArg("mainfiles").ToString();
            if (!File.Exists(bjianshu))
            {
                PrintInfo("所选择的数据:\"" + bjianshu + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("jianshu") == null)
            {
                PrintInfo("请选择城市热岛指数(减数)数据。");
                return null;
            }
            string jianshu = _argumentProvider.GetArg("jianshu").ToString();
            if (!File.Exists(jianshu))
            {
                PrintInfo("所选择的数据:\"" + jianshu + "\"不存在。");
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
                if (instance != null)
                {
                    Dictionary<string, Func<short, bool>> filters = new Dictionary<string, Func<short, bool>>();
                    foreach (float key in lstRegions.Keys)
                    {
                        float min = key;
                        float max = lstRegions[key];
                        string filterKey = min + "—" + max;
                        filters.Add(filterKey, (v) =>
                        {
                            float value = v / resultZoom;
                            return (value >= min && value < max);
                        });
                    }
                    return CHAZStatRaster<short>(instance, filters, progressTracker);
                }
            }
            return null;
        }

        public IExtractResult CHAZStatRaster<T>(SubProductInstanceDef instance, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            switch (instance.AOIProvider)
            {
                case "当前区域"://CCAR--CARC
                    return CHAZStatRasterByVector<T>(instance.Name+"差值", "CARC",null, filters, progressTracker);
                case "省市县行政区划"://0CCC--CCCC
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return CHAZStatRasterXJ<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name + "差值", instance.AOIProvider, statString, "CCCC", filters, progressTracker);
                    }
                case "省级行政区划"://0CBP--CBPC
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
                        return CHAZStatRasterByVector<T>(instance.Name + "差值","CBPC", statString, filters, progressTracker);
                    }
                case "土地利用类型"://CLUT--LUTC
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("土地利用类型");
                        return CHAZStatRasterByVector<T>(instance.Name + "差值","LUTC", statString, filters, progressTracker);
                    }
                case "自定义区域"://
                default:
                    break;
            }
            return null;
        }

        #endregion

        #region
        //public IExtractResult CHAZStatRasterByVector<T>(string instanceName, string outFileIdentify, string aoiTemplate, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker)
        //{
        //    object aioObj = _argumentProvider.GetArg("AOI");
        //    string title = GetStringArgument("title");
        //    string[] bjsfiles = GetStringArray("mainfiles");
        //    string[] jsfiles = GetStringArray("jianshu");
        //    string extInfos = GetStringArgument("extinfo");
        //    //string outFileIdentify = GetStringArgument("OutFileIdentify");//outfileidentify
        //    string statname = GetStringArgument("statname");
        //    string productName = _subProductDef.ProductDef.Name;
        //    string productIdentify = _subProductDef.ProductDef.Identify;
        //    Dictionary<string, SortedDictionary<string, double>> bjsdic, jsdic, dic = null;//<矢量面名称，<分级字段，统计值>>
        //    if (bjsfiles == null || bjsfiles.Length == 0 || jsfiles == null || jsfiles.Length == 0)
        //        return null;
        //    bjsdic = RasterStatFactory.Stat(bjsfiles[0], aoiTemplate, filters, progressTracker);
        //    if (bjsdic == null || bjsdic.Count == 0)
        //        return null;
        //    jsdic = RasterStatFactory.Stat(jsfiles[0], aoiTemplate, filters, progressTracker);
        //    if (jsdic == null || jsdic.Count == 0)
        //        return null;
        //    if (bjsdic.Count != jsdic.Count)
        //        return null;
        //    //bool isident = true;
        //    //foreach (string key in bjsdic.Keys)
        //    //{
        //    //    if (!jsdic.ContainsKey(key))
        //    //    {
        //    //        isident = false;
        //    //        break;
        //    //    }
        //    //}
        //    //if (!isident)
        //    //    return null;
        //    ////<矢量面名称，<分级字段，统计值>>
        //    //foreach (string key in jsdic.Keys)
        //    //{
        //    //    if (!bjsdic.ContainsKey(key))
        //    //    {
        //    //        isident = false;
        //    //        break;
        //    //    }
        //    //}
        //    //if (!isident)
        //    //    return null;
        //    dic = new Dictionary<string, SortedDictionary<string, double>>();
        //    SortedDictionary<string, double> dicSortedDic = new SortedDictionary<string, double>();
        //    double chazvalue = 0;
        //    foreach (string key in bjsdic.Keys)
        //    {
        //        foreach (string keyvalue in bjsdic[key].Keys)
        //        {
        //            chazvalue = bjsdic[key][keyvalue] - jsdic[key][keyvalue];
        //            if (!dicSortedDic.ContainsKey(keyvalue))
        //                dicSortedDic.Add(keyvalue, chazvalue);
        //        }
        //        if (!dic.ContainsKey(key))
        //            dic.Add(key, dicSortedDic);
        //        dicSortedDic.Clear();
        //    }
        //    if (dic.Count <= 0)
        //        return null;
        //    title = productName + statname + instanceName;
        //    string subTitle = GetCHAZSubTitle(bjsfiles[0], jsfiles[0]);
        //    IStatResult results = DicToStatResult(dic, filters.Keys.ToArray(), subTitle);
        //    if (results == null)
        //        return null;
        //    string filename = StatResultToFile(bjsfiles, results, productIdentify, outFileIdentify, title, extInfos, 1, true, 1);
        //    return new FileExtractResult(outFileIdentify, filename);
        //}

        //public IExtractResult CHAZStatRasterXJ<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker)
        //{
        //    object aioObj = _argumentProvider.GetArg("AOI");
        //    string title = GetStringArgument("title");
        //    string[] bjsfiles = GetStringArray("mainfiles");
        //    string[] jsfiles = GetStringArray("jianshu");
        //    string extInfos = GetStringArgument("extinfo");
        //    string statname = GetStringArgument("statname");
        //    Dictionary<string, SortedDictionary<string, double>> bjsdic, jsdic, dic = null;
        //    if (bjsfiles == null || bjsfiles.Length == 0 || jsfiles == null || jsfiles.Length == 0)
        //        return null;
        //    bjsdic = RasterStatFactory.Stat(bjsfiles[0], aoiTemplate, filters, progressTracker);
        //    if (bjsdic == null || bjsdic.Count == 0)
        //        return null;
        //    jsdic = RasterStatFactory.Stat(jsfiles[0], aoiTemplate, filters, progressTracker);
        //    if (jsdic == null || jsdic.Count == 0)
        //        return null;
        //    //bool isident = true;
        //    //foreach (string key in bjsdic.Keys)
        //    //{
        //    //    if (!jsdic.ContainsKey(key))
        //    //    {
        //    //        isident = false;
        //    //        break;
        //    //    }
        //    //}
        //    //if (!isident)
        //    //    return null;
        //    ////<矢量面名称，<分级字段，统计值>>
        //    //foreach (string key in jsdic.Keys)
        //    //{
        //    //    if (!bjsdic.ContainsKey(key))
        //    //    {
        //    //        isident = false;
        //    //        break;
        //    //    }
        //    //}
        //    //if (!isident)
        //    //    return null;
        //    dic = new Dictionary<string, SortedDictionary<string, double>>();
        //    SortedDictionary<string, double> dicSortedDic = new SortedDictionary<string, double>();
        //    double chazvalue = 0;
        //    foreach (string key in bjsdic.Keys)
        //    {
        //        foreach (string keyvalue in bjsdic[key].Keys)
        //        {
        //            chazvalue = bjsdic[key][keyvalue] - jsdic[key][keyvalue];
        //            if (!dicSortedDic.ContainsKey(keyvalue))
        //                dicSortedDic.Add(keyvalue, chazvalue);
        //        }
        //        if (!dic.ContainsKey(key))
        //            dic.Add(key, dicSortedDic);
        //        dicSortedDic.Clear();
        //    }
        //    if (dic.Count <= 0)
        //        return null;

        //    if (string.IsNullOrEmpty(statname))
        //        title = productName + instanceName;
        //    else
        //        title = statname + instanceName;
        //    SortedDictionary<string, string> keyNmaeDic = GetRasterStatDictionary(aoiTemplate);
        //    string subTitle = GetCHAZSubTitle(bjsfiles[0], jsfiles[0]);
        //    IStatResult results = SSXDicToStatResult(dic, filters.Keys.ToArray(), subTitle, instanceAOI, keyNmaeDic);
        //    if (results == null)
        //        return null;
        //    string filename = StatResultToFile(bjsfiles, results, productIdentify, outFileIdentify, title, extInfos, 1, false, 0);
        //    return new FileExtractResult(outFileIdentify, filename);
        //    throw new NotImplementedException();
        //}

        //private static string GetCHAZSubTitle(string bjsfile, string jsfile)
        //{
        //    string subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
        //    string orbitTimes = string.Empty;
        //    RasterIdentify rasterId = new RasterIdentify(bjsfile);
        //    orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + "减去";
        //    rasterId = new RasterIdentify(jsfile);
        //    orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + " ";
        //    return subTitle += "\n" + "轨道时间：" + orbitTimes;
        //}
        #endregion
    }
}
