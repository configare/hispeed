using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public partial class ContextulTabGroupsEditor : Form
    {
        private RadItemCollection collection = new RadItemCollection();
        private RadItemCollection ribbonBarTabs;

        public RadItemCollection Collection
        {
            get { return this.collection; }
            set { this.collection = value; }
        }

        public ContextulTabGroupsEditor(RadItemCollection collection, RadRibbonBar parentRibbon)
        {
            InitializeComponent();
            this.InitCollection(collection);
            this.FillListBoxeses(parentRibbon);
        }

        private void InitCollection(RadItemCollection collection)
        {
            this.collection.Clear();
            foreach (RadItem item in collection)
            {
                this.collection.Add(item);
            }
        }

        private void FillListBoxeses(RadRibbonBar parentRibbon)
        {
            this.FillAssociatedTabs();
            this.FillAvaibleTabs(parentRibbon);
        }

        private void FillAvaibleTabs(RadRibbonBar parentRibbon)
        {
            RadPageViewItem item;
            this.ribbonBarTabs = new RadItemCollection();
            foreach (RadItem tabItem in parentRibbon.RibbonBarElement.TabStripElement.Items)
            {
                this.ribbonBarTabs.Add(tabItem);
            }

            for (int i = 0; i < ribbonBarTabs.Count; ++i)
            {
                item = (RadPageViewItem)ribbonBarTabs[i];

                if (this.ContextualGroupsNotContainsThisTab(parentRibbon, item) && !(bool)item.GetValue(RadItem.IsAddNewItemProperty))
                {
                    RadListBoxItem listItem = new RadListBoxItem(item.Text);
                    listItem.Tag = item;
                    this.radListBoxAvaibleTabs.Items.Add(listItem);
                }
            }
        }

        private bool ContextualGroupsNotContainsThisTab(RadRibbonBar parentRibbon, RadPageViewItem tabItem)
        {
            foreach (ContextualTabGroup group in parentRibbon.ContextualTabGroups)
            {
                if (group.TabItems.Contains(tabItem))
                {
                    return false;
                }
            }
            return true;
        }

        private void FillAssociatedTabs()
        {
            RadItem item;
            for (int i = 0; i < this.collection.Count; ++i)
            {
                item = this.collection[i];
                RadListBoxItem listItem = new RadListBoxItem(item.Text);
                listItem.Tag = item;
                this.radListBoxAssociatedTabs.Items.Add(listItem);
            }
        }

        private void radButtonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void radButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radButtonAddOne_Click(object sender, EventArgs e)
        {
            this.MoveItems(this.radListBoxAvaibleTabs, this.ribbonBarTabs, this.radListBoxAssociatedTabs, this.collection, 1);
        }

        private void radButtonAddAll_Click(object sender, EventArgs e)
        {
            this.MoveItems(this.radListBoxAvaibleTabs, this.ribbonBarTabs, this.radListBoxAssociatedTabs, this.collection,
                           this.radListBoxAvaibleTabs.Items.Count);
        }

        private void radButtonRemoveAll_Click(object sender, EventArgs e)
        {
            this.MoveItems(this.radListBoxAssociatedTabs, this.collection, this.radListBoxAvaibleTabs, this.ribbonBarTabs,
                           this.radListBoxAssociatedTabs.Items.Count);
        }

        private void radButtonRemoveOne_Click(object sender, EventArgs e)
        {
            this.MoveItems(this.radListBoxAssociatedTabs, this.collection, this.radListBoxAvaibleTabs, this.ribbonBarTabs, 1);
        }

        private void MoveItems(RadListBox left,
                               RadItemCollection leftCollection,
                               RadListBox right,
                               RadItemCollection rigthCollection,
                               int countItemsToMove)
        {
            if (left.Items.Count == 0)
                return;

            if (left.SelectedItems.Count == 0)
            {
                for (int i = 0; i < countItemsToMove; ++i)
                {
                    RadItem itemToRemove = left.Items[0];
                    left.Items.Remove(itemToRemove);
                    right.Items.Add(itemToRemove);

                    itemToRemove = (RadItem)itemToRemove.Tag;
                    leftCollection.Remove(itemToRemove);
                    if (!rigthCollection.Contains(itemToRemove))
                    {
                        rigthCollection.Add(itemToRemove);
                    }
                }
            }
            else
            {
                for (int i = 0; i < left.SelectedItems.Count; ++i)
                {
                    RadItem itemToRemove = left.SelectedItems[i];
                    left.Items.Remove(itemToRemove);
                    right.Items.Add(itemToRemove);

                    itemToRemove = (RadItem)itemToRemove.Tag;
                    leftCollection.Remove(itemToRemove);
                    if (!rigthCollection.Contains(itemToRemove))
                    {
                        rigthCollection.Add(itemToRemove);
                    }
                }
            }
        }
    }
}