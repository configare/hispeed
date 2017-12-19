using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    public class CurveAdjustProcessorArg:RgbProcessorArg
    {
        private AdjustedValues _rgbAdjustedValues = new AdjustedValues(null);
        private AdjustedValues _redAdjustedValues = new AdjustedValues(null);
        private AdjustedValues _greenAdjustedValues = new AdjustedValues(null);
        private AdjustedValues _blueAdjustedValues = new AdjustedValues(null);

        public CurveAdjustProcessorArg()
            : base()
        { 
        }

        public AdjustedValues RGB
        {
            get { return _rgbAdjustedValues; }
        }

        public AdjustedValues Red
        {
            get { return _redAdjustedValues; }
        }

        public AdjustedValues Green
        {
            get { return _greenAdjustedValues; }
        }

        public AdjustedValues Blue
        {
            get { return _blueAdjustedValues; }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem;
            XmlElement subElemValue;
            if (!_rgbAdjustedValues.IsEmpty)
            {
                subElem = xmldoc.CreateElement("RGB");
                xmlElem.AppendChild(subElem);
                for (int i = 0; i < _rgbAdjustedValues.Values.Count(); i++)
                {
                    subElemValue = xmldoc.CreateElement("Value");
                    subElemValue.InnerText = _rgbAdjustedValues.Values[i].ToString();
                    subElem.AppendChild(subElemValue);
                }
            }
            if (!_redAdjustedValues.IsEmpty)
            {
                subElem = xmldoc.CreateElement("Red");
                xmlElem.AppendChild(subElem);
                for (int i = 0; i < _redAdjustedValues.Values.Count(); i++)
                {
                    subElemValue = xmldoc.CreateElement("Value");
                    subElemValue.InnerText = _redAdjustedValues.Values[i].ToString();
                    subElem.AppendChild(subElemValue);
                }
            }
            if (!_greenAdjustedValues.IsEmpty)
            {
                subElem = xmldoc.CreateElement("Green");
                xmlElem.AppendChild(subElem);
                for (int i = 0; i < _greenAdjustedValues.Values.Count(); i++)
                {
                    subElemValue = xmldoc.CreateElement("Value");
                    subElemValue.InnerText = _greenAdjustedValues.Values[i].ToString();
                    subElem.AppendChild(subElemValue);
                }
            }
            if (!_blueAdjustedValues.IsEmpty)
            {
                subElem = xmldoc.CreateElement("Blue");
                xmlElem.AppendChild(subElem);
                for (int i = 0; i < _blueAdjustedValues.Values.Count(); i++)
                {
                    subElemValue = xmldoc.CreateElement("Value");
                    subElemValue.InnerText = _blueAdjustedValues.Values[i].ToString();
                    subElem.AppendChild(subElemValue);
                }
            }
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            CurveAdjustProcessorArg arg = new CurveAdjustProcessorArg();
            return arg;
        }
    }
}
