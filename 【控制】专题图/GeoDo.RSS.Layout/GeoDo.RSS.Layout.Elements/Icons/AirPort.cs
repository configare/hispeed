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
    [Export(typeof(IElement)), Category("标注")]
    public class AirPort : PictureElement
    {
        public AirPort()
            : base()
        {
            _name = "机场";
            _bitmap = ImageGetter.GetImageByName("机场.png") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon机场.png");
            _size = _bitmap.Size;
        }
    }
}
