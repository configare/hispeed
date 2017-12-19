using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.Core
{
    public class PointFPropertyConverter : PropertyConverter
    {
        protected override void SetAttributes(XElement ele, object propertyValue)
        {
            PointF pt = (PointF)propertyValue;
            ele.SetAttributeValue("x", pt.X);
            ele.SetAttributeValue("y", pt.Y);
        }

        protected override object CreateAndFillObject(XElement propertyXml)
        {
            float x = AttToFloat(propertyXml, "x");
            float y = AttToFloat(propertyXml, "y");
            return new PointF(x, y);
        }
    }
}
