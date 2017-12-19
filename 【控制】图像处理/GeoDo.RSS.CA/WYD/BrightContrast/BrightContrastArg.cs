using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;


namespace GeoDo.RSS.CA
{
    /// <summary>
    /// 亮度/对比度的参数类
    /// </summary>
    public class BrightContrastArg:RgbProcessorArg
    {
        private enumChannel _channel = new enumChannel();
        private int _brightAdjustValue = 0;
        private int _contrastAdjustValue = 0;
        private double _brightchangevalue = 2.55;
        private bool _isChanged = false;

        public BrightContrastArg():base()
        {
            _channel = enumChannel.RGB;
        }

        public BrightContrastArg(enumChannel channel)
        {
            _channel = channel;
        }

        /// <summary>
        /// 需要处理图像的通道
        /// </summary>
        public enumChannel Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }
        
        /// <summary>
        /// 亮度值
        /// </summary>
        public int BrightAdjustValue
        {
            get { return _brightAdjustValue; }
            set { _brightAdjustValue = value; }
        }

        /// <summary>
        /// 对比度值
        /// </summary>
        public int ContrastAdjustValue
        {
            get { return _contrastAdjustValue; }
            set { _contrastAdjustValue = value; }
        }

        /// <summary>
        /// 对亮度参数的调整
        /// </summary>
        public double Brightchangevalue
        {
            get { return _brightchangevalue; }
            set { _brightchangevalue = value; }
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("Color");
            subElem.InnerText = _channel.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Step");
            subElem.InnerText = _brightchangevalue.ToString();
            xmlElem.AppendChild(subElem);
            subElem= xmldoc.CreateElement("Bright");
            subElem.InnerText = _brightAdjustValue.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Contrast");
            subElem.InnerText = _contrastAdjustValue.ToString();
            xmlElem.AppendChild(subElem);
            return xmlElem;
        }

         public override RgbProcessorArg Clone()
         {
             BrightContrastArg arg = new BrightContrastArg(_channel);
             arg.BrightAdjustValue = _brightAdjustValue;
             arg.ContrastAdjustValue = _contrastAdjustValue;
             arg.Brightchangevalue = _brightchangevalue;
             arg.Channel = _channel;
             return arg;
         }
    }
}
