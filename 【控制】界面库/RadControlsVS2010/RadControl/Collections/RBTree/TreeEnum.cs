using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.Collections.Generic
{
    /// <summary>
    /// Generic tree enumerator
    /// </summary>
    /// <typeparam name="N">Node type</typeparam>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="P">>Node parameter</typeparam>
    internal class RBTreeEnumerator<N, K, P> : IEnumerator<N>
                   where N : RBTreeNodeBase<K, P>, new()
    {
        ///<summary>
        ///Tree
        ///</summary>
        private RBTreeBase<K, N, P> mTree;

        ///<summary>
        ///Current item
        ///
        ///</summary>
        private N mCurrent;

        ///<summary>
        ///Constructor
        ///</summary>
        public RBTreeEnumerator(RBTreeBase<K, N, P> aTree)
        {
            mTree = aTree;
            mCurrent = null;
        }

        ///<summary>
        ///Get current element
        ///</summary>
        N IEnumerator<N>.Current
        {
            get
            {
                return mCurrent;
            }
        }

        ///<summary>
        ///Get current element
        ///</summary>
        object IEnumerator.Current
        {
            get
            {
                return mCurrent;
            }
        }

        ///<summary>
        ///Move to next element
        ///</summary>
        public bool MoveNext()
        {
            if (mCurrent == null)
                mCurrent = mTree.First();
            else
                mCurrent = mTree.Next(mCurrent);
            return mCurrent != null;
        }

        ///<summary>
        ///Reset enumeration
        ///</summary>
        public void Reset()
        {
            mCurrent = null;
        }

        ///<summary>
        ///Dispose object
        ///</summary>
        public void Dispose()
        {
            mTree = null;
        }
    }

    /// <summary>
    /// Generic tree value's enumerator
    /// </summary>
    /// <typeparam name="N">Node type</typeparam>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="P">Node parameter</typeparam>
    internal class RBTreeValueEnumerator<N, K, P> : IEnumerator<K>
                   where N : RBTreeNodeBase<K, P>, new()
    {
        ///<summary>
        ///Tree
        ///</summary>
        private RBTreeBase<K, N, P> mTree;

        ///<summary>
        ///Current item
        ///</summary>
        private N mCurrent;

        ///<summary>
        ///Constructor
        ///</summary>
        public RBTreeValueEnumerator(RBTreeBase<K, N, P> aTree)
        {
            mTree = aTree;
            mCurrent = null;
        }

        ///<summary>
        ///Get current element
        ///</summary>
        K IEnumerator<K>.Current
        {
            get
            {
                return mCurrent.Key;
            }
        }

        ///<summary>
        ///Get current element
        ///</summary>
        object IEnumerator.Current
        {
            get
            {
                return mCurrent.Key;
            }
        }

        ///<summary>
        ///Move to next element
        ///</summary>
        public bool MoveNext()
        {
            if (mCurrent == null)
                mCurrent = mTree.First();
            else
                mCurrent = mTree.Next(mCurrent);
            return mCurrent != null;
        }

        ///<summary>
        ///Reset enumeration
        ///</summary>
        public void Reset()
        {
            mCurrent = null;
        }

        ///<summary>
        ///Dispose object
        ///</summary>
        public void Dispose()
        {
            mTree = null;
        }
    }
}
