using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class BackstageItemEventArgs : EventArgs
    {
        private BackstageVisualElement item;

        public BackstageItemEventArgs(BackstageVisualElement item)
        {
            this.item = item;
        }

        public BackstageVisualElement Item
        {
            get
            {
                return item;
            }
        }
    }

    public class BackstageItemChangeEventArgs : EventArgs
    {
        private BackstageTabItem newItem, oldItem;

        public BackstageItemChangeEventArgs(BackstageTabItem newItem, BackstageTabItem oldItem)
        {
            this.newItem = newItem;
            this.oldItem = oldItem;
        }

        public BackstageTabItem NewItem
        {
            get
            {
                return newItem;
            }
        }

        public BackstageTabItem OldItem
        {
            get
            {
                return oldItem;
            }
        }
    }

    public class BackstageItemChangingEventArgs : BackstageItemChangeEventArgs
    {
        private bool cancel;

        public BackstageItemChangingEventArgs(BackstageTabItem newItem, BackstageTabItem oldItem)
            : base(newItem, oldItem)
        {

        }

        public bool Cancel
        {
            get
            {
                return cancel;
            }
            set
            {
                cancel = value;
            }
        }
    }
}
