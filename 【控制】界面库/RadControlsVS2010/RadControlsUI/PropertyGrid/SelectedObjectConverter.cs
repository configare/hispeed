using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    internal class SelectedObjectConverter: ReferenceConverter
    {
        public SelectedObjectConverter(): base(typeof(IComponent))
        {
        }
    }
}
