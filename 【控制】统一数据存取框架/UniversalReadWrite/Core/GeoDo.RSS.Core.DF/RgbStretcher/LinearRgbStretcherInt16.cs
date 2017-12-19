using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class LinearRgbStretcherInt16 : RgbStretcher<Int16>
    {
        protected Int16 _minData = 0;
        protected Int16 _maxData = 1000;
        private float _k = 0;

        public LinearRgbStretcherInt16(Int16 minData, Int16 maxData)
            : this(minData, maxData, 0, 255)
        {

        }

        public LinearRgbStretcherInt16(Int16 minData, Int16 maxData, bool isUseMap)
            : this(minData, maxData, 0, 255, isUseMap)
        {

        }

        public LinearRgbStretcherInt16(Int16 minData, Int16 maxData, byte minRgb, byte maxRgb)
        {
            _minData = minData;
            _maxData = maxData;
            _minRgb = minRgb;
            _maxRgb = maxRgb;
            _k = (_maxRgb - _minRgb) / (float)(_maxData - _minData);
            SetStretcherUseAlgorithm();
        }

        public LinearRgbStretcherInt16(Int16 minData, Int16 maxData, byte minRgb, byte maxRgb, bool isUseMap)
            : this(minData, maxData, minRgb, maxRgb)
        {
            IsUseMap = true;
        }

        public LinearRgbStretcherInt16(Int16 minData, Int16 maxData, byte minRgb, byte maxRgb, bool isUseMap, int[] defaultBands)
            : this(minData, maxData, minRgb, maxRgb, isUseMap)
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

        protected override void SetStretcherUseMap()
        {
            _stretcher = (x) =>
            {
                return _map[x + Int16.MaxValue + 1];
            };
        }

        protected override void BuildMap()
        {
            _map = new byte[UInt16.MaxValue + 1];
            int max = Int16.MaxValue;
            for (Int16 i = Int16.MinValue; i < max; i++)
                _map[i + max + 1] = _stretcher(i);
        }
    }
}
