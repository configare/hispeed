using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.CA;
using System.Xml;

namespace GeoDo.RSS.CA
{
    public class ExponentEnhanceArg : RgbProcessorArg
    {
        private bool _isSameArg = true;
        private int[] _mins = new int[3];
        private int[] _maxs = new int[3];

        public ExponentEnhanceArg()
        {
        }

        /// <summary>
        /// 三个通道使用相同的参数
        /// </summary>
        public bool IsSameArg
        {
            get { return _isSameArg; }
            set { _isSameArg = value; }
        }

        /// <summary>
        /// 增强的下限
        /// 0:RGB || R
        /// 1:G
        /// 2:B
        /// </summary>
        public int[] Mins
        {
            get { return _mins; }
        }

        /// <summary>
        /// 增强的上限
        /// 0:RGB || R
        /// 1:G
        /// 2:B
        /// </summary>
        public int[] Maxs
        {
            get { return _maxs; }
        }

        public void SaveToFile(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.Default))
            {
                sw.WriteLine(_isSameArg.ToString());
                foreach (float max in Maxs)
                    sw.WriteLine(max.ToString());
                foreach (int min in Mins)
                    sw.WriteLine(min.ToString());
            }
        }

        public void FillByFile(string filename)
        {
            using (StreamReader sr = new StreamReader(filename, Encoding.Default))
            {
                _isSameArg = sr.ReadLine().ToUpper() == "TRUE" ? true : false;
                for (int i = 0; i < 3; i++)
                    _maxs[i] = int.Parse(sr.ReadLine());
                for (int i = 0; i < 3; i++)
                    _mins[i] = int.Parse(sr.ReadLine());
            }
        }

        public override System.Xml.XmlElement ToXML(System.Xml.XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subElem = xmldoc.CreateElement("IsSameArg");
            subElem.InnerText = _isSameArg.ToString();
            xmlElem.AppendChild(subElem);
            for (int i = 0; i < _mins.Length;i++)
            {
                XmlElement min = xmldoc.CreateElement("Mins" + i.ToString());
                min.InnerText = _mins[i].ToString();
                xmlElem.AppendChild(min);
            }
            for (int i = 0; i < _maxs.Length; i++)
            {
                XmlElement max = xmldoc.CreateElement("Maxs" + i.ToString());
                max.InnerText = _maxs[i].ToString();
                xmlElem.AppendChild(max);
            }
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            ExponentEnhanceArg arg = new ExponentEnhanceArg();
            return arg;
        }
    }
}
