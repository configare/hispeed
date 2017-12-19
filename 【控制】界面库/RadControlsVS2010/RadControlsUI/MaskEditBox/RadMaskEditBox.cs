using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using System.Globalization;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    [TelerikToolboxCategory(ToolboxGroupStrings.EditorsGroup)]
    [ToolboxItem(true)]
    [Description("Uses a mask to distinguish between proper and improper user input")]
    [DefaultProperty("Mask")]
    [Designer(DesignerConsts.RadMaskEditBoxDesignerString)]
    public class RadMaskedEditBox : RadControl
    {
        protected RadMaskedEditBoxElement maskEditBoxElement;
        private string cachedMask = string.Empty;
        private static readonly object EventKeyDown = new object();
        private static readonly object EventKeyPress = new object();
        private static readonly object EventKeyUp = new object();
        private static readonly object MultilineChangedEventKey = new object();
        private static readonly object TextAlignChangedEventKey = new object();

        public RadMaskedEditBox()
        {
            this.AutoSize = true;
            this.TabStop = false;
            this.SetStyle(ControlStyles.Selectable, true);
            this.WireEvents();
        }

        #region Events
   
        /// <summary>
        /// Occurs when the editing value has been changed
        /// </summary>
        [Description("Occurs when the editing value has been changed")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged(object sender, EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when the editing value is changing.
        /// </summary>
        [Description(" Occurs when the editing value is changing.")]
        [Category("Action")]
        public event CancelEventHandler ValueChanging;

        /// <summary>
        /// Fires the ValueChanging event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnValueChanging(object sender, CancelEventArgs e)
        {
            if (this.ValueChanging != null)
            {
                ValueChanging(this, e);
            }
        }

        #endregion

        #region Overrides

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 20);
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.UnwireEvents();
            base.Dispose(disposing);            
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.maskEditBoxElement = new RadMaskedEditBoxElement();
            this.RootElement.Children.Add(this.maskEditBoxElement);
        }

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchVertically = false;
        }

        #endregion

        /// <summary>
        /// Gets or sets the character used as the placeholder in a masked editor.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the character used as the placeholder in a masked editor."),
        Obsolete("Obsolete, Please use PromtChar instead"),
        DefaultValue('_'),
        Localizable(true),
        RefreshProperties(RefreshProperties.All)]
        public char PlaceHolder
        {
            get
            {
                return this.PromptChar;
            }
            set
            {
                this.PromptChar = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public virtual RadMaskedEditBoxElement MaskedEditBoxElement
        { 
            get
            {
                return maskEditBoxElement;
            }
            set
            {
                maskEditBoxElement = value;
            }
        }

        /// <summary>
        /// Gets or sets a mask expression.
        /// </summary>
        [Description("Gets or sets a mask expression."),
        DefaultValue(""),
        Localizable(true),
        RefreshProperties(RefreshProperties.All)]
        [Category("Behavior")]
        public string Mask
        {
            get
            {
                if (cachedMask != string.Empty)
                {
                    return this.maskEditBoxElement.Mask;	
                }

                return cachedMask;//e.g. empty 
            }
            set
            {
                if (this.maskEditBoxElement.Mask != value)
                {
                    this.cachedMask = value;
                    this.maskEditBoxElement.Mask = value;
                    OnNotifyPropertyChanged("Mask");                    
                }
            }
        }

        /// <summary>
        /// Gets or sets the mask type.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the mask type."),
        DefaultValue(MaskType.None),
        Localizable(true),
        RefreshProperties(RefreshProperties.All)]
        public MaskType MaskType
        {
            get
            {
                return this.maskEditBoxElement.MaskType;
            }
            set
            {
                if (value != this.maskEditBoxElement.MaskType)
                {
                    this.maskEditBoxElement.MaskType = value;
                    OnNotifyPropertyChanged("MaskType");
                }
            }
        }

        [Browsable(true),
        Localizable(true),		
        Category(RadDesignCategory.BehaviorCategory),
        Description("Gets or sets the text associated with this control."),
        Bindable(true),
        SettingsBindable(true), 
        DefaultValue(""),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return maskEditBoxElement.Text;
            }
            set
            {
                this.maskEditBoxElement.Text = value;
                this.OnNotifyPropertyChanged("Text");
            }
        }

        /// <summary>
        /// selects the whole text
        /// </summary>
        public void SelectAll()
        {
            this.maskEditBoxElement.TextBoxItem.SelectAll();
        }

        /// <summary>
        /// Gets or sets the value associated to the mask edit box
        /// </summary>
        [Description("Gets or sets the value associated to the mask edit box")]
        [Category("Behavior")]
        [Browsable(true), Bindable(true)/*, Localizable(false)*/]
        [DefaultValue("")]        
        public object Value
        {
            get
            {
                return this.maskEditBoxElement.Value;
            }
            set
            {
                this.maskEditBoxElement.Value = value;
                this.OnNotifyPropertyChanged("Value");
            }
        }

        [System.ComponentModel.LocalizableAttribute(true)]      
        [System.ComponentModel.DefaultValueAttribute(32767)]
        public int MaxLength
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.MaxLength;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.MaxLength = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the bottom part of characters, clipped 
        /// due to font name or size particularities
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [RadDescription("UseGenericBorderPaint", typeof(RadTextBoxElement))]
        [RadDefaultValue("UseGenericBorderPaint", typeof(RadTextBoxElement))]
        public bool UseGenericBorderPaint
        {
            get
            {
                return this.maskEditBoxElement.UseGenericBorderPaint;
            }
            set
            {
                this.maskEditBoxElement.UseGenericBorderPaint = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets or sets the horizontal alignment of the text.")]
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TextAlign
        {
            get
            {
                return this.maskEditBoxElement.TextAlign;
            }
            set
            {
                this.maskEditBoxElement.TextAlign = value;
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// a value indicating whether the defined shortcuts are enabled.</span>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
        public bool ShortcutsEnabled
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.ShortcutsEnabled;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.ShortcutsEnabled = value;
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// the starting point of text selected in the text box.</span>
        /// </summary>
        [DefaultValue(0)]
        public int SelectionStart
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.SelectionStart;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.SelectionStart = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RadTextBox control has been modified
        /// by the user since the control was created or since its contents were last set.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(false)]
        public bool Modified
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.Modified;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.Modified = value;
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// a value indicating whether this is a multiline TextBox control.</span>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("Multiline", typeof(RadTextBoxItem))]
        public bool Multiline
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.Multiline;
            }
            set
            {
                // Allow save of the size when switching Multiline
                bool oldAutoSize = this.AutoSize;
                if (oldAutoSize)
                {
                    this.AutoSize = false;
                }
                this.maskEditBoxElement.TextBoxItem.Multiline = value;
                if (oldAutoSize)
                {
                    this.AutoSize = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text that is displayed when the ComboBox contains a null 
        /// reference. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Localizable(true)]
        [RadDefaultValue("NullText", typeof(RadTextBoxItem))]
        public string NullText
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.NullText;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.NullText = value;
                this.OnNotifyPropertyChanged("NullText");
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// the character used to mask characters of a password in a single-line TextBox
        /// control.</span>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("PasswordChar", typeof(RadTextBoxItem))]
        public char PasswordChar
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.PasswordChar;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.PasswordChar = value;
                this.OnNotifyPropertyChanged("PasswordChar");
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// a value indicating whether the contents of the TextBox control can be
        /// changed.</span>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.ReadOnly;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.ReadOnly = value;
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// which scroll bars should appear in a multiline TextBox control.</span>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(System.Windows.Forms.ScrollBars.None)]
        public ScrollBars ScrollBars
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.ScrollBars;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.ScrollBars = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the currently selected text in the
        /// control.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue("")]
        public string SelectedText
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.SelectedText;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.SelectedText = value;
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// the number of characters selected in the text box.</span>
        /// </summary>
        [DefaultValue(0)]
        public int SelectionLength
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.SelectionLength;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.SelectionLength = value;
            }
        }

        /// <summary>
        /// 	<para>Gets or sets a value indicating whether the selected text remains highlighted
        ///     even when the RadTextBox has lost the focus.</para>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
        public bool HideSelection
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.HideSelection;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.HideSelection = value;
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// the lines of text in multiline configurations.</span>
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] Lines
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.Lines;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.Lines = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether pressing ENTER in a multiline RadTextBox
        /// control creates a new line of text in the control or activates the default button for
        /// the form.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("AcceptsReturn", typeof(RadTextBoxItem))]
        public bool AcceptsReturn
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.AcceptsReturn;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.AcceptsReturn = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether pressing the TAB key in a multiline text
        /// box control types a TAB character in the control instead of moving the focus to the
        /// next control in the tab order.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("AcceptsTab", typeof(RadTextBoxItem))]
        public bool AcceptsTab
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.AcceptsTab;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.AcceptsTab = value;
            }
        }

        /// <summary>
        /// 	<para>Gets or sets a value indicating whether the RadTextBox control modifies the
        ///     case of characters as they are typed.</para>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(System.Windows.Forms.CharacterCasing.Normal)]
        public CharacterCasing CharacterCasing
        {
            get
            {
                return this.maskEditBoxElement.TextBoxItem.CharacterCasing;
            }
            set
            {
                this.maskEditBoxElement.TextBoxItem.CharacterCasing = value;
            }
        }

        /// <summary>
        /// Gets or sets the current culture associated to the RadMaskBox
        /// </summary>
        [Description("Gets or sets the current culture associated to the RadMaskBox")]
        [Category("Behavior")]
        public CultureInfo Culture
        {
            get
            {
                return this.maskEditBoxElement.Culture;
            }
            set
            {
                if (!object.ReferenceEquals(value, this.maskEditBoxElement.Culture))
                {
                    this.maskEditBoxElement.Culture = value;
                    OnNotifyPropertyChanged("Culture");
                }
            }
        }

        [Category("Appearance"),
        Localizable(true),
        DefaultValue('_'), 
        RefreshProperties(RefreshProperties.Repaint), 
        Description("MaskedTextBox Prompt Char")]
        public char PromptChar
        {
            get
            {
                return this.maskEditBoxElement.PromptChar;
            }
            set
            {
                this.maskEditBoxElement.PromptChar = value; 
            }
        }

        [DefaultValue(true),
        Description("MaskedTextBox Allow Prompt As Input"),
        Category("Behavior")]
        public bool AllowPromptAsInput
        {
            get
            {
                return this.maskEditBoxElement.AllowPromptAsInput;
            }
            set
            {
                this.maskEditBoxElement.AllowPromptAsInput = value;
            }
        }

        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        Description("Gets or sets a custom StringCollection to use when the AutoCompleteSource property is set to CustomSource."),
        Browsable(true),
        RadDefaultValue("AutoCompleteCustomSource", typeof(HostedTextBoxBase)),
        EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get
            {
                return ((TextBox)this.maskEditBoxElement.TextBoxItem.HostedControl).AutoCompleteCustomSource;
            }
            set
            {
                ((TextBox)this.maskEditBoxElement.TextBoxItem.HostedControl).AutoCompleteCustomSource = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always),
        Browsable(true),
        RadDefaultValue("AutoCompleteMode", typeof(HostedTextBoxBase)),
        Description("Gets or sets an option that controls how automatic completion works for the TextBox.")]
        public AutoCompleteMode AutoCompleteMode
        {
            get
            {
                return ((TextBox)this.maskEditBoxElement.TextBoxItem.HostedControl).AutoCompleteMode;
            }
            set
            {
                ((TextBox)this.maskEditBoxElement.TextBoxItem.HostedControl).AutoCompleteMode = value;
            }
        }

        [Browsable(true), TypeConverter(typeof(TextBoxAutoCompleteSourceConverter)),
        EditorBrowsable(EditorBrowsableState.Always),
        Description("Gets or sets a value specifying the source of complete strings used for automatic completion."),
        RadDefaultValue("AutoCompleteSource", typeof(HostedTextBoxBase))]
        public AutoCompleteSource AutoCompleteSource
        {
            get
            {
                return ((TextBox)this.maskEditBoxElement.TextBoxItem.HostedControl).AutoCompleteSource;
            }
            set
            {
                ((TextBox)this.maskEditBoxElement.TextBoxItem.HostedControl).AutoCompleteSource = value;
            }
        }

        #region Helpers

        protected internal void WireEvents()
        {
            this.maskEditBoxElement.ValueChanged += new EventHandler(OnValueChanged);
            this.maskEditBoxElement.ValueChanging += new CancelEventHandler(OnValueChanging);
            this.maskEditBoxElement.TextChanged += new EventHandler(maskEditBoxElement_TextChanged);
            this.maskEditBoxElement.KeyDown += new KeyEventHandler(OnKeyDown);
            this.maskEditBoxElement.KeyPress += new KeyPressEventHandler(OnKeyPress);
            this.maskEditBoxElement.KeyUp += new KeyEventHandler(OnKeyUp);
            this.maskEditBoxElement.MultilineChanged += new EventHandler(OnMultilineChanged);
            this.maskEditBoxElement.TextAlignChanged += new EventHandler(OnTextAlignChanged);
        }

        public virtual void OnTextAlignChanged(object sender, EventArgs e)
        {
            EventHandler handler = (EventHandler)base.Events[TextAlignChangedEventKey];
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public virtual void OnMultilineChanged(object sender, EventArgs e)
        {
            EventHandler handler = (EventHandler)base.Events[MultilineChangedEventKey];
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public virtual void OnKeyUp(object sender, KeyEventArgs e)
        {
            KeyEventHandler KeyUp = (KeyEventHandler)base.Events[EventKeyUp];
            if (KeyUp != null)
            {
                KeyUp(sender, e);
            }
        }

        public virtual void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEventHandler KeyPress = (KeyPressEventHandler)base.Events[EventKeyPress];
            if (KeyPress != null)
            {
                KeyPress(sender, e);
            }
        }

        public virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
            KeyEventHandler KeyDown = (KeyEventHandler)base.Events[EventKeyDown];
            if (KeyDown != null)
            {
                KeyDown(sender, e);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user pressees a key down
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key down")]
        public event KeyEventHandler KeyDown
        {
            add
            {
                base.Events.AddHandler(EventKeyDown, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventKeyDown, value);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user pressees a key
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key")]
        public event KeyPressEventHandler KeyPress
        {
            add
            {
                base.Events.AddHandler(EventKeyPress, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventKeyPress, value);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user releases the pressed key up
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user releases the pressed key up")]
        public event KeyEventHandler KeyUp
        {
            add
            {
                base.Events.AddHandler(EventKeyUp, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventKeyUp, value);
            }
        }

        string oldText = string.Empty;

        void maskEditBoxElement_TextChanged(object sender, EventArgs e)
        {
            if (this.Text != oldText)
            {
                this.OnTextChanged(e);
                this.oldText = this.Text;
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl02_LabelAbstract">Occurs when
        /// the value of the Multiline property has changed.</span>
        /// </summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the Multiline property has changed.")]
        public event EventHandler MultilineChanged
        {
            add
            {
                this.Events.AddHandler(MultilineChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(MultilineChangedEventKey, value);
            }
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl03_LabelAbstract">Occurs when
        /// the value of the TextAlign property has changed.</span>
        /// </summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the TextAlign property has changed.")]
        public event EventHandler TextAlignChanged
        {
            add
            {
                this.Events.AddHandler(TextAlignChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(TextAlignChangedEventKey, value);
            }
        }

        protected internal void UnwireEvents()
        {
            this.maskEditBoxElement.ValueChanged -= new EventHandler(OnValueChanged);
            this.maskEditBoxElement.ValueChanging -= new CancelEventHandler(OnValueChanging);
            this.maskEditBoxElement.TextChanged -= new EventHandler(maskEditBoxElement_TextChanged);
            this.maskEditBoxElement.KeyDown -= new KeyEventHandler(OnKeyDown);
            this.maskEditBoxElement.KeyPress -= new KeyPressEventHandler(OnKeyPress);
            this.maskEditBoxElement.KeyUp -= new KeyEventHandler(OnKeyUp);
            this.maskEditBoxElement.MultilineChanged -= new EventHandler(OnMultilineChanged);
            this.maskEditBoxElement.TextAlignChanged -= new EventHandler(OnTextAlignChanged);
        }

        /// <summary>
        /// Clears all text from the text box control.
        /// </summary>
        public void Clear()
        {
            this.maskEditBoxElement.TextBoxItem.Clear();
        }

        /// <summary>
        /// Clears information about the most recent operation from the undo buffer of the
        /// text box.
        /// </summary>
        public void ClearUndo()
        {
            this.maskEditBoxElement.TextBoxItem.ClearUndo();
        }

        #endregion
    }
}