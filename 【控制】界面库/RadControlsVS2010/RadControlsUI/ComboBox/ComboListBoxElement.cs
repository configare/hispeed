using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI.ComboBox;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
	public class ComboListBoxElement : RadListBoxElement
	{
        protected override void InstantiateDataProvider()
        {
            this.dataProvider = new RadComboBoxBindingProvider(this);
        }

        protected override void DisposeManagedResources()
        {
            foreach (RadItem item in this.Items)
            {
                item.Dispose();
            }

            base.DisposeManagedResources();
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadListBoxElement);
            }
        }
	}
}
