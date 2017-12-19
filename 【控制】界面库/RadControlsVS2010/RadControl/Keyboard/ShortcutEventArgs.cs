using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.Keyboard
{
    public class ChordEventArgs : CancelEventArgs
    {
        public ChordEventArgs(Control associatedControl, RadItem associatedItem)
        {
			this.associatedItem = associatedItem;
            this.associatedControl = associatedControl;
        }

        // Properties
        public Control AssociatedControl
        {
            get
            {
                return this.associatedControl;
            }
        }

		public RadItem AssociatedItem
		{
			get
			{
				return this.associatedItem;
			}
		}

        // Fields
        private Control associatedControl;
		private RadItem associatedItem;
    }
}
