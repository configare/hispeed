using System;
using System.ComponentModel;
using System.Text;

namespace Telerik.WinControls.Data
{
    public class CompositeFilterDescriptor : FilterDescriptor
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        public enum DescriptorType
        {
            /// <summary>
            /// Type is not predefined.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Between <see cref="CompositeFilterDescriptor"/>
            /// </summary>
            Between,
            /// <summary>
            /// Not Between <see cref="CompositeFilterDescriptor"/>
            /// </summary>
            NotBetween
        }

        #endregion

        #region Fields

        private FilterDescriptorCollection filters;
        private bool notOperator = false;

        #endregion

        #region Static Methods

        /// <summary>
        /// Gets the type of the <see cref="CompositeFilterDescriptor"/>.
        /// </summary>
        /// <param name="compositeDescriptor">The filter descriptor.</param>
        /// <returns></returns>
        public static DescriptorType GetDescriptorType(CompositeFilterDescriptor compositeDescriptor)
        {
            DescriptorType type = DescriptorType.Unknown;

            if (compositeDescriptor == null || compositeDescriptor.FilterDescriptors.Count != 2)
            {
                return type;
            }

            FilterDescriptor firstDescriptor = compositeDescriptor.FilterDescriptors[0];
            FilterDescriptor secondDescriptor = compositeDescriptor.FilterDescriptors[1];

            if (firstDescriptor is CompositeFilterDescriptor || secondDescriptor is CompositeFilterDescriptor)
            {
                return type;
            }

            // Between Descriptor
            if (firstDescriptor.Operator == FilterOperator.IsGreaterThanOrEqualTo &&
                secondDescriptor.Operator == FilterOperator.IsLessThanOrEqualTo &&
                compositeDescriptor.LogicalOperator == FilterLogicalOperator.And)
            {
                type = DescriptorType.Between;
            }

            // NotBetween Descriptor
            if ((compositeDescriptor.NotOperator && type == DescriptorType.Between) ||
                (type == DescriptorType.Unknown && firstDescriptor.Operator == FilterOperator.IsLessThanOrEqualTo &&
                 secondDescriptor.Operator == FilterOperator.IsGreaterThanOrEqualTo && !compositeDescriptor.NotOperator &&
                 compositeDescriptor.LogicalOperator == FilterLogicalOperator.Or))
            {
                type = DescriptorType.NotBetween;
            }

            return type;
        }

        /// <summary>
        /// Creates the descriptor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static CompositeFilterDescriptor CreateDescriptor(DescriptorType type, string propertyName, params object[] values)
        {
            if (type == DescriptorType.Unknown)
            {
                throw new InvalidOperationException("You cannot create undefined composite filter descriptor.");
            }

            //if (String.IsNullOrEmpty(propertyName))
            //{
            //    throw new ArgumentNullException("propertyName");
            //}

            CompositeFilterDescriptor compositeDescriptor = new CompositeFilterDescriptor();
            compositeDescriptor.PropertyName = propertyName;

            switch (type)
            {
                case DescriptorType.NotBetween:
                case DescriptorType.Between:

                    if (values != null && values.Length != 2)
                    {
                        throw new ArgumentException("values");
                    }

                    if (values == null)
                    {
                        values = new object[2];
                    }

                    compositeDescriptor.LogicalOperator = FilterLogicalOperator.And;
                    compositeDescriptor.FilterDescriptors.Add(new FilterDescriptor(propertyName, FilterOperator.IsGreaterThanOrEqualTo, values[0]));
                    compositeDescriptor.FilterDescriptors.Add(new FilterDescriptor(propertyName, FilterOperator.IsLessThanOrEqualTo, values[1]));
                    compositeDescriptor.NotOperator = type == DescriptorType.NotBetween;
                    break;
            }

            return compositeDescriptor;
        }

        private static CompositeFilterDescriptor ConvertDescriptor(CompositeFilterDescriptor compositeFilter, DescriptorType type)
        {
            if (compositeFilter == null)
            {
                throw new ArgumentNullException("compositeFilter");
            }

            if (type == DescriptorType.Unknown)
            {
                throw new InvalidOperationException("You cannot convert the filter descriptor to unknown type.");
            }

            CompositeFilterDescriptor result = compositeFilter.Clone() as CompositeFilterDescriptor;

            while (result.FilterDescriptors.Count > 2)
            {
                int lastIndex = result.FilterDescriptors.Count - 1;
                result.FilterDescriptors.RemoveAt(lastIndex);
            }

            while (result.FilterDescriptors.Count < 2)
            {
                result.FilterDescriptors.Add(new FilterDescriptor());
            }

            if (type == DescriptorType.Between || type == DescriptorType.NotBetween)
            {
                result.LogicalOperator = FilterLogicalOperator.And;

                FilterDescriptor firstFilterDescriptor = result.FilterDescriptors[0];
                firstFilterDescriptor.PropertyName = result.PropertyName;
                firstFilterDescriptor.Operator = FilterOperator.IsGreaterThanOrEqualTo;

                FilterDescriptor secondFilterDescriptor = result.FilterDescriptors[1];
                secondFilterDescriptor.PropertyName = result.PropertyName;
                secondFilterDescriptor.Operator = FilterOperator.IsLessThanOrEqualTo;

                result.NotOperator = type == DescriptorType.NotBetween;
            }

            return result;
        }

        #endregion

        #region Constructors

        public CompositeFilterDescriptor()
        {
            this.filters = new FilterDescriptorCollection();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the logical operator.
        /// </summary>
        /// <value>The logical operator.</value>
        [Browsable(true)]
        [DefaultValue(FilterLogicalOperator.And)]
        public virtual FilterLogicalOperator LogicalOperator
        {
            get { return this.FilterDescriptors.LogicalOperator; }
            set
            {
                if (this.FilterDescriptors.LogicalOperator != value)
                {
                    this.FilterDescriptors.LogicalOperator = value;
                    this.OnPropertyChanged("LogicalOperator");
                }
            }
        }

        /// <summary>
        /// Gets the filter descriptors.
        /// </summary>
        /// <value>The filter descriptors.</value>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return this.filters;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [not operator].
        /// </summary>
        /// <value><c>true</c> if [not operator]; otherwise, <c>false</c>.</value>
        public bool NotOperator
        {
            get { return this.notOperator; }
            set
            {
                if (this.notOperator != value)
                {
                    this.notOperator = value;
                    this.OnPropertyChanged("NotOperator");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public override string PropertyName
        {
            get
            {
                if (this.IsSameName)
                {
                    return this.filters[0].PropertyName;
                }

                return base.PropertyName;
            }
            set
            {
                if (this.IsSameName)
                {
                    this.filters.BeginUpdate();
                    for (int i = 0; i < this.filters.Count; i++)
                    {
                        this.filters[i].PropertyName = value;
                    }
                    this.filters.EndUpdate();
                }

                base.PropertyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public override FilterOperator Operator
        {
            get
            {
                return base.Operator;
            }
            set
            {
                base.Operator = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance contains FilterDescriptor's with different PropertyName.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if any child filters have the same name; otherwise <c>false</c>.
        /// </value>
        public bool IsSameName
        {
            get
            {
                if (this.filters.Count == 0)
                {
                    return false;
                }

                string propertyName = this.filters[0].PropertyName;
                for (int i = 1; i < this.filters.Count; i++)
                {
                    if (string.Compare(propertyName, this.filters[i].PropertyName, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        return false;
                    }
                }

                return true;
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
                if (this.filters.Count == 0)
                {
                    return base.Expression;
                }

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < this.filters.Count; i++)
                {
                    if (string.IsNullOrEmpty(this.filters[i].PropertyName))
                    {
                        continue;
                    }

                    stringBuilder.Append(string.Format("{0}", this.filters[i].Expression));
                    string logicalOperator = (this.filters.LogicalOperator == FilterLogicalOperator.And) ? "AND" : "OR";
                    if (i < this.filters.Count - 1)
                    {
                        stringBuilder.Append(string.Format(" {0} ", logicalOperator));
                    }
                }

                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Insert(0, (this.NotOperator) ? "NOT (" : "(");
                    stringBuilder.Append(")");
                }

                return stringBuilder.ToString();
            }
        }

        #endregion

        #region Methods

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

        public override object Clone()
        {
            CompositeFilterDescriptor filterDescriptor = new CompositeFilterDescriptor();

            filterDescriptor.PropertyName = this.PropertyName;
            filterDescriptor.Operator = this.Operator;
            filterDescriptor.Value = this.Value;
            filterDescriptor.IsFilterEditor = this.IsFilterEditor;
            filterDescriptor.notOperator = this.notOperator;
            filterDescriptor.LogicalOperator = this.LogicalOperator;

            foreach (FilterDescriptor descriptor in this.filters)
            {
                filterDescriptor.FilterDescriptors.Add(descriptor.Clone() as FilterDescriptor);
            }

            return filterDescriptor;
        }

        /// <summary>
        /// Converts to the filter descriptor to concrete type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The converted instance of <see cref="CompositeFilterDescriptor"/></returns>
        public CompositeFilterDescriptor ConvertTo(DescriptorType type)
        {
            return CompositeFilterDescriptor.ConvertDescriptor(this, type);
        }

        #endregion

    }
}
