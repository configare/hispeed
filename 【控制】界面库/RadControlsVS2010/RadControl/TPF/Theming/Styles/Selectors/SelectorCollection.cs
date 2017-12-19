using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Collections;

namespace Telerik.WinControls
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
    public class SelectorCollection : List<IElementSelector>
    {
        /// <summary>
        /// Initializes a new instance of the SelectorCollection class.
        /// </summary>
        public SelectorCollection() : base(1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SelectorCollection class.
        /// </summary>
        public SelectorCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
