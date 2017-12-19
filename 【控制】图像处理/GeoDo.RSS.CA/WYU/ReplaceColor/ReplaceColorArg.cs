using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    public class ReplaceColorArg : RgbProcessorArg
    {
        private Color _targetColor = Color.Empty;
        private int _colorTorence =40;
        private double _hue = 0;
        private double  _saturation = 0;
        private double _lightness = 0;

        public ReplaceColorArg()
        {
        }

        public Color TargetColor
        {
            get { return _targetColor; }
            set { _targetColor = value; }
        }

        public int ColorTorence
        {
            get { return _colorTorence; }
            set { _colorTorence = value; }
        }

        public double Hue
        {
            get { return _hue; }
            set { _hue = value; }
        }

        public double Saturation
        {
            get { return _saturation; }
            set { _saturation = value; }
        }

        public double Lightness
        {
            get { return _lightness; }
            set { _lightness = value; }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("Color");
            subElem.InnerText = _targetColor.ToArgb().ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("ColorTolerance");
            subElem.InnerText = _colorTorence.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Hue");
            subElem.InnerText = _hue.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Saturation");
            subElem.InnerText = _saturation.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Bright");
            subElem.InnerText = _lightness.ToString();
            xmlElem.AppendChild(subElem);
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            ReplaceColorArg replaceArg = new ReplaceColorArg();
            return replaceArg;
        }

    }
}
