using System.Collections.Generic;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class PropertyGridGroupCollection : GroupCollection<PropertyGridItem>, IReadOnlyCollection<PropertyGridItemBase>
    {
        public static new PropertyGridGroupCollection Empty = new PropertyGridGroupCollection(new List<Group<PropertyGridItem>>());

        public PropertyGridGroupCollection(IList<Group<PropertyGridItem>> list)
            :base(list)
        {
             
        }

        public new PropertyGridGroup this[int index] 
        {
            get { return (PropertyGridGroup)base[index]; }
        }

        #region IReadOnlyCollection<PropertyGridItemBase> Members

        PropertyGridItemBase IReadOnlyCollection<PropertyGridItemBase>.this[int index]
        {
            get
            {
                return this[index].GroupItem; 
            }
        }

        bool IReadOnlyCollection<PropertyGridItemBase>.Contains(PropertyGridItemBase value)
        {
            PropertyGridGroupItem groupItem = value as PropertyGridGroupItem;
            if (groupItem == null)
            {
                return false;
            }

            return this.IndexOf(groupItem.Group) >= 0;
        }

        void IReadOnlyCollection<PropertyGridItemBase>.CopyTo(PropertyGridItemBase[] array, int index)
        {
            for (int i = index; i < this.Count; i++)
            {
                array[i] = this[i].GroupItem;
            }
        }

        int IReadOnlyCollection<PropertyGridItemBase>.IndexOf(PropertyGridItemBase value)
        {
            PropertyGridGroupItem groupItem = value as PropertyGridGroupItem;
            if (groupItem == null)
            {
                return -1;
            }

            return this.IndexOf(groupItem.Group);
        }

        IEnumerator<PropertyGridItemBase> IEnumerable<PropertyGridItemBase>.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i].GroupItem;
            }
        }

        #endregion

    }
}
