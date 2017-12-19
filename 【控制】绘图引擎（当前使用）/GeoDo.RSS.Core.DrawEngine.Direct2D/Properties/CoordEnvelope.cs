using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class CoordEnvelope
    {
        private double _minX = 0;
        private double _maxX = 0;
        private double _minY = 0;
        private double _maxY = 0;

        public CoordEnvelope()
        { }

        public CoordEnvelope(double minX, double maxX, double minY, double maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public CoordEnvelope(CoordPoint location, double width, double height)
        {
            _minX = location.X;
            _minY = location.Y;
            _maxX = _minX + width;
            _maxY = _minY + height;
        }

        public double MinX
        {
            get { return _minX; }
        }

        public double MaxX
        {
            get { return _maxX; }
        }

        public double MinY
        {
            get { return _minY; }
        }

        public double MaxY
        {
            get { return _maxY; }
        }

        public CoordPoint Location
        {
            get { return new CoordPoint(_minX, _minY); }
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

        public CoordPoint Center
        {
            get { return new CoordPoint((_minX + _maxX) / 2, (_minY + _maxY) / 2); }
        }

        public CoordPoint LeftBottom
        {
            get { return new CoordPoint(_minX,_minY); }
        }

        public CoordPoint RightUp
        {
            get { return new CoordPoint(_maxX, _maxY); }
        }

        public double Width
        {
            get { return _maxX - _minX; }
            set 
            {
                _maxX = value + _minX;
            }
        }

        public double Height
        {
            get { return _maxY - _minY; }
            set 
            {
                _maxY = value + _minY;
            }
        }

        public void Reset(double minX, double minY, double width, double height)
        {
            _minX = minX;
            _minY = minY;
            _maxX = _minX + width;
            _maxY = _minY + height;
        }

        public bool IsEmpty()
        {
            return !(Math.Abs(_minX) > double.Epsilon || Math.Abs(_minY) > double.Epsilon || Math.Abs(_maxX) > double.Epsilon || Math.Abs(_maxY) > double.Epsilon);
        }

        public void MoveTo(double centerX, double centerY)
        {
            double width = Width;
            double height = Height;
            _minX = centerX - width / 2;
            _maxX = centerX + width / 2;
            _minY = centerY - height / 2;
            _maxY = centerY + height / 2;
        }

        public void Translate(double offsetX, double offsetY)
        {
            _minX += offsetX;
            _maxX += offsetX;
            _minY += offsetY;
            _maxY += offsetY;
        }

        public void Inflate(double offsetX, double offsetY)
        {
            _minX -= offsetX / 2;
            _maxX += offsetX / 2;
            _minY -= offsetY / 2;
            _maxY += offsetY / 2;
        }

        public CoordEnvelope Intersect(CoordEnvelope a)
        {
            if (a.MaxX < _minX || a.MinX > _maxX || a.MinY > _maxY || a.MinY < _minY)
                return null;
            double minX = Math.Max(_minX, a.MinX);
            double minY = Math.Max(_minY, a.MinY);
            double maxX = Math.Min(_maxX, a.MaxX);
            double maxY = Math.Min(_maxY, a._maxY);
            return new CoordEnvelope(minX, maxX, minY, maxY);
        }

        public CoordEnvelope Union(CoordEnvelope a)
        {
            double minX = Math.Min(_minX, a.MinX);
            double minY = Math.Min(_minY, a.MinY);
            double maxX = Math.Max(_maxX, a.MaxX);
            double maxY = Math.Max(_maxY, a._maxY);
            return new CoordEnvelope(minX, maxX, minY, maxY);
        }

        public static CoordEnvelope FromLBWH(double left, double bottom, double width, double height)
        {
            return new CoordEnvelope(new CoordPoint(left, bottom), width, height);
        }

        public CoordEnvelope Clone()
        {
            return new CoordEnvelope(_minX, _maxX, _minY, _maxY);
        }

        public override string ToString()
        {
            return string.Concat("{", string.Format("X:{0},Y:{1},Width:{2},Height:{3}", _minX, _minY, (_maxX - _minX), (_maxY - _minY)), "}");
        }
    }
}
