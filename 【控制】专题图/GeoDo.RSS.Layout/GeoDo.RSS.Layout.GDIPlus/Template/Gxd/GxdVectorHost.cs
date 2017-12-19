using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class GxdVectorHost : GxdItem,IGxdVectorHost
    {
        protected object _mcdFileContent;

        public GxdVectorHost(object mcdFileContent)
        {
            _mcdFileContent = mcdFileContent;
        }

        public object McdFileContent
        {
            get { return _mcdFileContent; }
        }

        public override XElement ToXml()
        {
            XElement ele = new XElement("GxdVectorHost");
            ele.SetValue(_mcdFileContent != null ? _mcdFileContent.ToString() : string.Empty);
            return ele;
        }
    }
}
