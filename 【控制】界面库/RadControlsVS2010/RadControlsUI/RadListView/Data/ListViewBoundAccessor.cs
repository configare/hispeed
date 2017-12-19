using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    class ListViewBoundAccessor : ListViewAccessor
    {
        public ListViewBoundAccessor(ListViewDetailColumn column)
            : base(column)
        {

        }

        public override object this[ListViewDataItem item]
        {
            get
            {
                if (item.DataBoundItem == null)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(this.Column.FieldName))
                {
                    return null;
                }

                return this.Owner.ListSource.GetBoundValue(item.DataBoundItem, this.Column.FieldName);
            }
            set
            {
                if (item.DataBoundItem == null)
                {
                    return;
                }

                this.Owner.ListSource.SetBoundValue(item, this.Column.FieldName, this.Column.Name, value, null);
            }
        }
    }
}
