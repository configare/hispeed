using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    ///   Xgeo = GT(0) + Xpixel*GT(1) + Yline*GT(2)
    ///   Ygeo = GT(3) + Xpixel*GT(4) + Yline*GT(5)
    /// </summary>
    public class RasterCoordTransform:IRasterCoordTransform
    {
        protected double[] _affinePoints = null;

        public RasterCoordTransform(double[] affinePoints)
        {
            _affinePoints = affinePoints;
        }

        public double[] GTs { get { return _affinePoints; } }

        public void GetGeoTransform(double[] affinePoints)
        {
            if (_affinePoints == null || _affinePoints.Length == 0  || _affinePoints.Length != affinePoints.Length)
                return;
            for (int i = 0; i < _affinePoints.Length; i++)
                affinePoints[i] = _affinePoints[i];
        }

        public void Raster2DataCoord(int row, int col, out double coordX, out double coordY)
        {
            coordX = _affinePoints[0] + col * _affinePoints[1] + row * _affinePoints[2];
            coordY = _affinePoints[3] + col * _affinePoints[4] + row * _affinePoints[5];
        }

        public void Raster2DataCoord(int row, int col, double[] coord)
        {
            coord[0] = _affinePoints[0] + col * _affinePoints[1] + row * _affinePoints[2];
            coord[1] = _affinePoints[3] + col * _affinePoints[4] + row * _affinePoints[5];
        }

        public void Dispose()
        {
            _affinePoints = null;
        }
    }
}
