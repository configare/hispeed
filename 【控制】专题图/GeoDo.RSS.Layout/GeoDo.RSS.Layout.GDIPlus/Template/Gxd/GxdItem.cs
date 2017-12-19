using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class GxdItem:IGxdItem
    {
        public GxdItem()
        { 
        }

        public virtual XElement ToXml()
        {
            return null;
        }
    }
}
