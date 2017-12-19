using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Themes;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents color in HSL color space.
    /// </summary>
    /// <remarks>
    /// Used for color blending operations, defined in HSL color space which are more precise than in RGB. 
    /// HSL colors are used by theming and painting sub-systems of RadControls.
    /// </remarks>
    [TypeConverter(typeof(HslColorConverter))]
    public struct HslColor
    {
        public static readonly HslColor Empty = new HslColor();

        private HslColor(int a, double h, double s, double l)            
        {
            alpha = a;
            hue = h;
            saturation = s;
            luminance = l;
            //set properties to validate values
            //TODO: extract value normalization methods to call them here
            this.A = a;
            this.H = hue;
            this.S = saturation;
            this.L = luminance;
        }

        private HslColor(double h, double s, double l)
        {
            alpha = 255;
            hue = h;
            saturation = s;
            luminance = l;
        }

        private HslColor(Color color)
        {
            alpha = color.A;
            hue = 0d;
            saturation = 0d;
            luminance = 0d;
            this.RGBtoHSL(color);
        }

        //private HslColor(HslColor color)
        //{
        //    this.alpha = color.alpha;
        //    this.hue = color.hue;
        //    this.saturation = color.saturation;
        //    this.luminance = color.luminance;
        //}

        public static HslColor FromArgb(int a, int r, int g, int b)
        {
            return new HslColor(Color.FromArgb(a,r,g,b));
        }

        public static HslColor FromColor(Color color)
        {
            return new HslColor(color);
        }

        public static HslColor FromAhsl(int a)
        {
            return new HslColor(a, 0, 0, 0);
        }

        public static HslColor FromAhsl(int a, HslColor hsl)
        {
            return new HslColor(a, hsl.hue, hsl.saturation, hsl.luminance);
        }

        public static HslColor FromAhsl(double h, double s, double l)
        {
            return new HslColor(255, h, s, l);
        }

        public static HslColor FromAhsl(int a, double h, double s, double l)
        {
            return new HslColor(a,h,s,l);
        }

        #region operations
        public static bool operator ==(HslColor left, HslColor right)
        {
            if (((left.A == right.A) && (left.H == right.H)) && (left.S == right.S) && (left.L == right.L))
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(HslColor left, HslColor right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is HslColor)
            {
                HslColor color = (HslColor)obj;
                if (((this.A == color.A) && (this.H == color.H)) && (this.S == color.S) && (this.L == color.L))
                {
                    return true;
                }
            }
            return false;
        }

		public override int GetHashCode()
		{
			return this.alpha.GetHashCode() ^ this.hue.GetHashCode() ^
				this.saturation.GetHashCode() ^ this.luminance.GetHashCode();
		}

        #endregion

        #region Properties

        double hue;
        /// <summary>
        /// H Channel value
        /// </summary>
        [DefaultValue(0d), Category("Appearance"), Description("H Channel value")]
        public double H
        {
            get { return hue; }
            set
            {
                hue = value;
                hue = hue > 1 ? 1 : hue < 0 ? 0 : hue;
            }
        }

        double saturation;
        /// <summary>
        /// S Channel value
        /// </summary>
        [DefaultValue(0d), Category("Appearance"), Description("S Channel value")]
        public double S
        {
            get { return saturation; }
            set
            {
                saturation = value;
                saturation = saturation > 1 ? 1 : saturation < 0 ? 0 : saturation;
            }
        }

        double luminance;
        /// <summary>
        /// L Channel value
        /// </summary>
        [DefaultValue(0d), Category("Appearance"), Description("L Channel value")]
        public double L
        {
            get { return luminance; }
            set
            {
                luminance = value;
                luminance = luminance > 1 ? 1 : luminance < 0 ? 0 : luminance;
            }
        }

        /// <summary>
        /// RGB color value
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color RgbValue
        {
            get
            {
                return HSLtoRGB();
            }
            set
            {
                RGBtoHSL(value);
            }
        }

        private int alpha;

        /// <summary>
        /// Gets or sets color'a alpha chanel level in terms of argb color. Used mainly for conversion from/to ARGB color values.
        /// </summary>
        public int A
        {
            get { return alpha; }
            set { alpha = alpha > 255 ? 255 : alpha < 0 ? 0 : alpha; }
        }

        public bool IsEmpty
        {
            get
            {
                return alpha == 0 &&
                       H == 0d &&
                       S == 0d &&
                       L == 0d;
            }
        }

        #endregion

        #region Private methods

        private Color HSLtoRGB()
        {
            int Max, Mid, Min;
            double q;

            Max = Round(luminance * 255);
            Min = Round((1.0 - saturation) * (luminance / 1.0) * 255);
            q = (double)(Max - Min) / 255;

            if (hue >= 0 && hue <= (double)1 / 6)
            {
                Mid = Round(((hue - 0) * q) * 1530 + Min);
                return Color.FromArgb(alpha, Max, Mid, Min);
            }
            else if (hue <= (double)1 / 3)
            {
                Mid = Round(-((hue - (double)1 / 6) * q) * 1530 + Max);
                return Color.FromArgb(alpha, Mid, Max, Min);
            }
            else if (hue <= 0.5)
            {
                Mid = Round(((hue - (double)1 / 3) * q) * 1530 + Min);
                return Color.FromArgb(alpha, Min, Max, Mid);
            }
            else if (hue <= (double)2 / 3)
            {
                Mid = Round(-((hue - 0.5) * q) * 1530 + Max);
                return Color.FromArgb(alpha, Min, Mid, Max);
            }
            else if (hue <= (double)5 / 6)
            {
                Mid = Round(((hue - (double)2 / 3) * q) * 1530 + Min);
                return Color.FromArgb(alpha, Mid, Min, Max);
            }
            else if (hue <= 1.0)
            {
                Mid = Round(-((hue - (double)5 / 6) * q) * 1530 + Max);
                return Color.FromArgb(alpha, Max, Min, Mid);
            }
            else return Color.FromArgb(alpha, 0, 0, 0);
        }
        private void RGBtoHSL(Color color)
        {
            int Max, Min, Diff, Sum;
            double q;

            this.alpha = color.A;

            if (color.R > color.G) { Max = color.R; Min = color.G; }
            else { Max = color.G; Min = color.R; }

            if (color.B > Max) Max = color.B;
            else if (color.B < Min) Min = color.B;

            Diff = Max - Min;
            Sum = Max + Min;

            luminance = (double)Max / 255;

            if (Max == 0) saturation = 0;
            else saturation = (double)Diff / Max;

            if (Diff == 0) q = 0;
            else q = (double)60 / Diff;

            if (Max == color.R)
            {
                if (color.G < color.B) hue = (double)(360 + q * (color.G - color.B)) / 360;
                else hue = (double)(q * (color.G - color.B)) / 360;
            }
            else if (Max == color.G) hue = (double)(120 + q * (color.B - color.R)) / 360;
            else if (Max == color.B) hue = (double)(240 + q * (color.R - color.G)) / 360;
            else hue = 0.0;
        }

        private int Round(double val)
        {
            return (int)(val + 0.5d);
        }

        #endregion        
    }    
}