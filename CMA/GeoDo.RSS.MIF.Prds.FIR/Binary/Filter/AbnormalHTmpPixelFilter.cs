using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /* 邻域内像元满足
     * 中红外 < 候选像元中红外 + 中红外偏移阈值(默认：3）
     * 中红外 < 中红外阈值(默认：315）
     * (中红外 - 远红外 ) < 偏移(默认：8）
     */
    /// <summary>
    /// 邻域内异常高温点判识
    /// eg:异常高温点>6就认为是火点
    /// </summary>
    internal class AbnormalHTmpPixelFilter : CandidatePixelFilter
    {
        private int _minWndSize;
        private int _maxWndSize;
        private int _midInfraredBandNo;
        private int _farInfraredBandNo;
        private float _bandZoom;
        private int _dltMiddleInfrared;
        private int _maxMiddleInfrared;
        private int _dltMid_FarInfrared;
        private int _abnormalPixelCount;
        private IArgumentProvider _argProvider;

        public AbnormalHTmpPixelFilter(int minWndSize, int maxWndSize, int midInfraredBandNo, int farInfraredBandNo,
            float bandZoom, int dltMiddleInfrared, int maxMiddleInfrared, int dltMid_FarInfrared,
            int abnormalPixelCount,
            IArgumentProvider argProvider)
        {
            _minWndSize = minWndSize;
            _maxWndSize = maxWndSize;
            _midInfraredBandNo = midInfraredBandNo;
            _farInfraredBandNo = farInfraredBandNo;
            _bandZoom = bandZoom;
            _dltMiddleInfrared = dltMiddleInfrared;
            _maxMiddleInfrared = maxMiddleInfrared;
            _dltMid_FarInfrared = dltMid_FarInfrared;
            _abnormalPixelCount = abnormalPixelCount;
            _argProvider = argProvider;
        }

        protected override int[] DoFilter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi)
        {
            return DoFilter(dataProvider, aoiRect, aoi, null);
        }

        private int GetCount(UInt16[] bandValues, UInt16[][] wndValues)
        {
            int count = 0;
            float midValue;
            float farValue;
            for (int i = 0; i < wndValues[0].Length; i++)
            {
                midValue = wndValues[0][i] / _bandZoom;
                farValue = wndValues[1][i] / _bandZoom;
                if (midValue < (midValue + _dltMiddleInfrared) &&
                    (midValue < _maxMiddleInfrared) &&
                    (midValue - farValue) < _dltMid_FarInfrared)
                {
                    count++;
                }
            }
            return count;
        }

        protected override int[] DoFilter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi, byte[] assistInfo)
        {
            List<int> retAOI = new List<int>(aoi.Length);
            using (IRasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(_argProvider))
            {
                int[] bandNos = new int[] { _midInfraredBandNo, _farInfraredBandNo };
                visitor.VisitPixelWnd(aoiRect, aoi, bandNos, bandNos, _minWndSize, _maxWndSize,
                    (pixelIdx, crtWndSize, bandValues, wndValues) =>
                    {
                        return GetCount(bandValues, wndValues) < _abnormalPixelCount;
                    },
                    (pixelIdx, crtWndSize, bandValues, wndValues) =>
                    {
                        int count = GetCount(bandValues, wndValues);
                        //
                        if (count >= _abnormalPixelCount)
                        {
                            retAOI.Add(pixelIdx);
                            if (assistInfo != null)
                                assistInfo[pixelIdx] = 1;
                        }
                    });
            }
            return retAOI.Count > 0 ? retAOI.ToArray() : null;
        }
    }
}
