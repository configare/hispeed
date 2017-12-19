using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class CheckedTreeNodeCollection : IReadOnlyCollection<RadTreeNode>
    {
        #region Fields

        private CheckedTreeNodeEnumerator enumerator;

        #endregion

        #region Constructor

        public CheckedTreeNodeCollection(RadTreeNode rootNode)
        {
            this.enumerator = new CheckedTreeNodeEnumerator(rootNode);
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                int count = 0;

                this.enumerator.Reset();

                while (enumerator.MoveNext())
                {
                    count++;
                }

                return count;
            }
        }

        public RadTreeNode this[int index]
        {
            get { return this.GetNodeByIndex(index); }
        }

        #endregion

        #region Methods

        public bool Contains(RadTreeNode value)
        {
            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == value)
                {
                    return true;
                }
            }

            return false;

        }

        public void CopyTo(RadTreeNode[] array, int index)
        {
            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                array[index] = enumerator.Current;
                index++;
            }
        }

        public int IndexOf(RadTreeNode value)
        {
            int index = 0;
            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == value)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public IEnumerator<RadTreeNode> GetEnumerator()
        {
            return this.enumerator.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.enumerator.GetEnumerator();
        }

        protected virtual RadTreeNode GetNodeByIndex(int index)
        {
            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                if (index == 0)
                {
                    return enumerator.Current;
                }

                index--;
            }

            throw new IndexOutOfRangeException();
        }

        #endregion
    }
}
