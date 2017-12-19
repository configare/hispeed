using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductAnalysisVgtNdvi : CmaMonitoringSubProduct
    {

        private IContextMessage _contextMessage = null;

        public SubProductAnalysisVgtNdvi()
            : base()
        {
        }

        public SubProductAnalysisVgtNdvi(SubProductDef subProductDef)
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
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("SelectedPrimaryFiles") == null)
            {
                PrintInfo("请选择统计文件！");
                return null;
            }
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
            if (_argumentProvider.GetArg("NDVIRegion") == null)
            {
                PrintInfo("请设置需要统计的植被指数分段值！");
                return null;
            }
            SortedDictionary<float, float> ndviRegions = _argumentProvider.GetArg("NDVIRegion") as SortedDictionary<float, float>;
            if (ndviRegions == null || ndviRegions.Count == 0)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "VTAT")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            string zoom = _argumentProvider.GetArg("resultZoom").ToString();
            float resultZoom = 1000;
            if (!string.IsNullOrEmpty(zoom))
                resultZoom = float.Parse(zoom);
            string outId = _argumentProvider.GetArg("OutFileIdentify") as string;
            string outFileId = CreatTitleByFileName(fname[0], outId);
            bool isCustom = false;
            if (_argumentProvider.GetArg("IsCustom") != null)
            {
                string s = _argumentProvider.GetArg("IsCustom").ToString();
                if (!string.IsNullOrEmpty(s))
                    isCustom = bool.Parse(s);
            }
            if (isCustom)
            {
                Dictionary<string, int[]> aoi = GetAOIArugment(fname[0], true);
                if (aoi != null && aoi.Count > 0)
                {
                    return CreatCustomStatResult(aoi, ndviRegions, resultZoom, fname, outFileId);
                }
            }
            else
            {
                SubProductInstanceDef instance = FindSubProductInstanceDefs(outFileId);
                if (instance == null)
                {
                    return STATAlgorithm();
                }
                else
                {
                    _argumentProvider.SetArg("OutFileIdentify", outFileId);
                    Dictionary<string, Func<short, bool>> filters = new Dictionary<string, Func<short, bool>>();
                    foreach (float key in ndviRegions.Keys)
                    {
                        float min = key;
                        float max = ndviRegions[key];
                        string filterKey = min + "-" + max;
                        filters.Add(filterKey, (v) =>
                        {
                            float value = v / resultZoom;
                            return (value >= min && value < max);
                        });
                    }
                    return StatProcentRaster<short>(fname[0], filters, progressTracker);
                }
            }
            return null;
        }

        private IExtractResult StatProcentRaster<T>(string fname, Dictionary<string, Func<short, bool>> filters, Action<int, string> progressTracker)
        {
            string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
            SortedDictionary<string, double[][]> srcResult = RasterStatFactory.StatPercent<short>(fname, statString, filters, progressTracker);
            //删除为空的AOI区域以及计算百分比
            SortedDictionary<string,double[][]> statResult=new SortedDictionary<string,double[][]>();
            //区间个数
            int regionCount=filters.Keys.Count;
            foreach (string key in srcResult.Keys)
            {
                if (srcResult[key] == null)
                    continue;
                else
                {
                    int zeroCount=0;
                    double[][] value = new double[regionCount][];
                    for (int i = 0; i < regionCount; i++)
                    {
                        value[i] = new double[] { srcResult[key][i][0], srcResult[key][i][0]/srcResult[key][i][1]*100 };
                        if(value[i][0]==0)
                           zeroCount++;
                    }
                    //如果全为0，也不添加
                    if(zeroCount< regionCount)
                       statResult.Add(key,value);
                }
            }
            if (statResult.Count == 0)
                return null;
            string productName = _subProductDef.ProductDef.Name;
            string title = productName + "行政区划" + "植被指数统计";
            string subTitle = GetSubTitle(fname);
            IStatResult results = DicToStatResult(statResult, filters.Keys.ToArray(), subTitle);
            if (results == null)
                return null;
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string productIdentify = _subProductDef.ProductDef.Identify;
            string filename = StatResultToFile(new string[]{fname}, results, productIdentify, outFileIdentify, title, null, 1, false, 1);
            return new FileExtractResult(outFileIdentify, filename);
        }

        private IStatResult DicToStatResult(SortedDictionary<string, double[][]> areaResultDic, string[] filterKeys, string subTitle)
        {
            string[] rowKeys = areaResultDic.Keys.ToArray(); //行
            List<string> cols = new List<string>();          //列
            cols.Add("统计分类");
            foreach (string item in filterKeys)
            {
                cols.AddRange(new string[]{item + "面积", item + "百分比"});
            }
            string[] columns = cols.ToArray();
            List<string[]> rows = new List<string[]>();
            for (int i = 0; i < rowKeys.Length; i++)
            {
                string type = rowKeys[i];
                string[] row = new string[1 + filterKeys.Length*2];
                row[0] = type;
                for (int j = 0; j < filterKeys.Length; j++)
                {
                    row[2 * j + 1] = areaResultDic[type][j][0].ToString();
                    row[2 * (j + 1)] = areaResultDic[type][j][1].ToString();
                }
                rows.Add(row);
            }
            if (rows == null || rows.Count == 0)
                return null;
            else
                return new StatResult(subTitle, columns, rows.ToArray());
        }

        private string CreatTitleByFileName(string fname, string statType)
        {
            RasterIdentify id = new RasterIdentify(fname);
            string identify = id.SubProductIdentify;
            if (string.IsNullOrEmpty(identify))
                return null;
            switch (identify)
            {
                case "NDVI":
                    statType = statType.Remove(0, 1);
                    return "N" + statType;
                case "0RVI":
                    statType = statType.Remove(0, 1);
                    return "R" + statType;
                case "0DVI":
                    statType = statType.Remove(0, 1);
                    return "D" + statType;
                case "0EVI":
                    statType = statType.Remove(0, 1);
                    return "E" + statType;
                case "0VCI":
                    statType = statType.Remove(0, 1);
                    return "V" + statType;
                case "CHAZ":
                    statType = statType.Remove(0, 1);
                    return "C" + statType;
                default:
                    statType = statType.Remove(0, 1);
                    return "N" + statType;
            }
        }

        private IExtractResult STATAlgorithm()
        {
            return AreaStatResult<Int16>("干旱", "DRT", (v) => { return v == 1; });
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
            string filename = StatResultToFile(fname, statResult, "VGT", outFileId, "植被指数自定义区域统计", extInfos, 1, true, 1);
            return new FileExtractResult(outFileId, filename);
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

        //private string CreatTitleByFileName(string fname)
        //{
        //    RasterIdentify id = new RasterIdentify(fname);
        //    string identify = id.SubProductIdentify;
        //    if (string.IsNullOrEmpty(identify))
        //        return null;
        //    switch (identify)
        //    {
        //        case "NDVI":
        //            return "植被指数NDVI面积统计";
        //        case "0RVI":
        //            return "植被指数RVI面积统计";
        //        case "0EVI":
        //            return "植被指数EVI面积统计";
        //        case "0DVI":
        //            return "植被指数DVI面积统计";
        //        case "0VCI":
        //            return "植被状态指数面积统计";
        //        case "CHAZ":
        //            return "植被指数差值面积统计";
        //        default:
        //            return "植被指数面积统计";
        //    }
        //}

        private bool TryCreatExcelFile(IStatResult StatResult, string productTitle, string outFileName)
        {
            try
            {
                if (File.Exists(outFileName))
                    File.Delete(outFileName);
                using (StatResultToExcelFile excelControl = new StatResultToExcelFile())
                {
                    excelControl.Init();
                    excelControl.Add(true, productTitle, StatResult, true, 1);
                    if (!outFileName.ToUpper().EndsWith(".XLSX"))
                        outFileName += ".XLSX";
                    excelControl.SaveFile(outFileName);
                }
                return true;
            }
            catch (Exception)
            {
                using (StatResultToTxtFile txtControl = new StatResultToTxtFile())
                {
                    if (!outFileName.ToUpper().EndsWith(".TXT"))
                        outFileName += ".TXT";
                    txtControl.WriteResultToTxt(productTitle + "\n");
                    txtControl.WriteResultToTxt("统计日期：" + DateTime.Today.Date.ToShortDateString() + "\n");
                    txtControl.WriteResultToTxt(StatResult);
                    bool isSave = txtControl.SaveFile(outFileName);
                    if (!isSave)
                        return false;
                }
            }
            return true;
        }

        private string CreatOutputFileName(string fname,bool isCustom)
        {
            IFileNameGenerator fng = GetFileNameGenerator();
            DataIdentify di = GetDataIdentify();
            string outputIdentify = _argumentProvider.GetArg("OutFileIdentify").ToString();
            if (string.IsNullOrEmpty(outputIdentify))
                outputIdentify = GetOutputIdentifyByFilename(fname,isCustom);
            RasterIdentify rid = CreatRasterIndetifyId(new string[] { fname }, "VGT", outputIdentify, di, ".xlsx", null);
            return (fng == null) ? string.Empty : fng.NewFileName(rid);
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
                    columns[i] = "植被指数[" + region.Key.ToString() + "到" + region.Value.ToString() + "]";
                    s[i] = (CalcTotalArea(prd, keyValue.Value, region.Key * resultZoom, region.Value * resultZoom).ToString());
                    i++;
                }
                rows.Add(s);
            }
            string[][] ros = rows.ToArray();
            return new StatResult(productTitle, columns, ros);
        }

        //private Dictionary<string, int[]> GetAoiDictionary(IRasterDataProvider prd, bool isCustom, string fname)
        //{
        //    if (!isCustom)
        //    {
        //        string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
        //        string[] statInfos = statString.Split(':');
        //        return ApplyStatByVectorTemplate(prd, statInfos[1], statInfos[2]);
        //    }
        //    else
        //        return GetAOIArugment(fname, true);
        //}

        private Dictionary<string, int[]> GetAOIArugment(string fname, bool multiSelect)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates(new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.ResolutionX))
                {
                    frm.listView1.MultiSelect = multiSelect;
                    if (frm.ShowDialog() == DialogResult.OK)
                        return frm.GetFeatureAOIIndex();
                    else
                        return null;
                }
            }
        }

        private string GetOutputIdentifyByFilename(string fname,bool isCustom)
        {
            RasterIdentify raster = new RasterIdentify(fname);
            string subIdentify = raster.SubProductIdentify;
            if (!isCustom)
            {
                switch (subIdentify)
                {
                    case "NDVI":
                        return "NDVC";
                    case "0RVI":
                        return "RVIC";
                    case "0EVI":
                        return "EVIC";
                    case "0DVI":
                        return "DVIC";
                    case "0VCI":
                        return "VCIC";
                    default:
                        return null;
                }
            }
            else
            {
                switch (subIdentify)
                {
                    case "NDVI":
                        return "NDVA";
                    case "0RVI":
                        return "RVIA";
                    case "0EVI":
                        return "EVIA";
                    case "0DVI":
                        return "DVIA";
                    case "0VCI":
                        return "CVCI";
                    default:
                        return null;
                }
            }
        }

        private IFileNameGenerator GetFileNameGenerator()
        {
            object obj = _argumentProvider.GetArg("FileNameGenerator");
            if (obj == null)
                return null;
            return obj as IFileNameGenerator;
        }

        private float[] GetNdviValue(string region)
        {
            if (string.IsNullOrEmpty(region))
                return null;
            string[] minmax = region.Split('~');
            if (minmax == null || minmax.Length == 0)
                return null;
            float min = float.Parse(minmax[0]);
            float max = float.Parse(minmax[1]);
            return new float[] { min, max };
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

        //private Dictionary<string, int[]> ApplyStatByVectorTemplate(IRasterDataProvider raster, string shpTemplateName, string primaryFieldName)
        //{
        //    string shpFullname = VectorAOITemplate.FindVectorFullname(shpTemplateName);
        //    return ResultsWithoutZero(CountByVector(raster, shpFullname, primaryFieldName));
        //}

        private Dictionary<string, int[]> ResultsWithoutZero(Dictionary<string, int[]> items)
        {
            if (items == null || items.Count() == 0)
                return null;
            Dictionary<string, int[]> results = new Dictionary<string, int[]>();
            foreach (KeyValuePair<string, int[]> item in items)
            {
                if (item.Value != null && item.Value.Count() != 0)
                    results.Add(item.Key, item.Value);
            }
            if (results == null || results.Count == 0)
                return null;
            return results;
        }

        //private Dictionary<string, int[]> CountByVector(IRasterDataProvider raster, string shpFullname, string shpPrimaryField)
        //{
        //    if (String.IsNullOrEmpty(shpFullname))
        //        return null;
        //    //step2:读取矢量
        //    CodeCell.AgileMap.Core.Feature[] features = _aoiTempStat.GetFeatures(shpFullname);
        //    if (features == null || features.Length == 0)
        //        return null;
        //    //step3:矢量栅格化
        //    Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
        //    Dictionary<string, int[]> dictList = new Dictionary<string, int[]>();
        //    using (Bitmap bitmap = _aoiTempStat.VectorsToBitmap(raster, features, shpPrimaryField, out nameColors))
        //    {
        //        int[] aoi;
        //        Color color;
        //        string name;
        //        foreach (Feature fea in features)
        //        {
        //            name = fea.GetFieldValue(shpPrimaryField);
        //            if (String.IsNullOrEmpty(name))
        //                continue;
        //            color = nameColors[name];
        //            aoi = _aoiTempStat.GetAOIByFeature(bitmap, color);
        //            dictList.Add(name, aoi);
        //        }
        //    }
        //    if (dictList == null)
        //        return null;
        //    return (dictList.Count() != 0) ? dictList : null;
        //}

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
