using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class TypeFilter<T> : Filter
    {
        public override bool Match(object obj)
        {
            return obj is T;
        }
    }
}
