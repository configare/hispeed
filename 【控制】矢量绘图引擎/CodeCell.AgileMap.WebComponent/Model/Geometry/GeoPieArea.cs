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
    public class GeoPieArea:GeoShape
    {
        protected GeoPoint _centerPoint = null;
        protected double _beginAngle = 0;
        protected double _endAngle = 0;
        protected double _radius = 0;

        public GeoPieArea()
            : base()
        { 
        }

        /// <summary>
        /// 中心点(椭圆中心)
        /// </summary>
        public GeoPoint CenterPoint
        {
            get { return _centerPoint; }
            set { _centerPoint = value; }
        }

        /// <summary>
        /// 开始角度(以X轴正方向为0度，顺时针为正)
        /// </summary>
        public double BeginAngle
        {
            get { return _beginAngle; }
            set { _beginAngle = value; }
        }

        /// <summary>
        /// 结束角度(以X轴正方向为0度，顺时针为正)
        /// </summary>
        public double EndAngle
        {
            get { return _endAngle; }
            set { _endAngle = value; }
        }

        /// <summary>
        /// 半径
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public override GeoPoint[] Points
        {
            get
            {
                return new GeoPoint[] { _centerPoint };
            }
        }

        public override double[] SingleValues
        {
            get
            {
                return new double[] { _radius };
            }
        }

        public override Geometry ToGeometry()
        {
            PathGeometry path = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = _centerPoint.ToPoint();
            //
            Point pt1, pt2;
            ComputeBeginEndPoints(out pt1, _beginAngle);
            ComputeBeginEndPoints(out pt2, _endAngle);
            //seg1 
            LineSegment seg1 = new LineSegment();
            seg1.Point = pt1;
            figure.Segments.Add(seg1);
            //seg2:arc
            ArcSegment seg2 = new ArcSegment();
            seg2.Size = new Size(_radius, _radius);
            seg2.SweepDirection = SweepDirection.Counterclockwise;
            seg2.Point = pt2;
            seg2.IsLargeArc = false;
            figure.Segments.Add(seg2);
            //seg3
            LineSegment seg3 = new LineSegment();
            seg3.Point = _centerPoint.ToPoint();
            figure.Segments.Add(seg3);
            //
            path.Figures.Add(figure);
            return path;
        }

        private void ComputeBeginEndPoints(out Point pt, double angle)
        {
            while (angle < 0)
                angle += 360;
             pt = new Point();
             pt.X = _centerPoint.X + Math.Cos(angle * Math.PI / 180) * _radius;
             pt.Y = _centerPoint.Y - Math.Sin(angle * Math.PI / 180) * _radius;
        }
    }
}
