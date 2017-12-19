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
    public class WaterLandBorder : PictureElement
    {
        public WaterLandBorder()
            : base()
        {
            _name = "水陆边界线";
            _bitmap = ImageGetter.GetImageByName("海岸线.bmp") as Bitmap;
            _icon = ImageGetter.GetImageByName("Icon海岸线.png");
            _size = _bitmap.Size;
        }
    }
}
