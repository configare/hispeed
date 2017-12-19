using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class TreeNodeTextEditor : RadTextBoxItem, IValueEditor
    {
        #region IValueEditor Members

        public void Initialize(object owner, object value)
        {
            if (value != null)
            {
                this.Text = value.ToString();
            }
        }

        public void BeginEdit()
        {
            
        }

        public bool EndEdit()
        {
            return true;
        }

        public bool Validate()
        {
            return true;
        }

        public object Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
       

        #endregion

        #region IValueEditor Members

        public event ValueChangingEventHandler ValueChanging;

        public event EventHandler ValueChanged;

        public event ValidationErrorEventHandler ValidationError;

        protected virtual void OnValueChanged(EventArgs args)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, args);
            }
        }

        protected virtual void OnValueChanging(ValueChangingEventArgs args)
        {
            if (this.ValueChanging != null)
            {
                this.ValueChanging(this, args);
            }
        }

        protected virtual void OnValidationError(ValidationErrorEventArgs args)
        {
            if (this.ValidationError != null)
            {
                this.ValidationError(this, args);
            }
        }

        #endregion
    }


}
