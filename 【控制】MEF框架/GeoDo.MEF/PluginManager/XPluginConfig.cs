using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.MEF
{
    public class XTheme
    {
        public XThemeItem[] Items = null;
    }

    public class XThemeItem
    {
        private XElement _content;

        public XThemeItem()
        { }

        public string LinkinFullname;
        public string Name { get; set; }
        public string Linkin;

        public XElement Content
        {
            get
            {
                if (_content != null)
                    return _content;
                if (string.IsNullOrWhiteSpace(LinkinFullname) || !File.Exists(LinkinFullname))
                    return null;
                _content = TryLoadContent();
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        private XElement TryLoadContent()
        {
            return XElement.Load(LinkinFullname);
        }
    }
}
