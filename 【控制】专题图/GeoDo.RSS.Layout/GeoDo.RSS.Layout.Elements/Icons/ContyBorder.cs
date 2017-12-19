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
    //[Export(typeof(IElement)), Category("标注"), ExportMetadata("VERSION", "1")]
    public class ContyBorder : PictureElement
    {
        public ContyBorder()
            : base()
        {
            _name = "县界";
            _bitmap = ImageGetter.GetImageByName("县界.bmp") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon县界.png");
            _size = _bitmap.Size;
        }
    }
}
