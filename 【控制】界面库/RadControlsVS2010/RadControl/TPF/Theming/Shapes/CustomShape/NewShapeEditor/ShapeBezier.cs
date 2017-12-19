using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public class ShapeBezier : IShapeCurve
    {
        #region Private Data

        private ShapePoint[] ctrl; // control points

        private ShapePoint[] points; // generated curve segment points

        private int detailLevel;

        private PointF snappedPoint;

        private ShapePoint snappedCtrl;

        private int snapSegmentNum;

        #endregion

        #region Constructors

        public ShapeBezier()
        {
            detailLevel = 32;
            ctrl = new ShapePoint[4];
            points = new ShapePoint[detailLevel];

            snappedCtrl = null;
            snappedPoint = new PointF();
            snapSegmentNum = -1;

        }

        public ShapeBezier(ShapePoint from, ShapePoint control1, ShapePoint control2, ShapePoint to)
        {
            detailLevel = 32;
            ctrl = new ShapePoint[4];
            ctrl[0] = from;
            ctrl[1] = control1;
            ctrl[2] = control2;
            ctrl[3] = to;

            snappedCtrl = null;
            snappedPoint = new PointF();

            points = new ShapePoint[detailLevel];
            snapSegmentNum = -1;

            GenerateSegments();
        }

        #endregion

        #region Accessors

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<PointF> TestPoints
        {
            get { return null; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint[] Points
        {
            get {
                if (IsModified) GenerateSegments();
                return points; 
            }
        }

        [Browsable(true)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("The starting point of the bezier curve.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint FirstPoint
        {
            get { return ctrl[0]; }
            set
            {
                if (value == null) return;
                        
                ctrl[0] = value;
            }
        }

        [Browsable(true)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("The first control point of the bezier curve. The line between this FirstPoint and this point is tangent to the bezier curve.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint Ctrl1
        {
            get { return ctrl[1]; }
            set {
                if (value == null) return;

                ctrl[1] = value;
            }
        }

        [Browsable(true)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("The second control point of the bezier curve. The line between this point and LastPoint is tangent to the bezier curve.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint Ctrl2
        {
            get { return ctrl[2]; }
            set
            {
                if (value == null) return;

                ctrl[2] = value;
            }
        }

        [Browsable(true)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("The ending point of the bezier curve.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint LastPoint
        {
            get { return ctrl[3]; }
            set { ctrl[3] = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint[] Extensions
        {
            get
            {
                return ctrl;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PointF SnappedPoint
        {
            get { return snappedPoint; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint SnappedCtrlPoint
        {
            get { return snappedCtrl; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint[] ControlPoints
        {
            get { return ctrl; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified
        {
            get {
                for (int i = 0; i < 4; i++)
                    if (ctrl[i].IsModified) return true;

                return false; 
            }
        }

        #endregion

        #region Methods

        private void GenerateSegments()
        {
            //double x1 = x, x2 = nextPoint.X, cx1 = controlPoint1.X, cx2 = controlPoint2.X;
            //double y1 = y, y2 = nextPoint.Y, cy1 = controlPoint1.Y, cy2 = controlPoint2.Y;

            //* calculate the polynomial coefficients */
            float cx = 3.0f * (ctrl[1].X - ctrl[0].X);
            float bx = 3.0f * (ctrl[2].X - ctrl[1].X) - cx;
            float ax = ctrl[3].X - ctrl[0].X - cx - bx;

            float cy = 3.0f * (ctrl[1].Y - ctrl[0].Y);
            float by = 3.0f * (ctrl[2].Y - ctrl[1].Y) - cy;
            float ay = ctrl[3].Y - ctrl[0].Y - cy - by;

            /* calculate the curve point at parameter value t */
            int i;
            float t;
            for (t = 0, i = 0; i < detailLevel; i++, t += (float)1 / (detailLevel - 1))
            {
                float tSquared = t * t;
                float tCubed = t * tSquared;

                points[i] = new ShapePoint(
                    (ax * tCubed) + (bx * tSquared) + (cx * t) + ctrl[0].X,
                    (ay * tCubed) + (by * tSquared) + (cy * t) + ctrl[0].Y
                    );
            }
        }

        public bool Update()
        {
            GenerateSegments();
            return true;
        }

        public bool TangentAt(PointF atPoint, ref PointF from, ref PointF to)
        {
            if ((snapSegmentNum < 0) || (snapSegmentNum > points.Length - 2))
                return false;

            from = points[snapSegmentNum].Location;
            to = points[snapSegmentNum + 1].Location;

            return true;
        }

        public bool SnapToCurve(PointF pt, float snapDistance)
        {
            bool res = false;
            PointF minPt = new PointF();
            PointF curPt = new PointF();

            float minDist = snapDistance * snapDistance;
            float curDist = 0;

            if (IsModified) return false;

            for (int i = 0; i < detailLevel - 1; i++)
            {
                if (!SnapToCurveSegment(ref curPt, pt, points[i], points[i + 1], snapDistance, ref curDist)) continue;

                curDist = ShapePoint.DistSquared(pt, curPt);

                if (minDist < curDist) continue;
                minDist = curDist;
                minPt = curPt;
                snapSegmentNum = i;
                res = true;
            }

            if (!res) return false;

            snappedPoint = minPt;

            return true;
        }

        public bool SnapToCurveExtension(PointF pt, float snapDistance)
        {
            PointF point1 = new PointF();
            PointF point2 = new PointF();

            bool finalSnapResult = true;

            int snapResult = 0;
            float dist1 = 0, dist2 = 0;
            if (!ctrl[0].IsModified && !ctrl[1].IsModified)
                snapResult += SnapToExtension(ref point1, pt, ctrl[0], ctrl[1], snapDistance, ref dist1) ? 1 : 0;
            
            if (!ctrl[3].IsModified && !ctrl[2].IsModified)
                snapResult += SnapToExtension(ref point2, pt, ctrl[3], ctrl[2], snapDistance, ref dist2) ? 2 : 0;

            switch (snapResult)
            {
                default:
                case 0:
                    // none is snapped
                    finalSnapResult = false;
                    break;
                case 1:
                    // point1 is snapped to extension 1
                    snappedPoint = point1;
                    break;
                case 2:
                    // point2 is snapped to extension 2
                    snappedPoint = point2;
                    break;
                case 3:
                    // both lines are snapped-to-, should find closer one
                    if (ShapePoint.DistSquared(pt, point1) <= ShapePoint.DistSquared(pt, point2))
                        snappedPoint = point1;
                    else
                        snappedPoint = point2;
                    break;
            }

            return finalSnapResult;
        }

        public bool SnapToCtrlPoints(PointF pt, float snapDistance)
        {
            float curDist;
            float minDist = snapDistance * snapDistance;

            snappedCtrl = null;

            for (int i = 0; i < 4; i++)
            {
                if (ctrl[i].IsModified) continue; // do not snap to a point whose location is being modified
                curDist = ShapePoint.DistSquared(ctrl[i].Location, pt);
                if (minDist < curDist) continue;

                minDist = curDist;
                snappedCtrl = ctrl[i];
            }

            return snappedCtrl != null;
        }

        public bool SnapToVertical(PointF pt, float xVal, float snapDistance)
        {
            bool res = false;
            float minDist = snapDistance;
            float dx, dy, y;

            if (IsModified) return false;

            for (int i = 0; i < points.Length - 1; i++)
            {
                dx = points[i + 1].X - points[i].X;
                if (dx == 0) continue;
                dy = points[i + 1].Y - points[i].Y;
                y = points[i].Y + (xVal - points[i].X) * dy / dx;

                if (!IsPointOnSegment(xVal, y, points[i], points[i+1])) continue;

                if (Math.Abs(y - pt.Y) > minDist) continue;
                snappedPoint.X = xVal;
                snappedPoint.Y = y;
                res = true;
            }

            return res;
        }

        public bool SnapToHorizontal(PointF pt, float yVal, float snapDistance)
        {
            bool res = false;
            float minDist = snapDistance;
            float dx, dy, x;

            if (IsModified) return false;

            for (int i = 0; i < points.Length - 1; i++)
            {
                dy = points[i + 1].Y - points[i].Y;
                if (dy == 0) continue;
                dx = points[i + 1].X - points[i].X;
                x = points[i].X + (yVal - points[i].Y) * dx / dy;

                if (!IsPointOnSegment(x, yVal, points[i], points[i + 1])) continue;

                if (Math.Abs(x - pt.X) > minDist) continue;
                snappedPoint.Y = yVal;
                snappedPoint.X = x;
                res = true;
            }

            return res;
        }

        private static bool SnapToExtension(ref PointF snapPoint, PointF pt, ShapePoint from, ShapePoint to, float snapDistance, ref float dist)
        {
            float dx = to.X - from.X;
            float dy = to.Y - from.Y;

            if ((dx == 0) || (dy == 0))
            {
                if ((dx == 0) && (dy == 0)) return false;
                snapPoint = pt;
                if (dx == 0)
                {
                    dist = Math.Abs(pt.X - to.X);
                    if (dist < snapDistance)
                        snapPoint.X = to.X;
                    else
                        return false;
                }
                else
                {
                    dist = Math.Abs(pt.Y - to.Y);
                    if (dist < snapDistance)
                        snapPoint.Y = to.Y;
                    else 
                        return false;
                }
                return true;
            }

            float dst = (float)((dx * (from.Y - pt.Y) - dy * (from.X - pt.X)) / Math.Sqrt(dx * dx + dy * dy));

            dist = Math.Abs(dst);


            if (dist > snapDistance) return false;

            
            snapPoint.Y = pt.Y + (float)(Math.Sign(dx) * dst / Math.Sqrt( 1 + (dy*dy)/(dx*dx) ));
            
            // Shortest calculation of snapPoint.X would be analogous, but roundings lead to incorrect
            // calculations of both points, so instead of:
            //      snapPoint.X = pt.X - (int)Math.Round(Math.Sign(dy) * dist);
            // for the snapPoint.Y, find which is snapPoint.X:

            snapPoint.X = from.X + ((snapPoint.Y - from.Y) * dx / dy);

            return true;
        }

        private static bool IsPointOnSegment(float x, float y, ShapePoint from, ShapePoint to)
        {
            if ((from.X != to.X) &&
                ((x < Math.Min(from.X, to.X)) || (x > Math.Max(from.X, to.X)))
                ) return false;

            if ((from.Y != to.Y) &&
                ((y < Math.Min(from.Y, to.Y)) || (y > Math.Max(from.Y, to.Y)))
                ) return false;

            return true;
        }

        private static bool SnapToCurveSegment(ref PointF snapPoint, PointF pt, ShapePoint from, ShapePoint to, float snapDistance, ref float dist)
        {
            //RectangleF rect = new RectangleF(
            //    Math.Min(from.X, to.X), Math.Min(from.Y, to.Y),
            //    Math.Abs(from.X - to.X), Math.Abs(from.Y - to.Y));

            //rect.Inflate(snapDistance, snapDistance);

            //if (!rect.Contains(pt)) return false;

            if (!SnapToExtension(ref snapPoint, pt, from, to, snapDistance, ref dist)) return false;

            return IsPointOnSegment(snapPoint.X, snapPoint.Y, from, to);
        }

        public IShapeCurve Create()
        {
            return new ShapeBezier();
        }

        #endregion

    }
}
