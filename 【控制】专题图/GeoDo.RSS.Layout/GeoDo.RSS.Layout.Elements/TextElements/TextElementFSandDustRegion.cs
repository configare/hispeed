using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Layout.GDIPlus;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Drawing;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("文本")]
    public class TextElementFSandDustRegion : MultlineTextElement
    {
        public TextElementFSandDustRegion()
            : base()
        {
            _text = "沙 尘 区";
            _font = new System.Drawing.Font("微软雅黑", 13f, FontStyle.Italic);
            _name = "沙尘区标注文本";
            _displayMaskColor = true;
            _fontBrush = new SolidBrush(Color.White);
            _fontColor = Color.White;
            _maskBrush = new SolidBrush(Color.Black);
        }
    }
}
