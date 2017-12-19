namespace Telerik.Data.Expressions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    class BinaryOpNode : ExpressionNode
    {
        protected ExpressionNode left;
        protected ExpressionNode right;
        protected Operator op;

        public ExpressionNode Left
        {
            get { return left; }
            set { left = value; }
        }

        public ExpressionNode Right
        {
            get { return right; }
            set { right = value; }
        }

        public Operator Op
        {
            get { return this.op; }
        }

        public override bool IsConst
        {
            get { return (this.left.IsConst && this.right.IsConst); }
        }

        public BinaryOpNode(Operator op, ExpressionNode left, ExpressionNode right)
        {
            this.op = op;
            this.left = left;
            this.right = right;
        }

        public override object Eval(object row, object context)
        {
            Operator.BinaryFunc func = (Operator.BinaryFunc)this.op.Func;

            return func(new Operand(this.left, row, context)
                , new Operand(this.right, row, context)
                , new OperatorContext(row, this.Culture, context));
        }

        public override string ToString()
        {
            return string.Format("BinaryOp {0} ({1}, {2})", this.op.Name, this.left, this.right);
        }


        public override IEnumerable<ExpressionNode> GetChildNodes()
        {
            if (null != this.left)
            {
                yield return this.left;
            }
            if (null != this.right)
            {
                yield return this.right;
            }
        }
    }

    class TernaryOpNode : ExpressionNode
    {
        Operator @operator;
        ExpressionNode[] operands;

        public TernaryOpNode(Operator @operator, ExpressionNode op1, ExpressionNode op2, ExpressionNode op3)
        {
            this.@operator = @operator;
            this.operands = new ExpressionNode[] { op1, op2, op3 };
        }

        public override object Eval(object row, object context)
        {
            Operator.TernaryFunc func = (Operator.TernaryFunc)this.@operator.Func;
            return func(new Operand(this.operands[0], row, context)
                , new Operand(this.operands[1], row, context)
                , new Operand(this.operands[2], row, context)
                , new OperatorContext(row, this.Culture, context));
        }
    }
}