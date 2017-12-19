using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GeoDo.RSS.Core.CA
{
    public abstract class RgbProcessorArg
    {
        public RgbProcessorArg()
        {
        }

        public abstract XmlElement ToXML(XmlDocument xmldoc);

        public abstract RgbProcessorArg Clone();
    }
}
