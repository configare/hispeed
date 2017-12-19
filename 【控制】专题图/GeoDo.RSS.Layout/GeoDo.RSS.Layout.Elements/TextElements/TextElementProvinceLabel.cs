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
    public class TextElementProvinceLabel : MultlineTextElement
    {
        public TextElementProvinceLabel()
            : base()
        {
            _name = "XX省";
            _text = "XX省";
            _font = new System.Drawing.Font("微软雅黑", 13f);
            _displayMaskColor = true;
            _maskBrush = new SolidBrush(Color.Black);
            _fontBrush = new SolidBrush(Color.Yellow);
            _fontColor = Color.Yellow;
        }
    }
}
