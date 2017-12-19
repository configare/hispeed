using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.WindowAnimation;
using System.Threading;

namespace Telerik.WinControls.UI
{
	[ToolboxItem(false)]
	public abstract class RadItemsPopupControl : RadPopupControlBase, IItemsControl
	{
		// fields
		internal IntPtr activeHwnd = IntPtr.Zero;
        private RadItemsControlImpl itemsControlImpl;

        public event ItemSelectedEventHandler ItemSelected;
        public event ItemSelectedEventHandler ItemDeselected;
        private RadItemOwnerCollection items;

        public RadItemsPopupControl(RadElement owner)
            : base(owner)
		{
            this.items = new RadItemOwnerCollection();
            this.itemsControlImpl = new RadItemsControlImpl(this.items);
            this.itemsControlImpl.ItemSelected += new ItemSelectedEventHandler(OnItemsControlImpl_ItemSelected);
            this.itemsControlImpl.ItemDeselected += new ItemSelectedEventHandler(OnItemsControlImpl_ItemDeselected);
			this.itemsControlImpl.RollOverItemSelection = true;
            this.PopupOpening += new RadPopupOpeningEventHandler(RadItemsPopupControl_PopupOpening);
            this.PopupClosing += new RadPopupClosingEventHandler(RadItemsPopupControl_PopupClosing);
            this.PopupOpened += new RadPopupOpenedEventHandler(RadItemsPopupControl_PopupOpened);
            this.PopupClosed += new RadPopupClosedEventHandler(RadItemsPopupControl_PopupClosed);        
		}

        private void OnItemsControlImpl_ItemDeselected(object sender, ItemSelectedEventArgs args)
        {
            if (this.ItemDeselected != null)
            {
                this.ItemDeselected(sender, args);
            }

            this.CallOnItemDeselected(args);
        }

        public void CallOnItemDeselected(ItemSelectedEventArgs args)
        {


            this.OnItemDeselected(args);
        }

        protected virtual void OnItemDeselected(ItemSelectedEventArgs args)
        {

        }

        private void OnItemsControlImpl_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            if (this.ItemSelected != null)
            {
                this.ItemSelected(sender, e);
            }

            this.CallOnItemSelected(e);
        }

        internal void CallOnItemSelected(ItemSelectedEventArgs args)
        {
            this.OnItemSelected(args);
        }

        protected virtual void OnItemSelected(ItemSelectedEventArgs args)
        {

        }

        private void RadItemsPopupControl_PopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            this.OnDropDownClosed(args);
        }

        private void RadItemsPopupControl_PopupOpened(object sender, EventArgs args)
        {
            this.OnDropDownOpened();
        }

        private void RadItemsPopupControl_PopupClosing(object sender, RadPopupClosingEventArgs args)
        {
            this.OnDropDownClosing(args);
        }

        private void RadItemsPopupControl_PopupOpening(object sender, CancelEventArgs args)
        {
            this.OnDropDownOpening(args);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                activeHwnd = IntPtr.Zero;
            }

            base.Dispose(disposing);
        }

        #region Events

		public event CancelEventHandler DropDownOpening;
		
		public event RadPopupClosingEventHandler DropDownClosing;
		
		public event EventHandler DropDownOpened;
		
		public event RadPopupClosedEventHandler DropDownClosed;


        /// <summary>
        /// Gets a boolean value indicating whether the popup is visible.
        /// </summary>
		public bool IsVisible
		{
			get
			{
				return PopupManager.Default.ContainsPopup(this);
			}
		}

        /// <summary>
        /// Gets menu items collection
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }


        #endregion

        protected virtual void OnDropDownOpening(CancelEventArgs args)
        {
            if (DropDownOpening != null)
            {
                DropDownOpening(this, args);
            }
        }

		protected virtual void OnDropDownClosing(RadPopupClosingEventArgs args)
		{
			if (DropDownClosing != null)
			{
				DropDownClosing(this, args);
			}
		}

        protected virtual void OnDropDownOpened()
        {
            if (DropDownOpened != null)
            {
                DropDownOpened(this, EventArgs.Empty);
            }
        }

		protected virtual void OnDropDownClosed(RadPopupClosedEventArgs args)
		{
			if (DropDownClosed != null)
			{
                DropDownClosed(this, args);
			}
		}

		private Size minimum = Size.Empty;
		private Size maximum = Size.Empty;

		/// <summary>
		/// Get/Set minimum value allowed for size
		/// </summary>
		public Size Minimum
		{
			get
			{
				return minimum;
			}

            set
            {
                minimum = value;
            }
        }

        /// <summary>
        /// Get/Set maximum value allowed for size
        /// </summary>
        public Size Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
            }
        }

        protected void AutoUpdateBounds()
        {
            this.minimum = new Size(this.Width, 10);
            this.maximum = this.Size;
        }


        #region IItemsControl Members

        public virtual bool CanProcessMnemonic(char keyData)
        {
            return this.itemsControlImpl.CanProcessMnemonic(keyData);
        }

        public virtual bool CanNavigate(Keys keyData)
        {
            return this.itemsControlImpl.CanNavigate(keyData);
        }

        public RadItem GetSelectedItem()
        {
            return this.itemsControlImpl.GetSelectedItem();
        }

        public void SelectItem(RadItem item)
        {
            this.itemsControlImpl.SelectItem(item);
        }

        public RadItem GetNextItem(RadItem item, bool forward)
        {
            return this.itemsControlImpl.GetNextItem(item, forward);
        }

        public RadItem SelectNextItem(RadItem item, bool forward)
        {
            return this.itemsControlImpl.SelectNextItem(item, forward);
        }

        public RadItem GetFirstVisibleItem()
        {
            return this.itemsControlImpl.GetFirstVisibleItem();
        }

        public RadItem GetLastVisibleItem()
        {
            return this.itemsControlImpl.GetLastVisibleItem();
        }

        public RadItem SelectFirstVisibleItem()
        {
            return this.itemsControlImpl.SelectFirstVisibleItem();
        }

        public RadItem SelectLastVisibleItem()
        {
            return this.itemsControlImpl.SelectLastVisibleItem();
        }

        public RadItemOwnerCollection ActiveItems
        {
            get
            {
                return this.itemsControlImpl.ActiveItems;
            }
        }

        public bool RollOverItemSelection
        {
            get
            {
                return this.itemsControlImpl.RollOverItemSelection;
            }
            set
            {
                this.itemsControlImpl.RollOverItemSelection = value;
            }
        }

        public bool ProcessKeyboard
        {
            get
            {
                return this.itemsControlImpl.ProcessKeyboard;
            }
            set
            {
                this.itemsControlImpl.ProcessKeyboard = value;
            }
        }

        #endregion

       
    }
}
