using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    public class ScanLinePolygon
    {
        private List<ScanLineRing> _rings = new List<ScanLineRing>();

        public ScanLinePolygon()
        {
        }

        public ScanLinePolygon(ScanLineRing[] rings)
        {
            if (rings == null || rings.Length == 0)
                return;
            _rings.AddRange(rings);
        }

        public ScanLineRing[] Rings
        {
            get { return _rings.Count > 0 ? _rings.ToArray() : null; }
        }

        public void AddRing(ScanLineRing ring)
        {
            if (ring != null)
                _rings.Add(ring);
        }
    }
}
