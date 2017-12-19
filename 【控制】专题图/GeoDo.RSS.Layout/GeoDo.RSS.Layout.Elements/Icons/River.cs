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
    public class River : PictureElement
    {
        public River()
            : base()
        {
            _name = "河流湖泊";
            _bitmap = ImageGetter.GetImageByName("河流湖泊.png") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon河流湖泊.png");
            _size = _bitmap.Size;
        }
    }
}
