using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class ItemDataBoundEventArgs : EventArgs
    {
        private RadItem dataBoundItem;
        private object dataItem;

        /// <summary>
        /// Takes as parameters the <see cref="RadItem"/> that is bound
        /// and the <see cref="object"/> that is being bound to the RadItem.
        /// </summary>
        /// <param name="dataBoundItem">The <see cref="RadItem"/> that is bound.</param>
        /// <param name="dataItem">The object that is being bound to the <see cref="DataBoundItem"/>.</param>
        public ItemDataBoundEventArgs(RadItem dataBoundItem, object dataItem)
        {
            this.dataBoundItem = dataBoundItem;
            this.dataItem = dataItem;
        }

        /// <summary>
        /// Gets the <see cref="RadItem"/> that is bound.
        /// </summary>
        public RadItem DataBoundItem
        {
            get { return dataBoundItem; }
        }

        /// <summary>
        /// Gets the object that is being bound to the <see cref="DataBoundItem"/>.
        /// </summary>
        public object DataItem
        {
            get { return dataItem; }
        }
    }
}
