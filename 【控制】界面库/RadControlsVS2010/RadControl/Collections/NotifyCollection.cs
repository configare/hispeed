using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.WinControls.Data;
using System.Collections.Generic;

namespace Telerik.Collections.Generic
{
    [Serializable]
    public class NotifyCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Fields

        private int update = 0;
        private int version;
        public static NotifyCollection<T> Empty = new NotifyCollection<T>(new List<T>());

        #endregion

        #region Constructors

        public NotifyCollection()
        {

        }

        public NotifyCollection(IList<T> list)
            :base(list)
        {

        }

        #endregion

        #region Public

        /// <summary>
        /// Moves the specified old index.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        public void Move(int oldIndex, int newIndex)
        {
            this.MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(params  T[] items)
        {
            this.BeginUpdate();

            for (int i = 0; i < items.Length; i++)
            {
                this.Add(items[i]);
            }

            this.EndUpdate();
        }

        /// <summary>
        /// Begins the update.
        /// </summary>
        public void BeginUpdate()
        {
            this.update++;
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        public void EndUpdate()
        {
            this.EndUpdate(true);
        }

        internal virtual void EndUpdate(bool notify)
        {
            update--;

            if (update <= 0)
            {
                this.update = 0;

                if (notify)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        #endregion

        #region Overrides

        protected internal int Version
        {
            get
            {
                return this.version;
            }
        }

        protected bool IsInUpdate
        {
            get { return this.update > 0; }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = base[index];
            base.SetItem(index, item);
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
        }

        protected override void RemoveItem(int index)
        {
            T item = base[index];
            base.RemoveItem(index);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            T item = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null && update == 0)
            {
                this.CollectionChanged(this, args);
                this.version++;
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, oldItem, item, index));
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
