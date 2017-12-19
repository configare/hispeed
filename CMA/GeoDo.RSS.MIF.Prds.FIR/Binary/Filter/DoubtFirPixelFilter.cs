using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal class DoubtFirPixelFilter : IBackTmpComputerHelper
    {
        private float _bandZoom = 1f;
        //
        private int _farIfrBandNo;
        private int _midIfrBandNo;
        private int _nearIfrBandNo;
        private int _visBandNo;
        private int _farIfrBandIndex;
        private int _midIfrBandIdx;
        private int _nearIfrBandIdx;
        private int _visBandIdx;
        private ISolarZenithProvider _solarZenithProvider;
        //太阳天顶角阈值
        private float _minSolarZenithValue;
        /*
         * 水体判识
         */
        private int _minMidIfrValue_water;//中红外阈值(默认:270)
        private int _minNearIfrValue_water;//近红外阈值(默认:50)
        private int _nearIfr_visValue_water;//(近红外-可见光阈值)(默认:0)
        /*
         * 云判识
         */
        //ref _minSolarZenithValue;
        private int _minVisValue_cloud;//可见光阈值(默认:200)
        private int _farIfrValue_cloud;//远红外阈值(默认:4)
        /*
         * 疑似火点判识
         */
        private int _midInfraredValue_DoubtFir;//中红外阈值(默认:320)
        private int _dltMidInfraredValue_DoubtFir;//中红外偏移(默认:0)
        private int _midInfrared_farInfrared_DoubtFir;//(中红外-远红外)差值阈值(默认:15)
        private int _minMidInfraredValue_DoubtFir;//中红外最小值（默认：315）
        private int _maxHTmpPixelCount_DoubtFir;//邻域内符合条件的像元个数(默认：6)
        private int _midInfraredAvg_DoubtFir;//中红外均值偏移阈值(默认:8)
        /*
         * 邻域直径
         */
        private int _minWndSize;//默认：7
        private int _maxWndSize;//默认：19
        /*
         * 邻域内满足条件的像元数量最大值
         */
        private int _maxHitedPixelCount;

        int _width = 0;
        public DoubtFirPixelFilter(
            int minWndSize, int maxWndSize,
            int farInfraredBandNo, int midInfraredBandNo, int nearInfraredBandNo,
            int visibleBandNo,
            float bandZoom,
            ISolarZenithProvider solarZenithProvider,
            int maxHitedPixelCount,
            /*以下4个阈值用于水体判识*/
            float minSolarZenithValue, int minMidInfraredValue_water, int minNearInfraredValue_water, int nearInfrared_visibleValue_water,
            /*以下2个参数用于云判识*/
            int minVisibleValue_cloud, int farInfraredValue_cloud,
            /*以下3个参数用于疑似火点判识*/
            int midInfraredValue_DoubtFir,
            int dltMidInfraredValue_DoubtFir,
            int midInfrared_farInfrared_DoubtFir,
            int minMidInfraredValue_DoubtFir,
            int maxHTmpPixelCount_DoubtFir,
            int midInfraredAvg_DoubtFir
            )
        {
            _minWndSize = minWndSize;
            _maxWndSize = maxWndSize;
            _farIfrBandNo = farInfraredBandNo;
            _midIfrBandNo = midInfraredBandNo;
            _nearIfrBandNo = nearInfraredBandNo;
            _visBandNo = visibleBandNo;
            _bandZoom = bandZoom;
            _solarZenithProvider = solarZenithProvider;
            _maxHitedPixelCount = maxHitedPixelCount;
            _minSolarZenithValue = minSolarZenithValue;
            _minMidIfrValue_water = minMidInfraredValue_water;
            _minNearIfrValue_water = minNearInfraredValue_water;
            _nearIfr_visValue_water = nearInfrared_visibleValue_water;
            _minVisValue_cloud = minVisibleValue_cloud;
            _farIfrValue_cloud = farInfraredValue_cloud;
            _midInfraredValue_DoubtFir = midInfraredValue_DoubtFir;
            _dltMidInfraredValue_DoubtFir = dltMidInfraredValue_DoubtFir;
            _midInfrared_farInfrared_DoubtFir = midInfrared_farInfrared_DoubtFir;
            _minMidInfraredValue_DoubtFir = minMidInfraredValue_DoubtFir;
            _maxHTmpPixelCount_DoubtFir = maxHTmpPixelCount_DoubtFir;
            _midInfraredAvg_DoubtFir = midInfraredAvg_DoubtFir;
        }

        public int[] Filter(IArgumentProvider argProvider, Rectangle aoiRect, int[] aoi)
        {
            List<int> retAOI = new List<int>(aoi.Length);
            using (IRasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(argProvider))
            {
                _width = argProvider.DataProvider.Width;
                //邻域内符合条件的像元个数
                int preHitedPixelCount = 0;
                int preWndSize = 0;
                int[] bandNos = new int[] { _visBandNo, _nearIfrBandNo, _midIfrBandNo, _farIfrBandNo };
                _visBandIdx = 0;
                _nearIfrBandIdx = 1;
                _midIfrBandIdx = 2;
                _farIfrBandIndex = 3;
                //
                bool isDoubtFir = false;
                //
                int[] hitedIndexes = new int[100 * 100]; //最大窗口100
                visitor.VisitPixelWnd(bandNos, bandNos, _minWndSize, _maxWndSize,
                     (pixelIdx, crtWndSize, bandValues, wndValues) =>
                     {
                         if (pixelIdx == 219998)
                             pixelIdx = 219998;
                         isDoubtFir = false;
                         //直接判定疑似火点
                         if (bandValues[_midIfrBandIdx] / _bandZoom > (_midInfraredValue_DoubtFir - _dltMidInfraredValue_DoubtFir) &&
                             ((bandValues[_midIfrBandIdx] - bandValues[_farIfrBandIndex]) / _bandZoom) > _midInfrared_farInfrared_DoubtFir)
                         {
                             isDoubtFir = true;
                             retAOI.Add(pixelIdx);
                             return false;
                         }
                         //
                         if (preWndSize != crtWndSize)
                         {
                             preHitedPixelCount = 0;
                             preWndSize = crtWndSize;
                         }
                         preHitedPixelCount = GetHitedPixelCount(argProvider, pixelIdx, bandValues, wndValues, hitedIndexes);
                         return preHitedPixelCount < _maxHitedPixelCount;
                     },
                    (pixelIdx, crtWndSize, bandValues, wndValues) =>
                    {
                        if (isDoubtFir)
                            return;
                        if (preHitedPixelCount < _maxHTmpPixelCount_DoubtFir)
                        {
                            return;
                        }
                        else
                        {
                            float avgMidIfr = Avg(_midIfrBandIdx, wndValues, preHitedPixelCount, hitedIndexes);
                            float avgMidFarIfr = DiffAvg(_midIfrBandIdx, _farIfrBandIndex, wndValues, preHitedPixelCount, hitedIndexes);
                            bool isOK = preHitedPixelCount >= _maxHTmpPixelCount_DoubtFir &&
                                    (bandValues[_midIfrBandIdx] / _bandZoom) > (avgMidIfr + _midInfraredAvg_DoubtFir)
                                    && ((bandValues[_midIfrBandIdx] - bandValues[_farIfrBandIndex]) / _bandZoom) >
                                    (avgMidFarIfr + _midInfraredAvg_DoubtFir)
                                    ;
                            if (isOK)
                                retAOI.Add(pixelIdx);
                        }
                    });
            }
            return retAOI.Count > 0 ? retAOI.ToArray() : null;
        }

        private float DiffAvg(int bandIdx1, int bandIdx2, UInt16[][] wndValues, int hitedPixelCount, int[] hitedIndexes)
        {
            float sum = 0;
            for (int i = 0; i < hitedPixelCount; i++)
            {
                sum += (wndValues[bandIdx1][hitedIndexes[i]] / _bandZoom - wndValues[bandIdx2][hitedIndexes[i]] / _bandZoom);
            }
            return sum / (float)hitedPixelCount;
        }

        private float Avg(int bandIdx, UInt16[][] wndValues, int hitedPixelCount, int[] hitedIndexes)
        {
            float sum = 0;
            for (int i = 0; i < hitedPixelCount; i++)
            {
                sum += (wndValues[bandIdx][hitedIndexes[i]] / _bandZoom);
            }
            return sum / (float)hitedPixelCount;
        }

        private float Avg(int bandIdx1, int bandIdx2, UInt16[][] wndValues)
        {
            float[] temp = new float[wndValues[bandIdx1].Length];
            for (int i = 0; i < wndValues[bandIdx1].Length; i++)
                temp[i] = wndValues[bandIdx1][i] - wndValues[bandIdx2][i];
            return (float)temp.Average<float>((v) => { return (float)v / _bandZoom; });
        }

        private int GetHitedPixelCount(IArgumentProvider argProvider, int pixelIdx, UInt16[] bandValues, UInt16[][] wndValues, int[] hitedIndexes)
        {
            int wndPixelCount = wndValues[0].Length;
            bool isWaterPixel = false;
            bool isCloudPixel = false;
            int hitedPixelCount = 0;
            int halfSize = (int)Math.Sqrt(wndPixelCount);
            int idx = 0;
            int oidx = 0;
            int r = 0;
            int c = 0;
            int rStart = pixelIdx / _width - halfSize / 2;
            int cStart = pixelIdx % _width - halfSize / 2;
            for (int i = 0; i < wndPixelCount; i++)
            {
                r = rStart + i / halfSize;
                c = cStart + i % halfSize;
                oidx = r * _width + c;
                isWaterPixel = VertifyIsWaterPixel(oidx, i, wndValues);
                //if (oidx >= 0)
                //    isCloudPixel = VertifyIsCloudPixel(argProvider, oidx, i, wndValues);
                isCloudPixel = VertifyIsCloudPixel(oidx, i, wndValues);
                if (!isWaterPixel && !isCloudPixel && (wndValues[_midIfrBandIdx][i] / _bandZoom < _minMidInfraredValue_DoubtFir))
                {
                    hitedPixelCount++;
                    hitedIndexes[idx++] = i;
                }
            }
            return hitedPixelCount;
        }

        public void SetBandNo(int visBandIdx, int nearIfrBandIdx, int midIfrBandIdx, int farIfrBandIndex)
        {
            _visBandIdx = visBandIdx;
            _nearIfrBandIdx = nearIfrBandIdx;
            _midIfrBandIdx = midIfrBandIdx;
            _farIfrBandIndex = farIfrBandIndex;
        }

        public void ResetBandNo()
        {
            _visBandIdx = 0;
            _nearIfrBandIdx = 1;
            _midIfrBandIdx = 2;
            _farIfrBandIndex = 3;
        }

        public bool VertifyIsCloudPixel(int pixelIdx, int iWndPixel, UInt16[][] wndValues)
        {
            return false;
            UInt16 minVisValue = 0;
            int minVisIndex = GetMinVisPixelIndex(wndValues[_visBandIdx], out minVisValue);
            UInt16 minVisFarIfrValue = wndValues[_farIfrBandIndex][minVisIndex];
            if (minVisValue >= _minVisValue_cloud)
                return false;
            if (_solarZenithProvider != null && _solarZenithProvider.GetSolarZenith(pixelIdx) > _minSolarZenithValue)
            {
                return wndValues[_farIfrBandIndex][iWndPixel] < minVisFarIfrValue - _farIfrValue_cloud;
            }
            else
            {
                return (wndValues[_visBandIdx][iWndPixel] / _bandZoom) > _minVisValue_cloud &&
                    (wndValues[_farIfrBandIndex][iWndPixel] / _bandZoom) < (minVisFarIfrValue - _farIfrValue_cloud);
            }
        }

        public bool VertifyIsWaterPixel(int pixelIdx, int iWndPixel, UInt16[][] wndValues)
        {
            if ((wndValues[_midIfrBandIdx][iWndPixel] / _bandZoom) < _minMidIfrValue_water)
                return true;
            if (_solarZenithProvider != null)
                if (_solarZenithProvider.GetSolarZenith(pixelIdx) > _minSolarZenithValue)
                    return false;
            return ((wndValues[_nearIfrBandIdx][iWndPixel] - wndValues[_visBandIdx][iWndPixel]) / _bandZoom) < _nearIfr_visValue_water &&
                (wndValues[_midIfrBandIdx][iWndPixel] / _bandZoom) < _minMidIfrValue_water;
        }

        private int GetMinVisPixelIndex(UInt16[] visBandValues, out UInt16 minValue)
        {
            int minVisPixelIndex = 0;
            minValue = 0;
            for (int i = 0; i < visBandValues.Length; i++)
            {
                if (visBandValues[i] < minValue)
                {
                    minValue = visBandValues[i];
                    minVisPixelIndex = i;
                }
            }
            return minVisPixelIndex;
        }
    }
}
