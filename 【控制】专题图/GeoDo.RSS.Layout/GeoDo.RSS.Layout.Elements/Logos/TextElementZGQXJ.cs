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
    public class TextElementZGQXJ : TextElement
    {
        public TextElementZGQXJ()
            : base()
        {
            _name = "中国气象局文本";
            _text = "中国气象局";
        }
    }
}
