using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    public class DefaultSortDescriptorCollectionFactory : ISortDescriptorCollectionFactory
    {
        #region ISortFactory Members

        public SortDescriptorCollection CreateCollection()
        {
            return new SortDescriptorCollection();
        }

        #endregion
    }
}
