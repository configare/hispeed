using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Collections.Generic;
using Telerik.Data.Expressions;

namespace Telerik.WinControls.Data
{
    public class FilterDescriptorCollection : NotifyCollection<FilterDescriptor>
    {
        #region Fields

        private FilterLogicalOperator logicalOperator = FilterLogicalOperator.And;

        #endregion

        #region Constructors

        public FilterDescriptorCollection()
        {

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
            get { return this.logicalOperator; }
            set
            {
                if (this.logicalOperator != value)
                {
                    this.logicalOperator = value;
                    this.OnPropertyChanged("LogicalOperator");
                }
            }
        }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public virtual string Expression
        {
            get
            {
                if (this.Count == 0)
                {
                    return string.Empty;
                }

                List<string> filterExpressions = new List<string>();

                for (int i = 0; i < this.Count; i++)
                {
                    string expression = this[i].Expression;

                    if (string.IsNullOrEmpty(expression))
                    {
                        continue;
                    }

                    filterExpressions.Add(expression);
                }

                string logicalOperator = (this.LogicalOperator == FilterLogicalOperator.And) ? " AND " : " OR ";
                string resultExpression = String.Join(logicalOperator, filterExpressions.ToArray());
                return resultExpression;

            }
            set
            {
                this.Parse(value);
                this.OnPropertyChanged("Expression");
            }
        }


        //public bool IsValid
        //{
        //    get
        //    {
        //        for (int i = 0; i < this.Count; i++)
        //        {
        //            CompositeFilterDescriptor composite = this[i] as CompositeFilterDescriptor;
        //            if (composite != null)
        //            {
        //                if (!composite.FilterDescriptors.IsValid)
        //                {
        //                    return false;
        //                }
        //            }

        //            if (string.IsNullOrEmpty(this[i].PropertyName) ||
        //                ((this[i].Operator != FilterOperator.IsNotNull && this[i].Operator != FilterOperator.IsNull) && this[i].Value == null))
        //            {
        //                return false;
        //            }
        //        }

        //        return true;
        //    }
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="filterOperator">The filter operator.</param>
        /// <param name="value">The value.</param>
        public void Add(string propertyName, FilterOperator filterOperator, object value)
        {
            FilterDescriptor filterDescriptor = new FilterDescriptor(propertyName, filterOperator, value);
            this.Add(filterDescriptor);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public int IndexOf(string propertyName)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (string.IsNullOrEmpty(this[i].PropertyName))
                {
                    continue;
                }

                if (this[i].PropertyName.Equals(propertyName, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines whether [contains] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string propertyName)
        {
            return (this.IndexOf(propertyName) >= 0);
        }

        /// <summary>
        /// Removes the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public bool Remove(string propertyName)
        {
            return this.Remove(propertyName, null);
        }

        /// <summary>
        /// Removes the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="predicate">The predicate which determine weather the filter can be deleted.</param>
        /// <returns></returns>
        public bool Remove(string propertyName, Predicate<FilterDescriptor> predicate)
        {
            bool result = false;

            int index = 0;
            this.BeginUpdate();
            while (index < this.Count)
            {
                CompositeFilterDescriptor compositeFilter = this[index] as CompositeFilterDescriptor;
                if (compositeFilter != null)
                {
                    if (compositeFilter.FilterDescriptors.Remove(propertyName, predicate))
                    {
                        result = true;
                    }
                    if (compositeFilter.FilterDescriptors.Count == 0)
                    {
                        this.RemoveAt(index);
                        continue;
                    }
                }

                index++;
            }

            bool notify = result;
            index = this.IndexOf(propertyName);
            bool performRemove = index >= 0;

            if (predicate != null && performRemove)
            {
                performRemove = predicate(this[index]);
            }

            if (performRemove)
            {
                this.RemoveAt(index);
                result = true;
            }

            this.EndUpdate(notify || result);

            return result;
        }

        #endregion

        #region Implementation

        //private void Parse(string expression)
        //{
        //    ExpressionNode root = ExpressionParser.Parse(expression, false);
        //    //validate name nodes

        //    this.BeginUpdate();
        //    this.Clear();

        //    Stack<ExpressionNode> nodeStack = new Stack<ExpressionNode>();
        //    nodeStack.Push(root);

        //    Stack<FilterExpressionCollection> collectionStack = new Stack<FilterExpressionCollection>();
        //    collectionStack.Push(this);

        //    while (nodeStack.Count > 0)
        //    {
        //        ExpressionNode current = nodeStack.Pop();
        //        FilterExpressionCollection expressions = collectionStack.Pop();

        //        UnaryOpNode unaryNode = current as UnaryOpNode;
        //        if (unaryNode != null)
        //        {
        //            if (unaryNode.Op == Operator.Noop)
        //            {
        //                nodeStack.Push(((UnaryOpNode)current).Right);
        //                collectionStack.Push(expressions);
        //                continue;
        //            }
        //        }

        //        BinaryOpNode binaryNode = current as BinaryOpNode;
        //        if (binaryNode != null)
        //        {
        //            if (this.IsPredicate(binaryNode))
        //            {
        //                FilterExpression filterExpression = new FilterExpression();
        //                expressions.Add(filterExpression);
        //                continue;
        //            }

        //            nodeStack.Push(binaryNode.Right);
        //            FilterExpressionCollection collection = expressions;
        //            if(!this.IsPredicate(binaryNode.Right))
        //            {
        //                CompositeFilterExpression  compositeExpression = new CompositeFilterExpression();
        //                expressions.Add(compositeExpression);
        //                collection = compositeExpression.FilterExpressions;
        //            }
        //            collectionStack.Push(collection);

        //            nodeStack.Push(binaryNode.Left);
        //            collection = expressions;
        //            if (!this.IsPredicate(binaryNode.Left))
        //            {
        //                CompositeFilterExpression compositeExpression = new CompositeFilterExpression();
        //                compositeExpression.BinaryOperator = (binaryNode.Op == Operator.And) ? FilterExpression.BinaryOperation.AND : FilterExpression.BinaryOperation.OR;
        //                expressions.Add(compositeExpression);
        //                collection = compositeExpression.FilterExpressions;
        //            }
        //            collectionStack.Push(collection);
        //        }
        //    }

        //    this.EndUpdate();
        //}

        //private bool IsPredicate(ExpressionNode node)
        //{
        //    while (node is UnaryOpNode)
        //    {
        //        node = ((UnaryOpNode)node).Right;
        //    }

        //    BinaryOpNode binaryNode = node as BinaryOpNode;
        //    if (binaryNode == null)
        //    {
        //        return false;
        //    }

        //    if (binaryNode.Left is NameNode || binaryNode.Right is NameNode)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        private void Parse(string expression)
        {
            ExpressionNode root = ExpressionParser.Parse(expression, false);
            //validate name nodes

            this.BeginUpdate();
            this.Clear();

            Stack<ExpressionNode> nodeStack = new Stack<ExpressionNode>();
            nodeStack.Push(root);

            Stack<FilterDescriptorCollection> collectionStack = new Stack<FilterDescriptorCollection>();
            collectionStack.Push(this);

            while (nodeStack.Count > 0)
            {
                ExpressionNode current = nodeStack.Pop();
                FilterDescriptorCollection expressions = collectionStack.Pop();

                UnaryOpNode unaryNode = current as UnaryOpNode;
                if (unaryNode != null)
                {
                    if (unaryNode.Op == Operator.Noop)
                    {
                        nodeStack.Push(((UnaryOpNode)current).Right);
                        collectionStack.Push(expressions);
                        continue;
                    }
                }

                BinaryOpNode binaryNode = current as BinaryOpNode;
                if (binaryNode != null)
                {
                    if (this.IsPredicate(binaryNode))
                    {
                        FilterDescriptor filterDescriptor = this.CreateFilterDescriptor(binaryNode);

                        expressions.Add(filterDescriptor);
                        continue;
                    }

                    nodeStack.Push(binaryNode.Right);
                    FilterDescriptorCollection collection = expressions;
                    if (!this.IsPredicate(binaryNode.Right))
                    {
                        CompositeFilterDescriptor compositeExpression = new CompositeFilterDescriptor();
                        expressions.Add(compositeExpression);
                        collection = compositeExpression.FilterDescriptors;
                    }
                    collectionStack.Push(collection);

                    nodeStack.Push(binaryNode.Left);
                    collection = expressions;
                    if (!this.IsPredicate(binaryNode.Left))
                    {
                        CompositeFilterDescriptor compositeExpression = new CompositeFilterDescriptor();
                        compositeExpression.LogicalOperator = (binaryNode.Op == Operator.And) ? FilterLogicalOperator.And : FilterLogicalOperator.Or;
                        expressions.Add(compositeExpression);
                        collection = compositeExpression.FilterDescriptors;
                    }
                    collectionStack.Push(collection);
                }
            }

            this.EndUpdate();
        }

        private FilterDescriptor CreateFilterDescriptor(BinaryOpNode binaryNode)
        {
            NameNode node = binaryNode.Left as NameNode;
            ConstNode val = binaryNode.Right as ConstNode;
            if (node == null)
            {
                node = binaryNode.Right as NameNode;
                val = binaryNode.Left as ConstNode;
            }

            if (node == null)
            {
                throw new ArgumentException("Invalid BinaryOpNode parameter");
            }

            FilterDescriptor filterDescriptor = new FilterDescriptor();
            filterDescriptor.PropertyName = ((NameNode)node).Name;
            filterDescriptor.Value = val.Value;
            filterDescriptor.Operator = this.GetOperator(binaryNode.Op);
            return filterDescriptor;
        }

        private FilterOperator GetOperator(Operator p)
        {
            if (p == Operator.Like)
            {
                return FilterOperator.IsLike;
            }
            else if (p == Operator.LessOrEqual)
            {
                return FilterOperator.IsLessThanOrEqualTo;
            }
            else if (p == Operator.LessThen)
            {
                return FilterOperator.IsLessThan;
            }
            else if (p == Operator.GreaterOrEqual)
            {
                return FilterOperator.IsGreaterThanOrEqualTo;
            }
            else if (p == Operator.GreaterThen)
            {
                return FilterOperator.IsGreaterThan;
            }
            else if (p == Operator.NotEqual)
            {
                return FilterOperator.IsNotEqualTo;
            }
            else if (p == Operator.EqualTo)
            {
                return FilterOperator.IsEqualTo;
            }
            else if (p == Operator.In)
            {
                return FilterOperator.IsContainedIn;
            }
            else if (p == Operator.Null)
            {
                return FilterOperator.IsNull;
            }

            return FilterOperator.Contains;
        }

        private bool IsPredicate(ExpressionNode node)
        {
            while (node is UnaryOpNode)
            {
                node = ((UnaryOpNode)node).Right;
            }

            BinaryOpNode binaryNode = node as BinaryOpNode;
            if (binaryNode == null)
            {
                return false;
            }

            if (binaryNode.Left is NameNode || binaryNode.Right is NameNode)
            {
                return true;
            }

            return false;
        }

        #endregion


    }
}
