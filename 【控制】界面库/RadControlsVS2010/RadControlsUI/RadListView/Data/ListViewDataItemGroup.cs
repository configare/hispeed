using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;
using System.ComponentModel; 

namespace Telerik.WinControls.UI
{
    [TypeConverter(typeof(ListViewDataItemGroupTypeConverter))]
    public class ListViewDataItemGroup : ListViewDataItem
    {
        #region Fields

        protected ListViewGroupedItemsCollection items; 
        protected Group<ListViewDataItem> dataGroup;
        protected const int ExpandedState = MajorBitState << 1;

        #endregion

        #region Ctors

        public ListViewDataItemGroup() 
        {
            items = new ListViewGroupedItemsCollection();
            this.bitState[ExpandedState] = true;
        }

        public ListViewDataItemGroup(string text) : this()
        {
            this.Text = text;
        }

        #endregion

        #region Properties

        [Browsable(false)]
        public override bool IsDataBound
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ListViewDataItemGroup Group
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ListViewSubDataItemCollection SubItems
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Enumerations.ToggleState CheckState
        {
            get
            {
                return base.CheckState;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the group's items should be displayed.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets a value indicating whether the group's items should be displayed.")]
        public bool Expanded
        {
            get
            {
                return this.bitState[ExpandedState];
            }
            set
            {
                if (this.Expanded != value && !OnNotifyPropertyChanging("Expanded"))
                {
                    this.bitState[ExpandedState] = value;
                    OnNotifyPropertyChanged("Expanded");
                }
            }
        }

        [Browsable(false)]
        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            internal set
            {
                if (value && this.owner.MultiSelect)
                {
                    bool allItemsSelected = this.Items.Count > 0;

                    foreach (ListViewDataItem item in this.Items)
                    {
                        item.Selected = true;
                        allItemsSelected &= item.Selected;
                    }

                    base.Selected = allItemsSelected;
                }
                else
                {
                    base.Selected = false;
                }
            }
        }

        /// <summary>
        /// Gets the items in this group.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the items in this group.")]
        public ListViewGroupedItemsCollection Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Gets the data group that is assigned to this group.
        /// </summary>
        [Browsable(false)]
        public Group<ListViewDataItem> DataGroup
        {
            get
            {
                return this.dataGroup;
            }
            internal set
            {
                if (this.dataGroup != value)
                {
                    this.SetDataGroup(value);
                }
            }
        }

        #endregion

        #region Methods

        internal virtual void OnItemSelectedChanged()
        {
            if (!this.owner.MultiSelect)
            {
                base.Selected = false;
                return;
            }

            bool allItemsSelected = this.Items.Count > 0;

            foreach (ListViewDataItem item in this.Items)
            {
                allItemsSelected &= item.Selected;
            }

            if (!this.Selected && allItemsSelected)
            {
                base.Selected = true;
            }
            if (this.Selected && !allItemsSelected)
            {
                base.Selected = false;
            }
        }

        private void SetDataGroup(Group<ListViewDataItem> value)
        {
            this.ClearUnboundItems();
            this.dataGroup = value;
            if (this.dataGroup != null)
            {
                this.items.innerList = this.dataGroup.Items;
                this.Text = value.Header;
                foreach (ListViewDataItem item in this.items)
                {
                    item.SetGroupCore(this, false);
                }
            }
            else
            {
                this.items.innerList = new List<ListViewDataItem>();
                this.Text = String.Empty;
            }
        }

        private void ClearUnboundItems()
        {
            while(this.items.Count > 0)
            {
                this.items[0].Group = null;
            }

            items.innerList.Clear();
        }

        protected override void OnNotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Owner")
            {
                foreach (ListViewDataItem item in this.Items)
                {
                    item.Owner = this.owner;
                }
            }

            base.OnNotifyPropertyChanged(args);
        }
         
        public override void Dispose()
        {
            ClearUnboundItems();
            base.Dispose();
        }

        #endregion
    }
}
