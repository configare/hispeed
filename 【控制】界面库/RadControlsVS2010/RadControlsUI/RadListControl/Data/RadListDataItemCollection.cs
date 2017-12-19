using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class RadListDataItemCollection: IList<RadListDataItem>, IList
    {
        #region Fields
        private ListDataLayer dataLayer;
        private RadListElement ownerListElement;

        #endregion

        #region Initialization

        public RadListDataItemCollection(ListDataLayer dataLayer, RadListElement ownerListElement)
        {
            this.dataLayer = dataLayer;
            this.ownerListElement = ownerListElement;
        }

        #endregion

        #region Properties

        public ListDataLayer Owner 
        {
            get 
            { 
                return this.dataLayer; 
            }
        }

        public RadListElement OwnerListElement
        {
            get
            {
                return this.ownerListElement;
            }
        }

        public RadListDataItem First
        {
            get
            {
                if (this.Count > 0)
                {
                    return this[0];
                }

                return null;
            }
        }

        public RadListDataItem Last
        {
            get
            {
                if (this.Count > 0)
                {
                    return this[this.Count - 1];
                }

                return null;
            }
        }

        #endregion

        #region IList<ListItem> Members

        public int IndexOf(RadListDataItem item)
        {
            return dataLayer.DataView.IndexOf(item);
        }

        public void Insert(int index, RadListDataItem item)
        {
            this.OwnerListElement.CheckReadyForUnboundMode();

            item.DataLayer = this.dataLayer;
            item.Owner = this.ownerListElement;

            Telerik.WinControls.Data.NotifyCollectionChangingEventArgs args = new Telerik.WinControls.Data.NotifyCollectionChangingEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Add, item);
            this.ownerListElement.OnItemsChanging(args);

            if (args.Cancel)
            {
                return;
            }
            Telerik.WinControls.Data.NotifyCollectionChangedEventArgs args1 = new Telerik.WinControls.Data.NotifyCollectionChangedEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Add, item);
            this.dataLayer.ListSource.Insert(index, item);

            this.ownerListElement.OnItemsChanged(args1);
        }

        public void RemoveAt(int index)
        {
            this.OwnerListElement.CheckReadyForUnboundMode();

            RadListDataItem itemToRemove = this.dataLayer.DataView[index];

            Telerik.WinControls.Data.NotifyCollectionChangingEventArgs args = new Telerik.WinControls.Data.NotifyCollectionChangingEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Remove, itemToRemove);
            this.ownerListElement.OnItemsChanging(args);
            if (args.Cancel)
            {
                return;
            }

            itemToRemove.DataLayer = null;
            itemToRemove.Owner = null;
            this.dataLayer.ListSource.RemoveAt(index);
            Telerik.WinControls.Data.NotifyCollectionChangedEventArgs args1 = new Telerik.WinControls.Data.NotifyCollectionChangedEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Remove, itemToRemove);
            this.ownerListElement.OnItemsChanged(args1);
        }

        public RadListDataItem this[int index]
        {
            get
            {
                return this.dataLayer.DataView[index];
            }
            set
            {
                this.OwnerListElement.CheckReadyForUnboundMode();

                RadListDataItem item = this.dataLayer.ListSource[index];

                Telerik.WinControls.Data.NotifyCollectionChangingEventArgs args = new Telerik.WinControls.Data.NotifyCollectionChangingEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Replace, value, item);
                this.ownerListElement.OnItemsChanging(args);
                if (args.Cancel)
                {
                    return;
                }

                item.DataLayer = null;
                item.Owner = null;

                if (item.Selected)
                {
                    ((IList)this.ownerListElement.SelectedItems).Remove(item);
                }

                this.dataLayer.ListSource[index] = value;

                value.DataLayer = this.dataLayer;
                value.Owner = this.ownerListElement;
                Telerik.WinControls.Data.NotifyCollectionChangedEventArgs args1 = new Telerik.WinControls.Data.NotifyCollectionChangedEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Replace, value, item);
                this.ownerListElement.OnItemsChanged(args1);
            }
        }

        #endregion

        public void AddRange(IEnumerable<RadListDataItem> range)
        {
            this.OwnerListElement.CheckReadyForUnboundMode();

            List<RadListDataItem> items = new List<RadListDataItem>(range);

            Telerik.WinControls.Data.NotifyCollectionChangingEventArgs args = new Telerik.WinControls.Data.NotifyCollectionChangingEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Batch, items);
            this.ownerListElement.OnItemsChanging(args);
            if (args.Cancel)
            {
                return;
            }

            this.ownerListElement.SuspendItemsChangeEvents = true;
            foreach (RadListDataItem item in range)
            {
                this.Add(item);
            }
            this.ownerListElement.SuspendItemsChangeEvents = false;
            Telerik.WinControls.Data.NotifyCollectionChangedEventArgs args1 = new Telerik.WinControls.Data.NotifyCollectionChangedEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Batch, items);
            this.ownerListElement.OnItemsChanged(args1);
        }

        public void AddRange(IEnumerable<string> textStrings)
        {
            List<RadListDataItem> result = new List<RadListDataItem>();
            foreach (string text in textStrings)
            {
                result.Add(new RadListDataItem(text));
            }

            this.AddRange(result);
        }

        public void Add(string itemText)
        {
            this.Add(new RadListDataItem(itemText));
        }

        #region ICollection<ListItem> Members

        public void Add(RadListDataItem item)
        {
            this.OwnerListElement.CheckReadyForUnboundMode();

            item.DataLayer = this.dataLayer;
            item.Owner = this.ownerListElement;

            Telerik.WinControls.Data.NotifyCollectionChangingEventArgs args = new Telerik.WinControls.Data.NotifyCollectionChangingEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Add, item);
            this.ownerListElement.OnItemsChanging(args);
            if (args.Cancel)
            {
                return;
            }

            if (this.dataLayer.ListSource.IsDataBound)
            {
                RadListDataItem newItem = this.dataLayer.ListSource.AddNew();
                newItem.Text = item.Text;
            }
            else
            {
                this.dataLayer.ListSource.Add(item);
            }

            Telerik.WinControls.Data.NotifyCollectionChangedEventArgs args1 = new Telerik.WinControls.Data.NotifyCollectionChangedEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Add, item);
            this.ownerListElement.OnItemsChanged(args1);
        }

        public void Clear()
        {
            this.OwnerListElement.CheckReadyForUnboundMode();

            foreach (RadListDataItem item in this.dataLayer.ListSource)
            {
                item.DataLayer = null;
                item.Owner = null;
            }

            this.ownerListElement.ActiveItem = null;

            this.dataLayer.ListSource.Clear();

            Telerik.WinControls.Data.NotifyCollectionChangedEventArgs args1 = new Telerik.WinControls.Data.NotifyCollectionChangedEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Reset);
            this.ownerListElement.OnItemsChanged(args1);
            this.ownerListElement.UpdateSelectedIndexOnItemsChanged();
        }

        public bool Contains(RadListDataItem item)
        {
            return this.Owner.DataView.Contains(item);
        }

        public void CopyTo(RadListDataItem[] array, int arrayIndex)
        {
        }

        public int Count
        {
            get 
            {
                return this.Owner.DataView.Count;
            }
        }

        public bool IsReadOnly
        {
            get 
            {
                return this.Owner.ListSource.IsReadOnly;
            }
        }

        public bool Remove(RadListDataItem item)
        {
            this.OwnerListElement.CheckReadyForUnboundMode();
            if (item == null)
            {
                return false; //fix null ref. exception
            }

            Telerik.WinControls.Data.NotifyCollectionChangingEventArgs args = new Telerik.WinControls.Data.NotifyCollectionChangingEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Remove, item);            
            this.ownerListElement.OnItemsChanging(args);
            if (args.Cancel)
            {
                return false;
            }

            item.DataLayer = null;
            item.Owner = null;

            bool removeValue = this.dataLayer.ListSource.Remove(item);
            Telerik.WinControls.Data.NotifyCollectionChangedEventArgs args1 = new Telerik.WinControls.Data.NotifyCollectionChangedEventArgs(Telerik.WinControls.Data.NotifyCollectionChangedAction.Remove, item);
            
            this.ownerListElement.OnItemsChanged(args1);
            return removeValue;
        }

        #endregion

        #region IEnumerable<ListItem> Members

        public IEnumerator<RadListDataItem> GetEnumerator()
        {
            return this.dataLayer.DataView.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.dataLayer.DataView).GetEnumerator();
        }

        #endregion

        //This interface is required for the Windows Forms designer to work.
        #region IList Members

        int IList.Add(object value)
        {
            RadListDataItem item = (RadListDataItem)value;
            this.Add(item);
            return item.RowIndex;
        }

        bool IList.Contains(object value)
        {
            return this.Contains((RadListDataItem)value);
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf((RadListDataItem)value);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (RadListDataItem)value);
        }

        bool IList.IsFixedSize
        {
            get
            {
                return ((IList)this.Owner.ListSource).IsFixedSize;
            }
        }

        void IList.Remove(object value)
        {
            this.Remove((RadListDataItem)value);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (RadListDataItem)value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)this.Owner.ListSource).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)this.Owner.ListSource).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)this.Owner.ListSource).SyncRoot;
            }
        }
        #endregion
    }
}
