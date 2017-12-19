using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.UI
{
    public class TextBoxEx : System.Windows.Forms.TextBox
    {
        public event EventHandler LostFocusValueChanged;

        private string _perValue;

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && LostFocusValueChanged != null)
                LostFocusValueChanged(this, e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (_perValue != Text)//值发生改变
            {
                if (LostFocusValueChanged != null)
                    LostFocusValueChanged(this, e);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            _perValue = Text;
            base.OnGotFocus(e);
        }
    }
}
