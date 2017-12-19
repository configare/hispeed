using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public abstract class ZoomControlLayerWithBox : DefaultControlLayer, IFlyLayer
    {
        public enum enumZoomType
        {
            ZoomIn,
            ZoomOut
        }

        protected bool _visible = true;
        protected PointF _startLocation = PointF.Empty;
        protected PointF _endLocation = PointF.Empty;
        protected enumZoomType _zoomType = enumZoomType.ZoomIn;

        public ZoomControlLayerWithBox()
            : base()
        {
            _name = "ZoomIn";
            _alias = "拉框放大";
            _enabledPan = false;
        }

        public enumZoomType ZoomType
        {
            get { return _zoomType; }
            set
            {
                if (_zoomType != value)
                {
                    _zoomType = value;
                    switch (_zoomType)
                    {
                        case enumZoomType.ZoomIn:
                            _name = "ZoomIn";
                            _alias = "拉框放大";
                            break;
                        case enumZoomType.ZoomOut:
                            _name = "ZoomOut";
                            _alias = "拉框缩小";
                            break;
                    }
                }
            }
        }

        #region IRenderLayer 成员

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public abstract void Render(object sender, IDrawArgs drawArgs);

        #endregion

        #region ICanvasEvent 成员

        public override void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            base.Event(sender, eventType, e);
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    _startLocation.X = e.ScreenX;
                    _startLocation.Y = e.ScreenY;
                    _canvas.DummyRenderModeSupport.SetToDummyRenderMode();
                    break;
                case enumCanvasEventType.MouseMove:
                    if (!_startLocation.IsEmpty)
                        HandleBox(sender as ICanvas, e);
                    break;
                case enumCanvasEventType.MouseUp:
                    if (!_startLocation.IsEmpty && !_endLocation.IsEmpty)
                    {
                        _endLocation.X = e.ScreenX;
                        _endLocation.Y = e.ScreenY;
                        HandleZoom(sender as ICanvas, e);
                        _startLocation = PointF.Empty;
                        _endLocation = PointF.Empty;
                        _canvas.DummyRenderModeSupport.SetToNomralRenderMode();
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
            }
        }

        private void HandleBox(ICanvas canvas, DrawingMouseEventArgs e)
        {
            _endLocation.X = e.ScreenX;
            _endLocation.Y = e.ScreenY;
            canvas.Refresh(enumRefreshType.FlyLayer);
        }

        private void HandleZoom(ICanvas canvas, DrawingMouseEventArgs e)
        {
            CoordEnvelope srcEnvelope = canvas.CurrentEnvelope;
            CoordEnvelope dstEnvelope = GetDstEnvelope(canvas);
            //by chennan 上海 修改大红叉
            if (dstEnvelope == null)
                return;
            //
            switch (_zoomType)
            {
                case enumZoomType.ZoomIn:
                    canvas.CurrentEnvelope = dstEnvelope;
                    break;
                case enumZoomType.ZoomOut:
                    double panzoomFactor = Math.Min(srcEnvelope.Width / dstEnvelope.Width, srcEnvelope.Height / dstEnvelope.Height);
                    double inflateWidth = srcEnvelope.Width * panzoomFactor;
                    double inflateHeight = srcEnvelope.Height * panzoomFactor;
                    srcEnvelope.Inflate(inflateWidth, inflateHeight);
                    canvas.CurrentEnvelope = srcEnvelope;
                    break;
            }
        }

        private CoordEnvelope GetDstEnvelope(ICanvas canvas)
        {
            double x1 = Math.Min(_startLocation.X, _endLocation.X);
            double y1 = Math.Min(_startLocation.Y, _endLocation.Y);
            double x2 = Math.Max(_startLocation.X, _endLocation.X);
            double y2 = Math.Max(_startLocation.Y, _endLocation.Y);
            //by chennan 上海 修改大红叉
            if (Math.Abs(x1 - x2) < 0.0001d && Math.Abs(y1 - y2)< 0.0001d)
                return null;
            //
            canvas.CoordTransform.QuickTransform.InverTransform(ref x1, ref y1);
            canvas.CoordTransform.QuickTransform.InverTransform(ref x2, ref y2);
            return new CoordEnvelope(x1, x2, y2, y1);
        }

        #endregion
    }
}
