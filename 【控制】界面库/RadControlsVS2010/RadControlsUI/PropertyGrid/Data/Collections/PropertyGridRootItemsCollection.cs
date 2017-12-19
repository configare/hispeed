using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class PropertyGridRootItemsCollection : IList<PropertyGridItem>
    {
        PropertyGridTableElement tableElement;

        public PropertyGridRootItemsCollection(PropertyGridTableElement tableElement)
        {
            this.tableElement = tableElement;
        }

        #region IList<PropertyGridItemBase> Members

        public int IndexOf(PropertyGridItem item)
        {
            return tableElement.CollectionView.IndexOf(item);
        }

        public void Insert(int index, PropertyGridItem item)
        {
            throw new NotImplementedException();
        }

        PropertyGridItem IList<PropertyGridItem>.this[int index]
        {
            get
            {
                return tableElement.CollectionView[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<PropertyGridItem> Members

        public int Count
        {
            get
            {
                return this.tableElement.CollectionView.Count;
            }
        }

        public void Add(PropertyGridItem item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(PropertyGridItem item)
        {
            return tableElement.CollectionView.Contains(item as PropertyGridItem);
        }

        public void CopyTo(PropertyGridItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(PropertyGridItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get 
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<PropertyGridItemBase> Members

        IEnumerator<PropertyGridItem> IEnumerable<PropertyGridItem>.GetEnumerator()
        {
            return new PropertyGridRootItemsCollectionEnumerator(this);
        }

        #endregion

        #region IEnumerable
        
        public IEnumerator GetEnumerator()
        {
            return new PropertyGridRootItemsCollectionEnumerator(this);
        }

        #endregion

        class PropertyGridRootItemsCollectionEnumerator : IEnumerator<PropertyGridItem>
        {
            PropertyGridRootItemsCollection list;
            int index = -1;

            public PropertyGridRootItemsCollectionEnumerator(PropertyGridRootItemsCollection list)
            {
                this.list = list;
            }

            #region IEnumerator<PropertyGridItemBase> Members

            public PropertyGridItem Current
            {
                get
                {
                    return ((IList<PropertyGridItem>)this.list)[index];
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                list = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return ((IList<PropertyGridItem>)list)[index]; }
            }

            public bool MoveNext()
            {
                if (index < this.list.Count - 1)
                {
                    index++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                index = -1;
            }

            #endregion
        }
    }
}
