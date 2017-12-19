using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Use the StyleBuilderReadOnly attribute to mark properties that should appear as readonly when edited in the Visual 
    /// Style Builder application
    /// </summary>
    public class StyleBuilderReadOnlyAttribute: Attribute
    {
        private bool readOnly = true;

        public StyleBuilderReadOnlyAttribute()
        {
        }

        public StyleBuilderReadOnlyAttribute(bool readOnly)
        {
            this.readOnly = readOnly;
        }

        public bool IsReadOnly
        {
            get { return readOnly; }            
        }
    }
}
