using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class ListViewListSource : RadListSource<ListViewDataItem>
    {
        private RadListViewElement owner;

        public ListViewListSource(RadListViewElement owner) : base(owner)
        {
            this.owner = owner;
        }

        protected override void InitializeBoundRow(ListViewDataItem item, object dataBoundItem)
        {
            item.SetDataBoundItem(true, dataBoundItem);
        }
    }
}
