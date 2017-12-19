using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.GDAL.H5BandPrd
{
    internal class IdentifyAttDef
    {
        public string Name = null;
        public string Value = null;

        public IdentifyAttDef(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static IdentifyAttDef FromXElement(XElement element)
        {
            if (element == null)
                return null;
            var att = element.Attribute(XName.Get("name"));
            string name = att != null ? att.Value : string.Empty;
            att = element.Attribute(XName.Get("value"));
            string value = att != null ? att.Value : string.Empty;
            return new IdentifyAttDef(name, value);
        }
    }
}
