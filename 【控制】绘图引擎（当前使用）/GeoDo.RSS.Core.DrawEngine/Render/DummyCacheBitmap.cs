using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class DummyCacheBitmap:CacheBitmap
    {
        protected CoordEnvelope _envelope = null;
        protected bool _isEnabled = true;
        protected bool _isValid = false;

        public DummyCacheBitmap(PixelFormat pixelFormat, Color fillColor, IDrawArgs drawArgs,InterpolationMode interpolationMode)
            : base(pixelFormat, fillColor, drawArgs,interpolationMode)
        { 
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public CoordEnvelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public void SetValid(bool isValid)
        {
            _isValid = isValid;
        }

        public override void Dispose()
        {
            _envelope = null;
            base.Dispose();
        }
    }
}
