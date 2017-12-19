using System;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewEventArgs : EventArgs
    {
        #region Fields

        protected RadTreeNode node;

        #endregion

        #region Constructor

        public RadTreeViewEventArgs(RadTreeNode node)
            : base()
        {
            this.node = node;
        }

        #endregion

        #region Properties

        public virtual RadTreeNode Node
        {
            get
            {
                return this.node;
            }
            set
            {
            }
        }

        public RadTreeViewElement TreeElement
        {
            get { return node.TreeViewElement; }
        }

        public RadTreeView TreeView
        {
            get { return node.TreeView; }
        }

        #endregion
    }
}
