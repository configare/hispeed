using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class CatalogNodeDef:CatalogNodeItemDef
    {
        public CatalogNodeDef()
            :base()
        { 
        }

        public CatalogNodeDef(string text, string format, string identify)
            : base(text, format, identify)
        { 
        }
    }
}
