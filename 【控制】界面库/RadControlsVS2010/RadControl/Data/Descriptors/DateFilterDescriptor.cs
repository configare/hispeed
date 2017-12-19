using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Telerik.Data.Expressions;

namespace Telerik.WinControls.Data
{
    public class DateFilterDescriptor : FilterDescriptor
    {
        #region Fields

        private bool ignoreTimePart = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
        /// <value>The date value.</value>
        public new DateTime? Value
        {
            get 
            { 
                return (DateTime?)base.Value; 
            }
            set 
            { 
                base.Value = value; 
            }
        }

        /// <summary>
        /// Gets the filter expression.
        /// </summary>
        /// <value>The filter expression.</value>
        public override string Expression
        {
            get
            {
                if (string.IsNullOrEmpty(this.PropertyName))
                {
                    return string.Empty;
                }

                return DateFilterDescriptor.GetExpression(this);
            }
        }

        /// <summary>
        /// Get or set if the time part of date value should be ignored.
        /// </summary>
        public bool IgnoreTimePart
        {
            get { return this.ignoreTimePart; }
            set { this.ignoreTimePart = value; }
        }

        #endregion

        #region Constructors

        public DateFilterDescriptor()
        {
        }

        public DateFilterDescriptor(string propertyName, FilterOperator filterOperator, DateTime? value, bool ignoreTimePart)
        {
            base.PropertyName = propertyName;
            base.Operator = filterOperator;
            base.Value = value;
            this.ignoreTimePart = ignoreTimePart;
        }

        public DateFilterDescriptor(string propertyName, FilterOperator filterOperator, DateTime? value)
            : this(propertyName, filterOperator, value, false)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <param name="dateTimeFilterDescriptor">The filter descriptor.</param>
        /// <returns></returns>
        public static string GetExpression(DateFilterDescriptor dateTimeFilterDescriptor)
        {
            if (dateTimeFilterDescriptor.IgnoreTimePart)
            {
                if (String.IsNullOrEmpty(dateTimeFilterDescriptor.PropertyName) || 
                    dateTimeFilterDescriptor.Operator == FilterOperator.None)
                {
                    return string.Empty;
                }

                if ((dateTimeFilterDescriptor.Operator != FilterOperator.IsNotNull && dateTimeFilterDescriptor.Operator != FilterOperator.IsNull)
                     && dateTimeFilterDescriptor.Value == null)
                {
                    return string.Empty;
                }

                string dateBegin = null;
                string dateEnd = null;
                string propertyName = DataStorageHelper.EscapeName(dateTimeFilterDescriptor.PropertyName);

                if (dateTimeFilterDescriptor.Value != null)
                {
                    dateBegin = String.Format(CultureInfo.InvariantCulture, "#{0}#", dateTimeFilterDescriptor.Value.Value.Date);
                    dateEnd = String.Format(CultureInfo.InvariantCulture, "#{0}#", dateTimeFilterDescriptor.Value.Value.Date.AddDays(1));
                }

                switch (dateTimeFilterDescriptor.Operator)
                {
                    case FilterOperator.None:
                        return String.Empty;

                    case FilterOperator.IsNull:
                        return string.Format("{0} IS NULL", propertyName);
                    case FilterOperator.IsNotNull:
                        return string.Format("NOT ({0} IS NULL)", propertyName);

                    case FilterOperator.IsLessThan:
                        return string.Format("{0} < {1}", propertyName, dateBegin);
                    case FilterOperator.IsLessThanOrEqualTo:
                        return string.Format("{0} < {1}", propertyName, dateEnd);
                    case FilterOperator.IsLike:
                    case FilterOperator.IsEqualTo:
                        return string.Format("{0} >= {1} AND {0} < {2}", propertyName, dateBegin, dateEnd);
                    case FilterOperator.IsNotLike:
                    case FilterOperator.IsNotEqualTo:
                        return string.Format("{0} < {1} OR {0} >= {2}", propertyName, dateBegin, dateEnd);
                    case FilterOperator.IsGreaterThanOrEqualTo:
                        return string.Format("{0} >= {1}", propertyName, dateBegin);
                    case FilterOperator.IsGreaterThan:
                        return string.Format("{0} >= {1}", propertyName, dateEnd);

                    case FilterOperator.StartsWith:
                    case FilterOperator.EndsWith:
                    case FilterOperator.Contains:
                    case FilterOperator.NotContains:
                    case FilterOperator.IsContainedIn:
                    case FilterOperator.IsNotContainedIn:
                    default:
                        return String.Empty;
                }
            }
            else
            {
                return FilterDescriptor.GetExpression(dateTimeFilterDescriptor);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return FilterDescriptor.GetExpression(this);
        }

        public override object Clone()
        {
            DateFilterDescriptor cloneDescriptor = new DateFilterDescriptor(this.PropertyName, this.Operator, this.Value, this.IgnoreTimePart);
            return cloneDescriptor;
        }

        #endregion
    }
}
