using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    public interface IGroupFactory<T>
        where T : IDataItem
    {
        Group<T> CreateGroup(object key, Group<T> parent, params  object[] metaData);
        GroupCollection<T> CreateCollection(IList<Group<T>> list);
    }
}
