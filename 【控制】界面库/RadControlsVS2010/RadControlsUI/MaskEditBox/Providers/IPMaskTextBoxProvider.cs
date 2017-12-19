using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class IPMaskTextBoxProvider : StandartMaskTextBoxProvider
    {
        public IPMaskTextBoxProvider(CultureInfo culture, RadMaskedEditBoxElement owner, bool allowPromptAsInput, char promptChar, char passwordChar, bool restrictToAscii)
            : base("###.###.###.###", culture, owner, allowPromptAsInput, promptChar, passwordChar, restrictToAscii)
        {
        }

      
        public override void KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            base.KeyPress(sender, e);
            int pointPos = -1;
            int selectionStart = this.TextBoxItem.SelectionStart;
            for (int i = selectionStart - 1; i >= 0; --i)
            {
                if (this.TextBoxItem.Text[i] == '.')
                {
                    pointPos = i;
                    break;
                }
            }
            if (e.KeyChar == 8)
            {
                return;
            }

            string stringToValidate = this.TextBoxItem.Text.Substring(pointPos + 1, Math.Min(selectionStart - pointPos + 1, 3));
            int result = 0;
            if (int.TryParse(stringToValidate, out result))
            {
                if (result > 255 || result < 0)
                {
                    string insertString = result < 0 ? "000" : "255";
                    MessageBox.Show("Value should be between 0 and 255");
                    result = 255;
                    string stringResult = this.TextBoxItem.Text.Substring(0, pointPos + 1) + insertString + this.TextBoxItem.Text.Substring(pointPos + 4);
                    this.Validate(stringResult);
                    this.TextBoxItem.SelectionStart = selectionStart;
                }
            }
        }

        

        public override string ToString()
        {
            return this.provider.ToString(false, true);
        }

        public override object Value
        {
            get
            {
                return this.ToString(false, true);
            }
            set
            {
                this.Validate(value.ToString());
            }
        }   

    }
}
