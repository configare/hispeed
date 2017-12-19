using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class LinearRgbStretcherDouble : RgbStretcher<Double>
    {
        protected Double _minData = 0;
        protected Double _maxData = 1000;
        private Double _k = 0;

        public LinearRgbStretcherDouble(Double minData, Double maxData)
            : this(minData, maxData, 0, 255)
        {

        }

        public LinearRgbStretcherDouble(Double minData, Double maxData, bool isUseMap)
            : this(minData, maxData, 0, 255,isUseMap)
        {

        }

        public LinearRgbStretcherDouble(Double minData, Double maxData, byte minRgb, byte maxRgb)
        {
            _minData = minData;
            _maxData = maxData;
            _minRgb = minRgb;
            _maxRgb = maxRgb;
            _k = (_maxRgb - _minRgb) / (Double)(_maxData - _minData);
            SetStretcherUseAlgorithm();
        }

        public LinearRgbStretcherDouble(Double minData, Double maxData, byte minRgb, byte maxRgb, bool isUseMap)
            :this(minData,maxData,minRgb,maxRgb)
        {
            IsUseMap = false;
        }

        public LinearRgbStretcherDouble(Double minData, Double maxData, byte minRgb, byte maxRgb, bool isUseMap,int[] defaultBands)
            : this(minData, maxData, minRgb, maxRgb,isUseMap)
        {
            DefaultBands = defaultBands;
        }

        protected override void SetStretcherUseAlgorithm()
        {
            _stretcher = (x) =>
            {
                if (x > _maxData)
                    return _maxRgb;
                else if (x < _minData)
                    return _minRgb;
                else
                    return (byte)((x - _minData) * _k);
            };
        }
    }
}
