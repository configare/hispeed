using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    public class WeightedAveFilterArg : RgbWndProcessorArg
    {
        private int[] _weight = null;

        public WeightedAveFilterArg():base()
        {
            _weight = new int[9];
            for (int i = 0; i < 9; i++)
            {
                _weight[i] = 1;
            }
        }

        public int[] Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("Height");
            subElem.InnerText = _wndHeight.ToString();
            xmlElem.AppendChild(subElem);
            subElem = xmldoc.CreateElement("Width");
            subElem.InnerText = _wndWidth.ToString();
            xmlElem.AppendChild(subElem);
            XmlElement subweights = xmldoc.CreateElement("Weights");
            xmlElem.AppendChild(subweights);
            XmlElement subweightelem;
            foreach (int w in _weight)
            {
                subweightelem = xmldoc.CreateElement("Weight");
                subweightelem.InnerText = w.ToString();
                subweights.AppendChild(subweightelem);
            }
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            WeightedAveFilterArg arg = new WeightedAveFilterArg();
            return arg;
        }
    }
}
