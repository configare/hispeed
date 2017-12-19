using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class CatalogNodeGroupDef:CatalogNodeItemDef
    {
        public string Pattern;
        public List<CatalogNodeItemDef> NodeDefs = new List<CatalogNodeItemDef>();

        public CatalogNodeGroupDef()
            : base()
        { 
        }

        public CatalogNodeGroupDef(string text, string format, string identify,string pattern)
            : base(text, format, identify)
        {
            Pattern = pattern;
        }
    }
}
