using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines the alignment of items within a strip item layout.
    /// </summary>
    public enum StripViewItemAlignment
    {
        /// <summary>
        /// Items are aligned starting from the near edge. This is Left for Left-to-right layout and Right for Right-to-left layout.
        /// </summary>
        Near,
        /// <summary>
        /// Items are centered within the layout.
        /// </summary>
        Center,
        /// <summary>
        /// Items are aligned starting from the far edge. This is Right for Left-to-right layout and Left for Right-to-left layout.
        /// </summary>
        Far,
    }
}
