using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    class ListViewColumnTraverser : ItemsTraverser<ListViewDetailColumn>
    {
        public ListViewColumnTraverser(IList<ListViewDetailColumn> collection)
            : base(collection)
        {
        }

        protected override bool OnItemsNavigating(ListViewDetailColumn current)
        {
            return base.OnItemsNavigating(current) || !current.Visible;
        }
    }
}
