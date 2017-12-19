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
    public struct PrjRectangleF
    {
        private double _minX;
        private double _maxY;
        private double _maxX;
        private double _minY;
   
        /// <summary>
        /// Left
        /// </summary>
        public double MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }

        /// <summary>
        /// Top
        /// </summary>
        public double MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        public double MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        public double MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        public double Width
        {
            get { return Math.Abs(_maxX - _minX); }
        }

        public double Height
        {
            get { return Math.Abs(_maxY - _minY); }
        }

        public void Offset(double offsetX, double offsetY)
        {
            _minX += offsetX;
            _maxX += offsetX;
            _minY += offsetY;
            _maxY += offsetY;
        }

        public void Inflate(double inflateX, double inflateY)
        {
            _minX -= inflateX;
            _maxY += inflateY;
            _maxX += inflateX;
            _minY -= inflateY;
        }

        public static PrjRectangleF FromLTRB(double left, double top, double right, double bottom)
        {
            PrjRectangleF rect = new PrjRectangleF();
            rect.MinX = left;
            rect.MaxX = right;
            rect.MinY = bottom;
            rect.MaxY = top;
            return rect;
        }
    }
}
