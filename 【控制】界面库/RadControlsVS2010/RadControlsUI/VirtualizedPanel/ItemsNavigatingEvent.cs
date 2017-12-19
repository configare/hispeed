using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void ItemsNavigatingEventHandler<T>(object sender, ItemsNavigatingEventArgs<T> e);

    public class ItemsNavigatingEventArgs<T> : EventArgs
    {
        private T item;
        private bool skipItem;

        public ItemsNavigatingEventArgs(T navigatingItem)
        {
            this.item = navigatingItem;
            this.skipItem = false;
        }

        public T Item
        {
            get
            {
                return this.item;
            }
        }

        public bool SkipItem
        {
            get
            {
                return this.skipItem;
            }
            set
            {
                this.skipItem = value;
            }
        }
    }
}
