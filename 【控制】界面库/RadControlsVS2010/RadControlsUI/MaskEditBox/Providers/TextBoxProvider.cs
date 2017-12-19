using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class TextBoxProvider : IMaskProvider
    {
        protected RadMaskedEditBoxElement owner;

        public TextBoxProvider(RadMaskedEditBoxElement owner)
        {
            this.owner = owner;
        }

        public void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
        }

        public void KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
        }

        public bool Validate(string value)
        {
            return true;
        }

        public bool Click()
        {
            return true;
        }

        public RadTextBoxItem TextBoxItem
        {
            get
            {
                return this.owner.TextBoxItem;
            }
        }

        public string ToString(bool includePromt, bool includeLiterals)
        {
            return this.owner.Text;
        }

        public IMaskProvider Clone()
        {
            return new TextBoxProvider(this.owner);
        }

        public System.Globalization.CultureInfo Culture
        {
            get
            {
                return null;
            }
        }

        public string Mask
        {
            get
            {
               return "";
            }
        }

        public bool IncludePrompt
        {
            get
            {
                return false;
            }
            set
            {                
            }
        }

        public char PromptChar
        {
            get
            {
                return ' ';
            }
            set
            {
                
            }
        }

        public object Value
        {
            get
            {
                return this.owner.Text;
            }
            set
            {
                this.owner.Text = Value.ToString();                
            }
        }

        public bool Delete()
        {
            return true;
        }
    }
}