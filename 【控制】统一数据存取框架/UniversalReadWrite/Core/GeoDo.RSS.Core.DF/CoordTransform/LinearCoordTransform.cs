using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public class LinearCoordTransform : ICoordTransform
    {
        private double _kx, _ky;
        private double _bx, _by;

        public LinearCoordTransform(Point pt1, Point pt2, double[] coordPt1, double[] coordPt2)
        {
            _kx = GetK(pt1.X, pt2.X, coordPt1[0], coordPt2[0]);
            _ky = GetK(pt1.Y, pt2.Y, coordPt1[1], coordPt2[1]);
            _bx = GetB(_kx, pt1.X, coordPt1[0]);
            _by = GetB(_ky, pt1.Y, coordPt1[1]);
        }

        public double[] GTs
        {
            get { return null; }
        }

        private double GetB(double k, int x, double y)
        {
            return y - k * x;
        }

        private double GetK(int x1, int x2, double y1, double y2)
        {
            return (y2 - y1) / (x2 - x1);
        }

        public void Raster2DataCoord(int row, int col, out double coordX, out double coordY)
        {
            coordX = _kx * col + _bx;
            coordY = _ky * row + _by;
        }

        public void Raster2DataCoord(int row, int col, double[] coord)
        {
            coord[0] = _kx * col + _bx;
            coord[1] = _ky * row + _by;
        }

        public void Dispose()
        {
        }
    }
}
