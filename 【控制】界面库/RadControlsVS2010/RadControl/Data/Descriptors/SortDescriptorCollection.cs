using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Telerik.Collections.Generic;
using Telerik.Data.Expressions;
using System.Collections.Generic;

namespace Telerik.WinControls.Data
{
    public class SortDescriptorCollection : NotifyCollection<SortDescriptor>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public virtual string Expression
        {
            get
            {
                return this.GetExpression();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.Clear();
                    return;
                }

                this.ParseSortString(value);
                OnPropertyChanged(new PropertyChangedEventArgs("Expression"));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="direction">The direction.</param>
        public void Add(string propertyName, ListSortDirection direction)
        {
            this.Add(new SortDescriptor(propertyName, direction));
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
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        return i;
                    }
                    continue;
                }

                if (this[i].PropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.Count < 1)
            {
                return base.ToString();
            }

            return this.GetExpression();
        }

        /// <summary>
        /// Removes the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public bool Remove(string propertyName)
        {
            bool result = false;
            int index = 0;
            while (index < this.Count)
            {
                if (this[index].PropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.RemoveAt(index);
                    result = true;
                    continue;
                }

                index++;
            }

            return result;
        }

        #endregion

        #region Implementation

        protected override void InsertItem(int index, SortDescriptor item)
        {
            int existingIndex = this.IndexOf(item.PropertyName);
            if (existingIndex >= 0 && this[existingIndex].Direction == item.Direction)
            {
                return;
            }

            base.InsertItem(index, item);
            item.Owner = this;

            item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PropertyName" || e.PropertyName == "Direction")
            {
                int index = this.IndexOf(sender as SortDescriptor);
                NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanged, sender, index);
                this.OnCollectionChanged(args);
            }
        }

        protected override void RemoveItem(int index)
        {
            SortDescriptor item = base[index];
            item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            item.Owner = null;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, SortDescriptor item)
        {
            SortDescriptor description = base[index];
            description.Owner = null;
            description.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);

            base.SetItem(index, item);
            item.Owner = this;
            item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
        }

        protected override void ClearItems()
        {
            foreach (SortDescriptor sortDescription in this)
            {
                sortDescription.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
                sortDescription.Owner = null;
            }

            base.ClearItems();
        }

        private void ParseSortString(string sortString)
        {
            this.BeginUpdate();
            this.Clear();

            this.AddRange(DataStorageHelper.ParseSortString(sortString).ToArray());

            this.EndUpdate();
        }

        private string GetExpression()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                string direction = "ASC";
                if (this[i].Direction == ListSortDirection.Descending)
                {
                    direction = "DESC";
                }

                string delimiter = "";
                if (this.Count > 1 && i < this.Count - 1)
                {
                    delimiter = ",";
                }
                stringBuilder.Append(string.Format("{0} {1}{2}", this[i].PropertyName, direction, delimiter));
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}
