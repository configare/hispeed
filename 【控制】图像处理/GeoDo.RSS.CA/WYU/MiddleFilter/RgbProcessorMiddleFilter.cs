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
    public class RgbProcessorMiddleFilter:RgbProcessorByWnd
    {
        protected int _midIndex = 0;
        protected MiddleFilterArg _actualArgs = null;

        public RgbProcessorMiddleFilter()
            : base()
        {
            _name = "中值滤波";
        }

        public RgbProcessorMiddleFilter(RgbProcessorArg arg)
            : base(arg)
        {
            
        }

        public RgbProcessorMiddleFilter(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
        }

        protected override void BeforeProcess()
        {
            base.BeforeProcess();
            _actualArgs = _arg as MiddleFilterArg;
            _midIndex = (_actualArg.WndHeight * _actualArg.WndWidth) / 2;
        }

        protected override unsafe void Process(byte[][] wndPixels, ref byte* ptr)
        {
            for (int band = 0; band < _bytesPerPixel; band++)
            {
                Array.Sort(wndPixels[band]);
                *(ptr++) = wndPixels[band][_midIndex];
            }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArgs == null)
                _actualArgs = new MiddleFilterArg();
            return _actualArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            RgbProcessorMiddleFilter pro = new RgbProcessorMiddleFilter();
            pro._actualArgs = new MiddleFilterArg();
            pro._actualArgs.WndHeight = Convert.ToInt32(elem.ChildNodes[0].InnerText);
            pro._actualArgs.WndWidth = Convert.ToInt32(elem.ChildNodes[1].InnerText);
            pro.Arguments = pro._actualArgs;
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new MiddleFilterArg();
        }
    }
}
