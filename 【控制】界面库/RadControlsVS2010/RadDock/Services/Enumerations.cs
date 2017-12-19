using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the possible states for a RadDockService instance.
    /// </summary>
    public enum ServiceState
    {
        /// <summary>
        /// The service is in its initial state.
        /// </summary>
        Initial,
        /// <summary>
        /// The service has finished some operation(s) and is currently not working.
        /// </summary>
        Stopped,
        /// <summary>
        /// The service is working.
        /// </summary>
        Working,
        /// <summary>
        /// The service has been started and paused while performing some operation.
        /// </summary>
        Paused,
    }
}
