namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    class UnaryOpNode : ExpressionNode
    {
        Operator op;      
        ExpressionNode right;

        public ExpressionNode Right
        {
            get { return right; }
            set { right = value; }
        }

        public override bool IsConst
        {
            get { return (this.right.IsConst); }
        }

        public UnaryOpNode(Operator op, ExpressionNode right)
        {
            this.op = op;
            this.right = right;
        }

        public override object Eval(object row, object context)
        {
            Operator.UnaryFunc func = (Operator.UnaryFunc)this.op.Func;
            return func(new Operand(this.right, row, context), new OperatorContext(row, this.Culture, context));
        }

        public override string ToString()
        {
            return string.Format("UnaryOp: {0} ({1})", this.op.Name, this.right);
        }

        public override IEnumerable<ExpressionNode> GetChildNodes()
        {
            if (null != this.right)
            {
                yield return this.right;
            }
        }

        public Operator Op
        {
            get { return op; }
        }
    }
}