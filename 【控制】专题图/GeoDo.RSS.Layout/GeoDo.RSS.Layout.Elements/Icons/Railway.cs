using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Layout.GDIPlus;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("标注")]
    public class Railway : PictureElement
    {
        public Railway()
            : base()
        {
            _name = "铁路";
            _bitmap = ImageGetter.GetImageByName("铁路.png") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon铁路.png");
            _size = _bitmap.Size;
        }
    }
}
