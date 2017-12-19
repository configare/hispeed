using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 利用背景温度确认火点
    /// </summary>
    internal class VertifyFirPixelFiter
    {
        private float _bandZoom = 1f;
        private int _farInfraredBandNo;
        private int _midInfraredBandNo;
        private int _visibleBandNo;
        private int _farInfraredBandIndex;
        private int _midInfraredBandIndex;
        private int _visibleBandIndex;
        //
        private int _maxFarIfrValue_fir;//远红外最大值阈值(默认:273)
        private int _minVisValue_fir;//可见光最小值阈值(默认:250)
        private int _secondMidIfrValue_fir;//亚像元火点中红外阈值(默认:340)
        private int _backTmpFactor_fir;//背景亮温系数阈值(默认:5)
        private float _midFarRatio;
        //
        private bool _isUsedMidFar = true;//是否启用中红外偏移量与远红外偏移量比值火点确认
        private float _defaultPercent;//默认云红外<中红外时，中红外偏移量与远红外偏移量比值（默认:0.2）

        public VertifyFirPixelFiter(int farInfraredBandNo, int midInfraredBandNo, int visibleBandNo,
            float bandZoom,
            int maxFarFarIfrValue_fir,
            int minVisValue_fir,
            int secondMidIfrValue_fir,
            int backTmpFactor_fir,
            float midFarRatio,
            bool isUsedMidFar,
            float defaultPercent
            )
        {
            _farInfraredBandNo = farInfraredBandNo;
            _midInfraredBandNo = midInfraredBandNo;
            _visibleBandNo = visibleBandNo;
            _bandZoom = bandZoom;
            _maxFarIfrValue_fir = maxFarFarIfrValue_fir;
            _minVisValue_fir = minVisValue_fir;
            _secondMidIfrValue_fir = secondMidIfrValue_fir;
            _backTmpFactor_fir = backTmpFactor_fir;
            _midFarRatio = midFarRatio;
            _isUsedMidFar = isUsedMidFar;
            _defaultPercent = defaultPercent;
        }

        public int[] Filter(IArgumentProvider argProvider, Dictionary<int, PixelFeature> features)
        {
            using (IRasterPixelsVisitor<UInt16> vistor = new RasterPixelsVisitor<UInt16>(argProvider))
            {
                int[] bandNos = new int[] { _visibleBandNo, _midInfraredBandNo, _farInfraredBandNo };
                _visibleBandIndex = 0;
                _midInfraredBandIndex = 1;
                _farInfraredBandIndex = 2;
                //
                int[] aoi = GetAOI(features, (fet) => { return fet.IsDoubtFirPixel; });
                if (aoi == null)
                    return null;
                Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, new System.Drawing.Size(argProvider.DataProvider.Width, argProvider.DataProvider.Height));
                vistor.VisitPixel(aoiRect, aoi, bandNos, (idx, values) =>
                {
                    float pvFarIfr = values[_farInfraredBandIndex] / _bandZoom;
                    float pvMidIfr = values[_midInfraredBandIndex] / _bandZoom;
                    float pvVis = values[_visibleBandIndex] / _bandZoom;
                    float pvMidIfr_FarIfr = values[_midInfraredBandIndex] - values[_farInfraredBandIndex];
                    //by chennan 20120821 一期火点确认错误代码
                    if (pvFarIfr < _maxFarIfrValue_fir || pvVis > _minVisValue_fir)
                        return;
                    PixelFeature fet = features[idx];
                    if (pvMidIfr > _secondMidIfrValue_fir)
                    {
                        //if (pvMidIfr > fet.MidIfrAvgValue + fet.MidIfr_StdDev)
                        if (pvMidIfr > fet.MidIfrAvgValue + _backTmpFactor_fir * fet.MidIfr_StdDev)
                            fet.IsVertified = true;
                    }
                    else
                    {
                        if (
                            (pvMidIfr > fet.MidIfrAvgValue + _backTmpFactor_fir * fet.MidIfr_StdDev)
                            &&
                            (pvMidIfr_FarIfr > fet.MidIfr_FarIfr_Diff_AvgValue + _backTmpFactor_fir * fet.MidIfr_FarIfr_Diff_StdDev)
                          )
                        {
                            fet.IsVertified = true;
                        }
                    }
                    //by chennan 20120821 增加中红外偏移量与远红外偏移量比值火点确认条件
                    if (fet.IsVertified == true && _isUsedMidFar)
                    {
                        float bzCH4 = (pvFarIfr - fet.FarIfrAvgValue);
                        if (bzCH4 <= 0)
                            bzCH4 = _defaultPercent;
                        //bzCH4 = 0.2f;
                        if ((pvMidIfr - fet.MidIfrAvgValue) / bzCH4 < _midFarRatio)
                        {
                            fet.IsVertified = false;
                            return;
                        }
                    }
                });
            }
            return GetAOI(features, (fet) => { return fet.IsVertified; });
        }

        private int[] GetAOI(Dictionary<int, PixelFeature> features, Func<PixelFeature, bool> where)
        {
            List<int> retAOI = new List<int>(features.Count);
            foreach (int idx in features.Keys)
                if (where(features[idx]))
                    retAOI.Add(idx);
            return retAOI.Count > 0 ? retAOI.ToArray() : null;
        }
    }
}
