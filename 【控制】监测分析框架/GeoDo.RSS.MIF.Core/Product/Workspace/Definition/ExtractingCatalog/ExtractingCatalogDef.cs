using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ExtractingCatalogDef:CatalogDef
    {
        public List<CatalogNodeItemDef> NodeDefs;

        public ExtractingCatalogDef()
            :base()
        { 
        }

        public ExtractingCatalogDef(string classString, string text, string identify)
            : base(classString, text, identify)
        { 
        }
    }
}
