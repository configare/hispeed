using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public class ShapeLine : IShapeCurve
    {
        #region Private Data

        private ShapePoint[] points;

        private PointF snapPoint;

        private ShapePoint snapCtrl;

        #endregion

        #region Constructors

        public ShapeLine()
        {
            points = new ShapePoint[2];
            snapPoint = new PointF();
            snapCtrl = null;
        }

        public ShapeLine(ShapePoint from, ShapePoint to)
        {
            points = new ShapePoint[2];
            points[0] = from;
            points[1] = to;
            snapPoint = new PointF();
            snapCtrl = null;
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
            get { return points; }
        }

        [Browsable(true)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("The starting point of the line")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint FirstPoint
        {
            get { return points[0]; }
            set { points[0] = value; }
        }

        [Browsable(true)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("The ending point of the line")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint LastPoint
        {
            get { return points[1]; }
            set { points[1] = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint[] Extensions
        {
            get { return points; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PointF SnappedPoint
        {
            get { return snapPoint; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint SnappedCtrlPoint
        {
            get { return snapCtrl; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapePoint[] ControlPoints
        {
            get { return points; }
        }

            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified
        {
            get { return (points[0].IsModified || points[1].IsModified); }
        }

        #endregion

        #region Methods

        public bool Update()
        {
            return true;
        }

        public IShapeCurve Create()
        {
            return new ShapeLine();
        }

        public bool SnapToCurve(PointF pt, float snapDistance)
        {
            /*RectangleF rect = new RectangleF(
                Math.Min(points[0].X, points[1].X), Math.Min(points[0].Y, points[1].Y),
                Math.Abs(points[0].X - points[1].X), Math.Abs(points[0].Y - points[1].Y));

            if (!rect.Contains(pt)) return false;*/

            if (!SnapToCurveExtension(pt, snapDistance)) return false;

            if ((points[0].X != points[1].X) &&
                ((snapPoint.X < Math.Min(points[0].X, points[1].X)) || (snapPoint.X > Math.Max(points[0].X, points[1].X)))
                ) return false;

            if ((points[0].Y != points[1].Y) &&
                 ((snapPoint.Y < Math.Min(points[0].Y, points[1].Y)) || (snapPoint.Y > Math.Max(points[0].Y, points[1].Y)))
                ) return false;

            return true;
        }

        public bool SnapToCurveExtension(PointF pt, float snapDistance)
        {
            float dx = points[1].X - points[0].X;
            float dy = points[1].Y - points[0].Y;

            if (IsModified) return false;

            if ((dx == 0) || (dy == 0))
            {
                if ((dx == 0) && (dy == 0)) return false;
                snapPoint = pt;
                if (dx == 0)
                    if (Math.Abs(pt.X - points[1].X) < snapDistance)
                        snapPoint.X = points[1].X;
                    else
                        return false;
                else
                    if (Math.Abs(pt.Y - points[1].Y) < snapDistance)
                        snapPoint.Y = points[1].Y;
                    else return false;

                return true;
            }

            float dist = (float)((dx * (points[0].Y - pt.Y) - dy * (points[0].X - pt.X)) / Math.Sqrt(dx * dx + dy * dy));

            if (Math.Abs(dist) > snapDistance) return false;

            snapPoint.Y = pt.Y + (float)(Math.Sign(dx) * dist / Math.Sqrt(1 + (dy * dy) / (dx * dx) ) );
            // Shortest calculation of snapPoint.X would be analogous, but roundings lead to incorrect
            // calculations of both points, so instead of:
            //      snapPoint.X = pt.X - (int)Math.Round(Math.Sign(dy) * dist);
            // for the snapPoint.Y, find which is snapPoint.X:
            snapPoint.X = points[0].X + ((snapPoint.Y - points[0].Y) * dx / dy);

            return true;
        }

        public bool SnapToCtrlPoints(PointF pt, float snapDistance)
        {
            float distSq = snapDistance * snapDistance;
            ShapePoint point;
            float dist1 = distSq + 2;
            float dist2 = distSq + 2;

            if (!points[0].IsModified)
                dist1 = ShapePoint.DistSquared(points[0].Location, pt);

            if (!points[1].IsModified)
                dist2 = ShapePoint.DistSquared(points[1].Location, pt);

            if (dist1 < dist2)
                point = points[0];
            else {
                dist1 = dist2;
                point = points[1];
            }

            if (dist1 >= distSq) return false;

            snapCtrl = point;

            return true;
        }

        public bool SnapToVertical(PointF pt, float xVal, float snapDistance)
        {
            if (IsModified) return false;
            float dx = points[1].X - points[0].X;

            if (dx == 0) return false;

            float dy = points[1].Y - points[0].Y;

            snapPoint.X = xVal;
            snapPoint.Y = points[0].Y + (snapPoint.X - points[0].X) * dy / dx;

            if (snapPoint.Y - pt.Y > snapDistance) return false;

            return true;
        }

        public bool SnapToHorizontal(PointF pt, float yVal, float snapDistance)
        {
            if (IsModified) return false;

            float dy = points[1].Y - points[0].Y;

            if (dy == 0) return false;

            float dx = points[1].X - points[0].X;

            snapPoint.Y = yVal;
            snapPoint.X = points[0].X + (snapPoint.Y - points[0].Y) * dx / dy;

            if (snapPoint.X - pt.X > snapDistance) return false;

            return true;
        }

        #endregion
    }
}
