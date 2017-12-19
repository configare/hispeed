using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls
{
    public class RadItemVirtualizationCollection : RadItemCollection, IVirtualizationCollection
    {
        private IVirtualViewport ownerViewport;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IVirtualViewport OwnerViewport
        {
            get
            {
                return this.ownerViewport;
            }
            set
            {
                this.ownerViewport = value;
                this.ownerViewport.SetVirtualItemsCollection(this);
            }
        }

        public RadItemVirtualizationCollection(IVirtualViewport ownerViewport)
        {
            this.OwnerViewport = ownerViewport;
        }

        public RadItemVirtualizationCollection()
        {
        }

        protected override void OnInsertComplete(int index, object value)
        {
            if (this.OwnerViewport != null)
                this.OwnerViewport.OnItemDataInserted(index, value);
            base.OnInsertComplete(index, value);
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            if (this.OwnerViewport != null)
                this.OwnerViewport.OnItemDataSet(index, oldValue, newValue);
            base.OnSetComplete(index, oldValue, newValue);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            if (this.OwnerViewport != null)
                this.OwnerViewport.OnItemDataRemoved(index, value);
            base.OnRemoveComplete(index, value);
        }

        protected override void OnClear()
        {
            if (this.OwnerViewport != null)
                this.OwnerViewport.OnItemsDataClear();
            base.OnClear();
        }

        protected override void OnClearComplete()
        {
            if (this.OwnerViewport != null)
                this.OwnerViewport.OnItemsDataClearComplete();
            base.OnClearComplete();
        }

        protected override void OnSort()
        {
            if (this.OwnerViewport != null)
                this.OwnerViewport.OnItemsDataSort();
            base.OnSort();
        }

        protected override void OnSortComplete()
        {
            if (this.OwnerViewport != null)
                this.OwnerViewport.OnItemsDataSortComplete();
            base.OnSortComplete();
        }


        #region IVirtualizationCollection Members

        public object GetVirtualData(int index)
        {
            if (index < 0 || index >= this.Count)
                return null;
            return this[index];
        }

        public int IndexOf(object data)
        {
            return List.IndexOf(data);
        }

        #endregion
    }
}
