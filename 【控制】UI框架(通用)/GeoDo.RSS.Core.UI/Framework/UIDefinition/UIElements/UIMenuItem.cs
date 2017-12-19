using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /*
       <MenuItem text="10%" image="" identify="1005" argument="0.1"/>
     */
    public class UIMenuItem:UICommand
    {
        protected int _identify = 0;
        protected string _argument = null;
        protected string _image = null;

        public UIMenuItem()
        { 
        }

        public UIMenuItem(string name, string text, string image, int identify, string argument)
            : base(name, text)
        {
            _identify = identify;
            _image = image;
            _argument = argument;
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public int Identify
        {
            get { return _identify; }
            set { _identify = value; }
        }

        public string Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }
    }
}
