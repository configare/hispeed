using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    public class GrstKey
    {
        private long _key;
        private string _path;

        public string Path
        {
            get { return _path; }
        }

        public long Key
        {
            get { return _key; }
        }

        public GrstKey(long k, string p)
        {
            _key = k;
            _path = p;
        }

        public static long CalKey(int level, int row, int col)
        {
            if (level == 0)
                return 0;
            long key = level;
            key <<= 28; key += row;
            key <<= 28; key += col;
            return key;

        }

        public static void CalRowCol(long key, ref int level, ref int row, ref int col)
        {
            col = (int)(key & (long)(0xFFFFFFF));
            key >>= 28;
            row = (int)(key & (long)(0xFFFFFFF));
            level = (byte)(key >> 28);
        }

        public static GeoEnvelop GetGeoRegion(int level, int row, int col)
        {
            double w = 1.0 / (1 << level);
            double h = w;
            double x = col * w;
            double y = row * h;
            GeoCoord sv1 = GetGeoPos(x, y);
            GeoCoord sv2 = GetGeoPos(x + w, y + h);

            return new GeoEnvelop(sv1.Longitude, sv2.Longitude, sv1.Latitude, sv2.Latitude);
        }

        private static GeoCoord GetGeoPos(double x, double y)
        {
            double lon = -180 + x * 180;
            double lat = -90 + y * 180;
            return new GeoCoord(lon, lat, 6378137);
        }
    }
}
