using System;
using System.Collections.Generic;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    public struct GeoEnvelop
    {
        public static GeoEnvelop Global = new GeoEnvelop(-Math.PI, Math.PI, -Math.PI / 2, Math.PI / 2);

        public double MinLon, MaxLon, MinLat, MaxLat;

        public GeoEnvelop(double minLon,
            double maxLon,
            double minLat,
            double maxLat)
        {
            MinLon = minLon;
            MaxLon = maxLon;
            MinLat = minLat;
            MaxLat = maxLat;
        }

        public double CenterLongitude
        {
            get { return (MaxLon + MinLon) / 2; }
        }
        public double CenterLatitude
        {
            get { return (MaxLat + MinLat) / 2; }
        }

        public double LongitudeSpan
        {
            get { return MaxLon - MinLon; }
        }
        public double LatitudeSpan
        {
            get { return MaxLat - MinLat; }
        }

        public void Normalize()
        {
            if (MinLon > MaxLon)
            {
                double temp = MinLon;
                MinLon = MaxLon;
                MaxLon = temp;
            }
            if (MinLat > MaxLat)
            {
                double temp = MinLat;
                MinLat = MaxLat;
                MaxLat = temp;
            }
        }

        public bool Contain(GeoCoord sv)
        {
            if (sv.Latitude >= MaxLat || sv.Latitude <= MinLat || sv.Longitude >= MaxLon ||
                sv.Longitude <= MinLon)
                return false;
            else
                return true;
        }

        public bool Intersect(GeoEnvelop r)
        {
            if (MaxLon <= r.MinLon ||
                MinLon >= r.MaxLon ||
                MaxLat <= r.MinLat ||
                MinLat >= r.MaxLat)
                return false;
            return true;
        }

        public bool Contain(GeoEnvelop r)
        {
            if (MaxLon >= r.MaxLon &&
                MinLon <= r.MinLon &&
                MaxLat >= r.MaxLat &&
                MinLat <= r.MinLat)
            {
                return true;
            }
            return false;
        }

    }
}
