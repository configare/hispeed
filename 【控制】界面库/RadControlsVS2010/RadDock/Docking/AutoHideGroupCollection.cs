using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A collection that containes instances of type <see cref="AutoHideGroup">AutoHideGroup</see>.
    /// </summary>
    public class AutoHideGroupCollection : ICollection, IList, IList<AutoHideGroup>
    {
        #region Fields

        private List<AutoHideGroup> innerList;

        #endregion

        #region Constructor

        internal AutoHideGroupCollection()
        {
            this.innerList = new List<AutoHideGroup>();
        }

        #endregion

        #region Implementation of IEnumerable

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection

        /// <summary>
        /// Copies the collection to the destination Array, starting from the specified index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            innerList.CopyTo((AutoHideGroup[])array, index);
        }

        /// <summary>
        /// Gets the number of AutoHideGroup instances currently contained.
        /// </summary>
        public int Count
        {
            get
            {
                return innerList.Count;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)innerList).SyncRoot;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)innerList).IsSynchronized;
            }
        }

        #endregion

        /// <summary>
        /// Gets the AutoHideGroup instance at the specified index in the collection
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AutoHideGroup this[int index]
        {
            get
            {
                return this.innerList[index];
            }
        }

        #region IList Members

        int IList.Add(object value)
        {
            return ((IList)this.innerList).Add((AutoHideGroup)value);
        }

        /// <summary>
        /// Removes all entries from the collection.
        /// </summary>
        public void Clear()
        {
            this.innerList.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.innerList.Contains((AutoHideGroup)value);
        }

        int IList.IndexOf(object value)
        {
            return this.innerList.IndexOf((AutoHideGroup)value);
        }

        void IList.Insert(int index, object value)
        {
            this.innerList.Insert(index, (AutoHideGroup)value);
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
            this.innerList.Remove((AutoHideGroup)value);
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
                this.innerList[index] = (AutoHideGroup)value;
            }
        }

        #endregion

        #region IList<AutoHideGroup> Members

        int IList<AutoHideGroup>.IndexOf(AutoHideGroup item)
        {
            return innerList.IndexOf(item);
        }

        void IList<AutoHideGroup>.Insert(int index, AutoHideGroup item)
        {
            innerList.Insert(index, item);
        }

        void IList<AutoHideGroup>.RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        AutoHideGroup IList<AutoHideGroup>.this[int index]
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

        #region ICollection<AutoHideGroup> Members

        /// <summary>
        /// Adds the specified <see cref="AutoHideGroup">AutoHideGroup</see> instance to the collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(AutoHideGroup item)
        {
            innerList.Add(item);
        }

        void ICollection<AutoHideGroup>.Clear()
        {
            innerList.Clear();
        }

        bool ICollection<AutoHideGroup>.Contains(AutoHideGroup item)
        {
            return innerList.Contains(item);
        }

        /// <summary>
        /// Copies the collection to the destination Array, starting from the specified index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(AutoHideGroup[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        int ICollection<AutoHideGroup>.Count
        {
            get { return innerList.Count; }
        }

        bool ICollection<AutoHideGroup>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified instance from the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(AutoHideGroup item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<AutoHideGroup> Members

        IEnumerator<AutoHideGroup> IEnumerable<AutoHideGroup>.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion
    }
}
