using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.Layouts
{
    /// <summary>
    /// Represents a light-weight 3*3 Matrix to be used for GDI+ transformations.
    /// </summary>
    public struct RadMatrix
    {
        #region Static Members

        public static readonly RadMatrix Identity = new RadMatrix(1F, 0F, 0F, 1F, 0F, 0F);
        public static readonly RadMatrix Empty = new RadMatrix(0F, 0F, 0F, 0F, 0F, 0F);
        public const float PI = 3.141593F;
        public const float TwoPI = PI * 2F;
        public const float RadianToDegree = (float)(180D / PI);
        public const float DegreeToRadian = (float)(PI / 180D);

        #endregion

        #region Fields

        public float DX;
        public float DY;
        public float M11;
        public float M12;
        public float M21;
        public float M22;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new RadMatrix, using the specified parameters.
        /// </summary>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public RadMatrix(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M21 = m21;
            this.M22 = m22;
            this.DX = dx;
            this.DY = dy;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public RadMatrix(RadMatrix source)
        {
            this.M11 = source.M11;
            this.M12 = source.M12;
            this.M21 = source.M21;
            this.M22 = source.M22;
            this.DX = source.DX;
            this.DY = source.DY;
        }

        /// <summary>
        /// Initializes a new RadMatrix, using the elements of the specified GDI+ Matrix instance.
        /// </summary>
        /// <param name="gdiMatrix"></param>
        public RadMatrix(Matrix gdiMatrix)
        {
            float[] elements = gdiMatrix.Elements;
            this.M11 = elements[0];
            this.M12 = elements[1];
            this.M21 = elements[2];
            this.M22 = elements[3];
            this.DX = elements[4];
            this.DY = elements[5];
        }

        /// <summary>
        /// Initializes a new RadMatrix, applying the specified X and Y values as DX and DY members of the matrix.
        /// </summary>
        /// <param name="offset"></param>
        public RadMatrix(PointF offset)
        {
            this.M11 = 1F;
            this.M12 = 0F;
            this.M21 = 0F;
            this.M22 = 1F;
            this.DX = offset.X;
            this.DY = offset.Y;
        }

        /// <summary>
        /// Initializes a new RadMatrix, scaling it by the provided parameters, at the origin (0, 0).
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        public RadMatrix(float scaleX, float scaleY)
            : this(scaleX, scaleY, PointF.Empty)
        {
        }

        /// <summary>
        /// Initializes a new RadMatrix, scaling it by the provided parameters, at the specified origin.
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="origin"></param>
        public RadMatrix(float scaleX, float scaleY, PointF origin)
        {
            this.M11 = scaleX;
            this.M12 = 0F;
            this.M21 = 0F;
            this.M22 = scaleY;
            this.DX = origin.X - (scaleX * origin.X);
            this.DY = origin.Y - (scaleY * origin.Y);
        }

        /// <summary>
        /// Initializes a new RadMatrix, rotated by the specified angle (in degrees) at origin (0, 0).
        /// </summary>
        /// <param name="angle"></param>
        public RadMatrix(float angle)
            : this(angle, PointF.Empty)
        {
        }

        /// <summary>
        /// Initializes a new RadMatrix, rotated by the specified angle (in degrees) at the provided origin.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="origin"></param>
        public RadMatrix(float angle, PointF origin)
        {
            if (angle == 0F || angle == 360F)
            {
                this = RadMatrix.Identity;
            }
            else
            {
                float cos;
                float sin;
                RadMatrix.GetCosSin(angle, out cos, out sin);

                this.M11 = cos;
                this.M12 = sin;
                this.M21 = -sin;
                this.M22 = cos;
                //calculate DX and DY
                if (origin != PointF.Empty)
                {
                    float x = origin.X;
                    float y = origin.Y;
                    this.DX = x - (cos * x) + (sin * y);
                    this.DY = y - (cos * y) - (sin * x);
                }
                else
                {
                    this.DX = 0F;
                    this.DY = 0F;
                }
            }
        }

        private static void GetCosSin(float angle, out float cos, out float sin)
        {
            //normalize angle - e.g. -90 = 270
            if (angle < 0)
            {
                angle = 360 + angle;
            }

            //handle special cases to eliminate floating-point approximation errors
            if (angle == 90F)
            {
                cos = 0F;
                sin = 1F;
            }
            else if (angle == 180)
            {
                cos = -1F;
                sin = 0F;
            }
            else if (angle == 270F)
            {
                cos = 0F;
                sin = -1;
            }
            else
            {
                float angleInRadians = angle * RadMatrix.DegreeToRadian;
                cos = (float)Math.Cos(angleInRadians);
                sin = (float)Math.Sin(angleInRadians);
            }
        }

        #endregion

        #region Matrix Operations

        public void Scale(float scaleX, float scaleY)
        {
            this.Scale(scaleX, scaleY, MatrixOrder.Prepend);
        }

        public void Scale(float scaleX, float scaleY, MatrixOrder order)
        {
            this.Multiply(new RadMatrix(scaleX, scaleY), order);
        }

        public void Rotate(float angle)
        {
            this.Rotate(angle, MatrixOrder.Prepend);
        }

        public void Rotate(float angle, MatrixOrder order)
        {
            this.Multiply(new RadMatrix(angle), order);
        }

        public void RotateAt(float angle, PointF origin)
        {
            this.RotateAt(angle, origin, MatrixOrder.Prepend);
        }

        public void RotateAt(float angle, PointF origin, MatrixOrder order)
        {
            if (angle != 0F)
            {
                this.Multiply(new RadMatrix(angle, origin), order);
            }
        }

        public void Translate(float dx, float dy)
        {
            this.Translate(dx, dy, MatrixOrder.Prepend);
        }

        public void Translate(float dx, float dy, MatrixOrder order)
        {
            this.Multiply(new RadMatrix(new PointF(dx, dy)), order);
        }

        public void Multiply(RadMatrix m)
        {
            this.Multiply(m, MatrixOrder.Prepend);
        }

        public void Multiply(RadMatrix m, MatrixOrder order)
        {
            if (order == MatrixOrder.Append)
            {
                this *= m;
            }
            else
            {
                this = m * this;
            }
        }

        public void Divide(RadMatrix m)
        {
            m.Invert();
            this.Multiply(m, MatrixOrder.Prepend);
        }

        public void Invert()
        {
            if (this.IsIdentity)
            {
                return;
            }

            float determinant = this.Determinant;
            if (determinant == 0F)
            {
                //nothing to invert, make us empty
                this = Empty;
                return;
            }

            float m11 = this.M22 / determinant;
            float m12 = -this.M12 / determinant;
            float m21 = -this.M21 / determinant;
            float m22 = this.M11 / determinant;
            float dx = (this.DX * -m11) - (this.DY * m21);
            float dy = (this.DX * -m12) - (this.DY * m22);

            this = new RadMatrix(m11, m12, m21, m22, dx, dy);
        }

        public void Reset()
        {
            this = Identity;
        }

        #endregion

        #region Transformation Methods

        public PointF TransformPoint(PointF point)
        {
            float x = point.X;
            float y = point.Y;

            return new PointF((x * this.M11 + y * this.M21 + this.DX), (x * this.M12 + y * this.M22 + this.DY));
        }

        public void TransformPoints(PointF[] points)
        {
            int length = points.Length;
            for (int i = 0; i < length; i++)
            {
                points[i] = this.TransformPoint(points[i]);
            }
        }

        public RectangleF TransformRectangle(RectangleF bounds)
        {
            PointF topLeft = bounds.Location;
            PointF bottomRight = new PointF(topLeft.X + bounds.Width, topLeft.Y + bounds.Height);

            topLeft = this.TransformPoint(topLeft);
            bottomRight = this.TransformPoint(bottomRight);

            return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }

        #endregion

        #region Helper Methods

        public Matrix ToGdiMatrix()
        {
            return new Matrix(this.M11, this.M12, this.M21, this.M22, this.DX, this.DY);
        }

        public bool Equals(Matrix gdiMatrix)
        {
            return this.Equals(gdiMatrix.Elements);
        }

        public bool Equals(float[] elements)
        {
            if (elements.Length != 6)
            {
                throw new ArgumentException("Invalid float array to compare to.");
            }

            return this.M11 == elements[0] &&
                    this.M12 == elements[1] &&
                    this.M21 == elements[2] &&
                    this.M22 == elements[3] &&
                    this.DX == elements[4] &&
                    this.DY == elements[5];
        }

        public static float PointsDistance(PointF pt1, PointF pt2)
        {
            double distX = pt2.X - pt1.X;
            double distY = pt2.Y - pt1.Y;

            return (float)Math.Sqrt((distX * distX) + (distY * distY));
        }

        #endregion

        #region Operators

        public static RadMatrix operator *(RadMatrix a, RadMatrix b)
        {
            return new RadMatrix((a.M11 * b.M11) + (a.M12 * b.M21),
                                 (a.M11 * b.M12) + (a.M12 * b.M22),
                                 (a.M21 * b.M11) + (a.M22 * b.M21),
                                 (a.M21 * b.M12) + (a.M22 * b.M22),
                                 ((a.DX * b.M11) + (a.DY * b.M21)) + b.DX,
                                 ((a.DX * b.M12) + (a.DY * b.M22)) + b.DY);
        }

        public static bool operator ==(RadMatrix a, RadMatrix b)
        {
            return a.M11 == b.M11 &&
                    a.M12 == b.M12 &&
                    a.M21 == b.M21 &&
                    a.M22 == b.M22 &&
                    a.DX == b.DX &&
                    a.DY == b.DY;
        }

        public static bool operator !=(RadMatrix a, RadMatrix b)
        {
            return !(a == b);
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return this.M11.GetHashCode() ^
                    this.M12.GetHashCode() ^
                    this.M21.GetHashCode() ^
                    this.M22.GetHashCode() ^
                    this.DX.GetHashCode() ^
                    this.DY.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RadMatrix))
            {
                return false;
            }

            return (RadMatrix)obj == this;
        }

        public override string ToString()
        {
            return "RadMatrix: Offset [" + this.DX + ", " + this.DY + "]";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the current matrix is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this == RadMatrix.Empty;
            }
        }

        /// <summary>
        /// Determines whether this matrix equals to the Identity one.
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                return this == Identity;
            }
        }

        /// <summary>
        /// Gets the determinant - [(M11 * M22) - (M12 * M21)] - of this Matrix.
        /// </summary>
        public float Determinant
        {
            get
            {
                return (this.M11 * this.M22) - (this.M12 * this.M21);
            }
        }

        /// <summary>
        /// Determines whether this matrix may be inverted. That is to have non-zero determinant.
        /// </summary>
        public bool IsInvertible
        {
            get
            {
                return this.Determinant != 0F;
            }
        }

        /// <summary>
        /// Gets the scale by the X axis, provided by this matrix.
        /// </summary>
        public float ScaleX
        {
            get
            {
                PointF pt1 = this.TransformPoint(PointF.Empty);
                PointF pt2 = this.TransformPoint(new PointF(1F, 0F));

                return RadMatrix.PointsDistance(pt1, pt2);
            }
        }

        /// <summary>
        /// Gets the scale by the Y axis, provided by this matrix.
        /// </summary>
        public float ScaleY
        {
            get
            {
                PointF pt1 = this.TransformPoint(PointF.Empty);
                PointF pt2 = this.TransformPoint(new PointF(0F, 1F));

                return RadMatrix.PointsDistance(pt1, pt2);
            }
        }

        /// <summary>
        /// Gets the rotation (in degrees) applied to this matrix.
        /// </summary>
        public float Rotation
        {
            get
            {
                double angleInRadians = Math.Atan2(this.M12, this.M11);
                return (float)(angleInRadians * RadMatrix.RadianToDegree);
            }
        }

        /// <summary>
        /// Gets all the six fields of the matrix as an array.
        /// </summary>
        public float[] Elements
        {
            get
            {
                return new float[] { this.M11, this.M12, this.M21, this.M22, this.DX, this.DY };
            }
        }

        #endregion
    }
}
