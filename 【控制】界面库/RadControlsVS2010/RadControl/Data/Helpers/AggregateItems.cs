using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Data.Expressions;
using System.Collections;

namespace Telerik.WinControls.Data
{
    internal class AggregateItems<T> : IDataAggregate
        where T : IDataItem
    {
        private IEnumerable<T> items;

        public AggregateItems(IEnumerable<T> items)
        {
            this.items = items;
        }

        #region IDataAggregate Members

        public System.Collections.IEnumerable GetData()
        {
            return this.items;
        }

        #endregion
    }
}
