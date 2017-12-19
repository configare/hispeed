using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class ListViewGroupedItemsCollection : IReadOnlyCollection<ListViewDataItem>
    {
        protected internal IList<ListViewDataItem> innerList;

        public ListViewGroupedItemsCollection()
        {
            innerList = new List<ListViewDataItem>();
        }
        
        public int Count
        {
            get { return innerList.Count; }
        }

        public ListViewDataItem this[int index]
        {
            get { return innerList[index]; }
        }

        public bool Contains(ListViewDataItem value)
        {
            return innerList.Contains(value);
        }

        public void CopyTo(ListViewDataItem[] array, int index)
        {
            innerList.CopyTo(array, index);
        }

        public int IndexOf(ListViewDataItem value)
        {
            return innerList.IndexOf(value);
        }

        public IEnumerator<ListViewDataItem> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }
    }
}
