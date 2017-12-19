using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.DF.GDAL.HDF5Universal
{
    public class DoubleTextBox:TextBox
    {
        protected int _selectionLen = 0;

        public double Value
        {
            get
            {
                double v = 0;
                if (double.TryParse(Text, out v))
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
            if (e.KeyChar == '.')
            {
                if (Text.Length == 0 || Text.EndsWith(".") || Text[SelectionStart - 1] == '.' || Text[SelectionStart - 1] == '-' || Text.IndexOf('.') >= 0)
                {
                    e.Handled = true;
                    return;
                }
                else
                {
                    if (e.KeyChar == '.')
                    {
                        if (SelectionStart == Text.Length)
                        {
                            Text += "0";
                            SelectionStart = Text.Length - 1;
                        }
                    }
                    return;
                }
            }
            else if (e.KeyChar == '-')
            {
                if (Text.Length == 0 || Text.Length == SelectionLength)
                    return;
            }
            e.Handled = true;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (Text == "-")
                Text = "0";
        }
    }
}
