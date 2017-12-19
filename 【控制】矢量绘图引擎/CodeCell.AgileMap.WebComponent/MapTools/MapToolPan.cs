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
    public class MapToolPan:MapToolWheel
    {
        private Point _startPoint = new Point();
        private Point _prePoint = new Point();
        private bool _ismoving = false;
     
        public MapToolPan()
            : base()
        {
            _name = "漫游";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/cmdPan.png";
        }

        public override void Active()
        {
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_ismoving)
            {
                _startPoint = e.GetPosition(_canvas);
                _prePoint = _startPoint;
                _ismoving = true;
                _canvas.Cursor = Cursors.Hand;
            }
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (_ismoving)
            {
                Point pt = e.GetPosition(_canvas);
                double offsetX = pt.X - _prePoint.X;
                double offsetY = pt.Y - _prePoint.Y;
                _mapcontrol.Offset(offsetX, offsetY);
                _prePoint = pt;
            }
        }

        public override void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_ismoving)
            {
                _canvas.Cursor = Cursors.Arrow;
                _ismoving = false;
                _mapcontrol.RefreshMap();
            }
        }
    }
}
