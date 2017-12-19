using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Themes
{
    public class VsbBrowsableAttribute : Attribute
    {
        private bool browsable;

        public VsbBrowsableAttribute()
            : this(true)
        {
        }

        public VsbBrowsableAttribute(bool browsable)
        {
            this.browsable = browsable;
        }

        public bool Browsable
        {
            get
            {
                return this.browsable;
            }
        }
    }
}
