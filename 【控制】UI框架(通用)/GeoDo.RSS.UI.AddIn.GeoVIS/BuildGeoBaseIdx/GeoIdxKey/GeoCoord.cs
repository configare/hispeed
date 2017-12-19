using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    public class GeoCoord
    {
        public static GeoCoord NaN = new GeoCoord(double.NaN, double.NaN, double.NaN);
        private double longitude;
        private double latitude;
        private double radius;

        public GeoCoord(double lon, double lat)
        {
            longitude = lon;
            latitude = lat;
            radius = 6378137.0;
        }

        public GeoCoord(double lon, double lat, double r)
        {
            Longitude = lon;
            Latitude = lat;
            radius = r;
        }

        #region IGeoCoord2D 成员

        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }

        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }

        #endregion

        #region IGeomCoord 成员

        public double X
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }

        public double Y
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }

        public double Z
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
            }
        }

        public int NumOrdinates
        {
            get { return 2; }
        }

        public GeoCoord Clone()
        {
            return new GeoCoord(longitude, latitude, radius);
        }

        public void Set(GeoCoord c)
        {
            X = c.X;
            Y = c.Y;
            Z = c.Z;
        }

        #endregion

        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public GeoCoord()
        {
            longitude = 0;
            latitude = 0;
        }

        public static bool operator !=(GeoCoord sv1, GeoCoord sv2)
        {
            return sv1.Longitude != sv2.Longitude || sv1.Latitude != sv2.Latitude || sv1.radius != sv2.radius;
        }

        public static bool operator ==(GeoCoord sv1, GeoCoord sv2)
        {
            return sv1.Longitude == sv2.Longitude && sv1.Latitude == sv2.Latitude && sv1.radius == sv2.radius;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsNaN
        {
            get { return double.IsNaN(Longitude) || double.IsNaN(Latitude) || double.IsNaN(radius); }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            string str = "  {0}   {1}  :";
            str = String.Format(str, this.Longitude, this.Latitude);
            return str;
        }
    }
}
