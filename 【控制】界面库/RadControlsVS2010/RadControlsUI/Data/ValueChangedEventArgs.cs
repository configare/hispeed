using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Data
{
    public class ValueChangedEventArgs : PositionChangedEventArgs
    {
        private object oldValue;
        private object newValue;

        public ValueChangedEventArgs(int newIndex, object newValue, object oldValue) : base(newIndex)
        {
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        public object NewValue
        {
            get
            {
                return this.newValue;
            }
        }

        public object OldValue
        {
            get
            {
                return this.oldValue;
            }
        }
    }
}
