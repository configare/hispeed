using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Core.DrawEngine
{
    /// <summary>
    /// 序列化、反序列时负责属性转换
    /// </summary>
    public interface IPropertyConverter
    {
        XElement ToXml(string name,object propertyValue);
        object FromXml(XElement propertyXml);
    }
}
