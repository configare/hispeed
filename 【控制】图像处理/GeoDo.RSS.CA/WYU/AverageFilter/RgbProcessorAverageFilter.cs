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
    public class RgbProcessorAverageFilter : RgbProcessorByWnd
    {
        public RgbProcessorAverageFilter()
            : base()
        {
            _name = "平均值滤波";
        }

        public RgbProcessorAverageFilter(RgbProcessorArg arg)
            : base(arg)
        {
        }

        public RgbProcessorAverageFilter(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
        }

        protected override void BeforeProcess()
        {
            base.BeforeProcess();
        }

        protected override unsafe void Process(byte[][] wndPixels, ref byte* ptr)
        {
            for (int band = 0; band < _bytesPerPixel; band++)
            {
               *(ptr++) = (byte)wndPixels[band].Average(s => s);
            }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("Height");
            if (_actualArg == null)
            {
                _actualArg = new RgbWndProcessorArg();
            }
            subElem.InnerText = _actualArg.WndHeight.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Width");
            subElem.InnerText = _actualArg.WndWidth.ToString();
            xmlElem.AppendChild(subElem);
            return xmlElem;
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            RgbProcessorAverageFilter pro = new RgbProcessorAverageFilter();
            pro._actualArg = new RgbWndProcessorArg();
            pro._actualArg.WndHeight = Convert.ToInt32(elem.ChildNodes[0].InnerText);
            pro._actualArg.WndWidth = Convert.ToInt32(elem.ChildNodes[1].InnerText);
            pro.Arguments = pro._actualArg;
            return pro;
        }
    }
}
