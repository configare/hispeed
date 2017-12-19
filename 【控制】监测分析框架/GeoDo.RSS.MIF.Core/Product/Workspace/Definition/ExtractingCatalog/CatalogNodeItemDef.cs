using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class CatalogNodeItemDef
    {
        public string Text;
        public string Format;
        public string Identify;

        public CatalogNodeItemDef()
        { 
        }

        public CatalogNodeItemDef(string text, string format, string identify)
        {
            Text = text;
            Format = format;
            Identify = identify;
        }

        public override string ToString()
        {
            return Text ?? string.Empty;
        }
    }
}
