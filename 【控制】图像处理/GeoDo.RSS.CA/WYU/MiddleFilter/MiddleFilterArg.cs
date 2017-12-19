using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    public class MiddleFilterArg : RgbWndProcessorArg
    {
        public MiddleFilterArg()
        {
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("Height");
            subElem.InnerText = _wndHeight.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Width");
            subElem.InnerText = _wndWidth.ToString();
            xmlElem.AppendChild(subElem);
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            MiddleFilterArg arg = new MiddleFilterArg();
            return arg;
        }
    }
}
