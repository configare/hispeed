using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
    public class DataItemGroup<TDataItem> : Group<TDataItem>
        where TDataItem : IDataItem
    {
        private const int ITEMS_CAPACITY = 4;
        private int version = -1;
        private List<TDataItem> items;
        private GroupBuilder<TDataItem> groupBuilder;
        private GroupCollection<TDataItem> groups;

        public DataItemGroup(object key)
            : this(key, null)
        {
        }

        public DataItemGroup(object key, Group<TDataItem> parent)
            : base(key, parent)
        {
            this.groups = null;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override GroupCollection<TDataItem> Groups
        {
            get
            {
                if (this.groupBuilder == null)
                {
                    return GroupCollection<TDataItem>.Empty;
                }

                if (groupBuilder.Version != this.version)
                {
                    this.groups = this.groupBuilder.Perform(this, this.Level + 1, this);
                    this.version = this.groupBuilder.Version;
                }

                return this.groups;
            }
        }

        protected internal GroupCollection<TDataItem> Cache
        {
            get
            {
                return this.groups;
            }
        }

        protected internal override IList<TDataItem> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new List<TDataItem>(ITEMS_CAPACITY);
                }

                return this.items;
            }
        }

        protected internal GroupBuilder<TDataItem> GroupBuilder
        {
            get { return this.groupBuilder; }
            set { groupBuilder = value; }
        }
    }
}
