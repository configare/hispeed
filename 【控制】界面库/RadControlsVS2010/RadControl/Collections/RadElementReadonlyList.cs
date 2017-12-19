using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class RadElementReadonlyList : List<RadElement>
    {
        public RadElementReadonlyList()
        {
        }
        public RadElementReadonlyList(IEnumerable<RadElement> initValues)
            : base(initValues)
        {
        }
    }
}
