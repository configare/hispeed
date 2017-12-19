using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public class ItemsPropertyConverter : PropertyConverter
    {
        protected override void SetAttributes(XElement ele, object propertyValue)
        {
            ContourClass[] items = (ContourClass[])propertyValue;
            if (items == null || items.Length == 0)
                return;
            foreach (ContourClass it in items)
            {
                XElement subEle = new XElement("ContourClass");
                subEle.SetAttributeValue("contourvalue", it.ContourValue.ToString());
                subEle.SetAttributeValue("isdisplay", it.IsDisplay.ToString());
                subEle.SetAttributeValue("linewidth", it.LineWidth.ToString());
                //
                ColorPropertyConverter c = new ColorPropertyConverter();
                XElement cElement = c.ToXml("ContourColor", it.ContourColor);
                if (cElement != null)
                    subEle.Add(cElement);
                //
                ele.Add(subEle);
            }
        }

        protected override object CreateAndFillObject(XElement propertyXml)
        {
            List<ContourClass> ccs = new List<ContourClass>();
            var itemElements = propertyXml.Elements("ContourClass");
            foreach (XElement itemElement in itemElements)
            {
                double contourValue = AttToDouble(itemElement, "contourvalue");
                bool isdisplay = AttToBool(itemElement, "isdisplay");
                float linewidth = AttToFloat(itemElement, "linewidth");
                Color contourColor = Color.Black;
                XElement cElement = itemElement.Element("ContourColor");
                if (cElement != null)
                {
                    ColorPropertyConverter c = new ColorPropertyConverter();
                    contourColor = (Color)c.FromXml(cElement);
                }
                ContourClass cc = new ContourClass(contourValue, contourColor, linewidth);
                ccs.Add(cc);
            }
            return ccs.Count > 0 ? ccs.ToArray() : null;
        }
    }
}
