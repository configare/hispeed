using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void PropertyValidatedEventHandler(object sender, PropertyValidatedEventArgs e);

    public class PropertyValidatedEventArgs : PropertyGridItemValueChangedEventArgs
    {
        public PropertyValidatedEventArgs(PropertyGridItemBase item)
            : base(item)
        {
        }
    }
}
