using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    internal static class DoubleUtil
    {
        private const float epsilon = 4.649123e-7f;//0.0000000024336f;//float.Epsilon

        public static bool AreClose(float value1, float value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            float num1 = ((Math.Abs(value1) + Math.Abs(value2)) + 10) * epsilon;
            float num2 = value1 - value2;
            if (-num1 < num2)
            {
                return (num1 > num2);
            }
            return false;
        }

        public static bool AreClose(PointF point1, PointF point2)
        {
            if (DoubleUtil.AreClose(point1.X, point2.X))
            {
                return DoubleUtil.AreClose(point1.Y, point2.Y);
            }
            return false;
        }

        public static bool AreClose(RectangleF rect1, RectangleF rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            if ((!rect2.IsEmpty && DoubleUtil.AreClose(rect1.X, rect2.X)) && (DoubleUtil.AreClose(rect1.Y, rect2.Y) && DoubleUtil.AreClose(rect1.Height, rect2.Height)))
            {
                return DoubleUtil.AreClose(rect1.Width, rect2.Width);
            }
            return false;
        }

        public static bool AreClose(SizeF size1, SizeF size2)
        {
            if (DoubleUtil.AreClose(size1.Width, size2.Width))
            {
                return DoubleUtil.AreClose(size1.Height, size2.Height);
            }
            return false;
        }

        public static int DoubleToInt(float val)
        {
            if (0 >= val)
            {
                return (int)(val - 0.5);
            }
            return (int)(val + 0.5);
        }

        public static bool GreaterThan(float value1, float value2)
        {
            if (value1 > value2)
            {
                return !DoubleUtil.AreClose(value1, value2);
            }
            return false;
        }

        public static bool GreaterThanOrClose(float value1, float value2)
        {
            if (value1 <= value2)
            {
                return DoubleUtil.AreClose(value1, value2);
            }
            return true;
        }

        public static bool IsBetweenZeroAndOne(float val)
        {
            if (DoubleUtil.GreaterThanOrClose(val, 0))
            {
                return DoubleUtil.LessThanOrClose(val, 1);
            }
            return false;
        }

        public static bool IsOne(float value)
        {
            return (Math.Abs((float)(value - 1)) < (10 * epsilon));
        }

        public static bool IsZero(float value)
        {
            return (Math.Abs(value) < (10 * epsilon));
        }

        public static bool LessThan(float value1, float value2)
        {
            if (value1 < value2)
            {
                return !DoubleUtil.AreClose(value1, value2);
            }
            return false;
        }

        public static bool LessThanOrClose(float value1, float value2)
        {
            if (value1 >= value2)
            {
                return DoubleUtil.AreClose(value1, value2);
            }
            return true;
        }
    }
}
