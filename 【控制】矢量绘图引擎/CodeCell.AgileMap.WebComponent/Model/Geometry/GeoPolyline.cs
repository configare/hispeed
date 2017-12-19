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
    public class GeoPolyline:GeoShape
    {
        private List<GeoLineString> _lines = new List<GeoLineString>();

        public GeoPolyline()
            : base()
        { 
        }

        public GeoPolyline(GeoLineString[] lineStrings)
            : this()
        {
            if (lineStrings != null)
                _lines.AddRange(lineStrings);
        }

        public List<GeoLineString> Lines 
        {
            get { return _lines; }
        }

        public override GeoPoint[] Points
        {
            get
            {
                if (_lines == null || _lines.Count == 0)
                    return null;
                List<GeoPoint> retPoints = new List<GeoPoint>();
                foreach (GeoLineString gls in _lines)
                {
                    if (gls.Points != null)
                        retPoints.AddRange(gls.Points);
                }
                return retPoints.Count > 0 ? retPoints.ToArray() : null;
            }
        }

        public override Geometry ToGeometry()
        {
            PathGeometry geometry = new PathGeometry();
            foreach (GeoLineString part in _lines)
            {
                PathFigure pathFigure = new PathFigure();
                for (int i = 0; i < part.Points.Length; i++)
                {
                    if (i == 0)
                        pathFigure.StartPoint = part.Points[i].ToPoint();
                    else
                    {
                        LineSegment seg = new LineSegment();
                        seg.Point = part.Points[i].ToPoint();
                        pathFigure.Segments.Add(seg);
                    }
                }
                geometry.Figures.Add(pathFigure);
            }
            return geometry;
        }
    }
}
