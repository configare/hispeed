using System;
using Telerik.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class PropertyGridItemCollection : ReadOnlyCollection<PropertyGridItem>
    {
        public PropertyGridItemCollection(IList<PropertyGridItem> items)
            : base(items)
        {
        }

        public PropertyGridItem this[string propertyName]
        {
            get
            {
                foreach (PropertyGridItem item in this)
                {
                    if (item.Name == propertyName)
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        internal void AddProperty(PropertyGridItem item)
        {
            this.Items.Add(item);
        }
    }
}
