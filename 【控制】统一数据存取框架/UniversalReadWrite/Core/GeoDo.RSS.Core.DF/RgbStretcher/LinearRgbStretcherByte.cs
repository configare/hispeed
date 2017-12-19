using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class LinearRgbStretcherByte : RgbStretcher<byte>
    {
        protected byte _minData = 0;
        protected byte _maxData = 255;
        private float _k = 0;

        public LinearRgbStretcherByte(byte minData, byte maxData)
            : this(minData, maxData, 0, 255)
        {

        }

        public LinearRgbStretcherByte(byte minData, byte maxData, bool isUseMap)
            : this(minData, maxData, 0, 255, isUseMap)
        {

        }

        public LinearRgbStretcherByte(byte minData, byte maxData, byte minRgb, byte maxRgb)
        {
            _minData = minData;
            _maxData = maxData;
            _minRgb = minRgb;
            _maxRgb = maxRgb;
            _k = (_maxRgb - _minRgb) / (float)(_maxData - _minData);
            SetStretcherUseAlgorithm();
        }

        public LinearRgbStretcherByte(byte minData, byte maxData, byte minRgb, byte maxRgb, bool isUseMap)
            : this(minData, maxData, minRgb, maxRgb)
        {
            IsUseMap = true;
        }

        public LinearRgbStretcherByte(byte minData, byte maxData, byte minRgb, byte maxRgb, bool isUseMap, int[] defaultBands)
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
                return _map[x];
            };
        }

        protected override void BuildMap()
        {
            _map = new byte[256];
            int max = byte.MaxValue;
            for (int i = byte.MinValue; i <= max; i++)
                _map[i] = _stretcher((byte)i);
        }
    }
}
