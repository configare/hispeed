using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.GDAL.H5BandPrd
{
    internal class DefaultBandDatasetDef
    {
        public string Name = null;
        public string BandNoAttribute = null;

        public DefaultBandDatasetDef(string name, string bandnoatt)
        {
            Name = name;
            BandNoAttribute = bandnoatt;
        }

        public static DefaultBandDatasetDef FromXElement(XElement element)
        {
            if (element == null)
                return null;
            var att = element.Attribute(XName.Get("name"));
            string name = att != null ? att.Value : string.Empty;
            att = element.Attribute(XName.Get("bandnoatt"));
            string value = att != null ? att.Value : string.Empty;
            return new DefaultBandDatasetDef(name, value);
        }
    }
}
