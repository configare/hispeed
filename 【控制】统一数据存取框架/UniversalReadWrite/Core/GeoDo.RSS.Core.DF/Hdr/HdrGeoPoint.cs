using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public class HdrGeoPoint
    {
        public Point PixelPoint = Point.Empty;
        public HdrGeoPointCoord GeoPoint = HdrGeoPointCoord.Empty;
        public HdrGeoPoint()
        {

        }
        public HdrGeoPoint(Point pixelPoint, HdrGeoPointCoord geoPoint)
        {
            PixelPoint = pixelPoint;
            GeoPoint = geoPoint;
        }
    }
}
