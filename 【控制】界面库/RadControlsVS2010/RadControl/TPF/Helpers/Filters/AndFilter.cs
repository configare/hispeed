using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class AndFilter : CompositeFilter
    {
        public AndFilter(params Filter[] filters)
            : base(filters)
        {
        }

        public override bool Match(object obj)
        {
            //all filters should match since this is AND operation
            foreach (Filter filter in this.filters)
            {
                if (!filter.Match(obj))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
