using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    public enum FilterOperator
    {
        None = 0,
        IsLike = 1,
        IsNotLike,
        IsLessThan,
        IsLessThanOrEqualTo,
        IsEqualTo,
        IsNotEqualTo,
        IsGreaterThanOrEqualTo,
        IsGreaterThan,
        StartsWith,
        EndsWith,
        Contains,
        NotContains,
        IsNull,
        IsNotNull,
        IsContainedIn,
        IsNotContainedIn
    }

    public enum FilterLogicalOperator
    {
        And,
        Or
    }
}
