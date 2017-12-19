using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI
{
    internal class PropertyGridGroupItemWrappedItems: IList<PropertyGridItem>
    {
        IList<PropertyGridItem> items;

        public PropertyGridGroupItemWrappedItems(IList<PropertyGridItem> items)
        {
            this.items = items;
        }

        #region IList<PropertyGridItem> Members

        public int IndexOf(PropertyGridItem item)
        {
            return items.IndexOf(item as PropertyGridItem);
        }

        public void Insert(int index, PropertyGridItem item)
        {
            items.Insert(index, item as PropertyGridItem);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public PropertyGridItem this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        #endregion

        #region ICollection<PropertyGridItem> Members

        public void Add(PropertyGridItem item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(PropertyGridItem item)
        {
            return items.Contains(item);
        }

        public void CopyTo(PropertyGridItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return items.IsReadOnly; }
        }

        public bool Remove(PropertyGridItem item)
        {
            return items.Remove(item as PropertyGridItem);
        }

        #endregion

        #region IEnumerable<PropertyGridItem> Members

        public IEnumerator<PropertyGridItem> GetEnumerator()
        {
            return new MyEnumerator(items.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        class MyEnumerator : IEnumerator<PropertyGridItem>
        {
            IEnumerator enumerator;

            public MyEnumerator(IEnumerator enumerator)
            {
                this.enumerator = enumerator;
            }

            #region IEnumerator<PropertyGridItem> Members

            public PropertyGridItem Current
            {
                get { return enumerator.Current as PropertyGridItem; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return enumerator.Current; }
            }

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }

            #endregion
        }
    }
}
