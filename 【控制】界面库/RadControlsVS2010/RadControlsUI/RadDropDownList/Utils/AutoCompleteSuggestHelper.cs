using System;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class AutoCompleteSuggestHelper : BaseAutoComplete
    {
        #region Fields
        private string filter;
        private RadDropDownListElement dropDownList;
        private bool isItemsDirty = false;
        private bool isUpdating = false;
        private StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;

        /// <summary>
        /// IsItemsDirty Property
        /// </summary>
        public virtual bool IsItemsDirty
        {
            get
            {
                return (isItemsDirty || this.dropDownList.ListElement.Items.Count == 0) && this.dropDownList.DataSource == null;
            }            
        }

        #endregion

        #region Cstors

        public AutoCompleteSuggestHelper(RadDropDownListElement owner)
            : base(owner)
        {        
            this.filter = "";
            this.dropDownList = new RadDropDownListElement();
            
            owner.Children.Add(this.dropDownList);
            this.dropDownList.isSuggestMode = true;
            this.dropDownList.Visibility = ElementVisibility.Hidden;
            this.dropDownList.DropDownSizingMode = SizingMode.RightBottom;
            this.AutoCompleteDataSource = owner.AutoCompleteDataSource;
            this.AutoCompleteDisplayMember = owner.AutoCompleteDisplayMember;
            this.AutoCompleteValueMember = owner.AutoCompleteValueMember;

            this.dropDownList.ListElement.SelectedIndexChanged += SelectedIndexChanged;
            this.owner.ListElement.ItemsChanged += CollectionChanged;
            this.dropDownList.PopupClosed += PopupClosed;
        }        

        #endregion

        #region Properties

        protected virtual string Filter
        {
            get
            {
                return this.filter;
            }
        }

        protected virtual StringComparison StringComparison
        {
            get
            {
                return this.stringComparison;
            }
        }

        /// <summary>
        /// Gets or sets the object that is responsible for providing data objects for the AutoComplete Suggest.
        /// </summary>
        public virtual object AutoCompleteDataSource
        {
            get
            {
                return this.dropDownList.DataSource;
            }
            set
            {
                this.isUpdating = true;
                this.dropDownList.DataSource = value;
                this.isUpdating = false;
            }
        }

        /// <summary>
        /// Gets or sets the object that is responsible for providing data objects for the AutoComplete Suggest.
        /// </summary>
        public virtual string AutoCompleteValueMember
        {
            get
            {
                return this.dropDownList.ValueMember;
            }
            set
            {
                this.isUpdating = true;
                this.dropDownList.ValueMember = value;
                this.isUpdating = false;
            }
        }

        /// <summary>
        /// Gets or sets the object that is responsible for providing data objects for the AutoComplete Suggest.
        /// </summary>
        public virtual string AutoCompleteDisplayMember
        {
            get
            {
                return this.dropDownList.DisplayMember;
            }
            set
            {
                this.isUpdating = true;
                this.dropDownList.DisplayMember = value;
                this.isUpdating = false;
            }
        }

        /// <summary>
        /// DropDownList Property
        /// </summary>
        public virtual RadDropDownListElement DropDownList
        {
            get
            {
                return dropDownList;
            }
            set
            {
                dropDownList = value;
            }
        }

        #endregion

        #region Event handlers

        virtual public void PopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            if (args.CloseReason == RadPopupCloseReason.Keyboard)
            {
                this.owner.SelectAllText();
                //this.owner.Focus();
            }
        }

        virtual public void CollectionChanged(object sender, Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            this.isItemsDirty = this.dropDownList.DataSource == null;//suspend items sync operation if datasource!=null
        }
        
        #endregion

        #region Methods

        virtual public void HandleSelectNextOrPrev(bool next)
        {
            int selectedIndex = this.owner.ClampSelectedIndex(next, this.dropDownList.ListElement.SelectedIndex, this.dropDownList.ListElement.Items.Count);
            this.dropDownList.ListElement.SelectedIndex = selectedIndex;
        }

        override public void AutoComplete(KeyPressEventArgs e)
        {
            string findString = "";

            if (e.KeyChar == (char)Keys.Enter)//close popup and select text
            {
                this.owner.Focus();
                this.owner.ClosePopup(RadPopupCloseReason.Keyboard);
                return;
            }

            if (e.KeyChar == (char)Keys.Back && owner.EditableElementText.Length >= 1)
            {
                findString = this.SetFindStringFromBack();
            }
            else
            {
                findString = this.SetFindString(e);
            }

            //Show dropDown
            this.HandleAutoSuggest(findString);
        }

        #endregion

        #region Implementation

        protected virtual bool DefaultFilter(RadListDataItem item)
        {
            return item.CachedText.StartsWith(this.Filter, this.StringComparison);
        }

        virtual protected void SyncOwnerElementWithSelectedIndex()
        {
            string textFromAutoSuggest = this.dropDownList.ListElement.SelectedItem.Text;
            int index = this.owner.SelectItemFromText(textFromAutoSuggest);
            if (index != -1)
            {
                return;
            }

            this.owner.Text = textFromAutoSuggest;
        }

        virtual protected void SyncItems()
        {
            if (!this.IsItemsDirty)
            {
                return;
            }

            this.isItemsDirty = false;
            this.SyncItemsCore();
        }

        virtual protected void SyncItemsCore()
        {
            this.dropDownList.ListElement.Items.Clear();
            this.dropDownList.ListElement.BeginUpdate();
            foreach (RadListDataItem item in owner.Items)
            {
                this.dropDownList.ListElement.Items.Add(new RadListDataItem(item.CachedText));
            }

            this.dropDownList.ListElement.EndUpdate();
        }

        protected virtual void SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (this.isUpdating)//databinding 
            {
                return;
            }

            if (this.dropDownList.ListElement.SelectedIndex != -1)
            {
                this.SyncOwnerElementWithSelectedIndex();
            }
        }

        public virtual string SetFindStringFromBack()
        {
            string findString = "";
            if (owner.SelectionLength == 0)
            {
                findString = owner.EditableElementText.Substring(0, owner.EditableElementText.Length - 1);
            }
            else
            {
                findString = owner.EditableElementText.Substring(0, owner.SelectionStart);//- 1);
            }

            return findString;
        }

        public virtual string SetFindString(KeyPressEventArgs e)
        {
            string findString = "";
            if (owner.SelectionLength == 0)
            {
                findString = owner.EditableElementText + e.KeyChar;
            }
            else
            {
                findString = owner.EditableElementText.Substring(0, owner.SelectionStart) + e.KeyChar;
            }

            return findString;
        }

        public virtual void HandleAutoSuggest(string filter)
        {
            this.SyncItems();
            this.stringComparison = this.owner.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;              
            this.ApplyFilterToDropDown(filter);           
            if ((this.dropDownList.ListElement.Items.Count == 0 || string.IsNullOrEmpty(filter)) ||
                (this.dropDownList.ListElement.Items.Count == 1 && string.Equals(filter, this.dropDownList.ListElement.Items[0].CachedText)))
            {
                this.dropDownList.ClosePopup();//no items for this filter -> close popup or only one item with Exact match
                return;
            }

            this.ShowDropDownList();
        }

        public virtual void ShowDropDownList()
        {
            this.dropDownList.Popup.Size = this.dropDownList.GetDesiredPopupSize();            
            this.dropDownList.ListElement.SelectionMode = SelectionMode.One;
            int selectionStart = this.owner.SelectionStart;
            int selectionLen = this.owner.SelectionLength;

            this.dropDownList.BeginUpdate();
            this.dropDownList.ShowPopup();
            this.dropDownList.EndUpdate();

            this.owner.SelectionStart = selectionStart;
            this.owner.SelectionLength = selectionLen;
        }

        public virtual void ApplyFilterToDropDown(string filter)
        {
            this.filter = filter;
            this.dropDownList.ListElement.SelectionMode = SelectionMode.None;
            this.dropDownList.ListElement.BeginUpdate();
            this.dropDownList.ListElement.Filter = null;
            this.dropDownList.ListElement.Filter = DefaultFilter;
            this.dropDownList.ListElement.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending;
            this.dropDownList.ListElement.EndUpdate();
        }
        
        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            this.dropDownList.ListElement.SelectedIndexChanged-= SelectedIndexChanged;
            this.owner.ListElement.ItemsChanged-= CollectionChanged;
            this.dropDownList.PopupClosed -= PopupClosed;
            this.owner.Children.Remove(this.dropDownList);
        }

        #endregion
    }
}
