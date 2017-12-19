using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using System.Drawing;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessor))]
    public class RgbProcessorReplacedColor : RgbProcessorByPixel
    {
        private ReplaceColorArg _actualArgs = null;
        private byte _red = 0;
        private byte _green = 0;
        private byte _blue = 0;
        private int _colorTorence = 0;
        private int _hue = 0;
        private int _saturation = 0;
        private int _lightness = 0;

        public RgbProcessorReplacedColor():base()
        {
            Init();
        }

        public RgbProcessorReplacedColor(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "颜色替换";
        }

        protected override void BeforeProcess()
        {
            _actualArgs = _arg as ReplaceColorArg;
            _red = _actualArgs.TargetColor.R;
            _green = _actualArgs.TargetColor.G;
            _blue = _actualArgs.TargetColor.B;
            _colorTorence = _actualArgs.ColorTorence;
            _hue =(int) _actualArgs.Hue;
            _saturation = (int)_actualArgs.Saturation;
            _lightness = (int)_actualArgs.Lightness;
        }

        protected override void Process(ref byte pixelValue)
        {
            Process(ref pixelValue, ref pixelValue, ref pixelValue);
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            float diff = ColorMath.ColorDistance(pixelRed, pixelGreen, pixelBlue,_red,_green,_blue);
            bool needAdjust = diff < _colorTorence;
            if (needAdjust)
                ColorAdjust(ref pixelRed, ref pixelGreen, ref pixelBlue, _hue,_saturation,_lightness);
        }

        /// <summary>
        /// 色相、饱和度、明度调整
        /// </summary>
        /// <param name="red">0~255</param>
        /// <param name="green">0~255</param>
        /// <param name="blue">0~255</param>
        /// <param name="hValue">色相(Hue)，单位：度</param>
        /// <param name="sValue">饱和度(Saturation),单位：-100~100</param>
        /// <param name="bValue">明度(Value,Brightness),单位：-100~100</param>
        public void ColorAdjust(ref byte red, ref byte green, ref byte blue, int hValue, int sValue, int bValue)
        {
            sValue = sValue * 255 / 100;
            bValue = bValue * 255 / 100;
            if (sValue > 0 && bValue != 0)
                SetBrightness(ref red, ref green, ref blue, bValue);
            SetHueSaturation(ref red, ref green, ref blue, hValue, sValue);
            if (bValue != 0 && sValue <= 0)
                SetBrightness(ref red, ref green, ref blue, bValue);
        }

        private void SetBrightness(ref byte red, ref byte green, ref byte blue, int bValue)
        {
            int r = red;
            int g = green;
            int b = blue;
            if (bValue > 0)
            {
                r = r + (255 - r) * bValue / 255;
                g = g + (255 - g) * bValue / 255;
                b = b + (255 - b) * bValue / 255;
            }
            else if (bValue < 0)
            {
                r = r + r * bValue / 255;
                g = g + g * bValue / 255;
                b = b + b * bValue / 255;
            }
            CheckRGB(ref r);
            CheckRGB(ref g);
            CheckRGB(ref b);
            AssignRGB(ref red, ref green, ref blue, r, g, b);
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
            int h = 0, s = 0, l = 0;
            l = entire >> 1;
            if (l < 128)
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
                int lum = l - 128;
                if (lum > 0)
                    g += (((255 - g) * lum + 64) / 128);
                else
                    g += (g * lum / 128);
                CheckRGB(ref g);
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
                    sValue = 65025 / sValue - 255;
                }
                r += ((r - l) * sValue / 255);
                g += ((g - l) * sValue / 255);
                b += ((b - l) * sValue / 255);
                CheckRGB(ref r);
                CheckRGB(ref g);
                CheckRGB(ref b);
            }
            AssignRGB(ref red, ref green, ref blue, r, g, b);
        }

        private void CheckRGB(ref int value)
        {
            if (value < 0)
                value = 0;
            else if (value > 255)
                value = 255;
        }

        private static void AssignRGB(ref byte red, ref byte green, ref byte blue, int r, int g, int b)
        {
            red = ColorMath.FixByte(r);
            green = ColorMath.FixByte(g);
            blue = ColorMath.FixByte(b);
        }

        private static void SwapRGB(ref int a, ref int b)
        {
            a += b;
            b = a - b;
            a -= b;
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArgs == null)
                _actualArgs = new ReplaceColorArg();
            return _actualArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            RgbProcessorReplacedColor pro = new RgbProcessorReplacedColor();
            pro._actualArgs = new ReplaceColorArg();
            pro._actualArgs.TargetColor = (Color)Color.FromArgb(Convert.ToInt32(elem.ChildNodes[0].InnerText));
            pro._actualArgs.ColorTorence = Convert.ToInt32(elem.ChildNodes[1].InnerText);
            pro._actualArgs.Hue = Convert.ToDouble(elem.ChildNodes[2].InnerText);
            pro._actualArgs.Saturation = Convert.ToDouble(elem.ChildNodes[3].InnerText);
            pro._actualArgs.Lightness = Convert.ToDouble(elem.ChildNodes[4].InnerText);
            pro.Arguments = pro._actualArgs;
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new ReplaceColorArg();
        }

    }
}
