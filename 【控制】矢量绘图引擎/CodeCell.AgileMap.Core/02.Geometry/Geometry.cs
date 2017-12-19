using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class Envelope : ICloneable
    {
        private double _minX;
        private double _maxX;
        private double _minY;
        private double _maxY;
        //
        public Envelope()
        {
        }

        public Envelope(double minX, double minY, double maxX, double maxY)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _centerPoint = null;
        }

        public void UnionWith(Envelope envelope)
        {
            _minX = Math.Min(_minX, envelope._minX);
            _minY = Math.Min(_minY, envelope._minY);
            _maxX = Math.Max(_maxX, envelope._maxX);
            _maxY = Math.Max(_maxY, envelope._maxY);
        }

        public ShapePolygon ToPolygon()
        {
            ShapePoint[] pts = new ShapePoint[] 
                            {
                                new ShapePoint(MinX,MaxY),
                                new ShapePoint(MaxX,MaxY),
                                new ShapePoint(MaxX,MinY),
                                new ShapePoint(MinX,MinY),
                                new ShapePoint(MinX,MaxY)
                            };
            ShapeRing ring = new ShapeRing(pts);
            return new ShapePolygon(new ShapeRing[] { ring });
        }

        [DisplayName("最小经度"), ReadOnly(true)]
        public double MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }

        [DisplayName("最小纬度"), ReadOnly(true)]
        public double MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        [DisplayName("最大经度"), ReadOnly(true)]
        public double MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        [DisplayName("最大纬度"), ReadOnly(true)]
        public double MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        public bool IsEmpty()
        {
            return _minX == 0d && _minY == 0d && _maxX == 0d && _maxY == 0d;
        }

        [Browsable(false)]
        public ShapePoint LeftUpPoint
        {
            get { return new ShapePoint(_minX, _maxY); }
        }

        [Browsable(false)]
        public ShapePoint RightDownPoint
        {
            get { return new ShapePoint(_maxX, _minY); }
        }

        [Browsable(false)]
        public ShapePoint[] Points
        {
            get
            {
                return new ShapePoint[] { LeftUpPoint, new ShapePoint(_maxX, _maxY), RightDownPoint, new ShapePoint(_minX, _minY) };
            }
        }

        public bool IsContains(ShapePoint pt)
        {
            return pt.X > _minX && pt.X < _maxX && pt.Y > _minY && pt.Y < _maxY;
        }

        public bool IsEqual(Envelope nEnvelope, double precisionX, double precisionY)
        {
            return Math.Abs(_maxX - nEnvelope.MaxX) < precisionX && Math.Abs(_maxY - nEnvelope._maxY) < precisionY
                && Math.Abs(_minX - nEnvelope._minX) < precisionX && Math.Abs(_minY - nEnvelope._minY) < precisionY;
        }

        public bool IsInteractived(Envelope envelope)
        {
            if (envelope.MaxX < _minX ||
                envelope.MinX > _maxX ||
                envelope.MinY > _maxY ||
                envelope.MaxY < _minY)
                return false;
            return true;
        }

        public bool IsInteractived(Envelope envelope, ref bool fullInternal)
        {
            if (envelope.MaxX < _minX || envelope.MinX > _maxX || envelope.MinY > _maxY || envelope._maxY < _minY)
                return false;
            fullInternal = envelope.MinX > _minX && envelope._maxX < _maxX && envelope.MinY > _minY && envelope._maxY < _maxY;
            return true;
        }

        public Envelope IntersectWith(Envelope envelope)
        {
            RectangleF a = new RectangleF((float)_minX, (float)_minY, (float)_maxX - (float)_minX, (float)_maxY - (float)_minY);
            RectangleF b = new RectangleF((float)envelope._minX, (float)envelope._minY, (float)envelope._maxX - (float)envelope._minX, (float)envelope._maxY - (float)envelope._minY);
            a.Intersect(b);
            if (Math.Abs(a.Left) < float.Epsilon && Math.Abs(a.Top) < float.Epsilon && Math.Abs(a.Right) < float.Epsilon && Math.Abs(a.Bottom) < float.Epsilon)
                return null;
            return new Envelope(a.Left, a.Top, a.Right, a.Bottom);
        }

        public bool Contains(Envelope envelope)
        {
            return envelope.MinX > _minX &&
                   envelope.MaxX < _maxX &&
                   envelope.MinY > _minY &&
                   envelope._maxY < _maxY;
        }

        private ShapePoint _centerPoint;
        [Browsable(false)]
        public ShapePoint CenterPoint
        {
            get
            {
                if (_centerPoint == null)
                {
                    _centerPoint = new ShapePoint();
                    _centerPoint.X = (_maxX + _minX) / 2d;
                    _centerPoint.Y = (_maxY + _minY) / 2d;
                }
                return _centerPoint;
            }
        }

        [Browsable(false)]
        public double Width
        {
            get { return _maxX - _minX; }
        }

        [Browsable(false)]
        public double Height
        {
            get { return _maxY - _minY; }
        }

        public override string ToString()
        {
            return "{" + string.Format("MinLon:{0},MaxLon:{1},MinLat:{2},MaxLat:{3}",
                                           _minX.ToString("0.####"),
                                           _maxX.ToString("0.####"),
                                           _minY.ToString("0.####"),
                                           _maxY.ToString("0.####") + "}");
        }

        public static bool TryParse(string text, out Envelope envelope)
        {
            envelope = null;
            if (string.IsNullOrEmpty(text))
                return false;
            string exp = @"^{MinLon:(?<MinLon>[-]?\d+(\.\d+)?),MaxLon:(?<MaxLon>[-]?\d+(\.\d+)?),MinLat:(?<MinLat>[-]?\d+(\.\d+)?),MaxLat:(?<MaxLat>[-]?\d+(\.\d+)?)}$";
            Match m = Regex.Match(text, exp);
            if (!m.Success)
                return false;
            envelope = new Envelope(double.Parse(m.Groups["MinLon"].Value),
                                    double.Parse(m.Groups["MinLat"].Value),
                                    double.Parse(m.Groups["MaxLon"].Value),
                                    double.Parse(m.Groups["MaxLat"].Value));
            return true;
        }

        #region ICloneable 成员

        public object Clone()
        {
            return new Envelope(_minX, _minY, _maxX, _maxY);
        }

        #endregion

        /// <summary>
        /// 将该封套外扩delta个单位，返回扩大后的封套
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public Envelope Expand(float delta)
        {
            return new Envelope(_minX - delta, _minY - delta, _maxX + delta, _maxY + delta);
        }

        public bool IsEquals(Envelope evp)
        {
            if (evp == null)
                return false;
            return (Math.Abs(evp.MinX - MinX) < double.Epsilon) &&
                       (Math.Abs(evp.MinY - MinY) < double.Epsilon) &&
                       (Math.Abs(evp.MaxX - MaxX) < double.Epsilon) &&
                       (Math.Abs(evp.MaxY - MaxY) < double.Epsilon);
        }

        public bool IsGeoRange()
        {
            if (_minX < -180 ||
              _maxX > 180 ||
                _minY < -90 ||
                _maxY > 90)
                return false;
            return true;
        }

        public RectangleF ToRectangleF()
        {
            return RectangleF.FromLTRB((float)Math.Min(_minX, _maxX),
                                       (float)Math.Min(-_minY, -_maxY),//- is important
                                       (float)Math.Max(_minX, _maxX),
                                       (float)Math.Max(-_minY, -_maxY));
        }
    }

    [Serializable]
    public abstract class Shape : IDisposable, ICloneable, IOGCWktSupport
    {
        protected Envelope _envelope;
        protected bool _isProjected = false;
        protected ShapePoint _centroid = null;

        public Envelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }

        public virtual ShapePoint Centroid
        {
            get { return _centroid; }
            set { _centroid = value; }
        }

        public virtual void UpdateCentroid()
        {
        }

        public virtual void UpdateEnvelope()
        {
        }

        public bool IsProjected
        {
            get { return _isProjected; }
            internal set { _isProjected = value; }
        }

        public bool HitTest(Shape geometry, double tolerance)
        {
            if (geometry is ShapePoint)
                return HitTestByPoint(geometry as ShapePoint, tolerance);
            else if (geometry is ShapePolyline)
                return HitTestByLine(geometry as ShapePolyline, tolerance);
            else if (geometry is ShapePolygon)
                return HitTestByPolygon(geometry as ShapePolygon);
            else
                throw new NotSupportedException("点击测试暂不支持\"" + geometry.GetType().ToString() + "\"类型。");
        }

        private bool HitTestByPolygon(ShapePolygon shapePolygon)
        {
            if (this is ShapePoint)
                return shapePolygon.Contains(this as ShapePoint);
            else if (this is ShapePolyline)
                return shapePolygon.Contains(this as ShapePolyline);
            else if (this is ShapePolygon)
                return shapePolygon.Contains(this as ShapePolygon);
            else
                throw new NotSupportedException("暂不支持对\"" + this.GetType().ToString() + "\"进行点击测试。");
        }

        private bool HitTestByLine(ShapePolyline shapePolyline, double tolerance)
        {
            if (this is ShapePoint)
                return GeometryMathLib.GetDistance(shapePolyline, this as ShapePoint) < tolerance;
            else if (this is ShapePolyline)
                return GeometryMathLib.IsCrossed2Lines(this as ShapePolyline, shapePolyline);
            else if (this is ShapePolygon)
                return (this as ShapePolygon).Contains(shapePolyline);
            else
                throw new NotSupportedException("暂不支持对\"" + this.GetType().ToString() + "\"进行点击测试。");
        }

        private bool HitTestByPoint(ShapePoint shapePoint, double tolerance)
        {
            if (this is ShapePoint)
            {
                Envelope evp = Envelope.Clone() as Envelope;
                evp = evp.Expand((float)tolerance);
                return evp.Contains(shapePoint.Envelope);
            }
            else if (this is ShapePolyline)
            {
                return GeometryMathLib.GetDistance(this as ShapePolyline, shapePoint) < tolerance;
            }
            else if (this is ShapePolygon)
            {
                return (this as ShapePolygon).Contains(shapePoint);
            }
            else
            {
                throw new NotSupportedException("暂不支持对\"" + this.GetType().ToString() + "\"进行点击测试。");
            }
        }

        public virtual bool Contains(Shape geometry)
        {
            throw new NotSupportedException();
        }

        public virtual bool Whitin(Shape geometry)
        {
            return geometry.Contains(this);
        }

        public virtual void Project(IProjectionTransform projectionTransform)
        {
            //
        }

        public virtual int EstimateSize()
        {
            return 0;
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            _envelope = null;
        }

        #endregion

        #region ICloneable Members

        public virtual object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IOGCWktSupport Members

        public virtual string ToWkt()
        {
            return null;
        }

        #endregion

        public static Shape FromWKT(string wkt)
        {
            if (wkt == null)
                return null;
            if (wkt.Contains("MULTIPOINT") || wkt.Contains("POINT"))
                return ShapePoint.FromWKT(wkt);
            else if (wkt.Contains("MULTILINESTRING") || wkt.Contains("LINESTRING"))
                return ShapePolyline.FromWKT(wkt);
            else if (wkt.Contains("MULTIPOLYGON") || wkt.Contains("POLYGON"))
                return ShapePolygon.FromWKT(wkt);
            else
                throw new NotSupportedException("不支持的几何类型或者错误的OGC WKT表达式," + wkt + "。");
        }
    }

    [Serializable]
    public class ShapePoint : Shape
    {
        //经度
        protected double _x = 0;
        //纬度
        protected double _y = 0;
        //测试y=kx+b  =>y = preV + b
        //public PointF PrePointF = PointF.Empty;

        public ShapePoint()
        {
            _envelope = new Envelope();
        }

        public ShapePoint(double x, double y)
        {
            _x = x;
            _y = y;
            _envelope = new Envelope(_x, _y, _x, _y);
        }

        public override ShapePoint Centroid
        {
            get
            {
                if (_centroid == null)
                    _centroid = new ShapePoint(_x, _y);
                return base.Centroid;
            }
            set
            {
                base.Centroid = value;
            }
        }

        public override void UpdateCentroid()
        {
            if (_centroid == null)
                _centroid = new ShapePoint();
            _centroid._x = _x;
            _centroid._y = _y;
        }

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                _envelope = new Envelope(_x, _y, _x, _y);
                UpdateCentroid();
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                _envelope = new Envelope(_x, _y, _x, _y);
                UpdateCentroid();
            }
        }

        public PointF ToPointF()
        {
            return new PointF((float)X, (float)Y);
        }

        public override void Project(IProjectionTransform projectionTransform)
        {
            projectionTransform.Transform(this);
            _envelope = new Envelope(_x, _y, _x, _y);
            UpdateCentroid();
        }

        public override bool Contains(Shape geometry)
        {
            if (geometry is ShapePoint)
            {
                ShapePoint pt = geometry as ShapePoint;
                return Math.Abs(pt.X - X) < double.Epsilon && Math.Abs(pt.Y - Y) < double.Epsilon;
            }
            else if (geometry is ShapePolyline)
            {
                return false;
            }
            else if (geometry is ShapePolygon)
            {
                return false;
            }
            return false;
        }

        public override void UpdateEnvelope()
        {
            _envelope = new Envelope(_x, _y, _x, _y);
            UpdateCentroid();
        }

        public override object Clone()
        {
            return new ShapePoint(_x, _y);
        }

        public override int EstimateSize()
        {
            return 2 * 8;
        }

        public override string ToWkt()
        {
            return "POINT(" + string.Format("{0} {1}", X.ToString(), Y.ToString()) + ")";
        }

        public new static ShapePoint FromWKT(string wkt)
        {
            if (wkt == null)
                return null;
            wkt = wkt.Replace("POINT", string.Empty);
            wkt = wkt.Replace("(", string.Empty);
            wkt = wkt.Replace(")", string.Empty);
            wkt = wkt.Trim();
            string[] vs = wkt.Split(' ');
            return new ShapePoint(double.Parse(vs[0].Trim()), double.Parse(vs[1].Trim()));
        }

        internal RectangleF ToRectangle(double tolerance)
        {
            float y1 = (float)(-(_y - tolerance));
            float y2 = (float)(-(_y + tolerance));
            return RectangleF.FromLTRB((float)(_x - tolerance),
                                       Math.Min(y1, y2),
                                       (float)(_x + tolerance),
                                       Math.Max(y1, y2));
        }

        public override string ToString()
        {
            return "{X=" + X.ToString() + ",Y=" + Y.ToString() + "}";
        }
    }

    [Serializable]
    public class ShapeMultiPoint : Shape
    {
        private ShapePoint[] _points = null;

        public ShapeMultiPoint()
        {
            _envelope = new Envelope();
        }

        public ShapeMultiPoint(ShapePoint[] points)
        {
            _points = points;
            UpdateEnvelope();
        }

        private void UpdateEnvelope()
        {
            if (_points != null && _points.Length > 0)
            {
                _envelope = _points[0].Envelope.Clone() as Envelope;
                for (int i = 1; i < _points.Length; i++)
                    _envelope.UnionWith(_points[i].Envelope);
            }
        }

        public ShapePoint[] Points
        {
            get { return _points; }
        }

        public ShapePoint ToPoint()
        {
            return _points != null && _points.Length > 0 ? _points[0] : null;
        }

        public override void Project(IProjectionTransform projectionTransform)
        {
            if (_points == null || _points.Length == 0)
                return;
            projectionTransform.Transform(_points);
            UpdateEnvelope();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_points != null)
            {
                _points = null;
            }
        }

        public override object Clone()
        {
            if (_points == null || _points.Length == 0)
                return null;
            ShapePoint[] pts = new ShapePoint[_points.Length];
            for (int i = 0; i < pts.Length; i++)
                pts[i] = _points[i].Clone() as ShapePoint;
            return new ShapeMultiPoint(pts);
        }

        public override int EstimateSize()
        {
            return _points != null ? 16 * _points.Length : 0;
        }

        public override string ToWkt()
        {
            if (_points == null || _points.Length == 0)
                return null;
            string wkt = null;
            foreach (ShapePoint pt in _points)
            {
                wkt = wkt + pt.X.ToString() + " " + pt.Y.ToString() + ",";
            }
            wkt = wkt.Substring(0, wkt.Length - 1);
            return "MULTIPOINT(" + wkt + ")";
        }
    }

    [Serializable]
    public class ShapeLineString : Shape
    {
        protected ShapePoint[] _points = null;

        public ShapeLineString(ShapePoint[] points)
        {
            if (points == null || points.Length < 2)
                throw new Exception("ShapeLineString构造函数必须至少两个顶点！");
            _points = points;
            UpdateEnvelope();
            UpdateCentroid();
        }

        public override void UpdateCentroid()
        {
        }

        public override void UpdateEnvelope()
        {
            if (_points == null || _points.Length == 0)
                return;
            _envelope = _points[0].Envelope.Clone() as Envelope;
            for (int i = 1; i < _points.Length; i++)
                _envelope.UnionWith(_points[i].Envelope);
        }

        public ShapeLineString(ShapePoint[] points, Envelope envelope)
        {
            if (points == null || points.Length < 2)
                throw new Exception("ShapeLineString构造函数必须至少两个顶点！");
            _points = points;
            _envelope = envelope;
        }

        public ShapePoint[] Points
        {
            get { return _points; }
        }

        public double GetLength()
        {
            if (_points == null || _points.Length < 2)
                return 0;
            double len = 0;
            for (int i = 0; i < _points.Length - 1; i++)
            {
                len += (Math.Sqrt(
                                  (_points[i + 1].X - _points[i].X) * (_points[i + 1].X - _points[i].X) +
                                  (_points[i + 1].Y - _points[i].Y) * (_points[i + 1].Y - _points[i].Y)
                                 )
                       );
            }
            return len;
        }

        public override void Project(IProjectionTransform projectionTransform)
        {
            if (_points == null || _points.Length == 0)
                return;
            projectionTransform.Transform(_points);
            UpdateEnvelope();
            UpdateCentroid();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_points != null)
            {
                _points = null;
            }
        }

        public void SplitByLongitude(double splitLon, out ShapeLineString lLineString, out ShapeLineString rLineString)
        {
            lLineString = rLineString = null;
            List<ShapePoint> lPoints = new List<ShapePoint>();
            List<ShapePoint> rPoints = new List<ShapePoint>();
            foreach (ShapePoint pt in _points)
            {
                if (pt.X > splitLon)
                    rPoints.Add(pt);
                else
                    lPoints.Add(pt);
            }
            if (lPoints.Count > 2)
                lLineString = new ShapeLineString(lPoints.ToArray());
            if (rPoints.Count > 2)
                rLineString = new ShapeLineString(rPoints.ToArray());
        }

        public void SplitByLatitude(double splitLat, out ShapeLineString lLineString, out ShapeLineString rLineString)
        {
            lLineString = rLineString = null;
            List<ShapePoint> lPoints = new List<ShapePoint>();
            List<ShapePoint> rPoints = new List<ShapePoint>();
            foreach (ShapePoint pt in _points)
            {
                if (pt.Y > splitLat)
                    rPoints.Add(pt);
                else
                    lPoints.Add(pt);
            }
            if (lPoints.Count > 2)
                lLineString = new ShapeLineString(lPoints.ToArray());
            if (rPoints.Count > 2)
                rLineString = new ShapeLineString(rPoints.ToArray());
        }

        public override object Clone()
        {
            if (_points == null || _points.Length == 0)
                return null;
            ShapePoint[] pts = new ShapePoint[_points.Length];
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = _points[i].Clone() as ShapePoint;
            }
            return new ShapeLineString(pts);
        }

        public override int EstimateSize()
        {
            if (_points == null)
                return 0;
            int estimateSize = 0;
            foreach (ShapePoint pt in _points)
                estimateSize += pt.EstimateSize();
            return estimateSize;
        }

        public override string ToWkt()
        {
            if (_points == null)
                return null;
            string wkt = null;
            foreach (ShapePoint pt in _points)
            {
                wkt = wkt + pt.X.ToString() + " " + pt.Y.ToString() + ",";
            }
            return "LINESTRING(" + wkt + ")";
        }
    }

    [Serializable]
    public class ShapePolyline : Shape
    {
        protected ShapeLineString[] _parts = null;
        [NonSerialized]
        protected int _angleAtCentroid = 0;

        public ShapePolyline(ShapeLineString[] parts)
        {
            if (parts == null || parts.Length < 1)
                throw new Exception("ShapePolyline构造函数必须至少有一个part!");
            _parts = parts;
            UpdateEnvelope();
            UpdateCentroid();
        }

        public ShapePolyline(ShapeLineString[] parts, Envelope envelope)
        {
            if (parts == null || parts.Length < 1)
                throw new Exception("ShapePolyline构造函数必须至少有一个part!");
            _parts = parts;
            _envelope = envelope;
            UpdateCentroid();
        }

        public override void UpdateEnvelope()
        {
            if (_parts == null || _parts.Length == 0)
                return;
            _envelope = _parts[0].Envelope.Clone() as Envelope;
            if (_parts.Length > 1)
            {
                for (int i = 1; i < _parts.Length; i++)
                    _envelope.UnionWith(_parts[i].Envelope);
            }
        }

        public override void UpdateCentroid()
        {
            //try
            //{
            if (_parts == null || _parts.Length == 0)
                return;
            int n = 0;
            foreach (ShapeLineString p in _parts)
                n += p.Points.Length;
            int idx = n / 2;
            n = 0;
            foreach (ShapeLineString p in _parts)
            {
                n += p.Points.Length;
                if (n >= idx)
                {
                    _centroid = p.Points[n - idx];
                    break;
                }
            }
            //}
            //catch (Exception ex)
            //{ 
            //}
        }

        public ShapeLineString[] Parts
        {
            get { return _parts; }
        }

        public double GetLength()
        {
            double len = 0;
            foreach (ShapeLineString line in _parts)
                len += line.GetLength();
            return len;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_parts != null)
            {
                foreach (ShapeLineString part in _parts)
                {
                    part.Dispose();
                }
                _parts = null;
            }
        }

        public void SplitByLatitude(double splitLat, out ShapePolyline lPly, out ShapePolyline rPly)
        {
            lPly = rPly = null;
            List<ShapeLineString> lStrings = new List<ShapeLineString>();
            List<ShapeLineString> rStrings = new List<ShapeLineString>();
            foreach (ShapeLineString str in _parts)
            {
                ShapeLineString lString = null, rString = null;
                str.SplitByLatitude(splitLat, out lString, out rString);
                if (lString != null)
                    lStrings.Add(lString);
                if (rString != null)
                    rStrings.Add(rString);
            }
            if (lStrings.Count > 0)
                lPly = new ShapePolyline(lStrings.ToArray());
            if (rStrings.Count > 0)
                rPly = new ShapePolyline(rStrings.ToArray());
        }

        public void SplitByLongitude(double splitLon, out ShapePolyline lPly, out ShapePolyline rPly)
        {
            lPly = rPly = null;
            List<ShapeLineString> lStrings = new List<ShapeLineString>();
            List<ShapeLineString> rStrings = new List<ShapeLineString>();
            foreach (ShapeLineString str in _parts)
            {
                ShapeLineString lString = null, rString = null;
                str.SplitByLongitude(splitLon, out lString, out rString);
                if (lString != null)
                    lStrings.Add(lString);
                if (rString != null)
                    rStrings.Add(rString);
            }
            if (lStrings.Count > 0)
                lPly = new ShapePolyline(lStrings.ToArray());
            if (rStrings.Count > 0)
                rPly = new ShapePolyline(rStrings.ToArray());
        }

        public override void Project(IProjectionTransform projectionTransform)
        {
            if (_parts == null || _parts.Length == 0)
                return;
            foreach (ShapeLineString line in _parts)
                line.Project(projectionTransform);
            UpdateEnvelope();
            UpdateCentroid();
        }

        public override object Clone()
        {
            if (_parts == null || _parts.Length == 0)
                return null;
            ShapeLineString[] parts = new ShapeLineString[_parts.Length];
            for (int i = 0; i < parts.Length; i++)
                parts[i] = _parts[i].Clone() as ShapeLineString;
            return new ShapePolyline(parts);
        }

        public override bool Contains(Shape geometry)
        {
            if (geometry is ShapePoint)
            {
                return GeometryMathLib.GetDistance(this, geometry as ShapePoint) < double.Epsilon;
            }
            else if (geometry is ShapePolyline)
            {
                return false;
            }
            else if (geometry is ShapePolygon)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public override int EstimateSize()
        {
            if (_parts == null)
                return 0;
            int estimateSize = 0;
            foreach (ShapeLineString part in _parts)
                estimateSize += part.EstimateSize();
            return estimateSize;
        }

        public override string ToWkt()
        {
            if (_parts == null || _parts.Length == 0)
                return null;
            StringBuilder sb = new StringBuilder();
            sb.Append("MULTILINESTRING(");
            for (int i = 0; i < _parts.Length; i++)
            {
                ShapeLineString part = _parts[i];
                if (part.Points == null || part.Points.Length == 0)
                    continue;
                sb.Append("(");
                for (int j = 0; j < part.Points.Length; j++)
                {
                    ShapePoint pt = part.Points[j];
                    sb.Append(pt.X.ToString());
                    sb.Append(" ");
                    sb.Append(pt.Y.ToString());
                    if (j != part.Points.Length - 1)
                        sb.Append(",");
                }
                sb.Append(")");
                if (i != _parts.Length - 1)
                    sb.Append(",");
            }
            sb.Append(")");
            return sb.ToString();
        }

        public new static ShapePolyline FromWKT(string wkt)
        {
            if (string.IsNullOrEmpty(wkt))
            {
                return null;
            }
            wkt = wkt.Replace("MULTILINESTRING", string.Empty);
            wkt = wkt.Replace("LINESTRING", string.Empty);
            wkt = wkt.Trim();

            //wkt = wkt.Substring(1, wkt.Length - 2);

            string[] parts1 = wkt.Split(new string[] { ")" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts1 == null || parts1.Length == 0)
                return null;

            int count = parts1.Length;
            for (int index = 0; index < count; index++)
            {
                if (index != 0)
                {
                    parts1[index] = parts1[index].Trim();
                    parts1[index] = parts1[index].Substring(1, parts1[index].Length - 1);
                }
                parts1[index] = parts1[index].Replace(")", string.Empty);
                parts1[index] = parts1[index].Replace("(", string.Empty);
            }

            List<ShapeLineString> part = new List<ShapeLineString>(parts1.Length);
            foreach (string partstring in parts1)
            {
                string[] parts2 = partstring.Split(',');
                List<ShapePoint> pts = new List<ShapePoint>(parts2.Length);
                foreach (string partpt in parts2)
                {
                    if (string.IsNullOrEmpty(partpt))
                    {
                        continue;
                    }
                    string[] xy = partpt.Trim().Split(' ');
                    pts.Add(new ShapePoint(double.Parse(xy[0].Trim()), double.Parse(xy[1].Trim())));
                }
                part.Add(new ShapeLineString(pts.ToArray()));
            }
            return new ShapePolyline(part.ToArray());
        }
    }

    [Serializable]
    public class ShapeRing : ShapeLineString
    {
        public ShapeRing(ShapePoint[] points)
            : base(points)
        {
        }

        private bool DoubleIsEquals(double a, double b)
        {
            return Math.Abs(a - b) < double.Epsilon;
        }

        /// <summary>
        /// S =( -x0*y1 - x1*y2 - x2*y0
        ///      + x1*y0  + x2*y1  + x0*y2 ) /2;
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            if (_points == null || _points.Length < 3)
                return 0;
            int nTrigangle = _points.Length - 2;
            double s = 0;
            for (int i = 0; i < nTrigangle; i++)
            {
                ShapePoint pt0 = _points[0];
                ShapePoint pt1 = _points[i + 1];
                ShapePoint pt2 = _points[i + 2];
                s += (0.5d * (-pt0.X * pt1.Y - pt1.X * pt2.Y - pt2.X * pt0.Y +
                             pt1.X * pt0.Y + pt2.X * pt1.Y + pt0.X * pt2.Y)
                      );
            }
            if (s < 0)
                Console.WriteLine(s.ToString());
            return s;
        }

        public bool IsExterior()
        {
            return GetArea() >= 0;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void SplitByLongitude(double splitLon, out List<ShapeRing> lRings, out List<ShapeRing> rRings)
        {
            lRings = null;
            rRings = null;
            ShapeLineString leftLineString = null;
            ShapeLineString rightLineString = null;
            SplitByLongitude(splitLon, out leftLineString, out rightLineString);
            //
            if (leftLineString != null)
            {
                lRings = new List<ShapeRing>();
                if (leftLineString.Points.Length > 2)
                    lRings.Add(new ShapeRing(leftLineString.Points));
            }
            if (rightLineString != null)
            {
                rRings = new List<ShapeRing>();
                if (rightLineString.Points.Length > 2)
                    rRings.Add(new ShapeRing(rightLineString.Points));
            }
        }

        public void SplitByLatitude(double splitLat, out ShapeRing lRing, out ShapeRing rRing)
        {
            lRing = rRing = null;
            List<ShapePoint> lPoints = new List<ShapePoint>();
            List<ShapePoint> rPoints = new List<ShapePoint>();
            foreach (ShapePoint pt in _points)
            {
                if (pt.Y > splitLat)
                    rPoints.Add(pt);
                else
                    lPoints.Add(pt);
            }
            if (lPoints.Count > 2)
                lRing = new ShapeRing(lPoints.ToArray());
            if (rPoints.Count > 2)
                rRing = new ShapeRing(rPoints.ToArray());
        }

        public override object Clone()
        {
            if (_points == null || _points.Length == 0)
                return null;
            ShapePoint[] pts = new ShapePoint[_points.Length];
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = _points[i].Clone() as ShapePoint;
            }
            return new ShapeRing(pts);
        }
    }

    [Serializable]
    public class ShapePolygon : Shape
    {
        protected ShapeRing[] _rings = null;

        public ShapePolygon(ShapeRing[] rings)
        {
            if (rings == null || rings.Length < 1)
                throw new Exception("ShapePolygon构造函数必须至少有一个ring!");
            _rings = rings;
            UpdateEnvelope();
            UpdateCentroid();
        }

        public ShapePolygon(ShapeRing[] rings, Envelope envelope)
        {
            if (rings == null || rings.Length < 1)
                throw new Exception("ShapePolygon构造函数必须至少有一个ring!");
            _rings = rings;
            _envelope = envelope;
            UpdateCentroid();
        }

        public override void UpdateCentroid()
        {
            _centroid = GetBarycenterQualityAtVertex();
        }

        public override void UpdateEnvelope()
        {
            if (_rings == null || _rings.Length == 0)
                return;
            _envelope = _rings[0].Envelope.Clone() as Envelope;
            if (_rings.Length > 1)
                for (int i = 1; i < _rings.Length; i++)
                    _envelope.UnionWith(_rings[i].Envelope);
        }

        public ShapeRing[] Rings
        {
            get { return _rings; }
        }

        /// <summary>
        /// 计算多变形面积 = S(外环)+S(内环)
        /// 外环为顺时针方向，面积为+
        /// 内环(内洞)为逆时针方向，面积为-
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            if (_rings == null || _rings.Length == 0)
                return 0;
            double area = 0;
            foreach (ShapeRing ring in _rings)
                area += ring.GetArea();
            return area;
        }

        public double GetLength()
        {
            double len = 0;
            foreach (ShapeRing ring in _rings)
                len += ring.GetLength();
            return len;
        }

        /// 计算多边形重心，假定质量分布在顶点上
        /// </summary>
        /// <returns></returns>
        public ShapePoint GetBarycenterQualityAtVertex()
        {
            ShapePoint[] _points = _rings[0].Points;
            if (_points == null || _points.Length < 3)
                return null;
            double tx = 0, ty = 0;
            int n = _points.Length;
            for (int i = 0; i < n; i++)
            {
                tx += _points[i].X;
                ty += _points[i].Y;
            }
            return new ShapePoint(tx / n, ty / n);
        }

        /// <summary>
        /// 计算多边形重心,假定质量分布式均匀的
        /// </summary>
        /// <returns></returns>
        public ShapePoint GetBarycenterQualityIsUniformity()
        {
            return GetBarycenterQualityAtVertex();
        }

        /// <summary>
        /// 计算多边形重心，假定质量分布式不均匀的
        /// </summary>
        /// <returns></returns>
        public ShapePoint GetBarycenterQualityIsNotUniformity()
        {
            ShapePoint[] _points = _rings[0].Points;
            if (_points == null || _points.Length < 3)
                return null;
            int n = _points.Length;
            int triangleCount = n - 2;
            if (triangleCount < 1)
                return null;
            int i = 1;
            double[] S = new double[triangleCount];
            ShapePoint[] centers = new ShapePoint[triangleCount];
            while (i <= triangleCount)
            {
                ShapePoint p0 = _points[0];
                //ShapePoint p1 = _points[i + 1];
                //ShapePoint p2 = _points[i + 2];
                ShapePoint p1 = _points[i];
                ShapePoint p2 = _points[i + 1];
                //S =( x0*y1 + x1*y2 + x2*y0 - x1*y0  - x2*y1  - x0*y2 ) /2;
                S[i - 1] = 0.5d * (p0.X * p1.Y + p1.X * p2.Y + p2.X * p0.Y -
                                         p1.X * p0.Y - p2.X * p1.Y - p0.X * p2.Y);
                centers[i - 1] = new ShapePoint((p0.X + p1.X + p2.X) / 3f, (p0.Y + p1.Y + p2.Y) / 3f);
                //
                i++;
            }
            //X = ∑( xi×mi ) / ∑mi
            //Y = ∑( yi×mi ) / ∑mi
            double sumx = 0, sumy = 0, sumi = 0;
            for (int j = 0; j < S.Length; j++)
            {
                sumx += (centers[j].X * S[j]);
                sumy += (centers[j].Y * S[j]);
                sumi += S[j];
            }
            return new ShapePoint(sumx / sumi, sumy / sumi);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_rings != null)
            {
                foreach (ShapeRing ring in _rings)
                {
                    ring.Dispose();
                }
                _rings = null;
            }
        }

        public void SplitByLongitude(double splitLon, out ShapePolygon lPly, out ShapePolygon rPly)
        {
            lPly = rPly = null;
            List<ShapeRing> lRings = new List<ShapeRing>();
            List<ShapeRing> rRings = new List<ShapeRing>();
            foreach (ShapeRing ring in _rings)
            {
                List<ShapeRing> lRing = null, rRing = null;
                ring.SplitByLongitude(splitLon, out lRing, out rRing);
                if (lRing != null)
                    lRings.AddRange(lRing);
                if (rRing != null)
                    rRings.AddRange(rRing);
            }
            if (lRings.Count > 0)
                lPly = new ShapePolygon(lRings.ToArray());
            if (rRings.Count > 0)
                rPly = new ShapePolygon(rRings.ToArray());
        }

        public void SplitByLatitude(double splitLat, out ShapePolygon lPly, out ShapePolygon rPly)
        {
            lPly = rPly = null;
            List<ShapeRing> lRings = new List<ShapeRing>();
            List<ShapeRing> rRings = new List<ShapeRing>();
            foreach (ShapeRing ring in _rings)
            {
                ShapeRing lRing = null, rRing = null;
                ring.SplitByLatitude(splitLat, out lRing, out rRing);
                if (lRing != null)
                    lRings.Add(lRing);
                if (rRing != null)
                    rRings.Add(rRing);
            }
            if (lRings.Count > 0)
                lPly = new ShapePolygon(lRings.ToArray());
            if (rRings.Count > 0)
                rPly = new ShapePolygon(rRings.ToArray());
        }

        public override void Project(IProjectionTransform projectionTransform)
        {
            if (_rings == null || _rings.Length == 0)
                return;
            foreach (ShapeRing ring in _rings)
                ring.Project(projectionTransform);
            UpdateEnvelope();
            UpdateCentroid();
        }

        public override bool Contains(Shape geometry)
        {
            if (!Envelope.Contains(geometry.Envelope))
                return false;
            if (geometry is ShapePoint)
            {
                return GeometryMathLib.IsPointInPolygon(geometry as ShapePoint, this);
            }
            else if (geometry is ShapePolyline)
            {
                ShapePolyline line = geometry as ShapePolyline;
                foreach (ShapeLineString part in line.Parts)
                {
                    foreach (ShapePoint pt in part.Points)
                        if (!GeometryMathLib.IsPointInPolygon(pt, this))
                            return false;
                }
                return true;
            }
            else if (geometry is ShapePolygon)
            {
                ShapePolygon ply = geometry as ShapePolygon;
                foreach (ShapeRing ring in ply.Rings)
                {
                    foreach (ShapePoint pt in ring.Points)
                        if (!GeometryMathLib.IsPointInPolygon(pt, this))
                            return false;
                }
                return true;
            }
            return false;
        }

        public override object Clone()
        {
            if (_rings == null || _rings.Length == 0)
                return null;
            ShapeRing[] rings = new ShapeRing[_rings.Length];
            for (int i = 0; i < rings.Length; i++)
            {
                rings[i] = _rings[i].Clone() as ShapeRing;
            }
            return new ShapePolygon(rings);
        }

        public override int EstimateSize()
        {
            if (_rings == null)
                return 0;
            int estimateSize = 0;
            foreach (ShapeRing ring in _rings)
                estimateSize += ring.EstimateSize();
            return estimateSize;
        }

        public override string ToWkt()
        {
            if (_rings == null || _rings.Length == 0)
                return null;
            StringBuilder sb = new StringBuilder();
            sb.Append("POLYGON(");
            for (int i = 0; i < _rings.Length; i++)
            {
                ShapeRing ring = _rings[i];
                if (ring.Points == null || ring.Points.Length == 0)
                    continue;
                sb.Append("(");
                for (int j = 0; j < ring.Points.Length; j++)
                {
                    ShapePoint pt = ring.Points[j];
                    sb.Append(pt.X.ToString());
                    sb.Append(" ");
                    sb.Append(pt.Y.ToString());
                    if (j != ring.Points.Length - 1)
                        sb.Append(",");
                }
                sb.Append(")");
                if (i != _rings.Length - 1)
                    sb.Append(",");
            }
            sb.Append(")");
            return sb.ToString();
        }

        public new static ShapePolygon FromWKT(string wkt)
        {
            if (string.IsNullOrEmpty(wkt))
            {
                return null;
            }
            wkt = wkt.Replace("MULTIPOLYGON", string.Empty);
            wkt = wkt.Replace("POLYGON", string.Empty);
            wkt = wkt.Trim();

            string[] parts1 = wkt.Split(new string[] { ")" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts1 == null || parts1.Length == 0)
                return null;

            int count = parts1.Length;
            for (int index = 0; index < count; index++)
            {
                if (index != 0)
                {
                    parts1[index] = parts1[index].Trim();
                    parts1[index] = parts1[index].Substring(1, parts1[index].Length - 1);
                }
                parts1[index] = parts1[index].Replace(")", string.Empty);
                parts1[index] = parts1[index].Replace("(", string.Empty);
            }


            List<ShapeRing> rings = new List<ShapeRing>(parts1.Length);
            foreach (string partstring in parts1)
            {
                string[] parts2 = partstring.Split(',');
                List<ShapePoint> pts = new List<ShapePoint>(parts2.Length);
                foreach (string partpt in parts2)
                {
                    if (string.IsNullOrEmpty(partpt))
                    {
                        continue;
                    }
                    string[] xy = partpt.Trim().Split(' ');//bank
                    pts.Add(new ShapePoint(double.Parse(xy[0].Trim()), double.Parse(xy[1].Trim())));
                }
                rings.Add(new ShapeRing(pts.ToArray()));
            }
            return new ShapePolygon(rings.ToArray());
        }
    }
}
