using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;
namespace GeoDo.RSS.CA
{
    class HistGetGradesArg : RgbProcessorArg
    {
        private float _lhRL;
        private float _lhGL;
        private float _lhBL;
        private float _lhRH;
        private float _lhGH;
        private float _lhBH;

        public HistGetGradesArg()
        {
            _lhRL = 0;
            _lhGL = 0;
            _lhBL = 0;
            _lhRH = 0;
            _lhGH = 0;
            _lhBH = 0; 
        }

        public float LhBH
        {
            get { return _lhBH; }
            set { _lhBH = value; }
        }

        public float LhGH
        {
            get { return _lhGH; }
            set { _lhGH = value; }
        }

        public float LhRH
        {
            get { return _lhRH; }
            set { _lhRH = value; }
        }


        public float LhBL
        {
            get { return _lhBL; }
            set { _lhBL = value; }
        }

        public float LhGL
        {
            get { return _lhGL; }
            set { _lhGL = value; }
        }
        public float LhRL
        {
            get { return _lhRL; }
            set { _lhRL = value; }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("Rlow");
            subElem.InnerText = _lhRL.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Rhigh");
            subElem.InnerText = _lhRH.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Glow");
            subElem.InnerText = _lhGL.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Ghigh");
            subElem.InnerText = _lhGH.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Blow");
            subElem.InnerText = _lhBL.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Bhigh");
            subElem.InnerText = _lhBH.ToString();
            xmlElem.AppendChild(subElem);
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            HistGetGradesArg it = new HistGetGradesArg();
            it.LhRL = _lhRL;
            it.LhGL = _lhGL;
            it.LhBL = _lhBL;
            it.LhRH = _lhRH;
            it.LhGH = _lhGH;
            it.LhBH = _lhBH;
            return it;
        }
    }
}