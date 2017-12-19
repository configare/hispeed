using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;


namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a encapsulated implementation of the IItemsControl interface.
    /// </summary>
    internal class RadItemsControlImpl : IItemsControl
    {
        #region Fields

        private bool rolloverItemSelection = false;
        private bool processKeyboard = false;
        private RadItem selectedItem;
        private RadItemOwnerCollection ownerCollection;
        public event ItemSelectedEventHandler ItemSelected;
        public event ItemSelectedEventHandler ItemDeselected;

        #endregion

        #region Constructor

        internal RadItemsControlImpl(RadItemOwnerCollection items)
        {
            this.ownerCollection = items;
        }

        #endregion

        #region Methods

        public bool CanProcessMnemonic(char keyData)
        {
            return false;
        }

        public bool CanNavigate(Keys keyData)
        {
            return false;
        }

        public RadItem GetSelectedItem()
		{
			return this.selectedItem;
		}

		public void SelectItem(RadItem item)
		{
            if (this.selectedItem != item && this.selectedItem != null && this.ItemDeselected != null)
            {
                ItemSelectedEventArgs args = new ItemSelectedEventArgs(selectedItem);
                this.ItemDeselected(this, args);
            }

			this.selectedItem = item;

            if (this.ItemSelected != null && item != null)
            {
                ItemSelectedEventArgs args = new ItemSelectedEventArgs(item);
                this.ItemSelected(this, args);
            }
		}

		public RadItem GetNextItem(RadItem item, bool forward)
		{
            if (rolloverItemSelection)
            {
                if (forward)
                {
                    RadItem lastItem = GetLastVisibleItem();
                    if (item == lastItem)
                        return GetFirstVisibleItem();
                }
                else
                {
                    RadItem firstItem = GetFirstVisibleItem();
                    if (item == firstItem)
                        return GetLastVisibleItem();
                }
            }

            RadItem nextItem = null;
            bool itemSelected = false;

            foreach (RadItem childItem in this.ActiveItems)
            {
                if (item == null && (childItem.Selectable && childItem.Visibility == ElementVisibility.Visible))
                {
                    return childItem;
                }
                if (childItem == item)
                {
                    if (forward)
                        itemSelected = true;
                    else
                        return nextItem;
                }
                else
                {
                    if (childItem.Selectable && childItem.Visibility == ElementVisibility.Visible)
                        nextItem = childItem;
                    else if (itemSelected)
                        nextItem = null;

                    if (itemSelected && (nextItem != null))
                        return nextItem;
                }
            }

            return null;
		}

		public RadItem SelectNextItem(RadItem item, bool forward)
		{
			RadItem nextItem = GetNextItem(item, forward);
			if (nextItem == null)
				return null;

			this.SelectItem(nextItem);
			return nextItem;
		}

		public RadItem GetFirstVisibleItem()
		{
			foreach (RadItem item in this.ActiveItems)
			{
				if (item.Selectable && item.Visibility == ElementVisibility.Visible)
					return item;
			}

			return null;
		}

		public RadItem GetLastVisibleItem()
		{
			for (int i = (this.ActiveItems.Count - 1); i >= 0; i--)
			{
				if (this.ActiveItems[i].Selectable && this.ActiveItems[i].Visibility == ElementVisibility.Visible)
					return this.ActiveItems[i];
			}
			return null;
		}

		public RadItem SelectFirstVisibleItem()
		{
			RadItem nextItem = GetFirstVisibleItem();
			if (nextItem == null)
				return null;

			this.SelectItem(nextItem);
			return nextItem;
		}

		public RadItem SelectLastVisibleItem()
		{
			RadItem nextItem = GetLastVisibleItem();
			if (nextItem == null)
				return null;

			this.SelectItem(nextItem);
			return nextItem;
		}

        public RadItemOwnerCollection ActiveItems
        {
            get
            {
                return this.Items;
            }
        }

        public RadItemOwnerCollection Items
        {
            get
            {
                return this.ownerCollection;
            }
        }

        public bool RollOverItemSelection
        {
            get
            {
                return this.rolloverItemSelection;
            }
            set
            {
                this.rolloverItemSelection = value;
            }
        }

        public bool ProcessKeyboard
        {
            get
            {
                return this.processKeyboard;
            }
            set
            {
                this.processKeyboard = value;
            }
        }

        #endregion
    }
}
