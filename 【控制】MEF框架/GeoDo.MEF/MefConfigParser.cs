using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.MEF
{
    public static class MefConfigParser
    {
        private static Dictionary<string, List<string>> _catalogs = new Dictionary<string, List<string>>();

        static MefConfigParser()
        { 
            string fname = AppDomain.CurrentDomain.BaseDirectory+@"SystemData\MefConfig.xml";
            XDocument doc = XDocument.Load(fname);
            foreach (XElement ele in doc.Root.Element("MefCatalogs").Elements("MefCatalog"))
            {
                string name = ele.Attribute("name").Value;
                List<string> dlls = new List<string>();
                foreach (XElement subele in ele.Elements("MefCatalogItem"))
                {
                    dlls.Add(AppDomain.CurrentDomain.BaseDirectory + subele.Attribute("assembly").Value);
                }
                _catalogs.Add(name, dlls);
            }
        }

        public static string[] GetAssemblysByCatalog(string catalog)
        {
            if (string.IsNullOrEmpty(catalog))
                return null;
            if (_catalogs.ContainsKey(catalog))
                return _catalogs[catalog].ToArray();
            return null;
        }
    }
}
