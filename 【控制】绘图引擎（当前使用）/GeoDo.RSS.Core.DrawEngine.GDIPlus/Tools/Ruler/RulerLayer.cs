using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class RulerLayer : Layer, IFlyLayer, IRenderLayer, IRulerLayer
    {
        protected bool _visible = true;
        protected int _rulerHeight = 28;
        protected int _1ScaleHeight = 20;
        protected int _2ScaleHeight = 12;
        protected int _3ScaleHeight = 6;
        private const int FONT_MARGIN = 8;
        protected int _1Span = 100;
        protected int _2Span = 50;
        protected int _3Span = 10;
        protected SolidBrush _barBrush = new SolidBrush(Color.FromArgb(64, 64, 64, 128));
        protected SolidBrush _labelBrush = new SolidBrush(Color.White);
        protected float _currentScreenX = 0, _currentScreenY = 0;
        protected Pen _focusLinePen = new Pen(Color.FromArgb(64, 64, 255), 1f);
        protected Font _font = new Font("微软雅黑", 8f);
        protected Matrix _matrix = new Matrix();
        protected bool _isShowCrossLines = false;
        protected bool _isFullCrossLines = false;
        protected bool _isDashCrossLines = false;
        protected Pen _scale1Pen = new Pen(Color.White);
        protected Pen _scale2Pen = new Pen(Color.White);
        protected Pen _scale3Pen = new Pen(Color.White);

        public RulerLayer()
        {
            _name = "坐标刻度尺";
        }

        [DisplayName("是否可见"), Category("显示")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [DisplayName("显示"), Category("十字线")]
        public bool IsShowCrossLines
        {
            get { return _isShowCrossLines; }
            set { _isShowCrossLines = value; }
        }

        [DisplayName("完全十字线"), Category("十字线")]
        public bool IsFullCrossLines
        {
            get { return _isFullCrossLines; }
            set { _isFullCrossLines = value; }
        }

        [DisplayName("显示为虚线"), Category("十字线")]
        public bool IsDashCrossLines
        {
            get { return _isDashCrossLines; }
            set 
            {
                if (_isDashCrossLines != value)
                {
                    _isDashCrossLines = value;
                    CrossLinesColor = this.CrossLinesColor;
                }
            }
        }

        [DisplayName("背景颜色"), Category("显示")]
        public Color BackColor
        {
            get { return _barBrush.Color; }
            set
            {
                if (_barBrush != null)
                    _barBrush.Dispose();
                _barBrush = new SolidBrush(value);
            }
        }

        [DisplayName("十字线颜色"), Category("十字线")]
        public Color CrossLinesColor
        {
            get { return _focusLinePen.Color; }
            set
            {
                if (_focusLinePen != null)
                    _focusLinePen.Dispose();
                _focusLinePen = new Pen(value);
                if (IsDashCrossLines)
                {
                    _focusLinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                    _focusLinePen.DashPattern = new float[] { 8, 8 };
                }
            }
        }

        [DisplayName("标注字体"), Category("标注")]
        public Font LabelFont
        {
            get { return _font; }
            set
            {
                if (_font != null)
                    _font.Dispose();
                _font = value;
            }
        }

        [DisplayName("标注颜色"), Category("标注")]
        public Color LabelColor
        {
            get { return _labelBrush.Color; }
            set
            {
                if (_labelBrush != null)
                    _labelBrush.Dispose();
                _labelBrush = new SolidBrush(value);
            }
        }

        [DisplayName("大刻度线颜色"), Category("刻度")]
        public Color Scale1Color
        {
            get { return _scale1Pen.Color; }
            set
            {
                if (_scale1Pen != null)
                    _scale1Pen.Dispose();
                _scale1Pen = new Pen(value);
            }
        }

        [DisplayName("中刻度线颜色"), Category("刻度")]
        public Color Scale2Color
        {
            get { return _scale2Pen.Color; }
            set
            {
                if (_scale2Pen != null)
                    _scale2Pen.Dispose();
                _scale2Pen = new Pen(value);
            }
        }

        [DisplayName("小刻度线颜色"), Category("刻度")]
        public Color Scale3Color
        {
            get { return _scale3Pen.Color; }
            set
            {
                if (_scale3Pen != null)
                    _scale3Pen.Dispose();
                _scale3Pen = new Pen(value);
            }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            ICanvas canvas = sender as ICanvas;
            if (canvas.PrimaryDrawObject == null || canvas.IsReverseDirection)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            IPrimaryDrawObject primaryObj = canvas.PrimaryDrawObject;
            int rasterWidth = primaryObj.Size.Width;
            int rasterHeight = primaryObj.Size.Height;
            DrawBarsAndRulers(rasterWidth, rasterHeight, g, canvas);
        }

        private void DrawBarsAndRulers(int rasterWidth, int rasterHeight, Graphics g, ICanvas canvas)
        {
            float screenX1, screenY1, screenX2, screenY2;
            canvas.CoordTransform.Raster2Screen(0, 0, out screenX1, out screenY1);
            canvas.CoordTransform.Raster2Screen(rasterHeight, rasterWidth, out screenX2, out screenY2);
            float pixelWidth = (screenX2 - screenX1);
            float pixelHeight = (screenY2 - screenY1);
            if (pixelWidth < 5 || pixelHeight < 5)
                return;
            //raster resolutions
            float rasterResX = rasterWidth / pixelWidth;
            float rasterResY = rasterHeight / pixelHeight;
            //top
            g.FillRectangle(_barBrush, screenX1, 0, pixelWidth, _rulerHeight);
            DrawHScales(g, screenX1, screenX2, _3ScaleHeight, _3Span, _scale3Pen);
            DrawHScales(g, screenX1, screenX2, _2ScaleHeight, _2Span, _scale2Pen);
            DrawHScales(g, screenX1, screenX2, _1ScaleHeight, _1Span, _scale1Pen);
            DrawHLabel1Scales(g, screenX1, screenX2, _1ScaleHeight, _1Span, _1Span * rasterResX);
            //left
            g.FillRectangle(_barBrush, 0, screenY1, _rulerHeight, pixelHeight);
            DrawVScales(g, screenY1, screenY2, _3ScaleHeight, _3Span, _scale3Pen);
            DrawVScales(g, screenY1, screenY2, _2ScaleHeight, _2Span, _scale2Pen);
            DrawVScales(g, screenY1, screenY2, _1ScaleHeight, _1Span, _scale1Pen);
            DrawVLabel1Scales(g, screenY1, screenY2, _1ScaleHeight, _1Span, _1Span * rasterResY);
            //draw focus lines
            if (_isShowCrossLines)
            {
                int w = canvas.Container.Width;
                int h = canvas.Container.Height;
                if (!_isFullCrossLines)
                {
                    w = h = _rulerHeight;
                }
                g.DrawLine(_focusLinePen, 0, _currentScreenY, w, _currentScreenY);
                g.DrawLine(_focusLinePen, _currentScreenX, 0, _currentScreenX, h);
            }
        }

        private void DrawHLabel1Scales(Graphics g, float beginX, float endX, int lineHeight, float span, float resX)
        {
            float v = 0;
            do
            {
                g.DrawString(((int)v).ToString(), _font, _labelBrush, beginX, FONT_MARGIN);
                v += resX;
                beginX += span;
            }
            while (beginX < endX);
        }


        private unsafe void DrawVLabel1Scales(Graphics g, float beginY, float endY, int lineHeight, float span, float resY)
        {
            Matrix oldMatrix = g.Transform;
            float v = 0;
            PointF pt = new PointF(3 * FONT_MARGIN, 0);
            GCHandle handle = GCHandle.Alloc(pt, GCHandleType.Pinned);
            PointF* ptr = (PointF*)handle.AddrOfPinnedObject();
            try
            {
                do
                {
                    ptr->Y = beginY + 1;
                    _matrix.Reset();
                    _matrix.RotateAt(90, *ptr);
                    g.Transform = _matrix;
                    g.DrawString(((int)v).ToString(), _font, _labelBrush, *ptr);
                    v += resY;
                    beginY += span;
                }
                while (beginY < endY);
            }
            finally
            {
                g.Transform = oldMatrix;
                handle.Free();
            }
        }

        private void DrawHScales(Graphics g, float beginX, float endX, int lineHeight, int span, Pen pen)
        {
            do
            {
                g.DrawLine(pen, beginX, 0, beginX, lineHeight);
                beginX += span;
            }
            while (beginX < endX);
        }

        private void DrawVScales(Graphics g, float beginY, float endY, int lineHeight, int span, Pen pen)
        {
            do
            {
                g.DrawLine(pen, 0, beginY, lineHeight, beginY);
                beginY += span;
            }
            while (beginY < endY);
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (!_isShowCrossLines)
                return;
            switch (eventType)
            {
                case enumCanvasEventType.MouseMove:
                    _currentScreenX = e.ScreenX;
                    _currentScreenY = e.ScreenY;
                    (sender as ICanvas).Refresh(enumRefreshType.All);
                    break;
            }
        }

        public override void Dispose()
        {
            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }
            if (_barBrush != null)
            {
                _barBrush.Dispose();
                _barBrush = null;
            }
            if (_focusLinePen != null)
            {
                _focusLinePen.Dispose();
                _focusLinePen = null;
            }
            if (_labelBrush != null)
            {
                _labelBrush.Dispose();
                _labelBrush = null;
            }
            if (_scale1Pen != null)
            {
                _scale1Pen.Dispose();
                _scale1Pen = null;
            }
            if (_scale2Pen != null)
            {
                _scale2Pen.Dispose();
                _scale2Pen = null;
            }
            if (_scale3Pen != null)
            {
                _scale3Pen.Dispose();
                _scale3Pen = null;
            }
            base.Dispose();
        }
    }
}
