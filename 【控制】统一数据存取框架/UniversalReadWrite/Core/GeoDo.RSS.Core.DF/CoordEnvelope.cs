using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics.Contracts;

namespace GeoDo.RSS.Core.DF
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
            get { return new CoordPoint(_minX, _minY); }
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

        public bool IsEmpty()
        {
            return !(Math.Abs(_minX) > double.Epsilon || Math.Abs(_minY) > double.Epsilon || Math.Abs(_maxX) > double.Epsilon || Math.Abs(_maxY) > double.Epsilon);
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

        public CoordEnvelope Intersect(CoordEnvelope b)
        {
            if (b.MaxX < _minX || b.MinX > _maxX || b.MinY > _maxY || b.MaxY < _minY)
                return null;
            double minX = Math.Max(_minX, b.MinX);
            double minY = Math.Max(_minY, b.MinY);
            double maxX = Math.Min(_maxX, b.MaxX);
            double maxY = Math.Min(_maxY, b._maxY);
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

        /// MAPINFO={Col,Row}:{X,Y}:{ResolutionX,ResolutionY}
        public string ToMapInfoString(Size size)
        {
            return "MAPINFO={1,1}:{" + _minX.ToString() + "," + _maxY.ToString() + "}:" +
                 "{" + (Width / (float)size.Width) + "," + (Height / (float)size.Height) + "}";
        }

        /// <summary>
        /// MAPINFO={Col,Row}:{X,Y}:{ResolutionX,ResolutionY}
        /// </summary>
        /// <param name="mapInfoString"></param>
        /// <returns></returns>
        public static CoordEnvelope FromMapInfoString(string mapInfoString, Size size)
        {
            HdrMapInfo mapInfo = ParseMapInfo(mapInfoString);
            float LonSolution = (float)mapInfo.XYResolution.Longitude;
            float LatSolution = (float)mapInfo.XYResolution.Latitude;
            float MinLon = (float)mapInfo.BaseMapCoordinateXY.Longitude - LonSolution * (mapInfo.BaseRowColNumber.X - 1);/*ENVI pixel from 1*/
            float MaxLat = (float)mapInfo.BaseMapCoordinateXY.Latitude + LatSolution * (mapInfo.BaseRowColNumber.Y - 1);
            float MaxLon = MinLon + LonSolution * size.Width;
            float MinLat = MaxLat - LatSolution * size.Height;
            return new CoordEnvelope(MinLon, MaxLon, MinLat, MaxLat);
        }

        private static HdrMapInfo ParseMapInfo(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            string[] parts = value.Split(':');
            if (parts.Length != 3)
                return null;
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Replace('{', ' ').Replace('}', ' ').Trim();
            string[] values = parts[0].Split(',');
            if (values.Length != 2)
                return null;
            HdrMapInfo mapInfo = new HdrMapInfo();
            mapInfo.BaseRowColNumber = new Point(int.Parse(values[0]), int.Parse(values[1]));
            values = parts[1].Split(',');
            if (values.Length != 2)
                return null;
            mapInfo.BaseMapCoordinateXY = new HdrGeoPointCoord(double.Parse(values[0]), double.Parse(values[1]));
            values = parts[2].Split(',');
            if (values.Length != 2)
                return null;
            mapInfo.XYResolution = new HdrGeoPointCoord(double.Parse(values[0]), double.Parse(values[1]));
            return mapInfo;
        }

        public override string ToString()
        {
            return string.Concat("{", string.Format("X:{0},Y:{1},Width:{2},Height:{3}", _minX, _minY, (_maxX - _minX), (_maxY - _minY)), "}");
        }
    }
}
