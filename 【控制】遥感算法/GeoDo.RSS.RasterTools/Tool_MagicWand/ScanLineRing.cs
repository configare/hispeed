using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    public class ScanLineRing
    {
        private List<int> _points = new List<int>();

        public ScanLineRing()
        {
        }

        public ScanLineRing(int[] points)
        {
            if (points != null && points.Length > 0)
                _points.AddRange(points);
        }

        public int[] Points
        {
            get { return _points.Count > 0 ? _points.ToArray() : null; }
        }

        public void AddPoint(int point)
        {
            _points.Add(point);
        }
    }
}
