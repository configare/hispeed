using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace GeoDo.RSS.Layout.GDIPlus
{
    //<GxdLaytouTemplate name="" fullname="">
    public class GxdTemplateHost:GxdItem,IGxdTemplateHost
    {
        protected ILayoutTemplate _layoutTemplate;

        public GxdTemplateHost(ILayoutTemplate layoutTemplate)
        {
            _layoutTemplate = layoutTemplate;
        }

        public ILayoutTemplate LayoutTemplate
        {
            get { return _layoutTemplate; }
        }

        public override XElement ToXml()
        {
            XElement ele = new XElement("GxdLaytouTemplate");
            string name = _layoutTemplate != null ?_layoutTemplate.Name :string.Empty;
            string fullname = _layoutTemplate != null ? _layoutTemplate.FullPath : string.Empty;
            ele.SetAttributeValue("name", name);
            ele.SetAttributeValue("fullname", fullname);
            ele.SetValue(GetTemplateContentString());
            return ele;
        }

        private object GetTemplateContentString()
        {
            if (_layoutTemplate == null)
                return string.Empty;
            else
            {
                XmlDocument doc = LayoutToFile.SaveToXmlDocument(_layoutTemplate, _layoutTemplate.Layout);
                if (doc == null )
                    return string.Empty;
                return doc.InnerXml;
            }
        }
    }
}
