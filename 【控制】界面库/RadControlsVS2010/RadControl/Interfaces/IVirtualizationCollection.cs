using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public interface IVirtualizationCollection
    {
        int Count
        {
            get;
        }

        object GetVirtualData(int index);
        int IndexOf(object data);
    }
}
