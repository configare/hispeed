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
    public class Data2PrdsArg
    {
        /// <summary>
        /// 数据集，对应产品
        /// </summary>
        public Dictionary<string,string > Bands2prds =new Dictionary<string,string >();

        public static Data2PrdsArg ParseXml(string argXml)
        {
            if (string.IsNullOrWhiteSpace(argXml))
                throw new ArgumentNullException("argXml", "参数文件为空");
            if (!File.Exists(argXml))
                throw new FileNotFoundException("参数文件不存在" + argXml, argXml);
            try
            {
                Dictionary<string, string> bands2prds = null;
                XElement xml = XElement.Load(argXml);
               ParseBands2Prds(xml.Element("Bands2prds"), out bands2prds);
               Data2PrdsArg arg = new Data2PrdsArg();
                arg.Bands2prds = bands2prds;
                return arg;
            }
            catch (Exception ex)
            {
                throw new Exception("解析波段产品映射参数文件失败" + ex.Message, ex);
            }
        }

        private static void ParseBands2Prds(XElement xElement, out Dictionary<string, string> bands2prds)
        {
            bands2prds = new Dictionary<string, string>();
            if (xElement == null || xElement.Elements("Bands") == null)
                return;
            foreach (XElement band in xElement.Elements("Bands"))
            {
                bands2prds.Add(band.Attribute("bandname").Value, band.Attribute("prdname").Value);
            }
        }

        public void ToXml(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("xmlFilename");
            XElement xml = new XElement("xml",
                new XElement("Bands2prds", Bands2prdsToXmlValue(Bands2prds))
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

        private object[] Bands2prdsToXmlValue(Dictionary<string, string> bands2prds)
        {
            if (bands2prds == null || bands2prds.Count == 0)
                return null;
            List<XElement> envs = new List<XElement>();
            foreach (string item in bands2prds.Keys)
            {
                if (item == null)
                    continue;
                envs.Add(new XElement("Bands",
                        new XAttribute("bandname", item),
                        new XAttribute("prdname", bands2prds[item])));
            }
            return envs.ToArray();
        }

    }
}
