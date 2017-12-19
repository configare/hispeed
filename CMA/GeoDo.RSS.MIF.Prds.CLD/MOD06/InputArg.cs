using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using System.IO;
using System.Xml.Linq;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class InputArg
    {
        public string InputDir;
        public string OutputDir;
        public PrjEnvelopeItem[] ValidEnvelopes;
        public PrjEnvelopeItem SelectedRegionEnvelope=null;
        public string[] Bands;
        public string[] StatisticsTypes;
        public string[] PeriodTypes;
        public bool OverWriteHistoryFiles;
        private string _dbxml;

        public InputArg()
        {
        }

        public InputArg(string dbxml)
        {
            _dbxml = dbxml;
        }

        public static InputArg ParseXml(string argXml)
        {
            if (string.IsNullOrWhiteSpace(argXml))
                throw new ArgumentNullException("argXml", "参数文件为空");
            if (!File.Exists(argXml))
                throw new FileNotFoundException("参数文件不存在" + argXml, argXml);
            try
            {
                string inputDirname;
                string outputDir;
                string bands=null;
                PrjEnvelopeItem[] validEnvelopes = null;
                XElement xml = XElement.Load(argXml);
                InputArg arg = new InputArg();
                XElement inputDirnameX = xml.Element("InputDir");
                if (!string.IsNullOrWhiteSpace(inputDirnameX.Value))
                {
                    inputDirname = inputDirnameX.Value;
                    arg.InputDir = inputDirname;
                }
                XElement outputDirX = xml.Element("OutputDir");
                if (!string.IsNullOrWhiteSpace(outputDirX.Value))
                {
                    outputDir = outputDirX.Value;
                    //if (!Directory.Exists(outputDir))
                    //    Directory.CreateDirectory(outputDir);
                    arg.OutputDir = outputDir;
                }
                validEnvelopes = ParseEnvelopes(xml.Element("ValidEnvelopes"));
                if (validEnvelopes.Length !=0)
                {
                    arg.ValidEnvelopes = validEnvelopes;
                }
                XElement bandsX = xml.Element("Bands");
                if (bandsX != null && !string.IsNullOrWhiteSpace(bandsX.Value))
                {
                    bands = bandsX.Value;
                    arg.Bands = ParseBands(bands);
                }
                return arg;
            }
            catch (Exception ex)
            {
                throw new Exception("解析投影输入参数文件失败" + ex.Message, ex);
            }
        }

        public static InputArg ParsePeriodArgsXml(string periodargXml)
        {
            if (string.IsNullOrWhiteSpace(periodargXml))
                throw new ArgumentNullException("argXml", "参数文件为空");
            if (!File.Exists(periodargXml))
                throw new FileNotFoundException("参数文件不存在" + periodargXml, periodargXml);
            try
            {
                XElement xml = XElement.Load(periodargXml);
                InputArg arg = new InputArg();
                //XElement inputDirnameX = xml.Element("InputDir");
                //if (!string.IsNullOrWhiteSpace(inputDirnameX.Value))
                //{
                //    arg.InputDir = inputDirnameX.Value;
                //}
                //XElement outputDirX = xml.Element("OutputDir");
                //if (!string.IsNullOrWhiteSpace(outputDirX.Value))
                //{
                //    if (!Directory.Exists(outputDirX.Value))
                //        Directory.CreateDirectory(outputDirX.Value);
                //    arg.OutputDir = outputDirX.Value;
                //}
                XElement staticsTypeX = xml.Element("StatisticsTypes");
                if (staticsTypeX != null && !string.IsNullOrWhiteSpace(staticsTypeX.Value))
                {
                    arg.StatisticsTypes = ParseBands(staticsTypeX.Value.ToUpper());
                }
                XElement periodTypeX = xml.Element("PeriodTypes");
                if (periodTypeX != null && !string.IsNullOrWhiteSpace(periodTypeX.Value))
                {
                    arg.PeriodTypes = ParseBands(periodTypeX.Value.ToUpper());
                }
                XElement OverWriteHistoryFilesX = xml.Element("OverWrite");
                if (OverWriteHistoryFilesX != null && !string.IsNullOrWhiteSpace(OverWriteHistoryFilesX.Value))
                {
                    string bl =OverWriteHistoryFilesX.Value;
                    if (bl.ToUpper()=="TRUE")
                        arg.OverWriteHistoryFiles = true;
                    else
                        arg.OverWriteHistoryFiles = false;
                }
                return arg;
            }
            catch (Exception ex)
            {
                throw new Exception("解析投影输入参数文件失败" + ex.Message, ex);
            }
        }

        public static InputArg ParseStatRegions(string regionxml)
        {
            if (string.IsNullOrWhiteSpace(regionxml))
                throw new ArgumentNullException("argXml", "参数文件为空");
            if (!File.Exists(regionxml))
                throw new FileNotFoundException("参数文件不存在" + regionxml);
            try
            {
                XElement xml = XElement.Load(regionxml);
                InputArg arg = new InputArg();
                PrjEnvelopeItem[]  validEnvelopes = ParseEnvelopes(xml.Element("ValidEnvelopes"));
                if (validEnvelopes.Length != 0)
                {
                    arg.ValidEnvelopes = validEnvelopes;
                }
                PrjEnvelopeItem selectedRegion = ParseEnvelope(xml.Element("SelectedRegion").Element("Envelope"));
                if (selectedRegion != null)
                {
                    arg.SelectedRegionEnvelope = selectedRegion;
                }
                return arg;
            }
            catch (Exception ex)
            {
                throw new Exception("解析投影输入参数文件失败" + ex.Message, ex);
            }

        }

        private static string[] ParseBands(string bands)
        {
            if (string.IsNullOrWhiteSpace(bands))
                return null;
            string[] bandsStr = bands.Split(',');
            List<string> retBands = new List<string>();
            for (int i = 0; i < bandsStr.Length; i++)
            {
                string bandstr = bandsStr[i];
                if (string.IsNullOrWhiteSpace(bandstr))
                    continue;
                else
                    retBands.Add(bandstr);
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

        #region 存储
        public void ToXml(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("xmlFilename");
            XElement xml = new XElement("xml",
                new XElement("InputDir", InputDir),
                new XElement("OutputDir", OutputDir),
                new XElement("ValidEnvelopes", EnvelopesToXmlValue(ValidEnvelopes)),
                new XElement("Bands", BandsToXmlValue(Bands)));
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            xml.Save(fileName);
        }

        public void RegionToXml(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("xmlFilename");
            XElement xml ;
            if (SelectedRegionEnvelope==null)
                xml = new XElement("xml",
                    new XElement("ValidEnvelopes", EnvelopesToXmlValue(ValidEnvelopes)));
            else
                xml = new XElement("xml",
                    new XElement("SelectedRegion", EnvelopeToXmlValue(SelectedRegionEnvelope)),
                    new XElement("ValidEnvelopes", EnvelopesToXmlValue(ValidEnvelopes))
                    );
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            xml.Save(fileName);
        }

        private string BandsToXmlValue(string[] Bands)
        {
            if (Bands == null || Bands.Length == 0)
                return null;
            string envs = "";
            foreach (string item in Bands)
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
        #endregion

    }
}
