using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.FileProject
{
    public class PrjTextBox:TextBox
    {
        MaskedTextBox _textBox;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }
    }
}
