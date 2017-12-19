using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class LinearRgbStretcherUInt16 : RgbStretcher<UInt16>
    {
        protected int _minData = 0;
        protected int _maxData = 1000;
        private float _k = 0;

        public LinearRgbStretcherUInt16(UInt16 minData, UInt16 maxData)
            : this(minData, maxData, 0, 255)
        {

        }

        public LinearRgbStretcherUInt16(UInt16 minData, UInt16 maxData,bool isUseMap)
            : this(minData, maxData, 0, 255,isUseMap)
        {

        }

        public LinearRgbStretcherUInt16(UInt16 minData, UInt16 maxData, byte minRgb, byte maxRgb)
        {
            _minData = minData;
            _maxData = maxData;
            _minRgb = minRgb;
            _maxRgb = maxRgb;
            _k = (_maxRgb - _minRgb) / (float)(_maxData - _minData);
            SetStretcherUseAlgorithm();
        }

        public LinearRgbStretcherUInt16(UInt16 minData, UInt16 maxData, byte minRgb, byte maxRgb, bool isUseMap)
            :this(minData,maxData,minRgb,maxRgb)
        {
            IsUseMap = isUseMap;
        }

        public LinearRgbStretcherUInt16(UInt16 minData, UInt16 maxData, byte minRgb, byte maxRgb, bool isUseMap,int[] defaultBands)
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

        protected override void SetStretcherUseMap()
        {
            _stretcher = (x) =>
            {
                return _map[x];
            };
        }

        protected override void BuildMap()
        {
            _map = new byte[UInt16.MaxValue + 1];
            int max = UInt16.MaxValue;
            for (UInt16 i = 0; i < max; i++)
                _map[i] = _stretcher(i);
        }
    }
}
