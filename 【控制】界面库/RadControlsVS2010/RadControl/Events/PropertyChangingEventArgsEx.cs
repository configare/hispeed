using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.Interfaces
{
    public class PropertyChangingEventArgsEx: CancelEventArgs
    {
        private readonly string propertyName;
        private object oldValue;
        private object newValue;

        #region Constructors

        public PropertyChangingEventArgsEx(string propertyName, object oldValue, object newValue, bool cancel):
            base(cancel)
        {
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public PropertyChangingEventArgsEx(string propertyName, object oldValue, object newValue)
            : this(propertyName, oldValue, newValue, false)
        {
        }

        public PropertyChangingEventArgsEx(string propertyName, bool cancel)
            : this(propertyName, null, null, cancel)
        {
        }

        public PropertyChangingEventArgsEx(string propertyName)
            : this(propertyName, null, null, false)
        {
        }

        #endregion

        public object OldValue
        {
            get { return this.oldValue; }
        }

        public object NewValue
        {
            get { return this.newValue; }
            set { this.newValue = value; }
        }

        public virtual string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }
    }
}
