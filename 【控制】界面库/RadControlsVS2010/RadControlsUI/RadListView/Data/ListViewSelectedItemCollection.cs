using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class ListViewSelectedItemCollection : ReadOnlyCollection<ListViewDataItem>
    {
        #region Fields

        private RadListViewElement listViewElement;

        #endregion

        #region Constructor

        public ListViewSelectedItemCollection(RadListViewElement listView)
            : base(new List<ListViewDataItem>())
        {
            this.listViewElement = listView;
        }

        #endregion

        #region Methods

        internal void ProcessSelectedItem(ListViewDataItem listViewItem)
        {
            if (listViewItem.Selected && listViewItem.Owner == this.listViewElement)
            {
                this.Items.Add(listViewItem);
                this.listViewElement.OnSelectedItemsChanged();
            }
            else
            {
                if (this.Items.Remove(listViewItem))
                {
                    this.listViewElement.OnSelectedItemsChanged();
                }

                if (this.listViewElement.SelectedItem == listViewItem)
                {
                    ListViewDataItem newItem = null;
                    if (this.Count > 0)
                    {
                        newItem = this[this.Count - 1];
                    }

                    this.listViewElement.SetSelectedItem(newItem);
                }
            } 
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            if (this.Items.Count == 0)
            {
                return;
            }

            while (this.Items.Count > 0)
            {
                ListViewDataItem node = this.Items[0];
                node.Selected = false;
            }
        }

        #endregion
    }
}
