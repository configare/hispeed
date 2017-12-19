using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public class UICommandGroup:UIItem
    {
        private UICommand[] _uiCommands;
        protected bool _allowCollapsed = true;

        public UICommandGroup()
        { }

        public UICommandGroup(string name, string text, UICommand[] uiCommands)
            :base(name,text)
        {
            _uiCommands = uiCommands;
        }

        public UICommand[] UICommands
        {
            get { return _uiCommands; }
            set { _uiCommands = value; }
        }

        public bool AllowCollapsed
        {
            get { return _allowCollapsed; }
            set { _allowCollapsed = value; }
        }
    }
}
