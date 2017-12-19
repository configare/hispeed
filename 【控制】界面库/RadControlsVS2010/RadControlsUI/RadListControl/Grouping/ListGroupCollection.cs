using System.Collections.Generic;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    internal class ListGroupCollection : GroupCollection<RadListDataItem>
    { 
        private ListGroupFactory factory;

        public ListGroupCollection(ListGroupFactory factory)
        : base(new List<Group<RadListDataItem>>())
        {
            this.factory = factory;
        }

        public ListGroup AddGroup(string name)
        {
            ListGroup newGroup = factory.CreateGroup(name);
            this.Items.Add(newGroup);
            return newGroup;
        }

        public void RemoveGroup(ListGroup group)
        {
            if (group.Items.Count > 0)
            {
                group.ClearItems();
            }

            this.Items.Remove(group);
        }

        public new ListGroup this[int index]
        {
            get
            {
                return base[index] as ListGroup;
            }
        }
    }
}