namespace Telerik.Data.Expressions
{
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    sealed class Operand
    {
        ExpressionNode node;
        object row;
        object value;
        object expressionContext;

        public object Value
        {
            get 
            {
                if (null == this.value)
                {
                    this.value = this.node.Eval(this.row, this.expressionContext);
                }
                return this.value;
            }
        }

        public bool IsConst
        {
            get { return this.node.IsConst; }
        }

        public ExpressionNode Node
        {
            get { return this.node; }
        }

        public Operand(ExpressionNode node
                       , object row
                       , object expressionContext)
        {
            this.node = node;
            this.row = row;
            this.value = null;
            this.expressionContext = expressionContext;
        }
    }
}