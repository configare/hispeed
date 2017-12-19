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
    //[Export(typeof(IElement)), Category("标注")]
   public class ProvinceBorderElement:PictureElement
    {
        public ProvinceBorderElement()
            : base()
        {
            _name = "省界";
            _bitmap = ImageGetter.GetImageByName("省界.bmp") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon省界.bmp") as Bitmap;
            _size = _bitmap.Size;
        }
    }
}
