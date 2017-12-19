using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout
{
    public interface IPersitable
    {
        void InitByXml(XElement xml);
    }
}
