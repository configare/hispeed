using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoDo.FileProject;
using GeoDo.RasterProject;

namespace GeoDo.Tools.Projection
{
    class MosaicOutputArg
    {
        public string Satellite { get; set; }

        public string Sensor { get; set; }

        public string Level { get; set; }

        public string ProjectionIdentify { get; set; }

        public string ObservationDate { get; set; }

        public string Station { get; set; }

        public string DayOrNight { get; set; }

        public OutFileArg[] OutputFiles { get; set; }

        public string LogLevel { get; set; }

        public string LogInfo { get; set; }

        public static void WriteXml(MosaicOutputArg arg,string argXml)
        {
            XElement xml = new XElement("xml",
                new XElement("Satellite", arg.Satellite),
                new XElement("Sensor", arg.Sensor),
                new XElement("Level", arg.Level),
                new XElement("ProjectionIdentify", arg.ProjectionIdentify),
                new XElement("ObservationDate", arg.ObservationDate),
                new XElement("Station", arg.Station),
                new XElement("DayOrNight", arg.DayOrNight),
                new XElement("OutputFiles",
                    WriteFiles(arg)),
                new XElement("log",
                    new XElement("loglevel", arg.LogLevel),
                    new XElement("loginfo", arg.LogInfo)));
            xml.Save(argXml);
        }

        private static object[] WriteFiles(MosaicOutputArg arg)
        {
            if (arg == null || arg.OutputFiles == null || arg.OutputFiles.Length == 0)
                return null;
            List<XElement> files = new List<XElement>();
            for (int i = 0; i < arg.OutputFiles.Length; i++)
            {
                OutFileArg file = arg.OutputFiles[i];
                files.Add(new XElement("File",
                    new XElement("OutputFilename", file.OutputFilename),
                    new XElement("Thumbnail", file.Thumbnail),
                    new XElement("ExtendFiles", file.ExtendFiles),
                    new XElement("Envelope",
                        new XAttribute("name", file.Envelope.Name),
                        new XAttribute("minx", file.Envelope.PrjEnvelope.MinX),
                        new XAttribute("maxx", file.Envelope.PrjEnvelope.MaxX),
                        new XAttribute("miny", file.Envelope.PrjEnvelope.MinY),
                        new XAttribute("maxy", file.Envelope.PrjEnvelope.MaxY)),
                    new XElement("ResolutionX", file.ResolutionX),
                    new XElement("ResolutionY", file.ResolutionY),
                    new XElement("Length", file.Length)));
            }
            return files.ToArray();
        }

        private static PrjEnvelopeItem[] ParseEnvelopes(XElement xElement)
        {
            if (xElement == null || xElement.Elements("Envelope") == null)
                return null;
           IEnumerable<PrjEnvelopeItem> prjs = from item in xElement.Elements("Envelope")
                                     select new PrjEnvelopeItem(item.Attribute("name").Value,
                                         new PrjEnvelope(double.Parse(item.Attribute("minx").Value)
                                             , double.Parse(item.Attribute("minx").Value)
                                             , double.Parse(item.Attribute("minx").Value)
                                             , double.Parse(item.Attribute("minx").Value)));
            return prjs.ToArray();
        }
    }
}
