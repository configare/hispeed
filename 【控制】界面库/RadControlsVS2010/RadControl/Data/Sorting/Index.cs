using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Collections.Generic;

namespace Telerik.WinControls.Data
{
    public abstract class Index<T> : IReadOnlyCollection<T> where T : IDataItem
    {
        private RadCollectionView<T> collectionView;

        public Index(RadCollectionView<T> collectionView)
        {
            this.collectionView = collectionView;
        }

        public RadCollectionView<T> CollectionView
        {
            get
            {
                return this.collectionView;
            }
        }

        protected internal abstract IList<T> Items
        {
            get;
        }

        protected internal virtual void SetDirty()
        {
            
        }

        public virtual void Load(IEnumerable<T> source)
        {

        }

        protected abstract void Perform();
        

        #region IReadOnlyCollection<T> Members

        public int Count
        {
            get { return this.Items.Count; }
        }

        public T this[int index]
        {
            get { return this.Items[index]; }
        }

        public bool Contains(T value)
        {
            return this.Items.Contains(value);
        }

        public void CopyTo(T[] array, int index)
        {
            this.Items.CopyTo(array, index);
        }

        public int IndexOf(T value)
        {
            return this.Items.IndexOf(value);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
