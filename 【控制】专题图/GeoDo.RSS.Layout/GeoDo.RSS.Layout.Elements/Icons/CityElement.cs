using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Layout.GDIPlus;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Drawing;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("标注"), ExportMetadata("VERSION", "1")]
    public class CityElement:PictureElement
    {
        public CityElement()
            : base()
        {
            _name = "城镇";
            _bitmap = ImageGetter.GetImageByName("Icon城镇.bmp") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon城镇.bmp");
            _size = _bitmap.Size;
        }
    }
}
