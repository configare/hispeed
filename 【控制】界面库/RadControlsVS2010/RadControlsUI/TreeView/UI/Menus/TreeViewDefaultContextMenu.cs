using System;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false)]
    public class TreeViewDefaultContextMenu : RadContextMenu
    {
        #region Fields

        private RadTreeViewElement treeView;
        private TreeViewMenuItem expandCollapseMenuItem;
        private TreeViewMenuItem editMenuItem;
        private TreeViewMenuItem addMenuItem;
        private TreeViewMenuItem deleteMenuItem;

        #endregion

        #region Constructor

        public TreeViewDefaultContextMenu(RadTreeViewElement treeView)
        {
            this.treeView = treeView;

            this.editMenuItem = new TreeViewMenuItem("Edit", "&Edit");
            this.Items.Add(editMenuItem);

            this.expandCollapseMenuItem = new TreeViewMenuItem("Expand", TreeViewLocalizationProvider.CurrentProvider.GetLocalizedString(TreeViewStringId.ContextMenuExpand));
            this.Items.Add(expandCollapseMenuItem);

            this.addMenuItem = new TreeViewMenuItem("Add", "&Add");
            this.Items.Add(addMenuItem);

            this.deleteMenuItem = new TreeViewMenuItem("Delete", "&Delete");
            this.Items.Add(deleteMenuItem);

            for (int i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].Click += menuItem_Click;
            }
        }

        #endregion

        #region Properties

        public TreeViewMenuItem ExpandCollapseMenuItem
        {
            get { return this.expandCollapseMenuItem; }
        }

        public TreeViewMenuItem EditMenuItem
        {
            get { return this.editMenuItem; }
        }

        public TreeViewMenuItem AddMenuItem
        {
            get { return this.addMenuItem; }
        }

        public TreeViewMenuItem DeleteMenuItem
        {
            get { return this.deleteMenuItem; }
        }

        #endregion

        #region Event Handlers

        protected override void OnDropDownOpening(CancelEventArgs args)
        {
            base.OnDropDownOpening(args);

            if (args.Cancel)
            {
                return;
            }

            this.editMenuItem.Text = TreeViewLocalizationProvider.CurrentProvider.GetLocalizedString(TreeViewStringId.ContextMenuEdit);
            this.addMenuItem.Text = TreeViewLocalizationProvider.CurrentProvider.GetLocalizedString(TreeViewStringId.ContextMenuNew);
            this.deleteMenuItem.Text = TreeViewLocalizationProvider.CurrentProvider.GetLocalizedString(TreeViewStringId.ContextMenuDelete);
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            TreeViewMenuItem menuItem = sender as TreeViewMenuItem;
            if (menuItem == null)
            {
                return;
            }

            switch (menuItem.Command)
            {
                case "Edit":
                    EditNode();
                    break;
                case "Expand":
                case "Collapse":
                    ExpandNode();
                    break;
                case "Add":
                    AddNode();
                    break;
                case "Delete":
                    DeleteNode();
                    break;
            }
        }

        #endregion

        #region Methods

        private void DeleteNode()
        {
            if (this.treeView.IsEditing || !this.treeView.AllowRemove)
            {
                return;
            }

            RadTreeNode node = this.treeView.SelectedNode;
            if (node == null)
            {
                return;
            }

            RadTreeNodeCollection nodes = this.treeView.Nodes;
            RadTreeNode parent = node.Parent;
            if (parent != null)
            {
                nodes = parent.Nodes;
            }

            nodes.Remove(node);
        }

        private void AddNode()
        {
            if (this.treeView.IsEditing || !this.treeView.AllowAdd)
            {
                return;
            }

            RadTreeNode node = this.treeView.SelectedNode;
            if (node == null)
            {
                return;
            }

            RadTreeNode newNode = new RadTreeNode("New Node");
            node.Nodes.Add(newNode);
            if (this.treeView != null)
            {
                node.Expanded = true;
                if (!this.treeView.ListSource.IsDataBound)
                {
                    this.treeView.SelectedNode = newNode;
                    this.treeView.UpdateLayout();
                    this.treeView.BeginEdit();
                }
            }
        }

        private void ExpandNode()
        {
            RadTreeNode node = this.treeView.SelectedNode;
            if (node != null)
            {
                if (node.Expanded)
                {
                    node.Collapse(true);
                }
                else
                {
                    node.Expand();
                }
            }
        }

        private void EditNode()
        {
            this.treeView.BeginEdit();
        }

        protected override void Dispose(bool disposing)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].Click -= menuItem_Click;
            }

            editMenuItem.Dispose();
            expandCollapseMenuItem.Dispose();
            addMenuItem.Dispose();
            deleteMenuItem.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}
