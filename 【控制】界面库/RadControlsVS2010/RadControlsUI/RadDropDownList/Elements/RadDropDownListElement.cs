using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI.Data;
using Telerik.WinControls.Enumerations;
using System.Drawing.Design;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    [RadToolboxItemAttribute(true)]
    public class RadDropDownListElement : PopupEditorElement, ITooltipOwner
    {
        #region Fields
        protected RadDropDownListEditableAreaElement editableElement;
        protected RadArrowButtonElement arrowButton;
        protected BorderPrimitive borderPrimitive;
        protected FillPrimitive fillPrimitive;
        private AutoCompleteMode autoCompleteMode;
        private SizingMode sizingMode;
        private int beginUpdateCount = 0;
        private bool selectNextOnDoubleClick;
        private bool enableMouseWheel = true;
        private AutoCompleteAppendHelper autoCompleteAppend;
        private AutoCompleteSuggestHelper autoCompleteSuggest;
        internal int selectedIndexOnPopupOpen = -1;
        private List<BaseAutoComplete> autoCompleteHelpers = new List<BaseAutoComplete>();
        internal bool isSuggestMode = false;//indicates this as Suggest DropDownList e.g. this is a second drop down
        private bool showImageInEditorArea = false;
        private object owner = null;
        #endregion

        #region Cstors

        static RadDropDownListElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadTextBoxElementStateManager(), typeof(RadDropDownListElement));
            new Telerik.WinControls.UI.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadDropDownList().DeserializeTheme();
        }

        public RadDropDownListElement()
        {        	
        }

        public RadDropDownListElement(object owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Dependency properties

        private static RadProperty DropDownStyleProperty = RadProperty.Register(
            "DropDownStyle", typeof(RadDropDownStyle), typeof(RadDropDownListElement), new RadElementPropertyMetadata(
                RadDropDownStyle.DropDown, ElementPropertyOptions.None));

        private static RadProperty CaseSensitiveProperty = RadProperty.Register(
            "CaseSensitive", typeof(bool), typeof(RadDropDownListElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        public static RadProperty IsDropDownShownProperty = RadProperty.Register(
            "IsDropDownShown", typeof(bool), typeof(RadDropDownListElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));
 
        #endregion

        #region Properties      

        /// <summary>
        /// Gets or sets that RadListDataItem Image will be displayd in Editor Element when DropDownStyle is set to DropDownStyleList
        /// </summary>
        public virtual bool ShowImageInEditorArea
        {
            get
            {
                return  this.showImageInEditorArea = true;              
            }
            set
            {
                this.showImageInEditorArea = value;
            }
        }

        /// <summary>
        /// Gets or sets a Predicate that will be called for every data item in order to determine
        /// if the item will be visible.
        /// </summary>
        public Predicate<RadListDataItem> Filter
        {
            get
            {
                return this.ListElement.Filter;
            }

            set
            {
                this.ListElement.Filter = value;
            }
        }

        /// <summary>
        /// Gets or sets a filter expression that determines which items will be visible.
        /// </summary>
        public string FilterExpression
        {
            get
            {
                return this.ListElement.FilterExpression;
            }

            set
            {
                this.ListElement.FilterExpression = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is a Filter or FilterExpression set.
        /// </summary>
        [Browsable(false)]
        public bool IsFilterActive
        {
            get
            {
                return this.ListElement.IsFilterActive;
            }
        }

        /// <summary>
        /// EditableElement Property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDropDownListEditableAreaElement EditableElement
        {
            get
            {
                return editableElement;
            }
            set
            {
                editableElement = value;
            }
        }       

        /// <summary>
        /// Gets or sets a value that indicates whether items will be sized according to
        /// their content. If this property is true the user can set the Height property of each
        /// individual RadListDataItem in the Items collection in order to override the automatic
        /// sizing.
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Description("Gets or sets a value that indicates whether items will be sized according to their content. If this property is true the user can set the Height property of each individual RadListDataItem in the Items collection in order to override the automatic sizing.")]
        [Browsable(true)]
        public bool AutoSizeItems
        {
            get
            {
                return this.listElement.AutoSizeItems;
            }
            set
            {
                this.listElement.AutoSizeItems = value;
            }
        }

        private bool IsDropDownShown
        {
            get
            {
                return (bool)this.GetValue(RadDropDownListElement.IsDropDownShownProperty);
            }
        }

        /// <summary>
        /// Enable or disable Mouse Wheel Scrolling.
        /// </summary>
        [Description("Indicates whether users can change the selected item by the mouse wheel."),
        Category(RadDesignCategory.BehaviorCategory), DefaultValue(true)]
        public bool EnableMouseWheel
        {
            get
            {
                return enableMouseWheel;
            }
            set
            {
                enableMouseWheel = value;                
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(DesignerConsts.RadListControlDataItemCollectionDesignerString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this RadDropDownList.")]
        public RadListDataItemCollection Items
        {
            get
            {
                return (RadListDataItemCollection)this.ListElement.Items;
            }
        }

        #region TextBox properties

        /// <summary>
        /// Gets or sets the text that is displayed when RadDropDownList has no text set.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        DefaultValue("")]
        public string NullText
        {
            get
            {
                return this.editableElement.NullText;
            }

            set
            {
                this.editableElement.NullText = value;
            }
        }

        /// <summary>
        /// Selects a range of text in the editable portion of the combo box
        /// </summary>
        /// <param name="start">The position of the first character in the current text selection within the text box.</param>
        /// <param name="length">The number of characters to select.</param>
        public void SelectText(int start, int length)
        {
            this.editableElement.Select(start, length);
        }

        /// <summary>
        /// Selects all the text in the editable portion of the combo box.
        /// </summary>
        public void SelectAllText()
        {
            this.editableElement.SelectAll();
        }

        /// <summary>
        /// Gets or sets the text that is selected in the editable portion of the DropDownList.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the text that is selected in the editable portion of the DropDownList."),
        Browsable(false),
        DefaultValue("")]
        public string SelectedText
        {
            get
            {
                return this.editableElement.SelectedText;
            }
            set
            {
                this.editableElement.SelectedText = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of characters selected in the editable portion of the combo box.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the number of characters selected in the editable portion of the combo box."),
        DefaultValue(0)]
        public int SelectionLength
        {
            get
            {
                return this.editableElement.SelectionLength;
            }

            set
            {
                this.editableElement.SelectionLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the starting index of text selected in the combo box.
        /// </summary>
        [Description("Gets or sets the starting index of text selected in the combo box."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        DefaultValue(0)]
        public int SelectionStart
        {
            get
            {
                return this.editableElement.SelectionStart;
            }

            set
            {
                this.editableElement.SelectionStart = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the maximum number of characters the user can type or paste into the text box control.
        /// </summary>
        [Description("Gets or sets the maximum number of characters the user can type or paste into the text box control."),
        Category(RadDesignCategory.BehaviorCategory), DefaultValue(0)]
        public int MaxLength
        {
            get
            {
                return this.editableElement.MaxLength;
            }

            set
            {
                this.editableElement.MaxLength = value;
            }
        }

        /// <summary>
        /// Specifies the mode for the automatic completion feature used in the DropDownList 
        /// and the TextBox controls.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(System.Windows.Forms.AutoCompleteMode.None), EditorBrowsable(EditorBrowsableState.Always),
        Description("Specifies the mode for the automatic completion feature used in the DropDownList and TextBox controls.")]
        public AutoCompleteMode AutoCompleteMode
        {
            get
            {
                return this.autoCompleteMode;
            }
            set
            {
                if (this.autoCompleteMode == value)
                {
                    return;	
                }

                this.autoCompleteMode = value;
                foreach (BaseAutoComplete helper in this.autoCompleteHelpers)
                {
                    helper.Dispose();
                }

                this.autoCompleteHelpers.Clear();

                this.autoCompleteAppend = null;
                this.autoCompleteSuggest = null;

                if ((this.autoCompleteMode & AutoCompleteMode.Append) != 0)
                {
                    this.autoCompleteAppend = new AutoCompleteAppendHelper(this);
                    this.autoCompleteHelpers.Add(this.autoCompleteAppend);
                }

                if ((this.autoCompleteMode & AutoCompleteMode.Suggest) != 0)
                {
                    this.autoCompleteSuggest = new AutoCompleteSuggestHelper(this);
                    this.autoCompleteHelpers.Add(this.autoCompleteSuggest);
                }
            }
        }

        //TODO
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down sizing mode. The mode can be: horizontal, veritcal or a combination of them.")]
        [DefaultValue(SizingMode.None)]
        public SizingMode DropDownSizingMode
        {
            get
            {
                return this.sizingMode;
            }
            set
            {
                this.sizingMode = value;
                if (this.popup != null)
                {
                    this.popup.SizingMode = this.sizingMode;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value specifying the style of the combo box. 
        /// </summary>        
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]     
        [Description("Gets or sets a value specifying the style of the combo box."),
        RefreshProperties(RefreshProperties.Repaint)]
        public RadDropDownStyle DropDownStyle
        {
            get
            {
                return (RadDropDownStyle)this.GetValue(DropDownStyleProperty);
            }
            set
            {
                this.SetValue(DropDownStyleProperty, value);
                this.editableElement.DropDownStyle = value;
            }
        }

        /// <commentsfrom cref="RadListElement.SelectedValue" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RadDescription("SelectedValue", typeof(RadListElement)),
        Browsable(false),
        Bindable(true)]
        public Object SelectedValue
        {
            get
            {                
                return this.ListElement.SelectedValue;
            }
            set
            {
                this.ListElement.SelectedValue = value;
            }
        }

        /// <commentsfrom cref="RadListElement.SelectedIndex" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        RadDescription("SelectedIndex", typeof(RadListElement))]
        public virtual int SelectedIndex
        {
            get
            {
                return this.ListElement.SelectedIndex;
            }
            set
            {
                this.ListElement.SelectedIndex = value;
            }
        }

        /// <commentsfrom cref="RadListElement.SelectedItem" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RadDescription("SelectedItem", typeof(RadListElement)),
        Browsable(false),
        Bindable(true)]
        public virtual RadListDataItem SelectedItem
        {
            get
            {
                return this.ListElement.SelectedItem;
            }
            set
            {
                this.ListElement.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to stop the selection events from firing. These are SelectedIndexChanged,
        /// SelectedIndexChanging and SelectedValueChanged.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SuspendSelectionEvents
        {
            get
            {
                return this.ListElement.SuspendSelectionEvents;
            }

            set
            {
                this.ListElement.SuspendSelectionEvents = value;
            }
        }

        /// <summary>
        /// For information on this property please refer to the MSDN.
        /// </summary>
        [DefaultValue(SelectionMode.One)]
        public SelectionMode SelectionMode
        {
            get
            {
                return this.ListElement.SelectionMode;
            }

            set
            {
                this.ListElement.SelectionMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the object that is responsible for providing data objects for the RadListElement.
        /// </summary>
        public object DataSource
        {
            get
            {
                return this.ListElement.DataSource;
            }
            set
            {
                this.ListElement.DataSource = value;
                if (value == null)
                {
                    this.EditableElementText = "";
                }
            }
        }

        /// <summary>
        /// Gets or sets a string which will be used to get a text string for each visual item. This value can not be set to null. Setting
        /// it to null will cause it to contain an empty string.
        /// </summary>
        public string DisplayMember
        {
            get
            {
                return this.ListElement.DisplayMember;
            }
            set
            {
                if (this.ListElement.DisplayMember == value)
                {
                    return;	
                }

                this.ListElement.DisplayMember = value;

                PopupEditorNotificationData notificationData = new PopupEditorNotificationData();
                notificationData.context = PopupEditorNotificationData.Context.DisplayMemberChanged;
                this.NotifyOwner(notificationData);
            }
        }

        /// <summary>
        /// Gets or sets the string through which the SelectedValue property will be determined. This property can not be set to null.
        /// Setting it to null will cause it to contain an empty string.
        /// </summary>
        public string ValueMember
        {
            get
            {
                return this.ListElement.ValueMember;
            }
            set
            {
                if (this.ListElement.ValueMember == value)
                {
                    return;
                }

                this.ListElement.ValueMember = value;

                PopupEditorNotificationData notificationData = new PopupEditorNotificationData();
                notificationData.context = PopupEditorNotificationData.Context.ValueMemberChanged;
                this.NotifyOwner(notificationData);
            }
        }

        /// <summary>
        /// Gets or sets the item height for the items.
        /// </summary>
        public int ItemHeight
        {
            get
            {
                return this.ListElement.ItemHeight;
            }
            set
            {
                this.ListElement.ItemHeight = value;
            }
        }        

        public void SelectAll()
        {
            this.ListElement.SelectAll();
        }

        public void SelectRange(int startIndex, int endIndex)
        {
            this.ListElement.SelectRange(startIndex, endIndex);
        }

        /// <summary>
        /// TextBox Property
        /// </summary>
        public RadDropDownTextBoxElement TextBox
        {
            get
            {
                return this.editableElement.TextBox;
            }
            set
            {
                this.editableElement.TextBox = value;
                this.InvalidateMeasure();
            }
        }

        /// <summary>
        /// ArrowButton Property
        /// </summary>
        public RadArrowButtonElement ArrowButton
        {
            get
            {
                return arrowButton;
            }
            set
            {
                arrowButton = value;
                this.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether string comparisons are case-sensitive.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether string comparisons are case-sensitive."),
        DefaultValue(false)]
        public bool CaseSensitive
        {
            get
            {
                return (bool)this.GetValue(CaseSensitiveProperty);
            }
            set
            {
                this.SetValue(CaseSensitiveProperty, value);
            }
        }

        /// <summary>
        /// Rotate items on double click in the edit box part
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Rotate items on double click in the edit box part"),
        DefaultValue(false)]
        public bool SelectNextOnDoubleClick
        {
            get
            {
                return this.selectNextOnDoubleClick;
            }
            set
            {
                this.selectNextOnDoubleClick = value;
            }
        }

        [Browsable(false),
        RadDescription("FormatInfo", typeof(RadDropDownListElement)),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        public IFormatProvider FormatInfo
        {
            get
            {
                return this.listElement.FormatInfo;
            }
            set
            {
                this.listElement.FormatInfo = value;
            }
        }
    
        //Editor(typeof(FormatStringEditor), typeof(UITypeEditor)), TODO put here real Typeui editor        
        public string FormatString
        {
            get
            {
                return this.listElement.FormatString;
            }
            set
            {
                this.listElement.FormatString = value;
            }
        }
     
        public bool FormattingEnabled
        {
            get
            {
                return this.listElement.FormattingEnabled;
            }
            set
            {
                this.listElement.FormattingEnabled = value;
            }
        }

        /// <summary>
        ///		Gets or sets the type of the DropDown animation.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]        
        [Description("Gets or sets the type of the DropDown animation.")]
        public RadEasingType DropDownAnimationEasing
        {
            get
            {
                return this.PopupForm.AnimationProperties.EasingType;
            }
            set
            {
                this.PopupForm.AnimationProperties.EasingType = value;                
            }
        }
        
        /// <summary>
        ///	Gets or sets a value indicating whether the RadDropDownList will be animated when displaying.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the RadDropDownList will be animated when displaying.")]
        public bool DropDownAnimationEnabled
        {
            get
            {
                return this.PopupForm.AnimationEnabled;
            }
            set
            {
                this.PopupForm.AnimationEnabled = value;
            }
        }

        public SortStyle SortStyle
        {
            get
            {
                return this.listElement.SortStyle;
            }
            set
            {
                this.listElement.SortStyle = value;
            }
        }

        /// <summary>
        ///		Gets or sets the number of frames that will be used when the DropDown is being animated.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(1)]
        [Description("Gets or sets the number of frames that will be used when the DropDown is being animated.")]
        public int DropDownAnimationFrames
        {
            set
            {
                this.Popup.AnimationFrames = value;
            }
            get
            {
                return this.Popup.AnimationFrames;
            }
        }

        /// <summary>
        /// AutoCompleteSuggest Property
        /// </summary>
        public virtual AutoCompleteSuggestHelper AutoCompleteSuggest
        {
            get
            {
                return autoCompleteSuggest;
            }
            set
            {
                autoCompleteSuggest = value;
            }
        }

        /// <summary>
        /// AutoCompleteAppend Property
        /// </summary>
        public virtual AutoCompleteAppendHelper AutoCompleteAppend
        {
            get
            {
                return autoCompleteAppend;
            }
            set
            {
                autoCompleteAppend = value;
            }
        }

        #endregion

        #region Public Methods

        public void BeginUpdate()
        {
            this.beginUpdateCount++;
            //this.ListElement.BeginUpdate();
        }

        public void EndUpdate()
        {
            if (this.beginUpdateCount > 0)
            {
                this.beginUpdateCount--;
            }
        }

        /// <summary>
        /// Defers the refresh.
        /// </summary>
        /// <returns></returns>
        public virtual IDisposable DeferRefresh()
        {
            this.BeginUpdate();
            return new DeferHelper(this);
        }

        private class DeferHelper : IDisposable
        {
            private RadDropDownListElement listElement;

            public DeferHelper(RadDropDownListElement listElement)
            {
                this.listElement = listElement;
            }

            public void Dispose()
            {
                if (this.listElement != null)
                {
                    this.listElement.EndUpdate();
                    this.listElement = null;
                }
            }
        }

        #endregion

        #region Events

        public event ValueChangedEventHandler SelectedValueChanged;
        public event PositionChangedEventHandler SelectedIndexChanged;
        public event PositionChangingEventHandler SelectedIndexChanging;
        public event ListItemDataBindingEventHandler ItemDataBinding;
        public event ListItemDataBoundEventHandler ItemDataBound;
        public event CreatingVisualListItemEventHandler CreatingVisualItem;
        public event SortStyleChangedEventHandler SortStyleChanged;
        public event VisualListItemFormattingEventHandler VisualItemFormatting;

        internal protected KeyEventHandler InternalKeyDown;
        internal protected KeyEventHandler InternalKeyUp;
        internal protected KeyPressEventHandler InternalKeyPress;

        #endregion

        #region Events Management

        protected void OnSelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            this.SyncEditorElementWithSelectedItem();         

            if (this.SelectedIndexChanged != null)
            {
                this.SelectedIndexChanged(this, e);
            } 
        }

        protected bool OnSelectedIndexChanging(object sender, PositionChangingCancelEventArgs e)
        {
            if (this.SelectedIndexChanging != null)
            {
                this.SelectedIndexChanging(this, e);
            }

            if (e.Cancel)
            {
                return true;
            }

            if (e.Position == -1)
            {
                int textPosition = this.SelectItemFromText(this.EditableElementText, false);
                if (textPosition == -1)
                {
                    return false;
                }
                else
                {
                    this.EditableElementText = "";
                }
            }

            return false;
        }

        protected void OnSelectedValueChanged(object sender, ValueChangedEventArgs e)
        {
            this.SyncVisualProperties(this.SelectedItem);
            if (this.SelectedValueChanged != null)
            {
                this.SelectedValueChanged(this, e);
            }
        }        

        protected void OnListItemDataBinding(object sender, ListItemDataBindingEventArgs args)
        {
            if (this.ItemDataBinding != null)
            {
                this.ItemDataBinding(sender, args);
            }            
        }

        protected void OnListItemDataBound(object sender, ListItemDataBoundEventArgs args)
        {
            if (this.ItemDataBound != null)
            {
                this.ItemDataBound(sender, args);
            }
        }

        protected void OnVisualElementCreated(object sender, CreatingVisualListItemEventArgs args)
        {
            if (this.CreatingVisualItem != null)
            {
                this.CreatingVisualItem(sender, args);
            }
        }

        protected void OnKeyPress(RadDropDownListEditableAreaElement sender, KeyPressEventArgs e)
        {
            this.HandleAutoComplete(e);

            this.HandleEnter(e);

            this.ProccesListFastNavigationInDropDownListMode(e.KeyChar);
        }

        private void HandleEnter(KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
            {
                return;
            }

            this.HandleEnterCore();
        }

        private void HandleEnterCore()
        {
            this.SelectAllText();
            this.SelectItemFromText(EditableElementText, false);
        }

        internal protected void EnterPressedOrLeaveControl()
        {
            this.HandleEnterCore();
        }

        private void HandleAutoComplete(KeyPressEventArgs e)
        {
            if (this.DropDownStyle == RadDropDownStyle.DropDownList)//turn off auto complete
            {
                return;
            }

            if (this.autoCompleteSuggest != null)
            {
                this.autoCompleteSuggest.AutoComplete(e);
            }

            if (this.autoCompleteAppend != null)
            {
                this.autoCompleteAppend.AutoComplete(e);
            }
        }

        private void ProccesListFastNavigationInDropDownListMode(char pressedChar)
        {
            if (this.DropDownStyle != RadDropDownStyle.DropDownList)
            {
                return;
            }

            this.listElement.ProcessKeyboardSearch(pressedChar);
        }

        /// <summary>
        /// Get or set the text in Editable arrea
        /// </summary>
        public virtual string EditableElementText
        {
            get
            {
                return this.editableElement.TextBox.TextBoxItem.HostedControl.Text;
            }
            set
            {
                this.editableElement.TextBox.TextBoxItem.HostedControl.Text = value;
            }
        }

        protected void OnTextChanged(RadDropDownListEditableAreaElement sender, EventArgs args)
        {
            if (this.EditableElementText == "" && !this.skipSelectionClear)
            {
                this.SelectedIndex = -1;
            }

            if (!this.IsPopupOpen)
            {
                return;
            }

            string textFromEditor = this.EditableElementText;
            this.ScrollToItemFromText(textFromEditor);     
        }

        protected virtual void OnSortStyleChanged(SortStyle sortStyle)
        {
            if (this.SortStyleChanged != null)
            {
                this.SortStyleChanged(this, new SortStyleChangedEventArgs(sortStyle));
            }
        }

        protected internal virtual void OnVisualItemFormatting(RadListVisualItem item)
        {
            if (this.VisualItemFormatting != null)
            {
                this.VisualItemFormatting(this, new VisualItemFormattingEventArgs(item));
            }
        }

        internal int SelectItemFromText(string text)
        {
            return this.SelectItemFromText(text, true);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but matches an item only if its text is exactly equal to the provided string.
        /// </summary>
        public virtual int FindStringExact(string s)
        {
            return this.ListElement.FindStringExact(s, 0);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but matches an item only if its text is exactly equal to the provided string.
        /// </summary>
        public virtual int FindStringExact(string s, int startIndex)
        {
            return this.ListElement.FindStringExact(s, startIndex);
        }

        //TODO replace with find string exact
        private int SelectItemFromText(string text, bool syncEditorElementWithSelectedItem)
        {
            //this fix issue:
            StringComparison stringComparison = this.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;           

            //handle issue
            //items (aaa, bbb, bbb). Click in text field and press twice down arrow. SelectedIndex is 2. Open drop down and you will see that the selected item is first 'bbb'.

            if (this.listElement.SelectedIndex > -1)
            {
                if (text.Equals(this.listElement.SelectedItem.CachedText, stringComparison))
                {
                    if (syncEditorElementWithSelectedItem)
                    {
                        this.SyncEditorElementWithSelectedItem();
                    }

                    return this.listElement.SelectedIndex;
                }
            }

            int count = this.Items.Count;
            for (int i = 0; i < count; ++i)
            {   
                if (text.Equals(this.Items[i].Text, stringComparison))
                {
                    this.ListElement.SelectedIndex = i;
                    if (syncEditorElementWithSelectedItem)
                    {
                        this.SyncEditorElementWithSelectedItem();
                    }

                    return i;
                }
            }

            if (syncEditorElementWithSelectedItem)
            {
                this.BeginUpdate();
                this.listElement.SelectedIndex = -1;
                this.EndUpdate();
            }
           
            return -1;
        }

        private void ScrollToItemFromText(string text)
        {
            StringComparison stringComparison = this.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
           
            if (this.ScrollToActiveItem(text, stringComparison))
            {
                return;
            }
            //no active item
            if (this.ScrollToItemText(text, stringComparison))
            {
                return;
            }
            //no matched text
            this.ScrollToFirstItem();
        }

        private bool ScrollToItemText(string text, StringComparison stringComparison)
        {
            int count = this.Items.Count;
            for (int i = 0; i < count; ++i)
            {
                if (text.Equals(this.listElement.Items[i].Text, stringComparison))
                {
                    this.listElement.ScrollToItem(this.listElement.Items[i]);                    
                    return true;
                }
            }

            return false;
        }

        private bool ScrollToActiveItem(string text, StringComparison stringComparison)
        {
            if (this.listElement.SelectedIndex > -1)
            {
                if (text.Equals(this.listElement.SelectedItem.CachedText, stringComparison))
                {
                    this.listElement.ScrollToActiveItem();
                    return true;
                }
            }
            return false;
        }

        private void ScrollToFirstItem()
        {
            if (this.listElement.Items.Count > 0)
            {
                this.listElement.ScrollToItem(this.listElement.Items[0]);
            }
        }

        #endregion

        #region Overrides

        protected override void OnLoaded()
        {
            base.OnLoaded();
            //fixed very strange bug with text 
            //TFS ID# 106558
            this.BeginUpdate();
            string oldText = this.TextBox.Text;
            this.TextBox.Text = string.Empty;
            this.TextBox.Text = oldText;
            this.EndUpdate();
        }
         
        public override bool RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
                this.listElement.RightToLeft = value;
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.CreateTextEditorElement();
            this.CreateArrowButtonElement();

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "DropDownListBorder";
           
            this.Children.Add(this.borderPrimitive);

            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.BindProperty(FillPrimitive.AutoSizeModeProperty, this, RadElement.AutoSizeModeProperty, PropertyBindingOptions.TwoWay);
            this.fillPrimitive.Class = "DropDownFill";
            this.fillPrimitive.ZIndex = -1;
            this.fillPrimitive.RadPropertyChanged += fillPrimitive_RadPropertyChanged;
            this.Children.Add(this.fillPrimitive);
            this.BindProperty(RadItem.TextProperty, this.editableElement, RadItem.TextProperty, PropertyBindingOptions.TwoWay);
            this.BindProperty(LightVisualElement.ImageProperty, this.editableElement, LightVisualElement.ImageProperty, PropertyBindingOptions.TwoWay);
            this.editableElement.TextBox.BindProperty(VisualElement.BackColorProperty, this.fillPrimitive, VisualElement.BackColorProperty, PropertyBindingOptions.TwoWay);
            //this.fillPrimitive.BindProperty(VisualElement.BackColorProperty, this.editableElement.TextBox, VisualElement.BackColorProperty, PropertyBindingOptions.TwoWay);
        }

        void fillPrimitive_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Property != VisualElement.BackColorProperty)
            {
                return;
            }

            this.editableElement.SetDefaultValueOverride(VisualElement.BackColorProperty, e.NewValue);
        }

        protected override void DisposeManagedResources()
        {
            this.arrowButton.Click -= arrowButton_Click;
            this.fillPrimitive.RadPropertyChanged -= fillPrimitive_RadPropertyChanged;
            base.DisposeManagedResources();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!this.enableMouseWheel)
            {
                return;
            }

            if (!this.IsPopupVisible)
            {
                this.HandleSelectNextOrPrev(e.Delta < 0, false);
            }
            
            if (e is HandledMouseEventArgs)
            {
                (e as HandledMouseEventArgs).Handled = true;
            }
        }        

        /// <summary>
        /// Gets a value that indicates if the popup associated with this RadDropDownListElement is open.
        /// </summary>
        public bool IsPopupVisible
        {
            get
            {
                return PopupManager.Default.ContainsPopup(this.Popup);
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (!this.selectNextOnDoubleClick)
            {
                return;
            }

            this.HandleSelectNextOrPrev(true, true);
        }        

        public override bool Focus()
        {
            return this.editableElement.Focus();
        }

        protected override void listElement_DataItemPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (sender != this.listElement.ActiveItem)
            {
                return;
            }

            //if (e.Property == LightVisualElement.TextProperty)
            //{
            //    this.EditableElementText = (string)e.NewValue;
            //}

            this.SyncVisualProperties(this.listElement.ActiveItem);
        }

        protected int oldSelectedIndex = -1;

        protected virtual void OnItemsChanged()
        {
            if (this.SelectedItem != null)
            {
                this.EditableElementText = this.SelectedItem.Text;
            }
            else
            {
                if (this.oldSelectedIndex!=-1)
                {
                    this.EditableElementText = "";
                }
            }
        }

        protected override void OnPopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            this.arrowButton.SetValue(RadDropDownListElement.IsDropDownShownProperty, false);
            this.SetValue(RadDropDownListElement.IsDropDownShownProperty, false);
        }

        protected override void OnPopupOpened(object sender, EventArgs e)
        {
            this.arrowButton.SetValue(RadDropDownListElement.IsDropDownShownProperty, true);
            this.SetValue(RadDropDownListElement.IsDropDownShownProperty, true);

            if (!this.AutoSizeItems)
            {
                return;
            }

            //foreach (RadListDataItem item in this.listElement.Items)
            //{
            //    item.MeasuredSize = SizeF.Empty;
            //}

            this.listElement.Scroller.UpdateScrollRange();
            this.listElement.ViewElement.UpdateItems();
            this.listElement.InvalidateMeasure(true);
            this.listElement.InvalidateArrange(true);
            this.listElement.UpdateLayout();
            this.listElement.PerformLayout();
        }

        protected override void OnAutoCompeleteDataSourceChanged()
        {
            this.SyncAutoCompleteSuggestHelperDataSource();
        }

        private void SyncAutoCompleteSuggestHelperDataSource()
        {
            if (this.autoCompleteSuggest != null)
            {
                this.autoCompleteSuggest.AutoCompleteDataSource = this.AutoCompleteDataSource;
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadObject.BindingContextProperty)
            {
                this.SetDropDownBindingContext();
            }

            if (e.Property == RadElement.RightToLeftProperty)
            {
                this.listElement.RightToLeft = this.RightToLeft;
            }
        }

        #endregion

        #region Layouts

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            //base.MeasureOverride(availableSize);       

            availableSize = GetClientRectangle(availableSize).Size;
            SizeF result = SizeF.Empty;
            
            this.ArrowButton.Measure(availableSize);
            SizeF arrowButtonDesiredSize = this.ArrowButton.DesiredSize;
            SizeF editableElementAvailableSize = new SizeF(availableSize.Width - arrowButtonDesiredSize.Width, availableSize.Height);

            this.editableElement.Measure(editableElementAvailableSize);
            SizeF editableElementDesiredSize = this.editableElement.DesiredSize;

            result.Width += editableElementDesiredSize.Width + arrowButtonDesiredSize.Width;
            result.Height = Math.Max(editableElementDesiredSize.Height, arrowButtonDesiredSize.Height);
            return result;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            
            //arrange button
            float arrowButtonWidth = this.arrowButton.DesiredSize.Width;
            float arrowLeftPos = this.RightToLeft ? 0 : finalSize.Width - arrowButtonWidth;
            RectangleF arrowArea = new RectangleF(arrowLeftPos, 0, arrowButtonWidth, finalSize.Height);
            this.arrowButton.Arrange(arrowArea);

            //arrange text
            float contentLeftPos = this.RightToLeft ? arrowButtonWidth : 0;
            RectangleF textArea = new RectangleF(contentLeftPos, 0, finalSize.Width - arrowButtonWidth, finalSize.Height);
            this.editableElement.Arrange(textArea);

            return finalSize;
        }


        protected virtual RectangleF GetClientRectangle(SizeF finalSize)
        {
            Padding padding = this.Padding;
            RectangleF clientRect = new RectangleF(padding.Left, padding.Top,
                finalSize.Width - padding.Horizontal, finalSize.Height - padding.Vertical);

            if (this.borderPrimitive.Visibility== ElementVisibility.Visible)
            {
                Padding thickness = this.GetBorderThickness(false);
                clientRect.X += thickness.Left;
                clientRect.Y += thickness.Top;
                clientRect.Width -= thickness.Horizontal;
                clientRect.Height -= thickness.Vertical;
            }

            clientRect.Width = Math.Max(0, clientRect.Width);
            clientRect.Height = Math.Max(0, clientRect.Height);

            return clientRect;
        }

        protected internal virtual Padding GetBorderThickness(bool checkDrawBorder)
        {
           
            Padding thickness = Padding.Empty;

            if (borderPrimitive.BoxStyle == BorderBoxStyle.SingleBorder)
            {
                thickness = borderPrimitive.BorderThickness;
            }
            else if (borderPrimitive.BoxStyle == BorderBoxStyle.FourBorders)
            {
                thickness = new Padding((int)borderPrimitive.LeftWidth, (int)borderPrimitive.TopWidth, (int)borderPrimitive.RightWidth, (int)borderPrimitive.BottomWidth);
            }
            else if (borderPrimitive.BoxStyle == BorderBoxStyle.OuterInnerBorders)
            {
                int borderWidth = borderPrimitive.BorderThickness.All;
                if (borderWidth == 1)
                {
                    borderWidth = 2;
                }
                thickness = new Padding(borderWidth);
            }

            return thickness;
        }


        protected override RadPopupControlBase CreatePopupForm()
        {
            this.popup = new DropDownPopupForm(this);
            this.popup.VerticalAlignmentCorrectionMode = AlignmentCorrectionMode.SnapToOuterEdges;
            this.popup.SizingMode = this.sizingMode;
            this.popup.Height = this.DropDownHeight;
            this.popup.HorizontalAlignmentCorrectionMode = AlignmentCorrectionMode.Smooth;
            this.popup.RightToLeft = this.RightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.Inherit;            
            this.WirePopupFormEvents(this.popup);

            return popup;
        }

        public override void ShowPopup()
        {
            base.ShowPopup();  
            if (!string.IsNullOrEmpty(this.Text))
            {
                this.SelectItemFromText(this.Text);
            }

            this.selectedIndexOnPopupOpen = this.listElement.SelectedIndex;//keep the selection            
            this.listElement.ScrollToActiveItem();

            this.RemoveSelectionInAutoSuggestPopup();
        }

        private void RemoveSelectionInAutoSuggestPopup()
        {
            if (!this.isSuggestMode)
            {
                return;
            }

            this.BeginUpdate();
            this.SelectedIndex = -1;
            this.EndUpdate();
        }

        protected internal override void ClosePopup(RadPopupCloseReason reason)
        {
            base.ClosePopup(reason);
            this.selectedIndexOnPopupOpen = -1;
        }

        protected internal virtual void ClosePopupCore()
        {
            if (this.listElement.IsFilterActive)
            {
                this.SyncEditorElementWithActiveItem();
            }
           
            this.Focus();
        }

        protected virtual void OnItemsClear()
        {
            this.EditableElementText = "";
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Searches for an item related to the specified string. The relation is described by the object assigned to FindStringComparer property.
        /// By default FindStringComparer uses the System.String.StartsWith() method.
        /// This method starts searching from the specified index. If the algorithm reaches the end of the Items collection it wraps to the beginning
        /// and continues until one before the provided index.
        /// </summary>
        /// <param name="s">The string with which every item will be compared.</param>       
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindString(string s)
        {
            return this.listElement.FindString(s, 0);
        }

        /// <summary>
        /// Searches for an item related to the specified string. The relation is described by the object assigned to FindStringComparer property.
        /// By default FindStringComparer uses the System.String.StartsWith() method.
        /// This method starts searching from the specified index. If the algorithm reaches the end of the Items collection it wraps to the beginning
        /// and continues until one before the provided index.
        /// </summary>
        /// <param name="s">The string with which every item will be compared.</param>
        /// <param name="startIndex">The index from which to start searching.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindString(string s, int startIndex)
        {
            return this.listElement.FindString(s, startIndex);
        }

        protected void HandleOnKeyUpKeyDownPress(KeyEventArgs keyEventArgs)
        {
            if (this.autoCompleteSuggest != null && autoCompleteSuggest.DropDownList.IsPopupOpen)//routed the event to Suggest helper
            {
                this.autoCompleteSuggest.HandleSelectNextOrPrev(keyEventArgs.KeyCode == Keys.Down);
                keyEventArgs.Handled = true;
                return;
            }

            this.listElement.ProcessKeyboardSelection(keyEventArgs.KeyCode);
            keyEventArgs.Handled = true;
        }

        protected virtual void HandleSelectNextOrPrev(bool next, bool startFromBeginningIfEndReached)
        {
            if (this.autoCompleteSuggest != null && autoCompleteSuggest.DropDownList.IsPopupOpen)//routed the event to Suggest helper
            {
                this.autoCompleteSuggest.HandleSelectNextOrPrev(next);
            }
            else
            {
                int selectedIndex = this.listElement.SelectedIndex;
                int itemsCount = this.listElement.Items.Count - 1;
                selectedIndex = startFromBeginningIfEndReached ? this.ClampSelectedIndexWithReverse(next, selectedIndex, itemsCount) : this.ClampSelectedIndex(next, selectedIndex, itemsCount);
                this.listElement.SelectedIndex = selectedIndex;                
                this.SyncEditorElementWithSelectedItem();
            }
        }

        internal int ClampSelectedIndex(bool down, int selectedIndex, int itemsCount)
        {
            if (down)
            {
                if (selectedIndex < itemsCount)
                {
                    selectedIndex++;
                }
            }
            else
            {
                if (selectedIndex > 0)
                {
                    selectedIndex--;
                }
            }

            return selectedIndex;
        }

        internal int ClampSelectedIndexWithReverse(bool down, int selectedIndex, int itemsCount)
        {
            if (down)
            {
                selectedIndex++;                    
                if (selectedIndex > itemsCount)
                {
                    selectedIndex = 0;	
                }
            }
            else
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex = itemsCount;
                }                
            }

            return selectedIndex;
        }

        bool skipSelectionClear = false;

        internal virtual void SyncEditorElementWithSelectedItem()
        {
            RadListDataItem listItem = this.listElement.SelectedItem;
            this.oldSelectedIndex = this.SelectedIndex;

            if (listItem == null)//keep the current text in editable portion if selectedIndex is -1
            {
                return;
            }

            if (listItem.Text == "")
            {
                this.skipSelectionClear = true;
            }

            this.SyncVisualProperties(listItem);
            this.skipSelectionClear = false;

            if (!this.isSuggestMode && this.EditableElement.SelectionLength != this.EditableElementText.Length)
            {
                this.SelectAllText();
            }
        }

        protected virtual void SyncVisualProperties(RadListDataItem listItem)
        {
            if (listItem == null )
            {
                return;
            }

            if (!listItem.Selected)
            {
                return;
            }

            if (this.listElement.SuspendSelectionEvents)
            {
                return;
            }
            
            this.EditableElementText = listItem.Text;
            this.editableElement.Text = listItem.Text;
            if (this.DropDownStyle == RadDropDownStyle.DropDown)
            {
                return;
            }

            if (!this.showImageInEditorArea)
            {
                return;
            }

            this.EditableElement.Image = listItem.Image;
            this.EditableElement.ImageAlignment = listItem.ImageAlignment;
            this.EditableElement.TextAlignment = listItem.TextAlignment;
            this.EditableElement.TextImageRelation = listItem.TextImageRelation;
        }

        internal virtual void SyncEditorElementWithActiveItem()
        {
            RadListDataItem listItem = this.listElement.ActiveItem;
            if (listItem == null)
            {
                this.listElement.SelectedIndex = -1;
                return;
            }

            this.listElement.SelectedItem = this.listElement.ActiveItem;
            this.EditableElementText = listItem.Text;
            this.SelectAllText();
        }

        protected virtual void CreateTextEditorElement()
        {
            this.editableElement = new RadDropDownListEditableAreaElement(this);            
            this.BindProperty(RadTextBoxItem.IsNullTextProperty, this.editableElement.TextBox.TextBoxItem, RadTextBoxItem.IsNullTextProperty, PropertyBindingOptions.OneWay);
            this.Children.Add(this.editableElement);
        }      

        protected virtual void CreateArrowButtonElement()
        {
            this.arrowButton = new RadDropDownListArrowButtonElement();
            this.arrowButton.Border.SetDefaultValueOverride(VisualElement.VisibilityProperty, ElementVisibility.Collapsed); 
            this.arrowButton.MinSize = new Size(RadArrowButtonElement.RadArrowButtonDefaultSize.Width, this.arrowButton.ArrowFullSize.Height);          
            this.arrowButton.ClickMode = ClickMode.Press;
            this.arrowButton.Click += arrowButton_Click;
            this.arrowButton.ZIndex = 1;
            this.Children.Add(this.arrowButton);
        }

        void arrowButton_Click(object sender, EventArgs e)
        {
            this.EditableElement.Entering = false;
            this.TooglePopupState();
        }

        protected virtual void ProcessKeyDown(object sender, KeyEventArgs e)
        {
        }

        protected virtual void ProcessKeyUp(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// main update entry point
        /// </summary>
        /// <param name="notificationData">contains notification context</param>
        public override void NotifyOwner(PopupEditorNotificationData notificationData)
        {
            base.NotifyOwner(notificationData);

            if (notificationData.context == PopupEditorNotificationData.Context.VisualItemFormatting)
            {
                this.OnVisualItemFormatting(notificationData.visualItemFormatting.VisualItem);
                return;
            }

            if (beginUpdateCount != 0)
            {
                return;
            }           

            switch (notificationData.context)
            {
                case PopupEditorNotificationData.Context.None:
                    break;
                case PopupEditorNotificationData.Context.SelectedIndexChanged:
                    this.OnSelectedIndexChanged(this.listElement, notificationData.positionChangedEventArgs);
                    break;
                case PopupEditorNotificationData.Context.SelectedIndexChanging:
                    this.OnSelectedIndexChanging(this.listElement, notificationData.positionChangingCancelEventArgs);
                    break;
                case PopupEditorNotificationData.Context.SelectedValueChanged:
                    this.OnSelectedValueChanged(this.listElement, notificationData.valueChangedEventArgs);
                    break;
                case PopupEditorNotificationData.Context.ListItemDataBinding:
                    this.OnListItemDataBinding(this.listElement, notificationData.listItemDataBindingEventArgs);
                    break;
                case PopupEditorNotificationData.Context.ListItemDataBound:
                    this.OnListItemDataBound(this.listElement, notificationData.listItemDataBoundEventArgs);
                    break;
                case PopupEditorNotificationData.Context.CreatingVisualItem:
                    this.OnVisualElementCreated(this.listElement, notificationData.creatingVisualListItemEventArgs);
                    break;
                case PopupEditorNotificationData.Context.KeyPress:
                    this.OnKeyPress(this.editableElement, notificationData.keyPressEventArgs);
                    break;
                case PopupEditorNotificationData.Context.TextChanged:
                    this.OnTextChanged(this.editableElement, new EventArgs());
                    break;
                case PopupEditorNotificationData.Context.SortStyleChanged:
                    this.OnSortStyleChanged(notificationData.sortStyleChanged.SortStyle);
                    break;
                case PopupEditorNotificationData.Context.MouseWheel:
                    this.OnMouseWheel(notificationData.mouseEventArgs);
                    break;
                case PopupEditorNotificationData.Context.TextBoxDoubleClick:
                    this.OnDoubleClick(EventArgs.Empty);
                    break;
                case PopupEditorNotificationData.Context.MouseUpOnEditorElement:
                    this.TooglePopupState();
                    break;
                case PopupEditorNotificationData.Context.DisplayMemberChanged:
                case PopupEditorNotificationData.Context.ValueMemberChanged:
                    this.SyncEditorElementWithSelectedItem();
                    break;
                case PopupEditorNotificationData.Context.F4Press:
                    this.TooglePopupState();
                    break;
                case PopupEditorNotificationData.Context.Esc:
                    this.ClosePopup(RadPopupCloseReason.Keyboard);
                    break;
                case PopupEditorNotificationData.Context.KeyUpKeyDownPress:
                    this.HandleOnKeyUpKeyDownPress(notificationData.keyEventArgs);
                    break;
                case PopupEditorNotificationData.Context.ItemsChanged:
                    this.OnItemsChanged();
                    break;
                case PopupEditorNotificationData.Context.ItemsClear:
                    this.OnItemsClear();
                    break;
                case PopupEditorNotificationData.Context.KeyDown:
                    this.ProcessKeyDown(this.editableElement.TextBox, notificationData.keyEventArgs);
                    break;
                case PopupEditorNotificationData.Context.KeyUp:
                    this.ProcessKeyUp(this.editableElement.TextBox, notificationData.keyEventArgs);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region ITooltipOwner
        public object Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = null;
            }
        }
        #endregion
    }

    public class RadDropDownListArrowButtonElement : RadArrowButtonElement
    {
        static RadDropDownListArrowButtonElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadDropDownArrowButtonElementStateManagerFactory(RadDropDownListElement.IsDropDownShownProperty), typeof(RadDropDownListArrowButtonElement));
        }
    }
}
