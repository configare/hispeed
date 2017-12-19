using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using System.Xml.Linq;

namespace GeoDo.Tools
{
    public class OutFileArg
    {
        /// <summary>
        /// 文件名（不带路径）
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
        /// <summary>
        /// 文件坐标范围
        /// </summary>
        public PrjEnvelopeItem Envelope;
        /// <summary>
        /// 分辨率
        /// </summary>
        public string ResolutionX;
        /// <summary>
        /// 分辨率
        /// </summary>
        public string ResolutionY;
        /// <summary>
        /// 文件的大小(字节)
        /// </summary>
        public long Length;

        public XElement ToXml()
        {
            return new XElement("File",
                       new XElement("OutputFilename", this.OutputFilename),
                       new XElement("Thumbnail", this.Thumbnail),
                       new XElement("ExtendFiles", this.ExtendFiles),
                       new XElement("Envelope",
                           new XAttribute("name", this.Envelope.Name),
                           new XAttribute("minx", this.Envelope.PrjEnvelope.MinX),
                           new XAttribute("maxx", this.Envelope.PrjEnvelope.MaxX),
                           new XAttribute("miny", this.Envelope.PrjEnvelope.MinY),
                           new XAttribute("maxy", this.Envelope.PrjEnvelope.MaxY)),
                       new XElement("ResolutionX", this.ResolutionX),
                       new XElement("ResolutionY", this.ResolutionY),
                       new XElement("Length", this.Length)); 
        }

        public static OutFileArg FromXml(XElement xml)
        {
            return null;
        }
    }
}
