using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    public abstract class ControlPointInterpolator : IControlPointInterpolator
    {
        private Point[] _controlPoints = null;

        public ControlPointInterpolator()
        {
            _controlPoints = new Point[] { new Point(0, 0), new Point(255, 255) };
        }

        public ControlPointInterpolator(Point[] controlPoints)
        {
            _controlPoints = controlPoints;
            BuildFunction();
        }

        public Point[] ControlPoints
        {
            get { return _controlPoints; }
        }

        public void UpdateControlPoints(Point[] controlPoints)
        {
            _controlPoints = controlPoints;
            BuildFunction();
        }

        protected abstract void BuildFunction();

        public abstract byte Interpolate(byte x);
    }
}
