using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class DetailListViewColumnScroller : ItemScroller<ListViewDetailColumn>
    { 
        public override void UpdateScrollRange()
        {
            base.UpdateScrollRange();
            this.OnScrollerUpdated(EventArgs.Empty);
        }
    }
}
