using System.Collections.Generic;

namespace Telerik.WinControls
{

    /// <summary>
    /// ComplexCondition evaluates two conditions related with a binary operator.
    /// Inherits Condition
    /// </summary>
    public class ComplexCondition : Condition
    {
        Condition condition1;
        Condition condition2;
        BinaryOperator binaryOperator = BinaryOperator.AndOperator;
        /// <summary>
        /// Gets or sets the first condition.
        /// </summary>
        public Condition Condition1
        {
            get { return condition1; }
            set { condition1 = value; }
        }
        /// <summary>
        /// Gets or sets the binary operator to be used for evaluating the condition.
        /// </summary>
        public BinaryOperator BinaryOperator
        {
            get { return binaryOperator; }
            set { binaryOperator = value; }
        }
        /// <summary>
        /// Gets or sets the second condition.
        /// </summary>
        public Condition Condition2
        {
            get { return condition2; }
            set { condition2 = value; }
        }
        /// <summary>
        /// Initializes a new instance of the ComplexCondition class.
        /// </summary>
        public ComplexCondition()
        {
        }
        /// <summary>
        /// Initializes a new instance of the ComplexCondition class from the first condition, 
        /// binary operator, and second condition.
        /// </summary>
        /// <param name="condition1"></param>
        /// <param name="binaryOperator"></param>
        /// <param name="condition2"></param>
        public ComplexCondition(Condition condition1, BinaryOperator binaryOperator, Condition condition2)
        {
            this.condition1 = condition1;
            this.binaryOperator = binaryOperator;
            this.condition2 = condition2;
        }
        /// <summary>
        /// Evaluates the complex condition.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool Evaluate(RadElement target)
        {
            if (target == null)
            {
                return false;
            }

            switch (this.BinaryOperator)
            {
                case BinaryOperator.AndOperator:
                    return this.Condition1.Evaluate(target) && this.Condition2.Evaluate(target);
                case BinaryOperator.OrOperator:
                    return this.Condition1.Evaluate(target) || this.Condition2.Evaluate(target);
                case BinaryOperator.XorOperator:
                    return this.Condition1.Evaluate(target) != this.Condition2.Evaluate(target);
            }

            return false;
        }

        protected override void FillAffectedProperties(List<RadProperty> inList)
        {
            inList.AddRange(this.Condition1.AffectedProperties);
            inList.AddRange(this.Condition2.AffectedProperties);
        }
        /// <summary>
        /// Retrieves a serializable instance of the ComplexCondtion.
        /// </summary>
        /// <returns></returns>
        public override XmlCondition CreateSerializableInstance()
        {
            return new XmlComplexCondition();
        }

        protected override void SerializeProperties(XmlCondition res)
        {
            base.SerializeProperties(res);
            XmlComplexCondition instance = (XmlComplexCondition)res;
            instance.Condition1 = this.Condition1.Serialize();
            instance.Condition2 = this.Condition2.Serialize();
            instance.BinaryOperator = this.BinaryOperator;
        }
        /// <summary>
        /// Retrives a string representation of the ComplexCondition class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Condition1 != null && this.Condition2 != null)
            {
                string op = this.BinaryOperator.ToString();

                return "(" + this.Condition1.ToString() + " " + op + " " + this.Condition2.ToString() + ")";
            }

            return base.ToString();
        }
    }
}
