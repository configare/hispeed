using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public class FeatureListStatConfig
    {
        private const string FileName = @"SystemData\FeatureListWndContent.xml";

        public static StatContent[] LoadStatContents()
        {
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + FileName;
            XElement xml = XElement.Load(filename);
            IEnumerable<XElement> xStatContents = xml.Elements("StatContent");
            List<StatContent> statItems = new List<StatContent>();
            foreach (XElement item in xStatContents)
            {
                string identify = item.Attribute("identify").Value;
                string filter = item.Attribute("layerfilter").Value;
                Dictionary<string, int> statItem = ParseStatItem(item);
                StatContent statContent = new StatContent();
                statContent.Identify = identify;
                statContent.LayerFilter = filter;
                statContent.StatItems = statItem;
                statItems.Add(statContent);
            }
            return statItems.ToArray();
        }

        private static Dictionary<string, int> ParseStatItem(XElement item)
        {
            IEnumerable<XElement> xmls = item.Elements("StatItem");
            int count = xmls.Count();
            Dictionary<string, int> statItems = new Dictionary<string, int>();
            foreach (XElement x in xmls)
            {
                string name = x.Attribute("name").Value;
                string value = x.Attribute("value").Value;
                int ivalue;
                int.TryParse(value, out ivalue);
                statItems.Add(name, ivalue);
            }
            return statItems;
        }

        public static StatContent GetStatContent(string identify)
        {
            StatContent[] statItems = LoadStatContents();
            foreach (StatContent item in statItems)
            {
                if (item.Identify == identify)
                    return item;
            }
            return null;
        }

        public static StatContent MatchStatContent(string input)
        {
            StatContent[] statItems = LoadStatContents();
            foreach (StatContent item in statItems)
            {
                if (item.MatchFilter.IsMatch(input))
                    return item;
            }
            return null;
        }
    }

    public class StatContent
    {
        private string _identify;
        private string _filter;
        private Dictionary<string, int> _statItem;
        private Regex _regex;

        public string Identify
        {
            get { return _identify; }
            set { _identify = value; }
        }

        public string LayerFilter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                    _regex = new Regex(value, RegexOptions.Compiled);
                _filter = value;
            }
        }

        public Dictionary<string, int> StatItems
        {
            get { return _statItem; }
            set { _statItem = value; }
        }

        public Regex MatchFilter
        {
            get
            {
                return _regex;
            }
        }
    }
}
