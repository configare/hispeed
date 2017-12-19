using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Elements;

namespace Telerik.WinControls
{
    /// <summary>
    /// Exposes methods and properties for e hierarchical items such as 
    /// RadMenuItem. 
    /// </summary>
    public interface IHierarchicalItem : IItemsOwner
    {
        /// <summary>
        /// Gets or sets the item's owner.
        /// </summary>
        object Owner { get;set; }
        /// <summary>
        /// Gets a value indicating whether the item has children.
        /// </summary>
        bool HasChildren { get;}
        /// <summary>
        /// Gets a value indicating whether the item is the root element if the
        /// hierarchy.
        /// </summary>
        bool IsRootItem { get;}
        //RadItemCollection Items { get;}
        /// <summary>
        /// Gets or sets the item's parent. 
        /// </summary>
        IHierarchicalItem ParentItem { get;set; }
        /// <summary>
        /// Gets the root item of this item's hierarchy.
        /// </summary>
        IHierarchicalItem RootItem { get; }
        /// <summary>
        /// Gets the next item.
        /// </summary>
        RadItem Next { get;}
        /// <summary>
        /// Gets the previous item.
        /// </summary>
        RadItem Previous { get;}
    }
}
