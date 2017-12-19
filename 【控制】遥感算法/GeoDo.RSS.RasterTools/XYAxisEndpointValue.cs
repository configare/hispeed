using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    public class XYAxisEndpointValue
    {
        public double MinX;
        public double MaxX;
        public double MinY;
        public double MaxY;

        public XYAxisEndpointValue()
        { }

        public XYAxisEndpointValue(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }
    }
}
