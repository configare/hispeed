using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI.UIElements.ListBox.Data;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.ComboBox
{
    class RadComboBoxBindingProvider : RadListBoxBindingProvider
    {
        public RadComboBoxBindingProvider(IBindableComponent owner)
            : base(owner)
        {

        }

        protected override RadListBoxItem CreateInstance()
        {
            return new RadComboBoxItem();
        }
    }
}
