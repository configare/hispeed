using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public class DoubleTextBox:TextBox
    {
        protected int _selectionLen = 0;
        private double _defaultValue =0;

        public DoubleTextBox()
        { }

        //public event KeyPressEventHandler KeyPressEnter;
        public event EventHandler LostFocusValueChanged;

        public double DefaultValue
        {
            get 
            { 
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
                Text = value.ToString();
            }
        }

        public double MinValue = double.MinValue;

        public double Value
        {
            get
            {
                double v = 0;
                if (double.TryParse(Text, out v))
                {
                    if (v <= MinValue)
                    {
                        v = DefaultValue;
                        Text = DefaultValue.ToString();
                    }
                    return v;
                }
                return DefaultValue;
            }
            set
            {
                Text = value.ToString();
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && LostFocusValueChanged != null)
                LostFocusValueChanged(this, e);
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
                            Text += DefaultValue.ToString();
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
                Text = DefaultValue.ToString();
            if (_perValue != Value)//值发生改变
            {
                if (LostFocusValueChanged != null)
                    LostFocusValueChanged(this, e);
            }
        }

        private double _perValue;

        protected override void OnGotFocus(EventArgs e)
        {
            _perValue = Value;
            base.OnGotFocus(e);
        }
    }
}
