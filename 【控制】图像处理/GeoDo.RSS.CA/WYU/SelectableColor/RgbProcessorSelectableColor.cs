using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using System.Drawing;
using GeoDo.RSS.Core.CA;
using System.IO;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessor))]
    public unsafe class RgbProcessorSelectableColor : RgbProcessorByPixel
    {
        private SelectableColorArg _actualArgs = null;

        public RgbProcessorSelectableColor()
            : base()
        {
            Init();
        }

        public RgbProcessorSelectableColor(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public RgbProcessorSelectableColor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "可选颜色";
        }

        protected override void BeforeProcess()
        {
            _actualArgs = _arg as SelectableColorArg;
        }

        protected override void Process(ref byte pixelValue)
        {
            Process(ref pixelValue, ref pixelValue, ref pixelValue);
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            bool needAdjust = false;
            int[] minHues = null;
            int[] maxHues = null;
            foreach (SelectableColorArgItem item in _actualArgs.Items)
            {
                if (item.IsEmpty())
                    continue;
                GetAffectHueRange(item.TargetColor, out minHues, out maxHues);
                float hue = ColorMath.ComputeHue(pixelRed, pixelGreen, pixelBlue);
                if (float.IsNaN(hue))
                    continue;
                needAdjust = false;
                if (minHues == null)
                {
                    switch (item.TargetColor)
                    {
                        case enumSelectableColor.Black:
                            needAdjust = pixelBlue < 128 && pixelGreen < 128 && pixelRed < 128;
                            break;
                        case enumSelectableColor.Neutrals:
                            needAdjust = !((pixelBlue == 0 && pixelGreen == 0 && pixelRed == 0) ||
                                                   (pixelBlue == 255 && pixelGreen == 255 && pixelRed == 255));
                            break;
                        case enumSelectableColor.White:
                            needAdjust = pixelBlue > 128 && pixelGreen > 128 && pixelRed > 128;
                            break;
                    }
                }
                else
                {
                    for (int ihue = 0; ihue < minHues.Length; ihue++)
                    {
                        if (hue >= minHues[ihue] && hue < maxHues[ihue])
                        {
                            needAdjust = true;
                            break;
                        }
                    }
                }
                if (needAdjust)
                    AdjustColor(item, ref pixelBlue, ref  pixelGreen, ref pixelRed);
            }
        }

        private void AdjustColor(SelectableColorArgItem item, ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            byte lim = GetLIM(item, ref pixelBlue, ref pixelGreen, ref pixelRed);
            byte inc = 0, dec = 0;

            if (item.CyanAdjustValue != 0)
            {
                GetDLT(lim, pixelRed, ref inc, ref dec);
                ApplyDLT(lim, ref pixelRed, item.CyanAdjustValue, inc, dec);
            }

            if (item.MagentaAdjustValue != 0)
            {
                GetDLT(lim, pixelGreen, ref inc, ref dec);
                ApplyDLT(lim, ref pixelGreen, item.MagentaAdjustValue, inc, dec);
            }

            if (item.YellowAdjustValue != 0)
            {
                GetDLT(lim, pixelBlue, ref inc, ref dec);
                ApplyDLT(lim, ref pixelBlue, item.YellowAdjustValue, inc, dec);
            }

            if (item.BlackAdjustValue != 0)
            {
                GetDLT(lim, pixelRed, ref inc, ref dec);
                ApplyDLT(lim, ref pixelRed, item.BlackAdjustValue, inc, dec);
                GetDLT(lim, pixelGreen, ref inc, ref dec);
                ApplyDLT(lim, ref pixelGreen, item.BlackAdjustValue, inc, dec);
                GetDLT(lim, pixelBlue, ref inc, ref dec);
                ApplyDLT(lim, ref pixelBlue, item.BlackAdjustValue, inc, dec);
            }
        }

        /// <summary>
        /// 应用调整量
        /// </summary>
        /// <param name="lim"></param>
        /// <param name="p"></param>
        /// <param name="adjPercent"></param>
        /// <param name="inc"></param>
        /// <param name="dec"></param>
        private void ApplyDLT(byte lim, ref byte p, float adjPercent, byte inc, byte dec)
        {
            if (p > 128)
                dec = inc;
            float incp = inc / (float)lim;
            float decp = dec / (float)lim;
            adjPercent /= 100f;
            if (adjPercent < 0)
            {
                adjPercent = Math.Min(Math.Abs(adjPercent), incp);
                p += ColorMath.FixByte(lim * adjPercent);
            }
            else if (adjPercent > 0)
            {
                adjPercent = Math.Min(adjPercent, decp);
                p -= ColorMath.FixByte(lim * adjPercent);
            }
        }

        /// <summary>
        /// 计算最大、最小调整量
        /// </summary>
        /// <param name="lim"></param>
        /// <param name="rgb"></param>
        /// <param name="inc"></param>
        /// <param name="dec"></param>
        private static void GetDLT(byte lim, byte rgb, ref byte inc, ref byte dec)
        {
            inc = ColorMath.FixByte(lim * (1 - rgb / 255f));
            dec = ColorMath.FixByte(lim * rgb / 255f);
        }

        /// <summary>
        /// 计算总调整量
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pixelBlue"></param>
        /// <param name="pixelGreen"></param>
        /// <param name="pixelRed"></param>
        /// <returns></returns>
        private static byte GetLIM(SelectableColorArgItem item, ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            //byte min = Math.Min(pixelBlue, Math.Min(pixelGreen, pixelRed));
            //byte max = Math.Max(pixelBlue, Math.Max(pixelGreen, pixelRed));
            //byte mid = 0;
            //bool mided = false;
            //if (pixelBlue != min && pixelBlue != max)
            //{
            //    mid = pixelBlue;
            //    mided = true;
            //}
            //else if (pixelGreen != min && pixelGreen != max)
            //{
            //    mid = pixelGreen;
            //    mided = true;
            //}
            //else if (pixelRed != min && pixelRed != max)
            //{
            //    mid = pixelRed;
            //    mided = true;
            //}
            //if (!mided)
            //    mid = min;
            byte min, max, mid, tmp;
            max = pixelRed;
            min = pixelGreen;
            mid = pixelBlue;
            if (max < mid)
            {
                tmp = max;
                max = mid;
                mid = tmp;
            }
            if (max < min)
            {
                tmp = max;
                max = min;
                min = tmp;
            }
            if (min > mid)
            {
                tmp = min;
                min = mid;
                mid = tmp;
            }
            switch (item.TargetColor)
            {
                case enumSelectableColor.Red:
                case enumSelectableColor.Green:
                case enumSelectableColor.Blue:
                    return ColorMath.FixByte(max - mid);
                case enumSelectableColor.Yellow:
                case enumSelectableColor.Cyan:
                case enumSelectableColor.Magenta:
                    return ColorMath.FixByte(mid - min);
                case enumSelectableColor.Neutrals:
                    return ColorMath.FixByte(255 - (Math.Abs(max - 127.5) + Math.Abs(min - 127.5)));
                case enumSelectableColor.White:
                    return ColorMath.FixByte((min - 127.5) * 2);
                case enumSelectableColor.Black:
                    return ColorMath.FixByte((127.5 - max) * 2);
            }
            return 0;
        }

        private void GetAffectHueRange(enumSelectableColor SelectableColor, out int[] minHues, out int[] maxHues)
        {
            minHues = null;
            maxHues = null;
            switch (SelectableColor)
            {
                case enumSelectableColor.Red:
                    minHues = new int[2] { 0, 300 };
                    maxHues = new int[2] { 60, 360 };
                    break;
                case enumSelectableColor.Yellow:
                    minHues = new int[] { 0 };
                    maxHues = new int[] { 120 };
                    break;
                case enumSelectableColor.Green:
                    minHues = new int[] { 60 };
                    maxHues = new int[] { 180 };
                    break;
                case enumSelectableColor.Cyan:
                    minHues = new int[] { 120 };
                    maxHues = new int[] { 240 };
                    break;
                case enumSelectableColor.Blue:
                    minHues = new int[] { 180 };
                    maxHues = new int[] { 300 };
                    break;
                case enumSelectableColor.Magenta:
                    minHues = new int[] { 240 };
                    maxHues = new int[] { 360 };
                    break;
                default:
                    return;
            }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArgs == null)
                _actualArgs = new SelectableColorArg();
            return _actualArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            RgbProcessorSelectableColor pro = new RgbProcessorSelectableColor();
            pro._actualArgs = new SelectableColorArg();
            switch (elem.ChildNodes[0].InnerText)
            {
                case "Absolute":
                    pro._actualArgs.ApplyType = enumSelectableColorApplyType.Absolute;
                    break;
                case "Relative":
                    pro._actualArgs.ApplyType = enumSelectableColorApplyType.Relative;
                    break;
            }
            enumSelectableColor enumColor = enumSelectableColor.Red;
            for (int i = 1; i < elem.ChildNodes.Count; i++)
            {
                switch (elem.ChildNodes[i].Name)
                {
                    case "Red":
                        enumColor = enumSelectableColor.Red;
                        break;
                    case "Yellow":
                        enumColor = enumSelectableColor.Yellow;
                        break;
                    case "Green":
                        enumColor = enumSelectableColor.Green;
                        break;
                    case "Cyan":
                        enumColor = enumSelectableColor.Cyan;
                        break;
                    case "Blue":
                        enumColor = enumSelectableColor.Blue;
                        break;
                    case "Magenta":
                        enumColor = enumSelectableColor.Magenta;
                        break;
                    case "Black":
                        enumColor = enumSelectableColor.Black;
                        break;
                    case "Neutrals":
                        enumColor = enumSelectableColor.Neutrals;
                        break;
                    case "White":
                        enumColor = enumSelectableColor.White;
                        break;
                }
                pro._actualArgs.GetSelectableColorArgItem(enumColor).CyanAdjustValue = Convert.ToInt32(elem.ChildNodes[i].ChildNodes[0].InnerText);
                pro._actualArgs.GetSelectableColorArgItem(enumColor).MagentaAdjustValue = Convert.ToInt32(elem.ChildNodes[i].ChildNodes[1].InnerText);
                pro._actualArgs.GetSelectableColorArgItem(enumColor).YellowAdjustValue = Convert.ToInt32(elem.ChildNodes[i].ChildNodes[2].InnerText);
                pro._actualArgs.GetSelectableColorArgItem(enumColor).BlackAdjustValue = Convert.ToInt32(elem.ChildNodes[i].ChildNodes[3].InnerText);
            }
            pro.Arguments = pro._actualArgs;
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new SelectableColorArg();
        }
    }
}
