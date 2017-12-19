using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Drawing;
using Telerik.WinControls.Design;
using System.Collections;
using System.Globalization;
using Telerik.WinControls;
using System.Diagnostics;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI.Design;
using Telerik.WinControls.Layouts;
using VisualStyles = System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Styles;

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
    [Obsolete("This control is obsolete, Use RadDropDownListElement instead.")]
    [RadToolboxItem(true)]
    public class RadComboBoxElement : PopupEditorBaseElement
    {
        #region BitState Keys

        internal const ulong KeyboardCommandIssuedStateKey = PopupEditorBaseElementLastStateKey << 1;
        internal const ulong ListAutoCompleteIssuedStateKey = KeyboardCommandIssuedStateKey << 1;

        #endregion

        // Fields
        private const int DefaultDropDownWidth = -1;
        private const int DefaultDropDownHeight = 106;
        private const int DefaultDropDownItems = 8;

        private RadTextBoxItem textBox;
        private RadTextBoxElement textBoxPanel;
        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive;
        private RadArrowButtonElement arrowButton;
        private ComboBoxEditorLayoutPanel layoutPanel;
        
        internal int OnTextBoxCaptureChanged = 0;
        //user interface
        private AutoCompleteMode autoCompleteMode = AutoCompleteMode.None;
        private bool dblClickRotate = false;
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
        internal string lastTextChangedValue;

        private Keys lastPressedKey = Keys.None;
        private Size dropDownMinSize = Size.Empty;
        private Size dropDownMaxSize = Size.Empty;

        private long autoCompleteTimeStamp;
        private string partial = string.Empty;

        #region Constructors
        static RadComboBoxElement()
        {
            //Moved to base
            //ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Resources.ComboBoxThemes.Office2007Blue.xml");

            CaseSensitiveChangedEventKey = new object();
            DropDownStyleChangedEventKey = new object();
            KeyDownEventKey = new object();
            KeyPressEventKey = new object();
            KeyUpEventKey = new object();
            SelectedIndexChangedEventKey = new object();
            SelectedValueChangedEventKey = new object();
            SortedChangedEventKey = new object();

            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadTextBoxElementStateManager(), typeof(RadComboBoxElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            // TODO: Remove this once we figure out how to convince the property store that we initially need an entry for this property.
            if (this.BindingContext != null)
            {
            }
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == RadElement.IsDesignModeStateKey)
            {
                this.ComboPopupForm.RootElement.SetIsDesignMode(newValue, true);
            }
        }

        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();

            base.DisposeManagedResources();
        }

        #endregion

      

        #region Dependency properties
        internal static RadProperty CaseSensitiveProperty = RadProperty.Register(
            "CaseSensitive", typeof(bool), typeof(RadComboBoxElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        internal static RadProperty SortedProperty = RadProperty.Register(
            "Sorted", typeof(SortStyle), typeof(RadComboBoxElement), new RadElementPropertyMetadata(
                SortStyle.None, ElementPropertyOptions.None));

        private static RadProperty DropDownStyleProperty = RadProperty.Register(
            "DropDownStyle", typeof(RadDropDownStyle), typeof(RadComboBoxElement), new RadElementPropertyMetadata(
                RadDropDownStyle.DropDown, ElementPropertyOptions.None));

        // PopUp Animation Properties
        private static RadProperty DropDownAnimationEnabledProperty = RadProperty.Register(
            "DropDownAnimationEnabled", typeof(bool), typeof(RadComboBoxElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        private static RadProperty DropDownAnimationEasingProperty = RadProperty.Register(
            "DropDownAnimationEasing", typeof(RadEasingType), typeof(RadComboBoxElement), new RadElementPropertyMetadata(
                RadEasingType.Linear, ElementPropertyOptions.None));

        private static RadProperty DropDownAnimationFramesProperty = RadProperty.Register(
            "DropDownAnimationFrames", typeof(int), typeof(RadComboBoxElement), new RadElementPropertyMetadata(
                4, ElementPropertyOptions.None));

        public static RadProperty IsDropDownShownProperty = RadProperty.Register(
            "IsDropDownShown", typeof(bool), typeof(RadComboBoxElement), new RadElementPropertyMetadata(
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

        public ComboPopupForm ComboPopupForm
        {
            get
            {
                return this.PopupForm as ComboPopupForm;
            }
        }

        /// <summary>
        /// Gets a reference to the listbox that drops down when combobox button is clicked
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListBoxElement ListBoxElement
        {
            get
            {
                return this.ComboPopupForm.ListBox;
            }
        }

        public RadArrowButtonElement ArrowButton
        {
            get
            {
                return this.arrowButton;
            }
        }

        public FillPrimitive ComboBoxFill
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        public BorderPrimitive ComboBoxBorder
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
        [Description("Rotate items on double click in the edit box part"),
        DefaultValue(false)]
        public bool DblClickRotate
        {
            get
            {
                return this.dblClickRotate;
            }
            set
            {
                this.dblClickRotate = value;
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
        public bool IntegralHeight
        {
            get
            {
                return this.ComboPopupForm.ListBox.IntegralHeight;
            }

            set
            {
                this.ComboPopupForm.ListBox.IntegralHeight = value;
            }
        }

        /// <summary>
        /// Gets a collection representing the items contained in this ComboBox. 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this ComboBox.")]
        public RadItemCollection Items
        {
            get
            {
                return this.ComboPopupForm.ListBox.Items;
            }
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

        /// <summary>
        /// Gets or sets the color of the text that is displayed when the ComboBox contains a null 
        /// reference.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        RadDefaultValue("NullColorText", typeof(RadTextBoxItem)),
        RadDescription("NullColorText", typeof(RadTextBoxItem))]
        public Color NullTextColor
        {
            get
            {
                return this.textBox.NullTextColor;
            }

            set
            {
                this.textBox.NullTextColor = value;
            }
        }

        /// <commentsfrom cref="RadListBoxElement.SelectedItem" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RadDescription("SelectedItem", typeof(RadListBoxElement)),
        Browsable(false),
        Bindable(true)]
        public virtual Object SelectedItem
        {
            get
            {
                return this.ComboPopupForm.ListBox.SelectedItem;
            }
            set
            {
                if (this.ComboPopupForm.ListBox.SelectedItem != value)
                {
                    this.ComboPopupForm.ListBox.SelectedItem = value;
                }
            }
        }

        /// <commentsfrom cref="RadListBoxElement.SelectedIndex" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        RadDescription("SelectedIndex", typeof(RadListBoxElement))]
        public virtual int SelectedIndex
        {
            get
            {
                return this.ComboPopupForm.ListBox.SelectedIndex;
            }
            set
            {
                this.ComboPopupForm.ListBox.SelectedIndex = value;
            }
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

        /// <summary>
        /// Gets or sets the displayed text.
        /// </summary>
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
                //TODO must be review  - ValueChanging cannot be canceled  Ticket ID: 232604
                //p.p. 05.08.09
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
        public SizingMode DropDownSizingMode
        {
            get
            {
                return this.ComboPopupForm.SizingMode;
            }
            set
            {
                this.ComboPopupForm.SizingMode = value;
            }
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
        public bool Virtualized
        {
            get
            {
                return this.ListBoxElement.Virtualized;
            }
            set
            {
                this.ListBoxElement.Virtualized = value;
            }
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
            this.arrowButton = new RadComboBoxArrowButtonElement();
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
            this.borderPrimitive.ZIndex = 5;

            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.BindProperty(FillPrimitive.AutoSizeModeProperty, this, RadElement.AutoSizeModeProperty, PropertyBindingOptions.TwoWay);
            this.fillPrimitive.Class = "ComboBoxFill";

            this.layoutPanel = new ComboBoxEditorLayoutPanel();
            this.layoutPanel.Content = textBoxPanel;
            this.layoutPanel.ArrowButton = this.arrowButton;

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.layoutPanel);

            this.BindProperty(RadTextBoxItem.IsNullTextProperty, this.textBoxPanel.TextBoxItem, RadTextBoxItem.IsNullTextProperty, PropertyBindingOptions.OneWay);
        }

        #endregion

        #region DataBind

        /// <commentsfrom cref="RadListBoxElement.DisplayMember" filter=""/>
        [RadDefaultValue("DisplayMember", typeof(RadListBoxElement)),
        TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
        RadDescription("DisplayMember", typeof(RadListBoxElement)),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        public string DisplayMember
        {
            get
            {
                return this.ListBoxElement.DisplayMember;
            }
            set
            {
                this.ListBoxElement.DisplayMember = value;
            }
        }

        /// <commentsfrom cref="RadListBoxElement.DataSource" filter=""/>
        [RadDefaultValue("DataSource", typeof(RadListBoxElement)),
        AttributeProvider(typeof(IListSource)),
        RadDescription("DataSource", typeof(RadListBoxElement)),
        Category(RadDesignCategory.DataCategory),
        RefreshProperties(RefreshProperties.Repaint)]
        public object DataSource
        {
            get
            {
                return this.ListBoxElement.DataSource;
            }
            set
            {
                if (value != null)
                {
                    this.Sorted = SortStyle.None;
                }
                this.ListBoxElement.DataSource = value;
            }
        }

        /// <commentsfrom cref="RadListBoxElement.FormatInfo" filter=""/>
        [Browsable(false),
        RadDescription("FormatInfo", typeof(RadListBoxElement)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        RadDefaultValue("FormatInfo", typeof(RadListBoxElement))]
        public IFormatProvider FormatInfo
        {
            get
            {
                return this.ListBoxElement.FormatInfo;
            }
            set
            {
                this.ListBoxElement.FormatInfo = value;
            }
        }

        /// <commentsfrom cref="RadListBoxElement.FormatString" filter=""/>
        [RadDefaultValue("FormatString", typeof(RadListBoxElement)),
        Editor(typeof(FormatStringEditor), typeof(UITypeEditor)),
        MergableProperty(false),
        RadDescription("FormatString", typeof(RadListBoxElement))]
        public string FormatString
        {
            get
            {
                return this.ListBoxElement.FormatString;
            }
            set
            {
                this.ListBoxElement.FormatString = value;
            }
        }

        /// <commentsfrom cref="RadListBoxElement.FormattingEnabled" filter=""/>
        [RadDescription("FormattingEnabled", typeof(RadListBoxElement)),
        RadDefaultValue("FormattingEnabled", typeof(RadListBoxElement))]
        public bool FormattingEnabled
        {
            get
            {
                return this.ListBoxElement.FormattingEnabled;
            }
            set
            {
                this.ListBoxElement.FormattingEnabled = value;
            }
        }

        /// <commentsfrom cref="RadListBoxElement.SelectedValue" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RadDescription("SelectedValue", typeof(RadListBoxElement)),
        Browsable(false),
        Bindable(true)]
        public Object SelectedValue
        {
            get
            {
                return this.ListBoxElement.SelectedValue;
            }
            set
            {
                this.ListBoxElement.SelectedValue = value;
            }
        }

        /// <commentsfrom cref="RadListBoxElement.ValueMember" filter=""/>
        [RadDescription("ValueMember", typeof(RadListBoxElement)),
        Category(RadDesignCategory.DataCategory),
        RadDefaultValue("ValueMember", typeof(RadListBoxElement)),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ValueMember
        {
            get
            {
                return this.ListBoxElement.ValueMember;
            }
            set
            {
                this.ListBoxElement.ValueMember = value;
            }
        }


        /// <summary>
        /// Gets the text of the specified item.
        /// </summary>
        public virtual string GetItemText(int item)
        {
            return this.ListBoxElement.Items[item].Text;
        }

        /// <summary>
        /// Gets the text of the specified item.
        /// </summary>
        [Obsolete("This method is obsolete and will be removed in the next release.")] // Skarlatov 25/09/2009
        public virtual string GetItemText(object item)
        {
            return item.ToString();
        }
        #endregion

        #region Events
        protected virtual void WireEvents()
        {
            if (this.textBox != null)
            {
                this.textBox.KeyDown += new KeyEventHandler(ProcessKeyDown);
                this.textBox.KeyPress += new KeyPressEventHandler(textBox_KeyPress);
                this.textBox.KeyUp += new KeyEventHandler(textBox_KeyUp);
                this.textBox.DoubleClick += new EventHandler(textBox_DoubleClick);
                this.textBox.TextChanged += new EventHandler(textBox_TextChanged);
                this.TextBoxControl.LostFocus += new EventHandler(TextBoxControl_LostFocus);
                this.TextBoxControl.Leave += new EventHandler(TextBoxControl_Leave);
                this.TextBoxControl.MouseCaptureChanged += new EventHandler(TextBoxControl_MouseCaptureChanged);
                this.TextBoxControl.GotFocus += new EventHandler(TextBoxControl_GotFocus);
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

        void TextBoxControl_GotFocus(object sender, EventArgs e)
        {
            if (this.DropDownStyle == RadDropDownStyle.DropDownList)
            {
                this.TextBoxControl.Select(0, 0);
                NativeMethods.HideCaret(this.textBox.TextBoxControl.Handle);
            }
            else
            {
                this.textBox.TextBoxControl.SelectAll();
            }
        }

        private void TextBoxControl_Leave(object sender, EventArgs e)
        {
            if (this.IsPopupOpen)
            {
                this.ClosePopup(RadPopupCloseReason.CloseCalled);
            }
        }       

        protected virtual void UnwireEvents()
        {
            if (this.textBox != null)
            {
                this.textBox.KeyDown -= new KeyEventHandler(ProcessKeyDown);
                this.textBox.KeyPress -= new KeyPressEventHandler(textBox_KeyPress);
                this.textBox.KeyUp -= new KeyEventHandler(textBox_KeyUp);
                this.textBox.DoubleClick -= new EventHandler(textBox_DoubleClick);
                this.textBox.TextChanged -= new EventHandler(textBox_TextChanged);
                this.TextBoxControl.LostFocus -= new EventHandler(TextBoxControl_LostFocus);
                this.TextBoxControl.Leave -= TextBoxControl_Leave;
                this.TextBoxControl.MouseCaptureChanged -= new EventHandler(TextBoxControl_MouseCaptureChanged);
                this.TextBoxControl.GotFocus -= new EventHandler(TextBoxControl_GotFocus);
                this.TextBoxControl.MouseEnter -= new EventHandler(TextBoxControl_MouseEnter);
            }
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            if (this.Items.Count == 0)
            {
                base.OnBubbleEvent(sender, args);
                return;
            }

            if (args.RoutedEvent == RadItem.MouseWheelEvent &&
                sender == this.textBox)
            {
                this.BitState[KeyboardCommandIssuedStateKey] = false;
                this.OnMouseWheel((MouseEventArgs)args.OriginalEventArgs);
            }

            if (args.RoutedEvent == RadElement.MouseDownEvent)
            {
                this.BitState[KeyboardCommandIssuedStateKey] = false;
                if ((sender == this.textBox && (this.DropDownStyle == RadDropDownStyle.DropDownList)) ||
                    (sender == this.arrowButton))
                {
                    if (!this.IsPopupOpen)
                    {
                        this.SetActiveItem(this.Text);//this.ComboPopupForm.SetActiveItem(this.Text);
                        this.textBox.Focus();
                        this.ShowPopup();
                    }
                    else //if (sender == this.arrowButton)// && this.IsPopupOpen
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

            if (args.RoutedEvent == RootRadElement.OnRoutedImageListChanged
                && this.ElementTree.ComponentTreeHandler != null)
            {
                this.ComboPopupForm.ImageList = this.ElementTree.ComponentTreeHandler.ImageList;
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
                this.Events.AddHandler(RadComboBoxElement.CaseSensitiveChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadComboBoxElement.CaseSensitiveChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the CaseSensitiveChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCaseSensitiveChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[RadComboBoxElement.CaseSensitiveChangedEventKey];
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
                this.Events.AddHandler(RadComboBoxElement.DropDownStyleChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadComboBoxElement.DropDownStyleChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the DropDownStyleChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownStyleChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[RadComboBoxElement.DropDownStyleChangedEventKey];
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
                this.Events.AddHandler(RadComboBoxElement.SelectedIndexChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadComboBoxElement.SelectedIndexChangedEventKey, value);
            }
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
            EventHandler handler1 = (EventHandler)this.Events[RadComboBoxElement.SelectedIndexChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Fires when the selected value is changed.
        /// </summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the SelectedValue property has changed.")]
        public event EventHandler SelectedValueChanged
        {
            add
            {
                this.Events.AddHandler(RadComboBoxElement.SelectedValueChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadComboBoxElement.SelectedValueChangedEventKey, value);
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
            EventHandler handler1 = (EventHandler)this.Events[RadComboBoxElement.SelectedValueChangedEventKey];
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
                this.Events.AddHandler(RadComboBoxElement.SortedChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadComboBoxElement.SortedChangedEventKey, value);
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
            EventHandler handler1 = (EventHandler)this.Events[RadComboBoxElement.SortedChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        #endregion
        
        protected override void OnUnloaded(ComponentThemableElementTree oldTree)
        {
            //if (this.PopupForm.Visible)
            //{
            //    this.ClosePopup(RadPopupCloseReason.CloseCalled);
            //}
            base.OnUnloaded(oldTree);
        }

        protected override void OnPropertyChanging(RadPropertyChangingEventArgs args)
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

        protected override void OnNotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "DropDownHeight":
                    this.IntegralHeight = false;
                    break;
            }
            base.OnNotifyPropertyChanged(propertyName);
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
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

            this.OnKeyUp(e);
        }

        private void TextBoxControl_LostFocus(object sender, EventArgs e)
        {
            this.OnTextBoxCaptureChanged = 0;
            this.textBox.TextBoxControl.Select(0, 0);
        }

        private void TextBoxControl_MouseCaptureChanged(object sender, EventArgs e)
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

        protected internal virtual bool ProcessReturnKey()
        {
            this.BitState[KeyboardCommandIssuedStateKey] = true;
            RadItem activeItem = this.ComboPopupForm.ActiveItem;

            base.Text = this.textBox.Text;

            if (this.DropDownStyle == RadDropDownStyle.DropDownList)
            {
                RadItem textItem = this.SelectedItem as RadItem;
                if (textItem != null)
                {
                    this.textBox.SetTextBoxTextSilently(textItem.Text);
                }
            }
            else if (this.autoCompleteMode != AutoCompleteMode.None)//this.autoCompleteMode == AutoCompleteMode.Append
            {
                RadItem item = this.FindItem(this.TextBoxControl.Text);
                if (item != null &&
                    item != this.SelectedItem)
                {
                    if (((this.SelectedItem != null) &&
                        !(this.SelectedItem as RadItem).Text.Equals(item.Text, StringComparison.InvariantCulture)) ||
                        this.SelectedItem == null)
                    {
                        this.SelectedItem = item;
                    }
                }
            }
            else if (activeItem != null)
            {
                this.SelectedItem = activeItem;
            }
            this.SelectAllText(this.TextBoxControl.Text);
            this.TextBoxItem.TextBoxControl.Refresh();
            this.PopupForm.ClosePopup(RadPopupCloseReason.Keyboard);
            this.LastTypedText = string.Empty;
            return true;
        }



        protected virtual void ProcessKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
            {
                this.OnKeyDown(e);
                return;
            }
            this.lastPressedKey = e.KeyCode;

            if (this.TogglePopupWithKeyboard(e))
            {
                this.OnKeyDown(e);
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Return:
                        {
                            e.Handled = this.ProcessReturnKey();
                        }
                        break;
                    case Keys.Delete:
                    case Keys.Back:
                        if (this.DropDownStyle == RadDropDownStyle.DropDownList)
                        {
                            this.BitState[KeyboardCommandIssuedStateKey] = true;
                            if (this.Items.Count > 0)
                            {
                                this.SelectedIndex = 0;
                            }
                            e.Handled = true;
                        }
                        break;
                    case Keys.Down:
                        this.BitState[KeyboardCommandIssuedStateKey] = true;
                        this.SelectNextItem();
                        e.Handled = true;
                        break;
                    case Keys.Up:
                        this.BitState[KeyboardCommandIssuedStateKey] = true;
                        this.SelectPreviousItem();
                        e.Handled = true;
                        break;
                    case Keys.Right:
                        if (this.DropDownStyle == RadDropDownStyle.DropDownList)
                        {
                            this.BitState[KeyboardCommandIssuedStateKey] = true;
                            this.SelectNextItem();
                            e.Handled = true;
                        }
                        break;
                    case Keys.Left:
                        if (this.DropDownStyle == RadDropDownStyle.DropDownList)
                        {
                            this.BitState[KeyboardCommandIssuedStateKey] = true;
                            this.SelectPreviousItem();
                            e.Handled = true;
                        }
                        break;
                    default:
                        break;
                }

                this.OnKeyDown(e);
            }
        }

        protected virtual string GetText(object item)
        {
            RadItem item1 = item as RadItem;
            if (item1 != null)
            {
                return item1.Text;
            }
            else if (item != null)
            {
                return Convert.ToString(item);
            }
            return null;
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!e.Handled)
            {
                string pressedChar = e.KeyChar.ToString();
                if (!char.IsControl(e.KeyChar) &&
                    !this.IsCommandKey(this.lastPressedKey) &&
                    (this.DropDownStyle == RadDropDownStyle.DropDownList))
                {
                    if ((DateTime.Now.Ticks - this.autoCompleteTimeStamp) > 10000000)
                    {
                        if (!((Control.ModifierKeys & Keys.ShiftKey) == Keys.ShiftKey))
                        {
                            partial = pressedChar.ToLower();
                        }
                        this.LastTypedText = partial;
                    }
                    else
                    {
                        partial += pressedChar.ToLower();
                        if (partial != this.lastTextChangedValue)
                        {
                            //base.OnTextChanged(e);
                            this.lastTextChangedValue = partial;
                            this.LastTypedText = partial;
                            Debug.WriteLine(partial);
                        }
                    }
                    this.autoCompleteTimeStamp = DateTime.Now.Ticks;

                    RadItem item;
                    if (this.ComboPopupForm.IndexChanging)
                    {
                        return;
                    }

                    int index = 0;
                    RadItemCollection items = this.FindAllItems(this.LastTypedText);
                    Debug.WriteLine(items.Count);
                    if (items.Count > 0)
                    {
                        bool typeInProgress = false;
                        if (partial.Length > 1)
                        {
                            typeInProgress = true;
                        }

                        if (this.SelectedItem != null)
                        {
                            index = items.IndexOf(this.SelectedItem as RadItem);
                            if (index > -1)
                            {
                                if (!typeInProgress)
                                {
                                    index++;
                                    if (index > (items.Count - 1))
                                    {
                                        index = 0;
                                    }
                                }
                            }
                            else
                            {
                                index = 0;
                            }
                        }
                        else
                        {
                            index = 0;
                        }
                        item = items[index];
                        index = 0;

                        if (this.autoCompleteMode == AutoCompleteMode.Suggest || this.autoCompleteMode == AutoCompleteMode.SuggestAppend)
                        {
                            if (!this.IsPopupOpen)
                            {
                                this.ShowPopup();
                            }
                        }
                        this.BitState[ListAutoCompleteIssuedStateKey] = true;
                        this.SelectedItem = item;
                        this.BitState[ListAutoCompleteIssuedStateKey] = false;
                        this.ListBoxElement.ScrollElementIntoView(item);
                    }
                }
            }
            this.OnKeyPress(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (this.IsDesignMode || !this.IsInValidState(true))
            {
                return;
            }
            if ((this.DropDownStyle == RadDropDownStyle.DropDownList) ||
                this.ComboPopupForm.IndexChanging)
            {
                return;
            }

            string text = this.TextBoxControl.Text;
            if (text != this.lastTextChangedValue)
            {
                this.lastTextChangedValue = text;
                this.LastTypedText = text;
            }

            base.Text = this.TextBoxControl.Text;

            if (string.IsNullOrEmpty(this.Text))
            {
                this.ComboPopupForm.ListBox.SetSelectedIndex(-1, false);
            }

            if (this.lastPressedKey == Keys.Delete ||
                 this.lastPressedKey == Keys.Back)
            {
                SelectItemByText();
                return;
            }

            switch (this.AutoCompleteMode)
            {
                case AutoCompleteMode.None:
                    SelectItemByText();
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
            }
            if (lastPressedKey != Keys.Up && lastPressedKey != Keys.Down)
            {
                this.SetActiveItem(this.Text);
            }
            else
            {
                this.SetActiveItem((RadComboBoxItem)this.SelectedItem);
            }
        }

        private void SelectItemByText()
        {
            RadComboBoxItem matchItem = FindItemExact(this.TextBoxControl.Text);
            RadListBoxElement lb = this.ComboPopupForm.ListBox;
            if (matchItem != null)
            {
                lb.SetSelectedIndex(matchItem.Index, false);
            }
            else
            {
                lb.SetSelectedIndex(-1, false);
            }
        }

        protected virtual RadItem SelectPreviousItem()
        {
            if (this.Items.Count == 0)
            {
                return null;
            }
            int index;
            if (this.ComboPopupForm.ActiveItem == null)
            {
                index = this.SelectedIndex;
            }
            else
            {
                index = this.Items.IndexOf(this.ComboPopupForm.ActiveItem);
            }
            index--;
            if (index < 0)
            {
                index = 0;
            }
            this.SelectedIndex = index;
            if (this.SelectedItem != this.ComboPopupForm.ActiveItem)
            {
                this.SetActiveItem(this.SelectedItem as RadComboBoxItem);
            }
            return this.SelectedItem as RadComboBoxItem;
        }

        protected virtual RadItem SelectNextItem()
        {
            if (this.Items.Count == 0)
            {
                return null;
            }
            int index;
            if (this.ComboPopupForm.ActiveItem == null)
            {
                index = this.SelectedIndex;
            }
            else
            {
                index = this.Items.IndexOf(this.ComboPopupForm.ActiveItem);
                if (index < 0)
                {
                    index = this.SelectedIndex;
                }
            }
            index++;
            if (index > (this.Items.Count - 1))
            {
                index = this.Items.Count - 1;
            }
            this.SelectedIndex = index;
            if (this.SelectedItem != this.ComboPopupForm.ActiveItem)
            {
                this.SetActiveItem(this.SelectedItem as RadComboBoxItem);
            }
            return this.SelectedItem as RadComboBoxItem;
        }

        private bool TogglePopupWithKeyboard(KeyEventArgs e)
        {
            if (this.ListBoxElement.Items.Count == 0)
            {
                return false;
            }

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

        private void SetAppendAutoComplete()
        {
            if (!string.IsNullOrEmpty(this.LastTypedText) && (this.lastPressedKey != Keys.Delete))
            {
                RadComboBoxItem item;
                if (this.lastPressedKey == Keys.Return && this.ComboPopupForm.ActiveItem != null)
                {
                    //item from the ListBox has been selected and the Return has been pressed
                    this.textBox.SetTextBoxTextSilently(this.ComboPopupForm.ActiveItem.Text);
                }
                else
                {
                    //user has changed the text in the editor
                    //item = FindItem(this.lastChangedText);
                    item = FindItem(this.LastTypedText);
                    if (item != null)
                    {
                        if (this.lastPressedKey != Keys.None)
                        {
                            this.textBox.SetTextBoxTextSilently(item.Text);
                        }
                        //int appendStart = this.lastChangedText.Length;
                        int appendStart = this.LastTypedText.Length;
                        this.textBox.SelectionStart = appendStart;
                        this.textBox.SelectionLength = item.Text.Length - appendStart;
                    }
                }
            }
        }

        private void SetSuggestAutoComplete()
        {
            if (this.IsPopupOpen)
            {
                if (lastPressedKey != Keys.Up && lastPressedKey != Keys.Down)
                {
                    RadComboBoxItem item = this.FindItemExact(this.Text);
                    if (item != null)
                    {
                        this.SelectedItem = item;
                        this.LastTypedText = string.Empty;
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

            if (e.Property == RadComboBoxElement.DropDownStyleProperty)
            {
                if ((RadDropDownStyle)e.NewValue == RadDropDownStyle.DropDownList)
                {
                    if (this.FindItemExact(this.Text) == null)
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
            else if (e.Property == RadComboBoxElement.CaseSensitiveProperty)
            {
                this.OnCaseSensitiveChanged(EventArgs.Empty);
            }
            else if (e.Property == RadComboBoxElement.DropDownAnimationEnabledProperty)
            {
                this.ComboPopupForm.AnimationEnabled = (bool)e.NewValue;
            }
            else if (e.Property == RadComboBoxElement.DropDownAnimationEasingProperty)
            {
                this.ComboPopupForm.AnimationProperties.EasingType = (RadEasingType)e.NewValue;
            }
            else if (e.Property == RadComboBoxElement.DropDownAnimationFramesProperty)
            {
                this.ComboPopupForm.AnimationProperties.AnimationFrames = (int)e.NewValue;
            }
            else if (e.Property == RadComboBoxElement.IsDropDownShownProperty)
            {
                foreach (RadElement child in this.ChildrenHierarchy)
                {
                    child.SetValue(RadComboBoxElement.IsDropDownShownProperty, e.NewValue);
                }
            }
            else if (e.Property == RadItem.TextProperty)
            {
                this.ComboPopupForm.SetActiveItem((string)e.NewValue);
            }
            else if (e.Property == RadElement.RightToLeftProperty)
            {
                if ((bool)e.NewValue)
                {
                    this.ComboPopupForm.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                }
                else
                {
                    this.ComboPopupForm.RightToLeft = System.Windows.Forms.RightToLeft.No;
                }
            }
            else if (e.Property == RadObject.BindingContextProperty)
            {
                this.PopupForm.BindingContext = this.BindingContext;
            }
            else if (e.Property == RadElement.VisibilityProperty)
            {
                if (this.IsPopupOpen)
                {
                    this.ClosePopup(RadPopupCloseReason.CloseCalled);
                }
            }
        }

        protected override bool CanDisplayPopup()
        {
            return (this.Items.Count > 0 ) && base.CanDisplayPopup();
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (this.DblClickRotate && this.Items.Count > 0)
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

        private void textBox_DoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta == 0)
            {
                return;
            }

            if (this.IsPopupOpen)
            {
                if (e.Delta > 0)
                {
                    this.ComboPopupForm.DoScrollLineUp();
                }
                else
                {
                    this.ComboPopupForm.DoScrollLineDown();
                }
            }

            if (e is HandledMouseEventArgs)
            {
                if ((e as HandledMouseEventArgs).Handled)
                {
                    return;
                }
            }

            if (!this.IsPopupOpen)
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
        }

        //private bool textIsFromSelectedItem = false;

        // update the text box text, and combo element text if required, 
        // then selects the text if possible.
        internal void SyncTextWithItem()
        {
            string text1 = "";
            if (this.SelectedIndex != -1 && this.SelectedIndex < this.Items.Count)
            {
                RadItem element = this.Items[this.SelectedIndex];
                if (element != null)
                {
                    text1 = element.Text;
                    //this.textIsFromSelectedItem = true;
                }
            }

            if ((this.TextBoxControl.Text != text1) &&
                (this.Text != text1))
            {
                //becuse of AutoCompleteMode.Append, when user types text and press Enter
                this.Text = text1;
            }
            else
            {
                this.TextBoxControl.Text = text1;
            }
            this.SelectAllText(text1);
        }

        //GEO - this is temporary fix intended for the autocomplete (Called from the RadComboBox control)
        internal void NotifyOnLeave()
        {
            if (this.AutoCompleteEnabled)
            {
                this.NotifyAutoComplete();
            }
        }

        internal void NotifyAutoComplete()
        {
            string text1 = this.TextBoxControl.Text;

            if(this.SelectedItem != null)
            {
                RadItem item = this.SelectedItem as RadItem;
                if (item != null && item.Text == text1)
                {
                    this.TextBoxControl.DeselectAll();
                    return;
                }
                // This fix is here because the FindItemExact() below will return the first item with text1.
                // However if there are more items with the same text and one of them is currently selected,
                // the selection will change, which is wrong.
            }

            int index = this.Items.IndexOf(this.FindItemExact(text1));
            if (index != -1)
            {
                if (this.Text != text1)
                {
                    this.Text = text1;
                }

                this.SelectedIndex = index;
                this.SelectAllText(text1);
            }
        }

        internal bool AutoCompleteEnabled
        {
            get
            {
                return (this.autoCompleteMode != AutoCompleteMode.None) && (this.DropDownStyle != RadDropDownStyle.DropDownList);
            }
        }

        private bool SelectAllText(string text)
        {
            if (!this.TextBoxControl.ReadOnly && this.textBox.TextBoxControl.Focused)
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
        public RadComboBoxItem FindItem(string startsWith)
        {
            return this.ComboPopupForm.ListBox.FindItem(startsWith) as RadComboBoxItem;
        }

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string. 
        /// </summary>
        /// <param name="text">The String to search for.</param>
        /// <returns>The first RadCOmboBoxItem found; returns null if no match is found.</returns>
        public RadComboBoxItem FindItemExact(string text)
        {
            return this.ComboPopupForm.ListBox.FindItemExact(text) as RadComboBoxItem;
        }

        /// <summary>
        /// Finds all items in the list box that starts with the specified string. 
        /// </summary>
        /// <param name="text">The string to search for.</param>
        /// <returns>Collection of items that match the criteria.</returns>
        public RadItemCollection FindAllItems(string text)
        {
            return this.ComboPopupForm.ListBox.FindAllItems(text);
        }

        internal void SetActiveItem(RadComboBoxItem item)
        {
            this.ComboPopupForm.SetActiveItem(item);
            this.ListBoxElement.ScrollElementIntoView(item);
        }

        internal void SetActiveItem(string text)
        {
            RadItem item = this.ComboPopupForm.SetActiveItem(text);
            if (item != null)
            {
                this.ListBoxElement.ScrollElementIntoView(item);
            }
        }

        /// <summary>
        /// Call BeginUpdate at the begining of a block that makes many modifications in the GUI
        /// <seealso cref="EndUpdate"/>
        /// </summary>
        public virtual void BeginUpdate()
        {
            if (this.ListBoxElement != null)
            {
                this.ListBoxElement.BeginUpdate();
            }
        }

        /// <summary>
        /// Call BeginUpdate at the end of a block that makes many modifications in the GUI
        /// <seealso cref="BeginUpdate"/>
        /// </summary>
        public virtual void EndUpdate()
        {
            if (this.ListBoxElement != null)
            {
                this.ListBoxElement.EndUpdate();
            }
        }

        /// <summary>
        /// Ends the initialization of a RadComboBoxElement control that is used on a form or used by another component. 
        /// The initialization occurs at run time. 
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();
            this.ListBoxElement.EndInit();
        }

        /// <summary>
        /// Call the GetItemHeight member function to retrieve the height of list items in a combo box.
        /// </summary>
        /// <param name="index">Specifies the item of the combo box whose height is to be retrieved.</param>
        /// <returns></returns>
        public int GetItemHeight(int index)
        {
            //return this.Items[index].Size.Height;
            return this.ListBoxElement.GetItemHeight(index);
        }

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

        protected override void UpdatePopupMinMaxSize(RadPopupControlBase popup)
        {
            base.UpdatePopupMinMaxSize(popup);

            ComboPopupForm comboPopup = popup as ComboPopupForm;
            if (comboPopup == null)
            {
                return;
            }

            if (comboPopup.SizingMode != SizingMode.None)
            {
                comboPopup.MinimumSize = LayoutUtils.UnionSizes(this.dropDownMinSize, comboPopup.SizingGrip.Children[3].FullSize);
                if (this.dropDownMaxSize != Size.Empty)
                {
                    comboPopup.MaximumSize = this.dropDownMaxSize;
                }
            }
            else
            {
                comboPopup.MinimumSize = Size.Empty;
                comboPopup.MaximumSize = Size.Empty;
            }


        }

        protected override Size GetPopupSize(RadPopupControlBase popup, bool measure)
        {
            Size dropDownSize = new Size(this.GetPopupWidth(), this.dropDownHeight);
            ComboPopupForm comboPopup = popup as ComboPopupForm;

            if (comboPopup == null)
            {
                return dropDownSize;
            }

            if (comboPopup.ListBox.IntegralHeight || this.Virtualized)
            {
                RadListBoxElement listBoxElement = comboPopup.ListBox;
                RadElement viewport = listBoxElement.Viewport;
                int dropDownItems = Math.Min(listBoxElement.Items.Count, this.maxDropDownItems);

                if (viewport.Children.Count == 0)
                {
                    for (int i = 0; i < dropDownItems; i++)
                    {
                        viewport.Children.Add(listBoxElement.Items[i]);
                    }

                    comboPopup.RootElement.Measure(new SizeF(float.PositiveInfinity, float.PositiveInfinity));
                    dropDownSize.Height = (int)Math.Round(viewport.DesiredSize.Height);
                    // Prevent subsequent scrolling from throwing exception "Child already added"
                    viewport.Children.Clear();
                }
                else
                {
                    dropDownSize.Height = 0;
                    if (viewport.Children.Count > 0)
                    {
                        viewport.UpdateLayout();
                        dropDownSize.Height = dropDownItems * viewport.Children[0].FullSize.Height;
                    }
                }
                // Add BorderThickness and Paddings
                dropDownSize.Height += (int)Math.Round(listBoxElement.DesiredSize.Height);

                if (comboPopup.SizingMode != SizingMode.None)
                {
                    dropDownSize.Height += (int)Math.Round(comboPopup.SizingGrip.DesiredSize.Height);
                }
            }

            return dropDownSize;
        }

        protected override Size GetInitialPopupSize()
        {
            if (!this.IsInValidState(true))
                return Size.Empty;
            int initialHeight = 0;

            int visibleItemsCount = Math.Min(this.maxDropDownItems, this.Items.Count);

            for (int i = 0; i < visibleItemsCount; i++)
            {
                SizeF desiredSize =
                    MeasurementControl.ThreadInstance.GetDesiredSize(this.Items[i], new SizeF(float.PositiveInfinity, float.PositiveInfinity));
                initialHeight += (int)desiredSize.Height;
            }

            return new Size(this.GetPopupWidth(), initialHeight);
        }

        private int GetPopupWidth()
        {
            int width = this.dropDownWidth;
            if (width == DefaultDropDownWidth)
            {
                width = this.Size.Width;
            }

            return width;
        }

        protected override RadPopupControlBase CreatePopupForm()
        {
            ComboPopupForm popupForm = new ComboPopupForm(this);
            popupForm.RightToLeft = this.RightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            popupForm.MinimumSize = this.DropDownMaxSize;
            popupForm.MaximumSize = this.DropDownMaxSize;
            popupForm.Height = this.DropDownHeight;

            //p.p. 28.07.09 sync. Font and ForeColor
            popupForm.Font = this.Font;
            popupForm.ForeColor = this.ForeColor;
            popupForm.VerticalAlignmentCorrectionMode = AlignmentCorrectionMode.SnapToOuterEdges;
            popupForm.HorizontalAlignmentCorrectionMode = AlignmentCorrectionMode.Smooth;
            return popupForm;
        }

        private void BringSelectedItemIntoView()
        {
            ComboPopupForm comboPopupForm = this.ComboPopupForm;
            if (comboPopupForm == null)
            {
                return;
            }

            if (this.SelectedItem != null)
            {
                comboPopupForm.ListBox.ScrollElementIntoView(this.SelectedItem as RadComboBoxItem);
            }
            else
            {
                comboPopupForm.ListBox.ScrollToHome();
            }
        }

        [Obsolete("Please use the ShowPopup method instead.")] // Marked obsolete for the Q3 2009 release.
        public void ShowDropDown()
        {
            this.ShowPopup();
        }

        [Obsolete("Please use the ClosePopup method instead.")] // Marked obsolete for the Q3 2009 release.
        public void CloseDropDown()
        {
            this.ClosePopup();
        }

        protected override void ShowPopupCore(RadPopupControlBase popup)
        {
            base.ShowPopupCore(popup);

            this.BringSelectedItemIntoView();

            RadComboBoxItem viewItem = this.SelectedItem as RadComboBoxItem;

            if (viewItem != null)
            {
                this.SetActiveItem(viewItem);
            }
        }

        protected override void OnPopupClosed(RadPopupClosedEventArgs e)
        {
            base.OnPopupClosed(e);

            this.IsDropDownShown = false;
        }

        protected override void OnPopupOpened(EventArgs args)
        {
            base.OnPopupOpened(args);

            this.IsDropDownShown = true;
        }

        #endregion

        #region System skinning

        protected override void OnUseSystemSkinChanged(EventArgs e)
        {
            base.OnUseSystemSkinChanged(e);

            this.ComboPopupForm.RootElement.Children[0].UseSystemSkin = this.ElementTree.RootElement.UseSystemSkin;
        }

        protected override void PaintElementSkin(Telerik.WinControls.Paint.IGraphics graphics)
        {
            if (object.ReferenceEquals(SystemSkinManager.Instance.CurrentElement, SystemSkinManager.DefaultElement))
            {
                return;
            }

            base.PaintElementSkin(graphics);
        }

        protected override void InitializeSystemSkinPaint()
        {
            base.InitializeSystemSkinPaint();

            if (DWMAPI.IsVista)
            {
                this.borderPrimitive.Visibility = ElementVisibility.Hidden;
                this.fillPrimitive.Visibility = ElementVisibility.Hidden;
            }
        }

        protected override void UnitializeSystemSkinPaint()
        {
            base.UnitializeSystemSkinPaint();

            if (DWMAPI.IsVista)
            {
                this.borderPrimitive.Visibility = ElementVisibility.Visible;
                this.fillPrimitive.Visibility = ElementVisibility.Visible;
            }
        }

        public override VisualStyleElement GetVistaVisualStyle()
        {

            if (!this.Enabled)
            {
                return VistaAeroTheme.ComboBox.Border.Disabled;
            }
            else
            {
                if (this.DropDownStyle == RadDropDownStyle.DropDown)
                {
                    if (!this.IsMouseOver && !this.IsMouseDown && !this.IsFocused)
                    {
                        return VistaAeroTheme.ComboBox.Border.Normal;
                    }
                    else if (this.IsMouseOver && !this.IsMouseDown && !this.IsFocused)
                    {
                        return VistaAeroTheme.ComboBox.Border.Hot;
                    }
                    else if (this.IsFocused)
                    {
                        return VistaAeroTheme.ComboBox.Border.Focused;
                    }
                }
                else
                {
                    if (!this.IsMouseOver && !this.IsMouseDown && !this.IsFocused)
                    {
                        return VistaAeroTheme.ComboBox.Readonly.Normal;
                    }
                    else if (this.IsMouseOver && !this.IsMouseDown && !this.IsFocused)
                    {
                        return VistaAeroTheme.ComboBox.Readonly.Hot;
                    }
                    else if (this.IsMouseDown)
                    {
                        return VistaAeroTheme.ComboBox.Readonly.Pressed;
                    }
                }
            }

            return SystemSkinManager.DefaultElement;

        }

        #endregion
    }
}
