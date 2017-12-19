using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// An ICollection implementation to store all <see cref="FloatingWindow">floating windows</see> associated with a RadDock instance.
    /// </summary>
    public class FloatingWindowCollection: ICollection
    {
        #region Fields

        private List<FloatingWindow> innerList = new List<FloatingWindow>(4);

        #endregion

        #region Implementation of IEnumerable

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            InnerList.CopyTo((FloatingWindow[])array, index);
        }

        /// <summary>
        /// Gets the number of FloatingWindow instances contained within the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return InnerList.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)InnerList).SyncRoot;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)InnerList).IsSynchronized;
            }
        }

        internal List<FloatingWindow> InnerList
        {
            get
            {
                return innerList;
            }
        }

        #endregion

        /// <summary>
        /// Get the floting window at specified index in the collection
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FloatingWindow this[int index]
        {
            get
            {
                return this.InnerList[index];
            }
        }
    }
}
