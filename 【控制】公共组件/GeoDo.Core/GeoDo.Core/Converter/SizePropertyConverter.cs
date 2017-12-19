using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.Core
{
    public class SizePropertyConverter : PropertyConverter
    {
        protected override void SetAttributes(XElement ele, object propertyValue)
        {
            Size size = (Size)propertyValue;
            ele.SetAttributeValue("width", size.Width);
            ele.SetAttributeValue("height", size.Height);
        }

        protected override object CreateAndFillObject(XElement propertyXml)
        {
            int width = AttToInt(propertyXml, "width");
            int height = AttToInt(propertyXml, "height");
            return new Size(width, height);
        }
    }
}
