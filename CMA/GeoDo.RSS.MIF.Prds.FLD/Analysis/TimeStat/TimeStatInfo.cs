using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class TimeStatInfo : CmaMonitoringSubProduct
    {
        public Dictionary<string, Dictionary<string, StatInfoBase[]>> GetStatInfo(SubProductDef subProductDef, IArgumentProvider argProvider, Action<int, string> progressTracker)
        {
            _subProductDef = subProductDef;
            _argumentProvider = argProvider;
            if (argProvider == null)
                return null;
            string[] filenames = GetStringArray("SelectedPrimaryFiles");
            if (filenames == null || filenames.Length == 0)
                return null;
            StatDimClass dimClass = argProvider.GetArg("StatDim") == null ? null : argProvider.GetArg("StatDim") as StatDimClass;
            if (dimClass == null)
                return null;
            enumStatDimType dimType = dimClass.DimType;
            enumStatDayMosaicType dayMosaicType = dimClass.DayMosaicType;
            Dictionary<string, Dictionary<string, StatInfoBase[]>> result = new Dictionary<string, Dictionary<string, StatInfoBase[]>>();
            RasterIdentify rid = null;
            StatInfoBase statInfo = null;
            string key = string.Empty;
            if (progressTracker != null)
                progressTracker.Invoke(10, "开始进行长序列面积统计,请稍后...");
            float interval = (80f / filenames.Length);
            int fileIndex = 0;
            foreach (string file in filenames)
            {
                if (progressTracker != null)
                    progressTracker.Invoke((int)(++fileIndex * interval) + 10, "开始进行第[" + fileIndex + "]个文件面积统计,请稍后...");
                if (string.IsNullOrEmpty(file) || !File.Exists(file))
                    continue;
                rid = new RasterIdentify(file);
                if (rid == null)
                    continue;
                statInfo = new StatInfoBase(rid.OrbitDateTime.ToString("yyyyMMddHHmmss"), file);
                switch (dimType)
                {
                    case enumStatDimType.不区分:
                        key = "不区分";
                        break;
                    case enumStatDimType.年:
                        key = rid.OrbitDateTime.ToString("yyyy年");
                        break;
                    case enumStatDimType.季:
                        key = GetSeason(rid.OrbitDateTime.ToString("yyyy年"), rid.OrbitDateTime.Month);
                        break;
                    case enumStatDimType.月:
                        key = rid.OrbitDateTime.ToString("yyyy年MM月");
                        break;
                    case enumStatDimType.旬:
                        key = GetTenDays(rid.OrbitDateTime.ToString("yyyy年MM月"), rid.OrbitDateTime.Day);
                        break;
                    case enumStatDimType.日:
                        key = rid.OrbitDateTime.ToString("yyyy年MM月dd日");
                        break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(key))
                    continue;
                if (!result.ContainsKey(key))
                {
                    Dictionary<string, StatInfoBase[]> tempInfo = GetDayInfo(file, rid.OrbitDateTime.ToString("yyyyMMdd"), new Dictionary<string, StatInfoBase[]>(), ref statInfo, argProvider, progressTracker);
                    if (tempInfo != null && tempInfo.Count != 0)
                        result.Add(key, tempInfo);

                }
                else
                {
                    Dictionary<string, StatInfoBase[]> tempInfo = GetDayInfo(file, rid.OrbitDateTime.ToString("yyyyMMdd"), result[key], ref statInfo, argProvider, progressTracker);
                    if (tempInfo != null && tempInfo.Count != 0)
                        result[key] = tempInfo;
                }
            }
            return result.Count == 0 ? null : result;
        }

        private string GetTenDays(string monthStr, int day)
        {
            if (day < 11)
                return monthStr + "上旬";
            if (day < 21)
                return monthStr + "中旬";
            if (day >= 21)
                return monthStr + "下旬";
            return monthStr;
        }

        private string GetSeason(string yearStr, int month)
        {
            if (month >= 1 && month < 4)
                return yearStr + "第一季度";
            if (month >= 4 && month < 7)
                return yearStr + "第二季度";
            if (month >= 7 && month < 10)
                return yearStr + "第三季度";
            if (month >= 10 && month < 12)
                return yearStr + "第四季度";
            return yearStr;
        }

        private Dictionary<string, StatInfoBase[]> GetDayInfo(string filename, string key, Dictionary<string, StatInfoBase[]> subResult, ref StatInfoBase statInfo, IArgumentProvider argProvider, Action<int, string> progressTracker)
        {
            IStatResult result = CalcAreaByAlgorithmOneFile(argProvider, progressTracker, filename);
            if (result == null)
                return subResult;
            statInfo.DayStatResult = result;
            if (!subResult.ContainsKey(key))
                subResult.Add(key, new StatInfoBase[] { statInfo });
            else
            {
                List<StatInfoBase> temp = new List<StatInfoBase>();
                temp.AddRange(subResult[key]);
                temp.Add(statInfo);
                subResult[key] = temp.ToArray();
            }
            return subResult;
        }

        public IStatResult CalcAreaByAlgorithmOneFile(IArgumentProvider argProvider, Action<int, string> progressTracker, string filename)
        {
            if (argProvider.GetArg("AlgorithmName").ToString() == "TSTAAlgorithmByFiles")
            {
                //按照Instance执行统计操作
                string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
                if (instanceIdentify != null)
                {
                    SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                    if (instance != null)
                    {
                        if (instance.OutFileIdentify == "TCCC" || instance.OutFileIdentify == "TLUT")     //省市县面积统计
                            return StatRasterToStatResult<short>(instance, (v) => { return v == 1; }, progressTracker);
                        if (!string.IsNullOrEmpty(instance.AOIProvider))
                            _argumentProvider.SetArg("AOI", instance.AOIProvider);
                    }
                }
                if (argProvider.GetArg("AlgorithmName").ToString() == "TSTAAlgorithmByFiles")
                    return STATAlgorithm(filename);
            }
            return null;
        }

        private IStatResult STATAlgorithm(string filename)
        {
            string[] files = new string[] { filename };
            if (files == null || files.Length == 0)
                return null;
            string mixFile = files[0].Replace("_DBLV_", "_0MIX_").Replace("_FLOD_", "_0MIX_");
            Dictionary<int, Int16> mixDic = new Dictionary<int, short>();
            if (File.Exists(mixFile))
            {
                using (IRasterDataProvider rd = GeoDataDriver.Open(mixFile) as IRasterDataProvider)
                {
                    ArgumentProvider ap = new ArgumentProvider(rd, null);
                    RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(ap);
                    rpVisitor.VisitPixel(new int[] { 1 },
                        (idx, values) =>
                        {
                            if (values[0] != 0)
                                mixDic.Add(idx, values[0]);
                        });
                }
            }
            if (mixDic.Count == 0)
            {
                return AreaStatResultToStatResult<Int16>(filename, "水情", "FLD", (v) => { return v == 1; });
            }
            else
                return AreaStatResultToStatResult<Int16>(filename, "水情", "FLD", (v, idx) =>
                {
                    if (v == 1)
                    {
                        if (mixDic.ContainsKey(idx))
                            return mixDic[idx];
                        else
                            return 100;
                    }
                    else
                        return 0;
                }, 100);
        }

    }

    public class StatInfoBase
    {
        public string DayFlag;
        public string DayFilename;
        public IStatResult DayStatResult = null;

        public StatInfoBase(string dayFlag, string dayFilename)
        {
            DayFlag = dayFlag;
            DayFilename = dayFilename;
        }

        public StatInfoBase(string dayFlag, string dayFilename, IStatResult dayStatResult)
        {
            DayFlag = dayFlag;
            DayFilename = dayFilename;
            DayStatResult = dayStatResult;
        }

    }
}
