using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    //直方图统计结果
    public class HistogramResult
    {
        //直方图最多采样256个灰度值
        public const int MAX_BUCKETS = 255;
        //全图像元个数
        public long PixelCount;
        //采样间隔值
        public double Bin;
        //实际采样的灰度值个数
        public int ActualBuckets;
        //直方图统计项
        public long[] Items = new long[MAX_BUCKETS+1];
        //最小DN值
        public double MinDN;
        //最大DN值
        public double MaxDN;
    }

    public class RasterQuickStatResult
    {
        public double MinValue;
        public double MaxValue;
        public double MeanValue;
        public double Stddev;
        public HistogramResult HistogramResult = new HistogramResult();
    }

    public interface IRasterQuickStatTool : IRasterTool
    {
        Dictionary<int, RasterQuickStatResult> Compute(IRasterDataProvider dataProvider,int[] aoi, int[] bandNos,Action<int,string> progressTracker);
    }
}
