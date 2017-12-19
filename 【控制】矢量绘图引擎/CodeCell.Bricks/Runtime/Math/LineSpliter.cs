using System;
using System.Drawing;

namespace CodeCell.Bricks.Runtime
{
    public class LineSpliter
    {
        private double _bX = 0;
        private double _bY = 0;
        private double _eX = 0;
        private double _eY = 0;

        private double _Sin = 0; //Sin(角度)
        private double _Cos = 0; //Cos(角度)

        private double _LineLength = 0;

        public LineSpliter()
        {
        }

        public LineSpliter(double bX, double bY, double eX, double eY)
        {
            Init(bX, bY, eX, eY);
        }

        public void Init(double bX, double bY, double eX, double eY)
        {
            this._bX = bX;
            this._bY = bY;
            this._eX = eX;
            this._eY = eY;
            _LineLength = this.GetLineLength(bX, bY, eX, eY);
            this.GetBevel();
        }

        public PointF GetMiddlePoint()
        {
            PointF[] pts = DoSplit(_LineLength / 2d, 0);
            if (pts == null || pts.Length == 0)
                return PointF.Empty;
            return pts[0];
        }

        public PointF[] DoSplit(double LenPerPart, double offset)
        {
            if (LenPerPart <= 0)
                return null;
            double effectLineLen = this._LineLength - offset;
            int N = Convert.ToInt32(effectLineLen / LenPerPart);  //N 等份
            double _PrePartsLen = offset + LenPerPart;
            PointF pt = PointF.Empty;
            PointF[] pts = new PointF[N];

            for (int i = 0; i < N; i++)
            {
                pt = this.GetPointByLen(_PrePartsLen);
                pts[i] = pt;
                _PrePartsLen = this.GetLineLength(this._bX, this._bY, pt.X, pt.Y) + LenPerPart;
            }

            return pts;
        }

        public PointF[] DoSplit(int nPart, double offset)
        {
            double LenPerPart = this._LineLength / nPart;
            return DoSplit(LenPerPart, offset);
        }

        private PointF GetPointByLen(double Len)
        {
            double y = 0;  //Sin(角度)=Math.abs(Y-bY)/Len
            double x = 0;  //Cos(角度)=Math.abs(X-bX)/Len
            y = this._bY + Len * this._Sin;
            x = this._bX + Len * this._Cos;

            PointF pt = new PointF((float)x, (float)y);
            return pt;
        }

        private void GetBevel()
        {
            if (this._LineLength == 0)
            {
                _Sin = 0;
                _Cos = 0;
            }
            else
            {
                _Sin = (this._eY - this._bY) / this._LineLength;
                _Cos = (this._eX - this._bX) / this._LineLength;
            }
        }

        private double GetLineLength(double bx, double by, double ex, double ey)
        {
            return Math.Sqrt(Math.Pow(ex - bx, 2) + Math.Pow(ey - by, 2));
        }
    }
}
