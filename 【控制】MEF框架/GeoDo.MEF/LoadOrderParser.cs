using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.MEF
{
    public class LoadOrderParser
    {
        public string[] Parse(string fname)
        {
            XDocument xdoc = XDocument.Load(fname);
            XElement root = xdoc.Root;
            if (root.Name.LocalName != "Components")
                return null;
            List<string> names = new List<string>();
            foreach (XElement ele in root.Elements(XName.Get("Component")))
            {
                XAttribute att = ele.Attribute(XName.Get("fullname"));
                if (att == null)
                    continue;
                string fullname = att.Value;
                if (fullname.Trim() == string.Empty || names.Contains(fullname))
                    continue;
                names.Add( att.Value);
            }
            return names.Count > 0 ? names.ToArray() : null;
        }
    }
}
