namespace Telerik.Data.Expressions
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    class FunctionContext
    {
        IFormatProvider formatProvider;
        object globalContext;

        public IFormatProvider FormatProvider
        {
            get { return this.formatProvider; }
        }

        public object GlobalContext
        {
            get { return this.globalContext; }
        }

        public FunctionContext(IFormatProvider formatProvider, object globalContext)
        {
            this.formatProvider = formatProvider;
            this.globalContext = globalContext;
        }
    }
}