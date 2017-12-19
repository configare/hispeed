using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Layout.GDIPlus;
using System.ComponentModel;
using System.Drawing;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("图标")]
    public class PictureElementSouthChinaSea557 : PictureElement
    {
        public PictureElementSouthChinaSea557()
            : base()
        {
            _name = "南海诸岛557_720";
            _icon = ImageGetter.GetImageByName("南海诸岛16_16.png");
            _bitmap = ImageGetter.GetImageByName("南海诸岛557_720.bmp") as Bitmap;
            _size = _bitmap.Size;
        }
    }
}
