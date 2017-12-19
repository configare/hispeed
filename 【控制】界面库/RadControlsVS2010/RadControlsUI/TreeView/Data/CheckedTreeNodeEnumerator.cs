using System.Collections;
using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class CheckedTreeNodeEnumerator : IEnumerator<RadTreeNode>, IEnumerable<RadTreeNode>, IEnumerable
    {
        #region Fields

        private RadTreeNode rootNode = null;
        private RadTreeNode current = null;

        #endregion

        #region Initialization

        public CheckedTreeNodeEnumerator(RadTreeNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public void Dispose()
        {
            this.Reset();
            this.rootNode = null;
        }

        #endregion

        #region Properties

        object IEnumerator.Current
        {
            get { return this.current; }
        }


        public RadTreeNode Current
        {
            get { return this.current; }
        }

        #endregion

        #region Methods

        public bool MoveNext()
        {
            RadTreeNode pointedNode = current ?? rootNode;

            while (pointedNode != null)
            {
                if (pointedNode.Checked && pointedNode != current)
                {
                    this.current = pointedNode;
                    return true;
                }

                pointedNode = pointedNode.NextVisibleNode;
            }

            return false;
        }

        public void Reset()
        {
            this.current = null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CheckedTreeNodeEnumerator(this.rootNode);
        }

        public IEnumerator<RadTreeNode> GetEnumerator()
        {
            return new CheckedTreeNodeEnumerator(this.rootNode);
        }

        #endregion
    }
}
