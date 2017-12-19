using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public struct NDVISetValue
    {
        private double _minNDVI;
        private double _maxNDVI;

        public NDVISetValue(double minNDVI, double maxNDVI)
        {
            _minNDVI = minNDVI;
            _maxNDVI = maxNDVI;
        }

        public double MinNDVI
        {
            get { return _minNDVI; }
        }

        public double MaxNDVI
        {
            get { return _maxNDVI; }
        }
    }
}
