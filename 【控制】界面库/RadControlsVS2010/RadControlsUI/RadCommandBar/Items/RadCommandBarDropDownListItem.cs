using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI.Data;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a drop down list in <see cref="CommandBarStripElement"/>.
    /// </summary>
    public class CommandBarDropDownList : RadCommandBarBaseItem
    {
        #region Static memebers

        static CommandBarDropDownList()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(CommandBarDropDownList));
        }

        #endregion

        #region Fields
        protected RadDropDownListElement dropDownListElement;
        #endregion

        #region Properties

        /// <summary>
        ///		Gets or sets the text associated with this item. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the text associated with this item.")]
        [Bindable(true)]
        [SettingsBindable(true)]
        [Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        [DefaultValue("")]
        [Localizable(true)]
        public override string Text
        {
            get
            {
                if (dropDownListElement != null)
                {
                    return dropDownListElement.Text;
                }

                return base.Text;
            }
            set
            {
                base.Text = value;
                if (dropDownListElement != null)
                {
                    dropDownListElement.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the hosted <see cref="RadDropDownListElement"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDropDownListElement CommandBarDropDownListElement
        {
            get
            {
                return dropDownListElement;
            }
            set
            {
                dropDownListElement = value;
            }
        }


        /// <summary>
        /// Gets the collection of data-binding objects for this IBindableComponent.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        ParenthesizePropertyName(true),
        RefreshProperties(RefreshProperties.All),
        Description("Gets the collection of data-binding objects for this IBindableComponent."),
        Category(RadDesignCategory.DataCategory)]
        public override ControlBindingsCollection DataBindings
        {
            get
            {
                return this.dropDownListElement.DataBindings;
            }
        }


        /// <summary>
        /// Gets or sets the BindingContext for the object.
        /// </summary>
        [RadPropertyDefaultValue("BindingContext", typeof(RadObject))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override BindingContext BindingContext
        {
            get
            {
                return this.dropDownListElement.BindingContext;
            }
            set
            {
                this.dropDownListElement.BindingContext = value;
            }
        }

        /// <summary>
        /// Gets the items collection of the <see cref="RadDropDownListElement"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(DesignerConsts.RadListControlDataItemCollectionDesignerString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this RadDropDownList.")]
        public RadListDataItemCollection Items
        {
            get { return dropDownListElement.Items; }
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
        [Browsable(false)]//TODO implement AutoCompleteDisplay and AutoCompleteValue member
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
         
        #region Overrides

        /// <summary>
        ///		Show or hide item from the strip
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), DefaultValue(true)]
        [Description("Indicates whether the item should be drawn in the strip.")]
        public override bool VisibleInStrip
        {
            get
            {
                return base.VisibleInStrip;
            }
            set
            {
                base.VisibleInStrip = value;
                if (this.dropDownListElement != null)
                {
                    this.dropDownListElement.SetValue(RadElement.VisibilityProperty, (value) ? ElementVisibility.Visible : ElementVisibility.Collapsed);
                }
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.dropDownListElement = new RadDropDownListElement();
            this.dropDownListElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.dropDownListElement.AutoSize = true;
            this.StretchHorizontally = this.StretchVertically = false;
            this.MinSize = new System.Drawing.Size(106, 22);
            //this.MaxSize = new System.Drawing.Size(106, 0);
            this.Children.Add(dropDownListElement);
        }

        #endregion

        #region Events

        public event PositionChangedEventHandler SelectedIndexChanged
        {
            add
            {
                this.dropDownListElement.SelectedIndexChanged += value;
            }
            remove
            {
                this.dropDownListElement.SelectedIndexChanged -= value;
            }
        }

        public event PositionChangingEventHandler SelectedIndexChanging
        {
            add
            {
                this.dropDownListElement.SelectedIndexChanging += value;
            }
            remove
            {
                this.dropDownListElement.SelectedIndexChanging -= value;
            }
        }

        public event ValueChangedEventHandler SelectedValueChanged
        {
            add
            {
                this.dropDownListElement.SelectedValueChanged += value;
            }
            remove
            {
                this.dropDownListElement.SelectedValueChanged -= value;
            }
        }

        /// <summary>
        ///		Occurs when the popup is about to be opened.
        /// </summary>
        [Category("Popup"), Description("Occurs when the popup is about to be opened.")]
        public event CancelEventHandler PopupOpening
        {
            add
            {
                this.dropDownListElement.PopupOpening += value;
            }
            remove
            {
                this.dropDownListElement.PopupOpening -= value;
            }
        }

        /// <summary>
        ///		Occurs when the popup is opened.
        /// </summary>
        [Category("Popup"), Description("Occurs when the popup is opened.")]
        public event EventHandler PopupOpened
        {
            add
            {
                this.dropDownListElement.PopupOpened += value;
            }
            remove
            {
                this.dropDownListElement.PopupOpened -= value;
            }
        }

        /// <summary>
        ///		Occurs when the popup is about to be closed.
        /// </summary>
        [Category("Popup"), Description("Occurs when the popup is about to be closed.")]
        public event RadPopupClosingEventHandler PopupClosing
        {
            add
            {
                this.dropDownListElement.PopupClosing += value;
            }
            remove
            {
                this.dropDownListElement.PopupClosing -= value;
            }
        }

        /// <summary>
        ///		Occurs when the popup is closed.
        /// </summary>
        [Category("Popup"), Description("Occurs when the popup is closed.")]
        public event RadPopupClosedEventHandler PopupClosed
        {
            add
            {
                this.dropDownListElement.PopupClosed += value;
            }
            remove
            {
                this.dropDownListElement.PopupClosed -= value;
            }
        }

        /// <summary>
        /// Occurs when the CommandBarTextBox has focus and the user pressees a key
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key")]
        public new event KeyPressEventHandler KeyPress
        {
            add
            {
                this.dropDownListElement.KeyPress += value;
            }

            remove
            {
                this.dropDownListElement.KeyPress -= value;
            }
        }

        /// <summary>
        /// Occurs when the CommandBarTextBox has focus and the user releases the pressed key up
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user releases the pressed key up")]
        public new event KeyEventHandler KeyUp
        {
            add
            {
                this.dropDownListElement.KeyUp += value;
            }

            remove
            {
                this.dropDownListElement.KeyUp -= value;
            }
        }

        /// <summary>
        /// Occurs when the CommandBarTextBox has focus and the user pressees a key down
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key down")]
        public new event KeyEventHandler KeyDown
        {
            add
            {
                this.dropDownListElement.KeyDown += value;
            }

            remove
            {
                this.dropDownListElement.KeyDown -= value;
            }
        }

        /// <summary>
        ///		Occurs when the Text property value is about to be changed.
        /// </summary>
        [Category("Property Changed"), Description("Occurs when the Text property value is about to be changed.")]
        public new event TextChangingEventHandler TextChanging
        {
            add
            {
                this.dropDownListElement.TextChanging += value;
            }

            remove
            {
                this.dropDownListElement.TextChanging -= value;
            }
        }

        /// <summary>
        ///		Occurs when the Text property value changes.
        /// </summary>
        [Category("Property Changed"), Description("Occurs when the Text property value changes.")]
        public new event EventHandler TextChanged
        {
            add
            {
                this.dropDownListElement.TextChanged += value;
            }

            remove
            {
                this.dropDownListElement.TextChanged -= value;
            }
        }

        #endregion

    }
}
