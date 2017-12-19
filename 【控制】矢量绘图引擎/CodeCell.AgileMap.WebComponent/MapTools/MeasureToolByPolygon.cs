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
    public class MeasureToolByPolygon:MeasureTool
    {
        private Path _path = null;
        private PathGeometry _pathGeometry = null;
        private PathFigure _figure = null;
        private LineSegment _segment = null;
        private LineSegment _lastSegment = null;
        private Point _startPoint;

        public MeasureToolByPolygon()
            : base()
        {
            _name = "面积";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/measureArea.png";
        }

        protected override void Start(Point point)
        {
            _pathGeometry = new PathGeometry();
            _figure = new PathFigure();
            _figure.StartPoint = point;
            _pathGeometry.Figures.Add(_figure);
            _segment = new LineSegment();
            _segment.Point = point;
            _figure.Segments.Add(_segment);
            _path = new Path();
            _path.Data = _pathGeometry;
            _path.Stroke = new SolidColorBrush(Colors.Red);
            _path.Fill = new SolidColorBrush(Color.FromArgb(40, 255, 0, 0));
            _canvas.Children.Add(_path);
            _startPoint = point;
        }

        protected override void MoveTo(Point point)
        {
            _segment.Point = point;
        }

        protected override void AddPoint(Point point)
        {
            _segment = new LineSegment();
            _segment.Point = point;
            _figure.Segments.Add(_segment);
            if (_lastSegment != null)
            {
                _figure.Segments.Remove(_lastSegment);
            }
            if (_figure.Segments.Count > 1)
            {
                _lastSegment = new LineSegment();
                _lastSegment.Point = _startPoint;
                _figure.Segments.Add(_lastSegment);
            }
        }

        protected override void Stop(Point point)
        {
            if (_path != null)
            {
                _canvas.Children.Remove(_path);
            }
        }

        protected override string GetTipsOfCurrentPoint(Point pt)
        {
            string disOfCrtSeg =  base.GetTipsOfCurrentPoint(pt);
            List<Point> pts = new List<Point>();
            pts = GetVertexes();
            double totalDis = GetTotalDistance(pts);
            double area = GetArea(pts) * Math.Pow(_mapcontrol.Resolution, 2) / Math.Pow(1000, 2);
            return "当前段长度:" + disOfCrtSeg + "\n" +
                   "多边形周长:" + totalDis.ToString("#,###.###") + "公里" + "\n" +
                   "多边形面积:" + area.ToString("#,###.###") + "平方公里";
        }

        private double GetArea(List<Point> pts)
        {
            if (pts == null || pts.Count < 3)
                return 0;
            int nTrigangle = pts.Count - 2;
            double s = 0;
            for (int i = 0; i < nTrigangle; i++)
            {
                Point pt0 = pts[0];
                Point pt1 = pts[i + 1];
                Point pt2 = pts[i + 2];
                s += (0.5d * (-pt0.X * pt1.Y - pt1.X * pt2.Y - pt2.X * pt0.Y +
                             pt1.X * pt0.Y + pt2.X * pt1.Y + pt0.X * pt2.Y)
                      );
            }
            return Math.Abs(s);
        }

        private double GetTotalDistance(List<Point> pts)
        {
            double dis = 0;
            for (int i = 0; i < pts.Count - 1; i++)
                dis += GetDistance(pts[i], pts[i + 1]);
            return dis;
        }

        private List<Point> GetVertexes()
        {
            List<Point> pts = new List<Point>();
            pts.Add(_figure.StartPoint);
            foreach (LineSegment seg in _figure.Segments)
                pts.Add(seg.Point);
            return pts;
        }
    }
}
