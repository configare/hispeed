using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Telerik.WinControls.UI
{
    public class ListViewCheckedItemCollection : ReadOnlyCollection<ListViewDataItem>
    {
        #region Fields

        private RadListViewElement listView;

        #endregion

        #region Constructor

        public ListViewCheckedItemCollection(RadListViewElement listView)
            : base(new List<ListViewDataItem>())
        {
            this.listView = listView;
        }

        #endregion

        #region Methods

        internal void ProcessCheckedItem(ListViewDataItem listViewItem)
        {
            if (listViewItem.CheckState == Enumerations.ToggleState.On)
            {
                this.Items.Add(listViewItem);
            }
            else
            {
                this.Items.Remove(listViewItem);
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
