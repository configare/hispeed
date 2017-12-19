using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class NDVISettingItem
    {
        string _name;
        CoordEnvelope _envelope;
        float _minValue;
        float _maxValue;
        bool _isUse;

        public NDVISettingItem()
        {
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public CoordEnvelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }

        public float MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public bool IsUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }

        public float MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }
    }
}
