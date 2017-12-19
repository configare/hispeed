using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles
{
    /// <summary>
    /// Defines the possible states that a StyleManager may enter.
    /// </summary>
    public enum StyleManagerState
    {
        Initial,
        Detaching,
        Detached,
        Attaching,
        Attached
    }
}
