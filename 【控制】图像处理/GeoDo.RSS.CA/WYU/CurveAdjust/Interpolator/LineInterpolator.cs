using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    public class LineInterpolator : ControlPointInterpolator
    {
        public LineInterpolator()
            : base()
        {
        }

        public LineInterpolator(Point[] controlPoints)
            : base(controlPoints)
        {
        }

        protected override void BuildFunction()
        {
            int len = ControlPoints.Length;
            for (int i = 0; i < len; i++)
                Interpolate((byte)ControlPoints[i].X);
        }

        public override byte Interpolate(byte x)
        {
            if (ControlPoints == null || ControlPoints.Length == 0)
                return x;

            byte fx = 0;
            int n = ControlPoints.Length;
            int k0 = 0;
            int ki = n - 1;

            while (ki - k0 > 1)
            {
                int k = (k0 + ki) >> 1;

                if (x > ControlPoints[k].X)
                {
                    k0 = k;
                }
                else
                {
                    ki = k;
                }
            }

            double hi = ControlPoints[ki].X - ControlPoints[k0].X;
            fx = (byte)((ControlPoints[ki].Y - ControlPoints[k0].Y) * (x - ControlPoints[k0].X) / hi + ControlPoints[k0].Y);
            return fx;
        }
    }
}
