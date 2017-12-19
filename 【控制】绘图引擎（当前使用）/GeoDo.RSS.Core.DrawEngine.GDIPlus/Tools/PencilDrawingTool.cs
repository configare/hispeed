using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class PencilDrawingTool : Layer, IFlyLayer,IPencilDrawingTool
    {
        private bool _visible = false;
        private ICanvas _canvas;
        private Point _bPoint;
        private Point _prePoint;
        private AOI _aoi = new AOI();

        public PencilDrawingTool()
            : base()
        {
            _name = "感兴趣区域";
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
            int h =  Math.Abs(_bPoint.Y - _prePoint.Y);
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
                    SetAOI();
                    _prePoint = Point.Empty;
                    _bPoint = Point.Empty;
                    break;
            }
        }

        private void SetAOI()
        {
            if (_bPoint.IsEmpty || _prePoint.IsEmpty)
                return;
            double x1 = Math.Min(_bPoint.X, _prePoint.X);
            double y1 = Math.Min(_bPoint.Y, _prePoint.Y);
            double x2 = Math.Max(_bPoint.X, _prePoint.X);
            double y2 = Math.Max(_bPoint.Y, _prePoint.Y);
            _canvas.CoordTransform.QuickTransform.InverTransform(ref x1, ref y1);
            _canvas.CoordTransform.QuickTransform.InverTransform(ref x2, ref y2);

            double minX, maxX, minY, maxY;
            minX = x1 > x2 ? x2 : x1;
            maxX = x1 > x2 ? x1 : x2;
            minY = y1 > y2 ? y2 : y1;
            maxY = y1 > y2 ? y1 : y2;
            _aoi.PrjEnvelopes = new CoordEnvelope[] { 
                new CoordEnvelope(minX, maxX, minY, maxY) 
            };
            double geoX1, geoX2, geoY1, geoY2;
            _canvas.CoordTransform.Prj2Geo(x1, y1, out geoX1, out geoY1);
            _canvas.CoordTransform.Prj2Geo(x2, y2, out geoX2, out geoY2);
            //有升轨和降轨
            minX = geoX1 > geoX2 ? geoX2 : geoX1;
            maxX = geoX1 > geoX2 ? geoX1 : geoX2;
            minY = geoY1 > geoY2 ? geoY2 : geoY1;
            maxY = geoY1 > geoY2 ? geoY1 : geoY2;
            _aoi.Envelopes = new CoordEnvelope[] { 
                new CoordEnvelope(minX, maxX, minY, maxY) 
            };
        }

        public AOI GetAOI()
        {
            return _aoi;
        }
    }
}
