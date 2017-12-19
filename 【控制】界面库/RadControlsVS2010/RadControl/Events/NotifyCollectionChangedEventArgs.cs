using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.Data
{
    /// <summary>
    /// Provides data for the CollectionChanged event. 
    /// </summary>
    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        // Fields
        protected NotifyCollectionChangedAction action;
        protected CollectionResetReason resetReason = CollectionResetReason.Refresh;
        protected IList newItems;
        protected IList oldItems;

        protected int newStartingIndex;
        protected int oldStartingIndex;
        protected string propertyName = string.Empty;

        /// <summary>
        /// Gets the name of the changed property when the Action is ItemChanged.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a Reset change. 
        /// </summary>
        /// <param name="action">The action that caused the event. This must be set to Reset.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            this.action = action;

            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a multi-item change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItems"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
            : this(action)
        {

            this.newItems = (changedItems == null) ? null : ArrayList.ReadOnly(changedItems);
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a one-item change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItem"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
            : this(action, new object[] { changedItem })
        {

        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a multi-item Replace change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItems"></param>
        /// <param name="oldItems"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
            : this(action, newItems)
        {
            this.oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a multi-item change or a reset change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItems"></param>
        /// <param name="startingIndex"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
            : this(action, changedItems)
        {
            this.newStartingIndex = startingIndex;
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a one-item change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItem"></param>
        /// <param name="index"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
            : this(action, changedItem)
        {
            this.newStartingIndex = index;
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a one-item Replace change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItem"></param>
        /// <param name="oldItem"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
            : this(action, new object[] { newItem }, new object[] { oldItem })
        {

        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a multi-item Replace change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItems"></param>
        /// <param name="oldItems"></param>
        /// <param name="startingIndex"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
            : this(action, newItems, oldItems)
        {
            this.newStartingIndex = startingIndex;
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a multi-item Move change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItems"></param>
        /// <param name="index"></param>
        /// <param name="oldIndex"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
            : this(action, changedItems)
        {
            this.newStartingIndex = index;
            this.oldStartingIndex = oldIndex;
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a multi-item Move change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItem"></param>
        /// <param name="index"></param>
        /// <param name="oldIndex"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
            : this(action, new object[] { changedItem }, new object[] { changedItem })
        {
            this.newStartingIndex = index;
            this.oldStartingIndex = oldIndex;

        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangedEventArgs class that describes a one-item Replace change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItem"></param>
        /// <param name="oldItem"></param>
        /// <param name="index"></param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
            : this(action, new object[] { newItem }, new object[] { oldItem })
        {
            this.newStartingIndex = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="index">The index.</param>
        /// <param name="propertyName">Name of the property.</param>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index, string propertyName)
            : this(action, newItem, oldItem, index)
        {
            this.propertyName = propertyName;
        }



        /// <summary>
        /// Provides data for the CollectionChanged event. 
        /// </summary>
        public NotifyCollectionChangedAction Action
        {
            get
            {
                return this.action;
            }
        }

        /// <summary>
        /// Gets the reason for a Reset notification.
        /// </summary>
        public CollectionResetReason ResetReason
        {
            get
            {
                return this.resetReason;
            }
            internal set
            {
                this.resetReason = value;
            }
        }

        /// <summary>
        /// Gets the list of new items involved in the change.
        /// </summary>
        public IList NewItems
        {
            get
            {
                return this.newItems;
            }
        }

        /// <summary>
        /// Gets the index at which the change occurred.
        /// </summary>
        public int NewStartingIndex
        {
            get
            {
                return this.newStartingIndex;
            }
        }

        /// <summary>
        /// Gets the list of items affected by a Replace, Remove, or Move action.
        /// </summary>
        public IList OldItems
        {
            get
            {
                return this.oldItems;
            }
        }

        /// <summary>
        /// Gets the index at which a Move, Remove, ore Replace action occurred.
        /// </summary>
        public int OldStartingIndex
        {
            get
            {
                return this.oldStartingIndex;
            }
        }
    }

}
