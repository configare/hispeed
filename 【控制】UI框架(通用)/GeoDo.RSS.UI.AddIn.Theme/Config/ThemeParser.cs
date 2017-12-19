using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public static class ThemeParser
    {
        public static ThemeConfigItem[] Parse()
        {
            try
            {
                string fname = AppDomain.CurrentDomain.BaseDirectory + "Themes.xml";
                if (!File.Exists(fname))
                    return null;
                XDocument doc = XDocument.Load(fname);
                return ParserThemeConfigItems(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        private  static ThemeConfigItem[] ParserThemeConfigItems(XDocument doc)
        {
            XElement root = doc.Element("Themes");
            if (root == null)
                return null;
            List<ThemeConfigItem> items = new List<ThemeConfigItem>();
            foreach (XElement ele in root.Elements("Theme"))
            {
                bool visible = GetBoolAttribute(ele, "visible");
                string fname = GetStringAttribute(ele, "file");
                if (string.IsNullOrEmpty(fname))
                    continue;
                if (!fname.Contains(":"))
                    fname = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fname);
                items.Add(new ThemeConfigItem(fname, visible));
            }
            return items.ToArray();
        }

        private static bool GetBoolAttribute(XElement itElement, string p)
        {
            if (itElement.Attribute("visible") == null)
                return true;
            string bstr = itElement.Attribute("visible").Value;
            bool v = true;
            if (bool.TryParse(bstr, out v))
                return v;
            return true;
        }

        private static string GetStringAttribute(XElement ele, string attName)
        {
            if (ele == null)
                return string.Empty;
            if (ele.Attribute(attName) == null)
                return string.Empty;
            return ele.Attribute(attName).Value;
        }
    }
}
