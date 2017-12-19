using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public interface IPropertyConverter
    {
        XElement ToXml(string name,object propertyValue);
        object FromXml(XElement propertyXml);
    }
}
