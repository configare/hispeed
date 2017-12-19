using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void CreatePropertyGridItemElementEventHandler(object sender, CreatePropertyGridItemElementEventArgs e);

    public class CreatePropertyGridItemElementEventArgs : EventArgs
    {
        PropertyGridItemElementBase itemElement;
        PropertyGridItemBase item;

        public CreatePropertyGridItemElementEventArgs(PropertyGridItemBase item)
        {
            this.item = item;
        }

        public PropertyGridItemElementBase ItemElement
        {
            get
            {
                return this.itemElement;
            }
            set
            {
                this.itemElement = value;
            }
        }

        public PropertyGridItemBase Item
        {
            get
            {
                return this.item;
            }        
        }
    }

}
