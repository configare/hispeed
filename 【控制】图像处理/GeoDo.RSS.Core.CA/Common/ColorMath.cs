using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.CA
{
    public class ColorMath
    {
        /// <summary>
        /// Convert RGB to CMYK
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="c">0~100</param>
        /// <param name="m">0~100</param>
        /// <param name="y">0~100</param>
        /// <param name="k">0~100</param>
        public static void RGB2CMYK(byte red, byte green, byte blue, ref byte c, ref byte m, ref byte y, ref byte k)
        {
            float r = red, g = green, b = blue;
            r = 1.0f - (r / 255.0f);
            g = 1.0f - (g / 255.0f);
            b = 1.0f - (b / 255.0f);
            float cyan, magenta, yellow, black;
            if (r < g)
                black = r;
            else
                black = g;
            if (b < black)
                black = b;
            cyan = (r - black) / (1.0f - black);
            magenta = (g - black) / (1.0f - black);
            yellow = (b - black) / (1.0f - black);
            cyan = (cyan * 100);
            magenta = (magenta * 100);
            yellow = (yellow * 100);
            black = (black * 100);
            c = FixByte(cyan);
            m = FixByte(magenta);
            y = FixByte(yellow);
            k = FixByte(black);
            c = Math.Max((byte)0, (byte)c);
            c = Math.Min((byte)c, (byte)100);
            m = Math.Max((byte)0, (byte)m);
            m = Math.Min((byte)m, (byte)100);
            y = Math.Max((byte)0, (byte)y);
            y = Math.Min((byte)y, (byte)100);
            k = Math.Max((byte)0, (byte)k);
            k = Math.Min((byte)k, (byte)100);
        }

        /// <summary>
        /// Convert CMYK to RGB
        /// </summary>
        /// <param name="c">0~100</param>
        /// <param name="m">0~100</param>
        /// <param name="y">0~100</param>
        /// <param name="k">0~100</param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void CMYK2RGB(byte c, byte m, byte y, byte k, ref byte red, ref byte green, ref byte blue)
        {
            float r, g, b;
            float cyan = c, magenta = m, yellow = y, black = k;
            cyan = cyan / 100;
            magenta = magenta / 100;
            yellow = yellow / 100;
            black = black / 100;
            r = cyan * (1.0f - black) + black;
            g = magenta * (1.0f - black) + black;
            b = yellow * (1.0f - black) + black;
            r = (1.0f - r) * 255.0f;
            g = (1.0f - g) * 255.0f;
            b = (1.0f - b) * 255.0f;
            red = FixByte(r);
            green = FixByte(g);
            blue = FixByte(b);
        }

        /*
         * 通过演化推导，任意一种颜色（R’，G’，B’）的色相计算公式：
                  Max为三个分量的最大值，Min为三个分量的最小值
                  若Max=Min，表示灰度色，此时，H=0
                  若Max≠Min，分为两种情况：
                        当G≥B时，H=（Max-R’+G’-Min+B’-Min）/（Max-Min）×60
                        当G＜B时，H=360-（Max-R’+G’-Min+B’-Min）/（Max-Min）×60
         */
        /// <summary>
        /// 计算色相(HUE)
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns>0-360 degree</returns>
        public static float ComputeHue(Color rgb)
        {
            //byte max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
            //byte min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
            //if (max == min)
            //    return float.NaN;//undefined!
            //if (rgb.G >= rgb.B)
            //    return (float)(max - rgb.R + rgb.G - min + rgb.B - min) / (max - min) * 60f;
            //else
            //    return 360 - (float)(max - rgb.R + rgb.G - min + rgb.B - min) / (max - min) * 60f;
            return ComputeHue(rgb.R, rgb.G, rgb.B);
        }

        /// <summary>
        /// 计算色相(HUE)
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns>0-360 degree</returns>
        public static float ComputeHue(byte red, byte green, byte blue)
        {
            if (red == green && green == blue)
            {
                return 0f;
            }
            float num = (float)red / 255f;
            float num2 = (float)green / 255f;
            float num3 = (float)blue / 255f;
            float num4 = 0f;
            float num5 = num;
            float num6 = num;
            if (num2 > num5)
            {
                num5 = num2;
            }
            if (num3 > num5)
            {
                num5 = num3;
            }
            if (num2 < num6)
            {
                num6 = num2;
            }
            if (num3 < num6)
            {
                num6 = num3;
            }
            float num7 = num5 - num6;
            if (num == num5)
            {
                num4 = (num2 - num3) / num7;
            }
            else
            {
                if (num2 == num5)
                {
                    num4 = 2f + (num3 - num) / num7;
                }
                else
                {
                    if (num3 == num5)
                    {
                        num4 = 4f + (num - num2) / num7;
                    }
                }
            }
            num4 *= 60f;
            if (num4 < 0f)
            {
                num4 += 360f;
            }
            return num4;
        }

        /// <summary>
        /// 颜色容差计算(与Shotoshop中颜色替换工具中颜色容差效果一样)
        /// </summary>
        /// <param name="red1"></param>
        /// <param name="green1"></param>
        /// <param name="blue1"></param>
        /// <param name="red2"></param>
        /// <param name="green2"></param>
        /// <param name="blue2"></param>
        /// <returns>color distance</returns>
        public static float ColorDistance(byte red1, byte green1, byte blue1, byte red2, byte green2, byte blue2)
        {
            int rmena = (red1 + red2) / 2;
            int r = red1 - red2;
            int g = green1 - green2;
            int b = blue1 - blue2;
            return (float)(Math.Sqrt((((512 + rmena) * r * r) >> 8) + 4 * g * g + (((767 - rmena) * b * b) >> 8)));
        }

        /// <summary>
        /// 颜色容差计算(CIE Lab*)
        /// </summary>
        /// <param name="red1"></param>
        /// <param name="green1"></param>
        /// <param name="blue1"></param>
        /// <param name="red2"></param>
        /// <param name="green2"></param>
        /// <param name="blue2"></param>
        /// <returns>Color Distance</returns>
        public static float CIELabColorDiff(byte red1, byte green1, byte blue1, byte red2, byte green2, byte blue2)
        {
            float L1 = 0, L2 = 0, a1 = 0, a2 = 0, b1 = 0, b2 = 0;
            RGB2CIELab(red1, green1, blue1, ref L1, ref  a1, ref b1);
            RGB2CIELab(red2, green2, blue2, ref L2, ref  a2, ref b2);
            return (float)Math.Sqrt(Math.Pow(L2 - L1, 2) + Math.Pow(a2 - a1, 2) + Math.Pow(b2 - b1, 2));
        }

        public static void RGB2CIELab(byte red, byte green, byte blue, ref float L, ref float a, ref float b)
        {
            float X = 0, Y = 0, Z = 0;
            RGB2XYZ(red, green, blue, ref X, ref Y, ref Z);
            XYZ2CIELab(X, Y, Z, ref L, ref a, ref b);
        }

        public static void CIELab2RGB(float L, float a, float b, ref byte red, ref byte green, ref byte blue)
        {
            float X = 0, Y = 0, Z = 0;
            CIELab2XYZ(L, a, b, ref X, ref Y, ref Z);
            XYZ2RGB(X, Y, Z, ref red, ref green, ref blue);
        }

        public static void CIELab2XYZ(float L, float a, float b, ref float X, ref float Y, ref float Z)
        {
            float varY = (L + 16) / 116f;
            float varX = a / 500f + varY;
            float varZ = varY - b / 200f;
            if (Math.Pow(varY, 3) > 0.008856f)
                varY = (float)Math.Pow(varY, 3);
            else
                varY = (varY - 16 / 116f) / 7.787f;
            //
            if (Math.Pow(varX, 3) > 0.008856f)
                varX = (float)Math.Pow(varX, 3);
            else
                varX = (varX - 16 / 116f) / 7.787f;
            //
            if (Math.Pow(varZ, 3) > 0.008856f)
                varZ = (float)Math.Pow(varZ, 3);
            else
                varZ = (varZ - 16 / 116f) / 7.787f;
            X = 95.047f * varX;
            Y = 100f * varY;
            Z = 108.883f * varZ;
        }

        public static void XYZ2CIELab(float X, float Y, float Z, ref float L, ref float a, ref float b)
        {
            float varX = X / 95.047f;
            float varY = Y / 100f;
            float varZ = Z / 108.883f;
            if (varX > 0.008856f)
                varX = (float)Math.Pow(varX, 1 / 3f);
            else
                varX = 7.787f * varX + 16 / 116f;
            //
            if (varY > 0.008856f)
                varY = (float)Math.Pow(varY, 1 / 3f);
            else
                varY = 7.787f * varY + 16 / 116f;
            //
            if (varZ > 0.008856f)
                varZ = (float)Math.Pow(varZ, 1 / 3f);
            else
                varZ = 7.787f * varZ + 16 / 116f;
            L = 116 * varY - 16;
            a = 500 * (varX - varY);
            b = 200 * (varY - varZ);
        }

        public static void RGB2XYZ(byte red, byte green, byte blue, ref float X, ref float Y, ref float Z)
        {
            float varR = red / 255f;
            float varG = green / 255f;
            float varB = blue / 255f;
            if (varR > 0.04045f)
                varR = (float)Math.Pow(((varR + 0.055f) / 1.055f), 2.4f);
            else
                varR = varR / 12.92f;
            if (varG > 0.04045f)
                varG = (float)Math.Pow(((varG + 0.055f) / 1.055f), 2.4f);
            else
                varG = varG / 12.92f;
            if (varB > 0.04045f)
                varB = (float)Math.Pow(((varB + 0.055f) / 1.055f), 2.4f);
            else
                varB = varB / 12.92f;
            varR *= 100;
            varG *= 100;
            varB *= 100;
            X = varR * 0.4124f + varG * 0.3576f + varB * 0.1805f;
            Y = varR * 0.2126f + varG * 0.7152f + varB * 0.0722f;
            Z = varR * 0.0193f + varG * 0.1192f + varB * 0.9505f;
        }

        public static void XYZ2RGB(float X, float Y, float Z, ref byte red, ref byte green, ref byte blue)
        {
            float varX = X / 100f;
            float varY = Y / 100f;
            float varZ = Z / 100f;
            float varR = varX * 3.2406f - varY * 1.5372f - varZ * 0.4986f;
            float varG = -varX * 0.9689f + varY * 1.8758f + varZ * 0.0415f;
            float varB = varX * 0.0557f - varY * 0.2040f + varZ * 1.0570f;
            if (varR > 0.0031308f)
                varR = 1.055f * (float)Math.Pow(varR, 1 / 2.4f) - 0.055f;
            else
                varR *= 12.92f;
            if (varG > 0.0031308f)
                varG = 1.055f * (float)Math.Pow(varG, 1 / 2.4f) - 0.055f;
            else
                varG *= 12.92f;
            if (varB > 0.0031308f)
                varB = 1.055f * (float)Math.Pow(varB, 1 / 2.4f) - 0.055f;
            else
                varB *= 12.92f;
            red = FixByte(varR * 255);
            green = FixByte(varG * 255);
            blue = FixByte(varB * 255);
        }

        /// <summary>
        /// Convert RGB to HSV(HSB)
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        public static void RGB2HSB(byte red, byte green, byte blue, ref double hue, ref double saturation, ref double brightness)
        {
            byte min = Math.Min(red, Math.Min(green, blue));
            byte max = Math.Max(red, Math.Max(green, blue));
            if (max == min)
                hue = 0;
            else if (max == red && green >= blue)
                hue = 60d * ((green - blue) / (double)(max - min)) + 0;
            else if (max == red && green == blue)
                hue = 60d * ((green - blue) / (double)(max - min)) + 360;
            else if (max == red)
                hue = 60d * (green - blue) / (double)(max - min);
            else if (max == green)
                hue = 60d * ((blue - red) / (double)(max - min)) + 120;
            else if (max == blue)
                hue = 60d * ((red - green) / (double)(max - min)) + 240;
            while (hue < 0)
                hue += 360;
            while (hue > 360)
                hue -= 360;
            //
            if (max == 0)
                saturation = 0;
            else
                saturation = (1d - min / (double)max);
            //
            brightness = max / 255d;
        }

        /// <summary>
        /// Convert HSB(HSV) to RGB
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public static void HSB2RGB(double hue, double saturation, double brightness, ref byte red, ref byte green, ref byte blue)
        {
            int hi = (int)Math.Floor((hue / 60d) % 6);
            double f = hue / 60d - hi;
            double p = Math.Round(brightness * (1 - saturation) * 255d);
            double q = Math.Round(brightness * (1 - f * saturation) * 255d);
            double t = Math.Round(brightness * (1 - (1 - f) * saturation) * 255d);
            brightness *= 255d;
            brightness = Math.Round(brightness);
            switch (hi)
            {
                case 0:
                    red = (byte)brightness; green = (byte)t; blue = (byte)p;
                    break;
                case 1:
                    red = (byte)q; green = (byte)brightness; blue = (byte)p;
                    break;
                case 2:
                    red = (byte)p; green = (byte)brightness; blue = (byte)t;
                    break;
                case 3:
                    red = (byte)p; green = (byte)q; blue = (byte)brightness;
                    break;
                case 4:
                    red = (byte)t; green = (byte)p; blue = (byte)brightness;
                    break;
                case 5:
                    red = (byte)brightness; green = (byte)p; blue = (byte)q;
                    break;
            }
        }

        /// <summary>
        /// Convert RGB to HSL
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="lightness"></param>
        public static void RGB2HSL(byte red, byte green, byte blue, ref double hue, ref double saturation, ref double lightness)
        {
            int min = Math.Min(red, Math.Min(green, blue));
            int max = Math.Max(red, Math.Max(green, blue));
            int dltmax = max - min;
            lightness = (min + max) / 2d / 255;
            if (dltmax == 0)
            {
                hue = 0;
                saturation = 0;
            }
            else
            {
                if (lightness < 0.5d)
                    saturation = dltmax / (double)(min + max);
                else
                    saturation = dltmax / (double)(510 - max - min);
                double dltR = (((max - red) / 6d) + (dltmax / 2d)) / dltmax;
                double dltG = (((max - green) / 6d) + (dltmax / 2d)) / dltmax;
                double dltB = (((max - blue) / 6d) + (dltmax / 2d)) / dltmax;
                if (red == max)
                    hue = dltB - dltG;
                else if (green == max)
                    hue = 1 / 3d + dltR - dltB;
                else if (blue == max)
                    hue = 2 / 3d + dltG - dltR;
                while (hue < 0)
                    hue += 1;
                while (hue > 1)
                    hue -= 1;
                hue *= 360d;
            }
        }

        /// <summary>
        /// Convert HSL to RGB
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="lightness"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public static void HSL2RGB(double hue, double saturation, double lightness, ref byte red, ref byte green, ref byte blue)
        {
            hue /= 360d;
            if (saturation < double.Epsilon)
            {
                red = FixByte(lightness * 255d);
                green = FixByte(lightness * 255d);
                blue = FixByte(lightness * 255d);
            }
            else
            {
                double var1 = 0;
                double var2 = 0;
                if (lightness < 0.5d)
                    var2 = lightness * (1 + saturation);
                else
                    var2 = lightness + saturation - saturation * lightness;
                var1 = 2 * lightness - var2;
                red = FixByte(255d * Hue2RGB(var1, var2, hue + 1 / 3d));
                green = FixByte(255d * Hue2RGB(var1, var2, hue));
                blue = FixByte(255d * Hue2RGB(var1, var2, hue - 1 / 3d));
            }
        }

        private static double Hue2RGB(double v1, double v2, double vh)
        {
            if (vh < 0)
                vh += 1;
            if (vh > 1)
                vh -= 1;
            if ((6 * vh) < 1)
                return v1 + (v2 - v1) * 6 * vh;
            if ((2 * vh) < 1)
                return v2;
            if ((3 * vh) < 2)
                return v1 + (v2 - v1) * (2 / 3d - vh) * 6;
            return v1;
        }

        public static byte FixByte(double value)
        {
            if (value > 255)
                return 255;
            if (value < 0)
                return 0;
            else
                return (byte)Math.Round(value);
        }

        public static byte FixByte(int value)
        {
            if (value > 255)
                return 255;
            if (value < 0)
                return 0;
            return (byte)value;
        }
    }
}
