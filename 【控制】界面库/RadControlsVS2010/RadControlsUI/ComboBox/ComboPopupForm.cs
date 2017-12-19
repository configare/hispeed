using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Telerik.WinControls.UI.ComboBox;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class ComboPopupForm : RadEditorPopupControlBase
    {
        // Fields
        private RadListBoxElement listBoxElement;
        internal RadComboBoxItem ActiveItem;
        internal bool IndexChanging = false;
        private Size popupSize;

		static ComboPopupForm()
		{
			//Instead of registering themes twice, force registration of themes
			RuntimeHelpers.RunClassConstructor(typeof(RadComboBoxElement).TypeHandle);
		}

        public ComboPopupForm(PopupEditorBaseElement owner)
            : base(owner)
        {
            if (this.listBoxElement != null)
            {
                this.listBoxElement.BindProperty(RadListBoxElement.CaseSensitiveProperty, owner, RadComboBoxElement.CaseSensitiveProperty, PropertyBindingOptions.OneWay);
                this.listBoxElement.BindProperty(RadListBoxElement.SortItemsProperty, owner, RadComboBoxElement.SortedProperty, PropertyBindingOptions.OneWay);
            }
        }

        private RadComboBoxElement OwnerComboItem
        {
            get
            {
                return this.OwnerElement as RadComboBoxElement;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListBoxElement ListBox
        {
            get
            {
                return this.listBoxElement;
            }
            internal set
            {
                this.listBoxElement = value;
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.listBoxElement.RightToLeft = this.RightToLeft == System.Windows.Forms.RightToLeft.Yes;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.InitializeListBoxElement();
            if (this.listBoxElement != null)
            {
                base.SizingGripDockLayout.Children.Add(this.listBoxElement);
            }
        }

        protected virtual void InitializeListBoxElement() 
        {
            this.listBoxElement = new ComboListBoxElement();
            this.listBoxElement.Items.ItemTypes = new Type[] { typeof(RadComboBoxItem) };
            this.listBoxElement.Items.SealedTypes = null;
            this.listBoxElement.Items.ExcludedTypes = null;
            this.listBoxElement.CanFocus = false;
            this.listBoxElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.listBoxElement.Viewport.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.listBoxElement.HorizontalScrollState = ScrollState.AlwaysHide;
            this.listBoxElement.Items.ItemsChanged += new ItemChangedDelegate(Items_ItemsChanged);
            this.listBoxElement.SelectedIndexChanged += new EventHandler(listBoxElement_SelectedIndexChanged);
            this.listBoxElement.SelectedItemChanged += new RadListBoxSelectionChangeEventHandler(listBoxElement_SelectedItemChanged);
            this.listBoxElement.SortItemsChanged += new EventHandler(listBoxElement_SortItemsChanged);
            this.listBoxElement.SelectedValueChanged += new EventHandler(listBoxElement_SelectedValueChanged);
            this.RootElement.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(RootElement_PropertyChanged);
            this.RootElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.listBoxElement.ItemClicked += new EventHandler(listBoxElement_ItemClicked);
        }
        
        void listBoxElement_ItemClicked(object sender, EventArgs e)
        {
            if (!this.OwnerComboItem.GetBitState(RadComboBoxElement.PopupEditorBaseElementLastStateKey) &&
                !this.OwnerComboItem.GetBitState(RadComboBoxElement.ListAutoCompleteIssuedStateKey))
            {
                this.ClosePopup(RadPopupCloseReason.Mouse);
            }

            this.SetActiveItem((RadComboBoxItem)this.listBoxElement.SelectedItem);
            this.OwnerComboItem.SyncTextWithItem();
        }

        void RootElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            this.listBoxElement.Items.ItemsChanged -= new ItemChangedDelegate(Items_ItemsChanged);
            this.listBoxElement.SelectedIndexChanged -= new EventHandler(listBoxElement_SelectedIndexChanged);
            this.listBoxElement.SortItemsChanged -= new EventHandler(listBoxElement_SortItemsChanged);
            this.listBoxElement.SelectedValueChanged -= new EventHandler(listBoxElement_SelectedValueChanged);
            this.listBoxElement.ItemClicked -= listBoxElement_ItemClicked;
            this.listBoxElement.SelectedItemChanged -= listBoxElement_SelectedItemChanged;
            base.Dispose(disposing);
        }

        internal void SetActiveItem(RadComboBoxItem item)
        {
			if (this.ActiveItem == item)
            if (this.ActiveItem == item)
            {
                return;
            }

            if (this.ActiveItem != null)
            {
                this.ActiveItem.Active = false;
            }
            this.ActiveItem = item;
            if (this.ActiveItem != null)
            {
                this.ActiveItem.Active = true;
            }
        }

        internal RadItem SetActiveItem(string text)
        {
            if (this.ActiveItem != null && 
                this.ActiveItem.Text == text)
            {
                return null;
            }
            RadItem item = this.FindItemByText(text);
            this.SetActiveItem(item as RadComboBoxItem);
            return item;
        }

        internal protected virtual RadItem FindItemByText(string text)
        {
            RadItem selectedItem = this.listBoxElement.SelectedItem as RadItem;
            if (selectedItem != null &&
                selectedItem.Text.Equals(text))
            {
                return selectedItem;
            }
            else if (this.OwnerComboItem.AutoCompleteMode == AutoCompleteMode.None)
            {
                return this.listBoxElement.FindItemExact(text);
            }
            else
            {
                return this.listBoxElement.FindItem(text);
            }
        }

        internal void DoScrollLineUp()
        {
            if (this.Visible)
            {
                this.listBoxElement.LineUp();
            }
        }

        internal void DoScrollLineDown()
        {
            if (this.Visible)
            {
                this.listBoxElement.LineDown();
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            this.popupSize = new Size(width, height);
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void SetVisibleCore(bool newVisible)
        {
            this.Size = this.popupSize;
            base.SetVisibleCore(newVisible);
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case NativeMethods.WM_LBUTTONDOWN:
                    if (this.OwnerComboItem != null)
                    {
                        this.OwnerComboItem.BitState[RadComboBoxElement.KeyboardCommandIssuedStateKey] = false;
                    }
                    break;
                case NativeMethods.WM_GETMINMAXINFO:
                    {
                        NativeMethods.MINMAXINFO info = (NativeMethods.MINMAXINFO)
                            Marshal.PtrToStructure(msg.LParam, typeof(NativeMethods.MINMAXINFO));
                        info.ptMinTrackSize.x = this.MinimumSize.Width;
                        info.ptMinTrackSize.y = this.MinimumSize.Height;

                        Marshal.StructureToPtr(info, msg.LParam, true);

                        return;
                    }
            }
            base.WndProc(ref msg);
        }

		protected override void AnimationStarting()
		{
			base.AnimationStarting();

			RadListBoxElement listBoxElement = this.ListBox;
			if (listBoxElement != null)
			{
				listBoxElement.VerticalScrollState = ScrollState.AlwaysHide;
			}
		}

		protected override void OnAnimationFinished(Telerik.WinControls.WindowAnimation.AnimationEventArgs e)
		{
            base.OnAnimationFinished(e);

			RadListBoxElement listBoxElement = this.ListBox;
			if (listBoxElement != null)
			{
				listBoxElement.VerticalScrollState = ScrollState.AutoHide;
			}
		}

        #region ListBox events
        private void listBoxElement_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IndexChanging = true;

            this.OwnerComboItem.SyncTextWithItem();
            this.OwnerComboItem.CallOnSelectedIndexChanged(e);

            this.IndexChanging = false;
        }

        private void listBoxElement_SelectedValueChanged(object sender, EventArgs e)
        {
            this.IndexChanging = true;

            this.OwnerComboItem.SyncTextWithItem();
            this.OwnerComboItem.CallOnSelectedValueChanged(e);

            this.IndexChanging = false;
        }

        private void listBoxElement_SelectedItemChanged(object sender, Telerik.WinControls.UI.UIElements.ListBox.RadListBoxSelectionChangeEventArgs e)
        {
            this.OwnerComboItem.SyncTextWithItem();
        }

        private void listBoxElement_SortItemsChanged(object sender, EventArgs e)
        {
            this.OwnerComboItem.CallOnSortedChanged(e);
        }

        private void Items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            RadComboBoxItem comboItem = target as RadComboBoxItem;
            if (comboItem == null)
            {
                return;
            }

            switch (operation)
            {
                case ItemsChangeOperation.Removed:
                    if (this.listBoxElement.SelectedItem != null &&
                        this.listBoxElement.SelectedItem.Equals(comboItem))
                    {
                        this.listBoxElement.SelectedIndex = -1;
                    }
                    comboItem.OwnerElement = null;
                    break;
               
                case ItemsChangeOperation.Cleared:
                    this.listBoxElement.SelectedIndex = -1;
                    break;
                case ItemsChangeOperation.Setting:
                    if (this.listBoxElement.SelectedItem != null && 
                        this.listBoxElement.SelectedItem.Equals(comboItem))
                    {
                        this.listBoxElement.SelectedIndex = -1;
                    }
                    comboItem.OwnerElement = null;
                    break;
                case ItemsChangeOperation.Inserted:
                case ItemsChangeOperation.Set:
                    if (comboItem != null)
                    {
                        comboItem.OwnerElement = this.OwnerComboItem;
                    }
                    break;
            }
        }

        #endregion

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            if (reason == RadPopupCloseReason.Mouse
                && this.OwnerComboItem != null 
                && this.OwnerComboItem.ElementState == ElementState.Loaded)
            {
                Point relativeMousePosition = this.OwnerComboItem.ElementTree.Control.PointToClient(Control.MousePosition);
                Rectangle arrowButtonRectangle = this.OwnerComboItem.ArrowButton.ControlBoundingRectangle;

                if (arrowButtonRectangle.Contains(relativeMousePosition))
                {
                    return false;
                }

                if (this.OwnerComboItem.DropDownStyle == RadDropDownStyle.DropDownList
                    && this.OwnerComboItem.Bounds.Contains(relativeMousePosition))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool OnMouseWheel(Control target, int delta)
        {
            return true;
        }

        public override bool OnKeyDown(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                this.OwnerComboItem.ProcessReturnKey();
                return true;
            }

            if (keyData == Keys.Back && this.OwnerComboItem.AutoCompleteMode == AutoCompleteMode.SuggestAppend)
            {
                return false;
            }

            return base.OnKeyDown(keyData);
        }
    }
}
