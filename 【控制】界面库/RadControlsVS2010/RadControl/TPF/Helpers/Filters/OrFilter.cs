using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class OrFilter : CompositeFilter
    {
        public OrFilter(params Filter[] filters)
            : base(filters)
        {
        }

        public override bool Match(object obj)
        {
            bool match = false;
            //we need only one match since this is OR operation
            foreach (Filter f in this.filters)
            {
                if (f.Match(obj))
                {
                    match = true;
                    break;
                }
            }

            return match;
        }
    }
}
