using System;

namespace Windows.Toolbar.Controls
{
    public class SelectedValueChangedEventArgs : EventArgs
    {
        public object Value { get; set; }

        public SelectedValueChangedEventArgs(object value)
        {
            Value = value;
        }
    }
}
