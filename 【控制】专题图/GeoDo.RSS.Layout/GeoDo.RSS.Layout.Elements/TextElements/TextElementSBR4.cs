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
    public class TextElementSBR4 : TextElement
    {
        public TextElementSBR4()
            : base()
        {
            _text = "XXXX";
            _font = new Font("微软雅黑", 14f);
            _name = "省界标注文本";
            _fontColor = Color.Red;
            _maskBrush = new System.Drawing.SolidBrush(Color.White);
        }
    }
}
