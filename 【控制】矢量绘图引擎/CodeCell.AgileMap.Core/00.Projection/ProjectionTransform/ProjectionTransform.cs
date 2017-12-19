using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    internal class ProjectionTransform:IProjectionTransform,IDisposable
    {
        private Proj4Projection _srcProjection = null;
        private Proj4Projection _dstProjection = null;

        public ProjectionTransform(ISpatialReference srcSpatialRef,ISpatialReference dstSpatialRef)
        {
            if (srcSpatialRef == null)
                throw new ArgumentNullException("源空间参考对象为空。");
            if (dstSpatialRef == null)
                throw new ArgumentNullException("目标空间参考对象为空。");
            _srcProjection = new Proj4Projection(srcSpatialRef.ToProj4String());
            _dstProjection = new Proj4Projection(dstSpatialRef.ToProj4String());
            _srcProjection._coordinateDomain = srcSpatialRef.CoordinateDomain;
            _dstProjection._coordinateDomain = dstSpatialRef.CoordinateDomain;
        }

        #region IProjectionTransform Members

        //Longitude latitude ->meters
        public void Transform(ShapePoint[] points)
        {
            DoTransform(_srcProjection, _dstProjection, points);
        }

        public void Transform(PointF[] points)
        {
            DoTransform(_srcProjection, _dstProjection, points);
        }

        public void Transform(ShapePoint pt)
        {
            ShapePoint[] tempPts = new ShapePoint[1];
            tempPts[0] = pt;
            Transform(tempPts);
        }

        public void Transform(ref PointF pt)
        {
            PointF[] tempPts = new PointF[1];
            tempPts[0] = pt;
            Transform(tempPts);
            pt.X = tempPts[0].X;
            pt.Y = tempPts[0].Y;
        }

        public void InverTransform(ShapePoint[] points)
        {
            DoTransform(_dstProjection,_srcProjection, points);
        }

        public void InverTransform(PointF[] points)
        {
            DoTransform(_dstProjection, _srcProjection, points);
        }

        public void InverTransform(ShapePoint pt)
        {
            ShapePoint[] tempPts = new ShapePoint[1];
            tempPts[0] = pt;
            InverTransform(tempPts);
        }

        public void InverTransform(ref PointF pt)
        {
            PointF[] tempPts = new PointF[1];
            tempPts[0] = pt;
            InverTransform(tempPts);
            pt.X = tempPts[0].X;
            pt.Y = tempPts[0].Y;
        }

        private void DoTransform(Proj4Projection srcPrj,Proj4Projection desPrj, ShapePoint[] points)
        {
            double[] xs = new double[points.Length];
            double[] ys = new double[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                xs[i] = points[i].X;
                ys[i] = points[i].Y;
            }
            Proj4Projection.Transform(srcPrj, desPrj, xs, ys);
            //
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = xs[i];
                points[i].Y = ys[i];
            }
        }

        private void DoTransform(Proj4Projection srcPrj, Proj4Projection desPrj, PointF[] points)
        {
            double[] xs = new double[points.Length];
            double[] ys = new double[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                xs[i] = points[i].X;
                ys[i] = points[i].Y;
            }
            Proj4Projection.Transform(srcPrj, desPrj, xs, ys);
            //
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = (float)xs[i];
                points[i].Y = (float)ys[i];
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_srcProjection != null)
            {
                _srcProjection.Dispose();
                _srcProjection = null;
            }
            if (_dstProjection != null)
            {
                _dstProjection.Dispose();
                _dstProjection = null;
            }
        }

        #endregion
    }
}
