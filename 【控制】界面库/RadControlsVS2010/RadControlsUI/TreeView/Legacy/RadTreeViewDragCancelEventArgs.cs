using System;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewDragCancelEventArgs : RadTreeViewCancelEventArgs
    {
        #region Fields

        private RadTreeNode targetNode;
        private ArrowDirection direction = ArrowDirection.Left;
        private DropPosition dropPosition = DropPosition.None;

        #endregion

        #region Constructors

        public RadTreeViewDragCancelEventArgs(RadTreeNode node, RadTreeNode targetNode) :
            this(node, targetNode, ArrowDirection.Left, false)
        {

        }

        public RadTreeViewDragCancelEventArgs(RadTreeNode node, RadTreeNode targetNode, ArrowDirection direction, bool cancel)
            : base(node, cancel)
        {
            this.targetNode = targetNode;
            this.direction = direction;
        }

        #endregion

        #region Properties

        public RadTreeNode TargetNode
        {
            get { return this.targetNode; }
        }

        [Obsolete("This property will be removed in the next version. Please use the DropPosition property instead.")]
        public ArrowDirection Direction
        {
            get { return this.direction; }
        }


        public DropPosition DropPosition
        {
            get { return this.dropPosition; }
            protected internal set { this.dropPosition = value; }
        }

        #endregion
    }
}
