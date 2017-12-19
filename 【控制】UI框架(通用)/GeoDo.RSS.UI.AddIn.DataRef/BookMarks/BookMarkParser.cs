using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    public class BookMarkParser
    {
        private List<BookMarkGroup> _groups = null;
        private Dictionary<string, CoordEnvelope> _bookMarks = null;
        private XDocument _doc = null;
        private List<string> _groupNames = null;
        private XElement _root = null;
        private string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\BookMarkConfig.xml");

        public BookMarkParser()
        {
            _groups = new List<BookMarkGroup>();
            _bookMarks = new Dictionary<string, CoordEnvelope>();
            _groupNames = new List<string>();
            Parser();
        }

        public BookMarkGroup[] BookMarkGroups
        {
            get
            {
                if (_groups != null && _groups.Count != 0)
                    return _groups.ToArray();
                else
                    return null;
            }
        }

        public Dictionary<string, CoordEnvelope> BookMarks
        {
            get { return _bookMarks; }
        }

        private void Parser()
        {
            if (!File.Exists(_path))
                _root = CreatBookMarkXmlFile(_path); //如果xml文件不存在则创建新的文件
            else
            {
                _doc = XDocument.Load(_path);
                if (_doc == null)
                    return;
                _root = _doc.Root;
            }
            if (_root == null)
                return;
            IEnumerable<XElement> groupEles = _root.Elements(XName.Get("BookMarkGroup"));
            if (groupEles == null || groupEles.Count() == 0)
                return;
            RefreshGroups(groupEles);
        }

        private void RefreshGroups(IEnumerable<XElement> groupEles)
        {
            BookMarkGroup group = null;
            _groupNames.Clear();
            _groups.Clear();
            _bookMarks.Clear();
            foreach (XElement ele in groupEles)
            {
                if (ele == null)
                    continue;
                group = CreatGroupFromXML(ele);
                if (group != null)
                    _groups.Add(group);
            }
        }

        //如果xml文件不存在，则创建；
        private XElement CreatBookMarkXmlFile(string path)
        {
            _doc = new XDocument();
            XElement root = new XElement("BookMarkGroups");
            _doc.Add(root);
            return root;
        }

        private BookMarkGroup CreatGroupFromXML(XElement ele)
        {
            if (ele == null)
                return null;
            IEnumerable<XElement> eles = ele.Elements();
            if (eles == null || eles.Count() == 0)
                return null;
            string name = null;
            CoordEnvelope envelop = null;
            Dictionary<string, CoordEnvelope> bookMarks = new Dictionary<string, CoordEnvelope>();
            foreach (XElement element in eles)
            {
                if (element == null)
                    continue;
                name = element.Attribute(XName.Get("name")).Value;
                envelop = ParseEnvelope(element);
                bookMarks.Add(name, envelop);
                if (!haveSameName(name))
                    _bookMarks.Add(name, envelop);
            }
            string groupName = ele.Attribute(XName.Get("name")).Value;
            _groupNames.Add(groupName);
            if (bookMarks.Count() == 0 || string.IsNullOrEmpty(groupName))
                return null;
            return new BookMarkGroup(groupName, bookMarks);
        }

        private bool haveSameName(string name)
        {
            if (_bookMarks.Count == 0)
                return false;
            foreach (string key in _bookMarks.Keys)
            {
                if (key == name)
                    return true;
            }
            return false;
        }

        private CoordEnvelope ParseEnvelope(XElement element)
        {
            if (element == null)
                return null;
            XElement ele = element.Element(XName.Get("Envelope"));
            if (ele == null)
                return null;
            double minX = double.Parse(ele.Attribute(XName.Get("minX")).Value);
            double maxX = double.Parse(ele.Attribute(XName.Get("maxX")).Value);
            double minY = double.Parse(ele.Attribute(XName.Get("minY")).Value);
            double maxY = double.Parse(ele.Attribute(XName.Get("maxY")).Value);
            return new CoordEnvelope(minX, maxX, minY, maxY);
        }

        public void AddBookMarkElement(string groupName, string bookMarkName, CoordEnvelope envelope)
        {
            if (_groupNames.Contains(groupName))
                AddElementToCurrentGroup(groupName, bookMarkName, envelope);
            else
                AddGroupElement(groupName, bookMarkName, envelope);
            _doc.Save(_path);
        }

        private void AddElementToCurrentGroup(string groupName, string bookMarkName, CoordEnvelope envelope)
        {
            IEnumerable<XElement> groupEles = _root.Elements(XName.Get("BookMarkGroup"));
            if (groupEles == null || groupEles.Count() == 0)
                return;
            foreach (XElement groupEle in groupEles)
            {
                if (groupEle.Attribute(XName.Get("name")).Value == groupName)
                    AddChildElement(groupEle, bookMarkName, envelope);
            }
            RefreshGroups(groupEles);
        }

        private void AddChildElement(XElement groupEle, string bookMarkName, CoordEnvelope envelope)
        {
            XElement sameEle = GetSameNameElement(groupEle, bookMarkName);
            if (sameEle != null)
            {
                DialogResult result = MessageBox.Show("已经包含名为" + bookMarkName + "的关注区域，是否要用此关注区域替换它？", "提示", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.No)
                    return;
                else
                    sameEle.Remove();
            }
            XElement newEle = CreatBookMarkNode(bookMarkName, envelope);
            groupEle.Add(newEle);
        }

        /// <summary>
        /// 判断组中是否已存在具有相同名字的书签
        /// </summary>
        /// <param name="groupEle"></param>
        /// <param name="bookMarkName"></param>
        /// <returns></returns>
        private XElement GetSameNameElement(XElement groupEle, string bookMarkName)
        {
            IEnumerable<XElement> eles = groupEle.Elements();
            if (eles == null || eles.Count() == 0)
                return null;
            foreach (XElement ele in eles)
            {
                if (ele.Attribute(XName.Get("name")).Value == bookMarkName)
                    return ele;
            }
            return null;
        }

        /// <summary>
        /// 新增书签的节点
        /// </summary>
        /// <returns></returns>
        private XElement CreatBookMarkNode(string bookMarkName, CoordEnvelope evp)
        {
            XElement ele = new XElement("BookMark", new XAttribute("name", bookMarkName));
            XAttribute minX = new XAttribute("minX", evp.MinX);
            XAttribute maxX = new XAttribute("maxX", evp.MaxX);
            XAttribute minY = new XAttribute("minY", evp.MinY);
            XAttribute maxY = new XAttribute("maxY", evp.MaxY);
            XElement sub = new XElement("Envelope", minX, maxX, minY, maxY);
            ele.Add(sub);
            return ele;
        }

        public void AddGroupElement(string groupName, string bookMarkName, CoordEnvelope envelope)
        {
            XElement eleType = new XElement("BookMarkGroup", new XAttribute("name", groupName));
            XElement sub = CreatBookMarkNode(bookMarkName, envelope);
            eleType.Add(sub);
            _root.Add(eleType);
            _doc.Save(_path);
            IEnumerable<XElement> groupEles = _root.Elements(XName.Get("BookMarkGroup"));
            RefreshGroups(groupEles);
        }

        public void DeleteBookMarkElement(string groupName, string bookMarkName)
        {
            XElement groupEle = null;
            IEnumerable<XElement> groupEles = _root.Elements(XName.Get("BookMarkGroup"));
            if (groupEles == null || groupEles.Count() == 0)
                return;
            foreach (XElement group in groupEles)
            {
                if (group.Attribute(XName.Get("name")).Value == groupName)
                    groupEle = group;
            }
            DeleteBookMarkInGroupElement(bookMarkName, groupEle);
            _doc.Save(_path);
            RefreshGroups(groupEles);
        }

        private void DeleteBookMarkInGroupElement(string bookMarkName, XElement groupEle)
        {
            if (groupEle == null)
                return;
            IEnumerable<XElement> markEles = groupEle.Elements(XName.Get("BookMark"));
            if (markEles == null || markEles.Count() == 0)
                return;
            XElement removeEle = null;
            foreach (XElement ele in markEles)
            {
                if (ele.Attribute(XName.Get("name")).Value == bookMarkName)
                    removeEle.Remove();
            }
        }

        public void DeleteBookMarkElement(string bookMarkName)
        {
            IEnumerable<XElement> groupEles = _root.Elements(XName.Get("BookMarkGroup"));
            if (groupEles == null || groupEles.Count() == 0)
                return;
            foreach (XElement group in groupEles)
            {
                IEnumerable<XElement> marks = group.Elements(XName.Get("BookMark"));
                if (marks == null || marks.Count() == 0)
                    continue;
                foreach (XElement bm in marks)
                {
                    if (bm.Attribute(XName.Get("name")).Value == bookMarkName)
                    {
                        bm.Remove();
                        _doc.Save(_path);
                        RefreshGroups(groupEles);
                        return;
                    }
                }
            }
        }

        public void DeleteAllBookMarkElements()
        {
            IEnumerable<XElement> groupEles = _root.Elements(XName.Get("BookMarkGroup"));
            foreach (XElement group in groupEles)
            {
                // DeleteBookMarksInGroup(group);
                group.RemoveNodes();
            }
            _doc.Save(_path);
            RefreshGroups(groupEles);
        }

        private void DeleteBookMarksInGroup(XElement group)
        {
            IEnumerable<XElement> marks = group.Elements(XName.Get("BookMark"));
            if (marks == null || marks.Count() == 0)
                return;
            foreach (XElement mark in marks)
                mark.Remove();
        }
    }
}
