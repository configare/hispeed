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
    public class InputArg
    {
        public InputArg()
        { }

        public string InputFilename;
        public string OutputDir;
        public PrjEnvelopeItem[] ValidEnvelopes;
        public int[] Bands;
        /// <summary>
        /// 投影：GLL\ LBT、MCT、OTG、AEA、PSG、NOM、NUL
        /// </summary>
        public string ProjectionIdentify;
        public PrjEnvelopeItem[] Envelopes;
        public float ResolutionX = 0f;
        public float ResolutionY = 0f;
        /// <summary>
        /// 前一个处理文件的观测日期，时间，轨道圈号,用于按轨道拼接时候，输出文件名的生成
        /// </summary>
        public string PervObservationDate;
        /// <summary>
        /// 前一个处理文件的观测日期，时间，轨道圈号,用于按轨道拼接时候，输出文件名的生成
        /// </summary>
        public string PervObservationTime;
        /// <summary>
        /// 前一个处理文件的观测日期，时间，轨道圈号[可用一个时间标识],用于按轨道拼接时候，输出文件名的生成
        /// </summary>
        public string OrbitIdentify;

        /// <summary>
        /// 是否只保留拼接后文件。默认是false
        /// </summary>
        public bool IsOnlySaveMosaicFile = false;
        /// <summary>
        /// 设定投影时候是否只处理白天或晚上数据，取值：day、night、dayandnight，默认值是dayandnight
        /// </summary>
        public string DayNight = "";
        /// <summary>
        /// 扩展参数
        /// </summary>
        public string[] ExtArgs = null;

        public MosaicInputArg MosaicInputArg;

        public static InputArg ParseXml(string argXml)
        {
            if (string.IsNullOrWhiteSpace(argXml))
                throw new ArgumentNullException("argXml", "参数文件为空");
            if (!File.Exists(argXml))
                throw new FileNotFoundException("参数文件不存在" + argXml, argXml);
            try
            {
                string inputfilename;
                string outputDir;
                PrjEnvelopeItem[] validEnvelopes = null;
                string projectionIdentify = null;
                string bands = null;
                string resolutionX = "";
                string resolutionY = "";
                PrjEnvelopeItem[] envelopes;
                MosaicInputArg mosaicInputArg = null;
                string dayNight = "";

                XElement xml = XElement.Load(argXml);
                XElement inputfilenameX = xml.Element("InputFilename");
                inputfilename = inputfilenameX.Value;
                XElement outputDirX = xml.Element("OutputDir");
                outputDir = outputDirX.Value;
                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);
                validEnvelopes = ParseEnvelopes(xml.Element("ValidEnvelopes"));
                XElement projectionIdentifyX = xml.Element("ProjectionIdentify");
                if (projectionIdentifyX != null)
                    projectionIdentify = projectionIdentifyX.Value;
                XElement bandsX = xml.Element("Bands");
                if (bandsX != null)
                    bands = bandsX.Value;
                XElement resolutionXX = xml.Element("ResolutionX");
                if (resolutionXX != null)
                    resolutionX = resolutionXX.Value;
                XElement resolutionYX = xml.Element("ResolutionY");
                if (resolutionYX != null)
                    resolutionY = resolutionYX.Value;
                envelopes = ParseEnvelopes(xml.Element("Envelopes"));
                mosaicInputArg = ParseMosaic(xml.Element("Mosaic"));
                bool isOnlySaveMosaicFile = (GetElementValue(xml, "IsOnlySaveMosaicFile") == "true");
                XElement dayNightX = xml.Element("DayNight");
                if (dayNightX != null && !dayNightX.IsEmpty)
                    dayNight = dayNightX.Value;
                string[] extArgs = null;
                XElement extArgsX = xml.Element("ExtArgs");
                if (extArgsX != null && !extArgsX.IsEmpty)
                    extArgs = ParseExtArgs(extArgsX.Value);

                InputArg arg = new InputArg();
                arg.InputFilename = inputfilename;
                arg.OutputDir = outputDir;
                arg.ValidEnvelopes = validEnvelopes;
                arg.ProjectionIdentify = string.IsNullOrWhiteSpace(projectionIdentify) ? "GLL" : projectionIdentify;
                arg.Bands = ParseBands(bands);
                if (!string.IsNullOrWhiteSpace(resolutionX))
                    arg.ResolutionX = float.Parse(resolutionX);
                if (!string.IsNullOrWhiteSpace(resolutionY))
                    arg.ResolutionY = float.Parse(resolutionY);
                arg.PervObservationDate = GetElementValue(xml, "PervObservationDate");
                arg.PervObservationTime = GetElementValue(xml, "PervObservationTime");
                arg.OrbitIdentify = GetElementValue(xml, "OrbitIdentify");
                arg.Envelopes = envelopes;
                arg.MosaicInputArg = mosaicInputArg;
                arg.IsOnlySaveMosaicFile = isOnlySaveMosaicFile;
                if (!string.IsNullOrWhiteSpace(dayNight))
                    arg.DayNight = dayNight;
                if (extArgs != null)
                    arg.ExtArgs = extArgs;
                return arg;
            }
            catch (Exception ex)
            {
                throw new Exception("解析投影输入参数文件失败" + ex.Message, ex);
            }
        }

        private static string[] ParseExtArgs(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.Split(',');
        }

        private static string GetElementValue(XElement xml, string xname)
        {
            XElement resolutionXX = xml.Element(xname);
            return resolutionXX == null ? "" : resolutionXX.Value;
        }

        private static MosaicInputArg ParseMosaic(XElement mosaic)
        {
            if (mosaic == null)
                return null;
            string outputdir = mosaic.Element("OutputDir").Value;
            PrjEnvelopeItem env = ParseEnvelope(mosaic.Element("Envelope"));
            MosaicInputArg mosaicArg = new MosaicInputArg();
            mosaicArg.OutputDir = outputdir;
            mosaicArg.Envelope = env;
            return mosaicArg;
        }

        private static int[] ParseBands(string bands)
        {
            if (string.IsNullOrWhiteSpace(bands))
                return null;
            string[] bandsStr = bands.Split(',');
            List<int> retBands = new List<int>();
            for (int i = 0; i < bandsStr.Length; i++)
            {
                string bandstr = bandsStr[i];
                if (string.IsNullOrWhiteSpace(bandstr))
                    continue;
                int band;
                if (int.TryParse(bandstr, out band))
                    retBands.Add(band);
            }
            return retBands.ToArray();
        }

        private static PrjEnvelopeItem[] ParseEnvelopes(XElement xElement)
        {
            if (xElement == null || xElement.Elements("Envelope") == null)
                return null;
            IEnumerable<PrjEnvelopeItem> prjs = from item in xElement.Elements("Envelope")
                                                where item.Value != null
                                                select ParseEnvelope(item);
            return prjs.ToArray();
        }

        private static PrjEnvelopeItem ParseEnvelope(XElement xElement)
        {
            if (xElement == null || xElement.Value == null)
                return null;
            return new PrjEnvelopeItem(xElement.Attribute("name").Value,
                                         new PrjEnvelope(double.Parse(xElement.Attribute("minx").Value)
                                             , double.Parse(xElement.Attribute("maxx").Value)
                                             , double.Parse(xElement.Attribute("miny").Value)
                                             , double.Parse(xElement.Attribute("maxy").Value)));
        }

        public void ToXml(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("xmlFilename");
            XElement xml = new XElement("xml",
                new XElement("InputFilename", InputFilename),
                new XElement("OutputDir", OutputDir),
                new XElement("ValidEnvelopes", EnvelopesToXmlValue(ValidEnvelopes)),
                new XElement("Bands", BandsToXmlValue(Bands)),
                new XElement("ProjectionIdentify", ProjectionIdentify),
                new XElement("Envelopes", EnvelopesToXmlValue(Envelopes)),
                new XElement("ResolutionX", ResolutionX),
                new XElement("ResolutionY", ResolutionY),
                new XElement("PervObservationDate", PervObservationDate),
                new XElement("PervObservationTime", PervObservationTime),
                new XElement("OrbitIdentify", OrbitIdentify),
                new XElement("IsOnlySaveMosaicFile", IsOnlySaveMosaicFile),
                new XElement("DayNight", DayNight),
                new XElement("ExtArgs", ExtArgsToXmlValue(ExtArgs)),
                MosaicArgToXmlValue(MosaicInputArg)
                );
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            xml.Save(fileName);
        }

        private string ExtArgsToXmlValue(string[] ExtArgs)
        {
            if (ExtArgs == null)
                return null;
            return string.Join(",",ExtArgs);
        }

        private XElement MosaicArgToXmlValue(MosaicInputArg mosaicInputArg)
        {
            if (mosaicInputArg == null)
                return null;
            return new XElement("Mosaic",
                new XElement("OutputDir", mosaicInputArg.OutputDir),
                EnvelopeToXmlValue(mosaicInputArg.Envelope));
        }

        private string BandsToXmlValue(int[] Bands)
        {
            if (Bands == null || Bands.Length == 0)
                return null;
            string envs = "";
            foreach (int item in Bands)
                envs += item + ",";
            envs = envs.TrimEnd(',');
            return envs;
        }

        private object[] EnvelopesToXmlValue(PrjEnvelopeItem[] envelopes)
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

        private XElement EnvelopeToXmlValue(PrjEnvelopeItem item)
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

        public InputArg Copy()
        {
            InputArg arg = new InputArg();
            arg.InputFilename = InputFilename;
            arg.ValidEnvelopes = ValidEnvelopes;
            arg.Bands = Bands;
            arg.OutputDir = OutputDir;
            arg.ResolutionX = ResolutionX;
            arg.ResolutionY = ResolutionY;
            arg.Envelopes = Envelopes;
            arg.ProjectionIdentify = ProjectionIdentify;
            arg.IsOnlySaveMosaicFile = IsOnlySaveMosaicFile;
            arg.MosaicInputArg = MosaicInputArg;
            arg.DayNight = DayNight;
            arg.ExtArgs = ExtArgs;
            return arg;
        }
    }
}
