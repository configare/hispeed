using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A collection, which supports serialization of the contained <see cref="DockWindow">DockWindow</see> instances.
    /// </summary>
    public class DockWindowSerializableCollection: ICollection, IList, IList<DockWindow>
    {
        #region Fields

        private List<DockWindow> innerList;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DockWindowSerializableCollection">DockWindowSerializableCollection</see> class.
        /// </summary>
        /// <param name="innerList">The list of DockWindow instances to initialize with.</param>
        public DockWindowSerializableCollection(List<DockWindow> innerList)
        {
            this.innerList = innerList;
        }

        #endregion

        #region Implementation of IEnumerable

        public IEnumerator GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection

        public void CopyTo(Array array, int index)
        {
            innerList.CopyTo((DockWindow[])array, index);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public object SyncRoot
        {
            get { return ((ICollection)innerList).SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return ((ICollection)innerList).IsSynchronized; }
        }

        #endregion

        /// <summary>
        /// Gets the AutoHideGroup instance at the specified index in the collection
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DockWindow this[int index]
        {
            get { return this.innerList[index]; }
        }

        #region IList Members

        int IList.Add(object value)
        {
            return ((IList)this.innerList).Add((DockWindow)value);
        }

        public void Clear()
        {
            this.innerList.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.innerList.Contains((DockWindow)value);
        }

        int IList.IndexOf(object value)
        {
            return this.innerList.IndexOf((DockWindow)value);
        }

        void IList.Insert(int index, object value)
        {
            this.innerList.Insert(index, (DockWindow)value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            this.innerList.Remove((DockWindow)value);
        }

        void IList.RemoveAt(int index)
        {
            this.innerList.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this.innerList[index];
            }
            set
            {
                this.innerList[index] = (DockWindow)value;
            }
        }

        #endregion

        #region IList<DockWindow> Members

        int IList<DockWindow>.IndexOf(DockWindow item)
        {
            return innerList.IndexOf(item);
        }

        void IList<DockWindow>.Insert(int index, DockWindow item)
        {
            innerList.Insert(index, item);
        }

        void IList<DockWindow>.RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        DockWindow IList<DockWindow>.this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
            }
        }

        #endregion

        #region ICollection<DockWindow> Members

        public void Add(DockWindow item)
        {
            innerList.Add(item);
        }

        void ICollection<DockWindow>.Clear()
        {
            innerList.Clear();
        }

        bool ICollection<DockWindow>.Contains(DockWindow item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(DockWindow[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        int ICollection<DockWindow>.Count
        {
            get { return innerList.Count; }
        }

        bool ICollection<DockWindow>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DockWindow item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<DockWindow> Members

        IEnumerator<DockWindow> IEnumerable<DockWindow>.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion
    }
}
