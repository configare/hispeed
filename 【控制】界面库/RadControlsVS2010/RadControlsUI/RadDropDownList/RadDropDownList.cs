using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.UI.Data;
using Telerik.WinControls.UI.Design;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a combo box class. The RadDropDownList class is essentially a simple
    ///     wrapper for the <see cref="RadDropDownListElement">RadDropDownListElement</see>. The latter
    ///     may be included in other telerik controls. All UI and logic functionality is
    ///     implemented by the <see cref="RadDropDownListElement">RadDropDownListElement</see> class.
    ///     RadDropDownList act to transfer event to and from its
    ///     <see cref="RadDropDownListElement">RadDropDownListElement</see> instance.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.DataControlsGroup)]
    [ToolboxItem(true)]
    [ComplexBindingProperties("DataSource", "ValueMember")]
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
    [Description("Displays an a DropDownList of permitted values")]
    [DefaultBindingProperty("Text"), DefaultEvent("SelectedIndexChanged"), DefaultProperty("Items")]
    [Designer(DesignerConsts.RadListControlDesignerString)]
    public class RadDropDownList : RadControl, ITooltipOwner
    {
        #region Event keys
        public static readonly object SelectedIndexChangedEventKey;
        public static readonly object SelectedIndexChangingEventKey;
        public static readonly object SelectedValueChangedEventKey;
        public static readonly object ListItemDataBindingEventKey;
        public static readonly object ListItemDataBoundEventKey;
        public static readonly object CreatingVisualListItemEventKey;
        public static readonly object PopupOpenedEventKey;
        public static readonly object PopupOpeningEventKey;
        public static readonly object PopupClosingEventKey;
        public static readonly object PopupClosedEventKey;
        public static readonly object SelectionRangeChangedKey;
        public static readonly object SortStyleChangedKey;
        public static readonly object VisualItemFormattingKey;
        public static readonly object KeyDownEventKey;
        public static readonly object KeyUpEventKey;
        public static readonly object KeyPressEventKey;

        #endregion

        #region Fields

        protected RadDropDownListElement dropDownListElement;

        #endregion

        #region Cstors

        static RadDropDownList()
        {
            SelectedIndexChangedEventKey = new object();
            SelectedIndexChangingEventKey = new object();
            SelectedValueChangedEventKey = new object();
            ListItemDataBindingEventKey = new object();
            ListItemDataBoundEventKey = new object();
            CreatingVisualListItemEventKey = new object();
            PopupOpenedEventKey = new object();
            PopupOpeningEventKey = new object();
            PopupClosingEventKey = new object();
            PopupClosedEventKey = new object();
            SelectionRangeChangedKey = new object();
            SortStyleChangedKey = new object();
            VisualItemFormattingKey = new object();
            KeyDownEventKey = new object();
            KeyUpEventKey = new object();
            KeyPressEventKey = new object();
        }

        public RadDropDownList()
        {
            this.AutoSize = true;
            this.TabStop = false;
            this.SetStyle(ControlStyles.Selectable, true);
        }

        #endregion

        #region Overrides

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.dropDownListElement.RightToLeft = this.RightToLeft == RightToLeft.Yes ? true : false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can give the focus to this control
        /// using the TAB key.
        /// </summary>        /// 
        /// <returns>true if the user can give the focus to the control using the TAB key;otherwise, false. The default is true.        </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get 
            {
                return base.TabStop;	
            }
            set 
            {
                base.TabStop = value;
            }
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType.Equals(typeof(RadTextBoxElement)))
            {
                return true;
            }
            else if (elementType.Equals(typeof(RadArrowButtonElement)))
            {
                return true;
            }             

            return base.ControlDefinesThemeForElement(element);             
        }
        
        protected override Size DefaultSize
        {
            get
            {
                return new Size(106, 20);
            }
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            base.OnBindingContextChanged(e);

            this.dropDownListElement.BindingContext = this.BindingContext;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.dropDownListElement = new RadDropDownListElement(this);
            this.RootElement.Children.Add(this.dropDownListElement);
            this.WireEvents();
        }

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchVertically = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.UnwireEvents();
            this.dropDownListElement = null;
            base.Dispose(disposing);      
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            if (keyCode == Keys.Left || keyCode == Keys.Right || keyCode == Keys.Up || keyCode == Keys.Down)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }
       
        #endregion

        #region Properties


         /// <summary>
        /// Gets or sets that RadListDataItem Image will be displayd in Editor Element when DropDownStyle is set to DropDownStyleList
        /// </summary>\
        [DefaultValue(false)]
        [Description("Gets or sets that RadListDataItem Image will be displayd in Editor Element when DropDownStyle is set to DropDownStyleList")]
        public virtual bool ShowImageInEditorArea
        {
            get
            {
                return this.DropDownListElement.ShowImageInEditorArea;
            }

            set
            {
                this.DropDownListElement.ShowImageInEditorArea = value;
            }
        }


        [DefaultValue(true)]        
        public bool FitItemsToSize
        {
            get
            {
                return this.ListElement.FitItemsToSize;
            }

            set
            {
                this.ListElement.FitItemsToSize = value;
            }
        }

        /// <summary>
        /// Gets a reference to the drop down form associated with this RadDropDownList.
        /// </summary>
        [Browsable(false)]
        public RadEditorPopupControlBase Popup
        {
            get
            {
                return this.DropDownListElement.Popup;
            }
        }

        /// <summary>
        /// Determines whether control's height will be determined automatically, depending on the current Font.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether control's height will be determines automatically, depending on the current Font.")]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of items to be shown in the drop-down portion of the RadDropDownList.
        /// </summary>
        [Description("Gets or sets the maximum number of items to be shown in the drop-down portion of the RadDropDownList."),
        Category(RadDesignCategory.BehaviorCategory), DefaultValue(PopupEditorElement.DefaultDropDownItems)]
        public int MaxDropDownItems
        {
            get
            {
                return this.dropDownListElement.MaxDropDownItems;
            }
            set
            {
                this.dropDownListElement.MaxDropDownItems = value;
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
                return this.dropDownListElement.AutoSizeItems;
            }
            set
            {
                this.dropDownListElement.AutoSizeItems = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of characters the user can type or paste into the text box control.
        /// </summary>
        [Description("Gets or sets the maximum number of characters the user can type or paste into the text box control."),
        Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        DefaultValue(32767)]
        public int MaxLength
        {
            get
            {
                return this.DropDownListElement.MaxLength;
            }
            set
            {
                this.DropDownListElement.MaxLength = value;
            }
        }

        /// <commentsfrom cref="PopupEditorElement.DropDownMinSize" filter=""/>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(typeof(Size), "0,0")]
        [Description("Gets or sets the drop down minimal size.")]
        public Size DropDownMinSize
        {
            get
            {
                return this.DropDownListElement.DropDownMinSize;
            }
            set
            {
                this.DropDownListElement.DropDownMinSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value of the <see cref=" Telerik.WinControls.UI.SizingMode"/> enumeration.
        /// This value determines how the pop-up form can be resized: vertically, horizontally or both.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down sizing mode. The mode can be: horizontal, veritcal or a combination of them.")]
        [DefaultValue(SizingMode.None)]
        public SizingMode DropDownSizingMode
        {
            get
            {
                return this.DropDownListElement.DropDownSizingMode;
            }
            set
            {
                this.DropDownListElement.DropDownSizingMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether string comparisons are case-sensitive.
        /// </summary>
        /// <commentsfrom cref="RadDropDownListElement.CaseSensitive" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("CaseSensitive", typeof(RadDropDownListElement))]
        [DefaultValue(false)]
        public bool CaseSensitive
        {
            get
            {
                return this.dropDownListElement.CaseSensitive;
            }
            set
            {
                this.dropDownListElement.CaseSensitive = value;
            }
        }

        /// <summary>
        /// Specifies the mode for the automatic completion feature used in the DropDownList 
        /// and the TextBox controls.
        /// </summary>
        /// <commentsfrom cref="RadDropDownListElement.AutoCompleteMode" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        EditorBrowsable(EditorBrowsableState.Always),
        RadDescription("AutoCompleteMode", typeof(RadDropDownListElement)),
        DefaultValue(System.Windows.Forms.AutoCompleteMode.None)]
        public AutoCompleteMode AutoCompleteMode
        {
            get
            {
                return this.DropDownListElement.AutoCompleteMode;
            }
            set
            {
                this.DropDownListElement.AutoCompleteMode = value;
            }
        }

        /// <summary>
        /// Rotate items on double click in the edit box part
        /// </summary>
        /// <commentsfrom cref="RadDropDownListElement.SelectNextOnDoubleClick" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("SelectNextOnDoubleClick", typeof(RadDropDownListElement))]
        [DefaultValue(false)]
        public bool SelectNextOnDoubleClick
        {
            get
            {
                return this.dropDownListElement.SelectNextOnDoubleClick;
            }
            set
            {
                this.dropDownListElement.SelectNextOnDoubleClick = value;
            }
        }

        /// <summary>
        /// Gets or sets an object that implements the IFormatProvider interface. This object is used when formatting items. The default object is
        /// CultureInfo.CurrentCulture.
        /// </summary>
        /// <commentsfrom cref="RadDropDownListElement.FormatInfo" filter=""/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public IFormatProvider FormatInfo
        {
            get
            {
                return this.dropDownListElement.FormatInfo;
            }
            set
            {
                this.dropDownListElement.FormatInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets a format string that will be used for visual item formatting if FormattingEnabled is set to true.
        /// </summary>
        /// <commentsfrom cref="RadDropDownListElement.FormatString" filter=""/>
        [DefaultValue("")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public string FormatString
        {
            get
            {
                return this.dropDownListElement.FormatString;
            }
            set
            {
                this.dropDownListElement.FormatString = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort style. It can be Ascending, Descending or None. Sorting is performed according to the property specified by DisplayMember.
        /// </summary>
        [DefaultValue(Telerik.WinControls.Enumerations.SortStyle.None)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public SortStyle SortStyle
        {
            get
            {
                return this.dropDownListElement.SortStyle;
            }
            set
            {
                this.dropDownListElement.SortStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether text formatting is enabled for the visual items.
        /// </summary>
        /// <commentsfrom cref="RadDropDownListElement.FormattingEnabled" filter=""/>
        [RadDescription("FormattingEnabled", typeof(RadDropDownListElement)),
        DefaultValue(true)]
        public bool FormattingEnabled
        {
            get
            {
                return this.dropDownListElement.FormattingEnabled;
            }
            set
            {
                this.dropDownListElement.FormattingEnabled = value;
            }
        }

        /// <commentsfrom cref="RadDropDownListElement.DropDownAnimationEasing" filter=""/>
        ///  /// <summary>
        /// Gets or sets the easing type of the animation.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(Telerik.WinControls.RadEasingType.InQuad)]
        public RadEasingType DropDownAnimationEasing
        {
            get
            {
                return this.dropDownListElement.DropDownAnimationEasing;
            }
            set
            {
                this.dropDownListElement.DropDownAnimationEasing = value;
            }
        }

        /// <summary>
        ///	Gets or sets a value indicating whether the RadDropDownList will be animated when displaying.
        /// </summary>
        /// <commentsfrom cref="RadDropDownListElement.DropDownAnimationEnabled" filter=""/>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(false)]
        public bool DropDownAnimationEnabled
        {
            get
            {
                return this.dropDownListElement.DropDownAnimationEnabled;
            }
            set
            {
                this.dropDownListElement.DropDownAnimationEnabled = value;
            }
        }

        /// <summary>
        ///		Gets or sets the number of frames that will be used when the DropDown is being animated.
        /// </summary>
        /// <commentsfrom cref="RadDropDownListElement.DropDownAnimationFrames" filter=""/>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(1)]
        [Description("Gets or sets the number of frames that will be used when the DropDown is being animated.")]
        public int DropDownAnimationFrames
        {
            get
            {
                return this.dropDownListElement.DropDownAnimationFrames;
            }
            set
            {
                this.dropDownListElement.DropDownAnimationFrames = value;
            }
        }

        /// <commentsfrom cref="PopupEditorElement.DropDownHeight" filter=""/>
        /// <summary>
        /// Gets or sets the height in pixels of the drop-down portion of the RadDropDownList. 
        /// </summary>        
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("DropDownHeight", typeof(RadDropDownListElement)),
        DefaultValue(RadDropDownListElement.DefaultDropDownHeight),
        EditorBrowsable(EditorBrowsableState.Always),
        RefreshProperties(RefreshProperties.Repaint)]
        public int DropDownHeight
        {
            get
            {
                return this.dropDownListElement.DropDownHeight;
            }
            set
            {
                this.dropDownListElement.DropDownHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying the style of the DropDownList
        /// </summary>        
        [Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("DropDownStyle", typeof(RadDropDownListElement)),
        Description("Gets or sets a value specifying the style of the combo box."),
        RefreshProperties(RefreshProperties.Repaint)]
        public RadDropDownStyle DropDownStyle
        {
            get
            {
                return this.dropDownListElement.DropDownStyle;
            }
            set
            {
                this.dropDownListElement.DropDownStyle = value;
                //if we have internal TextBox, do not use TabStop, do it otherwise
                this.TabStop = value == RadDropDownStyle.DropDown ? false : true;
            }
        }

        /// <summary>
        /// DefaultItems count in drop-down portion of the RadDropDownList.
        /// </summary>        
        [Browsable(true)]
        [DefaultValue(6)]
        public int DefaultItemsCountInDropDown
        {
            get
            {
                return this.DropDownListElement.DefaultItemsCountInDropDown;
            }
            set
            {
                this.DropDownListElement.DefaultItemsCountInDropDown = value;
            }
        }
        
        /// <commentsfrom cref="PopupEditorElement.DropDownMaxSize" filter=""/>
        /// Gets or sets the drop down maximum size.
        [Category(RadDesignCategory.AppearanceCategory)]
        [RadDefaultValue("DropDownMaxSize", typeof(RadDropDownListElement))]
        [Description("Gets or sets the drop down maximal size.")]
        public Size DropDownMaxSize
        {
            get
            {
                return this.dropDownListElement.DropDownMaxSize;
            }
            set
            {
                this.dropDownListElement.DropDownMaxSize = value;
            }
        }

        /// <summary>
        /// Represent the DropDownListElement element
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(true)]
        public RadDropDownListElement DropDownListElement
        {
            get
            {
                return this.dropDownListElement;
            }
            set
            {
                this.dropDownListElement = value;
            }
        }

        /// <summary>
        /// Represent the List element
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RadListElement ListElement
        {
            get
            {
                return this.DropDownListElement.ListElement;
            }

            set
            {
                this.UnwireEvents();
                this.DropDownListElement.ListElement = value;
                this.WireEvents();
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
                return this.dropDownListElement.Items;
            }
        }

        /// <summary>
        /// Provides a readonly interface to the currently selected items.
        /// </summary>
        public IReadOnlyCollection<RadListDataItem> SelectedItems
        {
            get
            {
                return ListElement.SelectedItems;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected value. Setting the SelectedValue to a value that is shared between many items causes the first item to be selected.
        /// This property triggers the selection events.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Bindable(true)]
        public object SelectedValue
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

        /// <summary>
        /// Gets or sets the selected logical list item.
        /// Setting this property will cause the selection events to fire.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        [Browsable(false)]
        [Bindable(true)]
        public RadListDataItem SelectedItem
        {
            get
            {
                return dropDownListElement.ListElement.SelectedItem;
            }
            set
            {
                dropDownListElement.ListElement.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the selection.
        /// Setting this property will cause the SelectedIndexChanging and SelectedIndexChanged events to fire.
        /// </summary>
        [Browsable(false)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get
            {
                return dropDownListElement.ListElement.SelectedIndex;
            }
            set
            {                
                dropDownListElement.ListElement.SelectedIndex = value;               
            }
        }

        /// <summary>
        /// Gets or sets the object that is responsible for providing data objects for the AutoComplete Suggest.
        /// </summary>
        [Browsable(false)]//TODO impelement AutoCompleteDisplay and AutoCompleteValue member
        [DefaultValue(null)]
        [AttributeProvider(typeof(IListSource))]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets the object that is responsible for providing data objects for the AutoComplete Suggest.")]
        public object AutoCompleteDataSource
        {
            get
            {
                return this.dropDownListElement.AutoCompleteDataSource;
            }
            set
            {
                this.dropDownListElement.AutoCompleteDataSource = value;
            }
        }

        /// <summary>
        /// Gets or sets a string which will be used to get a text string for each visual item. This value can not be set to null. Setting
        /// it to null will cause it to contain an empty string.
        /// </summary>
        [Browsable(false)]//TODO impelement AutoCompleteDisplay and AutoCompleteValue member
        [DefaultValue(""),
        TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
        RadDescription("DisplayMember", typeof(RadListElement)),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        public string AutoCompleteDisplayMember
        {
            get
            {
                return dropDownListElement.DisplayMember;
            }
            set
            {
                dropDownListElement.DisplayMember = value;
            }
        }

        /// <summary>
        /// Gets or sets the string through which the SelectedValue property will be determined. This property can not be set to null.
        /// Setting it to null will cause it to contain an empty string.
        /// </summary>
        [Browsable(false)]//TODO impelement AutoCompleteDisplay and AutoCompleteValue member
        [RadDescription("ValueMember", typeof(RadListElement)),
        Category(RadDesignCategory.DataCategory),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string AutoCompleteValueMember
        {
            get
            {
                return dropDownListElement.ValueMember;
            }
            set
            {
                dropDownListElement.ValueMember = value;
            }
        }


        /// <summary>
        /// Gets or sets the object that is responsible for providing data objects for the RadListElement.
        /// </summary>
        [RadDefaultValue("DataSource", typeof(RadListElement)),
        AttributeProvider(typeof(IListSource)),
        RadDescription("DataSource", typeof(RadListElement)),
        Category(RadDesignCategory.DataCategory)]
        public object DataSource
        {
            get
            {
                return dropDownListElement.DataSource;
            }
            set
            {
                dropDownListElement.DataSource = value;
                this.OnDataBindingComplete(this, new ListBindingCompleteEventArgs(ListChangedType.Reset));
            }
        }

        /// <summary>
        /// Gets or sets a string which will be used to get a text string for each visual item. This value can not be set to null. Setting
        /// it to null will cause it to contain an empty string.
        /// </summary>
        [DefaultValue(""),
        TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
        RadDescription("DisplayMember", typeof(RadListElement)),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        public string DisplayMember
        {
            get
            {
                return dropDownListElement.DisplayMember;
            }
            set
            {
                dropDownListElement.DisplayMember = value;
            }
        }

        /// <summary>
        /// Gets or sets the string through which the SelectedValue property will be determined. This property can not be set to null.
        /// Setting it to null will cause it to contain an empty string.
        /// </summary>
        [RadDescription("ValueMember", typeof(RadListElement)),
        Category(RadDesignCategory.DataCategory),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]      
        public string ValueMember
        {
            get
            {
                return dropDownListElement.ValueMember;
            }
            set
            {
                dropDownListElement.ValueMember = value;
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
                return this.dropDownListElement.EnableMouseWheel;
            }
            set
            {
                this.dropDownListElement.EnableMouseWheel = value;
            }
        }

        /// <summary>
        /// Indicating whether the Popup part of the control 
        /// are displayed.
        /// </summary>
        [Description("Indicating whether the Popup part of the control are displayed.")]
        [Browsable(false)]
        public bool IsPopupVisible
        {
            get
            {
                return this.dropDownListElement.IsPopupOpen;
            }
        }

        /// <summary>
        /// Gets or sets a predicate which filters which items can be visible.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Predicate<RadListDataItem> Filter
        {
            get
            {
                return this.DropDownListElement.Filter;
            }

            set
            {
                this.DropDownListElement.Filter = value;
            }
        }

        /// <summary>
        /// Gets or sets a filter expression which determines which items will be visible.
        /// </summary>
        [Category("Data")]
        [DefaultValue("")]
        [Description("Gets or sets a filter expression which determines which items will be visible.")]
        [Browsable(true)]
        public string FilterExpression
        {
            get
            {
                return this.DropDownListElement.FilterExpression;
            }

            set
            {
                this.DropDownListElement.FilterExpression = value;
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
                return this.DropDownListElement.IsFilterActive;
            }
        }

        #region TextBox properties

        [Localizable(true),
        Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        Description("Gets or sets the text associated with this control."),
        Bindable(true),
        SettingsBindable(true),
        DefaultValue("")]
        public override string Text
        {
            get
            {
                return this.dropDownListElement.EditableElementText;
            }
            set
            {
                base.Text = value;
                this.dropDownListElement.EditableElementText = value;
            }
        }
       
        /// <summary>
        /// Gets or sets the text that is displayed when RadDropDownList has no text set.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        RadDefaultValue("NullText", typeof(RadTextBoxItem)),
        RadDescription("NullText", typeof(RadTextBoxItem))]
        public string NullText
        {
            get
            {
                return this.dropDownListElement.NullText;
            }

            set
            {
                this.dropDownListElement.NullText = value;
            }
        }

        /// <summary>
        /// Selects a range of text in the editable portion of the combo box
        /// </summary>
        /// <param name="start">The position of the first character in the current text selection within the text box.</param>
        /// <param name="length">The number of characters to select.</param>
        public void SelectText(int start, int length)
        {
            this.dropDownListElement.SelectText(start, length);
        }

        /// <summary>
        /// Selects all the text in the editable portion of the DropDownList box.
        /// </summary>
        public void SelectAllText()
        {
            this.dropDownListElement.SelectAll();
        }

        /// <summary>
        /// Gets or sets the text that is selected in the editable portion of the DropDownList.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the text that is selected in the editable portion of the DropDownList."),
        Browsable(false)]
        public string SelectedText
        {
            get
            {
                return this.dropDownListElement.SelectedText;
            }
            set
            {
                this.dropDownListElement.SelectedText = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of characters selected in the editable portion of the combo box.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the number of characters selected in the editable portion of the combo box.")]
        public int SelectionLength
        {
            get
            {
                return this.dropDownListElement.SelectionLength;
            }

            set
            {
                this.dropDownListElement.SelectionLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the starting index of text selected in the combo box.
        /// </summary>
        [Description("Gets or sets the starting index of text selected in the combo box."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)]
        public int SelectionStart
        {
            get
            {
                return this.dropDownListElement.SelectionStart;
            }

            set
            {
                this.dropDownListElement.SelectionStart = value;
            }
        }

        #endregion

        #endregion

        #region Events

        #region DataBindingComplete

        /// <summary>
        /// Fires after data binding operation has finished.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        /// <seealso cref="DataSource"/>
        /// <seealso cref="DataMember"/>
        /// <seealso cref="ListBindingCompleteEventHandler"/>
        /// <seealso cref="OnDataBindingComplete(ListBindingCompleteEventArgs)"/>
        /// <seealso cref="DataError"/>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Fires after data binding operation has finished.")]
        public event ListBindingCompleteEventHandler DataBindingComplete;

        /// <summary>
        /// Raises the <see cref="DataBindingComplete" /> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">An <see cref="ListBindingCompleteEventArgs" /> instance that contains the event data.</param>
        /// <seealso cref="DataBindingComplete"/>
        /// <seealso cref="ListBindingCompleteEventArgs"/>
        protected virtual void OnDataBindingComplete(object sender, ListBindingCompleteEventArgs e)
        {
            if (DataBindingComplete != null)
            {
                DataBindingComplete(this, e);
            }
        }

        #endregion

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public new event System.Windows.Forms.KeyPressEventHandler KeyPress
        {
            add
            {
                this.Events.AddHandler(KeyPressEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(KeyPressEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public new event System.Windows.Forms.KeyEventHandler KeyUp
        {
            add
            {
                this.Events.AddHandler(KeyUpEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(KeyUpEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        public new event System.Windows.Forms.KeyEventHandler KeyDown
        {
            add
            {
                this.Events.AddHandler(KeyDownEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(KeyDownEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the popup-form is opened.
        /// </summary>
        public event EventHandler PopupOpened
        {
            add
            {
                this.Events.AddHandler(PopupOpenedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(PopupOpenedEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the popup-form is about to be opened.
        /// </summary>        
        public event CancelEventHandler PopupOpening
        {
            add
            {
                this.Events.AddHandler(PopupOpeningEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(PopupOpeningEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the popup is about to be closed.
        /// </summary>
        public event RadPopupClosingEventHandler PopupClosing
        {
            add
            {
                this.Events.AddHandler(PopupClosingEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(PopupClosingEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the popup is closed.
        /// </summary>
        public event RadPopupClosedEventHandler PopupClosed
        {
            add
            {
                this.Events.AddHandler(PopupClosedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(PopupClosedEventKey, value);
            }
        }

        /// <summary>
        /// This event fires when the selected index property changes.
        /// </summary>
        public event PositionChangedEventHandler SelectedIndexChanged
        {
            add
            {
                this.Events.AddHandler(SelectedIndexChangedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(SelectedIndexChangedEventKey, value);
            }
        }

        /// <summary>
        /// This event fires before SelectedIndex changes. This event allows the operation to be cancelled.
        /// </summary>
        public event PositionChangingEventHandler SelectedIndexChanging
        {
            add
            {
                this.Events.AddHandler(SelectedIndexChangingEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(SelectedIndexChangingEventKey, value);
            }
        }

        /// <summary>
        /// This event fires only if the SelectedValue has really changed. For example it will not fire if the previously selected item
        /// has the same value as the newly selected item.
        /// </summary>
        public event EventHandler SelectedValueChanged
        {
            add
            {
                this.Events.AddHandler(SelectedValueChangedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(SelectedValueChangedEventKey, value);
            }
        }

        /// <summary>
        /// This event fires before a RadListDataItem is data bound. This happens
        /// when the DataSource property is assigned and the event fires for every item provided by the data source.
        /// This event allows a custom RadListDataItem to be provided by the user.
        /// </summary>
        public event ListItemDataBindingEventHandler ItemDataBinding
        {
            add
            {
                this.Events.AddHandler(ListItemDataBindingEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(ListItemDataBindingEventKey, value);
            }
        }

        /// <summary>
        /// This event fires after a RadListDataItem is data bound. This happens
        /// when the DataSource property is assigned and the event is fired for every item provided by the data source.
        /// </summary>
        public event ListItemDataBoundEventHandler ItemDataBound
        {
            add
            {
                this.Events.AddHandler(ListItemDataBoundEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(ListItemDataBoundEventKey, value);
            }
        }

        /// <summary>
        /// This event allows the user to create custom visual items.
        /// It is fired initially for all the visible items and when the control is resized afterwards.
        /// </summary>
        public event CreatingVisualListItemEventHandler CreatingVisualListItem
        {
            add
            {
                this.Events.AddHandler(CreatingVisualListItemEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(CreatingVisualListItemEventKey, value);
            }
        }

        /// <summary>
        /// This event fires when the SortStyle property changes.
        /// </summary>
        public event SortStyleChangedEventHandler SortStyleChanged
        {
            add
            {
                this.Events.AddHandler(SortStyleChangedKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(SortStyleChangedKey, value);
            }
        }

        /// <summary>
        /// The VisualItemFormatting event fires whenever a property of a visible data item changes
        /// and whenever a visual item is associated with a new data item. During scrolling for example.
        /// </summary>
        public event VisualListItemFormattingEventHandler VisualListItemFormatting
        {
            add
            {
                this.Events.AddHandler(VisualItemFormattingKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(VisualItemFormattingKey, value);
            }
        }
     
        #endregion

        #region Event Handlers

        private void listDropDownElement_TextChanged(object sender, EventArgs e)
        {
            this.Text = this.dropDownListElement.Text;
            this.OnTextChanged(e);
        }

        private void element_SelectedIndexChanging(object sender, PositionChangingCancelEventArgs e)
        {
            e.Cancel = this.OnSelectedIndexChanging(sender, e.Position);
        }

        private void element_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            this.OnSelectedIndexChanged(sender, e.Position);
        }

        private void element_SelectedValueChanged(object sender, ValueChangedEventArgs e)
        {
            this.OnSelectedValueChanged(sender, e.Position, e.NewValue, e.OldValue);
        }

        private void element_ItemDataBound(object sender, ListItemDataBoundEventArgs args)
        {
            this.OnItemDataBound(sender, args);
        }

        private void element_ItemDataBinding(object sender, ListItemDataBindingEventArgs args)
        {
            this.OnItemDataBinding(sender, args);
        }

        private void element_CreatingVisualItem(object sender, CreatingVisualListItemEventArgs args)
        {
            this.OnCreatingVisualItem(sender, args);
        }

        void dropDownListElement_VisualItemFormatting(object sender, VisualItemFormattingEventArgs args)
        {
            this.OnVisualItemFormatting(args.VisualItem);
        }

        void dropDownListElement_SortStyleChanged(object sender, SortStyleChangedEventArgs args)
        {
            this.OnSortStyleChanged(args.SortStyle);
        }

        #endregion

        #region Events Management

        protected virtual void OnSortStyleChanged(SortStyle sortStyle)
        {
            SortStyleChangedEventHandler handler = (SortStyleChangedEventHandler)this.Events[SortStyleChangedKey];
            if (handler != null)
            {
                handler(this, new SortStyleChangedEventArgs(sortStyle));
            }
        }

        protected internal virtual void OnVisualItemFormatting(RadListVisualItem item)
        {
            VisualListItemFormattingEventHandler hanlder = (VisualListItemFormattingEventHandler)this.Events[VisualItemFormattingKey];
            if (hanlder != null)
            {
                hanlder(this, new VisualItemFormattingEventArgs(item));
            }
        }

        protected virtual void OnSelectedIndexChanged(object sender, int newIndex)
        {
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("SelectedIndex"));
            
            PositionChangedEventHandler handler = (PositionChangedEventHandler)this.Events[SelectedIndexChangedEventKey];
            if (handler != null)
            {
                handler(this, new PositionChangedEventArgs(newIndex));
            }
        }

        protected virtual bool OnSelectedIndexChanging(object sender, int newIndex)
        {
            PositionChangingEventHandler handler = (PositionChangingEventHandler)this.Events[SelectedIndexChangingEventKey];
            if (handler != null)
            {
                PositionChangingCancelEventArgs args = new PositionChangingCancelEventArgs(newIndex);
                handler(this, args);
                return args.Cancel;
            }

            return false;
        }

        protected virtual void OnSelectedValueChanged(object sender, int newIndex, object oldValue, object newValue)
        {
            EventHandler handler = (EventHandler)this.Events[SelectedValueChangedEventKey];
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs(newIndex, newValue, oldValue));
            }
        }

        protected virtual void OnItemDataBinding(object sender, ListItemDataBindingEventArgs args)
        {
            ListItemDataBindingEventHandler handler = (ListItemDataBindingEventHandler)this.Events[ListItemDataBindingEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnItemDataBound(object sender, ListItemDataBoundEventArgs args)
        {
            ListItemDataBoundEventHandler handler = (ListItemDataBoundEventHandler)this.Events[ListItemDataBoundEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnCreatingVisualItem(object sender, CreatingVisualListItemEventArgs args)
        {
            CreatingVisualListItemEventHandler handler = (CreatingVisualListItemEventHandler)this.Events[CreatingVisualListItemEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void element_PopupClosing(object sender, RadPopupClosingEventArgs args)
        {
            RadPopupClosingEventHandler handler = (RadPopupClosingEventHandler)this.Events[PopupClosingEventKey];
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        protected virtual void element_PopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            RadPopupClosedEventHandler handler = (RadPopupClosedEventHandler)this.Events[PopupClosedEventKey];
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        protected virtual void element_PopupOpening(object sender, CancelEventArgs e)
        {
            CancelEventHandler handler = (CancelEventHandler)this.Events[PopupOpeningEventKey];
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void element_PopupOpened(object sender, EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[PopupOpenedEventKey];
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void dropDownListElement_KeyUp(object sender, KeyEventArgs e)
        {
            System.Windows.Forms.KeyEventHandler handler = (System.Windows.Forms.KeyEventHandler)this.Events[KeyUpEventKey];
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void dropDownListElement_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.KeyPressEventHandler handler = (System.Windows.Forms.KeyPressEventHandler)this.Events[KeyPressEventKey];
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void dropDownListElement_KeyDown(object sender, KeyEventArgs e)
        {
            System.Windows.Forms.KeyEventHandler handler = (System.Windows.Forms.KeyEventHandler)this.Events[KeyDownEventKey];
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion

        #region Focus management

        private bool entering = false;

        protected override void OnEnter(EventArgs e)
        {
            if (!entering)
            {
                base.OnEnter(e);
                entering = true;
                this.DropDownListElement.EditableElement.Entering = true;
                if (!string.IsNullOrEmpty(this.dropDownListElement.Text))
                {
                    this.dropDownListElement.SelectAllText();
                }
                this.dropDownListElement.Focus();                
                this.OnGotFocus(e);
                entering = false;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            this.OnLostFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!entering)
            {
                if (this.IsPopupVisible)
                {
                    this.CloseDropDown();
                }

                this.DropDownListElement.EnterPressedOrLeaveControl();               
                if (this.SelectionLength == this.Text.Length)
                {
                    this.dropDownListElement.SelectionStart = 0;
                    this.dropDownListElement.SelectionLength = 0;
                }

                base.OnLostFocus(e);
            }
        }

        #endregion

        #region Helpers

        /// <commentsfrom cref="RadListElement.FindItemExact" filter=""/>
        public RadListDataItem FindItemExact(string text, bool caseSensitive)
        {
            return this.ListElement.FindItemExact(text, caseSensitive);
        }

        /// <summary>
        /// Searches for an item related to the specified string. The relation is described by the object assigned to FindStringComparer property.
        /// By default FindStringComparer uses the System.String.StartsWith() method.
        /// This method starts searching from zero based index. If the algorithm reaches the end of the Items collection it wraps to the beginning
        /// and continues until one before the provided index.
        /// </summary>
        /// <param name="s">The string with which every item will be compared.</param>       
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindString(string s)
        {
            return this.ListElement.FindString(s, 0);
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
            return this.ListElement.FindString(s, startIndex);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but matches an item only if its text is exactly equal to the provided string.
        /// </summary>
        public int FindStringExact(string s)
        {
            return this.FindStringExact(s, 0);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but matches an item only if its text is exactly equal to the provided string.
        /// </summary>
        public int FindStringExact(string s, int startIndex)
        {
            return this.ListElement.FindStringExact(s, startIndex);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but does not start from the beginning when the end of the Items collection
        /// is reached.
        /// </summary>
        /// <param name="s">The string that will be used to search for an item.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindStringNonWrapping(string s)
        {
            return this.FindStringNonWrapping(s, 0);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but does not start from the beginning when the end of the Items collection
        /// is reached.
        /// </summary>
        /// <param name="s">The string that will be used to search for an item.</param>
        /// <param name="startIndex">The index from which to start searching.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindStringNonWrapping(string s, int startIndex)
        {
             return this.ListElement.FindStringNonWrapping(s,startIndex);
        }

        /// <summary>
        /// Gets or sets an object that implements IFindStringComparer.
        /// The value of this property is used in the FindString() method when searching for an item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFindStringComparer FindStringComparer
        {
            get
            {
                return this.ListElement.FindStringComparer;
            }

            set
            {
                this.ListElement.FindStringComparer = value;
            }
        }

        /// <summary>
        /// Gets or sets an object that implements IComparer which is used when sorting items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IComparer<RadListDataItem> ItemsSortComparer
        {
            get
            {
                return this.ListElement.ItemsSortComparer;
            }

            set
            {
                this.ListElement.ItemsSortComparer = value;
            }
        }


        /// <summary>
        /// Forces re-evaluation of the current data source (if any).
        /// </summary>
        public void Rebind()
        {
            this.dropDownListElement.ListElement.Rebind();
        }

        /// <summary>
        /// Displays the popup on the screen.
        /// </summary>
        public virtual void ShowDropDown()
        {
            this.DropDownListElement.ShowPopup();
        }

        /// <summary>
        /// HIde the popup from the screen.
        /// </summary>
        public virtual void CloseDropDown()
        {
            this.DropDownListElement.ClosePopup();
        }

        /// <summary>
        /// Call BeginUpdate at the begining of a block that makes many modifications in the GUI
        /// <seealso cref="EndUpdate"/>
        /// </summary>
        public virtual void BeginUpdate()
        {
            this.DropDownListElement.BeginUpdate();
        }

        /// <summary>
        /// Call EndUpdate at the end of a block that makes many modifications in the GUI
        /// <seealso cref="BeginUpdate"/>
        /// </summary>
        public virtual void EndUpdate()
        {
            this.DropDownListElement.EndUpdate();
        }

        /// <summary>
        /// Defers the refresh.
        /// </summary>
        /// <returns></returns>
        public virtual IDisposable DeferRefresh()
        {
            return this.DropDownListElement.DeferRefresh();
        }

        private void WireEvents()
        {
            this.dropDownListElement.SelectedIndexChanged += element_SelectedIndexChanged;
            this.dropDownListElement.SelectedIndexChanging += element_SelectedIndexChanging;
            this.dropDownListElement.SelectedValueChanged += element_SelectedValueChanged;
            this.dropDownListElement.ItemDataBinding += element_ItemDataBinding;
            this.dropDownListElement.ItemDataBound += element_ItemDataBound;
            this.dropDownListElement.CreatingVisualItem += element_CreatingVisualItem;
            this.dropDownListElement.PopupOpened += element_PopupOpened;
            this.dropDownListElement.PopupOpening += element_PopupOpening;
            this.dropDownListElement.PopupClosed +=element_PopupClosed;
            this.dropDownListElement.PopupClosing += element_PopupClosing;
            this.dropDownListElement.SortStyleChanged += dropDownListElement_SortStyleChanged;
            this.dropDownListElement.VisualItemFormatting +=dropDownListElement_VisualItemFormatting;
            this.dropDownListElement.TextChanged += listDropDownElement_TextChanged;
            this.dropDownListElement.InternalKeyDown +=dropDownListElement_KeyDown;
            this.dropDownListElement.InternalKeyPress += dropDownListElement_KeyPress;
            this.dropDownListElement.InternalKeyUp += dropDownListElement_KeyUp;
        }       
       
        private void UnwireEvents()
        {
            this.dropDownListElement.SelectedIndexChanged -= element_SelectedIndexChanged;
            this.dropDownListElement.SelectedIndexChanging -= element_SelectedIndexChanging;
            this.dropDownListElement.SelectedValueChanged -= element_SelectedValueChanged;
            this.dropDownListElement.ItemDataBinding -= element_ItemDataBinding;
            this.dropDownListElement.ItemDataBound -= element_ItemDataBound;
            this.dropDownListElement.CreatingVisualItem -= element_CreatingVisualItem;
            this.dropDownListElement.PopupOpened -= element_PopupOpened;
            this.dropDownListElement.PopupOpening -= element_PopupOpening;
            this.dropDownListElement.PopupClosed -= element_PopupClosed;
            this.dropDownListElement.PopupClosing -= element_PopupClosing;
            this.dropDownListElement.SortStyleChanged -= dropDownListElement_SortStyleChanged;
            this.dropDownListElement.VisualItemFormatting -= dropDownListElement_VisualItemFormatting;
            this.dropDownListElement.TextChanged -= listDropDownElement_TextChanged;
            this.dropDownListElement.InternalKeyDown -= dropDownListElement_KeyDown;
            this.dropDownListElement.InternalKeyPress -= dropDownListElement_KeyPress;
            this.dropDownListElement.InternalKeyUp -= dropDownListElement_KeyUp;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadDropDownListAccessibleObject(this);
        }
        #endregion

        #region ITooltipOwner
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }
        #endregion
    }
}
