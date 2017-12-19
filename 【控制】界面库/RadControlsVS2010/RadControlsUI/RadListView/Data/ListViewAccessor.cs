using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    class ListViewAccessor : IDisposable
    {
        private ListViewDetailColumn column;
        private bool suspendItemChanged;

        public void SuspendItemNotifications()
        {
            this.suspendItemChanged = true;
        }

        public void ResumeItemNotifications()
        {
            this.suspendItemChanged = false;
        }

        public ListViewAccessor(ListViewDetailColumn column)
        {
            if (column == null)
            {
                throw new ArgumentException("Column argument can not be null.");
            }

            this.column = column;
        }

        public ListViewDetailColumn Column
        {
            get
            {
                return this.column;
            }
        }

        public RadListViewElement Owner
        {
            get
            {
                return this.column.Owner;
            }
        }

        public virtual object this[ListViewDataItem item]
        {
            get
            {
                return item.Cache[this.column];
            }
            set
            {
                this.SetUnboundValue(item, value);
            }
        }

        private void SetUnboundValue(ListViewDataItem item, object value)
        { 
            item.Cache[column] = value;

            if (item != null && this.Owner != null && !this.suspendItemChanged)
            {
                this.Owner.ListSource.NotifyItemChanged(item);
            }
        }

        public override bool Equals(object obj)
        {
            ListViewAccessor accessor = obj as ListViewAccessor;

            if (obj == null)
            {
                return false;
            }

            return accessor.GetType() == this.GetType() && accessor.Column == this.Column;
        }

        public override int GetHashCode()
        {
            int columnHashCode = 0;

            if (this.column != null)
            {
                columnHashCode = this.column.GetHashCode();
            }

            return base.GetHashCode() ^ columnHashCode;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.column = null;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
