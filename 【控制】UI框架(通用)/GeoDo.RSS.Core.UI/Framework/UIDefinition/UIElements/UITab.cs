using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public class UITab : UIItem
    {
        private string _font;
        private UIItem[] _children;        

        public UITab()
        { }

        public UITab(string name, string text, string font)
        {
            _name = name;
            _text = text;
            _font = font;
        }

        public string Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public UIItem[] Children
        {
            get { return _children; }
            set { _children = value; }
        }     
    }
}
