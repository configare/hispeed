using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class ProjectionTransform:IProjectionTransform,IDisposable
    {
        protected  Proj4Projection _srcProjection = null;
        protected Proj4Projection _dstProjection = null;

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

        public void Transform(double[] xs, double[] ys)
        {
            Proj4Projection.Transform(_srcProjection, _dstProjection, xs, ys);
        }

        public void InverTransform(double[] xs, double[] ys)
        {
            Proj4Projection.Transform(_dstProjection, _srcProjection, xs, ys);
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
