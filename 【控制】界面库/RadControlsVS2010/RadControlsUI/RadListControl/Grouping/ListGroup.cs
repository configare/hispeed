using System.Collections.Generic;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    internal class ListGroup : DataItemGroup<RadListDataItem>
    {
        private RadListElement owner;
        private bool collapsed = false, collapsible = false;

        public bool Collapsed
        {
            get
            {
                return (this.Collapsible) ? this.collapsed : false;
            }
            set
            {
                this.collapsed = value;
            }
        }

        public bool Collapsible
        {
            get
            {
                return collapsible;
            }
            set
            {
                collapsible = value;
            }
        }

        public ListGroup(object key, RadListElement owner) : base(key)
        {
            this.owner = owner;
        }

        public void AddItem(RadListDataItem item)
        {
            if (!this.Items.Contains(item))
            {
                if (item.Group != null)
                {
                    item.Group.RemoveItem(item);
                }
                   
                this.Items.Add(item);
                item.Group = this;
            }
        }

        public void AddRange(IEnumerable<RadListDataItem> items)
        {
            owner.SuspendGroupRefresh();

            foreach (RadListDataItem item in items)
            {
                this.AddItem(item);
            }

            owner.ResumeGroupRefresh(true);
        }

        public void RemoveItem(RadListDataItem item)
        {
            this.Items.Remove(item);
            item.Group = null;
        }

        public void ClearItems()
        {
            foreach (RadListDataItem item in this.Items)
            {
                item.Group = null;
            }

            this.Items.Clear();
        }
    }
}