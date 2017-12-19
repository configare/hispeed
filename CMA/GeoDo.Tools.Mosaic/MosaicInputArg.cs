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
    public class MosaicInputArg
    {
        public string InputFilename;
        /// <summary>
        /// 白天晚上标识
        /// </summary>
        public string DayOrNight;
        /// <summary>
        /// 轨道圈号标识
        /// </summary>
        public string OrbitIdentify;
        /// <summary>
        /// 
        /// 镶嵌或拼接
        ///  Mosaic：镶嵌，会根据定义的数据范围输出数据（默认值）
        ///  Splice：拼接，直接将在范围内的数据拼接在一起，具体输出文件范围不确定
        /// </summary>
        public string MosaicMode;
        /// <summary>
        /// 镶嵌文件范围，可定义多个，name不能相同
        /// MosaicMode=Mosaic时，表示输出范围
        /// MosaicMode=Splice时，表示有效范围
        /// </summary>
        public PrjEnvelopeItem[] MosaicEnvelopes;
        /// <summary>
        /// 输出文件目录
        /// </summary>
        public string OutputDir;
        /// <summary>
        ///数据分类：
        ///(不同的模式会依据来源数据提供的信息，生成不同的目标文件名，如果目标文件已经存在，则会直接镶嵌到此文件)
        /// DayNight：按日期，区分白天晚上，来拼接文件（默认值）
        /// Day：按日期，区分白天晚上，并且只处理白天的数据，来拼接文件
        /// Night：按日期，区分白天晚上，并且只处理晚上的数据，来拼接文件
        /// DayNightOrbit：按日期，区分白天晚上，区分轨道圈，来拼接文件
        /// </summary>
        public string OutputType;


        public void ToXml(string xmlFilename)
        { 
            XElement xml = ToXml();
            if(!Directory.Exists(Path.GetDirectoryName(xmlFilename)))
                Directory.CreateDirectory(Path.GetDirectoryName(xmlFilename));
            xml.Save(xmlFilename);
        }

        public XElement ToXml()
        {
            return new XElement("xml", new XAttribute("identify", "mosaic"),
                new XElement("InputFilename", InputFilename),
                new XElement("DayOrNight", DayOrNight),
                new XElement("OrbitIdentify", OrbitIdentify),
                new XElement("MosaicMode", MosaicMode),
                new XElement("MosaicEnvelopes", XmlHelper.EnvelopesToXmlValue(MosaicEnvelopes)),
                new XElement("OutputDir", OutputDir),
                new XElement("OutputType", OutputType)
                );
        }

        public static MosaicInputArg FromXml(string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename))
                    throw new ArgumentNullException("argXml", "参数文件为空");
                if (!File.Exists(filename))
                    throw new FileNotFoundException("参数文件不存在", filename);
                XElement xml = XElement.Load(filename);
                if (xml == null || xml.IsEmpty)
                    throw new FileNotFoundException("参数内容为空", filename);
                MosaicInputArg arg = new MosaicInputArg();
                arg.InputFilename = XmlHelper.ParseXElementValueToString(xml, "InputFilename"); ;
                arg.DayOrNight = XmlHelper.ParseXElementValueToString(xml, "DayOrNight"); ;
                arg.OrbitIdentify = XmlHelper.ParseXElementValueToString(xml, "OrbitIdentify"); ;
                arg.MosaicMode = XmlHelper.ParseXElementValueToString(xml, "MosaicMode"); ;
                arg.MosaicEnvelopes = XmlHelper.ParseXElementEnvelopes(xml, "MosaicEnvelopes"); ;
                arg.OutputDir = XmlHelper.ParseXElementValueToString(xml, "OutputDir"); ;
                arg.OutputType = XmlHelper.ParseXElementValueToString(xml, "OutputType"); ;
                return arg;
            }
            catch (Exception ex)
            {
                throw new Exception("解析拼接镶嵌输入参数文件失败", ex);
            }
        }

        public MosaicInputArg Copy()
        {
            MosaicInputArg arg = new MosaicInputArg();
            arg.InputFilename = InputFilename;
            arg.DayOrNight = DayOrNight;
            arg.OrbitIdentify = OrbitIdentify;
            arg.MosaicMode = MosaicMode;
            arg.MosaicEnvelopes = MosaicEnvelopes;
            arg.OutputDir = OutputDir;
            arg.OutputType = OutputType;
            return arg;
        }
    }
}
