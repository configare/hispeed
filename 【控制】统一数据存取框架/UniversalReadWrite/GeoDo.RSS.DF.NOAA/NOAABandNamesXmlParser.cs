using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.NOAA
{
    internal class NOAABandNamesXmlParser : IDisposable
    {
        private string _cnfgFile = null;

        public NOAABandNamesXmlParser(string cnfgfile)
        {
            _cnfgFile = cnfgfile;
        }

        public string[] GetBandNames(string satellite,string sensor)
        {
            string[] bandNames = null;
            List<string> bandNameList = new List<string>();
            XDocument doc = XDocument.Load(_cnfgFile);
            XElement root = doc.Root;
            var eleNames = root.Elements(XName.Get("BandnameRefTable"));
            foreach (var eleName in eleNames)
            {
                if (eleName.Attribute("satellite").Value.Equals(satellite) && eleName.Attribute("sensor").Value.Equals(sensor))
                {
                    foreach (XElement name in eleName.Elements())
                    {
                        bandNameList.Add(name.Attribute("name").Value);
                    }
                }
            }
            bandNames = bandNameList.ToArray();
            return bandNames;
        }

        public string[] GetBandNames(string satellite)
        {
            string[] bandNames = null;
            List<string> bandNameList = new List<string>();
            XDocument doc = XDocument.Load(_cnfgFile);
            XElement root = doc.Root;
            var eleNames = root.Elements(XName.Get("BandnameRefTable"));
            foreach (var eleName in eleNames)
            {
                if (eleName.Attribute("satellite").Value.Equals(satellite))
                {
                    foreach (XElement name in eleName.Elements())
                    {
                        bandNameList.Add(name.Attribute("name").Value);
                    }
                }
            }
            bandNames = bandNameList.ToArray();
            if (bandNameList.Count==0)
            {
                bandNames = new string[] { "可见光", "近红外", "短波红外(3A或者3B)", "远红外", "远红外" };
            }
            return bandNames;
        }

        public void Dispose()
        {
        }
    }
}
