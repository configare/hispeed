using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class CatalogItemInfo
    {
        protected Dictionary<string, object> _properties = new Dictionary<string, object>();

        public CatalogItemInfo()
        {
        }

        public CatalogItemInfo(string fname)
        {
            string[] lines = File.ReadAllLines(fname, Encoding.Default);
            if (lines == null || lines.Length == 0)
                return;
            foreach (string lne in lines)
            {
                string[] parts = lne.Split('=');
                if (parts.Length != 2)
                    continue;
                if (_properties.ContainsKey(parts[0]))
                    continue;
                _properties.Add(parts[0], parts[1]);
            }
        }

        public Dictionary<string, object> Properties
        {
            get { return _properties; }
        }

        public string GetPropertyValue(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return string.Empty;
            if (_properties.ContainsKey(propertyName) && _properties[propertyName] != null)
                return _properties[propertyName].ToString();
            return string.Empty;
        }

        public void SaveTo(string fname)
        {
            using (StreamWriter sw = new StreamWriter(fname, false, Encoding.Default))
            {
                foreach (string key in _properties.Keys)
                {
                    object v = _properties[key];
                    if (v == null)
                        continue;
                    sw.WriteLine(key + "=" + v.ToString());
                }
            }
        }
    }
}
