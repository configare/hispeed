using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class LogicExpressTuple:IExpressTuple
    {
        protected enumLogicOperator _operator = enumLogicOperator.Equals;
        protected string _compareValue = null;

        public LogicExpressTuple(enumLogicOperator soperator)
        {
            _operator = soperator;
        }

        public LogicExpressTuple(enumLogicOperator soperator, string compareValue)
            : this(soperator)
        {
            _compareValue = compareValue;
        }

        #region IExpressTuple Members

        public bool IsTrue(string originalValue)
        {
            switch (_operator)
            { 
                case enumLogicOperator.Equals:
                    return CompareEquals(originalValue, _compareValue);
                case enumLogicOperator.GreatThan:
                    return CompareGreatThan(originalValue, _compareValue);
                case enumLogicOperator.LessThan:
                    return CompareLessThan(originalValue, _compareValue);
                case enumLogicOperator.IsNull:
                    return originalValue == null;
                case enumLogicOperator.IsUnNull:
                    return originalValue != null;
                case enumLogicOperator.Like:
                    return CompareLike(originalValue, _compareValue);
                case enumLogicOperator.UnEquals:
                    return !CompareEquals(originalValue, _compareValue);
                default:
                    return false;
            }
        }

        private bool CompareEquals(string originalValue, string compareValue)
        {
            if (string.IsNullOrEmpty(originalValue) && string.IsNullOrEmpty(compareValue))
                return true;
            if (string.IsNullOrEmpty(originalValue) && !string.IsNullOrEmpty(compareValue))
                return false;
            if (!string.IsNullOrEmpty(originalValue) && string.IsNullOrEmpty(compareValue))
                return false;
            return originalValue == compareValue;
        }

        private bool CompareLike(string originalValue, string compareValue)
        {
            if (string.IsNullOrEmpty(originalValue) && string.IsNullOrEmpty(compareValue))
                return true;
            if (string.IsNullOrEmpty(originalValue) && !string.IsNullOrEmpty(compareValue))
                return false;
            if (!string.IsNullOrEmpty(originalValue) && string.IsNullOrEmpty(compareValue))
                return false;
            return originalValue.Contains(compareValue);
        }

        private bool CompareLessThan(string originalValue, string compareValue)
        {
            return !CompareGreatThan(originalValue, compareValue);
        }

        private bool CompareGreatThan(string originalValue, string compareValue)
        {
            if (string.IsNullOrEmpty(originalValue) && string.IsNullOrEmpty(compareValue))
                return true;
            if (string.IsNullOrEmpty(originalValue) && !string.IsNullOrEmpty(compareValue))
                return false;
            if (!string.IsNullOrEmpty(originalValue) && string.IsNullOrEmpty(compareValue))
                return true;
            originalValue = originalValue.Trim();
            compareValue = compareValue.Trim();
            double ov = 0, cv = 0;
            if (double.TryParse(originalValue, out ov) && double.TryParse(compareValue, out cv))
            {
                return ov > cv;
            }
            else
            {
                for (int i = 0; i < originalValue.Length; i++)
                {
                    if (i > compareValue.Length - 1)
                        return false;
                    else if (originalValue[i] > compareValue[i])
                        return true;
                }
            }
            return false;
        }

        #endregion
    }
}
