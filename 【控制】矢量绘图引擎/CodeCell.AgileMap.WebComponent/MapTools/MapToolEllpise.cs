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
    public class MapToolEllpise : MapToolWheel
    {
        protected object _container = null;
        protected Ellipse _rect = new Ellipse();
        protected Point _startPoint = new Point();
        protected bool _moving = false;

        public MapToolEllpise()
            : base()
        {
            _rect.Visibility = Visibility.Collapsed;
            _name = "圆";
        }

        public override void Active()
        {
            base.Active();
            _canvas.Children.Add(_rect);
            _rect.Stroke = new SolidColorBrush(Color.FromArgb(0xff,0x6d,0xdb,0xd1));
            _rect.Fill = new SolidColorBrush(Color.FromArgb(0x80, 0xdc, 0xda, 0xe4));
        }

        public override void Deactive()
        {
            if(_canvas.Equals(_rect.Parent))
                _canvas.Children.Remove(_rect);
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_moving)
            {
                _rect.Visibility = Visibility.Visible;
                _moving = true;
                Point pt = e.GetPosition(_canvas);
                _startPoint = pt;
                Canvas.SetLeft(_rect, pt.X);
                Canvas.SetTop(_rect, pt.Y);
            }
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (!_moving)
                return;
            Point pt = e.GetPosition(_canvas);
            double x = Math.Min(_startPoint.X, pt.X);
            double y = Math.Min(_startPoint.Y, pt.Y);
            Canvas.SetLeft(_rect,x);
            Canvas.SetTop(_rect, y);
            double w = pt.X - _startPoint.X;
            double h = pt.Y - _startPoint.Y;
            _rect.Width = Math.Max(Math.Abs(w), Math.Abs(h));
            _rect.Height = _rect.Width;
        }

        public override void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(_moving)
            {
                HandleZoomAction();
                _rect.Visibility = Visibility.Collapsed;
                _rect.Width = 0;
                _rect.Height = 0;
                _moving = false;
            }
        }

        private void HandleZoomAction()
        {
            Point prj1 = _mapcontrol.Pixel2Prj(new Point(Canvas.GetLeft(_rect), Canvas.GetTop(_rect)));
            Point prj2 = _mapcontrol.Pixel2Prj(new Point(Canvas.GetLeft(_rect) + _rect.Width, Canvas.GetTop(_rect) + _rect.Height));
            HandleRectBox(prj1, prj2);
        }

        protected virtual void HandleRectBox(Point prjLeftUp, Point prjRightBottom)
        {
        }
    }
}
