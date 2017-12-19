using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This enumerator defines the possible selection modes for items
    /// in a <see cref="RadPageViewStackElement"/>.
    /// </summary>
    public enum StackViewItemSelectionMode
    {
        /// <summary>
        /// The selected item is highlighted and its content is displayed in the content area.
        /// </summary>
        Standard,
        /// <summary>
        /// The selected item is highlighted and its content is displayed before it according to the stack orientation.
        /// </summary>
        ContentWithSelected,
        /// <summary>
        /// The selected item is highlighted and its content is displayed after it according to the
        /// stack orientation.
        /// </summary>
        ContentAfterSelected
    }
}
