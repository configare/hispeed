using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class CatalogAttriteDef
    {
        public string Text;
        public string Identify;
        public string Format;
        public bool Visible;

        public CatalogAttriteDef()
        { 
        }

        public CatalogAttriteDef(string text, string identify, string format, bool visible)
        {
            this.Text = text;
            this.Identify = identify;
            this.Format = format;
            this.Visible = visible;
        }
    }
}
