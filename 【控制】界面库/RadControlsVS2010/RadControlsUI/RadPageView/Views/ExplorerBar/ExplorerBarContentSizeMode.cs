using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This enumerator defines the possible size modes for the content areas in a <see cref="RadPageViewExplorerBarElement"/>.
    /// The size modes define how the content areas are calculated according to their content or the size of the <see cref="RadPageView"/> 
    /// control.
    /// </summary>
    public enum ExplorerBarContentSizeMode
    {
        /// <summary>
        /// The length of the content area is fixed and is defined by the PageLength value for each <see cref="RadPageViewPage"/>.
        /// </summary>
        FixedLength,
        /// <summary>
        /// The length of the content area is automatically calculated to fit the length of the content.
        /// </summary>
        AutoSizeToBestFit,
        /// <summary>
        /// The length of all visible content areas is equal. This usually implies that no scrollbars are shown.
        /// </summary>
        EqualLength
    }
}
