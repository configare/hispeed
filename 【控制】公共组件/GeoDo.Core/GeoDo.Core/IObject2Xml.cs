using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.Core
{
    public interface IObject2Xml
    {
        object FromXml(string fname);
        object FromXml(XElement element);
        void ToXmlFile(object obj, string fname);
        XElement ToXml(object obj);
        IPersistContextEnvironment PersistContextEnvironment { get; }
    }
}
