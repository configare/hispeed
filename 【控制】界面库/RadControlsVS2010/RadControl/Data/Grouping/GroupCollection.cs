using System;
using System.Collections.Generic;
using Telerik.Data.Expressions;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Telerik.WinControls.Data
{
    public class GroupCollection<T> : ReadOnlyCollection<Group<T>>, IDisposable
        where T : IDataItem
    {
        public static GroupCollection<T> Empty = new GroupCollection<T>(new List<Group<T>>());

        public GroupCollection(IList<Group<T>> list)
            : base(list)
        {

        }

        protected internal IList<Group<T>> GroupList
        {
            get
            {
                return this.Items;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Items.Clear();
        }

        #endregion
    }
}
