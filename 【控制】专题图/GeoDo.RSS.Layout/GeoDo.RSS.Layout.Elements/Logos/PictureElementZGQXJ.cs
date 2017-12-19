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
    [Export(typeof(IElement)), Category("图标")]
    public class PictureElementZGQXJ :PictureElement
    {
        public PictureElementZGQXJ()
            : base()
        {
            _name = "中国气象局图标";
            _bitmap = ImageGetter.GetImageByName("中国气象局.png") as Bitmap;
            _icon = ImageGetter.GetImageByName("ElementZGQXJ1.png");
            _bitmapOSize = new Size(_bitmap.Width, _bitmap.Height); 
        }
    }
}
