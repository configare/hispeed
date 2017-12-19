using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Telerik.WinControls.Data;
using Telerik.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Diagnostics;
using System.Collections;

namespace Telerik.WinControls.UI
{
    [Editor(DesignerConsts.ListViewItemCollectionDesignerString, typeof(UITypeEditor))]
    [Serializable()]
    [DebuggerDisplay("Count = {Count}")]
    public class ListViewDataItemCollection : IList<ListViewDataItem>, IList, INotifyCollectionChanged
    {
        #region Fields
 
        private RadListViewElement owner;
 
        #endregion

        #region Constructors
 
        public ListViewDataItemCollection(RadListViewElement owner)
        {
            this.owner = owner;
            this.owner.ListSource.CollectionView.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionView_CollectionChanged);
        }
 
        #endregion

        #region Event handlers
 
        void CollectionView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ListViewDataItem newItem in e.NewItems)
                {
                    newItem.Owner = this.owner;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ListViewDataItem oldItem in e.OldItems)
                {
                    oldItem.Owner = null;
                    this.owner.SelectedItems.ProcessSelectedItem(oldItem);
                }
            }

            this.owner.ViewElement.Scroller.UpdateScrollRange();
            this.owner.ViewElement.InvalidateMeasure(true);
            this.OnCollectionChanged(e);
        }
 
        #endregion

        #region Extended API

        public void BeginUpdate()
        {
            this.owner.ListSource.BeginUpdate();
        }

        public void EndUpdate()
        {
            this.EndUpdate(true);
        }

        private void EndUpdate(bool notifyUpdates)
        {
            this.owner.ListSource.EndUpdate(notifyUpdates);
        }

        public void Add(string text)
        {
            ListViewDataItem listItem = this.owner.NewItem() as ListViewDataItem;
            if (listItem == null)
            {
                return;
            }

            listItem.Text = text;
            this.owner.ListSource.Add(listItem);
        }

        public void Add(object value)
        {
            ListViewDataItem listItem = this.owner.NewItem() as ListViewDataItem;
            if (listItem == null)
            {
                return;
            }

            listItem.Value = value;
            this.owner.ListSource.Add(listItem);
        }

        public void Add(params string[] values)
        {
            ListViewDataItem listItem = this.owner.NewItem() as ListViewDataItem;
            if (listItem == null)
            {
                return;
            }

            int paramsCount = values.GetLength(0);
            for (int i = 0; i < paramsCount; i++)
            {
                listItem[i] = values[i];
            }

            this.owner.ListSource.Add(listItem);
        }

        public void Add(params object[] values)
        {
            ListViewDataItem listItem = this.owner.NewItem() as ListViewDataItem;
            if (listItem == null)
            {
                return;
            }

            int paramsCount = values.GetLength(0);
            for (int i = 0; i < paramsCount; i++)
            {
                listItem[i] = values[i];
            }

            this.owner.ListSource.Add(listItem);
        }

        public virtual void AddRange(params ListViewDataItem[] listViewDataItems)
        {
            this.BeginUpdate();

            for (int i = 0; i < listViewDataItems.Length; i++)
            {
                this.Add(listViewDataItems[i]);
            }

            this.EndUpdate();
        }

        #endregion

        #region IList members
 
        public int IndexOf(ListViewDataItem item)
        {
            return this.owner.ListSource.CollectionView.IndexOf(item);
        }

        public void Insert(int index, ListViewDataItem item)
        {
            try
            {
                this.owner.ListSource.Insert(index, item);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Items cannot be programmatically added to the RadListView's Items collection when the control is data-bound.", ex);
            }
        }

        public ListViewDataItem this[int index]
        {
            get
            {
                return this.owner.ListSource.CollectionView[index];
            }
            set
            {
                this.owner.ListSource[this.owner.ListSource.IndexOf(this[index])] = value;
            }
        }

        public void Add(ListViewDataItem item)
        {
            item.Owner = this.owner;
            this.owner.ListSource.Add(item);
        }

        public void Clear()
        {
            this.owner.ListSource.Clear();
        }

        public bool Contains(ListViewDataItem item)
        {
            return this.owner.ListSource.CollectionView.Contains(item);
        }

        public void CopyTo(ListViewDataItem[] array, int arrayIndex)
        {
            this.owner.ListSource.CollectionView.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return this.owner.ListSource.CollectionView.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.owner.ListSource.IsReadOnly;
            }
        }

        public bool Remove(ListViewDataItem item)
        {
            ListViewTraverser traverser = this.owner.ViewElement.Scroller.Traverser.GetEnumerator() as ListViewTraverser;
            traverser.Position = item;
            if (traverser.MovePrevious() && traverser.Current != null && traverser.Current != item)
            {
                this.owner.SelectedItem = traverser.Current;
            }
            else
            {
                traverser.Position = item;
                if (traverser.MoveNext() && traverser.Current != item)
                {
                    this.owner.SelectedItem = traverser.Current;
                }
                else
                {
                    this.owner.SelectedItem = null;
                }
            }
            
            bool result = this.owner.ListSource.Remove(item);
            item.Owner = null;
            this.owner.SelectedItems.ProcessSelectedItem(item);
            return result;
        }

        public void RemoveAt(int index)
        {

            this.Remove(this[index]);
        }

        public IEnumerator<ListViewDataItem> GetEnumerator()
        {
            return this.owner.ListSource.CollectionView.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.owner.ListSource.CollectionView.GetEnumerator();
        }
 
        #endregion

        #region INotifyCollectionChanged members
 
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }
 
        #endregion

        int IList.Add(object value)
        {
            ListViewDataItem item = value as ListViewDataItem;
            if (item != null)
            {
                this.Add(item);
                return this.Count - 1;
            }

            return -1;
        }

        public bool Contains(object value)
        {
            ListViewDataItem item = value as ListViewDataItem;
            if (item != null)
            {
                return this.Contains(item);
            }

            return false;
        }

        public int IndexOf(object value)
        {
            ListViewDataItem item = value as ListViewDataItem;
            if (item != null)
            {
                return this.IndexOf(item);
            }

            return -1;
        }

        public void Insert(int index, object value)
        {
            ListViewDataItem item = value as ListViewDataItem;
            if (item != null)
            {
                this.Insert(index, item);
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public void Remove(object value)
        {
            ListViewDataItem item = value as ListViewDataItem;
            if (item != null)
            {
                this.Remove(item);
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value as ListViewDataItem;
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)this.owner.ListSource).CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return ((ICollection)this.owner.ListSource).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((ICollection)this.owner.ListSource).SyncRoot; }
        }
    }
}
