using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Reflection;

namespace Telerik.WinControls
{

    /// <summary>
    /// Represents a serializable correspodence to the ComplexCondtion class.
    /// </summary>
    //[Serializable]
    //[XmlInclude(typeof(BinaryOperator))]
    //[XmlInclude(typeof(XmlSimpleCondition))]
    public class XmlComplexCondition : XmlCondition
    {
        XmlCondition condition1;
        XmlCondition condition2;
        BinaryOperator binaryOperator = BinaryOperator.AndOperator;
        /// <summary>
        /// Gets or sets a value indicating the first condition.
        /// </summary>
        [Browsable(false)]
        public XmlCondition Condition1
        {
            get { return this.condition1; }
            set { this.condition1 = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating the binary operator for the condition.
        /// </summary>
        [XmlAttribute]
        [DefaultValue(BinaryOperator.AndOperator)]
        public BinaryOperator BinaryOperator
        {
            get { return this.binaryOperator; }
            set { this.binaryOperator = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating the second condition.
        /// </summary>
        [Browsable(false)]
        public XmlCondition Condition2
        {
            get { return this.condition2; }
            set { this.condition2 = value; }
        }
        /// <summary>
        /// Compares two XmlComplexCondtion(s) for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is XmlComplexCondition)
            {
                XmlComplexCondition cond = obj as XmlComplexCondition;
                
                if (this.binaryOperator == cond.binaryOperator &&
                    this.condition1.Equals(cond.Condition1) &&
                    this.condition2.Equals(cond.Condition2))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Retrieves a hash code for the current instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        } 

        protected override Condition CreateInstance()
        {
            Condition condition1 = null;
            Condition condition2 = null;
            //if (this.Condition1 != null)
            {
                condition1 = this.Condition1.Deserialize();
            }

            //if (this.Condition2 != null)
            {
                condition2 = this.Condition2.Deserialize();
            }
            return new ComplexCondition(condition1, this.BinaryOperator, condition2);
        }

        protected override void DeserializeProperties(Condition selector)
        {
            //
        }
    }
}
