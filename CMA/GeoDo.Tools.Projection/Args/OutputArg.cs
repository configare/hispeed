using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RasterProject;
using System.Xml.Linq;
using System.IO;
using GeoDo.FileProject;

namespace GeoDo.Tools.Projection
{
    public class OutputArg
    {
        public OutputArg()
        { }

        public string OrbitFilename;
        public string Satellite;
        public string Sensor;
        public string Level;
        public string ProjectionIdentify;
        public string ObservationDate;
        public string ObservationTime;
        public string Station;
        public OutFileArg[] OutputFiles;
        public string LogInfo { get; set; }
        public string LogLevel { get; set; }
        public string DayOrNight;
        public long Length { get; set; }
        /// <summary>
        /// 轨道圈号,201200918号添加
        /// </summary>
        public string OrbitIdentify;

        public static void WriteXml(OutputArg arg,string argXml)
        {
            XElement xml = new XElement("xml",
                new XElement("OrbitFilename", arg.OrbitFilename),
                new XElement("Satellite", arg.Satellite),
                new XElement("Sensor", arg.Sensor),
                new XElement("Level", arg.Level),
                new XElement("ProjectionIdentify", arg.ProjectionIdentify),
                new XElement("ObservationDate", arg.ObservationDate),
                new XElement("ObservationTime", arg.ObservationTime),
                new XElement("Station", arg.Station),
                new XElement("DayOrNight", arg.DayOrNight),
                new XElement("Length", arg.Length),
                new XElement("OrbitIdentify", arg.OrbitIdentify),
                new XElement("OutputFiles",
                    WriteFiles(arg)),
                new XElement("log",
                    new XElement("loglevel", arg.LogLevel),
                    new XElement("loginfo", arg.LogInfo)));
            xml.Save(argXml);
        }

        private static object[] WriteFiles(OutputArg arg)
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

    public class OutFileArg
    {
        /// <summary>
        /// 文件
        /// </summary>
        public string OutputFilename;
        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thumbnail;
        /// <summary>
        /// 附属文件
        /// </summary>
        public string ExtendFiles;
        public PrjEnvelopeItem Envelope;
        public string ResolutionX;
        public string ResolutionY;
        /// <summary>
        /// 文件的大小(字节)
        /// </summary>
        public long Length;
    }
}
