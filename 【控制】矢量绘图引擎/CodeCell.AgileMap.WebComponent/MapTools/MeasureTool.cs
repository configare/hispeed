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
    public abstract class MeasureTool:MapTool
    {
        private List<Point> _points = new List<Point>();
        private bool _drawing = false;
        private TextBlock _currentTips = null;
        private Point _prePoint;

        public MeasureTool()
        { 
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(_canvas);
            if (!_drawing)
            {
                Start(pt);
                CreateCurrentTips();
                _drawing = true;
            }
            else
            {
                AddPoint(pt);
            }
            _prePoint = pt;
        }

        private void CreateCurrentTips()
        {
            Border b = new Border();
            b.CornerRadius = new CornerRadius(4);
            b.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 88, 89, 168));
            b.BorderThickness = new Thickness(2);
            b.Background = GetBackground();
            _currentTips = new TextBlock();
            _currentTips.FontWeight = FontWeights.Bold;
            _currentTips.Foreground = new SolidColorBrush(Colors.Black);
            b.Child = _currentTips;
            _canvas.Children.Add(b);
        }

        private Brush GetBackground()
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(1, 0);
            GradientStop stop = new GradientStop();
            stop.Color = Color.FromArgb(156,204,220,241);
            stop.Offset = 0;
            brush.GradientStops.Add(stop);
            stop = new GradientStop();
            stop.Color = Color.FromArgb(156, 8, 138, 254);
            brush.GradientStops.Add(stop);
            stop.Offset = 1;
            return brush;
        }

        /*
         *  <LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
                    <GradientStop Color="#DCE7F4" Offset="0.0" />
                    <GradientStop Color="#D2DFF0" Offset="0.3" />
                    <GradientStop Color="#C7D8ED" Offset="0.30"/>
                    <GradientStop Color="#D8E8F5" Offset="1" />
            </LinearGradientBrush>
         */

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (_drawing)
            {
                Point pt = e.GetPosition(_canvas);
                MoveTo(pt);
                _currentTips.Text = GetTipsOfCurrentPoint(pt);
                FrameworkElement sp = _currentTips.Parent as FrameworkElement;
                double x = pt.X + 16;
                double y = pt.Y;
                if (x + sp.ActualWidth > _canvas.ActualWidth)
                    x = x - (x + sp.ActualWidth - _canvas.ActualWidth);
                if (y + sp.ActualHeight > _canvas.ActualHeight)
                    y = y - (y + sp.ActualHeight - _canvas.ActualHeight);
                Canvas.SetLeft(sp, x);
                Canvas.SetTop(sp, y);
            }
        }

        protected virtual string GetTipsOfCurrentPoint(Point pt)
        {
            if (_prePoint.X < double.Epsilon || _prePoint.Y < double.Epsilon)
                return string.Empty;
            return GetDistance(_prePoint, pt).ToString("#,###.###") + "公里";
        }

        public double GetDistance(Point pt1, Point pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2)) * _mapcontrol.Resolution / 1000;
        }

        public override void MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            if (_drawing)
            {
                Stop(e.GetPosition(_canvas));
                _canvas.Children.Remove(_currentTips.Parent as UIElement);
                _drawing = false;
            }
        }
        protected abstract void MoveTo(Point point);
        protected abstract void Start(Point point);
        protected abstract void Stop(Point point);
        protected abstract void AddPoint(Point point);
    }
}
