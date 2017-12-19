using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RegexMaskTextBoxProvider : IMaskProvider
    {
        string mask;
        CultureInfo culture;
        RadMaskedEditBoxElement owner;
        RadTextBoxItem textBoxItem;
        Regex regex;
        ErrorProvider errorProvider;
        string errorMessage = "Input is not valid";

        public RegexMaskTextBoxProvider(string mask, CultureInfo culture, RadMaskedEditBoxElement owner)
        {
            this.mask = mask;
            this.culture = culture;
            this.owner = owner;
            this.textBoxItem = owner.TextBoxItem;
            this.textBoxItem.HostedControl.Leave += new EventHandler(HostedControl_Leave);
            this.errorProvider = new ErrorProvider();
            this.errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
            }
        }

        void HostedControl_Leave(object sender, EventArgs e)
        {           
            this.regex = new Regex(mask);
            if (!this.regex.IsMatch(this.textBoxItem.Text) && !string.IsNullOrEmpty(this.textBoxItem.Text))
            {
                errorProvider.SetError(this.owner.ElementTree.Control, errorMessage);
            }
        }

        public void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            errorProvider.SetError(this.owner.ElementTree.Control, "");
        }

        public void KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            errorProvider.SetError(this.owner.ElementTree.Control, "");            
        }

        public bool Validate(string value)
        {
            CancelEventArgs eventArgs = new CancelEventArgs();
            this.owner.CallValueChanging(eventArgs);
            if (eventArgs.Cancel)
            {
                return false;
            }

            this.textBoxItem.Text = value;

            this.regex = new Regex(this.mask);
            if (this.regex.IsMatch(this.textBoxItem.Text))
            {
                errorProvider.SetError(this.owner.ElementTree.Control, "");                
                this.owner.CallValueChanged(eventArgs);                
                return true;
            }
            errorProvider.SetError(this.owner.ElementTree.Control, errorMessage);
            return false;
        }

        public bool Click()
        {
            return true;
        }

        public RadTextBoxItem TextBoxItem
        {
            get { return textBoxItem; }
        }

        public string ToString(bool includePromt, bool includeLiterals)
        {
            return textBoxItem.Text;
        }

        public IMaskProvider Clone()
        {
            return new RegexMaskTextBoxProvider(mask, culture, owner);
        }

        public System.Globalization.CultureInfo Culture
        {
            get { return culture; }
        }

        public string Mask
        {
            get { return mask; }
        }

        public bool IncludePrompt
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public char PromptChar
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public object Value
        {
            get
            {
                return this.textBoxItem.Text;
            }
            set
            {
                this.textBoxItem.Text = value.ToString();
            }
        }

        public bool Delete()
        {
            return true;
        }
    }
}
