using Telerik.WinControls.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.UI.Data;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class PopupEditorElement : PopupEditorBaseElement
    {
        #region Consts
        internal const int DefaultDropDownHeight = 106;
        internal const int DefaultItemsCountForSuggest = 10;
        internal const int DefaultDropDownItems = 8;
        #endregion

        #region Fields
        protected RadListElement listElement;
        protected RadEditorPopupControlBase popup;
        protected int defaultItemsCountInDropDown = 6;
        private Size dropDownMinSize;
        private Size dropDownMaxSize;
        private int dropDownHeight = DefaultDropDownHeight;
        protected object autoCompleteDataSource;
        private int maxDropDownItems = DefaultDropDownItems;
        protected string autoCompleteValueMember;
        protected string autoCompleteDisplayMember;

       
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the maximum number of items to be shown in the drop-down portion of the ComboBox. 
        /// </summary>
        [Description("Gets or sets the maximum number of items to be shown in the drop-down portion of the ComboBox. "),
        Category(RadDesignCategory.BehaviorCategory),
        DefaultValue(DefaultDropDownItems)]
        public int MaxDropDownItems
        {
            get
            {
                return this.maxDropDownItems;
            }

            set
            {
                this.maxDropDownItems = value;
            }
        }

        /// <summary>
        /// Gets or sets the object that is responsible for providing data objects for the AutoComplete Suggest.
        /// </summary>
        public object AutoCompleteDataSource
        {
            get
            {
                return this.autoCompleteDataSource;
            }
            set
            {
                if (this.autoCompleteDataSource != value)
                {
                    this.autoCompleteDataSource = value;
                    this.OnAutoCompeleteDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// AutoCompleteValueMember Property
        /// </summary>
        public string AutoCompleteValueMember
        {
            get
            {
                return autoCompleteValueMember;
            }
            set
            {
                autoCompleteValueMember = value;
                this.OnAutoCompeleteDataSourceChanged();
            }
        }

        /// <summary>
        /// AutoCompleteDataMember Property
        /// </summary>
        public string AutoCompleteDisplayMember
        {
            get
            {
                return autoCompleteDisplayMember;
            }
            set
            {
                autoCompleteDisplayMember = value;
                this.OnAutoCompeleteDataSourceChanged();
            }
        }
         

        protected virtual void OnAutoCompeleteDataSourceChanged()
        {
        }

        /// <summary>
        /// Gets or sets the height in pixels of the drop-down portion of the ComboBox. 
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the height in pixels of the drop-down portion of the ComboBox."),
        DefaultValue(DefaultDropDownHeight),
        EditorBrowsable(EditorBrowsableState.Always)]
        public int DropDownHeight
        {
            get
            {
                return this.dropDownHeight;
            }
            set
            {
                this.dropDownHeight = value;
            }
        }

        /// <summary>
        /// Popup Property
        /// </summary>
        public RadEditorPopupControlBase Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = (RadEditorPopupControlBase)this.CreatePopupForm();
                }

                return popup;
            }
            set
            {
                popup = value;
            }
        }

        /// <summary>
        /// DefaultItemsCountInDropDown Property
        /// </summary>
        public int DefaultItemsCountInDropDown
        {
            get
            {
                return defaultItemsCountInDropDown;
            }
            set
            {
                defaultItemsCountInDropDown = value;
            }
        }      

        /// <summary>
        /// The input element hosted in the popup form. In the case of
        /// DropDownList the control is a ListElement.
        /// </summary>
        public RadListElement ListElement
        {
            get
            {
                return this.listElement;
            }
            set
            {
                this.listElement = value;
            }
        }

        //TODO
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down maximum size.")]
        [DefaultValue(typeof(Size), "0,0")]
        public Size DropDownMaxSize
        {
            get
            {
                return this.dropDownMaxSize;
            }
            set
            {
                if (this.dropDownMaxSize != value)
                {
                    this.dropDownMaxSize = value;

                    if (this.dropDownMinSize != Size.Empty)
                    {
                        if (this.dropDownMaxSize.Width < this.dropDownMinSize.Width)
                        {
                            this.dropDownMinSize.Width = this.dropDownMaxSize.Width;
                        }
                        if (this.dropDownMaxSize.Height < this.dropDownMinSize.Height)
                        {
                            this.dropDownMinSize.Height = this.dropDownMaxSize.Height;
                        }
                    }
                }
            }
        }

        //TODO
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down minimum size.")]
        [DefaultValue(typeof(Size), "0,0")]
        public Size DropDownMinSize
        {
            get
            {
                return this.dropDownMinSize;
            }
            set
            {
                if (this.dropDownMinSize != value)
                {
                    this.dropDownMinSize = value;

                    if (this.dropDownMaxSize != Size.Empty)
                    {
                        if (this.dropDownMinSize.Width > this.dropDownMaxSize.Width)
                        {
                            this.dropDownMaxSize.Width = this.dropDownMinSize.Width;
                        }
                        if (this.dropDownMinSize.Height > this.dropDownMaxSize.Height)
                        {
                            this.dropDownMaxSize.Height = this.dropDownMinSize.Height;
                        }
                    }
                }
            }
        }

        #endregion

        #region Overrides

        protected override void InitializeFields()
        {
            base.InitializeFields();
            //Special case here - we need an entry in the property store so that we receive PropertyChanged for BindingContext.
            //TODO: We need a better solution here.
            if (this.BindingContext != null)
            {
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.listElement = new RadListElement();
            this.WireEvents();
        }

        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();
            base.DisposeManagedResources();
        }

        protected virtual void WireEvents()
        {            
            this.listElement.SelectedIndexChanged += listElement_SelectedIndexChanged;
            this.listElement.SelectedIndexChanging += listElement_SelectedIndexChanging;
            this.listElement.SelectedValueChanged +=listElement_SelectedValueChanged;
            this.listElement.ItemDataBinding += listElement_ItemDataBinding;
            this.listElement.ItemDataBound +=listElement_ItemDataBound;
            this.listElement.CreatingVisualItem +=listElement_CreatingVisualItem;
            this.listElement.SortStyleChanged += listElement_SortStyleChanged;
            this.listElement.VisualItemFormatting += listElement_VisualItemFormatting;
            this.listElement.DataItemPropertyChanged += listElement_DataItemPropertyChanged;
            this.listElement.ItemsChanged += listElement_ItemsChanged;
            this.PopupOpened += OnPopupOpened;
            this.PopupClosed += OnPopupClosed;
        }   

        protected virtual void UnwireEvents()
        {
            this.listElement.SelectedIndexChanged -= listElement_SelectedIndexChanged;
            this.listElement.SelectedIndexChanging -= listElement_SelectedIndexChanging;
            this.listElement.SelectedValueChanged -= listElement_SelectedValueChanged;
            this.listElement.ItemDataBinding -= listElement_ItemDataBinding;
            this.listElement.ItemDataBound -= listElement_ItemDataBound;
            this.listElement.CreatingVisualItem -= listElement_CreatingVisualItem;
            this.listElement.SortStyleChanged -= listElement_SortStyleChanged;
            this.listElement.VisualItemFormatting -= listElement_VisualItemFormatting;
            this.listElement.DataItemPropertyChanged -= listElement_DataItemPropertyChanged;
            this.listElement.ItemsChanged -= listElement_ItemsChanged;
            this.PopupOpened -= OnPopupOpened;
            this.PopupClosed -= OnPopupClosed;
        }

        #endregion

        #region Events helpers

        void listElement_ItemsChanged(object sender, Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action==Telerik.WinControls.Data.NotifyCollectionChangedAction.Reset)
            {
                if (this.listElement.Items.Count == 0)//reset & no items 
                {
                    PopupEditorNotificationData dataReset = new PopupEditorNotificationData();
                    dataReset.context = PopupEditorNotificationData.Context.ItemsClear;
                    this.NotifyOwner(dataReset);
                }

                return;
            }

            PopupEditorNotificationData data = new PopupEditorNotificationData();
            data.context = PopupEditorNotificationData.Context.ItemsChanged;
            this.NotifyOwner(data);
        }   


        protected virtual void listElement_DataItemPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnPopupClosed(object sender, RadPopupClosedEventArgs args)
        {
        }

        protected virtual void OnPopupOpened(object sender, EventArgs e)
        {
        }

        void listElement_SelectedValueChanged(object sender, EventArgs e)
        {            
            this.NotifyOwner(new PopupEditorNotificationData((ValueChangedEventArgs)e));
        }

        void listElement_SelectedIndexChanging(object sender, PositionChangingCancelEventArgs e)
        {
            this.NotifyOwner(new PopupEditorNotificationData(e));
        }

        void listElement_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            this.NotifyOwner(new PopupEditorNotificationData(e));
        }

        void listElement_ItemDataBinding(object sender, ListItemDataBindingEventArgs args)
        {
            this.NotifyOwner(new PopupEditorNotificationData(args));
        }

        void listElement_ItemDataBound(object sender, ListItemDataBoundEventArgs args)
        {
            this.NotifyOwner(new PopupEditorNotificationData(args));
        }

        void listElement_CreatingVisualItem(object sender, CreatingVisualListItemEventArgs args)
        {
            this.NotifyOwner(new PopupEditorNotificationData(args));
        }

        void listElement_VisualItemFormatting(object sender, VisualItemFormattingEventArgs args)
        {
            this.NotifyOwner(new PopupEditorNotificationData(args));
        }

        void listElement_SortStyleChanged(object sender, SortStyleChangedEventArgs args)
        {
            this.NotifyOwner(new PopupEditorNotificationData(args));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Main entry point for updating DropDownList
        /// </summary>
        /// <param name="notificationData"></param>
        public virtual void NotifyOwner(PopupEditorNotificationData notificationData)
        {
        }
       
        protected virtual void SetDropDownBindingContext()
        {
            this.popup.BindingContext = this.BindingContext;
        }

        //protected override Size GetInitialPopupSize()
        //{
        //    if (!this.IsInValidState(true))
        //        return Size.Empty;
        //    int initialHeight = 0;

        //    int visibleItemsCount = Math.Min(this.maxDropDownItems, this.listElement.Items.Count);

        //    for (int i = 0; i < visibleItemsCount; i++)
        //    {
        //        SizeF desiredSize =  this.listElement.Items[i].Height;
        //        initialHeight += (int)desiredSize.Height;
        //    }

        //    return new Size(this.Size.Width, initialHeight);
        //}

        protected override Size GetPopupSize(RadPopupControlBase popup, bool measure)
        {
            RadSizablePopupControl sizablePopup = popup as RadSizablePopupControl;
            Debug.Assert(sizablePopup != null, "Popup is not an RadSizablePopupControl");

            int itemsCount = this.listElement.Items.Count;
               
            if (itemsCount > defaultItemsCountInDropDown || itemsCount == 0)
            {
                itemsCount = defaultItemsCountInDropDown;
            }

            if (itemsCount > this.maxDropDownItems)
            {
                itemsCount = this.maxDropDownItems;
            }

            int sizingGripSize = 0;
            if(sizablePopup.SizingGrip.Visibility== ElementVisibility.Visible)
            {
                sizingGripSize = sizablePopup.SizingGrip.FullBoundingRectangle.Height;
            }

            int height = itemsCount * (this.ListElement.ItemHeight + this.listElement.Scroller.ItemSpacing) +
                         sizingGripSize +
                         //this.listElement.HScrollBar.FullBoundingRectangle.Height +
                         (int)(this.listElement.BorderWidth * 2);
           
            return new Size(this.Size.Width, height);
        }

        public Size GetDesiredPopupSize()
        {
            return this.GetPopupSize(this.popup, true);
        }
   
        protected override void UpdatePopupMinMaxSize(RadPopupControlBase popup)
        {
            if (this.DropDownMinSize != Size.Empty)
            {
                popup.MinimumSize = this.DropDownMinSize;
            }

            if (this.DropDownMaxSize != Size.Empty)
            {
                popup.MaximumSize = this.DropDownMaxSize;
            }

            
            if (popup.MinimumSize == Size.Empty)
            {
                RadSizablePopupControl sizablePopup = popup as RadSizablePopupControl;
                Debug.Assert(sizablePopup != null, "Popup is not an RadSizablePopupControl");
                float height = (this.listElement.BorderWidth * 2) +
                               (this.listElement.VScrollBar.Visibility == ElementVisibility.Visible  ? this.listElement.VScrollBar.FirstButton.Size.Height +
                               this.listElement.VScrollBar.SecondButton.Size.Height : 0) +
                               //this.listElement.HScrollBar.FullBoundingRectangle.Height +
                               (sizablePopup.SizingGrip.Visibility == ElementVisibility.Collapsed ? 0: sizablePopup.SizingGrip.FullBoundingRectangle.Height);


                //TODO to be refactored - initialy the scroll bar of the list element is visible 
                popup.MinimumSize = new Size(0, (int)Math.Ceiling(this.listElement.Items.Count == 1 ? 0 : height));
            }
        }     
        #endregion
    }
}
