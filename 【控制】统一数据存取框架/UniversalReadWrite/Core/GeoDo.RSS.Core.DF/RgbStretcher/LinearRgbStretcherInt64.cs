using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class LinearRgbStretcherInt64 : RgbStretcher<Int64>
    {
        protected Int64 _minData = 0;
        protected Int64 _maxData = 1000;
        private float _k = 0;

        public LinearRgbStretcherInt64(Int64 minData, Int64 maxData)
            : this(minData, maxData, 0, 255)
        {

        }

        public LinearRgbStretcherInt64(Int64 minData, Int64 maxData,bool isUseMap)
            : this(minData, maxData, 0, 255,isUseMap)
        {

        }

        public LinearRgbStretcherInt64(Int64 minData, Int64 maxData, byte minRgb, byte maxRgb)
        {
            _minData = minData;
            _maxData = maxData;
            _minRgb = minRgb;
            _maxRgb = maxRgb;
            _k = (_maxRgb - _minRgb) / (float)(_maxData - _minData);
            SetStretcherUseAlgorithm();
        }

        public LinearRgbStretcherInt64(Int64 minData, Int64 maxData, byte minRgb, byte maxRgb,bool isUseMap)
            :this(minData,maxData,minRgb,maxRgb)
        {
            IsUseMap = false;
        }

        public LinearRgbStretcherInt64(Int64 minData, Int64 maxData, byte minRgb, byte maxRgb, bool isUseMap,int[] defaultBands)
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
