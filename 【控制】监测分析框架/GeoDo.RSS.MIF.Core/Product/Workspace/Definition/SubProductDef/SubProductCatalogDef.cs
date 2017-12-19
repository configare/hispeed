using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class SubProductCatalogDef : CatalogDef
    {
        public List<CatalogAttriteDef> AttributeDefs = new List<CatalogAttriteDef>();
        public string Filter;
        public string Pattern;
        public string Folder;

        public SubProductCatalogDef()
            : base()
        {
        }

        public SubProductCatalogDef(string classString, string text, string identify, string filter, string pattern, string folder)
            : base(classString, text, identify)
        {
            Filter = filter;
            Pattern = pattern;
            Folder = folder;
        }

        public CatalogAttriteDef GetCatalogAttrDefByIdentify(string identify)
        {
            if (AttributeDefs == null || AttributeDefs.Count == 0)
                return null;
            foreach (CatalogAttriteDef item in AttributeDefs)
            {
                if (item.Identify.ToLower() == identify.ToLower())
                    return item;
            }
            return null;
        }
    }
}
