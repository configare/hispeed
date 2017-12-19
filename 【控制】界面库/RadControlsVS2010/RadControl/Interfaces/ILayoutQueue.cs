using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public interface ILayoutQueue
    {
        int Count { get; }

        void Add(RadElement e);
        void Remove(RadElement e);
    }
}
