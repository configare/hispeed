using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace GeoDo.RSS.Layout.Elements
{
    public class LegendItem : IDisposable
    {
        private string _text = null;
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

        [DisplayName("图例项文本")]
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        [DisplayName("图例项颜色")]
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            _text = null;
        }

        #endregion
    }
}
