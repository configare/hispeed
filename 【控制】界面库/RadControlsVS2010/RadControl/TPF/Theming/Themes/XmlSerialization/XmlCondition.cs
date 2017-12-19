using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a serializable condition. 
    /// </summary>
    //[Serializable]
    //[XmlInclude(typeof(XmlSimpleCondition))]
    //[XmlInclude(typeof(XmlRoutedEventCondition))]
    //[XmlInclude(typeof(XmlComplexCondition))]
    public abstract class XmlCondition
    {
        /// <summary>
        /// Deserializes the condition.
        /// </summary>
        /// <returns></returns>
        public Condition Deserialize()
        {
            Condition res = this.CreateInstance();
            this.DeserializeProperties(res);

            return res;
        }
        /// <summary>
        /// Deserializes the properties for a given condition.
        /// </summary>
        /// <param name="selector"></param>
        protected abstract void DeserializeProperties(Condition selector);
        /// <summary>
        /// Creates a new instance of the Condition class.
        /// </summary>
        /// <returns></returns>
        protected abstract Condition CreateInstance();
        /// <summary>
        /// Build the expression string. 
        /// </summary>
        /// <returns></returns>
		public string BuildExpressionString()
		{
			//TODO: replace usage of inheritors 
			//with implementation and overriding of a virtual function 
			string s = "";

			if (this is XmlComplexCondition)
			{
				XmlComplexCondition complex = this as XmlComplexCondition;
				s += "(";
				if (complex.Condition1 != null)
					s += complex.Condition1.BuildExpressionString();
				else
					s += "unknown";
				s += " " + complex.BinaryOperator.ToString().Replace("Operator", "").ToLower() + " ";
				if (complex.Condition2 != null)
					s += complex.Condition2.BuildExpressionString();
				else
					s += "unknown";

				s += ")";
			}
			else if (this is XmlSimpleCondition)
			{
				XmlSimpleCondition simple = this as XmlSimpleCondition;
				if (simple.UnaryOperator == UnaryOperator.NotOperator) s += "!";
				if (simple.Setting != null)
					s += simple.Setting.GetPropertyName();
				else
					s += "unknown";
			}

			return s;
		}
	}
}
