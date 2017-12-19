using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using GeoDo.RSS.Layout.GDIPlus;
using System.Drawing;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("文本"), ExportMetadata("VERSION", "1")]
    public class TextElementQBG5 : TextElement
    {
        public TextElementQBG5()
            : base()
        {
            _name = "区界标注文本";
            _text = "XXXX";
            _font = new Font("微软雅黑", 11f);
            _fontColor = Color.DarkGray;
            _maskBrush = new System.Drawing.SolidBrush(Color.White);
        }
    }
}
