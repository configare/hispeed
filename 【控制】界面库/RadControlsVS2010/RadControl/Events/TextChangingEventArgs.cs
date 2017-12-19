using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls
{
    public class TextChangingEventArgs : CancelEventArgs
    {
        private string oldValue;
        private string newValue;

        public string OldValue
        {
            get { return this.oldValue; }
        }

        public string NewValue
        {
            get { return this.newValue; }
            set { this.newValue = value; }
        }

        public TextChangingEventArgs(string oldValue, string newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public TextChangingEventArgs(string oldValue, string newValue, bool cancel)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.Cancel = cancel;
        }
    }
}
