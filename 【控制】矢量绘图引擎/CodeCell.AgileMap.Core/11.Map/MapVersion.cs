using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    public class MapVersion:IPersistable
    {
        private int _authorYear = 0;
        private int _scaleOfMapping = 0;
        private string _source = string.Empty;
        private string _publisher = string.Empty;
        private string _description = string.Empty;

        public MapVersion()
        { 
        }

        public MapVersion(int authorYear, int scaleOfMapping, string source, string publisher, string description)
        {
            _authorYear = authorYear;
            _source = source;
            _publisher = publisher;
            _description = description;
        }

        [DisplayName("出版年代"), ReadOnly(true)]
        public int AuthorYear
        {
            get { return _authorYear; }
            set { _authorYear = value; }
        }

        [DisplayName("测绘比例尺"), ReadOnly(true)]
        public int ScaleOfMapping
        {
            get { return _scaleOfMapping; }
            set { _scaleOfMapping = value; }
        }


        [DisplayName("数据发布机构"), ReadOnly(true)]
        public string Publisher
        {
            get { return _publisher; }
            set { _publisher = value; }
        }

        [DisplayName("数据来源"), ReadOnly(true)]
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        [DisplayName("描述信息"), ReadOnly(true)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Version");
            obj.AddAttribute("year", _authorYear.ToString());
            obj.AddAttribute("scaleofmapping", _scaleOfMapping.ToString());
            obj.AddAttribute("publisher", _publisher);
            obj.AddAttribute("source", _source);
            obj.AddAttribute("description", _description);
            return obj;
        }

        #endregion

        public static MapVersion FromXElement(XElement xelement)
        {
            if (xelement == null)
                return null;
            MapVersion version = new MapVersion();
            version.AuthorYear = int.Parse(xelement.Attribute("year").Value);
            version.Description = xelement.Attribute("description").Value;
            version.Publisher = xelement.Attribute("publisher").Value;
            version.ScaleOfMapping = int.Parse(xelement.Attribute("scaleofmapping").Value);
            if(xelement.Attribute("source")!=null)
                version.Source = xelement.Attribute("source").Value;
            return version;
        }
    }
}
