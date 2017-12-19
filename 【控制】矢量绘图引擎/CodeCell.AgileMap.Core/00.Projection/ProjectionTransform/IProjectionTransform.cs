using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface IProjectionTransform:IDisposable
    {
        void Transform(ShapePoint[] points);
        void Transform(PointF[] points);
        void Transform(ref PointF pt);
        void Transform(ShapePoint pt);
        void InverTransform(ShapePoint[] points);
        void InverTransform(PointF[] points);
        void InverTransform(ref PointF pt);
        void InverTransform(ShapePoint pt);
    }
}
