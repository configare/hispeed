using System;
using System.Collections.Generic;

namespace GeoVis.GeoCore
{
    
    public struct Quaternion4D
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Quaternion4D(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override int GetHashCode()
        {
            return (int)(X / Y / Z / W);
        }

        public override bool Equals(object obj)
        {
            if (obj is Quaternion4D)
            {
                Quaternion4D q = (Quaternion4D)obj;
                return q == this;
            }
            else
                return false;
        }

        public static Quaternion4D EulerToQuaternion(double yaw, double pitch, double roll)
        {
            double cy = Math.Cos(yaw * 0.5);
            double cp = Math.Cos(pitch * 0.5);
            double cr = Math.Cos(roll * 0.5);
            double sy = Math.Sin(yaw * 0.5);
            double sp = Math.Sin(pitch * 0.5);
            double sr = Math.Sin(roll * 0.5);

            double qw = cy * cp * cr + sy * sp * sr;
            double qx = sy * cp * cr - cy * sp * sr;
            double qy = cy * sp * cr + sy * cp * sr;
            double qz = cy * cp * sr - sy * sp * cr;

            return new Quaternion4D(qx, qy, qz, qw);
        }

        /// <summary>
        /// Transforms a rotation in quaternion form to a set of Euler angles 
        /// </summary>
        /// <returns>The rotation transformed to Euler angles, X=Yaw, Y=Pitch, Z=Roll (radians)</returns>
        public static Vector3D QuaternionToEuler(Quaternion4D q)
        {
            double q0 = q.W;
            double q1 = q.X;
            double q2 = q.Y;
            double q3 = q.Z;

            double x = Math.Atan2(2 * (q2 * q3 + q0 * q1), (q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3));
            double y = Math.Asin(-2 * (q1 * q3 - q0 * q2));
            double z = Math.Atan2(2 * (q1 * q2 + q0 * q3), (q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3));

            return new Vector3D(x, y, z);
        }

        public static Quaternion4D operator +(Quaternion4D a, Quaternion4D b)
        {
            return new Quaternion4D(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Quaternion4D operator -(Quaternion4D a, Quaternion4D b)
        {
            return new Quaternion4D(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Quaternion4D operator *(Quaternion4D a, Quaternion4D b)
        {
            return new Quaternion4D(
                    a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                    a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z,
                    a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X,
                    a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
        }

        public static Quaternion4D operator *(double s, Quaternion4D q)
        {
            return new Quaternion4D(s * q.X, s * q.Y, s * q.Z, s * q.W);
        }

        public static Quaternion4D operator *(Quaternion4D q, double s)
        {
            return new Quaternion4D(s * q.X, s * q.Y, s * q.Z, s * q.W);
        }

        // equivalent to multiplying by the quaternion (0, v)
        public static Quaternion4D operator *(Vector3D v, Quaternion4D q)
        {
            return new Quaternion4D(
                     v.X * q.W + v.Y * q.Z - v.Z * q.Y,
                     v.Y * q.W + v.Z * q.X - v.X * q.Z,
                     v.Z * q.W + v.X * q.Y - v.Y * q.X,
                    -v.X * q.X - v.Y * q.Y - v.Z * q.Z);
        }

        public static Quaternion4D operator /(Quaternion4D q, double s)
        {
            return q * (1 / s);
        }

        // conjugate operator
        public Quaternion4D Conjugate()
        {
            return new Quaternion4D(-X, -Y, -Z, W);
        }

        public static double Norm(Quaternion4D q)
        {
            return q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W;
        }

        public static double Abs(Quaternion4D q)
        {
            return Math.Sqrt(Norm(q));
        }

        public static Quaternion4D operator /(Quaternion4D a, Quaternion4D b)
        {
            return a * (b.Conjugate() / Abs(b));
        }

        public static bool operator ==(Quaternion4D a, Quaternion4D b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        public static bool operator !=(Quaternion4D a, Quaternion4D b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        public static double Dot(Quaternion4D a, Quaternion4D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        public void Normalize()
        {
            double L = Length();

            X = X / L;
            Y = Y / L;
            Z = Z / L;
            W = W / L;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y +
                Z * Z + W * W);
        }

        public static Quaternion4D Slerp(Quaternion4D q0, Quaternion4D q1, double t)
        {
            double cosom = q0.X * q1.X + q0.Y * q1.Y + q0.Z * q1.Z + q0.W * q1.W;
            double tmp0, tmp1, tmp2, tmp3;
            if (cosom < 0.0)
            {
                cosom = -cosom;
                tmp0 = -q1.X;
                tmp1 = -q1.Y;
                tmp2 = -q1.Z;
                tmp3 = -q1.W;
            }
            else
            {
                tmp0 = q1.X;
                tmp1 = q1.Y;
                tmp2 = q1.Z;
                tmp3 = q1.W;
            }

            /* calc coeffs */
            double scale0, scale1;

            if ((1.0 - cosom) > double.Epsilon)
            {
                // standard case (slerp)
                double omega = Math.Acos(cosom);
                double sinom = Math.Sin(omega);
                scale0 = Math.Sin((1.0 - t) * omega) / sinom;
                scale1 = Math.Sin(t * omega) / sinom;
            }
            else
            {
                /* just lerp */
                scale0 = 1.0 - t;
                scale1 = t;
            }

            Quaternion4D q = new Quaternion4D();

            q.X = scale0 * q0.X + scale1 * tmp0;
            q.Y = scale0 * q0.Y + scale1 * tmp1;
            q.Z = scale0 * q0.Z + scale1 * tmp2;
            q.W = scale0 * q0.W + scale1 * tmp3;

            return q;
        }

        public Quaternion4D Ln()
        {
            return Ln(this);
        }

        public static Quaternion4D Ln(Quaternion4D q)
        {
            double t = 0;

            double s = Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z);
            double om = Math.Atan2(s, q.W);

            if (Math.Abs(s) < double.Epsilon)
                t = 0.0f;
            else
                t = om / s;

            q.X = q.X * t;
            q.Y = q.Y * t;
            q.Z = q.Z * t;
            q.W = 0.0f;

            return q;
        }

        //the below functions have not been certified to work properly
        public static Quaternion4D Exp(Quaternion4D q)
        {
            double sinom;
            double om = Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z);

            if (Math.Abs(om) < double.Epsilon)
                sinom = 1.0;
            else
                sinom = Math.Sin(om) / om;

            q.X = q.X * sinom;
            q.Y = q.Y * sinom;
            q.Z = q.Z * sinom;
            q.W = Math.Cos(om);

            return q;
        }

        public Quaternion4D Exp()
        {
            return Ln(this);
        }

        public static Quaternion4D Squad(
            Quaternion4D q1,
            Quaternion4D a,
            Quaternion4D b,
            Quaternion4D c,
            double t)
        {
            return Slerp(
                Slerp(q1, c, t), Slerp(a, b, t), 2 * t * (1.0 - t));
        }

        public static void SquadSetup(
            ref Quaternion4D outA,
            ref Quaternion4D outB,
            ref Quaternion4D outC,
            Quaternion4D q0,
            Quaternion4D q1,
            Quaternion4D q2,
            Quaternion4D q3)
        {
            q0 = q0 + q1;
            q0.Normalize();

            q2 = q2 + q1;
            q2.Normalize();

            q3 = q3 + q1;
            q3.Normalize();

            q1.Normalize();

            outA = q1 * Exp(-0.25 * (Ln(Exp(q1) * q2) + Ln(Exp(q1) * q0)));
            outB = q2 * Exp(-0.25 * (Ln(Exp(q2) * q3) + Ln(Exp(q2) * q1)));
            outC = q2;

        }
    }

    public struct PlaneD
    {
        public double A, B, C, D;
        public PlaneD(double a, double b, double c, double d)
        {
            A = a; B = b; C = c; D = d;
        }
    }

    public sealed class MathEngine
    {

        public static readonly double Radius = 6378137.0;

        private MathEngine()
        {
        }

        public static Vector3D SphericalToCartesian(
            double latitude,
            double longitude,
            double radius
            )
        {
            double latRadians = latitude*Angle.DegreeToRadians;
            double lonRadians = longitude * Angle.DegreeToRadians;

            double radCosLat = radius * Math.Cos(latRadians);

            return new Vector3D(
               radCosLat * Math.Cos(lonRadians),
               radCosLat * Math.Sin(lonRadians),
               radius * Math.Sin(latRadians));
        }

        public static Vector3D SphericalToCartesian(
            Angle latitude,
            Angle longitude,
            double radius)
        {
            double latRadians = latitude.Radians;
            double lonRadians = longitude.Radians;

             double radCosLat = radius * Math.Cos(latRadians);

             return new Vector3D(
                radCosLat * Math.Cos(lonRadians),
                radCosLat * Math.Sin(lonRadians),
                radius * Math.Sin(latRadians));
        }

        public static Vector3D SphericalToCartesian(GeoCoord sv)
        {
            double latRadians = sv.Latitude * Angle.DegreeToRadians;
            double lonRadians = sv.Longitude * Angle.DegreeToRadians;

            double radCosLat = sv.Radius * Math.Cos(latRadians);

            return new Vector3D(
                radCosLat * Math.Cos(lonRadians),
                radCosLat * Math.Sin(lonRadians),
                sv.Radius * Math.Sin(latRadians));
        }

        public static int getLevel(double distance)
        {
            int maxLevel = 0;
            int minLevel = 22;
            int numLevels = minLevel - maxLevel + 1;
            int retLevel = maxLevel;
            for (int i = 0; i < numLevels; i++)
            {
                retLevel = i + maxLevel;

                double dis = 2 * 6378137;
                for (int j = 0; j < i; j++)
                {
                    dis = dis / 2;
                }
                if (distance >= dis)
                {
                    break;
                }
            }
            return retLevel;

        }

        public static double CalAltFromGeoRegion(GeoRegion region)
        {
            double span = region.LatitudeSpan > region.LongitudeSpan ? region.LatitudeSpan : region.LongitudeSpan;

            double alt = Radius * Math.Abs(Math.Sin(Angle.DegreeToRadians * span));
            
            return alt;
        }


        public static GeoCoord CartesianToSpherical(double x, double y, double z)
        {
            double rho = Math.Sqrt(x * x + y * y + z * z);
            double longitude = Math.Atan2(y, x);
            double latitude = Math.Asin(z / rho);

            return new GeoCoord(longitude * Angle.RadiansToDegree, latitude * Angle.RadiansToDegree, rho);
        }

        public static GeoCoord CartesianToSpherical(Vector3D v)
        {
            return CartesianToSpherical(v.X, v.Y, v.Z);
           
        }

        /// <summary>
        /// Intermediate points on a great circle
        /// In previous sections we have found intermediate points on a great circle given either
        /// the crossing latitude or longitude. Here we find points (lat,lon) a given fraction of the
        /// distance (d) between them. Suppose the starting point is (lat1,lon1) and the final point
        /// (lat2,lon2) and we want the point a fraction f along the great circle route. f=0 is
        /// point 1. f=1 is point 2. The two points cannot be antipodal ( i.e. lat1+lat2=0 and
        /// abs(lon1-lon2)=pi) because then the route is undefined.
        /// </summary>
        /// <param name="f">Fraction of the distance for intermediate point (0..1)</param>
        public static void IntermediateGCPoint(float f, Angle lat1, Angle lon1, Angle lat2, Angle lon2, Angle d,
            out Angle lat, out Angle lon)
        {
            double sind = Math.Sin(d.Radians);
            double cosLat1 = Math.Cos(lat1.Radians);
            double cosLat2 = Math.Cos(lat2.Radians);
            double A = Math.Sin((1 - f) * d.Radians) / sind;
            double B = Math.Sin(f * d.Radians) / sind;
            double x = A * cosLat1 * Math.Cos(lon1.Radians) + B * cosLat2 * Math.Cos(lon2.Radians);
            double y = A * cosLat1 * Math.Sin(lon1.Radians) + B * cosLat2 * Math.Sin(lon2.Radians);
            double z = A * Math.Sin(lat1.Radians) + B * Math.Sin(lat2.Radians);
            lat = Angle.FromRadians(Math.Atan2(z, Math.Sqrt(x * x + y * y)));
            lon = Angle.FromRadians(Math.Atan2(y, x));

        }

        /// <summary>
        /// Intermediate points on a great circle
        /// In previous sections we have found intermediate points on a great circle given either
        /// the crossing latitude or longitude. Here we find points (lat,lon) a given fraction of the
        /// distance (d) between them. Suppose the starting point is (lat1,lon1) and the final point
        /// (lat2,lon2) and we want the point a fraction f along the great circle route. f=0 is
        /// point 1. f=1 is point 2. The two points cannot be antipodal ( i.e. lat1+lat2=0 and
        /// abs(lon1-lon2)=pi) because then the route is undefined.
        /// </summary>
        /// <param name="f">Fraction of the distance for intermediate point (0..1)</param>
        public Vector3D IntermediateGCPoint(float f, Angle lat1, Angle lon1, Angle lat2, Angle lon2, Angle d)
        {
            double sind = Math.Sin(d.Radians);
            double cosLat1 = Math.Cos(lat1.Radians);
            double cosLat2 = Math.Cos(lat2.Radians);
            double A = Math.Sin((1 - f) * d.Radians) / sind;
            double B = Math.Sin(f * d.Radians) / sind;
            double x = A * cosLat1 * Math.Cos(lon1.Radians) + B * cosLat2 * Math.Cos(lon2.Radians);
            double y = A * cosLat1 * Math.Sin(lon1.Radians) + B * cosLat2 * Math.Sin(lon2.Radians);
            double z = A * Math.Sin(lat1.Radians) + B * Math.Sin(lat2.Radians);
            Angle lat = Angle.FromRadians(Math.Atan2(z, Math.Sqrt(x * x + y * y)));
            Angle lon = Angle.FromRadians(Math.Atan2(y, x));

            Vector3D v = MathEngine.SphericalToCartesian(lat, lon, 6378134.0f);
            return v;
        }

        /// <summary>
        /// Computes the great circle distance between two pairs of lat/longs.
        /// TODO: Compute distance using ellipsoid.
        /// </summary>
        public static Angle ApproxAngularDistance(Angle latA, Angle lonA, Angle latB, Angle lonB)
        {
            Angle dlon = lonB - lonA;
            Angle dlat = latB - latA;
            double k = Math.Sin(dlat.Radians * 0.5);
            double l = Math.Sin(dlon.Radians * 0.5);
            double a = k * k + Math.Cos(latA.Radians) * Math.Cos(latB.Radians) * l * l;
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return Angle.FromRadians(c);
        }

        /// <summary>
        /// Computes the distance between two pairs of lat/longs in meters.(验证正确)
        /// </summary>
        public static double ApproxDistance(Angle latA, Angle lonA, Angle latB, Angle lonB)
        {
            double distance = 6378137 * ApproxAngularDistance(latA, lonA, latB, lonB).Radians;
            return distance;
        }


        /// <summary>
        /// Computes the angle (seen from the center of the sphere) between 2 sets of latitude/longitude values.
        /// </summary>
        /// <param name="latA">Latitude of point 1 (decimal degrees)</param>
        /// <param name="lonA">Longitude of point 1 (decimal degrees)</param>
        /// <param name="latB">Latitude of point 2 (decimal degrees)</param>
        /// <param name="lonB">Longitude of point 2 (decimal degrees)</param>
        /// <returns>Angle in decimal degrees</returns>
        public static double SphericalDistanceDegrees(double latA, double lonA, double latB, double lonB)
        {
            return Math.Acos(
                Math.Cos(latA) * Math.Cos(latB) * Math.Cos(lonA - lonB) + 
                Math.Sin(latA) * Math.Sin(latB));
        }

        /// <summary>
        /// Computes the angular distance between two pairs of lat/longs.
        /// Fails for distances (on earth) smaller than approx. 2km. (returns 0)
        /// </summary>
        public static Angle SphericalDistance(Angle latA, Angle lonA, Angle latB, Angle lonB)
        {
            double radLatA = latA.Radians;
            double radLatB = latB.Radians;
            double radLonA = lonA.Radians;
            double radLonB = lonB.Radians;

            return Angle.FromRadians(Math.Acos(
                Math.Cos(radLatA) * Math.Cos(radLatB) * Math.Cos(radLonA - radLonB) +
                Math.Sin(radLatA) * Math.Sin(radLatB)));
        }

        /// <summary>
        /// Compute the tile number (used in file names) for given latitude and tile size.
        /// </summary>
        /// <param name="latitude">Latitude (decimal degrees)</param>
        /// <param name="tileSize">Tile size  (decimal degrees)</param>
        /// <returns>The tile number</returns>
        public static int GetRowFromLatitude(double latitude, double tileSize)
        {
            return (int)System.Math.Round((System.Math.Abs(-90.0 - latitude) % 180) / tileSize, 1);
        }

        public static int GetCurLevel(double tileSize)
        {
            int level = 0;
            double s = 180;
            while (s > tileSize)
            {
                s = s / 2;
                level++;
            }
            return level;
        }

        /// <summary>
        /// Compute the tile number (used in file names) for given latitude and tile size.
        /// </summary>
        /// <param name="latitude">Latitude (decimal degrees)</param>
        /// <param name="tileSize">Tile size  (decimal degrees)</param>
        /// <returns>The tile number</returns>
        public static int GetRowFromLatitude(Angle latitude, double tileSize)
        {
            //return (int)System.Math.Round((System.Math.Abs(-90.0 - latitude.Degrees) % 180) / tileSize, 1);
            return (int)System.Math.Floor((System.Math.Abs(-90.0 - latitude.Degrees) % 180) / tileSize);
        }

        /// <summary>
        /// Compute the tile number (used in file names) for given longitude and tile size.
        /// </summary>
        /// <param name="longitude">Longitude (decimal degrees)</param>
        /// <param name="tileSize">Tile size  (decimal degrees)</param>
        /// <returns>The tile number</returns>
        public static int GetColFromLongitude(double longitude, double tileSize)
        {
            return (int)System.Math.Floor((System.Math.Abs(-180.0 - longitude) % 360) / tileSize);
           // return (int)System.Math.Round((System.Math.Abs(-180.0 - longitude) % 360) / tileSize, 1);
        }

        /// <summary>
        /// Compute the tile number (used in file names) for given longitude and tile size.
        /// </summary>
        /// <param name="longitude">Longitude (decimal degrees)</param>
        /// <param name="tileSize">Tile size  (decimal degrees)</param>
        /// <returns>The tile number</returns>
        public static int GetColFromLongitude(Angle longitude, double tileSize)
        {
            return (int)System.Math.Round((System.Math.Abs(-180.0 - longitude.Degrees) % 360) / tileSize, 1);
        }


        /// <summary>
        /// Calculates the azimuth from latA/lonA to latB/lonB
        /// Borrowed from http://williams.best.vwh.net/avform.htm
        /// </summary>
        public static Angle Azimuth(Angle latA, Angle lonA, Angle latB, Angle lonB)
        {
            double cosLatB = Math.Cos(latB.Radians);
            Angle tcA = Angle.FromRadians(Math.Atan2(
                Math.Sin(lonA.Radians - lonB.Radians) * cosLatB,
                Math.Cos(latA.Radians) * Math.Sin(latB.Radians) -
                Math.Sin(latA.Radians) * cosLatB *
                Math.Cos(lonA.Radians - lonB.Radians)));
            if (tcA.Radians < 0)
                tcA.Radians = tcA.Radians + Math.PI * 2;
            tcA.Radians = Math.PI * 2 - tcA.Radians;

            return tcA;
        }

        public static Quaternion4D EulerToQuaternion(double yaw, double pitch, double roll)
        {
            double cy = Math.Cos(yaw * 0.5);
            double cp = Math.Cos(pitch * 0.5);
            double cr = Math.Cos(roll * 0.5);
            double sy = Math.Sin(yaw * 0.5);
            double sp = Math.Sin(pitch * 0.5);
            double sr = Math.Sin(roll * 0.5);

            double qw = cy * cp * cr + sy * sp * sr;
            double qx = sy * cp * cr - cy * sp * sr;
            double qy = cy * sp * cr + sy * cp * sr;
            double qz = cy * cp * sr - sy * sp * cr;

            return new Quaternion4D(qx, qy, qz, qw);
        }

        public static Vector3D QuaternionToEuler(Quaternion4D q)
        {
            double q0 = q.W;
            double q1 = q.X;
            double q2 = q.Y;
            double q3 = q.Z;

            double yaw = Math.Atan2(2 * (q2 * q3 + q0 * q1), (q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3));
            double pitch = Math.Asin(-2 * (q1 * q3 - q0 * q2));
            double roll = Math.Atan2(2 * (q1 * q2 + q0 * q3), (q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3));

            return new Vector3D(yaw, pitch, roll);
        }

        public static double DistancePlaneToPoint(PlaneD p, Vector3D v)
        {
            return p.A * v.X + p.B * v.Y + p.C + v.Z + p.D;
        }

        public static double Hypot(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static GeoRegion GetGeoRegion(int level, int row, int col)
        {
            double w = 1.0 / (1 << level);
            double h = w;
            double x = col * w;
            double y = row * h;
            GeoCoord sv1 = GetGeoPos(x, y);
            GeoCoord sv2 = GetGeoPos(x + w, y + h);

            return new GeoRegion(sv1.Longitude, sv2.Longitude, sv1.Latitude, sv2.Latitude);
        }

        public static GeoCoord GetGeoPos(double x, double y)
        {
            double lon = -180 + x * 180;
            double lat = -90 + y * 180;
            return new GeoCoord(lon, lat, Radius);
        }
    }

}
