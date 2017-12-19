using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public class SubProductDayMaxSNW : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductDayMaxSNW(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (!CheckArguments())
                return null;
            Dictionary<DateTime, List<string>> orderedFiles = GroupByOrbitDate();
            if (orderedFiles.Count == 0)
                return null;
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return null;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            List<string> rdpList = new List<string>();//按照日期生成的多个日最大合成数据
            //每天生成一个最大日合成图

            foreach (DateTime time in orderedFiles.Keys)
            {
                RasterMoasicProcesser processer = new RasterMoasicProcesser();
                List<IRasterDataProvider> srcdata = new List<IRasterDataProvider>();
                try
                {
                    foreach (string item in orderedFiles[time])
                    {
                        IRasterDataProvider rdp = GeoDataDriver.Open(item) as IRasterDataProvider;
                        if (rdp != null)
                            srcdata.Add(rdp);
                    }
                    string fileName = null;
                    IFileNameGenerator generator = GetFileNameGenerator();
                    if (generator == null)
                        fileName = string.Empty;
                    else
                    {
                        fileName = generator.NewFileName(CreatRasterIndetifyId(orderedFiles[time].ToArray(), "SNW", "MAXI", GetDataIdentify(), ".LDF", null));
                    }
                    using (IRasterDataProvider dstPrd = processer.Moasic<Int16>(srcdata.ToArray(), "LDF", fileName, false, null, "MAX", null, (srcValue, dstValue) => { return srcValue > dstValue ? srcValue : dstValue; }))
                    {
                        if (dstPrd != null)
                            rdpList.Add(dstPrd.fileName);
                    }
                }
                finally
                {
                    foreach (IRasterDataProvider rdp in srcdata)
                        rdp.Dispose();
                    srcdata.Clear();
                }
            }
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            IExtractResultArray resultArray = new ExtractResultArray("MAXI");
            foreach (string file in rdpList)
            {
                tgg.Generate(file, templatName, null, null, "MAXI", colorTabelName);
                string resultFilename = tgg.Save();
                if (string.IsNullOrEmpty(resultFilename))
                    continue;
                resultArray.Add(new FileExtractResult("MAXI", resultFilename));
            }
            if (resultArray != null)
                return resultArray;
            return null;
        }

        private Dictionary<DateTime, List<string>> GroupByOrbitDate()
        {
            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            Dictionary<DateTime, List<string>> orderedFiles = new Dictionary<DateTime, List<string>>();
            foreach (string item in fileNames)
            {
                RasterIdentify rid = new RasterIdentify(item);
                if (orderedFiles.Keys.Contains(rid.OrbitDateTime.Date))
                    orderedFiles[rid.OrbitDateTime.Date].Add(rid.OriFileName[0]);
                else
                    orderedFiles.Add(rid.OrbitDateTime.Date, new List<string> { rid.OriFileName[0] });
            }
            return orderedFiles;
        }

        private bool CheckArguments()
        {
            if (_argumentProvider == null)
                return false;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return false;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "MAXI")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return false;
            }
            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            if (fileNames == null || fileNames.Count() == 0)
            {
                PrintInfo("请选择参与最大值合成的数据！");
                return false;
            }
            foreach (string f in fileNames)
            {
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
                    return false;
                }
            }
            return true;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private IFileNameGenerator GetFileNameGenerator()
        {
            object obj = _argumentProvider.GetArg("FileNameGenerator");
            if (obj == null)
                return null;
            return obj as IFileNameGenerator;
        }

        private string GetColorTableName(string attrubileName)
        {
            if (string.IsNullOrEmpty(attrubileName))
                return null;
            object obj = _argumentProvider.GetArg(attrubileName);
            if (obj == null)
                return null;
            return obj.ToString();
        }
    }
}
