using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.Core
{
    public class ColorPropertyConverter:PropertyConverter
    {
        protected override void SetAttributes(XElement ele, object propertyValue)
        {
            Color color = (Color)propertyValue;
            ele.SetAttributeValue("a", color.A);
            ele.SetAttributeValue("r", color.R);
            ele.SetAttributeValue("g", color.G);
            ele.SetAttributeValue("b", color.B);
        }

        protected override object CreateAndFillObject(XElement propertyXml)
        {
            byte a = AttToByte(propertyXml, "a");
            byte r = AttToByte(propertyXml, "r");
            byte g = AttToByte(propertyXml, "g");
            byte b = AttToByte(propertyXml, "b");
            return Color.FromArgb(a, r, g, b);
        }
    }
}
