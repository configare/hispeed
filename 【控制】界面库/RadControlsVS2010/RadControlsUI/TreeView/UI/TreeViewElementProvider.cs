using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class TreeViewElementProvider : VirtualizedPanelElementProvider<RadTreeNode, TreeNodeElement>
    {
        #region Fields

        private RadTreeViewElement treeViewElement;

        #endregion

        #region Constructor

        public TreeViewElementProvider(RadTreeViewElement treeViewElement)
        {
            this.treeViewElement = treeViewElement;
        }

        #endregion

        #region Properties

        protected RadTreeViewElement TreeViewElement
        {
            get
            {
                return this.treeViewElement;
            }
        }

        #endregion

        #region Methods

        public override IVirtualizedElement<RadTreeNode> CreateElement(RadTreeNode data, object context)
        {
            CreateTreeNodeElementEventArgs args = new CreateTreeNodeElementEventArgs(data);
            treeViewElement.OnCreateNodeElement(args);

            if (args.NodeElement != null)
            {
                return args.NodeElement;
            }

            return base.CreateElement(data, context);
        }

        public override SizeF GetElementSize(RadTreeNode item)
        {
            return item.ActualSize;
        }

        #endregion
    }
}
