using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public class MemoryRasterBitmap
    {
        private Bitmap _bitmap;
        private CoordEnvelope _coordEnvelope;

        public MemoryRasterBitmap(Bitmap bitmap,CoordEnvelope coordEnvelope)
        {
            _bitmap = null;
            _coordEnvelope = coordEnvelope;
        }

        public Bitmap Bitmap
        {
            get { return _bitmap; }
        }

        public CoordEnvelope CoordEnvelope
        {
            get { return _coordEnvelope; }
        }
    } 
}
