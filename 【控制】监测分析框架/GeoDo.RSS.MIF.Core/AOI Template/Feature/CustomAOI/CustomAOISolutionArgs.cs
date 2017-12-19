using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Core
{
    public class CustomAOISolutionArgs
    {
        public static string _newSolutionXML = AppDomain.CurrentDomain.BaseDirectory+ "SystemData\\CustomAOISolution.XML";
        public string _newSolutionName;
        public string _vectorName;
        public string _keyFieldName;
        public string[] _statRegionNames;
        public string[] _tryDeletedSolutions;     

        public CustomAOISolutionArgs()
        {

        }

        public string[] SolutionNames
        {
            get
            {
                if (!File.Exists(_newSolutionXML))
                    return null;//throw new ArgumentException("不存在可加载的自定义方案！请先添加自定义方案！");
                return GetAllSolutions();
            }
        }

        public void GetUniqueSolution(String SolutionName,out string vectorname,out string keyfieldName,out string [] statAOIValues)
        {
            vectorname = null;
            keyfieldName = null;
            statAOIValues = null;
            XDocument _doc = XDocument.Load(_newSolutionXML);
            if (_doc == null)
                return;
            XElement root = _doc.Root;
            IEnumerable<XElement> elements = root.Elements();
            foreach (XElement ele in elements)
            {
                if (ele.Name == "CustomAOISolution" 
                    && ele.Attributes("slsname").Count() != 0 
                    && ele.Attribute("slsname").Value == SolutionName)
                {
                    vectorname = ele.Element("vectorName").Value;
                    keyfieldName = ele.Element("keyFieldName").Value;
                    statAOIValues = ele.Element("statRegionNames").Value.Split(',');
                    break;
                }
            }
        }

        private string[] GetAllSolutions()
        {
            List<string> allSolutionList = new List<string>();
            XDocument _doc = XDocument.Load(_newSolutionXML);
            if (_doc == null)
                return null;
            XElement root = _doc.Root;
            IEnumerable<XElement> elements = root.Elements();
            foreach (XElement ele in elements)
            {
                if (ele.Name == "CustomAOISolution" && ele.Attributes("slsname").Count()!=0)
                {
                    allSolutionList.Add(ele.Attribute("slsname").Value);
                }
            }
            return allSolutionList.ToArray();
        }

        #region 存储
        public void ToXml()
        {
            string fileName = _newSolutionXML;
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("xmlFilename");
            XElement xml;
            if (File.Exists(fileName))
                xml = XElement.Load(fileName);
            else
                xml = new XElement("CustomAOISolutions");
            xml.Add(new XElement("CustomAOISolution", new XAttribute("slsname", _newSolutionName), new XElement("vectorName", _vectorName), new XElement("keyFieldName", _keyFieldName), new XElement("statRegionNames", statRegionNamesToXmlValue(_statRegionNames))));
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            xml.Save(fileName);
        }

        private string statRegionNamesToXmlValue(string[] Bands)
        {
            if (Bands == null || Bands.Length == 0)
                return null;
            string envs = "";
            foreach (string item in Bands)
                envs += item + ",";
            envs = envs.TrimEnd(',');
            return envs;
        }

        public void UpdateDeleteSolutions()
        {
            if (!File.Exists(_newSolutionXML))
                throw new ArgumentException("不存在可加载的自定义方案！请先添加自定义方案！");
            XElement xml = XElement.Load(_newSolutionXML);
            IEnumerable<XElement> elements = xml.Elements();
            foreach (string name in _tryDeletedSolutions)
            {
                foreach (XElement ele in elements)
                {
                    if (ele.Name == "CustomAOISolution" 
                        && ele.Attributes("slsname").Count() != 0 
                        && ele.Attribute("slsname").Value == name)
                    {
                        ele.RemoveAll();
                        xml.Save(_newSolutionXML);
                    }
                }
            }
        }
       #endregion


    }
}
