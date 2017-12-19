using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class InteractiveSelectTool : Layer, IFlyLayer, IInteractiveSelectTool
    {
        private bool _visible = false;
        private ICanvas _canvas;
        private Point _bPoint;
        private Point _prePoint;
        private InteractivePointHandlerScreen _interactivePoinScreen;
        private InteractivePointHandlerPrj _interactivePoinGeo;
        private InteractiveRectHandlerScreen _interactiveRectScreen;
        private InteractiveRectHandlerPrj _interactiveRectGeo;

        public InteractiveSelectTool()
            : base()
        {
            _name = "交互选择基类";
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (drawArgs.Graphics == null)
                return;
            if (_bPoint.IsEmpty || _prePoint.IsEmpty)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            int x = Math.Min(_bPoint.X, _prePoint.X);
            int y = Math.Min(_bPoint.Y, _prePoint.Y);
            int w = Math.Abs(_bPoint.X - _prePoint.X);
            int h = Math.Abs(_bPoint.Y - _prePoint.Y);
            g.DrawRectangle(Pens.Red, x, y, w, h);
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (_canvas == null)
                _canvas = sender as ICanvas;
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (Control.MouseButtons == MouseButtons.Left)
                    {
                        _bPoint.X = e.ScreenX;
                        _bPoint.Y = e.ScreenY;
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_bPoint.IsEmpty)
                        return;
                    _prePoint.X = e.ScreenX;
                    _prePoint.Y = e.ScreenY;
                    _canvas.Refresh(enumRefreshType.FlyLayer);
                    break;
                case enumCanvasEventType.MouseUp:
                    HandAction(e);
                    _prePoint = Point.Empty;
                    _bPoint = Point.Empty;
                    break;
            }
        }

        private void HandAction(DrawingMouseEventArgs e)
        {
            if (_bPoint.IsEmpty && _prePoint.IsEmpty)
                return;
            if (!_bPoint.IsEmpty && _prePoint.IsEmpty)
                HandPointAction(e);
            else
                HandRectAction();
        }

        private void HandRectAction()
        {
            if (_interactiveRectScreen != null)
            {
                float x1 = Math.Min(_bPoint.X, _prePoint.X);
                float y1 = Math.Min(_bPoint.Y, _prePoint.Y);
                float x2 = Math.Max(_bPoint.X, _prePoint.X);
                float y2 = Math.Max(_bPoint.Y, _prePoint.Y);
                _interactiveRectScreen(this, new RectangleF(x1, y1, x2 - x1, y2 - y1));
            }
            if (_interactiveRectGeo != null)
                _interactiveRectGeo(this, GetSelectedGeoEnvelope());
        }

        private void HandPointAction(DrawingMouseEventArgs e)
        {
            if (_interactivePoinScreen != null)
                _interactivePoinScreen(this, new PointF(e.ScreenX, e.ScreenY));
            if (_interactivePoinGeo != null)
                _interactivePoinGeo(this, GetSelectedGeoPoint(_bPoint));
        }

        private CoordPoint GetSelectedGeoPoint(Point pt)
        {
            double x = pt.X;
            double y = pt.Y;
            _canvas.CoordTransform.QuickTransform.InverTransform(ref x, ref y);
            return new CoordPoint(x, y);
        }

        private CoordEnvelope GetSelectedGeoEnvelope()
        {
            if (_bPoint.IsEmpty || _prePoint.IsEmpty)
                return null;
            double x1 = Math.Min(_bPoint.X, _prePoint.X);
            double y1 = Math.Min(_bPoint.Y, _prePoint.Y);
            double x2 = Math.Max(_bPoint.X, _prePoint.X);
            double y2 = Math.Max(_bPoint.Y, _prePoint.Y);
            _canvas.CoordTransform.QuickTransform.InverTransform(ref x1, ref y1);
            _canvas.CoordTransform.QuickTransform.InverTransform(ref x2, ref y2);
            //有上行，也有下行轨道
            double minX, maxX, minY, maxY;
            minX = x1 > x2 ? x2 : x1;
            maxX = x1 > x2 ? x1 : x2;
            minY = y1 > y2 ? y2 : y1;
            maxY = y1 > y2 ? y1 : y2;
            return new CoordEnvelope(minX, maxX, minY, maxY);
        }

        public InteractivePointHandlerScreen InteractivePoinScreen
        {
            get { return _interactivePoinScreen; }
            set { _interactivePoinScreen = value; }
        }

        public InteractivePointHandlerPrj InteractivePoinPrj
        {
            get { return _interactivePoinGeo; }
            set { _interactivePoinGeo = value; }
        }

        public InteractiveRectHandlerScreen InteractiveRectScreen
        {
            get { return _interactiveRectScreen; }
            set { _interactiveRectScreen = value; }
        }

        public InteractiveRectHandlerPrj InteractiveRectPrj
        {
            get { return _interactiveRectGeo; }
            set { _interactiveRectGeo = value; }
        }
    }
}
