using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines the possible states of a RadElement.
    /// </summary>
    [Obsolete("Use ElementState instead.")]
    public enum RadElementState
    {
        /// <summary>
        /// Indicates that the RadElement is created.
        /// </summary>
        Created = 0,
        /// <summary>
        /// Indicates that the RadElement is initialized.
        /// </summary>
        Initialized
    }

    /// <summary>
    /// Defines the life cycle of a RadElement instance.
    /// </summary>
    public enum ElementState
    {
        /// <summary>
        /// The element is in its initial state.
        /// </summary>
        Initial,
        /// <summary>
        /// The element is in a process of being constructed.
        /// </summary>
        Constructing,
        /// <summary>
        /// The element is already constructed but not loaded yet.
        /// </summary>
        Constructed,
        /// <summary>
        /// The element is loading. That is it is initializing on the owning control.
        /// </summary>
        Loading,
        /// <summary>
        /// The element is prepared for visualizing.
        /// </summary>
        Loaded,
        /// <summary>
        /// Special state, indicating that the element has been loaded once and removed from the element tree.
        /// </summary>
        Unloaded,
        /// <summary>
        /// The element is in a process of being disposed of.
        /// </summary>
        Disposing,
        /// <summary>
        /// The element is already disposed of.
        /// </summary>
        Disposed,
    }
}
