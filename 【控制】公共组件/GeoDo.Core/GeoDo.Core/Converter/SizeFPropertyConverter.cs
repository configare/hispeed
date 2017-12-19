using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.Core
{
    public class SizeFPropertyConverter : PropertyConverter
    {
        protected override void SetAttributes(XElement ele, object propertyValue)
        {
            SizeF size = (SizeF)propertyValue;
            ele.SetAttributeValue("width", size.Width);
            ele.SetAttributeValue("height", size.Height);
        }

        protected override object CreateAndFillObject(XElement propertyXml)
        {
            float width = AttToFloat(propertyXml, "width");
            float height = AttToFloat(propertyXml, "height");
            return new SizeF(width, height);
        }
    }
}
