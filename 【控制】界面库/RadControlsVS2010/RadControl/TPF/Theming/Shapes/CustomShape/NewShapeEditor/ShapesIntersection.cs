using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public class ShapesIntersection
    {
        #region Private Data
        
        private IShapeCurve curve1;
        private IShapeCurve curve2;

        private List<PointF> curveIntersections;
        private List<PointF> extIntersections;

        #endregion

        #region Accessors

        public IShapeCurve Curve1
        {
            get { return curve1; }
            set { curve1 = value; }
        }

        public IShapeCurve Curve2
        {
            get { return curve2; }
            set { curve2 = value; }
        }

        public List<PointF> CurveIntersections
        {
            get { return curveIntersections; }
        }

        #endregion

        public ShapesIntersection()
        {
            curve1 = null;
            curve2 = null;
            curveIntersections = new List<PointF>();
            extIntersections = new List<PointF>();
        }

        public ShapesIntersection(IShapeCurve c1, IShapeCurve c2)
        {
            curve1 = c1;
            curve2 = c2;

            curveIntersections = new List<PointF>();
            extIntersections = new List<PointF>();
        }

        public bool IntersectCurves()
        {
            bool res = false;

            if ( (curve1 == null) || (curve2 == null) || (curve1 == curve2))
                return false;

            RectangleF rect1, rect2; 
            //used to check if two line segments are close enough to have intersection

            rect1 = new Rectangle();
            rect2 = new Rectangle();

            PointF intersection;

            curveIntersections.Clear();
            // Each 2 points define segment on the curve. 
            // So, check each segment of curve1 against each segment on curve2
            for (int i = 0; i < curve1.Points.Length - 1; i++)
            {
                rect1 = SegmentBoundingRect(rect1, curve1.Points[i], curve1.Points[i + 1]);
                for (int j = 0; j < curve2.Points.Length - 1; j++)
                {
                    rect2 = SegmentBoundingRect(rect2, curve2.Points[j], curve2.Points[j + 1]);
                    rect2.Intersect(rect1);

                    if (rect2.IsEmpty) continue; // no intersection

                    if (IntersectSegment(out intersection,
                        curve1.Points[i].Location, curve1.Points[i + 1].Location,
                        curve2.Points[j].Location, curve2.Points[j + 1].Location
                        ))
                        curveIntersections.Add(intersection);


                }
            }

            return res;
        }

        private RectangleF SegmentBoundingRect(RectangleF rect, ShapePoint pt1, ShapePoint pt2)
        {
            rect.X = Math.Min(pt1.X, pt2.X);
            rect.Y = Math.Min(pt1.Y, pt2.Y);

            rect.Width = Math.Abs(pt1.X - pt2.X);
            rect.Height = Math.Abs(pt1.Y - pt2.Y);

            return rect;
        }

        public static bool Intersect(out PointF res, PointF a, PointF b, PointF c, PointF d)
        {
            float d1, d2, d3;

            res = new PointF(0, 0);

            //d1 = (x1 * y2) - (x2 * y1);
            //d2 = (x3 * y4) - (x4 * y3);
            //d3 = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            d3 = (a.X - b.X) * (c.Y - d.Y) - (a.Y - b.Y) * (c.X - d.X);

            if (d3 == 0) return false; // parallel lines encountered

            d1 = (a.X * b.Y) - (b.X * a.Y);
            d2 = (c.X * d.Y) - (d.X * c.Y);

            res.X = (d1 * (c.X - d.X) - d2 * (a.X - b.X)) / d3;
            res.Y = (d1 * (c.Y - d.Y) - d2 * (a.Y - b.Y)) / d3;

            return true;
        }

        public static bool IntersectSegment(out PointF res, PointF a, PointF b, PointF c, PointF d)
        {
            bool r = Intersect(out res, a, b, c, d);

            if (res.X < Math.Min(a.X, b.X) || (res.X > Math.Max(a.X, b.X)) ||
                res.Y < Math.Min(a.Y, b.Y) || (res.Y > Math.Max(a.Y, b.Y))
                ) return false;

            if (res.X < Math.Min(c.X, d.X) || (res.X > Math.Max(c.X, d.X)) ||
                res.Y < Math.Min(c.Y, d.Y) || (res.Y > Math.Max(c.Y, d.Y))
                ) return false;

            return true;
        }
    }
}
