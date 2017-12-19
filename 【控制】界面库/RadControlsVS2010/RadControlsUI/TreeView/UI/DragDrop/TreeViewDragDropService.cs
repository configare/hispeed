using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class TreeViewDragDropService : RadDragDropService
    {
        #region Fields

        private RadTreeViewElement owner;
        private RadLayeredWindow dropHintWindow;
        private Color dropHintColor;
        private bool showDragHint;
        private bool showDropHint;
        private RadTreeNode draggedNode;

        #endregion

        #region Constructors

        public TreeViewDragDropService(RadTreeViewElement owner)
        {
            this.owner = owner;
            this.dropHintColor = Color.Empty;
            this.showDragHint = true;
            this.showDropHint = true;
        }

        #endregion

        #region Properties

        public Color DropHintColor
        {
            get
            {
                return this.dropHintColor;
            }
            set
            {
                this.dropHintColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show drop hint should be shown.
        /// </summary>
        /// <value><c>true</c> if [show drop hint]; otherwise, <c>false</c>.</value>
        public bool ShowDropHint
        {
            get
            {
                return this.showDropHint;
            }
            set
            {
                this.showDropHint = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether drag hint should be shown.
        /// </summary>
        /// <value><c>true</c> if [show drag hint]; otherwise, <c>false</c>.</value>
        public bool ShowDragHint
        {
            get
            {
                return this.showDragHint;
            }
            set
            {
                this.showDragHint = value;
            }
        }

        #endregion

        #region Overrides

        protected override void PerformStart()
        {
            base.PerformStart();

            TreeNodeElement draggedNodeElement = this.Context as TreeNodeElement;
            this.draggedNode = draggedNodeElement.Data;
        }

        protected override void OnPreviewDragHint(PreviewDragHintEventArgs e)
        {
            if (!this.ShowDragHint)
            {
                e.DragHint = null;
                e.UseDefaultHint = false;
            }

            base.OnPreviewDragHint(e);
        }

        protected override void OnPreviewDragDrop(RadDropEventArgs e)
        {
            bool isScrollerNode = this.owner.Scroller.Traverser.Current == draggedNode;
            RadTreeNode newScrollerNode = null;

            if (isScrollerNode)
            {
                newScrollerNode = draggedNode.NextNode;

                if (newScrollerNode == null)
                {
                    newScrollerNode = draggedNode.PrevNode;
                }
            }

            TreeNodeElement targetNodeElement = e.HitTarget as TreeNodeElement;
            RadTreeViewElement targetTreeView = targetNodeElement == null ? e.HitTarget as RadTreeViewElement : targetNodeElement.TreeViewElement;
            List<RadTreeNode> draggedNodes = this.GetDraggedNodes(draggedNode);

            bool dragDropBetweenTrees = targetTreeView != this.owner;

            targetTreeView.BeginUpdate();

            if (dragDropBetweenTrees)
            {
                this.owner.BeginUpdate();
            }

            if (targetNodeElement != null)
            {
                this.PerformDragDrop(e.DropLocation, targetNodeElement, draggedNodes);
            }
            else
            {
                foreach (RadTreeNode node in draggedNodes)
                {
                    if (!this.OnDragEnding(DropPosition.AsChildNode, node, null))
                    {
                        continue;
                    }

                    node.Remove();
                    targetTreeView.Nodes.Add(node);
                    this.owner.OnDragEnded(new RadTreeViewDragEventArgs(node, null));
                }
            }

            if (dragDropBetweenTrees)
            {
                this.owner.EndUpdate();
            }

            if (isScrollerNode)
            {
                this.owner.Scroller.ScrollToItem(newScrollerNode);
            }

            targetTreeView.EndUpdate();
            targetTreeView.SelectedNode = draggedNode;

            base.OnPreviewDragDrop(e);
        }

        protected DropPosition GetDropPosition(Point dropLocation, TreeNodeElement targetNodeElement)
        {
            int part = targetNodeElement.Size.Height / 3;
            int mouseY = dropLocation.Y;
            bool dropAtTop = mouseY < part;

            if (dropAtTop)
            {
                return DropPosition.BeforeNode;
            }

            if (mouseY >= part && mouseY <= part * 2)
            {
                return DropPosition.AsChildNode;
            }

            return DropPosition.AfterNode;
        }

        protected virtual void PerformDragDrop(Point dropLocation, TreeNodeElement targetNodeElement, List<RadTreeNode> draggedNodes)
        {
            RadTreeNode targetNode = targetNodeElement.Data;
            DropPosition position = this.GetDropPosition(dropLocation, targetNodeElement);

            if (position == DropPosition.AsChildNode)
            {
                foreach (RadTreeNode node in draggedNodes)
                {
                    if (!this.OnDragEnding(position, node, targetNode))
                    {
                        continue;
                    }

                    node.Remove();
                    targetNode.Nodes.Add(node);
                    this.owner.OnDragEnded(new RadTreeViewDragEventArgs(node, targetNode));
                }

                targetNode.Expand();
            }
            else
            {
                this.PerformDragDropCore(position, targetNode, draggedNodes);
            }
        }

        protected virtual void PerformDragDropCore(DropPosition position, RadTreeNode targetNode, List<RadTreeNode> draggedNodes)
        {
            RadTreeNode parent = targetNode.Parent;
            RadTreeNodeCollection collection = targetNode.TreeViewElement.Nodes;

            if (parent != null)
            {
                collection = parent.Nodes;
            }

            bool dropAtTop = position == DropPosition.BeforeNode;
            int index = collection.IndexOf(targetNode);

            if (!dropAtTop && index + 1 <= collection.Count)
            {
                index++;
            }

            foreach (RadTreeNode node in draggedNodes)
            {
                if (!this.OnDragEnding(position, node, targetNode))
                {
                    continue;
                }

                if (node.Parent == parent && node.TreeViewElement == targetNode.TreeViewElement)
                {
                    int nodeIndex = collection.IndexOf(node);

                    if (nodeIndex < index)
                    {
                        index--;
                    }
                }

                node.Remove();
                collection.Insert(index, node);
                index++;
                this.owner.OnDragEnded(new RadTreeViewDragEventArgs(node, targetNode));
            }
        }

        private bool OnDragEnding(DropPosition position, RadTreeNode node, RadTreeNode targetNode)
        {
            RadTreeViewDragCancelEventArgs args = new RadTreeViewDragCancelEventArgs(node, targetNode);
            args.DropPosition = position;
            this.owner.OnDragEnding(args);
            return !args.Cancel;
        }

        protected virtual List<RadTreeNode> GetDraggedNodes(RadTreeNode draggedNode)
        {
            SelectedTreeNodeCollection selectedNodes = draggedNode.TreeViewElement.SelectedNodes;
            List<RadTreeNode> result = new List<RadTreeNode>();
            bool isDraggable = true;

            foreach (RadTreeNode node in selectedNodes)
            {
                RadTreeNode parent = node.Parent;
                isDraggable = true;

                while (parent != null)
                {
                    if (parent.Selected)
                    {
                        isDraggable = false;
                        break;
                    }

                    parent = parent.Parent;
                }

                if (isDraggable)
                {
                    result.Add(node);
                }
            }

            if (result.Count == 0)
            {
                result.Add(draggedNode);
            }

            return result;
        }

        protected override bool PrepareContext()
        {
            TreeNodeElement nodeElement = this.Context as TreeNodeElement;

            if (nodeElement != null)
            {
                RadTreeViewDragCancelEventArgs args = new RadTreeViewDragCancelEventArgs(nodeElement.Data, null);
                this.owner.OnDragStarting(args);

                if (args.Cancel)
                {
                    return false;
                }
            }

            bool result = base.PrepareContext();

            if (nodeElement != null)
            {
                this.owner.OnDragStarted(new RadTreeViewDragEventArgs(nodeElement.Data, null));
            }

            return result;
        }

        protected override bool IsDropTargetValid(ISupportDrop dropTarget)
        {
            TreeNodeElement targetNodeElement = dropTarget as TreeNodeElement;

            if (targetNodeElement != null)
            {
                return targetNodeElement.Data != this.draggedNode;
            }

            return base.IsDropTargetValid(dropTarget);
        }

        protected override bool CanStart(object context)
        {
            return base.CanStart(context) && this.IsInValidState(this.owner) && owner.AllowDragDrop;
        }

        private bool IsInValidState(RadTreeViewElement tree)
        {
            return !tree.IsEditing && !tree.ListSource.IsDataBound &&
                   tree.FilterDescriptors.Count == 0 && tree.SortDescriptors.Count == 0;
        }

        protected override void OnPreviewDragOver(RadDragOverEventArgs e)
        {
            base.OnPreviewDragOver(e);

            TreeNodeElement targetNodeElement = e.HitTarget as TreeNodeElement;
            RadTreeViewElement treeView = e.HitTarget as RadTreeViewElement;

            if (treeView != null)
            {
                e.CanDrop = treeView.ElementTree.Control.AllowDrop && treeView.Nodes.Count == 0 && this.IsInValidState(treeView);
            }
            else if (targetNodeElement != null)
            {
                DropPosition dropPosition = this.GetDropPosition(this.DropLocation, targetNodeElement);
                e.CanDrop = this.CanDragOver(dropPosition, targetNodeElement);

                RadTreeViewDragCancelEventArgs args = new RadTreeViewDragCancelEventArgs(draggedNode, targetNodeElement.Data);
                args.Cancel = !e.CanDrop;
                args.DropPosition = dropPosition;

                this.owner.OnDragOverNode(args);
                e.CanDrop = !args.Cancel;
            }
        }

        protected virtual bool CanDragOver(DropPosition dropPosition, TreeNodeElement targetNodeElement)
        {
            RadTreeViewElement targetTreeView = targetNodeElement.TreeViewElement;

            if (!targetNodeElement.Enabled || !targetTreeView.ElementTree.Control.AllowDrop || !this.IsInValidState(targetTreeView))
            {
                return false;
            }

            RadTreeNode targetNode = targetNodeElement.Data;
            List<RadTreeNode> nodes = new List<RadTreeNode>(draggedNode.TreeViewElement.SelectedNodes);

            // If the count is empty, we are in single selection mode
            if (nodes.Count == 0)
            {
                nodes.Add(draggedNode);
            }

            foreach (RadTreeNode selected in nodes)
            {
                if (selected == targetNode)
                {
                    return false;
                }

                RadTreeNode parent = targetNode.Parent;

                while (parent != null)
                {
                    if (parent == selected)
                    {
                        return false;
                    }

                    parent = parent.Parent;
                }

                if (dropPosition == DropPosition.AsChildNode && selected.Parent == targetNode)
                {
                    return false;
                }
            }

            if (dropPosition == DropPosition.AsChildNode)
            {
                targetTreeView.AutoExpand(targetNode);
            }

            targetTreeView.AutoScroll(targetNodeElement);
            return true;
        }

        protected override void HandleMouseMove(Point mousePosition)
        {
            base.HandleMouseMove(mousePosition);

            if (this.Initialized)
            {
                TreeNodeElement draggedNodeElement = this.Context as TreeNodeElement;

                if (draggedNodeElement != null)
                {
                    this.owner.OnItemDrag(new RadTreeViewEventArgs(draggedNodeElement.Data));
                }
            }

            ISupportDrop dropTarget = this.DropTarget;
            TreeNodeElement nodeElement = dropTarget as TreeNodeElement;

            if (nodeElement == null || !this.CanShowDropHint(mousePosition) || !this.CanCommit)
            {
                this.DisposeHint();
                return;
            }

            if (this.dropHintWindow == null)
            {
                this.PrepareDragHint(nodeElement);
            }

            if (this.dropHintWindow != null)
            {
                this.UpdateHintPosition(mousePosition);
            }
        }

        protected override void OnStopping(RadServiceStoppingEventArgs e)
        {
            base.OnStopping(e);

            if (!e.Commit)
            {
                this.owner.draggedNode = null;
            }
        }

        protected override void PerformStop()
        {
            base.PerformStop();
            this.DisposeHint();
            this.draggedNode = null;
        }

        protected virtual void DisposeHint()
        {
            if (this.dropHintWindow != null)
            {
                this.dropHintWindow.Dispose();
                this.dropHintWindow = null;
            }
        }

        #endregion

        #region Implementation

        protected virtual bool CanShowDropHint(Point mousePosition)
        {
            TreeNodeElement nodeElement = this.DropTarget as TreeNodeElement;

            if (nodeElement == null || !this.ShowDropHint)
            {
                return false;
            }

            Rectangle itemBounds = owner.ElementTree.Control.RectangleToScreen(nodeElement.ControlBoundingRectangle);
            Point client = nodeElement.PointFromScreen(mousePosition);
            int part = nodeElement.Size.Height / 3;
            return client.Y < part || client.Y > part * 2;
        }

        protected virtual void PrepareDragHint(TreeNodeElement nodeElement)
        {
            RadTreeViewElement treeView = nodeElement.TreeViewElement;
            Bitmap image = null;
            Size hintSize = Size.Empty;

            if (this.dropHintColor != Color.Empty)
            {
                hintSize = new Size(this.GetDropHintWidth(treeView), 1);
                image = new Bitmap(hintSize.Width, hintSize.Height);

                using (Graphics temp = Graphics.FromImage(image))
                {
                    using (SolidBrush solidBrush = new SolidBrush(this.DropHintColor))
                    {
                        temp.FillRectangle(solidBrush, new Rectangle(Point.Empty, hintSize));
                    }
                }
            }
            else if (treeView.ItemDropHint != null)
            {
                hintSize = new Size(this.GetDropHintWidth(treeView), treeView.ItemDropHint.Image.Size.Height);
                image = new Bitmap(hintSize.Width, hintSize.Height);

                using (Graphics temp = Graphics.FromImage(image))
                {
                    treeView.ItemDropHint.Paint(temp, new RectangleF(PointF.Empty, hintSize));
                }
            }

            if (image != null)
            {
                this.dropHintWindow = new RadLayeredWindow();
                this.dropHintWindow.BackgroundImage = image;
            }
        }

        private int GetDropHintWidth(RadTreeViewElement treeView)
        {
            int width = treeView.ControlBoundingRectangle.Width - treeView.GetBorderThickness(true).Horizontal;

            if (treeView.VScrollBar.Visibility == ElementVisibility.Visible)
            {
                width -= treeView.VScrollBar.Size.Width;
            }

            return width;
        }

        protected virtual void UpdateHintPosition(Point mousePosition)
        {
            TreeNodeElement nodeElement = this.DropTarget as TreeNodeElement;
            Rectangle itemBounds = nodeElement.ElementTree.Control.RectangleToScreen(nodeElement.ControlBoundingRectangle);
            Padding margins = Padding.Empty;
            int imageHeight = 1;

            if (this.DropHintColor == Color.Empty)
            {
                RadImageShape imageShape = this.owner.ItemDropHint;
                imageHeight = imageShape.Image.Size.Height;
                margins = imageShape.Margins;
            }


            Point client = nodeElement.PointFromScreen(mousePosition);
            bool isDropAtTop = client.Y <= nodeElement.Size.Height / 2;
            int y = isDropAtTop ? itemBounds.Y : itemBounds.Bottom;
            Point hitLocation = new Point(itemBounds.X - margins.Left, y - imageHeight / 2);
            this.dropHintWindow.ShowWindow(hitLocation);
        }

        #endregion
    }
}
