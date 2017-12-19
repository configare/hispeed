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
    public class StandartMaskTextBoxProvider : IMaskProvider
    {
        protected IMaskCharacterProvider provider;
        RadTextBoxItem textBoxItem;
        int hintPos;
        MaskedTextResultHint hint;        
        CultureInfo culture;
        string mask = "<>";
        private bool includePrompt;
        private char promptChar;
        RadMaskedEditBoxElement owner;
        private bool allowPromptAsInput;        
        private char passwordChar;
        private bool restrictToAscii;
        
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

        public StandartMaskTextBoxProvider(string mask, CultureInfo culture, RadMaskedEditBoxElement owner, bool allowPromptAsInput, char promptChar, char passwordChar, bool restrictToAscii)
        {
            this.allowPromptAsInput = allowPromptAsInput;
            this.promptChar = promptChar;
            this.passwordChar = passwordChar;
            this.restrictToAscii = restrictToAscii;
            this.mask = mask;
            this.culture = culture;
            this.owner = owner;
            this.provider = new StandartCharacterMaskEditBoxProvider(mask, culture, owner, allowPromptAsInput, promptChar, passwordChar, restrictToAscii);
            this.promptChar = this.provider.PromptChar;
            this.textBoxItem = owner.TextBoxItem;
        }

        public string ToString(bool includePromt, bool includeLiterals)
        {
            return this.provider.ToString(includePromt, includeLiterals);
        }

        public IMaskProvider Clone()
        {
            return new StandartMaskTextBoxProvider(mask, culture, owner, allowPromptAsInput, promptChar, passwordChar, restrictToAscii);
        }

        public bool Set(string input, out int testPosition, out MaskedTextResultHint resultHint)
        {
            return this.provider.Set(input, out testPosition, out resultHint);
        }

        //public bool RemoveAt(int startPosition, int endPosition)
        //{
        //    return this.provider.RemoveAt(startPosition, endPosition);
        //}

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

        public virtual void KeyPress(object sender, KeyPressEventArgs e)
        {
            this.provider.KeyPress(sender, e);
        }

        public virtual void KeyDown(object sender, KeyEventArgs e)
        {
            this.provider.KeyDown(sender, e);
        }   

        public virtual bool Validate(string value)
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

        public virtual object Value
        {
            get
            {
                return provider.ToString(false,false);
            }
            set
            {
                this.Validate(value.ToString());
            }
        }   
    
        public bool Delete()
        {
            return this.provider.Delete();
        }
    }
}
