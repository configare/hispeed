using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;
using System.Diagnostics;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessor))]
    public class HueSaturationProcess : RgbProcessorByPixel
    {
        private HSLColorArg _actualArgs = null;
        private int _sValue = 0;
        private int _bValue = 0;
        private int _hValue = 0;

        public HueSaturationProcess()
            : base()
        {
            Init();
        }

        public HueSaturationProcess(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public HueSaturationProcess(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "色相、饱和度";
        }

        protected override void BeforeProcess()
        {
            _actualArgs = _arg as HSLColorArg;
            _sValue = _actualArgs.PixelSaturation;
            _bValue = _actualArgs.PixelLum * 255 / 100;
            _hValue = _actualArgs.PixelHue * 255 / 100;
        }

        protected override void Process(ref byte pixelValue)
        {
            int bValue = _actualArgs.PixelLum * 255 / 100;
            pixelValue += (byte)bValue;
            pixelValue = pixelValue > (byte)255 ? (byte)255 : pixelValue;
            pixelValue = pixelValue < (byte)0 ? (byte)0 : pixelValue;
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            if (_sValue > 0 && _bValue != 0)
                SetBrightness(ref pixelRed, ref pixelGreen, ref pixelBlue, _bValue);
            SetHueSaturation(ref pixelRed, ref pixelGreen, ref pixelBlue, _hValue, _sValue);
            if (_bValue != 0 && _sValue <= 0)
                SetBrightness(ref pixelRed, ref pixelGreen, ref pixelBlue, _bValue);
        }

        private void SetBrightness(ref byte red, ref byte green, ref byte blue, int bValue)
        {
            if (bValue > 0)
            {
                red = ColorMath.FixByte(red + (255 - red) * bValue / 255);
                green = ColorMath.FixByte(green + (255 - green) * bValue / 255);
                blue = ColorMath.FixByte(blue + (255 - blue) * bValue / 255);
            }
            else if (bValue < 0)
            {
                red = ColorMath.FixByte(red + red * bValue / 255);
                green = ColorMath.FixByte(green + green * bValue / 255);
                blue = ColorMath.FixByte(blue + blue * bValue / 255);
            }
        }

        private void SetHueSaturation(ref byte red, ref byte green, ref byte blue, int hValue, int sValue)
        {
            int r = red;
            int g = green;
            int b = blue;

            if (r < g)
                SwapRGB(ref r, ref g);
            if (r < b)
                SwapRGB(ref r, ref b);
            if (b > g)
                SwapRGB(ref b, ref g);
            int delta = r - b;
            if (delta == 0)
                return;
            int entire = r + b;
            int h = 0, s = 0, L = 0;
            L = entire >> 1;
            if (L < 128)
                s = delta * 255 / entire;
            else
                s = delta * 255 / (510 - entire);
            if (hValue == 0)
            {
                r = red;
                g = green;
                b = blue;
            }
            else
            {
                if (r == red)
                    h = (green - blue) * 60 / delta;
                else if (r == green)
                    h = (blue - red) * 60 / delta + 120;
                else
                    h = (red - green) * 60 / delta + 240;
                h += hValue;
                while (h < 0)
                    h += 360;
                while (h > 360)
                    h -= 360;
                int index = h / 60;
                int extra = h % 60;
                if ((index & 1) != 0)
                    extra = 60 - extra;
                extra = (extra * 255 + 30) / 60;
                g = extra - (extra - 128) * (255 - s) / 255;
                int lum = L - 128;
                if (lum > 0)
                    g += (((255 - g) * lum + 64) / 128);
                else
                    g += (g * lum / 128);
                g = ColorMath.FixByte(g);
                switch (index)
                {
                    case 1:
                        SwapRGB(ref r, ref g);
                        break;
                    case 2:
                        SwapRGB(ref r, ref b);
                        SwapRGB(ref g, ref b);
                        break;
                    case 3:
                        SwapRGB(ref r, ref b);
                        break;
                    case 4:
                        SwapRGB(ref r, ref g);
                        SwapRGB(ref g, ref b);
                        break;
                    case 5:
                        SwapRGB(ref g, ref b);
                        break;
                }
            }

            if (sValue != 0)
            {
                if (sValue > 0)
                {
                    sValue = sValue + s >= 255 ? s : 255 - sValue;
                    sValue = (255 * 255) / sValue - 255;
                }
                r = ColorMath.FixByte(r + ((r - L) * sValue / 255));
                g = ColorMath.FixByte(g + ((g - L) * sValue / 255));
                b = ColorMath.FixByte(b + ((b - L) * sValue / 255));
            }
            AssignRGB(ref red, ref green, ref blue, r, g, b);
        }

        private static void SwapRGB(ref int a, ref int b)
        {
            a += b;
            b = a - b;
            a -= b;
        }

        private static void AssignRGB(ref byte red, ref byte green, ref byte blue, int r, int g, int b)
        {
            red = ColorMath.FixByte(r);
            green = ColorMath.FixByte(g);
            blue = ColorMath.FixByte(b);
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArgs == null)
                _actualArgs = new HSLColorArg();
            return _actualArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            HueSaturationProcess pro = new HueSaturationProcess();
            pro._actualArgs = new HSLColorArg();
            pro._actualArgs.PixelHue = Convert.ToInt32(elem.ChildNodes[0].InnerText);
            pro._actualArgs.PixelSaturation = Convert.ToInt32(elem.ChildNodes[1].InnerText);
            pro._actualArgs.PixelLum = Convert.ToInt32(elem.ChildNodes[2].InnerText);
            pro.Arguments = pro._actualArgs;
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new HSLColorArg();
        }
    }
}
