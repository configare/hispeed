using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class PencilDrawedResult
    {
        public Size Size;
        public enumPencilType PencilType;
        public Point[] RasterPoints;
        public PointF[] GeoPoints;
    }
}
