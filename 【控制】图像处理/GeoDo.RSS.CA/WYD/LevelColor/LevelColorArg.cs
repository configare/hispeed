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
    /// 色阶参数类
    /// </summary>
    public class LevelColorArg : RgbProcessorArg
    {
        private enumChannel _channel = new enumChannel();
        private int _inputMin = 0;
        private double _inputMiddle = 1;
        private int _inputMax = 255;
        private int _outputMin = 0;
        private int _outputMax = 255;
        private ObjPixelCount _objPixelCount = null;
        private bool _isChanged = false;

        public LevelColorArg():base()
        {
            _channel = enumChannel.RGB;
            _objPixelCount = new ObjPixelCount();
        }

        public LevelColorArg(enumChannel channel)
        {
            _channel = channel;
            _objPixelCount = new ObjPixelCount();
        }

        public enumChannel Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public int InputMin
        {
            get { return _inputMin; }
            set { _inputMin =Math.Max(0,value); }
        }

        public double InputMiddle
        {
            get { return _inputMiddle; }
            set { _inputMiddle = value; }
        }

        public int InputMax
        {
            get { return _inputMax; }
            set { _inputMax =Math.Min(255,value); }
        }

        public int OutputMin
        {
            get { return _outputMin; }
            set { _outputMin =Math.Max(0,value); }
        }

        public int OutputMax
        {
            get { return _outputMax; }
            set { _outputMax =Math.Min(255,value); }
        }
       
        public ObjPixelCount ObjPixelCount
        {
            get { return _objPixelCount; }
            set { _objPixelCount = value; }
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
            subElem = xmldoc.CreateElement("InputMin");
            subElem.InnerText = _inputMin.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("InputMiddle");
            subElem.InnerText = _inputMiddle.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("InputMax");
            subElem.InnerText = _inputMax.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("OutputMin");
            subElem.InnerText = _outputMin.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("OutputMax");
            subElem.InnerText = _outputMax.ToString();
            xmlElem.AppendChild(subElem);
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            LevelColorArg arg = new LevelColorArg(_channel);
            arg.Channel = _channel;
            arg.InputMin = _inputMin;
            arg.InputMiddle = _inputMiddle;
            arg.InputMax = _inputMax;
            arg.OutputMin = _outputMin;
            arg.OutputMax = _outputMax;
            arg.ObjPixelCount = _objPixelCount;
            return arg;
        }
    }
}
