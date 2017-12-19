using System;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public delegate void DialogClosedEventHandler(object sender, DialogClosedEventArgs e);

    public class DialogClosedEventArgs : EventArgs
    {
        private DialogResult dialogResult;

        public DialogClosedEventArgs(DialogResult dialogResult)
        {
            this.dialogResult = dialogResult;
        }

        public DialogResult DialogResult
        {
            get
            {
                return dialogResult;
            }
        }
    }
}
