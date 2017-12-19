using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;
namespace GeoDo.RSS.CA
{
    public class GaussianFliterARG : RgbWndProcessorArg
    {
        private decimal _gaussianSigma;
        private int _gaussianRadius;


        public decimal GaussianSigma
        {
            get { return _gaussianSigma; }
            set { _gaussianSigma = value; }
        }

        public int GaussianRadius
        {
            get { return _gaussianRadius; }
            set { _gaussianRadius = value; }
        }
        public GaussianFliterARG()
        {
            _gaussianSigma = 0.8m;
            _gaussianRadius = 3;
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("Radius");
            subElem.InnerText = _gaussianRadius.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Sigma");
            subElem.InnerText = _gaussianSigma.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Height");
            subElem.InnerText = _wndHeight.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Width");
            subElem.InnerText = _wndWidth.ToString();
            xmlElem.AppendChild(subElem);
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            GaussianFliterARG it = new GaussianFliterARG();
            it.GaussianSigma = _gaussianSigma;
            it.GaussianRadius = _gaussianRadius;
            return it;
        }
    }
}
