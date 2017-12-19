using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public class UITheme
    {
        private string _name;
        private string _assembly;
        private string _class;

        public UITheme(string name, string assembly, string classname)
        {
            _name = name;
            _assembly = assembly;
            _class = classname;
        }

        public string Name 
        { 
            get { return _name; } 
        }

        public string Assembly
        {
            get { return _assembly; }
        }

        public string Class
        {
            get { return _class; }
        }
    }
}
