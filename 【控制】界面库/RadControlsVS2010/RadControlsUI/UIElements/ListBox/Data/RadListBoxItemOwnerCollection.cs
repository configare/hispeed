using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.UIElements.ListBox.Data
{
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    class RadListBoxItemOwnerCollection : RadItemVirtualizationCollection
    {
        RadListBoxElement owner = null;
        public RadListBoxItemOwnerCollection(RadListBoxElement owner)
            : base()
        {
            this.owner = owner;
        }

        protected override void OnRemove(int index, object value)
        {
            if (owner.IsDataSourceSet)
            {
                RadItem itemToDelete = this[index];
                if (itemToDelete is RadListBoxItem && (itemToDelete as RadListBoxItem).IsDataBound)
                {
                    throw new InvalidOperationException("Data bound items can not be removed. Remove items from the data source instead.");
                }
            }

            base.OnRemove(index, value);
            if (value is RadListBoxItem)
            {
                (value as RadListBoxItem).Index = -1;
            }
        }

        protected override void OnClear()
        {
            if (owner.IsDataSourceSet)
            {
                throw new InvalidOperationException("Collection can not be cleared while the control is data bound. Clear the data source instead or set the DataSource property to null.");
            }

            base.OnClear();
        }

        protected override void OnInsert(int index, object value)
        {
            if (owner.IsDataSourceSet && index > owner.FindBoundItemsBeginning() && index < owner.FindBoundItemsEnd())
            {
                throw new InvalidOperationException("Custom items can only be inserted before or after data bound items.");
            }

            if (value is RadListBoxItem)
            {
                (value as RadListBoxItem).Index = index;
            }

            base.OnInsert(index, value);
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            if (newValue is RadListBoxItem)
            {
                (newValue as RadListBoxItem).Index = index;
            }
            base.OnSet(index, oldValue, newValue);
        }
    }
}
