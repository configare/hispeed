using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
    public class ListViewSubDataItemCollection : IList, ICollection, IEnumerable
    {
        private List<object> values = new List<object>();
        private ListViewDataItem owner;

        public ListViewSubDataItemCollection(ListViewDataItem owner)
        {
            this.owner = owner;
        }


        #region IList members

        public int Add(object value)
        {
            return ((IList)values).Add(value);
        }

        public void Clear()
        {
            values.Clear();
        }

        public bool Contains(object value)
        {
            return values.Contains(value);
        }

        public int IndexOf(object value)
        {
            return values.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            values.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            values.Remove(value);
        }

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                if (index >= 0 && index <= values.Count)
                {
                    return values[index];
                }
                return String.Empty;
            }
            set
            {
                if (index >= 0 && index <= values.Count)
                {
                    values[index] = value;
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)values).CopyTo(array, index);
        }

        public int Count
        {
            get { return values.Count; }
        }

        public bool IsSynchronized
        {
            get { return ((IList)values).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((IList)values).SyncRoot;}
        }

        public IEnumerator GetEnumerator()
        {
            return values.GetEnumerator();
        }

        #endregion
    }
}
