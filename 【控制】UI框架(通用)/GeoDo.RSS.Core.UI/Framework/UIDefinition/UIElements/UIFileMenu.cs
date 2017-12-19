using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public class UIFileMenuView
    {
        private UIMenuItem[] _items;

        public UIFileMenuView()
        { }

        public UIMenuItem[] MenuItems
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}
