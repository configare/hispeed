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
    public class TextElementFlogRegion : MultlineTextElement   
    {
        public TextElementFlogRegion()
            : base()
        {
            _text = "雾区";
            _font = new System.Drawing.Font("微软雅黑", 13f);
            _name = "雾区标注文本";
            _displayMaskColor = true;
            _fontBrush = new SolidBrush(Color.Blue);
            _fontColor = Color.Blue;
        }
    }
}
