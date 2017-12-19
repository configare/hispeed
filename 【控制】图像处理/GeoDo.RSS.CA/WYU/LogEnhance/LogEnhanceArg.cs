using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.CA;
using System.Xml;

namespace GeoDo.RSS.CA
{
    public class LogEnhanceArg:RgbProcessorArg
    {
        private bool _isSameArg = true;
        private List<int> _scales = new List<int>(new int[4] { 10, 0, 0, 0 });
        private List<double> _bases = new List<double>(new double[4] { Math.E, 0, 0, 0 });

        public LogEnhanceArg()
        {
        }

        public bool IsSameArg
        {
            get { return _isSameArg; }
            set { _isSameArg = value; }
        }

        /// <summary>
        /// 0:全图 or Grey
        /// 1:Red 
        /// 2:Green
        /// 3:Blue
        /// </summary>
        public List<double> LogBases
        {
            get { return _bases; }
        }

        /// <summary>
        /// 0:全图 or Grey
        /// 1:Red 
        /// 2:Green
        /// 3:Blue
        /// </summary>
        public List<int> Scales
        {
            get { return _scales; }
        }

        public override System.Xml.XmlElement ToXML(System.Xml.XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("IsSameArg");
            subElem.InnerText = _isSameArg.ToString();
            xmlElem.AppendChild(subElem);
            for (int i = 0; i < _bases.Count; i++)
            {
                XmlElement min = xmldoc.CreateElement("LogBases" + i.ToString());
                min.InnerText = _bases[i].ToString();
                xmlElem.AppendChild(min);
            }
            for (int i = 0; i < _scales.Count; i++)
            {
                XmlElement max = xmldoc.CreateElement("Scales" + i.ToString());
                max.InnerText = _scales[i].ToString();
                xmlElem.AppendChild(max);
            }
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            LogEnhanceArg arg = new LogEnhanceArg();
            return arg;
        }
    }
}
