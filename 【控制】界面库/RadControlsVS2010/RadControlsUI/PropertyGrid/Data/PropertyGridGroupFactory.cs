using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class PropertyGridGroupFactory : IGroupFactory<PropertyGridItem>
    {
        private PropertyGridTableElement propertyGridElement;

        public PropertyGridGroupFactory(PropertyGridTableElement propertyGridElement)
        {
            this.propertyGridElement = propertyGridElement;
        }

        #region IGroupFactory<PropertyGridGroupDataItem> Members

        public Group<PropertyGridItem> CreateGroup(object key, Group<PropertyGridItem> parent, params object[] metaData)
        {
            return new PropertyGridGroup(key, parent, this.propertyGridElement);
        }

        public GroupCollection<PropertyGridItem> CreateCollection(IList<Group<PropertyGridItem>> list)
        {
            return new PropertyGridGroupCollection(list);
        }

        #endregion
    }
}
