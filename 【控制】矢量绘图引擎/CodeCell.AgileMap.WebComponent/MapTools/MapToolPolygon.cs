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
    public class MapToolPolygon : MapToolWheel
    {
        protected object _container = null;
        protected Path _polygon = new Path();
        protected PathFigure _figure = null;
        protected LineSegment _crtSegment = null;
        protected bool _moving = false;

        public MapToolPolygon()
            : base()
        {
            _polygon.Visibility = Visibility.Collapsed;
            _name = "多边形";
        }

        public override void Active()
        {
            base.Active();
            _canvas.Children.Add(_polygon);
            _polygon.Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0x6d, 0xdb, 0xd1));
            _polygon.Fill = new SolidColorBrush(Color.FromArgb(0x80, 0xdc, 0xda, 0xe4));
        }

        public override void Deactive()
        {
            if (_canvas.Equals(_polygon.Parent))
                _canvas.Children.Remove(_polygon);
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(_canvas);
            if (_crtSegment == null)
            {
                PathGeometry pth = new PathGeometry();
                _figure = new PathFigure();
                _figure.StartPoint = pt;
                _crtSegment = new LineSegment();
                _crtSegment.Point = pt;
                _figure.Segments.Add(_crtSegment);
                pth.Figures.Add(_figure);
                _polygon.Data = pth;
                _polygon.Visibility = Visibility.Visible;
                _moving = true;
            }
            else
            {
                _crtSegment = new LineSegment();
                _crtSegment.Point = pt;
                _figure.Segments.Add(_crtSegment);
            }
        }

        public override void MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            if (_moving)
            {
                _moving = false;
                _polygon.Visibility = Visibility.Collapsed;
                if (_polygon.Data != null && _figure.Segments.Count > 2)
                {
                    PathGeometry pth = _polygon.Data as PathGeometry;
                    GeoPolygon ply = GetGeoPolygon(pth);
                    pth.Figures.Clear();
                    _figure = null;
                    _crtSegment = null;
                    OnPolygonIsFinished(ply);
                }
            }
        }

        protected virtual void OnPolygonIsFinished(GeoPolygon ply)
        {
            
        }

        private GeoPolygon GetGeoPolygon(PathGeometry pth)
        {
            List<GeoPoint> pts = new List<GeoPoint>();
            PathFigure pf = pth.Figures[0];
            pts.Add(new GeoPoint(pf.StartPoint.X, pf.StartPoint.Y));
            foreach(LineSegment seg in pf.Segments)
                pts.Add(new GeoPoint(seg.Point.X, seg.Point.Y));
            foreach (GeoPoint pt in pts)
            {
                Point prjPt = _mapcontrol.Pixel2Prj(new Point(pt.X, pt.Y));
                pt.X = prjPt.X;
                pt.Y = prjPt.Y;
            }
            GeoRing ring = new GeoRing(pts.ToArray());
            return new GeoPolygon(new GeoRing[] { ring });
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (!_moving)
                return;
            Point pt = e.GetPosition(_canvas);
            _crtSegment.Point = pt;
        }
    }
}
