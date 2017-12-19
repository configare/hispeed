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
    public class ContryBorderLine : PictureElement
    {
        public ContryBorderLine()
            : base()
        {
            _name = "国境线";
            _bitmap = ImageGetter.GetImageByName("国境线.bmp") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon国境线.bmp");
            _size = _bitmap.Size;
        }
    }
}
