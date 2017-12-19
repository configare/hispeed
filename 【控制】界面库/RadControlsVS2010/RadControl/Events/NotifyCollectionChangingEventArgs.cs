using System;
using System.Collections;
using System.ComponentModel;
using Telerik.WinControls.Interfaces;

namespace Telerik.WinControls.Data
{
    /// <summary>
    /// Provides data for the CollectionChanging event. 
    /// </summary>
    public class NotifyCollectionChangingEventArgs : CancelEventArgs
    {
        // Fields
        protected NotifyCollectionChangedAction action;
        protected IList newItems;
        protected int newStartingIndex;
        protected IList oldItems;
        protected int oldStartingIndex;
        protected PropertyChangingEventArgsEx propertyArgs;

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a Reset change. 
        /// </summary>
        /// <param name="action">The action that caused the event. This must be set to Reset.</param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            this.InitializeAdd(action, null, -1);
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a multi-item change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItems"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, IList changedItems)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            if (action == NotifyCollectionChangedAction.Reset)
            {
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                this.InitializeAddOrRemove(action, changedItems, -1);
            }
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a one-item change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItem"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, object changedItem)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            if (action == NotifyCollectionChangedAction.Reset)
            {
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                this.InitializeAddOrRemove(action, new object[] { changedItem }, -1);
            }
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a multi-item Replace change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItems"></param>
        /// <param name="oldItems"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            this.InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a multi-item change or a reset change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItems"></param>
        /// <param name="startingIndex"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            if (action == NotifyCollectionChangedAction.Reset)
            {
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }

                this.InitializeAddOrRemove(action, changedItems, startingIndex);
            }
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a one-item change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItem"></param>
        /// <param name="index"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            if (action == NotifyCollectionChangedAction.Reset)
            {
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                this.InitializeAddOrRemove(action, new object[] { changedItem }, index);
            }
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a one-item Replace change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItem"></param>
        /// <param name="oldItem"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, -1, -1);
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a multi-item Replace change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItems"></param>
        /// <param name="oldItems"></param>
        /// <param name="startingIndex"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            this.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a multi-item Move change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItems"></param>
        /// <param name="index"></param>
        /// <param name="oldIndex"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            this.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a multi-item Move change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItem"></param>
        /// <param name="index"></param>
        /// <param name="oldIndex"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            object[] newItems = new object[] { changedItem };
            this.InitializeMoveOrReplace(action, newItems, newItems, index, oldIndex);
        }

        /// <summary>
        /// Initializes a new instance of the NotifyCollectionChangingEventArgs class that describes a one-item Replace change. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItem"></param>
        /// <param name="oldItem"></param>
        /// <param name="index"></param>
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
            : base()
        {
            this.newStartingIndex = -1;
            this.oldStartingIndex = -1;

            this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, index);
        }

        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index, PropertyChangingEventArgsEx propertyArgs)
            : this(action, newItem, oldItem, index)
        {
            this.propertyArgs = propertyArgs;
        }

        private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex)
        {
            this.action = action;
            this.newItems = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
            this.newStartingIndex = newStartingIndex;
        }

        private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            this.action = action;

            if (action == NotifyCollectionChangedAction.Add || action == NotifyCollectionChangedAction.Batch)
            {
                this.InitializeAdd(action, changedItems, startingIndex);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                this.InitializeRemove(action, changedItems, startingIndex);
            }
        }

        private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex)
        {
            this.InitializeAdd(action, newItems, startingIndex);
            this.InitializeRemove(action, oldItems, oldStartingIndex);
        }

        private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex)
        {
            this.action = action;
            this.oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
            this.oldStartingIndex = oldStartingIndex;
        }

        /// <summary>
        /// Provides data for the CollectionChanging event. 
        /// </summary>
        public NotifyCollectionChangedAction Action
        {
            get
            {
                return this.action;
            }
        }

        /// <summary>
        /// Gets the property arguments when property changing has been fired.
        /// </summary>
        /// <value>The property arguments.</value>
        public PropertyChangingEventArgsEx PropertyArgs
        {
            get
            {
                return this.propertyArgs;
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
