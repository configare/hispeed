using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    public class MapAuthor:IPersistable
    {
        private string _author = string.Empty;
        private DateTime _beginMakeTime = DateTime.MinValue;
        private DateTime _endMakeTime = DateTime.MinValue;

        public MapAuthor()
        { 
        }

        public MapAuthor(string author, DateTime makeTime)
        {
            _author = author;
            _beginMakeTime = makeTime;
            _endMakeTime = makeTime;
        }

        public MapAuthor(string author, DateTime beginMakeTime, DateTime endMakeTime)
        {
            _author = author;
            _beginMakeTime = beginMakeTime;
            _endMakeTime = endMakeTime;
        }

        [DisplayName("地图制作者"), ReadOnly(true)]
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        [DisplayName("开始制作日期"), ReadOnly(true)]
        public DateTime BeginMakeTime
        {
            get { return _beginMakeTime; }
            set { _beginMakeTime = value; }
        }

        [DisplayName("结束制作日期"),ReadOnly(true)]
        public DateTime EndMakeTime
        {
            get { return _endMakeTime; }
            set { _endMakeTime = value; }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Author");
            obj.AddAttribute("author", _author);
            obj.AddAttribute("beginmaketime", _beginMakeTime.ToString("yyyy-MM-dd HH:mm:ss"));
            obj.AddAttribute("endmaketime", _endMakeTime.ToString("yyyy-MM-dd HH:mm:ss"));
            return obj;
        }

        #endregion

        public static MapAuthor FromXElement(XElement xelement)
        {
            if (xelement == null)
                return null;
            string author = string.Empty;
            author = xelement.Attribute("author").Value;
            DateTime bTime = DateTime.Parse(xelement.Attribute("beginmaketime").Value);
            DateTime eTime = DateTime.Parse(xelement.Attribute("endmaketime").Value);
            return new MapAuthor(author, bTime, eTime);
        }
    }
}
