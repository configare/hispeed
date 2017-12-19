using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class GeometryMathLib
    {
        public static bool IsCrossed2Lines(ShapePolyline line1, ShapePolyline line2)
        {
            throw new NotSupportedException();
        }

        public static double GetDistance(ShapePolyline plyline, ShapePoint hitPoint)
        {
            throw new NotSupportedException();
        }

        public static bool IsPointInPolygon(ShapePoint hitPoint, ShapePolygon polygon)
        {
            int n = 0;
            foreach (ShapeRing ring in polygon.Rings)
            {
                n += ComputeCrossPointCount(hitPoint, ring.Points);
            }
            return !(n % 2 == 0);
        }

        public static bool IsPointInPolygon(ShapePoint hitPoint, ShapePoint[] points)
        {
            return !(ComputeCrossPointCount(hitPoint, points) % 2 == 0);
        }

        public static int ComputeCrossPointCount(ShapePoint hitPoint, ShapePoint[] points)
        {
            ShapePoint pt1 = null, pt2 = null;
            int crossPointCount = 0;
            int i = 1;
            for (; i < points.Length; i++)
            {
                pt1 = points[i - 1];
                pt2 = points[i];
                //射线和边无交点
                if ((pt1.Y < hitPoint.Y && pt2.Y < hitPoint.Y) || (pt1.Y > hitPoint.Y && pt2.Y > hitPoint.Y))
                    continue;
                if (hitPoint.X > pt1.X && hitPoint.X > pt2.X)
                    continue;
                //
                double k = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
                double x0 = (hitPoint.Y - pt1.Y) / k + pt1.X;
                //交点射线的左半部分
                if (x0 < hitPoint.X)
                    continue;
                //交点在射线的右半部分
                else if (x0 > hitPoint.X)
                    crossPointCount++;
                //交点与顶点重合
                else
                { 
                }
            }
            return crossPointCount;
        }
    }
}
