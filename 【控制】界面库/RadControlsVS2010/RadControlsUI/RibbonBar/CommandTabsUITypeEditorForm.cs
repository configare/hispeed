using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace Telerik.WinControls.UI.RibbonBar
{
    public partial class CommandTabsUITypeEditorForm : Form
    {
        #region Fields

        private RadItemCollection collection;
        private RadItemCollection initialCollection;
        private RadItemCollection newlyAddedItems;
        private IDesignerHost host;

        #endregion

        #region Constructor

        public CommandTabsUITypeEditorForm(IDesignerHost host, RadItemCollection value)
        {
            InitializeComponent();
            this.host = host;
            this.initialCollection = new RadItemCollection(value);
            this.newlyAddedItems = new RadItemCollection();
            this.collection = value;
            this.radListBoxTabItems.SelectedIndexChanged += new EventHandler( radListBoxTabItems_SelectedIndexChanged);
            this.radListBoxTabItems.SelectionMode = SelectionMode.MultiExtended;
            this.LoadListBoxItems();
        }

       

        #endregion

        #region Methods

        private void LoadListBoxItems()
        {
            for (int i = 0; i < this.collection.Count; i++)
            {
                RibbonTab newTab = this.collection[i] as RibbonTab;

                if (newTab != null && !((bool)newTab.GetValue(RadItem.IsAddNewItemProperty)))
                {
                    RadListBoxItem listBoxItem = new RadListBoxItem();
                    listBoxItem.Text = newTab.Text;
                    listBoxItem.Tag = newTab;
                    this.radListBoxTabItems.Items.Insert(this.radListBoxTabItems.Items.Count, listBoxItem);
                }
            }

            if (this.radListBoxTabItems.Items.Count > 0)
            {
                this.radListBoxTabItems.SelectedIndex = 0;
            }
        }

        #endregion

        #region Event handlers

        private void radListBoxTabItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.radListBoxTabItems.SelectedItem != null)
            {
                RadListBoxItem selectedItem = this.radListBoxTabItems.SelectedItem as RadListBoxItem;

                this.propGridTabProperties.SelectedObject = selectedItem.Tag;
            }
        }

        private void radBtnAddNewTab_Click(object sender, EventArgs e)
        {
            RibbonTab newTab = (RibbonTab)this.host.CreateComponent(typeof(RibbonTab));
            newTab.Text = newTab.Site.Name;

            RadListBoxItem listBoxItem = new RadListBoxItem();
            listBoxItem.Text = newTab.Text;
            listBoxItem.Tag = newTab;

            if (this.radListBoxTabItems.SelectedIndex >= 0
                && this.radListBoxTabItems.SelectedIndex < this.radListBoxTabItems.Items.Count - 1)
            {
                    this.radListBoxTabItems.Items.Insert(this.radListBoxTabItems.SelectedIndex, listBoxItem);
            }
            else
            {
                this.radListBoxTabItems.Items.Insert(this.radListBoxTabItems.Items.Count, listBoxItem);
            }

            this.radListBoxTabItems.SelectedItem = listBoxItem;

            IComponentChangeService componentChangeService = this.host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            
            if (componentChangeService != null)
            {
                componentChangeService.OnComponentChanging(this.host.RootComponent, TypeDescriptor.GetProperties(this.host.RootComponent)["CommandTabs"]);

                if (this.collection.Count > 0)
                {
                    this.collection.Insert(this.collection.Count - 1, newTab);
                }
                else
                {
                    this.collection.Insert(this.collection.Count, newTab);
                }

                componentChangeService.OnComponentChanged(this.host.RootComponent, TypeDescriptor.GetProperties(this.host.RootComponent)["CommandTabs"], null, null);
                this.newlyAddedItems.Add(newTab);
            }
        }

        private void radBtnRemoveTabs_Click(object sender, EventArgs e)
        {
            int indexOfSelectedItem = this.radListBoxTabItems.SelectedIndex;
            
            RadListBoxItem[] itemsToRemove = new RadListBoxItem[this.radListBoxTabItems.SelectedItems.Count];

            for (int index = 0; index < this.radListBoxTabItems.SelectedItems.Count; index++)
            {
                itemsToRemove[index] = this.radListBoxTabItems.SelectedItems[index] as RadListBoxItem;
            }

            for (int i = 0; i < itemsToRemove.Length; i++)
            {
                RadListBoxItem currentItem = itemsToRemove[i] as RadListBoxItem;

                RibbonTab tabToRemove = currentItem.Tag as RibbonTab;

                IComponentChangeService componentChangeService = this.host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                if (componentChangeService != null)
                {
                    componentChangeService.OnComponentChanging(this.host.RootComponent, TypeDescriptor.GetProperties(this.host.RootComponent)["CommandTabs"]);

                    this.collection.Remove(tabToRemove);

                    if (tabToRemove.ContextualTabGroup != null)
                    {
                        ContextualTabGroup tabGroup = tabToRemove.ContextualTabGroup;
                        tabGroup.TabItems.Remove(tabToRemove);
                    }

                    componentChangeService.OnComponentChanged(this.host.RootComponent, TypeDescriptor.GetProperties(this.host.RootComponent)["CommandTabs"], null, null);

                    if (newlyAddedItems.Contains(tabToRemove))
                    {
                        this.newlyAddedItems.Remove(tabToRemove);
                    }

                    this.radListBoxTabItems.Items.Remove(currentItem);
                    this.host.DestroyComponent(tabToRemove);
                }

            }


            if (indexOfSelectedItem >= 0 && indexOfSelectedItem <= this.radListBoxTabItems.Items.Count)
            {
                if (indexOfSelectedItem == this.radListBoxTabItems.Items.Count)
                {
                    this.radListBoxTabItems.SelectedIndex = indexOfSelectedItem - 1;
                }
                else
                {
                    this.radListBoxTabItems.SelectedIndex = indexOfSelectedItem;
                }
            }

            if (this.radListBoxTabItems.Items.Count == 0)
            {
                this.propGridTabProperties.SelectedObject = null;
            }
        }

        private void radBtnUp_Click(object sender, EventArgs e)
        {
            RadItem selectedItem = this.radListBoxTabItems.SelectedItem as RadItem;

            if (selectedItem != null)
            {
                int itemIndex = this.radListBoxTabItems.Items.IndexOf(selectedItem);

                if (itemIndex > 0)
                {
                    this.radListBoxTabItems.Items.Remove(selectedItem);
                    this.radListBoxTabItems.Items.Insert(itemIndex - 1, selectedItem);
                    this.radListBoxTabItems.SelectedItem = selectedItem;
                }
            }
        }

        private void radBtnDown_Click(object sender, EventArgs e)
        {
            RadItem selectedItem = this.radListBoxTabItems.SelectedItem as RadItem;

            if (selectedItem != null)
            {
                int itemIndex = this.radListBoxTabItems.Items.IndexOf(selectedItem);

                if (itemIndex < this.radListBoxTabItems.Items.Count - 1)
                {
                    this.radListBoxTabItems.Items.Remove(selectedItem);
                    this.radListBoxTabItems.Items.Insert(itemIndex + 1, selectedItem);
                    this.radListBoxTabItems.SelectedItem = selectedItem;
                }
            }
        }

        private void radBtnOK_Click(object sender, EventArgs e)
        {
            RibbonTab addNewItemTab = null;

            for (int i = 0; i < this.collection.Count; i++)
            {
                if ((bool)this.collection[i].GetValue(RadItem.IsAddNewItemProperty))
                {
                    addNewItemTab = this.collection[i] as RibbonTab;
                }
            }

            this.collection.Clear();
            

            for (int i = 0; i < this.radListBoxTabItems.Items.Count; i++)
            {
                RibbonTab ribbonTab = this.radListBoxTabItems.Items[i].Tag as RibbonTab;
                this.radListBoxTabItems.Items[i].Tag = null;
                this.collection.Insert(this.collection.Count, ribbonTab);
            }

            if (addNewItemTab != null)
            {
                this.collection.Insert(this.collection.Count, addNewItemTab);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void radBtnCancel_Click(object sender, EventArgs e)
        {
            this.collection.Clear();

            IComponentChangeService componentChangeService = this.host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            if (componentChangeService != null)
            {
                componentChangeService.OnComponentChanging(this.host.RootComponent, TypeDescriptor.GetProperties(this.host.RootComponent)["CommandTabs"]);

                for (int i = 0; i < this.initialCollection.Count; i++)
                {
                    this.collection.Add(this.initialCollection[i]);
                }

                componentChangeService.OnComponentChanged(this.host.RootComponent, TypeDescriptor.GetProperties(this.host.RootComponent)["CommandTabs"], null, null);
            }

            for (int i = 0; i < this.newlyAddedItems.Count; i++)
            {
                this.host.DestroyComponent(this.newlyAddedItems[i] as IComponent);
            }

                this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

       
    }
}
