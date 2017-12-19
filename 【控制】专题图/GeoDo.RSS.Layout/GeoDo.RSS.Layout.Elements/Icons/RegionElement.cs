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
    //[Export(typeof(IElement)), Category("标注"), ExportMetadata("VERSION", "1")]
    public class RegionElement : PictureElement
    {
        public RegionElement()
            : base()
        {
            _name = "地区界";
            _bitmap = ImageGetter.GetImageByName("地区界.bmp") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon地区界.bmp");
            _size = _bitmap.Size;
        }
    }
}
