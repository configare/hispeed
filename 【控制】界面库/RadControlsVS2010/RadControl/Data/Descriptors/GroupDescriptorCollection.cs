using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Telerik.Collections.Generic;
using Telerik.Data.Expressions;

namespace Telerik.WinControls.Data
{
    public class GroupDescriptorCollection : NotifyCollection<GroupDescriptor>
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
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < this.Count; i++)
                {
                    for (int j = 0; j < this[i].GroupNames.Count; j++)
                    {
                        SortDescriptor sortDescription = this[i].GroupNames[j];
                        string direction = "ASC";
                        if (sortDescription.Direction == ListSortDirection.Descending)
                        {
                            direction = "DESC";
                        }

                        string delimiter = "";
                        if (this[i].GroupNames.Count > 1 && j < this[i].GroupNames.Count - 1)
                        {
                            delimiter = ",";
                        }

                        stringBuilder.Append(string.Format("{0} {1}{2}", sortDescription.PropertyName, direction, delimiter));
                    }

                    if (this.Count > 1 && i < this.Count - 1)
                    {
                        stringBuilder.Append(" > ");
                    }
                }

                return stringBuilder.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.Clear();
                    return;
                }

                this.ParseGroupString(value);
                OnPropertyChanged(new PropertyChangedEventArgs("Expression"));
            }
        }

        #endregion

        #region Methods


        //TODO uncomment this methods when remove old group by expressions (RGV)
        //public virtual void Add(string expression)
        //{
        //    GroupDescriptor groupDescriptor = new GroupDescriptor();
        //    groupDescriptor.Expression = expression;
        //    this.Add(groupDescriptor);
        //}

        //public virtual void Add(string expression, string format)
        //{
        //    this.Add(new GroupDescriptor(expression, format));
        //}

        /// <summary>
        /// Adds the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="direction">The direction.</param>
        public void Add(string propertyName, ListSortDirection direction)
        {
            this.Add(new GroupDescriptor(new SortDescriptor(propertyName, direction)));
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
                int groupIndex = 0;
                while (groupIndex < this[index].GroupNames.Count)
                {
                    if (this[index].GroupNames[groupIndex].PropertyName.Equals(propertyName, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        this[index].GroupNames.RemoveAt(groupIndex);
                        result = true;
                        continue;
                    }

                    groupIndex++;
                }

                if (this[index].GroupNames.Count == 0)
                {
                    this.RemoveAt(index);
                    result = true;
                    continue;
                }

                index++;
            }

            return result;
        }
        /// <summary>
        /// Determines whether [contains] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="caseSensitive"></param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string propertyName, bool caseSensitive)
        {
            CompareOptions compareFlags = this.GetCompareOpations(caseSensitive);

            CultureInfo culture = CultureInfo.CurrentCulture;

            if (culture.IsNeutralCulture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            CompareInfo compareInfo = culture.CompareInfo;

            for (int i = 0; i < this.Count; i++)
            {
                for (int j = 0; j < this[i].GroupNames.Count; j++)
                {
                    if (string.IsNullOrEmpty(this[i].GroupNames[j].PropertyName))
                    {
                        continue;
                    }

                    if (compareInfo.Compare(this[i].GroupNames[j].PropertyName, propertyName, compareFlags) == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected CompareOptions GetCompareOpations(bool caseSensitive)
        {
            CompareOptions compareFlags = CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreCase;

            if (caseSensitive)
            {
                compareFlags = CompareOptions.None;
            }

            return compareFlags;
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
            return this.Contains(propertyName, false);
        }


        /// <summary>
        /// Finds all sort descriptors associated with the group descriptors by property name
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>All sort descriptors contained in the group descriptors by the specified propertyName</returns>
        public ReadOnlyCollection<SortDescriptor> FindAllAssociatedSortDescriptors(string propertyName)
        {
            return this.FindAllAssociatedSortDescriptors(propertyName, false);
        }


        /// <summary>
        /// Finds all sort descriptors associated with the group descriptors by property name
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns>All sort descriptors contained in the group descriptors by the specified propertyName</returns>
        public ReadOnlyCollection<SortDescriptor> FindAllAssociatedSortDescriptors(string propertyName, bool caseSensitive)
        {
            List<SortDescriptor> result = new List<SortDescriptor>();

            CompareOptions compareFlags = this.GetCompareOpations(caseSensitive);

            CultureInfo culture = CultureInfo.CurrentCulture;

            if (culture.IsNeutralCulture)
            {
                culture = CultureInfo.InvariantCulture;
            }

            CompareInfo compareInfo = culture.CompareInfo;

            for (int i = 0; i < this.Count; i++)
            {
                for (int j = 0; j < this[i].GroupNames.Count; j++)
                {
                    if (string.IsNullOrEmpty(this[i].GroupNames[j].PropertyName))
                    {
                        continue;
                    }

                    if (compareInfo.Compare(this[i].GroupNames[j].PropertyName, propertyName, compareFlags) == 0)
                    {
                        result.Add(this[i].GroupNames[j]);
                    }
                }
            }

            return result.AsReadOnly();
        }

        #endregion

        #region Implementation

        //TODO: may add special expression property
        protected override void InsertItem(int index, GroupDescriptor item)
        {
            base.InsertItem(index, item);
            item.Owner = this;
            item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
        }

        protected override void SetItem(int index, GroupDescriptor item)
        {
            GroupDescriptor old = this[index];
            old.Owner = null;
            old.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);

            base.SetItem(index, item);
            item.Owner = this;
            item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
        }

        protected override void RemoveItem(int index)
        {
            GroupDescriptor old = this[index];
            base.RemoveItem(index);

            old.Owner = null;
            old.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
        }

        protected override void ClearItems()
        {
            foreach (GroupDescriptor groupDescription in this)
            {
                groupDescription.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
                groupDescription.Owner = null;
            }

            base.ClearItems();
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int index = this.IndexOf(sender as GroupDescriptor);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanged, sender, index);
            OnCollectionChanged(args);
        }

        private void ParseGroupString(string groupExpression)
        {
            this.BeginUpdate();
            this.Clear();

            this.AddRange(DataStorageHelper.ParseGroupString(groupExpression).ToArray());

            this.EndUpdate();
        }

        #endregion
    }
}
