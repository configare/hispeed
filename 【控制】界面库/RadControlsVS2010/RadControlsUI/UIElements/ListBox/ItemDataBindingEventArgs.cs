using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class ItemDataBindingEventArgs : EventArgs
    {
        private RadItem dataBindingItem = null;
        private object dataItem = null;
        private RadItem newBindingItem = null;

        /// <summary>
        /// Takes as parameters the <see cref="RadItem"/> that is binding
        /// and the <see cref="object"/> that is being bound to the RadItem.
        /// </summary>
        /// <param name="dataBindingItem">The <see cref="RadItem"/> that is binding.</param>
        /// <param name="dataItem">The object that is being bound to the <see cref="ItemDataBoundEventArgs.DataBoundItem"/>.</param>
        public ItemDataBindingEventArgs(RadItem dataBindingItem, object dataItem)
        {
            this.dataBindingItem = dataBindingItem;
            this.dataItem = dataItem;
        }
        
        /// <summary>
        /// Gets the <see cref="RadItem"/> that is bound.
        /// </summary>
        public RadItem DataBindingItem
        {
            get
            {
                return dataBindingItem; 
            }
        }

        /// <summary>
        /// Gets the <see cref="RadItem"/> that was swapped with a new RadItem.
        /// </summary>
        public RadItem NewBindingItem
        {
            get
            {
                return newBindingItem;
            }

            set
            {
                this.newBindingItem = value;
            }
        }

        /// <summary>
        /// Gets the object that is being bound to the <see cref="ItemDataBoundEventArgs.DataBoundItem"/>.
        /// </summary>
        public object DataItem
        {
            get 
            { 
                return dataItem;
            }
        }
    }
}
