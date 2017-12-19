using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI
{
    public class TreeNodeLinesContainer: StackLayoutElement
    {
        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchVertically = true; 
            this.FitToSizeMode = RadFitToSizeMode.FitToParentBounds;
            this.ElementSpacing = 0;
            this.Padding = new System.Windows.Forms.Padding(0);
            this.BorderWidth = 0;
        }
        
        #endregion

        #region Properties

        public TreeNodeElement NodeElement
        {
            get
            {
                return this.FindAncestor<TreeNodeElement>();
            }
        }

        #endregion

        #region Methods

        public virtual void Synchronize()
        {
            TreeNodeElement nodeElement = NodeElement;
            if (nodeElement == null)
            {
                return;
            }

            if (nodeElement.Data.Parent is RadTreeViewElement.RootTreeNode && !nodeElement.TreeViewElement.ShowRootLines)
            {
                this.Visibility = ElementVisibility.Collapsed;
                return;
            }
            else
            {
                this.Visibility = ElementVisibility.Visible;
            }
                
            int level = nodeElement.Data.Level;
            if (!nodeElement.TreeViewElement.FullLazyMode)
            {
                if (nodeElement.Data.Nodes.Count == 0)
                {
                    level++;
                }
                else if (!nodeElement.TreeViewElement.ShowExpandCollapse)
                {
                    level++;
                }
            }
           

            while (this.Children.Count > level)
            {
                this.Children.RemoveAt(this.Children.Count - 1);
            }

            while (this.Children.Count < level)
            {
                this.Children.Add(new TreeNodeLineElement(nodeElement));
            }

            if (this.Children.Count > 0)
            {
                UpdateLines();
            }
        }

        protected virtual void UpdateLines()
        {
            TreeNodeElement nodeElement = NodeElement;
            bool showLines = nodeElement.TreeViewElement.ShowLines;
            
            if (!showLines)
            {
                foreach (TreeNodeLineElement element in this.Children)
                {
                    element.Visibility = ElementVisibility.Hidden;
                }               
            }

           // bool collapseLastlevel = nodeElement.TreeViewElement.LazyMode ;
            if (this.Children.Count > 0 && !nodeElement.TreeViewElement.ShowExpandCollapse)
            {
                this.Children[this.Children.Count - 1].Visibility = ElementVisibility.Collapsed;
            }

            if (!showLines)
            {
                return;
            }

            int index = this.Children.Count - 1;
            RadTreeNode node = nodeElement.Data;

            if (node.Nodes.Count > 0)
            {
                node = node.Parent;
            }

            while (node != null && index >= 0)
            {
                RadTreeNode nextNode = node.NextNode;
                TreeNodeLineElement lineElement = (TreeNodeLineElement)this.Children[index];
                UpdateLine(lineElement, node, nextNode, nodeElement);
                node = node.Parent;
                index--;
            }
        }

        protected virtual void UpdateLine(TreeNodeLineElement lineElement, RadTreeNode node, RadTreeNode nextNode, TreeNodeElement lastNode)
        {
            if (this.Children[0] == lineElement && !lastNode.TreeViewElement.ShowRootLines)
            {
                lineElement.Visibility = ElementVisibility.Collapsed;
                return;
            }


            lineElement.Visibility = ElementVisibility.Visible;
            lineElement.ForeColor = lastNode.TreeViewElement.LineColor;
            lineElement.LineStyle = (DashStyle)lastNode.TreeViewElement.LineStyle;

            if (node == lastNode.Data)
            {
                if (node.Nodes.Count > 0)
                {
                    if (!lastNode.TreeViewElement.ShowExpandCollapse)
                    {
                        if (nextNode == null)
                        {
                            lineElement.Type = TreeNodeLineElement.LinkType.RightBottomAngleShape;
                        }
                        else
                        {
                            lineElement.Type = TreeNodeLineElement.LinkType.TShape;
                        }
                    }
                    else 
                    {
                        if (node.Parent != null && node.Parent.NextNode == null)
                        {
                            lineElement.Visibility = ElementVisibility.Hidden;
                        }
                        else
                        {
                            lineElement.Type = TreeNodeLineElement.LinkType.VerticalLine;
                        }
                    }
                }
                else if (nextNode == null)
                {
                    lineElement.Type = TreeNodeLineElement.LinkType.RightBottomAngleShape;
                }
                else
                {
                    if (node.Parent == null && node.PrevVisibleNode == null)
                    {
                        lineElement.Type = TreeNodeLineElement.LinkType.RightTopAngleShape;
                    }
                    else
                    {
                        lineElement.Type = (node.TreeViewElement.FullLazyMode) ? TreeNodeLineElement.LinkType.VerticalLine : TreeNodeLineElement.LinkType.TShape;
                    }
                }
            }
            else
            {
                if (nextNode == null)
                {
                    lineElement.Visibility = ElementVisibility.Hidden;
                }
                else if (node.Parent != null && node.Parent.NextNode == null && lineElement == this.Children[0])
                {
                    lineElement.Visibility = ElementVisibility.Hidden;
                }
                else
                {
                    lineElement.Type = TreeNodeLineElement.LinkType.VerticalLine;
                }
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);

            if (this.Children.Count > 0)
            {
                if (Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    desiredSize.Height--;
                }
                else
                {
                    desiredSize.Width--;
                }
            }

            return desiredSize;
        }

        #endregion
    }
}
