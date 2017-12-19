using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public interface ITraversable
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <value></value>
        object this[int index] { get; }
    }
}
