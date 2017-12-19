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
    public class GeoPolygon : GeoShape
    {
        private List<GeoRing> _rings = new List<GeoRing>();

        public GeoPolygon()
            : base()
        {
        }

        public GeoPolygon(GeoRing[] rings)
            : this()
        {
            if (rings != null)
                _rings.AddRange(rings);
        }

        public List<GeoRing> Lines
        {
            get { return _rings; }
        }

        public override GeoPoint[] Points
        {
            get
            {
                if (_rings == null || _rings.Count == 0)
                    return null;
                List<GeoPoint> retPoints = new List<GeoPoint>();
                foreach (GeoRing ring in _rings)
                {
                    retPoints.AddRange(ring.Points);
                }
                return retPoints.ToArray();
            }
        }

        public override Geometry ToGeometry()
        {
            PathGeometry geometry = new PathGeometry();
            foreach (GeoRing ring in _rings)
            {
                PathFigure pathFigure = new PathFigure();
                for (int i = 0; i < ring.Points.Length; i++)
                {
                    if (i == 0)
                        pathFigure.StartPoint = ring.Points[i].ToPoint();
                    else
                    {
                        LineSegment seg = new LineSegment();
                        seg.Point = ring.Points[i].ToPoint();
                        pathFigure.Segments.Add(seg);
                    }
                }
                pathFigure.IsClosed = true;
                geometry.Figures.Add(pathFigure);
            }
            return geometry;
        }
    }
}
