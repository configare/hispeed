using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.FileProject
{
    public class IntegerTextBox:TextBox
    {
        protected int _selectionLen = 0;

        public int Value
        {
            get
            {
                int v = 0;
                if (int.TryParse(Text, out v))
                    return v;
                return 0;
            }
            set 
            {
                Text = value.ToString();
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            _selectionLen = 0;
            if (char.IsDigit(e.KeyChar) || e.KeyChar == 8)
                return;
            try
            {
                if (e.KeyChar == '-')
                {
                    if (Text.Length == 0 || Text.Length == SelectionLength)
                        return;
                }
                e.Handled = true;
            }
            catch 
            {
                e.Handled = true;
                this.SelectAll();
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (Text == "-")
                Text = "0";
        }
    }
}
