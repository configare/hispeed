using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Data.Expressions;

namespace Telerik.WinControls.Data
{
    internal class DataItemComparer<TDataItem> : IComparer<TDataItem>
        where TDataItem : IDataItem
    {
        private SortDescriptorCollection sortContext;
        public DataItemComparer(SortDescriptorCollection sortContext)
        {
            this.sortContext = sortContext;
        }

        #region IComparer<TDataItem> Members

        int IComparer<TDataItem>.Compare(TDataItem x, TDataItem y)
        {
            int hashCheck = x.GetHashCode().CompareTo(y.GetHashCode());
            if (hashCheck == 0)
            {
                return 0;
            }
            if (sortContext == null || sortContext.Count == 0)
            {
                return hashCheck;
            }

            for (int i = 0; i < this.sortContext.Count; i++)
            {
                int result = 0;

                if (this.sortContext[i].PropertyIndex < 0)
                {
                    this.sortContext[i].PropertyIndex = x.IndexOf(this.sortContext[i].PropertyName);
                }

                object xValue = x[this.sortContext[i].PropertyIndex];
                object yValue = y[this.sortContext[i].PropertyIndex];

                IComparable xCompVal = xValue as IComparable;

                if (xCompVal != null && yValue != null && yValue.GetType() == xValue.GetType())
                {
                    result = ((IComparable)xValue).CompareTo(yValue);
                }
                else
                {
                    result = DataStorageHelper.CompareNulls(xValue, yValue);
                }

                if (result != 0)
                {
                    if (this.sortContext[i].Direction != ListSortDirection.Descending)
                    {
                        return result;
                    }

                    return -result;
                }
            }

            return 0;
        }

        #endregion
    }
}
