using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.FileProject
{
    public class ProjectionItem
    {
        public string Name = "";
        public string ShortName = "";
        public string FullName = "";
        public string WktProjection = "";

        public string ToXML()
        {
            return string.Format("<Projection Name=\"{0}\" ShortName=\"{0}\" FullName=\"{0}\">{0}</Projection>", Name, ShortName, FullName, WktProjection);
        }

        public ProjectionItem Clone()
        {
            return new ProjectionItem { Name = Name, ShortName = ShortName, FullName = FullName, WktProjection = WktProjection };
        }
    }

    public class ProjectionSetting
    {
        public ProjectionSetting()
        {
            LoadFromXML();
        }

        private bool _isQuicklyProject = true;
        private bool _isOpenProjected = true;
        private bool _isResolutionFusion = true;
        private ProjectionItem _gLLProjectionRef;
        private ProjectionItem[] _otherProjections;

        public bool IsOpenProjected
        {
            get { return _isOpenProjected; }
            set { _isOpenProjected = value; }
        }

        public bool IsQuicklyProject
        {
            get { return _isQuicklyProject; }
            set { _isQuicklyProject = value; }
        }

        public bool IsResolutionFusion
        {
            get { return _isResolutionFusion; }
            set { _isResolutionFusion = value; }
        }

        public ProjectionItem GLLProjectionRef
        {
            get { return _gLLProjectionRef; }
            set { _gLLProjectionRef = value; }
        }

        public ProjectionItem[] OtherProjections
        {
            get { return _otherProjections; }
            set { _otherProjections = value; }
        }

        public void LoadFromXML()
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "ProjectionSetting.xml";
            XElement doc = XElement.Load(fileName);
            XElement defaultControl = doc.Element("DefaultControl");
            XElement isQuicklyprj = defaultControl.Element("IsQuicklyProject");
            _isQuicklyProject = isQuicklyprj.Value == "true";
            XElement isOpenProjected = defaultControl.Element("IsOpenProjected");
            _isQuicklyProject = isOpenProjected.Value == "true";
            XElement gLLProjectionRef = doc.Element("GLLProjectionRef");
            _gLLProjectionRef = GetProjectionItem(gLLProjectionRef);
            XElement otherProjections = doc.Element("OtherProjections");
            IEnumerable<ProjectionItem> ps = from item in otherProjections.Elements("Projection")
                                             select GetProjectionItem(item);
            _otherProjections = ps.ToArray();
        }

        private ProjectionItem GetProjectionItem(XElement xelement)
        {
            return new ProjectionItem
            {
                Name = xelement.Attribute("Name").Value,
                ShortName = xelement.Attribute("ShortName").Value,
                FullName = xelement.Attribute("FullName").Value,
                WktProjection = xelement.Value
            };
        }

        public void SaveToXML()
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "ProjectionSetting.xml";
            XElement otherPrjs = new XElement("OtherProjections");
            foreach (ProjectionItem item in _otherProjections)
            {
                otherPrjs.Add(new XElement("Projection",
                    new XAttribute("Name", item.Name),
                    new XAttribute("ShortName", item.ShortName),
                    new XAttribute("FullName", item.FullName),
                    item.WktProjection));
            }
            XElement doc = new XElement("ProjectionSetting",
                new XElement("DefaultControl",
                    new XElement("IsQuicklyProject", _isQuicklyProject),
                    new XElement("IsOpenProjected", _isQuicklyProject)),
                new XElement("GLLProjectionRef",
                    new XAttribute("Name", _gLLProjectionRef.Name),
                    new XAttribute("ShortName", _gLLProjectionRef.ShortName),
                    new XAttribute("FullName", _gLLProjectionRef.FullName),
                    _gLLProjectionRef.WktProjection),
                otherPrjs);
            doc.Save(fileName);
        }
    }
}
