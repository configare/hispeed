using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Telerik.WinControls
{
    public class ShapeLinesCollection
    {
        #region Private Data
        private List<IShapeCurve> lines = null;

        private ShapePoint snappedCtrl;
        private PointF snappedPoint;
        private IShapeCurve snappedCurve;

        #endregion

        #region Accessors

        public List<IShapeCurve> Lines
        {
            get { return lines; }
        }

        public ShapePoint SnappedCtrlPoint
        {
            get { return snappedCtrl; }
        }

        public PointF SnappedPoint
        {
            get { return snappedPoint; }
        }

        public IShapeCurve SnappedCurve
        {
            get { return snappedCurve; }
        }

        #endregion

        #region Constructors

        public ShapeLinesCollection()
        {
            Reset();
        }

        #endregion

        #region Methods

        public void CopyFrom(ShapeLinesCollection shape)
        {
            Reset();

            List<ShapePoint> pts = new List<ShapePoint>();
            List<ShapePoint> newPts = new List<ShapePoint>();

            for ( int i = 0; i < shape.lines.Count; i++ )
            {
                IShapeCurve tmp = shape.lines[i].Create();

                for (int j = 0; j < shape.lines[i].ControlPoints.Length; j++)
                {
                    int pos = pts.IndexOf(shape.lines[i].ControlPoints[j]);
                    if (pos == -1)
                    {
                        pts.Add(shape.lines[i].ControlPoints[j]);
                        pos = pts.IndexOf(shape.lines[i].ControlPoints[j]);
                        newPts.Insert(pos, new ShapePoint(shape.lines[i].ControlPoints[j].Location));
                    }
                    tmp.ControlPoints[j] = newPts[pos];
                }
                Add(tmp);
                tmp.Update();
            }
        }

        public void Reset()
        {
            if (lines == null) 
                lines = new List<IShapeCurve>();
            else
                lines.Clear();

            snappedCtrl = null;
            snappedPoint = new PointF();
        }

        public void Add(IShapeCurve el)
        {
            lines.Add(el);
        }

        public bool Remove(IShapeCurve el)
        {
            return lines.Remove(el);
        }

        public void UpdateShape()
        {
            for (int i = 0; i < lines.Count; i++)
                lines[i].Update();
        }

        public RectangleF GetBoundingRect()
        {
            float minX, minY;
            float maxX, maxY;

            if (lines.Count < 1) return new RectangleF(0, 0, 0, 0);

            minX = maxX = lines[0].FirstPoint.X;
            minY = maxY = lines[0].FirstPoint.Y;

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Points.Length; j++)
                {
                    minX = Math.Min(minX, lines[i].Points[j].X);
                    minY = Math.Min(minY, lines[i].Points[j].Y);
                    maxX = Math.Max(maxX, lines[i].Points[j].X);
                    maxY = Math.Max(maxY, lines[i].Points[j].Y);
                }
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public void DeletePoint(ShapePoint pt)
        {
            IShapeCurve curve;
            ShapePoint to;

            if (pt.IsLocked) return;

            if (!FindLineContainingPoint(pt, out to, out curve)) return;

            Remove(curve);

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].FirstPoint == pt)
                {
                    lines[i].FirstPoint = to;
                    lines[i].Update();
                }

                if (lines[i].LastPoint == pt)
                {
                    lines[i].LastPoint = to;
                    lines[i].Update();
                }
            }

        }

        public void ConvertCurve(IShapeCurve curve)
        {
            if (curve is ShapeLine)
            {
                ShapePoint c1 = new ShapePoint(
                    (2 * snappedCurve.Points[0].X + snappedCurve.Points[1].X) / 3,
                    (2 * snappedCurve.Points[0].Y + snappedCurve.Points[1].Y) / 3
                );
                ShapePoint c2 = new ShapePoint(
                    (snappedCurve.Points[0].X + 2 * snappedCurve.Points[1].X) / 3,
                    (snappedCurve.Points[0].Y + 2 * snappedCurve.Points[1].Y) / 3
                );

                ShapeBezier bezier = new ShapeBezier(snappedCurve.FirstPoint, c1, c2, snappedCurve.LastPoint);

                lines[lines.IndexOf(curve)] = bezier;
            }
            else
            {
                ShapeLine line = new ShapeLine(snappedCurve.FirstPoint, snappedCurve.LastPoint);

                lines[lines.IndexOf(curve)] = line;
            }
            snappedCurve = null;
        }

        public void InsertPoint(IShapeCurve curve, PointF atPoint)
        {
            if (curve == null) return;
            
            if (curve is ShapeLine) 
            {
                ShapePoint end = curve.LastPoint;
                ShapePoint middle = new ShapePoint(atPoint);
                ShapeLine newLine = new ShapeLine(middle, end);
                curve.LastPoint = middle;
                lines.Insert(lines.IndexOf(curve) + 1, newLine);
                return;
            }

            if (curve is ShapeBezier)
            {
                PointF tg1 = new PointF(), tg2 = new PointF();

                ShapePoint end = snappedCurve.LastPoint;
                ShapePoint middle = new ShapePoint(snappedPoint);

                if (!(curve as ShapeBezier).TangentAt(snappedPoint, ref tg1, ref tg2))
                    return;

                ShapeBezier newBezier = new ShapeBezier();

                newBezier.ControlPoints[0] = middle;
                newBezier.ControlPoints[1] = new ShapePoint(tg2);
                newBezier.ControlPoints[2] = curve.ControlPoints[2];
                newBezier.ControlPoints[3] = curve.LastPoint;

                curve.ControlPoints[2] = new ShapePoint(tg1);
                curve.ControlPoints[3] = middle;

                curve.Update();
                newBezier.Update();

                lines.Insert(lines.IndexOf(curve) + 1, newBezier);

                return;

            }

        }
        
        private bool FindLineContainingPoint(ShapePoint pt, out ShapePoint changeTo, out IShapeCurve line)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].FirstPoint == pt)
                {
                    changeTo = lines[i].LastPoint;
                    line = lines[i];
                    return true;
                }
                if (lines[i].LastPoint == pt)
                {
                    changeTo = lines[i].FirstPoint;
                    line = lines[i];
                    return true;
                }
            }

            changeTo = null;
            line = null;

            return false;
        }

        #region Snappings

        public bool SnapToCtrlPoints(PointF pt, float snapDistance)
        {
            bool res = false;
            float minDist = snapDistance * snapDistance;
            float curDist;

            snappedCtrl = null;

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].SnapToCtrlPoints(pt, snapDistance)) continue;

                curDist = ShapePoint.DistSquared(pt, lines[i].SnappedCtrlPoint.Location);
                if (minDist < curDist) continue;

                minDist = curDist;

                snappedCtrl = lines[i].SnappedCtrlPoint;
                res = true;
            }

            return res;
        }
        
        public bool SnapToSegments(PointF pt, float snapDistance)
        {
            bool res = false;
            float curDist;
            float minDist = snapDistance *snapDistance;

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].SnapToCurve(pt, snapDistance)) continue;

                curDist = ShapePoint.DistSquared(lines[i].SnappedPoint, pt);

                if (minDist < curDist) continue;
                minDist = curDist;
                snappedPoint = lines[i].SnappedPoint;
                snappedCurve = lines[i];
                res = true;
            }

            return res;
        }
        
        public bool SnapToExtensions(PointF pt, float snapDistance)
        {
            bool res = false;
            float minDist = snapDistance * snapDistance;
            float curDist;

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].SnapToCurveExtension(pt, snapDistance)) continue;

                curDist = ShapePoint.DistSquared(lines[i].SnappedPoint, pt);

                if (minDist < curDist) continue;
                minDist = curDist;
                snappedPoint = lines[i].SnappedPoint;
                res = true;
            }

            return res;
        }

        public bool SnapToHorizontal(PointF pt, float yVal, float snapDistance)
        {
            bool res = false;
            float curDist;
            float minDist = snapDistance *snapDistance;
            PointF snapPt = new PointF();
            IShapeCurve snapCurve = null;

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].SnapToHorizontal(pt, yVal, snapDistance)) continue;

                curDist = ShapePoint.DistSquared(lines[i].SnappedPoint, pt);

                if (minDist < curDist) continue;
                minDist = curDist;
                snapPt = lines[i].SnappedPoint;
                snapCurve = lines[i];
                res = true;
            }

            if (res)
            {
                snappedPoint = snapPt;
                snappedCurve = snapCurve;
            }

            return res;
        }

        public bool SnapToVertical(PointF pt, float xVal, float snapDistance)
        {
            //return false;
            bool res = false;
            float curDist;
            float minDist = snapDistance * snapDistance;
            PointF snapPt = new PointF();
            IShapeCurve snapCurve = null;

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].SnapToVertical(pt, xVal, snapDistance)) continue;

                curDist = ShapePoint.DistSquared(lines[i].SnappedPoint, pt);

                if (minDist < curDist) continue;
                minDist = curDist;
                snapPt = lines[i].SnappedPoint;
                snapCurve = lines[i];
                res = true;
            }

            if (res)
            {
                snappedPoint = snapPt;
                snappedCurve = snapCurve;
            }

            return res;
        }


        // SnapToGrid types:
        // 1 - snap to horizontal
        // 2 - snap to vertical
        // 3 - snap to both
        public bool SnapToGrid(PointF pt, PointF gridPt, int type, float snapDistance)
        {
            bool snappingOccured = false;
            int tmpRes = 0; // temporary result to find the closer snapped point
            PointF point1 = new PointF();
            IShapeCurve snapCurve = null;

            if ((type & 0x01) != 0)
            {
                tmpRes += SnapToHorizontal(pt, gridPt.Y, snapDistance) ? 1 : 0;
                point1 = snappedPoint;
                snapCurve = snappedCurve;
            }

            if ((type & 0x02) != 0)
                tmpRes += SnapToVertical(pt, gridPt.X, snapDistance) ? 2 : 0;

            switch (tmpRes)
            {
                default:
                case 0:
                    // no snapping occured
                    snappingOccured = false;
                    break;
                case 1:
                    // only horizontal snapping occured
                    // snappedPoint already contains the right snapping point
                    snappingOccured = true;
                    break;
                case 2:
                    // only vertical snapping occured
                    // snappedPoint already contains the right snapping point
                    snappingOccured = true;
                    break;

                case 3:
                    // take the closer point and snap to it
                    if (ShapePoint.DistSquared(pt, point1) < ShapePoint.DistSquared(pt, snappedPoint))
                    {
                        snappedPoint = point1;
                        snappedCurve = snapCurve;
                    }

                    snappingOccured = true;
                    break;
            }

            return snappingOccured;
        }

        //public bool SnapToIntersection(PointF pt, float snapDistance)
        //{
        //    float distSq;
        //    float minDist = snapDistance * snapDistance;
        //    PointF closestPt = new PointF();
        //    PointF tmpPt = new PointF();
        //    bool res = false;

        //    for (int i = 0; i < lines.Count; i++)
        //        for (int j = i + 1; j < lines.Count; j++)
        //        {
        //            if (!ShapeLine.Intersect(out tmpPt, lines[i].Points[0].Point, lines[i].Points[1].Point, lines[j].Points[0].Point, lines[j].Points[1].Point)) continue;

        //            distSq = ShapePoint.DistSquared(pt, tmpPt);

        //            if (distSq > minDist) continue;
        //            minDist = distSq;
        //            closestPt = tmpPt;
        //            res = true;
        //        }

        //    if (res)
        //        snappedPoint = closestPt;

        //    return res;
        //}

        #endregion

        public GraphicsPath CreatePath(Rectangle dimension, Rectangle bound)
        {
            GraphicsPath path = new GraphicsPath();

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] is ShapeBezier)
                {
                    path.AddBezier(
                        lines[i].ControlPoints[0].GetPoint(dimension, bound),
                        lines[i].ControlPoints[1].GetPoint(dimension, bound),
                        lines[i].ControlPoints[2].GetPoint(dimension, bound),
                        lines[i].ControlPoints[3].GetPoint(dimension, bound)
                    );
                    continue;
                }
                if (lines[i] is ShapeLine)
                {
                    path.AddLine(
                        lines[i].FirstPoint.GetPoint(dimension, bound), 
                        lines[i].LastPoint.GetPoint(dimension, bound)
                    );
                    continue;
                }
            }
            /*for (int i = 0; i < points.Count; i++)
            {
                ShapePoint p1 = points[i];
                ShapePoint p2 = i < points.Count - 1 ? points[i + 1] : points[0];

                Point rp1 = p1.GetPoint(dimension, bounds);
                Point rp2 = p2.GetPoint(dimension, bounds);

                if (p1.Bezier)
                    path.AddBezier(rp1, p1.ControlPoint1.GetPoint(dimension, bounds),
                        p1.ControlPoint2.GetPoint(dimension, bounds), rp2);
                else
                    path.AddLine(rp1, rp2);
            }*/
            path.CloseAllFigures();

            return path;
        }

        public bool isSerializable()
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (lines[i].LastPoint != lines[i + 1].FirstPoint) return false;
            }

            if (lines[0].FirstPoint != lines[lines.Count - 1].LastPoint) return false;

            return true;
        }

        public string SerializeProperties()
        {
            StringBuilder res = new StringBuilder();

            for ( int i = 0; i < lines.Count; i++ )
            {
                if (lines[i] is ShapeBezier) {
                    res.AppendFormat(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7}:",
                        lines[i].ControlPoints[0].X, lines[i].ControlPoints[0].Y,
                        true,
                        lines[i].ControlPoints[1].X, lines[i].ControlPoints[1].Y,
                        lines[i].ControlPoints[2].X, lines[i].ControlPoints[2].Y,
                        0   // change to anchor
                        );
                    continue;
                }

                if (lines[i] is ShapeLine)
                {
                    res.AppendFormat(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7}:",
                        lines[i].ControlPoints[0].X, lines[i].ControlPoints[0].Y,
                        false,
                        0, 0, 0, 0, // no control points
                        0           // change to anchor
                    );
                    continue;
                }
            }

            return res.ToString();
        }

        public void DeserializeProperties(string propertiesString)
        {
            string[] tokens = propertiesString.Split(':');

            IShapeCurve curve = null;

            ShapePoint startPt = new ShapePoint();
            ShapePoint endPt = startPt;

            

            for (int i = 1; i < tokens.Length - 1; i++)
            {
                string[] strpt = tokens[i].Split(',');

                endPt.Set(float.Parse(strpt[0], CultureInfo.InvariantCulture), float.Parse(strpt[1], CultureInfo.InvariantCulture));

                if (bool.Parse(strpt[2]))
                {
                    // Bezier curve
                    curve = new ShapeBezier();

                    curve.ControlPoints[0] = endPt;
                    //bool b = CultureInfo.CurrentCulture.UseUserOverride;
                    NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
                    curve.ControlPoints[1] = new ShapePoint(float.Parse(strpt[3], CultureInfo.InvariantCulture), float.Parse(strpt[4], CultureInfo.InvariantCulture));
                    curve.ControlPoints[2] = new ShapePoint(float.Parse(strpt[5], CultureInfo.InvariantCulture), float.Parse(strpt[6], CultureInfo.InvariantCulture));

                    endPt = new ShapePoint();
                    curve.ControlPoints[3] = endPt;
                }
                else
                {
                    // Straight Line
                    curve = new ShapeLine();

                    curve.ControlPoints[0] = endPt;

                    endPt = new ShapePoint();
                    curve.ControlPoints[1] = endPt;
                }
                this.Add(curve);
            }

            // Attach the end of the last line to the first point to close the shape
            if (curve != null)
                curve.LastPoint = startPt;
        }

        public ShapePoint GetFirstPoint()
        {
            if (lines == null || lines.Count < 1) return null;

            return lines[0].FirstPoint;
        }

        public ShapePoint GetLastPoint()
        {
            if (lines == null || lines.Count < 1) return null;

            return lines[lines.Count - 1].LastPoint;
        }

        public IShapeCurve GetFirstCurve()
        {
            if (lines == null || lines.Count < 1) return null;

            return lines[0];
        }

        public IShapeCurve GetLastCurve()
        {
            if (lines == null || lines.Count < 1) return null;

            return lines[lines.Count - 1];
        }

        #endregion

    }
}
