using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.WinControls.UI
{
    public class SelectedTreeNodeCollection : ReadOnlyCollection<RadTreeNode>
    {
        #region Fields

        private RadTreeViewElement treeView;

        #endregion

        #region Constructor

        public SelectedTreeNodeCollection(RadTreeViewElement treeView)
            : base(new List<RadTreeNode>())
        {
            this.treeView = treeView;
        }

        #endregion

        #region Methods

        internal void ProcessSelectedNode(RadTreeNode radTreeNode)
        {
            if (radTreeNode.Selected)
            {
                this.Items.Add(radTreeNode);
            }
            else
            {
                this.Items.Remove(radTreeNode);
            }

            treeView.Update(Telerik.WinControls.UI.RadTreeViewElement.UpdateActions.StateChanged, radTreeNode);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            if (this.Items.Count == 0 || !this.treeView.MultiSelect)
            {
                return;
            }

            treeView.BeginUpdate();

            while (this.Items.Count > 0)
            {
                RadTreeNode node = this.Items[0];
                node.Selected = false;
            }

            treeView.EndUpdate(true, RadTreeViewElement.UpdateActions.StateChanged);
        }

        #endregion
    }
}
