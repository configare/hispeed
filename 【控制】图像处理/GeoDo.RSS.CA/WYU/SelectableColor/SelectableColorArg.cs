using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    public class SelectableColorArg : RgbProcessorArg
    {
        private enumSelectableColorApplyType _applyType = enumSelectableColorApplyType.Absolute;
        private Dictionary<enumSelectableColor, SelectableColorArgItem> _items = new Dictionary<enumSelectableColor, SelectableColorArgItem>();

        public SelectableColorArg()
        {
            Init();
        }

        public SelectableColorArg(enumSelectableColorApplyType applyType)
        {
            _applyType = applyType;
            Init();
        }

        private void Init()
        {
            _items.Add(enumSelectableColor.Red, new SelectableColorArgItem(enumSelectableColor.Red));
            _items.Add(enumSelectableColor.Yellow, new SelectableColorArgItem(enumSelectableColor.Yellow));
            _items.Add(enumSelectableColor.Green, new SelectableColorArgItem(enumSelectableColor.Green));
            _items.Add(enumSelectableColor.Cyan, new SelectableColorArgItem(enumSelectableColor.Cyan));
            _items.Add(enumSelectableColor.Blue, new SelectableColorArgItem(enumSelectableColor.Blue));
            _items.Add(enumSelectableColor.Magenta, new SelectableColorArgItem(enumSelectableColor.Magenta));
            _items.Add(enumSelectableColor.Black, new SelectableColorArgItem(enumSelectableColor.Black));
            _items.Add(enumSelectableColor.Neutrals, new SelectableColorArgItem(enumSelectableColor.Neutrals));
            _items.Add(enumSelectableColor.White, new SelectableColorArgItem(enumSelectableColor.White));
        }

        public IEnumerable<SelectableColorArgItem> Items
        {
            get 
            {
                return _items.Values;
            }
        }

        public enumSelectableColorApplyType ApplyType
        {
            get { return _applyType; }
            set { _applyType = value; }
        }

        public SelectableColorArgItem GetSelectableColorArgItem(enumSelectableColor color)
        {
            return _items[color];
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement xmlSon = xmldoc.CreateElement("ApplyType");
            XmlElement subSubElem;
            xmlSon.InnerText = _applyType.ToString();
            xmlElem.AppendChild(xmlSon);
            foreach (SelectableColorArgItem item in Items)
            {
                if (!item.IsEmpty())
                {
                    xmlSon = xmldoc.CreateElement(item.TargetColor.ToString());
                    xmlElem.AppendChild(xmlSon);
                    subSubElem = xmldoc.CreateElement("Cyan");
                    subSubElem.InnerText = item.CyanAdjustValue.ToString();
                    xmlSon.AppendChild(subSubElem);
                    subSubElem = xmldoc.CreateElement("Magenta");
                    subSubElem.InnerText = item.MagentaAdjustValue.ToString();
                    xmlSon.AppendChild(subSubElem);
                    subSubElem = xmldoc.CreateElement("Yellow");
                    subSubElem.InnerText = item.YellowAdjustValue.ToString();
                    xmlSon.AppendChild(subSubElem);
                    subSubElem = xmldoc.CreateElement("Black");
                    subSubElem.InnerText = item.BlackAdjustValue.ToString();
                    xmlSon.AppendChild(subSubElem);
                }
            }
            return xmlElem;
        }

        public override  RgbProcessorArg Clone()
        {
            SelectableColorArg arg = new SelectableColorArg(_applyType);
            foreach (enumSelectableColor key in _items.Keys)
            {
                arg._items[key] = _items[key].Clone();
            }
            return arg;
        }
    }
}
