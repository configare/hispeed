using System;
using System.Collections.Generic;
using System.Text;

namespace GeoVis.GeoCore
{
    public struct GeoRegion
    {
        public static GeoRegion Global = new GeoRegion(-Math.PI, Math.PI, -Math.PI / 2, Math.PI / 2);

        public double MinLon, MaxLon, MinLat, MaxLat;

        public GeoRegion(double minLon,
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

        public bool Intersect(GeoRegion r)
        {
            if (MaxLon <= r.MinLon ||
                MinLon >= r.MaxLon ||
                MaxLat <= r.MinLat ||
                MinLat >= r.MaxLat)
                return false;
            return true;
        }

        public bool Contain(GeoRegion r)
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

    public struct Vector3D
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        //public Microsoft.DirectX.Vector3 Vector3
        //{
        //    get { return new Microsoft.DirectX.Vector3((float)X, (float)Y, (float)Z); }
        //}

        public void Add(Vector3D v)
        {
            X += v.X; Y += v.Y; Z += v.Z;
        }

        public void Normalize()
        {
            double l = Length;
            X = X / l;
            Y = Y / l;
            Z = Z / l;

        }
        public static double Dot(Vector3D v, Vector3D p)
        {
            return p.X * v.X + p.Y * v.Y + p.Z * v.Z;
        }

        public void Subtract(Vector3D v)
        {
            X -= v.X; Y -= v.Y; Z -= v.Z;
        }

        public void Scale(double scale)
        {
            X *= scale; Y *= scale; Z *= scale;
        }

        public double Length
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3D operator *(Vector3D P, double k)	// multiply by real 2
        {
            return new Vector3D(P.X * k, P.Y * k, P.Z * k);
        }

        public static Vector3D operator *(double k, Vector3D P)	// and its reverse order!
        {
            return new Vector3D(P.X * k, P.Y * k, P.Z * k);
        }

        public static Vector3D operator /(Vector3D P, double k)	// divide by real 2
        {
            return new Vector3D(P.X / k, P.Y / k, P.Z / k);
        }

        // other operators
        public double norm()	// L2 norm
        {
            return Math.Sqrt(norm2());
        }

        public double norm2() // squared L2 norm
        {
            return X * X + Y * Y + Z * Z;
        }

        public Vector3D normalize() // normalization
        {
            double n = norm();
            return new Vector3D(X / n, Y / n, Z / n);
        }


    }

    public struct GeoCoord
    {
        public static GeoCoord NaN = new GeoCoord(double.NaN, double.NaN, double.NaN);
        public double Longitude;
        public double Latitude;
        public double Radius;

        public GeoCoord(double lon, double lat, double r)
        {
            Longitude = lon;
            Latitude = lat;
            Radius = r;
        }

        public static bool operator !=(GeoCoord sv1, GeoCoord sv2)
        {
            return sv1.Longitude != sv2.Longitude || sv1.Latitude != sv2.Latitude || sv1.Radius != sv2.Radius;
        }

        public static bool operator ==(GeoCoord sv1, GeoCoord sv2)
        {
            return sv1.Longitude == sv2.Longitude && sv1.Latitude == sv2.Latitude && sv1.Radius == sv2.Radius;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsNaN
        {
            get { return double.IsNaN(Longitude) || double.IsNaN(Latitude) || double.IsNaN(Radius); }
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
