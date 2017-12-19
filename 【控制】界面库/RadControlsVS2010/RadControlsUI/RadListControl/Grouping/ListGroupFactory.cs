using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{ 
    internal class ListGroupFactory : IGroupFactory<RadListDataItem>
    {
        private ListGroupCollection groups;
        private long groupCounter = 1;
        protected internal RadListElement owner;

        public ListGroup DefaultGroup;

        public ListGroupFactory(RadListElement owner)
        {
            this.owner = owner;
            groups = new ListGroupCollection(this);
            this.DefaultGroup = new ListGroup(0L, owner);
            this.DefaultGroup.Header = "Ungrouped";
            groups.GroupList.Add(this.DefaultGroup);
        }

        public ListGroupCollection Groups
        {
            get
            {
                return this.groups;
            } 
        }

        public ListGroup CreateGroup(string header)
        {
            ListGroup group = new ListGroup(groupCounter++, owner);
            group.Header = header;
            return group;
        }
          
        #region IGroupFactory members

        public Group<RadListDataItem> CreateGroup(object key, Group<RadListDataItem> parent, params object[] metaData)
        {
            foreach (ListGroup group in groups)
            {
                if (group.Key as long? == key as long?)
                {
                    group.Items.Clear();
                    return group;
                }
            }

            ListGroup newGroup =  new ListGroup(key, owner);
            newGroup.Header = key.ToString();
            newGroup.Items.Clear();
            this.groups.GroupList.Add(newGroup);

            return newGroup;
        }

        public GroupCollection<RadListDataItem> CreateCollection(IList<Group<RadListDataItem>> list)
        {
            return new GroupCollection<RadListDataItem>(list);
        }

        #endregion
    }
}
