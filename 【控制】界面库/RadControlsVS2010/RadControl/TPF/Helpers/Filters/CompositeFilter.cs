using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public abstract class CompositeFilter : Filter
    {
        #region Constructor

        public CompositeFilter(params Filter[] filters)
        {
            this.filters = new List<Filter>();

            if (filters != null)
            {
                foreach (Filter f in filters)
                {
                    this.filters.Add(f);
                }
            }
        }

        #endregion

        #region Fields

        protected List<Filter> filters;

        #endregion
    }
}
