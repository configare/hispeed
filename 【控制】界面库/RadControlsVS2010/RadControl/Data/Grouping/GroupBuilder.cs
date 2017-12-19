using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using Telerik.Data.Expressions;

namespace Telerik.WinControls.Data
{
    /// <summary>
    /// Used to build groups from indexer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBuilder<T> where T : IDataItem
    {
        #region Fields 
       
        private int version;
        private int countState;
        private SortDescriptor[] groupNames = new SortDescriptor[0];

        private Index<T> index;
        private GroupCollection<T> groups;
        private static List<Group<T>> Empty = new List<Group<T>>();
        private GroupPredicate<T> groupPredicate = null;
        private IComparer<Group<T>> comparer;

        #endregion

        #region Constructors

        public GroupBuilder(Index<T> index)
        {
            this.index = index;
            this.groupPredicate = this.GetItemKey;
            this.comparer = new GroupComparer<T>(null);
            this.groups = GroupCollection<T>.Empty;

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public GroupCollection<T> Groups
        {
            get
            {
                if (this.NeedsRefresh)
                {
                    this.groups = this.Perform(this.index, 0, null);
                    this.countState = this.index.Count;
                    if (this.index.CollectionView.GroupDescriptors.Count > 0)
                    {
                        this.groupNames = new SortDescriptor[this.index.CollectionView.GroupDescriptors[0].GroupNames.Count];
                        for (int i = 0; i < this.groupNames.Length; i++)
                        {
                            SortDescriptor current = this.index.CollectionView.GroupDescriptors[0].GroupNames[i];
                            groupNames[i] = new SortDescriptor(current.PropertyName, current.Direction);
                        }
                    }
                    else
                    {
                        groupNames = new SortDescriptor[0];
                    }
                }
               
                this.version = this.index.CollectionView.Version;
                return this.groups;
            }
        }

        /// <summary>
        /// Gets or sets the group predicate.
        /// </summary>
        /// <value>The group predicate.</value>
        public virtual GroupPredicate<T> GroupPredicate
        {
            get { return this.groupPredicate;}
            set
            {
                if (this.groupPredicate != value)
                {
                    this.groupPredicate = value;
                }
            }
        }

        /// <summary>
        /// Gets the default group predicate.
        /// </summary>
        /// <value>The group predicate.</value>
        public virtual GroupPredicate<T> DefaultGroupPredicate
        {
            get
            {
                return this.GetItemKey;
            }
        }

        public virtual IComparer<Group<T>> Comparer
        {
            get { return this.comparer; }
            set
            {
                if (this.comparer != value)
                {
                    this.comparer = value;
                    this.version++;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [needs refresh].
        /// </summary>
        /// <value><c>true</c> if [needs refresh]; otherwise, <c>false</c>.</value>
        public bool NeedsRefresh
        {
            get
            {
                if (this.version != this.index.CollectionView.Version)
                {
                    return true;
                }

                if (this.countState != this.index.Count)
                {
                    return true;
                }

                if (this.index.CollectionView.GroupDescriptors.Count == 0)
                {
                    return true;
                }
                    
                if (this.groupNames.Length != this.index.CollectionView.GroupDescriptors[0].GroupNames.Count)
                {
                    return true;
                }

                for (int i = 0; i < groupNames.Length; i++)
                {
                    if (groupNames[i].Direction != this.index.CollectionView.GroupDescriptors[0].GroupNames[i].Direction)
                    {
                        return true;
                    }

                    if (groupNames[i].PropertyName.CompareTo(this.index.CollectionView.GroupDescriptors[0].GroupNames[i].PropertyName) != 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the grouping operation for specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="level">The level.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public virtual GroupCollection<T> Perform(IReadOnlyCollection<T> items, int level, Group<T> parent)
        {
            GroupDescriptorCollection groupDescriptors = this.index.CollectionView.GroupDescriptors;
            if (level >= groupDescriptors.Count)
            {
                return GroupCollection<T>.Empty;
            }
            IGroupFactory<T> groupFactory = this.index.CollectionView.GroupFactory;

            GroupCollection<T> cache = GetCachedGroups(items, level);
            if (!IsValid(level, groupDescriptors))
            {
                if (cache != null) cache.Dispose();
                return this.index.CollectionView.GroupFactory.CreateCollection(GroupBuilder<T>.Empty);
            }

            IComparer<Group<T>> newComparer = Activator.CreateInstance(this.Comparer.GetType()) as IComparer<Group<T>>;
            if (newComparer == null)
            {
                newComparer = this.Comparer;
            }
            if (newComparer is GroupComparer<T>)
            {
                ((GroupComparer<T>)newComparer).Directions = this.GetComparerDirections(level);
            }

            AvlTree<Group<T>> groupList = new AvlTree<Group<T>>(newComparer);
            AvlTree<Group<T>> list = new AvlTree<Group<T>>(new GroupComparer<T>(this.GetComparerDirections(level)));
            foreach (T item in items)
            {
                object key = this.GroupPredicate(item, level);
                Group<T> group, newGroup = new DataItemGroup<T>(key);
                group = list.Find(newGroup); groupList.Find(newGroup);
                if (group == null)
                {
                    group = GroupBuilder<T>.GetCachedGroup(cache, newGroup);
                    if (group == null)
                    {
                        group = groupFactory.CreateGroup(key, parent);
                        DataItemGroup<T> dataGroup = group as DataItemGroup<T>;
                        dataGroup.GroupBuilder = this;
                    }

                    list.Add(group);// groupList.Add(group);
                }

                group.Items.Add(item);
            }

            for (int i = 0; i < list.Count; i++)
            {
                groupList.Add(list[i]);
            }

            if (cache != null)
            {
                cache.Dispose();
            }

            return groupFactory.CreateCollection(groupList);
        }

        private static bool IsValid(int level, GroupDescriptorCollection groupDescriptors)
        {
            bool valid = !(level >= groupDescriptors.Count || groupDescriptors[level].GroupNames.Count == 0);
            if (valid)
            {
                for (int i = 0; i < groupDescriptors[level].GroupNames.Count; i++)
                {
                    if (groupDescriptors[level].GroupNames[i].PropertyIndex < 0)
                    {
                        valid = false;
                        break;
                    }
                }
            }

            return valid;
        }

        #endregion

        #region Internal

        private static Group<T> GetCachedGroup(GroupCollection<T> cache, Group<T> newGroup)
        {
            if (cache != null)
            {
                AvlTree<Group<T>> avl = cache.GroupList as AvlTree<Group<T>>;
                if (avl == null)
                {
                    return null;
                }

                Group<T> group = avl.Find(newGroup);
                if (group != null)
                {
                    group.Items.Clear();
                }

                return group;
            }

            return null;
        }

        private GroupCollection<T> GetCachedGroups(IReadOnlyCollection<T> items, int level)
        {
            GroupCollection<T> cache = null;
            if (level == 0)
            {
                cache = this.groups;
            }

            if (cache == null && items is DataItemGroup<T>)
            {
                cache = ((DataItemGroup<T>)items).Cache;
            }
            return cache;
        }

        private object GetItemKey(T item, int level)
        {
            object[] key = new object[this.index.CollectionView.GroupDescriptors[level].GroupNames.Count];
            for (int k = 0; k < this.index.CollectionView.GroupDescriptors[level].GroupNames.Count; k++)
            {
                int index = this.index.CollectionView.GroupDescriptors[level].GroupNames[k].PropertyIndex;
                if(index < 0)
                {
                    continue;
                }

                key[k] = item[index];
                if (key[k] == DBNull.Value)
                {
                    key[k] = null;
                }
            }

            return key;
        }

        private ListSortDirection[] GetComparerDirections(int level)
        {
            ListSortDirection[] directions = new ListSortDirection[this.index.CollectionView.GroupDescriptors[level].GroupNames.Count];
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = this.index.CollectionView.GroupDescriptors[level].GroupNames[i].Direction;
            }

            return directions;
        }

        protected internal int Version
        {
            get { return this.version; }
        }

        #endregion
    }
}
