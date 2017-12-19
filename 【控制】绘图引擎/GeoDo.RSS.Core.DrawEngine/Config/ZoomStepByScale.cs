using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class ZoomStepByScale
    {
        private double _minscale;
        private double _maxscale;
        private int _steps;

        public ZoomStepByScale()
        {

        }

        public double Minscale
        {
            get { return _minscale; }
            set { _minscale = value; }
        }

        public double Maxscale
        {
            get { return _maxscale; }
            set { _maxscale = value; }
        }

        public int Steps
        {
            get { return _steps; }
            set { _steps = value; }
        }
    }
}
