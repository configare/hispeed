using System;
using System.Collections;
using System.Collections.Generic;

/*
   OK here we go, the balanced tree stuff. The algorithm is the
   fairly standard red/black taken from "Introduction to Algorithms"
   by Cormen, Leiserson & Rivest. Maybe one of these days I will
   fully understand all this stuff.

   Basically a red/black balanced tree has the following properties:
   1) Every node is either red or black (colour is Color.Red or Color.Black)
   2) A leaf (null pointer) is considered black
   3) If a node is red then its children are black
   4) Every path from a node to a leaf contains the same no
      of black nodes

   3) & 4) above guarantee that the longest path (alternating
   red and black nodes) is only twice as long as the shortest
   path (all black nodes). Thus the tree remains fairly balanced.
 */

namespace Telerik.Collections.Generic
{
    /// <summary>
    /// Base class for the tree.
    /// Based on the Damian Ivereigh implementation
    /// Support for the multi-trees has been added.
    /// Do not use this class directly. Use RBTree, RBMultiTree, RBOrderedTree and RBOrderedMultiTree classes
    /// </summary>
    /// <typeparam name="T">Key type</typeparam>
    /// <typeparam name="N">Node type</typeparam>
    /// <typeparam name="P">Node parameter type</typeparam>
    public abstract class RBTreeBase<T, N, P> : ISortedTree<T>, IEnumerable<N>
                    where N : RBTreeNodeBase<T, P>, new()
    {
        #region ITree interface wrapper
        ///<summary>
        ///Add item
        ///</summary>
        ITreeNode<T> ITree<T>.Add(T item)
        {
            return Add(item);
        }

        ///<summary>
        ///Add or get item
        ///</summary>
        ITreeNode<T> ITree<T>.AddOrGet(T item)
        {
            return AddOrGet(item);
        }

        ///<summary>
        ///Find item
        ///</summary>
        ITreeNode<T> ITree<T>.Find(T item)
        {
            return Find(item);
        }

        ///<summary>
        ///Delete item by key
        ///</summary>
        bool ITree<T>.Remove(T item)
        {
            return Remove(item);
        }

        ///<summary>
        ///Clear
        ///</summary>
        void ITree<T>.Clear()
        {
            Clear();
        }

        ///<summary>
        ///Delete item by key
        ///</summary>
        void ITree<T>.Remove(ITreeNode<T> node)
        {
            Remove((N)node);
        }

        ///<summary>
        ///Get first node
        ///</summary>
        ITreeNode<T> ISortedTree<T>.First()
        {
            return First();
        }

        ///<summary>
        ///Get last node
        ///</summary>
        ITreeNode<T> ISortedTree<T>.Last()
        {
            return Last();
        }

        ///<summary>
        ///Get next node
        ///</summary>
        ITreeNode<T> ISortedTree<T>.Previous(ITreeNode<T> node)
        {
            return Previous((N)node);
        }

        ///<summary>
        ///Get prior node
        ///</summary>
        ITreeNode<T> ISortedTree<T>.Next(ITreeNode<T> node)
        {
            return Next((N)node);
        }
        #endregion

        #region Collection adapter
        ///<summary>
        ///Adapter implementing collection interface
        ///</summary>
        class CollectionAdapter<T1, N1, P1> :IList<T1> , ICollection<T1>, ICollection
                    where N1 : RBTreeNodeBase<T1, P1>, new()
        {
            ///<summary>
            ///Referenced tree
            ///</summary>
            RBTreeBase<T1, N1, P1> mTree;

            ///<summary>
            ///Constructor
            ///</summary>
            public CollectionAdapter(RBTreeBase<T1, N1, P1> aTree)
            {
                mTree = aTree;
            }

            public int Count
            {
                get
                {
                    return mTree.Count;
                }
            }

            public bool IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            public object SyncRoot
            {
                get
                {
                    return mTree.SyncRoot;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public void Add(T1 item)
            {
                mTree.Add(item);
            }

            public void Clear()
            {
                mTree.Clear();
            }

            public bool Contains(T1 item)
            {
                return mTree.Find(item) != null;
            }

            public bool Remove(T1 item)
            {
                return mTree.Remove(item);
            }

            public void CopyTo(T1[] array, int index)
            {
                foreach(T1 value in this)
                    array[index++] = value;
            }

            public void CopyTo(Array array, int index)
            {
                foreach(T1 value in this)
                    array.SetValue(value, index++);
            }

            IEnumerator<T1> IEnumerable<T1>.GetEnumerator()
            {
                return new RBTreeValueEnumerator<N1, T1, P1>(mTree);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new RBTreeValueEnumerator<N1, T1, P1>(mTree);
            }

            #region IList<T> Members

            public int IndexOf(T1 item)
            {
                RBOrderedMultiTree<T1> multiTree = mTree as RBOrderedMultiTree<T1>;
                if (multiTree == null)
                {
                    return -1;
                }

                RBOrderedTreeNode<T1> node = multiTree.Find(item);
                return multiTree.GetOrder(node);
            }

            public void Insert(int index, T1 item)
            {
                this.Add(item);
            }

            public void RemoveAt(int index)
            {
                RBOrderedMultiTree<T1> multiTree = mTree as RBOrderedMultiTree<T1>;
                if (multiTree == null)
                {
                    return;
                }

                RBOrderedTreeNode<T1> node = multiTree.GetByOrder(index);
                if (node != null)
                {
                    multiTree.Delete(node);
                }
            }

            public T1 this[int index]
            {
                get
                {
                    RBOrderedMultiTree<T1> multiTree = mTree as RBOrderedMultiTree<T1>;
                    if (multiTree == null)
                    {
                        return default(T1);
                    }

                    RBOrderedTreeNode<T1> node = multiTree.GetByOrder(index);
                    if (node != null)
                    {
                        return node.Key;
                    }

                    return default(T1);
                }
                set
                {
                    RBOrderedMultiTree<T1> multiTree = mTree as RBOrderedMultiTree<T1>;
                    if (multiTree == null)
                    {
                        return;
                    }

                    RBOrderedTreeNode<T1> node = multiTree.GetByOrder(index);
                    if (node != null)
                    {
                        multiTree.Remove(node);
                        multiTree.Add(value);
                    }
                }
            }

            #endregion

            
        }
        #endregion

        #region Properties
        ///<summary>
        ///Is tree unique
        ///</summary>
        public bool Unique
        {
            get
            {
                return mUnique;
            }
        }
        private bool mUnique;

        ///<summary>
        ///Object can be used for synchronization
        ///</summary>
        public object SyncRoot
        {
            get
            {
                return mSyncRoot;
            }
        }
        protected object mSyncRoot = new object();

        ///<summary>
        ///Root of the tree
        ///</summary>
        public N Root
        {
            get
            {
                return (N)mRoot;
            }
        }
        protected RBTreeNodeBase<T, P> mRoot;

        ///<summary>
        ///Comparator
        ///</summary>
        internal IComparer<T> mComparer;

        ///<summary>
        ///Number of nodes in the tree
        ///</summary>
        public int Count
        {
            get
            {
                return mCount;
            }
        }
        protected int mCount;
        #endregion

        #region public methods
        ///<summary>
        ///Tree constructor
        ///</summary>
        public RBTreeBase(bool unique)
        {
            mRoot = null;
            mComparer = Comparer<T>.Default;
            mCount = 0;
            mUnique = unique;
        }

        ///<summary>
        ///Tree constructor with comparer
        ///</summary>
        public RBTreeBase(IComparer<T> aComparer, bool unique)
        {
            mRoot = null;
            mComparer = aComparer;
            mCount = 0;
            mUnique = unique;
        }

        ///<summary>
        ///Add new key into the tree
        ///
        ///This operation is O(logN) operation
        ///</summary>
        ///<exception cref="System.ArgumentException">In case the key is already in the tree</exception>
        public N Add(T aKey)
        {
            RBTreeNodeBase<T, P> key;
            bool insert = true;
            key = Traverse(ref insert, aKey);
            if (!insert)
                throw new System.ArgumentException();
            mCount++;
            return key as N;
        }

        ///<summary>
        ///Add new key into the tree or get existing node
        ///This operation is O(logN) operation
        ///</summary>
        public N AddOrGet(T aKey)
        {
            RBTreeNodeBase<T, P> key;
            bool insert = true;
            if (mUnique)
            {
                key = Traverse(ref insert, aKey);
                if (insert)
                    mCount++;
            }
            else
            {
                insert = false;
                key = Traverse(ref insert, aKey);
                if (key == null)
                {
                    insert = true;
                    key = Traverse(ref insert, aKey);
                    mCount++;
                }
            }
            return key as N;
        }

        ///<summary>
        ///Remove key from the dictionary
        ///This operation is O(logN) operation
        ///</summary>
        public bool Remove(T aKey)
        {
            RBTreeNodeBase<T, P> key;
            key = Find(aKey);
            if (key == null)
                return false;
            mCount--;
            Delete(key);
            return true;
        }

        ///<summary>
        ///Remove all items
        ///</summary>
        public void Clear()
        {
            mRoot = null;
            mCount = 0;
        }

        ///<summary>
        ///Remove node from the dictionary
        ///This operation is O(1) operation
        ///</summary>
        public bool Remove(N aNode)
        {
            Delete(aNode);
            mCount--;
            return true;
        }

        ///<summary>
        ///Find key in the dictionary
        ///This operation is O(logN) operation
        ///</summary>
        public N Find(T aKey)
        {
            RBTreeNodeBase<T, P> x, y;
            int cmp;

            //walk down the tree
            x = mRoot;
            while (x != null)
            {
                cmp = mComparer.Compare(aKey, x.mKey);
                if (cmp < 0)
                    x = x.mLeft;
                else if (cmp > 0)
                    x = x.mRight;
                else
                {
                    if (!mUnique)
                    {
                        y = x;
                        y = Predecessor(y);
                        while (y != null && mComparer.Compare(aKey, y.mKey) == 0)
                        {
                            x = y;
                            y = Predecessor(y);
                        }
                    }
                    return x as N;
                }
            }
            return null;
        }

        ///<summary>
        ///Get first node
        ///This operation is O(logN) operation
        ///</summary>
        public N First()
        {
            RBTreeNodeBase<T, P> x, y;

            //points to the parent of x
            y = null;
            x = mRoot;

            // Keep going left until we hit a NULL
            while(x != null)
            {
                y = x;
                x = x.mLeft;
            }
            return y as N;
        }

        ///<summary>
        ///Get last node
        ///This operation is O(logN) operation
        ///</summary>
        public N Last()
        {
            RBTreeNodeBase<T, P> x, y;

            //points to the parent of x
            y = null;
            x = mRoot;

            // Keep going right until we hit a NULL
            while (x != null)
            {
                y = x;
                x = x.mRight;
            }
            return y as N;
        }

        ///<summary>
        ///Get next node
        ///This operation is O(logN) operation
        ///</summary>
        public N Next(N aNode)
        {
            return Successor(aNode) as N;
        }

        ///<summary>
        ///Get previous node
        ///This operation is O(logN) operation
        ///</summary>
        public N Previous(N aNode)
        {
            return Predecessor(aNode) as N;
        }

        ///<summary>
        ///Get enumerator
        ///</summary>
        IEnumerator<N> IEnumerable<N>.GetEnumerator()
        {
            return new RBTreeEnumerator<N, T, P>(this);
        }

        ///<summary>
        ///Get enumerator
        ///</summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RBTreeEnumerator<N, T, P>(this);
        }


        #endregion

        #region internal methods
        ///<summary>
        ///Balance tree past inserting
        ///</summary>
        protected void Balance(RBTreeNodeBase<T, P> z)
        {
            RBTreeNodeBase<T, P> x, y;

            //Having added a red node, we must now walk back up the tree balancing
            //it, by a series of rotations and changing of colours
            x = z;

            //While we are not at the top and our parent node is red
            //N.B. Since the root node is garanteed black, then we
            //are also going to stop if we are the child of the root
            while (x != mRoot && (x.mParent.mColor == RBTreeColor.Red))
            {
                //if our parent is on the left side of our grandparent
                if (x.mParent == x.mParent.mParent.mLeft)
                {
                    //get the right side of our grandparent (uncle?)
                    y = x.mParent.mParent.mRight;
                    if (y != null && y.mColor == RBTreeColor.Red)
                    {
                        //make our parent black
                        x.mParent.mColor = RBTreeColor.Black;
                        //make our uncle black
                        y.mColor = RBTreeColor.Black;
                        //make our grandparent red
                        x.mParent.mParent.mColor = RBTreeColor.Red;
                        //now consider our grandparent
                        x = x.mParent.mParent;
                    }
                    else
                    {
                        //if we are on the right side of our parent
                        if (x == x.mParent.mRight)
                        {
                            //Move up to our parent
                            x = x.mParent;
                            LeftRotate(x);
                        }

                        /* make our parent black */
                        x.mParent.mColor = RBTreeColor.Black;
                        /* make our grandparent red */
                        x.mParent.mParent.mColor = RBTreeColor.Red;
                        /* right rotate our grandparent */
                        RightRotate(x.mParent.mParent);
                    }
                }
                else
                {
                    //everything here is the same as above, but
                    //exchanging left for right
                    y = x.mParent.mParent.mLeft;
                    if (y != null && y.mColor == RBTreeColor.Red)
                    {
                        x.mParent.mColor = RBTreeColor.Black;
                        y.mColor = RBTreeColor.Black;
                        x.mParent.mParent.mColor = RBTreeColor.Red;

                        x = x.mParent.mParent;
                    }
                    else
                    {
                        if (x == x.mParent.mLeft)
                        {
                            x = x.mParent;
                            RightRotate(x);
                        }

                        x.mParent.mColor = RBTreeColor.Black;
                        x.mParent.mParent.mColor = RBTreeColor.Red;
                        LeftRotate(x.mParent.mParent);
                    }
                }
            }
            mRoot.mColor = RBTreeColor.Black;
        }

        ///<summary>
        ///Create new node
        ///</summary>
        protected abstract RBTreeNodeBase<T, P> NewNode();

        ///<summary>
        ///Go trough tree and find the node by the key.
        ///Might add new node if node doesn't exist.
        ///</summary>
        internal RBTreeNodeBase<T, P> Traverse(ref bool aInsert, T aKey)
        {
            RBTreeNodeBase<T, P> x, y, z;
            int cmp;

            //walk down the tree
            y = null;
            x = mRoot;
            while (x != null)
            {
                y = x;
                cmp = mComparer.Compare(aKey, x.mKey);

                if (!mUnique && cmp == 0 && aInsert == true)
                    cmp = 1;

                if (cmp < 0)
                    x = x.mLeft;
                else if (cmp > 0)
                    x = x.mRight;
                else
                {
                    aInsert = false;
                    return x;
                }
            }

            //x is null. return null if node must not be inserted
            if (!aInsert)
                return null;

            z = NewNode();
            //x is null and insert operation is requested
            //create new node
            z.mKey = aKey;
            z.mParent = y;
            if (y == null)
                mRoot = z;
            else
            {
                cmp = mComparer.Compare(z.mKey, y.mKey);
                if (cmp == 0)
                    cmp = 1;
                if (cmp < 0)
                    y.SetLeft(z);
                else
                    y.SetRight(z);
            }
            z.mColor = RBTreeColor.Red;

            Balance(z);
            mRoot.mColor = RBTreeColor.Black;
            return z;
        }

        ///<summary>
        /// Rotate our tree Left
        ///
        ///             X        rb_left_rotate(X)--->            Y
        ///           /   \                                     /   \
        ///          A     Y                                   X     C
        ///              /   \                               /   \
        ///             B     C                             A     B
        ///
        /// N.B. This does not change the ordering.
        ///
        /// We assume that neither X or Y is NULL
        /// </summary>
        protected void LeftRotate(RBTreeNodeBase<T, P> x)
        {
            RBTreeNodeBase<T, P> y;

            // set Y
            y = x.mRight;

            // Turn Y's left subtree into X's right subtree (move B)
            x.mRight = y.mLeft;

            // If B is not null, set it's parent to be X
            if (y.mLeft != null)
                y.mLeft.mParent = x;

            // Set Y's parent to be what X's parent was
            y.mParent = x.mParent;

            // if X was the root
            if (x.mParent == null)
                mRoot = y;
            else
            {
                // Set X's parent's left or right pointer to be Y
                if (x == x.mParent.mLeft)
                    x.mParent.mLeft = y;
                else
                    x.mParent.mRight = y;
            }

            // Put X on Y's left
            y.mLeft = x;

            // Set X's parent to be Y
            x.mParent = y;

            x.OnUpdateCount();

        }

        ///<summary>
        /// Rotate our tree Right
        ///
        ///             X                                         Y
        ///           /   \                                     /   \
        ///          A     Y     leftArrow--rb_right_rotate(Y)        X     C
        ///              /   \                               /   \
        ///             B     C                             A     B
        ///
        /// N.B. This does not change the ordering.
        ///
        /// We assume that neither X or Y is NULL
        ///</summary>>
        protected void RightRotate(RBTreeNodeBase<T, P> y)
        {
            RBTreeNodeBase<T, P> x;

            // set X
            x = y.mLeft;

            // Turn X's right subtree into Y's left subtree (move B)
            y.mLeft = x.mRight;

            // If B is not null, set it's parent to be Y
            if (x.mRight != null)
                x.mRight.mParent = y;

            // Set X's parent to be what Y's parent was
            x.mParent = y.mParent;

            // if Y was the root
            if (y.mParent == null)
                mRoot = x;
            else
            {
                // Set Y's parent's left or right pointer to be X
                if (y == y.mParent.mLeft)
                    y.mParent.mLeft = x;
                else
                    y.mParent.mRight = x;
            }

            // Put Y on X's right
            x.mRight = y;

            // Set Y's parent to be X
            y.mParent = x;

            y.OnUpdateCount();
        }

        ///<summary>
        ///Return a pointer to the smallest key greater than x
        ///</summary>
        protected RBTreeNodeBase<T, P> Successor(RBTreeNodeBase<T, P> x)
        {
            RBTreeNodeBase<T, P> y;

            if (x.mRight != null)
            {
                // If right is not NULL then go right one and
                // then keep going left until we find a node with
                // no left pointer.
                for (y = x.mRight; y.mLeft != null; y = y.mLeft);
            }
            else
            {
                // Go up the tree until we get to a node that is on the
                // left of its parent (or the root) and then return the
                // parent.
                y = x.mParent;
                while (y != null && x == y.mRight)
                {
                    x = y;
                    y = y.mParent;
                }
            }
            return y;
        }
        ///<summary>
        ///Return a pointer to the largest key smaller than x
        ///</summary>
        protected RBTreeNodeBase<T, P> Predecessor(RBTreeNodeBase<T, P> x)
        {
            RBTreeNodeBase<T, P> y;

            if (x.mLeft != null)
            {
                // If left is not NULL then go left one and
                // then keep going right until we find a node with
                // no right pointer.
                for (y = x.mLeft; y.mRight != null; y = y.mRight);
            }
            else
            {
                // Go up the tree until we get to a node that is on the
                // right of its parent (or the root) and then return the
                // parent.
                y = x.mParent;
                while(y != null && x == y.mLeft)
                {
                    x = y;
                    y = y.mParent;
                }
            }
            return y;
        }

        /// <summary>
        /// Delete the node z, and free up the space
        /// </summary>
        protected virtual void Delete(RBTreeNodeBase<T, P> z)
        {
            RBTreeNodeBase<T, P> x, y;

            if (z.mLeft == null || z.mRight == null)
                y = z;
            else
                y = Successor(z);

            if (y.mLeft != null)
                x = y.mLeft;
            else
                x = y.mRight;

            if (x != null)
                x.SetParent(y.mParent);

            if (y.mParent == null)
                mRoot = x;
            else
            {
                if (y == y.mParent.mLeft)
                    y.mParent.SetLeft(x);
                else
                    y.mParent.SetRight(x);
            }

            if (y != z)
            {
                //we must replace 'z' with 'y' node
                y.CopyFrom(z);

                if (z == mRoot)
                    mRoot = y;

                //we do this all above instead of the following line in original code
                //to provide guarantee of the persistence of the node in the tree
                //z.mKey = y.mKey;
            }

            if (y.mColor == RBTreeColor.Black && x != null)
                DeleteFix(x);
        }

        /// <summary>
        /// Restore the reb-black properties after a delete
        /// </summary>
        /// <param name="x"></param>
        protected void DeleteFix(RBTreeNodeBase<T, P> x)
        {
            RBTreeNodeBase<T, P> w;

            while (x != mRoot && x.mColor == RBTreeColor.Black)
            {
                if (x == x.mParent.mLeft)
                {
                    w = x.mParent.mRight;
                    if (w == null)
                    {
                        x = x.mParent;
                        continue;
                    }


                    if (w.mColor == RBTreeColor.Red)
                    {
                        w.mColor = RBTreeColor.Black;
                        x.mParent.mColor = RBTreeColor.Red;
                        LeftRotate(x.mParent);
                        w = x.mParent.mRight;
                    }

                    if (w == null)
                    {
                        x = x.mParent;
                        continue;
                    }

                    if ((w.mLeft == null || w.mLeft.mColor == RBTreeColor.Black) &&
                        (w.mRight == null || w.mRight.mColor == RBTreeColor.Black))
                    {
                        w.mColor = RBTreeColor.Red;
                        x = x.mParent;
                    }
                    else
                    {
                        if (w.mRight == null || w.mRight.mColor == RBTreeColor.Black)
                        {
                            if (w.mLeft != null)
                                w.mLeft.mColor = RBTreeColor.Black;
                            w.mColor = RBTreeColor.Red;
                            RightRotate(w);
                            w = x.mParent.mRight;
                        }

                        w.mColor = x.mParent.mColor;
                        x.mParent.mColor = RBTreeColor.Black;
                        if (w.mRight != null)
                            w.mRight.mColor = RBTreeColor.Black;
                        LeftRotate(x.mParent);
                        x = mRoot;
                    }
                }
                else
                {
                    w = x.mParent.mLeft;
                    if (w == null)
                    {
                        x = x.mParent;
                        continue;
                    }

                    if (w.mColor == RBTreeColor.Red)
                    {
                        w.mColor = RBTreeColor.Black;
                        x.mParent.mColor = RBTreeColor.Red;
                        RightRotate(x.mParent);
                        w = x.mParent.mLeft;
                    }

                    if (w == null)
                    {
                        x = x.mParent;
                        continue;
                    }

                    if ((w.mRight == null || w.mRight.mColor == RBTreeColor.Black) &&
                        (w.mLeft == null || w.mLeft.mColor == RBTreeColor.Black))
                    {
                        w.mColor = RBTreeColor.Red;
                        x = x.mParent;
                    }
                    else
                    {
                        if (w.mLeft == null || w.mLeft.mColor == RBTreeColor.Black)
                        {
                            if (w.mRight != null)
                                w.mRight.mColor = RBTreeColor.Black;
                            w.mColor = RBTreeColor.Red;
                            LeftRotate(w);
                            w = x.mParent.mLeft;
                        }

                        w.mColor = x.mParent.mColor;
                        x.mParent.mColor = RBTreeColor.Black;
                        if (w.mLeft != null)
                            w.mLeft.mColor = RBTreeColor.Black;
                        RightRotate(x.mParent);
                        x = mRoot;
                    }
                }
            }
            x.mColor = RBTreeColor.Black;
        }


        CollectionAdapter<T, N, P> mAdapter;

        ///<summary>
        ///Get collection object for this
        ///</summary>
        public IList<T> Collection
        {
            get
            {
                if (mAdapter == null)
                {
                    mAdapter = new CollectionAdapter<T, N, P>(this);
                }

                return mAdapter;
            }
        }

        #endregion

    }
}
