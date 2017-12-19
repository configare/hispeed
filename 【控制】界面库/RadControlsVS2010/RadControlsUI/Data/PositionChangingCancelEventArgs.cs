using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Data
{
    public class PositionChangingCancelEventArgs : PositionChangedEventArgs
    {
        private bool cancel = false;

        public PositionChangingCancelEventArgs(int pos) : base(pos)
        {

        }

        public bool Cancel
        {
            get
            {
                return this.cancel;
            }

            set
            {
                this.cancel = value;
            }
        }
    }
}
