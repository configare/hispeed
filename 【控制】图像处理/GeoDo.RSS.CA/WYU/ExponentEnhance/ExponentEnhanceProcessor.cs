using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.CA;
using System.ComponentModel.Composition;
using System.Xml;

namespace GeoDo.RSS.CA
{
    /// <summary>
    /// 刘诚提供的指数增强函数
    /// PixelNew = 255 * (PiexlOld - Min)^2 / (Max - Min)^2
    /// </summary>
    [Export(typeof(IRgbProcessor))]
    public unsafe class ExponentEnhanceProcessor : RgbProcessorByPixel
    {
        private ExponentEnhanceArg _exp2Arg = null;
        private byte[] _rgbs0 = null;
        private byte[] _rgbs1 = null;
        private byte[] _rgbs2 = null;

        public ExponentEnhanceProcessor()
            : base()
        {
            _name = "指数增强";
        }

        public ExponentEnhanceProcessor(RgbProcessorArg arg)
            : base(arg)
        {
        }

        public ExponentEnhanceProcessor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
        }

        protected override void BeforeProcess()
        {
            base.BeforeProcess();
            _exp2Arg = _arg as ExponentEnhanceArg;
            _rgbs0 = new byte[256];
            _rgbs1 = new byte[256];
            _rgbs2 = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                _rgbs0[i] = ColorMath.FixByte(255 * Math.Pow(i - _exp2Arg.Mins[0], 2) / Math.Pow(_exp2Arg.Maxs[0] - _exp2Arg.Mins[0], 2));
                _rgbs1[i] = ColorMath.FixByte(255 * Math.Pow(i - _exp2Arg.Mins[1], 2) / Math.Pow(_exp2Arg.Maxs[1] - _exp2Arg.Mins[1], 2));
                _rgbs2[i] = ColorMath.FixByte(255 * Math.Pow(i - _exp2Arg.Mins[2], 2) / Math.Pow(_exp2Arg.Maxs[2] - _exp2Arg.Mins[2], 2));
            }
        }

        private void ProcessValue(ref byte p, int min, int max)
        {
            if (min == max)
                return;
            p = ColorMath.FixByte(255 * Math.Pow(p - min, 2) / Math.Pow(max - min, 2));
        }

        protected override void Process(ref byte pixelValue)
        {
            pixelValue = _rgbs0[pixelValue];
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            if (_exp2Arg.IsSameArg)
            {
              pixelRed = _rgbs0[pixelRed]; 
              pixelGreen = _rgbs0[pixelGreen];
              pixelBlue = _rgbs0[pixelBlue]; 
            }
            else
            {
                pixelRed = _rgbs0[pixelRed]; 
                pixelGreen = _rgbs1[pixelGreen];
                pixelBlue = _rgbs2[pixelBlue]; 
            }
        }

        public override void CreateDefaultArguments()
        {
            _arg = new ExponentEnhanceArg();
        }

        public override System.Xml.XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_exp2Arg == null)
                _exp2Arg = new ExponentEnhanceArg();
            return _exp2Arg.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement ele)
        {
            ExponentEnhanceProcessor pro = new ExponentEnhanceProcessor();
            pro._exp2Arg = new ExponentEnhanceArg();
            pro._exp2Arg.IsSameArg = Convert.ToBoolean(ele.ChildNodes[0].InnerText);
            for (int i = 0; i < 3; i++)
            {
                pro._exp2Arg.Mins[i] = Convert.ToInt16(ele.ChildNodes[1 + i].InnerText);
            }
            for (int i = 0; i < 3; i++)
            {
                pro._exp2Arg.Maxs[i] = Convert.ToInt16(ele.ChildNodes[4 + i].InnerText);
            }
            pro.Arguments = pro._exp2Arg;
            return pro;
        }
    }
}
