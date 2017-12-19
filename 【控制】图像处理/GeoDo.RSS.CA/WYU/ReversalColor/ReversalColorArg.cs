using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.CA;
using System.Xml;

namespace GeoDo.RSS.CA
{
    public class ReversalColorArg : RgbProcessorArg
    {
        private bool _r = true, _g = true, _b = true;

        public bool ApplyR
        {
            get { return _r; }
            set { _r = value; }
        }

        public bool ApplyG
        {
            get { return _g; }
            set { _g = value; }
        }

        public bool ApplyB
        {
            get { return _b; }
            set { _b = value; }
        }

        public override System.Xml.XmlElement ToXML(System.Xml.XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            string val = (_r ? "R" : "") + (_g ? "G" : "") + (_b ? "B" : "");
            xmlElem.InnerText = val;
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            ReversalColorArg arg = new ReversalColorArg();
            return arg;
        }
    }
}
