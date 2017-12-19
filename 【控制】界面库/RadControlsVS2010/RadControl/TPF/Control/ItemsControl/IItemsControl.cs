using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <summary>
    /// An interface which provides methods for handling a collection of RadItems.
    /// This interface is used throughout controls which represent a list of items.
    /// </summary>
    public interface IItemsControl
    {
        /// <summary>
        /// Fires when an item has been selected.
        /// </summary>
        event ItemSelectedEventHandler ItemSelected;

        /// <summary>
        /// Fires when an item has been deselected.
        /// </summary>
        event ItemSelectedEventHandler ItemDeselected;

        /// <summary>
        /// Returns the selected item in the control.
        /// </summary>
        /// <returns>An reference to a RadItem instance which represents
        /// the currently selected item.</returns>
        RadItem GetSelectedItem();

        /// <summary>
        /// Selects an item in the control.
        /// </summary>
        /// <param name="item">A reference to a RadItem instance which 
        /// represents the item which is to be selected.</param>
        void SelectItem(RadItem item);

        /// <summary>
        /// Gets an item from the collection that is next to a certain item.
        /// </summary>
        /// <param name="item">The item which neighbour to return.</param>
        /// <param name="forward">The direction in which to look for the neighbour.</param>
        /// <returns>A reference to a RadItem instance which represents the neighbour item.</returns>
        RadItem GetNextItem(RadItem item, bool forward);

        /// <summary>
        /// Selects an item from the collection that is next to a certain item.
        /// </summary>
        /// <param name="item">The item which neighbour to return.</param>
        /// <param name="forward">The direction in which to look for the neighbour.</param>
        /// <returns>A reference to a RadItem instance which represents the neighbour item.</returns>
        RadItem SelectNextItem(RadItem item, bool forward);

        
        /// <summary>
        /// Gets the first visible item from the collection.
        /// In a IItemsControl that is the first item that is visible on the control.
        /// </summary>
        /// <returns>A reference to a RadItem instance that represents
        /// the first visible control.</returns>
        RadItem GetFirstVisibleItem();

        /// <summary>
        /// Gets the last visible item from the collection.
        /// In a IItemsControl that is the last item that is visible on the control.
        /// </summary>
        /// <returns>A reference to a RadItem instance that represents
        /// the last visible control.</returns>
        RadItem GetLastVisibleItem();
        /// <summary>
        /// Selects the first visible item on the IItemsControl.
        /// </summary>
        /// <returns>A reference to a RadItem instance that represents the item selected.</returns>
        RadItem SelectFirstVisibleItem();

        /// <summary>
        /// Selects the last visible item on the IItemsControl.
        /// </summary>
        /// <returns>A reference to a RadItem instance that represents the item selected.</returns>
        RadItem SelectLastVisibleItem();
        
        /// <summary>
        /// Gets a collection containing the items
        /// that are currently active.
        /// </summary>
        RadItemOwnerCollection ActiveItems
        {
            get;
        }

        /// <summary>
        /// Gets the collection of items associated
        /// with the IItemsControl.
        /// </summary>
        RadItemOwnerCollection Items
        {
            get;
        }

        /// <summary>
        /// Gets or sets a boolean value that determines whether
        /// the rollover items functionality will be allowed.
        /// </summary>
        bool RollOverItemSelection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a boolean value that determines whether
        /// keyboard input will be processed by the IItemsControl.
        /// </summary>
        bool ProcessKeyboard
        {
            get;
            set;
        }

        /// <summary>
        /// Defines whether the IItemsControl can execute navigation
        /// operation based on the keydata provided.
        /// </summary>
        /// <param name="keyData">An instance of the <see cref="System.Windows.Forms.Keys"/>
        /// struct that defines the key command issued.</param>
        /// <returns>True if navigation possible, otherwise false.</returns>
        bool CanNavigate(Keys keyData);

        /// <summary>
        /// Defines whether the IItemsControl has an item that
        /// corresponds to the mnemonic passed in the parameter.
        /// </summary>
        /// <param name="keyData">A character that defines the mnemonic command issued.</param>
        /// <returns>True if mnemonic can be processed, otherwise false.</returns>
        bool CanProcessMnemonic(char keyData);
    }
}
