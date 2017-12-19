using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines how the editor will be positioned relatively to the edited container
    /// </summary>
    [Flags]
    public enum EditorVisualMode
    {
        /// <summary>
        /// Editor is positioned inline, inside of the editor container, and logically resides in container's children
        /// hierarchy. Usually it is recommended to use this option for compact-sized editors, 
        /// like textboxes, comboboxes, mask editors, checkboxes, etc.
        /// </summary>
        Inline = 0,
        /// <summary>
        /// Editor is positioned inside a popup control which is positioned vertically under the edited 
        /// container. Usually it is recommended to use this 
        /// option for medium to large-sized editors, like calendars, custom controls and panels,
        /// radiobutton lists, checkbox groups, etc.
        /// </summary>
        Dropdown = 1,
        /// <summary>
        /// Usually this means that the editor is positioned explicitly by the edited containers's logic. 
        /// Also it is used as a default initialization value.
        /// </summary>
        Default = 2
    }
}
