using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Telerik.WinControls.UI
{
    public class RadMaskedEditBoxElement : RadTextBoxElement
    {
        #region Fields 
        protected IMaskProvider provider;
        protected MaskedTextResultHint hint;
        protected char passwordChar = '*';
        protected int hintPos;
        protected bool restrictToAscii;
        protected CultureInfo culture;
        protected string unmaskedText;

        protected MaskType maskType = MaskType.None;
        protected string mask = "";
        protected string cachedMask = "";
        protected bool isNullValue = false;
        protected bool allowPromptAsInput;
        protected char promptChar = '_';
        protected bool isKeyBoard = false;

        protected internal DateTime minDate = DateTime.MinValue;
        protected internal DateTime maxDate = DateTime.MaxValue;
        #endregion

        #region CStors


        static RadMaskedEditBoxElement()
        {
            new Themes.ControlDefault.MaskEditBox().DeserializeTheme();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>There is no mask applied by default</remarks>
        public RadMaskedEditBoxElement()
        {
            this.CreateMaskProvider();
            this.TextBoxItem.KeyPress +=TextBoxItem_KeyPress;
            this.TextBoxItem.KeyDown +=TextBoxItem_KeyDown;
            this.TextBoxItem.Click +=TextBoxItem_Click;
            this.TextBoxItem.MouseWheel +=TextBoxItem_MouseWheel;
           // this.TextBoxItem.TextChanged += TextBoxItem_TextChanged;

            ((TextBox)this.TextBoxItem.HostedControl).ContextMenu = new ContextMenu();
        }      
        

        #endregion

        #region Overrides

        protected override void DisposeManagedResources()
        {
            this.TextBoxItem.KeyPress -= TextBoxItem_KeyPress;
            this.TextBoxItem.KeyDown -=TextBoxItem_KeyDown;
            this.TextBoxItem.Click -=TextBoxItem_Click;
            this.TextBoxItem.MouseWheel -=TextBoxItem_MouseWheel;
           // this.TextBoxItem.TextChanged -= TextBoxItem_TextChanged;
            base.DisposeManagedResources();
        }
       

        #endregion

        #region Events
        /// <summary>
        /// Occurs when the editing value has been changed
        /// </summary>
        [Description("Occurs when the editing value has been changed")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        /// <summary>
        /// Occurs when the editing value is changing.
        /// </summary>
        [Description(" Occurs when the editing value is changing.")]
        [Category("Action")]
        public event CancelEventHandler ValueChanging;
      
        /// <summary>
        /// Fires the ValueChanged event
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void CallValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Fires the ValueChanging event
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void CallValueChanging(CancelEventArgs e)
        {
            if (ValueChanging != null)
            {
                ValueChanging(this, e);
            }
        }


       
        
        #endregion

        #region Handlers

        //string oldTextValue = string.Empty;
        //void TextBoxItem_TextChanged(object sender, EventArgs e)
        //{
        //    if (this.Text != oldTextValue)
        //    {
        //        this.oldTextValue = this.Text;
        //        this.OnTextChanged(e);
        //    }
        //}


        void TextBoxItem_Click(object sender, EventArgs e)
        {
            this.provider.Click();
        }

        void TextBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            bool shouldRestoreNullValue = isNullValue;
            this.isNullValue = false;
            this.isKeyBoard = true;
            KeyEventArgsWithMinMax args = new KeyEventArgsWithMinMax(e.KeyData, this.minDate, this.maxDate);
            this.provider.KeyDown(sender, args);
            e.Handled = args.Handled;
            this.isNullValue = shouldRestoreNullValue && !e.Handled;
            this.isKeyBoard = false;
        }

        void TextBoxItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool shouldRestoreNullValue = isNullValue;
            this.isNullValue = false;
            this.isKeyBoard = true;
            this.HandleKeyPress(e);
            if (!e.Handled)
            {
                this.provider.KeyPress(sender, e);
            }
            this.isNullValue = shouldRestoreNullValue && !e.Handled;
            this.isKeyBoard = false;
        }



        void TextBoxItem_MouseWheel(object sender, MouseEventArgs e)
        {
            this.isNullValue = false;
            if(e.Delta>0)    
            {   
                this.Up();
            }
            else
            {
                this.Down();
            }
        }

        public void Up()
        {
            this.isNullValue = false;
            this.provider.KeyDown(this, new KeyEventArgsWithMinMax(Keys.Up, this.minDate, this.maxDate));
           
        }

        public void Down()
        {
            this.isNullValue = false;
            this.provider.KeyDown(this, new KeyEventArgsWithMinMax(Keys.Down, this.minDate, this.maxDate));            
        }

        /// <summary>
        /// handles the key press
        /// </summary>
        /// <param name="e"></param>
        public void HandleKeyPress(KeyPressEventArgs e)
        {
            bool shouldRestoreNullValue = isNullValue;
            this.isNullValue = false;

            switch (e.KeyChar)
            {
                case '\x03':
                    MakeClipboardCopy();
                    e.Handled = true;
                    break;
                case '\x16':
                    MakeClipboardPaste();
                    e.Handled = true;
                   
                    break;
                case '\x18':
                    MakeClipboardCut();
                    e.Handled = true;
                  
                    break;
                default:
                    break;
            }

            this.isNullValue = shouldRestoreNullValue && !e.Handled;
        }
        #endregion

        #region Copy Paste
        private void MakeClipboardCopy()
        {
            if (this.TextBoxItem.PasswordChar != '\0')
            {
                return;
            }

            Clipboard.SetDataObject(this.TextBoxItem.SelectedText);
        }

        private void MakeClipboardPaste()
        {
            string clipboardText = GetClipboardText();
            string textAfterPaste = this.TextBoxItem.Text;         
            if (this.TextBoxItem.SelectionLength > 0)
            {
                textAfterPaste = textAfterPaste.Remove(this.TextBoxItem.SelectionStart, this.TextBoxItem.SelectionLength);
            }

            if (textAfterPaste.Length == 0)
            {
                textAfterPaste = clipboardText;
                this.provider.Validate(clipboardText);
                return;
            }

            if (provider is RegexMaskTextBoxProvider)
            {
                provider.Validate(textAfterPaste);
            }
            else
            {
                foreach (char c in clipboardText)
                {
                    this.provider.KeyPress(this, new KeyPressEventArgs(c));
                }
            }
        }

        private void MakeClipboardCut()
        {
            this.MakeClipboardCopy();
            this.provider.Delete();
        }

        /// <summary>
        /// Gets the text which is in the clipboard
        /// </summary>
        /// <returns></returns>
        public static string GetClipboardText()
        {
            IDataObject iData;
            try
            { 
                iData = Clipboard.GetDataObject();
            }
            catch
            {
                return string.Empty;
            }
            if (iData.GetDataPresent(DataFormats.UnicodeText))
            {
                return (string)iData.GetData(DataFormats.UnicodeText);
            }
            else if (iData.GetDataPresent(DataFormats.Text))
            {
                return (string)iData.GetData(DataFormats.Text);
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public IMaskProvider Provider
        {
        	get
            {
                return this.provider;
            }
            set
            {
                this.provider = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool IsKeyBoard
        {
            get
            {
                return isKeyBoard;
            }
            internal set 
            {
                isKeyBoard = value;
            }
        }


        [DefaultValue(true),
        Description("MaskedTextBox AllowPrompt As Input"),
        Category("Behavior")]
        public bool AllowPromptAsInput
        {
            get
            {
                return this.allowPromptAsInput;
            }
            set
            {
                if (value != this.allowPromptAsInput)
                {
                    this.CreateMaskProvider();
                }
            }
        }

        //=====================================================================
        // Properties
        
        /// <summary>
        /// This returns a clone of the masked text provider currently being
        /// used by the masked label control.
        /// </summary>
        [Browsable(false)]
        public MaskedTextProvider MaskedTextProvider
        {
            get
            {
                return (MaskedTextProvider)provider.Clone();
            }
        }

        /// <summary>
        /// This returns the result hint for the last assignment to the
        /// <see cref="Text" /> property.
        /// </summary>
        /// <remarks>If the assigned text could not be properly formatted,
        /// this will contain a hint as to why not.  Positive values
        /// indicate success.  Negative values indicate failure.</remarks>
        [Browsable(false)]
        public MaskedTextResultHint ResultHint
        {
            get
            {
                return hint;
            }
        }

        /// <summary>
        /// This returns the result hint position for the last assignment to
        /// the <see cref="Text" /> property.
        /// </summary>
        /// <remarks>If the assigned text could not be properly formatted,
        /// this will contain the position of the first failure.</remarks>
        [Browsable(false)]
        public int HintPosition
        {
            get
            {
                return hintPos;
            }
        }

        /// <summary>
        /// This read-only property returns the unmasked copy of the text
        /// </summary>
        [Browsable(false)]
        public string UnmaskedText
        {
            get
            {
                return unmaskedText;
            }
        }

        /// <summary>
        /// This is used to set or get the label text.
        /// </summary>
        /// <remarks>When set, the text is formatted according to the current
        /// masked text provider settings.  If the mask is empty, the text is
        /// set as-is.  When retrieved, the text is returned in its formatted
        /// state.  Use <see cref="UnmaskedText" /> to get the text without
        /// the mask applied.</remarks>
        [Description("The text to display in the control")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                unmaskedText = value;

                // If there is no mask or no text, pass it through unchanged
                this.provider.Validate(value);
            }
        }

        public virtual void Validate(string value)
        {
            this.provider.Validate(value);        	
        }

        /// <summary>
        /// This is used to set or get the culture information associated with
        /// the masked label.
        /// </summary>
        /// <exception cref="ArgumentNullException">This is thrown if the
        /// culture value is null</exception>
        [Category("Appearance"),
        Description("Gets or sets the culture information associated with the masked label")]
        public CultureInfo Culture
        {
            get
            {
                return provider.Culture;
            }
            set
            {
                if (!object.ReferenceEquals(value, this.culture))
                {
                    // Recreate the provider with the new culture                  
                    //this.includePrompt = provider.IncludePrompt;
                    //newProvider.PromptChar = provider.PromptChar;
                    this.culture = value;
                    this.CreateMaskProvider();
                    this.Text = unmaskedText;
                }
            }
        }

        /// <summary>
        /// This is used to set or get the mask for the label text
        /// </summary>
        [Category("Appearance"), DefaultValue(""),
        Description("Gets or sets the mask to use for the label text")]
        public string Mask
        {
            get
            {
                if (cachedMask != string.Empty)
                {
                    return this.mask;
                }

                return string.Empty;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }

                if (this.mask != value)
                {
                    if (value == "h" || value == "H")
                    {
                        value = " " + value;
                    }

                    if (value == "yyy")
                    {
                        value = "yyyy";
                    }

                    // Recreate the provider with the new mask
                    this.cachedMask = value;
                    this.mask = value;
                    this.CreateMaskProvider();                                       
                }
            }
        }

        /// <summary>
        /// This is used to set or get the prompt character for display
        /// in the label text.
        /// </summary>
        /// <value>The default is an underscore (_).</value>
        [Category("Appearance"), DefaultValue('_'),
        Description("Gets or sets the prompt character for display in the label text")]
        public char PromptChar
        {
            get
            {
                return this.promptChar;
            }
            set
            {
                this.promptChar = value;
                this.CreateMaskProvider();
            }
        }

        /// <summary>
        /// This is used to set or get whether or not prompt characters are
        /// also displayed in the label text.
        /// </summary>
        /// <value>By default, prompt characters are not shown.</value>
        [Category("Appearance"), DefaultValue(false),
        Description("Gets or sets whether or not prompt characters are displayed in the label text")]
        public bool IncludePrompt
        {
            get
            {
                return provider.IncludePrompt;
            }
            set
            {
                provider.IncludePrompt = value;
                this.Text = unmaskedText;
            }
        }

      

        //=====================================================================
        
        /// <summary>
        /// Gets or sets the mask type.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the mask type."),
        DefaultValue(MaskType.None),
        Localizable(true),
        RefreshProperties(RefreshProperties.All)]
        public virtual MaskType MaskType
        {
            get
            {
                return this.maskType;
            }
            set
            {
                if (value != this.maskType)
                {
                    this.maskType = value;                   
                    this.Value = "";                    
                    this.CreateMaskProvider();
                    OnNotifyPropertyChanged("MaskType");                    
                }
            }
        }

        /// <summary>
        /// Gets or sets the edited value
        /// </summary>
        [Category("Data")]
        [Description("Gets or sets the edited value")]
        [DefaultValue(null)]
        public object Value
        {
            get
            {
                if (this.isNullValue)
                {
                    return null;
                }

                return this.provider.Value;
            }
            set
            {
                if (value == null)
                {
                    CancelEventArgs eventArgs = new CancelEventArgs();
                    this.CallValueChanging(eventArgs);
                    if (eventArgs.Cancel)
                    {
                        return;
                    }
                    this.isNullValue = true;
                    this.TextBoxItem.Text = string.Empty;
                    this.CallValueChanged(EventArgs.Empty);
                }
                else
                {
                    this.isNullValue = false;
                    this.provider.Value = value;
                    this.OnNotifyPropertyChanged("MaskEditValue");
                }
            }
        }

        #endregion

        #region MaskProvider

        protected virtual void CreateMaskProvider()
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;	
            }

            if (this.mask == "s")
            {
                this.mask = "";
            }

            switch (this.maskType)
            {              
                case MaskType.DateTime:
                    if (string.IsNullOrEmpty(this.Mask))
                    {
                        this.mask = "g";
                    }
                    this.provider = new MaskDateTimeProvider(this.mask, culture, this);
                    base.Text = provider.ToString(true, true);
                    break;
                case MaskType.Numeric:
                    if (string.IsNullOrEmpty(this.Mask))
                    {
                        this.mask = "n0";
                    }
                    this.provider = new NumericMaskTextBoxProvider(this.mask, culture, this);
                    base.Text = provider.ToString(true, true);
                    break;

                case MaskType.Standard:
                    if (string.IsNullOrEmpty(this.Mask))
                    {
                        this.mask = "############";
                    }
                    this.provider = new StandartMaskTextBoxProvider(this.mask, culture, this, allowPromptAsInput, promptChar, passwordChar, restrictToAscii);            
                    this.Text = provider.ToString(true, true);
                    break;
                case MaskType.IP:
                    this.provider = new IPMaskTextBoxProvider(culture,this,allowPromptAsInput, ' ',passwordChar,restrictToAscii); //RegexMaskTextBoxProvider(this.Mask, CultureInfo.CurrentCulture, this);
                    base.Text = provider.ToString(true, true);
                    break;
                case MaskType.Regex:
                    if (string.IsNullOrEmpty(this.Mask))
                    {
                        this.mask = "[A-z]";
                    }

                    this.provider = new RegexMaskTextBoxProvider(this.mask, culture, this);
                    base.Text = provider.ToString(true, true);
                    break;
                case MaskType.EMail:
                    this.mask = string.Empty;
                    this.provider = new EMailMaskTextBoxProvider(culture, this);
                    base.Text = provider.ToString(true, true);
                    break;
                case MaskType.None:
                    this.provider = new TextBoxProvider(this);
                    base.Text = "";
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region Private designer methods
        //=====================================================================
        // Private designer methods

        // <summary>
        // This is used to determine whether or not to serialize the culture
        // </summary>
        // <returns></returns>
        private bool ShouldSerializeCulture()
        {
            return !CultureInfo.CurrentCulture.Equals(this.Culture);
        }

        #endregion

        #region Static methods

        public static NumericCharacterTextBoxProvider.RadNumericMaskFormatType GetFormat(string formatString, CultureInfo culture)
        {
            // the default format would be decimal if the formatString is incorrect
            NumericCharacterTextBoxProvider.RadNumericMaskFormatType numericType = NumericCharacterTextBoxProvider.RadNumericMaskFormatType.None;

            if (Regex.IsMatch(formatString, "^[cCdDgGfFnNpP][0-9]{0,2}$"))
            {
                switch (formatString[0])
                {
                    case 'c':
                    case 'C':
                        numericType = NumericCharacterTextBoxProvider.RadNumericMaskFormatType.Currency;
                        break;
                    case 'd':
                    case 'D':
                        numericType = NumericCharacterTextBoxProvider.RadNumericMaskFormatType.Standard;
                        break;
                    case 'g':
                    case 'G':
                    case 'f':
                    case 'F':
                        numericType = NumericCharacterTextBoxProvider.RadNumericMaskFormatType.FixedPoint;
                        break;
                    case 'n':
                    case 'N':
                        numericType = NumericCharacterTextBoxProvider.RadNumericMaskFormatType.Decimal;
                        break;
                    case 'p':
                    case 'P':
                        numericType = NumericCharacterTextBoxProvider.RadNumericMaskFormatType.Percent;
                        break;
                    default:
                        numericType = NumericCharacterTextBoxProvider.RadNumericMaskFormatType.Decimal;
                        break;
                }
            }

            return numericType;
        }

        /// <summary>
        /// Format the specified text using the specified mask
        /// </summary>
        /// <param name="mask">The mask to use</param>
        /// <param name="text">The text to format</param>
        /// <returns>The formatted text string</returns>
        /// <overloads>There are four overloads for this method.</overloads>
        public static string Format(string mask, string text)
        {
            MaskedTextResultHint hint;
            int pos;

            return RadMaskedEditBoxElement.Format(mask, text, '\x0', null,
                                                out hint, out pos);
        }

        // Static methods

        /// <summary>
        /// Format the specified text using the specified mask and prompt
        /// character.
        /// </summary>
        /// <param name="mask">The mask to use.</param>
        /// <param name="text">The text to format.</param>
        /// <param name="promptChar">The prompt character to use for missing
        /// characters.  If a null character ('\x0') is specified, prompt
        /// characters are omitted.</param>
        /// <returns>The formatted text string.</returns>
        public static string Format(string mask, string text, char promptChar)
        {
            MaskedTextResultHint hint;
            int pos;

            return RadMaskedEditBoxElement.Format(mask, text, promptChar, null,
                                                out hint, out pos);
        }

        /// <summary>
        /// Format the specified text using the specified mask, prompt
        /// character, and culture information.
        /// </summary>
        /// <param name="mask">The mask to use.</param>
        /// <param name="text">The text to format.</param>
        /// <param name="promptChar">The prompt character to use for missing
        /// characters.  If a null character ('\x0') is specified, prompt
        /// characters are omitted.</param>
        /// <param name="culture">The culture information to use.  If null,
        /// the current culture is used.</param>
        /// <returns>The formatted text string.</returns>
        public static string Format(string mask, string text, char promptChar,
                                    CultureInfo culture)
        {
            MaskedTextResultHint hint;
            int pos;

            return RadMaskedEditBoxElement.Format(mask, text, promptChar, culture,
                                                out hint, out pos);
        }

        /// <summary>
        /// Format the specified text using the specified mask, prompt
        /// character, and culture information and return the result
        /// values.
        /// </summary>
        /// <param name="mask">The mask to use.</param>
        /// <param name="text">The text to format.</param>
        /// <param name="promptChar">The prompt character to use for missing
        /// characters.  If a null character ('\x0') is specified, prompt
        /// characters are omitted.</param>
        /// <param name="culture">The culture information to use.  If null,
        /// the current culture is used.</param>
        /// <param name="hint">The result of formatting the text.</param>
        /// <param name="hintPosition">The position related to the result
        /// hint.</param>
        /// <returns>The formatted text string.</returns>
        public static string Format(string mask, string text, char promptChar,
                                    CultureInfo culture, out MaskedTextResultHint hint,
                                    out int hintPosition)
        {
            if (text == null)
                text = String.Empty;

            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            MaskedTextProvider provider = new MaskedTextProvider(mask, culture);

            // Set the prompt character options
            if (promptChar != '\x0')
            {
                provider.PromptChar = promptChar;
                provider.IncludePrompt = true;
            }

            // Format and return the string
            provider.Set(text, out hintPosition, out hint);

            // Positive hint results are successful
            if (hint > 0)
                return provider.ToString();

            // Return the text as-is if it didn't fit the mask
            return text;
        }
        #endregion
    }
}
