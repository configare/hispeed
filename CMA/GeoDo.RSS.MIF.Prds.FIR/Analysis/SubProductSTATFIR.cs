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
    public class SubProductSTATFIR : CmaMonitoringSubProduct
    {
        public class FireAreaInfo
        {
            public int PixelCount;
            public float PixelArea;
            public float FireArea;

            public FireAreaInfo(int pixelCount, float pixelArea, float fireArea)
            {
                PixelCount = pixelCount;
                PixelArea = pixelArea;
                FireArea = fireArea;
            }
        }

        public SubProductSTATFIR(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            //按照Instance执行统计操作
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (instanceIdentify != null)
            {
                SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                if (instance != null && instance.OutFileIdentify == "0CCC" && instance.AOIProvider == "省市县行政区划")
                    return StatRaster<short>(instance, (v) => { return v == 1; }, progressTracker);
            }

            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "STATAlgorithm")
            {
                return STATAlgorithm();
            }
            return null;
        }

        private IExtractResult STATAlgorithm()
        {
            string[] files=GetStringArray("SelectedPrimaryFiles");
            if (files != null && files.Length > 0)
                return SelectAlgorithm(files[0]);
            else return null;
        }

        private IExtractResult SelectAlgorithm(string file)
        {
            RasterIdentify rid = new RasterIdentify(file);
            if (rid == null || string.IsNullOrEmpty(rid.SubProductIdentify))
                return null;
            if (rid.SubProductIdentify == "DBLV")
            {
                //查找火点信息列表文件
                string[] plstFiles = FindPLSTFiles();
                if (plstFiles == null || plstFiles.Length == 0)
                return null;
                //根据找到的火点信息列表文件进行统计
                return StatArea(plstFiles);
            }
            else if (rid.SubProductIdentify == "FIRG")
            {
                return AreaStatResult<Int16>("火情过火区", "FIR", (v) => { return v == 1; });
            }
            return null;
        }

        private IExtractResult StatArea(string[] plstFiles)
        {
            Dictionary<string, FireAreaInfo> statArea = null;
            object aoiObj = _argumentProvider.GetArg("AOI");
            string title = string.Empty;
            if (aoiObj == null)
            {
                title = "火情按当前区域面积统计";
                statArea = AreaStatByType(plstFiles,"当前区域");
            }
            else
            {
                if (aoiObj as Dictionary<string, int[]> != null)
                {
                    Dictionary<string, int[]> aoi = aoiObj as Dictionary<string, int[]>;
                    statArea = StatCustomArea(plstFiles, ref title,aoi);
                }
                else
                {
                    title = "火情按" + aoiObj.ToString() + "面积统计";
                    statArea = AreaStatByType(plstFiles, aoiObj.ToString());
                }
            }
            if (statArea!=null&&statArea.Count != 0)
            {
                //生成结果文件参数string[][] rows
                string[] columns ;
                string[][] resultRows = GetResultRows(statArea,out columns);
                string statInfo = "统计日期：" + DateTime.Now.ToShortDateString();
                IStatResult result = new StatResult(statInfo, columns, resultRows);
                string[] files = GetStringArray("SelectedPrimaryFiles");
                string outFileIdentify = GetStringArgument("OutFileIdentify");
                string extInfos = GetStringArgument("extinfo");
                string filename = StatResultToFile(files, result, "FIR", outFileIdentify, title, extInfos, 1,false,(byte)0);
                return new FileExtractResult(outFileIdentify, filename);
            }
            else
                return null;
        }

        private string[][] GetResultRows(Dictionary<string, FireAreaInfo> statArea,out string[] columns)
        {
            //按照覆盖范围降序排列
            float[] decArea = new float[statArea.Count];
            string[] decKey = new string[statArea.Count];
            int i=0;
            int totalCount=0;
            float totalPixelArea=0;
            float totalFirArea=0;
            foreach (string key in statArea.Keys)
            {
                decArea[i] = statArea[key].PixelArea;
                decKey[i++] = key;
                totalCount+=statArea[key].PixelCount;
                totalPixelArea+=statArea[key].PixelArea;
                totalFirArea+=statArea[key].FireArea;
            }
            Array.Sort(decArea, decKey);
            Array.Reverse(decKey);
            List<string> nameItem = new List<string>();
            List<string> countItem = new List<string>();
            List<string> pixelAreaItem = new List<string>();
            List<string> fireAreaItem = new List<string>();
            nameItem.Add("矢量分区");
            countItem.Add("像元个数");
            pixelAreaItem.Add("覆盖范围（平方公里）");
            fireAreaItem.Add("明火面积（公顷）");
            List<string[]> resultList = new List<string[]>();
            for (int j = 0; j < decKey.Length; j++)
            {
                nameItem.Add(decKey[j]);
                countItem.Add(statArea[decKey[j]].PixelCount.ToString());
                pixelAreaItem.Add(statArea[decKey[j]].PixelArea.ToString());
                fireAreaItem.Add(statArea[decKey[j]].FireArea.ToString());
            }
            //合计
            nameItem.Add("合计");
            countItem.Add(totalCount.ToString());
            pixelAreaItem.Add(totalPixelArea.ToString());
            fireAreaItem.Add(totalFirArea.ToString());
            columns = nameItem.ToArray();
            resultList.AddRange(new List<string[]>{countItem.ToArray(), pixelAreaItem.ToArray(), fireAreaItem.ToArray()});
            return resultList.ToArray();
        }

        private Dictionary<string, FireAreaInfo> StatCustomArea(string[] plstFiles, ref string title, Dictionary<string, int[]> aoi)
        {
            return null;
        }

        private Dictionary<string, FireAreaInfo> AreaStatByType(string[] plstFiles, string statType)
        {
           switch(statType)
           {
               case "当前区域":return StatCurrentArea(plstFiles);
               case "土地利用类型":return StatUseTypeArea(plstFiles);
               case "省级行政区划":return StatPriviceArea(plstFiles);
               case "分级行政区划": return StatCountyArea(plstFiles);
               default: return null;
           }
        }

        private Dictionary<string, FireAreaInfo> StatCountyArea(string[] plstFiles)
        {
            Dictionary<string, FireAreaInfo> statArea = new Dictionary<string, FireAreaInfo>();
            IRasterDictionaryTemplate<int> xianJieDictionary = RasterDictionaryTemplateFactory.CreateXjRasterTemplate();
            foreach (string file in plstFiles)
            {
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines)
                {
                    string[] infos = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (infos != null && infos.Length == 12)
                    {
                        //按行进行统计
                        //前两位为省标识
                        string county = infos[4].Trim();
                        float pixelArea, fireRate;
                        if (!float.TryParse(infos[6], out pixelArea) || !float.TryParse(infos[7], out fireRate))
                            continue;
                        if (county == "0")
                        {
                            if (statArea.ContainsKey("其他"))
                            {
                                statArea["其他"].PixelCount++;
                                statArea["其他"].PixelArea += pixelArea ;
                                statArea["其他"].FireArea += fireRate*pixelArea;
                            }
                            else
                                statArea.Add("其他", new FireAreaInfo(1, pixelArea, fireRate * pixelArea));
                        }
                        else
                        {
                            int countyNum = Int32.Parse(county);
                            county = xianJieDictionary.GetPixelName(countyNum);
                            if (statArea.ContainsKey(county))
                            {
                                statArea[county].PixelCount++;
                                statArea[county].PixelArea += pixelArea ;
                                statArea[county].FireArea += fireRate * pixelArea;
                            }
                            else
                            {
                                statArea.Add(county, new FireAreaInfo(1, pixelArea, fireRate * pixelArea));
                            }
                        }
                    }
                }
            }
            return statArea;
        }

        private Dictionary<string, FireAreaInfo> StatPriviceArea(string[] plstFiles)
        {
            Dictionary<string, FireAreaInfo> statArea = new Dictionary<string, FireAreaInfo>();
            using (IRasterDictionaryTemplate<int> xianJieDictionary = RasterDictionaryTemplateFactory.CreateXjRasterTemplate())
            {
                foreach (string file in plstFiles)
                {
                    string[] lines = File.ReadAllLines(file);
                    foreach (string line in lines)
                    {
                        string[] infos = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (infos != null && infos.Length == 12)
                        {
                            //按行进行统计
                            //前两位为省标识
                            float pixelArea, fireRate;
                            if (!float.TryParse(infos[6], out pixelArea) || !float.TryParse(infos[7], out fireRate))
                                continue;
                            string privice = infos[4].Substring(0, 2);
                            int priviceNum;
                            if (!int.TryParse(privice, out priviceNum) || infos[4].Trim() == "0")
                            {
                                if (statArea.ContainsKey("其他"))
                                {
                                    statArea["其他"].PixelCount++;
                                    statArea["其他"].PixelArea += pixelArea;
                                    statArea["其他"].FireArea += fireRate * pixelArea;
                                }
                                else
                                    statArea.Add("其他", new FireAreaInfo(1, pixelArea, fireRate * pixelArea));
                            }
                            else
                            {
                                priviceNum = priviceNum * 10000;
                                privice = xianJieDictionary.GetPixelName(priviceNum);
                                if (statArea.ContainsKey(privice))
                                {
                                    statArea[privice].PixelCount++;
                                    statArea[privice].PixelArea += pixelArea;
                                    statArea[privice].FireArea += fireRate * pixelArea;
                                }
                                else
                                {
                                    statArea.Add(privice, new FireAreaInfo(1, pixelArea, fireRate * pixelArea));
                                }
                            }
                        }
                    }
                }
                return statArea;
            }
        }

        private Dictionary<string, FireAreaInfo> StatUseTypeArea(string[] plstFiles)
        {
            Dictionary<string, FireAreaInfo> statArea = new Dictionary<string, FireAreaInfo>();
            using (IRasterDictionaryTemplate<byte> landTypeDictionary = RasterDictionaryTemplateFactory.CreateLandRasterTemplate())
            {
                foreach (string file in plstFiles)
                {
                    string[] lines = File.ReadAllLines(file);
                    foreach (string line in lines)
                    {
                        string[] infos = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (infos != null && infos.Length == 12)
                        {
                            //按行进行统计
                            //前两位为省标识
                            string useType = infos[5].Trim();
                            float pixelArea, fireRate;
                            if (!float.TryParse(infos[6], out pixelArea) || !float.TryParse(infos[7], out fireRate))
                                continue;
                            if (useType == "0")
                            {
                                if (statArea.ContainsKey("其他"))
                                {
                                    statArea["其他"].PixelCount++;
                                    statArea["其他"].PixelArea += pixelArea;
                                    statArea["其他"].FireArea += fireRate * pixelArea;
                                }
                                else
                                    statArea.Add("其他", new FireAreaInfo(1, pixelArea, fireRate * pixelArea));
                            }
                            else
                            {
                                Byte useTypeNum = Byte.Parse(useType);
                                useType = landTypeDictionary.GetPixelName(useTypeNum);
                                if (statArea.ContainsKey(useType))
                                {
                                    statArea[useType].PixelCount++;
                                    statArea[useType].PixelArea += pixelArea;
                                    statArea[useType].FireArea += fireRate * pixelArea;
                                }
                                else
                                {
                                    statArea.Add(useType, new FireAreaInfo(1, pixelArea, fireRate * pixelArea));
                                }
                            }
                        }
                    }
                }
                return statArea;
            }
        }

        private Dictionary<string, FireAreaInfo> StatCurrentArea(string[] plstFiles)
        {
            Dictionary<string, FireAreaInfo> statArea = new Dictionary<string, FireAreaInfo>();
            statArea.Add("当前区域", new FireAreaInfo(0, 0, 0));
            foreach (string file in plstFiles)
            {
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines)
                {
                    string[] infos = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (infos != null && infos.Length == 12)
                    {
                        //按行进行统计
                        float pixelArea, fireRate;
                        if (!float.TryParse(infos[6], out pixelArea) || !float.TryParse(infos[7], out fireRate))
                            continue;
                        statArea["当前区域"].PixelCount++;
                        statArea["当前区域"].PixelArea += pixelArea ;
                        statArea["当前区域"].FireArea += fireRate*pixelArea;
                    }
                }
            }
            return statArea;
        }

        private string[] FindPLSTFiles()
        {
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            List<string> plstList = new List<string>();
            foreach (string file in files)
            {
                string dir = Path.GetDirectoryName(file);
                string plstDir = null;
                if (!dir.Contains("栅格产品"))
                    return null;
                plstDir = Path.Combine(Directory.GetParent(dir).ToString(), "信息列表");
                if (!Directory.Exists(plstDir))
                    return null;
                string plstName = Path.GetFileNameWithoutExtension(file).Replace("DBLV", "PLST");
                plstName = Path.Combine(plstDir, plstName + ".txt");
                if (!File.Exists(plstName))
                    return null;
                plstList.Add(plstName);
            }
            return plstList.ToArray();
        }
    }
}
