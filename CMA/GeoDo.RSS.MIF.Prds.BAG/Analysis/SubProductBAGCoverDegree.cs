using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductBAGCoverDegree : CmaMonitoringSubProduct
    {
        //是否进行云覆盖度计算
        private bool _isComputCloudCD = false;

        public SubProductBAGCoverDegree(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _algorithmDefs = new List<AlgorithmDef>(subProductDef.Algorithms);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null || _argumentProvider.GetArg("BPCDFile") == null)
                return null;
            string bPCDFName = _argumentProvider.GetArg("BPCDFile").ToString();
            string algName = _argumentProvider.GetArg("AlgorithmName").ToString();
            string extinfo = GetStringArgument("extinfo");
            if (string.IsNullOrEmpty(bPCDFName) || !File.Exists(bPCDFName) || string.IsNullOrEmpty(algName))
                return null;
            if (_argumentProvider.AOI == null)
                return null;
            //若云判识文件未给出或者文件不存在，则不计算云覆盖度
            string cloudName = string.Empty;
            if (_argumentProvider.GetArg("CLMFile") != null)
            {
                cloudName = _argumentProvider.GetArg("CLMFile").ToString();
                if (!string.IsNullOrEmpty(cloudName) && File.Exists(cloudName))
                    _isComputCloudCD = true;
            }
            if (algName == "BACD")
            {
                IRasterDataProvider bPCDDataProvider = null;
                IRasterDataProvider cloudDataProvider = null;
                try
                {
                    bPCDDataProvider = GeoDataDriver.Open(bPCDFName) as IRasterDataProvider;
                    if (bPCDDataProvider == null)
                        return null;
                    if (_isComputCloudCD)
                    {
                        cloudDataProvider = GeoDataDriver.Open(cloudName) as IRasterDataProvider;
                    }  
                    //_pixelArea = new BAGStatisticHelper().CalPixelArea(bPCDDataProvider.ResolutionX);
                    //总覆盖度
                    float totalCD = StatTotalConvertDegree(bPCDDataProvider, _argumentProvider.AOI);
                    //计算实际覆盖度
                    float actualCD = CalcActualConvertDegree(bPCDDataProvider, _argumentProvider.AOI);
                    //计算相对覆盖度
                    float absoluteCD = CalcAbsoluteConvertDegree(bPCDDataProvider, _argumentProvider.AOI);
                    //计算云覆盖度（参数：云判识结果文件）
                    float cloudCD = CalcCloudConvertDegree(cloudDataProvider, _argumentProvider.AOI);
                    List<string[]> valueItems = new List<string[]>();
                    List<string> valueRow = new List<string>();
                    valueRow.Add("整个湖区");
                    valueRow.Add(Math.Round((totalCD * 100), 2).ToString() + "%");
                    valueRow.Add(Math.Round((actualCD * 100), 2).ToString() + "%");
                    valueRow.Add(Math.Round((absoluteCD * 100), 2).ToString() + "%");
                    valueRow.Add(Math.Round((cloudCD * 100), 2).ToString() + "%");
                    valueItems.Add(valueRow.ToArray());
                    Dictionary<string, string> templates = BAGStatisticHelper.GetAOITemplateList();   
                    if (templates != null && templates.Count > 0)
                    {
                        Dictionary<string, int[]> aoiValues = new Dictionary<string, int[]>();
                        foreach (string templateName in templates.Keys)
                        {
                            Dictionary<string, int[]> aois = VectorTemplateToAOIConvertor.GetFeatureAOIIndex(templates[templateName], "NAME",
                                bPCDDataProvider.CoordEnvelope, new Size(bPCDDataProvider.Width, bPCDDataProvider.Height));
                            if (aois != null && aois.Count != 0)
                            {
                                foreach (string key in aois.Keys)
                                {
                                    aoiValues.Add(key, aois[key]);
                                }
                            }
                        }
                        if (aoiValues.Count > 0)
                        {
                            foreach (string name in aoiValues.Keys)
                            {
                                //总覆盖度
                                float total = StatTotalConvertDegree(bPCDDataProvider, aoiValues[name]);
                                //计算实际覆盖度
                                float actual = CalcActualConvertDegree(bPCDDataProvider, aoiValues[name]);
                                //计算相对覆盖度
                                float absolute = CalcAbsoluteConvertDegree(bPCDDataProvider, aoiValues[name]);
                                //计算云覆盖度（参数：云判识结果文件）
                                float cloud = CalcCloudConvertDegree(cloudDataProvider, aoiValues[name]);
                                List<string> row = new List<string>();
                                row.Add(name);
                                row.Add(Math.Round((total * 100), 2).ToString() + "%");
                                row.Add(Math.Round((actual * 100), 2).ToString() + "%");
                                row.Add(Math.Round((absolute * 100), 2).ToString() + "%");
                                row.Add(Math.Round((cloud * 100), 2).ToString() + "%");
                                valueItems.Add(row.ToArray());
                            }
                        }
                    }
                    if (valueItems != null&&valueItems.Count>0)
                    {
                        //float resolution = bPCDDataProvider.ResolutionX;
                        string title = "统计日期：" + DateTime.Now.ToShortDateString();
                        string[] columns = new string[] { "区域名称", "总覆盖度", "实际覆盖度", "相对覆盖度", "云覆盖度" };
                        IStatResult result = new StatResult(title, columns, valueItems.ToArray());
                        string filename = StatResultToFile(new string[] { bPCDFName }, result, "BAG", "BACD", "蓝藻覆盖度统计", extinfo, 1,false);
                        return new FileExtractResult("BACD", filename);
                    }
                }
                finally
                {
                    if (bPCDDataProvider != null)
                        bPCDDataProvider.Dispose();
                    if (cloudDataProvider != null)
                        cloudDataProvider.Dispose();
                }
            }
            return null;
        }

        /// <summary>
        /// 计算实际覆盖度
        /// 实际覆盖度=(求和（每像元覆盖度*像元面积）)/(指定区域面积,例如太湖)
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <param name="aoi"></param>
        /// <returns></returns>
        public float CalcActualConvertDegree(IRasterDataProvider dataProvider, int[] aoi)
        {
            double actualArea = 0;
            double totalArea = aoi.Count() * dataProvider.ResolutionX;
            Size size = new Size(dataProvider.Width, dataProvider.Height);
            Rectangle rect = AOIHelper.ComputeAOIRect(aoi, size);
            ArgumentProvider ap = new ArgumentProvider(dataProvider, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            //solution 使用横向？
            visitor.VisitPixel(rect,aoi,new int[] { 1 }, (index, values) =>
            {
                if (values[0] <= 1f && values[0] >= 0f)
                    actualArea += values[0] * dataProvider.ResolutionX;
            });
            return (float)(actualArea / totalArea);
        }

        /// <summary>
        /// 计算云覆盖度
        /// </summary>
        /// <param name="dataProvider">云监测生成的判识结果文件</param>
        /// <param name="pixelCount">指定区域像素点个数（例如：太湖区域像素点总数）</param>
        /// <returns></returns>
        public float CalcCloudConvertDegree(IRasterDataProvider dataProvider, int[] aoi)
        {
            if (dataProvider == null || aoi.Count() == 0)
                return 0;
            long nCloud = 0;
            IRasterOperator<UInt16> rasterOper = new RasterOperator<UInt16>();
            nCloud = rasterOper.Count(dataProvider, aoi, (value) =>
            {
                if (value == 1)
                    return true;
                else return false;
            });
            return (float)nCloud / (float)aoi.Count();
        }

        /// <summary>
        /// 相对覆盖度=(求和（每像元覆盖度*像元面积）)/(指定区域面积中有蓝藻的面积,例如：太湖中有蓝藻的像元面积)
        /// </summary>
        /// <param name="dataProvider">覆盖度文件DataProvider</param>
        /// <returns></returns>
        public float CalcAbsoluteConvertDegree(IRasterDataProvider dataProvider, int[] aoi)
        {
            double actualArea = 0;
            double totalArea = 0;
            Size size = new Size(dataProvider.Width, dataProvider.Height);
            Rectangle rect = AOIHelper.ComputeAOIRect(aoi, size);
            ArgumentProvider ap = new ArgumentProvider(dataProvider, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            //solution 使用横向？
            visitor.VisitPixel(rect,aoi,new int[] { 1 }, (index, values) =>
            {
                if (values[0] <= 1f && values[0] >= 0f)
                {
                    actualArea += values[0] * dataProvider.ResolutionX;
                    totalArea += dataProvider.ResolutionX;
                }
            });
            if (totalArea == 0)
                return 0;
            return (float)(actualArea / totalArea);
        }

        /// <summary>
        /// 计算总覆盖度
        /// 总覆盖度=(有蓝藻的像元面积)/(指定区域面积,例如太湖);
        /// </summary>
        /// <returns>覆盖度(0-1)</returns>
        public float StatTotalConvertDegree(IRasterDataProvider dataProvider, int[] aoi)
        {
            int convertedCount = 0;
            bool[] convertedPixels = new bool[aoi.Length];
            int i = 0;
            Size size = new Size(dataProvider.Width, dataProvider.Height);
            Rectangle rect = AOIHelper.ComputeAOIRect(aoi, size);
            ArgumentProvider ap = new ArgumentProvider(dataProvider, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            visitor.VisitPixel(rect,aoi,new int[] { 1 }, (index, values) =>
            {
                if (values[0] >= 0f && values[0] <= 1f)
                {
                    convertedPixels[i] = true;
                    i++;
                }
            });
            foreach (bool isConverted in convertedPixels)
                if (isConverted)
                    convertedCount++;
            return (float)convertedCount / (float)convertedPixels.Length;
        }
    }
}
