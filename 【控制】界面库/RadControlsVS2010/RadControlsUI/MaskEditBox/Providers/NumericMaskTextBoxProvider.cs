using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Text.RegularExpressions;

namespace Telerik.WinControls.UI
{
    public class NumericMaskTextBoxProvider : IMaskProvider
    {
        protected IMaskCharacterProvider provider;       
        RadTextBoxItem textBoxItem;
        int hintPos;
        MaskedTextResultHint hint;
        NumericCharacterTextBoxProvider.RadNumericMaskFormatType numericType;
        CultureInfo culture;
        string mask = "<>";
        private bool includePrompt;
        private char promptChar;
        RadMaskedEditBoxElement owner;
       
        public bool Click()
        { 
            return false;
        }

        public RadTextBoxItem TextBoxItem
        {
            get
            {
                return this.textBoxItem;	
            }
        }

        public NumericMaskTextBoxProvider(string mask, CultureInfo culture, RadMaskedEditBoxElement owner)
        {
            this.owner = owner;
            this.numericType = GetFormat(mask, culture);
            this.mask = mask;
            this.culture = culture;           
            this.provider = new NumericCharacterTextBoxProvider(mask, culture, this.numericType, owner);           
            this.promptChar = this.provider.PromptChar;
            this.textBoxItem = owner.TextBoxItem;            
        }

        public string ToString(bool includePromt, bool includeLiterals)
        {
            return this.provider.ToString(includePromt, includeLiterals);
        }

        public IMaskProvider Clone()
        {
            return new NumericMaskTextBoxProvider(this.Mask, this.Culture, this.owner);
        }

        public bool Set(string input, out int testPosition, out MaskedTextResultHint resultHint)
        {
            return this.provider.Set(input, out testPosition, out resultHint);
        }

        
        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }
        }

        public string Mask
        {
            get
            {
                return this.mask;
            }
        }

        public bool IncludePrompt
        {
            get
            {
                return this.includePrompt;
            }
            set
            {
                this.includePrompt = value;
            }
        }

        public char PromptChar
        {
            get
            {
                return this.promptChar;
            }
            set
            {
                this.promptChar = value;
            }
        }

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            this.provider.KeyPress(sender, e);
        }

        public void KeyDown(object sender, KeyEventArgs e)
        { 
            this.provider.KeyDown(sender, e); 
        }

        private void HandleDeleteByBack()
        {
            int selectionStart = TextBoxItem.SelectionStart;
            int indexToDelete = TextBoxItem.SelectionLength == 0 ? selectionStart - 1 : selectionStart;
            if (indexToDelete < 0 && TextBoxItem.SelectionLength == 0)
            {
                return;
            }

            this.provider.RemoveAt(indexToDelete, indexToDelete + TextBoxItem.SelectionLength);
            this.TextBoxItem.Text = provider.ToString(true, true);
            this.TextBoxItem.SelectionLength = 0;
            this.TextBoxItem.SelectionStart = indexToDelete;
            return;
        }
        
        public bool Validate(string value)
        {
            CancelEventArgs eventArgs = new CancelEventArgs();
            this.owner.CallValueChanging(eventArgs);
            if (eventArgs.Cancel)
            {
                return false;
            }

            if (Mask != "<>" && value != null)
            {
                provider.Set(value, out hintPos, out hint);

                // Positive hint results are successful
                if (hint > 0)
                {
                    textBoxItem.Text = provider.ToString(true, true);
                    this.owner.CallValueChanged(EventArgs.Empty);
                    return true;
                }
            }

            //base.Text = unmaskedText;
            return false;
        }

        public object Value
        {
            get
            {
                return provider.ToString(true, true);
            }
            set
            {
                this.Validate(value.ToString());
            }
        }

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

        public bool Delete()
        {
            this.provider.Delete();
            return true;
        }

    }
}
