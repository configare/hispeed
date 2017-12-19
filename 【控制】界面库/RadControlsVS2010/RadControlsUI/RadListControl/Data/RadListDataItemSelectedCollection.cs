using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class RadListDataItemSelectedCollection : Collection<RadListDataItem>, IReadOnlyCollection<RadListDataItem>
    {
        private RadListElement owner = null;
        private bool removing = false;
        private bool inserting = false;

        public RadListDataItemSelectedCollection(RadListElement owner)
        {
            if (owner == null)
            {
                throw new ArgumentException("This collection can not be created without an owner. The owner argument can not be null.");
            }
            this.owner = owner;
        }

        protected override void InsertItem(int index, RadListDataItem item)
        {
            if (this.Inserting)
            {
                return;
            }

            this.Inserting = true;
            SelectionMode mode = owner.SelectionMode;

            if(mode == SelectionMode.None)
            {
                throw new InvalidOperationException("Items can not be selected when SelectionMode is None.");
            }

            base.InsertItem(index, item);
            this[index].SetValue(RadListDataItem.SelectedProperty, true);
            this.inserting = false;
        }

        protected override void RemoveItem(int index)
        {
            if (this.Removing || this.Clearing)
            {
                return;
            }

            this.Removing = true;
            if (this[index].Owner != null)
            {
                this[index].SetValue(RadListDataItem.SelectedProperty, false);
            }
            base.RemoveItem(index);
            this.Removing = false;
        }

        protected override void SetItem(int index, RadListDataItem item)
        {
            throw new InvalidOperationException("Set is not a valid operation on this collection.");
        }

        private bool clearing = false;
        protected override void ClearItems()
        {
            if (this.Clearing || this.Removing)
            {
                return;
            }

            this.Clearing = true;
            foreach (RadListDataItem item in this.Items)
            {
                if (item.Owner != null)
                {
                    item.SetValue(RadListDataItem.SelectedProperty, false);
                }
            }
            this.Clearing = false;
            base.ClearItems();
        }

        public bool Inserting
        {
            get
            {
                return this.inserting;
            }

            private set
            {
                this.inserting = value;
            }
        }

        public bool Removing
        {
            get
            {
                return this.removing;
            }

            private set
            {
                this.removing = value;
            }
        }

        public bool Clearing
        {
            get
            {
                return this.clearing;
            }

            private set
            {
                this.clearing = value;
            }
        }
    }
}
