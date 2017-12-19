using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("文本")]
    public class TextElementWRYHS5 : TextElement
    {
        public TextElementWRYHS5()
            : base()
        {
            _text = "XXXX";
            _font = new System.Drawing.Font("微软雅黑", 9f);
            _name = "小五号字体文本";
        }
    }
}
