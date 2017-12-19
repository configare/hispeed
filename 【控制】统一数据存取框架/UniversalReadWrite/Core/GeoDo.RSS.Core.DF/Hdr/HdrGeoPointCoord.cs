using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class HdrGeoPointCoord
    {
        public double Longitude = 0;
        public double Latitude = 0;
        public static HdrGeoPointCoord Empty = new HdrGeoPointCoord();

        public HdrGeoPointCoord()
        {

        }

        public HdrGeoPointCoord(double lon, double lat)
        {
            Longitude = lon;
            Latitude = lat;
        }

        public HdrGeoPointCoord(float lon, float lat)
        {
            Longitude = double.Parse(lon.ToString());
            Latitude = double.Parse(lat.ToString());
        }
    }
}
