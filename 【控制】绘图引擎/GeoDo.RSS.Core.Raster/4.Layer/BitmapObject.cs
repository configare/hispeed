using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class BitmapObject : IDisposable
    {
        private Bitmap _bitmap = null;
        private CoordEnvelope _evp = null;

        public BitmapObject(Bitmap bmp, CoordEnvelope prjCoordEnvelop)
        {
            _bitmap = bmp;
            _evp = prjCoordEnvelop;
        }

        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set { _bitmap = value; }
        }

        public CoordEnvelope Envelop
        {
            get { return _evp; }
            set { _evp = value; }
        }

        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }
    }
}
