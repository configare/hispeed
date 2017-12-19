using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.PropertyGridData
{
    public class ArrayItemAccessor: ItemAccessor
    {
        int index;

        public ArrayItemAccessor(PropertyGridItem owner, int index)
            : base(owner)
        {
            this.index = index;
        }

        public override object Value
        {
            get
            {
                return ((Array)this.owner.GetValueOwner()).GetValue(this.index);
            }
            set
            {
                ((Array)this.owner.GetValueOwner()).SetValue(value, this.index);
            }

        }

        public override Type PropertyType
        {
            get
            {
                return ((PropertyGridItem)base.owner.Parent).PropertyType.GetElementType();
            }
        }
    }
}
