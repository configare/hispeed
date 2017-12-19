using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using GeoDo.RasterProject;
using GeoDo.FileProject;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    /// <summary>
    /// SystemData\ProductArgs\ThemeGraphRegion_{产品标识}.xml
    /// 如SystemData\ProductArgs\ThemeGraphRegion_DST.xml
    /// </summary>
    public class ThemeGraphRegionSetting
    {
        private static string path = System.AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\";

        public static ThemeGraphRegion GetThemeGraphRegion(string productionIdentify)
        {
            if (string.IsNullOrWhiteSpace(productionIdentify))
                return null;
            string productPath = path + "ThemeGraphRegion_" + productionIdentify + ".xml";
            if (!File.Exists(productPath))
            {
                ThemeGraphRegion region = new ThemeGraphRegion();
                region.Enable = false;
                region.ProductIdentify = productionIdentify;
                region.SelectedIndex = -1;
                return region;
            }
            else
            {
                XElement xml = XElement.Load(productPath);
                bool enabled = GetAttBool(xml, "enabled");
                int selectedindex = GetAttInt(xml, "selectedindex");
                PrjEnvelopeItem[] items = ParseEnvelopes(xml);
                ThemeGraphRegion region = new ThemeGraphRegion();
                region.ProductIdentify = productionIdentify;
                region.Enable = enabled;
                region.SelectedIndex = selectedindex;
                region.PrjEnvelopeItems = items;
                return region;
            }
        }

        public static void SaveThemeGraphRegion(ThemeGraphRegion region)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            XElement xml = new XElement("ThemeGraphRegion",
                new XAttribute("enabled", region.Enable),
                new XAttribute("selectedindex", region.SelectedIndex),
                from block in region.PrjEnvelopeItems
                select new XElement("EnvelopeItem",
                    new XAttribute("name", block.Name),
                    new XAttribute("minLongitude", block.PrjEnvelope.MinX),
                    new XAttribute("minLatitude", block.PrjEnvelope.MinY),
                    new XAttribute("maxLongitude", block.PrjEnvelope.MaxX),
                    new XAttribute("maxLatitude", block.PrjEnvelope.MaxY)));
            string productPath = path + "ThemeGraphRegion_" + region.ProductIdentify + ".xml";
            xml.Save(productPath, SaveOptions.None);
        }

        private static PrjEnvelopeItem[] ParseEnvelopes(XElement xml)
        {
            List<PrjEnvelopeItem> items = new List<PrjEnvelopeItem>();
            IEnumerable<XElement> els = xml.Elements("EnvelopeItem");
            foreach (XElement item in els)
            {
                string name = item.Attribute("name").Value;
                PrjEnvelope env = ParseEnvelope(item);
                if (env != null)
                {
                    PrjEnvelopeItem envItem = new PrjEnvelopeItem(name, env);
                    items.Add(envItem);
                }
            }
            return items.ToArray();
        }

        //<EnvelopeItem name="海河流域" minLongitude="113.94" minLatitude="36.94" maxLongitude="119.06" maxLatitude="42.06" />
        private static PrjEnvelope ParseEnvelope(XElement item)
        {
            string minLongitude = item.Attribute("minLongitude").Value;
            string minLatitude = item.Attribute("minLatitude").Value;
            string maxLongitude = item.Attribute("maxLongitude").Value;
            string maxLatitude = item.Attribute("maxLatitude").Value;
            double minLongitudei, minLatitudei, maxLongitudei, maxLatitudei;
            if (double.TryParse(minLongitude, out minLongitudei) && double.TryParse(minLatitude, out minLatitudei)
            && double.TryParse(maxLongitude, out maxLongitudei) && double.TryParse(maxLatitude, out maxLatitudei))
            {
                PrjEnvelope env = new PrjEnvelope(minLongitudei, maxLongitudei, minLatitudei, maxLatitudei);
                return env;
            }
            else
                return null;
        }

        private static bool GetAttBool(XElement xml, string attname)
        {
            bool enabled = false;
            XAttribute att = xml.Attribute(attname);
            if (att != null)
                bool.TryParse(att.Value, out enabled);
            return enabled;
        }

        private static int GetAttInt(XElement xml, string attname)
        {
            int val = -1;
            XAttribute att = xml.Attribute(attname);
            if (att != null)
                int.TryParse(att.Value, out val);
            return val;
        }
    }

    public class ThemeGraphRegion
    {
        public string ProductIdentify;
        private bool _enable = false;
        private int _selectedIndex = -1;
        private List<PrjEnvelopeItem> _prjEnvelopeItems;

        public PrjEnvelopeItem[] PrjEnvelopeItems
        {
            get
            {
                return _prjEnvelopeItems.ToArray();
            }
            set
            {
                if (value == null)
                    _prjEnvelopeItems.Clear();
                else
                    _prjEnvelopeItems = new List<PrjEnvelopeItem>(value);
            }
        }

        public ThemeGraphRegion()
        {
            _prjEnvelopeItems = new List<PrjEnvelopeItem>();
        }

        public bool Enable
        {
            get
            {
                return _enable;
            }
            set
            {
                _enable = value;
            }
        }

        public PrjEnvelopeItem SelectedItem
        {
            get
            {
                if (!Enable)
                    return null;
                if (_prjEnvelopeItems == null || _prjEnvelopeItems.Count == 0)
                    return null;
                if (SelectedIndex >= _prjEnvelopeItems.Count)
                    return null;
                return _prjEnvelopeItems[SelectedIndex];
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
            }
        }

        public void Add(PrjEnvelopeItem item)
        {
            int findIndex = -1;
            for (int i = 0; i < _prjEnvelopeItems.Count; i++)
            {
                if (_prjEnvelopeItems[i].Name == item.Name)
                {
                    findIndex = i;
                }
            }
            if (findIndex == -1)
            {
                _prjEnvelopeItems.Add(item);
                _selectedIndex = _prjEnvelopeItems.Count - 1;
            }
            else
            {
                _prjEnvelopeItems[findIndex] = item;
                _selectedIndex = findIndex;
            }
            Enable = true;
        }
    }
}
