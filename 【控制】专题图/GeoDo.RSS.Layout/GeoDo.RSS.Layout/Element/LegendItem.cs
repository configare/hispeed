using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout
{
    [Serializable]
    public class LegendItem : IDisposable
    {
        private string _text = "图例X";
        private Color _color = Color.Empty;
        
        public LegendItem()
        {
            _text = string.Empty;
        }

        public LegendItem(string text)
        {
            _text = text;
        }

        public LegendItem(string text, Color color)
            : this(text)
        {
            _color = color;
        }

        [DisplayName("图例项文本"), Category("外观")]
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        [DisplayName("图例项颜色"), Category("外观")]
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// 暂未使用
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static XElement ToXml(LegendItem item)
        {
            if (item == null)
                return null;
            else
                return new XElement("LegendItem", new XAttribute("text", item.Text), new XAttribute("color", item.Color.ToArgb()));
        }

        /// <summary>
        /// 暂未使用
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static LegendItem ToLegendItem(XElement xml)
        {
            if (xml == null)
                return null;
            string text = xml.Attribute("text").Value;
            int color = int.Parse(xml.Attribute("color").Value);
            LegendItem item = new LegendItem(xml.Attribute("text").Value, Color.FromArgb(color));
            return item;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            _text = null;
        }

        #endregion
    }
}
