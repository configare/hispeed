using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public class FontPropertyConverter : PropertyConverter
    {
        protected override void SetAttributes(XElement ele, object propertyValue)
        {
            Font font = (Font)propertyValue;
            ele.SetAttributeValue("family",font.FontFamily.Name.ToString());
            ele.SetAttributeValue("size", font.Size.ToString());
        }

        protected override object CreateAndFillObject(XElement propertyXml)
        {
            string family = AttToString(propertyXml, "family");
            float sizef = AttToFloat(propertyXml, "size");
            return new Font(family ?? "宋体", sizef);
        }
    }
}
