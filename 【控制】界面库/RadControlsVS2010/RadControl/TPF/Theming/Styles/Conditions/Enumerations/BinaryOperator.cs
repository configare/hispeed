using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// A binary opeartor used by the CompolexCondition class.
    /// </summary>
    [Serializable]
    public enum BinaryOperator
    {
        /// <summary>
        /// Indicates conjunction.
        /// </summary>
        AndOperator = 0,
        /// <summary>
        /// Indicates disjunction.
        /// </summary>
        OrOperator,
        /// <summary>
        /// Indicates exclusive or.
        /// </summary>
        XorOperator
    }
}
