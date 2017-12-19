using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace GeoDo.RSS.RasterTools
{
    public class ContourLine:ICloneable
    {
        public class ContourEnvelope:ICloneable
        {
            private float _minX = 0;
            private float _maxX = 0;
            private float _minY = 0;
            private float _maxY = 0;

            public ContourEnvelope()
            { }

            public ContourEnvelope(float minX, float maxX, float minY, float maxY)
            {
                _minX = minX;
                _maxX = maxX;
                _minY = minY;
                _maxY = maxY;
            }

            public float MinX
            {
                get { return _minX; }
            }

            public float MaxX
            {
                get { return _maxX; }
            }

            public float MinY
            {
                get { return _minY; }
            }

            public float MaxY
            {
                get { return _maxY; }
            }

            public PointF Location
            {
                get { return new PointF(_minX, _minY); }
                set
                {
                    _minX = value.X;
                    _minY = value.Y;
                    if (_maxX <= _minX)
                        _maxX = _minX;
                    if (_maxY <= _minY)
                        _maxY = _minY;
                }
            }

            public void Reset(float minX, float maxX, float minY, float maxY)
            {
                _minX = minX;
                _maxX = maxX;
                _minY = minY;
                _maxY = maxY;
            }

            public bool IsEmpty()
            {
                return !(Math.Abs(_minX) > float.Epsilon || Math.Abs(_minY) > float.Epsilon || Math.Abs(_maxX) > float.Epsilon || Math.Abs(_maxY) > float.Epsilon);
            }

            public bool IsInteractived(ContourEnvelope envelope)
            {
                if (envelope.MaxX < _minX ||
                    envelope.MinX > _maxX ||
                    envelope.MinY > _maxY ||
                    envelope.MaxY < _minY)
                    return false;
                return true;
            }

            public bool IsInteractived(ContourEnvelope envelope, ref bool fullInternal)
            {
                if (envelope.MaxX < _minX || envelope.MinX > _maxX || envelope.MinY > _maxY || envelope._maxY < _minY)
                    return false;
                fullInternal = envelope.MinX > _minX && envelope._maxX < _maxX && envelope.MinY > _minY && envelope._maxY < _maxY;
                return true;
            }

            public ContourEnvelope Intersect(ContourEnvelope a)
            {
                if (a.MaxX < _minX || a.MinX > _maxX || a.MinY > _maxY || a.MinY < _minY)
                    return null;
                float minX = Math.Max(_minX, a.MinX);
                float minY = Math.Max(_minY, a.MinY);
                float maxX = Math.Min(_maxX, a.MaxX);
                float maxY = Math.Min(_maxY, a.MaxY);
                return new ContourEnvelope(minX, maxX, minY, maxY);
            }

            public float Width
            {
                get { return _maxX - _minX; }
            }

            public float Height
            {
                get { return _maxY - _minY; }
            }

            public override string ToString()
            {
                return string.Concat("{", string.Format("X:{0},Y:{1},Width:{2},Height:{3}", _minX, _minY, (_maxX - _minX), (_maxY - _minY)), "}");
            }

            public object Clone()
            {
                return new ContourEnvelope(_minX, _maxX, _minY, _maxY);
            }
        }

        private static int LAST_ID = -1;
        private static object _lockObj = new object();
        private int _id = 0;
        private double _contourValue;
        private List<PointF> _points = null;
        private int _count = 0;
        private PointF[] _pointsArray;
        private ContourEnvelope _envelope = null;
        private int _classIndex;

        public ContourLine()
        {
            lock (_lockObj)
            {
                Interlocked.Increment(ref LAST_ID);
                _id = LAST_ID;
            }
            _points = new List<PointF>();
        }

        public ContourLine(double contourValue)
            : this()
        {
            _contourValue = contourValue;
        }

        public int Id
        {
            get { return _id; }
        }

        public double ContourValue
        {
            get { return _contourValue; }
            set { _contourValue = value; }
        }

        public int ClassIndex
        {
            get { return _classIndex; }
            set { _classIndex = value; }
        }

        public int Count
        {
            get { return _count; }
        }

        public PointF[] Points
        {
            get
            {
                if (_pointsArray == null)
                {
                    _pointsArray = _points.Count > 0 ? _points.ToArray() : null;
                    _points = null;
                }
                return _pointsArray;
            }
        }

        public ContourEnvelope Envelope
        {
            get 
            {
                if (_envelope == null)
                    UpdateEnvelope();
                return _envelope; 
            }
        }

        public void UpdateEnvelope()
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            unsafe
            {
                fixed (PointF* ptr0 = this.Points)
                {
                    PointF* ptr = ptr0;
                    for (int i = 0; i < this.Count; i++,ptr++)
                    {
                        if (ptr->X < minX)
                            minX = ptr->X;
                        if (ptr->X > maxX)
                            maxX = ptr->X;
                        if (ptr->Y < minY)
                            minY = ptr->Y;
                        if (ptr->Y > maxY)
                            maxY = ptr->Y;
                    }
                }
            }
            _envelope = new ContourEnvelope(minX, maxX, minY, maxY);
        }

        public void AddPoint(PointF pt)
        {
            _points.Add(pt);
            _count++;
        }

        public void AddPoints(PointF[] pts)
        {
            if (pts == null || pts.Length == 0)
                return;
            _points.AddRange(pts);
            _count += pts.Length;
        }

        public object Clone()
        {
            ContourLine cntLine = new ContourLine(_contourValue);
            cntLine._classIndex = _classIndex;
            cntLine._count = _count;
            cntLine._envelope = this.Envelope.Clone() as ContourEnvelope;
            cntLine._pointsArray = this.Points.Clone() as PointF[];
            return cntLine;
        }
    }
}
