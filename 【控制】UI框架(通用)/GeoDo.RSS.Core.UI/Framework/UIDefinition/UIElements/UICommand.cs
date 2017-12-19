using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public class UICommand : UIItem
    {
        public UICommand()
            :base()
        {
        }

        public UICommand(string name, string text)
        {
            _name = name;
            _text = text;
        }
    }
}
