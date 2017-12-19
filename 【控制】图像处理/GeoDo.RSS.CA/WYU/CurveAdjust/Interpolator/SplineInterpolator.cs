using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    public class SplineInterpolator : ControlPointInterpolator
    {
        private double[] _y2 = new double[256];// 存储二阶导数，用于三次样条曲线插值计算

        public SplineInterpolator()
            : base()
        {
        }

        public SplineInterpolator(Point[] controlPoints)
            : base(controlPoints)
        {
        }

        protected override void BuildFunction()
        {
        }

        public override byte Interpolate(byte x)
        {
            byte fx = 0;
            int n = ControlPoints.Length;

            if (_y2 == null)
            {
                ComputeSecondDerivative(n);
            }

            int k0 = 0;
            int ki = n - 1;

            while (ki - k0 > 1)
            {
                int k = (k0 + ki) >> 1;

                if (ControlPoints[k].X > x)
                    ki = k;
                else k0 = k;
            }

            double hi = ControlPoints[ki].X - ControlPoints[k0].X;
            if (hi != 0)
            {
                double a = (ControlPoints[ki].X - x) / hi;
                double b = (x - ControlPoints[k0].X) / hi;

                fx = (byte)(a * ControlPoints[k0].Y + b * ControlPoints[ki].Y +
                           ((a * a * a - a) * _y2[k0] + (b * b * b - b) * _y2[ki]) * (hi * hi) / 6.0);
            }
            return fx;
        }

        private void ComputeSecondDerivative(int n)
        {
            double[] u = new double[n];

            for (int i = 1; i < n - 1; ++i)
            {
                double wx = ControlPoints[i + 1].X - ControlPoints[i - 1].X;
                double sig = (ControlPoints[i].X - ControlPoints[i - 1].X) / wx;
                double p = sig * _y2[i - 1] + 2.0;

                this._y2[i] = (sig - 1.0) / p;

                double ddydx = (ControlPoints[i + 1].Y - ControlPoints[i].Y) / (ControlPoints[i + 1].X - ControlPoints[i].X) -
                               (ControlPoints[i].Y - ControlPoints[i - 1].Y) / (ControlPoints[i].X - ControlPoints[i - 1].X);
                u[i] = (6.0 * ddydx / wx - sig * u[i - 1]) / p;
            }

            this._y2[n - 1] = 0;

            for (int i = n - 2; i >= 0; --i)
            {
                this._y2[i] = this._y2[i] * this._y2[i + 1] + u[i];
            }
        }


    }
}
