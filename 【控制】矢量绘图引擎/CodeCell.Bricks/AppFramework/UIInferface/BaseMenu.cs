using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    [Serializable]
    public class BaseMenu:BaseItem,IMenu
    {
        protected IItem[] _items = null;

        public BaseMenu()
        { 
        }

        #region IToolbar ≥…‘±

        public virtual IItem[] Items
        {
            get
            {
                return _items;
            }
        }

        #endregion
    }
}
