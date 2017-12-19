using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoDo.FileProject;
using GeoDo.RasterProject;

namespace GeoDo.Tools
{
    public class XmlHelper
    {
        public static string ParseXElementValueToString(XElement xml, XName name)
        {
            XElement subXml = xml.Element(name);
            return subXml == null ? "" : subXml.Value;
        }

        public static PrjEnvelopeItem[] ParseXElementEnvelopes(XElement xml, XName name)
        {
            XElement subXml = xml.Element(name);
            if (subXml == null || subXml.Elements("Envelope") == null)
                return null;
            IEnumerable<PrjEnvelopeItem> prjs = from item in subXml.Elements("Envelope")
                                                where item.Value != null
                                                select ParseEnvelope(item);
            return prjs.ToArray();
        }

        public static Dictionary<string, string> ParseXElementAttributeDic(XElement xml)
        {
            if (xml == null)
                return null;
            var attributes = xml.Attributes();
            if (attributes == null || attributes.Count() == 0)
                return null;
            Dictionary<string, string> attributeDic = new Dictionary<string, string>();
            foreach (var key in attributes.ToArray())
            {
                if (key.Value == null || string.IsNullOrEmpty(key.Value))
                    attributeDic.Add(key.Name.ToString(), "");
                else
                    attributeDic.Add(key.Name.ToString(), key.Value);
            }
            return attributeDic.Count == 0 ? null : attributeDic;
        }

        public static PrjEnvelopeItem ParseEnvelope(XElement xElement)
        {
            if (xElement == null || xElement.Value == null)
                return null;
            return new PrjEnvelopeItem(xElement.Attribute("name").Value,
                                         new PrjEnvelope(double.Parse(xElement.Attribute("minx").Value)
                                             , double.Parse(xElement.Attribute("maxx").Value)
                                             , double.Parse(xElement.Attribute("miny").Value)
                                             , double.Parse(xElement.Attribute("maxy").Value)));
        }

        public static object[] EnvelopesToXmlValue(PrjEnvelopeItem[] envelopes)
        {
            if (envelopes == null || envelopes.Length == 0)
                return null;
            List<XElement> envs = new List<XElement>();
            foreach (PrjEnvelopeItem item in envelopes)
            {
                envs.Add(EnvelopeToXmlValue(item));
            }
            return envs.ToArray();
        }

        public static XElement EnvelopeToXmlValue(PrjEnvelopeItem item)
        {
            if (item == null)
                return null;
            return new XElement("Envelope",
                        new XAttribute("name", item.Name),
                        new XAttribute("minx", item.PrjEnvelope.MinX),
                        new XAttribute("maxx", item.PrjEnvelope.MaxX),
                        new XAttribute("miny", item.PrjEnvelope.MinY),
                        new XAttribute("maxy", item.PrjEnvelope.MaxY));
        }

        public static XElement[] WriteFiles(OutFileArg[] outputFiles)
        {
            if (outputFiles == null || outputFiles.Length == 0)
                return null;
            List<XElement> files = new List<XElement>();
            for (int i = 0; i < outputFiles.Length; i++)
            {
                files.Add(outputFiles[i].ToXml());
            }
            return files.ToArray();
        }

        public static OutFileArg[] OutputFilesToXml(XElement xml, string p)
        {
            throw new NotImplementedException();
        }
    }
}
