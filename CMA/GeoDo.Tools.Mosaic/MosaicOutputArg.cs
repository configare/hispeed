using System;
using System.Collections.Generic;
using System.Text;
using GeoDo.FileProject;
using System.Xml.Linq;
using System.IO;
using GeoDo.RasterProject;
using GeoDo.Tools;

namespace GeoDo.Tools.Mosaic
{
    public class MosaicOutputArg
    {
        public MosaicOutputArg()
        { }

        public string InputFilename;
        public OutFileArg[] OutputFiles;
        public string LogInfo { get; set; }
        public string LogLevel { get; set; }

        public void ToXml(string xmlFilename)
        {
            XElement xml = ToXml();
            if (!Directory.Exists(Path.GetDirectoryName(xmlFilename)))
                Directory.CreateDirectory(Path.GetDirectoryName(xmlFilename));
            xml.Save(xmlFilename);
        }

        public XElement ToXml()
        {
            return new XElement("xml",
                new XElement("InputFilename", InputFilename),
                new XElement("OutputFiles",
                   XmlHelper.WriteFiles(OutputFiles)),
                new XElement("log",
                    new XElement("loglevel", LogLevel),
                    new XElement("loginfo", LogInfo)));
        }

        public static MosaicOutputArg FromXml(string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename))
                    throw new ArgumentNullException("argXml", "参数文件为空");
                if (!File.Exists(filename))
                    throw new FileNotFoundException("参数文件不存在", filename);
                XElement xml = XElement.Load(filename);
                if (xml == null || xml.IsEmpty)
                    throw new ArgumentNullException("参数内容为空", filename);
                MosaicOutputArg arg = new MosaicOutputArg();
                arg.InputFilename = XmlHelper.ParseXElementValueToString(xml, "InputFilename");
                //arg.OutputFiles = XmlHelper.OutputFilesToXml(xml, "OutputFiles");
                //arg.LogInfo = XmlHelper.ParseXElementValueToString(xml, "LogInfo");
                //arg.LogLevel = XmlHelper.ParseXElementValueToString(xml, "LogLevel");
                return arg;
            }
            catch (Exception ex)
            {
                throw new Exception("解析拼接镶嵌输入参数文件失败", ex);
            }
        }
    }
}
