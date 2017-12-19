using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.WinControls.Interfaces;

namespace Telerik.WinControls.Data
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObservableCollection<T> : Collection<T>, IList, INotifyCollectionChanged, INotifyCollectionChanging, INotifyPropertyChanged, INotifyPropertyChangingEx
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
		private int update = 0;
		private int itemUpdate = 0;

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class. 
        /// </summary>
        public ObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class that contains elements copied from the specified list. 
        /// </summary>
        /// <param name="list"></param>
		public ObservableCollection(IList<T> list) : base((list != null) ? new List<T>(list.Count) : new List<T>())
        {
            IList<T> items = base.Items;
            if ((list != null) && (items != null))
            {
                using (IEnumerator<T> enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        items.Add(enumerator.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Occurs before an item is added, removed, changed, moved, or the entire list is refreshed.
		/// </summary>
		public event NotifyCollectionChangingEventHandler CollectionChanging;

        /// <summary>
        /// Overridden. Removes all items from the collection. 
        /// </summary>
        protected override void ClearItems()
        {
			bool cancel = this.OnCollectionReseting();
		
            if (cancel)
			{
				return;
			}

            T[] oldItems = new T[this.Count];
            this.Items.CopyTo(oldItems, 0);
            base.ClearItems();

            this.OnNotifyPropertyChanged("Count");
            this.OnNotifyPropertyChanged("Item[]");
            this.OnCollectionReset(oldItems);
        }

        /// <summary>
        /// Overridden. Inserts an item into the collection at the specified index. 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
			this.InsertItem(index, item, null);
        }

		protected virtual void InsertItem(int index, T item, Action<T> approvedAction)
		{
		
            bool cancel = this.OnCollectionChanging(NotifyCollectionChangedAction.Add, item, index);
			
            if (cancel)
			{
				return;
			}
			
            if (approvedAction != null)
			{
				approvedAction(item);
			}
			
            base.InsertItem(index, item);
			this.OnNotifyPropertyChanged("Count");
			this.OnNotifyPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection. 
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public void Move(int oldIndex, int newIndex)
        {   
            this.MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection. 
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        protected virtual void MoveItem(int oldIndex, int newIndex)
		{
			T item = base[oldIndex];
			bool cancel = this.OnCollectionChanging(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
			
            if (cancel)
			{
				return;
			}

			base.RemoveItem(oldIndex);
			base.InsertItem(newIndex, item);
			this.OnNotifyPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
		}

		/// <summary>
		/// Suspends event notification.
		/// </summary>
		public virtual void BeginUpdate()
		{
			this.update++;
		}

		public virtual void BeginItemUpdate()
		{
			this.itemUpdate++;
		}

		public void EndItemUpdate()
		{
			this.EndItemUpdate(true);
		}

		/// <summary>
		/// Resumes event notification.
		/// </summary>
		public virtual void EndItemUpdate(bool notifyUpdates)
		{
			itemUpdate--;

			if (itemUpdate < 0)
			{
				itemUpdate = 0;
				return;
			}

			if (itemUpdate == 0 && notifyUpdates)
			{
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanged));
			}
		}

		/// <summary>
		/// Resumes event notification.
		/// </summary>
		public virtual void EndUpdate(bool notifyUpdates)
		{
			update--;

			if (update < 0)
			{
				update = 0;
				return;
			}

            if (update == 0 && notifyUpdates)
			{
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Batch));
			}
		}

        public void EndUpdate()
        {
            this.EndUpdate(true);
        }

        /// <summary>
        /// true to indicate the collection has completed update; otherwise false.
        /// </summary>
        public bool IsUpdated
        {
            get
			{
				return update == 0;
			}
        }

        /// <summary>
		/// Calls the NotifyListenersCollectionChanged method with the provided arguments if not in a batch update. 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.update == 0 && this.itemUpdate == 0)
            {
				this.NotifyListenersCollectionChanged(e);
            }
        }

		/// <summary>
		/// Raises the CollectionChanged event with the provided arguments. 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void NotifyListenersCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, e);
			}
		}

		protected internal void CallCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(sender, e);
			}
		}

		/// <summary>
		/// Calls the NotifyListenersCollectionChanging method with the provided arguments if not in a batch update. 
		/// </summary>
		/// <param name="e"></param>
		protected virtual bool OnCollectionChanging(NotifyCollectionChangingEventArgs e)
		{
			if (this.update == 0 && this.itemUpdate == 0)
			{
				this.NotifyListenersCollectionChanging(e);
				return e.Cancel;
			}
			return false;
		}

		/// <summary>
		/// Raises the CollectionChanging event with the provided arguments. 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void NotifyListenersCollectionChanging(NotifyCollectionChangingEventArgs e)
		{
			if (this.CollectionChanging != null)
			{
				this.CollectionChanging(this, e);
			}
		}

		protected internal void CallCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
		{
			if (this.CollectionChanging != null)
			{
				this.CollectionChanging(sender, e);
			}
		}

        /// <summary>
        /// Overridden. Removes the item at the specified index of the collection. 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            
            T item = base[index];
			bool cancel = this.OnCollectionChanging(NotifyCollectionChangedAction.Remove, item, index);
			
            if (cancel)
			{
				return;
			}

            base.RemoveItem(index);
            this.OnNotifyPropertyChanged("Count");
            this.OnNotifyPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        /// <summary>
        /// Overridden. Replaces the element at the specified index. 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, T item)
        {
            
            T oldItem = base[index];
			bool cancel = this.OnCollectionChanging(NotifyCollectionChangedAction.Replace, oldItem, item, index);
			
            if (cancel)
			{
				return;
			}

            base.SetItem(index, item);
            this.OnNotifyPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
		}

		#region INotifyCollectionChanged related methods

		protected void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        protected void OnCollectionReset(IList oldItems)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null, oldItems));
        }

		#endregion

		#region INotifyCollectionChanging related methods

		protected bool OnCollectionChanging(NotifyCollectionChangedAction action, object item, int index)
		{
			return this.OnCollectionChanging(new NotifyCollectionChangingEventArgs(action, item, index));
		}

		protected bool OnCollectionChanging(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
		{
			return this.OnCollectionChanging(new NotifyCollectionChangingEventArgs(action, item, index, oldIndex));
		}

        protected bool OnCollectionChanging(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
		{
			return this.OnCollectionChanging(new NotifyCollectionChangingEventArgs(action, newItem, oldItem, index));
		}

        protected bool OnCollectionReseting()
		{
			return this.OnCollectionChanging(new NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction.Reset));
		}

		#endregion

		#region INotifyPropertyChanged Members
		/// <summary>
        /// Occurs when a property of an object changes. 
        /// Calling the event is developer's responsibility.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        [Description("Occurs when when a property of an object changes change.")]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected virtual void OnNotifyPropertyChanged(string propertyName)
        {
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the NotifyPropertyChanged event
        /// </summary>
		/// <param name="e">A <see cref="PropertyChangedEventArgs"/> instance containing event data.</param>
        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            //Code extracted from all the properties and refactored
            //switch (e.PropertyName)
            //{
            //    default:
            //        break;
            //}

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }
        #endregion

		#region INotifyPropertyChangingEx Members

		/// <summary>
		/// Occurs before a property of an object changes. 		
		/// </summary>
		public event PropertyChangingEventHandlerEx PropertyChanging;

		/// <summary>
		/// Raises the PropertyChanging event
		/// </summary>
		/// <param name="propertyName">The name of the property</param>
		protected virtual void OnNotifyPropertyChanging(string propertyName)
		{
            this.OnNotifyPropertyChanging(new PropertyChangingEventArgsEx(propertyName));
		}

		/// <summary>
		/// Raises the NotifyPropertyChanging event
		/// </summary>
		/// <param name="e">A <see cref="PropertyChangingEventArgs"/> instance containing event data.</param>
		protected virtual void OnNotifyPropertyChanging(PropertyChangingEventArgsEx e)
		{
			if (this.PropertyChanging != null)
			{
				this.PropertyChanging(this, e);
			}
		}

		#endregion

        #region Explicit IList implementation for the design-time serialization

        #region IList Members

        int IList.Add(object value)
        {
            if (value is T)
            {
                this.Add((T)value);
                return this.IndexOf((T)value);
            }

            return -1;
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            if (value is T)
                return this.Contains((T)value);

            return false;
        }

        int IList.IndexOf(object value)
        {
            if (value is T)
                return this.IndexOf((T)value);

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (value is T)
                this.Insert(index, (T)value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            if (value is T)
                this.Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this.Items[index];
            }
            set
            {
                if (value is T)
                {
                    this.Items[index] = (T)value;
                }
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (array.Rank != 1)
                throw new ArgumentException("array", "Multidimentional arrays not supported!");

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("array", "Non-zero lower bound");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            if ((array.Length - index) < this.Count)
                throw new ArgumentOutOfRangeException("array", "Array too small");

            T[] localArray = array as T[];

            if (localArray != null)
            {
                this.Items.CopyTo(localArray, index);
            }
            else
            {
                Type elementType = array.GetType().GetElementType();
                Type c = typeof(T);

                if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
                    throw new ArgumentException("array", "Invalid array type");

                object[] objArray = array as object[];

                if (objArray == null)
                    throw new ArgumentException("array", "Invalid array type");

                try
                {
                    for (int i = 0; i < this.Items.Count; i++)
                        objArray[index++] = this.Items[i];
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("array", "Invalid array type");
                }
            }
        }

        int ICollection.Count
        {
            get { return this.Items.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #endregion		
	}
}