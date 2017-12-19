using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public class RasterQuickStatTool:IRasterQuickStatTool
    {
        public Dictionary<int, RasterQuickStatResult> Compute(IRasterDataProvider dataProvider,int[] aoi, int[] bandNos,Action<int,string> progressTracker)
        {
            if (dataProvider == null)
                throw new ArgumentNullException("dataProvider");
            if (bandNos == null || bandNos.Length == 0)
                throw new ArgumentNullException("bandNos");
            int minBandNo = bandNos.Min(), maxBandNo = bandNos.Max();
            if (minBandNo < 1 || maxBandNo > dataProvider.BandCount)
                throw new IndexOutOfRangeException("bandNo");
            //
            double[] minValues = new double[bandNos.Length];
            double[] maxValues = new double[bandNos.Length];
            double[] meanValues = new double[bandNos.Length];
            IMaxMinValueComputer computer = MaxMinValueComputerFactory.GetMaxMinValueComputer(dataProvider.DataType);
            IRasterBand[] srcRasters = new IRasterBand[bandNos.Length];
            for (int i = 0; i < bandNos.Length; i++)
                srcRasters[i] = dataProvider.GetRasterBand(bandNos[i]);
            //
            computer.Compute(srcRasters,aoi, out minValues, out maxValues, out meanValues, progressTracker);
            //
            Dictionary<int, RasterQuickStatResult> results = new Dictionary<int, RasterQuickStatResult>();
            List<IRasterBand> rasterBands = new List<IRasterBand>();
            int b = 0;
            foreach(int bandNo in bandNos)
            {
                IRasterBand rstBand = dataProvider.GetRasterBand(bandNo);
                rasterBands.Add(rstBand);
                RasterQuickStatResult result = BuildStatResult(rstBand,minValues[b],maxValues[b],meanValues[b]);
                if (result != null)
                    results.Add(bandNo, result);
                b++;
            }
            //
            IHistogramComputer histogramComputer = HistogramComputerFactory.GetHistogramComputer(dataProvider.DataType);
            //
            if(aoi == null || aoi.Length ==0)
                histogramComputer.Compute(rasterBands.ToArray(), results.Values.ToArray(), progressTracker);
            else
                histogramComputer.Compute(rasterBands.ToArray(),aoi, results.Values.ToArray(), progressTracker);
            //
            return results;
        }

        private RasterQuickStatResult BuildStatResult(IRasterBand rasterBand,double minValue,double maxValue,double meanValue)
        {
            int acutalBuckets;
            double bin = GetBin(rasterBand.DataType, maxValue, minValue, HistogramResult.MAX_BUCKETS,out acutalBuckets);
            RasterQuickStatResult result = new RasterQuickStatResult();
            result.HistogramResult.ActualBuckets = acutalBuckets + 1;
            result.HistogramResult.MinDN = minValue;
            result.HistogramResult.MaxDN = maxValue;
            result.HistogramResult.Bin = bin;
            result.MinValue = minValue;
            result.MaxValue = maxValue;
            result.MeanValue = meanValue;
            return result;
        }

        private double GetBin(enumDataType dataType,double maxValue, double minValue, int maxBucket,out int actualBucket)
        {
            double bin = 0;
            switch (dataType)
            {
                case enumDataType.Byte:
                case enumDataType.Int16:
                case enumDataType.UInt16:
                case enumDataType.Int32:
                case enumDataType.UInt32:
                case enumDataType.Int64:
                case enumDataType.UInt64:
                    bin = (int)Math.Ceiling((maxValue - minValue) / maxBucket);
                    //采样间隔值bin为0时,zyb，20131119新加
                    if (bin != 0.0d)
                    {
                        actualBucket = (int)Math.Ceiling((maxValue - minValue) / bin);
                    }
                    else
                    {
                        actualBucket = 0;
                    }
                    return bin;
                default:
                    bin = (maxValue - minValue) / (float)maxBucket;
                    actualBucket = (int)((maxValue - minValue) / bin);
                    return bin;
            }
        }
    }
}
