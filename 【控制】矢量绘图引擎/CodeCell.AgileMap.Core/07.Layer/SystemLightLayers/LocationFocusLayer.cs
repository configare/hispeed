using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    internal class LocationFocusLayer:ILocatingFocusLayer
    {
        private string _name = "定位焦点";
        private bool _enabled = true;
        private IMapRuntime _runtime = null;
        private ShapePoint _location = null;
        private bool _needRender = true;
        private bool _isShowBubble = true;
        private Image _bubbleImage = null;
        private bool _isFlash = false;
        private float _bubbleOffsetX = 19;
        private float _bubbleOffsetY = 63;
        private RectangleF _bubbleRect = new RectangleF();
        private string _labelString = null;
        private Font _font = new Font("微软雅黑", 12);

        public LocationFocusLayer()
        {
            _bubbleImage = GetDefaultBubbleImage();
        }

        public Bitmap GetDefaultBubbleImage()
        {
            Stream st = this.GetType().Assembly.GetManifestResourceStream("CodeCell.AgileMap.Core.red-pushpin.png");
            if (st == null)
                return new Bitmap(16, 16);
            return new Bitmap(st);
        }

        #region ILocatingFocusLayer 成员

        public RectangleF BubbleRect
        {
            get { return _bubbleRect; }
        }

        public float BubbleOffsetX
        {
            get { return _bubbleOffsetX; }
            set { _bubbleOffsetX = value; }
        }

        public float BubbleOffsetY
        {
            get { return _bubbleOffsetY; }
            set { _bubbleOffsetY = value; }
        }

        public Image BubbleImage
        {
            get { return _bubbleImage; }
            set
            {
                if (_bubbleImage != null)
                    _bubbleImage.Dispose();
                _bubbleImage = value; 
            }
        }

        public bool IsShowBubble
        {
            get { return _isShowBubble; }
            set { _isShowBubble = value; }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public void Focus(ShapePoint locationPrj, int times, int interval)
        {
            Focus(locationPrj, times, interval, null);
        }

        public void Focus(ShapePoint locationPrj, int times, int interval,string labelString)
        {
            if (times < 1 || locationPrj == null)
                return;
            _location = locationPrj;
            _labelString = labelString;
            _isFlash = true;
            try
            {
                for (int i = 0; i < times; i++)
                {
                    _needRender = i % 2 == 0;
                    _runtime.Host.RefreshContainer();
                    Thread.Sleep(interval);
                }
            }
            finally
            {
                _isFlash = false;
                _runtime.MapRefresh.Render();
            }
        }

        #endregion

        #region IHandinessLayer 成员

        public string Name
        {
            get { return _name; }
        }

        public void Init(IMapRuntime runtime)
        {
            _runtime = runtime;
        }

        public void Render(RenderArgs arg)
        {
            if (_location == null)
                return;
            PointF[] pts = new PointF[] { _location.ToPointF() };
            (_runtime as IFeatureRenderEnvironment).CoordinateTransform.PrjCoord2PixelCoord(pts);
            PointF pt = pts[0];
            if (_isShowBubble)
                DrawBubbleImage(arg, pt);
            if (!_isFlash)
                return;
            int halfw = 6;
            int w = 2 * halfw;
            if (_needRender)
            {
                SmoothingMode oldM = arg.Graphics.SmoothingMode;
                try
                {
                    using (Pen p = new Pen(Color.Gray, 2))
                    {
                        arg.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                        arg.Graphics.DrawLine(p, pt.X, 0, pt.X, _runtime.Host.CanvasSize.Height);
                        arg.Graphics.DrawLine(p, 0, pt.Y, _runtime.Host.CanvasSize.Width, pt.Y);
                        arg.Graphics.FillEllipse(Brushes.Green, (float)pt.X - halfw, (float)pt.Y - halfw, w, w);
                    }
                }
                finally 
                {
                    arg.Graphics.SmoothingMode = oldM;
                }
            }
        }
        
        private void DrawBubbleImage(RenderArgs arg, PointF pt)
        {
            if (_bubbleImage == null)
                return;
            float x = pt.X - _bubbleOffsetX;
            float y = pt.Y - _bubbleOffsetY;
            _bubbleRect = new RectangleF(x, y, _bubbleImage.Width, _bubbleImage.Height);
            arg.Graphics.DrawImage(_bubbleImage,x, y);
            if (!string.IsNullOrEmpty(_labelString))
            {
                SizeF fontsize = arg.Graphics.MeasureString(_labelString, _font);
                arg.Graphics.DrawString(_labelString, _font, Brushes.Red, x + _bubbleImage.Width, y + _bubbleImage.Height / 2 - fontsize.Height / 2);
            }
        }

        #endregion
    }
}
