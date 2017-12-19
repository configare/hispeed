using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class CatalogDef
    {
        public string ClassString;
        public string Text;
        public string Identify;

        public CatalogDef()
        { 
        }

        public CatalogDef(string classString, string text, string identify)
        {
            ClassString = classString;
            Text = text;
            Identify = identify;
        }

        public override string ToString()
        {
            return Text ?? string.Empty;
        }
    }
}
