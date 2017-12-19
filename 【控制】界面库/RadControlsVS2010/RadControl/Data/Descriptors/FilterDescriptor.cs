using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using Telerik.Data.Expressions;

namespace Telerik.WinControls.Data
{
    public class FilterDescriptor : INotifyPropertyChanged, ICloneable
    {
        #region Fields

        private string propertyName = null;
        private bool isFilterEditor = false;
        private FilterOperator filterOperator = FilterOperator.Contains;

        private object value;

        #endregion

        #region Constructors

        public FilterDescriptor()
        {

        }

        public FilterDescriptor(string propertyName, FilterOperator filterOperator, object value)
        {
            this.propertyName = propertyName;
            this.filterOperator = filterOperator;
            this.value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        [Browsable(true)]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string PropertyName
        {
            get { return this.propertyName; }
            set
            {
                if (this.propertyName != value)
                {
                    this.propertyName = value;
                    this.OnPropertyChanged("PropertyName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>The operator.</value>
        [Browsable(true)]
        [DefaultValue(FilterOperator.Contains)]
        public virtual FilterOperator Operator
        {
            get { return this.filterOperator; }
            set
            {
                if (this.filterOperator != value)
                {
                    this.filterOperator = value;
                    this.OnPropertyChanged("Operator");
                }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [DefaultValue(null)]
        [TypeConverter(typeof(FilterValueStringConverter))]
        public virtual object Value
        {
            get { return this.value; }
            set
            {
                if (!object.Equals(this.value, value))
                {
                    this.value = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }

        /// <summary>
        /// Gets the filter expression.
        /// </summary>
        /// <value>The filter expression.</value>
        [Browsable(true)]
        public virtual string Expression
        {
            get
            {
                if (string.IsNullOrEmpty(this.PropertyName))
                {
                    return string.Empty;
                }

                return FilterDescriptor.GetExpression(this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is default filter descriptor of the column
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        [Browsable(true)]
        [DefaultValue(false)]
        public virtual bool IsFilterEditor
        {
            get { return this.isFilterEditor; }
            set
            {
                if (value != this.isFilterEditor)
                {
                    this.isFilterEditor = value;
                    this.OnPropertyChanged("IsFilterEditor");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <param name="filterDescriptor">The filter descriptor.</param>
        /// <returns></returns>
        public static string GetExpression(FilterDescriptor filterDescriptor)
        {
            if (string.IsNullOrEmpty(filterDescriptor.PropertyName) ||
                filterDescriptor.filterOperator == FilterOperator.None)
            {
                return string.Empty;
            }

            if ((filterDescriptor.Operator != FilterOperator.IsNotNull && filterDescriptor.Operator != FilterOperator.IsNull)
                && filterDescriptor.value == null)
            {
                return string.Empty;
            }

            object value = filterDescriptor.Value;

            if (value is string || value is char)
            {
                value = "'" + DataStorageHelper.EscapeValue(Convert.ToString(value)) + "'";
            }
            else if (value is Enum)
            {
                value = Convert.ToInt32(value);
            }
            else if (value is DateTime)
            {
                value = String.Format(CultureInfo.InvariantCulture, "#{0}#", value);
            }
            else if (value is double)
            {
                value = ((double)value).ToString("R", CultureInfo.InvariantCulture);
            }
            else
            {
                value = Convert.ToString(filterDescriptor.Value, CultureInfo.InvariantCulture);
            }

            string likeValue = String.Empty;
            string name = DataStorageHelper.EscapeName(filterDescriptor.PropertyName);

            switch (filterDescriptor.Operator)
            {
                case FilterOperator.IsLike:
                    return string.Format("{0} LIKE {1}", name, value);
                case FilterOperator.IsNotLike:
                    return string.Format("{0} NOT LIKE {1}", name, value);
                case FilterOperator.StartsWith:
                    likeValue = DataStorageHelper.EscapeLikeValue(Convert.ToString(filterDescriptor.Value, CultureInfo.InvariantCulture));
                    return string.Format("{0} LIKE '{1}%'", name, likeValue);
                case FilterOperator.EndsWith:
                    likeValue = DataStorageHelper.EscapeLikeValue(Convert.ToString(filterDescriptor.Value, CultureInfo.InvariantCulture));
                    return string.Format("{0} LIKE '%{1}'", name, likeValue);
                case FilterOperator.Contains:
                    likeValue = DataStorageHelper.EscapeLikeValue(Convert.ToString(filterDescriptor.Value, CultureInfo.InvariantCulture));
                    return string.Format("{0} LIKE '%{1}%'", name, likeValue);
                case FilterOperator.NotContains:
                    likeValue = DataStorageHelper.EscapeLikeValue(Convert.ToString(filterDescriptor.Value, CultureInfo.InvariantCulture));
                    return string.Format("{0} NOT LIKE '%{1}%'", name, likeValue);
                case FilterOperator.IsEqualTo:
                    return string.Format("{0} = {1}", name, value);
                case FilterOperator.IsLessThan:
                    return string.Format("{0} < {1}", name, value);
                case FilterOperator.IsLessThanOrEqualTo:
                    return string.Format("{0} <= {1}", name, value);
                case FilterOperator.IsGreaterThan:
                    return string.Format("{0} > {1}", name, value);
                case FilterOperator.IsGreaterThanOrEqualTo:
                    return string.Format("{0} >= {1}", name, value);
                case FilterOperator.IsNotEqualTo:
                    return string.Format("{0} <> {1}", name, value);
                case FilterOperator.IsNull:
                    return string.Format("{0} IS NULL", name);
                case FilterOperator.IsNotNull:
                    return string.Format("NOT ({0} IS NULL)", name);
                case FilterOperator.IsContainedIn:
                    return string.Format("{0} IN ({1})", name, value);
                case FilterOperator.IsNotContainedIn:
                    return string.Format("{0} NOT IN ({1})", name, value);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Expression;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> instance containing event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        #endregion

        #region ICloneable Members

        public virtual object Clone()
        {
            FilterDescriptor filterDescriptor = new FilterDescriptor(this.propertyName, this.filterOperator, this.value);
            return filterDescriptor;
        }

        #endregion
    }
}
