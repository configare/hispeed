using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public class CoordPoint
    {
        private double _x = 0;
        private double _y = 0;

        public CoordPoint()
        { }

        public CoordPoint(double x, double y)
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

        public PointF ToPointF()
        {
            return new PointF((float)_x, (float)_y);
        }

        public bool IsEmpty()
        {
            return Math.Abs(_x) < double.Epsilon || Math.Abs(_y) < double.Epsilon;
        }

        public override string ToString()
        {
            return string.Concat("{", string.Format("X:{0},Y:{1}", _x, _y), "}");
        }
    }
}
