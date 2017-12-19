using System;
using System.Collections.Generic;
using System.Text;

namespace GeoVis.GeoCore
{
    /// <summary>
    /// A geometric angle
    /// </summary>
    public struct Angle
    {
        public static readonly double RadiansToDegree = 180 / Math.PI;
        public static readonly double DegreeToRadians = Math.PI / 180.0;

        [NonSerialized]
        public double Radians;

        /// <summary>
        /// Creates a new angle from angle in radians.
        /// </summary>
        public static Angle FromRadians(double radians)
        {
            Angle res = new Angle();
            res.Radians = radians;
            return res;
        }

        /// <summary>
        /// Creates a new angle from angle in degrees.
        /// </summary>
        public static Angle FromDegrees(double degrees)
        {
            Angle res = new Angle();
            res.Radians = degrees * DegreeToRadians;
            return res;
        }

        /// <summary>
        /// A zeroed angle
        /// </summary>
        public static readonly Angle Zero;

        /// <summary>
        /// Minimum value for angle
        /// </summary>
        public static readonly Angle MinValue = Angle.FromRadians(double.MinValue);

        /// <summary>
        /// Maximum value for angle
        /// </summary>
        public static readonly Angle MaxValue = Angle.FromRadians(double.MaxValue);

        /// <summary>
        /// Angle containing Not a Number
        /// </summary>
        public static readonly Angle NaN = Angle.FromRadians(double.NaN);

        public static readonly Angle PI = Angle.FromRadians(Math.PI);
        public static readonly Angle PI_2 = Angle.FromRadians(Math.PI/2);
        public static readonly Angle PI2 = Angle.FromRadians(Math.PI * 2);

        public double Degrees
        {
            get { return Radians * RadiansToDegree; }
            set { this.Radians = value * DegreeToRadians; }
        }

        /// <summary>
        /// Normalizes the angle so it is between 0?and 360?
        /// </summary>
        public void Normalize(bool bLongitude)
        {
            if (bLongitude)
            {
                while (Radians > Math.PI)
                    Radians -= Math.PI * 2;
                while (Radians < -Math.PI)
                    Radians += Math.PI * 2;
            }
            else
            {
                if (Radians > Math.PI / 2)
                    Radians = Math.PI / 2;
                if (Radians < -Math.PI / 2)
                    Radians = -Math.PI / 2;
            }
        }


        /// <summary>
        /// Returns the absolute value of the specified angle
        /// </summary>
        public static Angle Abs(Angle a)
        {
            return Angle.FromRadians(Math.Abs(a.Radians));
        }

        /// <summary>
        /// Checks for angle containing "Not a Number"
        /// </summary>
        public static bool IsNaN(Angle a)
        {
            return double.IsNaN(a.Radians);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Angle a = (Angle)obj;
            return Math.Abs(Radians - a.Radians) < Single.Epsilon;
        }

        public static bool operator ==(Angle a, Angle b)
        {
            return Math.Abs(a.Radians - b.Radians) < Double.Epsilon;
        }

        public static bool operator !=(Angle a, Angle b)
        {
            return Math.Abs(a.Radians - b.Radians) > Double.Epsilon;
        }

        public static bool operator <(Angle a, Angle b)
        {
            return a.Radians < b.Radians;
        }

        public static bool operator >(Angle a, Angle b)
        {
            return a.Radians > b.Radians;
        }

        public static Angle operator +(Angle a, Angle b)
        {
            double res = a.Radians + b.Radians;
            return Angle.FromRadians(res);
        }

        public static Angle operator -(Angle a, Angle b)
        {
            double res = a.Radians - b.Radians;
            return Angle.FromRadians(res);
        }

        public static Angle operator *(Angle a, double times)
        {
            return Angle.FromRadians(a.Radians * times);
        }

        public static Angle operator *(double times, Angle a)
        {
            return Angle.FromRadians(a.Radians * times);
        }

        public static Angle operator /(double divisor, Angle a)
        {
            return Angle.FromRadians(a.Radians / divisor);
        }

        public static Angle operator /(Angle a, double divisor)
        {
            return Angle.FromRadians(a.Radians / divisor);
        }

        public override int GetHashCode()
        {
            return (int)(Radians * 100000);
        }

        /// <summary>
        /// Normalizes the angle so it is between 0?and 360?
        /// </summary>
        public void Normalize()
        {
            if (Radians > Math.PI * 2)
                Radians -= Math.PI * 2;
            if (Radians < -Math.PI * 2)
                Radians += Math.PI * 2;
        }

        /// <summary>
        /// Converts degrees to degrees/minutes/seconds
        /// </summary>
        /// <returns>String on format ddDm'ss.sss"</returns>
        public override string ToString()
        {
            double decimalDegrees = this.Degrees;
            double d = Math.Abs(decimalDegrees);
            double m = (60 * (d - Math.Floor(d)));
            double s = (60 * (m - Math.Floor(m)));

            return String.Format("{0}бу{1}' {2:f3}\"",
                (int)d * Math.Sign(decimalDegrees),
                (int)m,
                s);
        }
    }
}
