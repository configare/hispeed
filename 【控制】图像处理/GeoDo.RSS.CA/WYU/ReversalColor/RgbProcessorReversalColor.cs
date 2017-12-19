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
    public unsafe class RgbProcessorReversalColor : RgbProcessorByPixel
    {
        protected ReversalColorArg _actualArg = null;

        public RgbProcessorReversalColor()
            : base()
        {
            Init();
        }

        public RgbProcessorReversalColor(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public RgbProcessorReversalColor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "反相";
        }

        protected override void BeforeProcess()
        {
            _actualArg = _arg as ReversalColorArg;
        }

        protected override void Process(ref byte pixelValue)
        {
            pixelValue = (byte)(255 - pixelValue);
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            if (_actualArg.ApplyR)
                pixelRed = (byte)(255 - pixelRed);
            if (_actualArg.ApplyG)
                pixelGreen = (byte)(255 - pixelGreen);
            if (_actualArg.ApplyB)
                pixelBlue = (byte)(255 - pixelBlue);
        }

        public override void CreateDefaultArguments()
        {
            _arg = new ReversalColorArg();
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArg == null)
                _actualArg = new ReversalColorArg();
            return _actualArg.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            RgbProcessorReversalColor pro = new RgbProcessorReversalColor();
            pro._actualArg = new ReversalColorArg();
            string val = elem.InnerText;
            pro._actualArg.ApplyR = val.Contains('R');
            pro._actualArg.ApplyG = val.Contains('G');
            pro._actualArg.ApplyB = val.Contains('B');
            pro.Arguments = pro._actualArg;
            return pro;
        }
    }
}
