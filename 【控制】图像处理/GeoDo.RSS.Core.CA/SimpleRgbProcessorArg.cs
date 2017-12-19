using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GeoDo.RSS.Core.CA
{
    public class SimpleRgbProcessorArg : RgbProcessorArg
    {
        protected Dictionary<string, object> _arguments = null;

        public SimpleRgbProcessorArg()
        {
        }

        public SimpleRgbProcessorArg(Dictionary<string, object> arguments)
        {
            _arguments = arguments;
        }

        public Dictionary<string, object> Arguments
        {
            get { return _arguments; }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlelem = xmldoc.CreateElement("null");
            return xmlelem;
        }

        public override RgbProcessorArg Clone()
        {
            Dictionary<string, object> target = new Dictionary<string, object>();
            if (_arguments == null || _arguments.Count == 0)
                return new SimpleRgbProcessorArg(target);
            foreach (string key in _arguments.Keys)
                target.Add(key, _arguments[key]);
            return new SimpleRgbProcessorArg(target);
        }
    }
}
