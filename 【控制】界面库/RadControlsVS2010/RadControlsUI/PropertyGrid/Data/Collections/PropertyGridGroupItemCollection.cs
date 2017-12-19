using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class PropertyGridGroupItemCollection : IList<PropertyGridGroupItem>
    {
        PropertyGridTableElement tableElement;

        public PropertyGridGroupItemCollection(PropertyGridTableElement tableElement)
        {
            this.tableElement = tableElement;
        }

        #region IList<PropertyGridItemBase> Members

        public int IndexOf(PropertyGridGroupItem item)
        {
            return tableElement.CollectionView.Groups.IndexOf(item.Group);
        }

        public void Insert(int index, PropertyGridGroupItem item)
        {
            throw new NotImplementedException();
        }

        public PropertyGridGroupItem this[int index]
        {
            get
            {
                return ((PropertyGridGroup)tableElement.CollectionView.Groups[index]).GroupItem;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public PropertyGridGroupItem this[string name]
        {
            get
            {
                int count = this.Count;
                for (int i = 0; i<count; i++)
                {
                    PropertyGridGroupItem item = this[i];
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<PropertyGridItemBase> Members

        public int Count
        {
            get
            {
                return this.tableElement.CollectionView.Groups.Count;
            }
        }

        public void Add(PropertyGridGroupItem item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(PropertyGridGroupItem item)
        {
            return tableElement.CollectionView.Groups.Contains(item.Group);
        }

        public void CopyTo(PropertyGridGroupItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(PropertyGridGroupItem item)
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
                return true;
            }
        }

        #endregion

        #region IEnumerable<PropertyGridGroupItem> Members

        IEnumerator<PropertyGridGroupItem> IEnumerable<PropertyGridGroupItem>.GetEnumerator()
        {
            return new PropertyGridGroupItemCollectionEnumerator(this);
        }

        #endregion

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            return new PropertyGridGroupItemCollectionEnumerator(this);
        }

        #endregion

        class PropertyGridGroupItemCollectionEnumerator : IEnumerator<PropertyGridGroupItem>
        {
            PropertyGridGroupItemCollection list;
            int index = -1;

            public PropertyGridGroupItemCollectionEnumerator(PropertyGridGroupItemCollection list)
            {
                this.list = list;
            }

            #region IEnumerator<PropertyGridItemBase> Members

            public PropertyGridGroupItem Current
            {
                get
                {
                    return ((IList<PropertyGridGroupItem>)this.list)[index];
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
                get { return ((IList<PropertyGridGroupItem>)list)[index]; }
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
