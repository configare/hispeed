using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines the animation state.
    /// </summary>
    public enum AnimationState
    {
        /// <summary>
        /// Indicates that the animation is not running.
        /// </summary>
        NotRunning = 0,
        /// <summary>
        /// Indicates that the animation is running.
        /// </summary>
        Applying,
        /// <summary>
        /// Indicates that the animation is running backwards.
        /// </summary>
        Reversing
    }
}
