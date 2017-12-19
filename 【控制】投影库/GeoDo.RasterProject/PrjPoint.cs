using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RasterProject
{
    public class PrjPoint
    {
        private double _x;
        private double _y;

        public static PrjPoint Empty
        {
            get { return new PrjPoint(0, 0); }
        }

        public PrjPoint(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        } 
    }
}
