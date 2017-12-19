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
    public class GeoRectangle:GeoShape
    {
        private GeoPoint _leftDownPoint = null;
        private Size _size = Size.Empty;

        public GeoRectangle()
            : base()
        { 
        }

        public GeoRectangle(GeoPoint leftDownPoint, Size size)
        {
            _leftDownPoint = leftDownPoint;
            _size = size;
        }

        public GeoPoint LeftDownPoint
        {
            get { return _leftDownPoint; }
            set { _leftDownPoint = value; }
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
                return new GeoPoint[] { _leftDownPoint };
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
            RectangleGeometry rect = new RectangleGeometry();
            rect.Rect = new Rect(_leftDownPoint.X, _leftDownPoint.Y,
                _size.Width, _size.Height);
            return rect;
        }
    }
}
