using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class NumericCharacterTextBoxProvider : IMaskCharacterProvider
    {
        #region Enums
        public enum RadNumericMaskFormatType
        {
            None,
            Currency,
            Standard,
            Percent,
            FixedPoint,
            Decimal,
        }

        #endregion

        #region Fields

        RadNumericMaskFormatType numericType;
        CultureInfo culture;
        string mask;
        string value = "0";
        char promptChar = '0';
        RadTextBoxItem textBoxItem;
        RadMaskedEditBoxElement owner;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the culture that determines the value of the localizable separators and
        /// placeholders in the input mask.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Globalization.CultureInfo"/> containing the culture information
        /// associated with the input mask.
        /// </returns>
        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }
        }

        /// <summary>
        /// Gets the input mask.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing the full mask.
        /// </returns>
        public string Mask
        {
            get
            {
                return this.mask;
            }
        }

        public char PromptChar
        {
            get
            {
                return promptChar;
            }
            set
            {
                promptChar = value;
            }
        }

        #endregion

        #region CStor
        /// <summary>
        /// Initializes a new instance of the NumericTextBoxProvider>
        /// class using the specified mask and culture.
        /// </summary>
        /// <param name="mask">
        /// A <see cref="T:System.String"/> that represents the input mask. 
        /// </param>
        /// <param name="culture">
        /// A <see cref="T:System.Globalization.CultureInfo"/> that is used to set region-sensitive
        /// separator characters.
        /// </param>
        public NumericCharacterTextBoxProvider(string mask, System.Globalization.CultureInfo culture, RadNumericMaskFormatType numericType, RadMaskedEditBoxElement owner) 
        {
            this.owner = owner;
            this.mask = mask;
            this.culture = culture;
            // this.value = this.ToString(true, true);
            this.numericType = numericType;
            this.promptChar = '0';
            this.textBoxItem = owner.TextBoxItem;
            // this.originalValues = this.ToString(true, true);
        }

        #endregion

        #region API
        public virtual string ToString(bool includePromt, bool includeLiterals)
        {
            return this.ParseText(this.value);
        }

        public bool Set(string input, out int testPosition, out MaskedTextResultHint resultHint)
        {
            resultHint = MaskedTextResultHint.Success;
            testPosition = 0;
            string returnString = this.ParseTextCore(input, out testPosition, out resultHint);
            if (resultHint == MaskedTextResultHint.Success)
            {
                this.value = returnString;
                return true;
            }

            return false;
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                this.HandleDeleteByDel();
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Up)
            {
                this.HandleUp();
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                this.HandleDown();
                e.Handled = true;
            }
        }

        private void HandleDown()
        {
            int selectionStart = this.textBoxItem.SelectionStart;
            if (this.textBoxItem.SelectionLength > 0)
            {
                return;
            }

            string text = this.textBoxItem.Text;
            this.UpdatePreviosChar(false, ref text, this.textBoxItem.SelectionStart - 1);
            this.textBoxItem.Text = text;
            this.textBoxItem.SelectionStart = selectionStart;
        }

        private void HandleUp()
        {
            int selectionStart = this.textBoxItem.SelectionStart;
            if (this.textBoxItem.SelectionLength > 0)
            {
                return;
            }

            string text = this.textBoxItem.Text;
            this.UpdatePreviosChar(true, ref text, this.textBoxItem.SelectionStart - 1);
            this.textBoxItem.Text = text;
            this.textBoxItem.SelectionStart = selectionStart;
        }

        private bool UpdatePreviosChar(bool up, ref string text, int pos )
        {
            while (pos >= 0)
            {
                char charToUpdate = text[pos];
                if (!char.IsDigit(charToUpdate))
                {
                    pos--;
                    continue;
                }

                int number = int.Parse(charToUpdate.ToString());
                if (up)
                {
                    number++;
                    if (number > 9)
                    {
                        number = 0;                        
                        this.UpdatePreviosChar(up, ref text, pos - 1);                        
                    }
                }
                else
                {
                	number--;
                    if (number < 0)
                    {
                        number = 9;
                        this.UpdatePreviosChar(up, ref text, pos - 1);                        
                    }
                }

                text = ReplaceAt(ref text, pos, number.ToString());
                return true;
            }

            return false;
        }

        string ReplaceAt(ref string text, int pos, string newChar)
        {
            string result = text.Remove(pos,1).Insert(pos, newChar);
            return result;
        }

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                HandleDeleteByBack();
                e.Handled = true;
                return;
            }
            
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsPunctuation(e.KeyChar) )
            {
                e.Handled = true;
                return;
            }

            if (this.textBoxItem.SelectionLength > 0)
            {
                HandleDeleteByDel();
            }
            //if (this.numericType == RadNumericMaskFormatType.Decimal)
            //{
            //    e.Handled = true;
            //    return;
            //}
            int selectionStart = this.textBoxItem.SelectionStart;
            int textLength = this.textBoxItem.TextLength;

            do
            {
                string newText = this.textBoxItem.Text;
                int lenBeforeFormat = newText.Length;
                char charToBeRemoved = '\0';
                while (selectionStart < newText.Length)
                {
                    charToBeRemoved = newText[selectionStart];
                    if (charToBeRemoved.ToString() == this.culture.NumberFormat.NumberDecimalSeparator)
                    {
                        break;
                    } 

                    if (char.IsDigit(charToBeRemoved))  
                    {
                        break;
                    }

                    ++selectionStart;
                }

                bool lastPos = selectionStart == newText.Length;
                int pointPos = newText.LastIndexOf(this.culture.NumberFormat.NumberDecimalSeparator);
                //if (charToBeRemoved != this.PromptChar && char.IsPunctuation(charToBeRemoved))
                //if (pointPos !=-1 && selectionStart <= pointPos && this.originalValues[selectionStart] != this.PromptChar)
                if (char.IsPunctuation(charToBeRemoved) ||
                    (lastPos && this.numericType != RadNumericMaskFormatType.Standard && this.numericType!=RadNumericMaskFormatType.Decimal && this.numericType != RadNumericMaskFormatType.FixedPoint) || 
                    ((selectionStart < pointPos || (pointPos == -1 && this.numericType != RadNumericMaskFormatType.Standard)) && charToBeRemoved != this.PromptChar))
                {
                    newText = newText.Insert(selectionStart, e.KeyChar.ToString());
                }
                else
                {
                    if (selectionStart < newText.Length)
                    {
                        newText = newText.Remove(selectionStart, 1).Insert(selectionStart, e.KeyChar.ToString());
                        ++selectionStart;
                    }
                }

                bool validationResult = this.Validate(newText);
                int lenAfterFormat = this.textBoxItem.Text.Length;
                if (validationResult)
                {                
                    selectionStart += lenAfterFormat - lenBeforeFormat;
                    if (selectionStart <= textLength)
                    {
                        this.textBoxItem.SelectionStart = selectionStart;
                    }
                    else
                    {
                        this.textBoxItem.SelectionStart = this.textBoxItem.Text.Length;
                    }                   

                    break;
                }

                ++selectionStart;
            }
            while (selectionStart < textLength);

            e.Handled = true;
        }

        public bool Validate(string value)
        {

            CancelEventArgs eventArgs = new CancelEventArgs();
            this.owner.CallValueChanging(eventArgs);
            if (eventArgs.Cancel)
            {
                return false;
            }

            int hintPos = 0;
            MaskedTextResultHint hint = MaskedTextResultHint.Success; 
            if (mask != "<>" && value != null)
            {
                Set(value, out hintPos, out hint);
                // Positive hint results are successful
                if (hint > 0)
                {
                    textBoxItem.Text = ToString(true, true);
                    this.owner.CallValueChanged(EventArgs.Empty);
                    return true;
                }
            }

            //base.Text = unmaskedText;
            return false;
        }

        public bool RemoveAt(int startPosition, int endPosition)
        {
            string text = this.textBoxItem.Text;// .Remove(startPosition, endPosition - startPosition);
            int decimalPos = text.IndexOf(this.culture.NumberFormat.NumberDecimalSeparator);
            string resultString;
            StringBuilder sb;

            if (this.numericType == RadNumericMaskFormatType.Decimal && decimalPos == -1)
            {
                if (startPosition >= text.Length)
                {
                    return false;
                }

                text = text.Remove(startPosition, endPosition - startPosition);
                this.Validate(text);
                return true;
            }

            if (decimalPos == -1)
            {
                sb = new StringBuilder(text.Length);
                for (int i = 0; i < text.Length; ++i)
                {
                    if (i >= startPosition && i < endPosition)
                    {
                        sb.Append(this.promptChar);
                    }
                    else
                    {
                        sb.Append(text[i]);
                    }
                }

                resultString = sb.ToString();
                this.Validate(resultString);
                return true;
            }

            if (startPosition < decimalPos)
            {   
                sb = new StringBuilder(text.Length);
                for (int i = 0; i <= decimalPos; ++i)
                {
                    sb.Append(text[i]);
                }

                for (int i = decimalPos + 1; i < text.Length; ++i)
                {
                    if (i >= startPosition && i < endPosition)
                    {
                        sb.Append(this.promptChar);
                    }
                    else
                    {
                        sb.Append(text[i]);
                    }
                }

                text = sb.ToString().Remove(startPosition, Math.Min(decimalPos, endPosition) - startPosition);                
                this.Validate(text);
                return true;
            }

            sb = new StringBuilder(text.Length);
            for (int i = decimalPos + 1; i < text.Length; ++i)
            {
                if (i >= startPosition && i < endPosition)
                {
                    sb.Append(this.promptChar);
                }
                else
                {
                    sb.Append(text[i]);
                }
            }

            resultString = text.Substring(0, decimalPos + 1) + sb.ToString();
            this.Validate(resultString);
            return true;//this.provider.RemoveAt(startPosition, endPosition);
        }

        #endregion

        #region Helpers

        private void HandleDeleteByBack()
        {
            int selectionStart = textBoxItem.SelectionStart;
            int indexToDelete = textBoxItem.SelectionLength == 0 ? selectionStart - 1 : selectionStart;
            if (indexToDelete < 0 && textBoxItem.SelectionLength == 0)
            {
                return;
            }

            this.RemoveAt(indexToDelete, indexToDelete + Math.Max(textBoxItem.SelectionLength, 1));
            textBoxItem.Text = this.ToString(true, true);
            this.textBoxItem.SelectionLength = 0;
            selectionStart = indexToDelete;// -(lenBeforeDelete - lenAfterDelete + 1);
            if (selectionStart <= 0)
            {
                selectionStart = 0;
            } 
            else if (selectionStart < textBoxItem.Text.Length && !char.IsDigit(textBoxItem.Text[selectionStart - 1]))
            {
                --selectionStart;
            }

            this.textBoxItem.SelectionStart = selectionStart;
            return;
        }

        private void HandleDeleteByDel()
        {
            int selectionStart = this.textBoxItem.SelectionStart;
            int indexToDelete = selectionStart;

            this.RemoveAt(indexToDelete, indexToDelete + Math.Max(this.textBoxItem.SelectionLength, 1));
            this.textBoxItem.Text = this.ToString(true, true);
            this.textBoxItem.SelectionLength = 0;
            this.textBoxItem.SelectionStart = indexToDelete;
            return;
        }

        public bool Delete()
        {
            this.HandleDeleteByDel();
            return true;
        }
        
        protected virtual string ParseText(string value)
        {
            int testPosition;
            MaskedTextResultHint resultHint;
            string result = this.ParseTextCore(value, out testPosition, out resultHint);
            if (resultHint == MaskedTextResultHint.Success)
            {
                return result;
            }

            return value;
        }

        protected virtual string ParseTextCore(string value, out int testPosition, out MaskedTextResultHint resultHint)
        {
            testPosition = 0;
            resultHint = MaskedTextResultHint.Success;
            string text = "";
            if (string.IsNullOrEmpty(value))
            { 
                value = "0";
            }

            value = value.Replace(this.culture.NumberFormat.CurrencyGroupSeparator, "");
            try
            {
                switch (this.numericType)
                {
                    case RadNumericMaskFormatType.None:
                        break;
                    case RadNumericMaskFormatType.Currency:
                        value = value.Replace(this.culture.NumberFormat.CurrencySymbol, "");
                        text = String.Format(this.Culture, "{0:" + this.Mask + "}", Decimal.Parse(value, this.Culture));
                        break;
                    case RadNumericMaskFormatType.Standard:                      
                        text = String.Format(this.Culture, "{0:" + this.Mask + "}", (Int64)Decimal.Parse(value, this.Culture));
                        break;
                    case RadNumericMaskFormatType.Percent:
                        double floatValue = double.Parse(value.Replace("%", ""), this.Culture);
                        floatValue /= 100;
                        text = String.Format(this.Culture, "{0:" + this.Mask + "}", floatValue);
                        break;
                    case RadNumericMaskFormatType.FixedPoint:
                        text = String.Format(this.Culture, "{0:" + this.Mask + "}", Decimal.Parse(value, this.Culture));
                        break;
                    case RadNumericMaskFormatType.Decimal:
                        text = String.Format(this.Culture, "{0:" + this.Mask + "}", Decimal.Parse(value, this.Culture));
                        break;
                    default:
                        break;
                }
            }
            catch 
            {
                resultHint = MaskedTextResultHint.UnavailableEditPosition;
            }

            return text;
        }
        #endregion
    }
}
