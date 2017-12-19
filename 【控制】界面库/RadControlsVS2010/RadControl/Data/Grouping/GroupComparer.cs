using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Data.Expressions;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
    internal class GroupComparer<T> : IComparer<Group<T>>
         where T : IDataItem
    {
        private ListSortDirection[] directions;

        public GroupComparer()
        {
            
        }

        public GroupComparer(ListSortDirection[] directions)
        {
            this.directions = directions;
        }

        public ListSortDirection[] Directions
        {
            get { return directions; }
            set { directions = value; }
        }

        #region IComparer<Group<T>> Members

        public int Compare(Group<T> x, Group<T> y)
        {
            object[] xArr = x.Key as object[];
            object[] yArr = y.Key as object[];

            if (xArr != null && yArr != null && xArr.Length == yArr.Length)
            {
                int result = 0;
                for (int i = 0; i < xArr.Length; i++)
                {
                    IComparable xComp = xArr[i] as IComparable;
                    IComparable yComp = yArr[i] as IComparable;

                    if (xComp == null || yComp == null)
                    {
                        if (xComp == yComp)
                        {
                            result = 0;
                        }
                        else
                        {
                            result = DataStorageHelper.CompareNulls(xComp, yComp);
                        }
                    }
                    else
                    {
                        if (xComp.GetType() == yComp.GetType())
                        {
                            result = xComp.CompareTo(yComp);
                        }
                        else
                        {
                            result = -1;
                        }
                    }

                    if (result != 0)
                    {
                        if (this.directions[i] == ListSortDirection.Descending)
                        {
                            return -result;
                        }

                        return result;
                    }
                }

                return result;
            }

            if (x.Key is IComparable && x.Key.GetType() == y.Key.GetType())
            {
                int result = ((IComparable)x.Key).CompareTo(y.Key);
                if (this.directions[this.directions.Length - 1] == ListSortDirection.Descending)
                {
                    return -result;
                }

                return result;
            }

            return x.GetHashCode().CompareTo(y.GetHashCode());
        }

        #endregion
    }
}
