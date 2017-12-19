using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class WaitingIndicatorCollection : ObservableCollection<WaitingBarIndicatorElement>
    {
        public WaitingIndicatorCollection()
            : base()
        {
            Add(new WaitingBarIndicatorElement());
            Add(new WaitingBarIndicatorElement());
        }
        protected override void RemoveItem(int index)
        {
            if (this.Count > 2)
            {
                base.RemoveItem(index);
            }
        }
    }
}
