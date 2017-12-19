using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    //<UIPage name ="" provider=""/>
    public class UIPage
    {
        protected string _name;
        protected ContentOfUIProvider _contentOfUIProvider = null;

        public UIPage()
        {
        }

        public UIPage(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ContentOfUIProvider ContentOfUIProvider
        {
            get { return _contentOfUIProvider; }
        }

        public string Provider
        {
            get { return _contentOfUIProvider != null ? _contentOfUIProvider.Provider : null; }
            set
            {
                _contentOfUIProvider = new ContentOfUIProvider(value);
            }
        }
    }
}
