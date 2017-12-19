using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides information about the new value that is edited inside the active 
    /// cell editor.
    /// </summary>
    public class ValueChangingEventArgs : CancelEventArgs
    {
        // Fields
        private object newValue;       
        private object oldValue;

        //initialize only new value
        public ValueChangingEventArgs(object newValue)
        {
            this.newValue = newValue;
        }

        //initialize both values
        public ValueChangingEventArgs(object newValue, object oldValue)
        {
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        /// <summary>
        /// Gets the new value that is edited by the active editor.
        /// </summary>
        public object NewValue
        {
            get
            {
                return this.newValue;
            }
            set
            {
                this.newValue = value;
            }
        }

        /// <summary>
        /// Gets the new value that is edited by the active editor.
        /// </summary>
        public object OldValue
        {
            get
            {
                return this.oldValue;
            }
            set
            {
                this.oldValue = value;
            }
        }
    }
}
