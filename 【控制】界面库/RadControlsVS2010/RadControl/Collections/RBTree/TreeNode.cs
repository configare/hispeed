using System;

namespace Telerik.Collections.Generic
{
    ///<summary>
    ///Colour of the node
    ///</summary>
    public enum RBTreeColor
    {
        ///<summary>
        ///Red
        ///</summary>
        Red,
        ///<summary>
        ///Black
        ///</summary>
        Black
    }

    /// <summary>
    /// Node of the red-black tree
    /// </summary>
    /// <typeparam name="T">Key type</typeparam>
    /// <typeparam name="P">Node's parameter</typeparam>
    public class RBTreeNodeBase<T, P> : ITreeNode<T>
    {
        #region Node relatives
        ///<summary>
        ///Parent node
        ///</summary>
        public RBTreeNodeBase<T, P> Parent
        {
            get
            {
                return mParent;
            }
        }
        internal RBTreeNodeBase<T, P> mParent;

        ///<summary>
        ///Set parent node
        ///</summary>
        internal virtual void SetParent(RBTreeNodeBase<T, P> value)
        {
            mParent = value;
        }

        ///<summary>
        ///Left node
        ///</summary>
        public virtual RBTreeNodeBase<T, P> Left
        {
            get
            {
                return mLeft;
            }
        }
        internal RBTreeNodeBase<T, P> mLeft;

        ///<summary>
        ///Set left node
        ///</summary>
        internal virtual void SetLeft(RBTreeNodeBase<T, P> value)
        {
            mLeft = value;
        }

        ///<summary>
        ///Right node
        ///</summary>
        public virtual RBTreeNodeBase<T, P> Right
        {
            get
            {
                return mRight;
            }
        }
        internal RBTreeNodeBase<T, P> mRight;

        ///<summary>
        ///Set right node
        ///</summary>
        internal virtual void SetRight(RBTreeNodeBase<T, P> value)
        {
            mRight = value;
        }
        #endregion

        #region Common data
        ///<summary>
        ///Key value of the node
        ///</summary>
        public virtual T Key
        {
            get
            {
                return mKey;
            }
        }
        internal T mKey;

        ///<summary>
        ///Colour of the node
        ///</summary>
        public RBTreeColor Color
        {
            get
            {
                return mColor;
            }
        }
        internal RBTreeColor mColor;

        ///<summary>
        ///Update reference count
        ///</summary>
        internal virtual void OnUpdateCount()
        {
            return ;
        }

        #pragma warning disable 649
        ///<summary>
        ///Node parameters
        ///</summary>
        internal P mParam;
        #pragma warning restore 649
        #endregion

        ///<summary>
        ///Constructor
        ///</summary>
        public RBTreeNodeBase()
        {
        }

        ///<summary>
        ///Copy from other node
        ///</summary>
        internal virtual void CopyFrom(RBTreeNodeBase<T, P> z)
        {
            if (z.mLeft != null)
                z.mLeft.mParent = this;
            this.mLeft = z.mLeft;

            if (z.mRight != null)
                z.mRight.mParent = this;
            this.mRight = z.mRight;

            //2) replace z with this in the parent node
            if (z.mParent != null)
                if (z.mParent.mLeft == z)
                    z.mParent.SetLeft(this);
                else
                    z.mParent.SetRight(this);

            this.mColor = z.mColor;
            this.SetParent(z.mParent);
        }

    }
}
