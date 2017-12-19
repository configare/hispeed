using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI.Design;


namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a combo box element. The <see cref="RadComboBox">RadComboBox</see>
    ///     class is a simple wrapper for the RadComboBoxElement class. The
    ///     <see cref="RadComboBox">RadComboBox</see> acts to transfer events to and from its
    ///     corresponding RadComboBoxElement instance. The RadComboBoxElement which is
    ///     essentially the <see cref="RadComboBox">RadComboBox</see> control may be nested in
    ///     other telerik controls. The RadComboBoxElement class implements all logical and UI
    ///     funcitonality.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
    public abstract class BaseComboBoxElement : PopupEditorBaseElement
    {
        // Fields
        private const int DefaultDropDownWidth = -1;
        private const int DefaultDropDownHeight = 106;
        private const int DefaultDropDownItems = 8;

        protected RadTextBoxItem textBox;
        private RadTextBoxElement textBoxPanel;
        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive;
        private RadArrowButtonElement arrowButton;
        private ComboBoxEditorLayoutPanel layoutPanel;
        internal bool KeyboardCommandIssued = false;
        internal bool PopupDisplayedForTheFirstTime = true;
        internal int OnTextBoxCaptureChanged = 0;
        //user interface
        private AutoCompleteMode autoCompleteMode = AutoCompleteMode.None;
        private bool dblClickRotate = false;
        private bool scrollOnMouseWheel = true;
        private int dropDownWidth = DefaultDropDownWidth;
        private int dropDownHeight = DefaultDropDownHeight;
        private int maxDropDownItems = DefaultDropDownItems;

        //Event keys
        private static readonly object CaseSensitiveChangedEventKey;
        private static readonly object DropDownStyleChangedEventKey;
        private static readonly object KeyDownEventKey;
        private static readonly object KeyPressEventKey;
        private static readonly object KeyUpEventKey;
        private static readonly object SelectedIndexChangedEventKey;
        private static readonly object SelectedValueChangedEventKey;
        private static readonly object SortedChangedEventKey;
        internal protected string LastTypedText = string.Empty;
        private Keys lastPressedKey = Keys.None;
        private Size dropDownMinSize = Size.Empty;
        private Size dropDownMaxSize = Size.Empty;

        internal bool ListAutoCompleteIssued = false;
        internal string lastTextChangedValue;

        #region Constructors
        static BaseComboBoxElement()
        {
            CaseSensitiveChangedEventKey = new object();
            DropDownStyleChangedEventKey = new object();
            KeyDownEventKey = new object();
            KeyPressEventKey = new object();
            KeyUpEventKey = new object();
            SelectedIndexChangedEventKey = new object();
            SelectedValueChangedEventKey = new object();
            SortedChangedEventKey = new object();
        }

        /// <summary>
        /// Initializes a new instance of the RadComboBoxElement class.
        /// </summary>
        public BaseComboBoxElement()
        {
            this.UseNewLayoutSystem = true;
            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
        }

        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();

            base.DisposeManagedResources();
        }
        #endregion

        #region Dependency properties
        internal static RadProperty CaseSensitiveProperty = RadProperty.Register(
            "CaseSensitive", typeof(bool), typeof(BaseComboBoxElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        internal static RadProperty SortedProperty = RadProperty.Register(
            "Sorted", typeof(SortStyle), typeof(BaseComboBoxElement), new RadElementPropertyMetadata(
                SortStyle.None, ElementPropertyOptions.None));

        private static RadProperty DropDownStyleProperty = RadProperty.Register(
            "DropDownStyle", typeof(RadDropDownStyle), typeof(BaseComboBoxElement), new RadElementPropertyMetadata(
                RadDropDownStyle.DropDown, ElementPropertyOptions.None));

        // PopUp Animation Properties
        private static RadProperty DropDownAnimationEnabledProperty = RadProperty.Register(
            "DropDownAnimationEnabled", typeof(bool), typeof(BaseComboBoxElement), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.None));

        private static RadProperty DropDownAnimationEasingProperty = RadProperty.Register(
            "DropDownAnimationEasing", typeof(RadEasingType), typeof(BaseComboBoxElement), new RadElementPropertyMetadata(
                RadEasingType.Linear, ElementPropertyOptions.None));

        private static RadProperty DropDownAnimationFramesProperty = RadProperty.Register(
            "DropDownAnimationFrames", typeof(int), typeof(BaseComboBoxElement), new RadElementPropertyMetadata(
                4, ElementPropertyOptions.None));

        public static RadProperty IsDropDownShownProperty = RadProperty.Register(
            "IsDropDownShown", typeof(bool), typeof(BaseComboBoxElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        #endregion

        #region Properties

        internal bool IsDropDownShown
        {
            get
            {
                return (bool)this.GetValue(RadComboBoxElement.IsDropDownShownProperty);
            }
            set
            {
                this.SetValue(RadComboBoxElement.IsDropDownShownProperty, value);
            }
        }

        internal RadArrowButtonElement ArrowButton
        {
            get
            {
                return this.arrowButton;
            }
        }

        internal FillPrimitive ComboBoxFill
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        internal BorderPrimitive ComboBoxBorder
        {
            get
            {
                return this.borderPrimitive;
            }
        }

        public int ArrowButtonMinWidth
        {
            get
            {
                return this.arrowButton.MinSize.Width;
            }
            set
            {
                this.arrowButton.MinSize = new Size(value, this.arrowButton.MinSize.Height);
            }
        }

        [Browsable(false)]
        public override System.Drawing.Text.TextRenderingHint TextRenderingHint
        {
            get
            {
                return base.TextRenderingHint;
            }
            set
            {
                base.TextRenderingHint = value;
            }
        }

        /// <summary>
        /// Specifies the mode for the automatic completion feature used in the ComboBox 
        /// and the TextBox controls.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(System.Windows.Forms.AutoCompleteMode.None), EditorBrowsable(EditorBrowsableState.Always),
        Description("Specifies the mode for the automatic completion feature used in the ComboBox and TextBox controls.")]
        public AutoCompleteMode AutoCompleteMode
        {
            get
            {
                return this.autoCompleteMode;
            }
            set
            {
                this.autoCompleteMode = value;
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
        [Description("Rotates selected items on double clicking inside the text edit control."),
        DefaultValue(false)]
        public bool DblClickRotate
        {
            get
            {
                return this.dblClickRotate;
            }
            set
            {
                if (value != this.dblClickRotate)
                {
                    this.dblClickRotate = value;
                    this.OnNotifyPropertyChanged("DblClickRotate");
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the user can scroll through the items
        /// when the popup is closed by using the mouse wheel.
        /// </summary>
        [DefaultValue(true)]
        [Description("Gets or sets a boolean value determining whether the user can scroll throught the items when the popup is closed by using the mouse wheel.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool ScrollOnMouseWheel
        {
            get
            {
                return this.scrollOnMouseWheel;
            }
            set
            {
                if (value != this.scrollOnMouseWheel)
                {
                    this.scrollOnMouseWheel = value;
                    this.OnNotifyPropertyChanged("ScrollOnMouseWheel");
                }
            }
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
                if (this.dropDownHeight != value)
                {
                    this.dropDownHeight = value;
                    this.OnNotifyPropertyChanged("DropDownHeight");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value specifying the style of the combo box. 
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("DropDownStyle", typeof(RadComboBoxElement)),
        Description("Gets or sets a value specifying the style of the combo box."),
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
            }
        }

        /// <summary>
        /// Gets whether the text input control of the combo box is in editable mode.
        /// </summary>
        public bool IsWritable
        {
            get
            {
                return /*(this.DropDownStyle == RadDropDownStyle.DropDown)&&*/ this.DropDownStyle == RadDropDownStyle.DropDown;
            }
        }

        /// <summary>
        /// Gets or sets the width of the of the drop-down portion of a combo box.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(DefaultDropDownWidth)]
        [Description("Gets or sets the width of the of the drop-down portion of a combo box.")]
        public int DropDownWidth
        {
            get
            {
                return this.dropDownWidth;
            }

            set
            {
                if (this.dropDownWidth != value)
                {
                    this.dropDownWidth = value;
                    this.OnNotifyPropertyChanged("DropDownWidth");
                }
            }
        }

        /// <commentsfrom cref="RadListBoxElement.IntegralHeight" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory),
        RadDescription("IntegralHeight", typeof(RadListBoxElement)),
        RadDefaultValue("IntegralHeight", typeof(RadListBoxElement))]
        public abstract bool IntegralHeight
        {
            get;
            set;
        }

        protected abstract bool IndexChanging
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection representing the items contained in this ComboBox. 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this ComboBox.")]
        public abstract RadItemCollection Items
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the combo box is displaying its drop-down portion.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets a value indicating whether the combo box is displaying its drop-down portion.")]
        public bool IsDroppedDown
        {
            get
            {
                return this.IsDropDownShown;
            }
        }

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
        /// Gets or sets the maximum number of characters the user can type or paste into the text box control.
        /// </summary>
        [Description("Gets or sets the maximum number of characters the user can type or paste into the text box control."),
        Category(RadDesignCategory.BehaviorCategory),
        RadDefaultValue("MaxLength", typeof(RadTextBoxItem))]
        public int MaxLength
        {
            get
            {
                return this.textBox.MaxLength;
            }

            set
            {
                this.textBox.MaxLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the text that is displayed when the ComboBox contains a null 
        /// reference.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        RadDefaultValue("NullText", typeof(RadTextBoxItem)),
        RadDescription("NullText", typeof(RadTextBoxItem))]
        public string NullText
        {
            get
            {
                return this.textBox.NullText;
            }

            set
            {
                this.textBox.NullText = value;
            }
        }

        /// <commentsfrom cref="RadListBoxElement.SelectedItem" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RadDescription("SelectedItem", typeof(RadListBoxElement)),
        Browsable(false),
        Bindable(true)]
        public abstract Object SelectedItem
        {
            get;
            set;
        }

        /// <commentsfrom cref="RadListBoxElement.SelectedIndex" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        RadDescription("SelectedIndex", typeof(RadListBoxElement))]
        public abstract int SelectedIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text that is selected in the editable portion of the ComboBox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the text that is selected in the editable portion of the ComboBox."),
        Browsable(false)]
        public string SelectedText
        {
            get
            {
                if (this.DropDownStyle != RadDropDownStyle.DropDownList)
                {
                    return this.textBox.SelectedText;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (this.DropDownStyle != RadDropDownStyle.DropDownList)
                {
                    this.textBox.SelectedText = value;
                }
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
                return this.textBox.SelectionLength;
            }

            set
            {
                this.textBox.SelectionLength = value;
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
                return this.textBox.SelectionStart;
            }

            set
            {
                this.textBox.SelectionStart = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the sort style the of items in the combo box.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        DefaultValue(SortStyle.None),
        Description("Gets or sets a value indicating the sort style the of items in the combo box.")]
        public SortStyle Sorted
        {
            get
            {
                return (SortStyle)this.GetValue(SortedProperty);
            }
            set
            {
                this.SetValue(SortedProperty, value);
            }
        }

        /// <summary>Gets or sets the displayed text.</summary>
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                if (this.TextBoxItem.Text != value)
                {
                    this.TextBoxItem.Text = value;
                }
            }
        }

        /// <summary>
        ///		Gets or sets a value indicating whether the ComboBox DropDown will be enabled when it shows.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("DropDownAnimationEnabled", typeof(RadComboBoxElement))]
        [Description("Gets or sets a value indicating whether the ComboBox DropDown will be enabled when it shows.")]
        public bool DropDownAnimationEnabled
        {
            get
            {
                return (bool)this.GetValue(DropDownAnimationEnabledProperty);
            }
            set
            {
                this.SetValue(DropDownAnimationEnabledProperty, value);
            }
        }

        /// <summary>
        ///		Gets or sets the type of the DropDown animation.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("DropDownAnimationEasing", typeof(RadComboBoxElement))]
        [Description("Gets or sets the type of the DropDown animation.")]
        public RadEasingType DropDownAnimationEasing
        {
            get
            {
                return (RadEasingType)this.GetValue(DropDownAnimationEasingProperty);
            }
            set
            {
                this.SetValue(DropDownAnimationEasingProperty, value);
            }
        }

        /// <summary>
        ///		Gets or sets the number of frames that will be used when the DropDown is being animated.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("DropDownAnimationFrames", typeof(RadComboBoxElement))]
        [Description("Gets or sets the number of frames that will be used when the DropDown is being animated.")]
        public int DropDownAnimationFrames
        {
            get
            {
                return (int)this.GetValue(DropDownAnimationFramesProperty);
            }
            set
            {
                this.SetValue(DropDownAnimationFramesProperty, value);
            }
        }

        internal TextBox TextBoxControl
        {
            get
            {
                return this.textBox.TextBoxControl;
            }
        }
        internal RadTextBoxItem TextBoxItem
        {
            get
            {
                return this.textBox;
            }
        }
        /// <summary>
        ///		Gets the TextBoxElement which is used in the ComboBox.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTextBoxElement TextBoxElement
        {
            get
            {
                return this.textBoxPanel;
            }
        }

        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down sizing mode. The mode can be: horizontal, veritcal or a combination of them.")]
        [DefaultValue(SizingMode.None)]
        public abstract SizingMode DropDownSizingMode
        {
            get;
            set;
        }


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

        /// <commentsfrom cref="RadListBoxElement.Virtualized" filter=""/>
        [Browsable(true)]
        [RadDefaultValue("Virtualized", typeof(RadListBoxElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether RadScrollViewer uses UI virtualization.")]
        public abstract bool Virtualized
        {
            get;
            set;
        }

        #endregion

        #region Layout

        public override bool OverridesDefaultLayout
        {
            get
            {
                return true;
            }
        }

        public override void PerformLayoutCore(RadElement affectedElement)
        {
            if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren &&
                (this.textBox.StretchHorizontally || this.textBox.StretchVertically))
            {
                Size proposedSize = this.Size;
                Size prefSize = this.borderPrimitive.GetPreferredSize(proposedSize);
                Size fillSize = Size.Subtract(proposedSize, this.LayoutEngine.GetBorderSize());
                prefSize = this.fillPrimitive.GetPreferredSize(fillSize);
                Size layoutSize = Size.Subtract(fillSize, this.Padding.Size);
                prefSize = this.layoutPanel.GetPreferredSize(layoutSize);

                if (!this.textBox.StretchHorizontally)
                {
                    layoutSize.Width = prefSize.Width;
                }
                if (!this.textBox.StretchVertically)
                {
                    layoutSize.Height = prefSize.Height;
                }

                this.layoutPanel.Size = layoutSize;
                this.fillPrimitive.Size = fillSize;
                this.borderPrimitive.Size = proposedSize;
            }
            else
            {
                base.PerformLayoutCore(affectedElement);
            }
        }

        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            Size res = base.GetPreferredSizeCore(proposedSize);

            if (this.AutoSize)
            {
                if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                {
                    res = proposedSize;
                }
                else
                {
                    if (this.textBox.StretchHorizontally)
                    {
                        res.Width = proposedSize.Width - this.LayoutEngine.GetBorderSize().Width - this.Padding.Horizontal;
                    }
                    if (this.textBox.StretchVertically)
                    {
                        res.Height = proposedSize.Height - this.LayoutEngine.GetBorderSize().Height - this.Padding.Vertical;
                    }
                }
            }
            return this.LayoutEngine.CheckSize(res);
        }

        protected override void CreateChildElements()
        {
            this.arrowButton = new RadArrowButtonElement();
            this.arrowButton.Arrow.AutoSize = true;
            this.arrowButton.MinSize = new Size(RadArrowButtonElement.RadArrowButtonDefaultSize.Width, this.arrowButton.ArrowFullSize.Height);
            this.arrowButton.Class = "ComboBoxdropDownButton";
            this.arrowButton.ClickMode = ClickMode.Press;
            this.textBoxPanel = new RadTextBoxElement();
            this.textBoxPanel.ThemeRole = "ComboTextBoxElement";
            this.textBoxPanel.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.textBoxPanel.ShowBorder = false;
            this.textBoxPanel.Class = "ComboBoxTextEditor";
            this.textBox = this.textBoxPanel.TextBoxItem;
            this.textBox.Multiline = false;

            this.WireEvents();

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "ComboBoxBorder";
            this.borderPrimitive.ZIndex = 1;

            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.BindProperty(FillPrimitive.AutoSizeModeProperty, this, RadElement.AutoSizeModeProperty, PropertyBindingOptions.TwoWay);
            this.fillPrimitive.Class = "ComboBoxFill";

            this.layoutPanel = new ComboBoxEditorLayoutPanel();
            this.layoutPanel.Content = textBoxPanel;
            this.layoutPanel.ArrowButton = this.arrowButton;

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.layoutPanel);

            if (DesignMode)
            {
                this.textBox.TextBoxControl.Enabled = false;
                this.textBox.TextBoxControl.BackColor = Color.White;
            }
        }

        #endregion

        #region DataBind

        /// <commentsfrom cref="RadListBoxElement.DisplayMember" filter=""/>
        [RadDefaultValue("DisplayMember", typeof(RadListBoxElement)),
        TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
        RadDescription("DisplayMember", typeof(RadListBoxElement)),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        public abstract string DisplayMember
        {
            get;
            set;
        }

        /// <commentsfrom cref="RadListBoxElement.DataSource" filter=""/>
        [RadDefaultValue("DataSource", typeof(RadListBoxElement)),
        AttributeProvider(typeof(IListSource)),
        RadDescription("DataSource", typeof(RadListBoxElement)),
        Category(RadDesignCategory.DataCategory),
        RefreshProperties(RefreshProperties.Repaint)]
        public abstract object DataSource
        {
            get;
            set;
        }

        /// <commentsfrom cref="RadListBoxElement.FormatInfo" filter=""/>
        [Browsable(false),
        RadDescription("FormatInfo", typeof(RadListBoxElement)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        RadDefaultValue("FormatInfo", typeof(RadListBoxElement))]
        public abstract IFormatProvider FormatInfo
        {
            get;
            set;
        }

        /// <commentsfrom cref="RadListBoxElement.FormatString" filter=""/>
        [RadDefaultValue("FormatString", typeof(RadListBoxElement)),
        Editor(typeof(FormatStringEditor), typeof(UITypeEditor)),
        MergableProperty(false),
        RadDescription("FormatString", typeof(RadListBoxElement))]
        public abstract string FormatString
        {
            get;
            set;
        }

        /// <commentsfrom cref="RadListBoxElement.FormattingEnabled" filter=""/>
        [RadDescription("FormattingEnabled", typeof(RadListBoxElement)),
        RadDefaultValue("FormattingEnabled", typeof(RadListBoxElement))]
        public abstract bool FormattingEnabled
        {
            get;
            set;
        }

        /// <commentsfrom cref="RadListBoxElement.SelectedValue" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RadDescription("SelectedValue", typeof(RadListBoxElement)),
        Browsable(false),
        Bindable(true)]
        public abstract Object SelectedValue
        {
            get;
            set;
        }

        /// <commentsfrom cref="RadListBoxElement.ValueMember" filter=""/>
        [RadDescription("ValueMember", typeof(RadListBoxElement)),
        Category(RadDesignCategory.DataCategory),
        RadDefaultValue("ValueMember", typeof(RadListBoxElement)),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public abstract string ValueMember
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the text of the specified item.
        /// </summary>
        public abstract string GetItemText(object item);
        #endregion

        #region Events
        protected virtual void WireEvents()
        {
            if (this.textBox != null)
            {
                this.textBox.KeyDown += new KeyEventHandler(ProcessKeyDown);
                this.textBox.KeyPress += new KeyPressEventHandler(ProcessTextKeyPress);
                this.textBox.KeyUp += new KeyEventHandler(ProcessTextKeyUp);
                this.textBox.DoubleClick += new EventHandler(ProcessTextDoubleClick);
                this.textBox.TextChanged += new EventHandler(OnTextBoxControl_TextChanged);
                //this.textBox.KeyPress += new KeyPressEventHandler(OnTextBoxControl_KeyPress);

                this.TextBoxControl.LostFocus += new EventHandler(ProcessTextLostFocus);
                this.TextBoxControl.GotFocus += new EventHandler(ProcessTextGotFocus);
                this.TextBoxControl.MouseCaptureChanged += new EventHandler(ProcessTextMouseCaptureChanged);
                this.TextBoxControl.MouseWheel += new MouseEventHandler(ProcessTextMouseWheel);
                this.TextBoxControl.MouseEnter += new EventHandler(TextBoxControl_MouseEnter);
            }
        }

        void TextBoxControl_MouseEnter(object sender, EventArgs e)
        {
            if (this.DropDownStyle == RadDropDownStyle.DropDownList)
            {
                this.textBox.TextBoxControl.Cursor = Cursors.Arrow;
            }
            else
            {
                this.textBox.TextBoxControl.Cursor = null;
            }
        }

        // temporary fix for showing the caret when the DropDownStyle is DropDownList
        void ProcessTextGotFocus(object sender, EventArgs e)
        {
            if (this.DropDownStyle == RadDropDownStyle.DropDownList)
            {
                NativeMethods.HideCaret(this.textBox.TextBoxControl.Handle);
            }
        }

        protected virtual void UnwireEvents()
        {
            if (this.textBox != null)
            {
                this.textBox.KeyDown -= new KeyEventHandler(ProcessKeyDown);
                this.textBox.KeyPress -= new KeyPressEventHandler(ProcessTextKeyPress);
                this.textBox.KeyUp -= new KeyEventHandler(ProcessTextKeyUp);
                this.textBox.DoubleClick -= new EventHandler(ProcessTextDoubleClick);
                this.textBox.TextChanged -= new EventHandler(OnTextBoxControl_TextChanged);
                //this.textBox.KeyPress -= new KeyPressEventHandler(OnTextBoxControl_KeyPress);

                this.TextBoxControl.LostFocus -= new EventHandler(ProcessTextLostFocus);
                this.TextBoxControl.GotFocus -= new EventHandler(ProcessTextGotFocus);
                this.TextBoxControl.MouseCaptureChanged -= new EventHandler(ProcessTextMouseCaptureChanged);
                this.TextBoxControl.MouseWheel -= new MouseEventHandler(ProcessTextMouseWheel);
                this.TextBoxControl.MouseEnter -= new EventHandler(TextBoxControl_MouseEnter);
            }
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args) //ok
        {
            if (args.RoutedEvent == RadItem.MouseWheelEvent &&
                sender == this.textBox)
            {
                this.KeyboardCommandIssued = false;
                this.OnMouseWheel((MouseEventArgs)args.OriginalEventArgs);
            }

            if (args.RoutedEvent == RadElement.MouseDownEvent)
            {
                this.KeyboardCommandIssued = false;
                if ((sender == this.textBox && (this.DropDownStyle == RadDropDownStyle.DropDownList)) ||
                    (sender == this.arrowButton))
                {
                    if (!this.IsPopupOpen)
                    {
                        this.ShowPopup();
                        this.textBox.Focus();
                    }
                    else
                    {
                        this.ClosePopup(RadPopupCloseReason.Mouse);
                    }
                }
            }
            base.OnBubbleEvent(sender, args);
        }

        protected override void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnTunnelEvent(sender, args);

            if (args.RoutedEvent == RootRadElement.OnRoutedImageListChanged)
            {
                this.PopupForm.ImageList = this.ElementTree.ComponentTreeHandler.ImageList;
            }
        }

        /// <summary>
        /// Occurs when the CaseSensitive property has changed.
        /// </summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the CaseSensitive property has changed.")]
        public event EventHandler CaseSensitiveChanged
        {
            add
            {
                this.Events.AddHandler(BaseComboBoxElement.CaseSensitiveChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BaseComboBoxElement.CaseSensitiveChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the CaseSensitiveChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCaseSensitiveChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[BaseComboBoxElement.CaseSensitiveChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        [Browsable(true),
        Category(RadDesignCategory.PropertyChangedCategory),
        Description("Occurs when the DropDownStyle property has changed.")]
        public event EventHandler DropDownStyleChanged
        {
            add
            {
                this.Events.AddHandler(BaseComboBoxElement.DropDownStyleChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BaseComboBoxElement.DropDownStyleChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the DropDownStyleChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownStyleChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[BaseComboBoxElement.DropDownStyleChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the SelectedIndex property has changed.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the SelectedIndex property has changed.")]
        public event EventHandler SelectedIndexChanged
        {
            add
            {
                this.Events.AddHandler(BaseComboBoxElement.SelectedIndexChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BaseComboBoxElement.SelectedIndexChangedEventKey, value);
            }
        }

        internal void CallOnTextChanged(EventArgs e)
        {
            this.OnTextChanged(e);
        }

        internal void CallOnSelectedIndexChanged(EventArgs e)
        {
            this.OnSelectedIndexChanged(e);
        }

        /// <summary>
        /// Raises the SelectedIndexChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[BaseComboBoxElement.SelectedIndexChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
            //this.OnNotifyPropertyChanged("SelectedIndex");
        }

        /// <summary>Fires when the selected value is changed.</summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the SelectedValue property has changed.")]
        public event EventHandler SelectedValueChanged
        {
            add
            {
                this.Events.AddHandler(BaseComboBoxElement.SelectedValueChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BaseComboBoxElement.SelectedValueChangedEventKey, value);
            }
        }

        internal void CallOnSelectedValueChanged(EventArgs e)
        {
            this.OnSelectedValueChanged(e);
        }

        /// <summary>
        /// Raises the SelectedValueChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSelectedValueChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[BaseComboBoxElement.SelectedValueChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the Sorted property has changed.
        /// </summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the Sorted property has changed.")]
        public event EventHandler SortedChanged
        {
            add
            {
                this.Events.AddHandler(BaseComboBoxElement.SortedChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BaseComboBoxElement.SortedChangedEventKey, value);
            }
        }

        internal void CallOnSortedChanged(EventArgs e)
        {
            this.OnSortedChanged(e);
        }

        /// <summary>
        /// Raises the SortedChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSortedChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[BaseComboBoxElement.SortedChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        #endregion

        protected override void OnPropertyChanging(RadPropertyChangingEventArgs args) //ok
        {
            if (args.Property == RadComboBoxElement.SortedProperty)
            {
                if (this.DataSource != null && (((SortStyle)args.NewValue) != SortStyle.None))
                {
                    throw new InvalidOperationException("ComboBox that has a DataSource set cannot be sorted. Sort the data using the underlying data model.");
                }
            }
            base.OnPropertyChanging(args);
        }

        protected override void OnNotifyPropertyChanged(string propertyName) //ok
        {
            switch (propertyName)
            {
                case "DropDownHeight":
                    this.IntegralHeight = false;
                    break;
            }
            base.OnNotifyPropertyChanged(propertyName);
        }

        private void ProcessTextMouseWheel(object sender, MouseEventArgs e) //ok
        {
            this.OnMouseWheel(e);
        }
        private string oldTextValue = String.Empty;
        protected virtual void ProcessTextKeyUp(object sender, KeyEventArgs e) // ok
        {
            //GEO - fix to prevent disappearing mouse cursor when typing text while popup is shown
            if (this.ElementTree != null && this.ElementTree.Control != null)
            {
                Form form = this.ElementTree.Control.FindForm();
                if (form != null)
                {
                    form.Cursor = Cursors.Arrow;
                }
                else if (Form.ActiveForm != null)
                {
                    Form.ActiveForm.Cursor = Form.ActiveForm.Cursor;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.Return:
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Escape:
                    {
                        break;
                    }
                default:
                    {
                        if (this.DropDownStyle == RadDropDownStyle.DropDownList)
                            break;

                        if (oldTextValue != this.textBox.TextBoxControl.Text)
                        {
                            this.ProcessTextChanged(sender, e);
                            this.KeyboardCommandIssued = false;
                            oldTextValue = this.textBox.TextBoxControl.Text;
                        }
                        break;
                    }
            }
            this.OnKeyUp(e);
        }

        private void ProcessTextLostFocus(object sender, EventArgs e)
        {
            this.OnTextBoxCaptureChanged = 0;
        }

        private void ProcessTextMouseCaptureChanged(object sender, EventArgs e)
        {
            //this is needed for eliminating the cases when the Append autocomplete is in action and the popup closes 
            //(then the both Text properties are different)
            this.OnTextBoxCaptureChanged++;
            if ((this.OnTextBoxCaptureChanged == 1) &&
                this.Text == this.TextBoxControl.Text)
            {
                if (!this.SelectAllText(this.Text))
                {
                    this.textBox.DeselectAll();
                }
            }
        }

        protected virtual void ProcessKeyDown(object sender, KeyEventArgs e)
        {
            this.oldTextValue = this.textBox.Text;
            if (e.Handled)
            {
                this.OnKeyDown(e);
                return;
            }
            this.lastPressedKey = e.KeyCode;

            if (this.TooglePopupWithKeyboard(e))
            {
                this.OnKeyDown(e);
            }
            else
            {
                this.KeyboardCommandIssued = false;
                switch (e.KeyCode)
                {
                    case Keys.Return:
                        this.ProcessReturnKey(e);
                        break;
                    case Keys.Delete:
                    case Keys.Back:
                        this.KeyboardCommandIssued = true;
                        this.ProcessDeleteKey(e);
                        break;
                    case Keys.Down:
                        this.KeyboardCommandIssued = true;
                        this.SelectNextItem();
                        e.Handled = true;
                        break;
                    case Keys.Up:
                        this.KeyboardCommandIssued = true;
                        this.SelectPreviousItem();
                        e.Handled = true;
                        break;
                    case Keys.Right:
                        if (this.DropDownStyle == RadDropDownStyle.DropDownList)
                        {
                            this.KeyboardCommandIssued = true;
                            this.SelectNextItem();
                            e.Handled = true;
                        }
                        break;
                    case Keys.Left:
                        if (this.DropDownStyle == RadDropDownStyle.DropDownList)
                        {
                            this.KeyboardCommandIssued = true;
                            this.SelectPreviousItem();
                            e.Handled = true;
                        }
                        break;
                    case Keys.Escape:
                        e.Handled = this.ProcessEscKey(e);

                        break;
                    default:
                        break;
                }
                this.OnKeyDown(e);
            }
        }

        protected virtual void ProcessDeleteKey(KeyEventArgs e)
        {
            if (this.DropDownStyle == RadDropDownStyle.DropDownList)
            {

                if (this.Items.Count > 0)
                {
                    this.SelectedIndex = 0;
                }
                e.Handled = true;
            }
        }

        protected internal virtual void ProcessReturnKey(KeyEventArgs e)
        {

        }

        protected internal virtual bool ProcessEscKey(KeyEventArgs e)
        {
            return false;
        }

        protected virtual string GetText(object item)
        {
            RadItem item1 = item as RadItem;
            if (item1 != null)
            {
                return item1.Text;
            }
            return null;
        }

        protected virtual object GetActiveItem()
        {
            return null;
        }

        private void OnTextBoxControl_TextChanged(object sender, EventArgs e)
        {
            if (this.Text != this.textBox.Text)
            {
                this.Text = this.textBox.Text;
            }
        }

        protected virtual void ProcessTextChanged(object sender, EventArgs e)
        {
            if (!this.IsInValidState(true) || this.IsDesignMode)
            {
                return;
            }

            if ((this.DropDownStyle == RadDropDownStyle.DropDownList) ||
                this.IndexChanging)
            {
                return;
            }
            string text = this.TextBoxControl.Text;
            if (text != this.lastTextChangedValue)
            {
                this.lastTextChangedValue = text;
                this.LastTypedText = text;
            }

            //base.Text = this.TextBoxControl.Text;
            if (this.lastPressedKey == Keys.Delete ||
                 this.lastPressedKey == Keys.Back)
            {
                return;
            }

            switch (this.AutoCompleteMode)
            {
                case AutoCompleteMode.None:
                    break;
                case AutoCompleteMode.Append:
                    this.SetAppendAutoComplete();
                    break;
                case AutoCompleteMode.Suggest:
                    if (this.lastPressedKey != Keys.Return)
                    {
                        this.SetSuggestAutoComplete();
                    }
                    break;
                case AutoCompleteMode.SuggestAppend:
                    this.SetAppendAutoComplete();
                    this.SetSuggestAutoComplete();
                    break;
                default:
                    break;
            }
        }


        private void ProcessTextKeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        public abstract ArrayList FindAllItems(string startsWith);

        protected virtual void SelectPreviousItem()
        {
        }

        protected virtual void SelectNextItem()
        {
        }

        private bool TooglePopupWithKeyboard(KeyEventArgs e)
        {
            if ((e.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)) ||
                (e.Modifiers == Keys.None && e.KeyCode == Keys.F4))
            {
                this.TooglePopupState();
                return true;
            }
            return false;
        }

        private bool IsCommandKey(Keys keyCode)
        {
            if (keyCode == Keys.Return ||
            keyCode == Keys.Delete ||
            keyCode == Keys.Back ||
            keyCode == Keys.Down ||
            keyCode == Keys.Up ||
            keyCode == Keys.Left ||
            keyCode == Keys.Right ||
            keyCode == Keys.Escape)
            {
                return true;
            }
            return false;
        }

        protected abstract void SetAppendAutoComplete();

        protected virtual void SetSuggestAutoComplete()
        {
            if (this.IsPopupOpen)
            {
                if (lastPressedKey != Keys.Up && lastPressedKey != Keys.Down)
                {
                    object item = this.FindItemExact(this.Text);
                    if (item != null)
                    {
                        this.SelectedItem = item;
                        this.ClosePopup();
                    }
                }
            }
            else
            {
                char partial = (char)this.lastPressedKey;
                if (!char.IsControl(partial) && this.lastPressedKey != Keys.Delete)
                {
                    this.ShowPopup();
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BaseComboBoxElement.DropDownStyleProperty)
            {
                if ((RadDropDownStyle)e.NewValue == RadDropDownStyle.DropDownList)
                {
                    if (!this.IsDesignMode &&
                        this.IsInValidState(true) &&
                        !this.ElementTree.ComponentTreeHandler.Initializing &&
                        this.FindItemIndexExact(this.Text) == -1)
                    {
                        this.TextBoxItem.Text = string.Empty;
                    }
                    this.TextBoxControl.ReadOnly = true;
                }
                else
                {
                    this.TextBoxControl.ReadOnly = false;
                }
                foreach (RadElement child in this.ChildrenHierarchy)
                {
                    child.SetValue(DropDownStyleProperty, e.NewValue);
                }
                this.OnDropDownStyleChanged(EventArgs.Empty);
            }
            if (e.Property == BaseComboBoxElement.CaseSensitiveProperty)
            {
                this.OnCaseSensitiveChanged(EventArgs.Empty);
            }
            if (e.Property == BaseComboBoxElement.DropDownAnimationEnabledProperty)
            {
                this.PopupForm.AnimationEnabled = (bool)e.NewValue;
            }
            if (e.Property == BaseComboBoxElement.DropDownAnimationEasingProperty)
            {
                this.PopupForm.AnimationProperties.EasingType = (RadEasingType)e.NewValue;
            }
            if (e.Property == BaseComboBoxElement.DropDownAnimationFramesProperty)
            {
                this.PopupForm.AnimationProperties.AnimationFrames = (int)e.NewValue;
            }

            if (e.Property == BaseComboBoxElement.IsDropDownShownProperty)
            {
                foreach (RadElement child in this.ChildrenHierarchy)
                {
                    child.SetValue(BaseComboBoxElement.IsDropDownShownProperty, e.NewValue);
                }
            }

            if (e.Property == VisualElement.BackColorProperty)
            {
                this.textBoxPanel.BackColor = (Color)e.NewValue;
            }

            if (e.Property == RadItem.TextProperty)
            {
                this.SetActiveItem((string)e.NewValue);
            }
            if (e.Property == RadElement.RightToLeftProperty)
            {
                if ((bool)e.NewValue)
                {
                    this.PopupForm.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                    this.textBox.RightToLeft = true;
                }
                else
                {
                    this.PopupForm.RightToLeft = System.Windows.Forms.RightToLeft.No;
                    this.textBox.RightToLeft = false;
                }
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (this.DblClickRotate &&
                this.Items != null &&
                this.Items.Count > 0)
            {
                if (this.SelectedIndex < this.Items.Count - 1)
                {
                    this.SelectedIndex++;
                }
                else
                {
                    this.SelectedIndex = 0;
                }
                this.ClosePopup();
            }
        }

        private void ProcessTextDoubleClick(object sender, EventArgs e) //ok
        {
            this.OnDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e) //ok
        {
            base.OnMouseWheel(e);
            if (e.Delta == 0)
            {
                return;
            }

            if (this.scrollOnMouseWheel && !this.IsPopupOpen)
            {
                if (e.Delta > 0)
                {
                    this.SelectPreviousItem();
                }
                else
                {
                    this.SelectNextItem();
                }
            }

            if (this.IsPopupOpen)
            {
                if (e.Delta > 0)
                {
                    this.DoScrollLineUp();
                }
                else
                {
                    this.DoScrollLineDown();
                }
            }
        }

        protected abstract void DoScrollLineUp();

        protected abstract void DoScrollLineDown();

        public bool SelectAllText(string text)
        {
            if (!this.TextBoxControl.ReadOnly)
            {
                this.SelectionStart = 0;
                this.SelectionLength = string.IsNullOrEmpty(text) ? 0 : text.Length;
                return true;
            }
            return false;
        }

        #region Methods
        /// <summary>
        /// Finds the first item in the combo box that starts with the specified string. 
        /// </summary>
        /// <param name="startsWith">The String to search for.</param>
        /// <returns>The first RadCOmboBoxItem found; returns null if no match is found.</returns>
        protected abstract object FindItem(string startsWith);

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string. 
        /// </summary>
        /// <param name="text">The String to search for.</param>
        /// <returns>The first item found; returns null if no match is found.</returns>
        protected abstract object FindItemExact(string text);

        /// <summary>
        /// Finds the index of the item with the specified text. The passed argument
        /// is compared with the DisplayMember value for each item in the items collection.
        /// </summary>
        /// <param name="text">The text of the item which index is to be acquired.</param>
        /// <returns>The index of the item if found, otherwise -1.</returns>
        protected abstract int FindItemIndexExact(string text);

        internal protected abstract void SetActiveItem(object item);

        internal protected abstract void SetActiveItem(string text);

        /// <summary>
        /// Call BeginUpdate at the begining of a block that makes many modifications in the GUI
        /// <seealso cref="EndUpdate"/>
        /// </summary>
        public abstract void BeginUpdate();

        /// <summary>
        /// Call BeginUpdate at the end of a block that makes many modifications in the GUI
        /// <seealso cref="BeginUpdate"/>
        /// </summary>
        public abstract void EndUpdate();

        /// <summary>
        /// Call the GetItemHeight member function to retrieve the height of list items in a combo box.
        /// </summary>
        /// <param name="index">Specifies the item of the combo box whose height is to be retrieved.</param>
        /// <returns></returns>
        public abstract int GetItemHeight(int index);

        /// <summary>
        /// Selects a range of text in the editable portion of the combo box
        /// </summary>
        /// <param name="start">The position of the first character in the current text selection within the text box.</param>
        /// <param name="length">The number of characters to select.</param>
        public void Select(int start, int length)
        {
            this.textBox.Select(start, length);
        }

        /// <summary>
        /// Selects all the text in the editable portion of the combo box.
        /// </summary>
        public void SelectAll()
        {
            this.textBox.SelectAll();
        }
        #endregion

        #region Overrides
        protected override RadPopupControlBase CreatePopupForm()
        {
            ComboPopupForm popupForm = new ComboPopupForm(this);
            popupForm.MinimumSize = this.DropDownMaxSize;
            popupForm.MaximumSize = this.DropDownMaxSize;
            popupForm.Height = this.DropDownHeight;

            return popupForm;
        }

        //protected abstract Size GetDropDownSize(bool measureExplicitly);

        protected abstract void ScrollToHome();

        protected abstract void ScrollItemIntoView(object item);

        protected virtual void ApplyThemeToPopupForm()
        {
            string popupThemeName = "ControlDefault";
            if (this.ElementTree != null && this.ElementTree.ComponentTreeHandler != null &&
                !string.IsNullOrEmpty(this.ElementTree.ComponentTreeHandler.ThemeName))
            {
                popupThemeName = this.ElementTree.ComponentTreeHandler.ThemeName;
            }

            RadPopupControlBase popupForm = this.PopupForm;
            if (popupForm.ThemeName != popupThemeName)
            {
                popupForm.ThemeName = popupThemeName;
                // Let the layout calculate the desired size of items
                // because the new theme could change it
                popupForm.RootElement.UpdateLayout();
            }
        }

        private void BringSelectedItemIntoView()
        {
            if (this.PopupForm == null)
            {
                return;
            }

            if (this.SelectedItem != null)
            {
                this.ScrollItemIntoView(this.SelectedItem as RadItem);
            }
            else
            {
                this.ScrollToHome();
            }
        }

        private void SetDropDownMinMaxSize()
        {
            RadPopupControlBase popupForm = this.PopupForm;
            if (this.DropDownSizingMode != SizingMode.None)
            {
                RadSizablePopupControl sizablePopupForm = popupForm as RadSizablePopupControl;
                if (sizablePopupForm != null)
                {
                    sizablePopupForm.MinimumSize = LayoutUtils.UnionSizes(this.dropDownMinSize,
                        sizablePopupForm.SizingGrip.Children[3].FullSize);

                    if (this.dropDownMaxSize != Size.Empty)
                    {
                        sizablePopupForm.MaximumSize = this.dropDownMaxSize;
                    }
                }
            }
            else
            {
                popupForm.MinimumSize = Size.Empty;
                popupForm.MaximumSize = Size.Empty;
            }
        }

        public override void ShowPopup()
        {
            // Apply the theme before the calculations for the drop-down size because it could
            // depend on the theme
            this.ApplyThemeToPopupForm();

            // The set of non-empty MaxSize and MinSize causes call of SetBoundsCore which resets
            // the NeverMeasured flag of RootRadElement
            this.SetDropDownMinMaxSize();

            this.BringSelectedItemIntoView();

            base.ShowPopup();
            if (!this.PopupDisplayedForTheFirstTime)
            {
                this.PopupDisplayedForTheFirstTime = false;
            }

            if (this.IsPopupOpen)
            {

                if (this.SelectedItem == null)
                {
                    this.ScrollItemIntoView(null /*this.Items[0]*/);
                }
                else
                {
                    this.SetActiveItem(this.SelectedItem as RadItem);
                }
            }
        }

        protected override void OnPopupOpened(EventArgs args)
        {
            base.OnPopupOpened(args);

            this.IsDropDownShown = true;
        }

        protected override void OnPopupClosed(RadPopupClosedEventArgs e)
        {
            base.OnPopupClosed(e);

            this.IsDropDownShown = false;
        }

        #endregion

        public override object Value
        {
            get
            {
                if (this.ValueMember != string.Empty)
                {
                    return this.SelectedValue;
                }
                return string.Empty;
            }
            set
            {
                this.originalValue = this.SelectedValue;

                if (this.ValueMember != string.Empty)
                {
                    this.SelectedValue = value;
                }
                else if (value != null)
                {
                    object item =
                        this.FindItemExact(value.ToString());
                    if (item != null)
                    {
                        this.SelectedItem = item;
                    }
                }
                else
                {
                    this.SelectedItem = null;
                }
            }
        }

        public override EditorVisualMode VisualMode
        {
            get
            {
                return EditorVisualMode.Dropdown;
            }
        }

        public override void BeginEdit()
        {
            this.SelectAllText(this.Text);
            this.originalValue = this.SelectedValue;
        }

    }
}
