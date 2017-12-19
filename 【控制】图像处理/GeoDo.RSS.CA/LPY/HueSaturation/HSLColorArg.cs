using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    class HSLColorArg : RgbProcessorArg
    {
        private int _pixelHue;
        private int _pixelSaturation;
        private int _pixelLum;
        private bool _isChanged = false;

        public HSLColorArg()
        {
            _pixelHue = 0;
            _pixelSaturation = 0;
            _pixelLum = 0;
        }        

        public int PixelHue
        {
            get { return _pixelHue; }
            set { _pixelHue = value; }
        }

        public int PixelSaturation
        {
            get { return _pixelSaturation; }
            set { _pixelSaturation = value; }
        }

        public int PixelLum
        {
            get { return _pixelLum; }
            set { _pixelLum = value; }
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; }
        }

        public bool IsEmpty()
        {
            return _pixelHue == 0 && _pixelSaturation == 0 && _pixelLum== 0 ;
        }

        public void SetEmpty()
        {
            _pixelHue = 0;
            _pixelSaturation = 0;
            _pixelLum = 0;
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("Hue");
            subElem.InnerText = _pixelHue.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Saturation");
            subElem.InnerText = _pixelSaturation.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Bright");
            subElem.InnerText = _pixelLum.ToString();
            xmlElem.AppendChild(subElem);
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            HSLColorArg it = new HSLColorArg ( );
            it.PixelHue = _pixelHue;
            it.PixelSaturation = _pixelSaturation;
            it.PixelLum = _pixelLum;
            return it;
        }
    }
}
