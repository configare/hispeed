using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class StandartCharacterMaskEditBoxProvider : IMaskCharacterProvider
    {
        protected MaskedTextProvider provider;
        protected RadTextBoxItem textBoxItem;
        RadMaskedEditBoxElement owner;

        private bool allowPromptAsInput;
        private char promptChar;
        private char passwordChar;
        private bool restrictToAscii;
        private string mask;
        CultureInfo culture;

        public StandartCharacterMaskEditBoxProvider(string mask, CultureInfo culture, RadMaskedEditBoxElement owner, bool allowPromptAsInput, char promptChar, char passwordChar, bool restrictToAscii)
        {
            this.owner = owner;
            this.allowPromptAsInput = allowPromptAsInput;
            this.promptChar = promptChar;
            this.passwordChar = passwordChar;
            this.restrictToAscii = restrictToAscii;
            this.mask = mask;
            this.culture = culture;
            this.provider = new MaskedTextProvider(mask, culture, allowPromptAsInput, promptChar, passwordChar, restrictToAscii);
            this.textBoxItem = owner.TextBoxItem;
        }

        public string ToString(bool includePromt, bool includeLiterals)
        {
            return this.provider.ToString(includePromt, includeLiterals);
        }

        public bool Set(string input, out int testPosition, out System.ComponentModel.MaskedTextResultHint resultHint)
        {
            return this.provider.Set(input, out testPosition, out resultHint);
        }

        public bool RemoveAt(int startPosition, int endPosition)
        {
            return this.provider.RemoveAt(startPosition, endPosition);
        }

        public char PromptChar
        {
            get
            {
                return provider.PromptChar;
            }
            set
            {
                provider.PromptChar = value;
                this.provider = new MaskedTextProvider(mask, culture, allowPromptAsInput, promptChar, passwordChar, restrictToAscii);
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                this.HandleDeleteByDel();
                e.Handled = true;
            }
        }

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            int selectionStart = this.textBoxItem.SelectionStart;
            int textLength = this.textBoxItem.TextLength;

            if (e.KeyChar == 8)
            {
                HandleDeleteByBack();
                e.Handled = true;
            }

            if (selectionStart == textLength)
            {
                e.Handled = true;
                return;
            }

            do
            {
                string newText = this.textBoxItem.Text;
                int lenBeforeFormat = newText.Length;
               
                newText = newText.Remove(selectionStart, 1).Insert(selectionStart, e.KeyChar.ToString());
               
                bool validationResult = this.Validate(newText);
                int lenAfterFormat = this.textBoxItem.Text.Length;
                if (validationResult)
                {
                    ++selectionStart;                   
                    selectionStart += lenAfterFormat - lenBeforeFormat;
                    if (selectionStart < textLength)
                    {
                        this.textBoxItem.SelectionStart = selectionStart;
                    }
                    else
                    {
                        this.textBoxItem.SelectionStart = textLength;
                    }

                    break;
                }

                ++selectionStart;
            }
            while (selectionStart < textLength);

            e.Handled = true;
        }


        string oldValue = string.Empty;

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
            if (provider.Mask != "<>" && value != null)
            {
                provider.Set(value, out hintPos, out hint);

                // Positive hint results are successful
                if (hint > 0)
                {
                    oldValue = provider.ToString(true,true);
                    if (textBoxItem.Text == oldValue)
                    {
                        return true;	
                    }

                    textBoxItem.Text = oldValue;
                    this.owner.CallValueChanged(EventArgs.Empty);
                    return true;
                }
            }

            //base.Text = unmaskedText;
            return false;
        }

        private void HandleDeleteByBack()
        {
            MaskedTextResultHint hint = MaskedTextResultHint.Success;
            int pos = 0;
            int selectionStart = textBoxItem.SelectionStart;
            int indexToDelete = textBoxItem.SelectionLength == 0 ? selectionStart - 1 : selectionStart;
            if (indexToDelete < 0 && textBoxItem.SelectionLength == 0)
            {
                return;
            }

            int lenToDelete = indexToDelete + textBoxItem.SelectionLength - 1;
            if (lenToDelete <= indexToDelete)
            {
                lenToDelete = indexToDelete + 1;
            }

            if (lenToDelete>=this.textBoxItem.Text.Length)
            {
                lenToDelete = this.textBoxItem.Text.Length - 1;
            }

            this.provider.RemoveAt(indexToDelete,  lenToDelete, out pos, out hint);
            textBoxItem.Text = provider.ToString(true, true);
            this.textBoxItem.SelectionLength = 0;
            this.textBoxItem.SelectionStart = indexToDelete;
            return;
        }

        private void HandleDeleteByDel()
        {            
            MaskedTextResultHint hint = MaskedTextResultHint.Success;
            int selectionStart = textBoxItem.SelectionStart;
            int selectionLen = textBoxItem.SelectionLength;
            int indexToDelete = selectionStart;
            int pos = indexToDelete;
            this.provider.RemoveAt(indexToDelete, indexToDelete + (selectionLen>0?selectionLen-1:0), out pos, out hint);
            textBoxItem.Text = provider.ToString(true, true);
            this.textBoxItem.SelectionLength = 0;
            this.textBoxItem.SelectionStart = indexToDelete;
            return;
        }

        public bool Delete()
        {
            this.HandleDeleteByDel();
            return true;
        }
    }
}
