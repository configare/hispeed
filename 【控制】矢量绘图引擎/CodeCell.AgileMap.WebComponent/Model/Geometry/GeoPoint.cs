using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CodeCell.AgileMap.WebComponent
{
    public class GeoPoint:GeoShape
    {
        private double _x = 0;
        private double _y = 0;
        private double _radius = 1000;

        public GeoPoint()
        { 
        }

        public GeoPoint(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public GeoPoint(double x, double y,double radiusByMeters)
        {
            _x = x;
            _y = y;
            _radius = radiusByMeters;
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// 缺省的点符号是一个椭圆，需要指定一个显示半径
        /// </summary>
        public double RadiusByMeters
        {
            get { return _radius; }
            set 
            {
                _radius = value;
                if (_radius < 1)
                    _radius = 1;
            }
        }

        public override Geometry ToGeometry()
        {
            GeometryGroup gg = new GeometryGroup();
            EllipseGeometry eg = new EllipseGeometry();
            eg.Center = new Point(_x, _y);
            eg.RadiusX = _radius;
            eg.RadiusY = _radius;
            gg.Children.Add(eg);
            return gg;
        }

        public override GeoPoint[] Points
        {
            get
            {
                return new GeoPoint[] { this};
            }
        }

        public Point ToPoint()
        {
            return new Point(_x, _y);
        }
    }
}
