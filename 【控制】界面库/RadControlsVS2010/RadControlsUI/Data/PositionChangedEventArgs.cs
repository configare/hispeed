using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Data
{
    public class PositionChangedEventArgs : EventArgs
    {
        private int pos = 0;
        public PositionChangedEventArgs(int position)
        {
            this.pos = position;
        }

        public int Position
        {
            get
            {
                return this.pos;
            }
        }
    }
}
