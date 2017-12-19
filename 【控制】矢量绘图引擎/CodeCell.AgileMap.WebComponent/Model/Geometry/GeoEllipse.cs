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
    public class GeoEllipse:GeoShape
    {
        private GeoPoint _centerPoint = null;
        private Size _size = Size.Empty;

        public GeoEllipse()
            : base()
        { 
        }

        public GeoEllipse(GeoPoint centerPoint, Size size)
        {
            _centerPoint = centerPoint;
            _size = size;
        }

        public GeoPoint CenterPoint
        {
            get { return _centerPoint; }
            set { _centerPoint = value; }
        }

        public Size Size
        {
            get { return _size; }
            set { _size = value; }
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
                return new double[] { _size.Width,_size.Height };
            }
        }

        public override Geometry ToGeometry()
        {
            EllipseGeometry elp = new EllipseGeometry();
            elp.Center = new Point(_centerPoint.X, _centerPoint.Y);
            elp.RadiusX = _size.Width;
            elp.RadiusY = _size.Height;
            return elp;
        }
    }
}
