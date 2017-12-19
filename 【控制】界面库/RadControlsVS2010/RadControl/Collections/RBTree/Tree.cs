using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.Collections.Generic
{
    #region Node
    ///<summary>
    ///Tree node
    ///</summary>
    public class RBTreeNode<T> : RBTreeNodeBase<T, bool>
    {
        ///<summary>
        ///Constructor
        ///</summary>
        public RBTreeNode()
        {
        }
    }

    ///<summary>
    ///Unique RBTree
    ///</summary>
    public class RBTree<T> : RBTreeBase<T, RBTreeNode<T>, bool>
    {
        ///<summary>
        ///Tree constructor
        ///</summary>
        public RBTree()
               : base(true)
        {
        }

        ///<summary>
        ///Tree constructor with comparer
        ///</summary>
        public RBTree(IComparer<T> aComparer)
               : base(aComparer, true)
        {
        }

        ///<summary>
        ///Create new node
        ///</summary>
        protected override RBTreeNodeBase<T, bool> NewNode()
        {
            return new RBTreeNode<T>();
        }
    }

    ///<summary>
    ///Non-unique RBMultiTree
    ///</summary>
    public class RBMultiTree<T> : RBTreeBase<T, RBTreeNode<T>, bool>
    {
        ///<summary>
        ///Tree constructor
        ///</summary>
        public RBMultiTree()
               : base(false)
        {
        }

        ///<summary>
        ///Tree constructor with comparer
        ///</summary>
        public RBMultiTree(IComparer<T> aComparer)
               : base(aComparer, false)
        {
        }

        ///<summary>
        ///Create new node
        ///</summary>
        protected override RBTreeNodeBase<T, bool> NewNode()
        {
            return new RBTreeNode<T>();
        }
    }

    #endregion
}
