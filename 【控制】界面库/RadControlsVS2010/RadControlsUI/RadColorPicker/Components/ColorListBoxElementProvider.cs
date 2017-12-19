using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

namespace Telerik.WinControls.UI.RadColorPicker
{
    public class ColorListBoxElementProvider: ListElementProvider
    {
        public ColorListBoxElementProvider(RadListElement listElement): base(listElement)
        {
        }

        public override IVirtualizedElement<RadListDataItem> CreateElement(RadListDataItem data, object context)
        {
            return new ColorListBoxItem();
        }
    }
}
