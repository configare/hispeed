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
using System.Collections.Generic;

namespace CodeCell.AgileMap.WebComponent
{
    public class GeoLineString:GeoShape
    {
        private List<GeoPoint> _points = new List<GeoPoint>();

        public GeoLineString()
            : base()
        { 
        }

        public GeoLineString(GeoPoint[] points)
            : this()
        {
            if (points != null && points.Length > 0)
                _points.AddRange(points);
        }

        public override GeoPoint[] Points
        {
            get
            {
                return _points.ToArray();
            }
        }

        public override Geometry ToGeometry()
        {
            if (_points == null || _points.Count == 0)
                return null;
            GeometryGroup gg = new GeometryGroup();
            for (int i = 0; i < _points.Count - 1; i++)
            {
                LineGeometry lg = new LineGeometry();
                lg.StartPoint = new Point(_points[i].X, _points[i].Y);
                gg.Children.Add(lg);
            }
            return gg;
        }
    }
}
