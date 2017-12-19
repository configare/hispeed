using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("文本"), ExportMetadata("VERSION", "1")]
    public class TextElementGJWXQXZX : TextElement
    {
        public TextElementGJWXQXZX()
            : base()
        {
            _name = "国家卫星气象中心文本";            
            _text =  "国家卫星气象中心";
        }
    }
}
