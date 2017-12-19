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
    [Export(typeof(IElement)), Category("图标")]
    public class PictureElementGJWXQXZX : PictureElement
    {
        public PictureElementGJWXQXZX()
            : base()
        {
            _name = "国家卫星气象中心图标";
            _bitmap = ImageGetter.GetImageByName("国家卫星气象中心.png") as Bitmap;
            _icon = ImageGetter.GetImageByName("ElementGJWXQXZX1.png");
            _bitmapOSize = new Size(_bitmap.Width, _bitmap.Height); 
        }
    }
}
