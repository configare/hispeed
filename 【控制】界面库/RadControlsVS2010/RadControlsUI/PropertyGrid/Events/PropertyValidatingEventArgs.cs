using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void PropertyValidatingEventHandler(object sender, PropertyValidatingEventArgs e);

    public class PropertyValidatingEventArgs : PropertyGridItemValueChangingEventArgs
    {
        public PropertyValidatingEventArgs(PropertyGridItemBase item, object newValue, object oldValue)
            : base(item, newValue, oldValue)
        { 
        }
    }
}
