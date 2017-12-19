using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class ListViewFilterDescriptorCollection : FilterDescriptorCollection
    {
        protected override void InsertItem(int index, FilterDescriptor item)
        {
            base.InsertItem(index, item);
            item.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
        }

        protected override void SetItem(int index, FilterDescriptor item)
        {
            this[index].PropertyChanged -= new PropertyChangedEventHandler(Item_PropertyChanged);
            base.SetItem(index, item);
            item.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
        }

        protected override void ClearItems()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i].PropertyChanged -= new PropertyChangedEventHandler(Item_PropertyChanged);
            }

            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            this[index].PropertyChanged -= new PropertyChangedEventHandler(Item_PropertyChanged);
            base.RemoveItem(index);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FilterDescriptor filterDescriptor = sender as FilterDescriptor;
            if (filterDescriptor == null)
            {
                return;
            }


            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanged,
                                    filterDescriptor));
        }
    }
}
