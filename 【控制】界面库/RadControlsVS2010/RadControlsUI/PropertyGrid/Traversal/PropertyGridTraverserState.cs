using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class PropertyGridTraverserState
    {
        private PropertyGridGroupItem group;
        private PropertyGridItemBase item;
        private int index;
        private int groupIndex;

        public PropertyGridTraverserState(PropertyGridGroupItem group, PropertyGridItemBase item, int index, int groupIndex)
        {
            this.group = group;
            this.item = item;
            this.index = index;
            this.groupIndex = groupIndex;
        }

        public PropertyGridGroupItem Group
        {
            get
            {
                return group;
            }
        }

        public PropertyGridItemBase Item
        {
            get
            {
                return item;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public int GroupIndex
        {
            get
            {
                return groupIndex;
            }
        }
    }
}
