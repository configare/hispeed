using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public struct DensityRange
    {
        public float minValue;

        public float maxValue;

        public byte RGB_r;

        public byte RGB_g;

        public byte RGB_b;

        public DensityRange(float value_l, float value_r, byte r, byte g, byte b)
        {
            minValue = value_l;
            maxValue = value_r;
            RGB_r = r;
            RGB_g = g;
            RGB_b = b;
        }
        public string ToString()
        {
            return string.Format("{0} to {1} [{2},{3},{4}]", minValue, maxValue, RGB_r, RGB_g, RGB_b);
        }
    }

    public class DensityDef
    {
        private float _maxValue = float.MaxValue;
        private float _minValue = float.MinValue;
        private int _rangeCount = 0;
        private float _interval = 0;
        private bool _applayInterval = false;
        private DensityRange[] _ranges = null;

        public DensityDef()
        {
        }

        public float MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public float MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public int RangeCount
        {
            get { return _rangeCount; }
            set { _rangeCount = value; }
        }

        public float Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public bool ApplayInterval
        {
            get { return _applayInterval; }
            set { _applayInterval = value; }
        }

        public DensityRange[] Ranges
        {
            get { return _ranges; }
            set { _ranges = value; }
        }
    }
}
