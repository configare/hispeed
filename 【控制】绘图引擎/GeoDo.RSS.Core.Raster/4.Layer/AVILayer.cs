using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class AVILayer : Layer, IAVILayer
    {
        protected bool _visible = true;
        private BitmapObject[] _bobjs = null;
        private CoordEnvelope _enp = null;
        private int _interval = 0;
        protected ImageAttributes _ia;
        protected ICanvas _canvas = null;
        private BitmapObject _currentBmp = null;
        private Timer _timer = null;
        private int _index = 0;
        private EventHandler _onTicked = null;
        private bool _isRunning = false;

        public AVILayer(BitmapObject[] bitmaps, int interval)
            : base()
        {
            _name = "动画层";
            _alias = "动画层";
            Init(bitmaps, interval);
        }

        public AVILayer(string name, string alias, BitmapObject[] bitmaps, int interval)
            : base()
        {
            _name = name;
            _alias = alias;
            Init(bitmaps, interval);
        }

        private void Init(BitmapObject[] bmps, int interval)
        {
            if (bmps == null || bmps.Length == 0)
                return;
            _bobjs = bmps;
            _currentBmp = bmps[0];
            _interval = interval;
            CreateImageAttributes();
        }

        private void CreateImageAttributes()
        {
            _ia = new ImageAttributes();
            ColorMap cm1 = new ColorMap();
            cm1.OldColor = Color.Black;
            cm1.NewColor = Color.Transparent;
            _ia.SetRemapTable(new ColorMap[] { cm1 }, ColorAdjustType.Bitmap);
        }

        [DisplayName("播放速度"), Category("动画设置")]
        public int Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                if (_timer == null)
                    return;
                _timer.Interval = value;
            }
        }

        [DisplayName("是否可见"),Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [Browsable(false)]
        public EventHandler OnTicked
        {
            get { return _onTicked; }
            set { _onTicked = value; }
        }

        [DisplayName("正在播放"),Category("状态")]
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                if (_timer == null)
                    InitTimer();
                if (_isRunning)
                    _timer.Enabled = true;
                else
                    _timer.Enabled = false;
            }
        }

        [DisplayName("动画帧数"),Category("基本信息")]
        public int BitmapCount
        {
            get { return _bobjs.Length; }
        }

        private void InitTimer()
        {
            _timer = new Timer();
            _timer.Interval = _interval;
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _currentBmp = _bobjs[_index];
            _index++;
            if (_index >= _bobjs.Length)
                _index = 0;
            if (_onTicked != null)
                _onTicked(sender, e);
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_canvas == null)
                _canvas = sender as ICanvas;
            if (_bobjs == null || _bobjs.Length == 0)
                return;
            int count = _bobjs.Length;
            if (_currentBmp == null)
                return;
            DrawBitmap(drawArgs);
        }

        private void DrawBitmap(IDrawArgs drawArgs)
        {
            if (_currentBmp == null)
                return;
            _enp = _currentBmp.Envelop;
            double x1 = _enp.MinX;
            double y1 = _enp.MinY;
            double x2 = _enp.MaxX;
            double y2 = _enp.MaxY;
            drawArgs.QuickTransformArgs.Transform(ref x1, ref y1);
            drawArgs.QuickTransformArgs.Transform(ref x2, ref y2);
            (drawArgs.Graphics as Graphics).DrawImage(_currentBmp.Bitmap, new PointF[] { new PointF((float)x1, (float)y2), new PointF((float)x2, (float)y2), new PointF((float)x1, (float)y1) },
            new RectangleF(0, 0, _currentBmp.Bitmap.Width, _currentBmp.Bitmap.Height), GraphicsUnit.Pixel, _ia);
        }

        public override void Dispose()
        {
            if (_bobjs != null)
            {
                for (int i = 0; i < _bobjs.Length; i++)
                {
                    if (_bobjs[i] != null)
                    {
                        _bobjs[i].Dispose();
                        _bobjs[i] = null;
                    }
                }
            }
            if (_ia != null)
            {
                _ia.Dispose();
                _ia = null;
            }
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            base.Dispose();
            _canvas = null;
        }
    }
}
