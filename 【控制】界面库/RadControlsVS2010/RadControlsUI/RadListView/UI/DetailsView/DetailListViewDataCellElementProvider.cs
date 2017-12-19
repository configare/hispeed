using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{ 
    public class DetailListViewDataCellElementProvider : BaseVirtualizedElementProvider<ListViewDetailColumn>
    {
        private DetailListViewVisualItem owner;

        public DetailListViewDataCellElementProvider(DetailListViewVisualItem owner)
        {
            this.owner = owner;
        }
     
        public override IVirtualizedElement<ListViewDetailColumn> CreateElement(ListViewDetailColumn data, object context)
        {
            ListViewCellElementCreatingEventArgs args = 
                new ListViewCellElementCreatingEventArgs(new DetailListViewDataCellElement(owner, data));

            if (data != null && data.Owner != null)
            {
                data.Owner.OnCellCreating(args);
            }

            return args.CellElement;
        }

        public override System.Drawing.SizeF GetElementSize(ListViewDetailColumn item)
        {
            return new System.Drawing.SizeF(item.Width, 50);
        }
    }
}
