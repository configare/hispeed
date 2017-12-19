using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.Core.UI
{
    public class Configer:IConfiger
    {
        private XDocument _doc = null;
        private XElement _elem = null;
        private string _fileName = "";

        public Configer()
        {
            _fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\Config.xml";
            if (!File.Exists(_fileName))
                return;
            _doc = XDocument.Load(_fileName);
            if (_doc == null)
                return;
            _elem = _doc.Root.Element("SimpleConfigItems");
        }

        public object GetConfigItemValue(string key)
        {
            if (_elem == null)
                return null;
            foreach (XElement subElem in _elem.Descendants())
            {
                if (subElem.Attribute("key").Value == key)
                {
                    return subElem.Attribute("value").Value;
                }
            }
            return null;
        }

        public void BeginUpdate()
        {
            if (_elem == null)
                return;
        }

        public void UpdateConfigItem(string key, object value)
        {
            if (_elem == null)
                return;
            foreach (XElement subElem in _elem.Descendants())
            {
                if (subElem.Attribute("key").Value == key)
                {
                    subElem.Attribute("value").Value = value.ToString();
                    return;
                }
            }
        }

        public void EndUpdate()
        {
            if (_doc == null)
                return;
            _doc.Save(_fileName);
        }
    }
}
