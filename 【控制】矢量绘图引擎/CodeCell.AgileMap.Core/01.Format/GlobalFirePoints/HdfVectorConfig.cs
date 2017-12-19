using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    public class HdfVectorConfig
    {
        private static string HdfVectorConfigFile = "HdfGlobalFirePointReader.xml";

        public static HdfVector[] Parse()
        {
            string configFile = System.AppDomain.CurrentDomain.BaseDirectory + HdfVectorConfigFile;
            XElement root = XElement.Load(configFile);
            IEnumerable<XElement> eles = root.Elements("HdfVector");
            List<HdfVector> HdfVectors = new List<HdfVector>();
            foreach (XElement ele in eles)
            {
                string bandname = ele.Attribute("bandname").Value;
                string longrowindex = ele.Attribute("longrowindex").Value;
                string latrowindex = ele.Attribute("latrowindex").Value;
                Dictionary<int, string> fields = new Dictionary<int, string>();
                XElement fieldrot = ele.Element("Fields");
                IEnumerable<XElement> fieldEles = fieldrot.Elements("Field");
                foreach (XElement fieldEle in fieldEles)
                {
                    string name = fieldEle.Attribute("name").Value;
                    string rowindex = fieldEle.Attribute("rowindex").Value;
                    fields.Add(int.Parse(rowindex), name);
                }
                HdfVector hdfVector = new HdfVector();
                hdfVector.BandName = bandname;
                hdfVector.LongIndex = int.Parse(longrowindex);
                hdfVector.LatIndex = int.Parse(latrowindex);
                hdfVector.Fields = fields;
                HdfVectors.Add(hdfVector);
            }
            return HdfVectors.ToArray();
        }
    }

    public class HdfVector
    {
        private string _bandName;

        public string BandName
        {
            get { return _bandName; }
            set { _bandName = value; }
        }
        private int _longIndex;

        public int LongIndex
        {
            get { return _longIndex; }
            set { _longIndex = value; }
        }
        private int _latIndex;

        public int LatIndex
        {
            get { return _latIndex; }
            set { _latIndex = value; }
        }
        private Dictionary<int, string> _fields;

        public Dictionary<int, string> Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }
    }
}
