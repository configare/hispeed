#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：李喜佳    时间：2013-10-24 17:06:27
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

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
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductNewSTATSNW
    /// 属性描述：雪深雪水当量不同深度范围面积统计
    /// 创建者：lxj   创建日期：2013-10-24 17:06:27
    /// 修改者： lxj            修改日期：
    /// 修改描述：雪深和雪水当量分开两个算法，根据面积由大小排列
    /// 备注：
    /// </summary>
    public class SubProductAreaSTATSNW : CmaMonitoringSubProduct  
    {
        private IContextMessage _contextMessage = null;
        private IArgumentProvider _curArguments = null;
        List<string> fieldValues = new List<string>();
        public SubProductAreaSTATSNW(SubProductDef subProductDef)
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
        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;

            if (_curArguments.GetArg("AlgorithmName").ToString() == "ASTATAlgorithm")
            {
                return ASTATAlgorithm(progressTracker, contextMessage);
            }

            if (_curArguments.GetArg("AlgorithmName").ToString() == "SWEASTATAlgorithm")
            {
                return SWEASTATAlgorithm(progressTracker,contextMessage);
            }

            return null;

        }
        private IExtractResult ASTATAlgorithm(Action<int, string> progressTracker, IContextMessage contextMessage)
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
            if (_argumentProvider.GetArg("UCRegionSnowDepth") == null)
            {
                PrintInfo("请设置需要统计的指数分段值！");
                return null;
            }
            SortedDictionary<float, float> sdRegions = _argumentProvider.GetArg("UCRegionSnowDepth") as SortedDictionary<float, float>;
            if (sdRegions == null || sdRegions.Count == 0)
                return null;

            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }

            string outId = _argumentProvider.GetArg("OutFileIdentify") as string;
            string outFileId = CreatTitleByFileName(fname[0], outId);
            
                SubProductInstanceDef instance = FindSubProductInstanceDefs(outId);
                if (instance == null)
                {
                    return AreaStatResult<Int16>("雪深", "MWS", (v) => { return v == 1; });
                }
                else
                {
                    _argumentProvider.SetArg("OutFileIdentify", outFileId);
                    Dictionary<string, Func<float, bool>> filters = new Dictionary<string, Func<float, bool>>();
                    foreach (float key in sdRegions.Keys)
                    {
                        float min = key;
                        float max = sdRegions[key];
                        string filterKey = min + "—" + max + "厘米 " + "覆盖面积(平方公里)";
                        filters.Add(filterKey, (v) =>
                        {
                            double value = v;
                            return (value >= min && value < max);
                        });
                    }
                    string fieldName;
                    string shapeFilename;
                    int fieldIndex = -1;
                    if (instance.AOIProvider == "当前区域" || instance.AOIProvider == "土地利用类型")
                        return StatRaster<float>(instance, filters, progressTracker);
                    else
                    {
                        if (instance.AOIProvider == "县级行政区划")
                        {
                            using (frmStatSXRegionTemplates frm = new frmStatSXRegionTemplates())
                            {
                                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    Feature[] fets = frm.GetSelectedFeatures();
                                    fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                                    if (fets == null)
                                        PrintInfo("未选择任何地区");
                                    fieldValues.Clear();
                                    foreach (Feature fet in fets)
                                    {
                                        fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                    }
                                }
                            }
                            string statString = AreaStatProvider.GetAreaStatItemFileName("县级行政区划");
                            return StatRasterByVector(instance.Name, statString, filters, progressTracker);
                        }
                        else
                        {
                            if (instance.AOIProvider == "省级行政区划")
                            {
                                using (frmStatProvinceRegionTemplates frm = new frmStatProvinceRegionTemplates())
                                {
                                    if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                    {
                                        Feature[] fets = frm.GetSelectedFeatures();
                                        fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                                        if (fets == null)
                                            PrintInfo("未选择任何地区");
                                        fieldValues.Clear();
                                        foreach (Feature fet in fets)
                                        {
                                            fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                        }
                                    }
                                }
                                string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
                                return StatRasterByVector(instance.Name, statString, filters, progressTracker);
                            }
                        }

                    }
                }
         
            return null;
        }
        private IExtractResult SWEASTATAlgorithm(Action<int, string> progressTracker, IContextMessage contextMessage)
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
            if (_argumentProvider.GetArg("UCRegionSnowDepth") == null)
            {
                PrintInfo("请设置需要统计的指数分段值！");
                return null;
            }
            SortedDictionary<float, float> sdRegions = _argumentProvider.GetArg("UCRegionSnowDepth") as SortedDictionary<float, float>;
            if (sdRegions == null || sdRegions.Count == 0)
                return null;

            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            
            string outId = _argumentProvider.GetArg("OutFileIdentify") as string;
            string outFileId = CreatTitleByFileName(fname[0], outId);

            SubProductInstanceDef instance = FindSubProductInstanceDefs(outId);
            if (instance == null)
            {
                return AreaStatResult<Int16>("雪深", "MWS", (v) => { return v == 1; });
            }
            else
            {
                _argumentProvider.SetArg("OutFileIdentify", outFileId);
                Dictionary<string, Func<float, bool>> filters = new Dictionary<string, Func<float, bool>>();
                foreach (float key in sdRegions.Keys)
                {
                    float min = key;
                    float max = sdRegions[key];
                    string filterKey = min + "—" + max + "毫米" + " 覆盖面积(平方公里)";
                    filters.Add(filterKey, (v) =>
                    {
                        double value = v;
                        return (value >= min && value < max);
                    });
                }
                //return StatRaster<float>(instance, filters, progressTracker);
                string fieldName;
                string shapeFilename;
                int fieldIndex = -1;
                if (instance.AOIProvider == "当前区域" || instance.AOIProvider == "土地利用类型")
                    return StatRaster<float>(instance, filters, progressTracker);
                else
                {
                    if (instance.AOIProvider == "县级行政区划")
                        {
                            using (frmStatSXRegionTemplates frm = new frmStatSXRegionTemplates())
                            {

                                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    Feature[] fets = frm.GetSelectedFeatures();
                                    fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                                    if(fets == null)
                                        PrintInfo("未选择任何地区");
                                    fieldValues.Clear();
                                    foreach (Feature fet in fets)
                                    {
                                        fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                    }
                                }
                            }
                            string statString = AreaStatProvider.GetAreaStatItemFileName("县级行政区划");
                            return StatRasterByVector(instance.Name, statString, filters, progressTracker);
                        }
                    else
                    {
                        if(instance.AOIProvider== "省级行政区划")
                        {
                            using (frmStatProvinceRegionTemplates frm = new frmStatProvinceRegionTemplates())
                            {
                                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    Feature[] fets = frm.GetSelectedFeatures();
                                    fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                                    if(fets == null)
                                        PrintInfo("未选择任何地区");
                                    fieldValues.Clear();
                                    foreach (Feature fet in fets)
                                    {
                                        fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                    }
                                }
                            }
                            string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
                                   
                            return StatRasterByVector(instance.Name, statString, filters, progressTracker);
                        }
                    }      
                }
             }
            return null;
        }
        private string CreatTitleByFileName(string fname, string statType)
        {
            RasterIdentify id = new RasterIdentify(fname);
            string identify = id.SubProductIdentify;
            if (string.IsNullOrEmpty(identify))
                return null;
            switch (identify)
            {
                case "MWSD":
                    statType = statType.Remove(0, 1);
                    return "D" + statType;
                case "MSWE":
                    statType = statType.Remove(0, 1);
                    return "E" + statType;
                default:
                    statType = statType.Remove(0, 1);
                    return "V" + statType;
            }
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
        
        private IExtractResult StatRasterByVector(string instanceName, string aoiTemplate, Dictionary<string, Func<float, bool>> filters, Action<int, string> progressTracker)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string title = GetStringArgument("title");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            string outFileIdentify = GetStringArgument("OutFileIdentify");//outfileidentify
            string statname = GetStringArgument("statname");
            string productName = _subProductDef.ProductDef.Name;
            string productIdentify = _subProductDef.ProductDef.Identify;
            Dictionary<string, SortedDictionary<string, double>> dic = null;
            if (files == null || files.Length == 0)
                return null;
            foreach (string file in files)
            {
                dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker);
            }
            dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker);
            if (dic == null || dic.Count == 0)
                return null;
            title = productName + statname + instanceName;
            string subTitle = GetSubTitle(files);
            IStatResult results = DicToStatResult(dic, filters.Keys.ToArray(), subTitle);
            if (results == null)
                return null;
            string filename = StatResultToFile(files, results, productIdentify, outFileIdentify, title, extInfos, 1, true, 1);
            return new FileExtractResult(outFileIdentify, filename);
        }
        private IStatResult DicToStatResult(Dictionary<string, SortedDictionary<string, double>> areaResultDic, string[] filterKeys, string subTitle)
        {
            List<string> rowKeys1 = new List<string>();
            //只加入所选的省份和地区
            foreach (string shengname in areaResultDic.Keys)
            {
                foreach (string name in fieldValues)
                {
                    if (shengname == name)
                    {
                        rowKeys1.Add(shengname);
                    }
                }
            }
            string[] rowKeys = rowKeys1.ToArray();  // 行
            List<string> cols = new List<string>(); // 列
            cols.Add("统计分类");
            cols.AddRange(filterKeys);
            string[] columns = cols.ToArray();
            List<string[]> rows = new List<string[]>();
            //1.定义Dictionary 一维是地名，二维是数组
            Dictionary<string, double[]> dRow = new Dictionary<string, double[]>();
            //2.定义Dictionary 一维是地名，二维是分段累加计算的和
            Dictionary<double,string> sRow = new Dictionary<double,string>();
            List<double> list = new List<double>();
            for (int i = 0; i < rowKeys.Length; i++)
            {
                string type = rowKeys[i]; //地名
                SortedDictionary<string, double> rowStat = areaResultDic[type];  
                double[] nRow = new double[filterKeys.Length]; //存放具体分段数值
                double sumR = 0;
                for (int j = 0; j < filterKeys.Length; j++)
                {
                    string key = filterKeys[j];
                    if (rowStat.ContainsKey(key))
                    {
                        nRow[j] = rowStat[key];
                        sumR += nRow[j];
                    }
                    else
                    {
                        nRow[j] = 0;
                        sumR += nRow[j];
                    }
                }
                dRow.Add(type , nRow);//(地名，分段数组)
                sRow.Add(sumR,type); //(分段面积加和，地名)
                list.Add(sumR);      //(分段面积的和)
            }
            list.Sort();  //排序
            string nam = null; // 地名
            double[] ddRow = new double[filterKeys.Length]; 
            double[] ll = list.ToArray();
            for (int i = ll.Length - 1; i >= 0; i--)// 降序，面积由大到小排列
            {
                bool bb = sRow.TryGetValue(ll[i], out nam); // 输出地名
                string[] row = new string[1 + filterKeys.Length];
                row[0] = nam;
                bool aa = dRow.TryGetValue(nam, out ddRow);  // 根据地名，输出地名对应的分段数组
                for (int j = 0; j < filterKeys.Length; j++)
                {
                    row[j + 1] = ddRow[j].ToString();
                }
                rows.Add(row);
            }
            if (rows == null || rows.Count == 0)
                return null;
            else
                return new StatResult(subTitle, columns, rows.ToArray());
        }
        private static string GetSubTitle(string[] files)
        {
            string subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
            string orbitTimes = string.Empty;
            foreach (string item in files)
            {
                if (!File.Exists(item))
                    break;
                RasterIdentify rasterId = new RasterIdentify(item);
                orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + " ";
            }
            return subTitle += "\n" + "轨道时间：" + orbitTimes;
        }
    }
}
