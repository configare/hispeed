using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public class UIItem
    {
        protected string _text;
        protected string _name;
        protected bool _visible;
        protected ContentOfUIProvider _contentOfUIProvider = null;
        protected bool _isLoadedUIProvider = false;
        protected string _uiProvider;

        public UIItem()
        { 
        }

        public UIItem(string name, string text)
        {
            _name = name;
            _text = text;
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ContentOfUIProvider ContentOfUIProvider
        {
            get 
            {
                //延迟加载
                if (!_isLoadedUIProvider && _uiProvider != null)
                {
                    _contentOfUIProvider = new ContentOfUIProvider(_uiProvider);
                    _isLoadedUIProvider = true;
                }
                return _contentOfUIProvider;
            }
        }

        public string Provider
        {
            get 
            {
                return _uiProvider;
            }
            set
            {
                _uiProvider = value;
            }
        }
    }
}
