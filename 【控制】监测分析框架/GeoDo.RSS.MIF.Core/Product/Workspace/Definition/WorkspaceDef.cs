using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class WorkspaceDef
    {
        public string Text;
        public string Identify;
        public bool IsEnabled;
        public List<CatalogDef> CatalogDefs = new List<CatalogDef>();

        public WorkspaceDef()
        { 
        }

        public WorkspaceDef(string text,string identify,bool isEnabled)
        {
            Text = text;
            Identify = identify;
            IsEnabled = isEnabled;
        }

        public WorkspaceDef(List<CatalogDef> catalogDef)
        {
            CatalogDefs = catalogDef;
        }

        public CatalogDef GetCatalogByClass(string classString)
        {
            if (CatalogDefs == null || CatalogDefs.Count == 0)
                return null;
            foreach(CatalogDef df in CatalogDefs)
                if(!(string.IsNullOrEmpty(classString) && classString.ToUpper() == df.ClassString.ToUpper()))
                    return df;
            return null ;
        }
    }
}
