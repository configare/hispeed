using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public delegate void OpenFileDialogOpeningEventHandler(object sender, OpenFileDialogOpeningEventArgs e);

    public class OpenFileDialogOpeningEventArgs : CancelEventArgs
    {
        private OpenFileDialog openFileDialog;

        public OpenFileDialogOpeningEventArgs(OpenFileDialog openFileDialog)
        {
            this.openFileDialog = openFileDialog;
        }

        /// <summary>
        /// Gets the <see cref="OpenFileDialog"/> that is opening.
        /// </summary>
        public OpenFileDialog OpenFileDialog
        {
            get
            {
                return openFileDialog;
            }
        }
    }
}
