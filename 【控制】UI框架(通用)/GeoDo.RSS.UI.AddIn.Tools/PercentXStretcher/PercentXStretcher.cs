using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public class PercentXStretcher
    {
        protected ISmartSession _smartSession;

        public PercentXStretcher(ISmartSession smartSession)
        {
            _smartSession = smartSession;
        }

        public object[] GetStretcher(IRasterDataProvider dataProvider, int[] bandNos, float percent)
        {
            int[] bNos = bandNos.Distinct().ToArray();
            Dictionary<int, RasterQuickStatResult> results;
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                progress.Reset("正在执行直方图统计...", 100);
                progress.Start(false);
                IRasterQuickStatTool stat = new RasterQuickStatTool();
                results = stat.Compute(dataProvider, null, bNos,
                    (idx, tip) =>
                    {
                        progress.Boost(idx, "正在执行直方图统计...");
                    });
            }
            finally
            {
                progress.Finish();
            }
            RasterQuickStatResult[] rts = new RasterQuickStatResult[bandNos.Length];
            for (int i = 0; i < bandNos.Length; i++)
            {
                rts[i] = results[bandNos[i]];
            }
            //
            object[] stretchers = GetStretcher(dataProvider.DataType, rts, percent);
            return stretchers;
        }

        private object[] GetStretcher(enumDataType dataType, RasterQuickStatResult[] resutls,float percent)
        {
            int bandCount= resutls.Length;
            //x%像元临界个数
            int criticalCount = (int)(percent * resutls[0].HistogramResult.PixelCount);
            object[] sts = new object[bandCount];
            //low
            for (int i = 0; i < bandCount; i++)
            {
                //边界DN值
                double lowValue = 0, heightValue = 0;
                //累计个数
                double accCount = 0;
                int bucks = resutls[i].HistogramResult.ActualBuckets;
                //lowValue
                for (int k = 0; k < bucks; k++)
                {
                    if (accCount > criticalCount)
                        break;
                    accCount += resutls[i].HistogramResult.Items[k];
                    lowValue = resutls[i].HistogramResult.MinDN + k * resutls[i].HistogramResult.Bin;
                }
                //HighValue
                accCount = 0;
                for (int k = bucks-1; k >=0; k--)
                {
                    if (accCount > criticalCount)
                        break;
                    accCount += resutls[i].HistogramResult.Items[k];
                    heightValue = resutls[i].HistogramResult.MinDN + k * resutls[i].HistogramResult.Bin;
                }
                //
                sts[i] = RgbStretcherFactory.CreateStretcher(dataType, lowValue, heightValue);
            }
            return sts;
        }
    }
}
