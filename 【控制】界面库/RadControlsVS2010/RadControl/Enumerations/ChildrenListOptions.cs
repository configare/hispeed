using System;
namespace Telerik.WinControls
{ 
    /// <summary>
    /// Defines the options used by RadElement.GetChildren(options) method.
    /// </summary>
    [Flags]
    public enum ChildrenListOptions
    {
        /// <summary>
        /// Indicates that all children are returned.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Indicates that children are returned sorted according to their z-index.
        /// </summary>
        ZOrdered = 1,
        /// <summary>
        /// Indicates that children are returned in reverse order.
        /// </summary>
        ReverseOrder = 2,
        /// <summary>
        /// Indicates that collapsed children are included.
        /// </summary>
        IncludeCollapsed = 4,
        /// <summary>
        /// Indicates that only children, which visibility is ElementVisibility.Visible, are included.
        /// </summary>
        IncludeOnlyVisible = 8
    }
}
