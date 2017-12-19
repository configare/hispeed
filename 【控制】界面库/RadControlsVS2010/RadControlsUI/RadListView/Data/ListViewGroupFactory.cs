using Telerik.WinControls.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;

namespace Telerik.WinControls.UI
{
    public class ListViewGroupFactory : IGroupFactory<ListViewDataItem>
    {
        private RadListViewElement owner;

        public ListViewGroupFactory(RadListViewElement owner)
        {
            this.owner = owner;
        }

        public Group<ListViewDataItem> CreateGroup(object key, Group<ListViewDataItem> parent, params object[] metaData)
        {
            return new DataItemGroup<ListViewDataItem>(key, parent);
        }

        public GroupCollection<ListViewDataItem> CreateCollection(IList<Group<ListViewDataItem>> list)
        {
            InitializeGroups(list);
            return new GroupCollection<ListViewDataItem>(list);
        }

        private void InitializeGroups(IList<Group<ListViewDataItem>> list)
        {
            HybridDictionary oldGroups = new HybridDictionary(), newGroups = new HybridDictionary();
            
            foreach (ListViewDataItemGroup group in this.owner.Groups.AutoGroups)
            {
                oldGroups.Add(group.DataGroup, group);
            }

            int index = 0;
            foreach (Group<ListViewDataItem> group in list)
            {
                newGroups.Add(group, index++);
            }

            foreach (ListViewDataItemGroup group in oldGroups.Values)
            {
                if (!newGroups.Contains(group.DataGroup))
                {
                    this.owner.Groups.AutoGroups.Remove(group);
                }
            }

            foreach (Group<ListViewDataItem> group in newGroups.Keys)
            {
                if (!oldGroups.Contains(group))
                {
                    ListViewDataItemGroup newGroup = new ListViewDataItemGroup();
                    newGroup.DataGroup = group;
                    this.owner.Groups.AutoGroups.Add(newGroup);
                }
            }

            int groupsCount = this.owner.Groups.AutoGroups.Count;

            ListViewDataItemGroup[] groups = new ListViewDataItemGroup[groupsCount];
            this.owner.Groups.AutoGroups.CopyTo(groups, 0);

            Array.Sort(groups, new GroupsComparer(newGroups));

            this.owner.Groups.AutoGroups.BeginUpdate();
            for (int i = 0; i < groupsCount; i++)
            {
                this.owner.Groups.AutoGroups[i] = groups[i];
            }
            this.owner.Groups.AutoGroups.EndUpdate();

            //Fixes a bug with items not knowing their group
            //TODO: Should investigate
            foreach (ListViewDataItemGroup group in this.owner.Groups)
            {
                foreach (ListViewDataItem item in group.Items)
                {
                    item.SetGroupCore(group, false);
                }
            } 
        }

        private class GroupsComparer : IComparer<ListViewDataItemGroup>
        {
            HybridDictionary keys;

            public GroupsComparer(HybridDictionary keys)
            {
                this.keys = keys;
            }

            public int Compare(ListViewDataItemGroup x, ListViewDataItemGroup y)
            {
                return ((int)keys[x.DataGroup]).CompareTo((int)keys[y.DataGroup]);
            }
        }
    }


}
