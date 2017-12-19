using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessor))]
    public class RgbProcessorLogEnhance : RgbProcessorByPixel
    {
        protected LogEnhanceArg _actualArgs = null;
        private byte[] _rgbs0 = null;
        private byte[] _rgbs1 = null;
        private byte[] _rgbs2 = null;
        private byte[] _rgbs3 = null;

        public RgbProcessorLogEnhance()
            : base()
        {
            _name = "对数增强";
        }

        public RgbProcessorLogEnhance(RgbProcessorArg arg)
            : base(arg)
        {
        }

        public RgbProcessorLogEnhance(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
        }

        protected override void BeforeProcess()
        {
            _actualArgs = _arg as LogEnhanceArg;
            _rgbs0 = new byte[256];
            _rgbs1 = new byte[256];
            _rgbs2 = new byte[256];
            _rgbs3 = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                _rgbs0[i] = ColorMath.FixByte(_actualArgs.Scales[0] * Math.Log(1 + i, _actualArgs.LogBases[0]));
                _rgbs1[i] = ColorMath.FixByte(_actualArgs.Scales[1] * Math.Log(1 + i, _actualArgs.LogBases[1]));
                _rgbs2[i] = ColorMath.FixByte(_actualArgs.Scales[2] * Math.Log(1 + i, _actualArgs.LogBases[2]));
                _rgbs3[i] = ColorMath.FixByte(_actualArgs.Scales[3] * Math.Log(1 + i, _actualArgs.LogBases[3]));
            }
        }

        protected override void Process(ref byte pixelValue)
        {
            if (_actualArgs.Scales[0] != 0)
                pixelValue = _rgbs0[pixelValue];
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            if (_actualArgs.Scales[0] == 0)
            {
                if (_actualArgs.Scales[1] != 0)
                    pixelRed = _rgbs1[pixelRed];
                if (_actualArgs.Scales[2] != 0)
                    pixelGreen = _rgbs2[pixelGreen];
                if (_actualArgs.Scales[3] != 0)
                    pixelBlue = _rgbs3[pixelBlue];
            }
            else
            {
                pixelRed = _rgbs0[pixelRed];
                pixelGreen = _rgbs0[pixelGreen];
                pixelBlue = _rgbs0[pixelBlue];
            }
        }

        public override void CreateDefaultArguments()
        {
            _arg = new LogEnhanceArg();
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArgs == null)
                _actualArgs = new LogEnhanceArg();
            return _actualArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement ele)
        {
            RgbProcessorLogEnhance pro = new RgbProcessorLogEnhance();
            pro._actualArgs = new LogEnhanceArg();
            pro._actualArgs.IsSameArg = Convert.ToBoolean(ele.ChildNodes[0].InnerText);
            for (int i = 0; i < 4; i++)
            {
                pro._actualArgs.LogBases[i] = Convert.ToDouble(ele.ChildNodes[1 + i].InnerText);
            }
            for (int i = 0; i < 4; i++)
            {
                pro._actualArgs.Scales[i] = Convert.ToInt16(ele.ChildNodes[5 + i].InnerText);
            }
            pro.Arguments = pro._actualArgs;
            return pro;
        }
    }
}
