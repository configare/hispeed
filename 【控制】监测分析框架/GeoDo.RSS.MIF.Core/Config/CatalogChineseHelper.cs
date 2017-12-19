using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Core
{
    public class CatalogCNHelper
    {
        public static Dictionary<string, Dictionary<string, string>> GetDic()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\CatalogCN\";
            if (!Directory.Exists(dir))
                return null;
            string[] files = Directory.GetFiles(dir, "*.xml", SearchOption.AllDirectories);
            Dictionary<string, string> catalogCN = null;
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            for (int i = 0; i < files.Length; i++)
            {
                XDocument doc = XDocument.Load(files[i]);
                if (doc == null)
                    return null;
                XElement product = doc.Element("ProductIdentify");
                if (product == null)
                    return null;
                catalogCN = GetCatalogCN(product);
                if (catalogCN == null)
                    continue;
                XAttribute attr = product.Attribute("name");
                if (attr == null || string.IsNullOrWhiteSpace(attr.Value))
                    continue;
                result.Add(attr.Value.ToString().ToUpper(), catalogCN);
            }
            return result.Count == 0 ? null : result;
        }

        private static Dictionary<string, string> GetCatalogCN(XElement element)
        {
            XElement[] eles = element.Elements("CatalogCN").ToArray();
            if (eles == null || eles.Length == 0)
                return null;
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (XElement item in eles)
            {
                if (item.Attributes("cnname").Count() == 0)
                    continue;
                result.Add(item.Attribute("identify").Value.ToString().ToUpper(),
                           item.Attribute("cnname").Value.ToString().ToUpper());
            }
            return result.Count == 0 ? null : result;
        }
    }
}
