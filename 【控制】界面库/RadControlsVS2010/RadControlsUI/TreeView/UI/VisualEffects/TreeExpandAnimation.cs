
using System.Collections.Generic;
namespace Telerik.WinControls.UI
{
    public abstract class TreeExpandAnimation
    {
        #region Fields

        private RadTreeViewElement treeViewElement;

        #endregion

        #region Constructor

        public TreeExpandAnimation(RadTreeViewElement treeViewElement)
        {
            this.treeViewElement = treeViewElement;
        }

        #endregion

        #region Properties

        public RadTreeViewElement TreeViewElement
        {
            get
            {
                return this.treeViewElement;
            }
        }

        #endregion

        #region Abstract Methods

        public abstract void Expand(RadTreeNode treeNode);
        public abstract void Collapse(RadTreeNode treeNode);

        #endregion

        #region Protected Methods

        protected void UpdateViewOnExpandChanged(RadTreeNode node)
        {
            treeViewElement.Update(RadTreeViewElement.UpdateActions.ExpandedChanged, node);
            treeViewElement.UpdateLayout();
        }

        protected List<TreeNodeElement> GetAssociatedNodes(RadTreeNode node)
        {
            List<TreeNodeElement> list = new List<TreeNodeElement>();

            RadElementCollection nodes = this.TreeViewElement.ViewElement.Children;

            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNodeElement child = nodes[i] as TreeNodeElement;
                if (this.IsHierarchyChild(node, child.Data))
                {
                    list.Add(child);
                }
            }

            return list;
        }

        private bool IsHierarchyChild(RadTreeNode node, RadTreeNode child)
        {
            RadTreeNode parent = child.Parent;

            while (parent != null)
            {
                if (parent == node)
                {
                    return true;
                }

                parent = parent.Parent;
            }

            return false;
        }

        #endregion
    }
}
