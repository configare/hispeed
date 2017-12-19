using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class CacheBitmap : IDisposable
    {
        protected Bitmap _bitmap = null;
        protected PixelFormat _pixelFormat = PixelFormat.Format24bppRgb;
        protected bool _isClean = true;
        protected Color _fillColor;
        protected IDrawArgs _drawArgs = null;
        protected InterpolationMode _interpolationMode = InterpolationMode.NearestNeighbor;

        public CacheBitmap(PixelFormat pixelFormat, Color fillColor,IDrawArgs drawArgs,InterpolationMode interpolationMode)
        {
            _pixelFormat = pixelFormat;
            _fillColor = fillColor;
            _drawArgs = drawArgs;
            _interpolationMode = interpolationMode;
        }

        public IDrawArgs DrawArgs
        {
            get { return _drawArgs; }
        }

        public Bitmap Bitmap
        {
            get { return _bitmap; }
        }

        public Graphics BeginUpdate()
        {
            Graphics g = Graphics.FromImage(_bitmap);
            g.InterpolationMode = _interpolationMode;
            if (!_isClean)
                Clear(g);
            return g;
        }

        public void EndUpdate(Graphics g)
        {
            _isClean = false;
            if (g != null)
                g.Dispose();
        }

        public void Reset(int width, int height)
        {
            if (_bitmap == null)
            {
                _bitmap = new Bitmap(width, height, _pixelFormat);
            }
            else if (_bitmap.Width != width || _bitmap.Height != height)
            {
                _bitmap.Dispose();
                _bitmap = new Bitmap(width, height, _pixelFormat);
            }
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                Clear(g);
            }
        }

        public void Clear(Graphics g)
        {
            g.Clear(_fillColor);
        }

        public virtual void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            _drawArgs = null;
        }
    }
}
