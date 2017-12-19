using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class PanAdjustControlLayer : Layer, IControlLayer,IFlyLayer
    {
        protected CoordPoint _preLocation;
        protected ICanvas _canvas = null;
        public event EventHandler AdjustFinishedHandler;
        private static Font _font = new Font("微软雅黑", 10,FontStyle.Italic);

        public PanAdjustControlLayer()
            : base()
        {
            _name = "PanAdjust";
            _alias = "平移校正";
        }

        #region ICanvasEvent 成员

        public virtual void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (_canvas == null)
                _canvas = sender as ICanvas;
            double prjX,prjY;
            double geoX,geoY;
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (_preLocation == null && Control.MouseButtons == MouseButtons.Left)
                    {
                        _preLocation = new CoordPoint();
                        _canvas.CoordTransform.Screen2Prj(e.ScreenX, e.ScreenY, out prjX, out prjY);
                        _canvas.CoordTransform.Prj2Geo(prjX, prjY, out geoX, out geoY);
                        _preLocation.X = geoX;
                        _preLocation.Y = geoY;
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                        if (_preLocation == null)
                            return;
                        _canvas.CoordTransform.Screen2Prj(e.ScreenX, e.ScreenY, out prjX, out prjY);
                        _canvas.CoordTransform.Prj2Geo(prjX, prjY, out geoX, out geoY);                    
                        ApplyOffset(sender as ICanvas, geoX - _preLocation.X, geoY - _preLocation.Y);
                        _preLocation.X = geoX;
                        _preLocation.Y = geoY;
                    break;
                case enumCanvasEventType.MouseUp:
                    _preLocation = null;
                    geoX = geoY = prjX = prjY = 0;
                    if (AdjustFinishedHandler != null)
                        AdjustFinishedHandler(this, null);
                    break;
            }
        }

        protected virtual void ApplyOffset(ICanvas canvas, double offsetX, double offsetY)
        {
            IRasterDrawing drawing = _canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null)
                return;
            IGeoPanAdjust adjust = drawing as IGeoPanAdjust;
            if (adjust == null)
                return;
            adjust.ApplyAdjust(offsetX, offsetY);
            _canvas.Refresh(enumRefreshType.All);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            _canvas = null;
        }

        public bool Visible
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            //Graphics g = drawArgs.Graphics as Graphics;
            //g.DrawString("正在进行平移校正,按住鼠标左键开始平移,松开鼠标左键保存。",
            //    _font, Brushes.Red, 60, 8);
        }
    }
}
