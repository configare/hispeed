using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines the time of the animation occurrence. 
    /// </summary>
    [Flags]
    public enum AnimatorStyles
    {
        /// <summary>
        /// Indicates that no animation is played.
        /// </summary>
        DoNotAnimate = 0,
        /// <summary>
        /// Indicates that animation is played on applying a setting.
        /// </summary>
        AnimateWhenApply = 1,
        /// <summary>
        /// Indicates that animation is played on up-apply a setting.
        /// </summary>
        AnimateWhenUnapply = 2,
        /// <summary>
        /// Indicates that animation is always played.
        /// </summary>
        AnimateAlways = AnimateWhenApply | AnimateWhenUnapply
    }

    /// <summary>
    /// Defines the possible types of animation looping.
    /// </summary>
    public enum LoopType
    {
        /// <summary>
        /// No animation looping is enabled.
        /// </summary>
        None,
        /// <summary>
        /// The animation is started from the beginning after it ends.
        /// </summary>
        Standard,
        /// <summary>
        /// The animation is started again, whereby
        /// end and start values are swapped.
        /// </summary>
        Reversed
    }
}
