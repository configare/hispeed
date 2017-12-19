using System;
using System.Collections.Generic;

using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents the states of <see cref="RadService"/>
    /// </summary>
    public enum RadServiceState : short
    {
        /// <summary>
        /// The state of <see cref="RadService"/>, when is created.
        /// </summary>
        Initial = 0,

        /// <summary>
        /// The state of <see cref="RadService"/>, when is stopped.
        /// </summary>
        Stopped = 1,

        /// <summary>
        /// The state of <see cref="RadService"/>, when is working.
        /// </summary>
        Working = 2,

        /// <summary>
        /// The state of <see cref="RadService"/>, when is paused.
        /// </summary>
        Paused = 3
    }
}
