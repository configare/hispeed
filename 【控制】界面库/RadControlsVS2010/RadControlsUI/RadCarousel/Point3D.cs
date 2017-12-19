using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Drawing;
using Telerik.WinControls.UI.Carousel;

namespace Telerik.WinControls.UI
{
    [TypeConverter(typeof(Point3DConverter))]
    public class Point3D
    {
        private double x = 0;
        private double y = 0;
        private double z = 0;

        public static Point3D Empty = new Point3D(0, 0, 0);

        #region Constructors

        public Point3D()
        {
        }

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point3D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Point3D(PointF pt)
            : this(pt.X, pt.Y)
        {
        }

        public Point3D(Point pt)
            : this(pt.X, pt.Y)
        {
        }

        public Point3D(Point3D pt)
            : this(pt.X, pt.Y, pt.Z)
        {
        }

        #endregion

        #region Properties

        [DefaultValue(0d)]
        [NotifyParentProperty(true)]
        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        [DefaultValue(0d)]
        [NotifyParentProperty(true)]
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        [DefaultValue(0d)]
        [NotifyParentProperty(true)]
        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        #endregion

        public void Negate()
        {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
        }

        public double Length()
        {
            return Math.Sqrt(( this.x * this.x ) + ( this.y * this.y ) + (this.z * this.z) );
        }

        public void Normalize()
        {
            double l = this.Length();

            this.x /= l;
            this.y /= l;
            this.z /= l;
        }

        public void Add(double x, double y, double z)
        {
            this.x += x;
            this.y += y;
            this.z += z;
        }

        public void Add(Point3D pt)
        {
            this.Add(pt.x, pt.y, pt.z);
        }

        public void Subtract(double x, double y, double z)
        {
            this.Add(-x, -y, -z);
        }

        public void Subtract(Point3D pt)
        {
            this.Add(-pt.x, -pt.y, -pt.z);
        }

        public void Multiply(double coef)
        {
            this.x *= coef;
            this.y *= coef;
            this.z *= coef;
        }

        public void Divide(double coef)
        {
            this.x /= coef;
            this.y /= coef;
            this.z /= coef;
        }

        public static Point3D CrossProduct(Point3D vect1, Point3D vect2)
        {
            return new Point3D(
                (vect1.y * vect2.z) - (vect1.z * vect2.y),
                (vect1.z * vect2.x) - (vect1.x * vect2.z),
                (vect1.x * vect2.y )- (vect1.y * vect2.x)
                );
        }

        public static double DotProduct(Point3D vect1, Point3D vect2)
        {
            return (vect1.x * vect2.x ) + ( vect1.y * vect2.y ) + (vect1.z * vect2.z );
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            Point3D pt = obj as Point3D;

            return pt != null &&
                double.Equals(this.x, pt.x)
                && double.Equals(this.y, pt.y)
                && double.Equals(this.z, pt.z);
        }

        public override int GetHashCode()
        {
            return (int)this.x ^ (int)this.y ^ (int)this.z;
        }

        #region Operators

        public static Point3D operator +(Point3D pt1, Point3D pt2)
        {
            return new Point3D(pt1.x + pt2.x, pt1.y + pt2.y, pt1.z + pt2.z);
        }

        public static Point3D operator -(Point3D pt1, Point3D pt2)
        {
            return new Point3D(pt1.x - pt2.x, pt1.y - pt2.y, pt1.z - pt2.z);
        }

        public static Point3D operator *(Point3D pt, float coef)
        {
            return new Point3D(pt.x * coef, pt.y * coef, pt.z * coef);
        }

        public static Point3D operator /(Point3D pt, float coef)
        {
            return new Point3D(pt.x / coef, pt.y / coef, pt.z / coef);
        }

        public static Point3D operator -(Point3D pt)
        {
            return new Point3D(-pt.x, -pt.y, -pt.z);
        }

        #endregion

        #region type casts (explicit)

        public static explicit operator PointF(Point3D pt)
        {
            return pt.ToPointF();
        }

        public static explicit operator Point(Point3D pt)
        {
            return pt.ToPoint();
        }

        #endregion

        public PointF ToPointF()
        {
            return new PointF((float)this.x, (float)this.y);
        }

        public Point ToPoint()
        {
            return new Point((int)Math.Round(this.x), (int)Math.Round(this.y));
        }        
    }
}
