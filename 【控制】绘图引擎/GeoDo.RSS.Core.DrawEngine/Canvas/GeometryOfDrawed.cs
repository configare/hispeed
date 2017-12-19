using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class GeometryOfDrawed
    {
        public PointF[] RasterPoints;
        public byte[] Types;
        public bool IsPrjCoord = false;
        public string ShapeType;
        public PointF[] ControlRasterPoints;
    }
}
