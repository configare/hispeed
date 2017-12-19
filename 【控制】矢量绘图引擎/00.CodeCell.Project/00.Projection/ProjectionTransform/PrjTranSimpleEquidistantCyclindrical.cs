using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// GLL:Geographic Latitude/Longitude
    /// Equirectangular  
    /// Alias  Plate Caree  
    /// Alias  Equidistant Cylindrical  
    /// Alias  Simple Cylindrical  
    /// </summary>
    internal class PrjTranSimpleEquidistantCyclindrical:IProjectionTransform,IDisposable
    {
        private const double DEGREEStoRADIANS = Math.PI / 180;
        private const double RADIANStoDEGREES = 180 / Math.PI;
        private readonly double WGS84SEMIMAJOR = 6378137.0;
        private readonly double ONEOVERWGS84SEMIMAJOR = 0;
        private readonly float a = 0;
        private readonly float b = 0;
        private readonly float c = 0;
        private readonly float d =0;
        private ShapePoint[] _tempShapePts = new ShapePoint[1];
        private PointF[] _tempPts = new PointF[1];

        public PrjTranSimpleEquidistantCyclindrical()
        {
            ONEOVERWGS84SEMIMAJOR = 1.0 / WGS84SEMIMAJOR;
            a = (float)(DEGREEStoRADIANS * Math.Cos(0) * WGS84SEMIMAJOR);
            b = (float)(DEGREEStoRADIANS * WGS84SEMIMAJOR);
            //
            c = (float)(ONEOVERWGS84SEMIMAJOR / Math.Cos(0) * RADIANStoDEGREES);
            d = (float)(ONEOVERWGS84SEMIMAJOR * RADIANStoDEGREES);
        }

        public PrjTranSimpleEquidistantCyclindrical(double semimajorAxis)
        {
            WGS84SEMIMAJOR = semimajorAxis;
            ONEOVERWGS84SEMIMAJOR = 1.0 / WGS84SEMIMAJOR;
            a = (float)(DEGREEStoRADIANS * Math.Cos(0) * WGS84SEMIMAJOR);
            b = (float)(DEGREEStoRADIANS * WGS84SEMIMAJOR);
            //
            c = (float)(ONEOVERWGS84SEMIMAJOR / Math.Cos(0) * RADIANStoDEGREES);
            d = (float)(ONEOVERWGS84SEMIMAJOR * RADIANStoDEGREES);
        }

        #region IProjectionTransform Members

        public void Transform(ShapePoint[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X *= a;
                points[i].Y *= b;
            }
        }

        public void Transform(PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X *= a;
                points[i].Y *= b;
            }
        }

        public void Transform(ShapePoint pt)
        {
            _tempShapePts[0] = pt;
            Transform(_tempShapePts);
        }

        public void Transform(ref PointF pt)
        {
            _tempPts[0] = pt;
            Transform(_tempPts);
            pt.X = _tempPts[0].X;
            pt.Y = _tempPts[0].Y;
        }

        public void Transform(double[] xs, double[] ys)
        { 
            int size = xs.Length ;
            for (int i = 0; i < size; i++)
            {
                xs[i] *= a;
                ys[i] *= b;
            }
        }

        public void InverTransform(double[] xs, double[] ys)
        {
            int size = xs.Length;
            for (int i = 0; i < size; i++)
            {
                xs[i] *= c;
                ys[i] *= d;
            }
        }

        public void InverTransform(ShapePoint[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X *= c;
                points[i].Y *= d;
            }
        }

        public void InverTransform(PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X *= c;
                points[i].Y *= d;
            }
        }

        public void InverTransform(ShapePoint pt)
        {
            _tempShapePts[0] = pt;
            InverTransform(_tempShapePts);
        }

        public void InverTransform(ref PointF pt)
        {
            _tempPts[0] = pt;
            InverTransform(_tempPts);
            pt.X = _tempPts[0].X;
            pt.Y = _tempPts[0].Y;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
