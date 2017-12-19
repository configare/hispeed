using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    /// <summary>
    /// 亮度/对比度的算法类
    /// </summary>
    [Export(typeof(IRgbProcessor))]
    public class BrightContrastProcessor : RgbProcessorByPixel
    {
        private BrightContrastArg _bcArgs = null;
        private byte[] _rgbs = null;
        private const int PARA_1 = 100;
        private const double PARA_2 = 0.01;
        private const double PARA_MIDDLE = 127;

        public BrightContrastProcessor()
            : base()
        {
            Init();
        }

        public BrightContrastProcessor(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public BrightContrastProcessor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "亮度/对比度";
        }

        protected override void BeforeProcess()
        {
            _bcArgs = _arg as BrightContrastArg;
            int bright = _bcArgs.BrightAdjustValue;
            double change = _bcArgs.Brightchangevalue;
            double dstepC = (_bcArgs.ContrastAdjustValue + PARA_1) * PARA_2;
            if (dstepC > 1)
            {
                dstepC = dstepC * dstepC * dstepC;
            }
            double bChange = bright * change * dstepC - PARA_MIDDLE * dstepC + PARA_MIDDLE;

            _rgbs = new byte[256];
            if (_bcArgs.IsChanged)
                for (int i = 0; i < 256; i++)
                    _rgbs[i] = ColorMath.FixByte(i * dstepC + bChange);
            else
                for (int i = 0; i < 256; i++)
                    _rgbs[i] = (byte)i;
        }

        /// <summary>
        /// 单色图像处理
        /// </summary>
        /// <param name="pixelValue"></param>
        protected override void Process(ref byte pixelValue)
        {
            pixelValue = _rgbs[pixelValue];
        }

        /// <summary>
        /// RGB图像处理方法
        /// </summary>
        /// <param name="pixelBlue"></param>
        /// <param name="pixelGreen"></param>
        /// <param name="pixelRed"></param>
        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            switch (_bcArgs.Channel)
            {
                case enumChannel.RGB:
                    {
                        pixelBlue = _rgbs[pixelBlue];
                        pixelGreen = _rgbs[pixelGreen];
                        pixelRed = _rgbs[pixelRed];
                        break;
                    }
                case enumChannel.R:
                    {
                        pixelRed = _rgbs[pixelRed];
                        break;
                    }
                case enumChannel.G:
                    {
                        pixelGreen = _rgbs[pixelGreen];
                        break;
                    }
                case enumChannel.B:
                    {
                        pixelBlue = _rgbs[pixelBlue];
                        break;
                    }
            }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_bcArgs == null)
                _bcArgs = new BrightContrastArg();
            return _bcArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            BrightContrastProcessor pro = new BrightContrastProcessor();
            pro._bcArgs = new BrightContrastArg();
            switch (elem.ChildNodes[0].InnerText)
            {
                case "RGB":
                    pro._bcArgs.Channel = enumChannel.RGB;
                    break;
                case "Red":
                    pro._bcArgs.Channel = enumChannel.R;
                    break;
                case "Green":
                    pro._bcArgs.Channel = enumChannel.G;
                    break;
                case "Blue":
                    pro._bcArgs.Channel = enumChannel.B;
                    break;
            }
            pro._bcArgs.Brightchangevalue = Convert.ToDouble(elem.ChildNodes[1].InnerText);
            pro._bcArgs.BrightAdjustValue = Convert.ToInt32(elem.ChildNodes[2].InnerText);
            pro._bcArgs.ContrastAdjustValue = Convert.ToInt32(elem.ChildNodes[3].InnerText);
            pro._bcArgs.IsChanged = true;
            pro.Arguments = pro._bcArgs;
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new BrightContrastArg();
        }
    }
}
