namespace Telerik.Data.Expressions
{
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    class ZeroOpNode : ExpressionNode
    {
        Operator op;

        public override bool IsConst
        {
            get { return true; }
        }

        public ZeroOpNode(Operator op)
        {
            this.op = op;    
        }

        public override object Eval(object row, object context)
        {
            Operator.ZeroFunc func = (Operator.ZeroFunc)this.op.Func;
            return func();
        }

        public override string ToString()
        {
            return ("ZeroOp: " + this.op);
        }
    }
}