using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;
using System.Collections;

namespace Telerik.WinControls.UI
{
    internal class ListGroupedItemsTraverser : ItemsTraverser<RadListDataItem>
    {
        protected GroupCollection<RadListDataItem> groupsCollection;
        protected Group<RadListDataItem> currentGroup;

        static IList<RadListDataItem> GetItemList(GroupCollection<RadListDataItem> groupsCollection,  Dictionary<Group<RadListDataItem>, RadListDataGroupItem> groupHeaderItems)
        {
            List<RadListDataItem> items = new List<RadListDataItem>();
            foreach (Group<RadListDataItem> group in groupsCollection)
            {
                if (group.ItemCount == 0)
                {
                    continue;
                }

                items.Add(groupHeaderItems[group]);
                items.AddRange(group);
            }

            return items;
        }

        public ListGroupedItemsTraverser(GroupCollection<RadListDataItem> groupsCollection, Dictionary<Group<RadListDataItem>, RadListDataGroupItem> groupHeaderItems)
            : base(ListGroupedItemsTraverser.GetItemList(groupsCollection, groupHeaderItems))
        {
            this.groupsCollection = groupsCollection;
        }
        
        protected override bool OnItemsNavigating(RadListDataItem current)
        {
            base.OnItemsNavigating(current);
            RadListDataGroupItem groupItem = current as RadListDataGroupItem;
            if (groupItem!=null)
            {
                if (groupItem.Group.Key.Equals(0L))
                {
                    return true;
                }
                return false;
            }

            if (current.Group.Collapsed)
            {
                return true;
            }

            return false;
        }
    }
}
