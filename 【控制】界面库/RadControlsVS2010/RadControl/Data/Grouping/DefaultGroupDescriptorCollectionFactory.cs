using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    public class DefaultGroupDescriptorCollectionFactory : IGroupDescriptorCollectionFactory
    {
        #region IGroupDescriptorCollectionFactory Members

        public GroupDescriptorCollection CreateCollection()
        {
            return new GroupDescriptorCollection();
        }

        #endregion
    }
}
