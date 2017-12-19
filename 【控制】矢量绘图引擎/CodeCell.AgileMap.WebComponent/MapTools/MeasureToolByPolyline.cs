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
    public class MeasureToolByPolyline:MeasureTool
    {
        private Path _path = null;
        private PathGeometry _pathGeometry = null;
        private PathFigure _figure = null;
        private LineSegment _segment = null;

        public MeasureToolByPolyline()
            : base()
        {
            _name = "距离";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/measureDistance.png";
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
            _canvas.Children.Add(_path);
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
            string disOfCrtSeg = "当前段长度:"+base.GetTipsOfCurrentPoint(pt);
            string disOfTotal = "总   长   度:"+GetTotalDistance().ToString("#,###.###")+"公里";
            return disOfCrtSeg + "\n" + disOfTotal;
        }

        private double GetTotalDistance()
        {
            if (_figure.Segments.Count == 0)
                return 0;
            double dis = 0;
            Point prePoint = _figure.StartPoint;
            foreach (LineSegment seg in _figure.Segments)
            {
                dis += GetDistance(seg.Point, prePoint);
                prePoint = seg.Point;
            }
            return dis;
        }
    }
}
