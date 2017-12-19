#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2013-11-4 11:22:35
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
    /// 类名：SubProductSweVolSTAT
    /// 属性描述：雪水当量体积统计
    /// 创建者：lxj    创建日期：2013-11-4 11:22:35
    /// 修改者：             修改日期：
    /// 修改描述：根据体积由大到小排序
    /// 备注：
    /// </summary>
    public class SubProductSweVolSTAT:CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;
        List<string> fieldValues = new List<string>();

        public SubProductSweVolSTAT(SubProductDef subProductDef)
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
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "VSTATAlgorithm")
            {
                string[] fname = GetStringArray("SelectedPrimaryFiles");
               
                //按照Instance执行统计操作
                string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
                string fieldName;
                string shapeFilename;
                int fieldIndex = -1;
                if (instanceIdentify != null)
                {
                    SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                    if (instance.OutFileIdentify == "VCBP")    
                    {
                        using (frmStatProvinceRegionTemplates frm = new frmStatProvinceRegionTemplates())
                        {
                            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Feature[] fets = frm.GetSelectedFeatures();
                                fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                                fieldValues.Clear();
                                foreach (Feature fet in fets)
                                {
                                    fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                }
                            }
                        }
                        string aoiprovider = "省级行政区域_面.shp";
                        string keyname = "NAME";
                        string sweVolFile = fname[0].Replace("MSWE", "SVOL");
                        if (!File.Exists(sweVolFile))
                        {
                            IFileExtractResult sweVolResult = ComputeSnowSWEVOL(fname[0]);
                            sweVolFile = sweVolResult.FileName;
                        }
                        return VSTATAlgorithm(sweVolFile, aoiprovider,keyname);
                    }
                    if (instance.OutFileIdentify == "VCBC")
                    {
                        using (frmStatSXRegionTemplates frm = new frmStatSXRegionTemplates())
                        {

                            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Feature[] fets = frm.GetSelectedFeatures();
                                fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                                fieldValues.Clear();
                                foreach (Feature fet in fets)
                                {
                                    fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                }
                            }
                        }
                        string aoiprovider = "县级行政区域_面.shp";
                        string keyname = "NAME";
                        string sweVolFile = fname[0].Replace("MSWE", "SVOL");
                        if (!File.Exists(sweVolFile))
                        {
                            IFileExtractResult sweVolResult = ComputeSnowSWEVOL(fname[0]);
                            sweVolFile = sweVolResult.FileName;
                        }
                        return VSTATAlgorithm(sweVolFile, aoiprovider, keyname);
                    }
                    if (instance.OutFileIdentify == "VLUT")
                    {
                        string aoiprovider = "土地利用类型_合并.shp";
                        string keyname = "NAME";
                        string sweVolFile = fname[0].Replace("MSWE", "SVOL");
                        if (!File.Exists(sweVolFile))
                        {
                            IFileExtractResult sweVolResult = ComputeSnowSWEVOL(fname[0]);
                            sweVolFile = sweVolResult.FileName;
                        }
                        return VSTATAlgorithm(sweVolFile, aoiprovider, keyname);
                    }

                    if (instance.OutFileIdentify == "VCAR")
                    {
                        string aoiprovider = "";
                        string keyname = "";
                        string sweVolFile = fname[0].Replace("MSWE", "SVOL");
                        if (!File.Exists(sweVolFile))
                        {
                            IFileExtractResult sweVolResult = ComputeSnowSWEVOL(fname[0]);
                            sweVolFile = sweVolResult.FileName;
                        }
                        return VSTATAlgorithm(sweVolFile, aoiprovider, keyname);
                    }
                   
                }
            }
            return null;
        }
        private IExtractResult VSTATAlgorithm(string rasterFileName, string aoi,string keyName)
        {
            //IRasterDataProvider inRaster = RasterDataDriver.Open(rasterFileName) as IRasterDataProvider;
            RasterStatByVector<float> stat = new RasterStatByVector<float>(null);
            IStatResult fresult = null;
            string filename = "";
            List<string[]> resultList = new List<string[]>();
            using (IRasterDataProvider inRaster = RasterDataDriver.Open(rasterFileName) as IRasterDataProvider)
            {
                if (aoi == "")
                {
                    double pixelSum = CompuCurPixel(rasterFileName);
                    resultList.Add(new string[] { "当前区域", pixelSum.ToString() });
                }
                else
                {
                    SortedDictionary<string, StatAreaItem> result;
                    result = stat.CountByVector(inRaster, aoi, keyName,
                        (cur, cursum) =>
                        {
                            return cursum += cur;
                        });
                    if (result.Count == 0)
                        return null;
                    if (aoi == "土地利用类型_合并.shp")
                    {
                        foreach (string key in result.Keys)
                        {
                            resultList.Add(new string[] { key, result[key].GrandTotal.ToString() });
                        }
                    }
                    else
                    {
                        Dictionary<double, string> dic = new Dictionary<double, string>();
                        List<double> list = new List<double>();
                        foreach (string key in result.Keys)
                        {
                            foreach (string name in fieldValues)
                            {
                                if (key == name)
                                {
                                    dic.Add(result[key].GrandTotal, key);
                                    list.Add(result[key].GrandTotal);
                                }
                            }
                        }
                        list.Sort();
                        string nam = null;
                        double[] ll = list.ToArray();
                        for (int i = ll.Length - 1; i >= 0; i--)// 降序加入到resultList中
                        {
                            bool bb = dic.TryGetValue(ll[i], out nam);
                            resultList.Add(new string[] { nam, ll[i].ToString() });
                        }
                    }
                }
                string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
                RasterIdentify id = new RasterIdentify(rasterFileName);
                id.ProductIdentify = "MWS";
                id.SubProductIdentify = "VSTA";
                if (id.OrbitDateTime != null)
                    sentitle += "    轨道日期：" + id.OrbitDateTime.ToShortDateString();
                string coluString = "";
                if (aoi == "省级行政区域_面.shp")
                {
                    coluString = "省界";
                }
                if (aoi == "县级行政区域_面.shp")
                {
                    coluString = "市县";
                }
                if (aoi == "土地利用类型_合并.shp")
                {
                    coluString = "土地利用类型";
                }
                if (aoi == "")
                {
                    coluString = "当前区域";
                }
                string[] columns = new string[] { coluString, "累计雪水当量体积(立方米)" };
                fresult = new StatResult(sentitle, columns, resultList.ToArray());
                string outputIdentify = _argumentProvider.GetArg("OutFileIdentify").ToString();
                string title = coluString + "雪水当量体积统计";
                filename = StatResultToFile(new string[] { rasterFileName }, fresult, "MWS", outputIdentify, title, null, 1, true, 1);
                return new FileExtractResult(outputIdentify, filename);
            }
        }
        private double CompuCurPixel(string rasterFileName)
        {
            IRasterDataProvider inRaster = RasterDataDriver.Open(rasterFileName) as IRasterDataProvider;
            ArgumentProvider ap = new ArgumentProvider(inRaster, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            double result = 0;
            visitor.VisitPixel(new int[] { 1 }, (index, values) =>
                {
                    if (values[0] > 0)
                        result += values[0];
                });
             return result;
 
        }

        //计算雪水当量体积
        private IFileExtractResult ComputeSnowSWEVOL(string swefilename)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(swefilename) as IRasterDataProvider;
                //计算文件中每个像元的面积
                int width = inRaster1.Width;
                float maxLat =(float) inRaster1.CoordEnvelope.MaxY;
                float res = inRaster1.ResolutionX;
                int row = inRaster1.Height;

                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);

                string sweVolFileName = GetFileName(new string[] { swefilename }, _subProductDef.ProductDef.Identify, "SVOL", ".dat", null);
                outRaster = CreateOutRaster(sweVolFileName, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        float[] swetmpVol = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            row = (int)(i / rvInVistor[0].SizeX) + 1;
                            float area = ComputePixelArea(row, maxLat, res);
                            //面积由平方公里转换为平方米，雪水当量mm转换为m
                            //swetmpVol[i] = rvInVistor[0].RasterBandsData[0][i] * area * 1000000.0f * 0.001f;
                            if (rvInVistor[0].RasterBandsData[0][i] == -999.0f || rvInVistor[0].RasterBandsData[0][i] == 0.003f)
                            {
                                swetmpVol[i] = 0.0f;
                            }
                            else 
                            {
                                swetmpVol[i] = rvInVistor[0].RasterBandsData[0][i] * area * 1000.0f;
                            }
                            rvOutVistor[0].RasterBandsData[0][i] = swetmpVol[i];
                        }

                    }
                }));
                rfr.Excute();
                IFileExtractResult res1 = new FileExtractResult(_subProductDef.Identify, sweVolFileName, true);
                res1.SetDispaly(false);
                return res1;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }

        public float ComputePixelArea(int row, float maxLat, float resolution)
        {
            float lat = maxLat - row * resolution;
            float a = 6378.137f;
            float c = 6356.7523142f;
            float latComputeFactor = 111.13f;
            float factor = (float)Math.Pow(Math.Tan(lat * Math.PI / 180d), 2);
            float lon =(float)( resolution * 2 * Math.PI * a * c * Math.Sqrt(1 / (c * c + a * a * factor)) / 360d);
            return lon * latComputeFactor * resolution;
        }

        //创建输出删格文件
        protected IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Intersect(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
