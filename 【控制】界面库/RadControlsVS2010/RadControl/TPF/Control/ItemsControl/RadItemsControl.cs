using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;
using Telerik.WinControls.Design;

namespace Telerik.WinControls
{
	public abstract class RadItemsControl : RadControl, IItemsControl
    {
        #region Fields

        private IItemsControl itemsControlImpl;

        public event ItemSelectedEventHandler ItemSelected;
        public event ItemSelectedEventHandler ItemDeselected;

        #endregion

        #region Ctor

        public RadItemsControl() : base()
		{
            this.itemsControlImpl = this.GetItemsControlImpl();
            this.itemsControlImpl.ItemSelected += new ItemSelectedEventHandler(OnItemsControlImpl_ItemSelected);
            this.itemsControlImpl.ItemDeselected += new ItemSelectedEventHandler(OnItemsControlImpl_ItemDeselected);
        }

        protected virtual IItemsControl GetItemsControlImpl()
        {
            return new RadItemsControlImpl(this.Items);
        }

        #endregion

        #region Methods



        #endregion

        #region Properties

    

        /// <summary>
		///		Gets or sets whether the rollover items functionality of the RadItemsControl will be allowed.
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets whether the rollover items functionality of the RadItemsControl will be allowed.")]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool RollOverItemSelection
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

		/// <summary>
		///		Gets or sets whether the RadItemsControl processes the keyboard.
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets whether the RadItemsControl processes the keyboard.")]
		[DefaultValue(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool ProcessKeyboard
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

        public virtual bool HasKeyboardInput
		{
			get
			{
				if (base.ContainsFocus)
				{
					return true;
				}
				return false;
			}
		}

        public abstract RadItemOwnerCollection Items
        {
            get;
        }


        public virtual RadItemOwnerCollection ActiveItems 
        {
            get { return this.itemsControlImpl.ActiveItems; }
        }

        #endregion

        #region Public methods

        public virtual bool CanProcessMnemonic(char keyData)
        {
            return this.itemsControlImpl.CanProcessMnemonic(keyData);
        }

        public virtual bool CanNavigate(Keys keyData)
        {
            return this.itemsControlImpl.CanNavigate(keyData);
        }

        public virtual RadItem GetSelectedItem()
        {
            return this.itemsControlImpl.GetSelectedItem();
        }

        public virtual void SelectItem(RadItem item)
        {
            this.itemsControlImpl.SelectItem(item);
        }

        public virtual RadItem GetNextItem(RadItem item, bool forward)
		{
            return this.itemsControlImpl.GetNextItem(item, forward);
		}

        public virtual RadItem GetFirstVisibleItem()
		{
            return this.itemsControlImpl.GetFirstVisibleItem();
		}

        public virtual RadItem GetLastVisibleItem()
		{
            return this.itemsControlImpl.GetLastVisibleItem();
		}

        public virtual RadItem SelectNextItem(RadItem item, bool forward)
		{
            return this.itemsControlImpl.SelectNextItem(item, forward);
		}

        public virtual RadItem SelectFirstVisibleItem()
		{
            RadItem item = this.itemsControlImpl.SelectFirstVisibleItem();

            if (item != null)
            {
                item.Focus();
            }

            return item;
		}

        public virtual RadItem SelectLastVisibleItem()
		{
            RadItem item = this.itemsControlImpl.SelectLastVisibleItem();

            if (item != null)
            {
                item.Focus();
            }

            return item;
        }

        #endregion

        #region Event handlers

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            RadItem selectedItem = this.GetSelectedItem();

            if (selectedItem != null)
                selectedItem.Focus();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool processResult = false;

            if (!this.ProcessKeyboard)
                return this.CallBaseProcessDialogKey(keyData);

            RadItem selectedItem = this.GetSelectedItem();
            if ((this.ContainsFocus && this.Focused) &&
                (selectedItem != null && selectedItem.Enabled) &&
                selectedItem.ProcessDialogKey(keyData))
            {
                return true;
            }
            bool isAltOrControlKey = (keyData & (Keys.Alt | Keys.Control)) != Keys.None;
            Keys keyCode = keyData & Keys.KeyCode;
            switch (keyCode)
            {
                case Keys.Back:
                    {
                        if (!base.ContainsFocus)
                            processResult = this.ProcessTabKey(false);
                        break;
                    }
                case Keys.Tab:
                    {
                        if (!isAltOrControlKey)
                            processResult = this.ProcessTabKey((keyData & Keys.Shift) == Keys.None);
                        break;
                    }
                case Keys.End:
                    {
                        if (this.Focused)
                        {
                            this.SelectLastVisibleItem();
                            processResult = true;
                        }
                        break;
                    }
                case Keys.Home:
                    {
                        if (this.Focused)
                        {
                            this.SelectFirstVisibleItem();
                            processResult = true;
                        }
                        break;
                    }
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                    {
                        processResult = this.ProcessArrowKey(keyCode);
                        break;
                    }
            }

            if (processResult)
                return processResult;

            return this.CallBaseProcessDialogKey(keyData);
        }

        protected virtual bool CallBaseProcessDialogKey(Keys keyData)
        {
            return base.ProcessDialogKey(keyData);
        }

        protected virtual bool OnHandleKeyDown(Message m)
        {
            return false;
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            RadItem item1 = this.GetSelectedItem();
            if ((item1 != null) && item1.ProcessCmdKey(ref m, keyData))
            {
                return true;
            }
            foreach (RadItem item2 in this.ActiveItems)
            {
                if ((item2 != item1) && item2.ProcessCmdKey(ref m, keyData))
                {
                    return true;
                }
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        protected virtual bool ProcessArrowKey(Keys keyCode)
        {
            bool processResult = false;
            
            switch (keyCode)
            {
                case Keys.Left:
                case Keys.Right:
                    {
                        return this.ProcessLeftRightArrowKey(keyCode == Keys.Right);
                    }
                case Keys.Up:
                case Keys.Down:
                    {
                        return this.ProcessUpDownArrowKey(keyCode == Keys.Down);
                    }
            }
            return processResult;
        }

        protected virtual bool ProcessLeftRightArrowKey(bool right)
        {
            this.SelectNextItem(this.GetSelectedItem(), right);
            return true;
        }

        protected virtual bool ProcessUpDownArrowKey(bool down)
        {
            this.SelectNextItem(this.GetSelectedItem(), down);
            return true;
        }

        protected virtual bool ProcessTabKey(bool forward)
        {
            return false;
        }

        #endregion

        #region Event handling

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

        private void OnItemsControlImpl_ItemSelected(object sender, ItemSelectedEventArgs args)
        {
            if (this.ItemSelected != null)
            {
                this.ItemSelected(sender, args);
            }

            this.CallOnItemSelected(args);
        }

        internal void CallOnItemSelected(ItemSelectedEventArgs args)
        {
            this.OnItemSelected(args);
        }

        protected virtual void OnItemSelected(ItemSelectedEventArgs args)
        {
        }


        #endregion

        protected override bool IsInputKey(Keys keyData)
        {
            RadItem item1 = this.GetSelectedItem();
            if ((item1 != null) && item1.IsInputKey(keyData))
            {
                return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override bool IsInputChar(char charCode)
        {
            return base.IsInputChar(charCode);
        }

        protected override void Select(bool directed, bool forward)
        {
            if (!this.ProcessKeyboard)
            {
                base.Select(directed, forward);
                return;
            }
            bool flag1 = true;
            if (this.Parent != null)
            {
                IContainerControl control1 = this.Parent.GetContainerControl();
                if (control1 != null)
                {
                    control1.ActiveControl = this;
                    flag1 = control1.ActiveControl == this;
                }
            }
            if (directed && flag1)
            {
                this.SelectNextToolStripItem(null, forward);
            }
        }

        protected virtual void GetChildMnemonicList(ArrayList mnemonicList)
        {
            foreach (RadItem item in this.ActiveItems)
            {
                char ch1 = WindowsFormsUtils.GetMnemonic(item.Text, true);
                if (ch1 != '\0')
                {
                    mnemonicList.Add(ch1);
                }
            }
        }

        protected virtual void ChangeSelection(RadItem nextItem)
        {
            if (nextItem != null)
            {
                RadHostItem host1 = nextItem as RadHostItem;
                if (base.ContainsFocus && !this.Focused)
                {
                    this.Focus();
                    if (host1 == null)
                    {
                        //this.KeyboardActive = true;
                    }
                }
                if (host1 != null)
                {
                    host1.HostedControl.Select();
                    host1.HostedControl.Focus();
                }
                //nextItem.Select();
            }
        }        

        private RadItem SelectNextToolStripItem(RadItem start, bool forward)
        {
            RadItem item1 = this.GetNextItem(start, forward);//? ArrowDirection.Right : ArrowDirection.Left
            this.ChangeSelection(item1);
            return item1;
        }


        [Obsolete("This method should not be used. It will be removed for Q1 2010.")]
        protected virtual bool OnHandleSysChar(Message m)
        {
            return false;
        }
   }
}