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
   public class TextElementXBYS5 : TextElement
    {
         public TextElementXBYS5()
             : base()
         {
             _text = "XX县";
             _font = new System.Drawing.Font("微软雅黑", 9f);
             _name = "县界标注文本";
             _fontColor = System.Drawing.Color.Yellow;
             _maskBrush = new System.Drawing.SolidBrush( System.Drawing.Color.White);
         }
    }
}
