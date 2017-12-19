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
    public class TextElementWRYH5 : TextElement
    {
         public TextElementWRYH5()
             : base()
         {
             _text = "XXXX";
             _font = new System.Drawing.Font("微软雅黑", 10.5f);
             _name = "五号字体文本";
         }
    }
}
