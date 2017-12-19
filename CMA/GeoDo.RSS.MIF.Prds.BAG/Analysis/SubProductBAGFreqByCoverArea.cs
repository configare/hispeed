using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductBAGFreqByCoverArea : CmaMonitoringSubProduct
    {
        //string[] _covertAreaRegions = null;
        //IRasterDataProvider[] _statRasters;
        //double _pixelArea = double.MinValue;

        public SubProductBAGFreqByCoverArea(SubProductDef productDef)
            : base(productDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            string[] covertAreaRegions = _argumentProvider.GetArg("CovertAreaRegion") as string[];
            string[] files = _argumentProvider.GetArg("SelectedPrimaryFiles") as string[];
            string extinfo = GetStringArgument("extinfo");
            if (covertAreaRegions == null || covertAreaRegions.Length == 0 || files == null)
                return null;
            IRasterDataProvider[] statRasters = null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "BCAF")
            {
                int count = covertAreaRegions.Count();
                List<string[]> results = new List<string[]>();
                try
                {
                    statRasters = new IRasterDataProvider[files.Length];
                    for (int i = 0; i < statRasters.Length; i++)
                    {
                        statRasters[i] = GeoDataDriver.Open(files[i]) as IRasterDataProvider;
                    }
                    double pixelArea = BAGStatisticHelper.CalPixelArea(statRasters[0].ResolutionX);
                    for (int i = 0; i < count; i++)
                    {
                        float[] minmax = BAGStatisticHelper.GetCovertAreaValue(covertAreaRegions[i]);
                        List<string[]> result = FREQAlgorithm(minmax[0], minmax[1],statRasters,pixelArea);
                        if (result != null && result.Count >= 1)
                            results.AddRange(result);
                    }
                    FileExtractResult fileResult = null;
                    if (results != null && results.Count() > 0)
                    {
                        string title = "统计日期：" + DateTime.Now.ToShortDateString();
                        RasterIdentify id = new RasterIdentify(files);
                        if (id.OrbitDateTime != null)
                            title += "    轨道日期：" + id.OrbitDateTime.ToShortDateString();
                        string[] columns = new string[] { "统计范围", "发生频数（次）" };
                        IStatResult result = new StatResult(title, columns, results.ToArray());
                        string filename = StatResultToFile(files, result, "BAG", "BCAF", "蓝藻按覆盖面积统计频次", extinfo, 1, false);
                        fileResult = new FileExtractResult("BCAF", filename);
                    }
                    return fileResult;
                }
                finally
                {
                    if (statRasters != null)
                        foreach (IRasterDataProvider prd in statRasters)
                            prd.Dispose();
                }
            }
            return null;
        }

        private List<string[]> FREQAlgorithm(float minValue, float maxValue, IRasterDataProvider[] statRasters,double pixelArea)
        {
            //计算总覆盖面积
            int totalAreaCount = StatFrequenceByTotalConvertArea(minValue, maxValue, 0, 1, statRasters,pixelArea);
            //计算实际覆盖面积
            int actualCount = StatFrequenceByActualConvertArea(minValue, maxValue, 0, 1, statRasters,pixelArea);
            List<string[]> results = new List<string[]>();
            string[] statLine = new string[] { "总覆盖面积在" + minValue + "平方公里—" + maxValue + "平方公里", totalAreaCount.ToString() };
            results.Add(statLine);
            statLine = new string[] { "实际覆盖面积在" + minValue + "平方公里—" + maxValue + "平方公里", actualCount.ToString() };
            results.Add(statLine);
            return results;
        }

        /// <summary>
        /// 计算指定面积范围内的频次(按总覆盖面积)
        /// </summary>
        /// <param name="minArea">最小总覆盖面积</param>
        /// <param name="maxArea">最大总覆盖面积</param>
        /// <param name="minConvertDegree">最小覆盖度</param>
        /// <param name="maxConvertDegree">最大覆盖度</param>
        /// <returns></returns>
        public int StatFrequenceByTotalConvertArea(double minArea, double maxArea, float minConvertDegree, float maxConvertDegree, IRasterDataProvider[] statRasters,double pixelArea)
        {
            int nCount = 0;
            foreach (IRasterDataProvider ds in statRasters)
                if (StatFrequenceByTotalConvertArea(ds, minArea, maxArea, minConvertDegree, maxConvertDegree,pixelArea))
                    nCount++;
            return nCount;
        }

        /// <summary>
        /// 计算指定面积范围内的频次(按总覆盖面积)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="minArea"></param>
        /// <param name="maxArea"></param>
        /// <returns></returns>
        private bool StatFrequenceByTotalConvertArea(IRasterDataProvider ds, double minArea, double maxArea, float minConvertDegree, float maxConvertDegree,double pixelArea)
        {
            if (ds == null)
                throw new Exception("数据集" + ds.fileName + "为空，不能计算指定总面积范围内的频次！");
            double area = CalcTotalConvertArea(ds, minConvertDegree, maxConvertDegree,pixelArea);
            return area > minArea && area <= maxArea;
        }

        /// <summary>
        ///  计算指定面积范围内的频次(按实际覆盖面积)
        /// </summary>
        /// <param name="minArea">最小实际覆盖面积</param>
        /// <param name="maxArea">最大实际覆盖面积</param>
        /// <param name="minConvertDegree">最小覆盖度[0~1]</param>
        /// <param name="maxConvertDegree">最大覆盖度[0~1]</param>
        /// <returns></returns>
        public int StatFrequenceByActualConvertArea(double minArea, double maxArea, float minConvertDegree, float maxConvertDegree, IRasterDataProvider[] statRasters,double pixelArea)
        {
            int nCount = 0;
            foreach (IRasterDataProvider ds in statRasters)
                if (StatFrequenceByActualConvertArea(ds, minArea, maxArea, minConvertDegree, maxConvertDegree,pixelArea))
                    nCount++;
            return nCount;
        }

        /// <summary>
        /// 计算指定面积范围内的频次(按实际覆盖面积)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="minArea"></param>
        /// <param name="maxArea"></param>
        /// <param name="minConvertDegree"></param>
        /// <param name="maxConvertDegree"></param>
        /// <returns></returns>
        private bool StatFrequenceByActualConvertArea(IRasterDataProvider ds, double minArea, double maxArea, float minConvertDegree, float maxConvertDegree,double pixelArea)
        {
            if (ds == null)
                throw new Exception("NDVI数据集为空，不能计算指定实际面积范围内的频次！");
            double area = CalcActualConvertArea(ds, minConvertDegree, maxConvertDegree,pixelArea);
            return area > minArea && area <= maxArea;
        }

        private string[] GetFileNameArray(string files)
        {
            if (string.IsNullOrEmpty(files))
                return null;
            else
            {
                string[] tempSplit = files.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tempSplit == null || tempSplit.Length == 0)
                    return null;
                List<string> result = new List<string>();
                for (int i = 0; i < tempSplit.Length; i++)
                {
                    if (File.Exists(tempSplit[i]))
                        result.Add(tempSplit[i]);
                }
                return result.Count == 0 ? null : result.ToArray();
            }
        }

        /// <summary>
        /// 计算指定等级的总覆盖面积
        /// 例如:重度(60%-100%)
        /// 比较条件为：minConvertDegree< x && x<=  maxConvertDegree
        /// </summary>
        /// <param name="minConvertDegree"></param>
        /// <param name="maxConvertDegree"></param>
        /// <returns></returns>
        public double CalcTotalConvertArea(IRasterDataProvider dataProvider, float minCovertDegree, float maxCovertDegree, double pixelArea)
        {
            double convertArea = 0;
            IRasterOperator<float> rasterOper = new RasterOperator<float>();
            int count = rasterOper.Count(dataProvider, null, (value) =>
            {
                if (minCovertDegree < value && value <= maxCovertDegree)
                    return true;
                else return false;
            });
            convertArea = count * pixelArea;
            return convertArea;
        }

        /// <summary>
        /// 计算指定等级的实际覆盖面积
        /// 比较条件为：minConvertDegree< x && x<=  maxConvertDegree
        /// </summary>
        /// <param name="dataProvider">像元覆盖度文件</param>
        /// <param name="minConvertDegree"></param>
        /// <param name="maxConvertDegree"></param>
        /// <returns></returns>
        public double CalcActualConvertArea(IRasterDataProvider dataProvider, float minConvertDegree, float maxConvertDegree,double pixelArea)
        {
            if (minConvertDegree > maxConvertDegree)
                return 0;
            double convertArea = 0;
            ArgumentProvider ap = new ArgumentProvider(dataProvider, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            visitor.VisitPixel(new int[] { 1 },
                (index, values) =>
                {
                    if (values[0] <= maxConvertDegree && values[0] > minConvertDegree)
                    {
                        convertArea += (pixelArea * values[0]);
                    }
                });
            return convertArea;
        }
    }
}
