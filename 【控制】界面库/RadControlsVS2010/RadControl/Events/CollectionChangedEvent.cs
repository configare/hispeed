using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents the method that will handle a CollectionChanged event.
    /// </summary>
    /// <param name="sender">the %sender:System.Collections.CollectionBase% of the event</param>
    /// <param name="args">the %event arguments:Telerik.WinControls.UI.CollectionChangedEventArgs" </param>
    public delegate void CollectionChangedHandler(CollectionBase sender, CollectionChangedEventArgs args);

    /// <summary>
    /// Represents event data for the CollectionChanged event.
    /// </summary>
    public class CollectionChangedEventArgs
    {
        private object target;
        private ItemsChangeOperation operation;
        private int index;
        /// <summary>
        /// Gets or sets a value specifing the target.
        /// </summary>
        public object Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the index in the collection of the changed item.
        /// </summary>
        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the items chnage operation.
        /// </summary>
        public ItemsChangeOperation Operation
        {
            get
            {
                return this.operation;
            }
            set
            {
                this.operation = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the CollectionChangedEventArgs class using the 
        /// target, the index of the item, and the item's change operation. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="index"></param>
        /// <param name="operation"></param>
        public CollectionChangedEventArgs(object target, int index, ItemsChangeOperation operation)
        {
            this.target = target;
            this.index = index;
            this.operation = operation;
        }
    }
}
