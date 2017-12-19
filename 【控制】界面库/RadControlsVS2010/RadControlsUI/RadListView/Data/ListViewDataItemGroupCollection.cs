using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Telerik.WinControls.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Specialized;

namespace Telerik.WinControls.UI
{
    [Editor(DesignerConsts.ListViewGroupCollectionDesignerString, typeof(UITypeEditor))]
    [Serializable()]
    [DebuggerDisplay("Count = {Count}")]
    public class ListViewDataItemGroupCollection : IList<ListViewDataItemGroup>, IList, Telerik.WinControls.Data.INotifyCollectionChanged
    {
        #region Fields
        protected ObservableCollection<ListViewDataItemGroup> autoGroups, customGroups;
        protected RadListViewElement owner;
        private int updateCount = 0;

        internal ObservableCollection<ListViewDataItemGroup> AutoGroups
        {
            get
            {
                return this.autoGroups;
            }
        }

        private void EnsureAutoGroups()
        {
            if (updateCount > 0)
            {
                return;
            }

            this.BeginUpdate();

            //This will trigger the GroupBuilder.Perform() method if needed
            GroupCollection<ListViewDataItem> groups = owner.ListSource.CollectionView.Groups;

            this.EndUpdate();
        }

        #endregion

        #region Constructors

        public ListViewDataItemGroupCollection(RadListViewElement owner)
        {
            this.owner = owner;
            autoGroups = new ObservableCollection<ListViewDataItemGroup>();
            autoGroups.CollectionChanged += new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(groups_CollectionChanged);

            customGroups = new ObservableCollection<ListViewDataItemGroup>();
            customGroups.CollectionChanged += new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(groups_CollectionChanged);
        }

        #endregion

        #region Extended API

        public virtual void BeginUpdate()
        {
            this.updateCount++;
        }

        public virtual void EndUpdate()
        {
            if (this.updateCount > 0)
            {
                this.updateCount--;
            }
        }

        private bool IsCustomGrouping
        {
            get
            {
                return this.owner.EnableCustomGrouping || this.owner.IsDesignMode;
            }
        }

        public virtual void AddRange(params ListViewDataItemGroup[] listViewDataItemGroups)
        {
            this.BeginUpdate();

            for (int i = 0; i < listViewDataItemGroups.Length; i++)
            {
                this.Add(listViewDataItemGroups[i]);
            }

            this.EndUpdate();
        }

        #endregion

        #region INotifyCollectionChanged

        public event Telerik.WinControls.Data.NotifyCollectionChangedEventHandler CollectionChanged;

        void groups_CollectionChanged(object sender, Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null &&
                (e.Action == Telerik.WinControls.Data.NotifyCollectionChangedAction.Add ||
                e.Action == Telerik.WinControls.Data.NotifyCollectionChangedAction.Replace))
            {
                foreach (ListViewDataItemGroup item in e.NewItems)
                {
                    item.Owner = this.owner;
                }
            }

            if (e.OldItems != null &&
                (e.Action == Telerik.WinControls.Data.NotifyCollectionChangedAction.Remove ||
                e.Action == Telerik.WinControls.Data.NotifyCollectionChangedAction.Replace))
            {
                foreach (ListViewDataItemGroup item in e.OldItems)
                {
                    item.Owner = null;
                }
            }

            if (e.Action == Telerik.WinControls.Data.NotifyCollectionChangedAction.Batch ||
                e.Action == Telerik.WinControls.Data.NotifyCollectionChangedAction.Reset)
            {
                foreach (ListViewDataItemGroup item in this)
                {
                    item.Owner = this.owner;
                }
            }

            if (sender == this.customGroups && this.owner.EnableCustomGrouping)
            {
                this.OnNotifyCollectionChanged(e);
            }
            else if (sender == this.autoGroups && !this.owner.EnableCustomGrouping)
            {
                this.OnNotifyCollectionChanged(e);
            }
        }

        private void OnNotifyCollectionChanged(Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            if (this.updateCount == 0 && this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        #endregion

        #region IList<ListViewDataItemGroup> Members

        public int IndexOf(ListViewDataItemGroup item)
        {
            if (this.IsCustomGrouping)
            {
                return customGroups.IndexOf(item);
            }
            else
            {
                EnsureAutoGroups();
                return autoGroups.IndexOf(item);
            }
        }

        public void Insert(int index, ListViewDataItemGroup item)
        {
            this.customGroups.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.customGroups.RemoveAt(index);
        }

        public ListViewDataItemGroup this[int index]
        {
            get
            {
                if (this.IsCustomGrouping)
                {
                    return this.customGroups[index];
                }
                else
                {
                    EnsureAutoGroups();
                    return this.autoGroups[index];
                }
            }
            set
            {
                this.customGroups[index] = value;
            }
        }

        public void Add(ListViewDataItemGroup item)
        {
            this.customGroups.Add(item);
        }

        public void Clear()
        {
            this.customGroups.Clear();
        }

        public bool Contains(ListViewDataItemGroup item)
        {
            if (this.IsCustomGrouping)
            {
                return customGroups.Contains(item);
            }
            else
            {
                EnsureAutoGroups();
                return autoGroups.Contains(item);
            }
        }

        public void CopyTo(ListViewDataItemGroup[] array, int arrayIndex)
        {
            if (this.IsCustomGrouping)
            {
                this.customGroups.CopyTo(array, arrayIndex);
            }
            else
            {
                EnsureAutoGroups();
                this.autoGroups.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                if (this.IsCustomGrouping)
                {
                    return customGroups.Count;
                }
                else
                {
                    EnsureAutoGroups();
                    return autoGroups.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return !this.IsCustomGrouping;
            }
        }

        public bool Remove(ListViewDataItemGroup item)
        {
            return this.customGroups.Remove(item);
        }

        public IEnumerator<ListViewDataItemGroup> GetEnumerator()
        {
            if (this.IsCustomGrouping)
            {
                return customGroups.GetEnumerator();
            }
            else
            {
                EnsureAutoGroups();
                return autoGroups.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.IsCustomGrouping)
            {
                return customGroups.GetEnumerator();
            }
            else
            {
                EnsureAutoGroups();
                return autoGroups.GetEnumerator();
            }
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            this.Add(value as ListViewDataItemGroup);
            return this.Count - 1;
        }

        public bool Contains(object value)
        {
            return this.Contains(value as ListViewDataItemGroup);
        }

        public int IndexOf(object value)
        {
            return this.IndexOf(value as ListViewDataItemGroup);
        }

        public void Insert(int index, object value)
        {
            this.Insert(index, value as ListViewDataItemGroup);
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
            this.Remove(value as ListViewDataItemGroup);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value as ListViewDataItemGroup;
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (this.IsCustomGrouping)
            {
                ((ICollection)this.customGroups).CopyTo(array, index);
            }
            else
            {
                EnsureAutoGroups();
                ((ICollection)this.autoGroups).CopyTo(array, index);
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this.IsCustomGrouping ?
                       ((ICollection)this.customGroups).IsSynchronized :
                       ((ICollection)this.autoGroups).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.IsCustomGrouping ?
                       ((ICollection)this.customGroups).SyncRoot :
                       ((ICollection)this.autoGroups).SyncRoot;
            }
        }
        #endregion
    }
}