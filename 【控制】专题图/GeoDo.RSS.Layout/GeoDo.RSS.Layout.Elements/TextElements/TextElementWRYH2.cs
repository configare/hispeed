using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Layout.GDIPlus;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("文本")]
    public class TextElementWRYH2 : TextElement
    {
        public TextElementWRYH2()
            : base()
        {
            _text = "XXXX";
            _font = new System.Drawing.Font("微软雅黑", 21.75f);
            _name = "二号字体文本";
        }
    }
}
