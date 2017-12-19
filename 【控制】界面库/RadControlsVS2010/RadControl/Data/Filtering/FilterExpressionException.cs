using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    public class FilterExpressionException : Exception
    {
        public FilterExpressionException(string message)
            : base(message)
        {

        }

        public FilterExpressionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
