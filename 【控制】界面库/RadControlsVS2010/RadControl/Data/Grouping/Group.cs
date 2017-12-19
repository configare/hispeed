using System.Collections;
using System.Collections.Generic;
using Telerik.Data.Expressions;
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
    public delegate object GroupPredicate<T>(T item, int level);

    public class Group<T> : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
        where T : IDataItem
    {
        #region Fields

        private object key;
        private string header;
        private bool isNullHeader;
        private Group<T> parent;

        #endregion

        #region Constructors

        public Group(object key)
            : this(key, null)
        {

        }

        public Group(object key, Group<T> parent)
        {
            this.key = key;
            this.parent = parent;
            this.isNullHeader = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the zero-based depth of the Group
        /// </summary>
        public int Level
        {
            get
            {
                int depth = 0;
                Group<T> parentGroup = this.parent;
                while (parentGroup != null)
                {
                    depth++;
                    parentGroup = parentGroup.parent;
                }

                return depth;
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public virtual string Header
        {
            get
            {
                if (isNullHeader && string.IsNullOrEmpty(this.header))
                {
                    this.header = this.DefaultHeader;
                }

                return header;
            }
            set
            {
                header = value;
                isNullHeader = false;
            }
        }

        /// <summary>
        /// Gets the key of the group.
        /// </summary>
        /// <value>The key.</value>
        public object Key
        {
            get { return this.key; }
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>The item count.</value>
        public virtual int ItemCount
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <value></value>
        public virtual T this[int index]
        {
            get
            {
                return this.Items[index];
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public virtual Group<T> Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual GroupCollection<T> Groups
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Contains(T item)
        {
            return this.Items.Contains(item);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public virtual int IndexOf(T item)
        {
            return this.Items.IndexOf(item);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public object Evaluate(string expression)
        {
            ExpressionNode node = ExpressionParser.Parse(expression, false);
            if (this.Items.Count == 0 || ExpressionNode.GetNodes<AggregateNode>(node).Count == 0)
            {
                return null;
            }

            List<NameNode> nameNodes = ExpressionNode.GetNodes<NameNode>(node);
            StringCollection contextProperties = new StringCollection();
            foreach (NameNode nameNode in nameNodes)
            {
                if (!contextProperties.Contains(nameNode.Name))
                {
                    contextProperties.Add(nameNode.Name);
                }
            }

            ExpressionContext context = ExpressionContext.Context;
            context.Clear();
            for (int i = 0; i < contextProperties.Count; i++)
            {
                if (context.ContainsKey(contextProperties[i]))
                {
                    context[contextProperties[i]] = this.Items[0][contextProperties[i]];
                }
                else
                {
                    context.Add(contextProperties[i], this.Items[0][contextProperties[i]]);
                }
            }

            return node.Eval(new AggregateItems<T>(this.Items), context);
        }

        #endregion

        #region  Implemetation

        protected internal virtual IList<T> Items
        {
            get
            {
                return null;
            }
        }

        private string DefaultHeader
        {
            get
            {
                object[] keyArray = key as object[];

                if (keyArray != null)
                {
                    StringBuilder headerText = new StringBuilder(255);

                    for (int i = 0; i < keyArray.Length; i++)
                    {
                        if((keyArray[i] != null))
                        {
                            headerText.Append(keyArray[i].ToString() + ',');
                        }
                    }

                    if (headerText.Length > 1)
                    {
                        return headerText.ToString(0, headerText.Length - 1);
                    }
                }

                return string.Empty;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IReadOnlyCollection<T> Members

        public virtual void CopyTo(T[] array, int index)
        {
            this.Items.CopyTo(array, index);
        }

        int IReadOnlyCollection<T>.Count
        {
            get { return this.ItemCount; }
        }

        #endregion
    }
}
