using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 背景温度计算
    /// </summary>
    internal class BackTmpComputer
    {
        private float _bandZoom = 1f;
        //
        private ISolarZenithProvider _solarZenithProvider;
        /*
         * 邻域直径
         */
        private int _minWndSize;//默认：7
        private int _maxWndSize;//默认：19
        //波段
        private int _farIfrBandNo;
        private int _nearIfrBandNo;
        private int _midIfrBandNo;
        private int _visBandNo;
        private int _farIfrBandIdx;
        private int _midIfrBandIdx;
        private int _visBandIdx;
        private int _nearIfrBandIdx;
        /*
                 * 计算亚像元面积
                 */
        //中红外中心波数
        private float _midIfrCenterWaveNum;
        private float _farIfrCenterWaveNum;
        private float _maxMidIfrValue;//最大中红外值阈值(默认:340)
        private float _firComputeFactor;//火点计算因子(默认:750)
        //火点强度计算
        private float _firIntensityFactor;//强度估算因子(默认:750)
        //火点可信度
        private int _wndSize_FirReliability;
        private float _midIfr_FirReliability;//中外红阈值(默认：10)
        private float _midIfr_farInfr_FirReliability;//（中红外-远红外）阈值（默认:10）
        private float _vis_FirReliability;//可见光阈值（默认：150）
        private float _cloudPixels_FirReliability;//云像元点数(默认：0)
        //太阳天顶角阈值
        private float _minSolarZenithValue;
        private float _maxSolarZenithValue;
        /*
         * 区域背景温度阈值
         */
        private int _localBackTmpMax;//区域背景温度最大值阈值(默认:30)
        private int _localBackTmpMin;//区域背景温度最小值阈值(默认:20)
        private int _correctedLocalBackTmpMin;
        private int _correctedLocalBackTmpMax;
        /*
       * 荒漠修正阈值
       */
        private int _glaringVIRR;
        /*
         * 荒漠修正阈值
         */
        private int _wildernessCorrect;
        //像元特征
        private Dictionary<int, PixelFeature> _features;
        /*
         * 邻域内满足条件的像元数量最大值(晴空像元个数)
         */
        private int _maxHitedPixelCount;
        //
        private IBackTmpComputerHelper _helper;
        //火点强度等级标准
        private float _fireGradeLevel1;
        private float _fireGradeLevel2;
        private float _fireGradeLevel3;
        private float _fireGradeLevel4;
        private float _fireGradeLevel5;
        //
        private List<int> _featrueIndex = null;
        private bool _isNight = false;

        public BackTmpComputer(int minWndSize, int maxWndSize,
            int maxHitedPixelCount,
            ISolarZenithProvider solarZenithProvider,
            float minSolarZenithValue, float maxSolarZenithValue,
            int farInfraredBandNo, int midInfraredBandNo, int visibleBandNo, int nearIfrBandNo,
            float bandZoom,
            float midIfrCenterWaveNum,
            float farIfrCenterWaveNum,
            int maxMidIfrValue,
            float firComputeFactor,
            float firIntensityfactor,
            /*以下2个参数用于修正区域背景温度最小值域最小值*/
            int localBackTmpMax, int localBackTmpMin,
            //耀斑修正值
            int glaringVIRR,
            int wildernessCorrect/*荒漠修正阈值*/,
            IBackTmpComputerHelper helper,
            float fireGradeLevel1,
            float fireGradeLevel2,
            float fireGradeLevel3,
            float fireGradeLevel4,
            float fireGradeLevel5,
            float vis_FirReliability,
            float midIfr_farInfr_FirReliability,
            float midIfr_FirReliability
            )
        {
            _minWndSize = minWndSize;
            _maxWndSize = maxWndSize;
            _maxHitedPixelCount = maxHitedPixelCount;
            _solarZenithProvider = solarZenithProvider;
            _minSolarZenithValue = minSolarZenithValue;
            _maxSolarZenithValue = maxSolarZenithValue;
            _farIfrBandNo = farInfraredBandNo;
            _midIfrBandNo = midInfraredBandNo;
            _visBandNo = visibleBandNo;
            _nearIfrBandNo = nearIfrBandNo;
            _bandZoom = bandZoom;
            _midIfrCenterWaveNum = midIfrCenterWaveNum;
            _farIfrCenterWaveNum = farIfrCenterWaveNum;
            _maxMidIfrValue = maxMidIfrValue;
            _firComputeFactor = firComputeFactor;
            _firIntensityFactor = firIntensityfactor;
            _localBackTmpMax = localBackTmpMax;
            _localBackTmpMin = localBackTmpMin;
            _glaringVIRR = glaringVIRR;
            _wildernessCorrect = wildernessCorrect;
            _helper = helper;
            _fireGradeLevel1 = fireGradeLevel1;
            _fireGradeLevel2 = fireGradeLevel2;
            _fireGradeLevel3 = fireGradeLevel3;
            _fireGradeLevel4 = fireGradeLevel4;
            _fireGradeLevel5 = fireGradeLevel5;
            _vis_FirReliability = vis_FirReliability;
            _midIfr_farInfr_FirReliability = midIfr_farInfr_FirReliability;
            _midIfr_FirReliability = midIfr_FirReliability;
        }

        int _width = 0;
        double _maxLat = 0;
        double _res = 0;
        int _cloudCount = 0;
        public Dictionary<int, PixelFeature> Compute(IArgumentProvider argProvider, Rectangle aoiRect, int[] aoi)
        {
            _width = argProvider.DataProvider.Width;
            _maxLat = argProvider.DataProvider.CoordEnvelope.MaxY;
            _res = argProvider.DataProvider.ResolutionY;
            _isNight = bool.Parse(argProvider.GetArg("IsNight").ToString());
            CreateAndInitFeatures(aoi);
            int tempIndex = 0;
            using (IRasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(argProvider))
            {
                //邻域内符合条件的像元个数
                int preHitedPixelCount = 0;
                int preWndSize = 0;
                int[] bandNos = new int[] { _visBandNo, _nearIfrBandNo, _midIfrBandNo, _farIfrBandNo };
                _visBandIdx = 0;
                _nearIfrBandIdx = 1;
                _midIfrBandIdx = 2;
                _farIfrBandIdx = 3;
                //
                int[] hitedIndexes = new int[100 * 100]; //最大窗口100
                //
                //by chennan 20140423 处理当当前窗口被恢复成最小窗口时，将晴空像元个数、索引更改回最小窗口状态
                int preHitedPixelCountSave = 0;
                List<int> hitedIndexesSave = new List<int>();
                //
                visitor.VisitPixelWnd(aoiRect, aoi, bandNos, bandNos, _minWndSize, _maxWndSize,
                     (pixelIdx, crtWndSize, bandValues, wndValues) =>
                     {
                         if (preWndSize != crtWndSize)
                         {
                             preHitedPixelCount = 0;
                             preWndSize = crtWndSize;
                         }
                         _cloudCount = 0;
                         preHitedPixelCount = GetHitedPixelCount(pixelIdx, bandValues, wndValues, hitedIndexes);
                         //by chennan 20140423 处理当当前窗口被恢复成最小窗口时，将晴空像元个数、索引更改回最小窗口状态
                         //
                         if (crtWndSize == _minWndSize)
                         {
                             preHitedPixelCountSave = preHitedPixelCount;
                             hitedIndexesSave.AddRange(hitedIndexes);
                         }
                         //
                         return preHitedPixelCount < _maxHitedPixelCount && crtWndSize < _maxWndSize;
                     },
                    (pixelIdx, crtWndSize, bandValues, wndValues) =>
                    {
                        if (preHitedPixelCount >= _maxHitedPixelCount)
                        {
                            //by chennan 20140423 处理当当前窗口被恢复成最小窗口时，将晴空像元个数、索引更改回最小窗口状态
                            if (crtWndSize == _minWndSize)
                            {
                                preHitedPixelCount = preHitedPixelCountSave;
                                hitedIndexes = hitedIndexesSave.ToArray();
                            }
                            //
                            Compute(preHitedPixelCount, pixelIdx, bandValues, wndValues, hitedIndexes);
                        }
                        else
                        {
                            int row = pixelIdx / _width;
                            int col = pixelIdx % _width;
                            int length = crtWndSize / 2;
                            for (int i = row - length; i <= row + length; i++)
                            {
                                if (i < 0)
                                    continue;
                                for (int j = col - length; j <= col + length; j++)
                                {
                                    tempIndex = i * _width + j;
                                    if (_featrueIndex.Contains(tempIndex))
                                        _features[pixelIdx] = _features[tempIndex];
                                }
                            }
                        }
                        preHitedPixelCountSave = 0;
                        hitedIndexesSave.Clear();
                    });
            }
            return _features;
        }

        /// <summary>
        /// 夜间火背景计算
        /// </summary>
        /// <param name="argProvider"></param>
        /// <param name="aoiRect"></param>
        /// <param name="aoi"></param>
        /// <returns></returns>
        public Dictionary<int, PixelFeature> ComputeNight(IArgumentProvider argProvider, Rectangle aoiRect, int[] aoi)
        {
            _width = argProvider.DataProvider.Width;
            _maxLat = argProvider.DataProvider.CoordEnvelope.MaxY;
            _res = argProvider.DataProvider.ResolutionY;
            _isNight = bool.Parse(argProvider.GetArg("IsNight").ToString());
            CreateAndInitFeatures(aoi);
            int tempIndex = 0;
            using (IRasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(argProvider))
            {
                //邻域内符合条件的像元个数
                int preHitedPixelCount = 0;
                int preWndSize = 0;
                int[] bandNos = new int[] { _midIfrBandNo, _farIfrBandNo };
                _midIfrBandIdx = 0;
                _farIfrBandIdx = 1;
                //
                int[] hitedIndexes = new int[100 * 100]; //最大窗口100
                //
                //by chennan 20140423 处理当当前窗口被恢复成最小窗口时，将晴空像元个数、索引更改回最小窗口状态
                int preHitedPixelCountSave = 0;
                List<int> hitedIndexesSave = new List<int>();
                //
                visitor.VisitPixelWnd(aoiRect, aoi, bandNos, bandNos, _minWndSize, _maxWndSize,
                     (pixelIdx, crtWndSize, bandValues, wndValues) =>
                     {
                         if (preWndSize != crtWndSize)
                         {
                             preHitedPixelCount = 0;
                             preWndSize = crtWndSize;
                         }
                         _cloudCount = 0;
                         preHitedPixelCount = GetNightHitedPixelCount(pixelIdx, bandValues, wndValues, hitedIndexes);
                         //by chennan 20140423 处理当当前窗口被恢复成最小窗口时，将晴空像元个数、索引更改回最小窗口状态
                         //
                         if (crtWndSize == _minWndSize)
                         {
                             preHitedPixelCountSave = preHitedPixelCount;
                             hitedIndexesSave.AddRange(hitedIndexes);
                         }
                         //
                         return preHitedPixelCount < _maxHitedPixelCount && crtWndSize < _maxWndSize;
                     },
                    (pixelIdx, crtWndSize, bandValues, wndValues) =>
                    {
                        if (preHitedPixelCount >= _maxHitedPixelCount)
                        {
                            //by chennan 20140423 处理当当前窗口被恢复成最小窗口时，将晴空像元个数、索引更改回最小窗口状态
                            //
                            if (crtWndSize == _minWndSize)
                            {
                                preHitedPixelCount = preHitedPixelCountSave;
                                hitedIndexes = hitedIndexesSave.ToArray();
                            }
                            //
                            Compute(preHitedPixelCount, pixelIdx, bandValues, wndValues, hitedIndexes);
                        }
                        else
                        {
                            int row = pixelIdx / _width;
                            int col = pixelIdx % _width;
                            int length = crtWndSize / 2;
                            for (int i = row - length; i <= row + length; i++)
                            {
                                if (i < 0)
                                    continue;
                                for (int j = col - length; j <= col + length; j++)
                                {
                                    tempIndex = i * _width + j;
                                    if (_featrueIndex.Contains(tempIndex))
                                        _features[pixelIdx] = _features[tempIndex];
                                }
                            }
                        }
                        preHitedPixelCountSave = 0;
                        hitedIndexesSave.Clear();
                    });
            }
            return _features;
        }

        private void Compute(int hitedPixelCount, int pixelIdx, UInt16[] bandValues, UInt16[][] wndValues, int[] hitedIndexes)
        {
            _featrueIndex.Add(pixelIdx);
            float pvVis = _isNight ? float.MinValue : bandValues[_visBandIdx];
            PixelFeature fet = _features[pixelIdx];
            fet.MidIfrAvgValue = Avg(_midIfrBandIdx, wndValues, hitedPixelCount, hitedIndexes);
            fet.FarIfrAvgValue = Avg(_farIfrBandIdx, wndValues, hitedPixelCount, hitedIndexes);
            fet.MidIfr_FarIfr_Diff_AvgValue = DiffAvg(_midIfrBandIdx, _farIfrBandIdx, wndValues, hitedPixelCount, hitedIndexes);
            fet.MidIfr_StdDev = GetStdDev(fet.MidIfrAvgValue, _midIfrBandIdx, wndValues, hitedPixelCount, hitedIndexes);
            fet.FarIfr_StdDev = GetStdDev(fet.FarIfrAvgValue, _midIfrBandIdx, wndValues, hitedPixelCount, hitedIndexes);
            fet.MidIfr_FarIfr_Diff_StdDev = GetStdDev(fet.MidIfr_FarIfr_Diff_AvgValue, _midIfrBandIdx, _farIfrBandIdx, wndValues, hitedPixelCount, hitedIndexes);
            CorrectLocalBackTmpMinMaxValue(pixelIdx, fet);
            CorrectStdDev(pixelIdx, fet);
            if (_solarZenithProvider != null)
                if (_solarZenithProvider.GetSolarZenith(pixelIdx) > _minSolarZenithValue &&
                    _solarZenithProvider.GetSolarZenith(pixelIdx) < _maxSolarZenithValue &&
                    (!_isNight && pvVis > _glaringVIRR))
                {
                    fet.MidIfr_StdDev = Math.Min(35, fet.MidIfr_StdDev);
                    fet.MidIfr_StdDev = Math.Max(40, fet.MidIfr_StdDev);
                    //
                    fet.MidIfr_FarIfr_Diff_StdDev = Math.Min(35, fet.MidIfr_FarIfr_Diff_StdDev);
                    fet.MidIfr_FarIfr_Diff_StdDev = Math.Max(40, fet.MidIfr_FarIfr_Diff_StdDev);
                }
            if (!_isNight && pvVis > Avg(_visBandIdx, wndValues, hitedPixelCount, hitedIndexes) + _wildernessCorrect)
            {
                fet.MidIfr_StdDev = Math.Min(40, fet.MidIfr_StdDev);
                fet.MidIfr_StdDev = Math.Max(45, fet.MidIfr_StdDev);
                //
                fet.MidIfr_FarIfr_Diff_StdDev = Math.Min(40, fet.MidIfr_FarIfr_Diff_StdDev);
                fet.MidIfr_FarIfr_Diff_StdDev = Math.Max(45, fet.MidIfr_FarIfr_Diff_StdDev);
            }
            //
            if (_midIfrCenterWaveNum > float.Epsilon && _farIfrCenterWaveNum > float.Epsilon)
            {
                float pixelArea = 0f;
                //计算亚像元面积
                fet.SecondPixelArea = (float)ComputeSecondPixelArea(pixelIdx, bandValues, out pixelArea);
                fet.PixelArea = pixelArea;
                int intensityGrade = 0;
                //火点强度
                fet.FirIntensity = (float)ComputeFirIntensity(pixelIdx, bandValues, out intensityGrade);
                fet.FireIntensityGrade = intensityGrade;
            }
            //火点可信度
            fet.FirReliability = ComputeFirReliability(pixelIdx, bandValues, fet);
        }

        //火点可信度
        private int ComputeFirReliability(int pixelIdx, UInt16[] bandValues, PixelFeature fet)
        {
            float pvMidIfr = bandValues[_midIfrBandIdx] / _bandZoom;
            float pvFarIfr = bandValues[_farIfrBandIdx] / _bandZoom;
            float pvVis = _isNight ? float.MinValue : bandValues[_visBandIdx] / _bandZoom;

            if (pvMidIfr - fet.MidIfrAvgValue > _midIfr_FirReliability &&
               pvMidIfr - pvFarIfr - fet.MidIfr_FarIfr_Diff_AvgValue > _midIfr_farInfr_FirReliability &&
               (_isNight || pvVis <= _vis_FirReliability))
            {
                return 200;
            }
            else if (_cloudCount > 0)
            {
                return 100;
            }
            else if (pvMidIfr - fet.MidIfrAvgValue < _midIfr_FirReliability &&
               pvMidIfr - pvFarIfr - fet.MidIfr_FarIfr_Diff_AvgValue < _midIfr_farInfr_FirReliability &&
               (_isNight || pvVis < _vis_FirReliability))
            {
                return 50;
            }
            else if (Math.Abs(_res - 0.01) < 0.00000001)
            {
                return 20;
            }
            return 0;
        }

        //计算火点强度
        private double ComputeFirIntensity(int pixelIdx, ushort[] bandValues, out int intensityGrade)
        {
            PixelFeature fet = _features[pixelIdx];
            double powerConst = 5.6693 * Math.Pow(10, -8);
            double firIntensity = fet.SecondPixelArea / 100f * powerConst * Math.Pow(_firIntensityFactor, 4);
            intensityGrade = Grade(firIntensity);
            return firIntensity;
        }

        private int Grade(double firIntensity)
        {
            if (firIntensity < _fireGradeLevel1)
                return 1;
            else if (firIntensity >= _fireGradeLevel1 && firIntensity < _fireGradeLevel2)
                return 2;
            else if (firIntensity >= _fireGradeLevel2 && firIntensity < _fireGradeLevel3)
                return 3;
            else if (firIntensity >= _fireGradeLevel3 && firIntensity < _fireGradeLevel4)
                return 4;
            else if (firIntensity >= _fireGradeLevel4 && firIntensity < _fireGradeLevel5)
                return 5;
            else if (firIntensity >= _fireGradeLevel5)
                return 6;
            return -1;
        }

        //计算亚像元面积
        private double ComputeSecondPixelArea(int pixelIdx, ushort[] bandValues, out float pixelArea)
        {
            int row = pixelIdx / _width;
            pixelArea = (float)RasterOperator<UInt16>.ComputePixelArea(row, _maxLat, _res);
            PixelFeature fet = _features[pixelIdx];
            float pvMidIfr = bandValues[_midIfrBandIdx] / _bandZoom;
            float pvFarIfr = bandValues[_farIfrBandIdx] / _bandZoom;
            double factor = 0;
            if (pvMidIfr < _maxMidIfrValue)
            {
                factor = 1.438833f * _midIfrCenterWaveNum / pvMidIfr * 10f;
                double n3Mix = (1.1910659 * Math.Pow(10, -5) * Math.Pow(_midIfrCenterWaveNum, 3)) / (Math.Pow(Math.E, factor) - 1f);
                factor = 1.438833f * _midIfrCenterWaveNum / _firComputeFactor;
                double n3bf = (1.1910659 * Math.Pow(10, -5) * Math.Pow(_midIfrCenterWaveNum, 3)) / (Math.Pow(Math.E, factor) - 1f);
                factor = 1.438833f * _midIfrCenterWaveNum / fet.MidIfrAvgValue * 10;
                double n3bt = (1.1910659 * Math.Pow(10, -5) * Math.Pow(_midIfrCenterWaveNum, 3)) / (Math.Pow(Math.E, factor) - 1f);
                return pixelArea * (n3Mix - n3bt) / (n3bf - n3bt) * 100f;
            }
            else
            {
                if (pvFarIfr < fet.FarIfrAvgValue)
                    factor = 1.438833f * _farIfrCenterWaveNum / (fet.FarIfrAvgValue + 3) * 10f;
                else
                    factor = 1.438833f * _farIfrCenterWaveNum / pvFarIfr * 10f;
                double n4Mix = (1.1910659 * Math.Pow(10, -5) * Math.Pow(_farIfrCenterWaveNum, 3)) / (Math.Pow(Math.E, factor) - 1f);
                factor = 1.438833f * _farIfrCenterWaveNum / _firComputeFactor;
                double n4bf = (1.1910659 * Math.Pow(10, -5) * Math.Pow(_farIfrCenterWaveNum, 3)) / (Math.Pow(Math.E, factor) - 1f);
                factor = 1.438833f * _farIfrCenterWaveNum / fet.FarIfrAvgValue * 10f;
                double n4bt = (1.1910659 * Math.Pow(10, -5) * Math.Pow(_farIfrCenterWaveNum, 3)) / (Math.Pow(Math.E, factor) - 1f);
                if (n4Mix - n4bt < 0)
                    n4Mix = n4bt + 2f;
                return pixelArea * (n4Mix - n4bt) / (n4bf - n4bt) * 100f;
            }
        }

        private float GetStdDev(float avg, int bandIdx1, int bandIdx2, UInt16[][] wndValues, int hitedPixelCount, int[] hitedIndexes)
        {
            float sum = 0;
            for (int i = 0; i < hitedPixelCount; i++)
            {
                sum += (float)Math.Pow((wndValues[bandIdx1][hitedIndexes[i]] - wndValues[bandIdx2][hitedIndexes[i]]) / _bandZoom - avg, 2);
            }
            sum /= hitedPixelCount;
            return (float)Math.Sqrt(sum);
        }

        private float GetStdDev(float avg, int bandIdx, UInt16[][] wndValues, int hitedPixelCount, int[] hitedIndexes)
        {
            float sum = 0;
            for (int i = 0; i < hitedPixelCount; i++)
            {
                sum += (float)Math.Pow(wndValues[bandIdx][hitedIndexes[i]] / _bandZoom - avg, 2);
            }
            sum /= hitedPixelCount;//方差
            return (float)Math.Sqrt(sum);
        }

        private void CorrectStdDev(int pixelIdx, PixelFeature fet)
        {
            fet.MidIfr_StdDev = Math.Min(_correctedLocalBackTmpMax, fet.MidIfr_StdDev);
            fet.MidIfr_StdDev = Math.Max(_correctedLocalBackTmpMin, fet.MidIfr_StdDev);
            //
            fet.FarIfr_StdDev = Math.Min(_correctedLocalBackTmpMax, fet.FarIfr_StdDev);
            fet.FarIfr_StdDev = Math.Max(_correctedLocalBackTmpMin, fet.FarIfr_StdDev);
            //
            fet.MidIfr_FarIfr_Diff_StdDev = Math.Min(_correctedLocalBackTmpMax, fet.MidIfr_FarIfr_Diff_StdDev);
            fet.MidIfr_FarIfr_Diff_StdDev = Math.Max(_correctedLocalBackTmpMin, fet.MidIfr_FarIfr_Diff_StdDev);
        }

        private void CorrectLocalBackTmpMinMaxValue(int pixelIdx, PixelFeature fet)
        {
            _correctedLocalBackTmpMax = _localBackTmpMax;
            _correctedLocalBackTmpMin = _localBackTmpMin;
            if (_solarZenithProvider == null)
                return;
            if (_solarZenithProvider.GetSolarZenith(pixelIdx) > _minSolarZenithValue)
            {
                _correctedLocalBackTmpMax = _localBackTmpMax - 5;
                _correctedLocalBackTmpMin = _localBackTmpMin - 5;
            }
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

        //计算晴空像元个数
        private int GetHitedPixelCount(int pixelIdx, UInt16[] bandValues, UInt16[][] wndValues, int[] hitedIndexes)
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
            _helper.SetBandNo(0, 1, 2, 3);
            for (int i = 0; i < wndPixelCount; i++)
            {
                r = rStart + i / halfSize;
                c = cStart + i % halfSize;
                oidx = r * _width + c;
                isWaterPixel = _helper.VertifyIsWaterPixel(oidx, i, wndValues);
                isCloudPixel = _helper.VertifyIsCloudPixel(oidx, i, wndValues);
                //非云、非水、非疑似火点
                if (!isWaterPixel && !isCloudPixel && !_features.ContainsKey(oidx))
                //if (!isWaterPixel && !isCloudPixel)
                {
                    hitedPixelCount++;
                    hitedIndexes[idx++] = i;
                }
                if (isCloudPixel)
                    _cloudCount++;
            }
            _helper.ResetBandNo();
            return hitedPixelCount;
        }

        //计算夜间晴空像元个数
        private int GetNightHitedPixelCount(int pixelIdx, UInt16[] bandValues, UInt16[][] wndValues, int[] hitedIndexes)
        {
            int wndPixelCount = wndValues[0].Length;
            bool isCloudPixel = false;
            int hitedPixelCount = 0;
            int halfSize = (int)Math.Sqrt(wndPixelCount);
            int idx = 0;
            int oidx = 0;
            int r = 0;
            int c = 0;
            int rStart = pixelIdx / _width - halfSize / 2;
            int cStart = pixelIdx % _width - halfSize / 2;
            _helper.SetBandNo(0, 1, 2, 3);
            for (int i = 0; i < wndPixelCount; i++)
            {
                r = rStart + i / halfSize;
                c = cStart + i % halfSize;
                oidx = r * _width + c;
                isCloudPixel = _helper.VertifyIsCloudPixel(oidx, i, wndValues);
                //非云、非疑似火点
                if (!isCloudPixel && !_features.ContainsKey(oidx))
                {
                    hitedPixelCount++;
                    hitedIndexes[idx++] = i;
                }
                if (isCloudPixel)
                    _cloudCount++;
            }
            _helper.ResetBandNo();
            return hitedPixelCount;
        }

        private void CreateAndInitFeatures(int[] aoi)
        {
            _features = new Dictionary<int, PixelFeature>(aoi.Length);
            for (int i = 0; i < aoi.Length; i++)
            {
                PixelFeature fet = new PixelFeature();
                fet.PixelIndex = aoi[i];
                fet.IsDoubtFirPixel = true;
                _features.Add(aoi[i], fet);
            }
            _featrueIndex = new List<int>();
        }
    }
}
