using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using System.IO;
using System.Xml.Linq;
using GeoDo.RasterProject;

namespace GeoDo.Tools.Block
{
    public class BlockArg
    {
        public string InputFilename;
        public string OutputDir;
        /// <summary>
        /// identify或者custom
        /// </summary>
        public string BlockType;
        public string BlockIdentify;
        public PrjEnvelopeItem[] BlockEnvelopes;

        public static BlockArg ParseXml(string argXml)
        {
            if (string.IsNullOrWhiteSpace(argXml))
                throw new ArgumentNullException("argXml", "参数文件为空");
            if (!File.Exists(argXml))
                throw new FileNotFoundException("参数文件不存在", argXml);
            string inputfilename;
            string outputDir;
            string blockType = "";
            string blockIdentify = "";
            PrjEnvelopeItem[] envelopes = null;
            XElement xml = XElement.Load(argXml);
            XElement inputfilenameX = xml.Element("InputFilename");
            if (inputfilenameX == null || string.IsNullOrWhiteSpace(inputfilenameX.Value))
                inputfilename = null;
            inputfilename = inputfilenameX.Value;
            XElement outputDirX = xml.Element("OutputDir");
            outputDir = outputDirX.Value;
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            XElement blockTypeX = xml.Element("BlockType");
            if (blockTypeX != null)
                blockType = blockTypeX.Value;
            if (blockType == "identify")
            {
                XElement blockIdentifyX = xml.Element("BlockIdentify");
                if (blockIdentifyX != null)
                    blockIdentify = blockIdentifyX.Value;
            }
            else
            {
                envelopes = ParseEnvelopes(xml.Element("Blocks"));
            }
            BlockArg arg = new BlockArg();
            arg.InputFilename = inputfilename;
            arg.OutputDir = outputDir;
            arg.BlockType = blockType;
            arg.BlockIdentify = blockIdentify;
            arg.BlockEnvelopes = envelopes;
            return arg;
        }

        private static PrjEnvelopeItem[] ParseEnvelopes(XElement xElement)
        {
            if (xElement == null || xElement.Elements("Envelope") == null)
                return null;
            IEnumerable<PrjEnvelopeItem> prjs = from item in xElement.Elements("Envelope")
                                                select new PrjEnvelopeItem(item.Attribute("name").Value,
                                                    new PrjEnvelope(double.Parse(item.Attribute("minx").Value)
                                                        , double.Parse(item.Attribute("maxx").Value)
                                                        , double.Parse(item.Attribute("miny").Value)
                                                        , double.Parse(item.Attribute("maxy").Value)));
            return prjs.ToArray();
        }
    }
}
