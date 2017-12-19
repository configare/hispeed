using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void CreatePropertyGridItemEventHandler(object sender, CreatePropertyGridItemEventArgs e);

    public class CreatePropertyGridItemEventArgs : EventArgs
    {
        PropertyGridItemBase item;
        Type itemType;

        public CreatePropertyGridItemEventArgs(Type itemType)
        {
            this.itemType = itemType;
        }

        public PropertyGridItemBase Item
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }

        public Type ItemType
        {
            get
            {
                return itemType;
            }
        }
    }
}
