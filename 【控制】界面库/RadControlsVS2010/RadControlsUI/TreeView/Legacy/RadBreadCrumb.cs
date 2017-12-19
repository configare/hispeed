using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using System.Drawing.Design;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.UI;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(true)]
    [RadThemeDesignerData(typeof(RadBreadCrumbDesignTimeData))]
    [Designer(DesignerConsts.RadBreadCrumbDesignerString)]
    public class RadBreadCrumb : RadControl
    {
        private RadTreeView defaultTreeView;
        private RadBreadCrumbElement breadCrumbElement;

        public RadBreadCrumb()
        {
            this.ThemeName = "Office2007Black";
        }

        protected override bool GetUseNewLayout()
        {
            return false;
        }

        [Category("Behavior")]
        [Description("Associates a TreeView to the btead crumb control")]
        [DefaultValue(null)]
        public RadTreeView DefaultTreeView
        {
            get
            {
                return this.defaultTreeView;
            }

            set
            {
                this.defaultTreeView = value;
                if (value != null)
                {
                    value.SelectedNodeChanged += value_Selected;
                    value.NodeExpandedChanged += new RadTreeView.TreeViewEventHandler(value_NodeExpand);
                }
            }
        }

        private void value_NodeExpand(object sender, RadTreeViewEventArgs tvea)
        {
            if (tvea.Node.Selected)
            {
                this.UpdateBreadCrumb(tvea.Node);
            }
        }


        private class AssociatedMenuItem : RadMenuItem
        {
            private RadTreeNode associatedNode;

            public AssociatedMenuItem(RadTreeNode associatedNode)
            {
                this.associatedNode = associatedNode;
            }

            protected override Type ThemeEffectiveType
            {
                get
                {
                    return typeof(RadMenuItem);
                }
            }

            public RadTreeNode AssociatedNode
            {
                get
                {
                    return this.associatedNode;
                }
            }
        }

        public int GetNodesCount(RadTreeNodeCollection nodes)
        {
            foreach (RadTreeNode node in nodes)
            {
                return nodes.Count + GetNodesCount(node.Nodes);
            }

            return nodes.Count;
        }

        public void UpdateBreadCrumb(RadTreeNode node)
        {
            this.ClearCurrentItems();
            Stack<RadSplitButtonElement> stack = new Stack<RadSplitButtonElement>();

            RadTreeNode lastNode = null;

            Image firstNodeImage = node.Image;
            // gets the image of the selected node

            while (node != null)
            {
                RadSplitButtonElement ddButton = new RadSplitButtonElement();

                ddButton.Text = node.Text;
                ddButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;

                // split button without children shouldn't have an arrow next to it
                if (node.Nodes.Count == 0)
                {
                    ddButton.ShowArrow = false;
                }

                this.SetDropDownItems(node, lastNode, ddButton);

                ddButton.Text = node.Text;
                stack.Push(ddButton);

                lastNode = node;

                node = node.Parent;
            }
            AddBreadCrumbsChildren(stack, firstNodeImage);
        }

        private void ClearCurrentItems()
        {
            while (this.breadCrumbElement.Items.Count > 0)
            {
                this.breadCrumbElement.Items[0].Dispose();
            }
        }

        private void AddBreadCrumbsChildren(Stack<RadSplitButtonElement> stack, Image firstNodeImage)
        {
            Size textSize = Size.Empty;
            Graphics graphics = this.CreateGraphics();
            RadSplitButtonElement currentSplitButton = null;
            bool anyPoppedElements = false;

            while (stack.Count > 0)
            {
                currentSplitButton = stack.Pop();

                if (!anyPoppedElements)
                {
                    textSize = Size.Ceiling(graphics.MeasureString(currentSplitButton.Text,
                                                                   currentSplitButton.Font));

                    if (!textSize.IsEmpty && firstNodeImage != null)
                    {
                        // sets the selected node's image next to the first split button
                        currentSplitButton.Image = new Bitmap(firstNodeImage, new Size(
                            Math.Min(textSize.Height, firstNodeImage.Width),
                            Math.Min(textSize.Height, firstNodeImage.Width)
                            ));
                    }

                    anyPoppedElements = true;
                }

                // subscribes each splitbutton to the following events 
                currentSplitButton.DropDownOpened += new EventHandler(currentSplitButton_DropDownOpened);
                currentSplitButton.DropDownClosed += new EventHandler(currentSplitButton_DropDownClosed);
                currentSplitButton.MouseEnter += new EventHandler(currentSplitButton_MouseEnter);

                this.breadCrumbElement.Items.Add(currentSplitButton);
            }

            graphics.Dispose();
        }



        private bool dropDownOpened;
        private RadSplitButtonElement currentExpandedSplitButton;

        private void currentSplitButton_MouseEnter(object sender, EventArgs e)
        {
            if (this.dropDownOpened)
            {
                RadSplitButtonElement splitButton = sender as RadSplitButtonElement;
                if (splitButton != null)
                {
                    // if the current split button is the sender doesn't do anything
                    // else if another split button is the sender then hides 
                    // the current drop down and openes the sender's one
                    if (splitButton == this.currentExpandedSplitButton)
                        return;

                    if (this.currentExpandedSplitButton != null)
                    {
                        this.currentExpandedSplitButton.HideDropDown();
                    }

                    this.currentExpandedSplitButton = splitButton;

                    if (!splitButton.IsDropDownShown)
                    {
                        splitButton.ShowDropDown();
                    }
                }
            }
        }

        private void currentSplitButton_DropDownClosed(object sender, EventArgs e)
        {
            // checks if all drop downs are closed  and sets the dropDownOpened flag 
            int count = 0;

            foreach (RadSplitButtonElement splitButton in this.breadCrumbElement.Items)
            {
                if (!splitButton.IsDropDownShown)
                    count++;
            }

            if (count == this.breadCrumbElement.Items.Count)
                this.dropDownOpened = false;
        }

        private void currentSplitButton_DropDownOpened(object sender, EventArgs e)
        {
            // sets the drop down opened flag
            this.dropDownOpened = true;
        }


        private void SetDropDownItems(RadTreeNode node, RadTreeNode lastNode, RadSplitButtonElement ddButton)
        {
            // adds and initializes the drop down items of ddButton instance of RadSplitButtonElement
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                AssociatedMenuItem item = new AssociatedMenuItem(node.Nodes[i]);
                item.Click += new EventHandler(item_Click);

                if (lastNode == node.Nodes[i])
                {
                    item.Font = new System.Drawing.Font(item.Font, System.Drawing.FontStyle.Bold);

                }

                item.Image = node.Nodes[i].Image;

                item.Text = node.Nodes[i].Text;
                ddButton.Items.Add(item);
            }
        }

        private void item_Click(object sender, EventArgs e)
        {
            AssociatedMenuItem assocItem = sender as AssociatedMenuItem;

            RadTreeNode node = assocItem.AssociatedNode;
            while (node != null)
            {
                node.Expand();
                node = node.Parent;
            }

            this.defaultTreeView.SelectedNode = assocItem.AssociatedNode;
            this.defaultTreeView.BringIntoView(assocItem.AssociatedNode);
        }

        private void value_Selected(object sender, RadTreeViewEventArgs e)
        {
            UpdateBreadCrumb(this.defaultTreeView.SelectedNode);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.breadCrumbElement = new RadBreadCrumbElement();

            this.RootElement.Children.Add(this.breadCrumbElement);
        }
    }
}
