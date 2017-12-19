using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Layout.GDIPlus;
using System.Drawing;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("图标")]
    public class PictureElementSouthChinaSea134 : PictureElement
    {
        public PictureElementSouthChinaSea134()
            : base()
        {
            _name = "南海诸岛134_174";
            _icon = ImageGetter.GetImageByName("南海诸岛16_16.png");
            _bitmap = ImageGetter.GetImageByName("南海诸岛134_174.png") as Bitmap;
            _size = _bitmap.Size;
        }
    }
}
