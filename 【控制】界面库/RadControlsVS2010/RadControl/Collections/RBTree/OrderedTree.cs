using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.Collections.Generic
{
    #region Node
    ///<summary>
    ///Parameters of ordered node
    ///</summary>
    public struct RBOrderedNodeParam
    {
        ///<summary>
        ///Node's rank
        ///</summary>
        internal int mRank;
        ///<summary>
        ///Number of sub nodes
        ///</summary>
        internal int mCount;
    }

    ///<summary>
    ///Ordered node
    ///</summary>
    public class RBOrderedTreeNode<T> : RBTreeNodeBase<T, RBOrderedNodeParam>
    {
        ///<summary>
        ///Constructor
        ///</summary>
        public RBOrderedTreeNode()
        {
            mParam.mRank = 1;
            mParam.mCount = 0;
        }

        ///<summary>
        ///Set parent node
        ///</summary>
        internal override void SetParent(RBTreeNodeBase<T, RBOrderedNodeParam> value)
        {
            mParent = value;
            if (mParent != null)
                mParent.OnUpdateCount();

        }

        ///<summary>
        ///Set left node
        ///</summary>
        internal override void SetLeft(RBTreeNodeBase<T, RBOrderedNodeParam> value)
        {
            mLeft = value;
            OnUpdateCount();
        }

        ///<summary>
        ///Set right node
        ///</summary>
        internal override void SetRight(RBTreeNodeBase<T, RBOrderedNodeParam> value)
        {
            mRight = value;
            OnUpdateCount();
        }

        ///<summary>
        ///Update reference count
        ///</summary>
        internal override void OnUpdateCount()
        {
            int oldcount = mParam.mCount;
            mParam.mCount = 0;
            if (mLeft != null)
            {
                mParam.mCount = mLeft.mParam.mCount + 1;
                mParam.mRank = mParam.mCount + 1;
            }
            else
                mParam.mRank = 1;

            if (mRight != null)
                mParam.mCount += mRight.mParam.mCount + 1;

            if (mParam.mCount != oldcount && mParent != null)
                mParent.OnUpdateCount();

            return ;
        }


        ///<summary>
        ///Copy from other node
        ///</summary>
        internal override void CopyFrom(RBTreeNodeBase<T, RBOrderedNodeParam> z)
        {
            this.mParam.mRank = z.mParam.mRank;
            this.mParam.mCount = z.mParam.mCount;
            base.CopyFrom(z);
        }
    }

    ///<summary>
    ///Basic RBTree with ordering
    ///
    ///Operation like Add and Remove are an O(2logN) operations.
    ///Operation Find is O(logN) operation.
    ///</summary>
    public class RBOrderedTreeBase<T> : RBTreeBase<T, RBOrderedTreeNode<T>, RBOrderedNodeParam>, IOrderedTree<T>
    {
        ///<summary>
        ///Tree constructor
        ///</summary>
        public RBOrderedTreeBase(bool aUnique)
               : base(aUnique)
        {
        }

        ///<summary>
        ///Tree constructor with comparer
        ///</summary>
        public RBOrderedTreeBase(IComparer<T> aComparer, bool aUnique)
               : base(aComparer, aUnique)
        {
        }

        ///<summary>
        ///Create new node
        ///</summary>
        protected override RBTreeNodeBase<T, RBOrderedNodeParam> NewNode()
        {
            return new RBOrderedTreeNode<T>();
        }

        ///<summary>
        ///Get item by order index
        ///This operation is O(logN) operation
        ///</summary>
        public RBOrderedTreeNode<T> GetByOrder(int idx)
        {
            int m = idx + 1;
            RBTreeNodeBase<T, RBOrderedNodeParam> node = mRoot;

            while (node != null && m > 0)
            {
                if (m < node.mParam.mRank)
                {
                    node = node.mLeft;
                }
                else if (m > node.mParam.mRank)
                {
                    m = m - node.mParam.mRank;
                    node = node.mRight;
                }
                else if (m == node.mParam.mRank)
                {
                    return node as RBOrderedTreeNode<T>;
                }
            }
            return null;
        }

        ///<summary>
        ///Get order index of item
        ///This operation is O(logN) operation
        ///</summary>
        public int GetOrder(RBOrderedTreeNode<T> aItem)
        {
            RBTreeNodeBase<T, RBOrderedNodeParam> node = aItem;
            int idx = node.mParam.mRank;

            while (true)
            {
                if (node.mParent == null)
                    break ;

                if (node.mParent.mRight == node)
                {
                    idx += node.mParent.mParam.mRank;
                }
                node = node.mParent;
            }
            return idx - 1;
        }

        ///<summary>
        ///Get item by order index
        ///</summary>
        ITreeNode<T> IOrderedTree<T>.GetByOrder(int idx)
        {
            return GetByOrder(idx);
        }

        ///<summary>
        ///Get index by item
        ///</summary>
        int IOrderedTree<T>.GetOrder(ITreeNode<T> node)
        {
            return GetOrder((RBOrderedTreeNode<T>) node);
        }

    }

    ///<summary>
    ///Unique ordered RBTree
    ///</summary>
    public class RBOrderedTree<T> : RBOrderedTreeBase<T>
    {
        ///<summary>
        ///Tree constructor
        ///</summary>
        public RBOrderedTree()
               : base(true)
        {
        }

        ///<summary>
        ///Tree constructor with comparer
        ///</summary>
        public RBOrderedTree(IComparer<T> aComparer)
               : base(aComparer, true)
        {
        }
    }

    ///<summary>
    ///Non-unique RBMultiTree
    ///</summary>
    public class RBOrderedMultiTree<T> : RBOrderedTreeBase<T>
    {
        ///<summary>
        ///Tree constructor
        ///</summary>
        public RBOrderedMultiTree()
               : base(false)
        {
        }

        ///<summary>
        ///Tree constructor with comparer
        ///</summary>
        public RBOrderedMultiTree(IComparer<T> aComparer)
               : base(aComparer, false)
        {
        }
    }
    #endregion
}
