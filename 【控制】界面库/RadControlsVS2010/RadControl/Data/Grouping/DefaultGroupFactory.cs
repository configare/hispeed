using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    public class DefaultGroupFactory<TDataItem> : IGroupFactory<TDataItem>
        where TDataItem : IDataItem
    {

        #region IGroupFactory<TDataItem> Members

        public Group<TDataItem> CreateGroup(object key, Group<TDataItem> parent, params object[] metaData)
        {
            return new DataItemGroup<TDataItem>(key, parent);
        }

        public GroupCollection<TDataItem> CreateCollection(IList<Group<TDataItem>> list)
        {
            return new GroupCollection<TDataItem>(list);
        }

        #endregion
    }
}
