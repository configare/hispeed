using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Prds.UHE
{
    public class SubProductUHPILST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductUHPILST(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "UHPIAlgorithm")
                return UHPIAlgorithmCompute(progressTracker);
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult UHPIAlgorithmCompute(Action<int, string> progressTracker)
        {
            MemPixelFeatureMapper<UInt16> resultTemp = null;
            int UHEBandCH = (int)_argumentProvider.GetArg("UHEBand");
            int NormalLevel = (int)_argumentProvider.GetArg("NormalLevel");
            int HILevel = (int)_argumentProvider.GetArg("HILevel");
            double UHEBandZoom = (double)_argumentProvider.GetArg("UHEBand_Zoom");
            UInt16[] nanValues = GetNanValues("CloudyValue");
            UInt16[] waterValues = GetNanValues("WaterValue");

            string[] files = null;
            Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("FileSelectType") as Dictionary<string, string[]>;
            if (pathDic == null || pathDic.Count == 0)
            {
                PrintInfo("请点击\"确定\"按钮,以确定文件参数设置完毕!");
                return null;
            }
            if (pathDic.Keys.Contains("DirectoryPath")) //选择局地文件夹路径
                files = GetFiles(pathDic["DirectoryPath"][0]);
            else if (pathDic.Keys.Contains("FileNames")) //选择多个文件进行计算
                files = pathDic["FileNames"];
            if (files == null || files.Length == 0)
            {
                PrintInfo("待计算的数据文件不存在,请检查路径或文件设置!");
                return null;
            }
            SortedDictionary<float, float> UHERegions = _argumentProvider.GetArg("LevelRegion") as SortedDictionary<float, float>;
            if (UHERegions == null || UHERegions.Count == 0 || UHERegions.Count != HILevel)
                return null;
            List<UInt16> hiLevelList = new List<ushort>();
            for (int i = NormalLevel, j = 0; j < HILevel; i--, j++)
                hiLevelList.Add((UInt16)i);
            hiLevelList.Sort();
            Dictionary<string, StatInfo> result = new Dictionary<string, StatInfo>();
            float step = 80 / files.Length;
            int stepLength = 0;
            foreach (string file in files)
            {
                if (progressTracker != null)
                    progressTracker.Invoke((int)Math.Floor(step * stepLength), "正在计算比例指数,请稍后...");
                stepLength++;
                result.Add(file, CalcUPHIByOneFile(file, UHEBandCH, UHERegions, hiLevelList.ToArray()));
            }
            if (result.Count != 0)
            {
                IExtractResultArray array = new ExtractResultArray("UHE");
                if (progressTracker != null)
                    progressTracker.Invoke(90, "正在生成比例指数统计结果,请稍后...");
                array.Add(StateUHPI(NormalLevel, files, result) as IFileExtractResult);
                if (progressTracker != null)
                    progressTracker.Invoke(95, "正在生成热岛面积统计结果,请稍后...");
                array.Add(StateAreaCol(NormalLevel, files, result, HILevel) as IFileExtractResult);
                return array;
            }
            return null;
        }

        private IExtractResult StateUHPI(int NormalLevel, string[] files, Dictionary<string, StatInfo> result)
        {
            float sum = 0;
            List<string[]> valueItems = new List<string[]>();
            string DateStr = GetStringArgument("DateStr");
            foreach (string key in result.Keys)
            {
                foreach (UInt16 level in result[key].UHPI.Keys)
                    sum += level * ((float)result[key].UHPI[level] / result[key].totalCount);
                valueItems.Add(new string[] { DataIdentifyMatcher.GetOrbitDateTime(key).AddHours(8).ToString(DateStr), 
                        Math.Round((1f / NormalLevel) * sum, 2).ToString() });
                sum = 0;
            }
            string title = "统计日期：" + DateTime.Now.ToShortDateString();
            string[] columns = new string[] { "日期", "比例指数" };
            IStatResult statResult = new StatResult(title, columns, valueItems.ToArray());
            string filename = StatResultToFile(files, statResult, "UHE", "UHPI", "比例指数", string.Empty, 1, false);
            FileExtractResult fileResult = new FileExtractResult("UHPI", filename);
            fileResult.Add2Workspace = true;
            fileResult.SetDispaly(false);
            return fileResult;
        }

        private IExtractResult StateAreaCol(int NormalLevel, string[] files, Dictionary<string, StatInfo> result, int HILevel)
        {
            string[] normalDesc = GetStringArray("NormalDesc");
            List<string[]> valueItems = new List<string[]>();
            List<string> temp = null;
            List<string> columnList = new List<string>();
            columnList.Add("日期");
            string DateStr = GetStringArgument("DateStr");
            for (UInt16 col = 0; col < HILevel; col++)
                columnList.Add(normalDesc == null || col >= normalDesc.Length ? (NormalLevel - col).ToString() : normalDesc[col]);
            IRasterDataProvider rdp = GeoDataDriver.Open(files[0]) as IRasterDataProvider;
            double areaBase = 0f;
            areaBase = GetAreaBase(rdp, areaBase);
            foreach (string key in result.Keys)
            {
                temp = new List<string>();
                temp.Add(DataIdentifyMatcher.GetOrbitDateTime(key).AddHours(8).ToString(DateStr));
                for (UInt16 level = 0; level < HILevel; level++)
                {
                    if (!result[key].UHPI.ContainsKey((UInt16)(NormalLevel - level)))
                        temp.Add("0");
                    else
                        temp.Add(Math.Round(result[key].UHPI[(UInt16)(NormalLevel - level)] * areaBase / Math.Pow(10, 6), 2).ToString());
                }
                valueItems.Add(temp.ToArray());
            }
            string title = "统计日期：" + DateTime.Now.ToShortDateString() + " 面积单位:平方公里";
            string[] columns =columnList.ToArray();
            IStatResult statResult = new StatResult(title, columns, valueItems.ToArray());
            string filename = StatResultToFile(files, statResult, "UHE", "UHAR", "热岛面积", string.Empty, 1, false, 1);
            FileExtractResult fileResult = new FileExtractResult("UHAR", filename);
            fileResult.Add2Workspace = true;
            fileResult.SetDispaly(false);
            return fileResult;
        }

        private IExtractResult StateAreaRow(int NormalLevel, string[] files, Dictionary<string, StatInfo> result, int HILevel)
        {
            string[] normalDesc = GetStringArray("NormalDesc");
            string DateStr = GetStringArgument("DateStr");
            List<string[]> valueItems = new List<string[]>();
            IRasterDataProvider rdp = GeoDataDriver.Open(files[0]) as IRasterDataProvider;
            double areaBase = 0f;
            areaBase = GetAreaBase(rdp, areaBase);
            int col = 0;
            List<string> columnList = new List<string>();
            columnList.Add("日期");
            for (int row = 0; row < HILevel; row++)
            {
                valueItems.Add(new string[result.Count + 1]);
                valueItems[row][0] = normalDesc == null || row >= normalDesc.Length ? (NormalLevel - row).ToString() : normalDesc[row];
            }
            foreach (string key in result.Keys)
            {
                col++;
                columnList.Add(DataIdentifyMatcher.GetOrbitDateTime(key).AddHours(8).ToString(DateStr));
                for (int row = 0; row < HILevel; row++)
                {
                    for (UInt16 level = (UInt16)(NormalLevel - row), num = 0; num < HILevel; num++)
                        valueItems[row][col] = Math.Round(result[key].UHPI[level] * areaBase / Math.Pow(10, 6), 2).ToString();
                }
            }
            string title = "统计日期：" + DateTime.Now.ToShortDateString() + " 面积单位:平方公里";
            string[] columns = columnList.ToArray();
            IStatResult statResult = new StatResult(title, columns, valueItems.ToArray());
            string filename = StatResultToFile(files, statResult, "UHE", "UHAR", "热岛面积", string.Empty, 1, false, 1);
            FileExtractResult fileResult = new FileExtractResult("UHAR", filename);
            fileResult.Add2Workspace = true;
            fileResult.SetDispaly(false);
            return fileResult;
        }

        private static double GetAreaBase(IRasterDataProvider rdp, double areaBase)
        {
            try
            {
                double lon = rdp.CoordEnvelope.Center.X;
                double lat = rdp.CoordEnvelope.Center.Y;
                areaBase = AreaCountHelper.CalcArea(lon, lat, rdp.ResolutionX, rdp.ResolutionX);
            }
            finally
            {
                if (rdp != null)
                    rdp.Dispose();
            }
            return areaBase;
        }

        public string[] GetStringArray(string arugmentName)
        {
            object obj = _argumentProvider.GetArg(arugmentName);
            if (obj == null)
                return null;
            return obj.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private StatInfo CalcUPHIByOneFile(string file, int UHEBandCH, SortedDictionary<float, float> UHERegions, UInt16[] HILevel)
        {
            Dictionary<UInt16, int> uhpi = new Dictionary<UInt16, int>();
            StatInfo result = new StatInfo();
            int levelIndex = -1;
            foreach (float key in UHERegions.Keys)
            {
                levelIndex++;
                uhpi.Add(HILevel[levelIndex], 0);
            }
            IRasterDataProvider rdp = GeoDataDriver.Open(file, null) as IRasterDataProvider;
            ArgumentProvider ap = new ArgumentProvider(rdp, null);
            RasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(ap);
            visitor.VisitPixel(new int[] { UHEBandCH },
                (index, values) =>
                {
                    if (values[0] != 0)
                    {
                        levelIndex = -1;
                        foreach (float key in UHERegions.Keys)
                        {
                            levelIndex++;
                            if (values[0] < key || values[0] >= UHERegions[key])
                                continue;
                            if (uhpi.ContainsKey(HILevel[levelIndex]))
                                uhpi[HILevel[levelIndex]]++;
                            else
                                uhpi.Add(HILevel[levelIndex], 1);
                        }
                        result.totalCount++;
                    }
                });
            result.UHPI = uhpi;
            return result;
        }

        private string[] GetFiles(string dirPath)
        {
            ObritFileFinder off = new ObritFileFinder();
            string extInfo = null;
            string[] files = null;
            if (string.IsNullOrEmpty(dirPath))
                return null;
            string flag = _argumentProvider.GetArg("FileDataIdentify").ToString();
            files = off.Find(string.Empty, ref extInfo, "Dir=" + dirPath + ",Flag=" + flag + ",Sort=asc,ProFlag=true,FindRegion=all");
            if (files == null || files.Length == 0)
            {
                PrintInfo("计算比例指数的城市强度分布文件未提供!");
                return null;
            }
            return files;
        }

        private UInt16[] GetNanValues(string argumentName)
        {
            string nanValuestring = _argumentProvider.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<ushort> values = new List<ushort>();
                    ushort value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (UInt16.TryParse(valueStrings[i], out value))
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

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }

    internal class StatInfo
    {
        public Dictionary<UInt16, int> UHPI = null;
        public int totalCount = 0;

        public StatInfo()
        { }
    }
}