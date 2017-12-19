using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Layouts;
using System.Collections;

namespace Telerik.WinControls.UI.TabStrip
{
    public enum OverFlowBehavior
    {
        None,
        BringIntoView,
        VisualStudioStyle,
    }

    public class ItemsOverflowDropDown
    {
        private RadItemOwnerCollection items;
        private RadTabStripElement tabStripElement;
        private RadDropDownButtonElement dropDownButton;

        public RadDropDownButtonElement DropDownButton
        {
            get { return dropDownButton; }
        }

        private void SetItemsEvents()
        {
            TabItem item = null;

            for (int i = 0; i < this.items.Count; i++)
            {
                item = this.items[i] as TabItem;
                if (item != null)
                {
                    item.VisibilityChanged -= new EventHandler(item_VisibilityChanged);
                    item.VisibilityChanged += new EventHandler(item_VisibilityChanged);
                }
            }
        }

        private void item_VisibilityChanged(object sender, EventArgs e)
        {
            PrepareDropDownItems();
        }

        public void InitializeOverflowDropDown(RadTabStripElement tabStripElement, LayoutPanel parent)
        {
            this.items = tabStripElement.Items;
            this.tabStripElement = tabStripElement;
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);


            this.dropDownButton = new RadDropDownButtonElement();
            dropDownButton.SetValue(TabLayoutPanel.IsDropDownButtonProperty, true);
            this.DropDownButton.DropDownOpening += new CancelEventHandler(button_DropDownShowing);
            this.DropDownButton.Text = "";
            this.DropDownButton.ZIndex = 9000;

            CollapseActionButtonBorder();
            this.DropDownButton.MouseDown += new MouseEventHandler(DropDownButton_MouseDown);

            this.dropDownButton.MinSize = new Size(20, 20);
            this.dropDownButton.ArrowButton.MinSize = new Size(20, 20);
            this.dropDownButton.ArrowButton.Arrow.AutoSize = true;
            this.dropDownButton.Visibility = ElementVisibility.Hidden;
            parent.Children.Add(this.dropDownButton);

            if (parent as TabLayoutPanel != null)
            {
                (parent as TabLayoutPanel).DropDownButton = this.dropDownButton;
            }
        }

        internal void Uninitialize()
        {
            this.items.ItemsChanged -= new ItemChangedDelegate(items_ItemsChanged);
            foreach (RadElement item in this.items)
            {
                TabItem tabItem = item as TabItem;
                if (tabItem != null)
                {
                    tabItem.VisibilityChanged -= item_VisibilityChanged;
                }
            }
        }

        public void SelectTabItem(TabItem item)
        {
            int index = this.tabStripElement.Items.IndexOf(item);
            if (index == -1)
            {
                return;
            }

            if (item.Visibility == ElementVisibility.Hidden)
            {
                item.Visibility = ElementVisibility.Visible;
                this.tabStripElement.Items.Remove(item);
                this.tabStripElement.Items.Insert(0, item);
                this.tabStripElement.SelectedTab = item;
                this.tabStripElement.InvalidateMeasure();
                this.tabStripElement.InvalidateArrange();
                this.tabStripElement.UpdateLayout();
            }
            else
            {
                this.tabStripElement.SelectedTab = item;
            }
        }

        public void HideOuterBoundsItems()
        {
            if (!this.tabStripElement.ShowOverFlowButton)
            {
                return;
            }

            Rectangle tabStripScreenBounds = this.tabStripElement.RectangleToScreen(this.tabStripElement.Bounds);
            if (this.tabStripElement.TabsPosition == TabPositions.Top || this.tabStripElement.TabsPosition == TabPositions.Bottom)
            {
                foreach (TabItem item in this.tabStripElement.Items)
                {
                    Rectangle tabItemScreenBounds = item.RectangleToScreen(item.Bounds);
                    if (tabItemScreenBounds.Right > tabStripScreenBounds.Right)
                    {
                        item.Visibility = ElementVisibility.Hidden;
                    }
                    else
                    {
                        item.Visibility = ElementVisibility.Visible;
                    }
                }
            }
            else
            {
                foreach (TabItem item in this.tabStripElement.Items)
                {
                    Rectangle tabItemScreenBounds = item.RectangleToScreen(item.Bounds);
                    if (tabItemScreenBounds.Bottom > tabStripScreenBounds.Bottom)
                    {
                        item.Visibility = ElementVisibility.Hidden;
                    }
                    else
                    {
                        item.Visibility = ElementVisibility.Visible;
                    }
                }
            }
        }


        private void DropDownButton_MouseDown(object sender, MouseEventArgs e)
        {
            this.PrepareDropDownItems();
        }

        private void CollapseActionButtonBorder()
        {
            RadElement element;
            for (int i = 0; i < DropDownButton.ActionButton.Children.Count; i++)
            {
                if ((DropDownButton.ActionButton.Children[i] as Primitives.BorderPrimitive) != null)
                {

                    element = DropDownButton.ActionButton.Children[i];
                    element.Visibility = ElementVisibility.Collapsed;
                    break;
                }
            }
        }

        private void button_DropDownShowing(object sender, EventArgs e)
        {
            //		System.Windows.Forms.MessageBox.Show("COOL");

            //	PrepareDropDownItems();
        }

        private void items_ItemsChanged(RadItemCollection changed, RadItem tartet, ItemsChangeOperation operation)
        {
            SetItemsEvents();
        }

        public void PrepareDropDownItems()
        {

            DropDownButton.Items.Clear();

            foreach (RadItem item in this.items)
            {
                OverflowDropDownMenuItem corresponding = new OverflowDropDownMenuItem(item);
                corresponding.AssociatedItem = item;
                corresponding.Click += new EventHandler(corresponding_Click);
                DropDownButton.Items.Add(corresponding);

            }
        }

        internal class OverflowDropDownMenuItemComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                OverflowDropDownMenuItem item1 = (OverflowDropDownMenuItem)x;
                OverflowDropDownMenuItem item2 = (OverflowDropDownMenuItem)y;

                return item1.AssociatedItem.Text.CompareTo(item2.AssociatedItem.Text);
            }
        }



        internal OverFlowBehavior overFlowBehavior = OverFlowBehavior.BringIntoView;


        private void corresponding_Click(object sender, EventArgs e)
        {
            OverflowDropDownMenuItem dropDownItem = (OverflowDropDownMenuItem)sender;
            this.DropDownButton.HideDropDown();
            //Ticket ID: 296681
            //This mimics the behavior of the TabStrip when clicking on
            //item to select it. The item focuses its container control in this case.
            //Without this we do not behave the same way when selecting an item over the drop down,
            //and selecting an item by clicking on it.
            dropDownItem.AssociatedItem.SetFocus();

            this.tabStripElement.SetSelectedTab(dropDownItem.AssociatedItem);

            if (overFlowBehavior != OverFlowBehavior.VisualStudioStyle)
            {
                this.tabStripElement.ScrollIntoView((TabItem)dropDownItem.AssociatedItem);
                return;
            }
            else
            {
                if (this.tabStripElement.UseNewLayoutSystem)
                {
                    TabItem tabItem = dropDownItem.AssociatedItem as TabItem;
                    SelectTabItem(tabItem);
                }
                else
                {
                    if ((this.tabStripElement.TabsPosition == TabPositions.Top) || (this.tabStripElement.TabsPosition == TabPositions.Bottom))
                    {
                        if (dropDownItem.AssociatedItem.BoundingRectangle.Right >= this.tabStripElement.Parent.Bounds.Right)
                        {
                            RearrangeItems(dropDownItem.AssociatedItem as TabItem);
                        }
                    }
                    else
                    {
                        if (dropDownItem.AssociatedItem.BoundingRectangle.Y + dropDownItem.AssociatedItem.FullSize.Width
                            >= this.tabStripElement.Parent.Bounds.Bottom)
                        {
                            RearrangeItems(dropDownItem.AssociatedItem as TabItem);
                        }
                    }
                }
            }
        }

        private void RearrangeItems(TabItem itemToMove)
        {
            TabItem item = itemToMove;

            if (!this.DropDownButton.UseNewLayoutSystem)
            {
                this.tabStripElement.Items.Remove(itemToMove);
                this.tabStripElement.Items.Insert(0, item);
            }
            else
            {
                TabItem replacedItem = (TabItem)this.tabStripElement.BoxLayout.Children[0];
                int index2 = this.tabStripElement.BoxLayout.Children.IndexOf(replacedItem);

                if (index2 != -1)
                {
                    int index = this.tabStripElement.BoxLayout.Children.IndexOf(itemToMove);
                    this.tabStripElement.BoxLayout.Children.SwitchItems(index, index2);
                    this.tabStripElement.BoxLayout.PerformLayout();

                }
            }
        }

        internal class OverflowDropDownMenuItem : RadMenuItem
        {
            private RadItem associatedItem;

            public OverflowDropDownMenuItem(RadItem associatedItem)
            {
                this.associatedItem = associatedItem;
                this.Text = associatedItem.Text;
            }

            protected override Type ThemeEffectiveType
            {
                get
                {
                    return typeof(RadMenuItem);
                }
            }

            public RadItem AssociatedItem
            {
                get
                {
                    return this.associatedItem;
                }
                set
                {
                    this.associatedItem = value;
                }
            }
        }


    }
}
