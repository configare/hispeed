using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public interface IKeyboardListener
    {
        MessagePreviewResult OnPreviewKeyDown(Control target, KeyEventArgs e);
        MessagePreviewResult OnPreviewKeyPress(Control target, KeyPressEventArgs e);
        MessagePreviewResult OnPreviewKeyUp(Control target, KeyEventArgs e);
    }
}
