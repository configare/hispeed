using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Core
{
    public interface IGridLayer
    {
        XElement ToXml();
    }
}
